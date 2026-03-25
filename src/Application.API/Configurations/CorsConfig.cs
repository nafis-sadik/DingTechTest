namespace DingTechTest.Configurations
{
    /// <summary>
    /// Extension methods for setting up CORS Configurations.
    /// </summary>
    public static class CorsConfig
    {
        /// <summary>
        /// CORS Policy name of custom configured policy
        /// </summary>
        public static string Policy = "SpecificOrigin";

        /// <summary>
        /// Registers CORS allowed domains from appsettings.json under "CORS:AllowedHosts"
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        public static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(opts =>
                opts.AddPolicy(Policy,
                    policy =>
                    {
                        var allowedHosts = configuration.GetSection("AccessConfig:Audiences")
                            .GetChildren()
                            .Select(c => c["Host"])
                            .Where(h => !string.IsNullOrWhiteSpace(h) && !string.IsNullOrEmpty(h))
                            .ToArray();

                        if (allowedHosts != null && allowedHosts.Any())
                        {
                            policy
                                .WithOrigins(allowedHosts!)
                                .WithExposedHeaders("x-total-count")
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                        }
                    }));

            return services;
        }
    }
}
