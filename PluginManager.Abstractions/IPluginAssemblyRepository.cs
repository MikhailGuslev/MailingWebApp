using PluginManager.Models;

namespace PluginManager.Abstractions;

public interface IPluginAssemblyRepository
{
    Task<PluginAssemblyInformation?> GetPluginAssemblyInformationAsync(int pluginId);
    Task<PluginAssembly?> GetPluginAssemblyAsync(int pluginId);
    Task<IReadOnlyList<PluginAssembly>?> GetAllPluginAssembliesAsync();
    Task AddPluginAssemblyAsync(PluginAssembly plugin);
}
