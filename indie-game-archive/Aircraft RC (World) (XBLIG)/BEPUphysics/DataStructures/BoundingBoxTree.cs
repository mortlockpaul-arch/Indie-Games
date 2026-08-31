using System.Collections.Generic;
using BEPUphysics.BroadPhaseSystems;
using Microsoft.Xna.Framework;

namespace BEPUphysics.DataStructures;

/// <summary>
///  Acceleration structure of objects surrounded by axis aligned bounding boxes, supporting various speedy queries.
/// </summary>
public class BoundingBoxTree<T> where T : IBoundingBoxOwner
{
	internal abstract class Node
	{
		internal BoundingBox BoundingBox;

		internal abstract bool IsLeaf { get; }

		internal abstract Node ChildA { get; }

		internal abstract Node ChildB { get; }

		internal abstract T Element { get; }

		internal abstract void GetOverlaps(ref BoundingBox boundingBox, IList<T> outputOverlappedElements);

		internal abstract void GetOverlaps(ref BoundingSphere boundingSphere, IList<T> outputOverlappedElements);

		internal abstract void GetOverlaps(ref BoundingFrustum boundingFrustum, IList<T> outputOverlappedElements);

		internal abstract void GetOverlaps(ref Ray ray, float maximumLength, IList<T> outputOverlappedElements);

		internal abstract void GetOverlaps<TElement>(BoundingBoxTree<TElement>.Node opposingNode, IList<TreeOverlapPair<T, TElement>> outputOverlappedElements) where TElement : IBoundingBoxOwner;

		internal abstract bool TryToInsert(LeafNode node, out Node treeNode);

		internal abstract void Analyze(List<int> depths, int depth, ref int nodeCount);

		internal abstract void Refit();
	}

	internal sealed class InternalNode : Node
	{
		internal Node childA;

		internal Node childB;

		internal override Node ChildA => childA;

		internal override Node ChildB => childB;

		internal override T Element => default(T);

		internal override bool IsLeaf => false;

		internal override void GetOverlaps(ref BoundingBox boundingBox, IList<T> outputOverlappedElements)
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

		internal override void GetOverlaps(ref BoundingSphere boundingSphere, IList<T> outputOverlappedElements)
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

		internal override void GetOverlaps(ref BoundingFrustum boundingFrustum, IList<T> outputOverlappedElements)
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

		internal override void GetOverlaps(ref Ray ray, float maximumLength, IList<T> outputOverlappedElements)
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

		internal override void GetOverlaps<TElement>(BoundingBoxTree<TElement>.Node opposingNode, IList<TreeOverlapPair<T, TElement>> outputOverlappedElements)
		{
			bool result;
			if (opposingNode.IsLeaf)
			{
				childA.BoundingBox.Intersects(ref opposingNode.BoundingBox, out result);
				if (result)
				{
					childA.GetOverlaps(opposingNode, outputOverlappedElements);
				}
				childB.BoundingBox.Intersects(ref opposingNode.BoundingBox, out result);
				if (result)
				{
					childB.GetOverlaps(opposingNode, outputOverlappedElements);
				}
				return;
			}
			BoundingBoxTree<TElement>.Node node = opposingNode.ChildA;
			BoundingBoxTree<TElement>.Node node2 = opposingNode.ChildB;
			childA.BoundingBox.Intersects(ref node.BoundingBox, out result);
			if (result)
			{
				childA.GetOverlaps(node, outputOverlappedElements);
			}
			childA.BoundingBox.Intersects(ref node2.BoundingBox, out result);
			if (result)
			{
				childA.GetOverlaps(node2, outputOverlappedElements);
			}
			childB.BoundingBox.Intersects(ref node.BoundingBox, out result);
			if (result)
			{
				childB.GetOverlaps(node, outputOverlappedElements);
			}
			childB.BoundingBox.Intersects(ref node2.BoundingBox, out result);
			if (result)
			{
				childB.GetOverlaps(node2, outputOverlappedElements);
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
					childA = new InternalNode
					{
						BoundingBox = result,
						childA = childA,
						childB = node
					};
					treeNode = null;
					return true;
				}
				childA.BoundingBox = result;
				treeNode = childA;
				return false;
			}
			if (childB.IsLeaf)
			{
				childB = new InternalNode
				{
					BoundingBox = result2,
					childA = node,
					childB = childB
				};
				treeNode = null;
				return true;
			}
			childB.BoundingBox = result2;
			treeNode = childB;
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
			childA.Refit();
			childB.Refit();
			BoundingBox.CreateMerged(ref childA.BoundingBox, ref childB.BoundingBox, out BoundingBox);
		}
	}

	internal sealed class LeafNode : Node
	{
		private T element;

		internal override Node ChildA => null;

		internal override Node ChildB => null;

		internal override T Element => element;

		internal override bool IsLeaf => true;

		internal LeafNode(T element)
		{
			this.element = element;
			BoundingBox = element.BoundingBox;
			BoundingBox.Max.X += BoundingBoxTree<T>.LeafMargin;
			BoundingBox.Max.Y += BoundingBoxTree<T>.LeafMargin;
			BoundingBox.Max.Z += BoundingBoxTree<T>.LeafMargin;
			BoundingBox.Min.X -= BoundingBoxTree<T>.LeafMargin;
			BoundingBox.Min.Y -= BoundingBoxTree<T>.LeafMargin;
			BoundingBox.Min.Z -= BoundingBoxTree<T>.LeafMargin;
		}

		internal override void GetOverlaps(ref BoundingBox boundingBox, IList<T> outputOverlappedElements)
		{
			outputOverlappedElements.Add(element);
		}

		internal override void GetOverlaps(ref BoundingSphere boundingSphere, IList<T> outputOverlappedElements)
		{
			outputOverlappedElements.Add(element);
		}

		internal override void GetOverlaps(ref BoundingFrustum boundingFrustum, IList<T> outputOverlappedElements)
		{
			outputOverlappedElements.Add(element);
		}

		internal override void GetOverlaps(ref Ray ray, float maximumLength, IList<T> outputOverlappedElements)
		{
			outputOverlappedElements.Add(element);
		}

		internal override void GetOverlaps<TElement>(BoundingBoxTree<TElement>.Node opposingNode, IList<TreeOverlapPair<T, TElement>> outputOverlappedElements)
		{
			if (opposingNode.IsLeaf)
			{
				outputOverlappedElements.Add(new TreeOverlapPair<T, TElement>(element, opposingNode.Element));
				return;
			}
			BoundingBoxTree<TElement>.Node childA = opposingNode.ChildA;
			BoundingBoxTree<TElement>.Node childB = opposingNode.ChildB;
			BoundingBox.Intersects(ref childA.BoundingBox, out var result);
			if (result)
			{
				GetOverlaps(childA, outputOverlappedElements);
			}
			BoundingBox.Intersects(ref childB.BoundingBox, out result);
			if (result)
			{
				GetOverlaps(childB, outputOverlappedElements);
			}
		}

		internal override bool TryToInsert(LeafNode node, out Node treeNode)
		{
			InternalNode internalNode = new InternalNode();
			BoundingBox.CreateMerged(ref BoundingBox, ref node.BoundingBox, out internalNode.BoundingBox);
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
			BoundingBox = element.BoundingBox;
			BoundingBox.Max.X += BoundingBoxTree<T>.LeafMargin;
			BoundingBox.Max.Y += BoundingBoxTree<T>.LeafMargin;
			BoundingBox.Max.Z += BoundingBoxTree<T>.LeafMargin;
			BoundingBox.Min.X -= BoundingBoxTree<T>.LeafMargin;
			BoundingBox.Min.Y -= BoundingBoxTree<T>.LeafMargin;
			BoundingBox.Min.Z -= BoundingBoxTree<T>.LeafMargin;
		}
	}

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
	/// Constructs a new tree.
	/// </summary>
	/// <param name="elements">Data to use to construct the tree.</param>
	public BoundingBoxTree(IList<T> elements)
	{
		Reconstruct(elements);
	}

	/// <summary>
	/// Reconstructs the tree based on the current data.
	/// </summary>
	public void Reconstruct(IList<T> elements)
	{
		root = null;
		int count = elements.Count;
		for (int i = 0; i < count; i++)
		{
			Add(elements[(int)(982451653L * (long)i % count)]);
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
			root.Refit();
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

	/// <summary>
	/// Adds an element to the tree.
	/// If a list of objects is available, using the Reconstruct method is recommended.
	/// </summary>
	/// <param name="element">Element to add.</param>
	public void Add(T element)
	{
		LeafNode leafNode = new LeafNode(element);
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
	public bool GetOverlaps(BoundingBox boundingBox, IList<T> outputOverlappedElements)
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
	public bool GetOverlaps(BoundingSphere boundingSphere, IList<T> outputOverlappedElements)
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
	public bool GetOverlaps(BoundingFrustum boundingFrustum, IList<T> outputOverlappedElements)
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
	public bool GetOverlaps(Ray ray, IList<T> outputOverlappedElements)
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
	public bool GetOverlaps(Ray ray, float maximumLength, IList<T> outputOverlappedElements)
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

	/// <summary>
	/// Gets the pairs of elements in each tree with overlapping bounding boxes.
	/// </summary>
	/// <typeparam name="TElement">Type of the elements in the opposing tree.</typeparam>
	/// <param name="tree">Other tree to test.</param>
	/// <param name="outputOverlappedElements">List of overlaps found by the query.</param>
	/// <returns>Whether or not any overlaps were found.</returns>
	public bool GetOverlaps<TElement>(BoundingBoxTree<TElement> tree, IList<TreeOverlapPair<T, TElement>> outputOverlappedElements) where TElement : IBoundingBoxOwner
	{
		root.BoundingBox.Intersects(ref tree.root.BoundingBox, out var result);
		if (result)
		{
			root.GetOverlaps(tree.root, outputOverlappedElements);
		}
		return outputOverlappedElements.Count > 0;
	}
}
