using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CirMin.API.IntegrationTests.Infrastructure;
using CirMin.Contracts.Errors;
using CirMin.DataAccess.Commons.Constants;
using CirMin.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace CirMin.API.IntegrationTests.FriendShips.ProcessFriendRequest;

[Collection(PostgreSqlCollection.Name)]
public class ProcessFriendRequestTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CirMinApiFactory _factory;
    private const string BaseEndpoint = "/api/friends/";

    public ProcessFriendRequestTests(PostgreSqlFixture database)
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
    public async Task ProcessFriendRequest_ValidRequest_ApproveFriendRequestSuccess()
    {
        //seed data
        var (user1, user2, friendRequest) = await SeedData();
        AuthenticateAs(user2);
        //act
        var response = await _client.PostAsync(BaseEndpoint + $"requests/{friendRequest.Id}/accept", null);

        //assert
        //assert http, body
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Đã chấp nhận lời mời kết bạn!", json.GetProperty("message").GetString());

        //assert db change
        var friendRequestInDb = await GetFriendshipByIdAsync(friendRequest);
        Assert.NotNull(friendRequestInDb);
        Assert.Equal(FriendShipStatus.Accept, friendRequestInDb.Status);
    }

    [Fact]
    public async Task ProcessFriendRequest_RequesterRejectFriendRequest_ReturnsForbidden()
    {
        var (user1, user2, friendRequest) = await SeedData();
        AuthenticateAs(user1);

        //act
        var response = await _client.PostAsync(BaseEndpoint + $"requests/{friendRequest.Id}/reject", null);

        //assert
        await AssertProblemAsync(response, HttpStatusCode.Forbidden, ErrorCodes.Auth.Forbidden);

        //assert db not change
        var friendRequestInBb = await GetFriendshipByIdAsync(friendRequest);
        Assert.NotNull(friendRequestInBb);
        Assert.Equal(FriendShipStatus.Pending, friendRequestInBb.Status);
    }

    [Fact]
    public async Task ProcessFriendRequest_ThirdUserRejectFriendRequest_ReturnsForbidden()
    {
        var (user1, user2, friendRequest) = await SeedData();

        var user3 = new User()
        {
            Id = Guid.CreateVersion7(),
            Username = $"user_{Guid.CreateVersion7():N}",
            HashedPassword = "not-used-in-this-test",
            FullName = "User 3",
        };
        await _factory.ExecuteDbContextAsync(async dbContext =>
        {
            dbContext.Users.Add(user3);
            await dbContext.SaveChangesAsync();
        });
        AuthenticateAs(user3);

        //act
        var response = await _client.PostAsync(BaseEndpoint + $"requests/{friendRequest.Id}/reject", null);

        //assert
        await AssertProblemAsync(response, HttpStatusCode.Forbidden, ErrorCodes.Auth.Forbidden);

        //assert db not change
        var friendRequestInBb = await GetFriendshipByIdAsync(friendRequest);
        Assert.NotNull(friendRequestInBb);
        Assert.Equal(FriendShipStatus.Pending, friendRequestInBb.Status);
    }

    //seed data
    private async Task<(User user1, User user2, FriendShip friendRequest)> SeedData()
    {
        var user1 = new User()
        {
            Id = Guid.CreateVersion7(),
            Username = $"user_{Guid.CreateVersion7():N}",
            HashedPassword = "not-used-in-this-test",
            FullName = "User 1",
        };

        var user2 = new User()
        {
            Id = Guid.CreateVersion7(),
            Username = $"user_{Guid.CreateVersion7():N}",
            HashedPassword = "not-used-in-this-test",
            FullName = "User 2",
        };
        var friendRequest = new FriendShip()
        {
            Id = Guid.CreateVersion7(),
            RequesterId = user1.Id,
            UserLeftId = user1.Id,
            UserRightId = user2.Id,
            Message = "Hello",
            Status = FriendShipStatus.Pending
        };

        await _factory.ExecuteDbContextAsync(async dbContext =>
        {
            dbContext.Users.Add(user1);
            dbContext.Users.Add(user2);
            dbContext.Friendships.Add(friendRequest);
            await dbContext.SaveChangesAsync();
        });
        return (user1, user2, friendRequest);
    }

    //helper

    private void AuthenticateAs(User user)
    {
        _client.DefaultRequestHeaders.Remove(TestAuthenticationHandler.UserIdHeaderName);
        _client.DefaultRequestHeaders.Add(TestAuthenticationHandler.UserIdHeaderName, user.Id.ToString());
    }

    private async Task<FriendShip?> GetFriendshipByIdAsync(FriendShip friendRequest)
    {
        var friendRequestInBb = await _factory.ExecuteDbContextAsync(async dbContext =>
        {
            return await dbContext.Friendships.FirstOrDefaultAsync(x => x.Id == friendRequest.Id);
        });
        return friendRequestInBb;
    }

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
}