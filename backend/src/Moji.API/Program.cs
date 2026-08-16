using Microsoft.AspNetCore.SignalR;
using Moji.API.Extensions;
using Moji.API.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// add cors
builder.Services.AddCorsServices();

//add map controller
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//add services, repositories
builder.Services.AddServiceServices(builder.Configuration);
builder.Services.AddRepositoryServices();

//add signalR
// builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddSignalR()
    .AddHubOptions<ChatHub>(options => { options.AddFilter<AuthVersionHubFilter>(); });
//add db
builder.Services.AddDatabaseServices(builder.Configuration);

//add identity
builder.Services.AddIdentityServices(builder.Configuration);

//add swagger
builder.Services.AddSwaggerServices();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseApplicationExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); //create file swagger.json
    app.UseSwaggerUI(); //show swagger ui
}

app.UseHttpsRedirection();

//add middleware cors
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

//add signalR middleware
app.MapHub<ChatHub>("/hubs/chat", options => { options.CloseOnAuthenticationExpiration = true; });

app.MapControllers();

app.Run();

public partial class Program
{
}