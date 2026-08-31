using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.MathExtensions;
using BEPUphysics.Settings;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionTests.CollisionAlgorithms.GJK;

/// <summary>
///  Helper class containing various tests based on GJK.
/// </summary>
public static class GJKToolbox
{
	/// <summary>
	/// Maximum number of iterations the GJK algorithm will do.  If the iterations exceed this number, the system will immediately quit and return whatever information it has at the time.
	/// </summary>
	public static int MaximumGJKIterations = 15;

	/// <summary>
	/// Defines how many iterations are required to consider a GJK attempt to be 'probably stuck' and proceed with protective measures.
	/// </summary>
	public static int HighGJKIterations = 8;

	/// <summary>
	///  Tests if the pair is intersecting.
	/// </summary>
	/// <param name="shapeA">First shape of the pair.</param>
	/// <param name="shapeB">Second shape of the pair.</param>
	/// <param name="transformA">Transform to apply to the first shape.</param>
	/// <param name="transformB">Transform to apply to the second shape.</param>
	/// <returns>Whether or not the shapes are intersecting.</returns>
	public static bool AreShapesIntersecting(ConvexShape shapeA, ConvexShape shapeB, ref RigidTransform transformA, ref RigidTransform transformB)
	{
		Vector3 localSeparatingAxis = Toolbox.ZeroVector;
		return AreShapesIntersecting(shapeA, shapeB, ref transformA, ref transformB, ref localSeparatingAxis);
	}

	/// <summary>
	///  Tests if the pair is intersecting.
	/// </summary>
	/// <param name="shapeA">First shape of the pair.</param>
	/// <param name="shapeB">Second shape of the pair.</param>
	/// <param name="transformA">Transform to apply to the first shape.</param>
	/// <param name="transformB">Transform to apply to the second shape.</param>
	/// <param name="localSeparatingAxis">Warmstartable separating axis used by the method to quickly early-out if possible.  Updated to the latest separating axis after each run.</param>
	/// <returns>Whether or not the objects were intersecting.</returns>
	public static bool AreShapesIntersecting(ConvexShape shapeA, ConvexShape shapeB, ref RigidTransform transformA, ref RigidTransform transformB, ref Vector3 localSeparatingAxis)
	{
		MinkowskiToolbox.GetLocalTransform(ref transformA, ref transformB, out var localTransformB);
		SimpleSimplex simpleSimplex = default(SimpleSimplex);
		MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref localSeparatingAxis, ref localTransformB, out var extremePoint);
		simpleSimplex.AddNewSimplexPoint(ref extremePoint);
		int num = 0;
		while (num++ < MaximumGJKIterations)
		{
			if (simpleSimplex.GetPointClosestToOrigin(out var point) || point.LengthSquared() <= simpleSimplex.GetErrorTolerance() * 1E-05f)
			{
				return true;
			}
			Vector3.Negate(ref point, out var result);
			MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref result, ref localTransformB, out extremePoint);
			Vector3.Dot(ref extremePoint, ref point, out var result2);
			if (result2 > 0f)
			{
				localSeparatingAxis = result;
				return false;
			}
			simpleSimplex.AddNewSimplexPoint(ref extremePoint);
		}
		return false;
	}

	/// <summary>
	///  Gets the closest points between the shapes.
	/// </summary>
	/// <param name="shapeA">First shape of the pair.</param>
	/// <param name="shapeB">Second shape of the pair.</param>
	/// <param name="transformA">Transform to apply to the first shape.</param>
	/// <param name="transformB">Transform to apply to the second shape.</param>
	/// <param name="closestPointA">Closest point on the first shape to the second shape.</param>
	/// <param name="closestPointB">Closest point on the second shape to the first shape.</param>
	/// <returns>Whether or not the objects were intersecting.  If they are intersecting, then the closest points cannot be identified.</returns>
	public static bool GetClosestPoints(ConvexShape shapeA, ConvexShape shapeB, ref RigidTransform transformA, ref RigidTransform transformB, out Vector3 closestPointA, out Vector3 closestPointB)
	{
		MinkowskiToolbox.GetLocalTransform(ref transformA, ref transformB, out var localTransformB);
		CachedSimplex cachedSimplex = new CachedSimplex
		{
			State = SimplexState.Point
		};
		bool closestPoints = GetClosestPoints(shapeA, shapeB, ref localTransformB, ref cachedSimplex, out closestPointA, out closestPointB);
		RigidTransform.Transform(ref closestPointA, ref transformA, out closestPointA);
		RigidTransform.Transform(ref closestPointB, ref transformA, out closestPointB);
		return closestPoints;
	}

	/// <summary>
	///  Gets the closest points between the shapes.
	/// </summary>
	/// <param name="shapeA">First shape of the pair.</param>
	/// <param name="shapeB">Second shape of the pair.</param>
	/// <param name="transformA">Transform to apply to the first shape.</param>
	/// <param name="transformB">Transform to apply to the second shape.</param>
	///  <param name="cachedSimplex">Simplex from a previous updated used to warmstart the current attempt.  Updated after each run.</param>
	/// <param name="closestPointA">Closest point on the first shape to the second shape.</param>
	/// <param name="closestPointB">Closest point on the second shape to the first shape.</param>
	/// <returns>Whether or not the objects were intersecting.  If they are intersecting, then the closest points cannot be identified.</returns>
	public static bool GetClosestPoints(ConvexShape shapeA, ConvexShape shapeB, ref RigidTransform transformA, ref RigidTransform transformB, ref CachedSimplex cachedSimplex, out Vector3 closestPointA, out Vector3 closestPointB)
	{
		MinkowskiToolbox.GetLocalTransform(ref transformA, ref transformB, out var localTransformB);
		bool closestPoints = GetClosestPoints(shapeA, shapeB, ref localTransformB, ref cachedSimplex, out closestPointA, out closestPointB);
		RigidTransform.Transform(ref closestPointA, ref transformA, out closestPointA);
		RigidTransform.Transform(ref closestPointB, ref transformA, out closestPointB);
		return closestPoints;
	}

	private static bool GetClosestPoints(ConvexShape shapeA, ConvexShape shapeB, ref RigidTransform localTransformB, ref CachedSimplex cachedSimplex, out Vector3 localClosestPointA, out Vector3 localClosestPointB)
	{
		PairSimplex pairSimplex = new PairSimplex(ref cachedSimplex, ref localTransformB);
		int num = 0;
		Vector3 point;
		do
		{
			if (pairSimplex.GetPointClosestToOrigin(out point) || point.LengthSquared() <= 1E-07f * pairSimplex.errorTolerance)
			{
				localClosestPointA = Toolbox.ZeroVector;
				localClosestPointB = Toolbox.ZeroVector;
				pairSimplex.UpdateCachedSimplex(ref cachedSimplex);
				return true;
			}
		}
		while (++num <= MaximumGJKIterations && !pairSimplex.GetNewSimplexPoint(shapeA, shapeB, num, ref point));
		pairSimplex.GetClosestPoints(out localClosestPointA, out localClosestPointB);
		pairSimplex.UpdateCachedSimplex(ref cachedSimplex);
		return false;
	}

	/// <summary>
	///  Tests a ray against a convex shape.
	/// </summary>
	/// <param name="ray">Ray to test against the shape.</param>
	/// <param name="shape">Shape to test.</param>
	/// <param name="shapeTransform">Transform to apply to the shape for the test.</param>
	/// <param name="maximumLength">Maximum length of the ray in units of the ray direction's length.</param>
	/// <param name="hit">Hit data of the ray cast, if any.</param>
	/// <returns>Whether or not the ray hit the shape.</returns>
	public static bool RayCast(Ray ray, ConvexShape shape, ref RigidTransform shapeTransform, float maximumLength, out RayHit hit)
	{
		Vector3.Subtract(ref ray.Position, ref shapeTransform.Position, out ray.Position);
		Quaternion.Conjugate(ref shapeTransform.Orientation, out var result);
		Vector3.Transform(ref ray.Position, ref result, out ray.Position);
		Vector3.Transform(ref ray.Direction, ref result, out ray.Direction);
		hit.T = 0f;
		hit.Location = ray.Position;
		hit.Normal = Toolbox.ZeroVector;
		Vector3 vector = hit.Location;
		RaySimplex simplex = default(RaySimplex);
		int num = 0;
		while (vector.LengthSquared() >= 1E-07f * simplex.GetErrorTolerance(ref ray.Position))
		{
			if (++num > MaximumGJKIterations)
			{
				hit = default(RayHit);
				return false;
			}
			shape.GetLocalExtremePoint(vector, out var extremePoint);
			Vector3.Subtract(ref hit.Location, ref extremePoint, out var result2);
			Vector3.Dot(ref vector, ref result2, out var result3);
			if (result3 > 0f)
			{
				Vector3.Dot(ref vector, ref ray.Direction, out var result4);
				if (result4 >= 0f)
				{
					hit = default(RayHit);
					return false;
				}
				hit.T -= result3 / result4;
				if (hit.T > maximumLength)
				{
					hit = default(RayHit);
					return false;
				}
				Vector3.Multiply(ref ray.Direction, hit.T, out hit.Location);
				Vector3.Add(ref hit.Location, ref ray.Position, out hit.Location);
				hit.Normal = vector;
			}
			simplex.AddNewSimplexPoint(ref extremePoint, ref hit.Location, out var shiftedSimplex);
			shiftedSimplex.GetPointClosestToOrigin(ref simplex, out vector);
		}
		Vector3.Transform(ref hit.Normal, ref shapeTransform.Orientation, out hit.Normal);
		Vector3.Transform(ref hit.Location, ref shapeTransform.Orientation, out hit.Location);
		Vector3.Add(ref hit.Location, ref shapeTransform.Position, out hit.Location);
		return true;
	}

	/// <summary>
	///  Sweeps a shape against another shape using a given sweep vector.
	/// </summary>
	/// <param name="sweptShape">Shape to sweep.</param>
	/// <param name="target">Shape being swept against.</param>
	/// <param name="sweep">Sweep vector for the sweptShape.</param>
	/// <param name="startingSweptTransform">Starting transform of the sweptShape.</param>
	/// <param name="targetTransform">Transform to apply to the target shape.</param>
	/// <param name="hit">Hit data of the sweep test, if any.</param>
	/// <returns>Whether or not the swept shape hit the other shape.</returns>
	public static bool ConvexCast(ConvexShape sweptShape, ConvexShape target, ref Vector3 sweep, ref RigidTransform startingSweptTransform, ref RigidTransform targetTransform, out RayHit hit)
	{
		return ConvexCast(sweptShape, target, ref sweep, ref Toolbox.ZeroVector, ref startingSweptTransform, ref targetTransform, out hit);
	}

	/// <summary>
	///  Sweeps two shapes against another.
	/// </summary>
	/// <param name="shapeA">First shape being swept.</param>
	/// <param name="shapeB">Second shape being swept.</param>
	/// <param name="sweepA">Sweep vector for the first shape.</param>
	/// <param name="sweepB">Sweep vector for the second shape.</param>
	/// <param name="transformA">Transform to apply to the first shape.</param>
	/// <param name="transformB">Transform to apply to the second shape.</param>
	/// <param name="hit">Hit data of the sweep test, if any.</param>
	/// <returns>Whether or not the swept shapes hit each other..</returns>
	public static bool ConvexCast(ConvexShape shapeA, ConvexShape shapeB, ref Vector3 sweepA, ref Vector3 sweepB, ref RigidTransform transformA, ref RigidTransform transformB, out RayHit hit)
	{
		Vector3.Subtract(ref sweepB, ref sweepA, out var result);
		Quaternion.Conjugate(ref transformA.Orientation, out var result2);
		Vector3.Transform(ref result, ref result2, out var result3);
		RigidTransform localTransformB = default(RigidTransform);
		Quaternion.Concatenate(ref transformB.Orientation, ref result2, out localTransformB.Orientation);
		Vector3.Subtract(ref transformB.Position, ref transformA.Position, out localTransformB.Position);
		Vector3.Transform(ref localTransformB.Position, ref result2, out localTransformB.Position);
		hit.T = 0f;
		hit.Location = Vector3.Zero;
		hit.Normal = Toolbox.ZeroVector;
		Vector3 direction = hit.Location;
		RaySimplex simplex = default(RaySimplex);
		int num = 0;
		do
		{
			if (++num > MaximumGJKIterations)
			{
				hit = default(RayHit);
				return false;
			}
			MinkowskiToolbox.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction, ref localTransformB, out var extremePoint);
			Vector3.Subtract(ref hit.Location, ref extremePoint, out var result4);
			Vector3.Dot(ref direction, ref result4, out var result5);
			if (result5 > 0f)
			{
				Vector3.Dot(ref direction, ref result3, out var result6);
				if (result6 >= 0f)
				{
					hit = default(RayHit);
					return false;
				}
				hit.T -= result5 / result6;
				if (hit.T > 1f)
				{
					hit = default(RayHit);
					return false;
				}
				Vector3.Multiply(ref result3, hit.T, out hit.Location);
				hit.Normal = direction;
			}
			simplex.AddNewSimplexPoint(ref extremePoint, ref hit.Location, out var shiftedSimplex);
			shiftedSimplex.GetPointClosestToOrigin(ref simplex, out direction);
		}
		while (direction.LengthSquared() >= 1E-07f * simplex.GetErrorTolerance(ref Toolbox.ZeroVector));
		Vector3.Transform(ref hit.Normal, ref transformA.Orientation, out hit.Normal);
		Vector3.Multiply(ref result, hit.T, out hit.Location);
		Vector3.Add(ref hit.Location, ref transformA.Position, out hit.Location);
		return true;
	}

	/// <summary>
	///  Casts a fat (sphere expanded) ray against the shape.
	/// </summary>
	/// <param name="ray">Ray to test against the shape.</param>
	/// <param name="radius">Radius of the ray.</param>
	/// <param name="shape">Shape to test against.</param>
	/// <param name="shapeTransform">Transform to apply to the shape for the test.</param>
	/// <param name="maximumLength">Maximum length of the ray in units of the ray direction's length.</param>
	/// <param name="hit">Hit data of the sphere cast, if any.</param>
	/// <returns>Whether or not the sphere cast hit the shape.</returns>
	public static bool SphereCast(Ray ray, float radius, ConvexShape shape, ref RigidTransform shapeTransform, float maximumLength, out RayHit hit)
	{
		Vector3.Subtract(ref ray.Position, ref shapeTransform.Position, out ray.Position);
		Quaternion.Conjugate(ref shapeTransform.Orientation, out var result);
		Vector3.Transform(ref ray.Position, ref result, out ray.Position);
		Vector3.Transform(ref ray.Direction, ref result, out ray.Direction);
		hit.T = 0f;
		hit.Location = ray.Position;
		hit.Normal = Toolbox.ZeroVector;
		Vector3 direction = hit.Location;
		RaySimplex simplex = default(RaySimplex);
		int num = 0;
		while (direction.LengthSquared() >= 1E-07f * simplex.GetErrorTolerance(ref ray.Position))
		{
			if (++num > MaximumGJKIterations)
			{
				hit = default(RayHit);
				return false;
			}
			shape.GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint);
			MinkowskiToolbox.ExpandMinkowskiSum(shape.collisionMargin, radius, ref direction, out var contribution);
			Vector3.Add(ref extremePoint, ref contribution, out extremePoint);
			Vector3.Subtract(ref hit.Location, ref extremePoint, out var result2);
			Vector3.Dot(ref direction, ref result2, out var result3);
			if (result3 > 0f)
			{
				Vector3.Dot(ref direction, ref ray.Direction, out var result4);
				hit.T -= result3 / result4;
				if (result4 >= 0f)
				{
					return false;
				}
				if (hit.T > maximumLength)
				{
					return false;
				}
				Vector3.Multiply(ref ray.Direction, hit.T, out hit.Location);
				Vector3.Add(ref hit.Location, ref ray.Position, out hit.Location);
				hit.Normal = direction;
			}
			simplex.AddNewSimplexPoint(ref extremePoint, ref hit.Location, out var shiftedSimplex);
			shiftedSimplex.GetPointClosestToOrigin(ref simplex, out direction);
		}
		Vector3.Transform(ref hit.Normal, ref shapeTransform.Orientation, out hit.Normal);
		Vector3.Transform(ref hit.Location, ref shapeTransform.Orientation, out hit.Location);
		Vector3.Add(ref hit.Location, ref shapeTransform.Position, out hit.Location);
		return true;
	}

	/// <summary>
	///  Casts a fat (sphere expanded) ray against the shape.  If the raycast appears to be stuck in the shape, the cast will be attempted
	///  with a smaller ray (scaled by the MotionSettings.CoreShapeScaling each time).
	/// </summary>
	/// <param name="ray">Ray to test against the shape.</param>
	/// <param name="radius">Radius of the ray.</param>
	/// <param name="target">Shape to test against.</param>
	/// <param name="shapeTransform">Transform to apply to the shape for the test.</param>
	/// <param name="maximumLength">Maximum length of the ray in units of the ray direction's length.</param>
	/// <param name="hit">Hit data of the sphere cast, if any.</param>
	/// <returns>Whether or not the sphere cast hit the shape.</returns>
	public static bool CCDSphereCast(Ray ray, float radius, ConvexShape target, ref RigidTransform shapeTransform, float maximumLength, out RayHit hit)
	{
		int num = 0;
		do
		{
			if (SphereCast(ray, radius, target, ref shapeTransform, maximumLength, out hit) && hit.T > 0f)
			{
				return true;
			}
			if (hit.T > maximumLength || hit.T < 0f)
			{
				return false;
			}
			radius *= MotionSettings.CoreShapeScaling;
			num++;
		}
		while (num <= 3);
		if (RayCast(ray, target, ref shapeTransform, maximumLength, out hit))
		{
			return hit.T > 0f;
		}
		return false;
	}
}
