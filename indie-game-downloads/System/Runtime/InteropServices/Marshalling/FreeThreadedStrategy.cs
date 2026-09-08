namespace System.Runtime.InteropServices.Marshalling;

internal sealed class FreeThreadedStrategy : IIUnknownStrategy
{
	public static readonly IIUnknownStrategy Instance = new System.Runtime.InteropServices.Marshalling.FreeThreadedStrategy();

	unsafe void* IIUnknownStrategy.CreateInstancePointer(void* unknown)
	{
		Marshal.AddRef((nint)unknown);
		return unknown;
	}

	unsafe int IIUnknownStrategy.QueryInterface(void* thisPtr, in Guid handle, out void* ppObj)
	{
		int num = Marshal.QueryInterface((nint)thisPtr, in handle, out var ppv);
		if (num < 0)
		{
			ppObj = null;
			return num;
		}
		ppObj = (void*)ppv;
		return num;
	}

	unsafe int IIUnknownStrategy.Release(void* thisPtr)
	{
		return Marshal.Release((nint)thisPtr);
	}
}
