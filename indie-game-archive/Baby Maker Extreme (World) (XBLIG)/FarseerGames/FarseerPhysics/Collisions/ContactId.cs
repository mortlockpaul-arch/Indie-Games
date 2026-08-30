using System;

namespace FarseerGames.FarseerPhysics.Collisions;

public struct ContactId : IEquatable<ContactId>
{
	public int Geom1Index { get; set; }

	public int Geom1Vertex { get; set; }

	public int Geom2Index { get; set; }

	public ContactId(int geometryAIndex, int geometryAVertex, int geometryBIndex)
	{
		this = default(ContactId);
		Geom1Index = geometryAIndex;
		Geom1Vertex = geometryAVertex;
		Geom2Index = geometryBIndex;
	}

	public bool Equals(ContactId other)
	{
		if (Geom1Index == other.Geom1Index && Geom1Vertex == other.Geom1Vertex)
		{
			return Geom2Index == other.Geom2Index;
		}
		return false;
	}

	public bool Equals(ref ContactId other)
	{
		if (Geom1Index == other.Geom1Index && Geom1Vertex == other.Geom1Vertex)
		{
			return Geom2Index == other.Geom2Index;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is ContactId))
		{
			return false;
		}
		return Equals((ContactId)obj);
	}

	public static bool operator ==(ContactId contactId1, ContactId contactId2)
	{
		return contactId1.Equals(ref contactId2);
	}

	public static bool operator !=(ContactId contactId1, ContactId contactId2)
	{
		return !contactId1.Equals(ref contactId2);
	}

	public override int GetHashCode()
	{
		return Geom1Index + Geom1Vertex + Geom2Index;
	}
}
