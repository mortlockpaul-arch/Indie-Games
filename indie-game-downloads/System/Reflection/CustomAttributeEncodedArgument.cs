using System.Buffers.Binary;
using System.Text;

namespace System.Reflection;

internal sealed class CustomAttributeEncodedArgument
{
	private unsafe ref struct CustomAttributeDataParser(ConstArray attributeBlob)
	{
		private int _curr = 0;

		private unsafe ReadOnlySpan<byte> _blob = new ReadOnlySpan<byte>((void*)attributeBlob.Signature, attributeBlob.Length);

		private ReadOnlySpan<byte> PeekData(int size)
		{
			return _blob.Slice(_curr, size);
		}

		private ReadOnlySpan<byte> ReadData(int size)
		{
			ReadOnlySpan<byte> result = PeekData(size);
			_curr += size;
			return result;
		}

		public byte GetU1()
		{
			return ReadData(1)[0];
		}

		public sbyte GetI1()
		{
			return (sbyte)GetU1();
		}

		public ushort GetU2()
		{
			return BinaryPrimitives.ReadUInt16LittleEndian(ReadData(2));
		}

		public short GetI2()
		{
			return (short)GetU2();
		}

		public uint GetU4()
		{
			return BinaryPrimitives.ReadUInt32LittleEndian(ReadData(4));
		}

		public int GetI4()
		{
			return (int)GetU4();
		}

		public ulong GetU8()
		{
			return BinaryPrimitives.ReadUInt64LittleEndian(ReadData(8));
		}

		public long GetI8()
		{
			return (long)GetU8();
		}

		public float GetR4()
		{
			return BinaryPrimitives.ReadSingleLittleEndian(ReadData(4));
		}

		public CustomAttributeEncoding GetTag()
		{
			return (CustomAttributeEncoding)GetI1();
		}

		public double GetR8()
		{
			return BinaryPrimitives.ReadDoubleLittleEndian(ReadData(8));
		}

		public ushort GetProlog()
		{
			return GetU2();
		}

		public bool ValidateProlog()
		{
			return GetProlog() == 1;
		}

		public string GetString()
		{
			byte b = PeekData(1)[0];
			if (b == byte.MaxValue)
			{
				ReadData(1);
				return null;
			}
			int packedLength = GetPackedLength(b);
			if (packedLength == 0)
			{
				return string.Empty;
			}
			ReadOnlySpan<byte> bytes = ReadData(packedLength);
			return Encoding.UTF8.GetString(bytes);
		}

		private int GetPackedLength(byte firstByte)
		{
			if ((firstByte & 0x80) == 0)
			{
				ReadData(1);
				return firstByte & 0x7F;
			}
			if ((firstByte & 0xC0) == 128)
			{
				ReadOnlySpan<byte> readOnlySpan = ReadData(2);
				return ((readOnlySpan[0] & 0x3F) << 8) + readOnlySpan[1];
			}
			if ((firstByte & 0xE0) == 192)
			{
				ReadOnlySpan<byte> readOnlySpan = ReadData(4);
				return ((readOnlySpan[0] & 0x1F) << 24) + (readOnlySpan[1] << 16) + (readOnlySpan[2] << 8) + readOnlySpan[3];
			}
			throw new OverflowException();
		}
	}

	public CustomAttributeType CustomAttributeType { get; }

	public PrimitiveValue PrimitiveValue { get; set; }

	public CustomAttributeEncodedArgument[] ArrayValue { get; set; }

	public string StringValue { get; set; }

	internal static void ParseAttributeArguments(ConstArray attributeBlob, CustomAttributeCtorParameter[] customAttributeCtorParameters, CustomAttributeNamedParameter[] customAttributeNamedParameters, RuntimeModule customAttributeModule)
	{
		ArgumentNullException.ThrowIfNull(customAttributeModule, "customAttributeModule");
		if (customAttributeCtorParameters.Length == 0 && customAttributeNamedParameters.Length == 0)
		{
			return;
		}
		CustomAttributeDataParser parser = new CustomAttributeDataParser(attributeBlob);
		try
		{
			if (!parser.ValidateProlog())
			{
				throw new BadImageFormatException(SR.Arg_CustomAttributeFormatException);
			}
			ParseCtorArgs(ref parser, customAttributeCtorParameters, customAttributeModule);
			ParseNamedArgs(ref parser, customAttributeNamedParameters, customAttributeModule);
		}
		catch (Exception ex) when (!(ex is OutOfMemoryException))
		{
			throw new CustomAttributeFormatException(ex.Message, ex);
		}
	}

	internal CustomAttributeEncodedArgument(CustomAttributeType type)
	{
		CustomAttributeType = type;
	}

	private static void ParseCtorArgs(ref CustomAttributeDataParser parser, CustomAttributeCtorParameter[] customAttributeCtorParameters, RuntimeModule module)
	{
		foreach (CustomAttributeCtorParameter customAttributeCtorParameter in customAttributeCtorParameters)
		{
			customAttributeCtorParameter.EncodedArgument = ParseCustomAttributeValue(ref parser, customAttributeCtorParameter.CustomAttributeType, module);
		}
	}

	private static void ParseNamedArgs(ref CustomAttributeDataParser parser, CustomAttributeNamedParameter[] customAttributeNamedParameters, RuntimeModule module)
	{
		int i = parser.GetI2();
		for (int j = 0; j < i; j++)
		{
			CustomAttributeEncoding tag = parser.GetTag();
			if (tag != CustomAttributeEncoding.Field && tag != CustomAttributeEncoding.Property)
			{
				throw new BadImageFormatException(SR.Arg_CustomAttributeFormatException);
			}
			CustomAttributeType customAttributeType = ParseCustomAttributeType(ref parser, module);
			string value = parser.GetString();
			if (string.IsNullOrEmpty(value))
			{
				throw new BadImageFormatException(SR.Arg_CustomAttributeFormatException);
			}
			CustomAttributeNamedParameter customAttributeNamedParameter = null;
			foreach (CustomAttributeNamedParameter customAttributeNamedParameter2 in customAttributeNamedParameters)
			{
				CustomAttributeType customAttributeType2 = customAttributeNamedParameter2.CustomAttributeType;
				if ((customAttributeType2.EncodedType == CustomAttributeEncoding.Object || (customAttributeType2.EncodedType == customAttributeType.EncodedType && (customAttributeType.EncodedType != CustomAttributeEncoding.Array || customAttributeType2.EncodedArrayType == CustomAttributeEncoding.Object || customAttributeType.EncodedArrayType == customAttributeType2.EncodedArrayType))) && customAttributeNamedParameter2.MemberInfo.Name.Equals(value) && ((customAttributeType2.EncodedType != CustomAttributeEncoding.Enum && (customAttributeType2.EncodedType != CustomAttributeEncoding.Array || customAttributeType2.EncodedArrayType != CustomAttributeEncoding.Enum)) || (object)customAttributeType.EnumType == customAttributeType2.EnumType))
				{
					customAttributeNamedParameter = customAttributeNamedParameter2;
					break;
				}
			}
			if (customAttributeNamedParameter == null)
			{
				throw new BadImageFormatException(SR.Arg_CustomAttributeUnknownNamedArgument);
			}
			if (customAttributeNamedParameter.EncodedArgument != null)
			{
				throw new BadImageFormatException(SR.Arg_CustomAttributeDuplicateNamedArgument);
			}
			customAttributeNamedParameter.EncodedArgument = ParseCustomAttributeValue(ref parser, customAttributeType, module);
		}
	}

	private static CustomAttributeEncodedArgument ParseCustomAttributeValue(ref CustomAttributeDataParser parser, CustomAttributeType type, RuntimeModule module)
	{
		CustomAttributeType customAttributeType = ((type.EncodedType == CustomAttributeEncoding.Object) ? ParseCustomAttributeType(ref parser, module) : type);
		CustomAttributeEncodedArgument customAttributeEncodedArgument = new CustomAttributeEncodedArgument(customAttributeType);
		switch ((customAttributeType.EncodedType == CustomAttributeEncoding.Enum) ? customAttributeType.EncodedEnumType : customAttributeType.EncodedType)
		{
		case CustomAttributeEncoding.Boolean:
		case CustomAttributeEncoding.SByte:
		case CustomAttributeEncoding.Byte:
			customAttributeEncodedArgument.PrimitiveValue = new PrimitiveValue
			{
				Byte4 = parser.GetU1()
			};
			break;
		case CustomAttributeEncoding.Char:
		case CustomAttributeEncoding.Int16:
		case CustomAttributeEncoding.UInt16:
			customAttributeEncodedArgument.PrimitiveValue = new PrimitiveValue
			{
				Byte4 = parser.GetU2()
			};
			break;
		case CustomAttributeEncoding.Int32:
		case CustomAttributeEncoding.UInt32:
			customAttributeEncodedArgument.PrimitiveValue = new PrimitiveValue
			{
				Byte4 = parser.GetI4()
			};
			break;
		case CustomAttributeEncoding.Int64:
		case CustomAttributeEncoding.UInt64:
			customAttributeEncodedArgument.PrimitiveValue = new PrimitiveValue
			{
				Byte8 = parser.GetI8()
			};
			break;
		case CustomAttributeEncoding.Float:
			customAttributeEncodedArgument.PrimitiveValue = new PrimitiveValue
			{
				Byte4 = BitConverter.SingleToInt32Bits(parser.GetR4())
			};
			break;
		case CustomAttributeEncoding.Double:
			customAttributeEncodedArgument.PrimitiveValue = new PrimitiveValue
			{
				Byte8 = BitConverter.DoubleToInt64Bits(parser.GetR8())
			};
			break;
		case CustomAttributeEncoding.String:
		case CustomAttributeEncoding.Type:
			customAttributeEncodedArgument.StringValue = parser.GetString();
			break;
		case CustomAttributeEncoding.Array:
		{
			customAttributeEncodedArgument.ArrayValue = null;
			int i = parser.GetI4();
			if (i != -1)
			{
				customAttributeType = new CustomAttributeType(customAttributeType.EncodedArrayType, CustomAttributeEncoding.Undefined, customAttributeType.EncodedEnumType, customAttributeType.EnumType);
				customAttributeEncodedArgument.ArrayValue = new CustomAttributeEncodedArgument[i];
				for (int j = 0; j < i; j++)
				{
					customAttributeEncodedArgument.ArrayValue[j] = ParseCustomAttributeValue(ref parser, customAttributeType, module);
				}
			}
			break;
		}
		default:
			throw new BadImageFormatException();
		}
		return customAttributeEncodedArgument;
	}

	private static CustomAttributeType ParseCustomAttributeType(ref CustomAttributeDataParser parser, RuntimeModule module)
	{
		CustomAttributeEncoding customAttributeEncoding = CustomAttributeEncoding.Undefined;
		CustomAttributeEncoding encodedEnumType = CustomAttributeEncoding.Undefined;
		Type type = null;
		CustomAttributeEncoding tag = parser.GetTag();
		if (tag == CustomAttributeEncoding.Array)
		{
			customAttributeEncoding = parser.GetTag();
		}
		if (tag == CustomAttributeEncoding.Enum || (tag == CustomAttributeEncoding.Array && customAttributeEncoding == CustomAttributeEncoding.Enum))
		{
			type = TypeNameResolver.GetTypeReferencedByCustomAttribute(parser.GetString() ?? throw new BadImageFormatException(), module);
			if (!type.IsEnum)
			{
				throw new BadImageFormatException();
			}
			encodedEnumType = RuntimeCustomAttributeData.TypeToCustomAttributeEncoding((RuntimeType)type.GetEnumUnderlyingType());
		}
		return new CustomAttributeType(tag, customAttributeEncoding, encodedEnumType, type);
	}
}
