using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Versioning;

namespace System.Runtime.InteropServices;

[SupportedOSPlatform("windows")]
internal static class BuiltInInteropVariantExtensions
{
	private unsafe static ref T GetByRefDataRef<T>(this ref ComVariant variant) where T : unmanaged
	{
		return ref Unsafe.AsRef<T>((void*)variant.GetRawDataRef<nint>());
	}

	public unsafe static void CopyFromIndirect(this ref ComVariant variant, object value)
	{
		VarEnum varEnum = variant.VarType & (VarEnum)(-16385);
		if (value == null)
		{
			if (varEnum == VarEnum.VT_DISPATCH || varEnum == VarEnum.VT_UNKNOWN || varEnum == VarEnum.VT_BSTR)
			{
				variant.GetRawDataRef<nint>() = IntPtr.Zero;
			}
			return;
		}
		if ((varEnum & VarEnum.VT_ARRAY) != VarEnum.VT_EMPTY)
		{
			Unsafe.SkipInit(out ComVariant comVariant);
			Marshal.GetNativeVariantForObject(value, (nint)(&comVariant));
			variant.GetRawDataRef<nint>() = comVariant.GetRawDataRef<nint>();
			return;
		}
		switch (varEnum)
		{
		case VarEnum.VT_I1:
			variant.GetByRefDataRef<sbyte>() = (sbyte)value;
			break;
		case VarEnum.VT_UI1:
			variant.GetByRefDataRef<byte>() = (byte)value;
			break;
		case VarEnum.VT_I2:
			variant.GetByRefDataRef<short>() = (short)value;
			break;
		case VarEnum.VT_UI2:
			variant.GetByRefDataRef<ushort>() = (ushort)value;
			break;
		case VarEnum.VT_BOOL:
			variant.GetByRefDataRef<short>() = (short)(((bool)value) ? (-1) : 0);
			break;
		case VarEnum.VT_I4:
		case VarEnum.VT_INT:
			variant.GetByRefDataRef<int>() = (int)value;
			break;
		case VarEnum.VT_UI4:
		case VarEnum.VT_UINT:
			variant.GetByRefDataRef<uint>() = (uint)value;
			break;
		case VarEnum.VT_ERROR:
			variant.GetByRefDataRef<int>() = ((ErrorWrapper)value).ErrorCode;
			break;
		case VarEnum.VT_I8:
			variant.GetByRefDataRef<long>() = (long)value;
			break;
		case VarEnum.VT_UI8:
			variant.GetByRefDataRef<ulong>() = (ulong)value;
			break;
		case VarEnum.VT_R4:
			variant.GetByRefDataRef<float>() = (float)value;
			break;
		case VarEnum.VT_R8:
			variant.GetByRefDataRef<double>() = (double)value;
			break;
		case VarEnum.VT_DATE:
			variant.GetByRefDataRef<double>() = ((DateTime)value).ToOADate();
			break;
		case VarEnum.VT_UNKNOWN:
			variant.GetByRefDataRef<nint>() = Marshal.GetIUnknownForObject(value);
			break;
		case VarEnum.VT_DISPATCH:
			variant.GetByRefDataRef<nint>() = Marshal.GetIDispatchForObject(value);
			break;
		case VarEnum.VT_BSTR:
			variant.GetByRefDataRef<nint>() = Marshal.StringToBSTR((string)value);
			break;
		case VarEnum.VT_CY:
			variant.GetByRefDataRef<long>() = decimal.ToOACurrency((decimal)value);
			break;
		case VarEnum.VT_DECIMAL:
			variant.GetByRefDataRef<decimal>() = (decimal)value;
			break;
		case VarEnum.VT_VARIANT:
			Marshal.GetNativeVariantForObject(value, variant.GetRawDataRef<nint>());
			break;
		default:
			throw new ArgumentException();
		}
	}

	public static object ToObject(this ref ComVariant variant)
	{
		switch (variant.VarType)
		{
		case VarEnum.VT_EMPTY:
			return null;
		case VarEnum.VT_NULL:
			return DBNull.Value;
		case VarEnum.VT_I1:
			return variant.As<sbyte>();
		case VarEnum.VT_I2:
			return variant.As<short>();
		case VarEnum.VT_I4:
			return variant.As<int>();
		case VarEnum.VT_I8:
			return variant.As<long>();
		case VarEnum.VT_UI1:
			return variant.As<byte>();
		case VarEnum.VT_UI2:
			return variant.As<ushort>();
		case VarEnum.VT_UI4:
			return variant.As<uint>();
		case VarEnum.VT_UI8:
			return variant.As<ulong>();
		case VarEnum.VT_INT:
			return variant.As<int>();
		case VarEnum.VT_UINT:
			return variant.As<uint>();
		case VarEnum.VT_BOOL:
			return variant.As<bool>();
		case VarEnum.VT_ERROR:
			return variant.As<int>();
		case VarEnum.VT_R4:
			return variant.As<float>();
		case VarEnum.VT_R8:
			return variant.As<double>();
		case VarEnum.VT_DECIMAL:
			return variant.As<decimal>();
		case VarEnum.VT_CY:
			return decimal.FromOACurrency(variant.GetRawDataRef<long>());
		case VarEnum.VT_DATE:
			return variant.As<DateTime>();
		case VarEnum.VT_BSTR:
			return (variant.GetRawDataRef<nint>() == 0) ? null : Marshal.PtrToStringBSTR(variant.GetRawDataRef<nint>());
		case VarEnum.VT_DISPATCH:
		case VarEnum.VT_UNKNOWN:
			return (variant.GetRawDataRef<nint>() == 0) ? null : Marshal.GetObjectForIUnknown(variant.GetRawDataRef<nint>());
		default:
			return GetObjectFromNativeVariant(ref variant);
		}
	}

	private unsafe static object GetObjectFromNativeVariant(ref ComVariant variant)
	{
		fixed (ComVariant* ptr = &variant)
		{
			void* pSrcNativeVariant = ptr;
			return Marshal.GetObjectForNativeVariant((nint)pSrcNativeVariant);
		}
	}
}
