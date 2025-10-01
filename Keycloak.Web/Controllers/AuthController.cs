using Duende.IdentityModel.Client;
using Keycloak.Web.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Keycloak.Web.Controllers
{
    public class AuthController(
        IHttpClientFactory httpClientFactory,
        ServiceOption serviceOption,
        ClientOption clientOption) : Controller
    {
        public async Task<IActionResult> SignIn()
        {
            var client = httpClientFactory.CreateClient("user_credential");


            client.BaseAddress = new Uri(serviceOption.IdentityServer.BaseAddress);


            var responseAsDiscovery = await client.GetDiscoveryDocumentAsync();

            if (responseAsDiscovery.IsError)
            {
                throw new Exception($"Failed to obtain discovery document: {responseAsDiscovery.Error}");
            }


            var response = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = responseAsDiscovery.TokenEndpoint,
                ClientId = clientOption.ClientId,
                ClientSecret = clientOption.ClientSecret,
                UserName = "f-cakiroglu@outlook.com",
                Password = "Password12*",
            });


            if (response.IsError)
            {
                var errors = response.ErrorDescription;
            }


            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(response.AccessToken);


            var identity = new ClaimsIdentity(jwtSecurityToken.Claims,
                CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimTypes.Name, ClaimTypes.Role);


            var authenticationTokens = new List<AuthenticationToken>
            {
                new()
                {
                    Name = OpenIdConnectParameterNames.AccessToken,
                    Value = response.AccessToken!
                },
                new()
                {
                    Name = OpenIdConnectParameterNames.RefreshToken,
                    Value = response.RefreshToken!
                },
                new()
                {
                    Name = OpenIdConnectParameterNames.ExpiresIn,
                    Value = DateTime.Now.AddSeconds(response.ExpiresIn)
                        .ToString("o", CultureInfo.InvariantCulture)
                },
            };

            var principal = new ClaimsPrincipal(identity);


            var authenticationProperties = new AuthenticationProperties();
            authenticationProperties.StoreTokens(authenticationTokens);

            await HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authenticationProperties);


            return View();
        }
    }
}