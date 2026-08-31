using System;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.DataStructures;
using BEPUphysics.ResourceManagement;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionTests.Manifolds;

/// <summary>
///  Manages persistent contact data between two boxes.
/// </summary>
public class BoxContactManifold : ContactManifold
{
	protected ConvexCollidable<BoxShape> boxA;

	protected ConvexCollidable<BoxShape> boxB;

	/// <summary>
	///  Gets the first collidable in the pair.
	/// </summary>
	public ConvexCollidable<BoxShape> CollidableA => boxA;

	/// <summary>
	/// Gets the second collidable in the pair.
	/// </summary>
	public ConvexCollidable<BoxShape> CollidableB => boxB;

	/// <summary>
	///  Constructs a new manifold.
	/// </summary>
	public BoxContactManifold()
	{
		contacts = new RawList<Contact>(4);
		unusedContacts = new UnsafeResourcePool<Contact>(4);
		contactIndicesToRemove = new RawList<int>(4);
	}

	/// <summary>
	///  Updates the manifold.
	/// </summary>
	/// <param name="dt">Timestep duration.</param>
	public unsafe override void Update(float dt)
	{
		if (BoxBoxCollider.AreBoxesColliding(boxA.Shape, boxB.Shape, ref boxA.worldTransform, ref boxB.worldTransform, out float _, out Vector3 axis, out BoxContactDataCache contactData))
		{
			BoxContactData* ptr = &contactData.D1;
			Vector3.Negate(ref axis, out axis);
			TinyList<int> tinyList = default(TinyList<int>);
			for (int i = 0; i < contacts.count; i++)
			{
				bool flag = false;
				for (int num = contactData.Count - 1; num >= 0; num--)
				{
					if (contacts.Elements[i].Id == ptr[num].Id)
					{
						flag = true;
						contacts.Elements[i].Position = ptr[num].Position;
						contacts.Elements[i].PenetrationDepth = 0f - ptr[num].Depth;
						contacts.Elements[i].Normal = axis;
						contactData.RemoveAt(num);
						break;
					}
				}
				if (!flag)
				{
					tinyList.Add(i);
				}
			}
			for (int num2 = tinyList.Count - 1; num2 >= 0; num2--)
			{
				Remove(tinyList[num2]);
			}
			for (int j = 0; j < contactData.Count; j++)
			{
				ContactData contactCandidate = new ContactData
				{
					Position = ptr[j].Position,
					PenetrationDepth = 0f - ptr[j].Depth,
					Normal = axis,
					Id = ptr[j].Id
				};
				Add(ref contactCandidate);
			}
		}
		else
		{
			for (int num3 = contacts.count - 1; num3 >= 0; num3--)
			{
				Remove(num3);
			}
		}
	}

	/// <summary>
	///  Initializes the manifold.
	/// </summary>
	/// <param name="newCollidableA">First collidable.</param>
	/// <param name="newCollidableB">Second collidable.</param>
	/// <exception cref="T:System.Exception">Thrown when the collidables being used are not of the proper type.</exception>
	public override void Initialize(Collidable newCollidableA, Collidable newCollidableB)
	{
		boxA = (ConvexCollidable<BoxShape>)newCollidableA;
		boxB = (ConvexCollidable<BoxShape>)newCollidableB;
		if (boxA == null || boxB == null)
		{
			throw new Exception("Inappropriate types used to initialize pair tester.");
		}
	}

	/// <summary>
	///  Cleans up the manifold.
	/// </summary>
	public override void CleanUp()
	{
		contacts.Clear();
		boxA = null;
		boxB = null;
		base.CleanUp();
	}
}
