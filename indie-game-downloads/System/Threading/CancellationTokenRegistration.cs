using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace System.Threading;

public readonly struct CancellationTokenRegistration : IEquatable<CancellationTokenRegistration>, IDisposable, IAsyncDisposable
{
	private readonly long _id;

	private readonly CancellationTokenSource.CallbackNode _node;

	public CancellationToken Token
	{
		get
		{
			CancellationTokenSource.CallbackNode node = _node;
			if (node == null)
			{
				return default(CancellationToken);
			}
			return new CancellationToken(node.Registrations.Source);
		}
	}

	internal CancellationTokenRegistration(long id, CancellationTokenSource.CallbackNode node)
	{
		_id = id;
		_node = node;
	}

	public void Dispose()
	{
		CancellationTokenSource.CallbackNode node = _node;
		if (node != null && !node.Registrations.Unregister(_id, node))
		{
			WaitForCallbackIfNecessary(_id, node);
		}
		static void WaitForCallbackIfNecessary(long id, CancellationTokenSource.CallbackNode callbackNode)
		{
			CancellationTokenSource source = callbackNode.Registrations.Source;
			if (source.IsCancellationRequested && !source.IsCancellationCompleted && callbackNode.Registrations.ThreadIDExecutingCallbacks != Environment.CurrentManagedThreadId)
			{
				callbackNode.Registrations.WaitForCallbackToComplete(id);
			}
		}
	}

	public ValueTask DisposeAsync()
	{
		CancellationTokenSource.CallbackNode node = _node;
		if (node == null || node.Registrations.Unregister(_id, node))
		{
			return default(ValueTask);
		}
		return WaitForCallbackIfNecessaryAsync(_id, node);
		static ValueTask WaitForCallbackIfNecessaryAsync(long id, CancellationTokenSource.CallbackNode callbackNode)
		{
			CancellationTokenSource source = callbackNode.Registrations.Source;
			if (source.IsCancellationRequested && !source.IsCancellationCompleted && callbackNode.Registrations.ThreadIDExecutingCallbacks != Environment.CurrentManagedThreadId)
			{
				return callbackNode.Registrations.WaitForCallbackToCompleteAsync(id);
			}
			return default(ValueTask);
		}
	}

	public bool Unregister()
	{
		CancellationTokenSource.CallbackNode node = _node;
		return node?.Registrations.Unregister(_id, node) ?? false;
	}

	public static bool operator ==(CancellationTokenRegistration left, CancellationTokenRegistration right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(CancellationTokenRegistration left, CancellationTokenRegistration right)
	{
		return !left.Equals(right);
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is CancellationTokenRegistration other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(CancellationTokenRegistration other)
	{
		if (_node == other._node)
		{
			return _id == other._id;
		}
		return false;
	}

	public override int GetHashCode()
	{
		if (_node == null)
		{
			return _id.GetHashCode();
		}
		return _node.GetHashCode() ^ _id.GetHashCode();
	}
}
