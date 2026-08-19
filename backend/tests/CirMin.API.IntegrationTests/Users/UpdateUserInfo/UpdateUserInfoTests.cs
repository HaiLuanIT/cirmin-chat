using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using CirMin.API.IntegrationTests.Infrastructure;
using CirMin.Contracts.Errors;
using CirMin.Contracts.Models.Users.UpdateUserInfo;
using CirMin.DataAccess.Entities;

namespace CirMin.API.IntegrationTests.Users.UpdateUserInfo;

[Collection(PostgreSqlCollection.Name)]
public class UpdateUserInfoTests : IAsyncLifetime
{
    private const string BaseEndpoint = "/api/users/me/update-information";
    private readonly HttpClient _client;
    private readonly CirMinApiFactory _factory;

    public UpdateUserInfoTests(PostgreSqlFixture database)
    {
        _factory = new CirMinApiFactory(database.ConnectionString);
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        _client.Dispose();
        _factory.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task UpdateUserInfo_WithoutAuthentication_ReturnsUnauthorized()
    {
        //Arrange
        var user = await SeedUserAsync();
        var request = new UpdateUserInfoRequest
        {
            DisplayName = "newDisplayName"
        };
        var beforeUpdate = await GetUserSnapshotAsync(user.Id);
        //act
        var response = await _client.PatchAsJsonAsync(BaseEndpoint, request);

        //assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        //assert: database
        var afterUpdate = await GetUserSnapshotAsync(user.Id);
        Assert.Equal(beforeUpdate, afterUpdate);
    }

    [Fact]
    public async Task UpdateUserInfo_AuthenticatedUserDoesNotExist_ReturnsNotFound()
    {
        var existingUser = await SeedUserAsync();
        var beforeUpdate = await GetUserSnapshotAsync(existingUser.Id);

        _client.DefaultRequestHeaders.Add(TestAuthenticationHandler.UserIdHeaderName, Guid.NewGuid().ToString());
        var request = new UpdateUserInfoRequest
        {
            DisplayName = "newDisplayName"
        };

        //act
        var response = await _client.PatchAsJsonAsync(BaseEndpoint, request);

        //assert
        await AssertProblemAsync(response, HttpStatusCode.NotFound, ErrorCodes.User.NotFound);

        //assert: database
        var afterUpdate = await GetUserSnapshotAsync(existingUser.Id);
        Assert.Equal(beforeUpdate, afterUpdate);
    }

    [Fact]
    public async Task UpdateUserInfo_ValidFullRequest_UpdatesResponseAndDatabase()
    {
        var user = await SeedUserAsync();

        AuthenticateAs(user);

        var request = new UpdateUserInfoRequest
        {
            DisplayName = "New Display Name",
            Bio = "New Bio",
            Email = "new@gmail.com"
        };

        var response = await _client.PatchAsJsonAsync(BaseEndpoint, request);

        //assert: HTTP
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //assert: response body
        var responseBody = await response.Content.ReadFromJsonAsync<UpdateUserInfoResponse>();
        Assert.NotNull(responseBody);
        Assert.Equal(request.DisplayName, responseBody.DisplayName);
        Assert.Equal(request.Bio, responseBody.Bio);
        Assert.Equal(request.Email, responseBody.Email);

        //assert: database
        var persistedUser = await GetUserSnapshotAsync(user.Id);
        Assert.Equal(request.DisplayName, persistedUser.DisplayName);
        Assert.Equal(request.Bio, persistedUser.Bio);
        Assert.Equal(request.Email, persistedUser.Email);
    }

    [Fact]
    public async Task UpdateUserInfo_OnlyDisplayName_UpdatesDisplayNameAndPreservesOtherFields()
    {
        var user = await SeedUserAsync();
        AuthenticateAs(user);

        var request = new UpdateUserInfoRequest
        {
            DisplayName = "New Display Name"
        };

        var response = await _client.PatchAsJsonAsync(BaseEndpoint, request);

        //assert: HTTP
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //assert: response body
        var responseBody = await response.Content.ReadFromJsonAsync<UpdateUserInfoResponse>();
        Assert.NotNull(responseBody);
        Assert.Equal(request.DisplayName, responseBody.DisplayName);
        Assert.Equal(user.Bio, responseBody.Bio);
        Assert.Equal(user.Email, responseBody.Email);

        //assert: database
        var persistedUser = await GetUserSnapshotAsync(user.Id);
        Assert.Equal(request.DisplayName, persistedUser.DisplayName);
        Assert.Equal(user.Bio, persistedUser.Bio);
        Assert.Equal(user.Email, persistedUser.Email);
    }

    public static IEnumerable<object[]> NoOpRequests()
    {
        yield return new object[]
        {
            new UpdateUserInfoRequest
            {
                DisplayName = null,
                Bio = null,
                Email = null
            }
        };

        yield return new object[]
        {
            new UpdateUserInfoRequest
            {
                DisplayName = "",
                Bio = "",
                Email = ""
            }
        };

        yield return new object[]
        {
            new UpdateUserInfoRequest
            {
                DisplayName = "   ",
                Bio = "   ",
                Email = "   "
            }
        };
    }
    [Theory]
    [MemberData(nameof(NoOpRequests))]
    public async Task UpdateUserInfo_NoOpRequest_NoUpdateAndPreservesOtherFields(UpdateUserInfoRequest request)
    {
        var user = await SeedUserAsync();
        AuthenticateAs(user);

        var beforeUpdate = await GetUserSnapshotAsync(user.Id);

        var response = await _client.PatchAsJsonAsync(BaseEndpoint, request);

        //assert: HTTP
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //assert: response body
        var responseBody = await response.Content.ReadFromJsonAsync<UpdateUserInfoResponse>();
        Assert.NotNull(responseBody);
        Assert.Equal(user.FullName, responseBody.DisplayName);
        Assert.Equal(user.Bio, responseBody.Bio);
        Assert.Equal(user.Email, responseBody.Email);

        //assert: database
        var afterUpdate = await GetUserSnapshotAsync(user.Id);
        Assert.Equal(beforeUpdate, afterUpdate);
    }

    [Fact]
    public async Task UpdateUserInfo_EmailBelongsToAnotherUser_ReturnsConflictAndDoesNotUpdateDatabase()
    {
        var user = await SeedUserAsync();
        AuthenticateAs(user);
        var duplicatedUser = await SeedUserAsync();
        var duplicatedEmail = duplicatedUser.Email;

        var beforeUpdate = await GetUserSnapshotAsync(user.Id);
        var request = new UpdateUserInfoRequest
        {
            Email = duplicatedEmail
        };

        var response = await _client.PatchAsJsonAsync(BaseEndpoint, request);

        //assert: HTTp, body
        await AssertProblemAsync(response, HttpStatusCode.Conflict, ErrorCodes.User.EmailAlreadyExists);

        //assert: database
        var afterUpdate = await GetUserSnapshotAsync(user.Id);
        Assert.Equal(beforeUpdate, afterUpdate);
    }

    [Fact]
    public async Task UpdateUserInfo_UserIsDeleted_ReturnsNotFoundAndDoesNotUpdateDatabase()
    {
        var user = await SeedUserAsync();
        AuthenticateAs(user);
        await _factory.ExecuteDbContextAsync(async dbContext =>
        {
            await dbContext.Users.Where(u => u.Id == user.Id)
                .ExecuteUpdateAsync(setters => setters.SetProperty(u => u.IsDeleted, true));
        });
        var beforeUpdate = await GetUserSnapshotAsync(user.Id);

        var request = new UpdateUserInfoRequest
        {
            DisplayName = "New Display Name"
        };

        var response = await _client.PatchAsJsonAsync(BaseEndpoint, request);

        //assert: HTTP, body
        await AssertProblemAsync(response, HttpStatusCode.NotFound, ErrorCodes.User.NotFound);

        //assert: database
        var afterUpdate = await GetUserSnapshotAsync(user.Id);
        Assert.Equal(beforeUpdate, afterUpdate);
    }
    //helper

    private static async Task AssertProblemAsync(HttpResponseMessage response, HttpStatusCode expectedStatus,
        string expectedCode)
    {
        Assert.Equal(expectedStatus, response.StatusCode);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problemDetails);
        var codeElement = Assert.IsType<JsonElement>(
            problemDetails.Extensions["code"]);
        Assert.Equal(expectedCode, codeElement.GetString());
    }

    private async Task<UserSnapshot> GetUserSnapshotAsync(Guid userId)
    {
        return await _factory.ExecuteDbContextAsync(async dbContext =>
            await dbContext.Users.AsNoTracking()
                .Where(x => x.Id == userId)
                .Select(x => new UserSnapshot(
                    x.FullName,
                    x.Bio,
                    x.Email,
                    x.UpdatedAt,
                    x.RowVersion,
                    x.IsDeleted
                )).SingleAsync());
    }

    private void AuthenticateAs(User user)
    {
        _client.DefaultRequestHeaders.Remove(TestAuthenticationHandler.UserIdHeaderName);
        _client.DefaultRequestHeaders.Add(TestAuthenticationHandler.UserIdHeaderName, user.Id.ToString());
    }

    private async Task<User> SeedUserAsync()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = $"user_{Guid.NewGuid():N}",
            HashedPassword = "not-used-in-this-test",
            FullName = "Old Display Name",
            Bio = "Old Bio",
            Email = $"old-{Guid.NewGuid():N}@example.com"
        };
        await _factory.ExecuteDbContextAsync(async dbContext =>
        {
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        });
        return user;
    }

    private sealed record UserSnapshot(
        string DisplayName,
        string? Bio,
        string? Email,
        DateTimeOffset UpdatedAt,
        uint RowVersion,
        bool IsDeleted);
}