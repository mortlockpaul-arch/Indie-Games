using System;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionTests.CollisionAlgorithms;

/// <summary>
///  Helper class that supports other systems using minkowski space operations.
/// </summary>
public static class MinkowskiToolbox
{
	/// <summary>
	///  Gets the local transform of B in the space of A.
	/// </summary>
	/// <param name="transformA">First transform.</param>
	/// <param name="transformB">Second transform.</param>
	/// <param name="localTransformB">Transform of B in the local space of A.</param>
	public static void GetLocalTransform(ref RigidTransform transformA, ref RigidTransform transformB, out RigidTransform localTransformB)
	{
		Quaternion.Conjugate(ref transformA.Orientation, out var result);
		Quaternion.Concatenate(ref transformB.Orientation, ref result, out localTransformB.Orientation);
		Vector3.Subtract(ref transformB.Position, ref transformA.Position, out localTransformB.Position);
		Vector3.Transform(ref localTransformB.Position, ref result, out localTransformB.Position);
	}

	/// <summary>
	///  Gets the extreme point of the minkowski difference of shapeA and shapeB in the local space of shapeA.
	/// </summary>
	/// <param name="shapeA">First shape.</param>
	/// <param name="shapeB">Second shape.</param>
	/// <param name="direction">Extreme point direction in local space.</param>
	/// <param name="localTransformB">Transform of shapeB in the local space of A.</param>
	/// <param name="extremePoint">The extreme point in the local space of A.</param>
	public static void GetLocalMinkowskiExtremePoint(ConvexShape shapeA, ConvexShape shapeB, ref Vector3 direction, ref RigidTransform localTransformB, out Vector3 extremePoint)
	{
		shapeA.GetLocalExtremePointWithoutMargin(ref direction, out extremePoint);
		Vector3.Negate(ref direction, out var result);
		shapeB.GetExtremePointWithoutMargin(result, ref localTransformB, out var extremePoint2);
		Vector3.Subtract(ref extremePoint, ref extremePoint2, out extremePoint);
		ExpandMinkowskiSum(shapeA.collisionMargin, shapeB.collisionMargin, ref direction, out extremePoint2);
		Vector3.Add(ref extremePoint, ref extremePoint2, out extremePoint);
	}

	/// <summary>
	///  Gets the extreme point of the minkowski difference of shapeA and shapeB in the local space of shapeA.
	/// </summary>
	/// <param name="shapeA">First shape.</param>
	/// <param name="shapeB">Second shape.</param>
	/// <param name="direction">Extreme point direction in local space.</param>
	/// <param name="localTransformB">Transform of shapeB in the local space of A.</param>
	///  <param name="extremePointA">The extreme point on shapeA.</param>
	/// <param name="extremePoint">The extreme point in the local space of A.</param>
	public static void GetLocalMinkowskiExtremePoint(ConvexShape shapeA, ConvexShape shapeB, ref Vector3 direction, ref RigidTransform localTransformB, out Vector3 extremePointA, out Vector3 extremePoint)
	{
		shapeA.GetLocalExtremePointWithoutMargin(ref direction, out extremePointA);
		Vector3.Negate(ref direction, out var result);
		shapeB.GetExtremePointWithoutMargin(result, ref localTransformB, out var extremePoint2);
		ExpandMinkowskiSum(shapeA.collisionMargin, shapeB.collisionMargin, direction, ref extremePointA, ref extremePoint2);
		Vector3.Subtract(ref extremePointA, ref extremePoint2, out extremePoint);
	}

	/// <summary>
	///  Gets the extreme point of the minkowski difference of shapeA and shapeB in the local space of shapeA.
	/// </summary>
	/// <param name="shapeA">First shape.</param>
	/// <param name="shapeB">Second shape.</param>
	/// <param name="direction">Extreme point direction in local space.</param>
	/// <param name="localTransformB">Transform of shapeB in the local space of A.</param>
	///  <param name="extremePointA">The extreme point on shapeA.</param>
	///  <param name="extremePointB">The extreme point on shapeB.</param>
	/// <param name="extremePoint">The extreme point in the local space of A.</param>
	public static void GetLocalMinkowskiExtremePoint(ConvexShape shapeA, ConvexShape shapeB, ref Vector3 direction, ref RigidTransform localTransformB, out Vector3 extremePointA, out Vector3 extremePointB, out Vector3 extremePoint)
	{
		shapeA.GetLocalExtremePointWithoutMargin(ref direction, out extremePointA);
		Vector3.Negate(ref direction, out var result);
		shapeB.GetExtremePointWithoutMargin(result, ref localTransformB, out extremePointB);
		ExpandMinkowskiSum(shapeA.collisionMargin, shapeB.collisionMargin, direction, ref extremePointA, ref extremePointB);
		Vector3.Subtract(ref extremePointA, ref extremePointB, out extremePoint);
	}

	/// <summary>
	///  Gets the extreme point of the minkowski difference of shapeA and shapeB in the local space of shapeA, without a margin.
	/// </summary>
	/// <param name="shapeA">First shape.</param>
	/// <param name="shapeB">Second shape.</param>
	/// <param name="direction">Extreme point direction in local space.</param>
	/// <param name="localTransformB">Transform of shapeB in the local space of A.</param>
	/// <param name="extremePoint">The extreme point in the local space of A.</param>
	public static void GetLocalMinkowskiExtremePointWithoutMargin(ConvexShape shapeA, ConvexShape shapeB, ref Vector3 direction, ref RigidTransform localTransformB, out Vector3 extremePoint)
	{
		shapeA.GetLocalExtremePointWithoutMargin(ref direction, out extremePoint);
		Vector3.Negate(ref direction, out var result);
		shapeB.GetExtremePointWithoutMargin(result, ref localTransformB, out var extremePoint2);
		Vector3.Subtract(ref extremePoint, ref extremePoint2, out extremePoint);
	}

	/// <summary>
	///  Computes the expansion of the minkowski sum due to margins in a given direction.
	/// </summary>
	/// <param name="marginA">First margin.</param>
	/// <param name="marginB">Second margin.</param>
	/// <param name="direction">Extreme point direction.</param>
	/// <param name="contribution">Margin contribution to the extreme point.</param>
	public static void ExpandMinkowskiSum(float marginA, float marginB, ref Vector3 direction, out Vector3 contribution)
	{
		float num = direction.LengthSquared();
		if (num > 1E-07f)
		{
			Vector3.Multiply(ref direction, (marginA + marginB) / (float)Math.Sqrt(num), out contribution);
		}
		else
		{
			contribution = default(Vector3);
		}
	}

	/// <summary>
	///  Computes the expansion of the minkowski sum due to margins in a given direction.
	/// </summary>
	/// <param name="marginA">First margin.</param>
	/// <param name="marginB">Second margin.</param>
	/// <param name="direction">Extreme point direction.</param>
	/// <param name="toExpandA">Margin contribution to the shapeA.</param>
	/// <param name="toExpandB">Margin contribution to the shapeB.</param>
	public static void ExpandMinkowskiSum(float marginA, float marginB, Vector3 direction, ref Vector3 toExpandA, ref Vector3 toExpandB)
	{
		float num = direction.LengthSquared();
		if (num > 1E-07f)
		{
			num = 1f / (float)Math.Sqrt(num);
			Vector3.Multiply(ref direction, marginA * num, out var result);
			Vector3.Add(ref toExpandA, ref result, out toExpandA);
			Vector3.Multiply(ref direction, marginB * num, out result);
			Vector3.Subtract(ref toExpandB, ref result, out toExpandB);
		}
	}
}
