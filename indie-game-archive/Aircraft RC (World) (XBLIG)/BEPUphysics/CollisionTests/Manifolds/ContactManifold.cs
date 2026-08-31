using System;
using BEPUphysics.Collidables;
using BEPUphysics.DataStructures;
using BEPUphysics.ResourceManagement;

namespace BEPUphysics.CollisionTests.Manifolds;

/// <summary>
///  Superclass of manifolds which manage persistent contacts over multiple frames.
/// </summary>
public abstract class ContactManifold
{
	protected RawList<int> contactIndicesToRemove;

	protected internal RawList<Contact> contacts;

	protected UnsafeResourcePool<Contact> unusedContacts;

	/// <summary>
	///  Gets the contacts in the manifold.
	/// </summary>
	public ReadOnlyList<Contact> Contacts => new ReadOnlyList<Contact>(contacts);

	/// <summary>
	///  Fires when a contact is added.
	/// </summary>
	public event Action<Contact> ContactAdded;

	/// <summary>
	///  Fires when a contact is removed.
	/// </summary>
	public event Action<Contact> ContactRemoved;

	protected void RemoveQueuedContacts()
	{
		for (int num = contactIndicesToRemove.count - 1; num >= 0; num--)
		{
			Remove(contactIndicesToRemove.Elements[num]);
		}
		contactIndicesToRemove.Clear();
	}

	protected virtual void Remove(int contactIndex)
	{
		Contact contact = contacts.Elements[contactIndex];
		contacts.FastRemoveAt(contactIndex);
		OnRemoved(contact);
		unusedContacts.GiveBack(contact);
	}

	protected virtual void Add(ref ContactData contactCandidate)
	{
		Contact contact = unusedContacts.Take();
		contact.Setup(ref contactCandidate);
		contacts.Add(contact);
		OnAdded(contact);
	}

	protected void OnAdded(Contact contact)
	{
		if (ContactAdded != null)
		{
			ContactAdded(contact);
		}
	}

	protected void OnRemoved(Contact contact)
	{
		if (ContactRemoved != null)
		{
			ContactRemoved(contact);
		}
	}

	/// <summary>
	///  Initializes the manifold.
	/// </summary>
	/// <param name="newCollidableA">First collidable.</param>
	/// <param name="newCollidableB">Second collidable.</param>
	public abstract void Initialize(Collidable newCollidableA, Collidable newCollidableB);

	/// <summary>
	///  Cleans up the manifold.
	/// </summary>
	public virtual void CleanUp()
	{
	}

	/// <summary>
	///  Updates the manifold.
	/// </summary>
	/// <param name="dt">Timestep duration.</param>
	public abstract void Update(float dt);

	/// <summary>
	/// Clears the contacts associated with this manifold.
	/// </summary>
	public virtual void ClearContacts()
	{
		for (int num = contacts.count - 1; num >= 0; num--)
		{
			Remove(num);
		}
	}
}
