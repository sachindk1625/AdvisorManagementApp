using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class AdvisorDbContext : DbContext
    {
        public AdvisorDbContext(DbContextOptions<AdvisorDbContext> options) : base(options) { }
        public AdvisorDbContext()
        {
        }
        public virtual DbSet<AdvisorEntity> Advisors { get; set; }
    }
}
