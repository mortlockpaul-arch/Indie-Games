using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class IDispatchComObject : ComObject, IDynamicMetaObjectProvider
{
	private ComTypeDesc _comTypeDesc;

	private static readonly Dictionary<Guid, ComTypeDesc> s_cacheComTypeDesc = new Dictionary<Guid, ComTypeDesc>();

	public ComTypeDesc ComTypeDesc
	{
		get
		{
			EnsureScanDefinedMethods();
			return _comTypeDesc;
		}
	}

	public IDispatch DispatchObject { get; }

	internal IDispatchComObject(IDispatch rcw)
		: base(rcw)
	{
		DispatchObject = rcw;
	}

	public override string ToString()
	{
		ComTypeDesc comTypeDesc = _comTypeDesc;
		string value = null;
		if (comTypeDesc != null)
		{
			value = comTypeDesc.TypeName;
		}
		if (string.IsNullOrEmpty(value))
		{
			value = "IDispatch";
		}
		return $"{base.RuntimeCallableWrapper} ({value})";
	}

	private static int GetIDsOfNames(IDispatch dispatch, string name, out int dispId)
	{
		int[] array = new int[1];
		Guid iid = Guid.Empty;
		int result = dispatch.TryGetIDsOfNames(ref iid, new string[1] { name }, 1u, 0, array);
		dispId = array[0];
		return result;
	}

	internal bool TryGetGetItem(out ComMethodDesc value)
	{
		ComMethodDesc getItem = _comTypeDesc.GetItem;
		if (getItem != null)
		{
			value = getItem;
			return true;
		}
		return SlowTryGetGetItem(out value);
	}

	private bool SlowTryGetGetItem(out ComMethodDesc value)
	{
		EnsureScanDefinedMethods();
		ComMethodDesc getItem = _comTypeDesc.GetItem;
		if (getItem == null)
		{
			string name = "[PROPERTYGET, DISPID(0)]";
			_comTypeDesc.EnsureGetItem(new ComMethodDesc(name, 0, INVOKEKIND.INVOKE_PROPERTYGET));
			getItem = _comTypeDesc.GetItem;
		}
		value = getItem;
		return true;
	}

	internal bool TryGetSetItem(out ComMethodDesc value)
	{
		ComMethodDesc setItem = _comTypeDesc.SetItem;
		if (setItem != null)
		{
			value = setItem;
			return true;
		}
		return SlowTryGetSetItem(out value);
	}

	private bool SlowTryGetSetItem(out ComMethodDesc value)
	{
		EnsureScanDefinedMethods();
		ComMethodDesc setItem = _comTypeDesc.SetItem;
		if (setItem == null)
		{
			string name = "[PROPERTYPUT, DISPID(0)]";
			_comTypeDesc.EnsureSetItem(new ComMethodDesc(name, 0, INVOKEKIND.INVOKE_PROPERTYPUT));
			setItem = _comTypeDesc.SetItem;
		}
		value = setItem;
		return true;
	}

	internal bool TryGetMemberMethod(string name, out ComMethodDesc method)
	{
		EnsureScanDefinedMethods();
		return _comTypeDesc.TryGetFunc(name, out method);
	}

	internal bool TryGetMemberEvent(string name, out ComEventDesc @event)
	{
		EnsureScanDefinedEvents();
		return _comTypeDesc.TryGetEvent(name, out @event);
	}

	internal bool TryGetMemberMethodExplicit(string name, out ComMethodDesc method)
	{
		EnsureScanDefinedMethods();
		int iDsOfNames = GetIDsOfNames(DispatchObject, name, out var dispId);
		switch (iDsOfNames)
		{
		case 0:
		{
			ComMethodDesc comMethodDesc = new ComMethodDesc(name, dispId, INVOKEKIND.INVOKE_FUNC);
			_comTypeDesc.AddFunc(name, comMethodDesc);
			method = comMethodDesc;
			return true;
		}
		case -2147352570:
			method = null;
			return false;
		default:
			throw Error.CouldNotGetDispId(name, $"0x{(uint)iDsOfNames:X})");
		}
	}

	internal bool TryGetPropertySetterExplicit(string name, out ComMethodDesc method, Type limitType, bool holdsNull)
	{
		EnsureScanDefinedMethods();
		int iDsOfNames = GetIDsOfNames(DispatchObject, name, out var dispId);
		switch (iDsOfNames)
		{
		case 0:
		{
			ComMethodDesc comMethodDesc = new ComMethodDesc(name, dispId, INVOKEKIND.INVOKE_PROPERTYPUT);
			_comTypeDesc.AddPut(name, comMethodDesc);
			ComMethodDesc comMethodDesc2 = new ComMethodDesc(name, dispId, INVOKEKIND.INVOKE_PROPERTYPUTREF);
			_comTypeDesc.AddPutRef(name, comMethodDesc2);
			if (ComBinderHelpers.PreferPut(limitType, holdsNull))
			{
				method = comMethodDesc;
			}
			else
			{
				method = comMethodDesc2;
			}
			return true;
		}
		case -2147352570:
			method = null;
			return false;
		default:
			throw Error.CouldNotGetDispId(name, $"0x{(uint)iDsOfNames:X})");
		}
	}

	internal override IList<string> GetMemberNames(bool dataOnly)
	{
		EnsureScanDefinedMethods();
		EnsureScanDefinedEvents();
		return ComTypeDesc.GetMemberNames(dataOnly);
	}

	internal override IList<KeyValuePair<string, object>> GetMembers(IEnumerable<string> names)
	{
		if (names == null)
		{
			names = GetMemberNames(dataOnly: true);
		}
		Type type = base.RuntimeCallableWrapper.GetType();
		List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
		foreach (string name in names)
		{
			if (name != null && ComTypeDesc.TryGetFunc(name, out var method) && method.IsDataMember)
			{
				try
				{
					object value = type.InvokeMember(method.Name, BindingFlags.GetProperty, null, base.RuntimeCallableWrapper, Array.Empty<object>(), CultureInfo.InvariantCulture);
					list.Add(new KeyValuePair<string, object>(method.Name, value));
				}
				catch (Exception value2)
				{
					list.Add(new KeyValuePair<string, object>(method.Name, value2));
				}
			}
		}
		return list.ToArray();
	}

	DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
	{
		EnsureScanDefinedMethods();
		return new IDispatchMetaObject(parameter, this);
	}

	private unsafe static void GetFuncDescForDescIndex(ITypeInfo typeInfo, int funcIndex, out FUNCDESC funcDesc, out nint funcDescHandle)
	{
		typeInfo.GetFuncDesc(funcIndex, out var ppFuncDesc);
		if (ppFuncDesc == IntPtr.Zero)
		{
			throw Error.CannotRetrieveTypeInformation();
		}
		funcDesc = *(FUNCDESC*)ppFuncDesc;
		funcDescHandle = ppFuncDesc;
	}

	private void EnsureScanDefinedEvents()
	{
		if (_comTypeDesc?.Events != null)
		{
			return;
		}
		ITypeInfo iTypeInfoFromIDispatch = ComRuntimeHelpers.GetITypeInfoFromIDispatch(DispatchObject);
		if (iTypeInfoFromIDispatch == null)
		{
			_comTypeDesc = ComTypeDesc.CreateEmptyTypeDesc();
			return;
		}
		TYPEATTR typeAttrForTypeInfo = ComRuntimeHelpers.GetTypeAttrForTypeInfo(iTypeInfoFromIDispatch);
		if (_comTypeDesc == null)
		{
			lock (s_cacheComTypeDesc)
			{
				if (s_cacheComTypeDesc.TryGetValue(typeAttrForTypeInfo.guid, out _comTypeDesc) && _comTypeDesc.Events != null)
				{
					return;
				}
			}
		}
		ComTypeDesc comTypeDesc = ComTypeDesc.FromITypeInfo(iTypeInfoFromIDispatch, typeAttrForTypeInfo);
		Dictionary<string, ComEventDesc> events;
		ITypeInfo coClassTypeInfo;
		if (!(base.RuntimeCallableWrapper is IConnectionPointContainer))
		{
			events = ComTypeDesc.EmptyEvents;
		}
		else if ((coClassTypeInfo = GetCoClassTypeInfo(base.RuntimeCallableWrapper, iTypeInfoFromIDispatch)) == null)
		{
			events = ComTypeDesc.EmptyEvents;
		}
		else
		{
			events = new Dictionary<string, ComEventDesc>();
			TYPEATTR typeAttrForTypeInfo2 = ComRuntimeHelpers.GetTypeAttrForTypeInfo(coClassTypeInfo);
			for (int i = 0; i < typeAttrForTypeInfo2.cImplTypes; i++)
			{
				coClassTypeInfo.GetRefTypeOfImplType(i, out var href);
				coClassTypeInfo.GetRefTypeInfo(href, out ITypeInfo ppTI);
				coClassTypeInfo.GetImplTypeFlags(i, out var pImplTypeFlags);
				if ((pImplTypeFlags & IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE) != 0)
				{
					ScanSourceInterface(ppTI, ref events);
				}
			}
			if (events.Count == 0)
			{
				events = ComTypeDesc.EmptyEvents;
			}
		}
		lock (s_cacheComTypeDesc)
		{
			if (s_cacheComTypeDesc.TryGetValue(typeAttrForTypeInfo.guid, out var value))
			{
				_comTypeDesc = value;
			}
			else
			{
				_comTypeDesc = comTypeDesc;
				s_cacheComTypeDesc.Add(typeAttrForTypeInfo.guid, _comTypeDesc);
			}
			_comTypeDesc.Events = events;
		}
	}

	private static void ScanSourceInterface(ITypeInfo sourceTypeInfo, ref Dictionary<string, ComEventDesc> events)
	{
		TYPEATTR typeAttrForTypeInfo = ComRuntimeHelpers.GetTypeAttrForTypeInfo(sourceTypeInfo);
		for (int i = 0; i < typeAttrForTypeInfo.cFuncs; i++)
		{
			nint funcDescHandle = IntPtr.Zero;
			try
			{
				GetFuncDescForDescIndex(sourceTypeInfo, i, out var funcDesc, out funcDescHandle);
				if ((funcDesc.wFuncFlags & 0x40) == 0 && (funcDesc.wFuncFlags & 1) == 0)
				{
					string nameOfMethod = ComRuntimeHelpers.GetNameOfMethod(sourceTypeInfo, funcDesc.memid);
					nameOfMethod = nameOfMethod.ToUpper(CultureInfo.InvariantCulture);
					if (!events.ContainsKey(nameOfMethod))
					{
						ComEventDesc value = new ComEventDesc
						{
							Dispid = funcDesc.memid,
							SourceIID = typeAttrForTypeInfo.guid
						};
						events.Add(nameOfMethod, value);
					}
				}
			}
			finally
			{
				if (funcDescHandle != IntPtr.Zero)
				{
					sourceTypeInfo.ReleaseFuncDesc(funcDescHandle);
				}
			}
		}
	}

	private static ITypeInfo GetCoClassTypeInfo(object rcw, ITypeInfo typeInfo)
	{
		if (rcw is IProvideClassInfo provideClassInfo)
		{
			nint info = IntPtr.Zero;
			try
			{
				provideClassInfo.GetClassInfo(out info);
				if (info != IntPtr.Zero)
				{
					return Marshal.GetObjectForIUnknown(info) as ITypeInfo;
				}
			}
			finally
			{
				if (info != IntPtr.Zero)
				{
					Marshal.Release(info);
				}
			}
		}
		typeInfo.GetContainingTypeLib(out ITypeLib ppTLB, out int _);
		string nameOfType = ComRuntimeHelpers.GetNameOfType(typeInfo);
		ComTypeClassDesc coClassForInterface = ComTypeLibDesc.GetFromTypeLib(ppTLB).GetCoClassForInterface(nameOfType);
		if (coClassForInterface == null)
		{
			return null;
		}
		Guid guid = coClassForInterface.Guid;
		ppTLB.GetTypeInfoOfGuid(ref guid, out ITypeInfo ppTInfo);
		return ppTInfo;
	}

	private void EnsureScanDefinedMethods()
	{
		if (_comTypeDesc?.Funcs != null)
		{
			return;
		}
		ITypeInfo iTypeInfoFromIDispatch = ComRuntimeHelpers.GetITypeInfoFromIDispatch(DispatchObject);
		if (iTypeInfoFromIDispatch == null)
		{
			_comTypeDesc = ComTypeDesc.CreateEmptyTypeDesc();
			return;
		}
		TYPEATTR typeAttrForTypeInfo = ComRuntimeHelpers.GetTypeAttrForTypeInfo(iTypeInfoFromIDispatch);
		if (_comTypeDesc == null)
		{
			lock (s_cacheComTypeDesc)
			{
				if (s_cacheComTypeDesc.TryGetValue(typeAttrForTypeInfo.guid, out _comTypeDesc) && _comTypeDesc.Funcs != null)
				{
					return;
				}
			}
		}
		ComTypeDesc comTypeDesc = ComTypeDesc.FromITypeInfo(iTypeInfoFromIDispatch, typeAttrForTypeInfo);
		ComMethodDesc candidate = null;
		ComMethodDesc comMethodDesc = null;
		Hashtable hashtable = new Hashtable(typeAttrForTypeInfo.cFuncs);
		Hashtable hashtable2 = new Hashtable();
		Hashtable hashtable3 = new Hashtable();
		for (int i = 0; i < typeAttrForTypeInfo.cFuncs; i++)
		{
			nint funcDescHandle = IntPtr.Zero;
			try
			{
				GetFuncDescForDescIndex(iTypeInfoFromIDispatch, i, out var funcDesc, out funcDescHandle);
				if ((funcDesc.wFuncFlags & 1) != 0)
				{
					continue;
				}
				ComMethodDesc comMethodDesc2 = new ComMethodDesc(iTypeInfoFromIDispatch, funcDesc);
				string key = comMethodDesc2.Name.ToUpper(CultureInfo.InvariantCulture);
				if ((funcDesc.invkind & INVOKEKIND.INVOKE_PROPERTYPUT) != 0)
				{
					hashtable2.Add(key, comMethodDesc2);
					if (comMethodDesc2.DispId == 0 && comMethodDesc == null)
					{
						comMethodDesc = comMethodDesc2;
					}
				}
				else if ((funcDesc.invkind & INVOKEKIND.INVOKE_PROPERTYPUTREF) != 0)
				{
					hashtable3.Add(key, comMethodDesc2);
					if (comMethodDesc2.DispId == 0 && comMethodDesc == null)
					{
						comMethodDesc = comMethodDesc2;
					}
				}
				else if (funcDesc.memid == -4)
				{
					hashtable.Add("GETENUMERATOR", comMethodDesc2);
				}
				else
				{
					hashtable.Add(key, comMethodDesc2);
					if (funcDesc.memid == 0)
					{
						candidate = comMethodDesc2;
					}
				}
			}
			finally
			{
				if (funcDescHandle != IntPtr.Zero)
				{
					iTypeInfoFromIDispatch.ReleaseFuncDesc(funcDescHandle);
				}
			}
		}
		lock (s_cacheComTypeDesc)
		{
			if (s_cacheComTypeDesc.TryGetValue(typeAttrForTypeInfo.guid, out var value))
			{
				_comTypeDesc = value;
			}
			else
			{
				_comTypeDesc = comTypeDesc;
				s_cacheComTypeDesc.Add(typeAttrForTypeInfo.guid, _comTypeDesc);
			}
			_comTypeDesc.Funcs = hashtable;
			_comTypeDesc.Puts = hashtable2;
			_comTypeDesc.PutRefs = hashtable3;
			_comTypeDesc.EnsureGetItem(candidate);
			_comTypeDesc.EnsureSetItem(comMethodDesc);
		}
	}

	internal bool TryGetPropertySetter(string name, out ComMethodDesc method, Type limitType, bool holdsNull)
	{
		EnsureScanDefinedMethods();
		if (ComBinderHelpers.PreferPut(limitType, holdsNull))
		{
			if (!_comTypeDesc.TryGetPut(name, out method))
			{
				return _comTypeDesc.TryGetPutRef(name, out method);
			}
			return true;
		}
		if (!_comTypeDesc.TryGetPutRef(name, out method))
		{
			return _comTypeDesc.TryGetPut(name, out method);
		}
		return true;
	}
}
