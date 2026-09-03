using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

public class EffectParameterCollectionRedux
{
	private Effect _parent;

	public List<object> pParameter;

	public Dictionary<string, int> pIndex;

	private int _count;

	public int Count => _count;

	public object this[int index]
	{
		get
		{
			if (index >= 0 && index < pParameter.Count)
			{
				return pParameter[index];
			}
			return null;
		}
	}

	public object this[string name]
	{
		get
		{
			if (pIndex.ContainsKey(name))
			{
				return pParameter[pIndex[name]];
			}
			return null;
		}
		set
		{
			if (pIndex.ContainsKey(name))
			{
				pParameter[pIndex[name]] = value;
				return;
			}
			pParameter.Add(value);
			pIndex.Add(name, _count);
			_count++;
		}
	}

	public EffectParameterCollectionRedux(Effect parent)
	{
		_parent = parent;
		_count = 0;
		pIndex = new Dictionary<string, int>();
		pParameter = new List<object>();
	}

	public EffectParameterCollectionRedux(EffectParameterCollectionRedux other)
		: this(other._parent)
	{
		_count = other._count;
		foreach (object item in other.pParameter)
		{
			pParameter.Add(item);
		}
		foreach (string key in other.pIndex.Keys)
		{
			pIndex.Add(key, other.pIndex[key]);
		}
	}

	public EffectParameterCollectionRedux(EffectParameterCollection other, Effect fx)
		: this(fx)
	{
		_count = other.Count;
		IEnumerator<EffectParameter> enumerator = other.GetEnumerator();
		enumerator.MoveNext();
		for (int i = 0; i < _count; i++)
		{
			pParameter.Add(enumerator.Current);
			pIndex.Add(enumerator.Current.Name, i);
			enumerator.MoveNext();
		}
	}

	private IEnumerator GetBaseEnumerator()
	{
		return GetBaseEnumerator();
	}

	public IEnumerator<object> GetEnumerator()
	{
		return pParameter.GetEnumerator();
	}
}
