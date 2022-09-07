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
        return await Storage.PluginAssembly
            .Select(x => new PluginAssembly
            {
                PluginAssemblyId = (int)x.PluginAssemblyId,
                Name = x.Name,
                Comment = x.Comment,
                Settings = x.Settings,
                Data = x.Data,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
            })
            .ToListAsync();
    }

    public async Task<PluginAssemblyInformation> GetPluginAssemblyInformationAsync(int pluginAssemblyId)
    {
        return await Storage.PluginAssembly
            .Select(x => new PluginAssemblyInformation
            {
                PluginAssemblyId = (int)x.PluginAssemblyId,
                Name = x.Name,
                Comment = x.Comment,
                Settings = x.Settings,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
            })
            .FirstAsync(x => x.PluginAssemblyId == pluginAssemblyId);
    }

    public async Task<PluginAssembly> GetPluginAssemblyAsync(int pluginAssemblyId)
    {
        return await Storage.PluginAssembly
            .Select(x => new PluginAssembly
            {
                PluginAssemblyId = (int)x.PluginAssemblyId,
                Name = x.Name,
                Comment = x.Comment,
                Settings = x.Settings,
                Data = x.Data,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
            })
            .FirstAsync(x => (int)x.PluginAssemblyId == pluginAssemblyId);
    }

    public async Task AddPluginAssemblyAsync(PluginAssembly plugin)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}
