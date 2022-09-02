using LinqToDB;
using Mailing.Abstractions;
using Mailing.Models;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using OpenReceivingMeterReadingsPeriodModelProvider.Models;
using Dal = DataLayer.Entities;

namespace OpenReceivingMeterReadingsPeriodModelProvider;

public sealed class FakeMessageModelProvider : MessageModelProviderBase
{
    public FakeMessageModelProvider(IServiceScopeFactory serviceScopeFactory)
        : base(serviceScopeFactory)
    {
    }

    public override async Task<IMessageModel> GetModelAsync(Recipient recipient)
    {
        string pictureFile = @"D:/Downloads/k9vu6lha38291.png";
        FileStream picture = File.OpenRead(pictureFile);

        MimePart attachment = new("image", "png")
        {
            Content = new MimeContent(picture, ContentEncoding.Default),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = Path.GetFileName(pictureFile)
        };

        using IServiceScope scope = ServiceScopeFactory.CreateScope();
        Dal.StorageDb storage = scope.ServiceProvider
            .GetRequiredService<Dal.StorageDb>();

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

        return new MessageModel(fakeDetails, attachment);
    }
}