using MassTransit;
using Microservice2.API.Consumers;
using Microservice2.API.Services;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        // message retry with 5 count;
        cfg.UseMessageRetry(r => r.Interval(5, 100));

        //message delayed redelivery with 3 count and 10 seconds interval
        cfg.UseDelayedRedelivery(configureRetry =>
            configureRetry.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(10)));


        cfg.UseInMemoryOutbox(context);

        cfg.Host("amqps://ffqycmjd:cYOzydQzAx6gSvtbPdF4zU5T-2r4c-UR@gorilla.lmq.cloudamqp.com/ffqycmjd");

        // receive endpoint
        cfg.ReceiveEndpoint("stock-microservice.order-created.event",
            e => { e.ConfigureConsumer<OrderCreatedEventConsumer>(context); });
    });
});

var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError() // HTTP 5xx, 408 ve network hatalarını yakalar
    /*.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)*/ // 404 hatalarını da yakalar
    .WaitAndRetryAsync(3,
        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))); // 3 kez dene, aradaki süreyi arttı

var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(3, TimeSpan.FromMinutes(1)); // 2 hata sonrası 1 dakika devreye girmez

//advanced circuit breaker

var circuitAdvancedBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError().AdvancedCircuitBreakerAsync(0.5, TimeSpan.FromMinutes(1), 10, TimeSpan.FromSeconds(30));

var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(5);
var combinedPolicy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, timeoutPolicy);

builder.Services.AddHttpClient<OrderService>(options =>
    {
        options.BaseAddress = new Uri(builder.Configuration.GetSection("Services")["Microservice1BaseUrl"]!);
    })
    .AddPolicyHandler(combinedPolicy);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseAuthorization();

app.MapControllers();

app.Run();