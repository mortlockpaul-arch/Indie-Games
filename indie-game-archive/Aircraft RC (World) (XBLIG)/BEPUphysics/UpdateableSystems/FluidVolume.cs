using System;
using System.Collections.Generic;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseSystems;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.DataStructures;
using BEPUphysics.ResourceManagement;
using BEPUphysics.Threading;
using Microsoft.Xna.Framework;

namespace BEPUphysics.UpdateableSystems;

/// <summary>
/// Volume in which physically simulated objects have a buoyancy force applied to them based on their density and volume.
/// </summary>
public class FluidVolume : Updateable, IDuringForcesUpdateable, ISpaceUpdateable, ISpaceObject
{
	private float surfacePlaneHeight;

	private Vector3 upVector;

	private BoundingBox boundingBox;

	private float maxDepth;

	private int samplePointsPerDimension = 8;

	private Vector3 flowDirection;

	private float flowForce;

	private float maxFlowSpeed;

	private List<Vector3[]> surfaceTriangles;

	private float gravity;

	private List<BroadPhaseEntry> collisionEntries = new List<BroadPhaseEntry>();

	private float dt;

	private Action<int> analyzeCollisionEntryDelegate;

	/// <summary>
	///  Gets the up vector of the fluid volume.
	/// </summary>
	public Vector3 UpVector
	{
		get
		{
			return upVector;
		}
		set
		{
			upVector = value;
			RecalculateBoundingBox();
		}
	}

	/// <summary>
	/// Bounding box surrounding the surface tris and entire depth of the object.
	/// </summary>
	public BoundingBox BoundingBox => boundingBox;

	/// <summary>
	/// Maximum depth of the fluid from the surface.
	/// </summary>
	public float MaxDepth
	{
		get
		{
			return maxDepth;
		}
		set
		{
			maxDepth = value;
			RecalculateBoundingBox();
		}
	}

	/// <summary>
	/// Density of the fluid represented in the volume.
	/// </summary>
	public float Density { get; set; }

	/// <summary>
	/// Number of locations along each of the horizontal axes from which to sample the shape.
	/// Defaults to 8.
	/// </summary>
	public int SamplePointsPerDimension
	{
		get
		{
			return samplePointsPerDimension;
		}
		set
		{
			samplePointsPerDimension = value;
		}
	}

	/// <summary>
	/// Fraction by which to reduce the linear momentum of floating objects each update.
	/// </summary>
	public float LinearDamping { get; set; }

	/// <summary>
	/// Fraction by which to reduce the angular momentum of floating objects each update.
	/// </summary>
	public float AngularDamping { get; set; }

	/// <summary>
	/// Direction in which to exert force on objects within the fluid.
	/// flowForce and maxFlowSpeed must have valid values as well for the flow to work.
	/// </summary>
	public Vector3 FlowDirection
	{
		get
		{
			return flowDirection;
		}
		set
		{
			float num = value.Length();
			if (num > 0f)
			{
				flowDirection = value / num;
			}
			else
			{
				flowDirection = Vector3.Zero;
			}
		}
	}

	/// <summary>
	/// Magnitude of the flow's force, in units of flow direction.
	/// flowDirection and maxFlowSpeed must have valid values as well for the flow to work.
	/// </summary>
	public float FlowForce
	{
		get
		{
			return flowForce;
		}
		set
		{
			flowForce = value;
		}
	}

	/// <summary>
	/// Maximum speed of the flow; objects will not be accelerated by the flow force beyond this speed.
	/// flowForce and flowDirection must have valid values as well for the flow to work.
	/// </summary>
	public float MaxFlowSpeed
	{
		get
		{
			return maxFlowSpeed;
		}
		set
		{
			maxFlowSpeed = value;
		}
	}

	private IQueryAccelerator QueryAccelerator { get; set; }

	/// <summary>
	///  Gets or sets the thread manager used by the fluid volume.
	/// </summary>
	public IThreadManager ThreadManager { get; set; }

	/// <summary>
	/// List of coplanar triangles composing the surface of the fluid.
	/// </summary>
	public List<Vector3[]> SurfaceTriangles
	{
		get
		{
			return surfaceTriangles;
		}
		set
		{
			surfaceTriangles = value;
			RecalculateBoundingBox();
		}
	}

	/// <summary>
	///  Gets or sets the gravity used by the fluid volume.
	/// </summary>
	public float Gravity
	{
		get
		{
			return gravity;
		}
		set
		{
			gravity = value;
		}
	}

	/// <summary>
	/// Creates a fluid volume.
	/// </summary>
	/// <param name="upVector">Up vector of the fluid volume.</param>
	/// <param name="gravity">Strength of gravity for the purposes of the fluid volume.</param>
	/// <param name="surfaceTriangles">List of triangles composing the surface of the fluid.  Set up as a list of length 3 arrays of Vector3's.</param>
	/// <param name="depth">Depth of the fluid back along the surface normal.</param>
	/// <param name="fluidDensity">Density of the fluid represented in the volume.</param>
	/// <param name="linearDamping">Fraction by which to reduce the linear momentum of floating objects each update, in addition to any of the body's own damping.</param>
	/// <param name="angularDamping">Fraction by which to reduce the angular momentum of floating objects each update, in addition to any of the body's own damping.</param>
	/// <param name="queryAccelerator">System to accelerate queries to find nearby entities.</param>
	/// <param name="threadManager">Thread manager used by the fluid volume.</param>
	public FluidVolume(Vector3 upVector, float gravity, List<Vector3[]> surfaceTriangles, float depth, float fluidDensity, float linearDamping, float angularDamping, IQueryAccelerator queryAccelerator, IThreadManager threadManager)
	{
		Gravity = gravity;
		SurfaceTriangles = surfaceTriangles;
		MaxDepth = depth;
		Density = fluidDensity;
		LinearDamping = linearDamping;
		AngularDamping = angularDamping;
		UpVector = upVector;
		QueryAccelerator = queryAccelerator;
		ThreadManager = threadManager;
		analyzeCollisionEntryDelegate = AnalyzeCollisionEntry;
	}

	/// <summary>
	/// Recalculates the bounding box of the fluid based on its depth, surface normal, and surface triangles.
	/// </summary>
	public void RecalculateBoundingBox()
	{
		RawList<Vector3> vectorList = Resources.GetVectorList();
		foreach (Vector3[] surfaceTriangle in SurfaceTriangles)
		{
			vectorList.Add(surfaceTriangle[0]);
			vectorList.Add(surfaceTriangle[1]);
			vectorList.Add(surfaceTriangle[2]);
			vectorList.Add(surfaceTriangle[0] - upVector * MaxDepth);
			vectorList.Add(surfaceTriangle[1] - upVector * MaxDepth);
			vectorList.Add(surfaceTriangle[2] - upVector * MaxDepth);
		}
		boundingBox = BoundingBox.CreateFromPoints(vectorList);
		surfacePlaneHeight = Vector3.Dot(vectorList[0], upVector);
		Resources.GiveBack(vectorList);
	}

	/// <summary>
	/// Applies buoyancy forces to appropriate objects.
	/// Called automatically when needed by the owning Space.
	/// </summary>
	/// <param name="dt">Time since last frame in physical logic.</param>
	void IDuringForcesUpdateable.Update(float dt)
	{
		QueryAccelerator.GetEntries(boundingBox, collisionEntries);
		this.dt = dt;
		if (collisionEntries.Count > 30 && ThreadManager.ThreadCount > 1)
		{
			ThreadManager.ForLoop(0, collisionEntries.Count, analyzeCollisionEntryDelegate);
		}
		else
		{
			for (int i = 0; i < collisionEntries.Count; i++)
			{
				AnalyzeCollisionEntry(i);
			}
		}
		collisionEntries.Clear();
	}

	private void AnalyzeCollisionEntry(int i)
	{
		if (!(collisionEntries[i] is EntityCollidable { IsActive: not false } entityCollidable) || !entityCollidable.entity.isDynamic)
		{
			return;
		}
		bool flag = false;
		foreach (Vector3[] surfaceTriangle in surfaceTriangles)
		{
			if (Toolbox.IsPointInsideTriangle(ref surfaceTriangle[0], ref surfaceTriangle[1], ref surfaceTriangle[2], ref entityCollidable.worldTransform.Position))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		GetBuoyancyInformation(entityCollidable, out var submergedVolume, out var submergedCenter);
		if (!(submergedVolume > 0f))
		{
			return;
		}
		Vector3.Multiply(ref upVector, (0f - gravity) * Density * dt * submergedVolume, out var result);
		entityCollidable.entity.ApplyImpulse(ref submergedCenter, ref result);
		float num = submergedVolume / entityCollidable.entity.volume;
		if (FlowForce != 0f)
		{
			float num2 = Math.Max(Vector3.Dot(entityCollidable.entity.linearVelocity, flowDirection), 0f);
			if (num2 < MaxFlowSpeed)
			{
				result = Math.Min(FlowForce, (MaxFlowSpeed - num2) * entityCollidable.entity.mass) * dt * num * FlowDirection;
				entityCollidable.entity.ApplyLinearImpulse(ref result);
			}
		}
		entityCollidable.entity.ModifyLinearDamping(num * LinearDamping);
		entityCollidable.entity.ModifyAngularDamping(num * AngularDamping);
	}

	private void GetBuoyancyInformation(EntityCollidable info, out float submergedVolume, out Vector3 submergedCenter)
	{
		BoundingBox entityBoundingBox = info.boundingBox;
		if (entityBoundingBox.Min.Y > surfacePlaneHeight)
		{
			submergedVolume = 0f;
			submergedCenter = info.worldTransform.Position;
			return;
		}
		if (entityBoundingBox.Max.Y < surfacePlaneHeight)
		{
			submergedVolume = info.entity.volume;
			submergedCenter = info.worldTransform.Position;
			return;
		}
		GetSamplingOrigin(ref entityBoundingBox, out var xSpacing, out var zSpacing, out var perColumnArea, out var origin);
		float boundingBoxHeight = entityBoundingBox.Max.Y - entityBoundingBox.Min.Y;
		float maxLength = surfacePlaneHeight - entityBoundingBox.Min.Y;
		submergedCenter = default(Vector3);
		submergedVolume = 0f;
		for (int i = 0; i < samplePointsPerDimension; i++)
		{
			for (int j = 0; j < samplePointsPerDimension; j++)
			{
				float submergedHeight;
				if ((submergedHeight = GetSubmergedHeight(info, maxLength, boundingBoxHeight, ref origin, ref xSpacing, ref zSpacing, i, j, out var value)) > 0f)
				{
					float num = submergedHeight * perColumnArea;
					Vector3.Multiply(ref value, num, out value);
					Vector3.Add(ref value, ref submergedCenter, out submergedCenter);
					submergedVolume += num;
				}
			}
		}
		Vector3.Divide(ref submergedCenter, submergedVolume, out submergedCenter);
	}

	private void GetSamplingOrigin(ref BoundingBox entityBoundingBox, out Vector3 xSpacing, out Vector3 zSpacing, out float perColumnArea, out Vector3 origin)
	{
		float num = (entityBoundingBox.Max.X - entityBoundingBox.Min.X) / (float)samplePointsPerDimension;
		float num2 = (entityBoundingBox.Max.Z - entityBoundingBox.Min.Z) / (float)samplePointsPerDimension;
		Vector3 value = Toolbox.RightVector;
		Vector3.Multiply(ref value, num, out xSpacing);
		Vector3 value2 = Toolbox.BackVector;
		Vector3.Multiply(ref value2, num2, out zSpacing);
		perColumnArea = num * num2;
		Vector3 value3 = entityBoundingBox.Min;
		Vector3.Multiply(ref xSpacing, 0.5f, out var result);
		Vector3.Add(ref value3, ref result, out origin);
		Vector3.Multiply(ref zSpacing, 0.5f, out result);
		Vector3.Add(ref origin, ref result, out origin);
	}

	private float GetSubmergedHeight(Collidable info, float maxLength, float boundingBoxHeight, ref Vector3 rayOrigin, ref Vector3 xSpacing, ref Vector3 zSpacing, int i, int j, out Vector3 volumeCenter)
	{
		Ray ray = default(Ray);
		Vector3.Multiply(ref xSpacing, i, out ray.Position);
		Vector3.Multiply(ref zSpacing, j, out ray.Direction);
		Vector3.Add(ref ray.Position, ref ray.Direction, out ray.Position);
		Vector3.Add(ref ray.Position, ref rayOrigin, out ray.Position);
		ray.Direction = upVector;
		if (info.RayCast(ray, maxLength, out var rayHit))
		{
			Vector3.Multiply(ref ray.Direction, boundingBoxHeight, out ray.Direction);
			Vector3.Add(ref ray.Position, ref ray.Direction, out ray.Position);
			Vector3.Negate(ref upVector, out ray.Direction);
			float y = rayHit.Location.Y;
			float t = rayHit.T;
			Vector3 value = rayHit.Location;
			if (info.RayCast(ray, boundingBoxHeight - rayHit.T, out rayHit))
			{
				Vector3.Add(ref rayHit.Location, ref value, out volumeCenter);
				Vector3.Multiply(ref volumeCenter, 0.5f, out volumeCenter);
				return Math.Min(surfacePlaneHeight - y, boundingBoxHeight - rayHit.T - t);
			}
			volumeCenter = Vector3.Zero;
			return 0f;
		}
		volumeCenter = Vector3.Zero;
		return 0f;
	}
}
