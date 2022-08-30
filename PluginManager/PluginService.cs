using Mailing.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PluginManager.Abstractions;
using PluginManager.Models;
using System.Reflection;
using System.Runtime.Loader;

namespace PluginManager;

public sealed class PluginService // NOTE: draft version !!!
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
	public async Task<(bool Success, IMessageModelProvider? Instance)> TryGetMessageModelProviderInstance(string typeName, int pluginId)
	{
		string info = "Запуск обработки запроса на получение экземпляра типа {type} из плагина {pluginId}";
		Logger.LogInformation(info, typeName, pluginId);

		using IServiceScope scope = ServiceProvider.CreateScope();
		IPluginRepository pluginRepository = scope.ServiceProvider
			.GetRequiredService<IPluginRepository>();

		Plugin plugin = await pluginRepository.GetPluginAsync(pluginId);
		(bool success, Assembly? assembly) = TryGetAssembly(plugin);
		if (success is false || assembly is null)
		{
			return (false, null);
		}

		Type? type = assembly
			.DefinedTypes
			.Where(t => t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase))
			.Where(t => t.IsAssignableTo(typeof(IMessageModelProvider)))
			.FirstOrDefault();
		if (type is null)
		{
			info = "Запрашиваемый тип {type} из плагина {pluginId} не обнаружен";
			Logger.LogInformation(info, typeName, pluginId);

			return (false, null);
		}

		bool isMessageModelProvider = type.IsAssignableTo(typeof(IMessageModelProvider));
		if (isMessageModelProvider is false)
		{
			info = "Запрашиваемый тип {type} из плагина {pluginId} не реализует интерфейс IMessageModelProvider";
			Logger.LogInformation(info, typeName, pluginId);

			return (false, null);
		}

		IMessageModelProvider? instance = null;
		try
		{
			instance = Activator.CreateInstance(type) as IMessageModelProvider;
		}
		catch (Exception exception)
		{
			info = "Извлечение типа {type} из плагина {pluginId} привело к исключению {exception}";
			Logger.LogInformation(info, typeName, pluginId, exception);

			return (false, null);
		}

		if (instance is null)
		{
			info = "Не удалось создать экземпляр типа {type} из плагина {pluginId}";
			Logger.LogInformation(info, typeName, pluginId);

			return (false, null);
		}

		info = "Запрос на получение экземпляра типа {type} из плагина {pluginId} ";
		Logger.LogInformation(info, typeName, pluginId);

		return (true, instance);
	}

	public async Task AddPluginAsync(Plugin plugin)
	{
		throw new NotImplementedException();
	}

	public async Task UpdatePluginAsync(Plugin plugin)
	{
		throw new NotImplementedException();
	}

	private (bool Success, Assembly? Assembly) TryGetAssembly(Plugin plugin)
	{
		string info = string.Empty;

		Assembly? assembly = AssemblyLoadContext.Assemblies
			.FirstOrDefault(x => x.FullName == plugin.Name);

		if (assembly != null)
		{
			info = "Cборка плагина {pluginId} извлечена из кэша";
			Logger.LogInformation(info, plugin.PluginId);

			return (true, assembly);
		}

		info = "Не удалось извлечь сборку плагина {pluginId} из кэша. "
			+ "Начало загрузки сборки плагина из хранилища.";
		Logger.LogInformation(info, plugin.PluginId);
		using MemoryStream stream = new(plugin.Data);
		try
		{
			assembly = AssemblyLoadContext.LoadFromStream(stream);

			info = "Загрузка сборки плагина {pluginId} завершена успешно";
			Logger.LogInformation(info, plugin.PluginId);

			return (true, assembly);
		}
		catch (Exception exception)
		{
			string error = "В процессе загрузки плагина {pluginName} возникло исключение {exception}";
			Logger.LogError(error, plugin.Name, exception);
		}

		return (false, null);
	}

}
