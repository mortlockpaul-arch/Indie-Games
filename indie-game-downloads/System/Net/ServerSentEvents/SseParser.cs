using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.ServerSentEvents;

public sealed class SseParser<T>
{
	private readonly long TimeSpan_MaxValueMilliseconds = (long)TimeSpan.MaxValue.TotalMilliseconds;

	private readonly Stream _stream;

	private readonly SseItemParser<T> _itemParser;

	private int _used;

	private byte[] _lineBuffer = Array.Empty<byte>();

	private int _lineOffset;

	private int _lineLength;

	private int _newlineIndex;

	private int _lastSearchedForNewline;

	private bool _eof;

	private byte[] _dataBuffer;

	private int _dataLength;

	private bool _dataAppended;

	private string _eventType;

	private string _eventId;

	private TimeSpan? _nextReconnectionInterval;

	private static ReadOnlySpan<byte> CRLF => "\r\n"u8;

	public string LastEventId { get; private set; } = string.Empty;

	public TimeSpan ReconnectionInterval { get; private set; } = Timeout.InfiniteTimeSpan;

	private static ReadOnlySpan<byte> Utf8Bom => "\ufeff"u8;

	internal SseParser(Stream stream, SseItemParser<T> itemParser)
	{
		_stream = stream;
		_itemParser = itemParser;
	}

	public IEnumerable<SseItem<T>> Enumerate()
	{
		ThrowIfNotFirstEnumeration();
		_lineBuffer = ArrayPool<byte>.Shared.Rent(1024);
		try
		{
			while (FillLineBuffer() != 0 && _lineLength < Utf8Bom.Length)
			{
			}
			SkipBomIfPresent();
			while (true)
			{
				GetNextSearchOffsetAndLength(out var searchOffset, out var searchLength);
				_newlineIndex = ((ReadOnlySpan<byte>)_lineBuffer.AsSpan(searchOffset, searchLength)).IndexOfAny((byte)13, (byte)10);
				if (_newlineIndex >= 0)
				{
					_lastSearchedForNewline = -1;
					_newlineIndex += searchOffset;
					if (_lineBuffer[_newlineIndex] == 10 || _newlineIndex - _lineOffset + 1 < _lineLength || _eof)
					{
						if (ProcessLine(out var sseItem, out var advance))
						{
							yield return sseItem;
						}
						_lineOffset += advance;
						_lineLength -= advance;
						continue;
					}
				}
				else
				{
					_lastSearchedForNewline = _lineOffset + _lineLength;
				}
				if (!_eof)
				{
					FillLineBuffer();
					continue;
				}
				break;
			}
		}
		finally
		{
			SseParser<T> sseParser = this;
			ArrayPool<byte>.Shared.Return(sseParser._lineBuffer);
			if (sseParser._dataBuffer != null)
			{
				ArrayPool<byte>.Shared.Return(sseParser._dataBuffer);
			}
		}
	}

	public async IAsyncEnumerable<SseItem<T>> EnumerateAsync([EnumeratorCancellation] CancellationToken cancellationToken = default(CancellationToken))
	{
		ThrowIfNotFirstEnumeration();
		_lineBuffer = ArrayPool<byte>.Shared.Rent(1024);
		try
		{
			while (await FillLineBufferAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false) != 0 && _lineLength < Utf8Bom.Length)
			{
			}
			SkipBomIfPresent();
			while (true)
			{
				GetNextSearchOffsetAndLength(out var searchOffset, out var searchLength);
				_newlineIndex = ((ReadOnlySpan<byte>)_lineBuffer.AsSpan(searchOffset, searchLength)).IndexOfAny((byte)13, (byte)10);
				if (_newlineIndex >= 0)
				{
					_lastSearchedForNewline = -1;
					_newlineIndex += searchOffset;
					if (_lineBuffer[_newlineIndex] == 10 || _newlineIndex - _lineOffset + 1 < _lineLength || _eof)
					{
						if (ProcessLine(out var sseItem, out var advance))
						{
							yield return sseItem;
						}
						_lineOffset += advance;
						_lineLength -= advance;
						continue;
					}
				}
				else
				{
					_lastSearchedForNewline = searchOffset + searchLength;
				}
				if (_eof)
				{
					break;
				}
				await FillLineBufferAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(_lineBuffer);
			if (_dataBuffer != null)
			{
				ArrayPool<byte>.Shared.Return(_dataBuffer);
			}
		}
	}

	private void GetNextSearchOffsetAndLength(out int searchOffset, out int searchLength)
	{
		if (_lastSearchedForNewline > _lineOffset)
		{
			searchOffset = _lastSearchedForNewline;
			searchLength = _lineLength - (_lastSearchedForNewline - _lineOffset);
		}
		else
		{
			searchOffset = _lineOffset;
			searchLength = _lineLength;
		}
	}

	private int GetNewLineLength()
	{
		if (!((ReadOnlySpan<byte>)_lineBuffer.AsSpan(_newlineIndex, _lineLength - (_newlineIndex - _lineOffset))).StartsWith(CRLF))
		{
			return 1;
		}
		return 2;
	}

	private void ShiftOrGrowLineBufferIfNecessary()
	{
		if (_lineOffset + _lineLength != _lineBuffer.Length)
		{
			return;
		}
		if (_lineOffset != 0)
		{
			_lineBuffer.AsSpan(_lineOffset, _lineLength).CopyTo(_lineBuffer);
			if (_lastSearchedForNewline >= 0)
			{
				_lastSearchedForNewline -= _lineOffset;
			}
			_lineOffset = 0;
		}
		else if (_lineLength == _lineBuffer.Length)
		{
			GrowBuffer(ref _lineBuffer, _lineBuffer.Length * 2);
		}
	}

	private bool ProcessLine(out SseItem<T> sseItem, out int advance)
	{
		ReadOnlySpan<byte> readOnlySpan = _lineBuffer.AsSpan(_lineOffset, _newlineIndex - _lineOffset);
		if (readOnlySpan.IsEmpty)
		{
			advance = GetNewLineLength();
			if (_dataAppended)
			{
				T data = _itemParser(_eventType ?? "message", _dataBuffer.AsSpan(0, _dataLength));
				sseItem = new SseItem<T>(data, _eventType)
				{
					EventId = _eventId,
					ReconnectionInterval = _nextReconnectionInterval
				};
				_eventType = null;
				_eventId = null;
				_nextReconnectionInterval = null;
				_dataLength = 0;
				_dataAppended = false;
				return true;
			}
			sseItem = default(SseItem<T>);
			return false;
		}
		int num = readOnlySpan.IndexOf((byte)58);
		ReadOnlySpan<byte> span;
		ReadOnlySpan<byte> readOnlySpan2;
		if (num >= 0)
		{
			span = readOnlySpan.Slice(0, num);
			readOnlySpan2 = readOnlySpan.Slice(num + 1);
			if (!readOnlySpan2.IsEmpty && readOnlySpan2[0] == 32)
			{
				readOnlySpan2 = readOnlySpan2.Slice(1);
			}
		}
		else
		{
			span = readOnlySpan;
			readOnlySpan2 = default(ReadOnlySpan<byte>);
		}
		long result;
		if (span.SequenceEqual("data"u8))
		{
			if (!_dataAppended)
			{
				int newLineLength = GetNewLineLength();
				ReadOnlySpan<byte> span2 = _lineBuffer.AsSpan(_newlineIndex + newLineLength, _lineLength - readOnlySpan.Length - newLineLength);
				if (!span2.IsEmpty && (span2[0] == 10 || (span2[0] == 13 && span2.Length > 1)))
				{
					advance = readOnlySpan.Length + newLineLength + ((!span2.StartsWith(CRLF)) ? 1 : 2);
					T data2 = _itemParser(_eventType ?? "message", readOnlySpan2);
					sseItem = new SseItem<T>(data2, _eventType)
					{
						EventId = _eventId,
						ReconnectionInterval = _nextReconnectionInterval
					};
					_eventType = null;
					_eventId = null;
					_nextReconnectionInterval = null;
					return true;
				}
			}
			if (_dataBuffer == null || _dataLength + _lineLength + 1 > _dataBuffer.Length)
			{
				GrowBuffer(ref _dataBuffer, _dataLength + _lineLength + 1);
			}
			if (_dataAppended)
			{
				_dataBuffer[_dataLength++] = 10;
			}
			readOnlySpan2.CopyTo(_dataBuffer.AsSpan(_dataLength));
			_dataLength += readOnlySpan2.Length;
			_dataAppended = true;
		}
		else if (span.SequenceEqual("event"u8))
		{
			_eventType = SseParser.Utf8GetString(readOnlySpan2);
		}
		else if (span.SequenceEqual("id"u8))
		{
			if (readOnlySpan2.IndexOf((byte)0) < 0)
			{
				LastEventId = (_eventId = SseParser.Utf8GetString(readOnlySpan2));
			}
		}
		else if (span.SequenceEqual("retry"u8) && long.TryParse(readOnlySpan2, NumberStyles.None, CultureInfo.InvariantCulture, out result) && 0 <= result && result <= TimeSpan_MaxValueMilliseconds)
		{
			TimeSpan timeSpan = ((result == TimeSpan_MaxValueMilliseconds) ? TimeSpan.MaxValue : TimeSpan.FromMilliseconds(result));
			TimeSpan value = (ReconnectionInterval = timeSpan);
			_nextReconnectionInterval = value;
		}
		advance = readOnlySpan.Length + GetNewLineLength();
		sseItem = default(SseItem<T>);
		return false;
	}

	private void ThrowIfNotFirstEnumeration()
	{
		if (Interlocked.Exchange(ref _used, 1) != 0)
		{
			throw new InvalidOperationException(System.SR.InvalidOperation_EnumerateOnlyOnce);
		}
	}

	private int FillLineBuffer()
	{
		ShiftOrGrowLineBufferIfNecessary();
		int start = _lineOffset + _lineLength;
		int num = _stream.Read(_lineBuffer.AsSpan(start));
		if (num > 0)
		{
			_lineLength += num;
		}
		else
		{
			_eof = true;
			num = 0;
		}
		return num;
	}

	private async ValueTask<int> FillLineBufferAsync(CancellationToken cancellationToken)
	{
		ShiftOrGrowLineBufferIfNecessary();
		int start = _lineOffset + _lineLength;
		int num = await _stream.ReadAsync(_lineBuffer.AsMemory(start), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		if (num > 0)
		{
			_lineLength += num;
		}
		else
		{
			_eof = true;
			num = 0;
		}
		return num;
	}

	private void SkipBomIfPresent()
	{
		if (((ReadOnlySpan<byte>)_lineBuffer.AsSpan(0, _lineLength)).StartsWith(Utf8Bom))
		{
			_lineOffset += 3;
			_lineLength -= 3;
		}
	}

	private static void GrowBuffer([NotNull] ref byte[] buffer, int minimumLength)
	{
		byte[] array = buffer;
		buffer = ArrayPool<byte>.Shared.Rent(Math.Max(minimumLength, 1024));
		if (array != null)
		{
			Array.Copy(array, buffer, array.Length);
			ArrayPool<byte>.Shared.Return(array);
		}
	}
}
public static class SseParser
{
	public const string EventTypeDefault = "message";

	public static SseParser<string> Create(Stream sseStream)
	{
		return Create(sseStream, (string _, ReadOnlySpan<byte> bytes) => Utf8GetString(bytes));
	}

	public static SseParser<T> Create<T>(Stream sseStream, SseItemParser<T> itemParser)
	{
		if (sseStream == null)
		{
			ThrowHelper.ThrowArgumentNullException("sseStream");
		}
		if (itemParser == null)
		{
			ThrowHelper.ThrowArgumentNullException("itemParser");
		}
		return new SseParser<T>(sseStream, itemParser);
	}

	internal static string Utf8GetString(ReadOnlySpan<byte> bytes)
	{
		return Encoding.UTF8.GetString(bytes);
	}
}
