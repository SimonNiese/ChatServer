using System.ComponentModel.DataAnnotations;

namespace ChatServer.Model;

public record Login {
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}