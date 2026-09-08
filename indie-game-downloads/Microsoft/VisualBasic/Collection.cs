using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.VisualBasic;

[DebuggerTypeProxy(typeof(CollectionDebugView))]
[DebuggerDisplay("Count = {Count}")]
public sealed class Collection : ICollection, IList
{
	internal sealed class Node
	{
		internal object m_Value;

		internal string m_Key;

		internal Node m_Next;

		internal Node m_Prev;

		internal Node(string Key, object Value)
		{
			m_Value = Value;
			m_Key = Key;
		}
	}

	internal sealed class CollectionDebugView
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Collection m_InstanceBeingWatched;

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public object[] Items
		{
			get
			{
				int count = m_InstanceBeingWatched.Count;
				if (count == 0)
				{
					return null;
				}
				checked
				{
					object[] array = new object[count + 1];
					array[0] = System.SR.EmptyPlaceHolderMessage;
					int num = count;
					for (int i = 1; i <= num; i++)
					{
						Node node = m_InstanceBeingWatched.InternalItemsList().get_Item(i - 1);
						array[i] = new KeyValuePair(node.m_Key, node.m_Value);
					}
					return array;
				}
			}
		}

		public CollectionDebugView(Collection RealClass)
		{
			m_InstanceBeingWatched = RealClass;
		}
	}

	private sealed class FastList
	{
		private Node m_StartOfList;

		private Node m_EndOfList;

		private int m_Count;

		// C# has no syntax for parameterized property 'Item'.
		internal Node get_Item(int Index)
		{
			Node PrevNode = null;
			return GetNodeAtIndex(Index, ref PrevNode) ?? throw new ArgumentOutOfRangeException("Index");
		}

		internal FastList()
		{
		}

		internal void Add(Node Node)
		{
			if (m_StartOfList == null)
			{
				m_StartOfList = Node;
			}
			else
			{
				m_EndOfList.m_Next = Node;
				Node.m_Prev = m_EndOfList;
			}
			m_EndOfList = Node;
			checked
			{
				m_Count++;
			}
		}

		internal int IndexOfValue(object Value)
		{
			Node node = m_StartOfList;
			int num = 0;
			while (node != null)
			{
				if (DataIsEqual(node.m_Value, Value))
				{
					return num;
				}
				node = node.m_Next;
				num = checked(num + 1);
			}
			return -1;
		}

		internal void RemoveNode(Node NodeToBeDeleted)
		{
			DeleteNode(NodeToBeDeleted, NodeToBeDeleted.m_Prev);
		}

		internal Node RemoveAt(int Index)
		{
			Node node = m_StartOfList;
			int i = 0;
			Node prevNode = null;
			for (; i < Index; i = checked(i + 1))
			{
				if (node == null)
				{
					break;
				}
				prevNode = node;
				node = node.m_Next;
			}
			if (node == null)
			{
				throw new ArgumentOutOfRangeException("Index");
			}
			DeleteNode(node, prevNode);
			return node;
		}

		internal int Count()
		{
			return m_Count;
		}

		internal void Clear()
		{
			m_StartOfList = null;
			m_EndOfList = null;
			m_Count = 0;
		}

		internal void Insert(int Index, Node Node)
		{
			Node PrevNode = null;
			if (Index < 0 || Index > m_Count)
			{
				throw new ArgumentOutOfRangeException("Index");
			}
			Node nodeAtIndex = GetNodeAtIndex(Index, ref PrevNode);
			Insert(Node, PrevNode, nodeAtIndex);
		}

		internal void InsertBefore(Node Node, Node NodeToInsertBefore)
		{
			Insert(Node, NodeToInsertBefore.m_Prev, NodeToInsertBefore);
		}

		internal void InsertAfter(Node Node, Node NodeToInsertAfter)
		{
			Insert(Node, NodeToInsertAfter, NodeToInsertAfter.m_Next);
		}

		internal Node GetFirstListNode()
		{
			return m_StartOfList;
		}

		private bool DataIsEqual(object obj1, object obj2)
		{
			if (obj1 == obj2)
			{
				return true;
			}
			if ((object)obj1.GetType() == obj2.GetType())
			{
				return object.Equals(obj1, obj2);
			}
			return false;
		}

		private Node GetNodeAtIndex(int Index, [Optional][DefaultParameterValue(null)] ref Node PrevNode)
		{
			Node node = m_StartOfList;
			int i = 0;
			PrevNode = null;
			for (; i < Index; i = checked(i + 1))
			{
				if (node == null)
				{
					break;
				}
				PrevNode = node;
				node = node.m_Next;
			}
			return node;
		}

		private void Insert(Node Node, Node PrevNode, Node CurrentNode)
		{
			Node.m_Next = CurrentNode;
			if (CurrentNode != null)
			{
				CurrentNode.m_Prev = Node;
			}
			if (PrevNode == null)
			{
				m_StartOfList = Node;
			}
			else
			{
				PrevNode.m_Next = Node;
				Node.m_Prev = PrevNode;
			}
			if (Node.m_Next == null)
			{
				m_EndOfList = Node;
			}
			checked
			{
				m_Count++;
			}
		}

		private void DeleteNode(Node NodeToBeDeleted, Node PrevNode)
		{
			if (PrevNode == null)
			{
				m_StartOfList = m_StartOfList.m_Next;
				if (m_StartOfList == null)
				{
					m_EndOfList = null;
				}
				else
				{
					m_StartOfList.m_Prev = null;
				}
			}
			else
			{
				PrevNode.m_Next = NodeToBeDeleted.m_Next;
				if (PrevNode.m_Next == null)
				{
					m_EndOfList = PrevNode;
				}
				else
				{
					PrevNode.m_Next.m_Prev = PrevNode;
				}
			}
			checked
			{
				m_Count--;
			}
		}
	}

	private struct KeyValuePair
	{
		private object m_Key;

		private object m_Value;

		internal KeyValuePair(object NewKey, object NewValue)
		{
			this = default(KeyValuePair);
			m_Key = NewKey;
			m_Value = NewValue;
		}
	}

	private CultureInfo m_CultureInfo;

	private Dictionary<string, Node> m_KeyedNodesHash;

	private FastList m_ItemsList;

	private List<object> m_Iterators;

	public object this[int Index]
	{
		get
		{
			IndexCheck(Index);
			return m_ItemsList.get_Item(checked(Index - 1)).m_Value;
		}
	}

	public object this[string Key]
	{
		get
		{
			if (Key == null)
			{
				throw new IndexOutOfRangeException(System.SR.Argument_CollectionIndex);
			}
			Node value = null;
			if (!m_KeyedNodesHash.TryGetValue(Key, out value))
			{
				throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Index"), "Key");
			}
			return value.m_Value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public object this[object Index]
	{
		get
		{
			if (Index is string || Index is char || Index is char[])
			{
				string key = Conversions.ToString(Index);
				return this[key];
			}
			int index;
			try
			{
				index = Conversions.ToInteger(Index);
			}
			catch (StackOverflowException ex)
			{
				throw ex;
			}
			catch (OutOfMemoryException ex2)
			{
				throw ex2;
			}
			catch (Exception)
			{
				throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Index"), "Index");
			}
			return this[index];
		}
	}

	public int Count => m_ItemsList.Count();

	private int ICollectionCount => m_ItemsList.Count();

	private bool ICollectionIsSynchronized => false;

	private object ICollectionSyncRoot => this;

	private bool IListIsFixedSize => false;

	private bool IListIsReadOnly => false;

	// C# has no syntax for parameterized property 'IListItem'.
	private object get_IListItem(int index)
	{
		return m_ItemsList.get_Item(index).m_Value;
	}

	object IList.get_Item(int index)
	{
		//ILSpy generated this explicit interface implementation from .override directive in get_IListItem
		return this.get_IListItem(index);
	}

	private void set_IListItem(int index, object value)
	{
		m_ItemsList.get_Item(index).m_Value = value;
	}

	void IList.set_Item(int index, object value)
	{
		//ILSpy generated this explicit interface implementation from .override directive in set_IListItem
		this.set_IListItem(index, value);
	}

	public Collection()
	{
		Initialize(Utils.GetCultureInfo());
	}

	public void Add(object Item, string Key = null, object Before = null, object After = null)
	{
		if (Before != null && After != null)
		{
			throw new ArgumentException(System.SR.Collection_BeforeAfterExclusive);
		}
		Node node = new Node(Key, Item);
		if (Key != null)
		{
			try
			{
				m_KeyedNodesHash.Add(Key, node);
			}
			catch (ArgumentException)
			{
				throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Collection_DuplicateKey), 457);
			}
		}
		try
		{
			if (Before == null && After == null)
			{
				m_ItemsList.Add(node);
			}
			else if (Before != null)
			{
				if (Before is string key)
				{
					Node value = null;
					if (!m_KeyedNodesHash.TryGetValue(key, out value))
					{
						throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Before"), "Before");
					}
					m_ItemsList.InsertBefore(node, value);
				}
				else
				{
					m_ItemsList.Insert(checked(Conversions.ToInteger(Before) - 1), node);
				}
			}
			else if (After is string key2)
			{
				Node value2 = null;
				if (!m_KeyedNodesHash.TryGetValue(key2, out value2))
				{
					throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "After"), "After");
				}
				m_ItemsList.InsertAfter(node, value2);
			}
			else
			{
				m_ItemsList.Insert(Conversions.ToInteger(After), node);
			}
		}
		catch (OutOfMemoryException)
		{
			throw;
		}
		catch (StackOverflowException)
		{
			throw;
		}
		catch (Exception)
		{
			if (Key != null)
			{
				m_KeyedNodesHash.Remove(Key);
			}
			throw;
		}
		AdjustEnumeratorsOnNodeInserted(node);
	}

	public void Clear()
	{
		m_KeyedNodesHash.Clear();
		m_ItemsList.Clear();
		checked
		{
			for (int num = m_Iterators.Count - 1; num >= 0; num--)
			{
				WeakReference weakReference = (WeakReference)m_Iterators[num];
				if (weakReference.IsAlive)
				{
					((ForEachEnum)weakReference.Target)?.AdjustOnListCleared();
				}
				else
				{
					m_Iterators.RemoveAt(num);
				}
			}
		}
	}

	public bool Contains(string Key)
	{
		if (Key == null)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Key"), "Key");
		}
		return m_KeyedNodesHash.ContainsKey(Key);
	}

	public void Remove(string Key)
	{
		Node value = null;
		if (m_KeyedNodesHash.TryGetValue(Key, out value))
		{
			AdjustEnumeratorsOnNodeRemoved(value);
			m_KeyedNodesHash.Remove(Key);
			m_ItemsList.RemoveNode(value);
			value.m_Prev = null;
			value.m_Next = null;
			return;
		}
		throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Key"), "Key");
	}

	public void Remove(int Index)
	{
		IndexCheck(Index);
		Node node = m_ItemsList.RemoveAt(checked(Index - 1));
		AdjustEnumeratorsOnNodeRemoved(node);
		if (node.m_Key != null)
		{
			m_KeyedNodesHash.Remove(node.m_Key);
		}
		node.m_Prev = null;
		node.m_Next = null;
	}

	public IEnumerator GetEnumerator()
	{
		checked
		{
			for (int num = m_Iterators.Count - 1; num >= 0; num--)
			{
				if (!((WeakReference)m_Iterators[num]).IsAlive)
				{
					m_Iterators.RemoveAt(num);
				}
			}
			ForEachEnum forEachEnum = new ForEachEnum(this);
			WeakReference item = (forEachEnum.WeakRef = new WeakReference(forEachEnum));
			m_Iterators.Add(item);
			return forEachEnum;
		}
	}

	internal void RemoveIterator(WeakReference weakref)
	{
		m_Iterators.Remove(weakref);
	}

	internal void AddIterator(WeakReference weakref)
	{
		m_Iterators.Add(weakref);
	}

	internal Node GetFirstListNode()
	{
		return m_ItemsList.GetFirstListNode();
	}

	private void Initialize(CultureInfo CultureInfo, int StartingHashCapacity = 0)
	{
		if (StartingHashCapacity > 0)
		{
			m_KeyedNodesHash = new Dictionary<string, Node>(StartingHashCapacity, StringComparer.Create(CultureInfo, ignoreCase: true));
		}
		else
		{
			m_KeyedNodesHash = new Dictionary<string, Node>(StringComparer.Create(CultureInfo, ignoreCase: true));
		}
		m_ItemsList = new FastList();
		m_Iterators = new List<object>();
		m_CultureInfo = CultureInfo;
	}

	private void AdjustEnumeratorsOnNodeInserted(Node NewNode)
	{
		AdjustEnumeratorsHelper(NewNode, ForEachEnum.AdjustIndexType.Insert);
	}

	private void AdjustEnumeratorsOnNodeRemoved(Node RemovedNode)
	{
		AdjustEnumeratorsHelper(RemovedNode, ForEachEnum.AdjustIndexType.Remove);
	}

	private void AdjustEnumeratorsHelper(Node NewOrRemovedNode, ForEachEnum.AdjustIndexType Type)
	{
		checked
		{
			for (int num = m_Iterators.Count - 1; num >= 0; num--)
			{
				WeakReference weakReference = (WeakReference)m_Iterators[num];
				if (weakReference.IsAlive)
				{
					((ForEachEnum)weakReference.Target)?.Adjust(NewOrRemovedNode, Type);
				}
				else
				{
					m_Iterators.RemoveAt(num);
				}
			}
		}
	}

	private void IndexCheck(int Index)
	{
		if (Index < 1 || Index > m_ItemsList.Count())
		{
			throw new IndexOutOfRangeException(System.SR.Argument_CollectionIndex);
		}
	}

	private FastList InternalItemsList()
	{
		return m_ItemsList;
	}

	private IEnumerator ICollectionGetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		//ILSpy generated this explicit interface implementation from .override directive in ICollectionGetEnumerator
		return this.ICollectionGetEnumerator();
	}

	private void ICollectionCopyTo(Array array, int index)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array", System.SR.Format(System.SR.Argument_InvalidNullValue1, "array"));
		}
		if (array.Rank != 1)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_RankEQOne1, "array"), "array");
		}
		checked
		{
			if (index < 0 || array.Length - index < Count)
			{
				throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "index"), "index");
			}
			if (array is object[] array2)
			{
				int count = Count;
				for (int i = 1; i <= count; i++)
				{
					array2[index + i - 1] = this[i];
				}
			}
			else
			{
				int count2 = Count;
				for (int i = 1; i <= count2; i++)
				{
					array.SetValue(this[i], index + i - 1);
				}
			}
		}
	}

	void ICollection.CopyTo(Array array, int index)
	{
		//ILSpy generated this explicit interface implementation from .override directive in ICollectionCopyTo
		this.ICollectionCopyTo(array, index);
	}

	private int IListAdd(object value)
	{
		Add(value);
		return checked(m_ItemsList.Count() - 1);
	}

	int IList.Add(object value)
	{
		//ILSpy generated this explicit interface implementation from .override directive in IListAdd
		return this.IListAdd(value);
	}

	private void IListInsert(int index, object value)
	{
		Node node = new Node(null, value);
		m_ItemsList.Insert(index, node);
		AdjustEnumeratorsOnNodeInserted(node);
	}

	void IList.Insert(int index, object value)
	{
		//ILSpy generated this explicit interface implementation from .override directive in IListInsert
		this.IListInsert(index, value);
	}

	private void IListRemoveAt(int index)
	{
		Node node = m_ItemsList.RemoveAt(index);
		AdjustEnumeratorsOnNodeRemoved(node);
		if (node.m_Key != null)
		{
			m_KeyedNodesHash.Remove(node.m_Key);
		}
		node.m_Prev = null;
		node.m_Next = null;
	}

	void IList.RemoveAt(int index)
	{
		//ILSpy generated this explicit interface implementation from .override directive in IListRemoveAt
		this.IListRemoveAt(index);
	}

	private void IListRemove(object value)
	{
		int num = IListIndexOf(value);
		if (num != -1)
		{
			IListRemoveAt(num);
		}
	}

	void IList.Remove(object value)
	{
		//ILSpy generated this explicit interface implementation from .override directive in IListRemove
		this.IListRemove(value);
	}

	private void IListClear()
	{
		Clear();
	}

	void IList.Clear()
	{
		//ILSpy generated this explicit interface implementation from .override directive in IListClear
		this.IListClear();
	}

	private bool IListContains(object value)
	{
		return IListIndexOf(value) != -1;
	}

	bool IList.Contains(object value)
	{
		//ILSpy generated this explicit interface implementation from .override directive in IListContains
		return this.IListContains(value);
	}

	private int IListIndexOf(object value)
	{
		return m_ItemsList.IndexOfValue(value);
	}

	int IList.IndexOf(object value)
	{
		//ILSpy generated this explicit interface implementation from .override directive in IListIndexOf
		return this.IListIndexOf(value);
	}
}
