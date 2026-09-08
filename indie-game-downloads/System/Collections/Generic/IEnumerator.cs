using System.Runtime.CompilerServices;

namespace System.Collections.Generic;

[Intrinsic]
public interface IEnumerator<out T> : IDisposable, IEnumerator where T : allows ref struct
{
	new T Current { get; }
}
