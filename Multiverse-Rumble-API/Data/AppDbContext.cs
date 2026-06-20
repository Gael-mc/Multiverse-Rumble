using Microsoft.EntityFrameworkCore;
using Multiverse_Rumble_API.Models;

namespace Multiverse_Rumble_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<MatchResult> Matches => Set<MatchResult>();
    }
}