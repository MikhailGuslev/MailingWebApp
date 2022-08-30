using DataLayer.Entities;
using LinqToDB;

namespace MailingWebApp.Repositories;

public sealed class FakeMessageTemplateRepository
{
	private readonly StorageDb Storage;

	public FakeMessageTemplateRepository(StorageDb storage)
	{
		Storage = storage;
	}

	public async Task<IReadOnlyList<MessageTemplate>> GetAllMessageTemplates()
	{
		return await Storage.MessageTemplate
			.ToListAsync();
	}
}
