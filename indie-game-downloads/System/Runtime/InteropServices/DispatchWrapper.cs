using System.ComponentModel;
using System.Runtime.Versioning;

namespace System.Runtime.InteropServices;

[SupportedOSPlatform("windows")]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class DispatchWrapper
{
	public object? WrappedObject { get; }

	public DispatchWrapper(object? obj)
	{
		if (obj != null)
		{
			Marshal.Release(Marshal.GetIDispatchForObject(obj));
			WrappedObject = obj;
		}
	}
}
