using BEPUphysics.DataStructures;

namespace BEPUphysics.BroadPhaseSystems.SortAndSweep;

internal class GridCell2D
{
	internal RawList<Grid2DEntry> entries = new RawList<Grid2DEntry>();

	internal Int2 cellIndex;

	internal int sortingHash;

	internal void Initialize(ref Int2 cellIndex, int hash)
	{
		this.cellIndex = cellIndex;
		sortingHash = hash;
	}

	internal int GetIndex(float x)
	{
		int num = 0;
		int num2 = entries.count;
		int num3 = 0;
		while (num2 - num > 0)
		{
			num3 = (num2 + num) / 2;
			if (entries.Elements[num3].item.boundingBox.Min.X > x)
			{
				num2 = num3;
				continue;
			}
			if (!(entries.Elements[num3].item.boundingBox.Min.X < x))
			{
				break;
			}
			num = ++num3;
		}
		return num3;
	}

	internal void Add(Grid2DEntry entry)
	{
		entries.Insert(GetIndex(entry.item.boundingBox.Min.X), entry);
	}

	internal void Remove(Grid2DEntry entry)
	{
		entries.Remove(entry);
	}

	internal void UpdateOverlaps(Grid2DSortAndSweep owner)
	{
		for (int i = 1; i < entries.count; i++)
		{
			Grid2DEntry grid2DEntry = entries.Elements[i];
			int num = i - 1;
			while (num >= 0 && grid2DEntry.item.boundingBox.Min.X < entries.Elements[num].item.boundingBox.Min.X)
			{
				entries.Elements[num + 1] = entries.Elements[num];
				entries.Elements[num] = grid2DEntry;
				num--;
			}
		}
		for (int j = 0; j < entries.count; j++)
		{
			Grid2DEntry grid2DEntry2 = entries.Elements[j];
			for (int k = j + 1; k < entries.count; k++)
			{
				Grid2DEntry grid2DEntry3;
				if (!(grid2DEntry2.item.boundingBox.Max.X >= (grid2DEntry3 = entries.Elements[k]).item.boundingBox.Min.X))
				{
					break;
				}
				if (!(grid2DEntry2.item.boundingBox.Min.Y > grid2DEntry3.item.boundingBox.Max.Y) && !(grid2DEntry2.item.boundingBox.Max.Y < grid2DEntry3.item.boundingBox.Min.Y) && !(grid2DEntry2.item.boundingBox.Min.Z > grid2DEntry3.item.boundingBox.Max.Z) && !(grid2DEntry2.item.boundingBox.Max.Z < grid2DEntry3.item.boundingBox.Min.Z))
				{
					Int2 previousMin = grid2DEntry2.previousMin;
					if (previousMin.Y < grid2DEntry3.previousMin.Y)
					{
						previousMin.Y = grid2DEntry3.previousMin.Y;
					}
					if (previousMin.Y > grid2DEntry3.previousMax.Y)
					{
						previousMin.Y = grid2DEntry3.previousMax.Y;
					}
					if (previousMin.Z < grid2DEntry3.previousMin.Z)
					{
						previousMin.Z = grid2DEntry3.previousMin.Z;
					}
					if (previousMin.Z > grid2DEntry3.previousMax.Z)
					{
						previousMin.Z = grid2DEntry3.previousMax.Z;
					}
					if (previousMin.Y == cellIndex.Y && previousMin.Z == cellIndex.Z)
					{
						owner.TryToAddOverlap(grid2DEntry2.item, grid2DEntry3.item);
					}
				}
			}
		}
	}

	public override string ToString()
	{
		return "{" + cellIndex.Y + ", " + cellIndex.Z + "}: " + entries.count;
	}
}
