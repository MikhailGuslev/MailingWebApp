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

    public async Task<IReadOnlyList<Plugin>> GetAllPluginsAsync()
    {
        return await Storage.Plugin
            .Select(x => new Plugin
            {
                PluginId = (int)x.PluginId,
                Name = x.Name,
                Comment = x.Comment,
                Data = x.Data,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
            })
            .ToListAsync();
    }

    public async Task<PluginInformation?> GetPluginInformationAsync(int pluginId)
    {
        return await Storage.Plugin
            .Select(x => new PluginInformation
            {
                PluginId = (int)x.PluginId,
                Name = x.Name,
                Comment = x.Comment,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
            })
            .FirstOrDefaultAsync(x => x.PluginId == pluginId);
    }

    public async Task<Plugin?> GetPluginAsync(int pluginId)
    {
        return await Storage.Plugin
            .Select(x => new Plugin
            {
                PluginId = (int)x.PluginId,
                Name = x.Name,
                Comment = x.Comment,
                Data = x.Data,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
            })
            .FirstOrDefaultAsync(x => (int)x.PluginId == pluginId);
    }

    public async Task AddPluginAsync(Plugin plugin)
    {
        throw new NotImplementedException();
    }
}
