using PluginManager.Models;

namespace PluginManager.Abstractions;

public interface IPluginRepository
{
    Task<Plugin> GetPluginAsync(int pluginId);
    Task<IReadOnlyList<Plugin>> GetAllPluginsAsync();
    Task AddPluginAsync(Plugin plugin);
}
