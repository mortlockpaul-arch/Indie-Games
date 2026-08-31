using System;
using System.Collections.Generic;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.DataStructures;
using BEPUphysics.ResourceManagement;
using BEPUphysics.Threading;
using Microsoft.Xna.Framework;

namespace BEPUphysics.BroadPhaseSystems.Hierarchies;

/// <summary>
/// Broad phase that incrementally updates the internal tree acceleration structure.
/// </summary>
/// <remarks>
/// This is a good all-around broad phase; its performance is consistent and all queries are supported and speedy.
/// The memory usage is higher than simple one-axis sort and sweep, but a bit lower than the Grid2DSortAndSweep option.
/// </remarks>
public class DynamicHierarchy : BroadPhase
{
	internal struct NodePair
	{
		internal Node a;

		internal Node b;
	}

	internal Node root;

	/// <summary>
	/// This is a few test-based values which help threaded scaling.
	/// By going deeper into the trees, a better distribution of work is achieved.
	/// Going above the tested core count theoretically benefits from a '0 if power of 2, 2 otherwise' rule of thumb.
	/// </summary>
	private int[] threadSplitOffsets = new int[4] { 2, 2, 2, 1 };

	private RawList<Node> multithreadingSourceNodes = new RawList<Node>(4);

	private Action<int> multithreadedRefit;

	private RawList<NodePair> multithreadingSourceOverlaps = new RawList<NodePair>(10);

	private Action<int> multithreadedOverlap;

	private UnsafeResourcePool<LeafNode> leafNodes = new UnsafeResourcePool<LeafNode>();

	/// <summary>
	/// Constructs a new dynamic hierarchy broad phase.
	/// </summary>
	public DynamicHierarchy()
	{
		multithreadedRefit = MultithreadedRefit;
		multithreadedOverlap = MultithreadedOverlap;
		base.QueryAccelerator = new DynamicHierarchyQueryAccelerator(this);
	}

	/// <summary>
	/// Constructs a new dynamic hierarchy broad phase.
	/// </summary>
	/// <param name="threadManager">Thread manager to use in the broad phase.</param>
	public DynamicHierarchy(IThreadManager threadManager)
		: base(threadManager)
	{
		multithreadedRefit = MultithreadedRefit;
		multithreadedOverlap = MultithreadedOverlap;
		base.QueryAccelerator = new DynamicHierarchyQueryAccelerator(this);
	}

	private void MultithreadedRefitPhase(int splitDepth)
	{
		if (splitDepth > 0)
		{
			root.CollectMultithreadingNodes(splitDepth, 1, multithreadingSourceNodes);
			base.ThreadManager.ForLoop(0, multithreadingSourceNodes.count, multithreadedRefit);
			multithreadingSourceNodes.Clear();
			root.PostRefit(splitDepth, 1);
		}
		else
		{
			SingleThreadedRefitPhase();
		}
	}

	private void MultithreadedOverlapPhase(int splitDepth)
	{
		if (splitDepth > 0)
		{
			if (!root.IsLeaf)
			{
				root.GetMultithreadedOverlaps(root, splitDepth, 1, this, multithreadingSourceOverlaps);
				base.ThreadManager.ForLoop(0, multithreadingSourceOverlaps.count, multithreadedOverlap);
				multithreadingSourceOverlaps.Clear();
			}
		}
		else
		{
			SingleThreadedOverlapPhase();
		}
	}

	protected override void UpdateMultithreaded()
	{
		lock (base.Locker)
		{
			base.Overlaps.Clear();
			if (root != null)
			{
				int num = ((base.ThreadManager.ThreadCount <= threadSplitOffsets.Length) ? threadSplitOffsets[base.ThreadManager.ThreadCount - 1] : (((base.ThreadManager.ThreadCount & (base.ThreadManager.ThreadCount - 1)) != 0) ? 2 : 0));
				int splitDepth = num + (int)Math.Ceiling(Math.Log(base.ThreadManager.ThreadCount, 2.0));
				MultithreadedRefitPhase(splitDepth);
				MultithreadedOverlapPhase(splitDepth);
			}
		}
	}

	private void MultithreadedRefit(int i)
	{
		multithreadingSourceNodes.Elements[i].Refit();
	}

	private void MultithreadedOverlap(int i)
	{
		NodePair nodePair = multithreadingSourceOverlaps.Elements[i];
		nodePair.a.GetOverlaps(nodePair.b, this);
	}

	private void SingleThreadedRefitPhase()
	{
		root.Refit();
	}

	private void SingleThreadedOverlapPhase()
	{
		if (!root.IsLeaf)
		{
			root.GetOverlaps(root, this);
		}
	}

	protected override void UpdateSingleThreaded()
	{
		lock (base.Locker)
		{
			base.Overlaps.Clear();
			if (root != null)
			{
				SingleThreadedRefitPhase();
				SingleThreadedOverlapPhase();
			}
		}
	}

	/// <summary>
	/// Adds an entry to the hierarchy.
	/// </summary>
	/// <param name="entry">Entry to remove.</param>
	public override void Add(BroadPhaseEntry entry)
	{
		base.Add(entry);
		Vector3.Subtract(ref entry.boundingBox.Max, ref entry.boundingBox.Min, out var result);
		if (result.X * result.Y * result.Z == 0f)
		{
			entry.UpdateBoundingBox();
		}
		LeafNode leafNode = leafNodes.Take();
		leafNode.Initialize(entry);
		if (root == null)
		{
			root = leafNode;
			return;
		}
		if (root.IsLeaf)
		{
			root.TryToInsert(leafNode, out root);
			return;
		}
		BoundingBox.CreateMerged(ref leafNode.BoundingBox, ref root.BoundingBox, out root.BoundingBox);
		InternalNode internalNode = (InternalNode)root;
		Vector3.Subtract(ref root.BoundingBox.Max, ref root.BoundingBox.Min, out result);
		internalNode.currentVolume = result.X * result.Y * result.Z;
		Node treeNode = root;
		while (!treeNode.TryToInsert(leafNode, out treeNode))
		{
		}
	}

	/// <summary>
	/// Removes an entry from the hierarchy.
	/// </summary>
	/// <param name="entry">Entry to remove.</param>
	public override void Remove(BroadPhaseEntry entry)
	{
		if (root == null)
		{
			throw new InvalidOperationException("Entry not present in the hierarchy.");
		}
		if (!RemoveFast(entry) && !RemoveBrute(entry))
		{
			throw new InvalidOperationException("Entry not present in the hierarchy.");
		}
	}

	internal bool RemoveFast(BroadPhaseEntry entry)
	{
		if (root.RemoveFast(entry, out var leafNode, out root))
		{
			leafNode.CleanUp();
			leafNodes.GiveBack(leafNode);
			base.Remove(entry);
			return true;
		}
		return false;
	}

	internal bool RemoveBrute(BroadPhaseEntry entry)
	{
		if (root.Remove(entry, out var leafNode, out root))
		{
			leafNode.CleanUp();
			leafNodes.GiveBack(leafNode);
			base.Remove(entry);
			return true;
		}
		return false;
	}

	internal void Analyze(List<int> depths, out int nodeCount)
	{
		nodeCount = 0;
		root.Analyze(depths, 0, ref nodeCount);
	}

	internal void ForceRevalidation()
	{
		((InternalNode)root).Revalidate();
	}
}
