using System;
using System.Collections.Generic;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.OtherSpaceStages;
using BEPUphysics.ResourceManagement;
using BEPUphysics.Threading;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Collidables;

/// <summary>
/// Manages the detection of entities within an arbitrary closed triangle mesh.
/// </summary>
public class DetectorVolume : BroadPhaseEntry, ISpaceObject, IDeferredEventCreator
{
	private struct ContainmentChange
	{
		public Entity Entity;

		public ContainmentChangeType Change;
	}

	private enum ContainmentChangeType : byte
	{
		BeganTouching,
		StoppedTouching,
		BeganContaining,
		StoppedContaining
	}

	internal Dictionary<Entity, DetectorVolumePairHandler> pairs = new Dictionary<Entity, DetectorVolumePairHandler>();

	private TriangleMesh triangleMesh;

	private ISpace space;

	private bool innerFacingIsClockwise;

	/// <summary>
	/// Used to protect against containment changes coming in from multithreaded narrowphase contexts.
	/// </summary>
	private SpinLock locker = new SpinLock();

	private Queue<ContainmentChange> containmentChanges = new Queue<ContainmentChange>();

	/// <summary>
	/// Gets the list of pairs associated with the detector volume.
	/// </summary>
	public ReadOnlyDictionary<Entity, DetectorVolumePairHandler> Pairs => new ReadOnlyDictionary<Entity, DetectorVolumePairHandler>(pairs);

	/// <summary>
	/// Gets or sets the triangle mesh data and acceleration structure.  Must be a closed mesh with consistent winding.
	/// </summary>
	public TriangleMesh TriangleMesh
	{
		get
		{
			return triangleMesh;
		}
		set
		{
			triangleMesh = value;
			UpdateBoundingBox();
			Reinitialize();
		}
	}

	ISpace ISpaceObject.Space
	{
		get
		{
			return space;
		}
		set
		{
			space = value;
		}
	}

	/// <summary>
	///  Space that owns the detector volume.
	/// </summary>
	public ISpace Space => space;

	protected internal override bool IsActive => false;

	DeferredEventDispatcher IDeferredEventCreator.DeferredEventDispatcher { get; set; }

	bool IDeferredEventCreator.IsActive
	{
		get
		{
			return true;
		}
		set
		{
			throw new NotSupportedException("Detector volumes are always active deferred event generators.");
		}
	}

	int IDeferredEventCreator.ChildDeferredEventCreators
	{
		get
		{
			return 0;
		}
		set
		{
			throw new NotSupportedException("The detector volume does not allow child deferred event creators.");
		}
	}

	/// <summary>
	/// Fires when an entity comes into contact with the volume.
	/// </summary>
	public event EntityBeginsTouchingVolumeEventHandler EntityBeganTouching;

	/// <summary>
	/// Fires when an entity ceases to intersect the volume.
	/// </summary>
	public event EntityStopsTouchingVolumeEventHandler EntityStoppedTouching;

	/// <summary>
	/// Fires when an entity becomes fully engulfed by a volume.
	/// </summary>
	public event VolumeBeginsContainingEntityEventHandler VolumeBeganContainingEntity;

	/// <summary>
	/// Fires when an entity ceases to be fully engulfed by a volume.
	/// </summary>
	public event VolumeStopsContainingEntityEventHandler VolumeStoppedContainingEntity;

	/// <summary>
	/// Creates a detector volume.
	/// </summary>
	/// <param name="triangleMesh">Closed and consistently wound mesh defining the volume.</param>
	public DetectorVolume(TriangleMesh triangleMesh)
	{
		TriangleMesh = triangleMesh;
		UpdateBoundingBox();
	}

	/// <summary>
	/// Determines if a point is contained by the detector volume.
	/// </summary>
	/// <param name="point">Point to check for containment.</param>
	/// <returns>Whether or not the point is contained by the detector volume.</returns>
	public bool IsPointContained(Vector3 point)
	{
		RawList<int> intList = Resources.GetIntList();
		bool result = IsPointContained(ref point, intList);
		Resources.GiveBack(intList);
		return result;
	}

	internal bool IsPointContained(ref Vector3 point, RawList<int> triangles)
	{
		Vector3.Add(ref boundingBox.Max, ref boundingBox.Min, out var result);
		Vector3.Multiply(ref result, 0.5f, out result);
		Vector3.Subtract(ref point, ref result, out result);
		if (result.LengthSquared() < 0.01f)
		{
			result = Vector3.Up;
		}
		Ray ray = new Ray(point, result);
		triangleMesh.Tree.GetOverlaps(ray, triangles);
		float num = float.MaxValue;
		bool flag = false;
		for (int i = 0; i < triangles.count; i++)
		{
			triangleMesh.Data.GetTriangle(triangles.Elements[i], out var v, out var v2, out var v3);
			if (Toolbox.FindRayTriangleIntersection(ref ray, float.MaxValue, ref v, ref v2, ref v3, out var hitClockwise, out var hit) && hit.T < num)
			{
				num = hit.T;
				flag = hitClockwise;
			}
		}
		triangles.Clear();
		if (num < float.MaxValue)
		{
			return flag == innerFacingIsClockwise;
		}
		return false;
	}

	protected override void CollisionRulesUpdated()
	{
		foreach (DetectorVolumePairHandler value in pairs.Values)
		{
			value.CollisionRule = CollisionRules.CollisionRuleCalculator(value.BroadPhaseOverlap.entryA, value.BroadPhaseOverlap.entryB);
		}
	}

	public override bool RayCast(Ray ray, float maximumLength, out RayHit rayHit)
	{
		return triangleMesh.RayCast(ray, maximumLength, TriangleSidedness.DoubleSided, out rayHit);
	}

	public override bool ConvexCast(ConvexShape castShape, ref RigidTransform startingTransform, ref Vector3 sweep, out RayHit hit)
	{
		hit = default(RayHit);
		Toolbox.GetExpandedBoundingBox(ref castShape, ref startingTransform, ref sweep, out var boundingBox);
		TriangleShape triangle = Resources.GetTriangle();
		RawList<int> intList = Resources.GetIntList();
		if (triangleMesh.Tree.GetOverlaps(boundingBox, intList))
		{
			hit.T = float.MaxValue;
			for (int i = 0; i < intList.Count; i++)
			{
				triangleMesh.Data.GetTriangle(intList[i], out triangle.vA, out triangle.vB, out triangle.vC);
				Vector3.Add(ref triangle.vA, ref triangle.vB, out var result);
				Vector3.Add(ref result, ref triangle.vC, out result);
				Vector3.Multiply(ref result, 1f / 3f, out result);
				Vector3.Subtract(ref triangle.vA, ref result, out triangle.vA);
				Vector3.Subtract(ref triangle.vB, ref result, out triangle.vB);
				Vector3.Subtract(ref triangle.vC, ref result, out triangle.vC);
				triangle.maximumRadius = triangle.vA.LengthSquared();
				float num = triangle.vB.LengthSquared();
				if (triangle.maximumRadius < num)
				{
					triangle.maximumRadius = num;
				}
				num = triangle.vC.LengthSquared();
				if (triangle.maximumRadius < num)
				{
					triangle.maximumRadius = num;
				}
				triangle.maximumRadius = (float)Math.Sqrt(triangle.maximumRadius);
				triangle.collisionMargin = 0f;
				RigidTransform transformB = new RigidTransform
				{
					Orientation = Quaternion.Identity,
					Position = result
				};
				if (MPRToolbox.Sweep(castShape, triangle, ref sweep, ref Toolbox.ZeroVector, ref startingTransform, ref transformB, out var hit2) && hit2.T < hit.T)
				{
					hit = hit2;
				}
			}
			triangle.maximumRadius = 0f;
			Resources.GiveBack(triangle);
			Resources.GiveBack(intList);
			return hit.T != float.MaxValue;
		}
		Resources.GiveBack(triangle);
		Resources.GiveBack(intList);
		return false;
	}

	/// <summary>
	/// Sets the bounding box of the detector volume to the current hierarchy root bounding box.  This is called automatically if the TriangleMesh property is set.
	/// </summary>
	public override void UpdateBoundingBox()
	{
		boundingBox = triangleMesh.Tree.BoundingBox;
	}

	/// <summary>
	/// Updates the detector volume's interpretation of the mesh.  This should be called when the the TriangleMesh is changed significantly.  This is called automatically if the TriangleMesh property is set.
	/// </summary>
	public void Reinitialize()
	{
		Vector3 vector = (triangleMesh.Tree.BoundingBox.Max - triangleMesh.Tree.BoundingBox.Min) * 1.5f + triangleMesh.Tree.BoundingBox.Min;
		triangleMesh.Data.GetTriangle(0, out var v, out var v2, out var v3);
		Vector3 direction = (v + v2 + v3) / 3f - vector;
		Ray ray = new Ray(vector, direction);
		RawList<int> intList = Resources.GetIntList();
		triangleMesh.Tree.GetOverlaps(ray, intList);
		float num = float.MaxValue;
		for (int i = 0; i < intList.count; i++)
		{
			triangleMesh.Data.GetTriangle(intList.Elements[i], out v, out v2, out v3);
			if (Toolbox.FindRayTriangleIntersection(ref ray, float.MaxValue, ref v, ref v2, ref v3, out var hitClockwise, out var hit) && hit.T < num)
			{
				num = hit.T;
				innerFacingIsClockwise = !hitClockwise;
			}
		}
		Resources.GiveBack(intList);
	}

	void ISpaceObject.OnAdditionToSpace(ISpace newSpace)
	{
	}

	void ISpaceObject.OnRemovalFromSpace(ISpace oldSpace)
	{
	}

	internal void BeganTouching(DetectorVolumePairHandler pair)
	{
		locker.Enter();
		containmentChanges.Enqueue(new ContainmentChange
		{
			Change = ContainmentChangeType.BeganTouching,
			Entity = pair.Collidable.entity
		});
		locker.Exit();
	}

	internal void StoppedTouching(DetectorVolumePairHandler pair)
	{
		locker.Enter();
		containmentChanges.Enqueue(new ContainmentChange
		{
			Change = ContainmentChangeType.StoppedTouching,
			Entity = pair.Collidable.entity
		});
		locker.Exit();
	}

	internal void BeganContaining(DetectorVolumePairHandler pair)
	{
		locker.Enter();
		containmentChanges.Enqueue(new ContainmentChange
		{
			Change = ContainmentChangeType.BeganContaining,
			Entity = pair.Collidable.entity
		});
		locker.Exit();
	}

	internal void StoppedContaining(DetectorVolumePairHandler pair)
	{
		locker.Enter();
		containmentChanges.Enqueue(new ContainmentChange
		{
			Change = ContainmentChangeType.StoppedContaining,
			Entity = pair.Collidable.entity
		});
		locker.Exit();
	}

	void IDeferredEventCreator.DispatchEvents()
	{
		while (containmentChanges.Count > 0)
		{
			ContainmentChange containmentChange = containmentChanges.Dequeue();
			switch (containmentChange.Change)
			{
			case ContainmentChangeType.BeganTouching:
				if (EntityBeganTouching != null)
				{
					EntityBeganTouching(this, containmentChange.Entity);
				}
				break;
			case ContainmentChangeType.StoppedTouching:
				if (EntityStoppedTouching != null)
				{
					EntityStoppedTouching(this, containmentChange.Entity);
				}
				break;
			case ContainmentChangeType.BeganContaining:
				if (VolumeBeganContainingEntity != null)
				{
					VolumeBeganContainingEntity(this, containmentChange.Entity);
				}
				break;
			case ContainmentChangeType.StoppedContaining:
				if (VolumeStoppedContainingEntity != null)
				{
					VolumeStoppedContainingEntity(this, containmentChange.Entity);
				}
				break;
			}
		}
	}
}
