using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Collidables.Events;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.CollisionShapes;
using BEPUphysics.DataStructures;
using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace BEPUphysics.Collidables;

/// <summary>
///  Superclass of objects living in the collision detection pipeline
///  that can result in contacts.
/// </summary>
public abstract class Collidable : BroadPhaseEntry
{
	internal CollisionShape shape;

	private Action<CollisionShape> shapeChangedDelegate;

	internal RawList<CollidablePairHandler> pairs = new RawList<CollidablePairHandler>();

	/// <summary>
	///  Gets the shape used by the collidable.
	/// </summary>
	public CollisionShape Shape
	{
		get
		{
			return shape;
		}
		protected set
		{
			if (shape != null)
			{
				shape.ShapeChanged -= shapeChangedDelegate;
			}
			shape = value;
			if (shape != null)
			{
				shape.ShapeChanged += shapeChangedDelegate;
			}
			OnShapeChanged(shape);
		}
	}

	protected internal abstract IContactEventTriggerer EventTriggerer { get; }

	/// <summary>
	/// Gets or sets whether or not to ignore shape changes.  When true, changing the collision shape will not force the collidable to perform any updates.
	/// </summary>
	public bool IgnoreShapeChanges { get; set; }

	/// <summary>
	///  Gets the list of pairs associated with the collidable.
	///  These pairs are found by the broad phase and are managed by the narrow phase;
	///  they can contain other collidables, entities, and contacts.
	/// </summary>
	public ReadOnlyList<CollidablePairHandler> Pairs => new ReadOnlyList<CollidablePairHandler>(pairs);

	/// <summary>
	///  Gets a list of all other collidables that this collidable overlaps.
	/// </summary>
	public CollidableCollection OverlappedCollidables => new CollidableCollection(this);

	protected Collidable()
	{
		shapeChangedDelegate = OnShapeChanged;
	}

	protected virtual void OnShapeChanged(CollisionShape collisionShape)
	{
	}

	protected override void CollisionRulesUpdated()
	{
		for (int i = 0; i < pairs.Count; i++)
		{
			pairs[i].CollisionRule = CollisionRules.CollisionRuleCalculator(pairs[i].BroadPhaseOverlap.entryA, pairs[i].BroadPhaseOverlap.entryB);
		}
	}

	internal void AddPair(CollidablePairHandler pair, ref int index)
	{
		index = pairs.count;
		pairs.Add(pair);
	}

	internal void RemovePair(CollidablePairHandler pair, ref int index)
	{
		if (pairs.count > index)
		{
			pairs.FastRemoveAt(index);
			if (pairs.count > index)
			{
				CollidablePairHandler collidablePairHandler = pairs.Elements[index];
				if (collidablePairHandler.CollidableA == this)
				{
					collidablePairHandler.listIndexA = index;
				}
				else
				{
					collidablePairHandler.listIndexB = index;
				}
			}
		}
		index = -1;
	}
}
