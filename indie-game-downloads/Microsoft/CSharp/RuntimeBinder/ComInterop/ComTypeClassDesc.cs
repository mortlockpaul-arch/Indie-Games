using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
internal sealed class ComTypeClassDesc : ComTypeDesc, IDynamicMetaObjectProvider
{
	private LinkedList<string> _itfs;

	private LinkedList<string> _sourceItfs;

	private Type _typeObj;

	public object CreateInstance()
	{
		if ((object)_typeObj == null)
		{
			_typeObj = Type.GetTypeFromCLSID(base.Guid);
		}
		return Activator.CreateInstance(Type.GetTypeFromCLSID(base.Guid));
	}

	internal ComTypeClassDesc(ITypeInfo typeInfo, ComTypeLibDesc typeLibDesc)
		: base(typeInfo, typeLibDesc)
	{
		TYPEATTR typeAttrForTypeInfo = ComRuntimeHelpers.GetTypeAttrForTypeInfo(typeInfo);
		base.Guid = typeAttrForTypeInfo.guid;
		for (int i = 0; i < typeAttrForTypeInfo.cImplTypes; i++)
		{
			typeInfo.GetRefTypeOfImplType(i, out var href);
			typeInfo.GetRefTypeInfo(href, out ITypeInfo ppTI);
			typeInfo.GetImplTypeFlags(i, out var pImplTypeFlags);
			bool isSourceItf = (pImplTypeFlags & IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE) != 0;
			AddInterface(ppTI, isSourceItf);
		}
	}

	private void AddInterface(ITypeInfo itfTypeInfo, bool isSourceItf)
	{
		string nameOfType = ComRuntimeHelpers.GetNameOfType(itfTypeInfo);
		if (isSourceItf)
		{
			if (_sourceItfs == null)
			{
				_sourceItfs = new LinkedList<string>();
			}
			_sourceItfs.AddLast(nameOfType);
		}
		else
		{
			if (_itfs == null)
			{
				_itfs = new LinkedList<string>();
			}
			_itfs.AddLast(nameOfType);
		}
	}

	internal bool Implements(string itfName, bool isSourceItf)
	{
		if (isSourceItf)
		{
			return _sourceItfs.Contains(itfName);
		}
		return _itfs.Contains(itfName);
	}

	public DynamicMetaObject GetMetaObject(Expression parameter)
	{
		return new ComClassMetaObject(parameter, this);
	}
}
