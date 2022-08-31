using DataLayer.Entities;
using LinqToDB;
using Mailing.Abstractions;
using PluginManager.Abstractions;
using PluginManager.Models;

namespace MailingWebApp.Repositories;

public sealed class FakeMessageModelProviderRepository : IMessageModelProviderRepository
{
    private readonly StorageDb Storage;
    private readonly IPluginService PluginService;

    public FakeMessageModelProviderRepository(StorageDb storage, IPluginService pluginService)
    {
        Storage = storage;
        PluginService = pluginService;
    }

    public async Task<IMessageModelProvider?> GetMessageModelProviderAsync(Type providerType)
    {
        throw new NotImplementedException();
    }

    public async Task<IMessageModelProvider?> GetMessageModelProviderAsync(string providerTypeName)
    {
        ModelProvider? modelProviderDetails = await Storage.ModelProvider
            .FirstOrDefaultAsync(x => x.ModelProviderTypeName == providerTypeName);

        IMessageModelProvider? instance = modelProviderDetails is not null
            ? await PluginService.GetPluggableTypeInstanceAsync(new InstanceCreationOptions
            {
                PluginId = (int)modelProviderDetails.PluginId,
                TypeName = modelProviderDetails.ModelProviderTypeName,
                ConstructorArgumets = Array.Empty<object>(),
                InterfaceType = typeof(IMessageModelProvider)
            }) as IMessageModelProvider
            : null;

        return instance;
    }

    public async Task AddMessageModelProviderAsync(IMessageModelProvider modelProvider)
    {
        throw new NotImplementedException();
    }
}
