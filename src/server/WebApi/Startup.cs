using System.Reflection;

namespace WebApi
{
    public static class Startup
    {
        public static string LocalCorsOrigin = nameof(LocalCorsOrigin);
        public static void AddWebApi(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: LocalCorsOrigin,
                    policy =>
                    {
                        options.AddPolicy("AllowAll", policy =>
                        {
                            policy.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                        });
                    });
            });

        }
    }
}
