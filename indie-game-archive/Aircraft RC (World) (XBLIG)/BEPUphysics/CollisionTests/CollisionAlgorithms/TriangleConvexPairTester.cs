using System;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests.CollisionAlgorithms.GJK;
using BEPUphysics.DataStructures;
using BEPUphysics.Settings;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionTests.CollisionAlgorithms;

/// <summary>
///  Persistent tester that compares triangles against convex objects.
/// </summary>
public class TriangleConvexPairTester : TrianglePairTester
{
	internal enum CollisionState
	{
		Plane,
		ExternalSeparated,
		ExternalNear,
		Deep
	}

	private const int EscapeAttemptPeriod = 10;

	internal ConvexShape convex;

	internal CollisionState state;

	private int escapeAttempts;

	private Vector3 localSeparatingAxis;

	public override bool ShouldCorrectContactNormal => state == CollisionState.Deep;

	/// <summary>
	///  Generates a contact between the triangle and convex.
	/// </summary>
	/// <param name="contactList">Contact between the shapes, if any.</param>
	/// <returns>Whether or not the shapes are colliding.</returns>
	public override bool GenerateContactCandidate(out TinyStructList<ContactData> contactList)
	{
		switch (state)
		{
		case CollisionState.Plane:
			return DoPlaneTest(out contactList);
		case CollisionState.ExternalSeparated:
			return DoExternalSeparated(out contactList);
		case CollisionState.ExternalNear:
			return DoExternalNear(out contactList);
		case CollisionState.Deep:
			return DoDeepContact(out contactList);
		default:
			contactList = default(TinyStructList<ContactData>);
			return false;
		}
	}

	private bool DoPlaneTest(out TinyStructList<ContactData> contactList)
	{
		Vector3.Subtract(ref triangle.vB, ref triangle.vA, out var result);
		Vector3.Subtract(ref triangle.vC, ref triangle.vA, out var result2);
		Vector3.Cross(ref result2, ref result, out var result3);
		Vector3.Dot(ref triangle.vA, ref result3, out var result4);
		contactList = default(TinyStructList<ContactData>);
		switch (triangle.sidedness)
		{
		case TriangleSidedness.DoubleSided:
			if (result4 < 0f)
			{
				Vector3.Negate(ref result3, out result3);
				result4 = 0f - result4;
			}
			break;
		case TriangleSidedness.Counterclockwise:
			Vector3.Negate(ref result3, out result3);
			result4 = 0f - result4;
			break;
		}
		convex.GetLocalExtremePointWithoutMargin(ref result3, out var extremePoint);
		if (GetVoronoiRegion(ref extremePoint) != VoronoiRegion.ABC)
		{
			state = CollisionState.ExternalSeparated;
			return DoExternalSeparated(out contactList);
		}
		Vector3.Dot(ref extremePoint, ref result3, out var result5);
		float num = (result4 - result5) / result3.LengthSquared();
		Vector3.Multiply(ref result3, num, out var result6);
		float num2 = result6.LengthSquared();
		float num3 = triangle.collisionMargin + convex.collisionMargin;
		if (num <= 0f || num2 < num3 * num3)
		{
			ContactData item = default(ContactData);
			if (num3 > 1E-07f)
			{
				Vector3.Multiply(ref result6, convex.collisionMargin / num3, out item.Position);
			}
			else
			{
				item.Position = default(Vector3);
			}
			Vector3.Add(ref extremePoint, ref item.Position, out item.Position);
			float num4 = result3.Length();
			Vector3.Divide(ref result3, num4, out item.Normal);
			float num5 = num4 * num;
			item.PenetrationDepth = num3 - num5;
			if (item.PenetrationDepth > num3)
			{
				if (TryInnerSphereContact(out var contact))
				{
					contactList.Add(ref contact);
				}
				CollisionState collisionState = state;
				state = CollisionState.ExternalNear;
				if (!DoExternalNear(out var contactList2))
				{
					state = collisionState;
					return false;
				}
				contactList2.Get(0, out contact);
				if (contact.PenetrationDepth + 0.01f < item.PenetrationDepth)
				{
					contactList.Add(ref contact);
				}
				else
				{
					contactList.Add(ref item);
					state = collisionState;
				}
			}
			else
			{
				contactList.Add(ref item);
			}
			return true;
		}
		return false;
	}

	private bool DoExternalSeparated(out TinyStructList<ContactData> contactList)
	{
		if (GJKToolbox.AreShapesIntersecting(convex, triangle, ref Toolbox.RigidIdentity, ref Toolbox.RigidIdentity, ref localSeparatingAxis))
		{
			state = CollisionState.ExternalNear;
			return DoExternalNear(out contactList);
		}
		TryToEscape();
		contactList = default(TinyStructList<ContactData>);
		return false;
	}

	private bool DoExternalNear(out TinyStructList<ContactData> contactList)
	{
		Vector3.Add(ref triangle.vA, ref triangle.vB, out var result);
		Vector3.Add(ref result, ref triangle.vC, out result);
		Vector3.Multiply(ref result, 1f / 3f, out result);
		CachedSimplex cachedSimplex = new CachedSimplex
		{
			State = SimplexState.Point,
			LocalSimplexB = 
			{
				A = result
			}
		};
		if (GJKToolbox.GetClosestPoints(convex, triangle, ref Toolbox.RigidIdentity, ref Toolbox.RigidIdentity, ref cachedSimplex, out var closestPointA, out var closestPointB))
		{
			state = CollisionState.Deep;
			return DoDeepContact(out contactList);
		}
		Vector3.Subtract(ref closestPointB, ref closestPointA, out var result2);
		float num = result2.LengthSquared();
		float num2 = convex.collisionMargin + triangle.collisionMargin;
		contactList = default(TinyStructList<ContactData>);
		if (num < num2 * num2)
		{
			ContactData item = default(ContactData);
			if (triangle.sidedness != TriangleSidedness.DoubleSided)
			{
				Vector3.Subtract(ref triangle.vB, ref triangle.vA, out var result3);
				Vector3.Subtract(ref triangle.vC, ref triangle.vA, out var result4);
				Vector3.Cross(ref result3, ref result4, out var result5);
				Vector3.Dot(ref result5, ref result2, out var result6);
				if (triangle.sidedness == TriangleSidedness.Clockwise && result6 > 0f)
				{
					return false;
				}
				if (triangle.sidedness == TriangleSidedness.Counterclockwise && result6 < 0f)
				{
					return false;
				}
			}
			if (num2 > 1E-07f)
			{
				Vector3.Multiply(ref result2, convex.collisionMargin / num2, out item.Position);
			}
			else
			{
				item.Position = default(Vector3);
			}
			Vector3.Add(ref closestPointA, ref item.Position, out item.Position);
			item.Normal = result2;
			float num3 = (float)Math.Sqrt(num);
			Vector3.Divide(ref item.Normal, num3, out item.Normal);
			item.PenetrationDepth = num2 - num3;
			contactList.Add(ref item);
			TryToEscape(ref item.Position);
			return true;
		}
		state = CollisionState.ExternalSeparated;
		return false;
	}

	private bool DoDeepContact(out TinyStructList<ContactData> contactList)
	{
		Vector3.Add(ref triangle.vA, ref triangle.vB, out var result);
		Vector3.Add(ref result, ref triangle.vC, out result);
		Vector3.Multiply(ref result, 1f / 3f, out result);
		contactList = default(TinyStructList<ContactData>);
		ContactData item = default(ContactData);
		if (MPRToolbox.AreLocalShapesOverlapping(convex, triangle, ref result, ref Toolbox.RigidIdentity))
		{
			Vector3.Subtract(ref triangle.vB, ref triangle.vA, out var result2);
			Vector3.Subtract(ref triangle.vC, ref triangle.vA, out var result3);
			Vector3.Cross(ref result2, ref result3, out var result4);
			float num = result4.LengthSquared();
			float result11;
			if (num < 1E-09f)
			{
				MPRToolbox.LocalSurfaceCast(convex, triangle, ref Toolbox.RigidIdentity, ref result, out item.PenetrationDepth, out item.Normal, out item.Position);
			}
			else
			{
				Vector3.Divide(ref result4, (float)Math.Sqrt(num), out result4);
				Vector3.Subtract(ref result, ref triangle.vA, out var result5);
				Vector3.Subtract(ref result, ref triangle.vB, out var result6);
				Vector3.Subtract(ref result, ref triangle.vC, out var result7);
				Vector3.Subtract(ref triangle.vB, ref triangle.vA, out var result8);
				Vector3.Subtract(ref triangle.vC, ref triangle.vB, out var result9);
				Vector3.Subtract(ref triangle.vA, ref triangle.vC, out var result10);
				Vector3.Dot(ref result5, ref result8, out result11);
				Vector3.Multiply(ref result8, result11 / result8.LengthSquared(), out var result12);
				Vector3.Subtract(ref result5, ref result12, out result12);
				result12.Normalize();
				Vector3.Dot(ref result6, ref result9, out result11);
				Vector3.Multiply(ref result9, result11 / result9.LengthSquared(), out var result13);
				Vector3.Subtract(ref result6, ref result13, out result13);
				result13.Normalize();
				Vector3.Dot(ref result7, ref result10, out result11);
				Vector3.Multiply(ref result10, result11 / result10.LengthSquared(), out var result14);
				Vector3.Subtract(ref result7, ref result14, out result14);
				result14.Normalize();
				MPRToolbox.LocalSurfaceCast(convex, triangle, ref Toolbox.RigidIdentity, ref result12, out item.PenetrationDepth, out item.Normal);
				Vector3.Dot(ref result4, ref item.Normal, out result11);
				if ((triangle.sidedness == TriangleSidedness.Clockwise && result11 > 0f) || (triangle.sidedness == TriangleSidedness.Counterclockwise && result11 < 0f))
				{
					Vector3 vector = item.Normal;
					Vector3.Dot(ref item.Normal, ref result4, out result11);
					Vector3.Multiply(ref item.Normal, result11, out var result15);
					Vector3.Subtract(ref item.Normal, ref result15, out item.Normal);
					float num2 = item.Normal.LengthSquared();
					if (num2 > 1E-07f)
					{
						Vector3.Divide(ref item.Normal, (float)Math.Sqrt(num2), out item.Normal);
						Vector3.Dot(ref item.Normal, ref vector, out result11);
						item.PenetrationDepth *= result11;
					}
					else
					{
						item.PenetrationDepth = float.MaxValue;
						item.Normal = default(Vector3);
					}
				}
				MPRToolbox.LocalSurfaceCast(convex, triangle, ref Toolbox.RigidIdentity, ref result13, out var t, out var normal);
				Vector3.Dot(ref result4, ref normal, out result11);
				if ((triangle.sidedness == TriangleSidedness.Clockwise && result11 > 0f) || (triangle.sidedness == TriangleSidedness.Counterclockwise && result11 < 0f))
				{
					Vector3 vector2 = normal;
					Vector3.Dot(ref normal, ref result4, out result11);
					Vector3.Multiply(ref normal, result11, out var result16);
					Vector3.Subtract(ref normal, ref result16, out normal);
					float num3 = normal.LengthSquared();
					if (num3 > 1E-07f)
					{
						Vector3.Divide(ref normal, (float)Math.Sqrt(num3), out normal);
						Vector3.Dot(ref normal, ref vector2, out result11);
						t *= result11;
					}
					else
					{
						item.PenetrationDepth = float.MaxValue;
						item.Normal = default(Vector3);
					}
				}
				if (t < item.PenetrationDepth)
				{
					item.Normal = normal;
					item.PenetrationDepth = t;
				}
				MPRToolbox.LocalSurfaceCast(convex, triangle, ref Toolbox.RigidIdentity, ref result14, out t, out normal);
				Vector3.Dot(ref result4, ref normal, out result11);
				if ((triangle.sidedness == TriangleSidedness.Clockwise && result11 > 0f) || (triangle.sidedness == TriangleSidedness.Counterclockwise && result11 < 0f))
				{
					Vector3 vector3 = normal;
					Vector3.Dot(ref normal, ref result4, out result11);
					Vector3.Multiply(ref normal, result11, out var result17);
					Vector3.Subtract(ref normal, ref result17, out normal);
					float num4 = normal.LengthSquared();
					if (num4 > 1E-07f)
					{
						Vector3.Divide(ref normal, (float)Math.Sqrt(num4), out normal);
						Vector3.Dot(ref normal, ref vector3, out result11);
						t *= result11;
					}
					else
					{
						item.PenetrationDepth = float.MaxValue;
						item.Normal = default(Vector3);
					}
				}
				if (t < item.PenetrationDepth)
				{
					item.Normal = normal;
					item.PenetrationDepth = t;
				}
				if (triangle.sidedness != TriangleSidedness.Clockwise)
				{
					MPRToolbox.LocalSurfaceCast(convex, triangle, ref Toolbox.RigidIdentity, ref result4, out t, out normal);
					if (t < item.PenetrationDepth)
					{
						item.Normal = normal;
						item.PenetrationDepth = t;
					}
				}
				if (triangle.sidedness != TriangleSidedness.Counterclockwise)
				{
					Vector3.Negate(ref result4, out result4);
					MPRToolbox.LocalSurfaceCast(convex, triangle, ref Toolbox.RigidIdentity, ref result4, out t, out normal);
					if (t < item.PenetrationDepth)
					{
						item.Normal = normal;
						item.PenetrationDepth = t;
					}
				}
			}
			MPRToolbox.RefinePenetration(convex, triangle, ref Toolbox.RigidIdentity, item.PenetrationDepth, ref item.Normal, out item.PenetrationDepth, out item.Normal, out item.Position);
			if (triangle.sidedness != TriangleSidedness.DoubleSided)
			{
				Vector3.Dot(ref result4, ref item.Normal, out result11);
				if (result11 < 0f)
				{
					goto IL_0625;
				}
			}
			item.Id = -1;
			if (item.PenetrationDepth < convex.collisionMargin + triangle.collisionMargin)
			{
				state = CollisionState.ExternalNear;
			}
			contactList.Add(ref item);
		}
		goto IL_0625;
		IL_0625:
		if (TryInnerSphereContact(out item))
		{
			contactList.Add(ref item);
		}
		if (contactList.count > 0)
		{
			return true;
		}
		state = CollisionState.ExternalSeparated;
		return false;
	}

	private void TryToEscape()
	{
		if (++escapeAttempts == 10)
		{
			escapeAttempts = 0;
			state = CollisionState.Plane;
		}
	}

	private void TryToEscape(ref Vector3 position)
	{
		if (++escapeAttempts == 10 && GetVoronoiRegion(ref position) == VoronoiRegion.ABC)
		{
			escapeAttempts = 0;
			state = CollisionState.Plane;
		}
	}

	private bool TryInnerSphereContact(out ContactData contact)
	{
		Toolbox.GetClosestPointOnTriangleToPoint(ref triangle.vA, ref triangle.vB, ref triangle.vC, ref Toolbox.ZeroVector, out var closestPoint);
		float num = closestPoint.LengthSquared();
		float num2 = convex.minimumRadius * (MotionSettings.CoreShapeScaling + 0.01f);
		if (num < num2 * num2)
		{
			Vector3.Subtract(ref triangle.vB, ref triangle.vA, out var result);
			Vector3.Subtract(ref triangle.vC, ref triangle.vA, out var result2);
			Vector3.Cross(ref result, ref result2, out var result3);
			Vector3.Dot(ref closestPoint, ref result3, out var result4);
			if ((triangle.sidedness == TriangleSidedness.Clockwise && result4 > 0f) || (triangle.sidedness == TriangleSidedness.Counterclockwise && result4 < 0f))
			{
				contact = default(ContactData);
				return false;
			}
			num = (float)Math.Sqrt(num);
			contact.Position = closestPoint;
			if (num > 1E-07f)
			{
				Vector3.Divide(ref closestPoint, num, out contact.Normal);
			}
			else
			{
				float num3 = result3.LengthSquared();
				if (!(result3.LengthSquared() > 1E-07f))
				{
					contact = default(ContactData);
					return false;
				}
				Vector3.Divide(ref result3, (float)Math.Sqrt(num3), out result3);
				if (triangle.sidedness == TriangleSidedness.Clockwise)
				{
					contact.Normal = result3;
				}
				else
				{
					Vector3.Negate(ref result3, out contact.Normal);
				}
			}
			MPRToolbox.LocalSurfaceCast(convex, triangle, ref Toolbox.RigidIdentity, ref contact.Normal, out contact.PenetrationDepth, out result3);
			contact.Id = -1;
			return true;
		}
		contact = default(ContactData);
		return false;
	}

	/// <summary>
	///  Determines what voronoi region a given point is in.
	/// </summary>
	/// <param name="p">Point to test.</param>
	/// <returns>Voronoi region containing the point.</returns>
	private VoronoiRegion GetVoronoiRegion(ref Vector3 p)
	{
		Vector3.Subtract(ref triangle.vB, ref triangle.vA, out var result);
		Vector3.Subtract(ref triangle.vC, ref triangle.vA, out var result2);
		Vector3.Subtract(ref p, ref triangle.vA, out var result3);
		Vector3.Dot(ref result3, ref result, out var result4);
		Vector3.Dot(ref result3, ref result2, out var result5);
		if (result5 <= 0f && result4 <= 0f)
		{
			return VoronoiRegion.A;
		}
		Vector3.Subtract(ref p, ref triangle.vB, out var result6);
		Vector3.Dot(ref result, ref result6, out var result7);
		Vector3.Dot(ref result2, ref result6, out var result8);
		if (result7 >= 0f && result8 <= result7)
		{
			return VoronoiRegion.B;
		}
		float num = result4 * result8 - result7 * result5;
		if (num <= 0f && result4 > 0f && result7 < 0f)
		{
			return VoronoiRegion.AB;
		}
		Vector3.Subtract(ref p, ref triangle.vC, out var result9);
		Vector3.Dot(ref result, ref result9, out var result10);
		Vector3.Dot(ref result2, ref result9, out var result11);
		if (result11 >= 0f && result10 <= result11)
		{
			return VoronoiRegion.C;
		}
		float num2 = result10 * result5 - result4 * result11;
		if (num2 <= 0f && result5 > 0f && result11 < 0f)
		{
			return VoronoiRegion.AC;
		}
		float num3 = result7 * result11 - result10 * result8;
		if (num3 <= 0f && result8 - result7 > 0f && result10 - result11 > 0f)
		{
			return VoronoiRegion.BC;
		}
		return VoronoiRegion.ABC;
	}

	/// <summary>
	///  Initializes the pair tester.
	/// </summary>
	/// <param name="convex">Convex shape to use.</param>
	/// <param name="triangle">Triangle shape to use.</param>
	public override void Initialize(ConvexShape convex, TriangleShape triangle)
	{
		this.convex = convex;
		base.triangle = triangle;
	}

	/// <summary>
	/// Cleans up the pair tester.
	/// </summary>
	public override void CleanUp()
	{
		triangle = null;
		convex = null;
		state = CollisionState.Plane;
		escapeAttempts = 0;
		localSeparatingAxis = default(Vector3);
		Updated = false;
	}

	public override VoronoiRegion GetRegion(ref ContactData contact)
	{
		Vector3.Dot(ref triangle.vA, ref contact.Normal, out var result);
		Vector3.Dot(ref triangle.vB, ref contact.Normal, out var result2);
		Vector3.Dot(ref triangle.vC, ref contact.Normal, out var result3);
		result = 0f - result;
		result2 = 0f - result2;
		result3 = 0f - result3;
		float num = 0.01f;
		Vector3 result4;
		float result5;
		if (result > result2 && result > result3)
		{
			if (result2 > result3)
			{
				if (Math.Abs(result - result3) < num)
				{
					return VoronoiRegion.ABC;
				}
				Vector3.Subtract(ref triangle.vB, ref triangle.vA, out result4);
				Vector3.Dot(ref result4, ref contact.Normal, out result5);
				if (result5 * result5 < result4.LengthSquared() * 0.01f)
				{
					return VoronoiRegion.AB;
				}
				return VoronoiRegion.A;
			}
			if (Math.Abs(result - result2) < num)
			{
				return VoronoiRegion.ABC;
			}
			Vector3.Subtract(ref triangle.vC, ref triangle.vA, out result4);
			Vector3.Dot(ref result4, ref contact.Normal, out result5);
			if (result5 * result5 < result4.LengthSquared() * 0.01f)
			{
				return VoronoiRegion.AC;
			}
			return VoronoiRegion.A;
		}
		if (result2 > result3)
		{
			if (result3 > result)
			{
				if (Math.Abs(result2 - result) < num)
				{
					return VoronoiRegion.ABC;
				}
				Vector3.Subtract(ref triangle.vC, ref triangle.vB, out result4);
				Vector3.Dot(ref result4, ref contact.Normal, out result5);
				if (result5 * result5 < result4.LengthSquared() * 0.01f)
				{
					return VoronoiRegion.BC;
				}
				return VoronoiRegion.B;
			}
			if (Math.Abs(result2 - result3) < num)
			{
				return VoronoiRegion.ABC;
			}
			Vector3.Subtract(ref triangle.vA, ref triangle.vB, out result4);
			Vector3.Dot(ref result4, ref contact.Normal, out result5);
			if (result5 * result5 < result4.LengthSquared() * 0.01f)
			{
				return VoronoiRegion.AB;
			}
			return VoronoiRegion.B;
		}
		if (result > result2)
		{
			if (Math.Abs(result3 - result2) < num)
			{
				return VoronoiRegion.ABC;
			}
			Vector3.Subtract(ref triangle.vA, ref triangle.vC, out result4);
			Vector3.Dot(ref result4, ref contact.Normal, out result5);
			if (result5 * result5 < result4.LengthSquared() * 0.01f)
			{
				return VoronoiRegion.AC;
			}
			return VoronoiRegion.C;
		}
		if (Math.Abs(result3 - result) < num)
		{
			return VoronoiRegion.ABC;
		}
		Vector3.Subtract(ref triangle.vB, ref triangle.vC, out result4);
		Vector3.Dot(ref result4, ref contact.Normal, out result5);
		if (result5 * result5 < result4.LengthSquared() * 0.01f)
		{
			return VoronoiRegion.BC;
		}
		return VoronoiRegion.C;
	}
}
