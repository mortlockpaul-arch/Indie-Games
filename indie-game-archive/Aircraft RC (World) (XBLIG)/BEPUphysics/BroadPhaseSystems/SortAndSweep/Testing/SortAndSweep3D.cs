using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.DataStructures;
using Microsoft.Xna.Framework;

namespace BEPUphysics.BroadPhaseSystems.SortAndSweep.Testing;

internal class SortAndSweep3D : BroadPhase
{
	private RawList<BroadPhaseEntry> entriesX = new RawList<BroadPhaseEntry>();

	private RawList<BroadPhaseEntry> entriesY = new RawList<BroadPhaseEntry>();

	private RawList<BroadPhaseEntry> entriesZ = new RawList<BroadPhaseEntry>();

	private HashSet<BroadPhaseOverlap> overlapCandidatesX = new HashSet<BroadPhaseOverlap>();

	private HashSet<BroadPhaseOverlap> overlapCandidatesY = new HashSet<BroadPhaseOverlap>();

	public override void Add(BroadPhaseEntry entry)
	{
		base.Add(entry);
		int num = 0;
		int num2 = entriesX.count;
		int num3 = 0;
		while (num2 - num > 0)
		{
			num3 = (num2 + num) / 2;
			if (entriesX.Elements[num3].boundingBox.Min.X > entry.boundingBox.Min.X)
			{
				num2 = num3;
				continue;
			}
			if (!(entriesX.Elements[num3].boundingBox.Min.X < entry.boundingBox.Min.X))
			{
				break;
			}
			num = ++num3;
		}
		entriesX.Insert(num3, entry);
		num = 0;
		num2 = entriesY.count;
		while (num2 - num > 0)
		{
			num3 = (num2 + num) / 2;
			if (entriesY.Elements[num3].boundingBox.Min.Y > entry.boundingBox.Min.Y)
			{
				num2 = num3;
				continue;
			}
			if (!(entriesY.Elements[num3].boundingBox.Min.Y < entry.boundingBox.Min.Y))
			{
				break;
			}
			num = ++num3;
		}
		entriesY.Insert(num3, entry);
		num = 0;
		num2 = entriesZ.count;
		while (num2 - num > 0)
		{
			num3 = (num2 + num) / 2;
			if (entriesZ.Elements[num3].boundingBox.Min.Z > entry.boundingBox.Min.Z)
			{
				num2 = num3;
				continue;
			}
			if (!(entriesZ.Elements[num3].boundingBox.Min.Z < entry.boundingBox.Min.Z))
			{
				break;
			}
			num = ++num3;
		}
		entriesZ.Insert(num3, entry);
	}

	public override void Remove(BroadPhaseEntry entry)
	{
		base.Remove(entry);
		entriesX.Remove(entry);
		entriesY.Remove(entry);
		entriesZ.Remove(entry);
	}

	protected override void UpdateMultithreaded()
	{
		UpdateSingleThreaded();
	}

	protected override void UpdateSingleThreaded()
	{
		overlapCandidatesX.Clear();
		overlapCandidatesY.Clear();
		base.Overlaps.Clear();
		for (int i = 1; i < entriesX.count; i++)
		{
			BroadPhaseEntry broadPhaseEntry = entriesX.Elements[i];
			int num = i - 1;
			while (num >= 0 && broadPhaseEntry.boundingBox.Min.X < entriesX.Elements[num].boundingBox.Min.X)
			{
				entriesX.Elements[num + 1] = entriesX.Elements[num];
				entriesX.Elements[num] = broadPhaseEntry;
				num--;
			}
		}
		for (int j = 1; j < entriesY.count; j++)
		{
			BroadPhaseEntry broadPhaseEntry2 = entriesY.Elements[j];
			int num2 = j - 1;
			while (num2 >= 0 && broadPhaseEntry2.boundingBox.Min.Y < entriesY.Elements[num2].boundingBox.Min.Y)
			{
				entriesY.Elements[num2 + 1] = entriesY.Elements[num2];
				entriesY.Elements[num2] = broadPhaseEntry2;
				num2--;
			}
		}
		for (int k = 1; k < entriesZ.count; k++)
		{
			BroadPhaseEntry broadPhaseEntry3 = entriesZ.Elements[k];
			int num3 = k - 1;
			while (num3 >= 0 && broadPhaseEntry3.boundingBox.Min.Z < entriesZ.Elements[num3].boundingBox.Min.Z)
			{
				entriesZ.Elements[num3 + 1] = entriesZ.Elements[num3];
				entriesZ.Elements[num3] = broadPhaseEntry3;
				num3--;
			}
		}
		for (int l = 0; l < entriesX.count; l++)
		{
			BoundingBox boundingBox = entriesX.Elements[l].boundingBox;
			for (int m = l + 1; m < entriesX.count && boundingBox.Max.X > entriesX.Elements[m].boundingBox.Min.X; m++)
			{
				overlapCandidatesX.Add(new BroadPhaseOverlap(entriesX.Elements[l], entriesX.Elements[m]));
			}
		}
		for (int n = 0; n < entriesY.count; n++)
		{
			BoundingBox boundingBox2 = entriesY.Elements[n].boundingBox;
			for (int num4 = n + 1; num4 < entriesY.count && boundingBox2.Max.Y > entriesY.Elements[num4].boundingBox.Min.Y; num4++)
			{
				overlapCandidatesY.Add(new BroadPhaseOverlap(entriesY.Elements[n], entriesY.Elements[num4]));
			}
		}
		for (int num5 = 0; num5 < entriesZ.count; num5++)
		{
			BoundingBox boundingBox3 = entriesZ.Elements[num5].boundingBox;
			for (int num6 = num5 + 1; num6 < entriesZ.count && boundingBox3.Max.Z > entriesZ.Elements[num6].boundingBox.Min.Z; num6++)
			{
				BroadPhaseOverlap item = new BroadPhaseOverlap(entriesZ.Elements[num5], entriesZ.Elements[num6]);
				if (overlapCandidatesX.Contains(item) && overlapCandidatesY.Contains(item))
				{
					TryToAddOverlap(entriesZ.Elements[num5], entriesZ.Elements[num6]);
				}
			}
		}
	}
}
