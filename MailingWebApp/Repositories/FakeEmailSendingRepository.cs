using Common.Infrastructure;
using LinqToDB;
using Mailing.Abstractions;
using Mailing.Models;
using System.Text.Json;
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
    private readonly FakeRecipientRepository FakeRecipientRepository;
    public FakeEmailSendingRepository(
        Dal.StorageDb storage,
        IMessageModelProviderRepository modelProvidersRepositry,
        FakeRecipientRepository fakeRecipientRepository)
    {
        Storage = storage;
        MessageModelProviderRepository = modelProvidersRepositry;
        FakeRecipientRepository = fakeRecipientRepository;
    }

    public async Task<IReadOnlyList<EmailSending>> GetEmailSendingsAsync()
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
            .Select(async x => await MapToEmailSending(x));

        IEnumerable<EmailSending> sendings = await Task.WhenAll(tasks);

        return sendings.ToList();
    }

    public async Task<EmailSending> GetEmailSendingByIdAsync(int id)
    {
        RawEmailSending? rawSendings = await Storage.EmailSending
            .Join(Storage.MessageTemplate,
                s => s.MessageTemplateId,
                t => t.MessageTemplateId,
                (s, t) => new { Sending = s, Template = t })
            .Join(Storage.ModelProvider,
                a => a.Template.ModelProviderId,
                b => b.ModelProviderId,
                (a, b) =>
                    new RawEmailSending(a.Sending, a.Template, b))
            .FirstAsync(x => x.Sending.SendingId == id);

        return await MapToEmailSending(rawSendings);
    }

    public async Task<IReadOnlyList<EmailSendingSchedule>> GetEmailSendingSchedulesAsync()
    {
        JsonSerializerOptions options = new();
        options.Converters.Add(new TimeOnlyConverter());

        IReadOnlyList<EmailSending> sendings = await GetEmailSendingsAsync();

        List<Dal.EmailSendingSchedule> rawSchedules = await Storage.EmailSendingSchedule
            .ToListAsync();

        List<EmailSendingSchedule> schedules = rawSchedules
        .Join(sendings,
            e => e.EmailSendingId,
            s => s.EmailSendingId,
            (e, s) => new EmailSendingSchedule(e.LastActivation)
            {
                EmailSendingScheduleId = (int)e.EmailSendingScheduleId,
                EmailSending = s,
                ActivationTimePoint = e.ActivationTimePoint,
                DeactivationTimePoint = e.DeactivationTimePoint,
                RecurrenceActivation = JsonSerializer
                    .Deserialize<Recurrence>(e.RecurrenceActivation, options)
                    ?? throw new ApplicationException("ошибка загрузки расписания рассылок"),
            })
        .ToList();

        return schedules;
    }

    public async Task AddEmailSendingAsync(EmailSending emailSending)
    {
        Dal.EmailSending entity = MapTyDalEntity(emailSending);
        await Storage.InsertAsync(entity);
    }

    public async Task AddEmailSendingScheduleAsync(EmailSendingSchedule schedule)
    {
        throw new NotImplementedException();
    }

    private Dal.EmailSending MapTyDalEntity(EmailSending emailSending)
    {
        string recipietnsAsString = emailSending.Recipients
            .Select(x => x.UserId.ToString())
            .Aggregate((a, b) => a + ',' + b);

        return new()
        {
            SendingId = emailSending.EmailSendingId,
            Name = emailSending.Name,
            Recipients = recipietnsAsString,
            MessageTemplateId = emailSending.MessageTemplate.MessageTemplateId,
        };
    }

    private async Task<EmailSending> MapToEmailSending(RawEmailSending rawData)
    {
        ArgumentNullException.ThrowIfNull(rawData, nameof(rawData));

        EmailSending emailSending = new EmailSending
        {
            EmailSendingId = (int)rawData.Sending.SendingId,
            Name = rawData.Sending.Name,
            MessageTemplate = new()
            {
                MessageTemplateId = (int)rawData.MessageTemplate.MessageTemplateId,
                Subject = rawData.MessageTemplate.Subject,
                Body = rawData.MessageTemplate.Body,
                ContentType = rawData.MessageTemplate.ContentType switch
                {
                    "PlainText" => Mailing.Enums.MessageContentType.PlainText,
                    "Html" => Mailing.Enums.MessageContentType.Html,
                    _ => throw new ArgumentException(
                        "ContentType задан неверно {type}",
                        rawData.MessageTemplate.ContentType)
                },
                IsBodyStatic = rawData.MessageTemplate.IsBodyStatic,
                IsSubjectStatic = rawData.MessageTemplate.IsSubjectStatic,
                ModelProvider = rawData.ModelProvider is null
                       ? null
                       : await MessageModelProviderRepository
                           .GetMessageModelProviderAsync(rawData.ModelProvider.ModelProviderTypeName)
            },
            Recipients = await FakeRecipientRepository
                   .GetRecipientsByStringAsync(rawData.Sending.Recipients)
        };

        return emailSending;
    }
}