using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics.Contacts;

namespace FarseerPhysics.Dynamics;

public class ContactManager
{
	public BeginContactDelegate BeginContact;

	public BroadPhase BroadPhase = new BroadPhase();

	public int ContactCount;

	public CollisionFilterDelegate ContactFilter;

	public Contact ContactList;

	public EndContactDelegate EndContact;

	public BroadphaseDelegate OnBroadphaseCollision;

	public PostSolveDelegate PostSolve;

	public PreSolveDelegate PreSolve;

	internal ContactManager()
	{
		ContactList = null;
		ContactCount = 0;
		OnBroadphaseCollision = AddPair;
	}

	private void AddPair(ref FixtureProxy proxyA, ref FixtureProxy proxyB)
	{
		Fixture fixture = proxyA.Fixture;
		Fixture fixture2 = proxyB.Fixture;
		int childIndex = proxyA.ChildIndex;
		int childIndex2 = proxyB.ChildIndex;
		Body body = fixture.Body;
		Body body2 = fixture2.Body;
		if (body == body2)
		{
			return;
		}
		for (ContactEdge contactEdge = body2.ContactList; contactEdge != null; contactEdge = contactEdge.Next)
		{
			if (contactEdge.Other == body)
			{
				Fixture fixtureA = contactEdge.Contact.FixtureA;
				Fixture fixtureB = contactEdge.Contact.FixtureB;
				int childIndexA = contactEdge.Contact.ChildIndexA;
				int childIndexB = contactEdge.Contact.ChildIndexB;
				if ((fixtureA == fixture && fixtureB == fixture2 && childIndexA == childIndex && childIndexB == childIndex2) || (fixtureA == fixture2 && fixtureB == fixture && childIndexA == childIndex2 && childIndexB == childIndex))
				{
					return;
				}
			}
		}
		if (body2.ShouldCollide(body) && (ContactFilter == null || ContactFilter(fixture, fixture2)))
		{
			Contact contact = Contact.Create(fixture, childIndex, fixture2, childIndex2);
			fixture = contact.FixtureA;
			fixture2 = contact.FixtureB;
			childIndex = contact.ChildIndexA;
			childIndex2 = contact.ChildIndexB;
			body = fixture.Body;
			body2 = fixture2.Body;
			contact.Prev = null;
			contact.Next = ContactList;
			if (ContactList != null)
			{
				ContactList.Prev = contact;
			}
			ContactList = contact;
			contact.NodeA.Contact = contact;
			contact.NodeA.Other = body2;
			contact.NodeA.Prev = null;
			contact.NodeA.Next = body.ContactList;
			if (body.ContactList != null)
			{
				body.ContactList.Prev = contact.NodeA;
			}
			body.ContactList = contact.NodeA;
			contact.NodeB.Contact = contact;
			contact.NodeB.Other = body;
			contact.NodeB.Prev = null;
			contact.NodeB.Next = body2.ContactList;
			if (body2.ContactList != null)
			{
				body2.ContactList.Prev = contact.NodeB;
			}
			body2.ContactList = contact.NodeB;
			ContactCount++;
		}
	}

	internal void FindNewContacts()
	{
		BroadPhase.UpdatePairs<FixtureProxy>(OnBroadphaseCollision);
	}

	internal void Destroy(Contact contact)
	{
		Fixture fixtureA = contact.FixtureA;
		Fixture fixtureB = contact.FixtureB;
		Body body = fixtureA.Body;
		Body body2 = fixtureB.Body;
		if (EndContact != null && contact.IsTouching())
		{
			EndContact(contact);
		}
		if (contact.Prev != null)
		{
			contact.Prev.Next = contact.Next;
		}
		if (contact.Next != null)
		{
			contact.Next.Prev = contact.Prev;
		}
		if (contact == ContactList)
		{
			ContactList = contact.Next;
		}
		if (contact.NodeA.Prev != null)
		{
			contact.NodeA.Prev.Next = contact.NodeA.Next;
		}
		if (contact.NodeA.Next != null)
		{
			contact.NodeA.Next.Prev = contact.NodeA.Prev;
		}
		if (contact.NodeA == body.ContactList)
		{
			body.ContactList = contact.NodeA.Next;
		}
		if (contact.NodeB.Prev != null)
		{
			contact.NodeB.Prev.Next = contact.NodeB.Next;
		}
		if (contact.NodeB.Next != null)
		{
			contact.NodeB.Next.Prev = contact.NodeB.Prev;
		}
		if (contact.NodeB == body2.ContactList)
		{
			body2.ContactList = contact.NodeB.Next;
		}
		contact.Destroy();
		ContactCount--;
	}

	internal void Collide()
	{
		Contact contact = ContactList;
		while (contact != null)
		{
			Fixture fixtureA = contact.FixtureA;
			Fixture fixtureB = contact.FixtureB;
			int childIndexA = contact.ChildIndexA;
			int childIndexB = contact.ChildIndexB;
			Body body = fixtureA.Body;
			Body body2 = fixtureB.Body;
			if (!body.Awake && !body2.Awake)
			{
				contact = contact.Next;
				continue;
			}
			if ((contact.Flags & ContactFlags.Filter) == ContactFlags.Filter)
			{
				if (!body2.ShouldCollide(body))
				{
					Contact contact2 = contact;
					contact = contact2.Next;
					Destroy(contact2);
					continue;
				}
				if (ContactFilter != null && !ContactFilter(fixtureA, fixtureB))
				{
					Contact contact3 = contact;
					contact = contact3.Next;
					Destroy(contact3);
					continue;
				}
				contact.Flags &= ~ContactFlags.Filter;
			}
			int proxyId = fixtureA.Proxies[childIndexA].ProxyId;
			int proxyId2 = fixtureB.Proxies[childIndexB].ProxyId;
			if (!BroadPhase.TestOverlap(proxyId, proxyId2))
			{
				Contact contact4 = contact;
				contact = contact4.Next;
				Destroy(contact4);
			}
			else
			{
				contact.Update(this);
				contact = contact.Next;
			}
		}
	}
}
