using System.Collections.Generic;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.DataStructures;
using Microsoft.Xna.Framework;

namespace BEPUphysics.BroadPhaseSystems.Hierarchies;

internal sealed class LeafNode : Node
{
	private BroadPhaseEntry element;

	internal override Node ChildA => null;

	internal override Node ChildB => null;

	internal override BroadPhaseEntry Element => element;

	internal override bool IsLeaf => true;

	internal void Initialize(BroadPhaseEntry element)
	{
		this.element = element;
		BoundingBox = element.BoundingBox;
	}

	internal void CleanUp()
	{
		element = null;
	}

	internal override void GetOverlaps(ref BoundingBox boundingBox, IList<BroadPhaseEntry> outputOverlappedElements)
	{
		outputOverlappedElements.Add(element);
	}

	internal override void GetOverlaps(ref BoundingSphere boundingSphere, IList<BroadPhaseEntry> outputOverlappedElements)
	{
		outputOverlappedElements.Add(element);
	}

	internal override void GetOverlaps(ref BoundingFrustum boundingFrustum, IList<BroadPhaseEntry> outputOverlappedElements)
	{
		outputOverlappedElements.Add(element);
	}

	internal override void GetOverlaps(ref Ray ray, float maximumLength, IList<BroadPhaseEntry> outputOverlappedElements)
	{
		outputOverlappedElements.Add(element);
	}

	internal override void GetOverlaps(Node opposingNode, DynamicHierarchy owner)
	{
		if (opposingNode.IsLeaf)
		{
			owner.TryToAddOverlap(element, opposingNode.Element);
			return;
		}
		Node childA = opposingNode.ChildA;
		Node childB = opposingNode.ChildB;
		BoundingBox.Intersects(ref childA.BoundingBox, out var result);
		if (result)
		{
			GetOverlaps(childA, owner);
		}
		BoundingBox.Intersects(ref childB.BoundingBox, out result);
		if (result)
		{
			GetOverlaps(childB, owner);
		}
	}

	internal override bool TryToInsert(LeafNode node, out Node treeNode)
	{
		InternalNode internalNode = InternalNode.nodePool.Take();
		BoundingBox.CreateMerged(ref BoundingBox, ref node.BoundingBox, out internalNode.BoundingBox);
		Vector3.Subtract(ref internalNode.BoundingBox.Max, ref internalNode.BoundingBox.Min, out var result);
		internalNode.currentVolume = result.X * result.Y * result.Z;
		internalNode.childA = this;
		internalNode.childB = node;
		treeNode = internalNode;
		return true;
	}

	public override string ToString()
	{
		return element.ToString();
	}

	internal override void Analyze(List<int> depths, int depth, ref int nodeCount)
	{
		nodeCount++;
		depths.Add(depth);
	}

	internal override void Refit()
	{
		BoundingBox = element.boundingBox;
	}

	internal override void RetrieveNodes(RawList<LeafNode> leafNodes)
	{
		Refit();
		leafNodes.Add(this);
	}

	internal override void CollectMultithreadingNodes(int splitDepth, int currentDepth, RawList<Node> multithreadingSourceNodes)
	{
	}

	internal override void PostRefit(int splitDepth, int currentDepth)
	{
		BoundingBox = element.boundingBox;
	}

	internal override void GetMultithreadedOverlaps(Node opposingNode, int splitDepth, int currentDepth, DynamicHierarchy owner, RawList<DynamicHierarchy.NodePair> multithreadingSourceOverlaps)
	{
		if (opposingNode.IsLeaf)
		{
			owner.TryToAddOverlap(element, opposingNode.Element);
			return;
		}
		Node childA = opposingNode.ChildA;
		Node childB = opposingNode.ChildB;
		bool result;
		if (splitDepth == currentDepth)
		{
			BoundingBox.Intersects(ref childA.BoundingBox, out result);
			if (result)
			{
				multithreadingSourceOverlaps.Add(new DynamicHierarchy.NodePair
				{
					a = this,
					b = childA
				});
			}
			BoundingBox.Intersects(ref childB.BoundingBox, out result);
			if (result)
			{
				multithreadingSourceOverlaps.Add(new DynamicHierarchy.NodePair
				{
					a = this,
					b = childB
				});
			}
		}
		else
		{
			BoundingBox.Intersects(ref childA.BoundingBox, out result);
			if (result)
			{
				GetOverlaps(childA, owner);
			}
			BoundingBox.Intersects(ref childB.BoundingBox, out result);
			if (result)
			{
				GetOverlaps(childB, owner);
			}
		}
	}

	internal override bool Remove(BroadPhaseEntry entry, out LeafNode leafNode, out Node replacementNode)
	{
		replacementNode = null;
		if (element == entry)
		{
			leafNode = this;
			return true;
		}
		leafNode = null;
		return false;
	}

	internal override bool RemoveFast(BroadPhaseEntry entry, out LeafNode leafNode, out Node replacementNode)
	{
		replacementNode = null;
		if (element == entry)
		{
			leafNode = this;
			return true;
		}
		leafNode = null;
		return false;
	}
}
