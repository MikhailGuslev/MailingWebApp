using PluginManager.Models;

namespace PluginManager.Abstractions;

public interface IPluginService
{
    Task<IPlugin> GetPluginInstanceAsync(int pluginAssemblyId, params object[] arguments);
    Task<IPluginSettings> GetPluginSettingsInstanceAsync(int pluginAssemblyId);
    Task<IReadOnlyList<PluginAssemblyInformation>> GetPluginAssemblyInformationsAsync();
    Task<PluginAssemblyInformation> GetPluginAssemblyInformationAsync(int pluginAssemblyId);
    Task AddPluginAssemblyAsync(PluginAssembly plugin);
    Task UpdatePluginAssemblyAsync(PluginAssembly plugin);
}
