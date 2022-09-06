using PluginManager.Models;

namespace PluginManager.Infrastructure;

public static class PluginAssemblyExtensions
{
    public static PluginAssemblyInformation GetInformation(this PluginAssembly pluginAssembly)
    {
        ArgumentNullException.ThrowIfNull(pluginAssembly, nameof(pluginAssembly));

        return new PluginAssemblyInformation
        {
            PluginId = pluginAssembly.PluginAssemblyId,
            Name = pluginAssembly.Name,
            Comment = pluginAssembly.Comment,
            Settings = pluginAssembly.Settings,
            CreatedDate = pluginAssembly.CreatedDate,
            UpdatedDate = pluginAssembly.UpdatedDate,
        };
    }
}
