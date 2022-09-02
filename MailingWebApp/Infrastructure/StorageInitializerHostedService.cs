using Common.Infrastructure;
using DataLayer.Entities;
using LinqToDB;
using LinqToDB.Data;
using System.Reflection;
using System.Text.Json;
using MailingModels = Mailing.Models;

namespace MailingWebApp.Infrastructure;

public sealed class StorageInitializerHostedService : IHostedService
{
    private readonly ILogger<StorageInitializerHostedService> Logger;
    private readonly IServiceScopeFactory ServiceScopeFactory;

    public StorageInitializerHostedService(
        ILogger<StorageInitializerHostedService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        Logger = logger;
        ServiceScopeFactory = serviceScopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("StorageInitializer запущен");
        Logger.LogInformation("Запуск инициализации хранилища");
        await StorageInitializeAsync();
        Logger.LogInformation("Инициализация хранилища завершена");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("StorageInitializer остановлен");
        return Task.CompletedTask;
    }

    private async Task StorageInitializeAsync()
    {
        using var scope = ServiceScopeFactory.CreateScope();
        StorageDb storage = scope.ServiceProvider
            .GetRequiredService<StorageDb>();

        await UserInitialize(storage);
        await MeterReadingsPeriodDetailsInitialize(storage);
        await PluginInitialize(storage);
        await ModelProviderInititalize(storage);
        await MessageTemplateInitialize(storage);
        await EmailSendingInitialize(storage);
        await EmailSendingScheduleInitialize(storage);
    }

    private async Task UserInitialize(StorageDb storage)
    {
        List<User> entities = new()
        {

            new() { UserId = 1, Email = "user_01_fake@gamil.com" },
            new() { UserId = 2, Email = "user_02_fake@gamil.com" },
            new() { UserId = 3, Email = "user_03_fake@gamil.com" },
            new() { UserId = 4, Email = "user_04_fake@gamil.com" },
            new() { UserId = 5, Email = "user_05_fake@gamil.com" },
        };

        await storage.DropTableAsync<User>().TryAsync();
        await storage.CreateTableAsync<User>().TryAsync();

        await storage.BulkCopyAsync(entities).TryAsync();
    }

    private async Task MeterReadingsPeriodDetailsInitialize(StorageDb storage)
    {
        List<MeterReadingsPeriodDetails> entities = new()
        {
            new()
            {
                MeterReadingsPeriodDetailsId = 1,
                ProvidedServiceName = "ООО Зима близко",
                ServiceProviderName = "Горячее водоснабжение",
                MeteringDevice = "qwerty",
                StartTakingReadings = DateTime.Now,
                EndTakingReadings = DateTime.Now.AddDays(3),
                UserId = 1,
            },
            new()
            {
                MeterReadingsPeriodDetailsId = 1,
                ProvidedServiceName = "ООО Энергия",
                ServiceProviderName = "Электроэнергия",
                MeteringDevice = "asdfg",
                StartTakingReadings = DateTime.Now,
                EndTakingReadings = DateTime.Now.AddDays(3),
                UserId = 1,
            },
            new()
            {
                MeterReadingsPeriodDetailsId = 1,
                ProvidedServiceName = "ООО МечтыСбываются",
                ServiceProviderName = "Газ",
                MeteringDevice = "zxcvb",
                StartTakingReadings = DateTime.Now,
                EndTakingReadings = DateTime.Now.AddDays(3),
                UserId = 1,
            },

            new()
            {
                MeterReadingsPeriodDetailsId = 1,
                ProvidedServiceName = "ООО Зима близко",
                ServiceProviderName = "Горячее водоснабжение",
                MeteringDevice = "fghfh",
                StartTakingReadings = DateTime.Now,
                EndTakingReadings = DateTime.Now.AddDays(3),
                UserId = 2,
            },
            new()
            {
                MeterReadingsPeriodDetailsId = 1,
                ProvidedServiceName = "ООО Энергия",
                ServiceProviderName = "Электроэнергия",
                MeteringDevice = "fghfgh",
                StartTakingReadings = DateTime.Now,
                EndTakingReadings = DateTime.Now.AddDays(3),
                UserId = 2,
            },

            new()
            {
                MeterReadingsPeriodDetailsId = 1,
                ProvidedServiceName = "ООО МечтыСбываются",
                ServiceProviderName = "Газ",
                MeteringDevice = "zxcvb",
                StartTakingReadings = DateTime.Now,
                EndTakingReadings = DateTime.Now.AddDays(3),
                UserId = 3,
            },

            new()
            {
                MeterReadingsPeriodDetailsId = 1,
                ProvidedServiceName = "ООО МечтыСбываются",
                ServiceProviderName = "Газ",
                MeteringDevice = "zxcvb",
                StartTakingReadings = DateTime.Now,
                EndTakingReadings = DateTime.Now.AddDays(3),
                UserId = 4,
            },

            new()
            {
                MeterReadingsPeriodDetailsId = 1,
                ProvidedServiceName = "ООО МечтыСбываются",
                ServiceProviderName = "Газ",
                MeteringDevice = "zxcvb",
                StartTakingReadings = DateTime.Now,
                EndTakingReadings = DateTime.Now.AddDays(3),
                UserId = 5,
            },

        };

        await storage.DropTableAsync<MeterReadingsPeriodDetails>().TryAsync();
        await storage.CreateTableAsync<MeterReadingsPeriodDetails>().TryAsync();

        await storage.BulkCopyAsync(entities).TryAsync();
    }

    private async Task PluginInitialize(StorageDb storage)
    {

        string? location = Assembly.GetExecutingAssembly().Location;
        string? curDir = Path.GetDirectoryName(location);

        string pluginFile = Path.Combine(
            curDir ?? string.Empty,
            "OpenReceivingMeterReadingsPeriodModelProvider.dll");

        if (File.Exists(pluginFile) is false)
        {
            return;
        }

        byte[] bytes = File.ReadAllBytes(pluginFile);

        DateTime fakeDate = DateTime.Now.AddDays(1);

        List<Plugin> entities = new()
        {
            new()
            {
                PluginId = 1,
                Name = "FakeMessageModelProvider",
                Comment = "FakeMessageModelProvider",
                Data = bytes,
                CreatedDate = fakeDate,
                UpdatedDate = fakeDate,
            },
        };

        await storage.DropTableAsync<Plugin>().TryAsync();
        await storage.CreateTableAsync<Plugin>().TryAsync();

        await storage.BulkCopyAsync(entities).TryAsync();
    }

    private async Task ModelProviderInititalize(StorageDb storage)
    {
        List<ModelProvider> entities = new()
        {
            new()
            {
                ModelProviderId = 1,
                ModelProviderTypeName = "FakeMessageModelProvider",
                PluginId = 1,
            }
        };

        await storage.DropTableAsync<ModelProvider>().TryAsync();
        await storage.CreateTableAsync<ModelProvider>().TryAsync();

        await storage.BulkCopyAsync(entities).TryAsync();
    }

    private async Task MessageTemplateInitialize(StorageDb storage)
    {
        string templateBody = @"
            <html>
            <body>
                <h3> Открыт приём показаний приборов учёта </h3>
                <table>
                    <thead>
                        <th>Поставщик</th>
                        <th>Услуга</th>
                        <th>ПУ</th>
                        <th>Начало приёма</th>
                        <th>Окончание приёма</th>
                    </thead>
                    <tbody>
                    {{ for item in body_model.meter_readings_period_details }}
                        <tr>
                            <td>{{ item.service_provider_name }}</td>
                            <td>{{ item.provided_service_name }}</td>
                            <td>{{ item.metering_device       }}</td>
                            <td>{{ item.start_taking_readings }}</td>
                            <td>{{ item.end_taking_readings   }}</td>
                        </tr>
                    {{ end }}
                    </tbody>
                </table>
            </body>
            </html>
        ";

        List<MessageTemplate> entities = new()
        {
            new()
            {
                MessageTemplateId = 1,
                Subject = "Открыт приём показаний приборов учета",
                Body = templateBody,
                ContentType = "Html",
                IsSubjectStatic = true,
                IsBodyStatic = false,
                ModelProviderId = 1,
            }
        };

        await storage.DropTableAsync<MessageTemplate>().TryAsync();
        await storage.CreateTableAsync<MessageTemplate>().TryAsync();

        await storage.BulkCopyAsync(entities).TryAsync();
    }

    private async Task EmailSendingInitialize(StorageDb storage)
    {
        List<EmailSending> entities = new()
        {
            new()
            {
                SendingId = 1,
                Name = "Открытие приёма показаний ПУ",
                Recipients = "1,2,3,4,5",
                MessageTemplateId = 1,
            }
        };

        await storage.DropTableAsync<EmailSending>().TryAsync();
        await storage.CreateTableAsync<EmailSending>().TryAsync();

        await storage.BulkCopyAsync(entities).TryAsync();
    }

    private async Task EmailSendingScheduleInitialize(StorageDb storage)
    {
        JsonSerializerOptions options = new();
        options.Converters.Add(new TimeOnlyConverter());

        MailingModels.Recurrence recurrence = new()
        {
            Time = new(10, 35),
            Month = new() { EveryMonth = true },
            DayOfMonth = new() { Special = MailingModels.RecurrenceDayOfMonthSpecial.EveryDay }
        };

        List<EmailSendingSchedule> entities = new()
        {
            new()
            {
                EmailSendingScheduleId = 1,
                ActivationTimePoint = DateTime.Now.AddMinutes(-1),
                DeactivationTimePoint = DateTime.Now.AddSeconds(5),
                RecurrenceActivation = JsonSerializer.Serialize(recurrence, options),
                EmailSendingId = 1,
            }
        };

        await storage.EmailSendingSchedule.DropAsync().TryAsync();
        await storage.CreateTableAsync<EmailSendingSchedule>().TryAsync();

        await storage.BulkCopyAsync(entities).TryAsync();
    }

}
