using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PluginManager.Abstractions;
using PluginManager.Models;
using System.Reflection;
using System.Runtime.Loader;

namespace PluginManager;

// NOTE: draft version !!!
public sealed class PluginService : IPluginService
{
    // TODO: заменить ServiceProvider на IServiceScopeFactory !
    private readonly IServiceProvider ServiceProvider;
    private readonly ILogger<PluginService> Logger;
    private readonly AssemblyLoadContext AssemblyLoadContext;

    public PluginService(
        ILogger<PluginService> logger,
        IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        Logger = logger;
        AssemblyLoadContext = new("plugins", true);
    }

    // TODO: реализовать передачу аргументов в конструктор экземпляра
    public async Task<object?> GetInstanceAsync(InstanceCreationOptions options)
    {
        using IServiceScope scope = ServiceProvider.CreateScope();
        IPluginRepository pluginRepository = scope.ServiceProvider
            .GetRequiredService<IPluginRepository>();

        string info = "Запрос экземпляра типа {type} из плагина {pluginId}";
        Logger.LogInformation(info, options.TypeName, options.PluginId);

        PluginInformation? pluginInfo = await GetPlugin(options.PluginId, pluginRepository);
        Assembly? assembly = pluginInfo is not null
            ? await GetAssembly(pluginInfo, pluginRepository)
            : null;
        Type? type = assembly is not null ? GetType(options.TypeName, assembly, options.InterfaceType) : null;
        object? instance = type is not null ? CreateInstance(type) : null;

        info = "Экземпляр типа {type} из плагина {pluginId}" +
            instance is not null ? " извлечен " : " не извлечен ";
        Logger.LogInformation(info, options.TypeName, options.PluginId);

        return instance;
    }

    public async Task AddPluginAsync(Plugin plugin)
    {
        throw new NotImplementedException();
    }

    public async Task UpdatePluginAsync(Plugin plugin)
    {
        throw new NotImplementedException();
    }


    private async Task<PluginInformation?> GetPlugin(int pluginId, IPluginRepository pluginRepository)
    {
        // TODO: реализовать кэширование данных загружаемых плагинов (в словаре)

        PluginInformation? pluginInfo = await pluginRepository
            .GetPluginInformationAsync(pluginId);

        if (pluginInfo is null)
        {
            string error = "Не удалось извлечь данные плагина {pluginId}";
            Logger.LogError(error, pluginId);
            return null;
        }

        string info = "Данные плагина {pluginId} загружены из хранилища";
        Logger.LogInformation(info, pluginId);

        return pluginInfo;
    }

    private async Task<Assembly?> GetAssembly(
        PluginInformation pluginInfo,
        IPluginRepository pluginRepository)
    {
        string info = string.Empty;

        Assembly? assembly = AssemblyLoadContext.Assemblies
            .FirstOrDefault(x => x.FullName == pluginInfo.Name);

        if (assembly != null)
        {
            info = "Cборка плагина {pluginId} извлечена из кэша";
            Logger.LogInformation(info, pluginInfo.PluginId);

            return assembly;
        }

        Plugin? plugin = await pluginRepository
                .GetPluginAsync(pluginInfo.PluginId);
        if (plugin is null)
        {
            string error = "(!?) Проблемный плагин {pluginName} - pluginInfo есть, а сам plugin равен null";
            Logger.LogError(error, pluginInfo.Name);
            return null;
        }

        try
        {
            using MemoryStream stream = new(plugin.Data);
            assembly = AssemblyLoadContext.LoadFromStream(stream);

            info = "Сборка плагина {pluginId} загружена из хранилища";
            Logger.LogInformation(info, pluginInfo.PluginId);

            return assembly;
        }
        catch (Exception exception)
        {
            string error = "В процессе загрузки плагина {pluginName} возникло исключение {exception}";
            Logger.LogError(error, pluginInfo.Name, exception);
        }

        return null;
    }

    private Type? GetType(string typeName, Assembly assembly, Type? interfaceType = null)
    {
        Type? type = assembly
            .DefinedTypes
            .Where(t => t.Name
                .Equals(typeName, StringComparison.OrdinalIgnoreCase))
            .Where(t => interfaceType == null || t.IsAssignableTo(interfaceType))
            .FirstOrDefault();

        if (type is null)
        {
            string error = "Запрашиваемый тип {type} не обнаружен";
            Logger.LogError(error, typeName);

            return null;
        }

        return type;
    }

    private object? CreateInstance(Type type)
    {
        object? instance = null;

        try
        {
            instance = Activator.CreateInstance(type);
        }
        catch (Exception exception)
        {
            string error = "Создание экземпляра {type} привело к исключению {exception}";
            Logger.LogError(error, type.Name, exception);

            return null;
        }

        if (instance is null)
        {
            string error = "Не удалось создать экземпляр {type}";
            Logger.LogError(error, type.Name);
        }

        return instance;
    }

}
