using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;

namespace BEPUphysics.DataStructures;

/// <summary>
///  Acceleration structure of triangles surrounded by axis aligned bounding boxes, supporting various speedy queries.
/// </summary>
public class MeshBoundingBoxTree
{
	private abstract class Node
	{
		internal BoundingBox BoundingBox;

		internal abstract bool IsLeaf { get; }

		internal abstract void GetOverlaps(ref BoundingBox boundingBox, IList<int> outputOverlappedElements);

		internal abstract void GetOverlaps(ref BoundingSphere boundingSphere, IList<int> outputOverlappedElements);

		internal abstract void GetOverlaps(ref BoundingFrustum boundingFrustum, IList<int> outputOverlappedElements);

		internal abstract void GetOverlaps(ref Ray ray, float maximumLength, IList<int> outputOverlappedElements);

		internal abstract bool TryToInsert(LeafNode node, out Node treeNode);

		internal abstract void Analyze(List<int> depths, int depth, ref int nodeCount);

		internal abstract void Refit(MeshBoundingBoxTreeData data);
	}

	private sealed class InternalNode : Node
	{
		internal Node ChildA;

		internal Node ChildB;

		internal override bool IsLeaf => false;

		internal override void GetOverlaps(ref BoundingBox boundingBox, IList<int> outputOverlappedElements)
		{
			ChildA.BoundingBox.Intersects(ref boundingBox, out var result);
			if (result)
			{
				ChildA.GetOverlaps(ref boundingBox, outputOverlappedElements);
			}
			ChildB.BoundingBox.Intersects(ref boundingBox, out result);
			if (result)
			{
				ChildB.GetOverlaps(ref boundingBox, outputOverlappedElements);
			}
		}

		internal override void GetOverlaps(ref BoundingSphere boundingSphere, IList<int> outputOverlappedElements)
		{
			ChildA.BoundingBox.Intersects(ref boundingSphere, out var result);
			if (result)
			{
				ChildA.GetOverlaps(ref boundingSphere, outputOverlappedElements);
			}
			ChildB.BoundingBox.Intersects(ref boundingSphere, out result);
			if (result)
			{
				ChildB.GetOverlaps(ref boundingSphere, outputOverlappedElements);
			}
		}

		internal override void GetOverlaps(ref BoundingFrustum boundingFrustum, IList<int> outputOverlappedElements)
		{
			boundingFrustum.Intersects(ref ChildA.BoundingBox, out var result);
			if (result)
			{
				ChildA.GetOverlaps(ref boundingFrustum, outputOverlappedElements);
			}
			boundingFrustum.Intersects(ref ChildB.BoundingBox, out result);
			if (result)
			{
				ChildB.GetOverlaps(ref boundingFrustum, outputOverlappedElements);
			}
		}

		internal override void GetOverlaps(ref Ray ray, float maximumLength, IList<int> outputOverlappedElements)
		{
			ray.Intersects(ref ChildA.BoundingBox, out var result);
			if (result.HasValue && result < maximumLength)
			{
				ChildA.GetOverlaps(ref ray, maximumLength, outputOverlappedElements);
			}
			ray.Intersects(ref ChildB.BoundingBox, out result);
			if (result.HasValue && result < maximumLength)
			{
				ChildB.GetOverlaps(ref ray, maximumLength, outputOverlappedElements);
			}
		}

		internal override bool TryToInsert(LeafNode node, out Node treeNode)
		{
			BoundingBox.CreateMerged(ref ChildA.BoundingBox, ref node.BoundingBox, out var result);
			BoundingBox.CreateMerged(ref ChildB.BoundingBox, ref node.BoundingBox, out var result2);
			Vector3.Subtract(ref ChildA.BoundingBox.Max, ref ChildA.BoundingBox.Min, out var result3);
			float num = result3.X * result3.Y * result3.Z;
			Vector3.Subtract(ref ChildB.BoundingBox.Max, ref ChildB.BoundingBox.Min, out result3);
			float num2 = result3.X * result3.Y * result3.Z;
			Vector3.Subtract(ref result.Max, ref result.Min, out result3);
			float num3 = result3.X * result3.Y * result3.Z;
			Vector3.Subtract(ref result2.Max, ref result2.Min, out result3);
			float num4 = result3.X * result3.Y * result3.Z;
			if (num3 - num < num4 - num2)
			{
				if (ChildA.IsLeaf)
				{
					ChildA = new InternalNode
					{
						BoundingBox = result,
						ChildA = ChildA,
						ChildB = node
					};
					treeNode = null;
					return true;
				}
				ChildA.BoundingBox = result;
				treeNode = ChildA;
				return false;
			}
			if (ChildB.IsLeaf)
			{
				ChildB = new InternalNode
				{
					BoundingBox = result2,
					ChildA = node,
					ChildB = ChildB
				};
				treeNode = null;
				return true;
			}
			ChildB.BoundingBox = result2;
			treeNode = ChildB;
			return false;
		}

		public override string ToString()
		{
			return "{" + ChildA.ToString() + ", " + ChildB.ToString() + "}";
		}

		internal override void Analyze(List<int> depths, int depth, ref int nodeCount)
		{
			nodeCount++;
			ChildA.Analyze(depths, depth + 1, ref nodeCount);
			ChildB.Analyze(depths, depth + 1, ref nodeCount);
		}

		internal override void Refit(MeshBoundingBoxTreeData data)
		{
			ChildA.Refit(data);
			ChildB.Refit(data);
			BoundingBox.CreateMerged(ref ChildA.BoundingBox, ref ChildB.BoundingBox, out BoundingBox);
		}
	}

	private sealed class LeafNode : Node
	{
		private int LeafIndex;

		internal override bool IsLeaf => true;

		internal LeafNode(int leafIndex, MeshBoundingBoxTreeData data)
		{
			LeafIndex = leafIndex;
			data.GetBoundingBox(leafIndex, out BoundingBox);
			BoundingBox.Max.X += LeafMargin;
			BoundingBox.Max.Y += LeafMargin;
			BoundingBox.Max.Z += LeafMargin;
			BoundingBox.Min.X -= LeafMargin;
			BoundingBox.Min.Y -= LeafMargin;
			BoundingBox.Min.Z -= LeafMargin;
		}

		internal override void GetOverlaps(ref BoundingBox boundingBox, IList<int> outputOverlappedElements)
		{
			outputOverlappedElements.Add(LeafIndex);
		}

		internal override void GetOverlaps(ref BoundingSphere boundingSphere, IList<int> outputOverlappedElements)
		{
			outputOverlappedElements.Add(LeafIndex);
		}

		internal override void GetOverlaps(ref BoundingFrustum boundingFrustum, IList<int> outputOverlappedElements)
		{
			outputOverlappedElements.Add(LeafIndex);
		}

		internal override void GetOverlaps(ref Ray ray, float maximumLength, IList<int> outputOverlappedElements)
		{
			outputOverlappedElements.Add(LeafIndex);
		}

		internal override bool TryToInsert(LeafNode node, out Node treeNode)
		{
			InternalNode internalNode = new InternalNode();
			BoundingBox.CreateMerged(ref BoundingBox, ref node.BoundingBox, out internalNode.BoundingBox);
			internalNode.ChildA = this;
			internalNode.ChildB = node;
			treeNode = internalNode;
			return true;
		}

		public override string ToString()
		{
			return LeafIndex.ToString(CultureInfo.InvariantCulture);
		}

		internal override void Analyze(List<int> depths, int depth, ref int nodeCount)
		{
			nodeCount++;
			depths.Add(depth);
		}

		internal override void Refit(MeshBoundingBoxTreeData data)
		{
			data.GetBoundingBox(LeafIndex, out BoundingBox);
			BoundingBox.Max.X += LeafMargin;
			BoundingBox.Max.Y += LeafMargin;
			BoundingBox.Max.Z += LeafMargin;
			BoundingBox.Min.X -= LeafMargin;
			BoundingBox.Min.Y -= LeafMargin;
			BoundingBox.Min.Z -= LeafMargin;
		}
	}

	private MeshBoundingBoxTreeData data;

	private Node root;

	/// <summary>
	/// The tiny extra margin added to leaf bounding boxes that allow the volume cost metric to function properly even in degenerate cases.
	/// </summary>
	public static float LeafMargin = 0.001f;

	/// <summary>
	/// Gets the bounding box surrounding the tree.
	/// </summary>
	public BoundingBox BoundingBox
	{
		get
		{
			if (root != null)
			{
				return root.BoundingBox;
			}
			return default(BoundingBox);
		}
	}

	/// <summary>
	/// Gets or sets the data used to construct the tree.
	/// When set, the tree will be reconstructed.
	/// </summary>
	public MeshBoundingBoxTreeData Data
	{
		get
		{
			return data;
		}
		set
		{
			data = value;
			Reconstruct();
		}
	}

	/// <summary>
	/// Constructs a new tree.
	/// </summary>
	/// <param name="data">Data to use to construct the tree.</param>
	public MeshBoundingBoxTree(MeshBoundingBoxTreeData data)
	{
		Data = data;
	}

	/// <summary>
	/// Reconstructs the tree based on the current data.
	/// </summary>
	public void Reconstruct()
	{
		root = null;
		for (int i = 0; i < data.indices.Length; i += 3)
		{
			Insert((int)(982451653L * (long)(i / 3) % (data.indices.Length / 3) * 3));
		}
	}

	/// <summary>
	/// Refits the tree based on the current data.
	/// This process is cheaper to perform than a reconstruction when the topology of the mesh
	/// does not change.
	/// </summary>
	public void Refit()
	{
		if (root != null)
		{
			root.Refit(data);
		}
	}

	private void Analyze(out List<int> depths, out int minDepth, out int maxDepth, out int nodeCount)
	{
		depths = new List<int>();
		nodeCount = 0;
		root.Analyze(depths, 0, ref nodeCount);
		maxDepth = 0;
		minDepth = int.MaxValue;
		for (int i = 0; i < depths.Count; i++)
		{
			if (depths[i] > maxDepth)
			{
				maxDepth = depths[i];
			}
			if (depths[i] < minDepth)
			{
				minDepth = depths[i];
			}
		}
	}

	private void Insert(int triangleIndex)
	{
		LeafNode leafNode = new LeafNode(triangleIndex, data);
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
		Node treeNode = root;
		while (!treeNode.TryToInsert(leafNode, out treeNode))
		{
		}
	}

	/// <summary>
	/// Gets the triangles whose bounding boxes are overlapped by the query.
	/// </summary>
	/// <param name="boundingBox">Shape to query against the tree.</param>
	/// <param name="outputOverlappedElements">Indices of triangles in the index buffer with bounding boxes which are overlapped by the query.</param>
	/// <returns>Whether or not any elements were overlapped.</returns>
	public bool GetOverlaps(BoundingBox boundingBox, IList<int> outputOverlappedElements)
	{
		if (root != null)
		{
			root.BoundingBox.Intersects(ref boundingBox, out var result);
			if (result)
			{
				root.GetOverlaps(ref boundingBox, outputOverlappedElements);
			}
		}
		return outputOverlappedElements.Count > 0;
	}

	/// <summary>
	/// Gets the triangles whose bounding boxes are overlapped by the query.
	/// </summary>
	/// <param name="boundingSphere">Shape to query against the tree.</param>
	/// <param name="outputOverlappedElements">Indices of triangles in the index buffer with bounding boxes which are overlapped by the query.</param>
	/// <returns>Whether or not any elements were overlapped.</returns>
	public bool GetOverlaps(BoundingSphere boundingSphere, IList<int> outputOverlappedElements)
	{
		if (root != null)
		{
			root.BoundingBox.Intersects(ref boundingSphere, out var result);
			if (result)
			{
				root.GetOverlaps(ref boundingSphere, outputOverlappedElements);
			}
		}
		return outputOverlappedElements.Count > 0;
	}

	/// <summary>
	/// Gets the triangles whose bounding boxes are overlapped by the query.
	/// </summary>
	/// <param name="boundingFrustum">Shape to query against the tree.</param>
	/// <param name="outputOverlappedElements">Indices of triangles in the index buffer with bounding boxes which are overlapped by the query.</param>
	/// <returns>Whether or not any elements were overlapped.</returns>
	public bool GetOverlaps(BoundingFrustum boundingFrustum, IList<int> outputOverlappedElements)
	{
		if (root != null)
		{
			boundingFrustum.Intersects(ref root.BoundingBox, out var result);
			if (result)
			{
				root.GetOverlaps(ref boundingFrustum, outputOverlappedElements);
			}
		}
		return outputOverlappedElements.Count > 0;
	}

	/// <summary>
	/// Gets the triangles whose bounding boxes are overlapped by the query.
	/// </summary>
	/// <param name="ray">Shape to query against the tree.</param>
	/// <param name="outputOverlappedElements">Indices of triangles in the index buffer with bounding boxes which are overlapped by the query.</param>
	/// <returns>Whether or not any elements were overlapped.</returns>
	public bool GetOverlaps(Ray ray, IList<int> outputOverlappedElements)
	{
		if (root != null)
		{
			ray.Intersects(ref root.BoundingBox, out var result);
			if (result.HasValue)
			{
				root.GetOverlaps(ref ray, float.MaxValue, outputOverlappedElements);
			}
		}
		return outputOverlappedElements.Count > 0;
	}

	/// <summary>
	/// Gets the triangles whose bounding boxes are overlapped by the query.
	/// </summary>
	/// <param name="ray">Shape to query against the tree.</param>
	/// <param name="maximumLength">Maximum length of the ray in units of the ray's length.</param>
	/// <param name="outputOverlappedElements">Indices of triangles in the index buffer with bounding boxes which are overlapped by the query.</param>
	/// <returns>Whether or not any elements were overlapped.</returns>
	public bool GetOverlaps(Ray ray, float maximumLength, IList<int> outputOverlappedElements)
	{
		if (root != null)
		{
			ray.Intersects(ref root.BoundingBox, out var result);
			if (result.HasValue)
			{
				root.GetOverlaps(ref ray, maximumLength, outputOverlappedElements);
			}
		}
		return outputOverlappedElements.Count > 0;
	}
}
