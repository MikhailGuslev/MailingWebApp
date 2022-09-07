namespace Mailing.Abstractions;

public interface IMessageModelProviderRepository
{
    Task<IMessageModelProvider> GetMessageModelProviderAsync(int messageModelProviderId);
    Task AddMessageModelProviderAsync(IMessageModelProvider modelProvider);
}