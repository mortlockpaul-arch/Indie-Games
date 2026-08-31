using System;
using System.Collections.Generic;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionShapes;

/// <summary>
///  Shape composed of multiple other shapes.
/// </summary>
public class CompoundShape : EntityShape
{
	internal RawList<CompoundShapeEntry> shapes;

	/// <summary>
	///  Gets the list of shapes in the compound shape.
	/// </summary>
	public ReadOnlyList<CompoundShapeEntry> Shapes => new ReadOnlyList<CompoundShapeEntry>(shapes);

	/// <summary>
	///  Constructs a compound shape.
	/// </summary>
	/// <param name="shapes">Shape entries used to create the compound.</param>
	///  <param name="center">Computed center of the compound shape, using the entry weights.</param>
	public CompoundShape(IList<CompoundShapeEntry> shapes, out Vector3 center)
	{
		if (shapes.Count > 0)
		{
			center = ComputeCenter(shapes);
			this.shapes = new RawList<CompoundShapeEntry>(shapes);
			for (int i = 0; i < this.shapes.count; i++)
			{
				this.shapes.Elements[i].LocalTransform.Position -= center;
			}
			return;
		}
		throw new Exception("Compound shape must have at least 1 subshape.");
	}

	/// <summary>
	///  Constructs a compound shape.
	/// </summary>
	/// <param name="shapes">Shape entries used to create the compound.</param>
	public CompoundShape(IList<CompoundShapeEntry> shapes)
	{
		if (shapes.Count > 0)
		{
			Vector3 vector = ComputeCenter(shapes);
			this.shapes = new RawList<CompoundShapeEntry>(shapes);
			for (int i = 0; i < this.shapes.count; i++)
			{
				this.shapes.Elements[i].LocalTransform.Position -= vector;
			}
			return;
		}
		throw new Exception("Compound shape must have at least 1 subshape.");
	}

	/// <summary>
	/// Computes the center of the shape.  This can be considered its 
	/// center of mass, based on the weightings of entries in the shape.
	/// For properly calibrated compound shapes, this will return a zero vector,
	/// since the shape recenters itself on construction.
	/// </summary>
	/// <returns>Center of the shape.</returns>
	public override Vector3 ComputeCenter()
	{
		float num = 0f;
		Vector3 value = default(Vector3);
		for (int i = 0; i < shapes.count; i++)
		{
			num += shapes.Elements[i].Weight;
			Vector3.Multiply(ref shapes.Elements[i].LocalTransform.Position, shapes.Elements[i].Weight, out var result);
			Vector3.Add(ref value, ref result, out value);
		}
		Vector3.Multiply(ref value, 1f / num, out value);
		return value;
	}

	/// <summary>
	///  Computes the center of a compound using its child data.
	///  Children are weighted using their volumes for contribution to the center of 'mass.'
	/// </summary>
	/// <param name="childData">Child data to use to compute the center.</param>
	/// <returns>Center of the children.</returns>
	public static Vector3 ComputeCenter(IList<CompoundChildData> childData)
	{
		Vector3 value = default(Vector3);
		float num = 0f;
		for (int i = 0; i < childData.Count; i++)
		{
			float num2 = childData[i].Entry.Shape.ComputeVolume();
			num += num2;
			value += childData[i].Entry.LocalTransform.Position * num2;
		}
		Vector3.Divide(ref value, num, out value);
		return value;
	}

	/// <summary>
	///  Computes the center of a compound using its child data.
	///  Children are weighted using their volumes for contribution to the center of 'mass.'
	/// </summary>
	/// <param name="childData">Child data to use to compute the center.</param>
	/// <returns>Center of the children.</returns>
	public static Vector3 ComputeCenter(IList<CompoundShapeEntry> childData)
	{
		Vector3 value = default(Vector3);
		float num = 0f;
		for (int i = 0; i < childData.Count; i++)
		{
			float weight = childData[i].Weight;
			num += weight;
			value += childData[i].LocalTransform.Position * weight;
		}
		Vector3.Divide(ref value, num, out value);
		return value;
	}

	/// <summary>
	/// Computes the volume of the shape.
	/// </summary>
	/// <returns>Volume of the shape.</returns>
	public override float ComputeVolume()
	{
		float num = 0f;
		for (int i = 0; i < shapes.count; i++)
		{
			num += shapes.Elements[i].Shape.ComputeVolume();
		}
		return num;
	}

	/// <summary>
	/// Computes the volume distribution of the shape as well as its volume.
	/// The volume distribution can be used to compute inertia tensors when
	/// paired with mass and other tuning factors.
	/// </summary>
	/// <param name="volume">Volume of the shape.</param>
	/// <returns>Volume distribution of the shape.</returns>
	public override Matrix3X3 ComputeVolumeDistribution(out float volume)
	{
		volume = ComputeVolume();
		return ComputeVolumeDistribution();
	}

	/// <summary>
	/// Computes the volume distribution of the shape.
	/// </summary>
	/// <returns>Volume distribution of the shape.</returns>
	public override Matrix3X3 ComputeVolumeDistribution()
	{
		Matrix3X3 b = default(Matrix3X3);
		float num = 0f;
		for (int i = 0; i < shapes.count; i++)
		{
			num += shapes.Elements[i].Weight;
			GetContribution(shapes.Elements[i].Shape, ref shapes.Elements[i].LocalTransform, ref Toolbox.ZeroVector, shapes.Elements[i].Weight, out var contribution);
			Matrix3X3.Add(ref contribution, ref b, out b);
		}
		Matrix3X3.Multiply(ref b, 1f / num, out b);
		return b;
	}

	/// <summary>
	/// Computes the volume distribution and center of the shape.
	/// </summary>
	/// <param name="entries">Mass-weighted entries of the compound.</param>
	/// <param name="center">Center of the compound.</param>
	/// <returns>Volume distribution of the shape.</returns>
	public static Matrix3X3 ComputeVolumeDistribution(IList<CompoundShapeEntry> entries, out Vector3 center)
	{
		center = default(Vector3);
		float num = 0f;
		for (int i = 0; i < entries.Count; i++)
		{
			center += entries[i].LocalTransform.Position * entries[i].Weight;
			num += entries[i].Weight;
		}
		center /= num;
		Matrix3X3 a = default(Matrix3X3);
		for (int j = 0; j < entries.Count; j++)
		{
			RigidTransform transform = entries[j].LocalTransform;
			GetContribution(entries[j].Shape, ref transform, ref center, entries[j].Weight, out var contribution);
			Matrix3X3.Add(ref a, ref contribution, out a);
		}
		return a;
	}

	/// <summary>
	///  Gets the volume distribution contributed by a single shape.
	/// </summary>
	/// <param name="shape">Shape to use to compute a contribution.</param>
	/// <param name="transform">Transform of the shape.</param>
	/// <param name="center">Center to use when computing the distribution.</param>
	/// <param name="weight">Weighting to apply to the contribution.</param>
	/// <param name="contribution">Volume distribution of the contribution.</param>
	public static void GetContribution(EntityShape shape, ref RigidTransform transform, ref Vector3 center, float weight, out Matrix3X3 contribution)
	{
		contribution = shape.ComputeVolumeDistribution();
		TransformContribution(ref transform, ref center, ref contribution, weight, out contribution);
	}

	/// <summary>
	/// Modifies a contribution using a transform, position, and weight.
	/// </summary>
	/// <param name="transform">Transform to use to modify the contribution.</param>
	/// <param name="center">Center to use to modify the contribution.</param>
	/// <param name="baseContribution">Original unmodified contribution.</param>
	/// <param name="weight">Weight of the contribution.</param>
	/// <param name="contribution">Transformed contribution.</param>
	public static void TransformContribution(ref RigidTransform transform, ref Vector3 center, ref Matrix3X3 baseContribution, float weight, out Matrix3X3 contribution)
	{
		Matrix3X3.CreateFromQuaternion(ref transform.Orientation, out var result);
		Matrix3X3.MultiplyTransposed(ref result, ref baseContribution, out var result2);
		Matrix3X3.Multiply(ref result2, ref result, out result2);
		contribution = result2;
		Vector3.Subtract(ref transform.Position, ref center, out var result3);
		Matrix3X3.CreateScale(result3.LengthSquared(), out var matrix);
		Matrix3X3.CreateOuterProduct(ref result3, ref result3, out var result4);
		Matrix3X3.Subtract(ref matrix, ref result4, out result2);
		Matrix3X3.Add(ref contribution, ref result2, out contribution);
		Matrix3X3.Multiply(ref contribution, weight, out contribution);
	}

	/// <summary>
	/// Retrieves an instance of an EntityCollidable that uses this EntityShape.  Mainly used by compound bodies.
	/// </summary>
	/// <returns>EntityCollidable that uses this shape.</returns>
	public override EntityCollidable GetCollidableInstance()
	{
		return new CompoundCollidable(this);
	}

	/// <summary>
	/// Computes the center of the shape and its volume.
	/// </summary>
	/// <param name="volume">Volume of the compound.</param>
	/// <returns>Volume of the compound.</returns>
	public override Vector3 ComputeCenter(out float volume)
	{
		volume = ComputeVolume();
		return ComputeCenter();
	}

	/// <summary>
	/// Computes a variety of shape information all at once.
	/// </summary>
	/// <param name="shapeInfo">Properties of the shape.</param>
	public override void ComputeDistributionInformation(out ShapeDistributionInformation shapeInfo)
	{
		shapeInfo.VolumeDistribution = ComputeVolumeDistribution(out shapeInfo.Volume);
		shapeInfo.Center = ComputeCenter();
	}

	/// <summary>
	/// Computes and returns the volume, volume distribution, and center contributions from each child shape in the compound shape.
	/// </summary>
	/// <returns>Volume, volume distribution, and center contributions from each child shape in the compound shape.</returns>
	public ShapeDistributionInformation[] ComputeChildContributions()
	{
		ShapeDistributionInformation[] array = new ShapeDistributionInformation[shapes.count];
		for (int i = 0; i < shapes.count; i++)
		{
			shapes.Elements[i].Shape.ComputeDistributionInformation(out array[i]);
		}
		return array;
	}
}
