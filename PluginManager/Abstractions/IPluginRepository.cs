using PluginManager.Models;

namespace PluginManager.Abstractions;

public interface IPluginRepository
{
    Plugin GetPlugin(int pluginId);
    IReadOnlyList<Plugin> GetPlugins();
    void AddPlugin(Plugin plugin);
}
