using MassTransit;
using Microservice2.API.Consumers;

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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();