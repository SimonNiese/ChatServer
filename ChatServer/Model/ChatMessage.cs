using System.ComponentModel.DataAnnotations;

namespace ChatServer.Model;

public record ChatMessage {
    [Required]
    public string Sender { get; set; }
    [Required]
    public string Recipient { get; set; }
    [Required]
    public byte[] Message { get; set; }
}