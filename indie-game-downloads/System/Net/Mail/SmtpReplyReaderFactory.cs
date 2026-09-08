using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Mail;

internal sealed class SmtpReplyReaderFactory
{
	private enum ReadState
	{
		Status0,
		Status1,
		Status2,
		ContinueFlag,
		ContinueCR,
		ContinueLF,
		LastCR,
		LastLF,
		Done
	}

	private readonly BufferedReadStream _bufferedStream;

	private byte[] _byteBuffer;

	private SmtpReplyReader _currentReader;

	private ReadState _readState;

	private SmtpStatusCode _statusCode;

	internal SmtpStatusCode StatusCode => _statusCode;

	internal SmtpReplyReaderFactory(Stream stream)
	{
		_bufferedStream = new BufferedReadStream(stream);
	}

	internal void Close(SmtpReplyReader caller)
	{
		if (_currentReader != caller)
		{
			return;
		}
		if (_readState != ReadState.Done)
		{
			if (_byteBuffer == null)
			{
				_byteBuffer = new byte[256];
			}
			while (Read(caller, _byteBuffer) != 0)
			{
			}
		}
		_currentReader = null;
	}

	internal SmtpReplyReader GetNextReplyReader()
	{
		_currentReader?.Close();
		_readState = ReadState.Status0;
		_currentReader = new SmtpReplyReader(this);
		return _currentReader;
	}

	private unsafe int ProcessRead(ReadOnlySpan<byte> buffer, bool readLine)
	{
		if (buffer.Length == 0)
		{
			throw new IOException(System.SR.Format(System.SR.net_io_readfailure, System.SR.net_io_connectionclosed));
		}
		fixed (byte* ptr = buffer)
		{
			byte* ptr2 = ptr;
			byte* ptr3 = ptr2 + buffer.Length;
			switch (_readState)
			{
			case ReadState.Status0:
				if (ptr2 < ptr3)
				{
					byte b = *(ptr2++);
					if (b < 48 && b > 57)
					{
						throw new FormatException(System.SR.SmtpInvalidResponse);
					}
					_statusCode = (SmtpStatusCode)(100 * (b - 48));
					goto case ReadState.Status1;
				}
				_readState = ReadState.Status0;
				break;
			case ReadState.Status1:
				if (ptr2 < ptr3)
				{
					byte b3 = *(ptr2++);
					if (b3 < 48 && b3 > 57)
					{
						throw new FormatException(System.SR.SmtpInvalidResponse);
					}
					_statusCode += 10 * (b3 - 48);
					goto case ReadState.Status2;
				}
				_readState = ReadState.Status1;
				break;
			case ReadState.Status2:
				if (ptr2 < ptr3)
				{
					byte b2 = *(ptr2++);
					if (b2 < 48 && b2 > 57)
					{
						throw new FormatException(System.SR.SmtpInvalidResponse);
					}
					_statusCode += b2 - 48;
					goto case ReadState.ContinueFlag;
				}
				_readState = ReadState.Status2;
				break;
			case ReadState.ContinueFlag:
				if (ptr2 < ptr3)
				{
					byte b4 = *(ptr2++);
					if (b4 != 32)
					{
						if (b4 != 45)
						{
							throw new FormatException(System.SR.SmtpInvalidResponse);
						}
						goto case ReadState.ContinueCR;
					}
					goto case ReadState.LastCR;
				}
				_readState = ReadState.ContinueFlag;
				break;
			case ReadState.ContinueCR:
				while (ptr2 < ptr3)
				{
					if (*(ptr2++) != 13)
					{
						continue;
					}
					goto case ReadState.ContinueLF;
				}
				_readState = ReadState.ContinueCR;
				break;
			case ReadState.ContinueLF:
				if (ptr2 < ptr3)
				{
					if (*(ptr2++) != 10)
					{
						throw new FormatException(System.SR.SmtpInvalidResponse);
					}
					if (readLine)
					{
						_readState = ReadState.Status0;
						return (int)(ptr2 - ptr);
					}
					goto case ReadState.Status0;
				}
				_readState = ReadState.ContinueLF;
				break;
			case ReadState.LastCR:
				while (ptr2 < ptr3)
				{
					if (*(ptr2++) != 13)
					{
						continue;
					}
					goto case ReadState.LastLF;
				}
				_readState = ReadState.LastCR;
				break;
			case ReadState.LastLF:
				if (ptr2 < ptr3)
				{
					if (*(ptr2++) != 10)
					{
						throw new FormatException(System.SR.SmtpInvalidResponse);
					}
					goto case ReadState.Done;
				}
				_readState = ReadState.LastLF;
				break;
			case ReadState.Done:
			{
				int result = (int)(ptr2 - ptr);
				_readState = ReadState.Done;
				return result;
			}
			}
			return (int)(ptr2 - ptr);
		}
	}

	internal int Read(SmtpReplyReader caller, Span<byte> buffer)
	{
		if (buffer.Length == 0 || _currentReader != caller || _readState == ReadState.Done)
		{
			return 0;
		}
		int num = _bufferedStream.Read(buffer);
		int num2 = ProcessRead(buffer.Slice(0, num), readLine: false);
		if (num2 < num)
		{
			_bufferedStream.Push(buffer.Slice(num2, num - num2));
		}
		return num2;
	}

	internal async Task<LineInfo[]> ReadLinesAsync<TIOAdapter>(SmtpReplyReader caller, bool oneLine = false, CancellationToken cancellationToken = default(CancellationToken)) where TIOAdapter : System.Net.IReadWriteAdapter
	{
		if (caller != _currentReader || _readState == ReadState.Done)
		{
			return Array.Empty<LineInfo>();
		}
		if (_byteBuffer == null)
		{
			_byteBuffer = new byte[256];
		}
		StringBuilder builder = new StringBuilder();
		List<LineInfo> lines = new List<LineInfo>();
		int statusRead = 0;
		int start = 0;
		int num = 0;
		while (true)
		{
			if (start == num)
			{
				start = 0;
				num = await TIOAdapter.ReadAsync(_bufferedStream, _byteBuffer, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				if (num == 0)
				{
					throw new IOException(System.SR.Format(System.SR.net_io_readfailure, System.SR.net_io_connectionclosed));
				}
			}
			int num2 = ProcessRead(_byteBuffer.AsSpan(start, num - start), readLine: true);
			if (statusRead < 4)
			{
				int num3 = Math.Min(4 - statusRead, num2);
				statusRead += num3;
				start += num3;
				num2 -= num3;
				if (num2 == 0)
				{
					continue;
				}
			}
			builder.Append(Encoding.UTF8.GetString(_byteBuffer, start, num2));
			start += num2;
			if (_readState == ReadState.Status0)
			{
				statusRead = 0;
				lines.Add(new LineInfo(_statusCode, builder.ToString(0, builder.Length - 2)));
				if (oneLine)
				{
					_bufferedStream.Push(_byteBuffer.AsSpan(start, num - start));
					return lines.ToArray();
				}
				builder.Clear();
			}
			else if (_readState == ReadState.Done)
			{
				break;
			}
		}
		lines.Add(new LineInfo(_statusCode, builder.ToString(0, builder.Length - 2)));
		_bufferedStream.Push(_byteBuffer.AsSpan(start, num - start));
		return lines.ToArray();
	}

	internal async Task<LineInfo> ReadLineAsync<TIOAdapter>(SmtpReplyReader caller, CancellationToken cancellationToken) where TIOAdapter : System.Net.IReadWriteAdapter
	{
		LineInfo[] array = await ReadLinesAsync<TIOAdapter>(caller, oneLine: true, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		return (array.Length != 0) ? array[0] : default(LineInfo);
	}
}
