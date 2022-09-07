using Mailing.Models;
using PluginManager.Abstractions;

namespace Mailing.Abstractions;

public interface IMessageModelProvider : IPlugin
{
    Task<IMessageModel> GetModelAsync(Recipient recipient);
}
