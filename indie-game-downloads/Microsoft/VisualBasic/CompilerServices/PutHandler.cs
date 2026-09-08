using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class PutHandler : IRecordEnum
{
	public VB6File m_oFile;

	[RequiresUnreferencedCode("This implementation of IRecordEnum is unsafe. Marking ctor unsafe in order to suppress warnings for overridden methods as unsafe.")]
	public PutHandler(VB6File oFile)
	{
		m_oFile = oFile;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The constructor of this interface implementation has been anotated.")]
	public bool Callback(FieldInfo field_info, ref object vValue)
	{
		Type fieldType = field_info.FieldType;
		if ((object)fieldType == null)
		{
			throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_UnsupportedFieldType2, field_info.Name, "Empty")), 5);
		}
		if (fieldType.IsArray)
		{
			int fixedStringLength = -1;
			object[] customAttributes = field_info.GetCustomAttributes(typeof(VBFixedArrayAttribute), inherit: false);
			VBFixedArrayAttribute vBFixedArrayAttribute = ((customAttributes == null || customAttributes.Length == 0) ? null : ((VBFixedArrayAttribute)customAttributes[0]));
			Type elementType = fieldType.GetElementType();
			if ((object)elementType == typeof(string))
			{
				customAttributes = field_info.GetCustomAttributes(typeof(VBFixedStringAttribute), inherit: false);
				fixedStringLength = ((customAttributes != null && customAttributes.Length != 0) ? ((VBFixedStringAttribute)customAttributes[0]).Length : (-1));
			}
			if (vBFixedArrayAttribute == null)
			{
				m_oFile.PutDynamicArray(0L, (Array)vValue, ContainedInVariant: false, fixedStringLength);
			}
			else
			{
				m_oFile.PutFixedArray(0L, (Array)vValue, elementType, fixedStringLength, vBFixedArrayAttribute.FirstBound, vBFixedArrayAttribute.SecondBound);
			}
		}
		else
		{
			switch (Type.GetTypeCode(fieldType))
			{
			case TypeCode.String:
			{
				string s = ((vValue == null) ? null : vValue.ToString());
				object[] customAttributes2 = field_info.GetCustomAttributes(typeof(VBFixedStringAttribute), inherit: false);
				if (customAttributes2 == null || customAttributes2.Length == 0)
				{
					m_oFile.PutStringWithLength(0L, s);
					break;
				}
				int num = ((VBFixedStringAttribute)customAttributes2[0]).Length;
				if (num == 0)
				{
					num = -1;
				}
				m_oFile.PutFixedLengthString(0L, s, num);
				break;
			}
			case TypeCode.Single:
				m_oFile.PutSingle(0L, SingleType.FromObject(vValue));
				break;
			case TypeCode.Double:
				m_oFile.PutDouble(0L, DoubleType.FromObject(vValue));
				break;
			case TypeCode.Int16:
				m_oFile.PutShort(0L, ShortType.FromObject(vValue));
				break;
			case TypeCode.Int32:
				m_oFile.PutInteger(0L, IntegerType.FromObject(vValue));
				break;
			case TypeCode.Byte:
				m_oFile.PutByte(0L, ByteType.FromObject(vValue));
				break;
			case TypeCode.Int64:
				m_oFile.PutLong(0L, LongType.FromObject(vValue));
				break;
			case TypeCode.DateTime:
				m_oFile.PutDate(0L, DateType.FromObject(vValue));
				break;
			case TypeCode.Boolean:
				m_oFile.PutBoolean(0L, BooleanType.FromObject(vValue));
				break;
			case TypeCode.Decimal:
				m_oFile.PutDecimal(0L, DecimalType.FromObject(vValue));
				break;
			case TypeCode.Char:
				m_oFile.PutChar(0L, CharType.FromObject(vValue));
				break;
			case TypeCode.DBNull:
				throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_UnsupportedFieldType2, field_info.Name, "DBNull")), 5);
			default:
				if ((object)fieldType == typeof(object))
				{
					m_oFile.PutObject(vValue, 0L);
					break;
				}
				if ((object)fieldType == typeof(Exception))
				{
					throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_UnsupportedFieldType2, field_info.Name, "Exception")), 5);
				}
				if ((object)fieldType == typeof(Missing))
				{
					throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_UnsupportedFieldType2, field_info.Name, "Missing")), 5);
				}
				throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_UnsupportedFieldType2, field_info.Name, fieldType.Name)), 5);
			}
		}
		return false;
	}

	bool IRecordEnum.Callback(FieldInfo field_info, ref object vValue)
	{
		//ILSpy generated this explicit interface implementation from .override directive in Callback
		return this.Callback(field_info, ref vValue);
	}
}
