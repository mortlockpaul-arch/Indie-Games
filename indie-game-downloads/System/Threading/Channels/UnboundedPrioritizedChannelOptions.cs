using System.Collections.Generic;

namespace System.Threading.Channels;

public sealed class UnboundedPrioritizedChannelOptions<T> : ChannelOptions
{
	public IComparer<T>? Comparer { get; set; }
}
