using System;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Collisions;

public struct Contact : IEquatable<Contact>
{
	public ContactId ContactId;

	public Vector2 Position;

	public Vector2 Normal;

	public float Separation;

	internal float bounceVelocity;

	internal float massNormal;

	internal float massTangent;

	internal float normalImpulse;

	internal float normalImpulseBias;

	internal float normalVelocityBias;

	internal Vector2 r1;

	internal Vector2 r2;

	internal float tangentImpulse;

	public Contact(Vector2 position, Vector2 normal, float separation, ContactId contactId)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		ContactId = contactId;
		Position = position;
		Normal = normal;
		Separation = separation;
		normalImpulse = 0f;
		tangentImpulse = 0f;
		massNormal = 0f;
		massTangent = 0f;
		normalVelocityBias = 0f;
		normalImpulseBias = 0f;
		r1 = Vector2.Zero;
		r2 = Vector2.Zero;
		bounceVelocity = 0f;
	}

	public bool Equals(Contact other)
	{
		return ContactId.Equals(ref other.ContactId);
	}

	public bool Equals(ref Contact other)
	{
		return ContactId.Equals(ref other.ContactId);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Contact))
		{
			return false;
		}
		return Equals((Contact)obj);
	}

	public static bool operator ==(Contact contact1, Contact contact2)
	{
		return contact1.Equals(ref contact2);
	}

	public static bool operator !=(Contact contact1, Contact contact2)
	{
		return !contact1.Equals(ref contact2);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
