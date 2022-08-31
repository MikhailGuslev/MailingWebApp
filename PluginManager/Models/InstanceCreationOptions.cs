namespace PluginManager.Models;

public sealed record class InstanceCreationOptions
{
    public int PluginId { get; init; }
    public string PluggableTypeName { get; init; } = string.Empty;
    public object[] ConstructorArgumets { get; init; } = Array.Empty<object>();
    public Type? InterfaceType { get; init; }
}
