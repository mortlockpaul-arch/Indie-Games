using System;
using System.Runtime.InteropServices;

namespace Internal.Runtime.InteropServices;

internal struct ComActivationContext
{
	public Guid ClassId;

	public Guid InterfaceId;

	public string AssemblyPath;

	public string AssemblyName;

	public string TypeName;

	public bool IsolatedContext;

	public unsafe static ComActivationContext Create(ref ComActivationContextInternal cxtInt, bool isolatedContext)
	{
		if (!Marshal.IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		return new ComActivationContext
		{
			ClassId = cxtInt.ClassId,
			InterfaceId = cxtInt.InterfaceId,
			AssemblyPath = Marshal.PtrToStringUni(new IntPtr(cxtInt.AssemblyPathBuffer)),
			AssemblyName = Marshal.PtrToStringUni(new IntPtr(cxtInt.AssemblyNameBuffer)),
			TypeName = Marshal.PtrToStringUni(new IntPtr(cxtInt.TypeNameBuffer)),
			IsolatedContext = isolatedContext
		};
	}
}
