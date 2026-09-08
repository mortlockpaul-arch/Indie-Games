using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework;

public class CurveKeyCollection : ICollection<CurveKey>, IEnumerable<CurveKey>, IEnumerable
{
	private readonly List<CurveKey> innerlist;

	public int Count => innerlist.Count;

	public bool IsReadOnly => false;

	public CurveKey this[int index]
	{
		get
		{
			return innerlist[index];
		}
		set
		{
			if ((object)value == null)
			{
				throw new ArgumentNullException();
			}
			if (innerlist[index].Position == value.Position)
			{
				innerlist[index] = value;
				return;
			}
			innerlist.RemoveAt(index);
			INTERNAL_Add(value);
		}
	}

	private CurveKeyCollection(List<CurveKey> innerlist)
	{
		this.innerlist = innerlist;
	}

	public CurveKeyCollection()
	{
		innerlist = new List<CurveKey>();
	}

	public void Add(CurveKey item)
	{
		if ((object)item == null)
		{
			throw new ArgumentNullException();
		}
		INTERNAL_Add(item);
	}

	public void Clear()
	{
		innerlist.Clear();
	}

	public CurveKeyCollection Clone()
	{
		return new CurveKeyCollection(new List<CurveKey>(innerlist));
	}

	public bool Contains(CurveKey item)
	{
		return innerlist.Contains(item);
	}

	public void CopyTo(CurveKey[] array, int arrayIndex)
	{
		innerlist.CopyTo(array, arrayIndex);
	}

	public IEnumerator<CurveKey> GetEnumerator()
	{
		return innerlist.GetEnumerator();
	}

	public int IndexOf(CurveKey item)
	{
		return innerlist.IndexOf(item);
	}

	public bool Remove(CurveKey item)
	{
		return innerlist.Remove(item);
	}

	public void RemoveAt(int index)
	{
		innerlist.RemoveAt(index);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return innerlist.GetEnumerator();
	}

	private void INTERNAL_Add(CurveKey item)
	{
		int num = innerlist.BinarySearch(item);
		if (num < 0)
		{
			num = ~num;
		}
		innerlist.Insert(num, item);
	}
}
