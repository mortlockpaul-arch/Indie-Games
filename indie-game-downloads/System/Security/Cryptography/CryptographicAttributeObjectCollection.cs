using System.Collections;
using System.Collections.Generic;

namespace System.Security.Cryptography;

internal sealed class CryptographicAttributeObjectCollection : ICollection, IEnumerable
{
	private readonly List<CryptographicAttributeObject> _list;

	public CryptographicAttributeObject this[int index] => _list[index];

	public int Count => _list.Count;

	public bool IsSynchronized => false;

	public object SyncRoot => this;

	public CryptographicAttributeObjectCollection()
	{
		_list = new List<CryptographicAttributeObject>();
	}

	internal void AddWithoutMerge(CryptographicAttributeObject attribute)
	{
		_list.Add(attribute);
	}

	public CryptographicAttributeObjectEnumerator GetEnumerator()
	{
		return new CryptographicAttributeObjectEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new CryptographicAttributeObjectEnumerator(this);
	}

	void ICollection.CopyTo(Array array, int index)
	{
		ArgumentNullException.ThrowIfNull(array, "array");
		if (array.Rank != 1)
		{
			throw new ArgumentException(System.SR.Arg_RankMultiDimNotSupported);
		}
		if (index < 0 || index >= array.Length)
		{
			throw new ArgumentOutOfRangeException("index", System.SR.ArgumentOutOfRange_IndexMustBeLess);
		}
		if (index > array.Length - Count)
		{
			throw new ArgumentException(System.SR.Argument_InvalidOffLen);
		}
		for (int i = 0; i < Count; i++)
		{
			array.SetValue(this[i], index);
			index++;
		}
	}
}
