using Play.Common.MongoDB;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Polly;
using Polly.Timeout;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMongo().AddMongoRepository<InventoryItem>("inventoryitems");
Random jitterer = new Random();


builder.Services.AddHttpClient<CatalogClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7286");
})
.AddTransientHttpErrorPolicy(build => build.Or<TimeoutRejectedException>().WaitAndRetryAsync(
retryCount: 5,
sleepDurationProvider => TimeSpan.FromSeconds(Math.Pow(2, sleepDurationProvider)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000)),
onRetry: (outcome, timespan, retryAttempt) =>
{
    var serviceProvider = builder.Services.BuildServiceProvider();

    serviceProvider.GetService<ILogger<CatalogClient>>()?.LogWarning($"Delaying for {timespan.TotalSeconds} second, then making retry {retryAttempt}");
}
))
.AddTransientHttpErrorPolicy(build => build.Or<TimeoutRejectedException>().CircuitBreakerAsync(
3,
TimeSpan.FromSeconds(15),
onBreak: (outcome, timespan) =>
{
    var serviceProvider = builder.Services.BuildServiceProvider();

    serviceProvider.GetService<ILogger<CatalogClient>>()?.LogWarning($"Opening the circuit for {timespan.TotalSeconds} seconds...");
},
onReset: () =>
{
    var serviceProvider = builder.Services.BuildServiceProvider();
    serviceProvider.GetService<ILogger<CatalogClient>>()?.LogWarning($"Closing the circuit...");
}
))
.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
