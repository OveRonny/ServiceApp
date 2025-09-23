using System.ComponentModel.DataAnnotations;

namespace serviceApp.Server.Features.Emails;

public sealed class EmailOptions
{
    public string FromName { get; set; } = "Progorb";

    [Required, EmailAddress]
    public string FromEmail { get; set; } = default!;

    [Required]
    public string Host { get; set; } = default!;

    [Range(1, 65535)]
    public int Port { get; set; } = 587;

    public string User { get; set; } = default!;
    public string Password { get; set; } = default!;
    public bool UseStartTls { get; set; } = true;

    // Timeouts in seconds (defaults reasonable for SMTP)
    [Range(1, 300)]
    public int ConnectTimeoutSeconds { get; set; } = 15;

    [Range(1, 300)]
    public int OperationTimeoutSeconds { get; set; } = 30;
}
