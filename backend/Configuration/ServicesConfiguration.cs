using Amazon.S3;
using FluentValidation;
using learnyx.SMTP.Interfaces;
using learnyx.Utilities.Mappings;
using learnyx.Utilities.Constants;
using learnyx.Services.Interfaces;
using learnyx.SMTP.Implementation;
using learnyx.Services.Implementation;
using learnyx.Repositories.Interfaces;
using learnyx.Repositories.Implementation;

namespace learnyx.Configuration;

public static class ServicesConfiguration
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        // External Services
        services.AddAWSService<IAmazonS3>();
        services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
        
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAmazonS3Service, AmazonS3Service>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Settings
        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
        services.Configure<AwsS3Settings>(configuration.GetSection("AwsS3"));

        return services;
    }
}