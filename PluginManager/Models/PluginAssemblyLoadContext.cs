using PluginManager.Abstractions;
using PluginManager.Infrastructure;
using System.Reflection;
using System.Runtime.Loader;

namespace PluginManager.Models;

internal sealed class PluginAssemblyLoadContext
{
    private AssemblyLoadContext? Context;
    private PluginAssemblyInformation? PluginAssemblyInformation;
    private WeakReference<Type>? PluginTypeRef;

    public IPlugin GetPluginInstance(params object[] arguments)
    {
        arguments = arguments ?? Array.Empty<object>();

        if (PluginTypeRef is null)
        {
            string error = "Невозможно получить экземпляр плагина используя контекст в который не загружена сборка плагина.";
            throw new PluginManagerException(error);
        }

        _ = PluginTypeRef.TryGetTarget(out Type? pluginType);

        if (pluginType is null)
        {
            string error =
                "Не удалось получить тип плагина из сборки " +
                $"ID:{PluginAssemblyInformation?.PluginId} Name:{PluginAssemblyInformation?.Name}." +
                "Ссылка на тип пуста. ";
            throw new PluginManagerException(error);
        }

        object? instance = null;

        try
        {
            instance = Activator.CreateInstance(pluginType, arguments);
        }
        catch (Exception ex)
        {
            string error =
                "Не удалось получить экземпляр плагина из сборки " +
                $"ID:{PluginAssemblyInformation?.PluginId} Name:{PluginAssemblyInformation?.Name}." +
                $"Произошло исключение: {ex.Message}.";
            throw new PluginManagerException(error, ex);
        }

        IPlugin? pluginInstance = instance is null ? null : instance as IPlugin;

        if (pluginInstance is null)
        {
            string error =
                "Не удалось получить экземпляр плагина из сборки " +
                $"ID:{PluginAssemblyInformation?.PluginId} Name:{PluginAssemblyInformation?.Name}." +
                "Получено значение null. ";
            throw new PluginManagerException(error);
        }

        return pluginInstance;
    }

    public void Load(PluginAssembly pluginAssembly)
    {
        if (Context is not null)
        {
            string error =
                "Невозможно в текущий контекст загрузить сборку плагина " +
                $"ID:{pluginAssembly.PluginAssemblyId} Name:{pluginAssembly.Name}. " +
                $"Контекст уже содержит другую сборку. ";
            throw new PluginManagerException(error);
        }

        PluginAssemblyInformation = pluginAssembly.GetInformation();

        Assembly assembly = GetAssembly(pluginAssembly);
        Type pluginType = GetPluginType(assembly);

        PluginTypeRef = new WeakReference<Type>(pluginType);
    }

    private Assembly GetAssembly(PluginAssembly pluginAssembly)
    {
        string contextName = pluginAssembly.Name + "_context";
        Context = new AssemblyLoadContext(contextName, isCollectible: true);
        using MemoryStream stream = new(pluginAssembly.Data);

        try
        {
            return Context.LoadFromStream(stream);
        }
        catch (Exception ex)
        {
            string error =
                "В процессе загрузки сборки плагина произошло исключение: " + ex.Message;
            throw new PluginManagerException(error, ex);
        }
    }

    private Type GetPluginType(Assembly assembly)
    {
        Type? pluginType = assembly.ExportedTypes
            .Where(t => t.IsAssignableTo(typeof(IPlugin)))
            .FirstOrDefault();

        if (pluginType is null)
        {
            string error =
                $"Не удалось получить тип плагина реализующий {nameof(IPlugin)} из сборки " +
                $"ID:{PluginAssemblyInformation?.PluginId} Name:{PluginAssemblyInformation?.Name}";
            throw new PluginManagerException(error);
        }

        return pluginType;
    }
}
