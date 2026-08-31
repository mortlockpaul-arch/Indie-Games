using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.DataStructures;
using BEPUphysics.Threading;
using Microsoft.Xna.Framework;

namespace BEPUphysics.BroadPhaseSystems.SortAndSweep;

/// <summary>
/// Simple and standard implementation of the one-axis sort and sweep (sweep and prune) algorithm.
/// </summary>
/// <remarks>
/// In small scenarios, it can be the quickest option.  It uses very little memory.
/// However, it tends to scale poorly relative to other options and can slow down significantly when entries cluster along the axis.
/// Additionally, it supports no queries at all.
/// </remarks>
public class SortAndSweep1D : BroadPhase
{
	private RawList<BroadPhaseEntry> entries = new RawList<BroadPhaseEntry>();

	private Action<int> sweepSegment;

	private int sweepSegmentCount = 32;

	private int sortSegmentCount = 4;

	private RawList<BroadPhaseEntry> backbuffer;

	/// <summary>
	/// Constructs a new sort and sweep broad phase.
	/// </summary>
	/// <param name="threadManager">Thread manager to use in the broad phase.</param>
	public SortAndSweep1D(IThreadManager threadManager)
		: base(threadManager)
	{
		sweepSegment = Sweep;
		backbuffer = new RawList<BroadPhaseEntry>();
	}

	/// <summary>
	/// Constructs a new sort and sweep broad phase.
	/// </summary>
	public SortAndSweep1D()
	{
		sweepSegment = Sweep;
		backbuffer = new RawList<BroadPhaseEntry>();
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
		int num = 0;
		int num2 = entries.count;
		int num3 = 0;
		while (num2 - num > 0)
		{
			num3 = (num2 + num) / 2;
			if (entries.Elements[num3].boundingBox.Min.X > entry.boundingBox.Min.X)
			{
				num2 = num3;
				continue;
			}
			if (!(entries.Elements[num3].boundingBox.Min.X < entry.boundingBox.Min.X))
			{
				break;
			}
			num = ++num3;
		}
		entries.Insert(num3, entry);
	}

	/// <summary>
	/// Removes an entry from the broad phase.
	/// </summary>
	/// <param name="entry">Entry to remove.</param>
	public override void Remove(BroadPhaseEntry entry)
	{
		base.Remove(entry);
		entries.Remove(entry);
	}

	protected override void UpdateMultithreaded()
	{
		if (backbuffer.count != entries.count)
		{
			backbuffer.Capacity = entries.Capacity;
			backbuffer.count = entries.count;
		}
		base.Overlaps.Clear();
		for (int i = 1; i < entries.count; i++)
		{
			BroadPhaseEntry broadPhaseEntry = entries.Elements[i];
			int num = i - 1;
			while (num >= 0 && broadPhaseEntry.boundingBox.Min.X < entries.Elements[num].boundingBox.Min.X)
			{
				entries.Elements[num + 1] = entries.Elements[num];
				entries.Elements[num] = broadPhaseEntry;
				num--;
			}
		}
		base.ThreadManager.ForLoop(0, sweepSegmentCount, sweepSegment);
	}

	protected override void UpdateSingleThreaded()
	{
		base.Overlaps.Clear();
		for (int i = 1; i < entries.count; i++)
		{
			BroadPhaseEntry broadPhaseEntry = entries.Elements[i];
			int num = i - 1;
			while (num >= 0 && broadPhaseEntry.boundingBox.Min.X < entries.Elements[num].boundingBox.Min.X)
			{
				entries.Elements[num + 1] = entries.Elements[num];
				entries.Elements[num] = broadPhaseEntry;
				num--;
			}
		}
		for (int j = 0; j < entries.count; j++)
		{
			BoundingBox boundingBox = entries.Elements[j].boundingBox;
			for (int k = j + 1; k < entries.count && boundingBox.Max.X >= entries.Elements[k].boundingBox.Min.X; k++)
			{
				if (!(boundingBox.Min.Y > entries.Elements[k].boundingBox.Max.Y) && !(boundingBox.Max.Y < entries.Elements[k].boundingBox.Min.Y) && !(boundingBox.Min.Z > entries.Elements[k].boundingBox.Max.Z) && !(boundingBox.Max.Z < entries.Elements[k].boundingBox.Min.Z))
				{
					TryToAddOverlap(entries.Elements[j], entries.Elements[k]);
				}
			}
		}
	}

	private void Sweep(int segment)
	{
		int num = entries.count / sweepSegmentCount;
		int num2 = ((segment != sweepSegmentCount - 1) ? (num * (segment + 1)) : entries.count);
		for (int i = num * segment; i < num2; i++)
		{
			BoundingBox boundingBox = entries.Elements[i].boundingBox;
			for (int j = i + 1; j < entries.count && boundingBox.Max.X >= entries.Elements[j].boundingBox.Min.X; j++)
			{
				if (!(boundingBox.Min.Y > entries.Elements[j].boundingBox.Max.Y) && !(boundingBox.Max.Y < entries.Elements[j].boundingBox.Min.Y) && !(boundingBox.Min.Z > entries.Elements[j].boundingBox.Max.Z) && !(boundingBox.Max.Z < entries.Elements[j].boundingBox.Min.Z))
				{
					TryToAddOverlap(entries.Elements[i], entries.Elements[j]);
				}
			}
		}
	}

	private void SortSection(int section)
	{
		int num = entries.count / sortSegmentCount;
		int num2 = section * num;
		int num3 = ((section != sortSegmentCount - 1) ? (num * (section + 1)) : entries.count);
		for (int i = num2 + 1; i < num3; i++)
		{
			BroadPhaseEntry broadPhaseEntry = entries.Elements[i];
			int num4 = i - 1;
			while (num4 >= 0 && broadPhaseEntry.boundingBox.Min.X < entries.Elements[num4].boundingBox.Min.X)
			{
				entries.Elements[num4 + 1] = entries.Elements[num4];
				entries.Elements[num4] = broadPhaseEntry;
				num4--;
			}
		}
	}

	private void MergeSections(int a, int b)
	{
		int num = entries.count / sortSegmentCount;
		int num2 = num * a;
		int num3 = num * (a + 1);
		int num4 = num * b;
		int num5;
		int num6;
		if (b == sortSegmentCount - 1)
		{
			num5 = entries.count;
			num6 = num + entries.count - num4;
		}
		else
		{
			num5 = num * (b + 1);
			num6 = num * 2;
		}
		int num7 = num2;
		int num8 = num4;
		for (int i = 0; i < num6; i++)
		{
			int num9 = ((i < num) ? (num2 + i) : (num4 + i - num));
			if (num7 < num3 && num8 < num5)
			{
				if (entries.Elements[num7].boundingBox.Min.X < entries.Elements[num8].boundingBox.Min.X)
				{
					backbuffer.Elements[num9] = entries.Elements[num7++];
				}
				else
				{
					backbuffer.Elements[num9] = entries.Elements[num8++];
				}
			}
			else if (num7 < num3)
			{
				backbuffer.Elements[num9] = entries.Elements[num7++];
			}
			else
			{
				backbuffer.Elements[num9] = entries.Elements[num8++];
			}
		}
	}
}
