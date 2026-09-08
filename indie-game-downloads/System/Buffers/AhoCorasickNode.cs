using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Buffers;

internal struct AhoCorasickNode
{
	public int SuffixLink = 0;

	public int MatchLength = 0;

	private int _firstChildChar = -1;

	private int _firstChildIndex = 0;

	private object _children = EmptyChildrenSentinel;

	private static object EmptyChildrenSentinel => Array.Empty<int>();

	public AhoCorasickNode()
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool TryGetChild(char c, out int index)
	{
		if (_firstChildChar == c)
		{
			index = _firstChildIndex;
			return true;
		}
		object children = _children;
		if (children.GetType() == typeof(int[]))
		{
			int[] array = (int[])children;
			if ((uint)c < (uint)array.Length)
			{
				index = array[(uint)c];
				if (index >= 0)
				{
					return true;
				}
			}
			index = 0;
			return false;
		}
		return Unsafe.As<Dictionary<char, int>>(children).TryGetValue(c, out index);
	}

	public void AddChild(char c, int index)
	{
		if (_firstChildChar < 0)
		{
			_firstChildChar = c;
			_firstChildIndex = index;
			return;
		}
		if (_children == EmptyChildrenSentinel)
		{
			_children = new Dictionary<char, int>();
		}
		((Dictionary<char, int>)_children).Add(c, index);
	}

	public readonly void AddChildrenToQueue(Queue<(char Char, int Index)> queue)
	{
		if (_firstChildChar < 0)
		{
			return;
		}
		queue.Enqueue(((char)_firstChildChar, _firstChildIndex));
		if (!(_children is Dictionary<char, int> dictionary))
		{
			return;
		}
		foreach (var (item, item2) in dictionary)
		{
			queue.Enqueue((item, item2));
		}
	}

	public void OptimizeChildren()
	{
		if (!(_children is Dictionary<char, int> dictionary))
		{
			return;
		}
		dictionary.Add((char)_firstChildChar, _firstChildIndex);
		float num = -2f;
		foreach (var (c2, firstChildIndex) in dictionary)
		{
			float num3 = (char.IsAscii(c2) ? CharacterFrequencyHelper.AsciiFrequency[c2] : (-1f));
			if (num3 > num)
			{
				num = num3;
				_firstChildChar = c2;
				_firstChildIndex = firstChildIndex;
			}
		}
		dictionary.Remove((char)_firstChildChar);
		if (TryCreateJumpTable(dictionary, out var table))
		{
			_children = table;
		}
		static int DictionaryMemoryFootprintBytesEstimate(int childCount)
		{
			if (childCount < 8)
			{
				if (childCount < 4)
				{
					return 192;
				}
				return 272;
			}
			if (childCount < 12)
			{
				return 352;
			}
			return childCount * 25;
		}
		static int TableMemoryFootprintBytesEstimate(int maxValue)
		{
			return 32 + maxValue * 4;
		}
		static bool TryCreateJumpTable(Dictionary<char, int> children, [NotNullWhen(true)] out int[] reference)
		{
			int num4 = -1;
			char key;
			int value;
			foreach (KeyValuePair<char, int> child in children)
			{
				child.Deconstruct(out key, out value);
				char val = key;
				num4 = Math.Max(num4, val);
			}
			int num5 = TableMemoryFootprintBytesEstimate(num4);
			int num6 = DictionaryMemoryFootprintBytesEstimate(children.Count);
			if (num5 > num6 * 2)
			{
				reference = null;
				return false;
			}
			reference = new int[num4 + 1];
			Array.Fill(reference, -1);
			foreach (KeyValuePair<char, int> child2 in children)
			{
				child2.Deconstruct(out key, out value);
				char c3 = key;
				int num7 = value;
				reference[(uint)c3] = num7;
			}
			return true;
		}
	}
}
