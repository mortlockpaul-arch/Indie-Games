using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseSystems;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionTests.CollisionAlgorithms.GJK;
using BEPUphysics.PositionUpdating;
using BEPUphysics.Settings;
using Microsoft.Xna.Framework;

namespace BEPUphysics.NarrowPhaseSystems.Pairs;

/// <summary>
///  Pair handler that manages a pair of two boxes.
/// </summary>
public abstract class ConvexPairHandler : StandardPairHandler
{
	public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
	{
		UpdateMaterialProperties();
		base.Initialize(entryA, entryB);
	}

	/// <summary>
	///  Updates the time of impact for the pair.
	/// </summary>
	/// <param name="requester">Collidable requesting the update.</param>
	/// <param name="dt">Timestep duration.</param>
	public override void UpdateTimeOfImpact(Collidable requester, float dt)
	{
		ConvexCollidable convexCollidable = CollidableA as ConvexCollidable;
		ConvexCollidable convexCollidable2 = CollidableB as ConvexCollidable;
		PositionUpdateMode positionUpdateMode = ((convexCollidable.entity != null) ? convexCollidable.entity.PositionUpdateMode : PositionUpdateMode.Discrete);
		PositionUpdateMode positionUpdateMode2 = ((convexCollidable2.entity != null) ? convexCollidable2.entity.PositionUpdateMode : PositionUpdateMode.Discrete);
		BroadPhaseOverlap broadPhaseOverlap = base.BroadPhaseOverlap;
		if ((broadPhaseOverlap.entryA.IsActive || broadPhaseOverlap.entryB.IsActive) && ((positionUpdateMode == PositionUpdateMode.Continuous && positionUpdateMode2 == PositionUpdateMode.Continuous && broadPhaseOverlap.entryA == requester) || ((positionUpdateMode == PositionUpdateMode.Continuous) ^ (positionUpdateMode2 == PositionUpdateMode.Continuous))))
		{
			Vector3 result;
			if (positionUpdateMode == PositionUpdateMode.Discrete)
			{
				result = convexCollidable2.entity.linearVelocity;
			}
			else if (positionUpdateMode2 == PositionUpdateMode.Discrete)
			{
				Vector3.Negate(ref convexCollidable.entity.linearVelocity, out result);
			}
			else
			{
				Vector3.Subtract(ref convexCollidable2.entity.linearVelocity, ref convexCollidable.entity.linearVelocity, out result);
			}
			Vector3.Multiply(ref result, dt, out result);
			float num = result.LengthSquared();
			float num2 = convexCollidable.Shape.minimumRadius * MotionSettings.CoreShapeScaling;
			timeOfImpact = 1f;
			if (num2 * num2 < num && GJKToolbox.CCDSphereCast(new Ray(convexCollidable.worldTransform.Position, -result), num2, convexCollidable2.Shape, ref convexCollidable2.worldTransform, timeOfImpact, out var hit))
			{
				timeOfImpact = hit.T;
			}
			float num3 = convexCollidable2.Shape.minimumRadius * MotionSettings.CoreShapeScaling;
			if (num3 * num3 < num && GJKToolbox.CCDSphereCast(new Ray(convexCollidable2.worldTransform.Position, result), num3, convexCollidable.Shape, ref convexCollidable.worldTransform, timeOfImpact, out var hit2))
			{
				timeOfImpact = hit2.T;
			}
			if (timeOfImpact == 0f)
			{
				timeOfImpact = 1f;
			}
		}
	}
}
