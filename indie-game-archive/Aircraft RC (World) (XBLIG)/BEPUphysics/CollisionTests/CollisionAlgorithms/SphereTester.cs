using System;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.Settings;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionTests.CollisionAlgorithms;

/// <summary>
///  Helper class to test spheres against each other.
/// </summary>
public static class SphereTester
{
	/// <summary>
	/// Computes contact data for two spheres.
	/// </summary>
	/// <param name="a">First sphere.</param>
	/// <param name="b">Second sphere.</param>
	/// <param name="positionA">Position of the first sphere.</param>
	/// <param name="positionB">Position of the second sphere.</param>
	/// <param name="contact">Contact data between the spheres, if any.</param>
	/// <returns>Whether or not the spheres are touching.</returns>
	public static bool AreSpheresColliding(SphereShape a, SphereShape b, ref Vector3 positionA, ref Vector3 positionB, out ContactData contact)
	{
		contact = default(ContactData);
		float num = a.collisionMargin + b.collisionMargin;
		Vector3.Subtract(ref positionB, ref positionA, out var result);
		float num2 = result.LengthSquared();
		if (num2 < (num + CollisionDetectionSettings.maximumContactDistance) * (num + CollisionDetectionSettings.maximumContactDistance))
		{
			if (num > 1E-07f)
			{
				Vector3.Multiply(ref result, a.collisionMargin / num, out contact.Position);
			}
			else
			{
				contact.Position = default(Vector3);
			}
			Vector3.Add(ref contact.Position, ref positionA, out contact.Position);
			num2 = (float)Math.Sqrt(num2);
			if (num2 > 1E-05f)
			{
				Vector3.Divide(ref result, num2, out contact.Normal);
			}
			else
			{
				contact.Normal = Toolbox.UpVector;
			}
			contact.PenetrationDepth = num - num2;
			return true;
		}
		return false;
	}
}
