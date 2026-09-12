using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CirMin.API.IntegrationTests.Infrastructure;
using CirMin.Contracts.Models.Messages.SendMessage;
using CirMin.DataAccess.Commons.Constants;
using CirMin.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace CirMin.API.IntegrationTests.Messages.SendMessage;

[Collection(PostgreSqlCollection.Name)]
public class SendMessageTests : IAsyncLifetime
{
    private const string BaseEndPoint = "/api/message/";
    private readonly HttpClient _client;
    private readonly CirMinApiFactory _factory;

    public SendMessageTests(PostgreSqlFixture database)
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
    
    //test cases
    [Fact]
    public async Task SendMessage_ValidRequest_CreatesMessageAndUpdatesMemberState()
    {
        var user1 = new User()
        {
            Id = Guid.CreateVersion7(),
            Username = $"user_{Guid.CreateVersion7():N}",
            HashedPassword = "not-used-in-this-test",
            FullName = "User 1"
        };

        var user2 = new User()
        {
            Id = Guid.CreateVersion7(),
            Username = $"user_{Guid.CreateVersion7():N}",
            HashedPassword = "not-used-in-this-test",
            FullName = "User 2"
        };
        
        var conversation = new Conversation()
        {
            Id = Guid.CreateVersion7(),
            Name = "Test Conversation",
            IsGroup = false,
            Members = new List<ConversationMember> { new ConversationMember { UserId = user1.Id }, new ConversationMember { UserId = user2.Id } },
        };
        await _factory.ExecuteDbContextAsync(async dbContext =>
        {
            dbContext.Users.Add(user1);
            dbContext.Users.Add(user2);
            dbContext.Conversations.Add(conversation);
            await dbContext.SaveChangesAsync();
        });
        
        var request = new SendMessageRequest
        (
            "Hello",
            conversation.Id,
            null
        );
        AuthenticateAs(user1);
        
        //act
        var response = await _client.PostAsJsonAsync(BaseEndPoint + "send", request);
        
        //assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        //assert db change
        var conversationInDb = await _factory.ExecuteDbContextAsync(async dbContext =>
        {
            return await dbContext.Conversations.Include(x => x.Messages).Include(c => c.Members).SingleOrDefaultAsync(c => c.Id == conversation.Id);
        });
        
        Assert.NotNull(conversationInDb);
        
        var createdMessage = Assert.Single(conversationInDb.Messages);
        
        var senderMember = Assert.Single(conversationInDb.Members.Where(m => m.UserId == user1.Id));
        var receiverMember = Assert.Single(conversationInDb.Members.Where(m => m.UserId == user2.Id));
        
        Assert.Equal(0, senderMember.UnreadCount);
        Assert.Equal(1, receiverMember.UnreadCount);
        
        Assert.Equal(createdMessage.Id, senderMember.LastSeenMessageId);
        Assert.Equal(0, receiverMember.LastSeenMessageId);
    }
    //seed data
    
    
    //helper methods
    private void AuthenticateAs(User user)
    {
        _client.DefaultRequestHeaders.Remove(TestAuthenticationHandler.UserIdHeaderName);
        _client.DefaultRequestHeaders.Add(TestAuthenticationHandler.UserIdHeaderName, user.Id.ToString());
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