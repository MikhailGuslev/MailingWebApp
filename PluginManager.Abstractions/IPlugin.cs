namespace PluginManager.Abstractions;

public interface IPlugin
{
    Type? SettingsType { get; protected set; }
}
