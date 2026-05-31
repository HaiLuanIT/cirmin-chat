using Moji.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// add cors
builder.Services.AddCorsServices();

//add map controller
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//add services, repositories
builder.Services.AddServiceServices();
builder.Services.AddRepositoryServices();

//add signalR
builder.Services.AddSignalR();

//add db
builder.Services.AddDatabaseServices(builder.Configuration);

//add identity
builder.Services.AddIdentityServices(builder.Configuration);

//add swagger
builder.Services.AddSwaggerServices();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//add middleware cors
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();