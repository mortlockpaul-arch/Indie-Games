using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Mail;

internal static class StartTlsCommand
{
	internal static async Task SendAsync<TIOAdapter>(SmtpConnection conn, CancellationToken cancellationToken = default(CancellationToken)) where TIOAdapter : System.Net.IReadWriteAdapter
	{
		PrepareCommand(conn);
		LineInfo lineInfo = await CheckCommand.SendAsync<TIOAdapter>(conn, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		CheckResponse(lineInfo.StatusCode, lineInfo.Line);
	}

	private static void CheckResponse(SmtpStatusCode statusCode, string response)
	{
		if (statusCode != SmtpStatusCode.ServiceReady)
		{
			if (statusCode != SmtpStatusCode.ClientNotPermitted)
			{
			}
			if (statusCode < (SmtpStatusCode)400)
			{
				throw new SmtpException(System.SR.net_webstatus_ServerProtocolViolation, response);
			}
			throw new SmtpException(statusCode, response, _: true);
		}
	}

	private static void PrepareCommand(SmtpConnection conn)
	{
		if (conn.IsStreamOpen)
		{
			throw new InvalidOperationException(System.SR.SmtpDataStreamOpen);
		}
		conn.BufferBuilder.Append(SmtpCommands.StartTls);
		conn.BufferBuilder.Append(SmtpCommands.CRLF);
	}
}
