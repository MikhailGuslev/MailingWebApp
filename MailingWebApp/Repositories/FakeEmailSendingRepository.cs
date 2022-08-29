using Mailing.Abstractions;
using Mailing.Models;
using OpenReceivingMeterReadingsPeriodModelProvider;

namespace MailingWebApp.Repositories;

public sealed class FakeEmailSendingRepository : IEmailSendingRepository
{
    private readonly FakeMessageModelProviderRepository fakeMessageModelProviderRepository = new();

    private const int FakeSize = 10;

    private readonly List<Recipient> FakeUsers;
    private readonly List<EmailSending> FakeSendings;
    private readonly List<EmailSendingSchedule> FakeSchedules;

    public FakeEmailSendingRepository()
    {
        IEnumerable<int> faker = Enumerable.Range(1, FakeSize);

        FakeUsers = faker
            .Select(i => new Recipient
            {
                UserId = i,
                Email = $"user_{i}@gmail.com"
            })
            .ToList();

        FakeSendings = faker
            .Select(i =>
                new EmailSending
                {
                    SendingId = i,
                    MessageTemplate = (i % 2 == 0)
                        ? GetDynamicFakeTemplate(i)
                        : GetStaticFakeTemplate(i),
                    Name = $"FUN_SENDING_{i}",
                    Recipients = FakeUsers.GetRange(0, i)
                })
            .ToList();

        FakeSchedules = new()
        {
            new EmailSendingSchedule
            {
                EmailSendingScheduleId = 0,
                EmailSending = FakeSendings[7],
                ActivationTimePoint = DateTime.Now.AddDays(-1),
                DeactivationTimePoint = DateTime.Now.AddSeconds(10),
                ActivationInterval = TimeSpan.FromSeconds(2)
            }
        };
    }

    public async Task<IReadOnlyList<EmailSending>> GetSendingsAsync()
    {
        await Task.CompletedTask;
        return FakeSendings;
    }

    public async Task<IReadOnlyList<EmailSendingSchedule>> GetEmailSendingSchedulesAsync()
    {
        await Task.CompletedTask;
        return FakeSchedules;
    }

    public async Task AddEmailSendingScheduleAsync(EmailSendingSchedule schedule)
    {
        await Task.CompletedTask;
        FakeSchedules.Add(schedule);
    }

    private MessageTemplate GetStaticFakeTemplate(int fakeId)
    {
        return new MessageTemplate
        {
            MessageTemplateId = fakeId,
            Subject = $"STATIC SUBJECT_{fakeId}",
            Body = "STATIC FAKE CONTENT",
            IsBodyStatic = true,
            IsSubjectStatic = true,
            ContentType = Mailing.Enums.MessageContentType.PlainText
        };
    }

    private MessageTemplate GetDynamicFakeTemplate(int fakeId)
    {
        string fakeSubjectTemplate = @$"[FAKE] Открыт приём показаний ПУ";
        string fakeBodyTemplate =
@"{{ for item in body_model.meter_readings_period_details }}
    * Поставщик {{ item.service_provider_name }} 
        Услуга {{ item.provided_service_name }}  
        ПУ {{item.metering_device}} 
        с {{item.start_taking_readings}} по {{item.end_taking_readings}}
{{ end }}
";

        return new MessageTemplate
        {
            MessageTemplateId = fakeId,
            Subject = fakeSubjectTemplate,
            Body = fakeBodyTemplate,
            IsBodyStatic = false,
            IsSubjectStatic = false,
            ContentType = Mailing.Enums.MessageContentType.PlainText,
            ModelProvider = new FakeMessageModelProvider()
        };
    }
}
