using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Mail;

internal static class CheckCommand
{
	internal static async Task<LineInfo> SendAsync<TIOAdapter>(SmtpConnection conn, CancellationToken cancellationToken = default(CancellationToken)) where TIOAdapter : System.Net.IReadWriteAdapter
	{
		await conn.FlushAsync<TIOAdapter>(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		return await conn.Reader.GetNextReplyReader().ReadLineAsync<TIOAdapter>(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}
}
