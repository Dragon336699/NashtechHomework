using BankSimulationMVC.Application.Validation.Accounts;
using BankSimulationMVC.BackgroundServices;
using BankSimulationMVC.Data;
using BankSimulationMVC.Infrastructure.Persistence.Seed;
using BankSimulationMVC.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddServices();
builder.Services.AddHostedService<InterestService>();

builder.Services.AddValidatorsFromAssemblyContaining<AccountDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DepositValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<WithdrawValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<TransferValidator>();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddDbContext<BankDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Index}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<SeederRunner>();
    await runner.RunAsync();
}

app.Run();
