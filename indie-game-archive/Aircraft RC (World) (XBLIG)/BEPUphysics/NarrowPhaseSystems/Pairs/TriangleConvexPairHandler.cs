using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseSystems;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests.CollisionAlgorithms.GJK;
using BEPUphysics.CollisionTests.Manifolds;
using BEPUphysics.Entities;
using BEPUphysics.PositionUpdating;
using BEPUphysics.Settings;
using Microsoft.Xna.Framework;

namespace BEPUphysics.NarrowPhaseSystems.Pairs;

/// <summary>
///  Handles a triangle-convex collision pair.
/// </summary>
public class TriangleConvexPairHandler : ConvexConstraintPairHandler
{
	private ConvexCollidable<TriangleShape> triangle;

	private ConvexCollidable convex;

	private TriangleConvexContactManifold contactManifold = new TriangleConvexContactManifold();

	public override Collidable CollidableA => convex;

	public override Collidable CollidableB => triangle;

	public override Entity EntityA => convex.entity;

	public override Entity EntityB => triangle.entity;

	/// <summary>
	/// Gets the contact manifold used by the pair handler.
	/// </summary>
	public override ContactManifold ContactManifold => contactManifold;

	/// <summary>
	///  Initializes the pair handler.
	/// </summary>
	/// <param name="entryA">First entry in the pair.</param>
	/// <param name="entryB">Second entry in the pair.</param>
	public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
	{
		triangle = entryA as ConvexCollidable<TriangleShape>;
		convex = entryB as ConvexCollidable;
		if (triangle == null || convex == null)
		{
			triangle = entryB as ConvexCollidable<TriangleShape>;
			convex = entryA as ConvexCollidable;
			if (triangle == null || convex == null)
			{
				throw new Exception("Inappropriate types used to initialize pair.");
			}
		}
		broadPhaseOverlap.entryA = convex;
		broadPhaseOverlap.entryB = triangle;
		base.Initialize(entryA, entryB);
	}

	/// <summary>
	///  Cleans up the pair handler.
	/// </summary>
	public override void CleanUp()
	{
		base.CleanUp();
		triangle = null;
		convex = null;
	}

	/// <summary>
	///  Updates the time of impact for the pair.
	/// </summary>
	/// <param name="requester">Collidable requesting the update.</param>
	/// <param name="dt">Timestep duration.</param>
	public override void UpdateTimeOfImpact(Collidable requester, float dt)
	{
		BroadPhaseOverlap broadPhaseOverlap = base.BroadPhaseOverlap;
		PositionUpdateMode positionUpdateMode = ((triangle.entity != null) ? triangle.entity.PositionUpdateMode : PositionUpdateMode.Discrete);
		PositionUpdateMode positionUpdateMode2 = ((convex.entity != null) ? convex.entity.PositionUpdateMode : PositionUpdateMode.Discrete);
		if ((!broadPhaseOverlap.entryA.IsActive && !broadPhaseOverlap.entryB.IsActive) || ((positionUpdateMode2 != PositionUpdateMode.Continuous || positionUpdateMode != PositionUpdateMode.Continuous || broadPhaseOverlap.entryA != requester) && !((positionUpdateMode2 == PositionUpdateMode.Continuous) ^ (positionUpdateMode == PositionUpdateMode.Continuous))))
		{
			return;
		}
		Vector3 result;
		if (positionUpdateMode2 == PositionUpdateMode.Discrete)
		{
			result = triangle.entity.linearVelocity;
		}
		else if (positionUpdateMode == PositionUpdateMode.Discrete)
		{
			Vector3.Negate(ref convex.entity.linearVelocity, out result);
		}
		else
		{
			Vector3.Subtract(ref triangle.entity.linearVelocity, ref convex.entity.linearVelocity, out result);
		}
		Vector3.Multiply(ref result, dt, out result);
		float num = result.LengthSquared();
		float num2 = convex.Shape.minimumRadius * MotionSettings.CoreShapeScaling;
		timeOfImpact = 1f;
		if (num2 * num2 < num && GJKToolbox.CCDSphereCast(new Ray(convex.worldTransform.Position, -result), num2, triangle.Shape, ref triangle.worldTransform, timeOfImpact, out var hit))
		{
			if (triangle.Shape.sidedness != TriangleSidedness.DoubleSided)
			{
				Vector3.Subtract(ref triangle.Shape.vB, ref triangle.Shape.vA, out var result2);
				Vector3.Subtract(ref triangle.Shape.vC, ref triangle.Shape.vA, out var result3);
				Vector3.Cross(ref result2, ref result3, out var result4);
				Vector3.Dot(ref hit.Normal, ref result4, out var result5);
				if ((triangle.Shape.sidedness == TriangleSidedness.Counterclockwise && result5 < 0f) || (triangle.Shape.sidedness == TriangleSidedness.Clockwise && result5 > 0f))
				{
					timeOfImpact = hit.T;
				}
			}
			else
			{
				timeOfImpact = hit.T;
			}
		}
		if (timeOfImpact == 0f)
		{
			timeOfImpact = 1f;
		}
	}
}
