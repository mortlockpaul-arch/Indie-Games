using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Mail;

internal static class MailCommand
{
	internal static Task SendAsync<TIOAdapter>(SmtpConnection conn, ReadOnlySpan<byte> command, MailAddress from, bool allowUnicode, CancellationToken cancellationToken = default(CancellationToken)) where TIOAdapter : System.Net.IReadWriteAdapter
	{
		PrepareCommand(conn, command, from, allowUnicode);
		return SendAndCheck(conn, cancellationToken);
		static async Task<LineInfo> SendAndCheck(SmtpConnection conn2, CancellationToken cancellationToken2)
		{
			LineInfo result = await CheckCommand.SendAsync<TIOAdapter>(conn2, cancellationToken2).ConfigureAwait(continueOnCapturedContext: false);
			CheckResponse(result.StatusCode, result.Line);
			return result;
		}
	}

	private static void CheckResponse(SmtpStatusCode statusCode, string response)
	{
		if (statusCode == SmtpStatusCode.Ok)
		{
			return;
		}
		switch (statusCode)
		{
		default:
			if (statusCode < (SmtpStatusCode)400)
			{
				throw new SmtpException(System.SR.net_webstatus_ServerProtocolViolation, response);
			}
			throw new SmtpException(statusCode, response, _: true);
		}
	}

	private static void PrepareCommand(SmtpConnection conn, ReadOnlySpan<byte> command, MailAddress from, bool allowUnicode)
	{
		if (conn.IsStreamOpen)
		{
			throw new InvalidOperationException(System.SR.SmtpDataStreamOpen);
		}
		conn.BufferBuilder.Append(command);
		string smtpAddress = from.GetSmtpAddress(allowUnicode);
		conn.BufferBuilder.Append(smtpAddress, allowUnicode);
		if (allowUnicode)
		{
			conn.BufferBuilder.Append(" BODY=8BITMIME SMTPUTF8");
		}
		conn.BufferBuilder.Append(SmtpCommands.CRLF);
	}
}
