using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Mail;

internal static class RecipientCommand
{
	internal static async Task<(bool success, string response)> SendAsync<TIOAdapter>(SmtpConnection conn, string to, CancellationToken cancellationToken = default(CancellationToken)) where TIOAdapter : System.Net.IReadWriteAdapter
	{
		PrepareCommand(conn, to);
		LineInfo lineInfo = await CheckCommand.SendAsync<TIOAdapter>(conn, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		return (success: CheckResponse(lineInfo.StatusCode, lineInfo.Line), response: lineInfo.Line);
	}

	private static bool CheckResponse(SmtpStatusCode statusCode, string response)
	{
		switch (statusCode)
		{
		case SmtpStatusCode.Ok:
		case SmtpStatusCode.UserNotLocalWillForward:
			return true;
		case SmtpStatusCode.MailboxBusy:
		case SmtpStatusCode.InsufficientStorage:
		case SmtpStatusCode.MailboxUnavailable:
		case SmtpStatusCode.UserNotLocalTryAlternatePath:
		case SmtpStatusCode.ExceededStorageAllocation:
		case SmtpStatusCode.MailboxNameNotAllowed:
			return false;
		default:
			if (statusCode < (SmtpStatusCode)400)
			{
				throw new SmtpException(System.SR.net_webstatus_ServerProtocolViolation, response);
			}
			throw new SmtpException(statusCode, response, _: true);
		}
	}

	private static void PrepareCommand(SmtpConnection conn, string to)
	{
		if (conn.IsStreamOpen)
		{
			throw new InvalidOperationException(System.SR.SmtpDataStreamOpen);
		}
		conn.BufferBuilder.Append(SmtpCommands.Recipient);
		conn.BufferBuilder.Append(to, allowUnicode: true);
		conn.BufferBuilder.Append(SmtpCommands.CRLF);
	}
}
