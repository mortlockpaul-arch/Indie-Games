using System;

namespace Microsoft.CSharp.RuntimeBinder;

[Flags]
internal enum CSharpBinaryOperationFlags
{
	None = 0,
	MemberAccess = 1,
	LogicalOperation = 2
}
