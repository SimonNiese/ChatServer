namespace Core.Model;

public record UserMessage()
{
    public string Message { get; }
    public string Sender { get; }
    public string Receiver { get; }
    public string ReceiverPublicKey { get; }
}