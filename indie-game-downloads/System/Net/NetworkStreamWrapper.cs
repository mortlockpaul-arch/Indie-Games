using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net;

internal class NetworkStreamWrapper : Stream
{
	private NetworkStream _networkStream;

	private SslStream _sslStream;

	protected bool UsingSecureStream => _sslStream != null;

	internal IPAddress ServerAddress => ((IPEndPoint)Socket.RemoteEndPoint).Address;

	internal Socket Socket => _networkStream.Socket;

	internal Stream Stream
	{
		get
		{
			return (Stream)(((object)_sslStream) ?? ((object)_networkStream));
		}
		set
		{
			_sslStream = (SslStream)value;
		}
	}

	public override bool CanRead => Stream.CanRead;

	public override bool CanSeek => Stream.CanSeek;

	public override bool CanWrite => Stream.CanWrite;

	public override bool CanTimeout => Stream.CanTimeout;

	public override int ReadTimeout
	{
		get
		{
			return Stream.ReadTimeout;
		}
		set
		{
			Stream.ReadTimeout = value;
		}
	}

	public override int WriteTimeout
	{
		get
		{
			return Stream.WriteTimeout;
		}
		set
		{
			Stream.WriteTimeout = value;
		}
	}

	public override long Length => Stream.Length;

	public override long Position
	{
		get
		{
			return Stream.Position;
		}
		set
		{
			Stream.Position = value;
		}
	}

	internal NetworkStreamWrapper(NetworkStream stream)
	{
		_networkStream = stream;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		return Stream.Seek(offset, origin);
	}

	public override int Read(byte[] buffer, int offset, int size)
	{
		return Stream.Read(buffer, offset, size);
	}

	public override void Write(byte[] buffer, int offset, int size)
	{
		Stream.Write(buffer, offset, size);
	}

	protected override void Dispose(bool disposing)
	{
		try
		{
			if (disposing)
			{
				CloseSocket();
			}
		}
		finally
		{
			base.Dispose(disposing);
		}
	}

	internal void CloseSocket()
	{
		Stream.Close();
	}

	public void Close(int timeout)
	{
		_networkStream.Close(timeout);
	}

	public override IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
	{
		return Stream.BeginRead(buffer, offset, size, callback, state);
	}

	public override int EndRead(IAsyncResult asyncResult)
	{
		return Stream.EndRead(asyncResult);
	}

	public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		return Stream.ReadAsync(buffer, offset, count, cancellationToken);
	}

	public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
	{
		return Stream.ReadAsync(buffer, cancellationToken);
	}

	public override IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
	{
		return Stream.BeginWrite(buffer, offset, size, callback, state);
	}

	public override void EndWrite(IAsyncResult asyncResult)
	{
		Stream.EndWrite(asyncResult);
	}

	public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		return Stream.WriteAsync(buffer, offset, count, cancellationToken);
	}

	public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
	{
		return Stream.WriteAsync(buffer, cancellationToken);
	}

	public override void Flush()
	{
		Stream.Flush();
	}

	public override Task FlushAsync(CancellationToken cancellationToken)
	{
		return Stream.FlushAsync(cancellationToken);
	}

	public override void SetLength(long value)
	{
		Stream.SetLength(value);
	}

	internal void SetSocketTimeoutOption(int timeout)
	{
		Stream.ReadTimeout = timeout;
		Stream.WriteTimeout = timeout;
	}
}
