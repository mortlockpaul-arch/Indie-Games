using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.VisualBasic.CompilerServices;

[DebuggerNonUserCode]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class IncompleteInitialization : Exception
{
}
