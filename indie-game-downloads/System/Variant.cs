using System.CodeDom.Compiler;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.StubHelpers;

namespace System;

internal static class Variant
{
	internal static bool IsSystemDrawingColor(Type type)
	{
		return type.FullName == "System.Drawing.Color";
	}

	[LibraryImport("QCall", EntryPoint = "Variant_ConvertValueTypeToRecord")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private unsafe static void ConvertValueTypeToRecord(ObjectHandleOnStack obj, out ComVariant pOle)
	{
		pOle = default(ComVariant);
		fixed (ComVariant* _pOle_native = &pOle)
		{
			__PInvoke(obj, _pOle_native);
		}
		[DllImport("QCall", EntryPoint = "Variant_ConvertValueTypeToRecord", ExactSpelling = true)]
		unsafe static extern void __PInvoke(ObjectHandleOnStack __obj_native, ComVariant* __pOle_native);
	}

	internal static ComVariant GetIUnknownOrIDispatchFromObject(object obj)
	{
		nint iUnknownOrIDispatchForObject = GetIUnknownOrIDispatchForObject(ObjectHandleOnStack.Create(ref obj), out var isIDispatch);
		return ComVariant.CreateRaw<nint>(isIDispatch ? VarEnum.VT_DISPATCH : VarEnum.VT_UNKNOWN, iUnknownOrIDispatchForObject);
	}

	[LibraryImport("QCall", EntryPoint = "MarshalNative_GetIUnknownOrIDispatchForObject")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private unsafe static nint GetIUnknownOrIDispatchForObject(ObjectHandleOnStack o, [MarshalAs(UnmanagedType.Bool)] out bool isIDispatch)
	{
		isIDispatch = false;
		Unsafe.SkipInit(out int num);
		nint result = __PInvoke(o, &num);
		isIDispatch = num != 0;
		return result;
		[DllImport("QCall", EntryPoint = "MarshalNative_GetIUnknownOrIDispatchForObject", ExactSpelling = true)]
		unsafe static extern nint __PInvoke(ObjectHandleOnStack __o_native, int* __isIDispatch_native);
	}

	private static object GetObjectFromIUnknown(nint pUnk)
	{
		if (pUnk != IntPtr.Zero)
		{
			return Marshal.GetObjectForIUnknown(pUnk);
		}
		return null;
	}

	private static object ConvertWrappedObject(object wrapped)
	{
		if (wrapped is ValueType)
		{
			if (wrapped is Enum)
			{
				return wrapped.GetType();
			}
			if ((wrapped is nint || wrapped is nuint) ? true : false)
			{
				return wrapped;
			}
			if (wrapped.GetType().IsPrimitive)
			{
				return null;
			}
			if (IsSystemDrawingColor(wrapped.GetType()))
			{
				return null;
			}
			if ((wrapped is DateTime || wrapped is TimeSpan || wrapped is Currency) ? true : false)
			{
				return null;
			}
			return wrapped;
		}
		if (wrapped is Empty)
		{
			return null;
		}
		return wrapped;
	}

	internal static void MarshalHelperConvertObjectToVariant(object o, out ComVariant pOle)
	{
		object obj = o;
		if (obj != null)
		{
			int num;
			if (obj is IConvertible convertible)
			{
				if (convertible.GetTypeCode() != TypeCode.Object)
				{
					IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
					pOle = convertible.GetTypeCode() switch
					{
						TypeCode.Empty => default(ComVariant), 
						TypeCode.DBNull => ComVariant.Create(DBNull.Value), 
						TypeCode.Boolean => ComVariant.Create(convertible.ToBoolean(invariantCulture)), 
						TypeCode.Char => ComVariant.Create((ushort)convertible.ToChar(invariantCulture)), 
						TypeCode.SByte => ComVariant.Create(convertible.ToSByte(invariantCulture)), 
						TypeCode.Byte => ComVariant.Create(convertible.ToByte(invariantCulture)), 
						TypeCode.Int16 => ComVariant.Create(convertible.ToInt16(invariantCulture)), 
						TypeCode.UInt16 => ComVariant.Create(convertible.ToUInt16(invariantCulture)), 
						TypeCode.Int32 => ComVariant.Create(convertible.ToInt32(invariantCulture)), 
						TypeCode.UInt32 => ComVariant.Create(convertible.ToUInt32(invariantCulture)), 
						TypeCode.Int64 => ComVariant.Create(convertible.ToInt64(invariantCulture)), 
						TypeCode.UInt64 => ComVariant.Create(convertible.ToUInt64(invariantCulture)), 
						TypeCode.Single => ComVariant.Create(convertible.ToSingle(invariantCulture)), 
						TypeCode.Double => ComVariant.Create(convertible.ToDouble(invariantCulture)), 
						TypeCode.Decimal => ComVariant.Create(convertible.ToDecimal(invariantCulture)), 
						TypeCode.DateTime => ComVariant.Create(convertible.ToDateTime(invariantCulture)), 
						TypeCode.String => ComVariant.Create(convertible.ToString(invariantCulture)), 
						_ => throw new NotSupportedException(SR.Format(SR.NotSupported_UnknownTypeCode, convertible.GetTypeCode())), 
					};
					return;
				}
				if (obj is DBNull)
				{
					pOle = ComVariant.Create(DBNull.Value);
					return;
				}
				num = 1;
			}
			else
			{
				if (obj is Missing)
				{
					pOle = ComVariant.CreateRaw(VarEnum.VT_ERROR, -2147352572);
					return;
				}
				if (obj is UnknownWrapper unknownWrapper)
				{
					object obj2 = ConvertWrappedObject(unknownWrapper.WrappedObject);
					pOle = ComVariant.CreateRaw<nint>(VarEnum.VT_UNKNOWN, (obj2 == null) ? IntPtr.Zero : Marshal.GetIUnknownForObject(obj2));
					return;
				}
				if (obj is DispatchWrapper dispatchWrapper)
				{
					object obj3 = ConvertWrappedObject(dispatchWrapper.WrappedObject);
					pOle = ComVariant.CreateRaw<nint>(VarEnum.VT_DISPATCH, (obj3 == null) ? IntPtr.Zero : Marshal.GetIDispatchForObject(obj3));
					return;
				}
				if (obj is ErrorWrapper value)
				{
					pOle = ComVariant.Create(value);
					return;
				}
				if (obj is CurrencyWrapper value2)
				{
					pOle = ComVariant.Create(value2);
					return;
				}
				if (obj is BStrWrapper value3)
				{
					pOle = ComVariant.Create(value3);
					return;
				}
				if (obj is Empty)
				{
					pOle = default(ComVariant);
					return;
				}
				num = 2;
			}
			if (!IsSystemDrawingColor(o.GetType()))
			{
				if (num == 1)
				{
					if (obj is ValueType)
					{
						goto IL_03f2;
					}
					if (obj is SafeHandle)
					{
						goto IL_0400;
					}
					if (obj is CriticalHandle)
					{
						goto IL_040b;
					}
				}
				else
				{
					if (num != 2)
					{
						goto IL_03c1;
					}
					if (obj is TimeSpan)
					{
						throw new ArgumentException(SR.ComVariant_UnsupportedSignature);
					}
					if (obj is Currency currency)
					{
						pOle = ComVariant.CreateRaw(VarEnum.VT_CY, currency.m_value);
						return;
					}
					if (obj is ValueType)
					{
						goto IL_03f2;
					}
					if (obj is SafeHandle)
					{
						goto IL_0400;
					}
					if (obj is CriticalHandle)
					{
						goto IL_040b;
					}
					if (obj is VariantWrapper)
					{
						throw new ArgumentException(SR.ComVariant_VariantWrapper_In_Variant);
					}
				}
				pOle = GetIUnknownOrIDispatchFromObject(o);
				return;
			}
			goto IL_03c1;
		}
		pOle = default(ComVariant);
		return;
		IL_0400:
		throw new ArgumentException(SR.ComVariant_SafeHandle_In_Variant);
		IL_03c1:
		pOle = ComVariant.Create((uint)ColorMarshaler.ConvertToNative(o));
		return;
		IL_03f2:
		ConvertValueTypeToRecord(ObjectHandleOnStack.Create(ref o), out pOle);
		return;
		IL_040b:
		throw new ArgumentException(SR.ComVariant_CriticalHandle_In_Variant);
	}

	internal unsafe static object MarshalHelperConvertVariantToObject(ref readonly ComVariant pOle)
	{
		switch (pOle.VarType)
		{
		case VarEnum.VT_I4:
		case VarEnum.VT_INT:
			return pOle.As<int>();
		case (VarEnum)16387:
		case (VarEnum)16406:
			return *(int*)pOle.GetRawDataRef<nint>();
		case VarEnum.VT_UI4:
		case VarEnum.VT_UINT:
			return pOle.As<uint>();
		case (VarEnum)16403:
		case (VarEnum)16407:
			return *(uint*)pOle.GetRawDataRef<nint>();
		case VarEnum.VT_I1:
			return pOle.As<sbyte>();
		case (VarEnum)16400:
			return *(sbyte*)pOle.GetRawDataRef<nint>();
		case VarEnum.VT_UI1:
			return pOle.As<byte>();
		case (VarEnum)16401:
			return *(byte*)pOle.GetRawDataRef<nint>();
		case VarEnum.VT_I2:
			return pOle.As<short>();
		case (VarEnum)16386:
			return *(short*)pOle.GetRawDataRef<nint>();
		case VarEnum.VT_UI2:
			return pOle.As<ushort>();
		case (VarEnum)16402:
			return *(ushort*)pOle.GetRawDataRef<nint>();
		case VarEnum.VT_I8:
			return pOle.As<long>();
		case (VarEnum)16404:
			return *(long*)pOle.GetRawDataRef<nint>();
		case VarEnum.VT_UI8:
			return pOle.As<ulong>();
		case (VarEnum)16405:
			return *(ulong*)pOle.GetRawDataRef<nint>();
		case VarEnum.VT_R4:
			return pOle.As<float>();
		case (VarEnum)16388:
			return *(float*)pOle.GetRawDataRef<nint>();
		case VarEnum.VT_R8:
			return pOle.As<double>();
		case (VarEnum)16389:
			return *(double*)pOle.GetRawDataRef<nint>();
		case VarEnum.VT_BOOL:
			return pOle.As<bool>();
		case (VarEnum)16395:
			return *(short*)pOle.GetRawDataRef<nint>() != 0;
		case VarEnum.VT_BSTR:
			return pOle.As<string>();
		case (VarEnum)16392:
		{
			nint rawDataRef3 = *(nint*)pOle.GetRawDataRef<nint>();
			if (rawDataRef3 != IntPtr.Zero)
			{
				return Marshal.PtrToStringBSTR(rawDataRef3);
			}
			return null;
		}
		case VarEnum.VT_EMPTY:
			return null;
		case VarEnum.VT_BYREF:
			return (ulong)pOle.GetRawDataRef<nint>();
		case VarEnum.VT_NULL:
		case (VarEnum)16385:
			return DBNull.Value;
		case VarEnum.VT_DATE:
			return pOle.As<DateTime>();
		case (VarEnum)16391:
			return DateTime.FromOADate(*(double*)pOle.GetRawDataRef<nint>());
		case VarEnum.VT_DECIMAL:
			return pOle.As<decimal>();
		case (VarEnum)16398:
			return *(decimal*)pOle.GetRawDataRef<nint>();
		case VarEnum.VT_CY:
			return decimal.FromOACurrency(pOle.GetRawDataRef<long>());
		case (VarEnum)16390:
			return decimal.FromOACurrency(*(long*)pOle.GetRawDataRef<nint>());
		case VarEnum.VT_DISPATCH:
		case VarEnum.VT_UNKNOWN:
			return GetObjectFromIUnknown(pOle.GetRawDataRef<nint>());
		case (VarEnum)16393:
		case (VarEnum)16397:
			return GetObjectFromIUnknown(*(nint*)pOle.GetRawDataRef<nint>());
		case VarEnum.VT_ERROR:
		{
			int rawDataRef2 = pOle.GetRawDataRef<int>();
			if (rawDataRef2 != -2147352572)
			{
				return rawDataRef2;
			}
			return Missing.Value;
		}
		case (VarEnum)16394:
		{
			int rawDataRef = *(int*)pOle.GetRawDataRef<nint>();
			if (rawDataRef != -2147352572)
			{
				return rawDataRef;
			}
			return Missing.Value;
		}
		case VarEnum.VT_VOID:
		case (VarEnum)16408:
			return null;
		default:
			throw new ArgumentException(SR.ComVariant_UnsupportedType);
		}
	}

	internal static void MarshalHelperCastVariant(object pValue, int vt, out ComVariant v)
	{
		if (!(pValue is IConvertible convertible))
		{
			switch ((VarEnum)vt)
			{
			case VarEnum.VT_DISPATCH:
				v = ComVariant.CreateRaw<nint>(VarEnum.VT_DISPATCH, (pValue == null) ? IntPtr.Zero : Marshal.GetIDispatchForObject(pValue));
				return;
			case VarEnum.VT_UNKNOWN:
				v = ComVariant.CreateRaw<nint>(VarEnum.VT_UNKNOWN, (pValue == null) ? IntPtr.Zero : Marshal.GetIUnknownForObject(pValue));
				return;
			case VarEnum.VT_RECORD:
				MarshalHelperConvertObjectToVariant(pValue, out v);
				if (v.VarType != VarEnum.VT_RECORD)
				{
					v.Dispose();
					throw new InvalidCastException(SR.InvalidCast_CannotCoerceByRefVariant);
				}
				return;
			case VarEnum.VT_BSTR:
				if (pValue == null)
				{
					v = ComVariant.CreateRaw<nint>(VarEnum.VT_BSTR, IntPtr.Zero);
					return;
				}
				break;
			}
			throw new InvalidCastException(SR.InvalidCast_CannotCoerceByRefVariant);
		}
		IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
		v = (VarEnum)vt switch
		{
			VarEnum.VT_EMPTY => default(ComVariant), 
			VarEnum.VT_NULL => ComVariant.Create(DBNull.Value), 
			VarEnum.VT_I2 => ComVariant.Create(convertible.ToInt16(invariantCulture)), 
			VarEnum.VT_I4 => ComVariant.Create(convertible.ToInt32(invariantCulture)), 
			VarEnum.VT_R4 => ComVariant.Create(convertible.ToSingle(invariantCulture)), 
			VarEnum.VT_R8 => ComVariant.Create(convertible.ToDouble(invariantCulture)), 
			VarEnum.VT_CY => ComVariant.CreateRaw(VarEnum.VT_CY, decimal.ToOACurrency(convertible.ToDecimal(invariantCulture))), 
			VarEnum.VT_DATE => ComVariant.Create(convertible.ToDateTime(invariantCulture)), 
			VarEnum.VT_BSTR => ComVariant.Create(convertible.ToString(invariantCulture)), 
			VarEnum.VT_DISPATCH => ComVariant.CreateRaw<nint>(VarEnum.VT_DISPATCH, Marshal.GetIDispatchForObject(convertible)), 
			VarEnum.VT_ERROR => ComVariant.CreateRaw(VarEnum.VT_ERROR, convertible.ToInt32(invariantCulture)), 
			VarEnum.VT_BOOL => ComVariant.Create(convertible.ToBoolean(invariantCulture)), 
			VarEnum.VT_UNKNOWN => ComVariant.CreateRaw<nint>(VarEnum.VT_UNKNOWN, Marshal.GetIUnknownForObject(convertible)), 
			VarEnum.VT_DECIMAL => ComVariant.Create(convertible.ToDecimal(invariantCulture)), 
			VarEnum.VT_I1 => ComVariant.Create(convertible.ToSByte(invariantCulture)), 
			VarEnum.VT_UI1 => ComVariant.Create(convertible.ToByte(invariantCulture)), 
			VarEnum.VT_UI2 => ComVariant.Create(convertible.ToUInt16(invariantCulture)), 
			VarEnum.VT_UI4 => ComVariant.Create(convertible.ToUInt32(invariantCulture)), 
			VarEnum.VT_I8 => ComVariant.Create(convertible.ToInt64(invariantCulture)), 
			VarEnum.VT_UI8 => ComVariant.Create(convertible.ToUInt64(invariantCulture)), 
			VarEnum.VT_INT => ComVariant.Create(convertible.ToInt32(invariantCulture)), 
			VarEnum.VT_UINT => ComVariant.Create(convertible.ToUInt32(invariantCulture)), 
			_ => throw new InvalidCastException(SR.InvalidCast_CannotCoerceByRefVariant), 
		};
	}
}
