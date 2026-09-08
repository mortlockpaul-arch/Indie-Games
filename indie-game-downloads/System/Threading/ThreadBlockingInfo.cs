using System.Runtime.CompilerServices;

namespace System.Threading;

internal struct ThreadBlockingInfo
{
	public ref struct Scope
	{
		private object _object;

		private ThreadBlockingInfo _blockingInfo;

		public Scope(Lock lockObj, int timeoutMs)
			: this(lockObj, ObjectKind.Lock, timeoutMs)
		{
		}

		private unsafe Scope(object obj, ObjectKind objectKind, int timeoutMs)
		{
			_blockingInfo = default(ThreadBlockingInfo);
			_object = obj;
			_blockingInfo.Push(Unsafe.AsPointer(in _object), objectKind, timeoutMs);
		}

		public void Dispose()
		{
			if (_object != null)
			{
				_blockingInfo.Pop();
				_object = null;
			}
		}
	}

	public enum ObjectKind
	{
		MonitorLock,
		MonitorWait,
		Lock,
		Condition
	}

	private static int s_monitorObjectOffsetOfLockOwnerOSThreadId;

	[ThreadStatic]
	private unsafe static ThreadBlockingInfo* t_first;

	private unsafe void* _objectPtr;

	private ObjectKind _objectKind;

	private int _timeoutMs;

	private unsafe ThreadBlockingInfo* _next;

	public unsafe ulong LockOwnerOSThreadId
	{
		get
		{
			switch (_objectKind)
			{
			case ObjectKind.MonitorLock:
			case ObjectKind.MonitorWait:
				if (s_monitorObjectOffsetOfLockOwnerOSThreadId != 0)
				{
					return *(nuint*)((byte*)_objectPtr + s_monitorObjectOffsetOfLockOwnerOSThreadId);
				}
				return 0uL;
			case ObjectKind.Lock:
				return ((Lock)Unsafe.AsRef<object>(_objectPtr)).OwningOSThreadId;
			default:
				return 0uL;
			}
		}
	}

	public unsafe int LockOwnerManagedThreadId
	{
		get
		{
			switch (_objectKind)
			{
			case ObjectKind.MonitorLock:
			case ObjectKind.MonitorWait:
				return 0;
			case ObjectKind.Lock:
				return ((Lock)Unsafe.AsRef<object>(_objectPtr)).OwningManagedThreadId;
			default:
				return 0;
			}
		}
	}

	private unsafe void Push(void* objectPtr, ObjectKind objectKind, int timeoutMs)
	{
		_objectPtr = objectPtr;
		_objectKind = objectKind;
		_timeoutMs = timeoutMs;
		_next = t_first;
		t_first = (ThreadBlockingInfo*)Unsafe.AsPointer<ThreadBlockingInfo>(this);
	}

	private unsafe void Pop()
	{
		t_first = _next;
		_objectPtr = null;
	}
}
