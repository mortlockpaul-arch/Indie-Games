using System;

namespace Internal.Runtime.InteropServices;

internal struct ComActivationContextInternal
{
	public Guid ClassId;

	public Guid InterfaceId;

	public unsafe char* AssemblyPathBuffer;

	public unsafe char* AssemblyNameBuffer;

	public unsafe char* TypeNameBuffer;

	public nint ClassFactoryDest;
}
