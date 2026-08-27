using CRN.Infrastructure.Data;
using CRN.Infrastructure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace CRN.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration(
            (context, config) =>
            {
                var settings = new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] =
                        "Server=LENOVO\\SQLEXPRESS;Database=CRN.TechnicalAssessmentDb_Test;Trusted_Connection=True;TrustServerCertificate=True;"
                };

                config.AddInMemoryCollection(settings);
            });

        builder.ConfigureServices(services =>
        {
            using var serviceProvider =
                services.BuildServiceProvider();

            using var scope =
                serviceProvider.CreateScope();

            var productDb =
                scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

            var identityDb =
                scope.ServiceProvider
                    .GetRequiredService<ApplicationIdentityDbContext>();

            productDb.Database.Migrate();
            identityDb.Database.Migrate();
        });
    }
}