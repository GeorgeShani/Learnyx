using FluentValidation;
using learnyx.Utilities.Mappings;
using learnyx.Repositories.Interfaces;
using learnyx.Repositories.Implementation;
using learnyx.SMTP.Implementation;
using learnyx.SMTP.Interfaces;
using learnyx.Utilities.Constants;

namespace learnyx.Configuration;

public static class ServicesConfiguration
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<Program>();

        // AutoMapper
        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

        // SMTP settings
        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));

        return services;
    }
}