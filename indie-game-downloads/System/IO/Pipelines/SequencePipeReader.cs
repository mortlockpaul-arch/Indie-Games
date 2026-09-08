using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Pipelines;

internal sealed class SequencePipeReader : PipeReader
{
	private ReadOnlySequence<byte> _sequence;

	private bool _isReaderCompleted;

	private int _cancelNext;

	public SequencePipeReader(ReadOnlySequence<byte> sequence)
	{
		_sequence = sequence;
	}

	public override void AdvanceTo(SequencePosition consumed)
	{
		AdvanceTo(consumed, consumed);
	}

	public override void AdvanceTo(SequencePosition consumed, SequencePosition examined)
	{
		ThrowIfCompleted();
		if (consumed.Equals(_sequence.End))
		{
			_sequence = ReadOnlySequence<byte>.Empty;
		}
		else
		{
			_sequence = _sequence.Slice(consumed);
		}
	}

	public override void CancelPendingRead()
	{
		Interlocked.Exchange(ref _cancelNext, 1);
	}

	public override void Complete(Exception? exception = null)
	{
		if (!_isReaderCompleted)
		{
			_isReaderCompleted = true;
			_sequence = ReadOnlySequence<byte>.Empty;
		}
	}

	public override ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		if (TryRead(out var result))
		{
			return new ValueTask<ReadResult>(result);
		}
		result = new ReadResult(ReadOnlySequence<byte>.Empty, isCanceled: false, isCompleted: true);
		return new ValueTask<ReadResult>(result);
	}

	public override bool TryRead(out ReadResult result)
	{
		ThrowIfCompleted();
		bool flag = Interlocked.Exchange(ref _cancelNext, 0) == 1;
		if (flag || _sequence.Length > 0)
		{
			result = new ReadResult(_sequence, flag, isCompleted: true);
			return true;
		}
		result = default(ReadResult);
		return false;
	}

	private void ThrowIfCompleted()
	{
		if (_isReaderCompleted)
		{
			ThrowHelper.ThrowInvalidOperationException_NoReadingAllowed();
		}
	}
}
