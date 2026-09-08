using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Net;

public class NetworkSessionProperties : IList<int?>, ICollection<int?>, IEnumerable<int?>, IEnumerable
{
	private List<int?> properties;

	public int Count => properties.Count;

	public int? this[int index]
	{
		get
		{
			return properties[index];
		}
		set
		{
			if (index >= properties.Count)
			{
				properties.Add(value);
			}
			else
			{
				properties[index] = value;
			}
		}
	}

	bool ICollection<int?>.IsReadOnly => true;

	public NetworkSessionProperties()
	{
		properties = new List<int?>();
	}

	public IEnumerator<int?> GetEnumerator()
	{
		return properties.GetEnumerator();
	}

	int IList<int?>.IndexOf(int? item)
	{
		return properties.IndexOf(item);
	}

	void IList<int?>.Insert(int index, int? item)
	{
		properties.Insert(index, item);
	}

	void IList<int?>.RemoveAt(int index)
	{
		properties.RemoveAt(index);
	}

	void ICollection<int?>.Add(int? item)
	{
		properties.Add(item);
	}

	bool ICollection<int?>.Remove(int? item)
	{
		return properties.Remove(item);
	}

	bool ICollection<int?>.Contains(int? item)
	{
		return properties.Contains(item);
	}

	void ICollection<int?>.Clear()
	{
		properties.Clear();
	}

	void ICollection<int?>.CopyTo(int?[] array, int arrayIndex)
	{
		properties.CopyTo(array, arrayIndex);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
