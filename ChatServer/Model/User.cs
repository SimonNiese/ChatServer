namespace ChatServer.Model;

public record User {
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string? Password { get; set; }
    public string ConnectionId { get; set; }
    public byte[] PublicKey { get; set; }
}