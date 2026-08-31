using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using BEPUphysics.ResourceManagement;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionShapes.ConvexShapes;

/// <summary>
///  Helper class used to compute volume distribution information, which is in turn used to compute inertia tensor information.
/// </summary>
public class InertiaHelper
{
	/// <summary>
	/// Value to scale any created entities' inertia tensors by.
	/// Larger tensors (above 1) improve stiffness of constraints and contacts, while smaller values (towards 1) are closer to 'realistic' behavior.
	/// Defaults to 2.5.
	/// </summary>
	public static float InertiaTensorScale = 2.5f;

	/// <summary>
	///  Number of samples the system takes along a side of an object's AABB when voxelizing it.
	/// </summary>
	public static int NumberOfSamplesPerDimension = 10;

	/// <summary>
	///  Computes the center of a convex shape.
	/// </summary>
	/// <param name="shape">Shape to compute the center of.</param>
	/// <returns>Center of the shape.</returns>
	public static Vector3 ComputeCenter(ConvexShape shape)
	{
		float volume;
		return ComputeCenter(shape, out volume);
	}

	/// <summary>
	///  Computes the center and volume of a convex shape.
	/// </summary>
	/// <param name="shape">Shape to compute the center of.</param>
	/// <param name="volume">Volume of the shape.</param>
	/// <returns>Center of the shape.</returns>
	public static Vector3 ComputeCenter(ConvexShape shape, out float volume)
	{
		RawList<Vector3> vectorList = Resources.GetVectorList();
		GetPoints(shape, out volume, vectorList);
		Vector3 result = AveragePoints(vectorList);
		Resources.GiveBack(vectorList);
		return result;
	}

	/// <summary>
	///  Averages together all the points in the point list.
	/// </summary>
	/// <param name="pointContributions">Point list to average.</param>
	/// <returns>Averaged point.</returns>
	public static Vector3 AveragePoints(RawList<Vector3> pointContributions)
	{
		Vector3 vector = default(Vector3);
		for (int i = 0; i < pointContributions.Count; i++)
		{
			vector += pointContributions[i];
		}
		return vector / pointContributions.Count;
	}

	/// <summary>
	///  Computes the volume and volume distribution of a shape.
	/// </summary>
	/// <param name="shape">Shape to compute the volume information of.</param>
	/// <param name="volume">Volume of the shape.</param>
	/// <returns>Volume distribution of the shape.</returns>
	public static Matrix3X3 ComputeVolumeDistribution(ConvexShape shape, out float volume)
	{
		RawList<Vector3> vectorList = Resources.GetVectorList();
		GetPoints(shape, out volume, vectorList);
		Vector3 center = AveragePoints(vectorList);
		Matrix3X3 result = ComputeVolumeDistribution(vectorList, ref center);
		Resources.GiveBack(vectorList);
		return result;
	}

	/// <summary>
	///  Computes the volume and volume distribution of a shape based on a given center.
	/// </summary>
	/// <param name="shape">Shape to compute the volume information of.</param>
	/// <param name="center">Location to use as the center of the shape when computing the volume distribution.</param>
	/// <param name="volume">Volume of the shape.</param>
	/// <returns>Volume distribution of the shape.</returns>
	public static Matrix3X3 ComputeVolumeDistribution(ConvexShape shape, ref Vector3 center, out float volume)
	{
		RawList<Vector3> vectorList = Resources.GetVectorList();
		GetPoints(shape, out volume, vectorList);
		Matrix3X3 result = ComputeVolumeDistribution(vectorList, ref center);
		Resources.GiveBack(vectorList);
		return result;
	}

	/// <summary>
	///  Computes a volume distribution based on a bunch of point contributions.
	/// </summary>
	/// <param name="pointContributions">Point contributions to the volume distribution.</param>
	/// <param name="center">Location to use as the center for purposes of computing point contributions.</param>
	/// <returns>Volume distribution of the point contributions.</returns>
	public static Matrix3X3 ComputeVolumeDistribution(RawList<Vector3> pointContributions, ref Vector3 center)
	{
		Matrix3X3 a = default(Matrix3X3);
		float pointWeight = 1f / (float)pointContributions.Count;
		for (int i = 0; i < pointContributions.Count; i++)
		{
			GetPointContribution(pointWeight, ref center, pointContributions[i], out var contribution);
			Matrix3X3.Add(ref a, ref contribution, out a);
		}
		return a;
	}

	/// <summary>
	///  Gets the point contributions within a convex shape.
	/// </summary>
	/// <param name="shape">Shape to compute the point contributions of.</param>
	/// <param name="volume">Volume of the shape.</param>
	/// <param name="outputPointContributions">Point contributions of the shape.</param>
	public static void GetPoints(ConvexShape shape, out float volume, RawList<Vector3> outputPointContributions)
	{
		RigidTransform shapeTransform = RigidTransform.Identity;
		shape.GetBoundingBox(ref shapeTransform, out var boundingBox);
		float num = boundingBox.Max.X - boundingBox.Min.X;
		float num2 = boundingBox.Max.Y - boundingBox.Min.Y;
		float num3 = boundingBox.Max.Z - boundingBox.Min.Z;
		float num4 = num2 * num3;
		float num5 = num * num3;
		float num6 = num * num2;
		float num7 = 1f / (float)NumberOfSamplesPerDimension;
		Ray ray = default(Ray);
		Vector3 increment;
		Vector3 increment2;
		float rayIncrement;
		float num8;
		if (num4 > num5 && num4 > num6)
		{
			ray.Direction = Vector3.Right;
			ray.Position = new Vector3(boundingBox.Min.X, boundingBox.Min.Y + 0.5f * num7 * num2, boundingBox.Min.Z + 0.5f * num7 * num3);
			increment = new Vector3(0f, num7 * num2, 0f);
			increment2 = new Vector3(0f, 0f, num7 * num3);
			rayIncrement = num7 * num;
			num8 = num;
		}
		else if (num5 > num6)
		{
			ray.Direction = Vector3.Up;
			ray.Position = new Vector3(boundingBox.Min.X + 0.5f * num7 * num, boundingBox.Min.Y, boundingBox.Min.Z + 0.5f * num7 * num3);
			increment = new Vector3(num7 * num, 0f, 0f);
			increment2 = new Vector3(0f, 0f, num7 * num2);
			rayIncrement = num7 * num2;
			num8 = num2;
		}
		else
		{
			ray.Direction = Vector3.Backward;
			ray.Position = new Vector3(boundingBox.Min.X + 0.5f * num7 * num, boundingBox.Min.Y + 0.5f * num7 * num2, boundingBox.Min.Z);
			increment = new Vector3(num7 * num, 0f, 0f);
			increment2 = new Vector3(0f, num7 * num2, 0f);
			rayIncrement = num7 * num3;
			num8 = num3;
		}
		volume = 0f;
		Ray ray2 = default(Ray);
		for (int i = 0; i < NumberOfSamplesPerDimension; i++)
		{
			for (int j = 0; j < NumberOfSamplesPerDimension; j++)
			{
				if (shape.RayTest(ref ray, ref shapeTransform, num8, out var hit))
				{
					Vector3.Multiply(ref ray.Direction, num8, out ray2.Position);
					Vector3.Add(ref ray2.Position, ref ray.Position, out ray2.Position);
					Vector3.Negate(ref ray.Direction, out ray2.Direction);
					if (shape.RayTest(ref ray2, ref shapeTransform, num8, out var hit2))
					{
						ScanObject(rayIncrement, num8, ref increment, ref increment2, ref ray, ref hit, ref hit2, outputPointContributions, out var volume2);
						volume += volume2;
					}
				}
				Vector3.Add(ref ray.Position, ref increment2, out ray.Position);
			}
			Vector3.Add(ref ray.Position, ref increment, out ray.Position);
			Vector3.Multiply(ref increment2, NumberOfSamplesPerDimension, out var result);
			Vector3.Subtract(ref ray.Position, ref result, out ray.Position);
		}
	}

	private static void ScanObject(float rayIncrement, float maxLength, ref Vector3 increment1, ref Vector3 increment2, ref Ray ray, ref RayHit startHit, ref RayHit endHit, RawList<Vector3> pointContributions, out float volume)
	{
		Vector3.Multiply(ref ray.Direction, rayIncrement, out var result);
		Vector3.Add(ref increment1, ref result, out result);
		Vector3.Add(ref increment2, ref result, out result);
		float num = result.X * result.Y * result.Z;
		volume = 0f;
		for (int i = (int)(startHit.T / rayIncrement); i <= (int)((maxLength - endHit.T) / rayIncrement); i++)
		{
			Vector3.Multiply(ref ray.Direction, ((float)i + 0.5f) * rayIncrement, out var result2);
			Vector3.Add(ref result2, ref ray.Position, out result2);
			pointContributions.Add(result2);
			volume += num;
		}
	}

	/// <summary>
	///  Computes the volume contribution of a point.
	/// </summary>
	/// <param name="pointWeight">Weight of the point.</param>
	/// <param name="center">Location to use as the center for the purposes of computing the contribution.</param>
	/// <param name="p">Point to compute the contribution of.</param>
	/// <param name="contribution">Contribution of the point.</param>
	public static void GetPointContribution(float pointWeight, ref Vector3 center, Vector3 p, out Matrix3X3 contribution)
	{
		Vector3.Subtract(ref p, ref center, out p);
		float num = pointWeight * p.X * p.X;
		float num2 = pointWeight * p.Y * p.Y;
		float num3 = pointWeight * p.Z * p.Z;
		contribution.M11 = num2 + num3;
		contribution.M22 = num + num3;
		contribution.M33 = num + num2;
		contribution.M12 = (0f - pointWeight) * p.X * p.Y;
		contribution.M13 = (0f - pointWeight) * p.X * p.Z;
		contribution.M23 = (0f - pointWeight) * p.Y * p.Z;
		contribution.M21 = contribution.M12;
		contribution.M31 = contribution.M13;
		contribution.M32 = contribution.M23;
	}
}
