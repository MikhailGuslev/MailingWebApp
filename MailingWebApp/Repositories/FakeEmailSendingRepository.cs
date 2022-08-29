using LinqToDB;
using Mailing.Abstractions;
using Mailing.Models;
using Dal = DataLayer.Entities;

namespace MailingWebApp.Repositories;

public sealed class FakeEmailSendingRepository : IEmailSendingRepository
{
    private sealed record class RawEmailSending(
        Dal.EmailSending Sending,
        Dal.MessageTemplate MessageTemplate,
        Dal.ModelProvider? ModelProvider);

    private readonly Dal.StorageDb Storage;
    private readonly IMessageModelProviderRepository MessageModelProviderRepository;

    public FakeEmailSendingRepository(
        Dal.StorageDb storage,
        IMessageModelProviderRepository modelProvidersRepositry)
    {
        Storage = storage;
        MessageModelProviderRepository = modelProvidersRepositry;
    }

    public async Task<IReadOnlyList<EmailSending>> GetSendingsAsync()
    {
        IReadOnlyList<RawEmailSending> rawSendings = await Storage.EmailSending
            .Join(Storage.MessageTemplate,
                s => s.MessageTemplateId,
                t => t.MessageTemplateId,
                (s, t) => new { Sending = s, Template = t })
            .Join(Storage.ModelProvider,
                a => a.Template.ModelProviderId,
                b => b.ModelProviderId,
                (a, b) =>
                    new RawEmailSending(a.Sending, a.Template, b))
            .ToListAsync();

        // TODO: get all modelproviders

        // TODO: get all users

        IEnumerable<Task<EmailSending>> tasks = rawSendings
            .Select(async x => new EmailSending
            {
                EmailSendingId = (int)x.Sending.SendingId,
                Name = x.Sending.Name,
                MessageTemplate = new()
                {
                    MessageTemplateId = (int)x.MessageTemplate.MessageTemplateId,
                    Subject = x.MessageTemplate.Subject,
                    Body = x.MessageTemplate.Body,
                    ContentType = x.MessageTemplate.ContentType switch
                    {
                        "PlainText" => Mailing.Enums.MessageContentType.PlainText,
                        "Html" => Mailing.Enums.MessageContentType.Html,
                        _ => throw new ArgumentException(
                            "ContentType задан неверно {type}",
                            x.MessageTemplate.ContentType)
                    },
                    IsBodyStatic = x.MessageTemplate.IsBodyStatic,
                    IsSubjectStatic = x.MessageTemplate.IsSubjectStatic,
                    ModelProvider = x.ModelProvider is null
                        ? null
                        : await MessageModelProviderRepository
                            .GetMessageModelProviderAsync(x.ModelProvider.ModelProviderTypeName)
                },
                Recipients = GetRecipietsFromString(x.Sending.Recipients)
            });

        IEnumerable<EmailSending> sendings = await Task.WhenAll(tasks);

        return sendings.ToList();

        List<Recipient> GetRecipietsFromString(string recipientsAsString)
        {
            // TODO: implement
            return new List<Recipient>();
        }
    }

    public async Task<IReadOnlyList<EmailSendingSchedule>> GetEmailSendingSchedulesAsync()
    {


        IReadOnlyList<EmailSending> sendings = await GetSendingsAsync();

        List<EmailSendingSchedule> schedules = await Storage.EmailSendingSchedule
            .Join(sendings,
                e => e.EmailSendingId,
                s => s.EmailSendingId,
                (e, s) => new EmailSendingSchedule
                {
                    EmailSendingScheduleId = (int)e.EmailSendingScheduleId,
                    EmailSending = s,
                    ActivationTimePoint = e.ActivationTimePoint,
                    DeactivationTimePoint = e.DeactivationTimePoint,
                    ActivationInterval = TimeSpan.FromTicks(e.ActivationInterval)
                })
            .ToListAsync();

        return schedules;
    }

    public async Task AddEmailSendingScheduleAsync(EmailSendingSchedule schedule)
    {
        throw new NotImplementedException();
    }
}
