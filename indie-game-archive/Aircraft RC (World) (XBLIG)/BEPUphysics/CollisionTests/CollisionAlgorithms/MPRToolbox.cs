using System;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionTests.CollisionAlgorithms;

/// <summary>
/// Contains a variety of queries and computation methods that make use of minkowski portal refinement.
/// </summary>
public static class MPRToolbox
{
	/// <summary>
	/// Number of iterations that the MPR system will run in its inner loop before giving up and returning with failure.
	/// </summary>
	public static int InnerIterationLimit = 15;

	/// <summary>
	/// Number of iterations that the MPR system will run in its outer loop before giving up and moving on to its inner loop.
	/// </summary>
	public static int OuterIterationLimit = 15;

	private static float surfaceEpsilon = 1E-07f;

	private static float depthRefinementEpsilon = 0.0001f;

	private static float rayCastSurfaceEpsilon = 1E-09f;

	private static int maximumDepthRefinementIterations = 3;

	/// <summary>
	/// Gets or sets how close surface-finding based MPR methods have to get before exiting.
	/// Defaults to 1e-7.
	/// </summary>
	public static float SurfaceEpsilon
	{
		get
		{
			return surfaceEpsilon;
		}
		set
		{
			if (value > 0f)
			{
				surfaceEpsilon = value;
				return;
			}
			throw new Exception("Epsilon must be positive.");
		}
	}

	/// <summary>
	/// Gets or sets how close the penetration depth refinement system should converge before quitting.
	/// Making this smaller can help more precisely find a local minimum at the cost of performance.
	/// The change will likely only be visible on curved shapes, since polytopes will converge extremely rapidly to a precise local minimum.
	/// Defaults to 1e-4.
	/// </summary>
	public static float DepthRefinementEpsilon
	{
		get
		{
			return depthRefinementEpsilon;
		}
		set
		{
			if (value > 0f)
			{
				depthRefinementEpsilon = value;
				return;
			}
			throw new Exception("Epsilon must be positive.");
		}
	}

	/// <summary>
	/// Gets or sets how close surface-finding ray casts have to get before exiting.
	/// Defaults to 1e-9.
	/// </summary>
	public static float RayCastSurfaceEpsilon
	{
		get
		{
			return rayCastSurfaceEpsilon;
		}
		set
		{
			if (value > 0f)
			{
				rayCastSurfaceEpsilon = value;
				return;
			}
			throw new Exception("Epsilon must be positive.");
		}
	}

	/// <summary>
	/// Gets or sets the maximum number of iterations to use to reach the local penetration depth minimum when using the RefinePenetration function.
	/// Increasing this allows the system to work longer to find local penetration minima.
	/// The change will likely only be visible on curved shapes, since polytopes will converge extremely rapidly to a precise local minimum.
	/// Defaults to 3.
	/// </summary>
	public static int MaximumDepthRefinementIterations
	{
		get
		{
			return maximumDepthRefinementIterations;
		}
		set
		{
			if (value > 0)
			{
				maximumDepthRefinementIterations = value;
				return;
			}
			throw new Exception("Iteration count must be positive.");
		}
	}

	/// <summary>
	/// Gets a world space point in the overlapped volume between two shapes.
	/// </summary>
	/// <param name="shapeA">First shape in the pair.</param>
	/// <param name="shapeB">Second shape in the pair.</param>
	/// <param name="transformA">Transformation to apply to the first shape.</param>
	/// <param name="transformB">Transformation to apply to the second shape.</param>
	/// <param name="position">Position within the overlapped volume of the two shapes, if any.</param>
	/// <returns>Whether or not the two shapes overlap.</returns>
	public static bool GetOverlapPosition(ConvexShape shapeA, ConvexShape shapeB, ref RigidTransform transformA, ref RigidTransform transformB, out Vector3 position)
	{
		MinkowskiToolbox.GetLocalTransform(ref transformA, ref transformB, out var localTransformB);
		bool localOverlapPosition = GetLocalOverlapPosition(shapeA, shapeB, ref localTransformB, out position);
		RigidTransform.Transform(ref position, ref transformA, out position);
		return localOverlapPosition;
	}

	/// <summary>
	/// Gets a point in the overlapped volume between two shapes in shape A's local space.
	/// </summary>
	/// <param name="shapeA">First shape in the pair.</param>
	/// <param name="shapeB">Second shape in the pair.</param>
	/// <param name="position">Position within the overlapped volume of the two shapes in shape A's local space, if any.</param>
	/// <returns>Whether or not the two shapes overlap.</returns>
	public static bool GetLocalOverlapPosition(ConvexShape shapeA, ConvexShape shapeB, ref RigidTransform localTransformB, out Vector3 position)
	{
		return GetLocalOverlapPosition(shapeA, shapeB, ref localTransformB.Position, ref localTransformB, out position);
	}

	internal static bool GetLocalOverlapPosition(ConvexShape shapeA, ConvexShape shapeB, ref Vector3 originRay, ref RigidTransform localTransformB, out Vector3 position)
	{
		if (originRay.LengthSquared() < 1E-07f)
		{
			position = default(Vector3);
			return true;
		}
		Vector3.Negate(ref originRay, out var result);
		Vector3 direction = originRay;
		MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction, ref localTransformB, out var extremePointA, out var extremePointB, out var extremePoint);
		Vector3.Cross(ref extremePoint, ref result, out direction);
		if (direction.LengthSquared() < 1E-07f)
		{
			Vector3.Dot(ref extremePoint, ref originRay, out var result2);
			if (result2 < 0f)
			{
				position = default(Vector3);
				return false;
			}
			Vector3.Dot(ref result, ref originRay, out var result3);
			float scaleFactor = (0f - result3) / (result2 - result3);
			Vector3.Multiply(ref extremePointA, scaleFactor, out position);
			return true;
		}
		MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction, ref localTransformB, out var extremePointA2, out var extremePointB2, out var extremePoint2);
		Vector3.Subtract(ref extremePoint, ref result, out var result4);
		Vector3.Subtract(ref extremePoint2, ref result, out var result5);
		Vector3.Cross(ref result4, ref result5, out direction);
		int num = 0;
		Vector3 extremePointA3;
		Vector3 extremePoint3;
		while (true)
		{
			MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction, ref localTransformB, out extremePointA3, out var extremePointB3, out extremePoint3);
			if (num > OuterIterationLimit)
			{
				break;
			}
			num++;
			Vector3.Cross(ref extremePoint, ref extremePoint3, out result4);
			Vector3.Dot(ref result4, ref result, out var result6);
			if (result6 < 0f)
			{
				extremePoint2 = extremePoint3;
				extremePointA2 = extremePointA3;
				extremePointB2 = extremePointB3;
				Vector3.Subtract(ref extremePoint, ref result, out result4);
				Vector3.Subtract(ref extremePoint3, ref result, out result5);
				Vector3.Cross(ref result4, ref result5, out direction);
				continue;
			}
			Vector3.Cross(ref extremePoint3, ref extremePoint2, out result4);
			Vector3.Dot(ref result4, ref result, out result6);
			if (!(result6 < 0f))
			{
				break;
			}
			extremePoint = extremePoint3;
			extremePointA = extremePointA3;
			extremePointB = extremePointB3;
			Vector3.Subtract(ref extremePoint2, ref result, out result4);
			Vector3.Subtract(ref extremePoint3, ref result, out result5);
			Vector3.Cross(ref result4, ref result5, out direction);
		}
		while (true)
		{
			Vector3.Subtract(ref extremePoint3, ref extremePoint2, out result4);
			Vector3.Subtract(ref extremePoint, ref extremePoint2, out result5);
			Vector3.Cross(ref result4, ref result5, out direction);
			Vector3.Dot(ref direction, ref extremePoint, out var result7);
			if (result7 >= 0f)
			{
				Vector3.Subtract(ref extremePoint, ref result, out result4);
				Vector3.Subtract(ref extremePoint2, ref result, out result5);
				Vector3.Subtract(ref extremePoint3, ref result, out var result8);
				Vector3.Cross(ref result4, ref result5, out var result9);
				Vector3.Dot(ref result9, ref result8, out var result10);
				Vector3.Cross(ref extremePoint, ref extremePoint2, out result9);
				Vector3.Dot(ref result9, ref extremePoint3, out var result11);
				Vector3.Cross(ref originRay, ref result5, out result9);
				Vector3.Dot(ref result9, ref result8, out var result12);
				Vector3.Cross(ref result4, ref originRay, out result9);
				Vector3.Dot(ref result9, ref result8, out var result13);
				if (result10 > 1E-09f)
				{
					float num2 = 1f / result10;
					float num3 = result11 * num2;
					float num4 = result12 * num2;
					float num5 = result13 * num2;
					float num6 = 1f - num3 - num4 - num5;
					position = num4 * extremePointA + num5 * extremePointA2 + num6 * extremePointA3;
				}
				else
				{
					position = default(Vector3);
				}
				return true;
			}
			MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction, ref localTransformB, out var extremePointA4, out var extremePointB4, out var extremePoint4);
			Vector3.Dot(ref extremePoint4, ref direction, out var result14);
			if (result14 < 0f)
			{
				position = default(Vector3);
				return false;
			}
			if (result14 - result7 < surfaceEpsilon || num > InnerIterationLimit)
			{
				break;
			}
			num++;
			Vector3.Cross(ref extremePoint4, ref result, out result4);
			Vector3.Dot(ref extremePoint, ref result4, out result7);
			if (result7 >= 0f)
			{
				Vector3.Dot(ref extremePoint2, ref result4, out result7);
				if (result7 >= 0f)
				{
					extremePoint = extremePoint4;
					extremePointA = extremePointA4;
					extremePointB = extremePointB4;
				}
				else
				{
					extremePoint3 = extremePoint4;
					extremePointA3 = extremePointA4;
					Vector3 extremePointB3 = extremePointB4;
				}
			}
			else
			{
				Vector3.Dot(ref extremePoint3, ref result4, out result7);
				if (result7 >= 0f)
				{
					extremePoint2 = extremePoint4;
					extremePointA2 = extremePointA4;
					extremePointB2 = extremePointB4;
				}
				else
				{
					extremePoint = extremePoint4;
					extremePointA = extremePointA4;
					extremePointB = extremePointB4;
				}
			}
		}
		position = default(Vector3);
		return false;
	}

	/// <summary>
	/// Determines if two shapes are colliding.
	/// </summary>
	/// <param name="shapeA">First shape in the pair.</param>
	/// <param name="shapeB">Second shape of the pair.</param>
	/// <param name="transformA">Transformation to apply to shape A.</param>
	/// <param name="transformB">Transformation to apply to shape B.</param>
	/// <returns>Whether or not the shapes are overlapping.</returns>
	public static bool AreShapesOverlapping(ConvexShape shapeA, ConvexShape shapeB, ref RigidTransform transformA, ref RigidTransform transformB)
	{
		MinkowskiToolbox.GetLocalTransform(ref transformA, ref transformB, out var localTransformB);
		return AreLocalShapesOverlapping(shapeA, shapeB, ref localTransformB);
	}

	/// <summary>
	/// Determines if two shapes are colliding.  Shape B is positioned relative to shape A.
	/// </summary>
	/// <param name="shapeA">First shape in the pair.</param>
	/// <param name="shapeB">Second shape of the pair.</param>
	/// <param name="localTransformB">Relative transform of shape B to shape A.</param>
	/// <returns>Whether or not the shapes are overlapping.</returns>
	public static bool AreLocalShapesOverlapping(ConvexShape shapeA, ConvexShape shapeB, ref RigidTransform localTransformB)
	{
		return AreLocalShapesOverlapping(shapeA, shapeB, ref localTransformB.Position, ref localTransformB);
	}

	/// <summary>
	/// Determines if two shapes are colliding.  Shape B is positioned relative to shape A.
	/// </summary>
	/// <param name="shapeA">First shape in the pair.</param>
	/// <param name="shapeB">Second shape of the pair.</param>
	/// <param name="originRay">Direction in which to cast the overlap ray.  Necessary when an object's origin is not contained in its geometry.</param>
	/// <param name="localTransformB">Relative transform of shape B to shape A.</param>
	/// <returns>Whether or not the shapes are overlapping.</returns>
	internal static bool AreLocalShapesOverlapping(ConvexShape shapeA, ConvexShape shapeB, ref Vector3 originRay, ref RigidTransform localTransformB)
	{
		if (originRay.LengthSquared() < 1E-07f)
		{
			return true;
		}
		Vector3.Negate(ref originRay, out var result);
		Vector3 direction = originRay;
		MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction, ref localTransformB, out var extremePoint);
		Vector3.Cross(ref extremePoint, ref result, out direction);
		if (direction.LengthSquared() < 1E-07f)
		{
			Vector3.Dot(ref extremePoint, ref originRay, out var result2);
			if (result2 < 0f)
			{
				return false;
			}
			return true;
		}
		MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction, ref localTransformB, out var extremePoint2);
		Vector3.Subtract(ref extremePoint, ref result, out var result3);
		Vector3.Subtract(ref extremePoint2, ref result, out var result4);
		Vector3.Cross(ref result3, ref result4, out direction);
		int num = 0;
		Vector3 extremePoint3;
		while (true)
		{
			MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction, ref localTransformB, out extremePoint3);
			if (num > OuterIterationLimit)
			{
				break;
			}
			num++;
			Vector3.Cross(ref extremePoint, ref extremePoint3, out result3);
			Vector3.Dot(ref result3, ref result, out var result5);
			if (result5 < 0f)
			{
				extremePoint2 = extremePoint3;
				Vector3.Subtract(ref extremePoint, ref result, out result3);
				Vector3.Subtract(ref extremePoint3, ref result, out result4);
				Vector3.Cross(ref result3, ref result4, out direction);
				continue;
			}
			Vector3.Cross(ref extremePoint3, ref extremePoint2, out result3);
			Vector3.Dot(ref result3, ref result, out result5);
			if (!(result5 < 0f))
			{
				break;
			}
			extremePoint = extremePoint3;
			Vector3.Subtract(ref extremePoint2, ref result, out result3);
			Vector3.Subtract(ref extremePoint3, ref result, out result4);
			Vector3.Cross(ref result3, ref result4, out direction);
		}
		while (true)
		{
			Vector3.Subtract(ref extremePoint3, ref extremePoint2, out result3);
			Vector3.Subtract(ref extremePoint, ref extremePoint2, out result4);
			Vector3.Cross(ref result3, ref result4, out direction);
			Vector3.Dot(ref direction, ref extremePoint, out var result6);
			if (result6 >= 0f)
			{
				return true;
			}
			MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction, ref localTransformB, out var extremePoint4);
			Vector3.Dot(ref extremePoint4, ref direction, out var result7);
			if (result7 < 0f)
			{
				return false;
			}
			if (result7 - result6 < surfaceEpsilon || num > InnerIterationLimit)
			{
				break;
			}
			num++;
			Vector3.Cross(ref extremePoint4, ref result, out result3);
			Vector3.Dot(ref extremePoint, ref result3, out result6);
			if (result6 >= 0f)
			{
				Vector3.Dot(ref extremePoint2, ref result3, out result6);
				if (result6 >= 0f)
				{
					extremePoint = extremePoint4;
				}
				else
				{
					extremePoint3 = extremePoint4;
				}
			}
			else
			{
				Vector3.Dot(ref extremePoint3, ref result3, out result6);
				if (result6 >= 0f)
				{
					extremePoint2 = extremePoint4;
				}
				else
				{
					extremePoint = extremePoint4;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// Casts a ray from the origin in the given direction at the surface of the minkowski difference.
	/// Assumes that the origin is within the minkowski difference.
	/// </summary>
	/// <param name="shapeA">First shape in the pair.</param>
	/// <param name="shapeB">Second shape in the pair.</param>
	/// <param name="localTransformB">Transformation of shape B relative to shape A.</param>
	/// <param name="direction">Direction to cast the ray.</param>
	/// <param name="t">Length along the direction vector that the impact was found.</param>
	/// <param name="normal">Normal of the impact at the surface of the convex.</param>
	public static void LocalSurfaceCast(ConvexShape shapeA, ConvexShape shapeB, ref RigidTransform localTransformB, ref Vector3 direction, out float t, out Vector3 normal)
	{
		Vector3 direction2 = direction;
		MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction2, ref localTransformB, out var extremePoint);
		Vector3.Cross(ref direction, ref extremePoint, out direction2);
		if (direction2.LengthSquared() < 1E-07f)
		{
			float num = direction.LengthSquared();
			if (num > 1E-09f)
			{
				Vector3.Divide(ref direction, (float)Math.Sqrt(num), out normal);
			}
			else
			{
				normal = default(Vector3);
			}
			Vector3.Dot(ref normal, ref direction, out var result);
			Vector3.Dot(ref normal, ref extremePoint, out var result2);
			if (result > 0f)
			{
				t = result2 / result;
			}
			else
			{
				t = 0f;
			}
			return;
		}
		MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction2, ref localTransformB, out var extremePoint2);
		Vector3.Cross(ref extremePoint, ref extremePoint2, out direction2);
		Vector3.Dot(ref direction2, ref direction, out var result3);
		Vector3 result4;
		if (result3 > 0f)
		{
			Vector3.Negate(ref direction2, out direction2);
			result4 = extremePoint;
			extremePoint = extremePoint2;
			extremePoint2 = result4;
		}
		int num2 = 0;
		Vector3 extremePoint3;
		while (true)
		{
			MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction2, ref localTransformB, out extremePoint3);
			if (num2 > OuterIterationLimit)
			{
				t = float.MaxValue;
				normal = Toolbox.UpVector;
				return;
			}
			num2++;
			Vector3.Cross(ref extremePoint, ref extremePoint3, out result4);
			Vector3.Dot(ref result4, ref direction, out result3);
			if (result3 < 0f)
			{
				extremePoint2 = extremePoint3;
				Vector3.Cross(ref extremePoint, ref extremePoint3, out direction2);
				continue;
			}
			Vector3.Cross(ref extremePoint3, ref extremePoint2, out result4);
			Vector3.Dot(ref result4, ref direction, out result3);
			if (!(result3 < 0f))
			{
				break;
			}
			extremePoint = extremePoint3;
			Vector3.Cross(ref extremePoint2, ref extremePoint3, out direction2);
		}
		num2 = 0;
		float result6;
		while (true)
		{
			Vector3.Subtract(ref extremePoint, ref extremePoint2, out result4);
			Vector3.Subtract(ref extremePoint3, ref extremePoint2, out var result5);
			Vector3.Cross(ref result4, ref result5, out direction2);
			MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction2, ref localTransformB, out var extremePoint4);
			Vector3.Dot(ref direction2, ref extremePoint, out result3);
			Vector3.Dot(ref extremePoint4, ref direction2, out result6);
			if (result6 - result3 < surfaceEpsilon || num2 > InnerIterationLimit)
			{
				break;
			}
			Vector3.Cross(ref extremePoint4, ref direction, out result4);
			Vector3.Dot(ref extremePoint, ref result4, out result3);
			if (result3 >= 0f)
			{
				Vector3.Dot(ref extremePoint2, ref result4, out result3);
				if (result3 >= 0f)
				{
					extremePoint = extremePoint4;
				}
				else
				{
					extremePoint3 = extremePoint4;
				}
			}
			else
			{
				Vector3.Dot(ref extremePoint3, ref result4, out result3);
				if (result3 >= 0f)
				{
					extremePoint2 = extremePoint4;
				}
				else
				{
					extremePoint = extremePoint4;
				}
			}
			num2++;
		}
		float num3 = direction2.LengthSquared();
		if (num3 > 1E-09f)
		{
			Vector3.Divide(ref direction2, (float)Math.Sqrt(num3), out normal);
			Vector3.Dot(ref normal, ref direction, out result3);
			Vector3.Dot(ref normal, ref extremePoint, out result6);
			if (result3 > 0f)
			{
				t = result6 / result3;
			}
			else
			{
				t = 0f;
			}
		}
		else
		{
			normal = Vector3.Up;
			t = 0f;
		}
	}

	/// <summary>
	/// Casts a ray from the origin in the given direction at the surface of the minkowski difference.
	/// Assumes that the origin is within the minkowski difference.
	/// </summary>
	/// <param name="shapeA">First shape in the pair.</param>
	/// <param name="shapeB">Second shape in the pair.</param>
	/// <param name="localTransformB">Transformation of shape B relative to shape A.</param>
	/// <param name="direction">Direction to cast the ray.</param>
	/// <param name="t">Length along the direction vector that the impact was found.</param>
	/// <param name="normal">Normal of the impact at the surface of the convex.</param>
	/// <param name="position">Location of the ray cast hit on the surface of A.</param>
	public static void LocalSurfaceCast(ConvexShape shapeA, ConvexShape shapeB, ref RigidTransform localTransformB, ref Vector3 direction, out float t, out Vector3 normal, out Vector3 position)
	{
		Vector3 direction2 = direction;
		MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction2, ref localTransformB, out var extremePointA, out var extremePoint);
		Vector3.Cross(ref direction, ref extremePoint, out direction2);
		if (direction2.LengthSquared() < 1E-07f)
		{
			float num = direction.LengthSquared();
			if (num > 1E-09f)
			{
				Vector3.Divide(ref direction, (float)Math.Sqrt(num), out normal);
			}
			else
			{
				normal = default(Vector3);
			}
			Vector3.Dot(ref normal, ref direction, out var result);
			Vector3.Dot(ref normal, ref extremePoint, out var result2);
			if (result > 0f)
			{
				t = result2 / result;
			}
			else
			{
				t = 0f;
			}
			position = extremePointA;
			return;
		}
		MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction2, ref localTransformB, out var extremePointA2, out var extremePoint2);
		Vector3.Cross(ref extremePoint, ref extremePoint2, out direction2);
		Vector3.Dot(ref direction2, ref direction, out var result3);
		Vector3 vector;
		if (result3 > 0f)
		{
			Vector3.Negate(ref direction2, out direction2);
			vector = extremePoint;
			extremePoint = extremePoint2;
			extremePoint2 = vector;
			vector = extremePointA;
			extremePointA = extremePointA2;
			extremePointA2 = vector;
		}
		int num2 = 0;
		Vector3 extremePointA3;
		Vector3 extremePoint3;
		while (true)
		{
			MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction2, ref localTransformB, out extremePointA3, out extremePoint3);
			if (num2 > OuterIterationLimit)
			{
				t = float.MaxValue;
				normal = Toolbox.UpVector;
				position = default(Vector3);
				return;
			}
			num2++;
			Vector3.Cross(ref extremePoint, ref extremePoint3, out vector);
			Vector3.Dot(ref vector, ref direction, out result3);
			if (result3 < 0f)
			{
				extremePoint2 = extremePoint3;
				extremePointA2 = extremePointA3;
				Vector3.Cross(ref extremePoint, ref extremePoint3, out direction2);
				continue;
			}
			Vector3.Cross(ref extremePoint3, ref extremePoint2, out vector);
			Vector3.Dot(ref vector, ref direction, out result3);
			if (!(result3 < 0f))
			{
				break;
			}
			extremePoint = extremePoint3;
			extremePointA = extremePointA3;
			Vector3.Cross(ref extremePoint2, ref extremePoint3, out direction2);
		}
		num2 = 0;
		float result5;
		while (true)
		{
			Vector3.Subtract(ref extremePoint, ref extremePoint2, out vector);
			Vector3.Subtract(ref extremePoint3, ref extremePoint2, out var result4);
			Vector3.Cross(ref vector, ref result4, out direction2);
			MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction2, ref localTransformB, out var extremePointA4, out var extremePoint4);
			Vector3.Dot(ref direction2, ref extremePoint, out result3);
			Vector3.Dot(ref extremePoint4, ref direction2, out result5);
			if (result5 - result3 < surfaceEpsilon || num2 > InnerIterationLimit)
			{
				break;
			}
			Vector3.Cross(ref extremePoint4, ref direction, out vector);
			Vector3.Dot(ref extremePoint, ref vector, out result3);
			if (result3 >= 0f)
			{
				Vector3.Dot(ref extremePoint2, ref vector, out result3);
				if (result3 >= 0f)
				{
					extremePoint = extremePoint4;
					extremePointA = extremePointA4;
				}
				else
				{
					extremePoint3 = extremePoint4;
					extremePointA3 = extremePointA4;
				}
			}
			else
			{
				Vector3.Dot(ref extremePoint3, ref vector, out result3);
				if (result3 >= 0f)
				{
					extremePoint2 = extremePoint4;
					extremePointA2 = extremePointA4;
				}
				else
				{
					extremePoint = extremePoint4;
					extremePointA = extremePointA4;
				}
			}
			num2++;
		}
		float num3 = direction2.LengthSquared();
		if (num3 > 1E-09f)
		{
			Vector3.Divide(ref direction2, (float)Math.Sqrt(num3), out normal);
			Vector3.Dot(ref normal, ref direction, out result3);
			Vector3.Dot(ref normal, ref extremePoint, out result5);
			if (result3 > 0f)
			{
				t = result5 / result3;
			}
			else
			{
				t = 0f;
			}
		}
		else
		{
			normal = Vector3.Up;
			t = 0f;
		}
		Vector3.Multiply(ref direction, t, out position);
		Toolbox.GetBarycentricCoordinates(ref position, ref extremePoint, ref extremePoint2, ref extremePoint3, out var aWeight, out var bWeight, out var cWeight);
		Vector3.Multiply(ref extremePointA, aWeight, out position);
		Vector3.Multiply(ref extremePointA2, bWeight, out var result6);
		Vector3.Add(ref result6, ref position, out position);
		Vector3.Multiply(ref extremePointA3, cWeight, out result6);
		Vector3.Add(ref result6, ref position, out position);
	}

	private static bool VerifySimplex(ref Vector3 v0, ref Vector3 v1, ref Vector3 v2, ref Vector3 v3, ref Vector3 direction)
	{
		Vector3 vector = Vector3.Cross(v0 - v1, v3 - v1);
		float num = Vector3.Dot(vector, direction);
		vector = Vector3.Cross(v0 - v3, v2 - v3);
		float num2 = Vector3.Dot(vector, direction);
		vector = Vector3.Cross(v0 - v2, v1 - v2);
		float num3 = Vector3.Dot(vector, direction);
		if (!(num <= 0f) || !(num2 <= 0f) || !(num3 <= 0f))
		{
			if (num >= 0f && num2 >= 0f)
			{
				return num3 >= 0f;
			}
			return false;
		}
		return true;
	}

	/// <summary>
	/// Gets a contact point between two convex shapes.
	/// </summary>
	/// <param name="shapeA">First shape in the pair.</param>
	/// <param name="shapeB">Second shape in the pair.</param>
	/// <param name="transformA">Transformation to apply to the first shape.</param>
	/// <param name="transformB">Transformation to apply to the second shape.</param>
	/// <param name="penetrationAxis">Axis along which to first test the penetration depth.</param>
	/// <param name="contact">Contact data between the two shapes, if any.</param>
	/// <returns>Whether or not the shapes overlap.</returns>
	public static bool GetContact(ConvexShape shapeA, ConvexShape shapeB, ref RigidTransform transformA, ref RigidTransform transformB, ref Vector3 penetrationAxis, out ContactData contact)
	{
		MinkowskiToolbox.GetLocalTransform(ref transformA, ref transformB, out var localTransformB);
		if (AreLocalShapesOverlapping(shapeA, shapeB, ref localTransformB))
		{
			float num = penetrationAxis.LengthSquared();
			Vector3 result;
			if (num > 1E-07f)
			{
				Vector3.Divide(ref penetrationAxis, (float)Math.Sqrt(num), out result);
				LocalSurfaceCast(shapeA, shapeB, ref localTransformB, ref result, out contact.PenetrationDepth, out contact.Normal);
			}
			else
			{
				contact.PenetrationDepth = float.MaxValue;
				contact.Normal = Toolbox.UpVector;
			}
			num = localTransformB.Position.LengthSquared();
			if (num > 1E-07f)
			{
				Vector3.Divide(ref localTransformB.Position, (float)Math.Sqrt(num), out result);
				LocalSurfaceCast(shapeA, shapeB, ref localTransformB, ref result, out var t, out var normal);
				if (t < contact.PenetrationDepth)
				{
					contact.Normal = normal;
					contact.PenetrationDepth = t;
				}
			}
			RefinePenetration(shapeA, shapeB, ref localTransformB, contact.PenetrationDepth, ref contact.Normal, out contact.PenetrationDepth, out contact.Normal, out contact.Position);
			contact.Id = -1;
			Matrix3X3.CreateFromQuaternion(ref transformA.Orientation, out var result2);
			Matrix3X3.Transform(ref contact.Normal, ref result2, out contact.Normal);
			Matrix3X3.Transform(ref contact.Position, ref result2, out contact.Position);
			Vector3.Add(ref contact.Position, ref transformA.Position, out contact.Position);
			return true;
		}
		contact = default(ContactData);
		return false;
	}

	/// <summary>
	/// Incrementally refines the penetration depth and normal towards the local minimum.
	/// </summary>
	/// <param name="shapeA">First shape in the pair.</param>
	/// <param name="shapeB">Second shape in the pair.</param>
	/// <param name="localTransformB">Transformation of shape B relative to shape A.</param>
	/// <param name="initialDepth">Initial depth estimate.</param>
	/// <param name="initialNormal">Initial normal estimate.</param>
	/// <param name="penetrationDepth">Refined penetration depth.</param>
	/// <param name="refinedNormal">Refined normal.</param>
	/// <param name="position">Refined position.</param>
	public static void RefinePenetration(ConvexShape shapeA, ConvexShape shapeB, ref RigidTransform localTransformB, float initialDepth, ref Vector3 initialNormal, out float penetrationDepth, out Vector3 refinedNormal, out Vector3 position)
	{
		int num = 0;
		refinedNormal = initialNormal;
		penetrationDepth = initialDepth;
		float t;
		while (true)
		{
			LocalSurfaceCast(shapeA, shapeB, ref localTransformB, ref refinedNormal, out t, out var normal, out position);
			if (penetrationDepth - t <= depthRefinementEpsilon || ++num >= maximumDepthRefinementIterations)
			{
				break;
			}
			penetrationDepth = t;
			refinedNormal = normal;
		}
		penetrationDepth = t;
	}

	/// <summary>
	/// Sweeps the shapes against each other and finds a point, time, and normal of impact.
	/// </summary>
	/// <param name="shapeA">First shape in the pair.</param>
	/// <param name="shapeB">Second shape in the pair.</param>
	/// <param name="sweepA">Sweep direction and amount to apply to the first shape.</param>
	/// <param name="sweepB">Sweep direction and amount to apply to the second shape.</param>
	/// <param name="transformA">Initial transform to apply to the first shape.</param>
	/// <param name="transformB">Initial transform to apply to the second shape.</param>
	/// <param name="hit">Hit data between the two shapes, if any.</param>
	/// <returns>Whether or not the swept shapes hit each other.</returns>
	public static bool Sweep(ConvexShape shapeA, ConvexShape shapeB, ref Vector3 sweepA, ref Vector3 sweepB, ref RigidTransform transformA, ref RigidTransform transformB, out RayHit hit)
	{
		Vector3.Subtract(ref sweepA, ref sweepB, out var result);
		Quaternion.Conjugate(ref transformA.Orientation, out var result2);
		Vector3.Transform(ref result, ref result2, out var result3);
		MinkowskiToolbox.GetLocalTransform(ref transformA, ref transformB, out var localTransformB);
		float num = result3.LengthSquared();
		float result4;
		if (num > 1E-09f)
		{
			Vector3.Dot(ref localTransformB.Position, ref result3, out result4);
			result4 /= num;
			result4 += (shapeA.maximumRadius + shapeB.maximumRadius) / (float)Math.Sqrt(num);
		}
		else
		{
			num = 0f;
			result4 = 0f;
		}
		bool flag;
		if (flag = result4 < 0f)
		{
			result4 = 0f;
		}
		Vector3.Multiply(ref result3, result4, out var result5);
		if (!AreSweptShapesIntersecting(shapeA, shapeB, ref result5, ref localTransformB, out hit.Location))
		{
			hit.T = float.MaxValue;
			hit.Normal = default(Vector3);
			hit.Location = default(Vector3);
			return false;
		}
		if (flag)
		{
			hit.T = 0f;
			Vector3.Normalize(ref result3, out hit.Normal);
			Vector3.Transform(ref hit.Normal, ref transformA.Orientation, out hit.Normal);
			Vector3.Transform(ref hit.Location, ref transformA.Orientation, out hit.Location);
			Vector3.Add(ref hit.Location, ref transformA.Position, out hit.Location);
			hit.Location += sweepA * hit.T;
			return true;
		}
		if (LocalSweepCast(shapeA, shapeB, result4, num, ref result3, ref result5, ref localTransformB, out hit))
		{
			Vector3 minkowskiPosition = (0f - hit.T) * result3;
			GetLocalPosition(shapeA, shapeB, ref localTransformB, ref minkowskiPosition, out hit.Location);
			RigidTransform.Transform(ref hit.Location, ref transformA, out hit.Location);
			Vector3.Transform(ref hit.Normal, ref transformA.Orientation, out hit.Normal);
			Vector3.Multiply(ref sweepA, hit.T, out var result6);
			Vector3.Add(ref result6, ref hit.Location, out hit.Location);
			return true;
		}
		return false;
	}

	private static bool LocalSweepCast(ConvexShape shapeA, ConvexShape shapeB, float sweepLength, float rayLengthSquared, ref Vector3 localDirection, ref Vector3 sweep, ref RigidTransform localTransformB, out RayHit hit)
	{
		Vector3 extremePointDirection = localDirection;
		GetSweptExtremePoint(shapeA, shapeB, ref localTransformB, ref sweep, ref extremePointDirection, out var extremePointA, out var extremePoint);
		Vector3.Cross(ref localDirection, ref extremePoint, out extremePointDirection);
		hit.Location = default(Vector3);
		if (extremePointDirection.LengthSquared() < 1E-09f)
		{
			if (rayLengthSquared > 1E-09f)
			{
				Vector3.Divide(ref localDirection, (float)Math.Sqrt(rayLengthSquared), out hit.Normal);
			}
			else
			{
				hit.Normal = default(Vector3);
			}
			Vector3.Dot(ref hit.Normal, ref localDirection, out var result);
			Vector3.Dot(ref hit.Normal, ref extremePoint, out var result2);
			if (result > 0f)
			{
				hit.T = sweepLength - result2 / result;
			}
			else
			{
				hit.T = sweepLength;
			}
			if (hit.T < 0f)
			{
				hit.T = 0f;
			}
			return hit.T <= 1f;
		}
		GetSweptExtremePoint(shapeA, shapeB, ref localTransformB, ref sweep, ref extremePointDirection, out var extremePointA2, out var extremePoint2);
		Vector3.Cross(ref extremePoint, ref extremePoint2, out extremePointDirection);
		Vector3.Dot(ref extremePointDirection, ref localDirection, out var result3);
		Vector3 vector;
		if (result3 > 0f)
		{
			Vector3.Negate(ref extremePointDirection, out extremePointDirection);
			vector = extremePoint;
			extremePoint = extremePoint2;
			extremePoint2 = vector;
			vector = extremePointA;
			extremePointA = extremePointA2;
			extremePointA2 = vector;
		}
		int num = 0;
		Vector3 extremePoint3;
		while (true)
		{
			GetSweptExtremePoint(shapeA, shapeB, ref localTransformB, ref sweep, ref extremePointDirection, out var extremePointA3, out extremePoint3);
			if (num > OuterIterationLimit)
			{
				hit.T = float.MaxValue;
				hit.Normal = default(Vector3);
				hit.Location = default(Vector3);
				return false;
			}
			num++;
			Vector3.Cross(ref extremePoint, ref extremePoint3, out vector);
			Vector3.Dot(ref vector, ref localDirection, out result3);
			if (result3 < 0f)
			{
				extremePoint2 = extremePoint3;
				extremePointA2 = extremePointA3;
				Vector3.Cross(ref extremePoint, ref extremePoint3, out extremePointDirection);
				continue;
			}
			Vector3.Cross(ref extremePoint3, ref extremePoint2, out vector);
			Vector3.Dot(ref vector, ref localDirection, out result3);
			if (!(result3 < 0f))
			{
				break;
			}
			extremePoint = extremePoint3;
			extremePointA = extremePointA3;
			Vector3.Cross(ref extremePoint2, ref extremePoint3, out extremePointDirection);
		}
		num = 0;
		float result5;
		while (true)
		{
			Vector3.Subtract(ref extremePoint, ref extremePoint2, out vector);
			Vector3.Subtract(ref extremePoint3, ref extremePoint2, out var result4);
			Vector3.Cross(ref vector, ref result4, out extremePointDirection);
			GetSweptExtremePoint(shapeA, shapeB, ref localTransformB, ref sweep, ref extremePointDirection, out var extremePointA4, out var extremePoint4);
			Vector3.Dot(ref extremePointDirection, ref extremePoint, out result3);
			Vector3.Dot(ref extremePoint4, ref extremePointDirection, out result5);
			if (result5 - result3 < rayCastSurfaceEpsilon || num > InnerIterationLimit)
			{
				break;
			}
			Vector3.Cross(ref extremePoint4, ref localDirection, out vector);
			Vector3.Dot(ref extremePoint, ref vector, out result3);
			if (result3 >= 0f)
			{
				Vector3.Dot(ref extremePoint2, ref vector, out result3);
				if (result3 >= 0f)
				{
					extremePoint = extremePoint4;
					extremePointA = extremePointA4;
				}
				else
				{
					extremePoint3 = extremePoint4;
					Vector3 extremePointA3 = extremePointA4;
				}
			}
			else
			{
				Vector3.Dot(ref extremePoint3, ref vector, out result3);
				if (result3 >= 0f)
				{
					extremePoint2 = extremePoint4;
					extremePointA2 = extremePointA4;
				}
				else
				{
					extremePoint = extremePoint4;
					extremePointA = extremePointA4;
				}
			}
			num++;
		}
		float num2 = extremePointDirection.LengthSquared();
		if (num2 > 1E-12f)
		{
			Vector3.Divide(ref extremePointDirection, (float)Math.Sqrt(num2), out hit.Normal);
			Vector3.Dot(ref hit.Normal, ref localDirection, out result3);
			Vector3.Dot(ref hit.Normal, ref extremePoint, out result5);
			hit.T = sweepLength - result5 / result3;
		}
		else
		{
			Vector3.Normalize(ref localDirection, out hit.Normal);
			hit.T = sweepLength;
		}
		if (hit.T < 0f)
		{
			hit.T = 0f;
		}
		return hit.T <= 1f;
	}

	/// <summary>
	/// Computes the position of the minkowski point in the local space of A.
	/// This assumes that the minkowski point is contained in A-B.
	/// </summary>
	/// <param name="shapeA">First shape to test.</param>
	/// <param name="shapeB">Second shape to test.</param>
	/// <param name="localTransformB">Transform of shape B in the local space of A.</param>
	/// <param name="minkowskiPosition">Position in minkowski space to pull into the local space of A.</param>
	/// <param name="position">Position of the minkowski space point in the local space of A.</param>
	internal static void GetLocalPosition(ConvexShape shapeA, ConvexShape shapeB, ref RigidTransform localTransformB, ref Vector3 minkowskiPosition, out Vector3 position)
	{
		Vector3.Add(ref minkowskiPosition, ref localTransformB.Position, out var result);
		if (result.LengthSquared() < 1E-07f)
		{
			position = default(Vector3);
			return;
		}
		Vector3.Negate(ref localTransformB.Position, out var result2);
		Vector3 direction = result;
		MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction, ref localTransformB, out var extremePointA, out var extremePointB, out var extremePoint);
		Vector3.Cross(ref extremePoint, ref result2, out direction);
		if (direction.LengthSquared() < 1E-07f)
		{
			float num = Vector3.Dot(extremePoint - minkowskiPosition, result);
			float num2 = Vector3.Dot(result2 - minkowskiPosition, result);
			float scaleFactor = (0f - num2) / (num - num2);
			Vector3.Multiply(ref extremePointA, scaleFactor, out position);
			Vector3.Subtract(ref extremePointB, ref localTransformB.Position, out var _);
			return;
		}
		MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction, ref localTransformB, out var extremePointA2, out var extremePointB2, out var extremePoint2);
		Vector3.Subtract(ref extremePoint, ref result2, out var result4);
		Vector3.Subtract(ref extremePoint2, ref result2, out var result5);
		Vector3.Cross(ref result4, ref result5, out direction);
		Vector3.Subtract(ref result2, ref minkowskiPosition, out var result6);
		int num3 = 0;
		Vector3 extremePointA3;
		Vector3 extremePoint3;
		Vector3 result8;
		while (true)
		{
			MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction, ref localTransformB, out extremePointA3, out var extremePointB3, out extremePoint3);
			if (num3 > OuterIterationLimit)
			{
				break;
			}
			num3++;
			Vector3.Subtract(ref extremePoint, ref result2, out result4);
			Vector3.Subtract(ref extremePoint3, ref result2, out var result7);
			Vector3.Cross(ref result4, ref result7, out result8);
			Vector3.Dot(ref result8, ref result6, out var result9);
			if (result9 < 0f)
			{
				extremePoint2 = extremePoint3;
				extremePointA2 = extremePointA3;
				extremePointB2 = extremePointB3;
				Vector3.Cross(ref result4, ref result7, out direction);
				continue;
			}
			Vector3.Subtract(ref extremePoint2, ref result2, out result5);
			Vector3.Cross(ref result7, ref result5, out result8);
			Vector3.Dot(ref result8, ref result6, out result9);
			if (!(result9 < 0f))
			{
				break;
			}
			extremePoint = extremePoint3;
			extremePointA = extremePointA3;
			extremePointB = extremePointB3;
			Vector3.Cross(ref result5, ref result7, out direction);
		}
		while (true)
		{
			Vector3.Subtract(ref extremePoint3, ref extremePoint2, out var result10);
			Vector3.Subtract(ref extremePoint, ref extremePoint2, out var result11);
			Vector3.Cross(ref result10, ref result11, out direction);
			Vector3.Subtract(ref extremePoint, ref minkowskiPosition, out var result12);
			Vector3.Dot(ref result12, ref direction, out var result13);
			MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction, ref localTransformB, out var extremePointA4, out var extremePointB4, out var extremePoint4);
			Vector3.Subtract(ref extremePoint4, ref minkowskiPosition, out var result14);
			Vector3.Dot(ref result14, ref direction, out var result15);
			if (result15 - result13 < rayCastSurfaceEpsilon || num3 > InnerIterationLimit)
			{
				break;
			}
			num3++;
			Vector3.Cross(ref result14, ref result6, out result8);
			Vector3.Dot(ref result12, ref result8, out result13);
			if (result13 >= 0f)
			{
				Vector3.Subtract(ref extremePoint2, ref minkowskiPosition, out var result16);
				Vector3.Dot(ref result16, ref result8, out result13);
				if (result13 >= 0f)
				{
					extremePoint = extremePoint4;
					extremePointA = extremePointA4;
					extremePointB = extremePointB4;
				}
				else
				{
					extremePoint3 = extremePoint4;
					extremePointA3 = extremePointA4;
					Vector3 extremePointB3 = extremePointB4;
				}
			}
			else
			{
				Vector3.Subtract(ref extremePoint3, ref minkowskiPosition, out var result17);
				Vector3.Dot(ref result17, ref result8, out result13);
				if (result13 >= 0f)
				{
					extremePoint2 = extremePoint4;
					extremePointA2 = extremePointA4;
					extremePointB2 = extremePointB4;
				}
				else
				{
					extremePoint = extremePoint4;
					extremePointA = extremePointA4;
					extremePointB = extremePointB4;
				}
			}
		}
		Toolbox.GetBarycentricCoordinates(ref minkowskiPosition, ref extremePoint, ref extremePoint2, ref extremePoint3, out var aWeight, out var bWeight, out var cWeight);
		Vector3.Multiply(ref extremePointA, aWeight, out position);
		Vector3.Multiply(ref extremePointA2, bWeight, out extremePointA2);
		Vector3.Multiply(ref extremePointA3, cWeight, out extremePointA3);
		Vector3.Add(ref extremePointA2, ref position, out position);
		Vector3.Add(ref extremePointA3, ref position, out position);
	}

	/// <summary>
	/// Determines if two shapes are intersecting.
	/// </summary>
	/// <param name="shapeA">First shape in the pair.</param>
	/// <param name="shapeB">Second shape in the pair.</param>
	/// <param name="sweep">Sweep direction and magnitude.</param>
	/// <param name="localTransformB">Transformation of shape B in the local space of A.</param>
	/// <param name="position">Position of the minkowski difference origin in the local space of A, if the swept volumes intersect.</param>
	/// <returns>Whether the swept shapes intersect.</returns>
	public static bool AreSweptShapesIntersecting(ConvexShape shapeA, ConvexShape shapeB, ref Vector3 sweep, ref RigidTransform localTransformB, out Vector3 position)
	{
		if (localTransformB.Position.LengthSquared() < 1E-07f)
		{
			position = default(Vector3);
			return true;
		}
		Vector3.Negate(ref localTransformB.Position, out var result);
		Vector3 extremePointDirection = localTransformB.Position;
		GetSweptExtremePoint(shapeA, shapeB, ref localTransformB, ref sweep, ref extremePointDirection, out var extremePointA, out var extremePoint);
		Vector3.Cross(ref extremePoint, ref result, out extremePointDirection);
		if (extremePointDirection.LengthSquared() < 1E-07f)
		{
			Vector3.Dot(ref extremePoint, ref localTransformB.Position, out var result2);
			if (result2 < 0f)
			{
				position = default(Vector3);
				return false;
			}
			Vector3.Dot(ref result, ref localTransformB.Position, out var result3);
			float scaleFactor = (0f - result3) / (result2 - result3);
			Vector3.Multiply(ref extremePointA, scaleFactor, out position);
			return true;
		}
		GetSweptExtremePoint(shapeA, shapeB, ref localTransformB, ref sweep, ref extremePointDirection, out var extremePointA2, out var extremePoint2);
		Vector3.Subtract(ref extremePoint, ref result, out var result4);
		Vector3.Subtract(ref extremePoint2, ref result, out var result5);
		Vector3.Cross(ref result4, ref result5, out extremePointDirection);
		int num = 0;
		Vector3 extremePointA3;
		Vector3 extremePoint3;
		while (true)
		{
			GetSweptExtremePoint(shapeA, shapeB, ref localTransformB, ref sweep, ref extremePointDirection, out extremePointA3, out extremePoint3);
			if (num > OuterIterationLimit)
			{
				break;
			}
			num++;
			Vector3.Cross(ref extremePoint, ref extremePoint3, out result4);
			Vector3.Dot(ref result4, ref result, out var result6);
			if (result6 < 0f)
			{
				extremePoint2 = extremePoint3;
				extremePointA2 = extremePointA3;
				Vector3.Subtract(ref extremePoint, ref result, out result4);
				Vector3.Subtract(ref extremePoint3, ref result, out result5);
				Vector3.Cross(ref result4, ref result5, out extremePointDirection);
				continue;
			}
			Vector3.Cross(ref extremePoint3, ref extremePoint2, out result4);
			Vector3.Dot(ref result4, ref result, out result6);
			if (!(result6 < 0f))
			{
				break;
			}
			extremePoint = extremePoint3;
			extremePointA = extremePointA3;
			Vector3.Subtract(ref extremePoint2, ref result, out result4);
			Vector3.Subtract(ref extremePoint3, ref result, out result5);
			Vector3.Cross(ref result4, ref result5, out extremePointDirection);
		}
		while (true)
		{
			Vector3.Subtract(ref extremePoint3, ref extremePoint2, out result4);
			Vector3.Subtract(ref extremePoint, ref extremePoint2, out result5);
			Vector3.Cross(ref result4, ref result5, out extremePointDirection);
			Vector3.Dot(ref extremePointDirection, ref extremePoint, out var result7);
			if (result7 >= 0f)
			{
				Vector3.Subtract(ref extremePoint, ref result, out result4);
				Vector3.Subtract(ref extremePoint2, ref result, out result5);
				Vector3.Subtract(ref extremePoint3, ref result, out var result8);
				Vector3.Cross(ref result4, ref result5, out var result9);
				Vector3.Dot(ref result9, ref result8, out var result10);
				Vector3.Cross(ref extremePoint, ref extremePoint2, out result9);
				Vector3.Dot(ref result9, ref extremePoint3, out var result11);
				Vector3.Cross(ref localTransformB.Position, ref result5, out result9);
				Vector3.Dot(ref result9, ref result8, out var result12);
				Vector3.Cross(ref result4, ref localTransformB.Position, out result9);
				Vector3.Dot(ref result9, ref result8, out var result13);
				float num2 = 1f / result10;
				float num3 = result11 * num2;
				float num4 = result12 * num2;
				float num5 = result13 * num2;
				float num6 = 1f - num3 - num4 - num5;
				position = num4 * extremePointA + num5 * extremePointA2 + num6 * extremePointA3;
				return true;
			}
			GetSweptExtremePoint(shapeA, shapeB, ref localTransformB, ref sweep, ref extremePointDirection, out var extremePointA4, out var extremePoint4);
			Vector3.Dot(ref extremePoint4, ref extremePointDirection, out var result14);
			if (result14 < 0f)
			{
				position = default(Vector3);
				return false;
			}
			if (result14 - result7 < surfaceEpsilon || num > InnerIterationLimit)
			{
				break;
			}
			num++;
			Vector3.Cross(ref extremePoint4, ref result, out result4);
			Vector3.Dot(ref extremePoint, ref result4, out result7);
			if (result7 >= 0f)
			{
				Vector3.Dot(ref extremePoint2, ref result4, out result7);
				if (result7 >= 0f)
				{
					extremePoint = extremePoint4;
					extremePointA = extremePointA4;
				}
				else
				{
					extremePoint3 = extremePoint4;
					extremePointA3 = extremePointA4;
				}
			}
			else
			{
				Vector3.Dot(ref extremePoint3, ref result4, out result7);
				if (result7 >= 0f)
				{
					extremePoint2 = extremePoint4;
					extremePointA2 = extremePointA4;
				}
				else
				{
					extremePoint = extremePoint4;
					extremePointA = extremePointA4;
				}
			}
		}
		position = default(Vector3);
		return false;
	}

	private static void GetSweptExtremePoint(ConvexShape shapeA, ConvexShape shapeB, ref RigidTransform localTransformB, ref Vector3 sweep, ref Vector3 extremePointDirection, out Vector3 extremePoint)
	{
		MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref extremePointDirection, ref localTransformB, out extremePoint);
		Vector3.Dot(ref extremePointDirection, ref sweep, out var result);
		if (result > 0f)
		{
			Vector3.Add(ref extremePoint, ref sweep, out extremePoint);
		}
	}

	private static void GetSweptExtremePoint(ConvexShape shapeA, ConvexShape shapeB, ref RigidTransform localTransformB, ref Vector3 sweep, ref Vector3 extremePointDirection, out Vector3 extremePointA, out Vector3 extremePoint)
	{
		MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref extremePointDirection, ref localTransformB, out extremePointA, out var _, out extremePoint);
		Vector3.Dot(ref extremePointDirection, ref sweep, out var result);
		if (result > 0f)
		{
			Vector3.Add(ref extremePoint, ref sweep, out extremePoint);
		}
	}
}
