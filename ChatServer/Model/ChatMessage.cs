namespace ChatServer.Model;

public record ChatMessage {
    public string Sender { get; set; }
    public string Recipient { get; set; }
    public byte[] Message { get; set; }
}