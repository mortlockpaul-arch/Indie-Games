using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace JamSouls;

internal class ProfilingManager
{
	protected class TraceNode : IComparable
	{
		public string _mName = "";

		public long _mTime;

		public TraceNode _Parent;

		public List<TraceNode> _mChildren = new List<TraceNode>();

		public TraceNode(string _Name, TraceNode _Parent)
		{
			_mName = _Name;
		}

		public int CompareTo(object _Other)
		{
			return (int)(((TraceNode)_Other)._mTime - _mTime);
		}
	}

	protected const int cStackSize = 128;

	public static string m_output;

	protected static Stopwatch _mTimer = new Stopwatch();

	protected static TraceNode _mRoot = new TraceNode("JamSouls", null);

	protected static TraceNode[] _mStack = new TraceNode[128];

	protected static int _mStackIdx = 0;

	public static void OnNewFrame()
	{
		_mRoot._mTime = _mTimer.ElapsedTicks;
		if (_mRoot._mChildren.Count > 0)
		{
			Dump(_mRoot, 0);
			Sort(_mRoot);
			Dump(_mRoot, 0);
		}
		_mStackIdx = 0;
		_mStack[0] = _mRoot;
		_mRoot._mChildren.Clear();
		_mRoot._mTime = 0L;
		_mTimer.Reset();
		_mTimer.Start();
	}

	public static void Push(string _ScopeName)
	{
		TraceNode traceNode = new TraceNode(_ScopeName, _mStack[_mStackIdx]);
		_mStack[_mStackIdx]._mChildren.Add(traceNode);
		_mStackIdx++;
		_mStack[_mStackIdx] = traceNode;
		traceNode._mTime = _mTimer.ElapsedTicks;
	}

	public static void Pop()
	{
		TraceNode traceNode = _mStack[_mStackIdx];
		traceNode._mTime = _mTimer.ElapsedTicks - traceNode._mTime;
		_mStackIdx--;
	}

	protected static void Dump(TraceNode _Node, int _Depth)
	{
		string text = "";
		for (int i = 0; i < _Depth; i++)
		{
			text += "|  ";
		}
		text = text + _Node._mName + " ";
		for (int j = text.Length; j < 32; j++)
		{
			text += ".";
		}
		text += " ";
		for (int k = 0; k < _Depth; k++)
		{
			text += "|  ";
		}
		text += $"{TimeFromTicks(_Node._mTime).ToString(),-16:R}";
		long num = 0L;
		foreach (TraceNode mChild in _Node._mChildren)
		{
			num += mChild._mTime;
		}
		text += $"{TimeFromTicks(_Node._mTime - num).ToString(),-16:#.0000}";
		m_output = m_output + text + "\n";
		foreach (TraceNode mChild2 in _Node._mChildren)
		{
			Dump(mChild2, _Depth + 1);
		}
	}

	protected static float TimeFromTicks(long _Ticks)
	{
		return (float)(_Ticks * 1000) / (float)Stopwatch.Frequency;
	}

	protected static void Sort(TraceNode _Node)
	{
		_Node._mChildren.Sort();
		foreach (TraceNode mChild in _Node._mChildren)
		{
			Sort(mChild);
		}
	}
}
