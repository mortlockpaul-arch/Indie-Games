using System;

namespace Microsoft.CSharp.RuntimeBinder;

[Flags]
internal enum CSharpCallFlags
{
	None = 0,
	SimpleNameCall = 1,
	EventHookup = 2,
	ResultDiscarded = 4
}
