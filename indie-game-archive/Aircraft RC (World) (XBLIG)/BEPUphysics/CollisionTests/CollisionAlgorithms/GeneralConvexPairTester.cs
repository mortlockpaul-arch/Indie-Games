using System;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionTests.CollisionAlgorithms.GJK;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionTests.CollisionAlgorithms;

/// <summary>
///  Tests convex shapes against other convex shapes for contact generation.
/// </summary>
public class GeneralConvexPairTester
{
	private enum CollisionState
	{
		Separated,
		ShallowContact,
		DeepContact
	}

	/// <summary>
	///  Whether or not to use simplex caching in general case convex-convex collisions.
	///  This will improve performance in simulations relying on the general case system, 
	///  but may decrease quality of behavior for curved shapes.
	/// </summary>
	public static bool UseSimplexCaching;

	private CollisionState state;

	private CollisionState previousState;

	private Vector3 localSeparatingAxis;

	private CachedSimplex cachedSimplex;

	protected internal ConvexCollidable collidableA;

	protected internal ConvexCollidable collidableB;

	private Vector3 localDirection;

	/// <summary>
	///  Gets the first collidable in the pair.
	/// </summary>
	public ConvexCollidable CollidableA => collidableA;

	/// <summary>
	///  Gets the second collidable in the pair.
	/// </summary>
	public ConvexCollidable CollidableB => collidableB;

	/// <summary>
	///  Generates a contact between the objects, if possible.
	/// </summary>
	/// <param name="contact">Contact created between the pair, if possible.</param>
	/// <returns>Whether or not the objects were colliding.</returns>
	public bool GenerateContactCandidate(out ContactData contact)
	{
		previousState = state;
		switch (state)
		{
		case CollisionState.Separated:
			if (GJKToolbox.AreShapesIntersecting(collidableA.Shape, collidableB.Shape, ref collidableA.worldTransform, ref collidableB.worldTransform, ref localSeparatingAxis))
			{
				state = CollisionState.ShallowContact;
				return DoShallowContact(out contact);
			}
			contact = default(ContactData);
			return false;
		case CollisionState.ShallowContact:
			return DoShallowContact(out contact);
		case CollisionState.DeepContact:
			return DoDeepContact(out contact);
		default:
			contact = default(ContactData);
			return false;
		}
	}

	private bool DoShallowContact(out ContactData contact)
	{
		bool closestPoints;
		Vector3 closestPointA;
		Vector3 closestPointB;
		if (UseSimplexCaching)
		{
			closestPoints = GJKToolbox.GetClosestPoints(collidableA.Shape, collidableB.Shape, ref collidableA.worldTransform, ref collidableB.worldTransform, ref this.cachedSimplex, out closestPointA, out closestPointB);
		}
		else
		{
			CachedSimplex cachedSimplex = this.cachedSimplex;
			closestPoints = GJKToolbox.GetClosestPoints(collidableA.Shape, collidableB.Shape, ref collidableA.worldTransform, ref collidableB.worldTransform, ref cachedSimplex, out closestPointA, out closestPointB);
		}
		Vector3.Subtract(ref closestPointB, ref closestPointA, out var result);
		if (closestPoints)
		{
			state = CollisionState.DeepContact;
			return DoDeepContact(out contact);
		}
		localDirection = result;
		float num = result.LengthSquared();
		float num2 = collidableA.Shape.collisionMargin + collidableB.Shape.collisionMargin;
		if (num < num2 * num2)
		{
			contact = default(ContactData);
			if (num2 > 1E-07f)
			{
				Vector3.Multiply(ref result, collidableA.Shape.collisionMargin / num2, out contact.Position);
			}
			else
			{
				contact.Position = default(Vector3);
			}
			Vector3.Add(ref closestPointA, ref contact.Position, out contact.Position);
			contact.Normal = result;
			float num3 = (float)Math.Sqrt(num);
			Vector3.Divide(ref contact.Normal, num3, out contact.Normal);
			contact.PenetrationDepth = num2 - num3;
			return true;
		}
		state = CollisionState.Separated;
		contact = default(ContactData);
		return false;
	}

	private bool DoDeepContact(out ContactData contact)
	{
		if (previousState == CollisionState.Separated)
		{
			if (collidableA.entity != null && collidableB.entity != null)
			{
				Vector3.Subtract(ref collidableA.entity.linearVelocity, ref collidableB.entity.linearVelocity, out localDirection);
			}
			else
			{
				localDirection = localSeparatingAxis;
			}
			if (localDirection.LengthSquared() < 1E-07f)
			{
				localDirection = Vector3.Up;
			}
		}
		if (MPRToolbox.GetContact(collidableA.Shape, collidableB.Shape, ref collidableA.worldTransform, ref collidableB.worldTransform, ref localDirection, out contact))
		{
			if (contact.PenetrationDepth < collidableA.Shape.collisionMargin + collidableB.Shape.collisionMargin)
			{
				state = CollisionState.ShallowContact;
			}
			return true;
		}
		state = CollisionState.Separated;
		return false;
	}

	/// <summary>
	///  Initializes the pair tester.
	/// </summary>
	/// <param name="shapeA">First shape in the pair.</param>
	/// <param name="shapeB">Second shape in the pair.</param>
	public void Initialize(Collidable shapeA, Collidable shapeB)
	{
		collidableA = (ConvexCollidable)shapeA;
		collidableB = (ConvexCollidable)shapeB;
		cachedSimplex = new CachedSimplex
		{
			State = SimplexState.Point
		};
	}

	/// <summary>
	///  Cleans up the pair tester.
	/// </summary>
	public void CleanUp()
	{
		state = CollisionState.Separated;
		previousState = CollisionState.Separated;
		cachedSimplex = default(CachedSimplex);
		localSeparatingAxis = default(Vector3);
		collidableA = null;
		collidableB = null;
	}
}
