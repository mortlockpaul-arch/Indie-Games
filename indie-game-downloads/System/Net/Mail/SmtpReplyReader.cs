using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Mail;

internal sealed class SmtpReplyReader : IDisposable
{
	private readonly SmtpReplyReaderFactory _reader;

	public void Dispose()
	{
		Close();
	}

	internal SmtpReplyReader(SmtpReplyReaderFactory reader)
	{
		_reader = reader;
	}

	public void Close()
	{
		_reader.Close(this);
	}

	internal Task<LineInfo[]> ReadLinesAsync<TIOAdapter>(CancellationToken cancellationToken) where TIOAdapter : System.Net.IReadWriteAdapter
	{
		return _reader.ReadLinesAsync<TIOAdapter>(this, oneLine: false, cancellationToken);
	}

	internal Task<LineInfo> ReadLineAsync<TIOAdapter>(CancellationToken cancellationToken) where TIOAdapter : System.Net.IReadWriteAdapter
	{
		return _reader.ReadLineAsync<TIOAdapter>(this, cancellationToken);
	}
}
