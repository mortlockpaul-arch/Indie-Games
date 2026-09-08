using System;

namespace Microsoft.CSharp.RuntimeBinder.Errors;

[Flags]
internal enum ErrArgFlags
{
	None = 0,
	NoStr = 2,
	Unique = 4,
	UseGetErrorInfo = 8
}
