using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Versioning;

namespace System.Runtime.InteropServices;

[SupportedOSPlatform("windows")]
internal sealed class ComEventsSink : IDispatch, ICustomQueryInterface
{
	private Guid _iidSourceItf;

	private IConnectionPoint _connectionPoint;

	private int _cookie;

	private ComEventsMethod _methods;

	private ComEventsSink _next;

	public ComEventsSink(object rcw, Guid iid)
	{
		_iidSourceItf = iid;
		Advise(rcw);
	}

	public static ComEventsSink Find(ComEventsSink sinks, ref Guid iid)
	{
		ComEventsSink comEventsSink = sinks;
		while (comEventsSink != null && comEventsSink._iidSourceItf != iid)
		{
			comEventsSink = comEventsSink._next;
		}
		return comEventsSink;
	}

	public static ComEventsSink Add(ComEventsSink sinks, ComEventsSink sink)
	{
		sink._next = sinks;
		return sink;
	}

	public static ComEventsSink RemoveAll(ComEventsSink sinks)
	{
		while (sinks != null)
		{
			sinks.Unadvise();
			sinks = sinks._next;
		}
		return null;
	}

	public static ComEventsSink Remove(ComEventsSink sinks, ComEventsSink sink)
	{
		ComEventsSink result = sinks;
		if (sink == sinks)
		{
			result = sinks._next;
		}
		else
		{
			ComEventsSink comEventsSink = sinks;
			while (comEventsSink != null && comEventsSink._next != sink)
			{
				comEventsSink = comEventsSink._next;
			}
			if (comEventsSink != null)
			{
				comEventsSink._next = sink._next;
			}
		}
		sink.Unadvise();
		return result;
	}

	public ComEventsMethod RemoveMethod(ComEventsMethod method)
	{
		_methods = ComEventsMethod.Remove(_methods, method);
		return _methods;
	}

	public ComEventsMethod FindMethod(int dispid)
	{
		return ComEventsMethod.Find(_methods, dispid);
	}

	public ComEventsMethod AddMethod(int dispid)
	{
		ComEventsMethod comEventsMethod = new ComEventsMethod(dispid);
		_methods = ComEventsMethod.Add(_methods, comEventsMethod);
		return comEventsMethod;
	}

	int IDispatch.GetTypeInfoCount()
	{
		return 0;
	}

	ITypeInfo IDispatch.GetTypeInfo(int iTInfo, int lcid)
	{
		throw new NotImplementedException();
	}

	void IDispatch.GetIDsOfNames(ref Guid iid, string[] names, int cNames, int lcid, int[] rgDispId)
	{
		throw new NotImplementedException();
	}

	private unsafe static ref ComVariant GetVariant(ref ComVariant pSrc)
	{
		if (pSrc.VarType == (VarEnum)16396)
		{
			ref ComVariant reference = ref *(ComVariant*)((IntPtr)pSrc.GetRawDataRef<nint>()).ToPointer();
			if ((reference.VarType & (VarEnum)0x4FFF) == (VarEnum)16396)
			{
				return ref reference;
			}
		}
		return ref pSrc;
	}

	unsafe void IDispatch.Invoke(int dispid, ref Guid riid, int lcid, InvokeFlags wFlags, ref DISPPARAMS pDispParams, nint pVarResult, nint pExcepInfo, nint puArgErr)
	{
		ComEventsMethod comEventsMethod = FindMethod(dispid);
		if (comEventsMethod == null)
		{
			return;
		}
		object[] array = new object[pDispParams.cArgs];
		int[] array2 = new int[pDispParams.cArgs];
		bool[] array3 = new bool[pDispParams.cArgs];
		int length = pDispParams.cNamedArgs + pDispParams.cArgs;
		Span<ComVariant> span = new Span<ComVariant>(((IntPtr)pDispParams.rgvarg).ToPointer(), length);
		Span<int> span2 = new Span<int>(((IntPtr)pDispParams.rgdispidNamedArgs).ToPointer(), length);
		int num;
		int i;
		for (i = 0; i < pDispParams.cNamedArgs; i++)
		{
			num = span2[i];
			ref ComVariant variant = ref GetVariant(ref span[i]);
			array[num] = variant.ToObject();
			array3[num] = true;
			int num2 = -1;
			if (variant.VarType.HasFlag(VarEnum.VT_BYREF))
			{
				num2 = i;
			}
			array2[num] = num2;
		}
		num = 0;
		for (; i < pDispParams.cArgs; i++)
		{
			for (; array3[num]; num++)
			{
			}
			ref ComVariant variant2 = ref GetVariant(ref span[pDispParams.cArgs - 1 - i]);
			array[num] = variant2.ToObject();
			int num3 = -1;
			if (variant2.VarType.HasFlag(VarEnum.VT_BYREF))
			{
				num3 = pDispParams.cArgs - 1 - i;
			}
			array2[num] = num3;
			num++;
		}
		object obj = comEventsMethod.Invoke(array);
		if (pVarResult != IntPtr.Zero)
		{
			Marshal.GetNativeVariantForObject(obj, pVarResult);
		}
		for (i = 0; i < pDispParams.cArgs; i++)
		{
			int num4 = array2[i];
			if (num4 != -1)
			{
				GetVariant(ref span[num4]).CopyFromIndirect(array[i]);
			}
		}
	}

	CustomQueryInterfaceResult ICustomQueryInterface.GetInterface(ref Guid iid, out nint ppv)
	{
		ppv = IntPtr.Zero;
		if (iid == _iidSourceItf || iid == typeof(IDispatch).GUID)
		{
			ppv = Marshal.GetComInterfaceForObject(this, typeof(IDispatch), CustomQueryInterfaceMode.Ignore);
			return CustomQueryInterfaceResult.Handled;
		}
		return CustomQueryInterfaceResult.NotHandled;
	}

	private void Advise(object rcw)
	{
		((IConnectionPointContainer)rcw).FindConnectionPoint(ref _iidSourceItf, out IConnectionPoint ppCP);
		ppCP.Advise(this, out _cookie);
		_connectionPoint = ppCP;
	}

	private void Unadvise()
	{
		if (_connectionPoint == null)
		{
			return;
		}
		try
		{
			_connectionPoint.Unadvise(_cookie);
			Marshal.ReleaseComObject(_connectionPoint);
		}
		catch
		{
		}
		finally
		{
			_connectionPoint = null;
		}
	}
}
