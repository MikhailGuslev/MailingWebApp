namespace PluginManager.Models;

public sealed record class PluginAssembly
{
    public int PluginAssemblyId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Comment { get; init; } = string.Empty;
    public string Settings { get; init; } = string.Empty;
    public byte[] Data { get; init; } = Array.Empty<byte>();
    public DateTime CreatedDate { get; init; }
    public DateTime UpdatedDate { get; init; }
}
