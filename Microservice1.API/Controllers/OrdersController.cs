using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Shared.Bus;

namespace Microservice1.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(
        IPublishEndpoint publishEndpoint,
        ISendEndpointProvider sendEndpointProvider,
        IHttpContextAccessor httpContextAccessor)
        : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create()
        {
            if (httpContextAccessor.HttpContext.Request.Headers.TryGetValue("idempotency-key", out StringValues value))
            {
                var idempotencyKey = value.ToString();
            }


            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));

            //transaction scope
            // create order
            // outbox table ( Json body string,event type,isSend,created,idempotency key)


            await publishEndpoint.Publish(new OrderCreatedEvent(Guid.NewGuid(), Guid.NewGuid(), 2, Guid.NewGuid()),
                pipeline =>
                {
                    pipeline.SetAwaitAck(true);
                    pipeline.Durable = true;
                    pipeline.MessageId = Guid.NewGuid();
                    pipeline.Headers.Set("version", "v1");
                    pipeline.Headers.Set("idempotency-key", Guid.NewGuid());
                }, cancellationTokenSource.Token);

            return Ok();
        }


        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { Id = 1, OrderCode = "123" });
        }
    }
}