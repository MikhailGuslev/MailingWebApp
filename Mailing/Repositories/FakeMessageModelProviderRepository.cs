using DataLayer;
using Mailing.Abstractions;

namespace Mailing.Repositories;

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
        public async Task<IMessageModel> GetModelAsync(User recipient)
        {
            await Task.CompletedTask;

            return new FakeMessageModel();
        }
    }

    public IMessageModelProvider GetMessageModelProvider(Type providerType)
    {
        return new FakeMessageModelProvider();
    }

    public IMessageModelProvider GetMessageModelProvider(string providerTypeName)
    {
        return new FakeMessageModelProvider();
    }

    public void AddMessageModelProvider(IMessageModelProvider modelProvider)
    {
        throw new NotImplementedException();
    }
}
