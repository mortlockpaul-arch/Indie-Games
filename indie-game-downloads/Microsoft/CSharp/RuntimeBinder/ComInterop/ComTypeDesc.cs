using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal class ComTypeDesc
{
	private readonly string _typeName;

	private readonly string _documentation;

	private ComMethodDesc _getItem;

	private ComMethodDesc _setItem;

	internal static Dictionary<string, ComEventDesc> EmptyEvents { get; } = new Dictionary<string, ComEventDesc>();

	internal Hashtable Funcs { get; set; }

	internal Hashtable Puts { get; set; }

	internal Hashtable PutRefs { get; set; }

	internal Dictionary<string, ComEventDesc> Events { get; set; }

	public string TypeName => _typeName;

	public ComTypeLibDesc TypeLib { get; }

	internal Guid Guid { get; set; }

	internal ComMethodDesc GetItem => _getItem;

	internal ComMethodDesc SetItem => _setItem;

	internal ComTypeDesc(ITypeInfo typeInfo, ComTypeLibDesc typeLibDesc)
	{
		if (typeInfo != null)
		{
			ComRuntimeHelpers.GetInfoFromType(typeInfo, out _typeName, out _documentation);
		}
		TypeLib = typeLibDesc;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal static ComTypeDesc FromITypeInfo(ITypeInfo typeInfo, TYPEATTR typeAttr)
	{
		switch (typeAttr.typekind)
		{
		case TYPEKIND.TKIND_COCLASS:
			return new ComTypeClassDesc(typeInfo, null);
		case TYPEKIND.TKIND_ENUM:
			return new ComTypeEnumDesc(typeInfo, null);
		case TYPEKIND.TKIND_INTERFACE:
		case TYPEKIND.TKIND_DISPATCH:
			return new ComTypeDesc(typeInfo, null);
		default:
			throw new InvalidOperationException(System.SR.UnsupportedEnum);
		}
	}

	internal static ComTypeDesc CreateEmptyTypeDesc()
	{
		return new ComTypeDesc(null, null)
		{
			Funcs = new Hashtable(),
			Puts = new Hashtable(),
			PutRefs = new Hashtable(),
			Events = EmptyEvents
		};
	}

	internal bool TryGetFunc(string name, out ComMethodDesc method)
	{
		name = name.ToUpper(CultureInfo.InvariantCulture);
		if (Funcs.ContainsKey(name))
		{
			method = Funcs[name] as ComMethodDesc;
			return true;
		}
		method = null;
		return false;
	}

	internal void AddFunc(string name, ComMethodDesc method)
	{
		name = name.ToUpper(CultureInfo.InvariantCulture);
		lock (Funcs)
		{
			Funcs[name] = method;
		}
	}

	internal bool TryGetPut(string name, out ComMethodDesc method)
	{
		name = name.ToUpper(CultureInfo.InvariantCulture);
		if (Puts.ContainsKey(name))
		{
			method = Puts[name] as ComMethodDesc;
			return true;
		}
		method = null;
		return false;
	}

	internal void AddPut(string name, ComMethodDesc method)
	{
		name = name.ToUpper(CultureInfo.InvariantCulture);
		lock (Puts)
		{
			Puts[name] = method;
		}
	}

	internal bool TryGetPutRef(string name, out ComMethodDesc method)
	{
		name = name.ToUpper(CultureInfo.InvariantCulture);
		if (PutRefs.ContainsKey(name))
		{
			method = PutRefs[name] as ComMethodDesc;
			return true;
		}
		method = null;
		return false;
	}

	internal void AddPutRef(string name, ComMethodDesc method)
	{
		name = name.ToUpper(CultureInfo.InvariantCulture);
		lock (PutRefs)
		{
			PutRefs[name] = method;
		}
	}

	internal bool TryGetEvent(string name, out ComEventDesc @event)
	{
		name = name.ToUpper(CultureInfo.InvariantCulture);
		return Events.TryGetValue(name, out @event);
	}

	internal string[] GetMemberNames(bool dataOnly)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		lock (Funcs)
		{
			foreach (ComMethodDesc value in Funcs.Values)
			{
				if (!dataOnly || value.IsDataMember)
				{
					dictionary.Add(value.Name, null);
				}
			}
		}
		if (!dataOnly)
		{
			lock (Puts)
			{
				foreach (ComMethodDesc value2 in Puts.Values)
				{
					if (!dictionary.ContainsKey(value2.Name))
					{
						dictionary.Add(value2.Name, null);
					}
				}
			}
			lock (PutRefs)
			{
				foreach (ComMethodDesc value3 in PutRefs.Values)
				{
					if (!dictionary.ContainsKey(value3.Name))
					{
						dictionary.Add(value3.Name, null);
					}
				}
			}
			if (Events != null && Events.Count > 0)
			{
				foreach (string key in Events.Keys)
				{
					if (!dictionary.ContainsKey(key))
					{
						dictionary.Add(key, null);
					}
				}
			}
		}
		string[] array = new string[dictionary.Keys.Count];
		dictionary.Keys.CopyTo(array, 0);
		return array;
	}

	internal void EnsureGetItem(ComMethodDesc candidate)
	{
		Interlocked.CompareExchange(ref _getItem, candidate, null);
	}

	internal void EnsureSetItem(ComMethodDesc candidate)
	{
		Interlocked.CompareExchange(ref _setItem, candidate, null);
	}
}
