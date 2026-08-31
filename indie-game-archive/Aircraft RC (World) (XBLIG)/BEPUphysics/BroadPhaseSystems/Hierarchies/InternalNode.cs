using System;
using System.Collections.Generic;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.DataStructures;
using BEPUphysics.ResourceManagement;
using Microsoft.Xna.Framework;

namespace BEPUphysics.BroadPhaseSystems.Hierarchies;

internal sealed class InternalNode : Node
{
	private class XComparer : IComparer<LeafNode>
	{
		public int Compare(LeafNode x, LeafNode y)
		{
			if (!(x.BoundingBox.Min.X < y.BoundingBox.Min.X))
			{
				return 1;
			}
			return -1;
		}
	}

	private class YComparer : IComparer<LeafNode>
	{
		public int Compare(LeafNode x, LeafNode y)
		{
			if (!(x.BoundingBox.Min.Y < y.BoundingBox.Min.Y))
			{
				return 1;
			}
			return -1;
		}
	}

	private class ZComparer : IComparer<LeafNode>
	{
		public int Compare(LeafNode x, LeafNode y)
		{
			if (!(x.BoundingBox.Min.Z < y.BoundingBox.Min.Z))
			{
				return 1;
			}
			return -1;
		}
	}

	internal Node childA;

	internal Node childB;

	internal float currentVolume;

	internal float maximumVolume;

	internal static float MaximumVolumeScale = 1.4f;

	internal static LockingResourcePool<InternalNode> nodePool = new LockingResourcePool<InternalNode>();

	internal static LockingResourcePool<RawList<LeafNode>> nodeListPool = new LockingResourcePool<RawList<LeafNode>>();

	private static XComparer xComparer = new XComparer();

	private static YComparer yComparer = new YComparer();

	private static ZComparer zComparer = new ZComparer();

	internal override Node ChildA => childA;

	internal override Node ChildB => childB;

	internal override BroadPhaseEntry Element => null;

	internal override bool IsLeaf => false;

	internal override void GetOverlaps(ref BoundingBox boundingBox, IList<BroadPhaseEntry> outputOverlappedElements)
	{
		childA.BoundingBox.Intersects(ref boundingBox, out var result);
		if (result)
		{
			childA.GetOverlaps(ref boundingBox, outputOverlappedElements);
		}
		childB.BoundingBox.Intersects(ref boundingBox, out result);
		if (result)
		{
			childB.GetOverlaps(ref boundingBox, outputOverlappedElements);
		}
	}

	internal override void GetOverlaps(ref BoundingSphere boundingSphere, IList<BroadPhaseEntry> outputOverlappedElements)
	{
		childA.BoundingBox.Intersects(ref boundingSphere, out var result);
		if (result)
		{
			childA.GetOverlaps(ref boundingSphere, outputOverlappedElements);
		}
		childB.BoundingBox.Intersects(ref boundingSphere, out result);
		if (result)
		{
			childB.GetOverlaps(ref boundingSphere, outputOverlappedElements);
		}
	}

	internal override void GetOverlaps(ref BoundingFrustum boundingFrustum, IList<BroadPhaseEntry> outputOverlappedElements)
	{
		boundingFrustum.Intersects(ref childA.BoundingBox, out var result);
		if (result)
		{
			childA.GetOverlaps(ref boundingFrustum, outputOverlappedElements);
		}
		boundingFrustum.Intersects(ref childB.BoundingBox, out result);
		if (result)
		{
			childB.GetOverlaps(ref boundingFrustum, outputOverlappedElements);
		}
	}

	internal override void GetOverlaps(ref Ray ray, float maximumLength, IList<BroadPhaseEntry> outputOverlappedElements)
	{
		ray.Intersects(ref childA.BoundingBox, out var result);
		if (result.HasValue && result < maximumLength)
		{
			childA.GetOverlaps(ref ray, maximumLength, outputOverlappedElements);
		}
		ray.Intersects(ref childB.BoundingBox, out result);
		if (result.HasValue && result < maximumLength)
		{
			childB.GetOverlaps(ref ray, maximumLength, outputOverlappedElements);
		}
	}

	internal override void GetOverlaps(Node opposingNode, DynamicHierarchy owner)
	{
		bool result;
		if (this == opposingNode)
		{
			if (!childA.IsLeaf)
			{
				childA.GetOverlaps(childA, owner);
			}
			if (!childB.IsLeaf)
			{
				childB.GetOverlaps(childB, owner);
			}
			childA.BoundingBox.Intersects(ref childB.BoundingBox, out result);
			if (result)
			{
				childA.GetOverlaps(childB, owner);
			}
			return;
		}
		if (opposingNode.IsLeaf)
		{
			childA.BoundingBox.Intersects(ref opposingNode.BoundingBox, out result);
			if (result)
			{
				childA.GetOverlaps(opposingNode, owner);
			}
			childB.BoundingBox.Intersects(ref opposingNode.BoundingBox, out result);
			if (result)
			{
				childB.GetOverlaps(opposingNode, owner);
			}
			return;
		}
		Node node = opposingNode.ChildA;
		Node node2 = opposingNode.ChildB;
		childA.BoundingBox.Intersects(ref node.BoundingBox, out result);
		if (result)
		{
			childA.GetOverlaps(node, owner);
		}
		childA.BoundingBox.Intersects(ref node2.BoundingBox, out result);
		if (result)
		{
			childA.GetOverlaps(node2, owner);
		}
		childB.BoundingBox.Intersects(ref node.BoundingBox, out result);
		if (result)
		{
			childB.GetOverlaps(node, owner);
		}
		childB.BoundingBox.Intersects(ref node2.BoundingBox, out result);
		if (result)
		{
			childB.GetOverlaps(node2, owner);
		}
	}

	internal override bool TryToInsert(LeafNode node, out Node treeNode)
	{
		BoundingBox.CreateMerged(ref childA.BoundingBox, ref node.BoundingBox, out var result);
		BoundingBox.CreateMerged(ref childB.BoundingBox, ref node.BoundingBox, out var result2);
		Vector3.Subtract(ref childA.BoundingBox.Max, ref childA.BoundingBox.Min, out var result3);
		float num = result3.X * result3.Y * result3.Z;
		Vector3.Subtract(ref childB.BoundingBox.Max, ref childB.BoundingBox.Min, out result3);
		float num2 = result3.X * result3.Y * result3.Z;
		Vector3.Subtract(ref result.Max, ref result.Min, out result3);
		float num3 = result3.X * result3.Y * result3.Z;
		Vector3.Subtract(ref result2.Max, ref result2.Min, out result3);
		float num4 = result3.X * result3.Y * result3.Z;
		if (num3 - num < num4 - num2)
		{
			if (childA.IsLeaf)
			{
				InternalNode internalNode = nodePool.Take();
				internalNode.BoundingBox = result;
				internalNode.childA = childA;
				internalNode.childB = node;
				internalNode.currentVolume = num3;
				childA = internalNode;
				treeNode = null;
				return true;
			}
			childA.BoundingBox = result;
			InternalNode internalNode2 = (InternalNode)childA;
			internalNode2.currentVolume = num3;
			treeNode = childA;
			return false;
		}
		if (childB.IsLeaf)
		{
			InternalNode internalNode3 = nodePool.Take();
			internalNode3.BoundingBox = result2;
			internalNode3.childA = node;
			internalNode3.childB = childB;
			internalNode3.currentVolume = num4;
			childB = internalNode3;
			treeNode = null;
			return true;
		}
		childB.BoundingBox = result2;
		treeNode = childB;
		InternalNode internalNode4 = (InternalNode)childB;
		internalNode4.currentVolume = num4;
		return false;
	}

	public override string ToString()
	{
		return string.Concat("{", childA, ", ", childB, "}");
	}

	internal override void Analyze(List<int> depths, int depth, ref int nodeCount)
	{
		nodeCount++;
		childA.Analyze(depths, depth + 1, ref nodeCount);
		childB.Analyze(depths, depth + 1, ref nodeCount);
	}

	internal override void Refit()
	{
		if (currentVolume > maximumVolume)
		{
			Revalidate();
			return;
		}
		childA.Refit();
		childB.Refit();
		BoundingBox.CreateMerged(ref childA.BoundingBox, ref childB.BoundingBox, out BoundingBox);
		currentVolume = (BoundingBox.Max.X - BoundingBox.Min.X) * (BoundingBox.Max.Y - BoundingBox.Min.Y) * (BoundingBox.Max.Z - BoundingBox.Min.Z);
	}

	internal void Revalidate()
	{
		Node node = childA;
		Node node2 = childB;
		childA = null;
		childB = null;
		RawList<LeafNode> rawList = nodeListPool.Take();
		node.RetrieveNodes(rawList);
		node2.RetrieveNodes(rawList);
		for (int i = 0; i < rawList.count; i++)
		{
			rawList.Elements[i].Refit();
		}
		Reconstruct(rawList, 0, rawList.count);
		rawList.Clear();
		nodeListPool.GiveBack(rawList);
	}

	private void Reconstruct(RawList<LeafNode> leafNodes, int begin, int end)
	{
		BoundingBox.CreateMerged(ref leafNodes.Elements[begin].BoundingBox, ref leafNodes.Elements[begin + 1].BoundingBox, out BoundingBox);
		for (int i = begin + 2; i < end; i++)
		{
			BoundingBox.CreateMerged(ref BoundingBox, ref leafNodes.Elements[i].BoundingBox, out BoundingBox);
		}
		Vector3.Subtract(ref BoundingBox.Max, ref BoundingBox.Min, out var result);
		currentVolume = result.X * result.Y * result.Z;
		maximumVolume = currentVolume * MaximumVolumeScale;
		if (result.X > result.Y && result.X > result.Z)
		{
			Array.Sort(leafNodes.Elements, begin, end - begin, xComparer);
		}
		else if (result.Y > result.Z)
		{
			Array.Sort(leafNodes.Elements, begin, end - begin, yComparer);
		}
		else
		{
			Array.Sort(leafNodes.Elements, begin, end - begin, zComparer);
		}
		int num = (begin + end) / 2;
		if (num - begin >= 2)
		{
			InternalNode internalNode = nodePool.Take();
			internalNode.Reconstruct(leafNodes, begin, num);
			childA = internalNode;
		}
		else
		{
			childA = leafNodes.Elements[begin];
		}
		if (end - num >= 2)
		{
			InternalNode internalNode2 = nodePool.Take();
			internalNode2.Reconstruct(leafNodes, num, end);
			childB = internalNode2;
		}
		else
		{
			childB = leafNodes.Elements[num];
		}
	}

	internal override void RetrieveNodes(RawList<LeafNode> leafNodes)
	{
		Node node = childA;
		Node node2 = childB;
		childA = null;
		childB = null;
		nodePool.GiveBack(this);
		node.RetrieveNodes(leafNodes);
		node2.RetrieveNodes(leafNodes);
	}

	internal override void CollectMultithreadingNodes(int splitDepth, int currentDepth, RawList<Node> multithreadingSourceNodes)
	{
		if (currentVolume > maximumVolume)
		{
			Revalidate();
		}
		else if (currentDepth == splitDepth)
		{
			multithreadingSourceNodes.Add(childA);
			multithreadingSourceNodes.Add(childB);
		}
		else
		{
			childA.CollectMultithreadingNodes(splitDepth, currentDepth + 1, multithreadingSourceNodes);
			childB.CollectMultithreadingNodes(splitDepth, currentDepth + 1, multithreadingSourceNodes);
		}
	}

	internal override void PostRefit(int splitDepth, int currentDepth)
	{
		if (splitDepth > currentDepth)
		{
			childA.PostRefit(splitDepth, currentDepth + 1);
			childB.PostRefit(splitDepth, currentDepth + 1);
		}
		BoundingBox.CreateMerged(ref childA.BoundingBox, ref childB.BoundingBox, out BoundingBox);
		currentVolume = (BoundingBox.Max.X - BoundingBox.Min.X) * (BoundingBox.Max.Y - BoundingBox.Min.Y) * (BoundingBox.Max.Z - BoundingBox.Min.Z);
	}

	internal override void GetMultithreadedOverlaps(Node opposingNode, int splitDepth, int currentDepth, DynamicHierarchy owner, RawList<DynamicHierarchy.NodePair> multithreadingSourceOverlaps)
	{
		bool result;
		if (currentDepth == splitDepth)
		{
			if (this == opposingNode)
			{
				if (!childA.IsLeaf)
				{
					multithreadingSourceOverlaps.Add(new DynamicHierarchy.NodePair
					{
						a = childA,
						b = childA
					});
				}
				if (!childB.IsLeaf)
				{
					multithreadingSourceOverlaps.Add(new DynamicHierarchy.NodePair
					{
						a = childB,
						b = childB
					});
				}
				childA.BoundingBox.Intersects(ref childB.BoundingBox, out result);
				if (result)
				{
					multithreadingSourceOverlaps.Add(new DynamicHierarchy.NodePair
					{
						a = childA,
						b = childB
					});
				}
				return;
			}
			if (opposingNode.IsLeaf)
			{
				childA.BoundingBox.Intersects(ref opposingNode.BoundingBox, out result);
				if (result)
				{
					multithreadingSourceOverlaps.Add(new DynamicHierarchy.NodePair
					{
						a = childA,
						b = opposingNode
					});
				}
				childB.BoundingBox.Intersects(ref opposingNode.BoundingBox, out result);
				if (result)
				{
					multithreadingSourceOverlaps.Add(new DynamicHierarchy.NodePair
					{
						a = childB,
						b = opposingNode
					});
				}
				return;
			}
			Node node = opposingNode.ChildA;
			Node node2 = opposingNode.ChildB;
			childA.BoundingBox.Intersects(ref node.BoundingBox, out result);
			if (result)
			{
				multithreadingSourceOverlaps.Add(new DynamicHierarchy.NodePair
				{
					a = childA,
					b = node
				});
			}
			childA.BoundingBox.Intersects(ref node2.BoundingBox, out result);
			if (result)
			{
				multithreadingSourceOverlaps.Add(new DynamicHierarchy.NodePair
				{
					a = childA,
					b = node2
				});
			}
			childB.BoundingBox.Intersects(ref node.BoundingBox, out result);
			if (result)
			{
				multithreadingSourceOverlaps.Add(new DynamicHierarchy.NodePair
				{
					a = childB,
					b = node
				});
			}
			childB.BoundingBox.Intersects(ref node2.BoundingBox, out result);
			if (result)
			{
				multithreadingSourceOverlaps.Add(new DynamicHierarchy.NodePair
				{
					a = childB,
					b = node2
				});
			}
		}
		else if (this == opposingNode)
		{
			if (!childA.IsLeaf)
			{
				childA.GetMultithreadedOverlaps(childA, splitDepth, currentDepth + 1, owner, multithreadingSourceOverlaps);
			}
			if (!childB.IsLeaf)
			{
				childB.GetMultithreadedOverlaps(childB, splitDepth, currentDepth + 1, owner, multithreadingSourceOverlaps);
			}
			childA.BoundingBox.Intersects(ref childB.BoundingBox, out result);
			if (result)
			{
				childA.GetMultithreadedOverlaps(childB, splitDepth, currentDepth + 1, owner, multithreadingSourceOverlaps);
			}
		}
		else if (opposingNode.IsLeaf)
		{
			childA.BoundingBox.Intersects(ref opposingNode.BoundingBox, out result);
			if (result)
			{
				childA.GetMultithreadedOverlaps(opposingNode, splitDepth, currentDepth + 1, owner, multithreadingSourceOverlaps);
			}
			childB.BoundingBox.Intersects(ref opposingNode.BoundingBox, out result);
			if (result)
			{
				childB.GetMultithreadedOverlaps(opposingNode, splitDepth, currentDepth + 1, owner, multithreadingSourceOverlaps);
			}
		}
		else
		{
			Node node3 = opposingNode.ChildA;
			Node node4 = opposingNode.ChildB;
			childA.BoundingBox.Intersects(ref node3.BoundingBox, out result);
			if (result)
			{
				childA.GetMultithreadedOverlaps(node3, splitDepth, currentDepth + 1, owner, multithreadingSourceOverlaps);
			}
			childA.BoundingBox.Intersects(ref node4.BoundingBox, out result);
			if (result)
			{
				childA.GetMultithreadedOverlaps(node4, splitDepth, currentDepth + 1, owner, multithreadingSourceOverlaps);
			}
			childB.BoundingBox.Intersects(ref node3.BoundingBox, out result);
			if (result)
			{
				childB.GetMultithreadedOverlaps(node3, splitDepth, currentDepth + 1, owner, multithreadingSourceOverlaps);
			}
			childB.BoundingBox.Intersects(ref node4.BoundingBox, out result);
			if (result)
			{
				childB.GetMultithreadedOverlaps(node4, splitDepth, currentDepth + 1, owner, multithreadingSourceOverlaps);
			}
		}
	}

	internal override bool Remove(BroadPhaseEntry entry, out LeafNode leafNode, out Node replacementNode)
	{
		if (childA.Remove(entry, out leafNode, out replacementNode))
		{
			if (childA.IsLeaf)
			{
				replacementNode = childB;
			}
			else
			{
				childA = replacementNode;
				replacementNode = this;
			}
			return true;
		}
		if (childB.Remove(entry, out leafNode, out replacementNode))
		{
			if (childB.IsLeaf)
			{
				replacementNode = childA;
			}
			else
			{
				childB = replacementNode;
				replacementNode = this;
			}
			return true;
		}
		replacementNode = this;
		return false;
	}

	internal override bool RemoveFast(BroadPhaseEntry entry, out LeafNode leafNode, out Node replacementNode)
	{
		childA.BoundingBox.Intersects(ref entry.boundingBox, out var result);
		if (result && childA.RemoveFast(entry, out leafNode, out replacementNode))
		{
			if (childA.IsLeaf)
			{
				replacementNode = childB;
			}
			else
			{
				childA = replacementNode;
				replacementNode = this;
			}
			return true;
		}
		childB.BoundingBox.Intersects(ref entry.boundingBox, out result);
		if (result && childB.RemoveFast(entry, out leafNode, out replacementNode))
		{
			if (childB.IsLeaf)
			{
				replacementNode = childA;
			}
			else
			{
				childB = replacementNode;
				replacementNode = this;
			}
			return true;
		}
		replacementNode = this;
		leafNode = null;
		return false;
	}
}
