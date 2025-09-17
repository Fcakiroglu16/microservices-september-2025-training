using MassTransit;
using Shared.Bus;

namespace Microservice2.API.Consumers
{
    public class OrderCreatedEventConsumer(
        ILogger<OrderCreatedEventConsumer> logger,
        IPublishEndpoint publishEndpoint,
        IServiceProvider serviceProvider)
        : IConsumer<Shared.Bus.OrderCreatedEvent>
    {
        public Task Consume(ConsumeContext<Shared.Bus.OrderCreatedEvent> context)
        {
            var version = context.Headers.Get("version", "v1");


            // message
            logger.LogInformation("Consume methodu çalıştı");


            throw new Exception("Db hatası");

            using (var scope = serviceProvider.CreateScope())
            {
                // var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            }

            publishEndpoint.Publish(new ExampleEvent());
            publishEndpoint.Publish(new ExampleEvent());
            logger.LogInformation($"order Id:{context.Message.OrderId},userId :{context.Message.UserId}");

            // handle event
            return Task.CompletedTask;
        }
    }
}