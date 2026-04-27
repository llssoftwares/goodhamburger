using GoodHamburger.Api.Extensions;
using GoodHamburger.Api.Features.Menu;
using GoodHamburger.Api.Features.Orders;
using GoodHamburger.Infrastructure.Data;
using GoodHamburger.ServiceDefaults;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services
    .AddProblemDetails()
    .AddOpenApi();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("AllowAll");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GoodHamburgerDbContext>();
    db.Database.Migrate();
}

app.MapMenuEndpoints();
app.MapOrderEndpoints();

app.MapDefaultEndpoints();

app.Run();