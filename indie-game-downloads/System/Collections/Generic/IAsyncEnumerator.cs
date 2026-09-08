using System.Threading.Tasks;

namespace System.Collections.Generic;

public interface IAsyncEnumerator<out T> : IAsyncDisposable where T : allows ref struct
{
	T Current { get; }

	ValueTask<bool> MoveNextAsync();
}
