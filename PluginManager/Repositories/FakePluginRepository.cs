using PluginManager.Abstractions;
using PluginManager.Models;

namespace PluginManager.Repositories;

public sealed class FakePluginRepository : IPluginRepository
{
    private const int FakeSize = 5;
    private readonly List<Plugin> FakePlugins;

    public FakePluginRepository()
    {
        Func<byte[]> GetFakeData = () =>
        {
            byte[] FakeBytes = new byte[FakeSize];
            Random.Shared.NextBytes(FakeBytes);

            return FakeBytes;
        };

        FakePlugins = Enumerable.Range(1, FakeSize)
            .Select(i => new Plugin
            {
                PluginId = i,
                Name = $"PLUGIN_{i}",
                Comment = "FAKE PLUGIN",
                Data = GetFakeData(),
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            })
            .ToList();
    }

    public Plugin GetPlugin(int pluginId)
    {
        return FakePlugins.First(p => p.PluginId == pluginId);
    }

    public IReadOnlyList<Plugin> GetPlugins()
    {
        return FakePlugins;
    }

    public void AddPlugin(Plugin plugin)
    {
        FakePlugins.Add(plugin);
    }
}
