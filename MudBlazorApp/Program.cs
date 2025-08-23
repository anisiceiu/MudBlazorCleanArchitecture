using Application.Services;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using MudBlazor.Services;
using MudBlazorApp.Components;
using System.Data;

namespace MudBlazorApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            // Add MudBlazor Services
            builder.Services.AddMudServices();

            builder.Services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IChartsRepository, ChartsRepository>();
            builder.Services.AddTransient<IDbConnection>(sp =>
                                            new SqlConnection(builder.Configuration.GetConnectionString("NorthwindConnection")));
            builder.Services.AddScoped<CategoryService>();
            builder.Services.AddScoped<ChartsService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
