using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CRN.Infrastructure.Identity;

public class ApplicationIdentityDbContext
    : IdentityDbContext<ApplicationUser>
{
    public ApplicationIdentityDbContext(
        DbContextOptions<ApplicationIdentityDbContext> options)
        : base(options)
    {
    }
}