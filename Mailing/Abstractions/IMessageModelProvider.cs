using Mailing.Models;

namespace Mailing.Abstractions;

public interface IMessageModelProvider
{
    Task<IMessageModel> GetModelAsync(Recipient recipient);
}
