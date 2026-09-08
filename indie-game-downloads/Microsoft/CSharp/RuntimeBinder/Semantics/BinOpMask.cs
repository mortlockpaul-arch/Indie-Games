using System;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

[Flags]
internal enum BinOpMask
{
	None = 0,
	Add = 1,
	Sub = 2,
	Mul = 4,
	Shift = 8,
	Equal = 0x10,
	Compare = 0x20,
	Bitwise = 0x40,
	BitXor = 0x80,
	Logical = 0x100,
	Integer = Add | Sub | Mul | Equal | Compare | Bitwise | BitXor,
	Real = 0x37,
	BoolNorm = 0x90,
	Delegate = 0x13,
	Enum = 0xF2,
	EnumUnder = 3,
	UnderEnum = Add
}
