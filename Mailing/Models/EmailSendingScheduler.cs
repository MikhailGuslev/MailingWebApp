using DataLayer;
using Mailing.Abstractions;
using Mailing.Infrastructure;
using Mailing.Settings;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Mailing.Models;

// TODO: реализовать на уровне приложения отправку логов
//       со специализированным уровнем (EmailInfo, EmailError ...) по email

// TODO: реализовать регистрацию моментальной одноразовой рассылки отправки простого сообщения

// TODO: реализовать логирование в методах сервсиа рассылки и его внутренних модулей
// TODO: реализовать загрузку из хранилища только не отработавших триггеров рассылок
// TODO: реализовать удаление отработавших триггеров рассылок из списка прослушиваемых

public sealed class EmailSendingScheduler
{
    private readonly ILogger Logger;
    private readonly IEmailSendingRepository EmailSendingRepository;
    private readonly MailingServiceSettings Settings;

    internal readonly ConcurrentBag<EmailSenderTrigger> ListenedTriggers;

    internal EmailSendingScheduler(
        ILogger logger,
        IEmailSendingRepository sendingRepository,
        MailingServiceSettings settings)
    {
        Logger = logger;
        EmailSendingRepository = sendingRepository;
        Settings = settings;

        ListenedTriggers = new();
    }

    public void ScheduleEmailSending(EmailSendingSchedule schedule)
    {
        // NOTE: сохранить в базу новое расписание рассылки
        EmailSendingRepository.AddEmailSendingSchedule(schedule);

        // NOTE: создать добавить в список прослушиваемых новый активатор рассылки
        EmailSenderTrigger trigger = CreateNewSendingTrigger(schedule);
        ListenedTriggers.Add(trigger);
    }

    internal async Task RestoringStoragedSendingSchedules()
    {
        if (ListenedTriggers.IsEmpty is false)
        {
            string error =
                "Ошибка восстановления списока рассылок из хранилища. " +
                "Для выполнения этой операции требуется пустой список обрабатываемых планировщиком рассылок.";
            throw new MailingException(error);
        }

        // TODO: сделать методы всех репозиториев асинхронными
        IReadOnlyList<EmailSendingSchedule> storagedSchedules = EmailSendingRepository.GetEmailSendingSchedules();

        foreach (var schedule in storagedSchedules)
        {
            // NOTE: создать добавить в список прослушиваемых новый активатор рассылки
            EmailSenderTrigger trigger = CreateNewSendingTrigger(schedule);
            ListenedTriggers.Add(trigger);
        }

        await Task.CompletedTask;
    }

    // обновить запланированную рассылку (не запущенную)
    // отменить запланированную рассылку
    // отменить запущенную рассылку

    private EmailSenderTrigger CreateNewSendingTrigger(EmailSendingSchedule schedule)
    {
        string senderName = Settings.SenderServer;
        string senderEmail = Settings.SenderEmail;
        User[] recipients = schedule.EmailSending.Recipients.ToArray();
        MessageTemplate template = schedule.EmailSending.MessageTemplate;

        EmailMessageFactory emailMessageFactory = new(senderName, senderEmail, template);
        EmailSender emailSender = new(Logger, Settings, recipients, emailMessageFactory);
        EmailSenderTrigger newTrigger = new(emailSender, schedule);

        return newTrigger;
    }
}
