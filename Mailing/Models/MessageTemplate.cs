using Mailing.Abstractions;
using Mailing.Enums;

namespace Mailing.Models;

public sealed record MessageTemplate
{
    public int MessageTemplateId { get; init; }
    public string Subject { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public IMessageModelProvider? ModelProvider { get; init; } = null;
    public MessageContentType ContentType { get; init; } = MessageContentType.PlainText;
    /// <summary>
    /// Признак фиксированного содержимого тела сообщения
    /// </summary>
    public bool IsBodyStatic { get; init; } = true;
    /// <summary>
    /// Признак фиксированной темы сообщения
    /// </summary>
    public bool IsSubjectStatic { get; init; } = true;
}