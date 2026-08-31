using System.Collections.Generic;

namespace BEPUphysics.CollisionRuleManagement;

/// <summary>
/// A group which can have interaction rules created between it and other collision groups.
/// Every entity has a collision group and considers the group's interaction rules in collisions with other entities.
/// </summary>
public class CollisionGroup
{
	private readonly int hashCode;

	/// <summary>
	/// Constructs a new collision group.
	/// </summary>
	public CollisionGroup()
	{
		ulong num = (ulong)base.GetHashCode();
		num = num * num * num * num * num * 3625334849u;
		hashCode = (int)num;
	}

	/// <summary>
	/// Defines the CollisionRule between the two groups for a given space.
	/// </summary>
	/// <param name="groupA">First CollisionGroup of the pair.</param>
	/// <param name="groupB">Second CollisionGroup of the pair.</param>
	/// <param name="rule">CollisionRule to use between the pair.</param>
	public static void DefineCollisionRule(CollisionGroup groupA, CollisionGroup groupB, CollisionRule rule)
	{
		CollisionGroupPair key = new CollisionGroupPair(groupA, groupB);
		if (CollisionRules.CollisionGroupRules.ContainsKey(key))
		{
			CollisionRules.CollisionGroupRules[key] = rule;
		}
		else
		{
			CollisionRules.CollisionGroupRules.Add(key, rule);
		}
	}

	/// <summary>
	/// Defines a CollisionRule between every group in the first set and every group in the second set for a given space.
	/// </summary>
	/// <param name="aGroups">First set of groups.</param>
	/// <param name="bGroups">Second set of groups.</param>
	/// <param name="rule">Collision rule to define between the sets.</param>
	public static void DefineCollisionRulesBetweenSets(List<CollisionGroup> aGroups, List<CollisionGroup> bGroups, CollisionRule rule)
	{
		foreach (CollisionGroup aGroup in aGroups)
		{
			DefineCollisionRulesWithSet(aGroup, bGroups, rule);
		}
	}

	/// <summary>
	/// Defines a CollisionRule between every group in a set with itself and the others in the set for a given space.
	/// </summary>
	/// <param name="groups">Set of CollisionGroups.</param>
	/// <param name="self">CollisionRule between each group and itself.</param>
	/// <param name="other">CollisionRule between each group and every other group in the set.</param>
	public static void DefineCollisionRulesInSet(List<CollisionGroup> groups, CollisionRule self, CollisionRule other)
	{
		for (int i = 0; i < groups.Count; i++)
		{
			DefineCollisionRule(groups[i], groups[i], self);
		}
		for (int j = 0; j < groups.Count - 1; j++)
		{
			for (int k = j + 1; k < groups.Count; k++)
			{
				DefineCollisionRule(groups[j], groups[k], other);
			}
		}
	}

	/// <summary>
	/// Defines a CollisionRule between a group and every group in a set of groups for a given space.
	/// </summary>
	/// <param name="group">First CollisionGroup of the pair.</param>
	/// <param name="groups">Set of CollisionGroups; each group will have its CollisionRule with the first group defined.</param>
	/// <param name="rule">CollisionRule to use between the pairs.</param>
	public static void DefineCollisionRulesWithSet(CollisionGroup group, List<CollisionGroup> groups, CollisionRule rule)
	{
		foreach (CollisionGroup group2 in groups)
		{
			DefineCollisionRule(group, group2, rule);
		}
	}

	/// <summary>
	/// Removes any rule between the two groups in the space.
	/// </summary>
	/// <param name="groupA">First CollisionGroup of the pair.</param>
	/// <param name="groupB">SecondCollisionGroup of the pair.</param>
	public static void RemoveCollisionRule(CollisionGroup groupA, CollisionGroup groupB)
	{
		Dictionary<CollisionGroupPair, CollisionRule> collisionGroupRules = CollisionRules.CollisionGroupRules;
		CollisionGroupPair key = new CollisionGroupPair(groupA, groupB);
		if (collisionGroupRules.ContainsKey(key))
		{
			collisionGroupRules.Remove(key);
		}
	}

	/// <summary>
	/// Removes any rule between every group in the first set and every group in the second set for a given space.
	/// </summary>
	/// <param name="aGroups">First set of groups.</param>
	/// <param name="bGroups">Second set of groups.</param>
	public static void RemoveCollisionRulesBetweenSets(List<CollisionGroup> aGroups, List<CollisionGroup> bGroups)
	{
		foreach (CollisionGroup aGroup in aGroups)
		{
			RemoveCollisionRulesWithSet(aGroup, bGroups);
		}
	}

	/// <summary>
	/// Removes any rule between every group in a set with itself and the others in the set for a given space.
	/// </summary>
	/// <param name="groups">Set of CollisionGroups.</param>
	public static void RemoveCollisionRulesInSet(List<CollisionGroup> groups)
	{
		for (int i = 0; i < groups.Count; i++)
		{
			RemoveCollisionRule(groups[i], groups[i]);
		}
		for (int j = 0; j < groups.Count - 1; j++)
		{
			for (int k = j + 1; k < groups.Count; k++)
			{
				RemoveCollisionRule(groups[j], groups[k]);
			}
		}
	}

	/// <summary>
	/// Removes any rule between a group and every group in a set of groups for a given space.
	/// </summary>
	/// <param name="group">First CollisionGroup of the pair.</param>
	/// <param name="groups">Set of CollisionGroups; each group will have its CollisionRule with the first group removed.</param>
	public static void RemoveCollisionRulesWithSet(CollisionGroup group, List<CollisionGroup> groups)
	{
		foreach (CollisionGroup group2 in groups)
		{
			RemoveCollisionRule(group, group2);
		}
	}

	/// <summary>
	/// Gets a hash code for the object.
	/// </summary>
	/// <returns>Hash code for the object.</returns>
	public override int GetHashCode()
	{
		return hashCode;
	}
}
