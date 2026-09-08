using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;

namespace System.Threading;

public sealed class Lock
{
	public ref struct Scope
	{
		private Lock _lockObj;

		private ThreadId _currentThreadId;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Scope(Lock lockObj, ThreadId currentThreadId)
		{
			_lockObj = lockObj;
			_currentThreadId = currentThreadId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
		{
			Lock lockObj = _lockObj;
			if (lockObj != null)
			{
				_lockObj = null;
				lockObj.Exit(_currentThreadId);
			}
		}
	}

	private struct State : IEquatable<State>
	{
		private uint _state;

		public bool IsLocked => (_state & 1) != 0;

		private bool ShouldNotPreemptWaiters => (_state & 2) != 0;

		private bool ShouldNonWaiterAttemptToAcquireLock => (_state & 3) == 0;

		private bool HasAnySpinners => (_state & 0x1C) != 0;

		public bool UseTrivialWaits => (_state & 0x40) != 0;

		public bool HasAnyWaiters => _state >= 128;

		public bool NeedToSignalWaiter => (_state & 0x3C) == 0;

		public State(Lock lockObj)
			: this(lockObj._state)
		{
		}

		private State(uint state)
		{
			_state = state;
		}

		private static uint Neg(uint state)
		{
			return 0 - state;
		}

		private void SetIsLocked()
		{
			_state++;
		}

		private void SetShouldNotPreemptWaiters()
		{
			_state += 2u;
		}

		private void ClearShouldNotPreemptWaiters()
		{
			_state -= 2u;
		}

		private bool TryIncrementSpinnerCount()
		{
			uint state = _state + 4;
			if (new State(state).HasAnySpinners)
			{
				_state = state;
				return true;
			}
			return false;
		}

		private void DecrementSpinnerCount()
		{
			_state -= 4u;
		}

		private void SetIsWaiterSignaledToWake()
		{
			_state += 32u;
		}

		private void ClearIsWaiterSignaledToWake()
		{
			_state -= 32u;
		}

		public static void InitializeUseTrivialWaits(Lock lockObj, bool useTrivialWaits)
		{
			if (useTrivialWaits)
			{
				lockObj._state = 64u;
			}
		}

		private bool TryIncrementWaiterCount()
		{
			uint state = _state + 128;
			if (new State(state).HasAnyWaiters)
			{
				_state = state;
				return true;
			}
			return false;
		}

		private void DecrementWaiterCount()
		{
			_state -= 128u;
		}

		public static bool operator ==(State state1, State state2)
		{
			return state1._state == state2._state;
		}

		bool IEquatable<State>.Equals(State other)
		{
			return this == other;
		}

		public override bool Equals(object obj)
		{
			if (obj is State state)
			{
				return this == state;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (int)_state;
		}

		private static State CompareExchange(Lock lockObj, State toState, State fromState)
		{
			return new State(Interlocked.CompareExchange(ref lockObj._state, toState._state, fromState._state));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryLock(Lock lockObj)
		{
			State state = new State(lockObj);
			if (!state.ShouldNonWaiterAttemptToAcquireLock)
			{
				return false;
			}
			State toState = state;
			toState.SetIsLocked();
			return CompareExchange(lockObj, toState, state) == state;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static State Unlock(Lock lockObj)
		{
			return new State(Interlocked.Decrement(ref lockObj._state));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TryLockResult TryLockBeforeSpinLoop(Lock lockObj, short spinCount, out bool isFirstSpinner)
		{
			isFirstSpinner = false;
			State state = new State(lockObj);
			TryLockResult tryLockResult;
			while (true)
			{
				State toState = state;
				tryLockResult = TryLockResult.Spin;
				if (toState.HasAnyWaiters)
				{
					if (toState.ShouldNotPreemptWaiters)
					{
						return TryLockResult.Wait;
					}
					if (lockObj.ShouldStopPreemptingWaiters)
					{
						toState.SetShouldNotPreemptWaiters();
						tryLockResult = TryLockResult.Wait;
					}
				}
				if (tryLockResult == TryLockResult.Spin)
				{
					if (!toState.IsLocked)
					{
						toState.SetIsLocked();
						tryLockResult = TryLockResult.Locked;
					}
					else if ((toState.HasAnySpinners && spinCount == 0) || !toState.TryIncrementSpinnerCount())
					{
						return TryLockResult.Wait;
					}
				}
				State state2 = CompareExchange(lockObj, toState, state);
				if (state2 == state)
				{
					break;
				}
				state = state2;
			}
			if (tryLockResult == TryLockResult.Spin && !state.HasAnySpinners)
			{
				isFirstSpinner = true;
			}
			return tryLockResult;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TryLockResult TryLockInsideSpinLoop(Lock lockObj)
		{
			State state = new State(lockObj);
			while (true)
			{
				if (!state.ShouldNonWaiterAttemptToAcquireLock)
				{
					if (!state.ShouldNotPreemptWaiters)
					{
						return TryLockResult.Spin;
					}
					return TryLockResult.Wait;
				}
				State toState = state;
				toState.SetIsLocked();
				toState.DecrementSpinnerCount();
				State state2 = CompareExchange(lockObj, toState, state);
				if (state2 == state)
				{
					break;
				}
				state = state2;
			}
			return TryLockResult.Locked;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TryLockResult TryLockAfterSpinLoop(Lock lockObj)
		{
			State state = new State(Interlocked.Add(ref lockObj._state, Neg(4u)));
			while (true)
			{
				if (state.IsLocked)
				{
					return TryLockResult.Wait;
				}
				State toState = state;
				toState.SetIsLocked();
				State state2 = CompareExchange(lockObj, toState, state);
				if (state2 == state)
				{
					break;
				}
				state = state2;
			}
			return TryLockResult.Locked;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryLockBeforeWait(Lock lockObj)
		{
			State state = new State(lockObj);
			bool flag = false;
			while (true)
			{
				State toState = state;
				if (toState.ShouldNonWaiterAttemptToAcquireLock)
				{
					toState.SetIsLocked();
				}
				else
				{
					if (!toState.TryIncrementWaiterCount())
					{
						ThrowHelper.ThrowOutOfMemoryException_LockEnter_WaiterCountOverflow();
					}
					if (!state.HasAnyWaiters && !flag)
					{
						flag = true;
						lockObj.ResetWaiterStartTime();
					}
				}
				State state2 = CompareExchange(lockObj, toState, state);
				if (state2 == state)
				{
					break;
				}
				state = state2;
			}
			if (state.ShouldNonWaiterAttemptToAcquireLock)
			{
				return true;
			}
			if (!state.HasAnyWaiters | flag)
			{
				lockObj.RecordWaiterStartTime();
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryLockInsideWaiterSpinLoop(Lock lockObj)
		{
			bool flag = false;
			State state = new State(lockObj);
			State toState;
			while (true)
			{
				if (state.IsLocked)
				{
					return false;
				}
				toState = state;
				toState.SetIsLocked();
				toState.ClearIsWaiterSignaledToWake();
				toState.DecrementWaiterCount();
				if (toState.ShouldNotPreemptWaiters)
				{
					toState.ClearShouldNotPreemptWaiters();
					if (toState.HasAnyWaiters && !flag)
					{
						flag = true;
						lockObj.RecordWaiterStartTime();
					}
				}
				State state2 = CompareExchange(lockObj, toState, state);
				if (state2 == state)
				{
					break;
				}
				state = state2;
			}
			if (toState.HasAnyWaiters && !flag)
			{
				lockObj.RecordWaiterStartTime();
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryLockAfterWaiterSpinLoop(Lock lockObj)
		{
			State state = new State(Interlocked.Add(ref lockObj._state, Neg(32u)));
			bool flag = false;
			State toState;
			while (true)
			{
				if (state.IsLocked)
				{
					return false;
				}
				toState = state;
				toState.SetIsLocked();
				toState.DecrementWaiterCount();
				if (toState.ShouldNotPreemptWaiters)
				{
					toState.ClearShouldNotPreemptWaiters();
					if (toState.HasAnyWaiters && !flag)
					{
						flag = true;
						lockObj.RecordWaiterStartTime();
					}
				}
				State state2 = CompareExchange(lockObj, toState, state);
				if (state2 == state)
				{
					break;
				}
				state = state2;
			}
			if (toState.HasAnyWaiters && !flag)
			{
				lockObj.RecordWaiterStartTime();
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void UnregisterWaiter(Lock lockObj)
		{
			State state = new State(lockObj);
			while (true)
			{
				State toState = state;
				toState.DecrementWaiterCount();
				if (toState.ShouldNotPreemptWaiters && !toState.HasAnyWaiters)
				{
					toState.ClearShouldNotPreemptWaiters();
				}
				State state2 = CompareExchange(lockObj, toState, state);
				if (state2 == state)
				{
					break;
				}
				state = state2;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TrySetIsWaiterSignaledToWake(Lock lockObj, State state)
		{
			while (true)
			{
				if (!state.NeedToSignalWaiter)
				{
					return false;
				}
				State toState = state;
				toState.SetIsWaiterSignaledToWake();
				if (!toState.ShouldNotPreemptWaiters && lockObj.ShouldStopPreemptingWaiters)
				{
					toState.SetShouldNotPreemptWaiters();
				}
				State state2 = CompareExchange(lockObj, toState, state);
				if (state2 == state)
				{
					return true;
				}
				if (!state2.HasAnyWaiters)
				{
					break;
				}
				state = state2;
			}
			return false;
		}
	}

	private enum TryLockResult
	{
		Locked,
		Spin,
		Wait
	}

	internal struct ThreadId(uint id)
	{
		[ThreadStatic]
		private static uint t_threadId;

		private uint _id = id;

		public uint Id => _id;

		public bool IsInitialized => _id != 0;

		public static ThreadId Current_NoInitialize => new ThreadId(t_threadId);

		public void InitializeForCurrentThread()
		{
			uint num = (uint)Interop.Kernel32.GetCurrentThreadId();
			if (num == 0)
			{
				num--;
			}
			t_threadId = (_id = num);
		}
	}

	private static long s_contentionCount;

	private uint _owningThreadId;

	private uint _state;

	private uint _recursionCount;

	private short _spinCount;

	private ushort _waiterStartTimeMs;

	private AutoResetEvent _waitEvent;

	private static readonly short s_maxSpinCount = DetermineMaxSpinCount();

	private static readonly short s_minSpinCountForAdaptiveSpin = DetermineMinSpinCountForAdaptiveSpin();

	private bool ShouldStopPreemptingWaiters
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			ushort waiterStartTimeMs = _waiterStartTimeMs;
			if (waiterStartTimeMs != 0)
			{
				return (ushort)(Environment.TickCount - waiterStartTimeMs) >= 100;
			}
			return false;
		}
	}

	public bool IsHeldByCurrentThread
	{
		get
		{
			ThreadId threadId = new ThreadId(_owningThreadId);
			if (threadId.IsInitialized)
			{
				return threadId.Id == ThreadId.Current_NoInitialize.Id;
			}
			return false;
		}
	}

	internal static long ContentionCount => s_contentionCount;

	internal nint LockIdForEvents => _waitEvent.SafeWaitHandle.DangerousGetHandle();

	internal ulong OwningThreadId => _owningThreadId;

	internal ulong OwningOSThreadId => _owningThreadId;

	internal int OwningManagedThreadId => 0;

	private static bool IsSingleProcessor => Environment.IsSingleProcessor;

	internal Lock(bool useTrivialWaits)
		: this()
	{
		State.InitializeUseTrivialWaits(this, useTrivialWaits);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Enter()
	{
		TryEnter_Inlined(-1);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private ThreadId EnterAndGetCurrentThreadId()
	{
		return TryEnter_Inlined(-1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Scope EnterScope()
	{
		return new Scope(this, EnterAndGetCurrentThreadId());
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool TryEnter()
	{
		return TryEnter_Inlined(0).IsInitialized;
	}

	public bool TryEnter(int millisecondsTimeout)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(millisecondsTimeout, -1, "millisecondsTimeout");
		return TryEnter_Outlined(millisecondsTimeout);
	}

	public bool TryEnter(TimeSpan timeout)
	{
		return TryEnter_Outlined(WaitHandle.ToTimeoutMilliseconds(timeout));
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private bool TryEnter_Outlined(int timeoutMs)
	{
		return TryEnter_Inlined(timeoutMs).IsInitialized;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ThreadId TryEnter_Inlined(int timeoutMs)
	{
		ThreadId current_NoInitialize = ThreadId.Current_NoInitialize;
		if (current_NoInitialize.IsInitialized && State.TryLock(this))
		{
			_owningThreadId = current_NoInitialize.Id;
			return current_NoInitialize;
		}
		return TryEnterSlow(timeoutMs, current_NoInitialize);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Exit()
	{
		ThreadId threadId = new ThreadId(_owningThreadId);
		if (!threadId.IsInitialized || threadId.Id != ThreadId.Current_NoInitialize.Id)
		{
			ThrowHelper.ThrowSynchronizationLockException_LockExit();
		}
		ExitImpl();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void Exit(ThreadId currentThreadId)
	{
		if (_owningThreadId != currentThreadId.Id)
		{
			ThrowHelper.ThrowSynchronizationLockException_LockExit();
		}
		ExitImpl();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ExitImpl()
	{
		if (_recursionCount == 0)
		{
			_owningThreadId = 0u;
			State state = State.Unlock(this);
			if (state.HasAnyWaiters)
			{
				SignalWaiterIfNecessary(state);
			}
		}
		else
		{
			_recursionCount--;
		}
	}

	private static bool IsAdaptiveSpinEnabled(short minSpinCountForAdaptiveSpin)
	{
		return minSpinCountForAdaptiveSpin <= 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private ThreadId TryEnterSlow(int timeoutMs, ThreadId currentThreadId)
	{
		if (!currentThreadId.IsInitialized)
		{
			currentThreadId.InitializeForCurrentThread();
			if (State.TryLock(this))
			{
				goto IL_0135;
			}
		}
		else if (_owningThreadId == currentThreadId.Id)
		{
			uint num = _recursionCount + 1;
			if (num != 0)
			{
				_recursionCount = num;
				return currentThreadId;
			}
			throw new LockRecursionException(SR.Lock_Enter_LockRecursionException);
		}
		if (timeoutMs == 0)
		{
			return new ThreadId(0u);
		}
		Debugger.NotifyOfCrossThreadDependency();
		_ = 1;
		short num2 = s_maxSpinCount;
		short num3;
		bool isFirstSpinner;
		TryLockResult tryLockResult;
		if (num2 != 0)
		{
			num3 = s_minSpinCountForAdaptiveSpin;
			short num4 = _spinCount;
			if (num4 >= 0)
			{
				tryLockResult = State.TryLockBeforeSpinLoop(this, num4, out isFirstSpinner);
				if (tryLockResult == TryLockResult.Spin)
				{
					if (isFirstSpinner)
					{
						num4 = num2;
					}
					short num5 = 0;
					while (true)
					{
						LowLevelSpinWaiter.Wait(num5, 10, isSingleProcessor: false);
						if (++num5 >= num4)
						{
							break;
						}
						tryLockResult = State.TryLockInsideSpinLoop(this);
						if (tryLockResult == TryLockResult.Spin)
						{
							continue;
						}
						goto IL_00c6;
					}
					goto IL_00ec;
				}
				goto IL_0131;
			}
			_spinCount = (short)(num4 + 1);
		}
		goto IL_0144;
		IL_0144:
		bool flag = NativeRuntimeEventSource.Log.IsEnabled(EventLevel.Informational, (EventKeywords)16384L);
		AutoResetEvent autoResetEvent = _waitEvent ?? CreateWaitEvent(flag);
		if (!State.TryLockBeforeWait(this))
		{
			try
			{
				Interlocked.Increment(ref s_contentionCount);
				long num6 = 0L;
				if (flag)
				{
					NativeRuntimeEventSource.Log.ContentionStart(this);
					num6 = Stopwatch.GetTimestamp();
				}
				using (new ThreadBlockingInfo.Scope(this, timeoutMs))
				{
					bool flag2 = false;
					int num7 = ((timeoutMs >= 0) ? Environment.TickCount : 0);
					int num8 = timeoutMs;
					while (autoResetEvent.WaitOneNoCheck(num8, new State(this).UseTrivialWaits))
					{
						for (short num9 = 0; num9 < num2; num9++)
						{
							if (State.TryLockInsideWaiterSpinLoop(this))
							{
								flag2 = true;
								break;
							}
							LowLevelSpinWaiter.Wait(num9, 10, isSingleProcessor: false);
						}
						if (flag2)
						{
							break;
						}
						if (State.TryLockAfterWaiterSpinLoop(this))
						{
							flag2 = true;
							break;
						}
						if (num8 >= 0)
						{
							uint num10 = (uint)(Environment.TickCount - num7);
							if (num10 >= (uint)timeoutMs)
							{
								break;
							}
							num8 = timeoutMs - (int)num10;
						}
					}
					if (flag2)
					{
						_owningThreadId = currentThreadId.Id;
						if (flag)
						{
							double durationNs = (double)(Stopwatch.GetTimestamp() - num6) * 1000000000.0 / (double)Stopwatch.Frequency;
							NativeRuntimeEventSource.Log.ContentionStop(durationNs);
						}
						return currentThreadId;
					}
				}
			}
			catch
			{
				State.UnregisterWaiter(this);
				throw;
			}
			State.UnregisterWaiter(this);
			return new ThreadId(0u);
		}
		goto IL_0135;
		IL_00ec:
		tryLockResult = State.TryLockAfterSpinLoop(this);
		if (isFirstSpinner && IsAdaptiveSpinEnabled(num3))
		{
			if (tryLockResult == TryLockResult.Locked)
			{
				short num4 = _spinCount;
				if (num4 < num2)
				{
					_spinCount = (short)(num4 + 1);
				}
			}
			else
			{
				short num4 = _spinCount;
				_spinCount = ((num4 > 0) ? ((short)(num4 - 1)) : num3);
			}
		}
		goto IL_0131;
		IL_00c6:
		if (tryLockResult != TryLockResult.Locked)
		{
			goto IL_00ec;
		}
		if (isFirstSpinner && IsAdaptiveSpinEnabled(num3))
		{
			short num4 = _spinCount;
			if (num4 < num2)
			{
				_spinCount = (short)(num4 + 1);
			}
		}
		goto IL_0135;
		IL_0131:
		if (tryLockResult != TryLockResult.Wait)
		{
			goto IL_0135;
		}
		goto IL_0144;
		IL_0135:
		_owningThreadId = currentThreadId.Id;
		return currentThreadId;
	}

	private void ResetWaiterStartTime()
	{
		_waiterStartTimeMs = 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void RecordWaiterStartTime()
	{
		ushort num = (ushort)Environment.TickCount;
		if (num == 0)
		{
			num--;
		}
		_waiterStartTimeMs = num;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private AutoResetEvent CreateWaitEvent(bool areContentionEventsEnabled)
	{
		AutoResetEvent autoResetEvent = new AutoResetEvent(initialState: false);
		AutoResetEvent autoResetEvent2 = Interlocked.CompareExchange(ref _waitEvent, autoResetEvent, null);
		if (autoResetEvent2 == null)
		{
			if (areContentionEventsEnabled && NativeRuntimeEventSource.Log.IsEnabled())
			{
				NativeRuntimeEventSource.Log.ContentionLockCreated(this);
			}
			return autoResetEvent;
		}
		autoResetEvent.Dispose();
		return autoResetEvent2;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SignalWaiterIfNecessary(State state)
	{
		if (State.TrySetIsWaiterSignaledToWake(this, state))
		{
			_waitEvent.Set();
		}
	}

	private static short DetermineMaxSpinCount()
	{
		if (IsSingleProcessor)
		{
			return 0;
		}
		return AppContextConfigHelper.GetInt16Config("System.Threading.Lock.SpinCount", "DOTNET_Lock_SpinCount", 22, allowNegative: false);
	}

	private static short DetermineMinSpinCountForAdaptiveSpin()
	{
		short num = AppContextConfigHelper.GetInt16Config("System.Threading.Lock.AdaptiveSpinPeriod", "DOTNET_Lock_AdaptiveSpinPeriod", 100);
		if (num < -1)
		{
			num = 100;
		}
		return (short)(-num);
	}

	public Lock()
	{
		_spinCount = s_maxSpinCount;
	}
}
