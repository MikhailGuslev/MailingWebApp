using Mailing.Abstractions;
using MimeKit;

namespace OpenReceivingMeterReadingsPeriodModelProvider.Models;

public sealed class MessageModel : IMessageModel
{
    public MessageModel(MeterReadingsPeriodDetails[] details, MimePart attachment)
    {
        BodyModel = new MessageBodyModel
        {
            MeterReadingsPeriodDetails = details,
        };

        Attachments = new List<MimeEntity> { attachment };
    }

    public IMessageSubjectModel? SubjectModel => null;

    public IMessageBodyModel? BodyModel { get; }
    public IReadOnlyList<MimeEntity> Attachments { get; init; }
}
