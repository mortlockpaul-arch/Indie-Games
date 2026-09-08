using System.Diagnostics;
using System.Threading;

namespace System.Runtime.CompilerServices;

internal struct MethodTableAuxiliaryData
{
	private uint Flags;

	private unsafe void* LoaderModule;

	private nint ExposedClassObjectRaw;

	public bool HasCheckedCanCompareBitsOrUseFastGetHashCode => (Flags & 2) != 0;

	public bool CanCompareBitsOrUseFastGetHashCode => (Flags & 4) != 0;

	public bool HasCheckedStreamOverride => (Flags & 0x400) != 0;

	public bool IsStreamOverriddenRead => (Flags & 0x800) != 0;

	public bool IsStreamOverriddenWrite => (Flags & 0x1000) != 0;

	public unsafe RuntimeType ExposedClassObject => Unsafe.Read<RuntimeType>(Unsafe.AsPointer(in ExposedClassObjectRaw));

	public bool IsClassInited => (Volatile.Read(in Flags) & 1) != 0;

	public bool IsClassInitedAndActive => (Volatile.Read(in Flags) & 0x2001) == 8193;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[DebuggerHidden]
	[DebuggerStepThrough]
	public ref DynamicStaticsInfo GetDynamicStaticsInfo()
	{
		return ref Unsafe.Subtract<DynamicStaticsInfo>(ref Unsafe.As<MethodTableAuxiliaryData, DynamicStaticsInfo>(ref this), 1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[DebuggerHidden]
	[DebuggerStepThrough]
	public ref ThreadStaticsInfo GetThreadStaticsInfo()
	{
		return ref Unsafe.Subtract<ThreadStaticsInfo>(ref Unsafe.As<MethodTableAuxiliaryData, ThreadStaticsInfo>(ref this), 1);
	}
}
