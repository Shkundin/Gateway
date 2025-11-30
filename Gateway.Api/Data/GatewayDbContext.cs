using Gateway.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Api.Data
{
    public class GatewayDbContext : DbContext
    {
        public GatewayDbContext(DbContextOptions<GatewayDbContext> options)
            : base(options)
        {
        }

        public DbSet<Submission> Submissions => Set<Submission>();
    }
}
