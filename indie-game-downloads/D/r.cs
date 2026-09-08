using System;
using System.Collections;
using System.Collections.Generic;

namespace D;

internal class r : IList<_0001>, ICollection<_0001>, IEnumerable<_0001>, IEnumerable
{
	public struct _00065h : IEnumerator<_0001>, IDisposable, IEnumerator
	{
		private r a5h;

		private int a5b;

		private int a56;

		public _0001 Current => a5h[a5b];

		object IEnumerator.Current => Current;

		internal _00065h(r P_0)
		{
			a5h = P_0;
			a5b = -1;
			a56 = P_0.Count;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			return ++a5b < a56;
		}

		public void Reset()
		{
			a5b = -1;
			a56 = a5h.Count;
		}
	}

	private b a5h;

	public int Count => a5h.ContactCount;

	public _0001 this[int index]
	{
		get
		{
			a5h.GetContactInformation(index, out var info);
			return info;
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	bool ICollection<_0001>.IsReadOnly => true;

	internal r(b P_0)
	{
		a5h = P_0;
	}

	IEnumerator<_0001> IEnumerable<_0001>.GetEnumerator()
	{
		return new _00065h(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new _00065h(this);
	}

	public _00065h GetEnumerator()
	{
		return new _00065h(this);
	}

	public bool Contains(_0001 item)
	{
		int count = Count;
		for (int i = 0; i < count; i++)
		{
			if (this[i].Contact == item.Contact)
			{
				return true;
			}
		}
		return false;
	}

	public void CopyTo(_0001[] array, int arrayIndex)
	{
		int count = Count;
		for (int i = 0; i < count; i++)
		{
			ref _0001 reference = ref array[arrayIndex + i];
			reference = this[i];
		}
	}

	public int IndexOf(_0001 item)
	{
		int count = Count;
		for (int i = 0; i < count; i++)
		{
			if (this[i].Contact == item.Contact)
			{
				return i;
			}
		}
		return -1;
	}

	bool ICollection<_0001>.Remove(_0001 item)
	{
		throw new NotSupportedException();
	}

	void ICollection<_0001>.Add(_0001 item)
	{
		throw new NotSupportedException();
	}

	void ICollection<_0001>.Clear()
	{
		throw new NotSupportedException();
	}

	void IList<_0001>.Insert(int index, _0001 item)
	{
		throw new NotSupportedException();
	}

	void IList<_0001>.RemoveAt(int index)
	{
		throw new NotSupportedException();
	}
}
