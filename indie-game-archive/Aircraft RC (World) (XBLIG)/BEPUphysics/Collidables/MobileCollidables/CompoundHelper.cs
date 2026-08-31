using System;
using System.Collections.Generic;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Collidables.MobileCollidables;

/// <summary>
/// Contains methods to help with splitting compound objects into multiple pieces.
/// </summary>
public static class CompoundHelper
{
	/// <summary>
	/// Splits a single compound collidable into two separate compound collidables and computes information needed by the simulation.
	/// </summary>
	/// <param name="splitPredicate">Delegate which determines if a child in the original compound should be moved to the new compound.</param>
	/// <param name="a">Original compound to be split.  Children in this compound will be removed and added to the other compound.</param>
	/// <param name="b">Compound to receive children removed from the original compound.</param>
	/// <returns>Whether or not the predicate returned true for any element in the original compound and split the compound.</returns>
	public static bool SplitCompound(Func<CompoundChild, bool> splitPredicate, Entity<CompoundCollidable> a, out Entity<CompoundCollidable> b)
	{
		ShapeDistributionInformation[] childContributions = a.CollisionInformation.Shape.ComputeChildContributions();
		return SplitCompound(childContributions, splitPredicate, a, out b);
	}

	/// <summary>
	/// Splits a single compound collidable into two separate compound collidables and computes information needed by the simulation.
	/// </summary>
	/// <param name="childContributions">List of distribution information associated with each child shape of the whole compound shape used by the compound being split.</param>
	/// <param name="splitPredicate">Delegate which determines if a child in the original compound should be moved to the new compound.</param>
	/// <param name="a">Original compound to be split.  Children in this compound will be removed and added to the other compound.</param>
	/// <param name="b">Compound to receive children removed from the original compound.</param>
	/// <returns>Whether or not the predicate returned true for any element in the original compound and split the compound.</returns>
	public static bool SplitCompound(IList<ShapeDistributionInformation> childContributions, Func<CompoundChild, bool> splitPredicate, Entity<CompoundCollidable> a, out Entity<CompoundCollidable> b)
	{
		if (SplitCompound(childContributions, splitPredicate, a, out b, out var _, out var _))
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Splits a single compound collidable into two separate compound collidables and computes information needed by the simulation.
	/// </summary>
	/// <param name="childContributions">List of distribution information associated with each child shape of the whole compound shape used by the compound being split.</param>
	/// <param name="splitPredicate">Delegate which determines if a child in the original compound should be moved to the new compound.</param>
	/// <param name="a">Original compound to be split.  Children in this compound will be removed and added to the other compound.</param>
	/// <param name="b">Compound to receive children removed from the original compound.</param>
	/// <param name="distributionInfoA">Volume, volume distribution, and center information about the new form of the original compound collidable.</param>
	/// <param name="distributionInfoB">Volume, volume distribution, and center information about the new compound collidable.</param>
	/// <returns>Whether or not the predicate returned true for any element in the original compound and split the compound.</returns>
	public static bool SplitCompound(IList<ShapeDistributionInformation> childContributions, Func<CompoundChild, bool> splitPredicate, Entity<CompoundCollidable> a, out Entity<CompoundCollidable> b, out ShapeDistributionInformation distributionInfoA, out ShapeDistributionInformation distributionInfoB)
	{
		CompoundCollidable compoundCollidable = new CompoundCollidable();
		compoundCollidable.Shape = a.CollisionInformation.Shape;
		CompoundCollidable compoundCollidable2 = compoundCollidable;
		b = null;
		if (SplitCompound(childContributions, splitPredicate, a.CollisionInformation, compoundCollidable2, out distributionInfoA, out distributionInfoB, out var weightA, out var weightB))
		{
			float mass = a.mass;
			if (a.CollisionInformation.children.count > 0)
			{
				float num = weightA / (weightA + weightB) * mass;
				Matrix3X3.Multiply(ref distributionInfoA.VolumeDistribution, num * InertiaHelper.InertiaTensorScale, out distributionInfoA.VolumeDistribution);
				a.Initialize(a.CollisionInformation, num, distributionInfoA.VolumeDistribution, distributionInfoA.Volume);
			}
			if (compoundCollidable2.children.count > 0)
			{
				float num2 = weightB / (weightA + weightB) * mass;
				Matrix3X3.Multiply(ref distributionInfoB.VolumeDistribution, num2 * InertiaHelper.InertiaTensorScale, out distributionInfoB.VolumeDistribution);
				b = new Entity<CompoundCollidable>();
				b.Initialize(compoundCollidable2, num2, distributionInfoB.VolumeDistribution, distributionInfoB.Volume);
			}
			SplitReposition(a, b, ref distributionInfoA, ref distributionInfoB, weightA, weightB);
			return true;
		}
		return false;
	}

	private static void SplitReposition(Entity a, Entity b, ref ShapeDistributionInformation distributionInfoA, ref ShapeDistributionInformation distributionInfoB, float weightA, float weightB)
	{
		Vector3.Multiply(ref distributionInfoA.Center, weightA, out var result);
		Vector3.Multiply(ref distributionInfoB.Center, weightB, out var result2);
		Vector3.Add(ref result, ref result2, out var result3);
		Vector3.Divide(ref result3, weightA + weightB, out result3);
		Vector3.Subtract(ref distributionInfoA.Center, ref result3, out var result4);
		Vector3.Subtract(ref distributionInfoB.Center, ref result3, out var result5);
		Vector3 position = a.position;
		b.Orientation = a.Orientation;
		Vector3 vector = Vector3.Transform(result4, a.Orientation);
		Vector3 vector2 = Vector3.Transform(result5, a.Orientation);
		a.Position = position + vector;
		b.Position = position + vector2;
		Vector3 linearVelocity = a.linearVelocity;
		Vector3 vector3 = (b.AngularVelocity = (a.AngularVelocity = a.angularVelocity));
		a.LinearVelocity = linearVelocity + Vector3.Cross(vector3, vector);
		b.LinearVelocity = linearVelocity + Vector3.Cross(vector3, vector2);
	}

	/// <summary>
	/// Splits a single compound collidable into two separate compound collidables and computes information needed by the simulation.
	/// </summary>
	/// <param name="splitPredicate">Delegate which determines if a child in the original compound should be moved to the new compound.</param>
	/// <param name="a">Original compound to be split.  Children in this compound will be removed and added to the other compound.</param>
	/// <param name="b">Compound to receive children removed from the original compound.</param>
	/// <returns>Whether or not the predicate returned true for any element in the original compound and split the compound.</returns>
	public static bool SplitCompound(Func<CompoundChild, bool> splitPredicate, Entity<CompoundCollidable> a, Entity<CompoundCollidable> b)
	{
		ShapeDistributionInformation[] childContributions = a.CollisionInformation.Shape.ComputeChildContributions();
		if (SplitCompound(childContributions, splitPredicate, a, b))
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Splits a single compound collidable into two separate compound collidables and computes information needed by the simulation.
	/// </summary>
	/// <param name="childContributions">List of distribution information associated with each child shape of the whole compound shape used by the compound being split.</param>
	/// <param name="splitPredicate">Delegate which determines if a child in the original compound should be moved to the new compound.</param>
	/// <param name="a">Original compound to be split.  Children in this compound will be removed and added to the other compound.</param>
	/// <param name="b">Compound to receive children removed from the original compound.</param>
	/// <returns>Whether or not the predicate returned true for any element in the original compound and split the compound.</returns>
	public static bool SplitCompound(IList<ShapeDistributionInformation> childContributions, Func<CompoundChild, bool> splitPredicate, Entity<CompoundCollidable> a, Entity<CompoundCollidable> b)
	{
		if (SplitCompound(childContributions, splitPredicate, a, b, out var _, out var _))
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Splits a single compound collidable into two separate compound collidables and computes information needed by the simulation.
	/// </summary>
	/// <param name="childContributions">List of distribution information associated with each child shape of the whole compound shape used by the compound being split.</param>
	/// <param name="splitPredicate">Delegate which determines if a child in the original compound should be moved to the new compound.</param>
	/// <param name="distributionInfoA">Volume, volume distribution, and center information about the new form of the original compound collidable.</param>
	/// <param name="distributionInfoB">Volume, volume distribution, and center information about the new compound collidable.</param>
	/// <param name="a">Original compound to be split.  Children in this compound will be removed and added to the other compound.</param>
	/// <param name="b">Compound to receive children removed from the original compound.</param>
	/// <returns>Whether or not the predicate returned true for any element in the original compound and split the compound.</returns>
	public static bool SplitCompound(IList<ShapeDistributionInformation> childContributions, Func<CompoundChild, bool> splitPredicate, Entity<CompoundCollidable> a, Entity<CompoundCollidable> b, out ShapeDistributionInformation distributionInfoA, out ShapeDistributionInformation distributionInfoB)
	{
		if (SplitCompound(childContributions, splitPredicate, a.CollisionInformation, b.CollisionInformation, out distributionInfoA, out distributionInfoB, out var weightA, out var weightB))
		{
			float mass = a.mass;
			if (a.CollisionInformation.children.count > 0)
			{
				float num = weightA / (weightA + weightB) * mass;
				Matrix3X3.Multiply(ref distributionInfoA.VolumeDistribution, num * InertiaHelper.InertiaTensorScale, out distributionInfoA.VolumeDistribution);
				a.Initialize(a.CollisionInformation, num, distributionInfoA.VolumeDistribution, distributionInfoA.Volume);
			}
			if (b.CollisionInformation.children.count > 0)
			{
				float num2 = weightB / (weightA + weightB) * mass;
				Matrix3X3.Multiply(ref distributionInfoB.VolumeDistribution, num2 * InertiaHelper.InertiaTensorScale, out distributionInfoB.VolumeDistribution);
				b.Initialize(b.CollisionInformation, num2, distributionInfoB.VolumeDistribution, distributionInfoB.Volume);
			}
			SplitReposition(a, b, ref distributionInfoA, ref distributionInfoB, weightA, weightB);
			return true;
		}
		return false;
	}

	/// <summary>
	/// Splits a single compound collidable into two separate compound collidables and computes information needed by the simulation.
	/// </summary>
	/// <param name="childContributions">List of distribution information associated with each child shape of the whole compound shape used by the compound being split.</param>
	/// <param name="splitPredicate">Delegate which determines if a child in the original compound should be moved to the new compound.</param>
	/// <param name="a">Original compound to be split.  Children in this compound will be removed and added to the other compound.</param>
	/// <param name="b">Compound to receive children removed from the original compound.</param>
	/// <param name="distributionInfoA">Volume, volume distribution, and center information about the new form of the original compound collidable.</param>
	/// <param name="distributionInfoB">Volume, volume distribution, and center information about the new compound collidable.</param>
	/// <param name="weightA">Total weight associated with the new form of the original compound collidable.</param>
	/// <param name="weightB">Total weight associated with the new compound collidable.</param>
	/// <returns>Whether or not the predicate returned true for any element in the original compound and split the compound.</returns>
	public static bool SplitCompound(IList<ShapeDistributionInformation> childContributions, Func<CompoundChild, bool> splitPredicate, CompoundCollidable a, CompoundCollidable b, out ShapeDistributionInformation distributionInfoA, out ShapeDistributionInformation distributionInfoB, out float weightA, out float weightB)
	{
		bool flag = false;
		for (int num = a.children.count - 1; num >= 0; num--)
		{
			CompoundChild compoundChild = a.children.Elements[num];
			if (splitPredicate(compoundChild))
			{
				flag = true;
				a.children.FastRemoveAt(num);
				b.children.Add(compoundChild);
				compoundChild.CollisionInformation.events.Parent = b.Events;
			}
		}
		if (!flag)
		{
			distributionInfoA = default(ShapeDistributionInformation);
			distributionInfoB = default(ShapeDistributionInformation);
			weightA = 0f;
			weightB = 0f;
			return false;
		}
		distributionInfoA = default(ShapeDistributionInformation);
		weightA = 0f;
		distributionInfoB = default(ShapeDistributionInformation);
		weightB = 0f;
		for (int num2 = a.children.count - 1; num2 >= 0; num2--)
		{
			CompoundChild compoundChild2 = a.children.Elements[num2];
			CompoundShapeEntry entry = compoundChild2.Entry;
			ShapeDistributionInformation shapeDistributionInformation = childContributions[compoundChild2.shapeIndex];
			Vector3.Add(ref shapeDistributionInformation.Center, ref entry.LocalTransform.Position, out shapeDistributionInformation.Center);
			Vector3.Multiply(ref shapeDistributionInformation.Center, compoundChild2.Entry.Weight, out shapeDistributionInformation.Center);
			Vector3.Add(ref shapeDistributionInformation.Center, ref distributionInfoA.Center, out distributionInfoA.Center);
			distributionInfoA.Volume += shapeDistributionInformation.Volume;
			weightA += entry.Weight;
		}
		for (int num3 = b.children.count - 1; num3 >= 0; num3--)
		{
			CompoundChild compoundChild3 = b.children.Elements[num3];
			CompoundShapeEntry entry2 = compoundChild3.Entry;
			ShapeDistributionInformation shapeDistributionInformation2 = childContributions[compoundChild3.shapeIndex];
			Vector3.Add(ref shapeDistributionInformation2.Center, ref entry2.LocalTransform.Position, out shapeDistributionInformation2.Center);
			Vector3.Multiply(ref shapeDistributionInformation2.Center, compoundChild3.Entry.Weight, out shapeDistributionInformation2.Center);
			Vector3.Add(ref shapeDistributionInformation2.Center, ref distributionInfoB.Center, out distributionInfoB.Center);
			distributionInfoB.Volume += shapeDistributionInformation2.Volume;
			weightB += entry2.Weight;
		}
		if (weightA > 0f)
		{
			Vector3.Divide(ref distributionInfoA.Center, weightA, out distributionInfoA.Center);
		}
		if (weightB > 0f)
		{
			Vector3.Divide(ref distributionInfoB.Center, weightB, out distributionInfoB.Center);
		}
		Vector3.Negate(ref distributionInfoA.Center, out var result);
		Vector3.Negate(ref distributionInfoB.Center, out var result2);
		for (int num4 = a.children.count - 1; num4 >= 0; num4--)
		{
			CompoundChild compoundChild4 = a.children.Elements[num4];
			CompoundShapeEntry entry3 = compoundChild4.Entry;
			Quaternion.Conjugate(ref entry3.LocalTransform.Orientation, out var result3);
			Vector3.Transform(ref result, ref result3, out var result4);
			compoundChild4.CollisionInformation.localPosition = result4;
			ShapeDistributionInformation shapeDistributionInformation3 = childContributions[compoundChild4.shapeIndex];
			CompoundShape.TransformContribution(ref entry3.LocalTransform, ref distributionInfoA.Center, ref shapeDistributionInformation3.VolumeDistribution, entry3.Weight, out shapeDistributionInformation3.VolumeDistribution);
			Matrix3X3.Add(ref shapeDistributionInformation3.VolumeDistribution, ref distributionInfoA.VolumeDistribution, out distributionInfoA.VolumeDistribution);
		}
		for (int num5 = b.children.count - 1; num5 >= 0; num5--)
		{
			CompoundChild compoundChild5 = b.children.Elements[num5];
			CompoundShapeEntry entry4 = compoundChild5.Entry;
			Quaternion.Conjugate(ref entry4.LocalTransform.Orientation, out var result5);
			Vector3.Transform(ref result2, ref result5, out var result6);
			compoundChild5.CollisionInformation.localPosition = result6;
			ShapeDistributionInformation shapeDistributionInformation4 = childContributions[compoundChild5.shapeIndex];
			CompoundShape.TransformContribution(ref entry4.LocalTransform, ref distributionInfoB.Center, ref shapeDistributionInformation4.VolumeDistribution, entry4.Weight, out shapeDistributionInformation4.VolumeDistribution);
			Matrix3X3.Add(ref shapeDistributionInformation4.VolumeDistribution, ref distributionInfoB.VolumeDistribution, out distributionInfoB.VolumeDistribution);
		}
		Matrix3X3.Multiply(ref distributionInfoA.VolumeDistribution, 1f / weightA, out distributionInfoA.VolumeDistribution);
		Matrix3X3.Multiply(ref distributionInfoB.VolumeDistribution, 1f / weightB, out distributionInfoB.VolumeDistribution);
		a.hierarchy.Tree.Reconstruct(a.children);
		b.hierarchy.Tree.Reconstruct(b.children);
		return true;
	}

	private static void RemoveReposition(Entity compound, ref ShapeDistributionInformation distributionInfo, float weight, float removedWeight, ref Vector3 removedCenter)
	{
		Vector3.Multiply(ref distributionInfo.Center, weight, out var result);
		Vector3.Multiply(ref removedCenter, removedWeight, out var result2);
		Vector3.Add(ref result, ref result2, out var result3);
		Vector3.Divide(ref result3, weight + removedWeight, out result3);
		Vector3.Subtract(ref distributionInfo.Center, ref result3, out var result4);
		Vector3 position = compound.position;
		Vector3 vector = Vector3.Transform(result4, compound.orientation);
		compound.Position = position + vector;
		Vector3 linearVelocity = compound.linearVelocity;
		Vector3 vector2 = (compound.AngularVelocity = compound.angularVelocity);
		compound.LinearVelocity = linearVelocity + Vector3.Cross(vector2, vector);
	}

	/// <summary>
	/// Removes a child from a compound body.
	/// </summary>
	/// <param name="childContributions">List of distribution information associated with each child shape of the whole compound shape used by the compound being split.</param>
	/// <param name="removalPredicate">Delegate which determines if a child in the original compound should be moved to the new compound.</param>
	/// <param name="compound">Original compound to have a child removed.</param>
	/// <returns>Whether or not the predicate returned true for any element in the original compound and split the compound.</returns>
	public static bool RemoveChildFromCompound(Entity<CompoundCollidable> compound, Func<CompoundChild, bool> removalPredicate, IList<ShapeDistributionInformation> childContributions)
	{
		if (RemoveChildFromCompound(compound, removalPredicate, childContributions, out var _))
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Removes a child from a compound body.
	/// </summary>
	/// <param name="childContributions">List of distribution information associated with each child shape of the whole compound shape used by the compound being split.</param>
	/// <param name="removalPredicate">Delegate which determines if a child in the original compound should be moved to the new compound.</param>
	/// <param name="distributionInfo">Volume, volume distribution, and center information about the new form of the original compound collidable.</param>
	/// <param name="compound">Original compound to have a child removed.</param>
	/// <returns>Whether or not the predicate returned true for any element in the original compound and split the compound.</returns>
	public static bool RemoveChildFromCompound(Entity<CompoundCollidable> compound, Func<CompoundChild, bool> removalPredicate, IList<ShapeDistributionInformation> childContributions, out ShapeDistributionInformation distributionInfo)
	{
		if (RemoveChildFromCompound(compound.CollisionInformation, removalPredicate, childContributions, out distributionInfo, out var weight, out var removedWeight, out var removedCenter))
		{
			if (compound.CollisionInformation.Children.Count > 0)
			{
				float mass = compound.mass;
				float num = weight / (weight + removedWeight) * mass;
				Matrix3X3.Multiply(ref distributionInfo.VolumeDistribution, num * InertiaHelper.InertiaTensorScale, out distributionInfo.VolumeDistribution);
				compound.Initialize(compound.CollisionInformation, num, distributionInfo.VolumeDistribution, distributionInfo.Volume);
				RemoveReposition(compound, ref distributionInfo, weight, removedWeight, ref removedCenter);
			}
			return true;
		}
		return false;
	}

	/// <summary>
	/// Removes a child from a compound collidable.
	/// </summary>
	/// <param name="compound">Compound collidable to remove a child from.</param>
	/// <param name="removalPredicate">Callback which analyzes a child and determines if it should be removed from the compound.</param>
	/// <param name="childContributions">Distribution contributions from all shapes in the compound shape.  This can include shapes which are not represented in the compound.</param>
	/// <param name="distributionInfo">Distribution information of the new compound.</param>
	/// <param name="weight">Total weight of the new compound.</param>
	/// <param name="removedWeight">Weight removed from the compound.</param>
	/// <param name="removedCenter">Center of the chunk removed from the compound.</param>
	/// <returns>Whether or not any removal took place.</returns>
	public static bool RemoveChildFromCompound(CompoundCollidable compound, Func<CompoundChild, bool> removalPredicate, IList<ShapeDistributionInformation> childContributions, out ShapeDistributionInformation distributionInfo, out float weight, out float removedWeight, out Vector3 removedCenter)
	{
		bool flag = false;
		removedWeight = 0f;
		removedCenter = default(Vector3);
		for (int num = compound.children.count - 1; num >= 0; num--)
		{
			CompoundChild compoundChild = compound.children.Elements[num];
			if (removalPredicate(compoundChild))
			{
				flag = true;
				CompoundShapeEntry entry = compoundChild.Entry;
				removedWeight += entry.Weight;
				Vector3.Multiply(ref entry.LocalTransform.Position, entry.Weight, out var result);
				Vector3.Add(ref removedCenter, ref result, out removedCenter);
				compoundChild.CollisionInformation.events.Parent = null;
				compound.children.FastRemoveAt(num);
			}
		}
		if (!flag)
		{
			distributionInfo = default(ShapeDistributionInformation);
			weight = 0f;
			return false;
		}
		if (removedWeight > 0f)
		{
			Vector3.Divide(ref removedCenter, removedWeight, out removedCenter);
		}
		distributionInfo = default(ShapeDistributionInformation);
		weight = 0f;
		for (int num2 = compound.children.count - 1; num2 >= 0; num2--)
		{
			CompoundChild compoundChild2 = compound.children.Elements[num2];
			CompoundShapeEntry entry2 = compoundChild2.Entry;
			ShapeDistributionInformation shapeDistributionInformation = childContributions[compoundChild2.shapeIndex];
			Vector3.Add(ref shapeDistributionInformation.Center, ref entry2.LocalTransform.Position, out shapeDistributionInformation.Center);
			Vector3.Multiply(ref shapeDistributionInformation.Center, compoundChild2.Entry.Weight, out shapeDistributionInformation.Center);
			Vector3.Add(ref shapeDistributionInformation.Center, ref distributionInfo.Center, out distributionInfo.Center);
			distributionInfo.Volume += shapeDistributionInformation.Volume;
			weight += entry2.Weight;
		}
		Vector3.Divide(ref distributionInfo.Center, weight, out distributionInfo.Center);
		Vector3.Negate(ref distributionInfo.Center, out var result2);
		for (int num3 = compound.children.count - 1; num3 >= 0; num3--)
		{
			CompoundChild compoundChild3 = compound.children.Elements[num3];
			CompoundShapeEntry entry3 = compoundChild3.Entry;
			Quaternion.Conjugate(ref entry3.LocalTransform.Orientation, out var result3);
			Vector3.Transform(ref result2, ref result3, out var result4);
			compoundChild3.CollisionInformation.localPosition = result4;
			ShapeDistributionInformation shapeDistributionInformation2 = childContributions[compoundChild3.shapeIndex];
			CompoundShape.TransformContribution(ref entry3.LocalTransform, ref distributionInfo.Center, ref shapeDistributionInformation2.VolumeDistribution, entry3.Weight, out shapeDistributionInformation2.VolumeDistribution);
			Matrix3X3.Add(ref shapeDistributionInformation2.VolumeDistribution, ref distributionInfo.VolumeDistribution, out distributionInfo.VolumeDistribution);
		}
		Matrix3X3.Multiply(ref distributionInfo.VolumeDistribution, 1f / weight, out distributionInfo.VolumeDistribution);
		compound.hierarchy.Tree.Reconstruct(compound.children);
		return true;
	}

	/// <summary>
	/// Constructs a compound collidable containing only the specified subset of children.
	/// </summary>
	/// <param name="shape">Shape to base the compound collidable on.</param>
	/// <param name="childIndices">Indices of child shapes from the CompoundShape to include in the compound collidable.</param>
	/// <returns>Compound collidable containing only the specified subset of children.</returns>
	public static CompoundCollidable CreatePartialCompoundCollidable(CompoundShape shape, IList<int> childIndices)
	{
		if (childIndices.Count == 0)
		{
			throw new Exception("Cannot create a compound from zero shapes.");
		}
		CompoundCollidable compoundCollidable = new CompoundCollidable();
		Vector3 value = default(Vector3);
		float num = 0f;
		for (int i = 0; i < childIndices.Count; i++)
		{
			CompoundShapeEntry compoundShapeEntry = shape.shapes[childIndices[i]];
			compoundCollidable.children.Add(new CompoundChild(shape, compoundShapeEntry.Shape.GetCollidableInstance(), childIndices[i]));
			Vector3.Multiply(ref compoundShapeEntry.LocalTransform.Position, compoundShapeEntry.Weight, out var result);
			Vector3.Add(ref value, ref result, out value);
			num += compoundShapeEntry.Weight;
		}
		if (num <= 0f)
		{
			throw new Exception("Compound has zero total weight; invalid configuration.");
		}
		Vector3.Divide(ref value, num, out value);
		compoundCollidable.LocalPosition = -value;
		compoundCollidable.hierarchy.Tree.Reconstruct(compoundCollidable.children);
		compoundCollidable.Shape = shape;
		return compoundCollidable;
	}
}
