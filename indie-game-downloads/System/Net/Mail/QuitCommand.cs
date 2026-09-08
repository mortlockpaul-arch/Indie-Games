using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Mail;

internal static class QuitCommand
{
	internal static async Task SendAsync<TIOAdapter>(SmtpConnection conn, CancellationToken cancellationToken = default(CancellationToken)) where TIOAdapter : System.Net.IReadWriteAdapter
	{
		PrepareCommand(conn);
		await conn.FlushAsync<TIOAdapter>(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	private static void PrepareCommand(SmtpConnection conn)
	{
		if (conn.IsStreamOpen)
		{
			throw new InvalidOperationException(System.SR.SmtpDataStreamOpen);
		}
		conn.BufferBuilder.Append(SmtpCommands.Quit);
	}
}
