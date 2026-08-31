using BEPUphysics.Threading;

namespace BEPUphysics.OtherSpaceStages;

/// <summary>
///  Thead-safely buffers up space objects for addition and removal.
/// </summary>
public class SpaceObjectBuffer : ProcessingStage
{
	private struct SpaceObjectChange(ISpaceObject spaceObject, bool shouldAdd)
	{
		public readonly ISpaceObject SpaceObject = spaceObject;

		public readonly bool ShouldAdd = shouldAdd;
	}

	private ConcurrentDeque<SpaceObjectChange> objectsToChange = new ConcurrentDeque<SpaceObjectChange>();

	private ISpace space;

	/// <summary>
	///  Gets the space which owns this buffer.
	/// </summary>
	public ISpace Space => space;

	/// <summary>
	///  Constructs the buffer.
	/// </summary>
	/// <param name="space">Space that owns the buffer.</param>
	public SpaceObjectBuffer(ISpace space)
	{
		Enabled = true;
		this.space = space;
	}

	/// <summary>
	///  Adds a space object to the buffer.
	///  It will be added to the space the next time the buffer is flushed.
	/// </summary>
	/// <param name="spaceObject">Space object to add.</param>
	public void Add(ISpaceObject spaceObject)
	{
		objectsToChange.Enqueue(new SpaceObjectChange(spaceObject, shouldAdd: true));
	}

	/// <summary>
	/// Enqueues a removal request to the buffer.
	/// It will be processed the next time the buffer is flushed.
	/// </summary>
	/// <param name="spaceObject">Space object to remove.</param>
	public void Remove(ISpaceObject spaceObject)
	{
		objectsToChange.Enqueue(new SpaceObjectChange(spaceObject, shouldAdd: false));
	}

	protected override void UpdateStage()
	{
		SpaceObjectChange item;
		while (objectsToChange.TryDequeueFirst(out item))
		{
			if (item.ShouldAdd)
			{
				space.Add(item.SpaceObject);
			}
			else
			{
				space.Remove(item.SpaceObject);
			}
		}
	}
}
