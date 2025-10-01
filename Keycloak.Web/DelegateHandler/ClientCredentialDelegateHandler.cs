using Duende.IdentityModel.Client;
using Keycloak.Web.Options;
using System.Net.Http.Headers;

namespace Keycloak.Web.DelegateHandler
{
    public class ClientCredentialDelegateHandler(
        ClientOption clientOption,
        ServiceOption serviceOption,
        IHttpClientFactory httpClientFactory) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var client = httpClientFactory.CreateClient("client_credential");


            client.BaseAddress = new Uri(serviceOption.IdentityServer.BaseAddress);


            var response = await client.GetDiscoveryDocumentAsync(cancellationToken: cancellationToken);

            if (response.IsError)
            {
                throw new UnauthorizedAccessException($"Failed to obtain discovery document: {response.Error}");
            }

            var requestDto = new ClientCredentialsTokenRequest()
            {
                ClientId = clientOption.ClientId,
                ClientSecret = clientOption.ClientSecret,
                Address = response.TokenEndpoint
            };


            var tokenResponse =
                await client.RequestClientCredentialsTokenAsync(requestDto, cancellationToken: cancellationToken);

            if (tokenResponse.IsError)
            {
                throw new Exception($"Failed to obtain access token: {tokenResponse.Error}");
            }


            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            var response2 = await base.SendAsync(request, cancellationToken);
            return response2;
        }
    }
}