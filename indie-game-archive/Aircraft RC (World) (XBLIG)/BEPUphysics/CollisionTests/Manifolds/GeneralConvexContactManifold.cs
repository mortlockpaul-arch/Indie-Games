using System;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using BEPUphysics.ResourceManagement;
using BEPUphysics.Settings;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionTests.Manifolds;

/// <summary>
///  Manages persistent contacts for two convex collidables.
/// </summary>
public class GeneralConvexContactManifold : ContactManifold
{
	private RawValueList<ContactSupplementData> supplementData = new RawValueList<ContactSupplementData>(4);

	private GeneralConvexPairTester pairTester;

	protected ConvexCollidable collidableA;

	protected ConvexCollidable collidableB;

	/// <summary>
	///  Gets the pair tester used by the manifold to do testing.
	/// </summary>
	public GeneralConvexPairTester PairTester => pairTester;

	/// <summary>
	///  Gets the first collidable in the pair.
	/// </summary>
	public ConvexCollidable CollidableA => collidableA;

	/// <summary>
	/// Gets the second collidable in the pair.
	/// </summary>
	public ConvexCollidable CollidableB => collidableB;

	/// <summary>
	///  Constructs a new convex-convex manifold.
	/// </summary>
	public GeneralConvexContactManifold()
	{
		contacts = new RawList<Contact>(4);
		unusedContacts = new UnsafeResourcePool<Contact>(4);
		contactIndicesToRemove = new RawList<int>(4);
		pairTester = new GeneralConvexPairTester();
	}

	/// <summary>
	///  Updates the manifold.
	/// </summary>
	/// <param name="dt">Timestep duration.</param>
	public override void Update(float dt)
	{
		ContactRefresher.ContactRefresh(contacts, supplementData, ref collidableA.worldTransform, ref collidableB.worldTransform, contactIndicesToRemove);
		RemoveQueuedContacts();
		if (pairTester.GenerateContactCandidate(out var contact))
		{
			if (!IsContactUnique(ref contact))
			{
				return;
			}
			if (contacts.count == 4)
			{
				ContactReducer.ReduceContacts(contacts, ref contact, contactIndicesToRemove, out var addCandidate);
				RemoveQueuedContacts();
				if (addCandidate)
				{
					Add(ref contact);
				}
			}
			else
			{
				Add(ref contact);
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
		RigidTransform.TransformByInverse(ref contactCandidate.Position, ref collidableA.worldTransform, out item.LocalOffsetA);
		RigidTransform.TransformByInverse(ref contactCandidate.Position, ref collidableB.worldTransform, out item.LocalOffsetB);
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
				RigidTransform.TransformByInverse(ref contactCandidate.Position, ref collidableA.worldTransform, out supplementData.Elements[i].LocalOffsetA);
				RigidTransform.TransformByInverse(ref contactCandidate.Position, ref collidableB.worldTransform, out supplementData.Elements[i].LocalOffsetB);
				return false;
			}
		}
		return true;
	}

	/// <summary>
	///  Initializes the manifold.
	/// </summary>
	/// <param name="newCollidableA">First collidable.</param>
	/// <param name="newCollidableB">Second collidable.</param>
	public override void Initialize(Collidable newCollidableA, Collidable newCollidableB)
	{
		collidableA = newCollidableA as ConvexCollidable;
		collidableB = newCollidableB as ConvexCollidable;
		pairTester.Initialize(newCollidableA, newCollidableB);
		if (collidableA == null || collidableB == null)
		{
			throw new Exception("Inappropriate types used to initialize pair tester.");
		}
	}

	/// <summary>
	///  Cleans up the manifold.
	/// </summary>
	public override void CleanUp()
	{
		supplementData.Clear();
		contacts.Clear();
		collidableA = null;
		collidableB = null;
		pairTester.CleanUp();
		base.CleanUp();
	}
}
