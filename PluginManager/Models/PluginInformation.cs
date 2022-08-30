namespace PluginManager.Models;

public sealed record class PluginInformation
{
    public int PluginId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Comment { get; init; } = string.Empty;
    public DateTime CreatedDate { get; init; }
    public DateTime UpdatedDate { get; init; }
}
