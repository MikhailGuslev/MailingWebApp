using DataLayer.Entities;
using LinqToDB;

namespace MailingWebApp.Repositories;

public sealed class FakeUserRepository
{
    private readonly StorageDb Storage;

    public FakeUserRepository(StorageDb storage)
    {
        Storage = storage;
    }

    public async Task<IReadOnlyList<User>> GetAllUsersAsync()
    {
        return await Storage.User.ToListAsync();
    }
}
