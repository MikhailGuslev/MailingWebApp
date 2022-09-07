namespace PluginManager.Models;

public sealed record class PluginAssemblyInformation
{
    public int PluginAssemblyId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Comment { get; init; } = string.Empty;
    public string Settings { get; init; } = string.Empty;
    public DateTime CreatedDate { get; init; }
    public DateTime UpdatedDate { get; init; }
}
