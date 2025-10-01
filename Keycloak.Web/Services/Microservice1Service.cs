using Keycloak.Web.Options;
using Refit;
using System.Text.Json;

namespace Keycloak.Web.Services
{
    public record GetProductResponse(int Id, string Name, decimal Price);

    public record ProductViewModel(int Id, string Name, decimal Price);

    public interface IMicroservice1Refit
    {
        [Get("/products")]
        Task<ApiResponse<List<GetProductResponse>>> GetProductsAsync();
    }


    public class Microservice1Service(ClientOption clientOption, IMicroservice1Refit microservice1Refit)
    {
        public async Task<List<ProductViewModel>> GetProducts()
        {
            var response = await microservice1Refit.GetProductsAsync();

            if (response.IsSuccessStatusCode)
            {
                return response.Content!.Select(p => new ProductViewModel(p.Id, p.Name, p.Price)).ToList();
            }
            else
            {
                var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(response.Error.Content!);
                return [];
            }
        }
    }
}