namespace System.Threading.Channels;

public static class Channel
{
	public static Channel<T> CreateUnbounded<T>()
	{
		return new UnboundedChannel<T>(runContinuationsAsynchronously: true);
	}

	public static Channel<T> CreateUnbounded<T>(UnboundedChannelOptions options)
	{
		ArgumentNullException.ThrowIfNull(options, "options");
		if (options.SingleReader)
		{
			return new SingleConsumerUnboundedChannel<T>(!options.AllowSynchronousContinuations);
		}
		return new UnboundedChannel<T>(!options.AllowSynchronousContinuations);
	}

	public static Channel<T> CreateBounded<T>(int capacity)
	{
		if (capacity <= 0)
		{
			if (capacity != 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			return new RendezvousChannel<T>(BoundedChannelFullMode.Wait, runContinuationsAsynchronously: true, null);
		}
		return new BoundedChannel<T>(capacity, BoundedChannelFullMode.Wait, runContinuationsAsynchronously: true, null);
	}

	public static Channel<T> CreateBounded<T>(BoundedChannelOptions options)
	{
		return CreateBounded<T>(options, null);
	}

	public static Channel<T> CreateBounded<T>(BoundedChannelOptions options, Action<T>? itemDropped)
	{
		ArgumentNullException.ThrowIfNull(options, "options");
		if (options.Capacity <= 0)
		{
			return new RendezvousChannel<T>(options.FullMode, !options.AllowSynchronousContinuations, itemDropped);
		}
		return new BoundedChannel<T>(options.Capacity, options.FullMode, !options.AllowSynchronousContinuations, itemDropped);
	}

	public static Channel<T> CreateUnboundedPrioritized<T>()
	{
		return new UnboundedPrioritizedChannel<T>(runContinuationsAsynchronously: true, null);
	}

	public static Channel<T> CreateUnboundedPrioritized<T>(UnboundedPrioritizedChannelOptions<T> options)
	{
		ArgumentNullException.ThrowIfNull(options, "options");
		return new UnboundedPrioritizedChannel<T>(!options.AllowSynchronousContinuations, options.Comparer);
	}
}
public abstract class Channel<T> : Channel<T, T>
{
}
public abstract class Channel<TWrite, TRead>
{
	public ChannelReader<TRead> Reader { get; protected set; }

	public ChannelWriter<TWrite> Writer { get; protected set; }

	public static implicit operator ChannelReader<TRead>(Channel<TWrite, TRead> channel)
	{
		return channel.Reader;
	}

	public static implicit operator ChannelWriter<TWrite>(Channel<TWrite, TRead> channel)
	{
		return channel.Writer;
	}
}
