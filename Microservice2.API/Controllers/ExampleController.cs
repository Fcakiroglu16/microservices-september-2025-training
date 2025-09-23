using Microservice2.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Microservice2.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExampleController(OrderService orderService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await orderService.GetOrder();
            return Ok();
        }
    }
}