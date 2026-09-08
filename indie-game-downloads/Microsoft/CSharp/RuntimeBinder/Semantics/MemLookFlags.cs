using System;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

[Flags]
internal enum MemLookFlags : uint
{
	None = 0u,
	Ctor = 2u,
	NewObj = 0x10u,
	Operator = 8u,
	Indexer = 4u,
	UserCallable = 0x100u,
	BaseCall = 0x40u,
	MustBeInvocable = 0x20000000u,
	All = Ctor | NewObj | Operator | Indexer | UserCallable | BaseCall | MustBeInvocable
}
