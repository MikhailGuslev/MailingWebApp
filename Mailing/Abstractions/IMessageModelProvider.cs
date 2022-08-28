using DataLayer;

namespace Mailing.Abstractions;

public interface IMessageModelProvider
{
    Task<IMessageModel> GetModelAsync(User recipient);
}
