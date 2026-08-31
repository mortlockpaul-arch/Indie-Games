using System;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using BEPUphysics.Threading;
using Microsoft.Xna.Framework;

namespace BEPUphysics.EntityStateManagement;

/// <summary>
///  Manages the interpolated states of entities.  Interpolated states are those
///  based on the previous entity states and the current entity states, blended together
///  using the time remainder from internal time stepping.
/// </summary>
public class InterpolatedStatesManager : MultithreadedProcessingStage
{
	private BufferedStatesManager manager;

	private RigidTransform[] backBuffer;

	private RigidTransform[] states = new RigidTransform[64];

	private float blendAmount;

	private Action<int> multithreadedWithReadBuffersDelegate;

	/// <summary>
	///  Gets or sets whether or not the manager is updating.
	/// </summary>
	/// <exception cref="T:System.InvalidOperationException">Thrown when enabling the interpolated manager without having the read buffers active.</exception>
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
				Disable();
				base.Enabled = false;
			}
			else if (!base.Enabled && value)
			{
				if (!manager.ReadBuffers.Enabled)
				{
					throw new InvalidOperationException("Cannot enable interpolated states unless the read buffers are enabled.");
				}
				Enable();
				base.Enabled = true;
			}
		}
	}

	/// <summary>
	///  Gets the synchronization object locked prior to flipping the internal buffers.
	///  Acquiring a lock on this object will prevent the internal buffers from flipping for the duration
	///  of the lock.
	/// </summary>
	public object FlipLocker { get; private set; }

	/// <summary>
	///  Gets or sets the blending amount to use.
	///  This is set automatically when the space is using internal timestepping
	///  (I.E. when Space.Update(dt) is called).  It is a value from 0 to 1
	///  that defines the amount of the previous and current frames to include
	///  in the blended state.  A value of 1 means use only the current frame;
	///  a value of 0 means use only the previous frame.
	/// </summary>
	public float BlendAmount
	{
		get
		{
			return blendAmount;
		}
		set
		{
			blendAmount = MathHelper.Clamp(value, 0f, 1f);
		}
	}

	internal void Enable()
	{
		lock (FlipLocker)
		{
			int num = Math.Max(manager.entities.Count, 64);
			backBuffer = new RigidTransform[num];
			states = new RigidTransform[num];
			for (int i = 0; i < manager.entities.Count; i++)
			{
				Entity entity = manager.entities[i];
				backBuffer[i].Position = entity.position;
				backBuffer[i].Orientation = entity.orientation;
			}
			Array.Copy(backBuffer, states, backBuffer.Length);
		}
	}

	internal void Disable()
	{
		lock (FlipLocker)
		{
			backBuffer = null;
			states = null;
		}
	}

	/// <summary>
	///  Constructs a new interpolated states manager.
	/// </summary>
	/// <param name="manager">Owning buffered states manager.</param>
	public InterpolatedStatesManager(BufferedStatesManager manager)
	{
		this.manager = manager;
		multithreadedWithReadBuffersDelegate = UpdateIndex;
		FlipLocker = new object();
	}

	/// <summary>
	///  Constructs a new interpolated states manager.
	/// </summary>
	/// <param name="manager">Owning buffered states manager.</param>
	///  <param name="threadManager">Thread manager to use.</param>
	public InterpolatedStatesManager(BufferedStatesManager manager, IThreadManager threadManager)
	{
		this.manager = manager;
		multithreadedWithReadBuffersDelegate = UpdateIndex;
		FlipLocker = new object();
		base.ThreadManager = threadManager;
		base.AllowMultithreading = true;
	}

	private void UpdateIndex(int i)
	{
		Entity entity = manager.entities[i];
		Vector3.Lerp(ref manager.ReadBuffers.backBuffer[i].Position, ref entity.position, blendAmount, out backBuffer[i].Position);
		Quaternion.Slerp(ref manager.ReadBuffers.backBuffer[i].Orientation, ref entity.orientation, blendAmount, out backBuffer[i].Orientation);
	}

	protected override void UpdateMultithreaded()
	{
		base.ThreadManager.ForLoop(0, manager.entities.Count, multithreadedWithReadBuffersDelegate);
		FlipBuffers();
	}

	protected override void UpdateSingleThreaded()
	{
		for (int i = 0; i < manager.entities.Count; i++)
		{
			UpdateIndex(i);
		}
		FlipBuffers();
	}

	/// <summary>
	///  Acquires a lock on the FlipLocker and flips the internal buffers.
	/// </summary>
	public void FlipBuffers()
	{
		lock (FlipLocker)
		{
			RigidTransform[] array = states;
			states = backBuffer;
			backBuffer = array;
		}
	}

	/// <summary>
	///  Returns an interpolated state associated with an entity with the given index.
	///  Does not lock the FlipLocker.
	/// </summary>
	/// <param name="motionStateIndex">Motion state of the entity.</param>
	/// <returns>Interpolated state associated with the entity at the given index.</returns>
	public RigidTransform GetState(int motionStateIndex)
	{
		return states[motionStateIndex];
	}

	/// <summary>
	///  Gets the interpolated states of all entities.
	/// </summary>
	/// <param name="states">Interpolated states of all entities.</param>
	/// <exception cref="T:System.InvalidOperationException">Thrown when the array is too small to hold the states.</exception>
	public void GetStates(RigidTransform[] states)
	{
		lock (FlipLocker)
		{
			if (states.Length < manager.entities.Count)
			{
				throw new ArgumentException("Array is not large enough to hold the buffer.", "states");
			}
			Array.Copy(this.states, states, manager.entities.Count);
		}
	}

	internal void Add(Entity e)
	{
		if (states.Length <= e.BufferedStates.motionStateIndex)
		{
			RigidTransform[] array = new RigidTransform[states.Length * 2];
			states.CopyTo(array, 0);
			states = array;
		}
		states[e.BufferedStates.motionStateIndex].Position = e.position;
		states[e.BufferedStates.motionStateIndex].Orientation = e.orientation;
		if (backBuffer.Length <= e.BufferedStates.motionStateIndex)
		{
			RigidTransform[] array2 = new RigidTransform[backBuffer.Length * 2];
			backBuffer.CopyTo(array2, 0);
			backBuffer = array2;
		}
		backBuffer[e.BufferedStates.motionStateIndex].Position = e.position;
		backBuffer[e.BufferedStates.motionStateIndex].Orientation = e.orientation;
	}

	internal void Remove(int index, int endIndex)
	{
		ref RigidTransform reference = ref states[index];
		reference = states[endIndex];
		ref RigidTransform reference2 = ref backBuffer[index];
		reference2 = backBuffer[endIndex];
	}
}
