using System;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.DataStructures;

namespace BEPUphysics.CollisionTests.Manifolds;

/// <summary>
///  Manages persistent contact data between two boxes.
/// </summary>
public class BoxSphereContactManifold : ContactManifold
{
	protected ConvexCollidable<BoxShape> box;

	protected ConvexCollidable<SphereShape> sphere;

	private Contact contact = new Contact();

	private bool previouslyColliding;

	/// <summary>
	///  Gets the first collidable in the pair.
	/// </summary>
	public ConvexCollidable<BoxShape> CollidableA => box;

	/// <summary>
	/// Gets the second collidable in the pair.
	/// </summary>
	public ConvexCollidable<SphereShape> CollidableB => sphere;

	/// <summary>
	///  Constructs a new manifold.
	/// </summary>
	public BoxSphereContactManifold()
	{
		contacts = new RawList<Contact>(1);
	}

	/// <summary>
	///  Updates the manifold.
	/// </summary>
	/// <param name="dt">Timestep duration.</param>
	public override void Update(float dt)
	{
		bool flag = false;
		if (BoxSphereTester.AreShapesColliding(box.Shape, sphere.Shape, ref box.worldTransform, ref sphere.worldTransform.Position, out var contactCandidate))
		{
			if (!previouslyColliding && contactCandidate.PenetrationDepth >= 0f)
			{
				Add(ref contactCandidate);
				flag = true;
			}
			else if (previouslyColliding)
			{
				contact.Normal = contactCandidate.Normal;
				contact.PenetrationDepth = contactCandidate.PenetrationDepth;
				contact.Position = contactCandidate.Position;
				flag = true;
			}
		}
		else if (previouslyColliding)
		{
			Remove(0);
		}
		previouslyColliding = flag;
	}

	protected override void Add(ref ContactData contactCandidate)
	{
		contact.Normal = contactCandidate.Normal;
		contact.PenetrationDepth = contactCandidate.PenetrationDepth;
		contact.Position = contactCandidate.Position;
		contacts.Add(contact);
		OnAdded(contact);
	}

	protected override void Remove(int index)
	{
		contacts.RemoveAt(index);
		OnRemoved(contact);
	}

	/// <summary>
	///  Initializes the manifold.
	/// </summary>
	/// <param name="newCollidableA">First collidable.</param>
	/// <param name="newCollidableB">Second collidable.</param>
	/// <exception cref="T:System.Exception">Thrown when the collidables being used are not of the proper type.</exception>
	public override void Initialize(Collidable newCollidableA, Collidable newCollidableB)
	{
		box = newCollidableA as ConvexCollidable<BoxShape>;
		sphere = newCollidableB as ConvexCollidable<SphereShape>;
		if (box == null || sphere == null)
		{
			box = newCollidableB as ConvexCollidable<BoxShape>;
			sphere = newCollidableA as ConvexCollidable<SphereShape>;
			if (box == null || sphere == null)
			{
				throw new Exception("Inappropriate types used to initialize pair.");
			}
		}
	}

	/// <summary>
	///  Cleans up the manifold.
	/// </summary>
	public override void CleanUp()
	{
		contacts.Clear();
		box = null;
		sphere = null;
		previouslyColliding = false;
		base.CleanUp();
	}

	/// <summary>
	/// Clears the contacts associated with this manifold.
	/// </summary>
	public override void ClearContacts()
	{
		previouslyColliding = false;
		base.ClearContacts();
	}
}
