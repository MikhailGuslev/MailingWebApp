namespace Mailing.Abstractions;

public interface IMessageModelProviderRepository
{
    Task<IMessageModelProvider?> GetMessageModelProviderAsync(Type providerType);
    Task<IMessageModelProvider?> GetMessageModelProviderAsync(string providerTypeName);

    Task AddMessageModelProviderAsync(IMessageModelProvider modelProvider);
}