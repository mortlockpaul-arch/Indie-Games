using System.Diagnostics;
using System.Numerics;

namespace System.Threading.Tasks;

[DebuggerDisplay("ShouldExitCurrentIteration = {ShouldExitCurrentIteration}")]
public class ParallelLoopState
{
	private readonly ParallelLoopStateFlags _flagsBase;

	internal virtual bool InternalShouldExitCurrentIteration
	{
		get
		{
			throw new NotSupportedException(System.SR.ParallelState_NotSupportedException_UnsupportedMethod);
		}
	}

	public bool ShouldExitCurrentIteration => InternalShouldExitCurrentIteration;

	public bool IsStopped => (_flagsBase.LoopStateFlags & 4) != 0;

	public bool IsExceptional => (_flagsBase.LoopStateFlags & 1) != 0;

	internal virtual long? InternalLowestBreakIteration
	{
		get
		{
			throw new NotSupportedException(System.SR.ParallelState_NotSupportedException_UnsupportedMethod);
		}
	}

	public long? LowestBreakIteration => InternalLowestBreakIteration;

	internal ParallelLoopState(ParallelLoopStateFlags fbase)
	{
		_flagsBase = fbase;
	}

	public void Stop()
	{
		_flagsBase.Stop();
	}

	internal virtual void InternalBreak()
	{
		throw new NotSupportedException(System.SR.ParallelState_NotSupportedException_UnsupportedMethod);
	}

	public void Break()
	{
		InternalBreak();
	}

	internal static void Break<TInt>(TInt iteration, ParallelLoopStateFlags<TInt> pflags) where TInt : struct, IBinaryInteger<TInt>, IMinMaxValue<TInt>
	{
		int oldState = 0;
		if (!pflags.AtomicLoopStateUpdate(2, 13, ref oldState))
		{
			if ((oldState & 4) != 0)
			{
				throw new InvalidOperationException(System.SR.ParallelState_Break_InvalidOperationException_BreakAfterStop);
			}
			return;
		}
		TInt lowestBreakIteration = pflags.LowestBreakIteration;
		if (!(iteration < lowestBreakIteration))
		{
			return;
		}
		SpinWait spinWait = default(SpinWait);
		while (Interlocked.CompareExchange(ref pflags._lowestBreakIteration, iteration, lowestBreakIteration) != lowestBreakIteration)
		{
			spinWait.SpinOnce();
			lowestBreakIteration = pflags.LowestBreakIteration;
			if (iteration > lowestBreakIteration)
			{
				break;
			}
		}
	}
}
internal sealed class ParallelLoopState<TInt> : ParallelLoopState where TInt : struct, IBinaryInteger<TInt>, IMinMaxValue<TInt>
{
	private readonly ParallelLoopStateFlags<TInt> _sharedParallelStateFlags;

	private TInt _currentIteration;

	internal TInt CurrentIteration
	{
		get
		{
			return _currentIteration;
		}
		set
		{
			_currentIteration = value;
		}
	}

	internal override bool InternalShouldExitCurrentIteration => _sharedParallelStateFlags.ShouldExitLoop(CurrentIteration);

	internal override long? InternalLowestBreakIteration => _sharedParallelStateFlags.NullableLowestBreakIteration;

	internal ParallelLoopState(ParallelLoopStateFlags<TInt> sharedParallelStateFlags)
		: base(sharedParallelStateFlags)
	{
		_sharedParallelStateFlags = sharedParallelStateFlags;
	}

	internal override void InternalBreak()
	{
		ParallelLoopState.Break(CurrentIteration, _sharedParallelStateFlags);
	}
}
