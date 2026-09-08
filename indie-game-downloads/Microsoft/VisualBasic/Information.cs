using System;
using System.Reflection;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.VisualBasic;

[StandardModule]
public sealed class Information
{
	private static readonly int[] QBColorTable = new int[16]
	{
		0, 8388608, 32768, 8421376, 128, 8388736, 32896, 12632256, 8421504, 16711680,
		65280, 16776960, 255, 16711935, 65535, 16777215
	};

	public static ErrObject Err()
	{
		ProjectData projectData = ProjectData.GetProjectData();
		if (projectData.m_Err == null)
		{
			projectData.m_Err = new ErrObject();
		}
		return projectData.m_Err;
	}

	public static int Erl()
	{
		return ProjectData.GetProjectData().m_Err.Erl;
	}

	public static bool IsArray(object VarName)
	{
		if (VarName == null)
		{
			return false;
		}
		return VarName is Array;
	}

	public static bool IsDate(object Expression)
	{
		if (Expression == null)
		{
			return false;
		}
		if (Expression is DateTime)
		{
			return true;
		}
		if (Expression is string value)
		{
			DateTime Result = default(DateTime);
			return Conversions.TryParseDate(value, ref Result);
		}
		return false;
	}

	public static bool IsDBNull(object Expression)
	{
		if (Expression == null)
		{
			return false;
		}
		if (Expression is DBNull)
		{
			return true;
		}
		return false;
	}

	public static bool IsNothing(object Expression)
	{
		return Expression == null;
	}

	public static bool IsError(object Expression)
	{
		if (Expression == null)
		{
			return false;
		}
		return Expression is Exception;
	}

	public static bool IsReference(object Expression)
	{
		return !(Expression is ValueType);
	}

	public static int LBound(Array Array, int Rank = 1)
	{
		if (Array == null)
		{
			throw ExceptionUtils.VbMakeException(new ArgumentNullException("Array"), 9);
		}
		if (Rank < 1 || Rank > Array.Rank)
		{
			throw new RankException(System.SR.Format(System.SR.Argument_InvalidRank1, "Rank"));
		}
		return Array.GetLowerBound(checked(Rank - 1));
	}

	public static int UBound(Array Array, int Rank = 1)
	{
		if (Array == null)
		{
			throw ExceptionUtils.VbMakeException(new ArgumentNullException("Array"), 9);
		}
		if (Rank < 1 || Rank > Array.Rank)
		{
			throw new RankException(System.SR.Format(System.SR.Argument_InvalidRank1, "Rank"));
		}
		return Array.GetUpperBound(checked(Rank - 1));
	}

	internal static string TypeNameOfCOMObject(object VarName, bool bThrowException)
	{
		string text = "__ComObject";
		UnsafeNativeMethods.ITypeInfo pTypeInfo = null;
		string pBstrName = null;
		string pBstrDocString = null;
		string pBstrHelpFile = null;
		int pdwHelpContext;
		if (VarName is UnsafeNativeMethods.IProvideClassInfo provideClassInfo)
		{
			try
			{
				pTypeInfo = provideClassInfo.GetClassInfo();
				if (pTypeInfo.GetDocumentation(-1, out pBstrName, out pBstrDocString, out pdwHelpContext, out pBstrHelpFile) >= 0)
				{
					text = pBstrName;
					goto IL_007c;
				}
				pTypeInfo = null;
			}
			catch (StackOverflowException ex)
			{
				throw ex;
			}
			catch (OutOfMemoryException ex2)
			{
				throw ex2;
			}
			catch (Exception)
			{
			}
		}
		if (VarName is UnsafeNativeMethods.IDispatch dispatch && dispatch.GetTypeInfo(0, 1033, out pTypeInfo) >= 0 && pTypeInfo.GetDocumentation(-1, out pBstrName, out pBstrDocString, out pdwHelpContext, out pBstrHelpFile) >= 0)
		{
			text = pBstrName;
		}
		goto IL_007c;
		IL_007c:
		if (text[0] == '_')
		{
			text = text.Substring(1);
		}
		return text;
	}

	public static int QBColor(int Color)
	{
		if ((Color & 0xFFF0) != 0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Color"), "Color");
		}
		return QBColorTable[Color];
	}

	public static int RGB(int Red, int Green, int Blue)
	{
		if ((Red & int.MinValue) != 0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Red"), "Red");
		}
		if ((Green & int.MinValue) != 0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Green"), "Green");
		}
		if ((Blue & int.MinValue) != 0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Blue"), "Blue");
		}
		if (Red > 255)
		{
			Red = 255;
		}
		if (Green > 255)
		{
			Green = 255;
		}
		if (Blue > 255)
		{
			Blue = 255;
		}
		return checked(Blue * 65536 + Green * 256 + Red);
	}

	public static VariantType VarType(object VarName)
	{
		if (VarName == null)
		{
			return VariantType.Object;
		}
		return VarTypeFromComType(VarName.GetType());
	}

	internal static VariantType VarTypeFromComType(Type typ)
	{
		if ((object)typ == null)
		{
			return VariantType.Object;
		}
		if (typ.IsArray)
		{
			typ = typ.GetElementType();
			if (typ.IsArray)
			{
				return (VariantType)8201;
			}
			VariantType variantType = VarTypeFromComType(typ);
			if ((variantType & VariantType.Array) != VariantType.Empty)
			{
				return (VariantType)8201;
			}
			return variantType | VariantType.Array;
		}
		if (typ.IsEnum)
		{
			typ = Enum.GetUnderlyingType(typ);
		}
		if ((object)typ == null)
		{
			return VariantType.Empty;
		}
		switch (Type.GetTypeCode(typ))
		{
		case TypeCode.String:
			return VariantType.String;
		case TypeCode.Int32:
			return VariantType.Integer;
		case TypeCode.Int16:
			return VariantType.Short;
		case TypeCode.Int64:
			return VariantType.Long;
		case TypeCode.Single:
			return VariantType.Single;
		case TypeCode.Double:
			return VariantType.Double;
		case TypeCode.DateTime:
			return VariantType.Date;
		case TypeCode.Boolean:
			return VariantType.Boolean;
		case TypeCode.Decimal:
			return VariantType.Decimal;
		case TypeCode.Byte:
			return VariantType.Byte;
		case TypeCode.Char:
			return VariantType.Char;
		case TypeCode.DBNull:
			return VariantType.Null;
		default:
			if ((object)typ == typeof(Missing) || (object)typ == typeof(Exception) || typ.IsSubclassOf(typeof(Exception)))
			{
				return VariantType.Error;
			}
			if (typ.IsValueType)
			{
				return VariantType.UserDefinedType;
			}
			return VariantType.Object;
		}
	}

	internal static bool IsOldNumericTypeCode(TypeCode TypCode)
	{
		switch (TypCode)
		{
		case TypeCode.Boolean:
		case TypeCode.Byte:
		case TypeCode.Int16:
		case TypeCode.Int32:
		case TypeCode.Int64:
		case TypeCode.Single:
		case TypeCode.Double:
		case TypeCode.Decimal:
			return true;
		default:
			return false;
		}
	}

	public static bool IsNumeric(object Expression)
	{
		IConvertible convertible = Expression as IConvertible;
		if (convertible == null)
		{
			if (!(Expression is char[] value))
			{
				return false;
			}
			Expression = new string(value);
		}
		TypeCode typeCode = convertible.GetTypeCode();
		if (typeCode == TypeCode.String || typeCode == TypeCode.Char)
		{
			string value2 = convertible.ToString(null);
			try
			{
				long i64Value = default(long);
				if (Utils.IsHexOrOctValue(value2, ref i64Value))
				{
					return true;
				}
			}
			catch (StackOverflowException ex)
			{
				throw ex;
			}
			catch (OutOfMemoryException ex2)
			{
				throw ex2;
			}
			catch (Exception)
			{
				return false;
			}
			double Result = default(double);
			return DoubleType.TryParse(value2, ref Result);
		}
		return IsOldNumericTypeCode(typeCode);
	}

	internal static string OldVBFriendlyNameOfTypeName(string typename)
	{
		string text = null;
		checked
		{
			int num = typename.Length - 1;
			if (typename[num] == ']')
			{
				int num2 = typename.IndexOf('[');
				text = ((num2 + 1 != num) ? typename.Substring(num2, num - num2 + 1).Replace('[', '(').Replace(']', ')') : "()");
				typename = typename.Substring(0, num2);
			}
			string text2 = OldVbTypeName(typename);
			if (text2 == null)
			{
				text2 = typename;
			}
			if (text == null)
			{
				return text2;
			}
			return text2 + Utils.AdjustArraySuffix(text);
		}
	}

	public static string TypeName(object VarName)
	{
		if (VarName == null)
		{
			return "Nothing";
		}
		Type type = VarName.GetType();
		bool flag = default(bool);
		if (type.IsArray)
		{
			flag = true;
			type = type.GetElementType();
		}
		string text;
		if (type.IsEnum)
		{
			text = type.Name;
			goto IL_011f;
		}
		switch (Type.GetTypeCode(type))
		{
		case TypeCode.DBNull:
			break;
		case TypeCode.Int16:
			goto IL_009c;
		case TypeCode.Int32:
			goto IL_00a7;
		case TypeCode.Single:
			goto IL_00b2;
		case TypeCode.Double:
			goto IL_00ba;
		case TypeCode.DateTime:
			goto IL_00c2;
		case TypeCode.String:
			goto IL_00ca;
		case TypeCode.Boolean:
			goto IL_00d2;
		case TypeCode.Decimal:
			goto IL_00da;
		case TypeCode.Byte:
			goto IL_00e2;
		case TypeCode.Char:
			goto IL_00ea;
		case TypeCode.Int64:
			goto IL_00f2;
		default:
			goto IL_00fa;
		}
		text = "DBNull";
		goto IL_0139;
		IL_00ca:
		text = "String";
		goto IL_0139;
		IL_00c2:
		text = "Date";
		goto IL_0139;
		IL_00fa:
		text = type.Name;
		if (type.IsCOMObject && string.Equals(text, "__ComObject", StringComparison.Ordinal))
		{
			text = LegacyTypeNameOfCOMObject(VarName, bThrowException: true);
		}
		goto IL_011f;
		IL_0139:
		if (flag)
		{
			Array array = (Array)VarName;
			text = ((array.Rank != 1) ? (text + "[" + new string(',', checked(array.Rank - 1)) + "]") : (text + "[]"));
			text = OldVBFriendlyNameOfTypeName(text);
		}
		return text;
		IL_011f:
		int num = text.IndexOf('+');
		if (num >= 0)
		{
			text = text.Substring(checked(num + 1));
		}
		goto IL_0139;
		IL_00d2:
		text = "Boolean";
		goto IL_0139;
		IL_00ea:
		text = "Char";
		goto IL_0139;
		IL_00e2:
		text = "Byte";
		goto IL_0139;
		IL_009c:
		text = "Short";
		goto IL_0139;
		IL_00a7:
		text = "Integer";
		goto IL_0139;
		IL_00f2:
		text = "Long";
		goto IL_0139;
		IL_00b2:
		text = "Single";
		goto IL_0139;
		IL_00ba:
		text = "Double";
		goto IL_0139;
		IL_00da:
		text = "Decimal";
		goto IL_0139;
	}

	public static string SystemTypeName(string VbName)
	{
		return Strings.Trim(VbName).ToUpperInvariant() switch
		{
			"OBJECT" => "System.Object", 
			"SHORT" => "System.Int16", 
			"INTEGER" => "System.Int32", 
			"SINGLE" => "System.Single", 
			"DOUBLE" => "System.Double", 
			"DATE" => "System.DateTime", 
			"STRING" => "System.String", 
			"BOOLEAN" => "System.Boolean", 
			"DECIMAL" => "System.Decimal", 
			"BYTE" => "System.Byte", 
			"CHAR" => "System.Char", 
			"LONG" => "System.Int64", 
			_ => null, 
		};
	}

	public static string VbTypeName(string UrtName)
	{
		return OldVbTypeName(UrtName);
	}

	internal static string OldVbTypeName(string UrtName)
	{
		UrtName = Strings.Trim(UrtName).ToUpperInvariant();
		if (Operators.CompareString(Strings.Left(UrtName, 7), "SYSTEM.", TextCompare: false) == 0)
		{
			UrtName = Strings.Mid(UrtName, 8);
		}
		return UrtName switch
		{
			"OBJECT" => "Object", 
			"INT16" => "Short", 
			"INT32" => "Integer", 
			"SINGLE" => "Single", 
			"DOUBLE" => "Double", 
			"DATETIME" => "Date", 
			"STRING" => "String", 
			"BOOLEAN" => "Boolean", 
			"DECIMAL" => "Decimal", 
			"BYTE" => "Byte", 
			"CHAR" => "Char", 
			"INT64" => "Long", 
			_ => null, 
		};
	}

	internal static string LegacyTypeNameOfCOMObject(object VarName, bool bThrowException)
	{
		string text = "__ComObject";
		UnsafeNativeMethods.ITypeInfo pTypeInfo = null;
		string pBstrName = null;
		string pBstrDocString = null;
		string pBstrHelpFile = null;
		if (VarName is UnsafeNativeMethods.IDispatch dispatch && dispatch.GetTypeInfo(0, 1033, out pTypeInfo) >= 0 && pTypeInfo.GetDocumentation(-1, out pBstrName, out pBstrDocString, out var _, out pBstrHelpFile) >= 0)
		{
			text = pBstrName;
		}
		if (text[0] == '_')
		{
			text = text.Substring(1);
		}
		return text;
	}
}
