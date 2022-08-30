using LinqToDB;
using PluginManager.Abstractions;
using PluginManager.Models;
using Dal = DataLayer.Entities;

namespace MailingWebApp.Repositories;

public sealed class FakePluginRepository : IPluginRepository
{
    private readonly Dal.StorageDb Storage;

    public FakePluginRepository(Dal.StorageDb storage)
    {
        Storage = storage;
    }

    public async Task AddPluginAsync(Plugin plugin)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<Plugin>> GetAllPluginsAsync()
    {
        List<Plugin> plugins = await Storage.Plugin
            .Select(x => MapToModel(x))
            .ToListAsync();

        return plugins;
    }

    public async Task<Plugin> GetPluginAsync(int pluginId)
    {
        Dal.Plugin plugin = await Storage.Plugin
            .FirstAsync(x => x.PluginId == pluginId);

        return MapToModel(plugin);
    }

    private Plugin MapToModel(Dal.Plugin original)
    {
        return new Plugin
        {
            PluginId = (int)original.PluginId,
            Name = original.Name,
            Comment = original.Comment,
            Data = original.Data,
            CreatedDate = original.CreatedDate,
            UpdatedDate = original.UpdatedDate,
        };
    }
}
