namespace PluginManager.Infrastructure;

public sealed class PluginManagerException : Exception
{
    public PluginManagerException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
