using System;
using BEPUphysics.Entities;
using BEPUphysics.Threading;

namespace BEPUphysics.EntityStateManagement;

/// <summary>
///  Manages the buffered states of entities.
/// </summary>
public class StateReadBuffers : MultithreadedProcessingStage
{
	private BufferedStatesManager manager;

	internal MotionState[] backBuffer;

	internal MotionState[] frontBuffer;

	private Action<int> multithreadedStateUpdateDelegate;

	/// <summary>
	///  Gets or sets whether or not the buffers are active.
	/// </summary>
	/// <exception cref="T:System.InvalidOperationException">Thrown if the read buffers are disabled while the interpolated states are enabled.</exception>
	public override bool Enabled
	{
		get
		{
			return base.Enabled;
		}
		set
		{
			if (base.Enabled && !value)
			{
				if (!manager.InterpolatedStates.Enabled)
				{
					throw new InvalidOperationException("Cannot disable read buffers unless the interpolated states are disabled.");
				}
				Disable();
				base.Enabled = false;
			}
			else if (!base.Enabled && value)
			{
				Enable();
				base.Enabled = true;
			}
		}
	}

	/// <summary>
	///  Gets the synchronization object which is locked during internal buffer flips.
	///  Acquiring a lock on this object will prevent the manager from flipping the buffers
	///  for the duration of the lock.
	/// </summary>
	public object FlipLocker { get; private set; }

	internal void Enable()
	{
		lock (FlipLocker)
		{
			int num = Math.Max(manager.entities.Count, 64);
			backBuffer = new MotionState[num];
			frontBuffer = new MotionState[num];
			for (int i = 0; i < manager.entities.Count; i++)
			{
				Entity entity = manager.entities[i];
				backBuffer[i].Position = entity.position;
				backBuffer[i].Orientation = entity.orientation;
				backBuffer[i].LinearVelocity = entity.linearVelocity;
				backBuffer[i].AngularVelocity = entity.angularVelocity;
			}
			Array.Copy(backBuffer, frontBuffer, backBuffer.Length);
		}
	}

	internal void Disable()
	{
		lock (FlipLocker)
		{
			backBuffer = null;
			frontBuffer = null;
		}
	}

	/// <summary>
	///  Constructs a read buffer manager.
	/// </summary>
	/// <param name="manager">Owning buffered states manager.</param>
	public StateReadBuffers(BufferedStatesManager manager)
	{
		this.manager = manager;
		multithreadedStateUpdateDelegate = MultithreadedStateUpdate;
		FlipLocker = new object();
	}

	/// <summary>
	///  Constructs a read buffer manager.
	/// </summary>
	/// <param name="manager">Owning buffered states manager.</param>
	/// <param name="threadManager">Thread manager to use.</param>
	public StateReadBuffers(BufferedStatesManager manager, IThreadManager threadManager)
	{
		this.manager = manager;
		multithreadedStateUpdateDelegate = MultithreadedStateUpdate;
		FlipLocker = new object();
		base.ThreadManager = threadManager;
		base.AllowMultithreading = true;
	}

	private void MultithreadedStateUpdate(int i)
	{
		Entity entity = manager.entities[i];
		backBuffer[i].Position = entity.position;
		backBuffer[i].Orientation = entity.orientation;
		backBuffer[i].LinearVelocity = entity.linearVelocity;
		backBuffer[i].AngularVelocity = entity.angularVelocity;
	}

	protected override void UpdateMultithreaded()
	{
		base.ThreadManager.ForLoop(0, manager.entities.Count, multithreadedStateUpdateDelegate);
		FlipBuffers();
	}

	protected override void UpdateSingleThreaded()
	{
		for (int i = 0; i < manager.entities.Count; i++)
		{
			Entity entity = manager.entities[i];
			backBuffer[i].Position = entity.position;
			backBuffer[i].Orientation = entity.orientation;
			backBuffer[i].LinearVelocity = entity.linearVelocity;
			backBuffer[i].AngularVelocity = entity.angularVelocity;
		}
		FlipBuffers();
	}

	internal void Add(Entity e)
	{
		if (frontBuffer.Length <= e.BufferedStates.motionStateIndex)
		{
			MotionState[] array = new MotionState[frontBuffer.Length * 2];
			frontBuffer.CopyTo(array, 0);
			frontBuffer = array;
		}
		frontBuffer[e.BufferedStates.motionStateIndex].Position = e.position;
		frontBuffer[e.BufferedStates.motionStateIndex].Orientation = e.orientation;
		if (backBuffer.Length <= e.BufferedStates.motionStateIndex)
		{
			MotionState[] array2 = new MotionState[backBuffer.Length * 2];
			backBuffer.CopyTo(array2, 0);
			backBuffer = array2;
		}
		backBuffer[e.BufferedStates.motionStateIndex].Position = e.position;
		backBuffer[e.BufferedStates.motionStateIndex].Orientation = e.orientation;
	}

	internal void Remove(int index, int endIndex)
	{
		ref MotionState reference = ref frontBuffer[index];
		reference = frontBuffer[endIndex];
		ref MotionState reference2 = ref backBuffer[index];
		reference2 = backBuffer[endIndex];
	}

	/// <summary>
	///  Acquires a lock on the FlipLocker and forces the internal buffers to flip.
	/// </summary>
	public void FlipBuffers()
	{
		lock (FlipLocker)
		{
			MotionState[] array = frontBuffer;
			frontBuffer = backBuffer;
			backBuffer = array;
		}
	}

	/// <summary>
	///  Gets the state of the entity associated with the given index.
	///  Does not lock the FlipLocker.
	/// </summary>
	/// <param name="motionStateIndex">Index of the entity.</param>
	/// <returns>MotionState of the entity at the index.</returns>
	public MotionState GetState(int motionStateIndex)
	{
		return frontBuffer[motionStateIndex];
	}

	/// <summary>
	///  Gets the states of all entities atomically.
	/// </summary>
	/// <param name="states">Entity states.</param>
	/// <exception cref="T:System.InvalidOperationException">Thrown when the array is too small.</exception>
	public void GetStates(MotionState[] states)
	{
		lock (FlipLocker)
		{
			if (states.Length < manager.entities.Count)
			{
				throw new ArgumentException("Array is not large enough to hold the buffer.", "states");
			}
			Array.Copy(frontBuffer, states, manager.entities.Count);
		}
	}
}
