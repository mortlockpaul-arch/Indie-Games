using System.Collections.Generic;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.DataStructures;

namespace BEPUphysics.NarrowPhaseSystems.Pairs;

/// <summary>
///  Superclass of pairs between collidables that generate contact points.
/// </summary>
public abstract class DetectorVolumeGroupPairHandler : DetectorVolumePairHandler, IDetectorVolumePairHandlerParent
{
	private Dictionary<EntityCollidable, DetectorVolumePairHandler> subPairs = new Dictionary<EntityCollidable, DetectorVolumePairHandler>();

	private BEPUphysics.DataStructures.HashSet<EntityCollidable> containedPairs = new BEPUphysics.DataStructures.HashSet<EntityCollidable>();

	private RawList<EntityCollidable> pairsToRemove = new RawList<EntityCollidable>();

	/// <summary>
	/// Gets a read-only dictionary of collidables associated with this group pair handler all the subpairs associated with them.
	/// </summary>
	public ReadOnlyDictionary<EntityCollidable, DetectorVolumePairHandler> Pairs => new ReadOnlyDictionary<EntityCollidable, DetectorVolumePairHandler>(subPairs);

	/// <summary>
	///  Called when the pair handler is added to the narrow phase.
	/// </summary>
	protected internal override void OnAddedToNarrowPhase()
	{
		base.DetectorVolume.pairs.Add(Collidable.entity, this);
	}

	/// <summary>
	///  Cleans up the pair handler.
	/// </summary>
	public override void CleanUp()
	{
		foreach (DetectorVolumePairHandler value in subPairs.Values)
		{
			value.CleanUp();
		}
		subPairs.Clear();
		base.CleanUp();
	}

	protected void TryToAdd(EntityCollidable collidable)
	{
		CollisionRule collisionRule;
		if ((collisionRule = CollisionRules.collisionRuleCalculator(base.DetectorVolume, collidable)) < CollisionRule.NoNarrowPhasePair)
		{
			if (collisionRule < base.CollisionRule)
			{
				collisionRule = base.CollisionRule;
			}
			if (!subPairs.ContainsKey(collidable) && NarrowPhaseHelper.GetPairHandler(base.DetectorVolume, collidable, collisionRule) is DetectorVolumePairHandler detectorVolumePairHandler)
			{
				detectorVolumePairHandler.Parent = this;
				subPairs.Add(collidable, detectorVolumePairHandler);
			}
			containedPairs.Add(collidable);
		}
	}

	protected abstract void UpdateContainedPairs();

	public override void UpdateCollision(float dt)
	{
		base.WasContaining = base.Containing;
		base.WasTouching = base.Touching;
		UpdateContainedPairs();
		foreach (EntityCollidable key in subPairs.Keys)
		{
			if (!containedPairs.Contains(key))
			{
				pairsToRemove.Add(key);
			}
		}
		for (int i = 0; i < pairsToRemove.count; i++)
		{
			DetectorVolumePairHandler detectorVolumePairHandler = subPairs[pairsToRemove.Elements[i]];
			subPairs.Remove(pairsToRemove.Elements[i]);
			detectorVolumePairHandler.CleanUp();
			detectorVolumePairHandler.Factory.GiveBack(detectorVolumePairHandler);
		}
		containedPairs.Clear();
		pairsToRemove.Clear();
		base.Touching = false;
		base.Containing = subPairs.Count > 0;
		foreach (DetectorVolumePairHandler value in subPairs.Values)
		{
			if (value is DetectorVolumeConvexPairHandler detectorVolumeConvexPairHandler)
			{
				detectorVolumeConvexPairHandler.CheckContainment = base.Containing || !base.Touching;
			}
			value.UpdateCollision(dt);
			if (value.Touching)
			{
				base.Touching = true;
			}
			else
			{
				base.Containing = false;
			}
			if (!value.Containing)
			{
				base.Containing = false;
			}
			if (!base.Containing && base.Touching)
			{
				break;
			}
		}
		NotifyDetectorVolumeOfChanges();
	}
}
