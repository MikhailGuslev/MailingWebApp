namespace Mailing.Abstractions;

public interface IMessageModelProviderRepository
{
    IMessageModelProvider GetMessageModelProvider(Type providerType);
    IMessageModelProvider GetMessageModelProvider(string providerTypeName);

    void AddMessageModelProvider(IMessageModelProvider modelProvider);
}