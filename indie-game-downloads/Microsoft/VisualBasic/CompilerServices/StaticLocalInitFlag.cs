using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.VisualBasic.CompilerServices;

[DebuggerNonUserCode]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class StaticLocalInitFlag
{
	public short State;
}
