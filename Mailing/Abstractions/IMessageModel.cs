using Mailing.Models;

namespace Mailing.Abstractions;

public interface IMessageModel
{
    IMessageSubjectModel? SubjectModel { get; }
    IMessageBodyModel? BodyModel { get; }
    IReadOnlyList<MessageAttachment> Attachments { get; init; }
}