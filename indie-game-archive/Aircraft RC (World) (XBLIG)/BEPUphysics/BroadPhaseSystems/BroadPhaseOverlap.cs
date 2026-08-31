using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.CollisionRuleManagement;

namespace BEPUphysics.BroadPhaseSystems;

/// <summary>
/// A pair of overlapping BroadPhaseEntries.
/// </summary>
public struct BroadPhaseOverlap : IEquatable<BroadPhaseOverlap>
{
	internal BroadPhaseEntry entryA;

	internal BroadPhaseEntry entryB;

	internal CollisionRule collisionRule;

	/// <summary>
	/// First entry in the pair.
	/// </summary>
	public BroadPhaseEntry EntryA => entryA;

	/// <summary>
	/// Second entry in the pair.
	/// </summary>
	public BroadPhaseEntry EntryB => entryB;

	/// <summary>
	/// Gets the collision rule calculated for the pair.
	/// </summary>
	public CollisionRule CollisionRule => collisionRule;

	/// <summary>
	/// Constructs an overlap.
	/// </summary>
	/// <param name="entryA">First entry in the pair.</param>
	/// <param name="entryB">Second entry in the pair.</param>
	public BroadPhaseOverlap(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
	{
		this.entryA = entryA;
		this.entryB = entryB;
		collisionRule = CollisionRules.DefaultCollisionRule;
	}

	/// <summary>
	/// Constructs an overlap.
	/// </summary>
	/// <param name="entryA">First entry in the pair.</param>
	/// <param name="entryB">Second entry in the pair.</param>
	/// <param name="collisionRule">Collision rule calculated for the pair.</param>
	public BroadPhaseOverlap(BroadPhaseEntry entryA, BroadPhaseEntry entryB, CollisionRule collisionRule)
	{
		this.entryA = entryA;
		this.entryB = entryB;
		this.collisionRule = collisionRule;
	}

	/// <summary>
	/// Gets the hash code of the object.
	/// </summary>
	/// <returns>Hash code of the object.</returns>
	public override int GetHashCode()
	{
		return (int)((entryA.hashCode + entryB.hashCode) * 3625334849u);
	}

	/// <summary>
	/// Compares the overlaps for equality based on the involved entries.
	/// </summary>
	/// <param name="other">Overlap to compare.</param>
	/// <returns>Whether or not the overlaps were equal.</returns>
	public bool Equals(BroadPhaseOverlap other)
	{
		if (other.entryA != entryA || other.entryB != entryB)
		{
			if (other.entryA == entryB)
			{
				return other.entryB == entryA;
			}
			return false;
		}
		return true;
	}

	public override string ToString()
	{
		return string.Concat("{", entryA, ", ", entryB, "}");
	}
}
