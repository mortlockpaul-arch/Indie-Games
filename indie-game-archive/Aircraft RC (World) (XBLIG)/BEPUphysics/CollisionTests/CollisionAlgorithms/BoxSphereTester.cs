using System;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.MathExtensions;
using BEPUphysics.Settings;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionTests.CollisionAlgorithms;

/// <summary>
///  Static class with methods to help with testing box shapes against sphere shapes.
/// </summary>
public static class BoxSphereTester
{
	/// <summary>
	///  Tests if a box and sphere are colliding.
	/// </summary>
	/// <param name="box">Box to test.</param>
	/// <param name="sphere">Sphere to test.</param>
	/// <param name="boxTransform">Transform to apply to the box.</param>
	/// <param name="spherePosition">Transform to apply to the sphere.</param>
	/// <param name="contact">Contact point between the shapes, if any.</param>
	/// <returns>Whether or not the shapes were colliding.</returns>
	public static bool AreShapesColliding(BoxShape box, SphereShape sphere, ref RigidTransform boxTransform, ref Vector3 spherePosition, out ContactData contact)
	{
		contact = default(ContactData);
		RigidTransform.TransformByInverse(ref spherePosition, ref boxTransform, out var result);
		Vector3 position = new Vector3
		{
			X = MathHelper.Clamp(result.X, 0f - box.halfWidth, box.halfWidth),
			Y = MathHelper.Clamp(result.Y, 0f - box.halfHeight, box.halfHeight),
			Z = MathHelper.Clamp(result.Z, 0f - box.halfLength, box.halfLength)
		};
		RigidTransform.Transform(ref position, ref boxTransform, out contact.Position);
		Vector3.Subtract(ref spherePosition, ref contact.Position, out var result2);
		float num = result2.LengthSquared();
		if (num > (sphere.collisionMargin + CollisionDetectionSettings.maximumContactDistance) * (sphere.collisionMargin + CollisionDetectionSettings.maximumContactDistance))
		{
			return false;
		}
		if (num > 1E-07f)
		{
			num = (float)Math.Sqrt(num);
			Vector3.Divide(ref result2, num, out contact.Normal);
			contact.PenetrationDepth = sphere.collisionMargin - num;
		}
		else
		{
			Vector3 vector = default(Vector3);
			vector.X = ((position.X < 0f) ? (position.X + box.halfWidth) : (box.halfWidth - position.X));
			vector.Y = ((position.Y < 0f) ? (position.Y + box.halfHeight) : (box.halfHeight - position.Y));
			vector.Z = ((position.Z < 0f) ? (position.Z + box.halfLength) : (box.halfLength - position.Z));
			if (vector.X < vector.Y && vector.X < vector.Z)
			{
				contact.Normal = ((position.X > 0f) ? Toolbox.RightVector : Toolbox.LeftVector);
				contact.PenetrationDepth = vector.X;
			}
			else if (vector.Y < vector.Z)
			{
				contact.Normal = ((position.Y > 0f) ? Toolbox.UpVector : Toolbox.DownVector);
				contact.PenetrationDepth = vector.Y;
			}
			else
			{
				contact.Normal = ((position.Z > 0f) ? Toolbox.BackVector : Toolbox.ForwardVector);
				contact.PenetrationDepth = vector.X;
			}
			contact.PenetrationDepth += sphere.collisionMargin;
			Vector3.Transform(ref contact.Normal, ref boxTransform.Orientation, out contact.Normal);
		}
		return true;
	}
}
