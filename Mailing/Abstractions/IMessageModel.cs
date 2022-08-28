namespace Mailing.Abstractions;

public interface IMessageModel
{
    IMessageSubjectModel? SubjectModel { get; }
    IMessageBodyModel? BodyModel { get; }
}