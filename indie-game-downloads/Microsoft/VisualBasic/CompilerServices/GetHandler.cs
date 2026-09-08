using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class GetHandler : IRecordEnum
{
	private VB6File m_oFile;

	[RequiresUnreferencedCode("This implementation of IRecordEnum is unsafe. Marking ctor unsafe in order to suppress warnings for overridden methods as unsafe.")]
	public GetHandler(VB6File oFile)
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
			object[] customAttributes = field_info.GetCustomAttributes(typeof(VBFixedArrayAttribute), inherit: false);
			Array arr = null;
			int fixedStringLength = -1;
			object[] customAttributes2 = field_info.GetCustomAttributes(typeof(VBFixedStringAttribute), inherit: false);
			if (customAttributes2 != null && customAttributes2.Length > 0)
			{
				VBFixedStringAttribute vBFixedStringAttribute = (VBFixedStringAttribute)customAttributes2[0];
				if (vBFixedStringAttribute.Length > 0)
				{
					fixedStringLength = vBFixedStringAttribute.Length;
				}
			}
			if (customAttributes == null || customAttributes.Length == 0)
			{
				m_oFile.GetDynamicArray(ref arr, fieldType.GetElementType(), fixedStringLength);
			}
			else
			{
				VBFixedArrayAttribute obj = (VBFixedArrayAttribute)customAttributes[0];
				int firstBound = obj.FirstBound;
				int secondBound = obj.SecondBound;
				arr = (Array)vValue;
				m_oFile.GetFixedArray(0L, ref arr, fieldType.GetElementType(), firstBound, secondBound, fixedStringLength);
			}
			vValue = arr;
		}
		else
		{
			switch (Type.GetTypeCode(fieldType))
			{
			case TypeCode.String:
			{
				object[] customAttributes3 = field_info.GetCustomAttributes(typeof(VBFixedStringAttribute), inherit: false);
				if (customAttributes3 == null || customAttributes3.Length == 0)
				{
					vValue = m_oFile.GetLengthPrefixedString(0L);
					break;
				}
				int num = ((VBFixedStringAttribute)customAttributes3[0]).Length;
				if (num == 0)
				{
					num = -1;
				}
				vValue = m_oFile.GetFixedLengthString(0L, num);
				break;
			}
			case TypeCode.Single:
				vValue = m_oFile.GetSingle(0L);
				break;
			case TypeCode.Double:
				vValue = m_oFile.GetDouble(0L);
				break;
			case TypeCode.Int16:
				vValue = m_oFile.GetShort(0L);
				break;
			case TypeCode.Int32:
				vValue = m_oFile.GetInteger(0L);
				break;
			case TypeCode.Byte:
				vValue = m_oFile.GetByte(0L);
				break;
			case TypeCode.Int64:
				vValue = m_oFile.GetLong(0L);
				break;
			case TypeCode.DateTime:
				vValue = m_oFile.GetDate(0L);
				break;
			case TypeCode.Boolean:
				vValue = m_oFile.GetBoolean(0L);
				break;
			case TypeCode.Decimal:
				vValue = m_oFile.GetDecimal(0L);
				break;
			case TypeCode.Char:
				vValue = m_oFile.GetChar(0L);
				break;
			case TypeCode.DBNull:
				throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_UnsupportedFieldType2, field_info.Name, "DBNull")), 5);
			default:
				if ((object)fieldType == typeof(object))
				{
					m_oFile.GetObject(ref vValue, 0L);
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
