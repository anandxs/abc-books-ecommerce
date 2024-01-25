namespace AbcBooks.Models;

public class SmtpConfig
{
    public string SenderAddress { get; set; } = null!;
    public string SenderDisplayName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string EnableSSL { get; set; } = null!;
    public string UseDefaultCredentials { get; set; } = null!;
    public bool IsBodyHTML { get; set; }
}
