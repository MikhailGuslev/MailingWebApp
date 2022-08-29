using Mailing.Abstractions;
using Mailing.Models;

namespace MailingWebApp.Repositories;

public sealed class FakeMessageModelProviderRepository : IMessageModelProviderRepository
{
    private sealed record class FakeMessageSubjectModel : IMessageSubjectModel
    {
        public string Value => "FFFFFFFFFFFFFF";
    }

    private sealed record class FakeMessageBodyModel : IMessageBodyModel
    {
        public List<string> Items => new() { "qwerty", "asdfg", "zxcvb" };
    }

    private sealed record class FakeMessageModel : IMessageModel
    {
        public IMessageSubjectModel? SubjectModel => new FakeMessageSubjectModel();

        public IMessageBodyModel? BodyModel => new FakeMessageBodyModel();
    }

    private sealed record class FakeMessageModelProvider : IMessageModelProvider
    {
        public async Task<IMessageModel> GetModelAsync(Recipient recipient)
        {
            await Task.CompletedTask;

            return new FakeMessageModel();
        }
    }

    public async Task<IMessageModelProvider> GetMessageModelProviderAsync(Type providerType)
    {
        await Task.CompletedTask;
        return new FakeMessageModelProvider();
    }

    public async Task<IMessageModelProvider> GetMessageModelProviderAsync(string providerTypeName)
    {
        await Task.CompletedTask;
        return new FakeMessageModelProvider();
    }

    public async Task AddMessageModelProviderAsync(IMessageModelProvider modelProvider)
    {
        throw new NotImplementedException();
    }
}
