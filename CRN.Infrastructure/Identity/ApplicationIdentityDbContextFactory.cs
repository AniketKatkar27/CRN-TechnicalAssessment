using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CRN.Infrastructure.Identity;

public class ApplicationIdentityDbContextFactory
    : IDesignTimeDbContextFactory<ApplicationIdentityDbContext>
{
    public ApplicationIdentityDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder =
            new DbContextOptionsBuilder<ApplicationIdentityDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=LENOVO\\SQLEXPRESS;Database=CRN.TechnicalAssessmentDb;Trusted_Connection=True;TrustServerCertificate=True;");

        return new ApplicationIdentityDbContext(
            optionsBuilder.Options);
    }
}