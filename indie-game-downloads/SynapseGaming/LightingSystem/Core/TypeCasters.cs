using System.Collections.Generic;
using _0003;

namespace SynapseGaming.LightingSystem.Core;

/// <summary />
public class TypeCasters<TTypeCaster, TType> where TTypeCaster : ITypeCaster<TType>, new()
{
	private Dictionary<TType, TTypeCaster> _3A_0018 = new Dictionary<TType, TTypeCaster>(128);

	private _0003.t<TTypeCaster> _3AL = new _0003.t<TTypeCaster>();

	/// <summary />
	public void Clear()
	{
		_3A_0018.Clear();
		_3AL.FreeAllTracked();
	}

	/// <summary />
	public TTypeCaster Get(TType obj)
	{
		if (_3A_0018.TryGetValue(obj, out var value))
		{
			return value;
		}
		value = _3AL.New();
		value.Set(obj);
		_3A_0018.Add(obj, value);
		return value;
	}
}
