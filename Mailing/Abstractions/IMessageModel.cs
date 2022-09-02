using MimeKit;

namespace Mailing.Abstractions;

public interface IMessageModel
{
    IMessageSubjectModel? SubjectModel { get; }
    IMessageBodyModel? BodyModel { get; }

    // NOTE: стоит подумать над собственной моделью представления вложений
    IReadOnlyList<MimeEntity> Attachments { get; init; }
}