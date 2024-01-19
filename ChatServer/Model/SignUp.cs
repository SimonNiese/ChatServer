using System.ComponentModel.DataAnnotations;

namespace ChatServer.Model;

public record SignUp {
    [Required]
    public string Username { get; set; }
    [Required]
    public string PublicKey { get; set; }
}