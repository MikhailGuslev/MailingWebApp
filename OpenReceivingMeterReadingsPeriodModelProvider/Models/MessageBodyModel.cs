using Mailing.Abstractions;

namespace OpenReceivingMeterReadingsPeriodModelProvider.Models;

public class MessageBodyModel : IMessageBodyModel
{
    public MessageBodyModel()
    {
        MeterReadingsPeriodDetails = Array.Empty<MeterReadingsPeriodDetails>();
    }

    public MeterReadingsPeriodDetails[] MeterReadingsPeriodDetails { get; init; }
}
