using Keycloak.Web.DelegateHandler;
using Keycloak.Web.Options;
using Keycloak.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services
    .AddOptions<ClientOption>()
    .BindConfiguration(nameof(ClientOption))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<ClientOption>(sp => sp.GetRequiredService<IOptions<ClientOption>>().Value);

builder.Services
    .AddOptions<ServiceOption>()
    .BindConfiguration(nameof(ServiceOption))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<ServiceOption>(sp => sp.GetRequiredService<IOptions<ServiceOption>>().Value);


var serviceOption = builder.Configuration.GetSection(nameof(ServiceOption)).Get<ServiceOption>();


builder.Services.AddScoped<Microservice1Service>();

builder.Services.AddScoped<ClientCredentialDelegateHandler>();
builder.Services.AddRefitClient<IMicroservice1Refit>()
    .ConfigureHttpClient(c =>
        c.BaseAddress = new Uri(serviceOption!.MicroService1.BaseAddress)
    ).AddHttpMessageHandler<ClientCredentialDelegateHandler>();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
    CookieAuthenticationDefaults.AuthenticationScheme, opts =>
    {
        opts.LoginPath = "/Auth/SignIn";
        opts.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        opts.Cookie.Name = "webCookie";
        opts.AccessDeniedPath = "/AccessDenied";
    });


builder.Services.AddAuthentication();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();