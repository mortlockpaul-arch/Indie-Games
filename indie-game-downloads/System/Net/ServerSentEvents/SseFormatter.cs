using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.ServerSentEvents;

public static class SseFormatter
{
	private static readonly byte[] s_newLine = "\n"u8.ToArray();

	public static Task WriteAsync(IAsyncEnumerable<SseItem<string>> source, Stream destination, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException("source");
		}
		if (destination == null)
		{
			ThrowHelper.ThrowArgumentNullException("destination");
		}
		return WriteAsyncCore<string>(source, destination, delegate(SseItem<string> item, IBufferWriter<byte> writer)
		{
			writer.WriteUtf8String(item.Data.AsSpan());
		}, cancellationToken);
	}

	public static Task WriteAsync<T>(IAsyncEnumerable<SseItem<T>> source, Stream destination, Action<SseItem<T>, IBufferWriter<byte>> itemFormatter, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException("source");
		}
		if (destination == null)
		{
			ThrowHelper.ThrowArgumentNullException("destination");
		}
		if (itemFormatter == null)
		{
			ThrowHelper.ThrowArgumentNullException("itemFormatter");
		}
		return WriteAsyncCore(source, destination, itemFormatter, cancellationToken);
	}

	private static async Task WriteAsyncCore<T>(IAsyncEnumerable<SseItem<T>> source, Stream destination, Action<SseItem<T>, IBufferWriter<byte>> itemFormatter, CancellationToken cancellationToken)
	{
		using PooledByteBufferWriter bufferWriter = new PooledByteBufferWriter();
		using PooledByteBufferWriter userDataBufferWriter = new PooledByteBufferWriter();
		await foreach (SseItem<T> item in source.WithCancellation(cancellationToken).ConfigureAwait(continueOnCapturedContext: false))
		{
			itemFormatter(item, userDataBufferWriter);
			FormatSseEvent(bufferWriter, item._eventType, userDataBufferWriter.WrittenMemory.Span, item.EventId, item.ReconnectionInterval);
			await destination.WriteAsync(bufferWriter.WrittenMemory, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			userDataBufferWriter.Reset();
			bufferWriter.Reset();
		}
	}

	private static void FormatSseEvent(PooledByteBufferWriter bufferWriter, string eventType, ReadOnlySpan<byte> data, string eventId, TimeSpan? reconnectionInterval)
	{
		if (eventType != null)
		{
			bufferWriter.WriteUtf8String("event: "u8);
			bufferWriter.WriteUtf8String(eventType.AsSpan());
			bufferWriter.WriteUtf8String(s_newLine);
		}
		WriteLinesWithPrefix(bufferWriter, "data: "u8, data);
		bufferWriter.Write(s_newLine);
		if (eventId != null)
		{
			bufferWriter.WriteUtf8String("id: "u8);
			bufferWriter.WriteUtf8String(eventId.AsSpan());
			bufferWriter.WriteUtf8String(s_newLine);
		}
		if (reconnectionInterval.HasValue)
		{
			TimeSpan valueOrDefault = reconnectionInterval.GetValueOrDefault();
			bufferWriter.WriteUtf8String("retry: "u8);
			bufferWriter.WriteUtf8Number((long)valueOrDefault.TotalMilliseconds);
			bufferWriter.WriteUtf8String(s_newLine);
		}
		bufferWriter.WriteUtf8String(s_newLine);
	}

	private static void WriteLinesWithPrefix(PooledByteBufferWriter writer, ReadOnlySpan<byte> prefix, ReadOnlySpan<byte> data)
	{
		while (true)
		{
			writer.WriteUtf8String(prefix);
			int num = data.IndexOfAny((byte)13, (byte)10);
			if (num < 0)
			{
				break;
			}
			int length = num;
			if (data[num++] == 13 && num < data.Length && data[num] == 10)
			{
				num++;
			}
			ReadOnlySpan<byte> value = data.Slice(0, length);
			data = data.Slice(num);
			writer.WriteUtf8String(value);
			writer.WriteUtf8String(s_newLine);
		}
		writer.WriteUtf8String(data);
	}
}
