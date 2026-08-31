using System;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.Threading;

namespace BEPUphysics.DeactivationManagement;

/// <summary>
/// Object owned by an entity which lives in a simulation island.
/// Can be considered the entity's deactivation system proxy, just as the CollisionInformation property stores the collision pipeline proxy.
/// </summary>
public class SimulationIslandMember
{
	private Entity owner;

	private float previousVelocity;

	internal float velocityTimeBelowLimit;

	internal bool isSlowing;

	internal RawList<SimulationIslandConnection> connections = new RawList<SimulationIslandConnection>(8);

	private bool isDeactivationCandidate;

	internal SpinLock simulationIslandChangeLocker = new SpinLock();

	private bool previouslyActive = true;

	private bool isAlwaysActive;

	internal bool allowStabilization = true;

	internal SimulationIsland simulationIsland;

	/// <summary>
	///  Gets or sets the current search state of the simulation island member.  This is used by the simulation island system
	///  to efficiently split islands.
	/// </summary>
	internal SimulationIslandSearchState searchState;

	/// <summary>
	/// Gets the entity that owns this simulation island member.
	/// </summary>
	public Entity Owner => owner;

	/// <summary>
	///  Gets the connections associated with this member.
	/// </summary>
	public ReadOnlyList<SimulationIslandConnection> Connections => new ReadOnlyList<SimulationIslandConnection>(connections);

	/// <summary>
	///  Gets or sets whether or not the object is a deactivation candidate.
	/// </summary>
	public bool IsDeactivationCandidate
	{
		get
		{
			return isDeactivationCandidate;
		}
		private set
		{
			if (value && !isDeactivationCandidate)
			{
				isDeactivationCandidate = true;
				OnBecameDeactivationCandidate();
			}
			else if (!value && isDeactivationCandidate)
			{
				isDeactivationCandidate = false;
				OnBecameNonDeactivationCandidate();
			}
			if (!value)
			{
				velocityTimeBelowLimit = 0f;
			}
		}
	}

	/// <summary>
	///  Gets whether or not the member is active.
	/// </summary>
	public bool IsActive => SimulationIsland?.isActive ?? (velocityTimeBelowLimit <= 0f);

	/// <summary>
	/// Gets or sets whether or not this member is always active.
	/// </summary>
	public bool IsAlwaysActive
	{
		get
		{
			return isAlwaysActive;
		}
		set
		{
			isAlwaysActive = value;
			if (isAlwaysActive)
			{
				Activate();
			}
		}
	}

	/// <summary>
	/// Gets or sets whether or not the entity can be stabilized by the deactivation system.  This allows systems of objects to go to sleep faster.
	/// Defaults to true.
	/// </summary>
	public bool AllowStabilization
	{
		get
		{
			return allowStabilization;
		}
		set
		{
			allowStabilization = value;
		}
	}

	/// <summary>
	///  Gets the simulation island that owns this member.
	/// </summary>
	public SimulationIsland SimulationIsland
	{
		get
		{
			if (simulationIsland == null)
			{
				return null;
			}
			return simulationIsland.Parent;
		}
		internal set
		{
			simulationIsland = value;
		}
	}

	/// <summary>
	/// Gets the deactivation manager that is managing this member.
	/// </summary>
	public DeactivationManager DeactivationManager { get; internal set; }

	/// <summary>
	///  Gets whether or not the object is dynamic.
	///  Non-dynamic members act as dead-ends in connection graphs.
	/// </summary>
	public bool IsDynamic => owner.isDynamic;

	/// <summary>
	///  Fired when the object activates.
	/// </summary>
	public event Action<SimulationIslandMember> Activated;

	/// <summary>
	///  Fired when the object becomes a deactivation candidate.
	/// </summary>
	public event Action<SimulationIslandMember> BecameDeactivationCandidate;

	/// <summary>
	///  Fired when the object is no longer a deactivation candidate.
	/// </summary>
	public event Action<SimulationIslandMember> BecameNonDeactivationCandidate;

	/// <summary>
	///  Fired when the object deactivates.
	/// </summary>
	public event Action<SimulationIslandMember> Deactivated;

	internal SimulationIslandMember(Entity owner)
	{
		this.owner = owner;
	}

	/// <summary>
	///  Updates the member's deactivation state.
	/// </summary>
	/// <param name="dt">Timestep duration.</param>
	public void UpdateDeactivationCandidacy(float dt)
	{
		float num = owner.linearVelocity.LengthSquared() + owner.angularVelocity.LengthSquared();
		bool isActive = IsActive;
		if (isActive)
		{
			TryToCompressIslandHierarchy();
			isSlowing = num <= previousVelocity;
			if (IsDynamic)
			{
				if (num < DeactivationManager.velocityLowerLimitSquared)
				{
					velocityTimeBelowLimit += dt;
				}
				else
				{
					velocityTimeBelowLimit = 0f;
				}
				if (!IsAlwaysActive)
				{
					if (!isDeactivationCandidate)
					{
						if (velocityTimeBelowLimit > DeactivationManager.lowVelocityTimeMinimum && isSlowing)
						{
							IsDeactivationCandidate = true;
						}
					}
					else if (velocityTimeBelowLimit <= DeactivationManager.lowVelocityTimeMinimum)
					{
						IsDeactivationCandidate = false;
					}
				}
				else
				{
					IsDeactivationCandidate = false;
				}
			}
			else
			{
				IsDeactivationCandidate = num == 0f && !IsAlwaysActive;
				if (IsDeactivationCandidate)
				{
					if (velocityTimeBelowLimit == 0f)
					{
						velocityTimeBelowLimit = 1f;
					}
					else if (velocityTimeBelowLimit < 0f)
					{
						velocityTimeBelowLimit = 0f;
					}
				}
				else
				{
					velocityTimeBelowLimit = -1f;
				}
				if (velocityTimeBelowLimit <= 0f)
				{
					for (int i = 0; i < connections.count; i++)
					{
						RawList<SimulationIslandConnection.Entry> entries = connections.Elements[i].entries;
						for (int num2 = entries.count - 1; num2 >= 0; num2--)
						{
							entries.Elements[num2].Member.simulationIslandChangeLocker.Enter();
							SimulationIsland simulationIsland = entries.Elements[num2].Member.SimulationIsland;
							if (simulationIsland != null)
							{
								simulationIsland.Activate();
								simulationIsland.allowDeactivation = false;
							}
							entries.Elements[num2].Member.simulationIslandChangeLocker.Exit();
						}
					}
				}
			}
		}
		previousVelocity = num;
		if (previouslyActive && !isActive)
		{
			OnDeactivated();
		}
		else if (!previouslyActive && isActive)
		{
			OnActivated();
		}
		previouslyActive = isActive;
	}

	private void TryToCompressIslandHierarchy()
	{
		SimulationIsland simulationIsland = this.simulationIsland;
		if (simulationIsland != null && simulationIsland.immediateParent != simulationIsland)
		{
			simulationIslandChangeLocker.Enter();
			lock (simulationIsland)
			{
				simulationIsland.Remove(this);
			}
			simulationIsland = simulationIsland.Parent;
			lock (simulationIsland)
			{
				simulationIsland.Add(this);
			}
			simulationIslandChangeLocker.Exit();
		}
	}

	/// <summary>
	/// Attempts to activate the entity.
	/// </summary>
	public void Activate()
	{
		IsDeactivationCandidate = false;
		SimulationIsland simulationIsland = SimulationIsland;
		if (simulationIsland != null)
		{
			simulationIsland.IsActive = true;
		}
		else
		{
			velocityTimeBelowLimit = -1f;
		}
	}

	protected internal void OnActivated()
	{
		if (Activated != null)
		{
			Activated(this);
		}
	}

	protected internal void OnBecameDeactivationCandidate()
	{
		if (BecameDeactivationCandidate != null)
		{
			BecameDeactivationCandidate(this);
		}
	}

	protected internal void OnBecameNonDeactivationCandidate()
	{
		if (BecameNonDeactivationCandidate != null)
		{
			BecameNonDeactivationCandidate(this);
		}
	}

	protected internal void OnDeactivated()
	{
		if (Deactivated != null)
		{
			Deactivated(this);
		}
	}

	/// <summary>
	///  Removes a connection reference from the member.
	/// </summary>
	/// <param name="connection">Reference to remove.</param>
	/// <param name="index">Index of the connection in this member's list</param>
	internal void RemoveConnectionReference(SimulationIslandConnection connection, int index)
	{
		if (connections.count > index)
		{
			connections.FastRemoveAt(index);
			if (connections.count > index)
			{
				connections.Elements[index].SetListIndex(this, index);
			}
		}
	}

	/// <summary>
	///  Adds a connection reference to the member.
	/// </summary>
	/// <param name="connection">Reference to add.</param>
	/// <returns>Index of the connection in the member's list.</returns>
	internal int AddConnectionReference(SimulationIslandConnection connection)
	{
		connections.Add(connection);
		return connections.count - 1;
	}
}
