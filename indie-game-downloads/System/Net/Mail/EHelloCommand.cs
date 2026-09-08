using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Mail;

internal static class EHelloCommand
{
	internal static async Task<string[]> SendAsync<TIOAdapter>(SmtpConnection conn, string domain, CancellationToken cancellationToken = default(CancellationToken)) where TIOAdapter : System.Net.IReadWriteAdapter
	{
		PrepareCommand(conn, domain);
		return CheckResponse(await ReadLinesCommand.SendAsync<TIOAdapter>(conn, cancellationToken).ConfigureAwait(continueOnCapturedContext: false));
	}

	private static string[] CheckResponse(LineInfo[] lines)
	{
		if (lines == null || lines.Length == 0)
		{
			throw new SmtpException(System.SR.SmtpEhloResponseInvalid);
		}
		if (lines[0].StatusCode != SmtpStatusCode.Ok)
		{
			if (lines[0].StatusCode < (SmtpStatusCode)400)
			{
				throw new SmtpException(System.SR.net_webstatus_ServerProtocolViolation, lines[0].Line);
			}
			throw new SmtpException(lines[0].StatusCode, lines[0].Line, _: true);
		}
		string[] array = new string[lines.Length - 1];
		for (int i = 1; i < lines.Length; i++)
		{
			array[i - 1] = lines[i].Line;
		}
		return array;
	}

	private static void PrepareCommand(SmtpConnection conn, string domain)
	{
		if (conn.IsStreamOpen)
		{
			throw new InvalidOperationException(System.SR.SmtpDataStreamOpen);
		}
		conn.BufferBuilder.Append(SmtpCommands.EHello);
		conn.BufferBuilder.Append(domain);
		conn.BufferBuilder.Append(SmtpCommands.CRLF);
	}
}
