using System.Diagnostics.CodeAnalysis;

namespace System.Net.Http;

internal struct RequestQueue<T> where T : HttpConnectionBase
{
	public struct QueueItem
	{
		public HttpRequestMessage Request;

		public HttpConnectionWaiter<T> Waiter;
	}

	private QueueItem[] _array = Array.Empty<QueueItem>();

	private int _head = 0;

	private int _tail = 0;

	private int _size = 0;

	private int _attemptedConnectionsOffset = 0;

	public int Count => _size;

	public int RequestsWithoutAConnectionAttempt => _size - _attemptedConnectionsOffset;

	public RequestQueue()
	{
	}

	private void Enqueue(QueueItem queueItem)
	{
		if (_size == _array.Length)
		{
			Grow();
		}
		_array[_tail] = queueItem;
		MoveNext(ref _tail);
		_size++;
	}

	private QueueItem Dequeue()
	{
		int head = _head;
		QueueItem[] array = _array;
		QueueItem result = array[head];
		array[head] = default(QueueItem);
		MoveNext(ref _head);
		if (_attemptedConnectionsOffset > 0)
		{
			_attemptedConnectionsOffset--;
		}
		_size--;
		return result;
	}

	private bool TryPeek(out QueueItem queueItem)
	{
		if (_size == 0)
		{
			queueItem = default(QueueItem);
			return false;
		}
		queueItem = _array[_head];
		return true;
	}

	private void MoveNext(ref int index)
	{
		int num = index + 1;
		if (num == _array.Length)
		{
			num = 0;
		}
		index = num;
	}

	private void Grow()
	{
		QueueItem[] array = new QueueItem[Math.Max(4, _array.Length * 2)];
		if (_size != 0)
		{
			if (_head < _tail)
			{
				Array.Copy(_array, _head, array, 0, _size);
			}
			else
			{
				Array.Copy(_array, _head, array, 0, _array.Length - _head);
				Array.Copy(_array, 0, array, _array.Length - _head, _tail);
			}
		}
		_array = array;
		_head = 0;
		_tail = _size;
	}

	public HttpConnectionWaiter<T> EnqueueRequest(HttpRequestMessage request)
	{
		HttpConnectionWaiter<T> httpConnectionWaiter = new HttpConnectionWaiter<T>();
		EnqueueRequest(request, httpConnectionWaiter);
		return httpConnectionWaiter;
	}

	public void EnqueueRequest(HttpRequestMessage request, HttpConnectionWaiter<T> waiter)
	{
		Enqueue(new QueueItem
		{
			Request = request,
			Waiter = waiter
		});
	}

	public void PruneCompletedRequestsFromHeadOfQueue(HttpConnectionPool pool)
	{
		QueueItem queueItem;
		while (TryPeek(out queueItem) && queueItem.Waiter.Task.IsCompleted)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				pool.Trace(queueItem.Waiter.Task.IsCanceled ? "Discarding canceled request from queue." : "Discarding signaled request waiter from queue.", "PruneCompletedRequestsFromHeadOfQueue");
			}
			Dequeue();
		}
	}

	public bool TryDequeueWaiter(HttpConnectionPool pool, [MaybeNullWhen(false)] out HttpConnectionWaiter<T> waiter)
	{
		PruneCompletedRequestsFromHeadOfQueue(pool);
		if (Count != 0)
		{
			waiter = Dequeue().Waiter;
			return true;
		}
		waiter = null;
		return false;
	}

	public void TryDequeueSpecificWaiter(HttpConnectionWaiter<T> waiter)
	{
		if (TryPeek(out var queueItem) && queueItem.Waiter == waiter)
		{
			Dequeue();
		}
	}

	public QueueItem PeekNextRequestForConnectionAttempt()
	{
		int num = _head + _attemptedConnectionsOffset;
		_attemptedConnectionsOffset++;
		if (num >= _array.Length)
		{
			num -= _array.Length;
		}
		return _array[num];
	}
}
