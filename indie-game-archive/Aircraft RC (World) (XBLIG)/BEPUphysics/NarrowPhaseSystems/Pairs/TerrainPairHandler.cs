using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests.CollisionAlgorithms.GJK;
using BEPUphysics.CollisionTests.Manifolds;
using BEPUphysics.Constraints.Collision;
using BEPUphysics.Entities;
using BEPUphysics.PositionUpdating;
using BEPUphysics.ResourceManagement;
using BEPUphysics.Settings;
using Microsoft.Xna.Framework;

namespace BEPUphysics.NarrowPhaseSystems.Pairs;

/// <summary>
///  Handles a terrain-convex collision pair.
/// </summary>
public abstract class TerrainPairHandler : StandardPairHandler
{
	private Terrain terrain;

	private ConvexCollidable convex;

	private NonConvexContactManifoldConstraint contactConstraint = new NonConvexContactManifoldConstraint();

	public override Collidable CollidableA => convex;

	public override Collidable CollidableB => terrain;

	public override Entity EntityA => convex.entity;

	public override Entity EntityB => null;

	/// <summary>
	/// Gets the contact constraint used by the pair handler.
	/// </summary>
	public override ContactManifoldConstraint ContactConstraint => contactConstraint;

	/// <summary>
	/// Gets the contact manifold used by the pair handler.
	/// </summary>
	public override ContactManifold ContactManifold => TerrainManifold;

	protected abstract TerrainContactManifold TerrainManifold { get; }

	/// <summary>
	///  Initializes the pair handler.
	/// </summary>
	/// <param name="entryA">First entry in the pair.</param>
	/// <param name="entryB">Second entry in the pair.</param>
	public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
	{
		terrain = entryA as Terrain;
		convex = entryB as ConvexCollidable;
		if (terrain == null || convex == null)
		{
			terrain = entryB as Terrain;
			convex = entryA as ConvexCollidable;
			if (terrain == null || convex == null)
			{
				throw new Exception("Inappropriate types used to initialize pair.");
			}
		}
		broadPhaseOverlap.entryA = convex;
		broadPhaseOverlap.entryB = terrain;
		UpdateMaterialProperties((convex.entity != null) ? convex.entity.material : null, terrain.material);
		base.Initialize(entryA, entryB);
	}

	/// <summary>
	///  Cleans up the pair handler.
	/// </summary>
	public override void CleanUp()
	{
		base.CleanUp();
		terrain = null;
		convex = null;
	}

	/// <summary>
	///  Updates the time of impact for the pair.
	/// </summary>
	/// <param name="requester">Collidable requesting the update.</param>
	/// <param name="dt">Timestep duration.</param>
	public override void UpdateTimeOfImpact(Collidable requester, float dt)
	{
		if (!convex.IsActive || convex.entity.PositionUpdateMode != PositionUpdateMode.Continuous)
		{
			return;
		}
		Vector3.Multiply(ref convex.entity.linearVelocity, dt, out var result);
		float num = result.LengthSquared();
		float num2 = convex.Shape.minimumRadius * MotionSettings.CoreShapeScaling;
		timeOfImpact = 1f;
		if (!(num2 * num2 < num))
		{
			return;
		}
		TriangleShape triangle = Resources.GetTriangle();
		triangle.collisionMargin = 0f;
		Vector3 vector = new Vector3(terrain.worldTransform.LinearTransform.M21, terrain.worldTransform.LinearTransform.M22, terrain.worldTransform.LinearTransform.M23);
		for (int i = 0; i < TerrainManifold.overlappedTriangles.count; i++)
		{
			terrain.Shape.GetTriangle(ref TerrainManifold.overlappedTriangles.Elements[i], ref terrain.worldTransform, out triangle.vA, out triangle.vB, out triangle.vC);
			Vector3.Subtract(ref triangle.vA, ref convex.worldTransform.Position, out triangle.vA);
			Vector3.Subtract(ref triangle.vB, ref convex.worldTransform.Position, out triangle.vB);
			Vector3.Subtract(ref triangle.vC, ref convex.worldTransform.Position, out triangle.vC);
			if (GJKToolbox.CCDSphereCast(new Ray(Toolbox.ZeroVector, result), num2, triangle, ref Toolbox.RigidIdentity, timeOfImpact, out var hit) && hit.T > 1E-05f)
			{
				Vector3.Subtract(ref triangle.vB, ref triangle.vA, out var result2);
				Vector3.Subtract(ref triangle.vC, ref triangle.vA, out var result3);
				Vector3.Cross(ref result3, ref result2, out var result4);
				Vector3.Dot(ref result4, ref vector, out var result5);
				if (result5 < 0f)
				{
					Vector3.Dot(ref result4, ref hit.Normal, out result5);
				}
				else
				{
					Vector3.Dot(ref result4, ref hit.Normal, out result5);
					result5 = 0f - result5;
				}
				if (result5 < 0f)
				{
					timeOfImpact = hit.T;
				}
			}
		}
		Resources.GiveBack(triangle);
	}

	protected internal override void GetContactInformation(int index, out ContactInformation info)
	{
		info.Contact = TerrainManifold.contacts.Elements[index];
		info.FrictionImpulse = 0f;
		info.NormalImpulse = 0f;
		for (int i = 0; i < contactConstraint.frictionConstraints.count; i++)
		{
			if (contactConstraint.frictionConstraints.Elements[i].PenetrationConstraint.contact == info.Contact)
			{
				info.FrictionImpulse = contactConstraint.frictionConstraints.Elements[i].accumulatedImpulse;
				info.NormalImpulse = contactConstraint.frictionConstraints.Elements[i].PenetrationConstraint.accumulatedImpulse;
				break;
			}
		}
		if (convex.entity != null)
		{
			Vector3.Subtract(ref info.Contact.Position, ref convex.entity.position, out var result);
			Vector3.Cross(ref convex.entity.angularVelocity, ref result, out result);
			Vector3.Add(ref result, ref convex.entity.linearVelocity, out info.RelativeVelocity);
		}
		else
		{
			info.RelativeVelocity = default(Vector3);
		}
		info.Pair = this;
	}
}
