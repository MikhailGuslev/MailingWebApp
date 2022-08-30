using DataLayer.Entities;
using LinqToDB;
using Mailing.Abstractions;
using PluginManager;

namespace MailingWebApp.Repositories;

public sealed class FakeMessageModelProviderRepository : IMessageModelProviderRepository
{
    private readonly StorageDb Storage;
    private readonly PluginService PluginService;

    public FakeMessageModelProviderRepository(StorageDb storage, PluginService pluginService)
    {
        Storage = storage;
        PluginService = pluginService;
    }

    public async Task<IMessageModelProvider> GetMessageModelProviderAsync(Type providerType)
    {
        throw new NotImplementedException();
    }

    public async Task<IMessageModelProvider> GetMessageModelProviderAsync(string providerTypeName)
    {
        ModelProvider? modelProviderDetails = await Storage.ModelProvider
            .FirstOrDefaultAsync(x => x.ModelProviderTypeName == providerTypeName);

        if (modelProviderDetails is null)
        {
            return null;
        }
        IMessageModelProvider? instance = null;

        (bool success, instance) = await PluginService.TryGetMessageModelProviderInstance(
            modelProviderDetails.ModelProviderTypeName,
            (int)modelProviderDetails.PluginId);

        return instance;
    }

    public async Task AddMessageModelProviderAsync(IMessageModelProvider modelProvider)
    {
        throw new NotImplementedException();
    }
}
