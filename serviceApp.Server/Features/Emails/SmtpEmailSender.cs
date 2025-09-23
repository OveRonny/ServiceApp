using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace serviceApp.Server.Features.Emails;

public sealed class SmtpEmailSender(IOptions<EmailOptions> opts) : ISmtpEmailSender
{
    private readonly EmailOptions _o = opts.Value;

    public async Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default)
    {
        var msg = new MimeMessage();
        msg.From.Add(new MailboxAddress(_o.FromName, _o.FromEmail));
        msg.To.Add(MailboxAddress.Parse(toEmail));
        msg.Subject = subject;

        var body = new BodyBuilder
        {
            HtmlBody = htmlBody,
            TextBody = StripHtml(htmlBody)
        };
        msg.Body = body.ToMessageBody();

        using var client = new SmtpClient
        {
            Timeout = _o.OperationTimeoutSeconds * 1000
        };

        var socket = _o.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.SslOnConnect;

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(TimeSpan.FromSeconds(_o.ConnectTimeoutSeconds));
        await client.ConnectAsync(_o.Host, _o.Port, socket, cts.Token);

        if (!string.IsNullOrWhiteSpace(_o.User))
            await client.AuthenticateAsync(_o.User, _o.Password, ct);

        await client.SendAsync(msg, ct);
        await client.DisconnectAsync(true, ct);
    }

    private static string StripHtml(string html) =>
        string.IsNullOrWhiteSpace(html)
            ? string.Empty
            : System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);
}