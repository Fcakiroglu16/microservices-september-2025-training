namespace Keycloak.Web.Options
{
    public class ServiceOption
    {
        public required ServiceOptionItem MicroService1 { get; set; }
        public required ServiceOptionItem MicroService2 { get; set; }

        public required ServiceOptionItem IdentityServer { get; set; }
    }

    public class ServiceOptionItem
    {
        public required string BaseAddress { get; set; }
    }
}