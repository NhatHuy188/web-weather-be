using Microsoft.EntityFrameworkCore;
using WeatherAPI.Models.Domains;

namespace WeatherAPI.Data
{
    public class SubcsribersDbContext: DbContext
    {
        public SubcsribersDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            
        }
        public DbSet<Subscriber> Subcribers { get; set; }
    }
}
