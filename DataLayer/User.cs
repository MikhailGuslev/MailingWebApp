namespace DataLayer;

public sealed record class User
{
    public int UserId { get; init; }
    public string Email { get; init; } = string.Empty;
}