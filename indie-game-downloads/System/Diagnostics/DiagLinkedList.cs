using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace System.Diagnostics;

internal sealed class DiagLinkedList<T> : IEnumerable<T>, IEnumerable
{
	private DiagNode<T> _first;

	private DiagNode<T> _last;

	public DiagNode<T> First => _first;

	public DiagLinkedList()
	{
	}

	public DiagLinkedList(T firstValue)
	{
		_last = (_first = new DiagNode<T>(firstValue));
	}

	public DiagLinkedList(IEnumerator<T> e)
	{
		_last = (_first = new DiagNode<T>(e.Current));
		while (e.MoveNext())
		{
			_last.Next = new DiagNode<T>(e.Current);
			_last = _last.Next;
		}
	}

	public void Clear()
	{
		lock (this)
		{
			_first = (_last = null);
		}
	}

	private void UnsafeAdd(DiagNode<T> newNode)
	{
		if (_first == null)
		{
			_first = (_last = newNode);
			return;
		}
		_last.Next = newNode;
		_last = newNode;
	}

	public void Add(T value)
	{
		DiagNode<T> newNode = new DiagNode<T>(value);
		lock (this)
		{
			UnsafeAdd(newNode);
		}
	}

	public bool AddIfNotExist(T value, Func<T, T, bool> compare)
	{
		lock (this)
		{
			for (DiagNode<T> diagNode = _first; diagNode != null; diagNode = diagNode.Next)
			{
				if (compare(value, diagNode.Value))
				{
					return false;
				}
			}
			DiagNode<T> newNode = new DiagNode<T>(value);
			UnsafeAdd(newNode);
			return true;
		}
	}

	public T Remove(T value, Func<T, T, bool> compare)
	{
		lock (this)
		{
			DiagNode<T> diagNode = _first;
			if (diagNode == null)
			{
				return default(T);
			}
			if (compare(diagNode.Value, value))
			{
				_first = diagNode.Next;
				if (_first == null)
				{
					_last = null;
				}
				return diagNode.Value;
			}
			for (DiagNode<T> next = diagNode.Next; next != null; next = next.Next)
			{
				if (compare(next.Value, value))
				{
					diagNode.Next = next.Next;
					if (_last == next)
					{
						_last = diagNode;
					}
					return next.Value;
				}
				diagNode = next;
			}
			return default(T);
		}
	}

	public DiagEnumerator<T> GetEnumerator()
	{
		return new DiagEnumerator<T>(_first);
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private static void ActivityLinkToString(ref ActivityLink al, ref System.Text.ValueStringBuilder vsb)
	{
		ActivityContext context = al.Context;
		vsb.Append("(");
		vsb.Append(context.TraceId.ToHexString());
		vsb.Append(",\u200b");
		vsb.Append(context.SpanId.ToHexString());
		vsb.Append(",\u200b");
		vsb.Append(context.TraceFlags.ToString());
		vsb.Append(",\u200b");
		vsb.Append(context.TraceState ?? "null");
		vsb.Append(",\u200b");
		vsb.Append(context.IsRemote ? "true" : "false");
		if (al.Tags != null)
		{
			vsb.Append(",\u200b[");
			string s = "";
			foreach (ref KeyValuePair<string, object> item in al.EnumerateTagObjects())
			{
				KeyValuePair<string, object> current = item;
				vsb.Append(s);
				vsb.Append(current.Key);
				vsb.Append(":\u200b");
				vsb.Append(current.Value?.ToString() ?? "null");
				s = ",\u200b";
			}
			vsb.Append("]");
		}
		vsb.Append(")");
	}

	private static void ActivityEventToString(ref ActivityEvent ae, ref System.Text.ValueStringBuilder vsb)
	{
		vsb.Append("(");
		vsb.Append(ae.Name);
		vsb.Append(",\u200b");
		vsb.Append(ae.Timestamp.ToString("o"));
		if (ae.Tags != null)
		{
			vsb.Append(",\u200b[");
			string s = "";
			foreach (ref KeyValuePair<string, object> item in ae.EnumerateTagObjects())
			{
				KeyValuePair<string, object> current = item;
				vsb.Append(s);
				vsb.Append(current.Key);
				vsb.Append(":\u200b");
				vsb.Append(current.Value?.ToString() ?? "null");
				s = ",\u200b";
			}
			vsb.Append("]");
		}
		vsb.Append(")");
	}

	public override string ToString()
	{
		lock (this)
		{
			DiagNode<T> diagNode = _first;
			if (diagNode == null)
			{
				return "[]";
			}
			Span<char> initialBuffer = stackalloc char[256];
			System.Text.ValueStringBuilder vsb = new System.Text.ValueStringBuilder(initialBuffer);
			vsb.Append("[");
			if (typeof(T) == typeof(ActivityLink))
			{
				while (diagNode != null)
				{
					ActivityLink al = (ActivityLink)(object)diagNode.Value;
					ActivityLinkToString(ref al, ref vsb);
					diagNode = diagNode.Next;
					if (diagNode != null)
					{
						vsb.Append(",\u200b");
					}
				}
			}
			else if (typeof(T) == typeof(ActivityEvent))
			{
				while (diagNode != null)
				{
					ActivityEvent ae = (ActivityEvent)(object)diagNode.Value;
					ActivityEventToString(ref ae, ref vsb);
					diagNode = diagNode.Next;
					if (diagNode != null)
					{
						vsb.Append(",\u200b");
					}
				}
			}
			else
			{
				while (diagNode != null)
				{
					vsb.Append(diagNode.Value?.ToString() ?? "null");
					diagNode = diagNode.Next;
					if (diagNode != null)
					{
						vsb.Append(",\u200b");
					}
				}
			}
			vsb.Append("]");
			return vsb.ToString();
		}
	}
}
