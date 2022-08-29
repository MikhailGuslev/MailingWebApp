using Mailing.Abstractions;
using Mailing.Models;
using OpenReceivingMeterReadingsPeriodModelProvider.Models;

namespace OpenReceivingMeterReadingsPeriodModelProvider;

public class FakeMessageModelProvider : IMessageModelProvider
{
    private readonly List<(string ProviderName, string ServiceName)> FakeProvidersAndServices = new()
    {
        ("ООО Зима близко", "Горячее водоснабжение"),
        ("ООО Энергия", "Электроэнергия"),
        ("ООО МечтыСбываются", "Газ"),
    };

    public async Task<IMessageModel> GetModelAsync(Recipient recipient)
    {
        await Task.CompletedTask;

        int detailsCount = FakeProvidersAndServices.Count;

        MeterReadingsPeriodDetails[] fakeDetails = Enumerable.Range(0, detailsCount)
            .Select(i => new MeterReadingsPeriodDetails
            {
                ProvidedServiceName = FakeProvidersAndServices[i].ProviderName,
                ServiceProviderName = FakeProvidersAndServices[i].ServiceName,
                MeteringDevice = GetFakeDevice(recipient, i),
                StartTakingReadings = DateTime.Now,
                EndTakingReadings = DateTime.Now.AddDays(3).AddHours(3)
            })
            .ToArray();

        return new MessageModel(fakeDetails);
    }

    private string GetFakeDevice(Recipient user, int index)
    {
        return "dev_"
            + string.Concat(Enumerable.Repeat(index, 2))
            + "_"
            + string.Concat(Enumerable.Repeat(user.UserId, 4));
    }
}