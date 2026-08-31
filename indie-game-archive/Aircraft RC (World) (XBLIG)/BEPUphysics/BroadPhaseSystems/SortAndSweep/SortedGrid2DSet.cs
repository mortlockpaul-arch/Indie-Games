using BEPUphysics.DataStructures;
using BEPUphysics.ResourceManagement;

namespace BEPUphysics.BroadPhaseSystems.SortAndSweep;

internal class SortedGrid2DSet
{
	internal RawList<GridCell2D> cells = new RawList<GridCell2D>();

	private UnsafeResourcePool<GridCell2D> cellPool = new UnsafeResourcePool<GridCell2D>();

	internal int count;

	internal bool TryGetIndex(ref Int2 cellIndex, out int index, out int sortingHash)
	{
		sortingHash = cellIndex.GetSortingHash();
		int num = 0;
		int num2 = count;
		index = 0;
		while (num2 - num > 0)
		{
			index = (num2 + num) / 2;
			if (cells.Elements[index].sortingHash > sortingHash)
			{
				num2 = index;
			}
			else if (cells.Elements[index].sortingHash < sortingHash)
			{
				num = ++index;
			}
			else if (cells.Elements[index].cellIndex.Y == cellIndex.Y && cells.Elements[index].cellIndex.Z == cellIndex.Z)
			{
				return true;
			}
		}
		return false;
	}

	internal bool TryGetCell(ref Int2 cellIndex, out GridCell2D cell)
	{
		if (TryGetIndex(ref cellIndex, out var index, out var _))
		{
			cell = cells.Elements[index];
			return true;
		}
		cell = null;
		return false;
	}

	internal void Add(ref Int2 index, Grid2DEntry entry)
	{
		if (TryGetIndex(ref index, out var index2, out var sortingHash))
		{
			cells.Elements[index2].Add(entry);
			return;
		}
		GridCell2D gridCell2D = cellPool.Take();
		gridCell2D.Initialize(ref index, sortingHash);
		gridCell2D.Add(entry);
		cells.Insert(index2, gridCell2D);
		count++;
	}

	internal void Remove(ref Int2 index, Grid2DEntry entry)
	{
		if (TryGetIndex(ref index, out var index2, out var _))
		{
			cells.Elements[index2].Remove(entry);
			if (cells.Elements[index2].entries.count == 0)
			{
				GridCell2D item = cells.Elements[index2];
				cells.RemoveAt(index2);
				cellPool.GiveBack(item);
				count--;
			}
		}
	}
}
