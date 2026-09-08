using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class StructUtils
{
	private sealed class StructByteLengthHandler : IRecordEnum
	{
		private int m_StructLength;

		private int m_PackSize;

		internal int Length
		{
			get
			{
				if (m_PackSize == 1)
				{
					return m_StructLength;
				}
				checked
				{
					return m_StructLength + unchecked(m_StructLength % m_PackSize);
				}
			}
		}

		internal StructByteLengthHandler(int PackSize)
		{
			m_PackSize = PackSize;
		}

		internal void SetAlignment(int size)
		{
			checked
			{
				if (m_PackSize != 1)
				{
					m_StructLength += unchecked(m_StructLength % size);
				}
			}
		}

		internal bool Callback(FieldInfo field_info, ref object vValue)
		{
			Type fieldType = field_info.FieldType;
			if ((object)fieldType == null)
			{
				throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_UnsupportedFieldType2, field_info.Name, "Empty")), 5);
			}
			checked
			{
				int align = default(int);
				if (fieldType.IsArray)
				{
					object[] customAttributes = field_info.GetCustomAttributes(typeof(VBFixedArrayAttribute), inherit: false);
					VBFixedArrayAttribute vBFixedArrayAttribute = ((customAttributes == null || customAttributes.Length == 0) ? null : ((VBFixedArrayAttribute)customAttributes[0]));
					Type elementType = fieldType.GetElementType();
					int num;
					int size = default(int);
					if (vBFixedArrayAttribute == null)
					{
						num = 1;
						size = 4;
					}
					else
					{
						num = vBFixedArrayAttribute.Length;
						GetFieldSize(field_info, elementType, ref align, ref size);
					}
					SetAlignment(align);
					m_StructLength += num * size;
					return false;
				}
				int size2 = default(int);
				GetFieldSize(field_info, fieldType, ref align, ref size2);
				SetAlignment(align);
				m_StructLength += size2;
				return false;
			}
		}

		bool IRecordEnum.Callback(FieldInfo field_info, ref object vValue)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Callback
			return this.Callback(field_info, ref vValue);
		}

		private void GetFieldSize(FieldInfo field_info, Type FieldType, ref int align, ref int size)
		{
			switch (Type.GetTypeCode(FieldType))
			{
			case TypeCode.String:
			{
				object[] customAttributes = field_info.GetCustomAttributes(typeof(VBFixedStringAttribute), inherit: false);
				if (customAttributes == null || customAttributes.Length == 0)
				{
					align = 4;
					size = 4;
					break;
				}
				int num = ((VBFixedStringAttribute)customAttributes[0]).Length;
				if (num == 0)
				{
					num = -1;
				}
				size = num;
				break;
			}
			case TypeCode.Single:
				align = 4;
				size = 4;
				break;
			case TypeCode.Double:
				align = 8;
				size = 8;
				break;
			case TypeCode.Int16:
				align = 2;
				size = 2;
				break;
			case TypeCode.Int32:
				align = 4;
				size = 4;
				break;
			case TypeCode.Byte:
				align = 1;
				size = 1;
				break;
			case TypeCode.Int64:
				align = 8;
				size = 8;
				break;
			case TypeCode.DateTime:
				align = 8;
				size = 8;
				break;
			case TypeCode.Boolean:
				align = 2;
				size = 2;
				break;
			case TypeCode.Decimal:
				align = 16;
				size = 16;
				break;
			case TypeCode.Char:
				align = 2;
				size = 2;
				break;
			case TypeCode.DBNull:
				throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_UnsupportedFieldType2, field_info.Name, "DBNull")), 5);
			}
			if ((object)FieldType == typeof(Exception))
			{
				throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_UnsupportedFieldType2, field_info.Name, "Exception")), 5);
			}
			if ((object)FieldType == typeof(Missing))
			{
				throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_UnsupportedFieldType2, field_info.Name, "Missing")), 5);
			}
			if ((object)FieldType == typeof(object))
			{
				throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_UnsupportedFieldType2, field_info.Name, "Object")), 5);
			}
		}
	}

	[RequiresUnreferencedCode("Calls Object.GetType which cannot be statically analyzed.")]
	internal static object EnumerateUDT(ValueType oStruct, IRecordEnum intfRecEnum, bool fGet)
	{
		Type type = oStruct.GetType();
		if (Information.VarTypeFromComType(type) != VariantType.UserDefinedType || type.IsPrimitive)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "oStruct"));
		}
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
		int upperBound = fields.GetUpperBound(0);
		int num = upperBound;
		for (int i = 0; i <= num; i = checked(i + 1))
		{
			FieldInfo fieldInfo = fields[i];
			Type fieldType = fieldInfo.FieldType;
			object Value = fieldInfo.GetValue(oStruct);
			if (Information.VarTypeFromComType(fieldType) == VariantType.UserDefinedType)
			{
				if (fieldType.IsPrimitive)
				{
					throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_UnsupportedFieldType2, fieldInfo.Name, fieldType.Name)), 5);
				}
				EnumerateUDT((ValueType)Value, intfRecEnum, fGet);
			}
			else
			{
				intfRecEnum.Callback(fieldInfo, ref Value);
			}
			if (fGet)
			{
				fieldInfo.SetValue(oStruct, Value);
			}
		}
		return null;
	}

	[RequiresUnreferencedCode("Calls EnumerateUDT which is unsafe.")]
	internal static int GetRecordLength(object o, int PackSize = -1)
	{
		if (o == null)
		{
			return 0;
		}
		IRecordEnum recordEnum2;
		IRecordEnum recordEnum = (recordEnum2 = new StructByteLengthHandler(PackSize));
		if (recordEnum2 == null)
		{
			throw ExceptionUtils.VbMakeException(5);
		}
		EnumerateUDT((ValueType)o, recordEnum2, fGet: false);
		return ((StructByteLengthHandler)recordEnum).Length;
	}
}
