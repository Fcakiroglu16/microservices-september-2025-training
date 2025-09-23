using Microsoft.AspNetCore.Mvc;

namespace Microservice2.API.Services
{
    public record GetOrderResponse(int Id, string OrderCode);

    public class OrderService(HttpClient client)
    {
        public async Task GetOrder()
        {
            var response = await client.GetAsync("/api/orders");


            if (response.IsSuccessStatusCode)
            {
                var newGetOrderResponse = await response.Content.ReadFromJsonAsync<GetOrderResponse>();
            }
            else
            {
                var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            }
        }
    }
}