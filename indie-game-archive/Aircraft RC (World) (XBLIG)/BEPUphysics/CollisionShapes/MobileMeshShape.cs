using System;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using BEPUphysics.ResourceManagement;
using BEPUphysics.Settings;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionShapes;

/// <summary>
///  Local space data associated with a mobile mesh.
///  This contains a hierarchy and all the other heavy data needed
///  by an MobileMesh.
/// </summary>
public class MobileMeshShape : EntityShape
{
	private float meshCollisionMargin = CollisionDetectionSettings.DefaultMargin;

	private TriangleMesh triangleMesh;

	private RawList<Vector3> surfaceVertices = new RawList<Vector3>();

	internal MobileMeshSolidity solidity;

	/// <summary>
	/// Sidedness required if the mesh is in solid mode.
	/// If the windings were reversed or double sided,
	/// the solidity would fight against shell contacts,
	/// leading to very bad jittering.
	/// </summary>
	internal TriangleSidedness solidSidedness;

	/// <summary>
	/// The difference in t parameters in a ray cast under which two hits are considered to be redundant.
	/// </summary>
	public static float MeshHitUniquenessThreshold = 0.0001f;

	/// <summary>
	/// Gets or sets the margin of the mobile mesh to use when colliding with other meshes.
	/// When colliding with non-mesh shapes, the mobile mesh has no margin.
	/// </summary>
	public float MeshCollisionMargin
	{
		get
		{
			return meshCollisionMargin;
		}
		set
		{
			if (value < 0f)
			{
				throw new Exception("Mesh margin must be nonnegative.");
			}
			meshCollisionMargin = value;
			OnShapeChanged();
		}
	}

	/// <summary>
	///  Gets or sets the TriangleMesh data structure used by this shape.
	/// </summary>
	public TriangleMesh TriangleMesh => triangleMesh;

	/// <summary>
	/// Gets the transform used by the local mesh shape.
	/// </summary>
	public AffineTransform Transform => ((TransformableMeshData)triangleMesh.Data).worldTransform;

	/// <summary>
	///  Gets the solidity of the mesh.
	/// </summary>
	public MobileMeshSolidity Solidity => solidity;

	/// <summary>
	/// Gets or sets the sidedness of the shape.  This is a convenience property based on the Solidity property.
	/// If the shape is solid, this returns whatever sidedness is computed to make the triangles of the shape face outward.
	/// If the shape is solid, setting this property will change the sidedness that is used while the shape is solid.
	/// </summary>
	public TriangleSidedness Sidedness
	{
		get
		{
			return solidity switch
			{
				MobileMeshSolidity.Clockwise => TriangleSidedness.Clockwise, 
				MobileMeshSolidity.Counterclockwise => TriangleSidedness.Counterclockwise, 
				MobileMeshSolidity.DoubleSided => TriangleSidedness.DoubleSided, 
				MobileMeshSolidity.Solid => solidSidedness, 
				_ => TriangleSidedness.DoubleSided, 
			};
		}
		set
		{
			if (solidity == MobileMeshSolidity.Solid)
			{
				solidSidedness = value;
				return;
			}
			switch (value)
			{
			case TriangleSidedness.Clockwise:
				solidity = MobileMeshSolidity.Clockwise;
				break;
			case TriangleSidedness.Counterclockwise:
				solidity = MobileMeshSolidity.Counterclockwise;
				break;
			case TriangleSidedness.DoubleSided:
				solidity = MobileMeshSolidity.DoubleSided;
				break;
			}
		}
	}

	/// <summary>
	///  Constructs a new mobile mesh shape.
	/// </summary>
	/// <param name="vertices">Vertices of the mesh.</param>
	/// <param name="indices">Indices of the mesh.</param>
	/// <param name="localTransform">Local transform to apply to the shape.</param>
	/// <param name="solidity">Solidity state of the shape.</param>
	public MobileMeshShape(Vector3[] vertices, int[] indices, AffineTransform localTransform, MobileMeshSolidity solidity)
	{
		this.solidity = solidity;
		TransformableMeshData data = new TransformableMeshData(vertices, indices, localTransform);
		ComputeShapeInformation(data, out var shapeInformation);
		for (int i = 0; i < surfaceVertices.count; i++)
		{
			Vector3.Subtract(ref surfaceVertices.Elements[i], ref shapeInformation.Center, out surfaceVertices.Elements[i]);
		}
		triangleMesh = new TriangleMesh(data);
		ComputeSolidSidedness();
	}

	/// <summary>
	///  Constructs a new mobile mesh shape.
	/// </summary>
	/// <param name="vertices">Vertices of the mesh.</param>
	/// <param name="indices">Indices of the mesh.</param>
	/// <param name="localTransform">Local transform to apply to the shape.</param>
	/// <param name="solidity">Solidity state of the shape.</param>
	/// <param name="distributionInfo">Information computed about the shape during construction.</param>
	public MobileMeshShape(Vector3[] vertices, int[] indices, AffineTransform localTransform, MobileMeshSolidity solidity, out ShapeDistributionInformation distributionInfo)
	{
		this.solidity = solidity;
		TransformableMeshData data = new TransformableMeshData(vertices, indices, localTransform);
		ComputeShapeInformation(data, out distributionInfo);
		for (int i = 0; i < surfaceVertices.count; i++)
		{
			Vector3.Subtract(ref surfaceVertices.Elements[i], ref distributionInfo.Center, out surfaceVertices.Elements[i]);
		}
		triangleMesh = new TriangleMesh(data);
		ComputeSolidSidedness();
	}

	/// <summary>
	/// Tests to see if a ray's origin is contained within the mesh.
	/// If it is, the hit location is found.
	/// If it isn't, the hit location is still valid if a hit occurred.
	/// If the origin isn't inside and there was no hit, the hit has a T value of float.MaxValue.
	/// </summary>
	/// <param name="ray">Ray in the local space of the shape to test.</param>
	/// <param name="hit">The first hit against the mesh, if any.</param>
	/// <returns>Whether or not the ray origin was in the mesh.</returns>
	public bool IsLocalRayOriginInMesh(ref Ray ray, out RayHit hit)
	{
		RawList<int> intList = Resources.GetIntList();
		hit = default(RayHit);
		hit.T = float.MaxValue;
		if (triangleMesh.Tree.GetOverlaps(ray, intList))
		{
			bool flag = false;
			for (int i = 0; i < intList.Count; i++)
			{
				triangleMesh.Data.GetTriangle(intList[i], out var v, out var v2, out var v3);
				if (Toolbox.FindRayTriangleIntersection(ref ray, float.MaxValue, ref v, ref v2, ref v3, out var hitClockwise, out var hit2) && hit2.T < hit.T)
				{
					hit = hit2;
					flag = hitClockwise;
				}
			}
			Resources.GiveBack(intList);
			if (hit.T < float.MaxValue)
			{
				if (solidSidedness != TriangleSidedness.Clockwise || flag)
				{
					if (solidSidedness == TriangleSidedness.Counterclockwise)
					{
						return flag;
					}
					return false;
				}
				return true;
			}
			return false;
		}
		Resources.GiveBack(intList);
		return false;
	}

	internal bool IsHitUnique(RawList<RayHit> hits, ref RayHit hit)
	{
		for (int i = 0; i < hits.count; i++)
		{
			if (Math.Abs(hits.Elements[i].T - hit.T) < MeshHitUniquenessThreshold)
			{
				return false;
			}
		}
		hits.Add(hit);
		return true;
	}

	private void ComputeSolidSidedness()
	{
		Ray ray = default(Ray);
		triangleMesh.Data.GetTriangle(triangleMesh.Data.indices.Length / 3 / 2 * 3, out var v, out var v2, out var v3);
		ray.Direction = (v + v2 + v3) / 3f;
		ray.Direction.Normalize();
		solidSidedness = ComputeSolidSidednessHelper(ray);
	}

	private TriangleSidedness ComputeSolidSidednessHelper(Ray ray)
	{
		RawList<int> intList = Resources.GetIntList();
		TriangleSidedness result;
		if (triangleMesh.Tree.GetOverlaps(ray, intList))
		{
			RawList<RayHit> rayHitList = Resources.GetRayHitList();
			int triangleIndex = 0;
			int triangleIndex2 = 0;
			float num = float.MaxValue;
			float num2 = -1f;
			Vector3 v;
			Vector3 v2;
			Vector3 v3;
			for (int i = 0; i < intList.Count; i++)
			{
				triangleMesh.Data.GetTriangle(intList[i], out v, out v2, out v3);
				if (Toolbox.FindRayTriangleIntersection(ref ray, float.MaxValue, TriangleSidedness.DoubleSided, ref v, ref v2, ref v3, out var hit) && IsHitUnique(rayHitList, ref hit))
				{
					if (hit.T < num)
					{
						num = hit.T;
						triangleIndex = intList[i];
					}
					if (hit.T > num2)
					{
						num2 = hit.T;
						triangleIndex2 = intList[i];
					}
				}
			}
			if (rayHitList.count % 2 == 0)
			{
				triangleMesh.Data.GetTriangle(triangleIndex, out v, out v2, out v3);
				Vector3 vector = Vector3.Cross(v - v2, v - v3);
				result = ((Vector3.Dot(vector, ray.Direction) < 0f) ? TriangleSidedness.Clockwise : TriangleSidedness.Counterclockwise);
			}
			else
			{
				triangleMesh.Data.GetTriangle(triangleIndex2, out v, out v2, out v3);
				Vector3 vector2 = Vector3.Cross(v - v2, v - v3);
				result = ((!(Vector3.Dot(vector2, ray.Direction) < 0f)) ? TriangleSidedness.Clockwise : TriangleSidedness.Counterclockwise);
			}
			Resources.GiveBack(rayHitList);
		}
		else
		{
			result = TriangleSidedness.DoubleSided;
		}
		Resources.GiveBack(intList);
		return result;
	}

	private void ComputeShapeInformation(TransformableMeshData data, out ShapeDistributionInformation shapeInformation)
	{
		surfaceVertices.Clear();
		try
		{
			ConvexHullHelper.GetConvexHull(data.vertices, surfaceVertices);
			for (int i = 0; i < surfaceVertices.count; i++)
			{
				AffineTransform.Transform(ref surfaceVertices.Elements[i], ref data.worldTransform, out surfaceVertices.Elements[i]);
			}
		}
		catch
		{
			surfaceVertices.Clear();
			BoundingBox boundingBox = default(BoundingBox);
			for (int j = 0; j < data.vertices.Length; j++)
			{
				data.GetVertexPosition(j, out var vertex);
				if (vertex.X > boundingBox.Max.X)
				{
					boundingBox.Max.X = vertex.X;
				}
				if (vertex.X < boundingBox.Min.X)
				{
					boundingBox.Min.X = vertex.X;
				}
				if (vertex.Y > boundingBox.Max.Y)
				{
					boundingBox.Max.Y = vertex.Y;
				}
				if (vertex.Y < boundingBox.Min.Y)
				{
					boundingBox.Min.Y = vertex.Y;
				}
				if (vertex.Z > boundingBox.Max.Z)
				{
					boundingBox.Max.Z = vertex.Z;
				}
				if (vertex.Z < boundingBox.Min.Z)
				{
					boundingBox.Min.Z = vertex.Z;
				}
			}
			surfaceVertices.Add(boundingBox.Min);
			surfaceVertices.Add(boundingBox.Max);
			surfaceVertices.Add(new Vector3(boundingBox.Min.X, boundingBox.Min.Y, boundingBox.Max.Z));
			surfaceVertices.Add(new Vector3(boundingBox.Min.X, boundingBox.Max.Y, boundingBox.Min.Z));
			surfaceVertices.Add(new Vector3(boundingBox.Max.X, boundingBox.Min.Y, boundingBox.Min.Z));
			surfaceVertices.Add(new Vector3(boundingBox.Min.X, boundingBox.Max.Y, boundingBox.Max.Z));
			surfaceVertices.Add(new Vector3(boundingBox.Max.X, boundingBox.Max.Y, boundingBox.Min.Z));
			surfaceVertices.Add(new Vector3(boundingBox.Max.X, boundingBox.Min.Y, boundingBox.Max.Z));
		}
		shapeInformation.Center = default(Vector3);
		if (solidity == MobileMeshSolidity.Solid)
		{
			shapeInformation.Volume = 0f;
			for (int k = 0; k < data.indices.Length; k += 3)
			{
				data.GetTriangle(k, out var v, out var v2, out var v3);
				float num = v.X * (v2.Y * v3.Z - v2.Z * v3.Y) - v2.X * (v.Y * v3.Z - v.Z * v3.Y) + v3.X * (v.Y * v2.Z - v.Z * v2.Y);
				shapeInformation.Volume += num;
				shapeInformation.Center += num * (v + v2 + v3);
			}
			shapeInformation.Center /= shapeInformation.Volume * 4f;
			shapeInformation.Volume /= 6f;
			shapeInformation.Volume = Math.Abs(shapeInformation.Volume);
			data.worldTransform.Translation -= shapeInformation.Center;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 0f;
			float num8 = 0f;
			for (int l = 0; l < data.indices.Length; l += 3)
			{
				data.GetTriangle(l, out var v4, out var v5, out var v6);
				float num9 = v4.X * (v5.Y * v6.Z - v5.Z * v6.Y) - v5.X * (v4.Y * v6.Z - v4.Z * v6.Y) + v6.X * (v4.Y * v5.Z - v4.Z * v5.Y);
				num8 += num9;
				num2 += num9 * (v4.Y * v4.Y + v4.Y * v5.Y + v5.Y * v5.Y + v4.Y * v6.Y + v5.Y * v6.Y + v6.Y * v6.Y + v4.Z * v4.Z + v4.Z * v5.Z + v5.Z * v5.Z + v4.Z * v6.Z + v5.Z * v6.Z + v6.Z * v6.Z);
				num3 += num9 * (v4.X * v4.X + v4.X * v5.X + v5.X * v5.X + v4.X * v6.X + v5.X * v6.X + v6.X * v6.X + v4.Z * v4.Z + v4.Z * v5.Z + v5.Z * v5.Z + v4.Z * v6.Z + v5.Z * v6.Z + v6.Z * v6.Z);
				num4 += num9 * (v4.X * v4.X + v4.X * v5.X + v5.X * v5.X + v4.X * v6.X + v5.X * v6.X + v6.X * v6.X + v4.Y * v4.Y + v4.Y * v5.Y + v5.Y * v5.Y + v4.Y * v6.Y + v5.Y * v6.Y + v6.Y * v6.Y);
				num5 += num9 * (2f * v4.Y * v4.Z + v5.Y * v4.Z + v6.Y * v4.Z + v4.Y * v5.Z + 2f * v5.Y * v5.Z + v6.Y * v5.Z + v4.Y * v6.Z + v5.Y * v6.Z + 2f * v6.Y * v6.Z);
				num6 += num9 * (2f * v4.X * v4.Z + v5.X * v4.Z + v6.X * v4.Z + v4.X * v5.Z + 2f * v5.X * v5.Z + v6.X * v5.Z + v4.X * v6.Z + v5.X * v6.Z + 2f * v6.X * v6.Z);
				num7 += num9 * (2f * v4.X * v4.Y + v5.X * v4.Y + v6.X * v4.Y + v4.X * v5.Y + 2f * v5.X * v5.Y + v6.X * v5.Y + v4.X * v6.Y + v5.X * v6.Y + 2f * v6.X * v6.Y);
			}
			float num10 = 1f / num8;
			float num11 = num10 / 10f;
			float num12 = (0f - num10) / 20f;
			num2 *= num11;
			num3 *= num11;
			num4 *= num11;
			num5 *= num12;
			num6 *= num12;
			num7 *= num12;
			shapeInformation.VolumeDistribution = new Matrix3X3(num2, num6, num7, num6, num3, num5, num7, num5, num4);
		}
		else
		{
			shapeInformation.Center = default(Vector3);
			float num13 = 0f;
			for (int m = 0; m < data.indices.Length; m += 3)
			{
				data.GetTriangle(m, out var v7, out var v8, out var v9);
				Vector3.Subtract(ref v8, ref v7, out var result);
				Vector3.Subtract(ref v9, ref v7, out var result2);
				Vector3.Cross(ref result, ref result2, out var result3);
				float num14 = result3.Length();
				num13 += num14;
				shapeInformation.Center += num14 * (v7 + v8 + v9) / 3f;
			}
			shapeInformation.Center /= num13;
			shapeInformation.Volume = 0f;
			data.worldTransform.Translation -= shapeInformation.Center;
			shapeInformation.VolumeDistribution = default(Matrix3X3);
			for (int n = 0; n < data.indices.Length; n += 3)
			{
				data.GetTriangle(n, out var v10, out var v11, out var v12);
				Vector3.Subtract(ref v11, ref v10, out var result4);
				Vector3.Subtract(ref v12, ref v10, out var result5);
				Vector3.Cross(ref result4, ref result5, out var result6);
				float num15 = result6.Length();
				num13 += num15;
				Matrix3X3.CreateScale(v10.LengthSquared(), out var matrix);
				Matrix3X3.CreateOuterProduct(ref v10, ref v10, out var result7);
				Matrix3X3.Subtract(ref matrix, ref result7, out var result8);
				Matrix3X3.Multiply(ref result8, num15, out result8);
				Matrix3X3.Add(ref shapeInformation.VolumeDistribution, ref result8, out shapeInformation.VolumeDistribution);
				Matrix3X3.CreateScale(v11.LengthSquared(), out matrix);
				Matrix3X3.CreateOuterProduct(ref v11, ref v11, out result7);
				Matrix3X3.Subtract(ref matrix, ref result7, out result7);
				Matrix3X3.Multiply(ref result8, num15, out result8);
				Matrix3X3.Add(ref shapeInformation.VolumeDistribution, ref result8, out shapeInformation.VolumeDistribution);
				Matrix3X3.CreateScale(v12.LengthSquared(), out matrix);
				Matrix3X3.CreateOuterProduct(ref v12, ref v12, out result7);
				Matrix3X3.Subtract(ref matrix, ref result7, out result8);
				Matrix3X3.Multiply(ref result8, num15, out result8);
				Matrix3X3.Add(ref shapeInformation.VolumeDistribution, ref result8, out shapeInformation.VolumeDistribution);
			}
			Matrix3X3.Multiply(ref shapeInformation.VolumeDistribution, 1f / (6f * num13), out shapeInformation.VolumeDistribution);
		}
	}

	private void GetBoundingBox(ref Matrix3X3 o, out BoundingBox boundingBox)
	{
		boundingBox = default(BoundingBox);
		Vector3 vector = new Vector3(o.M11, o.M21, o.M31);
		Vector3 vector2 = new Vector3(o.M12, o.M22, o.M32);
		Vector3 vector3 = new Vector3(o.M13, o.M23, o.M33);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		float num7 = float.MaxValue;
		float num8 = float.MinValue;
		float num9 = float.MaxValue;
		float num10 = float.MinValue;
		float num11 = float.MaxValue;
		float num12 = float.MinValue;
		for (int i = 0; i < surfaceVertices.count; i++)
		{
			Vector3.Dot(ref vector, ref surfaceVertices.Elements[i], out var result);
			Vector3.Dot(ref vector2, ref surfaceVertices.Elements[i], out var result2);
			Vector3.Dot(ref vector3, ref surfaceVertices.Elements[i], out var result3);
			if (result < num7)
			{
				num7 = result;
				num2 = i;
			}
			if (result > num8)
			{
				num8 = result;
				num = i;
			}
			if (result2 < num9)
			{
				num9 = result2;
				num4 = i;
			}
			if (result2 > num10)
			{
				num10 = result2;
				num3 = i;
			}
			if (result3 < num11)
			{
				num11 = result3;
				num6 = i;
			}
			if (result3 > num12)
			{
				num12 = result3;
				num5 = i;
			}
		}
		Vector3.Multiply(ref vector, meshCollisionMargin / (float)Math.Sqrt(vector.Length()), out vector);
		Vector3.Multiply(ref vector2, meshCollisionMargin / (float)Math.Sqrt(vector2.Length()), out vector2);
		Vector3.Multiply(ref vector3, meshCollisionMargin / (float)Math.Sqrt(vector3.Length()), out vector3);
		Vector3 value = surfaceVertices.Elements[num];
		Vector3 value2 = surfaceVertices.Elements[num2];
		Vector3 value3 = surfaceVertices.Elements[num3];
		Vector3 value4 = surfaceVertices.Elements[num4];
		Vector3 value5 = surfaceVertices.Elements[num5];
		Vector3 value6 = surfaceVertices.Elements[num6];
		Vector3.Add(ref value, ref vector, out value);
		Vector3.Subtract(ref value2, ref vector, out value2);
		Vector3.Add(ref value3, ref vector2, out value3);
		Vector3.Subtract(ref value4, ref vector2, out value4);
		Vector3.Add(ref value5, ref vector3, out value5);
		Vector3.Subtract(ref value6, ref vector3, out value6);
		Matrix3X3.Transform(ref value, ref o, out var result4);
		Matrix3X3.Transform(ref value2, ref o, out var result5);
		Matrix3X3.Transform(ref value3, ref o, out var result6);
		Matrix3X3.Transform(ref value4, ref o, out var result7);
		Matrix3X3.Transform(ref value5, ref o, out var result8);
		Matrix3X3.Transform(ref value6, ref o, out var result9);
		boundingBox.Max.X = result4.X;
		boundingBox.Max.Y = result6.Y;
		boundingBox.Max.Z = result8.Z;
		boundingBox.Min.X = result5.X;
		boundingBox.Min.Y = result7.Y;
		boundingBox.Min.Z = result9.Z;
	}

	/// <summary>
	///  Computes the bounding box of the transformed mesh shape.
	/// </summary>
	/// <param name="shapeTransform">Transform to apply to the shape during the bounding box calculation.</param>
	/// <param name="boundingBox">Bounding box containing the transformed mesh shape.</param>
	public void GetBoundingBox(ref RigidTransform shapeTransform, out BoundingBox boundingBox)
	{
		Matrix3X3.CreateFromQuaternion(ref shapeTransform.Orientation, out var result);
		GetBoundingBox(ref result, out boundingBox);
		boundingBox.Max.X += shapeTransform.Position.X;
		boundingBox.Max.Y += shapeTransform.Position.Y;
		boundingBox.Max.Z += shapeTransform.Position.Z;
		boundingBox.Min.X += shapeTransform.Position.X;
		boundingBox.Min.Y += shapeTransform.Position.Y;
		boundingBox.Min.Z += shapeTransform.Position.Z;
	}

	/// <summary>
	/// Gets the bounding box of the mesh transformed first into world space, and then into the local space of another affine transform.
	/// </summary>
	/// <param name="shapeTransform">Transform to use to put the shape into world space.</param>
	/// <param name="spaceTransform">Used as the frame of reference to compute the bounding box.
	/// In effect, the shape is transformed by the inverse of the space transform to compute its bounding box in local space.</param>
	/// <param name="boundingBox">Bounding box in the local space.</param>
	public void GetLocalBoundingBox(ref RigidTransform shapeTransform, ref AffineTransform spaceTransform, out BoundingBox boundingBox)
	{
		boundingBox = default(BoundingBox);
		AffineTransform.Invert(ref spaceTransform, out var inverse);
		AffineTransform.Multiply(ref shapeTransform, ref inverse, out inverse);
		GetBoundingBox(ref inverse.LinearTransform, out boundingBox);
		boundingBox.Max.X += inverse.Translation.X;
		boundingBox.Max.Y += inverse.Translation.Y;
		boundingBox.Max.Z += inverse.Translation.Z;
		boundingBox.Min.X += inverse.Translation.X;
		boundingBox.Min.Y += inverse.Translation.Y;
		boundingBox.Min.Z += inverse.Translation.Z;
	}

	/// <summary>
	/// Gets the bounding box of the mesh transformed first into world space, and then into the local space of another affine transform.
	/// </summary>
	/// <param name="shapeTransform">Transform to use to put the shape into world space.</param>
	/// <param name="spaceTransform">Used as the frame of reference to compute the bounding box.
	/// In effect, the shape is transformed by the inverse of the space transform to compute its bounding box in local space.</param>
	/// <param name="sweep">World space sweep direction to transform and add to the bounding box.</param>
	/// <param name="boundingBox">Bounding box in the local space.</param>
	public void GetSweptLocalBoundingBox(ref RigidTransform shapeTransform, ref AffineTransform spaceTransform, ref Vector3 sweep, out BoundingBox boundingBox)
	{
		GetLocalBoundingBox(ref shapeTransform, ref spaceTransform, out boundingBox);
		Matrix3X3.TransformTranspose(ref sweep, ref spaceTransform.LinearTransform, out var result);
		Toolbox.ExpandBoundingBox(ref boundingBox, ref result);
	}

	/// <summary>
	/// Computes the volume, center of mass, and volume distribution of the shape.
	/// </summary>
	/// <param name="shapeInfo">Data about the shape.</param>
	public override void ComputeDistributionInformation(out ShapeDistributionInformation shapeInfo)
	{
		ComputeShapeInformation(TriangleMesh.Data as TransformableMeshData, out shapeInfo);
	}

	public override EntityCollidable GetCollidableInstance()
	{
		return new MobileMeshCollidable(this);
	}
}
