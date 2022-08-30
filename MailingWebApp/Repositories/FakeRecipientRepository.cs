using Mailing.Models;

namespace MailingWebApp.Repositories;

public sealed class FakeRecipientRepository
{
    private const char Separator = ',';
    private readonly FakeUserRepository UserRepository;

    public FakeRecipientRepository(FakeUserRepository userRepository)
    {
        UserRepository = userRepository;
    }

    public async Task<IReadOnlyList<Recipient>> GetAllRecipientsAsync()
    {
        IReadOnlyList<DataLayer.Entities.User> users = await UserRepository
            .GetAllUsersAsync();

        return users
            .Select(x => new Recipient { UserId = x.UserId, Email = x.Email })
            .ToList();
    }
    public async Task<IReadOnlyList<Recipient>> GetRecipientsFromStringAsync(string recipientIdsAsString)
    {
        long[] ids = recipientIdsAsString
            .Split(Separator)
            .Select(x => long.TryParse(x, out long value) ? value : default)
            .Where(x => x != default)
            .ToArray();

        IReadOnlyList<DataLayer.Entities.User> users = await UserRepository
            .GetAllUsersAsync();

        return users
            .Where(x => ids.Contains(x.UserId))
            .Select(x => new Recipient { UserId = x.UserId, Email = x.Email })
            .ToList();
    }
}
