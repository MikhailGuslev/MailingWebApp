using DataLayer;
using Mailing.Abstractions;
using Mailing.Models;
using Scriban;

namespace Mailing.Infrastructure;

public sealed class MessageFactory
{
    private CompiledMessageTemplate? CompiledTemplate = null;

    public MessageFactory(MessageTemplate messageTemplate)
    {
        MessageTemplate = messageTemplate;
    }

    public MessageTemplate MessageTemplate { get; init; }

    public async Task<Message> CreateMessageAsync(User recipient)
    {
        if (CompiledTemplate is null)
        {
            CompileMessageTemplate();
        }

        IMessageModel? model = MessageTemplate.ModelProvider is null
            ? null
            : await MessageTemplate.ModelProvider.GetModelAsync(recipient);

        return new Message
        {
            Subject = await GetSubject(model),
            Body = await GetBody(model)
        };
    }

    private void CompileMessageTemplate()
    {
        CompiledTemplate = new()
        {
            Subject = MessageTemplate.IsSubjectStatic
                ? null
                : Template.Parse(MessageTemplate.Subject),
            Body = MessageTemplate.IsBodyStatic
                ? null
                : Template.Parse(MessageTemplate.Body)
        };
    }

    private async Task<string> GetSubject(IMessageModel? model)
    {
        if (MessageTemplate.IsSubjectStatic)
        {
            return MessageTemplate.Subject;
        }

        if (CompiledTemplate?.Subject is null)
        {
            string error = "Невозможно создать сообщение. Тема шаблона сообщения равна null.";
            throw new MailingException(error);
        }

        if (model is null)
        {
            string error = "Невозможно создать сообщение. Отсутствуют данные для подстановки в шаблон.";
            throw new MailingException(error);
        }

        return await CompiledTemplate.Subject.RenderAsync(model.SubjectModel);
    }

    private async Task<string> GetBody(IMessageModel? model)
    {
        if (MessageTemplate.IsBodyStatic)
        {
            return MessageTemplate.Body;
        }

        if (CompiledTemplate?.Body is null)
        {
            string error = "Невозможно создать сообщение. Тело шаблона сообщения равна null.";
            throw new MailingException(error);
        }

        if (model is null)
        {
            string error = "Невозможно создать сообщение. Отсутствуют данные для подстановки в шаблон.";
            throw new MailingException(error);
        }

        return await CompiledTemplate.Body.RenderAsync(model.BodyModel);
    }
}
