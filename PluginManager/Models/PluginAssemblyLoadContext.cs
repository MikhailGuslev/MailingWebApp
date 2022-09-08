using PluginManager.Abstractions;
using PluginManager.Infrastructure;
using System.Reflection;
using System.Runtime.Loader;

namespace PluginManager.Models;

internal sealed class PluginAssemblyLoadContext
{
    private AssemblyLoadContext? Context;
    private PluginAssemblyInformation? PluginAssemblyInformation;
    private Type? PluginType;
    private Type? PluginSettingsType;

    public IPlugin GetPluginInstance(params object[] arguments)
    {
        arguments = arguments ?? Array.Empty<object>();

        if (PluginType is null)
        {
            string error = "Невозможно получить экземпляр плагина используя контекст в который не загружена сборка плагина.";
            throw new PluginManagerException(error);
        }

        object? instance = null;

        try
        {
            instance = Activator.CreateInstance(PluginType, arguments);
        }
        catch (Exception ex)
        {
            string error =
                "Не удалось получить экземпляр плагина из сборки " +
                $"ID:{PluginAssemblyInformation?.PluginAssemblyId} Name:{PluginAssemblyInformation?.Name}." +
                $"Произошло исключение: {ex.Message}.";
            throw new PluginManagerException(error, ex);
        }

        IPlugin? pluginInstance = instance is null ? null : instance as IPlugin;

        if (pluginInstance is null)
        {
            string error =
                "Не удалось получить экземпляр плагина из сборки " +
                $"ID:{PluginAssemblyInformation?.PluginAssemblyId} Name:{PluginAssemblyInformation?.Name}." +
                "Получено значение null. ";
            throw new PluginManagerException(error);
        }

        return pluginInstance;
    }

    public Type? GetPluginSettingsType()
    {
        if (PluginType is null)
        {
            string error = "Невозможно получить тип настроек плагина используя контекст в который не загружена сборка плагина.";
            throw new PluginManagerException(error);
        }

        return PluginSettingsType;
    }

    public IPluginSettings? GetPluginSettingsInstance()
    {
        if (PluginAssemblyInformation is null)
        {
            throw new PluginManagerException("Осутствуют данные о сборке плагина");
        }

        Type? settingsType = GetPluginSettingsType();

        if (settingsType is null)
        {
            return null;
        }

        object? instance = null;

        try
        {
            instance = Activator.CreateInstance(settingsType);
        }
        catch (Exception ex)
        {
            string error =
                $"Не удалось получить экземпляр настроек плагина {PluginAssemblyInformation.PluginAssemblyId} " +
                $"произошло исключение {ex.Message}";
            throw new PluginManagerException(error);
        }

        IPluginSettings? settingsInstance = instance is not null
            ? instance as IPluginSettings
            : null;

        if (settingsInstance is null)
        {
            string error =
                $"Не удалось получить экземпляр настроек плагина {PluginAssemblyInformation.PluginAssemblyId} " +
                $"получено значение null";
            throw new PluginManagerException(error);
        }

        return settingsInstance;
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

        string contextName = pluginAssembly.Name + "_context";
        Context = new AssemblyLoadContext(contextName, isCollectible: true);

        PluginAssemblyInformation = pluginAssembly.GetInformation();

        Assembly assembly = GetAssembly(pluginAssembly);

        PluginType = GetPluginType(assembly);
        PluginSettingsType = GetPluginSettingsType(assembly);
    }

    public void Unload(Action<PluginAssemblyInformation>? unloadedHandler = null)
    {
        if (Context is null || PluginAssemblyInformation is null)
        {
            string error = "Невозможно выгрузить контекст в который не загружена сборка плагина.";
            throw new PluginManagerException(error);
        }

        AssemblyLoadContext context = Context;
        PluginAssemblyInformation assemblyInformation = PluginAssemblyInformation;

        Context = null;
        PluginAssemblyInformation = null;
        PluginType = null;
        PluginSettingsType = null;

        if (unloadedHandler is not null)
        {
            context.Unloading += OnUnloaded;
        }

        context.Unload();

        void OnUnloaded(AssemblyLoadContext ctx)
        {
            ctx.Unloading -= OnUnloaded;
            unloadedHandler?.Invoke(assemblyInformation);
        }
    }

    private Assembly GetAssembly(PluginAssembly pluginAssembly)
    {
        using MemoryStream stream = new(pluginAssembly.Data);
        try
        {
            return Context!.LoadFromStream(stream);
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
                $"ID:{PluginAssemblyInformation?.PluginAssemblyId} Name:{PluginAssemblyInformation?.Name}";
            throw new PluginManagerException(error);
        }

        return pluginType;
    }

    private Type? GetPluginSettingsType(Assembly assembly)
    {
        Type? pluginSettingsType = assembly.ExportedTypes
            .Where(t => t.IsAssignableTo(typeof(IPluginSettings)))
            .FirstOrDefault();

        return pluginSettingsType;
    }
}
