using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Internal;

namespace System.Threading;

internal sealed class LowLevelLifoSemaphore : IDisposable
{
	private struct Counts : IEquatable<Counts>
	{
		private ulong _data;

		public uint SignalCount
		{
			get
			{
				return GetUInt32Value(0);
			}
			set
			{
				SetUInt32Value(value, 0);
			}
		}

		public ushort WaiterCount => GetUInt16Value(32);

		public byte SpinnerCount => GetByteValue(48);

		public byte CountOfWaitersSignaledToWake => GetByteValue(56);

		private Counts(ulong data)
		{
			_data = data;
		}

		private uint GetUInt32Value(byte shift)
		{
			return (uint)(_data >> (int)shift);
		}

		private void SetUInt32Value(uint value, byte shift)
		{
			_data = (_data & ~(4294967295uL << (int)shift)) | ((ulong)value << (int)shift);
		}

		private ushort GetUInt16Value(byte shift)
		{
			return (ushort)(_data >> (int)shift);
		}

		private byte GetByteValue(byte shift)
		{
			return (byte)(_data >> (int)shift);
		}

		public void AddSignalCount(uint value)
		{
			_data += value;
		}

		public void DecrementSignalCount()
		{
			_data--;
		}

		public void IncrementWaiterCount()
		{
			_data += 4294967296uL;
		}

		public void DecrementWaiterCount()
		{
			_data -= 4294967296uL;
		}

		public void InterlockedDecrementWaiterCount()
		{
			new Counts(Interlocked.Add(ref _data, 18446744069414584320uL));
		}

		public void IncrementSpinnerCount()
		{
			_data += 281474976710656uL;
		}

		public void DecrementSpinnerCount()
		{
			_data -= 281474976710656uL;
		}

		public void AddUpToMaxCountOfWaitersSignaledToWake(uint value)
		{
			uint num = (uint)(255 - CountOfWaitersSignaledToWake);
			if (value > num)
			{
				value = num;
			}
			_data += (ulong)value << 56;
		}

		public void DecrementCountOfWaitersSignaledToWake()
		{
			_data -= 72057594037927936uL;
		}

		public Counts InterlockedCompareExchange(Counts newCounts, Counts oldCounts)
		{
			return new Counts(Interlocked.CompareExchange(ref _data, newCounts._data, oldCounts._data));
		}

		public static bool operator ==(Counts lhs, Counts rhs)
		{
			return lhs.Equals(rhs);
		}

		public override bool Equals([NotNullWhen(true)] object obj)
		{
			if (obj is Counts other)
			{
				return Equals(other);
			}
			return false;
		}

		public bool Equals(Counts other)
		{
			return _data == other._data;
		}

		public override int GetHashCode()
		{
			return (int)_data + (int)(_data >> 32);
		}
	}

	private struct CacheLineSeparatedCounts
	{
		private readonly PaddingFor32 _pad1;

		public Counts _counts;

		private readonly PaddingFor32 _pad2;
	}

	private CacheLineSeparatedCounts _separated;

	private readonly int _maximumSignalCount;

	private readonly int _spinCount;

	private readonly Action _onWait;

	private nint _completionPort;

	public LowLevelLifoSemaphore(int initialSignalCount, int maximumSignalCount, int spinCount, Action onWait)
	{
		_separated = default(CacheLineSeparatedCounts);
		_separated._counts.SignalCount = (uint)initialSignalCount;
		_maximumSignalCount = maximumSignalCount;
		_spinCount = spinCount;
		_onWait = onWait;
		Create(maximumSignalCount);
	}

	public bool Wait(int timeoutMs, bool spinWait)
	{
		int num = (spinWait ? _spinCount : 0);
		Counts counts = _separated._counts;
		Counts newCounts;
		while (true)
		{
			newCounts = counts;
			if (counts.SignalCount != 0)
			{
				newCounts.DecrementSignalCount();
			}
			else if (timeoutMs != 0)
			{
				if (num > 0 && newCounts.SpinnerCount < byte.MaxValue)
				{
					newCounts.IncrementSpinnerCount();
				}
				else
				{
					newCounts.IncrementWaiterCount();
				}
			}
			Counts counts2 = _separated._counts.InterlockedCompareExchange(newCounts, counts);
			if (counts2 == counts)
			{
				break;
			}
			counts = counts2;
		}
		if (counts.SignalCount != 0)
		{
			return true;
		}
		if (newCounts.WaiterCount != counts.WaiterCount)
		{
			return WaitForSignal(timeoutMs);
		}
		if (timeoutMs == 0)
		{
			return false;
		}
		bool isSingleProcessor = Environment.IsSingleProcessor;
		int num2 = (isSingleProcessor ? 10 : 0);
		while (num2 < num)
		{
			LowLevelSpinWaiter.Wait(num2, 10, isSingleProcessor);
			num2++;
			counts = _separated._counts;
			while (counts.SignalCount != 0)
			{
				Counts newCounts2 = counts;
				newCounts2.DecrementSignalCount();
				newCounts2.DecrementSpinnerCount();
				Counts counts3 = _separated._counts.InterlockedCompareExchange(newCounts2, counts);
				if (counts3 == counts)
				{
					return true;
				}
				counts = counts3;
			}
		}
		counts = _separated._counts;
		while (true)
		{
			Counts newCounts3 = counts;
			newCounts3.DecrementSpinnerCount();
			if (counts.SignalCount != 0)
			{
				newCounts3.DecrementSignalCount();
			}
			else
			{
				newCounts3.IncrementWaiterCount();
			}
			Counts counts4 = _separated._counts.InterlockedCompareExchange(newCounts3, counts);
			if (counts4 == counts)
			{
				break;
			}
			counts = counts4;
		}
		if (counts.SignalCount == 0)
		{
			return WaitForSignal(timeoutMs);
		}
		return true;
	}

	private bool WaitForSignal(int timeoutMs)
	{
		_onWait();
		Counts counts;
		do
		{
			int num = ((timeoutMs != -1) ? Environment.TickCount : 0);
			if (timeoutMs == 0 || !WaitCore(timeoutMs))
			{
				_separated._counts.InterlockedDecrementWaiterCount();
				return false;
			}
			int num2 = ((timeoutMs != -1) ? Environment.TickCount : 0);
			counts = _separated._counts;
			while (true)
			{
				Counts newCounts = counts;
				if (counts.SignalCount != 0)
				{
					newCounts.DecrementSignalCount();
					newCounts.DecrementWaiterCount();
				}
				if (counts.CountOfWaitersSignaledToWake != 0)
				{
					newCounts.DecrementCountOfWaitersSignaledToWake();
				}
				Counts counts2 = _separated._counts.InterlockedCompareExchange(newCounts, counts);
				if (counts2 == counts)
				{
					break;
				}
				counts = counts2;
				if (timeoutMs != -1)
				{
					int num3 = num2 - num;
					timeoutMs = ((num3 >= 0 && num3 < timeoutMs) ? (timeoutMs - num3) : 0);
				}
			}
		}
		while (counts.SignalCount == 0);
		return true;
	}

	public void Release(int releaseCount)
	{
		Counts counts = _separated._counts;
		int num;
		while (true)
		{
			Counts newCounts = counts;
			newCounts.AddSignalCount((uint)releaseCount);
			num = (int)(Math.Min(newCounts.SignalCount, (uint)(counts.WaiterCount + counts.SpinnerCount)) - counts.SpinnerCount - counts.CountOfWaitersSignaledToWake);
			if (num > 0)
			{
				if (num > releaseCount)
				{
					num = releaseCount;
				}
				newCounts.AddUpToMaxCountOfWaitersSignaledToWake((uint)num);
			}
			Counts counts2 = _separated._counts.InterlockedCompareExchange(newCounts, counts);
			if (counts2 == counts)
			{
				break;
			}
			counts = counts2;
		}
		if (num > 0)
		{
			ReleaseCore(num);
		}
	}

	private void Create(int maximumSignalCount)
	{
		_completionPort = Interop.Kernel32.CreateIoCompletionPort(new IntPtr(-1), IntPtr.Zero, UIntPtr.Zero, maximumSignalCount);
		if (_completionPort == IntPtr.Zero)
		{
			int hRForLastWin32Error = Marshal.GetHRForLastWin32Error();
			throw new OutOfMemoryException
			{
				HResult = hRForLastWin32Error
			};
		}
	}

	~LowLevelLifoSemaphore()
	{
		if (_completionPort != IntPtr.Zero)
		{
			Dispose();
		}
	}

	public bool WaitCore(int timeoutMs)
	{
		uint lpNumberOfBytesTransferred;
		nuint CompletionKey;
		nint lpOverlapped;
		return Interop.Kernel32.GetQueuedCompletionStatus(_completionPort, out lpNumberOfBytesTransferred, out CompletionKey, out lpOverlapped, timeoutMs);
	}

	private void ReleaseCore(int count)
	{
		for (int i = 0; i < count; i++)
		{
			if (!Interop.Kernel32.PostQueuedCompletionStatus(_completionPort, 1u, UIntPtr.Zero, IntPtr.Zero))
			{
				int lastPInvokeError = Marshal.GetLastPInvokeError();
				throw new OutOfMemoryException
				{
					HResult = lastPInvokeError
				};
			}
		}
	}

	public void Dispose()
	{
		Interop.Kernel32.CloseHandle(_completionPort);
		_completionPort = IntPtr.Zero;
		GC.SuppressFinalize(this);
	}
}
