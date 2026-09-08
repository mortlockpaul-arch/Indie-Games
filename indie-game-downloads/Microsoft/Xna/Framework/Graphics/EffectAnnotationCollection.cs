using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Graphics;

public sealed class EffectAnnotationCollection : IEnumerable<EffectAnnotation>, IEnumerable
{
	internal static readonly EffectAnnotationCollection Empty = new EffectAnnotationCollection(new List<EffectAnnotation>());

	private List<EffectAnnotation> elements;

	public int Count => elements.Count;

	public EffectAnnotation this[int index] => ((uint)index < (uint)elements.Count) ? elements[index] : null;

	public EffectAnnotation this[string name]
	{
		get
		{
			foreach (EffectAnnotation element in elements)
			{
				if (name == element.Name)
				{
					return element;
				}
			}
			return null;
		}
	}

	internal EffectAnnotationCollection(List<EffectAnnotation> value)
	{
		elements = value;
	}

	public List<EffectAnnotation>.Enumerator GetEnumerator()
	{
		return elements.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return elements.GetEnumerator();
	}

	IEnumerator<EffectAnnotation> IEnumerable<EffectAnnotation>.GetEnumerator()
	{
		return elements.GetEnumerator();
	}
}
