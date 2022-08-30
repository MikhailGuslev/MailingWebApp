using Mailing.Abstractions;
using MailingWebApp.Repositories;

namespace MailingWebApp.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddScopedRepositories(this IServiceCollection services)
    {
        services
            .AddScoped<IMessageModelProviderRepository, FakeMessageModelProviderRepository>()
            .AddScoped<IEmailSendingRepository, FakeEmailSendingRepository>()
            .AddScoped<FakeUserRepository>()
            .AddScoped<FakeRecipientRepository>();

        return services;
    }
}
