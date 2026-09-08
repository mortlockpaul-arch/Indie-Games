using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace System.Threading.Channels;

internal static class ChannelUtilities
{
	internal static readonly Exception s_doneWritingSentinel = new Exception("s_doneWritingSentinel");

	internal static readonly Task<bool> s_trueTask = Task.FromResult(result: true);

	internal static readonly Task<bool> s_falseTask = Task.FromResult(result: false);

	internal static readonly Task s_neverCompletingTask = new TaskCompletionSource<bool>().Task;

	internal static void Complete(TaskCompletionSource tcs, Exception error = null)
	{
		if (error is OperationCanceledException ex)
		{
			tcs.TrySetCanceled(ex.CancellationToken);
		}
		else if (error != null && error != s_doneWritingSentinel)
		{
			if (tcs.TrySetException(error))
			{
				_ = tcs.Task.Exception;
			}
		}
		else
		{
			tcs.TrySetResult();
		}
	}

	internal static ValueTask<T> GetInvalidCompletionValueTask<T>(Exception error)
	{
		return new ValueTask<T>((error == s_doneWritingSentinel) ? Task.FromException<T>(CreateInvalidCompletionException()) : ((error is OperationCanceledException { CancellationToken: var cancellationToken } ex) ? Task.FromCanceled<T>(cancellationToken.IsCancellationRequested ? ex.CancellationToken : new CancellationToken(canceled: true)) : Task.FromException<T>(CreateInvalidCompletionException(error))));
	}

	internal static TAsyncOp TryDequeueAndReserveCompletionIfCancelable<TAsyncOp>(ref TAsyncOp head) where TAsyncOp : AsyncOperation<TAsyncOp>
	{
		TAsyncOp op;
		while (TryDequeue(ref head, out op))
		{
			if (op.TryReserveCompletionIfCancelable())
			{
				return op;
			}
		}
		return null;
	}

	internal static bool TryDequeue<TAsyncOp>(ref TAsyncOp head, [NotNullWhen(true)] out TAsyncOp op) where TAsyncOp : AsyncOperation<TAsyncOp>
	{
		op = head;
		if (head == null)
		{
			return false;
		}
		if (head.Next == head)
		{
			head = null;
		}
		else
		{
			TAsyncOp previous = head.Previous;
			head = head.Next;
			head.Previous = previous;
			previous.Next = head;
		}
		TAsyncOp val = op;
		TAsyncOp next = (op.Previous = null);
		val.Next = next;
		return true;
	}

	internal static void Enqueue<TAsyncOp>(ref TAsyncOp head, TAsyncOp op) where TAsyncOp : AsyncOperation<TAsyncOp>
	{
		if (head == null)
		{
			TAsyncOp val = (op.Previous = op);
			TAsyncOp val3 = (op.Next = val);
			head = val3;
		}
		else
		{
			TAsyncOp previous = head.Previous;
			op.Next = head;
			op.Previous = previous;
			previous.Next = op;
			head.Previous = op;
		}
	}

	internal static void Remove<TAsyncOp>(ref TAsyncOp head, TAsyncOp op) where TAsyncOp : AsyncOperation<TAsyncOp>
	{
		if (head == null || op.Next == null)
		{
			return;
		}
		if (op.Next == op)
		{
			head = null;
		}
		else
		{
			op.Previous.Next = op.Next;
			op.Next.Previous = op.Previous;
			if (head == op)
			{
				head = op.Next;
			}
		}
		TAsyncOp next = (op.Previous = null);
		op.Next = next;
	}

	internal static void SetOrFailOperations<TAsyncOp, T>(TAsyncOp head, T result, Exception error = null) where TAsyncOp : AsyncOperation<TAsyncOp, T>
	{
		if (error != null)
		{
			FailOperations(head, error);
		}
		else
		{
			SetOperations(ref head, result);
		}
	}

	internal static void SetOperations<TAsyncOp, TResult>(ref TAsyncOp head, TResult result) where TAsyncOp : AsyncOperation<TAsyncOp, TResult>
	{
		TAsyncOp val = head;
		if (val != null)
		{
			do
			{
				TAsyncOp next = val.Next;
				TAsyncOp val2 = val;
				TAsyncOp next2 = (val.Previous = null);
				val2.Next = next2;
				val.TrySetResult(result);
				val = next;
			}
			while (val != head);
			head = null;
		}
	}

	internal static void DangerousSetOperations<TAsyncOp, TResult>(TAsyncOp head, TResult result) where TAsyncOp : AsyncOperation<TAsyncOp, TResult>
	{
		TAsyncOp val = head;
		if (val != null)
		{
			do
			{
				TAsyncOp next = val.Next;
				TAsyncOp val2 = val;
				TAsyncOp next2 = (val.Previous = null);
				val2.Next = next2;
				val.DangerousSetResult(result);
				val = next;
			}
			while (val != head);
		}
	}

	internal static TAsyncOp TryReserveCompletionIfCancelable<TAsyncOp>(ref TAsyncOp head) where TAsyncOp : AsyncOperation<TAsyncOp>
	{
		TAsyncOp head2 = null;
		TAsyncOp val = head;
		if (val != null)
		{
			do
			{
				TAsyncOp next = val.Next;
				TAsyncOp val2 = val;
				TAsyncOp next2 = (val.Previous = null);
				val2.Next = next2;
				if (val.TryReserveCompletionIfCancelable())
				{
					Enqueue(ref head2, val);
				}
				val = next;
			}
			while (val != head);
			head = null;
		}
		return head2;
	}

	internal static void FailOperations<TAsyncOp>(TAsyncOp head, Exception error) where TAsyncOp : AsyncOperation<TAsyncOp>
	{
		TAsyncOp val = head;
		if (val != null)
		{
			do
			{
				TAsyncOp next = val.Next;
				TAsyncOp val2 = val;
				TAsyncOp next2 = (val.Previous = null);
				val2.Next = next2;
				val.TrySetException(error);
				val = next;
			}
			while (val != head);
		}
	}

	internal static long CountOperations<TAsyncOp>(TAsyncOp head) where TAsyncOp : AsyncOperation<TAsyncOp>
	{
		TAsyncOp val = head;
		long num = 0L;
		if (val != null)
		{
			do
			{
				num++;
				val = val.Next;
			}
			while (val != head);
		}
		return num;
	}

	internal static Exception CreateInvalidCompletionException(Exception inner = null)
	{
		if (!(inner is OperationCanceledException))
		{
			if (inner == null || inner == s_doneWritingSentinel)
			{
				return new ChannelClosedException();
			}
			return new ChannelClosedException(inner);
		}
		return inner;
	}

	internal static void UnsafeQueueUserWorkItem<TState>(Action<TState> action, TState state)
	{
		ThreadPool.UnsafeQueueUserWorkItem(action, state, preferLocal: false);
	}

	internal static void QueueUserWorkItem(Action<object> action, object state)
	{
		ThreadPool.QueueUserWorkItem(action, state, preferLocal: false);
	}
}
