using System.Collections.Generic;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.DataStructures;
using Microsoft.Xna.Framework;

namespace BEPUphysics.BroadPhaseSystems.Hierarchies;

internal abstract class Node
{
	internal BoundingBox BoundingBox;

	internal abstract bool IsLeaf { get; }

	internal abstract Node ChildA { get; }

	internal abstract Node ChildB { get; }

	internal abstract BroadPhaseEntry Element { get; }

	internal abstract void GetOverlaps(ref BoundingBox boundingBox, IList<BroadPhaseEntry> outputOverlappedElements);

	internal abstract void GetOverlaps(ref BoundingSphere boundingSphere, IList<BroadPhaseEntry> outputOverlappedElements);

	internal abstract void GetOverlaps(ref BoundingFrustum boundingFrustum, IList<BroadPhaseEntry> outputOverlappedElements);

	internal abstract void GetOverlaps(ref Ray ray, float maximumLength, IList<BroadPhaseEntry> outputOverlappedElements);

	internal abstract void GetOverlaps(Node node, DynamicHierarchy owner);

	internal abstract bool TryToInsert(LeafNode node, out Node treeNode);

	internal abstract void Analyze(List<int> depths, int depth, ref int nodeCount);

	internal abstract void Refit();

	internal abstract void RetrieveNodes(RawList<LeafNode> leafNodes);

	internal abstract void CollectMultithreadingNodes(int splitDepth, int currentDepth, RawList<Node> multithreadingSourceNodes);

	internal abstract void PostRefit(int splitDepth, int currentDepth);

	internal abstract void GetMultithreadedOverlaps(Node opposingNode, int splitDepth, int currentDepth, DynamicHierarchy owner, RawList<DynamicHierarchy.NodePair> multithreadingSourceOverlaps);

	internal abstract bool Remove(BroadPhaseEntry entry, out LeafNode leafNode, out Node replacementNode);

	internal abstract bool RemoveFast(BroadPhaseEntry entry, out LeafNode leafNode, out Node replacementNode);
}
