using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class Startup
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddDbContext<AdvisorDbContext>(opts => 
                opts.UseInMemoryDatabase("AdvisorDb"));
        }
    }
}
