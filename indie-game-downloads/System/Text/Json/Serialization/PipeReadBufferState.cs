using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Text.Json.Serialization;

[StructLayout(LayoutKind.Auto)]
internal struct PipeReadBufferState(PipeReader utf8Json) : IReadBufferState<PipeReadBufferState, PipeReader>, IDisposable
{
	private readonly PipeReader _utf8Json = utf8Json;

	private ReadOnlySequence<byte> _sequence = ReadOnlySequence<byte>.Empty;

	private bool _isFinalBlock = false;

	private bool _isFirstBlock = true;

	private int _unsuccessfulReadBytes = 0;

	public readonly bool IsFinalBlock => _isFinalBlock;

	public void Advance(long bytesConsumed)
	{
		_unsuccessfulReadBytes = 0;
		if (bytesConsumed == 0L)
		{
			long length = _sequence.Length;
			_unsuccessfulReadBytes = (int)Math.Min(2147483647L, length * 2);
		}
		_utf8Json.AdvanceTo(_sequence.Slice(bytesConsumed).Start, _sequence.End);
		_sequence = ReadOnlySequence<byte>.Empty;
	}

	public async ValueTask<PipeReadBufferState> ReadAsync(PipeReader utf8Json, CancellationToken cancellationToken, bool fillBuffer = true)
	{
		PipeReadBufferState bufferState = this;
		int minimumSize = ((_unsuccessfulReadBytes > 0) ? _unsuccessfulReadBytes : 0);
		ReadResult readResult = await _utf8Json.ReadAtLeastAsync(minimumSize, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		bufferState._sequence = readResult.Buffer;
		bufferState._isFinalBlock = readResult.IsCompleted;
		bufferState.ProcessReadBytes();
		if (readResult.IsCanceled)
		{
			ThrowHelper.ThrowOperationCanceledException_PipeReadCanceled();
		}
		return bufferState;
	}

	public void Read(PipeReader utf8Json)
	{
		throw new NotImplementedException();
	}

	public void GetReader(JsonReaderState jsonReaderState, out Utf8JsonReader reader)
	{
		if (_sequence.IsSingleSegment)
		{
			reader = new Utf8JsonReader(_sequence.FirstSpan, IsFinalBlock, jsonReaderState);
		}
		else
		{
			reader = new Utf8JsonReader(_sequence, IsFinalBlock, jsonReaderState);
		}
	}

	private void ProcessReadBytes()
	{
		if (!_isFirstBlock)
		{
			return;
		}
		_isFirstBlock = false;
		if (_sequence.Length <= 0)
		{
			return;
		}
		if (_sequence.First.Length >= JsonConstants.Utf8Bom.Length)
		{
			if (_sequence.First.Span.StartsWith(JsonConstants.Utf8Bom))
			{
				_sequence = _sequence.Slice((byte)JsonConstants.Utf8Bom.Length);
			}
			return;
		}
		SequencePosition position = _sequence.Start;
		int num = 0;
		ReadOnlyMemory<byte> memory;
		while (num < JsonConstants.Utf8Bom.Length && _sequence.TryGet(ref position, out memory))
		{
			ReadOnlySpan<byte> span = memory.Span;
			int num2 = 0;
			while (num2 < span.Length && num < JsonConstants.Utf8Bom.Length)
			{
				if (span[num2] != JsonConstants.Utf8Bom[num])
				{
					num = 0;
					break;
				}
				num2++;
				num++;
			}
		}
		if (num == JsonConstants.Utf8Bom.Length)
		{
			_sequence = _sequence.Slice(JsonConstants.Utf8Bom.Length);
		}
	}

	public void Dispose()
	{
		if (!_sequence.Equals(ReadOnlySequence<byte>.Empty))
		{
			_utf8Json.AdvanceTo(_sequence.Start);
			_sequence = ReadOnlySequence<byte>.Empty;
		}
	}
}
