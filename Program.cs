using PaymentBroker.Contexts;

using DotEnv.Core;

new EnvLoader().Load();

string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION") ?? throw new ArgumentException("db connection string env not defined");
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddAuthorization();

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

await app.RunAsync();