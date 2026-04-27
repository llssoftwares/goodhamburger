using GoodHamburger.ServiceDefaults;
using GoodHamburger.Web.Clients;
using GoodHamburger.Web.Clients.Menu;
using GoodHamburger.Web.Clients.Orders;
using GoodHamburger.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddApiHttpClient<MenuClient>(builder.Configuration);
builder.Services.AddApiHttpClient<OrderClient>(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();