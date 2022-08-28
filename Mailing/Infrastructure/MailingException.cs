namespace Mailing.Infrastructure;

public class MailingException : Exception
{
    public MailingException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
