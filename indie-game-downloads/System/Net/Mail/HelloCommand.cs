using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Mail;

internal static class HelloCommand
{
	internal static async Task SendAsync<TIOAdapter>(SmtpConnection conn, string domain, CancellationToken cancellationToken = default(CancellationToken)) where TIOAdapter : System.Net.IReadWriteAdapter
	{
		PrepareCommand(conn, domain);
		LineInfo lineInfo = await CheckCommand.SendAsync<TIOAdapter>(conn, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		CheckResponse(lineInfo.StatusCode, lineInfo.Line);
	}

	private static void CheckResponse(SmtpStatusCode statusCode, string serverResponse)
	{
		if (statusCode == SmtpStatusCode.Ok)
		{
			return;
		}
		if (statusCode < (SmtpStatusCode)400)
		{
			throw new SmtpException(System.SR.net_webstatus_ServerProtocolViolation, serverResponse);
		}
		throw new SmtpException(statusCode, serverResponse, _: true);
	}

	private static void PrepareCommand(SmtpConnection conn, string domain)
	{
		if (conn.IsStreamOpen)
		{
			throw new InvalidOperationException(System.SR.SmtpDataStreamOpen);
		}
		conn.BufferBuilder.Append(SmtpCommands.Hello);
		conn.BufferBuilder.Append(domain);
		conn.BufferBuilder.Append(SmtpCommands.CRLF);
	}
}
