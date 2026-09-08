using System.Threading;

namespace System.Collections.Generic;

public interface IAsyncEnumerable<out T> where T : allows ref struct
{
	IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken));
}
