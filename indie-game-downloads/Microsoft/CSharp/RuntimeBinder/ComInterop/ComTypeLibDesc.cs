using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal sealed class ComTypeLibDesc : IDynamicMetaObjectProvider
{
	private readonly LinkedList<ComTypeClassDesc> _classes;

	private readonly Dictionary<string, ComTypeEnumDesc> _enums;

	private TYPELIBATTR _typeLibAttributes;

	private static readonly Dictionary<Guid, ComTypeLibDesc> s_cachedTypeLibDesc = new Dictionary<Guid, ComTypeLibDesc>();

	public Guid Guid => _typeLibAttributes.guid;

	public string Name { get; private set; }

	private ComTypeLibDesc()
	{
		_enums = new Dictionary<string, ComTypeEnumDesc>();
		_classes = new LinkedList<ComTypeClassDesc>();
	}

	public override string ToString()
	{
		return "<type library " + Name + ">";
	}

	DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
	{
		return new TypeLibMetaObject(parameter, this);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal static ComTypeLibDesc GetFromTypeLib(ITypeLib typeLib)
	{
		TYPELIBATTR typeAttrForTypeLib = ComRuntimeHelpers.GetTypeAttrForTypeLib(typeLib);
		ComTypeLibDesc value;
		lock (s_cachedTypeLibDesc)
		{
			if (s_cachedTypeLibDesc.TryGetValue(typeAttrForTypeLib.guid, out value))
			{
				return value;
			}
		}
		value = new ComTypeLibDesc
		{
			Name = ComRuntimeHelpers.GetNameOfLib(typeLib),
			_typeLibAttributes = typeAttrForTypeLib
		};
		int typeInfoCount = typeLib.GetTypeInfoCount();
		for (int i = 0; i < typeInfoCount; i++)
		{
			typeLib.GetTypeInfoType(i, out var pTKind);
			typeLib.GetTypeInfo(i, out ITypeInfo ppTI);
			switch (pTKind)
			{
			case TYPEKIND.TKIND_COCLASS:
			{
				ComTypeClassDesc value3 = new ComTypeClassDesc(ppTI, value);
				value._classes.AddLast(value3);
				break;
			}
			case TYPEKIND.TKIND_ENUM:
			{
				ComTypeEnumDesc comTypeEnumDesc = new ComTypeEnumDesc(ppTI, value);
				value._enums.Add(comTypeEnumDesc.TypeName, comTypeEnumDesc);
				break;
			}
			case TYPEKIND.TKIND_ALIAS:
			{
				TYPEATTR typeAttrForTypeInfo = ComRuntimeHelpers.GetTypeAttrForTypeInfo(ppTI);
				if (typeAttrForTypeInfo.tdescAlias.vt == 29)
				{
					ComRuntimeHelpers.GetInfoFromType(ppTI, out var name, out var _);
					ppTI.GetRefTypeInfo(((IntPtr)typeAttrForTypeInfo.tdescAlias.lpValue).ToInt32(), out ITypeInfo ppTI2);
					if (ComRuntimeHelpers.GetTypeAttrForTypeInfo(ppTI2).typekind == TYPEKIND.TKIND_ENUM)
					{
						ComTypeEnumDesc value2 = new ComTypeEnumDesc(ppTI2, value);
						value._enums.Add(name, value2);
					}
				}
				break;
			}
			}
		}
		lock (s_cachedTypeLibDesc)
		{
			s_cachedTypeLibDesc.Add(typeAttrForTypeLib.guid, value);
			return value;
		}
	}

	public object GetTypeLibObjectDesc(string member)
	{
		foreach (ComTypeClassDesc @class in _classes)
		{
			if (member == @class.TypeName)
			{
				return @class;
			}
		}
		if (_enums != null && _enums.TryGetValue(member, out var value))
		{
			return value;
		}
		return null;
	}

	public string[] GetMemberNames()
	{
		string[] array = new string[_enums.Count + _classes.Count];
		int num = 0;
		foreach (ComTypeClassDesc @class in _classes)
		{
			array[num++] = @class.TypeName;
		}
		foreach (KeyValuePair<string, ComTypeEnumDesc> @enum in _enums)
		{
			array[num++] = @enum.Key;
		}
		return array;
	}

	internal bool HasMember(string member)
	{
		foreach (ComTypeClassDesc @class in _classes)
		{
			if (member == @class.TypeName)
			{
				return true;
			}
		}
		if (_enums.ContainsKey(member))
		{
			return true;
		}
		return false;
	}

	internal ComTypeClassDesc GetCoClassForInterface(string itfName)
	{
		foreach (ComTypeClassDesc @class in _classes)
		{
			if (@class.Implements(itfName, isSourceItf: false))
			{
				return @class;
			}
		}
		return null;
	}
}
