using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal static class DynamicVariantExtensions
{
	private struct Record
	{
		public nint _record;

		public nint _recordInfo;
	}

	public static bool IsPrimitiveType(this VarEnum varEnum)
	{
		switch (varEnum)
		{
		case VarEnum.VT_I2:
		case VarEnum.VT_I4:
		case VarEnum.VT_R4:
		case VarEnum.VT_R8:
		case VarEnum.VT_CY:
		case VarEnum.VT_DATE:
		case VarEnum.VT_BSTR:
		case VarEnum.VT_ERROR:
		case VarEnum.VT_BOOL:
		case VarEnum.VT_DECIMAL:
		case VarEnum.VT_I1:
		case VarEnum.VT_UI1:
		case VarEnum.VT_UI2:
		case VarEnum.VT_UI4:
		case VarEnum.VT_I8:
		case VarEnum.VT_UI8:
		case VarEnum.VT_INT:
		case VarEnum.VT_UINT:
			return true;
		default:
			return false;
		}
	}

	public static void SetAsIConvertible(this ref ComVariant variant, IConvertible value)
	{
		TypeCode typeCode = value.GetTypeCode();
		CultureInfo currentCulture = CultureInfo.CurrentCulture;
		switch (typeCode)
		{
		case TypeCode.Object:
			variant = ComVariant.CreateRaw<nint>(VarEnum.VT_UNKNOWN, Marshal.GetIUnknownForObject(value));
			break;
		case TypeCode.DBNull:
			variant = ComVariant.Null;
			break;
		case TypeCode.Boolean:
			variant = ComVariant.Create(value.ToBoolean(currentCulture));
			break;
		case TypeCode.Char:
			variant = ComVariant.Create((ushort)value.ToChar(currentCulture));
			break;
		case TypeCode.SByte:
			variant = ComVariant.Create(value.ToSByte(currentCulture));
			break;
		case TypeCode.Byte:
			variant = ComVariant.Create(value.ToByte(currentCulture));
			break;
		case TypeCode.Int16:
			variant = ComVariant.Create(value.ToInt16(currentCulture));
			break;
		case TypeCode.UInt16:
			variant = ComVariant.Create(value.ToUInt16(currentCulture));
			break;
		case TypeCode.Int32:
			variant = ComVariant.Create(value.ToInt32(currentCulture));
			break;
		case TypeCode.UInt32:
			variant = ComVariant.Create(value.ToUInt32(currentCulture));
			break;
		case TypeCode.Int64:
			variant = ComVariant.Create(value.ToInt64(currentCulture));
			break;
		case TypeCode.UInt64:
			variant = ComVariant.Create(value.ToUInt64(currentCulture));
			break;
		case TypeCode.Single:
			variant = ComVariant.Create(value.ToSingle(currentCulture));
			break;
		case TypeCode.Double:
			variant = ComVariant.Create(value.ToDouble(currentCulture));
			break;
		case TypeCode.Decimal:
			variant = ComVariant.Create(value.ToDecimal(currentCulture));
			break;
		case TypeCode.DateTime:
			variant = ComVariant.Create(value.ToDateTime(currentCulture));
			break;
		case TypeCode.String:
			variant = ComVariant.Create(new BStrWrapper(value.ToString(currentCulture)));
			break;
		default:
			throw new NotSupportedException();
		case TypeCode.Empty:
			break;
		}
	}

	public static void SetAsByrefI1(this ref ComVariant variant, ref sbyte value)
	{
		variant.SetAsByref(ref value, VarEnum.VT_I1);
	}

	public static void SetAsByrefI2(this ref ComVariant variant, ref short value)
	{
		variant.SetAsByref(ref value, VarEnum.VT_I2);
	}

	public static void SetAsByrefI4(this ref ComVariant variant, ref int value)
	{
		variant.SetAsByref(ref value, VarEnum.VT_I4);
	}

	public static void SetAsByrefI8(this ref ComVariant variant, ref long value)
	{
		variant.SetAsByref(ref value, VarEnum.VT_I8);
	}

	public static void SetAsByrefUi1(this ref ComVariant variant, ref byte value)
	{
		variant.SetAsByref(ref value, VarEnum.VT_UI1);
	}

	public static void SetAsByrefUi2(this ref ComVariant variant, ref ushort value)
	{
		variant.SetAsByref(ref value, VarEnum.VT_UI2);
	}

	public static void SetAsByrefUi4(this ref ComVariant variant, ref uint value)
	{
		variant.SetAsByref(ref value, VarEnum.VT_UI4);
	}

	public static void SetAsByrefUi8(this ref ComVariant variant, ref ulong value)
	{
		variant.SetAsByref(ref value, VarEnum.VT_UI8);
	}

	public static void SetAsByrefInt(this ref ComVariant variant, ref int value)
	{
		variant.SetAsByref(ref value, VarEnum.VT_INT);
	}

	public static void SetAsByrefUint(this ref ComVariant variant, ref uint value)
	{
		variant.SetAsByref(ref value, VarEnum.VT_UINT);
	}

	public static void SetAsByrefBool(this ref ComVariant variant, ref short value)
	{
		variant.SetAsByref(ref value, VarEnum.VT_BOOL);
	}

	public static void SetAsByrefError(this ref ComVariant variant, ref int value)
	{
		variant.SetAsByref(ref value, VarEnum.VT_ERROR);
	}

	public static void SetAsByrefR4(this ref ComVariant variant, ref float value)
	{
		variant.SetAsByref(ref value, VarEnum.VT_R4);
	}

	public static void SetAsByrefR8(this ref ComVariant variant, ref double value)
	{
		variant.SetAsByref(ref value, VarEnum.VT_R8);
	}

	public static void SetAsByrefDecimal(this ref ComVariant variant, ref decimal value)
	{
		variant.SetAsByref(ref value, VarEnum.VT_DECIMAL);
	}

	public static void SetAsByrefCy(this ref ComVariant variant, ref long value)
	{
		variant.SetAsByref(ref value, VarEnum.VT_CY);
	}

	public static void SetAsByrefDate(this ref ComVariant variant, ref double value)
	{
		variant.SetAsByref(ref value, VarEnum.VT_DATE);
	}

	public static void SetAsByrefBstr(this ref ComVariant variant, ref nint value)
	{
		variant.SetAsByref(ref value, VarEnum.VT_BSTR);
	}

	public static void SetAsByrefUnknown(this ref ComVariant variant, ref nint value)
	{
		variant.SetAsByref(ref value, VarEnum.VT_UNKNOWN);
	}

	public static void SetAsByrefDispatch(this ref ComVariant variant, ref nint value)
	{
		variant.SetAsByref(ref value, VarEnum.VT_DISPATCH);
	}

	private unsafe static void SetAsByref<T>(this ref ComVariant variant, ref T value, VarEnum type)
	{
		variant = ComVariant.CreateRaw<nint>(type | VarEnum.VT_BYREF, (nint)Unsafe.AsPointer(in value));
	}

	public static void SetAsByrefVariant(this ref ComVariant variant, ref ComVariant value)
	{
		variant.SetAsByref(ref value, VarEnum.VT_VARIANT);
	}

	public unsafe static void SetAsByrefVariantIndirect(this ref ComVariant variant, ref ComVariant value)
	{
		switch (value.VarType)
		{
		case VarEnum.VT_EMPTY:
		case VarEnum.VT_NULL:
			variant.SetAsByrefVariant(ref value);
			break;
		case VarEnum.VT_RECORD:
			variant = ComVariant.CreateRaw(value.VarType | VarEnum.VT_BYREF, value.GetRawDataRef<Record>());
			break;
		case VarEnum.VT_DECIMAL:
			variant = ComVariant.CreateRaw<nint>(value.VarType | VarEnum.VT_BYREF, (nint)Unsafe.AsPointer(in value));
			break;
		default:
			variant = ComVariant.CreateRaw<nint>(value.VarType | VarEnum.VT_BYREF, (nint)Unsafe.AsPointer(in value.GetRawDataRef<nint>()));
			break;
		}
	}

	internal static MethodInfo GetByrefSetter(VarEnum varType)
	{
		switch (varType)
		{
		case VarEnum.VT_I1:
			return typeof(DynamicVariantExtensions).GetMethod("SetAsByrefI1");
		case VarEnum.VT_I2:
			return typeof(DynamicVariantExtensions).GetMethod("SetAsByrefI2");
		case VarEnum.VT_I4:
			return typeof(DynamicVariantExtensions).GetMethod("SetAsByrefI4");
		case VarEnum.VT_I8:
			return typeof(DynamicVariantExtensions).GetMethod("SetAsByrefI8");
		case VarEnum.VT_UI1:
			return typeof(DynamicVariantExtensions).GetMethod("SetAsByrefUi1");
		case VarEnum.VT_UI2:
			return typeof(DynamicVariantExtensions).GetMethod("SetAsByrefUi2");
		case VarEnum.VT_UI4:
			return typeof(DynamicVariantExtensions).GetMethod("SetAsByrefUi4");
		case VarEnum.VT_UI8:
			return typeof(DynamicVariantExtensions).GetMethod("SetAsByrefUi8");
		case VarEnum.VT_INT:
			return typeof(DynamicVariantExtensions).GetMethod("SetAsByrefInt");
		case VarEnum.VT_UINT:
			return typeof(DynamicVariantExtensions).GetMethod("SetAsByrefUint");
		case VarEnum.VT_BOOL:
			return typeof(DynamicVariantExtensions).GetMethod("SetAsByrefBool");
		case VarEnum.VT_ERROR:
			return typeof(DynamicVariantExtensions).GetMethod("SetAsByrefError");
		case VarEnum.VT_R4:
			return typeof(DynamicVariantExtensions).GetMethod("SetAsByrefR4");
		case VarEnum.VT_R8:
			return typeof(DynamicVariantExtensions).GetMethod("SetAsByrefR8");
		case VarEnum.VT_DECIMAL:
			return typeof(DynamicVariantExtensions).GetMethod("SetAsByrefDecimal");
		case VarEnum.VT_CY:
			return typeof(DynamicVariantExtensions).GetMethod("SetAsByrefCy");
		case VarEnum.VT_DATE:
			return typeof(DynamicVariantExtensions).GetMethod("SetAsByrefDate");
		case VarEnum.VT_BSTR:
			return typeof(DynamicVariantExtensions).GetMethod("SetAsByrefBstr");
		case VarEnum.VT_UNKNOWN:
			return typeof(DynamicVariantExtensions).GetMethod("SetAsByrefUnknown");
		case VarEnum.VT_DISPATCH:
			return typeof(DynamicVariantExtensions).GetMethod("SetAsByrefDispatch");
		case VarEnum.VT_VARIANT:
			return typeof(DynamicVariantExtensions).GetMethod("SetAsByrefVariant");
		case VarEnum.VT_RECORD:
		case VarEnum.VT_ARRAY:
			return typeof(DynamicVariantExtensions).GetMethod("SetAsByrefVariantIndirect");
		default:
			throw new NotSupportedException();
		}
	}

	public static void SetI1(this ref ComVariant variant, sbyte value)
	{
		variant = ComVariant.Create(value);
	}

	public static void SetUi1(this ref ComVariant variant, byte value)
	{
		variant = ComVariant.Create(value);
	}

	public static void SetI2(this ref ComVariant variant, short value)
	{
		variant = ComVariant.Create(value);
	}

	public static void SetUi2(this ref ComVariant variant, ushort value)
	{
		variant = ComVariant.Create(value);
	}

	public static void SetI4(this ref ComVariant variant, int value)
	{
		variant = ComVariant.Create(value);
	}

	public static void SetUi4(this ref ComVariant variant, uint value)
	{
		variant = ComVariant.Create(value);
	}

	public static void SetI8(this ref ComVariant variant, long value)
	{
		variant = ComVariant.Create(value);
	}

	public static void SetUi8(this ref ComVariant variant, ulong value)
	{
		variant = ComVariant.Create(value);
	}

	public static void SetInt(this ref ComVariant variant, int value)
	{
		variant = ComVariant.CreateRaw(VarEnum.VT_INT, value);
	}

	public static void SetUint(this ref ComVariant variant, uint value)
	{
		variant = ComVariant.CreateRaw(VarEnum.VT_UINT, value);
	}

	public static void SetBool(this ref ComVariant variant, bool value)
	{
		variant = ComVariant.Create(value);
	}

	public static void SetR4(this ref ComVariant variant, float value)
	{
		variant = ComVariant.Create(value);
	}

	public static void SetR8(this ref ComVariant variant, double value)
	{
		variant = ComVariant.Create(value);
	}

	public static void SetDecimal(this ref ComVariant variant, decimal value)
	{
		variant = ComVariant.Create(value);
	}

	public static void SetDate(this ref ComVariant variant, DateTime value)
	{
		variant = ComVariant.Create(value);
	}

	public static void SetBstr(this ref ComVariant variant, string value)
	{
		variant = ComVariant.Create(new BStrWrapper(value));
	}

	public static void SetUnknown(this ref ComVariant variant, object value)
	{
		variant = ComVariant.CreateRaw<nint>(VarEnum.VT_UNKNOWN, (value == null) ? IntPtr.Zero : Marshal.GetIUnknownForObject(value));
	}

	public static void SetDispatch(this ref ComVariant variant, object value)
	{
		variant = ComVariant.CreateRaw<nint>(VarEnum.VT_DISPATCH, (value == null) ? IntPtr.Zero : Marshal.GetIDispatchForObject(value));
	}

	public static void SetError(this ref ComVariant variant, int value)
	{
		variant = ComVariant.CreateRaw(VarEnum.VT_ERROR, value);
	}

	public static void SetCy(this ref ComVariant variant, decimal value)
	{
		variant = ComVariant.CreateRaw(VarEnum.VT_CY, decimal.ToOACurrency(value));
	}

	public static void SetVariant(this ref ComVariant variant, object value)
	{
		if (value != null)
		{
			UnsafeMethods.InitVariantForObject(value, ref variant);
		}
	}

	internal static MethodInfo GetSetter(VarEnum varType)
	{
		switch (varType)
		{
		case VarEnum.VT_I1:
			return typeof(DynamicVariantExtensions).GetMethod("SetI1", BindingFlags.Static | BindingFlags.Public);
		case VarEnum.VT_I2:
			return typeof(DynamicVariantExtensions).GetMethod("SetI2", BindingFlags.Static | BindingFlags.Public);
		case VarEnum.VT_I4:
			return typeof(DynamicVariantExtensions).GetMethod("SetI4", BindingFlags.Static | BindingFlags.Public);
		case VarEnum.VT_I8:
			return typeof(DynamicVariantExtensions).GetMethod("SetI8", BindingFlags.Static | BindingFlags.Public);
		case VarEnum.VT_UI1:
			return typeof(DynamicVariantExtensions).GetMethod("SetUi1", BindingFlags.Static | BindingFlags.Public);
		case VarEnum.VT_UI2:
			return typeof(DynamicVariantExtensions).GetMethod("SetUi2", BindingFlags.Static | BindingFlags.Public);
		case VarEnum.VT_UI4:
			return typeof(DynamicVariantExtensions).GetMethod("SetUi4", BindingFlags.Static | BindingFlags.Public);
		case VarEnum.VT_UI8:
			return typeof(DynamicVariantExtensions).GetMethod("SetUi8", BindingFlags.Static | BindingFlags.Public);
		case VarEnum.VT_INT:
			return typeof(DynamicVariantExtensions).GetMethod("SetInt", BindingFlags.Static | BindingFlags.Public);
		case VarEnum.VT_UINT:
			return typeof(DynamicVariantExtensions).GetMethod("SetUint", BindingFlags.Static | BindingFlags.Public);
		case VarEnum.VT_BOOL:
			return typeof(DynamicVariantExtensions).GetMethod("SetBool", BindingFlags.Static | BindingFlags.Public);
		case VarEnum.VT_ERROR:
			return typeof(DynamicVariantExtensions).GetMethod("SetError", BindingFlags.Static | BindingFlags.Public);
		case VarEnum.VT_R4:
			return typeof(DynamicVariantExtensions).GetMethod("SetR4", BindingFlags.Static | BindingFlags.Public);
		case VarEnum.VT_R8:
			return typeof(DynamicVariantExtensions).GetMethod("SetR8", BindingFlags.Static | BindingFlags.Public);
		case VarEnum.VT_DECIMAL:
			return typeof(DynamicVariantExtensions).GetMethod("SetDecimal", BindingFlags.Static | BindingFlags.Public);
		case VarEnum.VT_CY:
			return typeof(DynamicVariantExtensions).GetMethod("SetCy", BindingFlags.Static | BindingFlags.Public);
		case VarEnum.VT_DATE:
			return typeof(DynamicVariantExtensions).GetMethod("SetDate", BindingFlags.Static | BindingFlags.Public);
		case VarEnum.VT_BSTR:
			return typeof(DynamicVariantExtensions).GetMethod("SetBstr", BindingFlags.Static | BindingFlags.Public);
		case VarEnum.VT_UNKNOWN:
			return typeof(DynamicVariantExtensions).GetMethod("SetUnknown", BindingFlags.Static | BindingFlags.Public);
		case VarEnum.VT_DISPATCH:
			return typeof(DynamicVariantExtensions).GetMethod("SetDispatch", BindingFlags.Static | BindingFlags.Public);
		case VarEnum.VT_VARIANT:
		case VarEnum.VT_RECORD:
		case VarEnum.VT_ARRAY:
			return typeof(DynamicVariantExtensions).GetMethod("SetVariant", BindingFlags.Static | BindingFlags.Public);
		default:
			throw new NotSupportedException();
		}
	}
}
