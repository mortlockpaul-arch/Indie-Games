using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Reflection;

internal static class InvokeUtils
{
	public static object ConvertOrWiden(RuntimeType srcType, object srcObject, RuntimeType dstType, CorElementType dstElementType)
	{
		if (dstType.IsPointer || dstType.IsFunctionPointer)
		{
			if (TryConvertPointer(srcObject, out var dstPtr))
			{
				return dstPtr;
			}
			throw new NotSupportedException();
		}
		switch (dstElementType)
		{
		case CorElementType.ELEMENT_TYPE_BOOLEAN:
			return Convert.ToBoolean(srcObject);
		case CorElementType.ELEMENT_TYPE_CHAR:
		{
			char c = Convert.ToChar(srcObject);
			return dstType.IsEnum ? Enum.ToObject(dstType, c) : ((object)c);
		}
		case CorElementType.ELEMENT_TYPE_I1:
		{
			sbyte b = Convert.ToSByte(srcObject);
			return dstType.IsEnum ? Enum.ToObject(dstType, b) : ((object)b);
		}
		case CorElementType.ELEMENT_TYPE_I2:
		{
			short num5 = Convert.ToInt16(srcObject);
			return dstType.IsEnum ? Enum.ToObject(dstType, num5) : ((object)num5);
		}
		case CorElementType.ELEMENT_TYPE_I4:
		{
			int num = Convert.ToInt32(srcObject);
			return dstType.IsEnum ? Enum.ToObject(dstType, num) : ((object)num);
		}
		case CorElementType.ELEMENT_TYPE_I8:
		{
			long num4 = Convert.ToInt64(srcObject);
			return dstType.IsEnum ? Enum.ToObject(dstType, num4) : ((object)num4);
		}
		case CorElementType.ELEMENT_TYPE_U1:
		{
			byte b2 = Convert.ToByte(srcObject);
			return dstType.IsEnum ? Enum.ToObject(dstType, b2) : ((object)b2);
		}
		case CorElementType.ELEMENT_TYPE_U2:
		{
			ushort num2 = Convert.ToUInt16(srcObject);
			return dstType.IsEnum ? Enum.ToObject(dstType, num2) : ((object)num2);
		}
		case CorElementType.ELEMENT_TYPE_U4:
		{
			uint num6 = Convert.ToUInt32(srcObject);
			return dstType.IsEnum ? Enum.ToObject(dstType, num6) : ((object)num6);
		}
		case CorElementType.ELEMENT_TYPE_U8:
		{
			ulong num3 = Convert.ToUInt64(srcObject);
			return dstType.IsEnum ? Enum.ToObject(dstType, (long)num3) : ((object)num3);
		}
		case CorElementType.ELEMENT_TYPE_R4:
			if (srcType == typeof(char))
			{
				return (float)(int)(char)srcObject;
			}
			return Convert.ToSingle(srcObject);
		case CorElementType.ELEMENT_TYPE_R8:
			if (srcType == typeof(char))
			{
				return (double)(int)(char)srcObject;
			}
			return Convert.ToDouble(srcObject);
		default:
			throw new NotSupportedException();
		}
	}

	private static bool TryConvertPointer(object srcObject, [NotNullWhen(true)] out object dstPtr)
	{
		if ((srcObject is nint || srcObject is nuint) ? true : false)
		{
			dstPtr = srcObject;
			return true;
		}
		dstPtr = null;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void PrimitiveWiden(ref byte srcElement, ref byte destElement, CorElementType srcElType, CorElementType destElType)
	{
		switch (srcElType)
		{
		case CorElementType.ELEMENT_TYPE_U1:
			switch (destElType)
			{
			case CorElementType.ELEMENT_TYPE_CHAR:
			case CorElementType.ELEMENT_TYPE_I2:
			case CorElementType.ELEMENT_TYPE_U2:
				Unsafe.As<byte, ushort>(ref destElement) = srcElement;
				break;
			case CorElementType.ELEMENT_TYPE_I4:
			case CorElementType.ELEMENT_TYPE_U4:
				Unsafe.As<byte, uint>(ref destElement) = srcElement;
				break;
			case CorElementType.ELEMENT_TYPE_I8:
			case CorElementType.ELEMENT_TYPE_U8:
				Unsafe.As<byte, ulong>(ref destElement) = srcElement;
				break;
			case CorElementType.ELEMENT_TYPE_R4:
				Unsafe.As<byte, float>(ref destElement) = (int)srcElement;
				break;
			case CorElementType.ELEMENT_TYPE_R8:
				Unsafe.As<byte, double>(ref destElement) = (int)srcElement;
				break;
			case CorElementType.ELEMENT_TYPE_I1:
			case CorElementType.ELEMENT_TYPE_U1:
				break;
			}
			break;
		case CorElementType.ELEMENT_TYPE_I1:
			switch (destElType)
			{
			case CorElementType.ELEMENT_TYPE_I2:
				Unsafe.As<byte, short>(ref destElement) = Unsafe.As<byte, sbyte>(ref srcElement);
				break;
			case CorElementType.ELEMENT_TYPE_I4:
				Unsafe.As<byte, int>(ref destElement) = Unsafe.As<byte, sbyte>(ref srcElement);
				break;
			case CorElementType.ELEMENT_TYPE_I8:
				Unsafe.As<byte, long>(ref destElement) = Unsafe.As<byte, sbyte>(ref srcElement);
				break;
			case CorElementType.ELEMENT_TYPE_R4:
				Unsafe.As<byte, float>(ref destElement) = Unsafe.As<byte, sbyte>(ref srcElement);
				break;
			case CorElementType.ELEMENT_TYPE_R8:
				Unsafe.As<byte, double>(ref destElement) = Unsafe.As<byte, sbyte>(ref srcElement);
				break;
			case CorElementType.ELEMENT_TYPE_U2:
			case CorElementType.ELEMENT_TYPE_U4:
			case CorElementType.ELEMENT_TYPE_U8:
				break;
			}
			break;
		case CorElementType.ELEMENT_TYPE_CHAR:
		case CorElementType.ELEMENT_TYPE_U2:
			switch (destElType)
			{
			case CorElementType.ELEMENT_TYPE_CHAR:
			case CorElementType.ELEMENT_TYPE_U2:
				Unsafe.As<byte, ushort>(ref destElement) = Unsafe.As<byte, ushort>(ref srcElement);
				break;
			case CorElementType.ELEMENT_TYPE_I4:
			case CorElementType.ELEMENT_TYPE_U4:
				Unsafe.As<byte, uint>(ref destElement) = Unsafe.As<byte, ushort>(ref srcElement);
				break;
			case CorElementType.ELEMENT_TYPE_I8:
			case CorElementType.ELEMENT_TYPE_U8:
				Unsafe.As<byte, ulong>(ref destElement) = Unsafe.As<byte, ushort>(ref srcElement);
				break;
			case CorElementType.ELEMENT_TYPE_R4:
				Unsafe.As<byte, float>(ref destElement) = (int)Unsafe.As<byte, ushort>(ref srcElement);
				break;
			case CorElementType.ELEMENT_TYPE_R8:
				Unsafe.As<byte, double>(ref destElement) = (int)Unsafe.As<byte, ushort>(ref srcElement);
				break;
			case CorElementType.ELEMENT_TYPE_I1:
			case CorElementType.ELEMENT_TYPE_U1:
			case CorElementType.ELEMENT_TYPE_I2:
				break;
			}
			break;
		case CorElementType.ELEMENT_TYPE_I2:
			switch (destElType)
			{
			case CorElementType.ELEMENT_TYPE_I4:
				Unsafe.As<byte, int>(ref destElement) = Unsafe.As<byte, short>(ref srcElement);
				break;
			case CorElementType.ELEMENT_TYPE_I8:
				Unsafe.As<byte, long>(ref destElement) = Unsafe.As<byte, short>(ref srcElement);
				break;
			case CorElementType.ELEMENT_TYPE_R4:
				Unsafe.As<byte, float>(ref destElement) = Unsafe.As<byte, short>(ref srcElement);
				break;
			case CorElementType.ELEMENT_TYPE_R8:
				Unsafe.As<byte, double>(ref destElement) = Unsafe.As<byte, short>(ref srcElement);
				break;
			case CorElementType.ELEMENT_TYPE_U4:
			case CorElementType.ELEMENT_TYPE_U8:
				break;
			}
			break;
		case CorElementType.ELEMENT_TYPE_U4:
			switch (destElType)
			{
			case CorElementType.ELEMENT_TYPE_I8:
			case CorElementType.ELEMENT_TYPE_U8:
				Unsafe.As<byte, ulong>(ref destElement) = Unsafe.As<byte, uint>(ref srcElement);
				break;
			case CorElementType.ELEMENT_TYPE_R4:
				Unsafe.As<byte, float>(ref destElement) = Unsafe.As<byte, uint>(ref srcElement);
				break;
			case CorElementType.ELEMENT_TYPE_R8:
				Unsafe.As<byte, double>(ref destElement) = Unsafe.As<byte, uint>(ref srcElement);
				break;
			}
			break;
		case CorElementType.ELEMENT_TYPE_I4:
			switch (destElType)
			{
			case CorElementType.ELEMENT_TYPE_I8:
				Unsafe.As<byte, long>(ref destElement) = Unsafe.As<byte, int>(ref srcElement);
				break;
			case CorElementType.ELEMENT_TYPE_R4:
				Unsafe.As<byte, float>(ref destElement) = Unsafe.As<byte, int>(ref srcElement);
				break;
			case CorElementType.ELEMENT_TYPE_R8:
				Unsafe.As<byte, double>(ref destElement) = Unsafe.As<byte, int>(ref srcElement);
				break;
			case CorElementType.ELEMENT_TYPE_U8:
				break;
			}
			break;
		case CorElementType.ELEMENT_TYPE_U8:
			switch (destElType)
			{
			case CorElementType.ELEMENT_TYPE_R4:
				Unsafe.As<byte, float>(ref destElement) = Unsafe.As<byte, ulong>(ref srcElement);
				break;
			case CorElementType.ELEMENT_TYPE_R8:
				Unsafe.As<byte, double>(ref destElement) = Unsafe.As<byte, ulong>(ref srcElement);
				break;
			}
			break;
		case CorElementType.ELEMENT_TYPE_I8:
			switch (destElType)
			{
			case CorElementType.ELEMENT_TYPE_R4:
				Unsafe.As<byte, float>(ref destElement) = Unsafe.As<byte, long>(ref srcElement);
				break;
			case CorElementType.ELEMENT_TYPE_R8:
				Unsafe.As<byte, double>(ref destElement) = Unsafe.As<byte, long>(ref srcElement);
				break;
			}
			break;
		case CorElementType.ELEMENT_TYPE_R4:
			Unsafe.As<byte, double>(ref destElement) = Unsafe.As<byte, float>(ref srcElement);
			break;
		}
	}
}
