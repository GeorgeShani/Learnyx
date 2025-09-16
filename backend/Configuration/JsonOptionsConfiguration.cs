using System.Text.Json.Serialization;

namespace learnyx.Configuration;

public static class JsonOptionsConfiguration
{
    public static IServiceCollection AddJsonOptions(this IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        
        return services;   
    }  
}