using System.ComponentModel.DataAnnotations;

namespace ChatServer.Model;

public record ChatMessage {
    [Required]
    public string Sender { get; set; }
    [Required]
    public string Recipient { get; set; }
    [Required]
    public string Message { get; set; }
    [Required]
    public string Signature { get; set; }
}