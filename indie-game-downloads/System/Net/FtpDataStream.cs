using System.IO;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;

namespace System.Net;

internal sealed class FtpDataStream : Stream, ICloseEx
{
	private readonly FtpWebRequest _request;

	private readonly Stream _stream;

	private readonly NetworkStream _originalStream;

	private bool _writeable;

	private bool _readable;

	private bool _isFullyRead;

	private bool _closing;

	public override bool CanRead => _readable;

	public override bool CanSeek => _stream.CanSeek;

	public override bool CanWrite => _writeable;

	public override long Length => _stream.Length;

	public override long Position
	{
		get
		{
			return _stream.Position;
		}
		set
		{
			_stream.Position = value;
		}
	}

	public override bool CanTimeout => _stream.CanTimeout;

	public override int ReadTimeout
	{
		get
		{
			return _stream.ReadTimeout;
		}
		set
		{
			_stream.ReadTimeout = value;
		}
	}

	public override int WriteTimeout
	{
		get
		{
			return _stream.WriteTimeout;
		}
		set
		{
			_stream.WriteTimeout = value;
		}
	}

	internal FtpDataStream(Stream stream, NetworkStream originalStream, FtpWebRequest request, TriState writeOnly)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(this, null, ".ctor");
		}
		_readable = true;
		_writeable = true;
		switch (writeOnly)
		{
		case TriState.True:
			_readable = false;
			break;
		case TriState.False:
			_writeable = false;
			break;
		}
		_stream = stream;
		_originalStream = originalStream;
		_request = request;
	}

	protected override void Dispose(bool disposing)
	{
		try
		{
			if (disposing)
			{
				((ICloseEx)this).CloseEx(CloseExState.Normal);
			}
			else
			{
				((ICloseEx)this).CloseEx(CloseExState.Abort | CloseExState.Silent);
			}
		}
		finally
		{
			base.Dispose(disposing);
		}
	}

	void ICloseEx.CloseEx(CloseExState closeState)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(this, $"state = {closeState}", "CloseEx");
		}
		lock (this)
		{
			if (_closing)
			{
				return;
			}
			_closing = true;
			_writeable = false;
			_readable = false;
		}
		try
		{
			try
			{
				if ((closeState & CloseExState.Abort) == 0)
				{
					_originalStream.Close(-1);
				}
				else
				{
					_originalStream.Close(0);
				}
			}
			finally
			{
				_request.DataStreamClosed(closeState);
			}
		}
		catch (Exception ex)
		{
			bool flag = true;
			if (ex is WebException { Response: FtpWebResponse response } && !_isFullyRead && response.StatusCode == FtpStatusCode.ConnectionClosed)
			{
				flag = false;
			}
			if (flag && (closeState & CloseExState.Silent) == 0)
			{
				throw;
			}
		}
	}

	private void CheckError()
	{
		if (_request.Aborted)
		{
			throw ExceptionHelper.RequestAbortedException;
		}
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		CheckError();
		try
		{
			return _stream.Seek(offset, origin);
		}
		catch
		{
			CheckError();
			throw;
		}
	}

	public override int Read(byte[] buffer, int offset, int size)
	{
		CheckError();
		int num;
		try
		{
			num = _stream.Read(buffer, offset, size);
		}
		catch
		{
			CheckError();
			throw;
		}
		if (num == 0)
		{
			_isFullyRead = true;
			Close();
		}
		return num;
	}

	public override int Read(Span<byte> buffer)
	{
		CheckError();
		int num;
		try
		{
			num = _stream.Read(buffer);
		}
		catch
		{
			CheckError();
			throw;
		}
		if (num == 0)
		{
			_isFullyRead = true;
			Close();
		}
		return num;
	}

	public override void Write(byte[] buffer, int offset, int size)
	{
		CheckError();
		try
		{
			_stream.Write(buffer, offset, size);
		}
		catch
		{
			CheckError();
			throw;
		}
	}

	public override void Write(ReadOnlySpan<byte> buffer)
	{
		CheckError();
		try
		{
			_stream.Write(buffer);
		}
		catch
		{
			CheckError();
			throw;
		}
	}

	private void AsyncReadCallback(IAsyncResult ar)
	{
		LazyAsyncResult lazyAsyncResult = (LazyAsyncResult)ar.AsyncState;
		try
		{
			try
			{
				int num = _stream.EndRead(ar);
				if (num == 0)
				{
					_isFullyRead = true;
					Close();
				}
				lazyAsyncResult.InvokeCallback(num);
			}
			catch (Exception result)
			{
				if (!lazyAsyncResult.IsCompleted)
				{
					lazyAsyncResult.InvokeCallback(result);
				}
			}
		}
		catch
		{
		}
	}

	public override IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
	{
		CheckError();
		LazyAsyncResult lazyAsyncResult = new LazyAsyncResult(this, state, callback);
		try
		{
			_stream.BeginRead(buffer, offset, size, AsyncReadCallback, lazyAsyncResult);
			return lazyAsyncResult;
		}
		catch
		{
			CheckError();
			throw;
		}
	}

	public override int EndRead(IAsyncResult ar)
	{
		try
		{
			object obj = ((LazyAsyncResult)ar).InternalWaitForCompletion();
			if (obj is Exception source)
			{
				ExceptionDispatchInfo.Throw(source);
			}
			return (int)obj;
		}
		finally
		{
			CheckError();
		}
	}

	public override IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
	{
		CheckError();
		try
		{
			return _stream.BeginWrite(buffer, offset, size, callback, state);
		}
		catch
		{
			CheckError();
			throw;
		}
	}

	public override void EndWrite(IAsyncResult asyncResult)
	{
		try
		{
			_stream.EndWrite(asyncResult);
		}
		finally
		{
			CheckError();
		}
	}

	public override void Flush()
	{
		_stream.Flush();
	}

	public override void SetLength(long value)
	{
		_stream.SetLength(value);
	}

	internal void SetSocketTimeoutOption(int timeout)
	{
		_stream.ReadTimeout = timeout;
		_stream.WriteTimeout = timeout;
	}
}
