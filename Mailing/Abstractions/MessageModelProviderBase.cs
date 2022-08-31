using Mailing.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Mailing.Abstractions;

public abstract class MessageModelProviderBase : IMessageModelProvider
{
    public MessageModelProviderBase(IServiceScopeFactory serviceScopeFactory)
    {
        ServiceScopeFactory = serviceScopeFactory;
    }

    public IServiceScopeFactory ServiceScopeFactory { get; private set; }

    public abstract Task<IMessageModel> GetModelAsync(Recipient recipient);
}
