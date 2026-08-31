using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.NarrowPhaseSystems.Pairs;

/// <summary>
/// Handles the tests between a DetectorVolume and a convex collidable.
/// </summary>
public class DetectorVolumeConvexPairHandler : DetectorVolumePairHandler
{
	private ConvexCollidable convex;

	private bool checkContainment = true;

	private RawList<int> overlaps = new RawList<int>(8);

	private TriangleShape triangle = new TriangleShape
	{
		collisionMargin = 0f
	};

	/// <summary>
	/// Gets or sets whether or not to check the convex object for total containment within the detector volume.
	/// </summary>
	public bool CheckContainment
	{
		get
		{
			return checkContainment;
		}
		set
		{
			checkContainment = value;
		}
	}

	public override EntityCollidable Collidable => convex;

	public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
	{
		base.Initialize(entryA, entryB);
		convex = entryA as ConvexCollidable;
		if (convex == null)
		{
			convex = entryB as ConvexCollidable;
			if (convex == null)
			{
				throw new Exception("Incorrect types passed to pair handler.");
			}
		}
	}

	public override void CleanUp()
	{
		base.CleanUp();
		convex = null;
		checkContainment = true;
	}

	public override void UpdateCollision(float dt)
	{
		base.WasContaining = base.Containing;
		base.WasTouching = base.Touching;
		RigidTransform transformB = new RigidTransform
		{
			Orientation = Quaternion.Identity
		};
		base.DetectorVolume.TriangleMesh.Tree.GetOverlaps(convex.boundingBox, overlaps);
		int num = 0;
		while (true)
		{
			if (num < overlaps.count)
			{
				base.DetectorVolume.TriangleMesh.Data.GetTriangle(overlaps.Elements[num], out triangle.vA, out triangle.vB, out triangle.vC);
				Vector3.Add(ref triangle.vA, ref triangle.vB, out transformB.Position);
				Vector3.Add(ref triangle.vC, ref transformB.Position, out transformB.Position);
				Vector3.Multiply(ref transformB.Position, 1f / 3f, out transformB.Position);
				Vector3.Subtract(ref triangle.vA, ref transformB.Position, out triangle.vA);
				Vector3.Subtract(ref triangle.vB, ref transformB.Position, out triangle.vB);
				Vector3.Subtract(ref triangle.vC, ref transformB.Position, out triangle.vC);
				if (MPRToolbox.AreShapesOverlapping(convex.Shape, triangle, ref convex.worldTransform, ref transformB))
				{
					base.Touching = true;
					base.Containing = false;
					overlaps.Clear();
					break;
				}
				num++;
				continue;
			}
			overlaps.Clear();
			if (CheckContainment && base.DetectorVolume.IsPointContained(ref convex.worldTransform.Position, overlaps))
			{
				base.Touching = true;
				base.Containing = true;
			}
			else
			{
				base.Touching = false;
				base.Containing = false;
			}
			break;
		}
		NotifyDetectorVolumeOfChanges();
	}
}
