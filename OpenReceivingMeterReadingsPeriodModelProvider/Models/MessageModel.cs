using Mailing.Abstractions;

namespace OpenReceivingMeterReadingsPeriodModelProvider.Models;

public sealed class MessageModel : IMessageModel
{
    public MessageModel(MeterReadingsPeriodDetails[] details)
    {
        BodyModel = new MessageBodyModel
        {
            MeterReadingsPeriodDetails = details
        };
    }

    public IMessageSubjectModel? SubjectModel => null;

    public IMessageBodyModel? BodyModel { get; }
}
