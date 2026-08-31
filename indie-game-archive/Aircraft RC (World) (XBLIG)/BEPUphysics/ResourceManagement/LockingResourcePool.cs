using System;
using BEPUphysics.Threading;

namespace BEPUphysics.ResourceManagement;

/// <summary>
/// Uses a spinlock to safely access resources.
/// </summary>
/// <typeparam name="T">Type of object to store in the pool.</typeparam>
public class LockingResourcePool<T> : ResourcePool<T> where T : class, new()
{
	private readonly ConcurrentDeque<T> stack;

	/// <summary>
	/// Gets the number of resources in the pool.
	/// Even if the resource count hits 0, resources
	/// can still be requested; they will be allocated
	/// dynamically.
	/// </summary>
	public override int Count => stack.Count;

	/// <summary>
	/// Constructs a new thread-unsafe resource pool.
	/// </summary>
	/// <param name="initialResourceCount">Number of resources to include in the pool by default.</param>
	/// <param name="initializer">Function to initialize new instances in the resource pool with.</param>
	public LockingResourcePool(int initialResourceCount, Action<T> initializer)
	{
		base.InstanceInitializer = initializer;
		stack = new ConcurrentDeque<T>(initialResourceCount);
		Initialize(initialResourceCount);
	}

	/// <summary>
	/// Constructs a new thread-unsafe resource pool.
	/// </summary>
	/// <param name="initialResourceCount">Number of resources to include in the pool by default.</param>
	public LockingResourcePool(int initialResourceCount)
		: this(initialResourceCount, (Action<T>)null)
	{
	}

	/// <summary>
	/// Constructs a new thread-unsafe resource pool.
	/// </summary>
	public LockingResourcePool()
		: this(10)
	{
	}

	/// <summary>
	/// Gives an item back to the resource pool.
	/// </summary>
	/// <param name="item">Item to return.</param>
	public override void GiveBack(T item)
	{
		stack.Enqueue(item);
	}

	/// <summary>
	/// Initializes the pool with some resources.
	/// Throws away excess resources.
	/// </summary>
	/// <param name="initialResourceCount">Number of resources to include.</param>
	public override void Initialize(int initialResourceCount)
	{
		while (stack.Count > initialResourceCount)
		{
			stack.TryUnsafeDequeueFirst(out var _);
		}
		int num = stack.lastIndex - stack.firstIndex + 1;
		if (base.InstanceInitializer != null)
		{
			for (int i = 0; i < num; i++)
			{
				base.InstanceInitializer(stack.array[(stack.firstIndex + i) % stack.array.Length]);
			}
		}
		while (stack.Count < initialResourceCount)
		{
			stack.UnsafeEnqueue(CreateNewResource());
		}
	}

	/// <summary>
	/// Takes an item from the resource pool.
	/// </summary>
	/// <returns>Item to take.</returns>
	public override T Take()
	{
		if (stack.TryDequeueFirst(out var item))
		{
			return item;
		}
		return CreateNewResource();
	}

	/// <summary>
	/// Clears out the resource pool.
	/// </summary>
	public override void Clear()
	{
		while (stack.Count > 0)
		{
			stack.TryDequeueFirst(out var _);
		}
	}
}
