using System.ComponentModel.DataAnnotations;

namespace Keycloak.Web.Options
{
    public class ClientOption
    {
        [Required] public required string ClientId { get; set; }
        [Required] public required string ClientSecret { get; set; }
    }
}