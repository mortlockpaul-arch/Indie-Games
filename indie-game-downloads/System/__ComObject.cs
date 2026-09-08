using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.CustomMarshalers;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Versioning;

namespace System;

[SupportedOSPlatform("windows")]
internal class __ComObject : MarshalByRefObject, IDynamicInterfaceCastable
{
	[DynamicInterfaceCastableImplementation]
	private interface IEnumerableOverDispatchImpl : IEnumerable
	{
		unsafe static IEnumVARIANT QueryForNewEnum(__ComObject obj)
		{
			IDispatch dispatch = (IDispatch)obj;
			ComVariant variant = default(ComVariant);
			object obj2 = null;
			try
			{
				void* value = &variant;
				DISPPARAMS pDispParams = default(DISPPARAMS);
				Guid riid = Guid.Empty;
				dispatch.Invoke(-4, ref riid, 1, InvokeFlags.DISPATCH_METHOD | InvokeFlags.DISPATCH_PROPERTYGET, ref pDispParams, new IntPtr(value), IntPtr.Zero, IntPtr.Zero);
				obj2 = variant.ToObject();
			}
			finally
			{
				variant.Dispose();
			}
			return (obj2 as IEnumVARIANT) ?? throw new InvalidOperationException(SR.InvalidOp_InvalidNewEnumVariant);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			IEnumVARIANT o = QueryForNewEnum((__ComObject)this);
			nint num = IntPtr.Zero;
			try
			{
				num = Marshal.GetIUnknownForObject(o);
				return (IEnumerator)EnumeratorToEnumVariantMarshaler.GetInstance(null).MarshalNativeToManaged(num);
			}
			finally
			{
				if (num != IntPtr.Zero)
				{
					Marshal.Release(num);
				}
			}
		}
	}

	[DynamicInterfaceCastableImplementation]
	private interface ICustomAdapterOverDispatchImpl : ICustomAdapter
	{
		object ICustomAdapter.GetUnderlyingObject()
		{
			return this;
		}
	}

	[DynamicInterfaceCastableImplementation]
	private interface IEnumeratorOverEnumVARIANTImpl : IEnumerator
	{
		object IEnumerator.Current => ((IEnumerator)((__ComObject)this).GetData(typeof(IEnumeratorOverEnumVARIANTImpl))).Current;

		bool IEnumerator.MoveNext()
		{
			return ((IEnumerator)((__ComObject)this).GetData(typeof(IEnumeratorOverEnumVARIANTImpl))).MoveNext();
		}

		void IEnumerator.Reset()
		{
			((IEnumerator)((__ComObject)this).GetData(typeof(IEnumeratorOverEnumVARIANTImpl))).Reset();
		}
	}

	private Hashtable m_ObjectToDataMap;

	protected __ComObject()
	{
	}

	internal object GetData(object key)
	{
		object result = null;
		lock (this)
		{
			if (m_ObjectToDataMap != null)
			{
				result = m_ObjectToDataMap[key];
			}
		}
		return result;
	}

	internal bool SetData(object key, object data)
	{
		bool result = false;
		lock (this)
		{
			if (m_ObjectToDataMap == null)
			{
				m_ObjectToDataMap = new Hashtable();
			}
			if (m_ObjectToDataMap[key] == null)
			{
				m_ObjectToDataMap[key] = data;
				result = true;
			}
		}
		return result;
	}

	internal void ReleaseAllData()
	{
		lock (this)
		{
			if (m_ObjectToDataMap == null)
			{
				return;
			}
			foreach (object value in m_ObjectToDataMap.Values)
			{
				if (value is IDisposable disposable)
				{
					disposable.Dispose();
				}
				if (value is __ComObject o)
				{
					Marshal.ReleaseComObject(o);
				}
			}
			m_ObjectToDataMap = null;
		}
	}

	internal object GetEventProvider([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] RuntimeType t)
	{
		object data = GetData(t);
		if (data != null)
		{
			return data;
		}
		return CreateEventProvider(t);
	}

	private object CreateEventProvider([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] RuntimeType t)
	{
		object obj = Activator.CreateInstance(t, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, new object[1] { this }, null);
		if (!SetData(t, obj))
		{
			if (obj is IDisposable disposable)
			{
				disposable.Dispose();
			}
			obj = GetData(t);
		}
		return obj;
	}

	bool IDynamicInterfaceCastable.IsInterfaceImplemented(RuntimeTypeHandle interfaceType, bool throwIfNotImplemented)
	{
		if (interfaceType.Equals(typeof(IEnumerable).TypeHandle))
		{
			return IsIEnumerable(this);
		}
		if (interfaceType.Equals(typeof(IEnumerator).TypeHandle))
		{
			return IsIEnumerator(this);
		}
		if (interfaceType.Equals(typeof(ICustomAdapter).TypeHandle))
		{
			if (!IsIEnumerator(this))
			{
				return IsIEnumerable(this);
			}
			return true;
		}
		return false;
		static bool IsIEnumerable(__ComObject co)
		{
			try
			{
				IEnumerableOverDispatchImpl.QueryForNewEnum(co);
				return true;
			}
			catch
			{
				return false;
			}
		}
		static bool IsIEnumerator(__ComObject co)
		{
			return co is IEnumVARIANT;
		}
	}

	RuntimeTypeHandle IDynamicInterfaceCastable.GetInterfaceImplementation(RuntimeTypeHandle interfaceType)
	{
		if (interfaceType.Equals(typeof(IEnumerable).TypeHandle))
		{
			return typeof(IEnumerableOverDispatchImpl).TypeHandle;
		}
		if (interfaceType.Equals(typeof(ICustomAdapter).TypeHandle))
		{
			return typeof(ICustomAdapterOverDispatchImpl).TypeHandle;
		}
		if (interfaceType.Equals(typeof(IEnumerator).TypeHandle))
		{
			object obj = GetData(typeof(IEnumeratorOverEnumVARIANTImpl));
			if (obj == null)
			{
				nint num = IntPtr.Zero;
				try
				{
					num = Marshal.GetIUnknownForObject((IEnumVARIANT)this);
					obj = (IEnumerator)EnumeratorToEnumVariantMarshaler.GetInstance(null).MarshalNativeToManaged(num);
				}
				finally
				{
					if (num != IntPtr.Zero)
					{
						Marshal.Release(num);
					}
				}
				SetData(typeof(IEnumeratorOverEnumVARIANTImpl), obj);
			}
			return typeof(IEnumeratorOverEnumVARIANTImpl).TypeHandle;
		}
		return default(RuntimeTypeHandle);
	}
}
