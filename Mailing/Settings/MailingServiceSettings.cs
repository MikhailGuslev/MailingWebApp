namespace Mailing.Settings;

public sealed record class MailingServiceSettings
{
    public string SenderEmail { get; init; } = string.Empty;
    public string SenderEmailPassword { get; init; } = string.Empty;
    public string SenderServer { get; init; } = string.Empty;
    public int SenderServerPort { get; init; }
}
