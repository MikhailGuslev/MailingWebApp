using PluginManager.Models;

namespace PluginManager.Abstractions;

public interface IPluginAssemblyRepository
{
    Task<PluginAssemblyInformation> GetPluginAssemblyInformationAsync(int pluginAssemblyId);
    Task<PluginAssembly> GetPluginAssemblyAsync(int pluginAssemblyId);
    Task<IReadOnlyList<PluginAssembly>> GetAllPluginAssembliesAsync();
    Task AddPluginAssemblyAsync(PluginAssembly plugin);
}
