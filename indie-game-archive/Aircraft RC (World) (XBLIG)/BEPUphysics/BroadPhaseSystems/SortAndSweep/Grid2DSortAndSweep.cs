using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.DataStructures;
using BEPUphysics.ResourceManagement;
using BEPUphysics.Threading;
using Microsoft.Xna.Framework;

namespace BEPUphysics.BroadPhaseSystems.SortAndSweep;

/// <summary>
/// Broad phase implementation that partitions objects into a 2d grid, and then performs a sort and sweep on the final axis.
/// </summary>
/// <remarks>
/// This broad phase typically has very good collision performance and scales well with multithreading, but its query times can sometimes be worse than tree-based systems
/// since it must scan cells.  Keeping rays as short as possible helps avoid unnecessary cell checks.
/// The performance can degrade noticeably in some situations involving significant off-axis motion.
/// </remarks>
public class Grid2DSortAndSweep : BroadPhase
{
	internal static float cellSizeInverse = 0.125f;

	internal SortedGrid2DSet cellSet = new SortedGrid2DSet();

	private RawList<Grid2DEntry> entries = new RawList<Grid2DEntry>();

	private Action<int> updateEntry;

	private Action<int> updateCell;

	private UnsafeResourcePool<Grid2DEntry> entryPool = new UnsafeResourcePool<Grid2DEntry>();

	private SpinLock cellSetLocker = new SpinLock();

	/// <summary>
	/// Gets or sets the width of cells in the 2D grid.  For sparser, larger scenes, increasing this can help performance.
	/// For denser scenes, decreasing this may help.
	/// </summary>
	public static float CellSize
	{
		get
		{
			return 1f / cellSizeInverse;
		}
		set
		{
			cellSizeInverse = 1f / value;
		}
	}

	internal static void ComputeCell(ref Vector3 v, out Int2 cell)
	{
		cell.Y = (int)Math.Floor(v.Y * cellSizeInverse);
		cell.Z = (int)Math.Floor(v.Z * cellSizeInverse);
	}

	/// <summary>
	/// Constructs a grid-based sort and sweep broad phase.
	/// </summary>
	/// <param name="threadManager">Thread manager to use for the broad phase.</param>
	public Grid2DSortAndSweep(IThreadManager threadManager)
		: base(threadManager)
	{
		updateEntry = UpdateEntry;
		updateCell = UpdateCell;
		base.QueryAccelerator = new Grid2DSortAndSweepQueryAccelerator(this);
	}

	/// <summary>
	/// Constructs a grid-based sort and sweep broad phase.
	/// </summary>
	public Grid2DSortAndSweep()
	{
		updateEntry = UpdateEntry;
		updateCell = UpdateCell;
		base.QueryAccelerator = new Grid2DSortAndSweepQueryAccelerator(this);
	}

	/// <summary>
	/// Adds an entry to the broad phase.
	/// </summary>
	/// <param name="entry">Entry to add.</param>
	public override void Add(BroadPhaseEntry entry)
	{
		base.Add(entry);
		Vector3.Subtract(ref entry.boundingBox.Max, ref entry.boundingBox.Min, out var result);
		if (result.X * result.Y * result.Z == 0f)
		{
			entry.UpdateBoundingBox();
		}
		Grid2DEntry grid2DEntry = entryPool.Take();
		grid2DEntry.Initialize(entry);
		entries.Add(grid2DEntry);
		for (int i = grid2DEntry.previousMin.Y; i <= grid2DEntry.previousMax.Y; i++)
		{
			for (int j = grid2DEntry.previousMin.Z; j <= grid2DEntry.previousMax.Z; j++)
			{
				Int2 index = new Int2
				{
					Y = i,
					Z = j
				};
				cellSet.Add(ref index, grid2DEntry);
			}
		}
	}

	/// <summary>
	/// Removes an entry from the broad phase.
	/// </summary>
	/// <param name="entry">Entry to remove.</param>
	public override void Remove(BroadPhaseEntry entry)
	{
		base.Remove(entry);
		for (int i = 0; i < entries.count; i++)
		{
			if (entries.Elements[i].item != entry)
			{
				continue;
			}
			Grid2DEntry grid2DEntry = entries.Elements[i];
			entries.RemoveAt(i);
			for (int j = grid2DEntry.previousMin.Y; j <= grid2DEntry.previousMax.Y; j++)
			{
				for (int k = grid2DEntry.previousMin.Z; k <= grid2DEntry.previousMax.Z; k++)
				{
					Int2 index = new Int2
					{
						Y = j,
						Z = k
					};
					cellSet.Remove(ref index, grid2DEntry);
				}
			}
			grid2DEntry.item = null;
			entryPool.GiveBack(grid2DEntry);
			break;
		}
	}

	protected override void UpdateMultithreaded()
	{
		lock (base.Locker)
		{
			base.Overlaps.Clear();
			base.ThreadManager.ForLoop(0, entries.count, updateEntry);
			base.ThreadManager.ForLoop(0, cellSet.count, updateCell);
		}
	}

	protected override void UpdateSingleThreaded()
	{
		lock (base.Locker)
		{
			base.Overlaps.Clear();
			for (int i = 0; i < entries.count; i++)
			{
				Grid2DEntry grid2DEntry = entries.Elements[i];
				ComputeCell(ref grid2DEntry.item.boundingBox.Min, out var cell);
				ComputeCell(ref grid2DEntry.item.boundingBox.Max, out var cell2);
				for (int j = grid2DEntry.previousMin.Y; j <= grid2DEntry.previousMax.Y; j++)
				{
					for (int k = grid2DEntry.previousMin.Z; k <= grid2DEntry.previousMax.Z; k++)
					{
						if (j < cell.Y || j > cell2.Y || k < cell.Z || k > cell2.Z)
						{
							Int2 index = new Int2
							{
								Y = j,
								Z = k
							};
							cellSet.Remove(ref index, grid2DEntry);
						}
					}
				}
				for (int l = cell.Y; l <= cell2.Y; l++)
				{
					for (int m = cell.Z; m <= cell2.Z; m++)
					{
						if (l < grid2DEntry.previousMin.Y || l > grid2DEntry.previousMax.Y || m < grid2DEntry.previousMin.Z || m > grid2DEntry.previousMax.Z)
						{
							Int2 index2 = new Int2
							{
								Y = l,
								Z = m
							};
							cellSet.Add(ref index2, grid2DEntry);
						}
					}
				}
				grid2DEntry.previousMin = cell;
				grid2DEntry.previousMax = cell2;
			}
			for (int n = 0; n < cellSet.count; n++)
			{
				cellSet.cells.Elements[n].UpdateOverlaps(this);
			}
		}
	}

	private void UpdateEntry(int i)
	{
		Grid2DEntry grid2DEntry = entries.Elements[i];
		ComputeCell(ref grid2DEntry.item.boundingBox.Min, out var cell);
		ComputeCell(ref grid2DEntry.item.boundingBox.Max, out var cell2);
		for (int j = grid2DEntry.previousMin.Y; j <= grid2DEntry.previousMax.Y; j++)
		{
			for (int k = grid2DEntry.previousMin.Z; k <= grid2DEntry.previousMax.Z; k++)
			{
				if (j < cell.Y || j > cell2.Y || k < cell.Z || k > cell2.Z)
				{
					Int2 index = new Int2
					{
						Y = j,
						Z = k
					};
					cellSetLocker.Enter();
					cellSet.Remove(ref index, grid2DEntry);
					cellSetLocker.Exit();
				}
			}
		}
		for (int l = cell.Y; l <= cell2.Y; l++)
		{
			for (int m = cell.Z; m <= cell2.Z; m++)
			{
				if (l < grid2DEntry.previousMin.Y || l > grid2DEntry.previousMax.Y || m < grid2DEntry.previousMin.Z || m > grid2DEntry.previousMax.Z)
				{
					Int2 index2 = new Int2
					{
						Y = l,
						Z = m
					};
					cellSetLocker.Enter();
					cellSet.Add(ref index2, grid2DEntry);
					cellSetLocker.Exit();
				}
			}
		}
		grid2DEntry.previousMin = cell;
		grid2DEntry.previousMax = cell2;
	}

	private void UpdateCell(int i)
	{
		cellSet.cells.Elements[i].UpdateOverlaps(this);
	}
}
