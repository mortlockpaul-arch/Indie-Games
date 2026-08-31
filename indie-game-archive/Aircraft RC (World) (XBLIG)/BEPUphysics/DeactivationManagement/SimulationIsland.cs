using System;
using System.Threading;

namespace BEPUphysics.DeactivationManagement;

/// <summary>
///  A collection of simulation island members bound together with connections.
///  An island is activated and deactivated as a group.
/// </summary>
public class SimulationIsland
{
	internal SimulationIsland immediateParent;

	internal bool allowDeactivation = true;

	internal bool isActive = true;

	internal int memberCount;

	internal int deactivationCandidateCount;

	private Action<SimulationIslandMember> memberActivatedDelegate;

	private Action<SimulationIslandMember> becameDeactivationCandidateDelegate;

	private Action<SimulationIslandMember> becameNonDeactivationCandidateDelegate;

	internal SimulationIsland Parent
	{
		get
		{
			if (immediateParent != this)
			{
				return immediateParent.Parent;
			}
			return this;
		}
	}

	/// <summary>
	///  Gets whether or not the island is currently active.
	/// </summary>
	public bool IsActive
	{
		get
		{
			return isActive;
		}
		set
		{
			isActive = value;
		}
	}

	/// <summary>
	///  Constructs a simulation island.
	/// </summary>
	public SimulationIsland()
	{
		memberActivatedDelegate = MemberActivated;
		becameDeactivationCandidateDelegate = BecameDeactivationCandidate;
		becameNonDeactivationCandidateDelegate = BecameNonDeactivationCandidate;
		CleanUp();
	}

	private void MemberActivated(SimulationIslandMember member)
	{
		Activate();
	}

	private void BecameDeactivationCandidate(SimulationIslandMember member)
	{
		Interlocked.Increment(ref deactivationCandidateCount);
	}

	private void BecameNonDeactivationCandidate(SimulationIslandMember member)
	{
		Interlocked.Decrement(ref deactivationCandidateCount);
	}

	/// <summary>
	///  Activates the simulation island.
	/// </summary>
	public void Activate()
	{
		if (!isActive)
		{
			isActive = true;
		}
	}

	/// <summary>
	///  Attempts to deactivate the simulation island.
	/// </summary>
	/// <returns>Whether or not the simulation island was successfully deactivated.</returns>
	public bool TryToDeactivate()
	{
		if (allowDeactivation)
		{
			if (isActive && deactivationCandidateCount == memberCount)
			{
				isActive = false;
				return true;
			}
			return false;
		}
		allowDeactivation = true;
		return false;
	}

	/// <summary>
	///  Adds a member to the simulation island.
	/// </summary>
	/// <param name="member">Member to add.</param>
	/// <exception cref="T:System.Exception">Thrown when the member being added is either non-dynamic or already has a simulation island.</exception>
	public void Add(SimulationIslandMember member)
	{
		if (member.IsDynamic && member.simulationIsland == null)
		{
			member.simulationIsland = this;
			memberCount++;
			member.Activated += memberActivatedDelegate;
			member.BecameDeactivationCandidate += becameDeactivationCandidateDelegate;
			member.BecameNonDeactivationCandidate += becameNonDeactivationCandidateDelegate;
			if (member.IsDeactivationCandidate)
			{
				deactivationCandidateCount++;
			}
			return;
		}
		throw new Exception("Member either is not dynamic or already has a simulation island; cannot add.");
	}

	/// <summary>
	///  Removes a member from the simulation island.
	/// </summary>
	/// <param name="member">Member to remove.</param>
	/// <exception cref="T:System.Exception">Thrown when the member does not belong to this simulation island.</exception>
	public void Remove(SimulationIslandMember member)
	{
		if (member.simulationIsland == this)
		{
			memberCount--;
			member.simulationIsland = null;
			member.Activated -= memberActivatedDelegate;
			member.BecameDeactivationCandidate -= becameDeactivationCandidateDelegate;
			member.BecameNonDeactivationCandidate -= becameNonDeactivationCandidateDelegate;
			if (member.IsDeactivationCandidate)
			{
				deactivationCandidateCount--;
			}
			return;
		}
		throw new Exception("Member does not belong to island; cannot remove.");
	}

	internal void CleanUp()
	{
		isActive = true;
		deactivationCandidateCount = 0;
		memberCount = 0;
		immediateParent = this;
	}
}
