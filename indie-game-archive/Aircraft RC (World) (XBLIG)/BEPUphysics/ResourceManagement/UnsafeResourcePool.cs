using System;
using System.Collections.Generic;

namespace BEPUphysics.ResourceManagement;

/// <summary>
/// Manages a resource type, but performs no locking to handle asynchronous access.
/// </summary>
/// <typeparam name="T">Type of object to store in the pool.</typeparam>
public class UnsafeResourcePool<T> : ResourcePool<T> where T : class, new()
{
	private readonly Stack<T> stack;

	/// <summary>
	/// Gets the number of resources in the pool.
	/// Even if the resource count hits 0, resources
	/// can still be requested; they will be allocated
	/// dynamically.
	/// </summary>
	public override int Count => stack.Count;

	/// <summary>
	/// Constructs a new locking resource pool.
	/// </summary>
	/// <param name="initialResourceCount">Number of resources to include in the pool by default.</param>
	/// <param name="initializer">Function to initialize new instances in the resource pool with.</param>
	public UnsafeResourcePool(int initialResourceCount, Action<T> initializer)
	{
		base.InstanceInitializer = initializer;
		stack = new Stack<T>(initialResourceCount);
		Initialize(initialResourceCount);
	}

	/// <summary>
	/// Constructs a new locking resource pool.
	/// </summary>
	/// <param name="initialResourceCount">Number of resources to include in the pool by default.</param>
	public UnsafeResourcePool(int initialResourceCount)
		: this(initialResourceCount, (Action<T>)null)
	{
	}

	/// <summary>
	/// Constructs a new locking resource pool.
	/// </summary>
	public UnsafeResourcePool()
		: this(10)
	{
	}

	/// <summary>
	/// Gives an item back to the resource pool.
	/// </summary>
	/// <param name="item">Item to return.</param>
	public override void GiveBack(T item)
	{
		stack.Push(item);
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
			stack.Pop();
		}
		if (base.InstanceInitializer != null)
		{
			foreach (T item in stack)
			{
				base.InstanceInitializer(item);
			}
		}
		while (stack.Count < initialResourceCount)
		{
			stack.Push(CreateNewResource());
		}
	}

	/// <summary>
	/// Takes an item from the resource pool.
	/// </summary>
	/// <returns>Item to take.</returns>
	public override T Take()
	{
		if (stack.Count > 0)
		{
			return stack.Pop();
		}
		return CreateNewResource();
	}

	/// <summary>
	/// Clears out the resource pool.
	/// </summary>
	public override void Clear()
	{
		stack.Clear();
	}
}
