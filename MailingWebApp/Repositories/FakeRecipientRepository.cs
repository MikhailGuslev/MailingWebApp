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

    public async Task<IReadOnlyList<Recipient>> GetRecipientsAsync(int[] recipientIds)
    {
        IReadOnlyList<DataLayer.Entities.User> users = await UserRepository
            .GetAllUsersAsync();

        return users
            .Where(x => recipientIds.Contains((int)x.UserId))
            .Select(x => new Recipient { UserId = x.UserId, Email = x.Email })
            .ToList();
    }

    public async Task<IReadOnlyList<Recipient>> GetRecipientsByStringAsync(string recipientIdsAsString)
    {
        int[] ids = recipientIdsAsString
            .Split(Separator)
            .Select(x => int.TryParse(x, out int value) ? value : default)
            .Where(x => x != default)
            .ToArray();

        return await GetRecipientsAsync(ids);
    }
}
