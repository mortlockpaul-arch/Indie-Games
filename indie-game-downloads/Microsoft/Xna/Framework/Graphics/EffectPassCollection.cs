using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Graphics;

public sealed class EffectPassCollection : IEnumerable<EffectPass>, IEnumerable
{
	private List<EffectPass> elements;

	private EffectPass singleItem;

	public int Count
	{
		get
		{
			if (elements == null)
			{
				return (singleItem != null) ? 1 : 0;
			}
			return elements.Count;
		}
	}

	public EffectPass this[int index]
	{
		get
		{
			if (elements != null)
			{
				return ((uint)index < (uint)elements.Count) ? elements[index] : null;
			}
			return (index == 0) ? singleItem : null;
		}
	}

	public EffectPass this[string name]
	{
		get
		{
			if (elements == null)
			{
				return (singleItem.Name == name) ? singleItem : null;
			}
			foreach (EffectPass element in elements)
			{
				if (name == element.Name)
				{
					return element;
				}
			}
			return null;
		}
	}

	internal EffectPassCollection(List<EffectPass> value)
	{
		elements = value;
	}

	internal EffectPassCollection(EffectPass pass)
	{
		singleItem = pass;
	}

	internal List<EffectPass> GetList()
	{
		if (elements == null)
		{
			elements = new List<EffectPass>(1);
			elements.Add(singleItem);
		}
		return elements;
	}

	public List<EffectPass>.Enumerator GetEnumerator()
	{
		return GetList().GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetList().GetEnumerator();
	}

	IEnumerator<EffectPass> IEnumerable<EffectPass>.GetEnumerator()
	{
		return GetList().GetEnumerator();
	}
}
