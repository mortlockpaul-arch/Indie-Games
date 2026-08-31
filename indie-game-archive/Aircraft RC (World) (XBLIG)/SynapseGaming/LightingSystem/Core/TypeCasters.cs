using System.Collections.Generic;
using Z;

namespace SynapseGaming.LightingSystem.Core;

/// <summary />
public class TypeCasters<TTypeCaster, TType> where TTypeCaster : ITypeCaster<TType>, new()
{
	private Dictionary<TType, TTypeCaster> HCB = new Dictionary<TType, TTypeCaster>(128);

	private Z.y<TTypeCaster> HC_0002 = new Z.y<TTypeCaster>();

	/// <summary />
	public void Clear()
	{
		HCB.Clear();
		HC_0002.FreeAllTracked();
	}

	/// <summary />
	public TTypeCaster Get(TType obj)
	{
		if (HCB.TryGetValue(obj, out var value))
		{
			return value;
		}
		value = HC_0002.New();
		value.Set(obj);
		HCB.Add(obj, value);
		return value;
	}
}
