using Mailing.Abstractions;
using Mailing.Models;

namespace OpenReceivingMeterReadingsPeriodModelProvider.Models;

public sealed class MessageModel : IMessageModel
{
    public MessageModel(MeterReadingsPeriodDetails[] details, MessageAttachment attachment)
    {
        BodyModel = new MessageBodyModel
        {
            MeterReadingsPeriodDetails = details,
        };

        Attachments = new List<MessageAttachment> { attachment };
    }

    public IMessageSubjectModel? SubjectModel => null;

    public IMessageBodyModel? BodyModel { get; }
    public IReadOnlyList<MessageAttachment> Attachments { get; init; }
}
