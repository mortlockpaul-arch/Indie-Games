using System;

namespace BEPUphysics.CollisionRuleManagement;

/// <summary>
/// Storage strucure containing two CollisionGroup instances used as a key in a collision rules dictionary.
/// </summary>
public struct CollisionGroupPair : IEquatable<CollisionGroupPair>
{
	/// <summary>
	/// First collision group in the pair.
	/// </summary>
	public readonly CollisionGroup A;

	/// <summary>
	/// Second collision group in the pair.
	/// </summary>
	public readonly CollisionGroup B;

	private readonly int hashCode;

	/// <summary>
	/// Constructs a new collision group pair.
	/// </summary>
	/// <param name="groupA">First collision group in the pair.</param>
	/// <param name="groupB">Second collision group in the pair.</param>
	public CollisionGroupPair(CollisionGroup groupA, CollisionGroup groupB)
	{
		if (groupA == null)
		{
			throw new ArgumentNullException("groupA", "The first collision group in the pair is null.  If this pair was being created for CollisionRule calculation purposes, simply consider the rule to be CollisionRule.Defer.");
		}
		if (groupB == null)
		{
			throw new ArgumentNullException("groupB", "The second collision group in the pair is null.  If this pair was being created for CollisionRule calculation purposes, simply consider the rule to be CollisionRule.Defer.");
		}
		A = groupA;
		B = groupB;
		ulong num = (ulong)(((long)groupA.GetHashCode() + (long)groupB.GetHashCode()) * 3625334849u);
		hashCode = (int)num;
	}

	bool IEquatable<CollisionGroupPair>.Equals(CollisionGroupPair other)
	{
		if (other.A != A || other.B != B)
		{
			if (other.B == A)
			{
				return other.A == B;
			}
			return false;
		}
		return true;
	}

	/// <summary>
	/// Determines whether or not the two objects are equal.
	/// Use the IEquatable interface implementation if possible.
	/// </summary>
	/// <param name="obj">Object to compare.</param>
	/// <returns>Whether or not the two objects are equal.</returns>
	public override bool Equals(object obj)
	{
		CollisionGroupPair collisionGroupPair = (CollisionGroupPair)obj;
		if (collisionGroupPair.A != A || collisionGroupPair.B != B)
		{
			if (collisionGroupPair.B == A)
			{
				return collisionGroupPair.A == B;
			}
			return false;
		}
		return true;
	}

	/// <summary>
	/// Gets the hash code of the entity type pair.
	/// </summary>
	/// <returns>Hash code of the entity type pair.</returns>
	public override int GetHashCode()
	{
		return hashCode;
	}
}
