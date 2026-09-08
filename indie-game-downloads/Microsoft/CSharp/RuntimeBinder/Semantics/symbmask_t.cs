using System;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

[Flags]
internal enum symbmask_t : long
{
	MASK_NamespaceSymbol = 1L,
	MASK_AggregateSymbol = 2L,
	MASK_TypeParameterSymbol = 4L,
	MASK_FieldSymbol = 8L,
	MASK_MethodSymbol = 0x20L,
	MASK_PropertySymbol = 0x40L,
	MASK_EventSymbol = 0x80L,
	MASK_ALL = -1L,
	MASK_Member = 0xE8L
}
