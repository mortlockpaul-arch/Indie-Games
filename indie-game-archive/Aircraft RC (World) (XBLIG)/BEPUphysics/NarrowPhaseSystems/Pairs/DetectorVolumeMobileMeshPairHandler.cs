using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.NarrowPhaseSystems.Pairs;

/// <summary>
///  Superclass of pairs between collidables that generate contact points.
/// </summary>
public class DetectorVolumeMobileMeshPairHandler : DetectorVolumePairHandler
{
	private MobileMeshCollidable mesh;

	private TriangleShape mobileTriangle = new TriangleShape();

	private TriangleShape detectorTriangle = new TriangleShape
	{
		collisionMargin = 0f
	};

	private RawList<int> overlaps = new RawList<int>(8);

	/// <summary>
	/// Gets the entity collidable associated with the pair.
	/// </summary>
	public override EntityCollidable Collidable => mesh;

	/// <summary>
	///  Called when the pair handler is added to the narrow phase.
	/// </summary>
	protected internal override void OnAddedToNarrowPhase()
	{
		base.DetectorVolume.pairs.Add(Collidable.entity, this);
	}

	public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
	{
		base.Initialize(entryA, entryB);
		mesh = entryA as MobileMeshCollidable;
		if (mesh == null)
		{
			mesh = entryB as MobileMeshCollidable;
			if (mesh == null)
			{
				throw new Exception("Invalid types used to initialize pair handler.");
			}
		}
	}

	/// <summary>
	///  Cleans up the pair handler.
	/// </summary>
	public override void CleanUp()
	{
		base.CleanUp();
		mesh = null;
	}

	public override void UpdateCollision(float dt)
	{
		base.WasContaining = base.Containing;
		base.WasTouching = base.Touching;
		mobileTriangle.collisionMargin = mesh.Shape.MeshCollisionMargin;
		base.Touching = false;
		base.Containing = true;
		MeshBoundingBoxTreeData data = mesh.Shape.TriangleMesh.Data;
		RigidTransform shapeTransform = default(RigidTransform);
		shapeTransform.Orientation = Quaternion.Identity;
		RigidTransform transformA = default(RigidTransform);
		transformA.Orientation = Quaternion.Identity;
		for (int i = 0; i < data.Indices.Length; i += 3)
		{
			data.GetTriangle(i, out mobileTriangle.vA, out mobileTriangle.vB, out mobileTriangle.vC);
			RigidTransform.Transform(ref mobileTriangle.vA, ref mesh.worldTransform, out mobileTriangle.vA);
			RigidTransform.Transform(ref mobileTriangle.vB, ref mesh.worldTransform, out mobileTriangle.vB);
			RigidTransform.Transform(ref mobileTriangle.vC, ref mesh.worldTransform, out mobileTriangle.vC);
			Vector3.Add(ref mobileTriangle.vA, ref mobileTriangle.vB, out shapeTransform.Position);
			Vector3.Add(ref mobileTriangle.vC, ref shapeTransform.Position, out shapeTransform.Position);
			Vector3.Multiply(ref shapeTransform.Position, 1f / 3f, out shapeTransform.Position);
			Vector3.Subtract(ref mobileTriangle.vA, ref shapeTransform.Position, out mobileTriangle.vA);
			Vector3.Subtract(ref mobileTriangle.vB, ref shapeTransform.Position, out mobileTriangle.vB);
			Vector3.Subtract(ref mobileTriangle.vC, ref shapeTransform.Position, out mobileTriangle.vC);
			mobileTriangle.GetBoundingBox(ref shapeTransform, out var boundingBox);
			base.DetectorVolume.TriangleMesh.Tree.GetOverlaps(boundingBox, overlaps);
			int num = 0;
			bool flag;
			bool flag2;
			while (true)
			{
				if (num < overlaps.count)
				{
					base.DetectorVolume.TriangleMesh.Data.GetTriangle(overlaps.Elements[num], out detectorTriangle.vA, out detectorTriangle.vB, out detectorTriangle.vC);
					Vector3.Add(ref detectorTriangle.vA, ref detectorTriangle.vB, out transformA.Position);
					Vector3.Add(ref detectorTriangle.vC, ref transformA.Position, out transformA.Position);
					Vector3.Multiply(ref transformA.Position, 1f / 3f, out transformA.Position);
					Vector3.Subtract(ref detectorTriangle.vA, ref transformA.Position, out detectorTriangle.vA);
					Vector3.Subtract(ref detectorTriangle.vB, ref transformA.Position, out detectorTriangle.vB);
					Vector3.Subtract(ref detectorTriangle.vC, ref transformA.Position, out detectorTriangle.vC);
					if (MPRToolbox.AreShapesOverlapping(detectorTriangle, mobileTriangle, ref transformA, ref shapeTransform))
					{
						flag = true;
						flag2 = false;
						overlaps.Clear();
						break;
					}
					num++;
					continue;
				}
				overlaps.Clear();
				if ((!base.Touching || base.Containing) && base.DetectorVolume.IsPointContained(ref shapeTransform.Position, overlaps))
				{
					flag = true;
					flag2 = true;
				}
				else
				{
					flag = false;
					flag2 = false;
				}
				break;
			}
			if (flag)
			{
				base.Touching = true;
			}
			else
			{
				base.Containing = false;
			}
			if (!flag2)
			{
				base.Containing = false;
			}
			if (!base.Containing && base.Touching)
			{
				break;
			}
		}
		if (mesh.Shape.solidity == MobileMeshSolidity.Solid && !base.Containing && !base.Touching)
		{
			base.DetectorVolume.TriangleMesh.Data.GetVertexPosition(0, out var vertex);
			Ray ray = default(Ray);
			ray.Direction = Vector3.Up;
			RigidTransform.TransformByInverse(ref vertex, ref mesh.worldTransform, out ray.Position);
			if (mesh.Shape.IsLocalRayOriginInMesh(ref ray, out var _))
			{
				base.Touching = true;
			}
		}
		NotifyDetectorVolumeOfChanges();
	}
}
