using System.Runtime.InteropServices;

namespace System.Security.Cryptography;

internal sealed class FixedMemoryKeyBox : SafeHandle
{
	private readonly int _length;

	internal unsafe ReadOnlySpan<byte> DangerousKeySpan => new ReadOnlySpan<byte>((void*)handle, _length);

	public override bool IsInvalid => handle == IntPtr.Zero;

	internal unsafe FixedMemoryKeyBox(ReadOnlySpan<byte> key)
		: base(IntPtr.Zero, ownsHandle: true)
	{
		void* pointer = NativeMemory.Alloc((nuint)key.Length);
		key.CopyTo(new Span<byte>(pointer, key.Length));
		SetHandle((nint)pointer);
		_length = key.Length;
	}

	protected unsafe override bool ReleaseHandle()
	{
		CryptographicOperations.ZeroMemory(new Span<byte>((void*)handle, _length));
		NativeMemory.Free((void*)handle);
		return true;
	}

	internal TRet UseKey<TState, TRet>(TState state, Func<TState, ReadOnlySpan<byte>, TRet> func)
	{
		bool success = false;
		try
		{
			DangerousAddRef(ref success);
			return func(state, DangerousKeySpan);
		}
		finally
		{
			if (success)
			{
				DangerousRelease();
			}
		}
	}

	internal TRet UseKey<TState, TRet>(ReadOnlySpan<byte> state1, TState state2, Func<ReadOnlySpan<byte>, TState, ReadOnlySpan<byte>, TRet> func)
	{
		bool success = false;
		try
		{
			DangerousAddRef(ref success);
			return func(state1, state2, DangerousKeySpan);
		}
		finally
		{
			if (success)
			{
				DangerousRelease();
			}
		}
	}
}
