using System;
using System.Collections.Generic;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision;

public class DynamicTree
{
	internal const int NullNode = -1;

	private static Stack<int> _stack = new Stack<int>(256);

	private int _freeList;

	private int _insertionCount;

	private int _nodeCapacity;

	private int _nodeCount;

	private DynamicTreeNode[] _nodes;

	private int _path;

	private int _root;

	public DynamicTree()
	{
		_root = -1;
		_nodeCapacity = 16;
		_nodes = new DynamicTreeNode[_nodeCapacity];
		for (int i = 0; i < _nodeCapacity - 1; i++)
		{
			_nodes[i].ParentOrNext = i + 1;
		}
		_nodes[_nodeCapacity - 1].ParentOrNext = -1;
	}

	public int CreateProxy(ref AABB aabb, object userData)
	{
		int num = AllocateNode();
		Vector2 vector = new Vector2(0.1f, 0.1f);
		_nodes[num].AABB.LowerBound = aabb.LowerBound - vector;
		_nodes[num].AABB.UpperBound = aabb.UpperBound + vector;
		_nodes[num].UserData = userData;
		_nodes[num].LeafCount = 1;
		InsertLeaf(num);
		return num;
	}

	public void DestroyProxy(int proxyId)
	{
		RemoveLeaf(proxyId);
		FreeNode(proxyId);
	}

	public bool MoveProxy(int proxyId, ref AABB aabb, Vector2 displacement)
	{
		if (_nodes[proxyId].AABB.Contains(ref aabb))
		{
			return false;
		}
		RemoveLeaf(proxyId);
		AABB aABB = aabb;
		Vector2 vector = new Vector2(0.1f, 0.1f);
		aABB.LowerBound -= vector;
		aABB.UpperBound += vector;
		Vector2 vector2 = 2f * displacement;
		if (vector2.X < 0f)
		{
			aABB.LowerBound.X += vector2.X;
		}
		else
		{
			aABB.UpperBound.X += vector2.X;
		}
		if (vector2.Y < 0f)
		{
			aABB.LowerBound.Y += vector2.Y;
		}
		else
		{
			aABB.UpperBound.Y += vector2.Y;
		}
		_nodes[proxyId].AABB = aABB;
		InsertLeaf(proxyId);
		return true;
	}

	public void Rebalance(int iterations)
	{
		if (_root == -1)
		{
			return;
		}
		for (int i = 0; i < iterations; i++)
		{
			int num = _root;
			int num2 = 0;
			while (!_nodes[num].IsLeaf())
			{
				num = ((((_path >> num2) & 1) == 0) ? _nodes[num].Child1 : _nodes[num].Child2);
				num2 = (num2 + 1) & 0x1F;
			}
			_path++;
			RemoveLeaf(num);
			InsertLeaf(num);
		}
	}

	public T GetUserData<T>(int proxyId)
	{
		return (T)_nodes[proxyId].UserData;
	}

	public void GetFatAABB(int proxyId, out AABB fatAABB)
	{
		fatAABB = _nodes[proxyId].AABB;
	}

	public int ComputeHeight()
	{
		return ComputeHeight(_root);
	}

	public void Query(Func<int, bool> callback, ref AABB aabb)
	{
		_stack.Clear();
		_stack.Push(_root);
		while (_stack.Count > 0)
		{
			int num = _stack.Pop();
			if (num == -1)
			{
				continue;
			}
			DynamicTreeNode dynamicTreeNode = _nodes[num];
			if (!AABB.TestOverlap(ref dynamicTreeNode.AABB, ref aabb))
			{
				continue;
			}
			if (dynamicTreeNode.IsLeaf())
			{
				if (!callback(num))
				{
					break;
				}
			}
			else
			{
				_stack.Push(dynamicTreeNode.Child1);
				_stack.Push(dynamicTreeNode.Child2);
			}
		}
	}

	public void RayCast(RayCastCallbackInternal callback, ref RayCastInput input)
	{
		Vector2 point = input.Point1;
		Vector2 point2 = input.Point2;
		Vector2 a = point2 - point;
		a.Normalize();
		Vector2 vector = MathUtils.Cross(1f, a);
		Vector2 value = MathUtils.Abs(vector);
		float num = input.MaxFraction;
		AABB b = default(AABB);
		Vector2 value2 = point + num * (point2 - point);
		b.LowerBound = Vector2.Min(point, value2);
		b.UpperBound = Vector2.Max(point, value2);
		_stack.Clear();
		_stack.Push(_root);
		RayCastInput input2 = default(RayCastInput);
		while (_stack.Count > 0)
		{
			int num2 = _stack.Pop();
			if (num2 == -1)
			{
				continue;
			}
			DynamicTreeNode dynamicTreeNode = _nodes[num2];
			if (!AABB.TestOverlap(ref dynamicTreeNode.AABB, ref b))
			{
				continue;
			}
			Vector2 center = dynamicTreeNode.AABB.Center;
			Vector2 extents = dynamicTreeNode.AABB.Extents;
			float num3 = Math.Abs(Vector2.Dot(vector, point - center)) - Vector2.Dot(value, extents);
			if (num3 > 0f)
			{
				continue;
			}
			if (dynamicTreeNode.IsLeaf())
			{
				input2.Point1 = input.Point1;
				input2.Point2 = input.Point2;
				input2.MaxFraction = num;
				float num4 = callback(ref input2, num2);
				if (num4 == 0f)
				{
					break;
				}
				if (num4 > 0f)
				{
					num = num4;
					Vector2 value3 = point + num * (point2 - point);
					b.LowerBound = Vector2.Min(point, value3);
					b.UpperBound = Vector2.Max(point, value3);
				}
			}
			else
			{
				_stack.Push(dynamicTreeNode.Child1);
				_stack.Push(dynamicTreeNode.Child2);
			}
		}
	}

	private int CountLeaves(int nodeId)
	{
		if (nodeId == -1)
		{
			return 0;
		}
		DynamicTreeNode dynamicTreeNode = _nodes[nodeId];
		if (dynamicTreeNode.IsLeaf())
		{
			return 1;
		}
		int num = CountLeaves(dynamicTreeNode.Child1);
		int num2 = CountLeaves(dynamicTreeNode.Child2);
		return num + num2;
	}

	private void Validate()
	{
		CountLeaves(_root);
	}

	private int AllocateNode()
	{
		if (_freeList == -1)
		{
			DynamicTreeNode[] nodes = _nodes;
			_nodeCapacity *= 2;
			_nodes = new DynamicTreeNode[_nodeCapacity];
			Array.Copy(nodes, _nodes, _nodeCount);
			for (int i = _nodeCount; i < _nodeCapacity - 1; i++)
			{
				_nodes[i].ParentOrNext = i + 1;
			}
			_nodes[_nodeCapacity - 1].ParentOrNext = -1;
			_freeList = _nodeCount;
		}
		int freeList = _freeList;
		_freeList = _nodes[freeList].ParentOrNext;
		_nodes[freeList].ParentOrNext = -1;
		_nodes[freeList].Child1 = -1;
		_nodes[freeList].Child2 = -1;
		_nodes[freeList].LeafCount = 0;
		_nodeCount++;
		return freeList;
	}

	private void FreeNode(int nodeId)
	{
		_nodes[nodeId].ParentOrNext = _freeList;
		_freeList = nodeId;
		_nodeCount--;
	}

	private void InsertLeaf(int leaf)
	{
		_insertionCount++;
		if (_root == -1)
		{
			_root = leaf;
			_nodes[_root].ParentOrNext = -1;
			return;
		}
		AABB aabb = _nodes[leaf].AABB;
		int num = _root;
		while (!_nodes[num].IsLeaf())
		{
			_nodes[num].AABB.Combine(ref aabb);
			_nodes[num].LeafCount++;
			int child = _nodes[num].Child1;
			int child2 = _nodes[num].Child2;
			AABB aABB = default(AABB);
			AABB aABB2 = default(AABB);
			aABB.Combine(ref aabb, ref _nodes[child].AABB);
			aABB2.Combine(ref aabb, ref _nodes[child2].AABB);
			float num2 = (float)(_nodes[child].LeafCount + 1) * aABB.Perimeter;
			float num3 = (float)(_nodes[child2].LeafCount + 1) * aABB2.Perimeter;
			num = ((!(num2 < num3)) ? child2 : child);
		}
		int parentOrNext = _nodes[num].ParentOrNext;
		int num4 = AllocateNode();
		_nodes[num4].ParentOrNext = parentOrNext;
		_nodes[num4].UserData = null;
		_nodes[num4].AABB.Combine(ref aabb, ref _nodes[num].AABB);
		_nodes[num4].LeafCount = _nodes[num].LeafCount + 1;
		if (parentOrNext != -1)
		{
			if (_nodes[parentOrNext].Child1 == num)
			{
				_nodes[parentOrNext].Child1 = num4;
			}
			else
			{
				_nodes[parentOrNext].Child2 = num4;
			}
			_nodes[num4].Child1 = num;
			_nodes[num4].Child2 = leaf;
			_nodes[num].ParentOrNext = num4;
			_nodes[leaf].ParentOrNext = num4;
		}
		else
		{
			_nodes[num4].Child1 = num;
			_nodes[num4].Child2 = leaf;
			_nodes[num].ParentOrNext = num4;
			_nodes[leaf].ParentOrNext = num4;
			_root = num4;
		}
	}

	private void RemoveLeaf(int leaf)
	{
		if (leaf == _root)
		{
			_root = -1;
			return;
		}
		int parentOrNext = _nodes[leaf].ParentOrNext;
		int parentOrNext2 = _nodes[parentOrNext].ParentOrNext;
		int num = ((_nodes[parentOrNext].Child1 != leaf) ? _nodes[parentOrNext].Child1 : _nodes[parentOrNext].Child2);
		if (parentOrNext2 != -1)
		{
			if (_nodes[parentOrNext2].Child1 == parentOrNext)
			{
				_nodes[parentOrNext2].Child1 = num;
			}
			else
			{
				_nodes[parentOrNext2].Child2 = num;
			}
			_nodes[num].ParentOrNext = parentOrNext2;
			FreeNode(parentOrNext);
			for (parentOrNext = parentOrNext2; parentOrNext != -1; parentOrNext = _nodes[parentOrNext].ParentOrNext)
			{
				_nodes[parentOrNext].AABB.Combine(ref _nodes[_nodes[parentOrNext].Child1].AABB, ref _nodes[_nodes[parentOrNext].Child2].AABB);
				_nodes[parentOrNext].LeafCount--;
			}
		}
		else
		{
			_root = num;
			_nodes[num].ParentOrNext = -1;
			FreeNode(parentOrNext);
		}
	}

	private int ComputeHeight(int nodeId)
	{
		if (nodeId == -1)
		{
			return 0;
		}
		DynamicTreeNode dynamicTreeNode = _nodes[nodeId];
		int val = ComputeHeight(dynamicTreeNode.Child1);
		int val2 = ComputeHeight(dynamicTreeNode.Child2);
		return 1 + Math.Max(val, val2);
	}
}
