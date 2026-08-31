using System;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.DataStructures;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionTests.CollisionAlgorithms;

/// <summary>
///  Persistent tester that compares triangles against convex objects.
/// </summary>
public sealed class TriangleSpherePairTester : TrianglePairTester
{
	internal SphereShape sphere;

	private VoronoiRegion lastRegion;

	public override bool ShouldCorrectContactNormal => false;

	/// <summary>
	///  Generates a contact between the triangle and convex.
	/// </summary>
	/// <param name="contactList">Contact between the shapes, if any.</param>
	/// <returns>Whether or not the shapes are colliding.</returns>
	public override bool GenerateContactCandidate(out TinyStructList<ContactData> contactList)
	{
		contactList = default(TinyStructList<ContactData>);
		Vector3.Subtract(ref triangle.vB, ref triangle.vA, out var result);
		Vector3.Subtract(ref triangle.vC, ref triangle.vA, out var result2);
		Vector3.Cross(ref result, ref result2, out var result3);
		if (result3.LengthSquared() < 1E-09f)
		{
			Vector3.Add(ref triangle.vA, ref triangle.vB, out result3);
			Vector3.Add(ref result3, ref triangle.vC, out result3);
			Vector3.Multiply(ref result3, 1f / 3f, out result3);
			if (result3.LengthSquared() < 1E-09f)
			{
				result3 = Toolbox.UpVector;
			}
		}
		Vector3.Dot(ref result3, ref triangle.vA, out var result4);
		switch (triangle.sidedness)
		{
		case TriangleSidedness.DoubleSided:
			if (result4 < 0f)
			{
				Vector3.Negate(ref result3, out result3);
			}
			break;
		case TriangleSidedness.Clockwise:
			if (result4 > 0f)
			{
				return false;
			}
			break;
		case TriangleSidedness.Counterclockwise:
			if (result4 < 0f)
			{
				return false;
			}
			break;
		}
		lastRegion = Toolbox.GetClosestPointOnTriangleToPoint(ref triangle.vA, ref triangle.vB, ref triangle.vC, ref Toolbox.ZeroVector, out var closestPoint);
		float num = closestPoint.LengthSquared();
		float num2 = triangle.collisionMargin + sphere.collisionMargin;
		if (num <= num2 * num2)
		{
			ContactData item = default(ContactData);
			if (num < 1E-07f)
			{
				Vector3.Negate(ref result3, out item.Normal);
				item.Normal.Normalize();
				item.PenetrationDepth = num2;
				contactList.Add(ref item);
				return true;
			}
			num = (float)Math.Sqrt(num);
			Vector3.Divide(ref closestPoint, num, out item.Normal);
			item.PenetrationDepth = num2 - num;
			item.Position = closestPoint;
			contactList.Add(ref item);
			return true;
		}
		return false;
	}

	public override VoronoiRegion GetRegion(ref ContactData contact)
	{
		return lastRegion;
	}

	/// <summary>
	///  Initializes the pair tester.
	/// </summary>
	/// <param name="convex">Convex shape to use.</param>
	/// <param name="triangle">Triangle shape to use.</param>
	public override void Initialize(ConvexShape convex, TriangleShape triangle)
	{
		sphere = (SphereShape)convex;
		base.triangle = triangle;
	}

	/// <summary>
	/// Cleans up the pair tester.
	/// </summary>
	public override void CleanUp()
	{
		triangle = null;
		sphere = null;
		Updated = false;
	}
}
