namespace learnyx.Configuration;

public static class CorsConfiguration
{
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()  // Allow all domains
                    .AllowAnyMethod()    // Allow all HTTP methods (GET, POST, PUT, DELETE...)
                    .AllowAnyHeader();   // Allow all headers
            });
        });
        
        return services;   
    }   
}