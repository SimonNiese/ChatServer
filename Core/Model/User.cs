namespace Core.Model;

public record User
{
    public string UserName { get; }
    public string HashedPassword { get; }
    public string Salt { get; }
    public string PublicKey { get; }
}