using LinqToDB;
using Mailing.Abstractions;
using Mailing.Models;
using OpenReceivingMeterReadingsPeriodModelProvider.Models;
using Dal = DataLayer.Entities;

namespace OpenReceivingMeterReadingsPeriodModelProvider;

public class FakeMessageModelProvider : IMessageModelProvider
{
    public async Task<IMessageModel> GetModelAsync(Recipient recipient)
    {
        using Dal.StorageDb storage = new("Data Source=./bin/Debug/storage.db");

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