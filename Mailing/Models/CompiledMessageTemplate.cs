using Scriban;

namespace Mailing.Models;

internal sealed record CompiledMessageTemplate
{
    public Template? Subject { get; init; } = null;
    public Template? Body { get; init; } = null;
}
