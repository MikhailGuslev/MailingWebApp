using PluginManager.Models;

namespace PluginManager.Abstractions;

public interface IPluginService
{
    Task<object?> GetInstanceAsync(InstanceCreationOptions options);
    Task AddPluginAsync(Plugin plugin);
    Task UpdatePluginAsync(Plugin plugin);
}
