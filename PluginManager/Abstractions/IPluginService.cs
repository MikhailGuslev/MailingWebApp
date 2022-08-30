using PluginManager.Models;

namespace PluginManager.Abstractions;

public interface IPluginService
{
    Task<object?> GetInstanceAsync(string typeName, int pluginId, Type? interfaceType);
    Task AddPluginAsync(Plugin plugin);
    Task UpdatePluginAsync(Plugin plugin);
}
