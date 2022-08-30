using DataLayer.Entities;
using LinqToDB.AspNet;
using LinqToDB.Configuration;
using Mailing.Abstractions;
using MailingWebApp.Repositories;
using PluginManager.Abstractions;

namespace MailingWebApp.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStorageContext(this IServiceCollection services)
    {
        services
            .AddLinqToDBContext<StorageDb>((provider, options) =>
            {
                string connectStr = provider
                    .GetRequiredService<IConfiguration>()
                    .GetConnectionString(nameof(StorageDb));
                options.UseSQLite(connectStr);
            });

        return services;
    }

    public static IServiceCollection AddScopedRepositories(this IServiceCollection services)
    {
        services
            .AddScoped<IMessageModelProviderRepository, FakeMessageModelProviderRepository>()
            .AddScoped<IEmailSendingRepository, FakeEmailSendingRepository>()
            .AddScoped<IPluginRepository, FakePluginRepository>()
            .AddScoped<FakeMessageTemplateRepository>()
            .AddScoped<FakeUserRepository>()
            .AddScoped<FakeRecipientRepository>();

        return services;
    }
}
