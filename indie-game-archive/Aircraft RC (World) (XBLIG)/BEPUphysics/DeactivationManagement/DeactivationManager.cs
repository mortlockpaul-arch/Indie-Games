using System;
using System.Collections.Generic;
using BEPUphysics.DataStructures;
using BEPUphysics.ResourceManagement;
using BEPUphysics.Threading;

namespace BEPUphysics.DeactivationManagement;

/// <summary>
///  Manages the sleeping states of objects.
/// </summary>
public class DeactivationManager : MultithreadedProcessingStage
{
	private int maximumDeactivationAttemptsPerFrame = 100;

	private int deactivationIslandIndex;

	internal float velocityLowerLimitSquared = 0.07f;

	internal float lowVelocityTimeMinimum = 1f;

	internal bool useStabilization = true;

	private Queue<SimulationIslandMember> member1Friends = new Queue<SimulationIslandMember>();

	private Queue<SimulationIslandMember> member2Friends = new Queue<SimulationIslandMember>();

	private List<SimulationIslandMember> searchedMembers1 = new List<SimulationIslandMember>();

	private List<SimulationIslandMember> searchedMembers2 = new List<SimulationIslandMember>();

	private TimeStepSettings timeStepSettings;

	private RawList<SimulationIslandMember> simulationIslandMembers = new RawList<SimulationIslandMember>();

	private RawList<SimulationIsland> simulationIslands = new RawList<SimulationIsland>();

	private UnsafeResourcePool<SimulationIsland> islandPool = new UnsafeResourcePool<SimulationIsland>();

	private Action<int> multithreadedCandidacyLoopDelegate;

	private Queue<SimulationIslandConnection> splitAttempts = new Queue<SimulationIslandConnection>();

	private static float maximumSplitAttemptsFraction = 0.01f;

	private static int minimumSplitAttempts = 3;

	/// <summary>
	///  Gets or sets the velocity under which the deactivation system will consider 
	///  objects to be deactivation candidates (if their velocity stays below the limit
	///  for the LowVelocityTimeMinimum).
	///  Defaults to 0.26.
	/// </summary>
	public float VelocityLowerLimit
	{
		get
		{
			return (float)Math.Sqrt(velocityLowerLimitSquared);
		}
		set
		{
			velocityLowerLimitSquared = value * value;
		}
	}

	/// <summary>
	/// Gets or sets the time limit above which the deactivation system will consider
	/// objects to be deactivation candidates (if their velocity stays below the VelocityLowerLimit for the duration).
	/// Defaults to 1.
	/// </summary>
	public float LowVelocityTimeMinimum
	{
		get
		{
			return lowVelocityTimeMinimum;
		}
		set
		{
			if (value <= 0f)
			{
				throw new Exception("Must use a positive, non-zero value for deactivation time minimum.");
			}
			lowVelocityTimeMinimum = value;
		}
	}

	/// <summary>
	///  Gets or sets whether or not to use a stabilization effect on nearly motionless objects.
	///  This removes a lot of energy from a system when things are settling down, allowing them to go 
	///  to sleep faster.  It also makes most simulations appear a lot more robust.
	///  Defaults to true.
	/// </summary>
	public bool UseStabilization
	{
		get
		{
			return useStabilization;
		}
		set
		{
			useStabilization = value;
		}
	}

	/// <summary>
	///  Gets or sets the maximum number of objects to attempt to deactivate each frame.
	///  Defaults to 100.
	/// </summary>
	public int MaximumDeactivationAttemptsPerFrame
	{
		get
		{
			return maximumDeactivationAttemptsPerFrame;
		}
		set
		{
			maximumDeactivationAttemptsPerFrame = value;
		}
	}

	/// <summary>
	///  Gets or sets the time step settings used by the deactivation manager.
	/// </summary>
	public TimeStepSettings TimeStepSettings
	{
		get
		{
			return timeStepSettings;
		}
		set
		{
			timeStepSettings = value;
		}
	}

	/// <summary>
	///  Gets the simulation islands currently in the manager.
	/// </summary>
	public ReadOnlyList<SimulationIsland> SimulationIslands => new ReadOnlyList<SimulationIsland>(simulationIslands);

	/// <summary>
	/// Gets or sets the fraction of splits that the deactivation manager will attempt in a single frame.
	/// The total splits queued multiplied by this value results in the number of splits managed.
	/// Defaults to .04f.
	/// </summary>
	public static float MaximumSplitAttemptsFraction
	{
		get
		{
			return maximumSplitAttemptsFraction;
		}
		set
		{
			if (value > 1f || value < 0f)
			{
				throw new Exception("Value must be from zero to one.");
			}
			maximumSplitAttemptsFraction = value;
		}
	}

	/// <summary>
	/// Gets or sets the minimum number of splits attempted in a single frame.
	/// Defaults to 5.
	/// </summary>
	public static int MinimumSplitAttempts
	{
		get
		{
			return minimumSplitAttempts;
		}
		set
		{
			if (value >= 0)
			{
				throw new Exception("Minimum split count must be nonnegative.");
			}
			minimumSplitAttempts = value;
		}
	}

	/// <summary>
	///  Constructs a deactivation manager.
	/// </summary>
	/// <param name="timeStepSettings">The time step settings used by the manager.</param>
	public DeactivationManager(TimeStepSettings timeStepSettings)
	{
		Enabled = true;
		multithreadedCandidacyLoopDelegate = MultithreadedCandidacyLoop;
		this.timeStepSettings = timeStepSettings;
	}

	/// <summary>
	///  Constructs a deactivation manager.
	/// </summary>
	/// <param name="timeStepSettings">The time step settings used by the manager.</param>
	///  <param name="threadManager">Thread manager used by the manager.</param>
	public DeactivationManager(TimeStepSettings timeStepSettings, IThreadManager threadManager)
		: this(timeStepSettings)
	{
		base.ThreadManager = threadManager;
		base.AllowMultithreading = true;
	}

	private void GiveBackIsland(SimulationIsland island)
	{
		island.CleanUp();
		islandPool.GiveBack(island);
	}

	/// <summary>
	///  Adds a simulation island member to the manager.
	/// </summary>
	/// <param name="simulationIslandMember">Member to add.</param>
	/// <exception cref="T:System.Exception">Thrown if the member already belongs to a manager.</exception>
	public void Add(SimulationIslandMember simulationIslandMember)
	{
		if (simulationIslandMember.DeactivationManager == null)
		{
			simulationIslandMember.Activate();
			simulationIslandMember.DeactivationManager = this;
			simulationIslandMembers.Add(simulationIslandMember);
			if (simulationIslandMember.IsDynamic)
			{
				AddSimulationIslandToMember(simulationIslandMember);
			}
			else
			{
				RemoveSimulationIslandFromMember(simulationIslandMember);
			}
			return;
		}
		throw new Exception("Cannot add that member to this DeactivationManager; it already belongs to a manager.");
	}

	/// <summary>
	/// Removes the member from this island.
	/// </summary>
	/// <param name="simulationIslandMember">Removes the member from the manager.</param>
	public void Remove(SimulationIslandMember simulationIslandMember)
	{
		if (simulationIslandMember.DeactivationManager == this)
		{
			if (simulationIslandMember.IsDynamic)
			{
				simulationIslandMember.Activate();
			}
			else
			{
				foreach (SimulationIslandConnection connection in simulationIslandMember.connections)
				{
					foreach (SimulationIslandConnection.Entry entry in connection.entries)
					{
						if (entry.Member != simulationIslandMember)
						{
							entry.Member.Activate();
						}
					}
				}
			}
			simulationIslandMember.DeactivationManager = null;
			simulationIslandMembers.Remove(simulationIslandMember);
			RemoveSimulationIslandFromMember(simulationIslandMember);
			return;
		}
		throw new Exception("Cannot remove that member from this DeactivationManager; it belongs to a different or no manager.");
	}

	private void MultithreadedCandidacyLoop(int i)
	{
		simulationIslandMembers.Elements[i].UpdateDeactivationCandidacy(timeStepSettings.TimeStepDuration);
	}

	protected override void UpdateMultithreaded()
	{
		FlushSplits();
		base.ThreadManager.ForLoop(0, simulationIslandMembers.count, multithreadedCandidacyLoopDelegate);
		DeactivateObjects();
	}

	protected override void UpdateSingleThreaded()
	{
		FlushSplits();
		for (int i = 0; i < simulationIslandMembers.count; i++)
		{
			simulationIslandMembers.Elements[i].UpdateDeactivationCandidacy(timeStepSettings.TimeStepDuration);
		}
		DeactivateObjects();
	}

	private void FlushSplits()
	{
		int num = Math.Max(minimumSplitAttempts, (int)((float)splitAttempts.Count * maximumSplitAttemptsFraction));
		int num2 = 0;
		while (num2 < num && splitAttempts.Count > 0)
		{
			SimulationIslandConnection simulationIslandConnection = splitAttempts.Dequeue();
			if (!simulationIslandConnection.SlatedForRemoval)
			{
				continue;
			}
			simulationIslandConnection.SlatedForRemoval = false;
			simulationIslandConnection.RemoveReferencesFromConnectedMembers();
			bool flag = false;
			for (int i = 0; i < simulationIslandConnection.entries.count; i++)
			{
				for (int j = i + 1; j < simulationIslandConnection.entries.count; j++)
				{
					flag |= TryToSplit(simulationIslandConnection.entries.Elements[i].Member, simulationIslandConnection.entries.Elements[j].Member);
				}
			}
			if (flag)
			{
				num2++;
			}
			if (simulationIslandConnection.Owner == null)
			{
				Resources.GiveBack(simulationIslandConnection);
			}
		}
	}

	private void DeactivateObjects()
	{
		int num = 0;
		int num2 = 0;
		int count = simulationIslands.count;
		while (num < maximumDeactivationAttemptsPerFrame && simulationIslands.count > 0 && num2 < count)
		{
			deactivationIslandIndex = (deactivationIslandIndex + 1) % simulationIslands.count;
			SimulationIsland simulationIsland = simulationIslands.Elements[deactivationIslandIndex];
			if (simulationIsland.memberCount == 0)
			{
				simulationIslands.FastRemoveAt(deactivationIslandIndex);
				GiveBackIsland(simulationIsland);
			}
			else
			{
				simulationIsland.TryToDeactivate();
				num += simulationIsland.memberCount;
			}
			num2++;
		}
	}

	/// <summary>
	///  Adds a simulation island connection to the deactivation manager.
	/// </summary>
	/// <param name="connection">Connection to add.</param>
	/// <exception cref="T:System.ArgumentException">Thrown if the connection already belongs to a manager.</exception>
	public void Add(SimulationIslandConnection connection)
	{
		if (connection.DeactivationManager == null)
		{
			connection.DeactivationManager = this;
			if (connection.entries.count <= 0)
			{
				return;
			}
			SimulationIsland simulationIsland = connection.entries.Elements[0].Member.SimulationIsland;
			for (int i = 1; i < connection.entries.count; i++)
			{
				SimulationIsland simulationIsland2;
				if (simulationIsland != (simulationIsland2 = connection.entries.Elements[i].Member.SimulationIsland))
				{
					simulationIsland = Merge(simulationIsland, simulationIsland2);
				}
			}
			if (connection.SlatedForRemoval)
			{
				connection.SlatedForRemoval = false;
			}
			else
			{
				connection.AddReferencesToConnectedMembers();
			}
			return;
		}
		throw new ArgumentException("Cannot add connection to deactivation manager; it already belongs to one.");
	}

	private SimulationIsland Merge(SimulationIsland s1, SimulationIsland s2)
	{
		if (s1 == null)
		{
			s2.Activate();
			return s2;
		}
		if (s2 == null)
		{
			s1.Activate();
			return s1;
		}
		if (s1.memberCount < s2.memberCount)
		{
			SimulationIsland simulationIsland = s2;
			s2 = s1;
			s1 = simulationIsland;
		}
		s1.Activate();
		s2.immediateParent = s1;
		return s1;
	}

	/// <summary>
	///  Removes a simulation island connection from the manager.
	/// </summary>
	/// <param name="connection">Connection to remove from the manager.</param>
	/// <exception cref="T:System.ArgumentException">Thrown if the connection does not belong to this manager.</exception>
	public void Remove(SimulationIslandConnection connection)
	{
		if (connection.DeactivationManager == this)
		{
			connection.DeactivationManager = null;
			connection.SlatedForRemoval = true;
			splitAttempts.Enqueue(connection);
			return;
		}
		throw new ArgumentException("Cannot remove connection from activity manager; it is owned by a different or no activity manager.");
	}

	/// <summary>
	/// Tries to split connections between the two island members.
	/// </summary>
	/// <param name="member1">First island member.</param>
	/// <param name="member2">Second island member.</param>
	/// <returns>Whether a split operation was run.  This does not mean a split was
	/// successful, just that the expensive test was performed.</returns>
	private bool TryToSplit(SimulationIslandMember member1, SimulationIslandMember member2)
	{
		if (member1.SimulationIsland != member2.SimulationIsland || member1.SimulationIsland == null || member2.SimulationIsland == null)
		{
			return false;
		}
		member1Friends.Enqueue(member1);
		member2Friends.Enqueue(member2);
		searchedMembers1.Add(member1);
		searchedMembers2.Add(member2);
		member1.searchState = SimulationIslandSearchState.OwnedByFirst;
		member2.searchState = SimulationIslandSearchState.OwnedBySecond;
		while (true)
		{
			if (member1Friends.Count > 0 && member2Friends.Count > 0)
			{
				SimulationIslandMember simulationIslandMember = member1Friends.Dequeue();
				for (int i = 0; i < simulationIslandMember.connections.count; i++)
				{
					for (int j = 0; j < simulationIslandMember.connections.Elements[i].entries.count; j++)
					{
						SimulationIslandMember member3;
						if ((member3 = simulationIslandMember.connections.Elements[i].entries.Elements[j].Member) == simulationIslandMember || member3.SimulationIsland == null)
						{
							continue;
						}
						switch (member3.searchState)
						{
						case SimulationIslandSearchState.Unclaimed:
							member1Friends.Enqueue(member3);
							member3.searchState = SimulationIslandSearchState.OwnedByFirst;
							searchedMembers1.Add(member3);
							continue;
						case SimulationIslandSearchState.OwnedBySecond:
							break;
						default:
							continue;
						}
						goto IL_00ea;
					}
				}
				simulationIslandMember = member2Friends.Dequeue();
				for (int k = 0; k < simulationIslandMember.connections.count; k++)
				{
					for (int l = 0; l < simulationIslandMember.connections.Elements[k].entries.count; l++)
					{
						SimulationIslandMember member4;
						if ((member4 = simulationIslandMember.connections.Elements[k].entries.Elements[l].Member) == simulationIslandMember || member4.SimulationIsland == null)
						{
							continue;
						}
						switch (member4.searchState)
						{
						case SimulationIslandSearchState.Unclaimed:
							member2Friends.Enqueue(member4);
							member4.searchState = SimulationIslandSearchState.OwnedBySecond;
							searchedMembers2.Add(member4);
							continue;
						case SimulationIslandSearchState.OwnedByFirst:
							break;
						default:
							continue;
						}
						goto IL_01c8;
					}
				}
				continue;
			}
			SimulationIsland simulationIsland = islandPool.Take();
			simulationIslands.Add(simulationIsland);
			if (member1Friends.Count == 0)
			{
				for (int m = 0; m < searchedMembers1.Count; m++)
				{
					searchedMembers1[m].simulationIsland.Remove(searchedMembers1[m]);
					simulationIsland.Add(searchedMembers1[m]);
				}
				member2Friends.Clear();
			}
			else if (member2Friends.Count == 0)
			{
				for (int n = 0; n < searchedMembers2.Count; n++)
				{
					searchedMembers2[n].simulationIsland.Remove(searchedMembers2[n]);
					simulationIsland.Add(searchedMembers2[n]);
				}
				member1Friends.Clear();
			}
			member1.Activate();
			member2.Activate();
			break;
			IL_00ea:
			member1Friends.Clear();
			member2Friends.Clear();
			break;
			IL_01c8:
			member1Friends.Clear();
			member2Friends.Clear();
			break;
		}
		for (int num = 0; num < searchedMembers1.Count; num++)
		{
			searchedMembers1[num].searchState = SimulationIslandSearchState.Unclaimed;
		}
		for (int num2 = 0; num2 < searchedMembers2.Count; num2++)
		{
			searchedMembers2[num2].searchState = SimulationIslandSearchState.Unclaimed;
		}
		searchedMembers1.Clear();
		searchedMembers2.Clear();
		return true;
	}

	/// <summary>
	///  Strips a member of its simulation island.
	/// </summary>
	/// <param name="member">Member to be stripped.</param>
	public void RemoveSimulationIslandFromMember(SimulationIslandMember member)
	{
		if (member.simulationIsland != null)
		{
			SimulationIsland simulationIsland = member.simulationIsland;
			simulationIsland.Remove(member);
			if (simulationIsland.memberCount == 0)
			{
				simulationIslands.Remove(simulationIsland);
				GiveBackIsland(simulationIsland);
				return;
			}
		}
		if (member.connections.count <= 0)
		{
			return;
		}
		for (int i = 0; i < member.Connections.Count; i++)
		{
			SimulationIslandMember simulationIslandMember = null;
			for (int j = 0; j < member.connections.Elements[i].entries.count; j++)
			{
				if (member.connections.Elements[i].entries.Elements[j].Member.SimulationIsland != null)
				{
					simulationIslandMember = member;
					break;
				}
			}
			if (simulationIslandMember == null)
			{
				continue;
			}
			for (int k = i + 1; k < member.Connections.Count; k++)
			{
				SimulationIslandMember simulationIslandMember2 = null;
				for (int l = 0; l < member.connections.Elements[k].entries.count; l++)
				{
					if (member.connections.Elements[k].entries.Elements[l].Member.SimulationIsland != null)
					{
						simulationIslandMember2 = member;
						break;
					}
				}
				if (simulationIslandMember2 != null)
				{
					TryToSplit(simulationIslandMember, simulationIslandMember2);
				}
			}
		}
	}

	/// <summary>
	///  Adds a simulation island to a member.
	/// </summary>
	/// <param name="member">Member to gain a simulation island.</param>
	/// <exception cref="T:System.Exception">Thrown if the member already has a simulation island.</exception>
	public void AddSimulationIslandToMember(SimulationIslandMember member)
	{
		if (member.SimulationIsland != null)
		{
			throw new Exception("Cannot initialize member's simulation island; it already has one.");
		}
		if (member.Connections.Count > 0)
		{
			SimulationIsland simulationIsland = null;
			for (int i = 0; i < member.Connections.Count; i++)
			{
				for (int j = 0; j < member.connections.Elements[i].entries.count; j++)
				{
					simulationIsland = member.connections.Elements[i].entries.Elements[j].Member.SimulationIsland;
					if (simulationIsland != null)
					{
						simulationIsland.Add(member);
						break;
					}
				}
				if (simulationIsland != null)
				{
					break;
				}
			}
			if (member.SimulationIsland == null)
			{
				SimulationIsland simulationIsland2 = islandPool.Take();
				simulationIslands.Add(simulationIsland2);
				simulationIsland2.Add(member);
				return;
			}
			for (int k = 0; k < member.connections.count; k++)
			{
				for (int l = 0; l < member.connections.Elements[k].entries.count; l++)
				{
					if (member.connections.Elements[k].entries.Elements[l].Member == member)
					{
						continue;
					}
					SimulationIsland simulationIsland3 = member.connections.Elements[k].entries.Elements[l].Member.SimulationIsland;
					if (simulationIsland3 != null)
					{
						if (simulationIsland != simulationIsland3)
						{
							simulationIsland = Merge(simulationIsland, simulationIsland3);
						}
						break;
					}
				}
			}
		}
		else
		{
			SimulationIsland simulationIsland4 = islandPool.Take();
			simulationIslands.Add(simulationIsland4);
			simulationIsland4.Add(member);
		}
	}
}
