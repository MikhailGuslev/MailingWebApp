using LinqToDB;
using PluginManager.Abstractions;
using PluginManager.Models;
using Dal = DataLayer.Entities;

namespace MailingWebApp.Repositories;

public sealed class FakePluginRepository : IPluginAssemblyRepository
{
    private readonly Dal.StorageDb Storage;

    public FakePluginRepository(Dal.StorageDb storage)
    {
        Storage = storage;
    }

    public async Task<IReadOnlyList<PluginAssembly>> GetAllPluginAssembliesAsync()
    {
        return await Storage.Plugin
            .Select(x => new PluginAssembly
            {
                PluginAssemblyId = (int)x.PluginId,
                Name = x.Name,
                Comment = x.Comment,
                Data = x.Data,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
            })
            .ToListAsync();
    }

    public async Task<PluginAssemblyInformation?> GetPluginAssemblyInformationAsync(int pluginId)
    {
        return await Storage.Plugin
            .Select(x => new PluginAssemblyInformation
            {
                PluginId = (int)x.PluginId,
                Name = x.Name,
                Comment = x.Comment,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
            })
            .FirstOrDefaultAsync(x => x.PluginId == pluginId);
    }

    public async Task<PluginAssembly?> GetPluginAssemblyAsync(int pluginId)
    {
        return await Storage.Plugin
            .Select(x => new PluginAssembly
            {
                PluginAssemblyId = (int)x.PluginId,
                Name = x.Name,
                Comment = x.Comment,
                Data = x.Data,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
            })
            .FirstOrDefaultAsync(x => (int)x.PluginAssemblyId == pluginId);
    }

    public async Task AddPluginAssemblyAsync(PluginAssembly plugin)
    {
        throw new NotImplementedException();
    }
}
