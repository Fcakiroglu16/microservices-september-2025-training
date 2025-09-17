using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Bus;

namespace Microservice1.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpointProvider)
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Craete()
        {
            // create order

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));

            await publishEndpoint.Publish(new OrderCreatedEvent(Guid.NewGuid(), Guid.NewGuid(), 2, Guid.NewGuid()),
                pipeline =>
                {

                    pipeline.SetAwaitAck(true);
                    pipeline.Durable = true;
                    pipeline.MessageId = Guid.NewGuid();
                    pipeline.Headers.Set("version", "v1");
                }, cancellationTokenSource.Token);

            return Ok();
        }
    }
}