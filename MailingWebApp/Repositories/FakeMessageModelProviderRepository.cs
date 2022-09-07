using DataLayer.Entities;
using LinqToDB;
using Mailing.Abstractions;
using PluginManager.Abstractions;

namespace MailingWebApp.Repositories;

public sealed class FakeMessageModelProviderRepository : IMessageModelProviderRepository
{
    private readonly StorageDb Storage;
    private readonly IPluginService PluginService;
    private readonly IServiceScopeFactory ServiceScopeFactory;

    public FakeMessageModelProviderRepository(
        IServiceScopeFactory scopeFactory,
        StorageDb storage,
        IPluginService pluginService)
    {
        Storage = storage;
        PluginService = pluginService;
        ServiceScopeFactory = scopeFactory;
    }

    public async Task<IMessageModelProvider> GetMessageModelProviderAsync(int messageModelProviderId)
    {
        ModelProvider? modelProvider = await Storage.ModelProvider
            .FirstOrDefaultAsync(x => x.ModelProviderId == messageModelProviderId);

        if (modelProvider is null)
        {
            string error = $"Не удалось получить поставщика модели данных с id:{messageModelProviderId}.";
            throw new ApplicationException(error);
        }

        IPlugin pluginInstance = await PluginService.GetPluginInstanceAsync(
            (int)modelProvider.PluginAssemblyId,
            ServiceScopeFactory);

        IMessageModelProvider? instance = pluginInstance as IMessageModelProvider;

        if (instance is null)
        {
            string error =
                $"Не удалось получить экземпляр поставщика модели данных с id:{messageModelProviderId}. " +
                "Вместо экземпляра получено значение null.";
            throw new ApplicationException(error);
        }

        return instance;
    }

    public async Task AddMessageModelProviderAsync(IMessageModelProvider modelProvider)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}
