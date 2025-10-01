using Keycloak.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Keycloak.Web.Controllers
{
    public class ProductsController(Microservice1Service microservice1Service) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await microservice1Service.GetProducts());
        }
    }
}