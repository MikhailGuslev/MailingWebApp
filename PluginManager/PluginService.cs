using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PluginManager.Abstractions;
using PluginManager.Infrastructure;
using PluginManager.Models;
using System.Collections.Concurrent;

namespace PluginManager;

// NOTE: draft version !!!
public sealed class PluginService : IPluginService
{
    private readonly IServiceScopeFactory ServiceScopeFactory;
    private readonly ILogger<PluginService> Logger;
    private readonly ConcurrentDictionary<int, PluginAssemblyLoadContext> PluginAssemblyLoadContexts = new();

    public PluginService(
        ILogger<PluginService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        Logger = logger;
        ServiceScopeFactory = serviceScopeFactory;
    }

    // NOTE: вызывать обернув в try catch
    public async Task<IPlugin> GetPluginInstanceAsync(int pluginAssemblyId, params object[] arguments)
    {
        PluginAssemblyLoadContext context = await GetPluginAssemblyLoadContextAsync(pluginAssemblyId);
        return context.GetPluginInstance(arguments);
    }

    // NOTE: вызывать обернув в try catch
    public async Task<Type?> GetPluginSettingsTypeAsync(int pluginAssemblyId)
    {
        PluginAssemblyLoadContext context = await GetPluginAssemblyLoadContextAsync(pluginAssemblyId);
        return context.GetPluginSettingsType();
    }

    public Task<IPluginSettings> GetPluginSettingsInstanceAsync(int pluginAssemblyId)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<PluginAssemblyInformation>> GetPluginAssemblyInformationsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<PluginAssemblyInformation> GetPluginAssemblyInformationAsync(int pluginAssemblyId)
    {
        throw new NotImplementedException();
    }

    public Task AddPluginAssemblyAsync(PluginAssembly plugin)
    {
        throw new NotImplementedException();
    }

    public Task UpdatePluginAssemblyAsync(PluginAssembly plugin)
    {
        throw new NotImplementedException();
    }

    private async Task<PluginAssemblyLoadContext> GetPluginAssemblyLoadContextAsync(int pluginAssemblyId)
    {
        PluginAssemblyLoadContext? context = null;

        PluginAssemblyLoadContexts.TryGetValue(pluginAssemblyId, out context);

        if (context is null)
        {
            PluginAssembly pluginAssembly = await GetPluginAssembly(pluginAssemblyId);
            context = new();
            context.Load(pluginAssembly);
        }

        bool ok = PluginAssemblyLoadContexts.TryAdd(pluginAssemblyId, context);
        if (ok is false)
        {
            // NOTE: для подстраховки перепроверить наличие контекста в списке зарегистрированных
            ok = PluginAssemblyLoadContexts.TryGetValue(pluginAssemblyId, out context);
        }

        if (ok is false || context is null)
        {
            string error =
                $"Не удалось получить контекст загрузки {nameof(PluginAssemblyLoadContext)}" +
                $"сборки плагина с ID: {pluginAssemblyId}. " +
                $"Конфликт длоступа к элементам словаря {nameof(PluginAssemblyLoadContexts)}";
            throw new PluginManagerException(error);
        }

        return context;
    }

    private async Task<PluginAssembly> GetPluginAssembly(int pluginAssemblyId)
    {
        using IServiceScope scope = ServiceScopeFactory.CreateScope();
        IPluginAssemblyRepository repository = scope.ServiceProvider
            .GetRequiredService<IPluginAssemblyRepository>();

        PluginAssembly pluginAssembly;

        try
        {
            pluginAssembly = await repository.GetPluginAssemblyAsync(pluginAssemblyId);
        }
        catch (Exception ex)
        {
            string error =
                $"Не удалось получить сборку плагина с ID:{pluginAssemblyId}. " +
                $"Возникло исключение: {ex.Message}";
            throw new PluginManagerException(error, ex);
        }

        return pluginAssembly;
    }
}
