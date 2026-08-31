using System;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using BEPUphysics.ResourceManagement;
using BEPUphysics.Settings;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionTests.Manifolds;

/// <summary>
///  Manages persistent contacts between a triangle and convex.
/// </summary>
public class TriangleConvexContactManifold : ContactManifold
{
	private RawValueList<ContactSupplementData> supplementData = new RawValueList<ContactSupplementData>(4);

	private TriangleConvexPairTester pairTester;

	private TriangleShape localTriangleShape = new TriangleShape();

	protected ConvexCollidable convex;

	protected ConvexCollidable<TriangleShape> triangle;

	/// <summary>
	///  Gets the pair tester used by the manifold.
	/// </summary>
	public TriangleConvexPairTester PairTester => pairTester;

	/// <summary>
	///  Gets the convex associated with the pair.
	/// </summary>
	public ConvexCollidable Convex => convex;

	/// <summary>
	///  Gets the triangle associated with the pair.
	/// </summary>
	public ConvexCollidable<TriangleShape> Triangle => triangle;

	/// <summary>
	///  Constructs a new manifold.
	/// </summary>
	public TriangleConvexContactManifold()
	{
		contacts = new RawList<Contact>(4);
		unusedContacts = new UnsafeResourcePool<Contact>(4);
		contactIndicesToRemove = new RawList<int>(4);
		pairTester = new TriangleConvexPairTester();
	}

	public override void Update(float dt)
	{
		ContactRefresher.ContactRefresh(contacts, supplementData, ref convex.worldTransform, ref triangle.worldTransform, contactIndicesToRemove);
		RemoveQueuedContacts();
		localTriangleShape.collisionMargin = triangle.Shape.collisionMargin;
		localTriangleShape.sidedness = triangle.Shape.sidedness;
		Matrix3X3.CreateFromQuaternion(ref triangle.worldTransform.Orientation, out var result);
		Matrix3X3.Transform(ref triangle.Shape.vA, ref result, out localTriangleShape.vA);
		Matrix3X3.Transform(ref triangle.Shape.vB, ref result, out localTriangleShape.vB);
		Matrix3X3.Transform(ref triangle.Shape.vC, ref result, out localTriangleShape.vC);
		Vector3.Add(ref localTriangleShape.vA, ref triangle.worldTransform.Position, out localTriangleShape.vA);
		Vector3.Add(ref localTriangleShape.vB, ref triangle.worldTransform.Position, out localTriangleShape.vB);
		Vector3.Add(ref localTriangleShape.vC, ref triangle.worldTransform.Position, out localTriangleShape.vC);
		Vector3.Subtract(ref localTriangleShape.vA, ref convex.worldTransform.Position, out localTriangleShape.vA);
		Vector3.Subtract(ref localTriangleShape.vB, ref convex.worldTransform.Position, out localTriangleShape.vB);
		Vector3.Subtract(ref localTriangleShape.vC, ref convex.worldTransform.Position, out localTriangleShape.vC);
		Matrix3X3.CreateFromQuaternion(ref convex.worldTransform.Orientation, out result);
		Matrix3X3.TransformTranspose(ref localTriangleShape.vA, ref result, out localTriangleShape.vA);
		Matrix3X3.TransformTranspose(ref localTriangleShape.vB, ref result, out localTriangleShape.vB);
		Matrix3X3.TransformTranspose(ref localTriangleShape.vC, ref result, out localTriangleShape.vC);
		if (pairTester.GenerateContactCandidate(out var contactList))
		{
			for (int i = 0; i < contactList.count; i++)
			{
				contactList.Get(i, out var item);
				Matrix3X3.Transform(ref item.Position, ref result, out item.Position);
				Vector3.Add(ref item.Position, ref convex.worldTransform.Position, out item.Position);
				Matrix3X3.Transform(ref item.Normal, ref result, out item.Normal);
				if (!IsContactUnique(ref item))
				{
					continue;
				}
				if (contacts.count == 4)
				{
					ContactReducer.ReduceContacts(contacts, ref item, contactIndicesToRemove, out var addCandidate);
					RemoveQueuedContacts();
					if (addCandidate)
					{
						Add(ref item);
					}
				}
				else
				{
					Add(ref item);
				}
			}
		}
		else
		{
			for (int num = contacts.count - 1; num >= 0; num--)
			{
				Remove(num);
			}
		}
	}

	protected override void Add(ref ContactData contactCandidate)
	{
		ContactSupplementData item = default(ContactSupplementData);
		item.BasePenetrationDepth = contactCandidate.PenetrationDepth;
		RigidTransform.TransformByInverse(ref contactCandidate.Position, ref convex.worldTransform, out item.LocalOffsetA);
		RigidTransform.TransformByInverse(ref contactCandidate.Position, ref triangle.worldTransform, out item.LocalOffsetB);
		supplementData.Add(ref item);
		base.Add(ref contactCandidate);
	}

	protected override void Remove(int contactIndex)
	{
		supplementData.RemoveAt(contactIndex);
		base.Remove(contactIndex);
	}

	private bool IsContactUnique(ref ContactData contactCandidate)
	{
		for (int i = 0; i < contacts.count; i++)
		{
			Vector3.DistanceSquared(ref contacts.Elements[i].Position, ref contactCandidate.Position, out var result);
			if (result < CollisionDetectionSettings.ContactMinimumSeparationDistanceSquared)
			{
				contacts.Elements[i].Normal = contactCandidate.Normal;
				contacts.Elements[i].Position = contactCandidate.Position;
				contacts.Elements[i].PenetrationDepth = contactCandidate.PenetrationDepth;
				supplementData.Elements[i].BasePenetrationDepth = contactCandidate.PenetrationDepth;
				RigidTransform.TransformByInverse(ref contactCandidate.Position, ref convex.worldTransform, out supplementData.Elements[i].LocalOffsetA);
				RigidTransform.TransformByInverse(ref contactCandidate.Position, ref triangle.worldTransform, out supplementData.Elements[i].LocalOffsetB);
				return false;
			}
		}
		return true;
	}

	public override void Initialize(Collidable newCollidableA, Collidable newCollidableB)
	{
		convex = newCollidableA as ConvexCollidable;
		triangle = newCollidableB as ConvexCollidable<TriangleShape>;
		if (convex == null || triangle == null)
		{
			convex = newCollidableB as ConvexCollidable;
			triangle = newCollidableA as ConvexCollidable<TriangleShape>;
			if (convex == null || triangle == null)
			{
				throw new Exception("Inappropriate types used to initialize contact manifold.");
			}
		}
		pairTester.Initialize(convex.Shape, localTriangleShape);
	}

	public override void CleanUp()
	{
		supplementData.Clear();
		contacts.Clear();
		convex = null;
		triangle = null;
		pairTester.CleanUp();
		base.CleanUp();
	}
}
