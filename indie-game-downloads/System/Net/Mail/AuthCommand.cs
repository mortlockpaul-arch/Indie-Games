using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Mail;

internal static class AuthCommand
{
	internal static async Task<LineInfo> SendAsync<TIOAdapter>(SmtpConnection conn, string type, string message, CancellationToken cancellationToken = default(CancellationToken)) where TIOAdapter : System.Net.IReadWriteAdapter
	{
		PrepareCommand(conn, type, message);
		return CheckResponse(await ReadLinesCommand.SendAsync<TIOAdapter>(conn, cancellationToken).ConfigureAwait(continueOnCapturedContext: false));
	}

	internal static async Task<LineInfo> SendAsync<TIOAdapter>(SmtpConnection conn, string message, CancellationToken cancellationToken = default(CancellationToken)) where TIOAdapter : System.Net.IReadWriteAdapter
	{
		PrepareCommand(conn, message);
		return CheckResponse(await ReadLinesCommand.SendAsync<TIOAdapter>(conn, cancellationToken).ConfigureAwait(continueOnCapturedContext: false));
	}

	private static LineInfo CheckResponse(LineInfo[] lines)
	{
		if (lines == null || lines.Length == 0)
		{
			throw new SmtpException(System.SR.SmtpAuthResponseInvalid);
		}
		return lines[0];
	}

	private static void PrepareCommand(SmtpConnection conn, string type, string message)
	{
		conn.BufferBuilder.Append(SmtpCommands.Auth);
		conn.BufferBuilder.Append(type);
		conn.BufferBuilder.Append(32);
		conn.BufferBuilder.Append(message);
		conn.BufferBuilder.Append(SmtpCommands.CRLF);
	}

	private static void PrepareCommand(SmtpConnection conn, string message)
	{
		conn.BufferBuilder.Append(message);
		conn.BufferBuilder.Append(SmtpCommands.CRLF);
	}
}
