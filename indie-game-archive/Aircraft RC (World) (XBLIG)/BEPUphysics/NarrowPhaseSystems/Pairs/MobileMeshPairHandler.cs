using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseSystems;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests.CollisionAlgorithms.GJK;
using BEPUphysics.CollisionTests.Manifolds;
using BEPUphysics.Constraints.Collision;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using BEPUphysics.PositionUpdating;
using BEPUphysics.ResourceManagement;
using BEPUphysics.Settings;
using Microsoft.Xna.Framework;

namespace BEPUphysics.NarrowPhaseSystems.Pairs;

/// <summary>
///  Handles a mobile mesh-convex collision pair.
/// </summary>
public abstract class MobileMeshPairHandler : StandardPairHandler
{
	private MobileMeshCollidable mobileMesh;

	private ConvexCollidable convex;

	private NonConvexContactManifoldConstraint contactConstraint = new NonConvexContactManifoldConstraint();

	public override Collidable CollidableA => convex;

	public override Collidable CollidableB => mobileMesh;

	public override Entity EntityA => convex.entity;

	public override Entity EntityB => mobileMesh.entity;

	/// <summary>
	/// Gets the contact manifold used by the pair handler.
	/// </summary>
	public override ContactManifold ContactManifold => MeshManifold;

	/// <summary>
	/// Gets the contact constraint used by the pair handler.
	/// </summary>
	public override ContactManifoldConstraint ContactConstraint => contactConstraint;

	protected internal abstract MobileMeshContactManifold MeshManifold { get; }

	/// <summary>
	///  Initializes the pair handler.
	/// </summary>
	/// <param name="entryA">First entry in the pair.</param>
	/// <param name="entryB">Second entry in the pair.</param>
	public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
	{
		mobileMesh = entryA as MobileMeshCollidable;
		convex = entryB as ConvexCollidable;
		if (mobileMesh == null || convex == null)
		{
			mobileMesh = entryB as MobileMeshCollidable;
			convex = entryA as ConvexCollidable;
			if (mobileMesh == null || convex == null)
			{
				throw new Exception("Inappropriate types used to initialize pair.");
			}
		}
		broadPhaseOverlap.entryA = convex;
		broadPhaseOverlap.entryB = mobileMesh;
		UpdateMaterialProperties((convex.entity != null) ? convex.entity.material : null, (mobileMesh.entity != null) ? mobileMesh.entity.material : null);
		base.Initialize(entryA, entryB);
	}

	/// <summary>
	///  Cleans up the pair handler.
	/// </summary>
	public override void CleanUp()
	{
		base.CleanUp();
		mobileMesh = null;
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
		PositionUpdateMode positionUpdateMode = ((mobileMesh.entity != null) ? mobileMesh.entity.PositionUpdateMode : PositionUpdateMode.Discrete);
		PositionUpdateMode positionUpdateMode2 = ((convex.entity != null) ? convex.entity.PositionUpdateMode : PositionUpdateMode.Discrete);
		if ((!mobileMesh.IsActive && !convex.IsActive) || ((positionUpdateMode2 != PositionUpdateMode.Continuous || positionUpdateMode != PositionUpdateMode.Continuous || broadPhaseOverlap.entryA != requester) && !((positionUpdateMode2 == PositionUpdateMode.Continuous) ^ (positionUpdateMode == PositionUpdateMode.Continuous))))
		{
			return;
		}
		Vector3 result;
		if (positionUpdateMode2 == PositionUpdateMode.Discrete)
		{
			Vector3.Negate(ref mobileMesh.entity.linearVelocity, out result);
		}
		else if (positionUpdateMode == PositionUpdateMode.Discrete)
		{
			result = convex.entity.linearVelocity;
		}
		else
		{
			Vector3.Subtract(ref convex.entity.linearVelocity, ref mobileMesh.entity.linearVelocity, out result);
		}
		Vector3.Multiply(ref result, dt, out result);
		float num = result.LengthSquared();
		float num2 = convex.Shape.minimumRadius * MotionSettings.CoreShapeScaling;
		timeOfImpact = 1f;
		if (!(num2 * num2 < num))
		{
			return;
		}
		TriangleSidedness sidedness = mobileMesh.Shape.Sidedness;
		Matrix3X3.CreateFromQuaternion(ref mobileMesh.worldTransform.Orientation, out var result2);
		TriangleShape triangle = Resources.GetTriangle();
		triangle.collisionMargin = 0f;
		for (int i = 0; i < MeshManifold.overlappedTriangles.count; i++)
		{
			MeshBoundingBoxTreeData data = mobileMesh.Shape.TriangleMesh.Data;
			int triangleIndex = MeshManifold.overlappedTriangles.Elements[i];
			data.GetTriangle(triangleIndex, out triangle.vA, out triangle.vB, out triangle.vC);
			Matrix3X3.Transform(ref triangle.vA, ref result2, out triangle.vA);
			Matrix3X3.Transform(ref triangle.vB, ref result2, out triangle.vB);
			Matrix3X3.Transform(ref triangle.vC, ref result2, out triangle.vC);
			Vector3.Add(ref triangle.vA, ref mobileMesh.worldTransform.Position, out triangle.vA);
			Vector3.Add(ref triangle.vB, ref mobileMesh.worldTransform.Position, out triangle.vB);
			Vector3.Add(ref triangle.vC, ref mobileMesh.worldTransform.Position, out triangle.vC);
			Vector3.Subtract(ref triangle.vA, ref convex.worldTransform.Position, out triangle.vA);
			Vector3.Subtract(ref triangle.vB, ref convex.worldTransform.Position, out triangle.vB);
			Vector3.Subtract(ref triangle.vC, ref convex.worldTransform.Position, out triangle.vC);
			if (!GJKToolbox.CCDSphereCast(new Ray(Toolbox.ZeroVector, result), num2, triangle, ref Toolbox.RigidIdentity, timeOfImpact, out var hit) || !(hit.T > 1E-05f))
			{
				continue;
			}
			if (sidedness != TriangleSidedness.DoubleSided)
			{
				Vector3.Subtract(ref triangle.vB, ref triangle.vA, out var result3);
				Vector3.Subtract(ref triangle.vC, ref triangle.vA, out var result4);
				Vector3.Cross(ref result3, ref result4, out var result5);
				Vector3.Dot(ref result5, ref hit.Normal, out var result6);
				if ((sidedness == TriangleSidedness.Counterclockwise && result6 < 0f) || (sidedness == TriangleSidedness.Clockwise && result6 > 0f))
				{
					timeOfImpact = hit.T;
				}
			}
			else
			{
				timeOfImpact = hit.T;
			}
		}
		Resources.GiveBack(triangle);
	}

	protected internal override void GetContactInformation(int index, out ContactInformation info)
	{
		info.Contact = MeshManifold.contacts.Elements[index];
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
		Vector3 result;
		if (convex.entity != null)
		{
			Vector3.Subtract(ref info.Contact.Position, ref convex.entity.position, out result);
			Vector3.Cross(ref convex.entity.angularVelocity, ref result, out result);
			Vector3.Add(ref result, ref convex.entity.linearVelocity, out info.RelativeVelocity);
		}
		else
		{
			info.RelativeVelocity = default(Vector3);
		}
		if (mobileMesh.entity != null)
		{
			Vector3.Subtract(ref info.Contact.Position, ref mobileMesh.entity.position, out result);
			Vector3.Cross(ref mobileMesh.entity.angularVelocity, ref result, out result);
			Vector3.Add(ref result, ref mobileMesh.entity.linearVelocity, out result);
			Vector3.Subtract(ref info.RelativeVelocity, ref result, out info.RelativeVelocity);
		}
		info.Pair = this;
	}
}
