using LinqToDB;
using LinqToDB.Configuration;
using Mailing.Abstractions;
using Mailing.Models;
using OpenReceivingMeterReadingsPeriodModelProvider.Models;
using Dal = DataLayer.Entities;

namespace OpenReceivingMeterReadingsPeriodModelProvider;

public class FakeMessageModelProvider : IMessageModelProvider
{
    private readonly LinqToDBConnectionOptions<Dal.StorageDb> ConnectionOptions;

    public FakeMessageModelProvider()
    {
        ConnectionOptions = new LinqToDBConnectionOptionsBuilder()
            .UseSQLite("Data Source=./bin/Debug/storage.db")
            .Build<Dal.StorageDb>();
    }

    public async Task<IMessageModel> GetModelAsync(Recipient recipient)
    {
        using Dal.StorageDb storage = new(ConnectionOptions);

        MeterReadingsPeriodDetails[] fakeDetails = await storage.MeterReadingsPeriodDetails
            .Where(x => x.UserId == recipient.UserId)
            .Select(x => new MeterReadingsPeriodDetails
            {
                ProvidedServiceName = x.ProvidedServiceName,
                ServiceProviderName = x.ServiceProviderName,
                MeteringDevice = x.MeteringDevice,
                StartTakingReadings = x.StartTakingReadings,
                EndTakingReadings = x.EndTakingReadings
            })
            .ToArrayAsync();

        return new MessageModel(fakeDetails);
    }
}