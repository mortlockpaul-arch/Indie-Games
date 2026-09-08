using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;

namespace Microsoft.VisualBasic.CompilerServices;

[DebuggerNonUserCode]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class Utils
{
	private enum PropertyKind
	{
		ReadWrite,
		ReadOnly,
		WriteOnly
	}

	internal static char[] m_achIntlSpace = new char[2] { ' ', '\u3000' };

	private static readonly Type s_voidType = Type.GetType("System.Void");

	private static Assembly s_VBRuntimeAssembly;

	internal static Assembly VBRuntimeAssembly
	{
		get
		{
			if ((object)s_VBRuntimeAssembly != null)
			{
				return s_VBRuntimeAssembly;
			}
			s_VBRuntimeAssembly = typeof(Utils).Assembly;
			return s_VBRuntimeAssembly;
		}
	}

	private static string GetFallbackMessage(string name, params object[] args)
	{
		return name;
	}

	internal static string GetResourceString(vbErrors ResourceId)
	{
		string text = "ID" + Conversions.ToString((int)ResourceId);
		return System.SR.GetResourceString(text, text);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static string GetResourceString(string ResourceKey)
	{
		string text = null;
		try
		{
			text = System.SR.GetResourceString(ResourceKey);
			if (text == null)
			{
				text = System.SR.GetResourceString("ID95");
			}
			if (text == null)
			{
				text = GetFallbackMessage(ResourceKey);
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
			text = "Message text unavailable.  Resource file 'Microsoft.VisualBasic resources' not found.";
		}
		return text;
	}

	public static string GetResourceString(string ResourceKey, params string[] Args)
	{
		string text = null;
		string text2 = null;
		try
		{
			text = GetResourceString(ResourceKey);
			text2 = string.Format((IFormatProvider?)Thread.CurrentThread.CurrentCulture, text, (object?[])Args);
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
		if (Operators.CompareString(text2, "", TextCompare: false) != 0)
		{
			return text2;
		}
		return text;
	}

	internal static string StdFormat(string s)
	{
		NumberFormatInfo numberFormat = Thread.CurrentThread.CurrentCulture.NumberFormat;
		int num = s.IndexOf(numberFormat.NumberDecimalSeparator);
		if (num == -1)
		{
			return s;
		}
		char c = default(char);
		char c2 = default(char);
		char c3 = default(char);
		try
		{
			c = s[0];
			c2 = s[1];
			c3 = s[2];
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
		if (s[num] == '.')
		{
			if (c == '0' && c2 == '.')
			{
				return s.Substring(1);
			}
			if ((c != '-' && c != '+' && c != ' ') || c2 != '0' || c3 != '.')
			{
				return s;
			}
		}
		StringBuilder stringBuilder = new StringBuilder(s);
		stringBuilder[num] = '.';
		if (c == '0' && c2 == '.')
		{
			return stringBuilder.ToString(1, checked(stringBuilder.Length - 1));
		}
		if ((c == '-' || c == '+' || c == ' ') && c2 == '0' && c3 == '.')
		{
			stringBuilder.Remove(1, 1);
			return stringBuilder.ToString();
		}
		return stringBuilder.ToString();
	}

	internal static string OctFromLong(long Val)
	{
		string text = "";
		int num = Convert.ToInt32('0');
		checked
		{
			bool flag = default(bool);
			if (Val < 0)
			{
				Val = long.MaxValue + Val + 1;
				flag = true;
			}
			do
			{
				int num2 = (int)unchecked(Val % 8);
				Val >>= 3;
				text += Conversions.ToString(Strings.ChrW(num2 + num));
			}
			while (Val > 0);
			text = Strings.StrReverse(text);
			if (flag)
			{
				text = "1" + text;
			}
			return text;
		}
	}

	internal static string OctFromULong(ulong Val)
	{
		string text = "";
		int num = Convert.ToInt32('0');
		checked
		{
			do
			{
				int num2 = (int)unchecked(Val % 8);
				Val >>= 3;
				text += Conversions.ToString(Strings.ChrW(num2 + num));
			}
			while (Val != 0L);
			return Strings.StrReverse(text);
		}
	}

	internal static CultureInfo GetCultureInfo()
	{
		return CultureInfo.CurrentCulture;
	}

	internal static CultureInfo GetInvariantCultureInfo()
	{
		return CultureInfo.InvariantCulture;
	}

	internal static string ToHalfwidthNumbers(string s, CultureInfo culture)
	{
		return s;
	}

	internal static bool IsHexOrOctValue(string value, ref long i64Value)
	{
		int length = value.Length;
		checked
		{
			int num = default(int);
			char c;
			while (true)
			{
				if (num < length)
				{
					c = value[num];
					if (c == '&' && num + 2 < length)
					{
						break;
					}
					if (c != ' ' && c != '\u3000')
					{
						return false;
					}
					num++;
					continue;
				}
				return false;
			}
			c = char.ToLowerInvariant(value[num + 1]);
			string value2 = ToHalfwidthNumbers(value.Substring(num + 2), GetCultureInfo());
			switch (c)
			{
			case 'h':
				i64Value = Convert.ToInt64(value2, 16);
				break;
			case 'o':
				i64Value = Convert.ToInt64(value2, 8);
				break;
			default:
				throw new FormatException();
			}
			return true;
		}
	}

	internal static bool IsHexOrOctValue(string value, ref ulong ui64Value)
	{
		int length = value.Length;
		checked
		{
			int num = default(int);
			char c;
			while (true)
			{
				if (num < length)
				{
					c = value[num];
					if (c == '&' && num + 2 < length)
					{
						break;
					}
					if (c != ' ' && c != '\u3000')
					{
						return false;
					}
					num++;
					continue;
				}
				return false;
			}
			c = char.ToLowerInvariant(value[num + 1]);
			string value2 = ToHalfwidthNumbers(value.Substring(num + 2), GetCultureInfo());
			switch (c)
			{
			case 'h':
				ui64Value = Convert.ToUInt64(value2, 16);
				break;
			case 'o':
				ui64Value = Convert.ToUInt64(value2, 8);
				break;
			default:
				throw new FormatException();
			}
			return true;
		}
	}

	internal static string VBFriendlyName(object obj)
	{
		if (obj == null)
		{
			return "Nothing";
		}
		return VBFriendlyName(obj.GetType(), obj);
	}

	internal static string VBFriendlyName(Type typ)
	{
		return VBFriendlyNameOfType(typ);
	}

	internal static string VBFriendlyName(Type typ, object o)
	{
		if (typ.IsCOMObject && Operators.CompareString(typ.FullName, "System.__ComObject", TextCompare: false) == 0)
		{
			return Information.TypeNameOfCOMObject(o, bThrowException: false);
		}
		return VBFriendlyNameOfType(typ);
	}

	internal static string VBFriendlyNameOfType(Type typ, bool fullName = false)
	{
		string arraySuffixAndElementType = GetArraySuffixAndElementType(ref typ);
		string text;
		switch (typ.IsEnum ? TypeCode.Object : ReflectionExtensions.GetTypeCode(typ))
		{
		case TypeCode.Boolean:
			text = "Boolean";
			break;
		case TypeCode.SByte:
			text = "SByte";
			break;
		case TypeCode.Byte:
			text = "Byte";
			break;
		case TypeCode.Int16:
			text = "Short";
			break;
		case TypeCode.UInt16:
			text = "UShort";
			break;
		case TypeCode.Int32:
			text = "Integer";
			break;
		case TypeCode.UInt32:
			text = "UInteger";
			break;
		case TypeCode.Int64:
			text = "Long";
			break;
		case TypeCode.UInt64:
			text = "ULong";
			break;
		case TypeCode.Decimal:
			text = "Decimal";
			break;
		case TypeCode.Single:
			text = "Single";
			break;
		case TypeCode.Double:
			text = "Double";
			break;
		case TypeCode.DateTime:
			text = "Date";
			break;
		case TypeCode.Char:
			text = "Char";
			break;
		case TypeCode.String:
			text = "String";
			break;
		default:
		{
			if (Symbols.IsGenericParameter(typ))
			{
				text = typ.Name;
				break;
			}
			string text2 = null;
			string genericArgsSuffix = GetGenericArgsSuffix(typ);
			string text3;
			if (fullName)
			{
				if ((object)typ.DeclaringType != null)
				{
					text2 = VBFriendlyNameOfType(typ.DeclaringType, fullName: true);
					text3 = typ.Name;
				}
				else
				{
					text3 = typ.FullName;
					if (text3 == null)
					{
						text3 = typ.Name;
					}
				}
			}
			else
			{
				text3 = typ.Name;
			}
			if (genericArgsSuffix != null)
			{
				int num = text3.LastIndexOf('`');
				if (num != -1)
				{
					text3 = text3.Substring(0, num);
				}
				text = text3 + genericArgsSuffix;
			}
			else
			{
				text = text3;
			}
			if (text2 != null)
			{
				text = text2 + "." + text;
			}
			break;
		}
		}
		if (arraySuffixAndElementType != null)
		{
			text += arraySuffixAndElementType;
		}
		return text;
	}

	private static string GetArraySuffixAndElementType(ref Type typ)
	{
		if (!typ.IsArray)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		do
		{
			stringBuilder.Append('(');
			stringBuilder.Append(',', checked(typ.GetArrayRank() - 1));
			stringBuilder.Append(')');
			typ = typ.GetElementType();
		}
		while (typ.IsArray);
		return stringBuilder.ToString();
	}

	private static string GetGenericArgsSuffix(Type typ)
	{
		if (!typ.IsGenericType)
		{
			return null;
		}
		Type[] genericArguments = typ.GetGenericArguments();
		int num = genericArguments.Length;
		int num2 = num;
		checked
		{
			if ((object)typ.DeclaringType != null && typ.DeclaringType.IsGenericType)
			{
				num2 -= typ.DeclaringType.GetGenericArguments().Length;
			}
			if (num2 == 0)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(Of ");
			int num3 = num - num2;
			int num4 = num - 1;
			for (int i = num3; i <= num4; i++)
			{
				stringBuilder.Append(VBFriendlyNameOfType(genericArguments[i]));
				if (i != num - 1)
				{
					stringBuilder.Append(',');
				}
			}
			stringBuilder.Append(')');
			return stringBuilder.ToString();
		}
	}

	internal static string ParameterToString(ParameterInfo parameter)
	{
		string text = "";
		Type type = parameter.ParameterType;
		if (parameter.IsOptional)
		{
			text += "[";
		}
		if (type.IsByRef)
		{
			text += "ByRef ";
			type = type.GetElementType();
		}
		else if (Symbols.IsParamArray(parameter))
		{
			text += "ParamArray ";
		}
		text = text + parameter.Name + " As " + VBFriendlyNameOfType(type, fullName: true);
		if (parameter.IsOptional)
		{
			object defaultValue = parameter.DefaultValue;
			if (defaultValue == null)
			{
				text += " = Nothing";
			}
			else
			{
				Type type2 = defaultValue.GetType();
				if ((object)type2 != s_voidType)
				{
					if (Symbols.IsEnum(type2))
					{
						throw new InvalidOperationException();
					}
					text = text + " = " + Conversions.ToString(defaultValue);
				}
			}
			text += "]";
		}
		return text;
	}

	internal static string MethodToString(MethodBase method)
	{
		Type type = null;
		string text = "";
		if (method.MemberType == MemberTypes.Method)
		{
			type = ((MethodInfo)method).ReturnType;
		}
		if (method.IsPublic)
		{
			text += "Public ";
		}
		else if (method.IsPrivate)
		{
			text += "Private ";
		}
		else if (method.IsAssembly)
		{
			text += "Friend ";
		}
		if ((method.Attributes & MethodAttributes.Virtual) != MethodAttributes.PrivateScope)
		{
			if (!method.DeclaringType.IsInterface)
			{
				text += "Overrides ";
			}
		}
		else if (Symbols.IsShared(method))
		{
			text += "Shared ";
		}
		Symbols.UserDefinedOperator userDefinedOperator = Symbols.UserDefinedOperator.UNDEF;
		if (Symbols.IsUserDefinedOperator(method))
		{
			userDefinedOperator = Symbols.MapToUserDefinedOperator(method);
		}
		if (userDefinedOperator == Symbols.UserDefinedOperator.UNDEF)
		{
			text = (((object)type != null && (object)type != s_voidType) ? (text + "Function ") : (text + "Sub "));
		}
		else
		{
			switch (userDefinedOperator)
			{
			case Symbols.UserDefinedOperator.Narrow:
				text += "Narrowing ";
				break;
			case Symbols.UserDefinedOperator.Widen:
				text += "Widening ";
				break;
			}
			text += "Operator ";
		}
		text = ((userDefinedOperator != Symbols.UserDefinedOperator.UNDEF) ? (text + Symbols.OperatorNames[(int)userDefinedOperator]) : ((method.MemberType != MemberTypes.Constructor) ? (text + method.Name) : (text + "New")));
		bool flag;
		if (Symbols.IsGeneric(method))
		{
			text += "(Of ";
			flag = true;
			Type[] typeParameters = Symbols.GetTypeParameters(method);
			foreach (Type typ in typeParameters)
			{
				if (!flag)
				{
					text += ", ";
				}
				else
				{
					flag = false;
				}
				text += VBFriendlyNameOfType(typ);
			}
			text += ")";
		}
		text += "(";
		flag = true;
		ParameterInfo[] parameters = method.GetParameters();
		foreach (ParameterInfo parameter in parameters)
		{
			if (!flag)
			{
				text += ", ";
			}
			else
			{
				flag = false;
			}
			text += ParameterToString(parameter);
		}
		text += ")";
		if ((object)type != null && (object)type != s_voidType)
		{
			text = text + " As " + VBFriendlyNameOfType(type, fullName: true);
		}
		return text;
	}

	internal static string PropertyToString(PropertyInfo prop)
	{
		string text = "";
		PropertyKind propertyKind = PropertyKind.ReadWrite;
		MethodInfo methodInfo = prop.GetGetMethod();
		checked
		{
			ParameterInfo[] array;
			Type typ;
			if ((object)methodInfo != null)
			{
				propertyKind = (((object)prop.GetSetMethod() == null) ? PropertyKind.ReadOnly : PropertyKind.ReadWrite);
				array = methodInfo.GetParameters();
				typ = methodInfo.ReturnType;
			}
			else
			{
				propertyKind = PropertyKind.WriteOnly;
				methodInfo = prop.GetSetMethod();
				ParameterInfo[] parameters = methodInfo.GetParameters();
				array = new ParameterInfo[parameters.Length - 2 + 1];
				Array.Copy(parameters, array, array.Length);
				typ = parameters[parameters.Length - 1].ParameterType;
			}
			text += "Public ";
			if ((methodInfo.Attributes & MethodAttributes.Virtual) != MethodAttributes.PrivateScope)
			{
				if (!prop.DeclaringType.IsInterface)
				{
					text += "Overrides ";
				}
			}
			else if (Symbols.IsShared(methodInfo))
			{
				text += "Shared ";
			}
			if (propertyKind == PropertyKind.ReadOnly)
			{
				text += "ReadOnly ";
			}
			if (propertyKind == PropertyKind.WriteOnly)
			{
				text += "WriteOnly ";
			}
			text = text + "Property " + prop.Name + "(";
			bool flag = true;
			ParameterInfo[] array2 = array;
			foreach (ParameterInfo parameter in array2)
			{
				if (!flag)
				{
					text += ", ";
				}
				else
				{
					flag = false;
				}
				text += ParameterToString(parameter);
			}
			return text + ") As " + VBFriendlyNameOfType(typ, fullName: true);
		}
	}

	internal static string AdjustArraySuffix(string sRank)
	{
		string text = null;
		checked
		{
			for (int num = sRank.Length; num > 0; num--)
			{
				char c = sRank[num - 1];
				text = c switch
				{
					')' => text + "(", 
					'(' => text + ")", 
					',' => text + Conversions.ToString(c), 
					_ => Conversions.ToString(c) + text, 
				};
			}
			return text;
		}
	}

	internal static string MemberToString(MemberInfo member)
	{
		switch (member.MemberType)
		{
		case MemberTypes.Constructor:
		case MemberTypes.Method:
			return MethodToString((MethodBase)member);
		case MemberTypes.Field:
			return FieldToString((FieldInfo)member);
		case MemberTypes.Property:
			return PropertyToString((PropertyInfo)member);
		default:
			return member.Name;
		}
	}

	internal static string FieldToString(FieldInfo field)
	{
		string text = "";
		Type fieldType = field.FieldType;
		if (field.IsPublic)
		{
			text += "Public ";
		}
		else if (field.IsPrivate)
		{
			text += "Private ";
		}
		else if (field.IsAssembly)
		{
			text += "Friend ";
		}
		else if (field.IsFamily)
		{
			text += "Protected ";
		}
		else if (field.IsFamilyOrAssembly)
		{
			text += "Protected Friend ";
		}
		text += field.Name;
		text += " As ";
		return text + VBFriendlyNameOfType(fieldType, fullName: true);
	}

	[DebuggerHidden]
	internal static void SetTime(DateTime dtTime)
	{
		NativeTypes.SystemTime systemTime = new NativeTypes.SystemTime();
		SafeNativeMethods.GetLocalTime(systemTime);
		checked
		{
			systemTime.wHour = (short)dtTime.Hour;
			systemTime.wMinute = (short)dtTime.Minute;
			systemTime.wSecond = (short)dtTime.Second;
			systemTime.wMilliseconds = (short)dtTime.Millisecond;
			if (UnsafeNativeMethods.SetLocalTime(systemTime) == 0)
			{
				if (Marshal.GetLastWin32Error() == 87)
				{
					throw new ArgumentException(System.SR.Argument_InvalidValue);
				}
				throw new SecurityException(System.SR.SetLocalTimeFailure);
			}
		}
	}

	[DebuggerHidden]
	internal static void SetDate(DateTime vDate)
	{
		NativeTypes.SystemTime systemTime = new NativeTypes.SystemTime();
		SafeNativeMethods.GetLocalTime(systemTime);
		checked
		{
			systemTime.wYear = (short)vDate.Year;
			systemTime.wMonth = (short)vDate.Month;
			systemTime.wDay = (short)vDate.Day;
			if (UnsafeNativeMethods.SetLocalTime(systemTime) == 0)
			{
				if (Marshal.GetLastWin32Error() == 87)
				{
					throw new ArgumentException(System.SR.Argument_InvalidValue);
				}
				throw new SecurityException(System.SR.SetLocalDateFailure);
			}
		}
	}

	internal static DateTimeFormatInfo GetDateTimeFormatInfo()
	{
		return Thread.CurrentThread.CurrentCulture.DateTimeFormat;
	}

	internal static Encoding GetFileIOEncoding()
	{
		return Encoding.Default;
	}

	internal static int GetLocaleCodePage()
	{
		return Thread.CurrentThread.CurrentCulture.TextInfo.ANSICodePage;
	}

	public static Array CopyArray(Array arySrc, Array aryDest)
	{
		if (arySrc == null)
		{
			return aryDest;
		}
		int length = arySrc.Length;
		if (length == 0)
		{
			return aryDest;
		}
		if (aryDest.Rank != arySrc.Rank)
		{
			throw new InvalidCastException();
		}
		checked
		{
			int num = aryDest.Rank - 2;
			for (int i = 0; i <= num; i++)
			{
				if (aryDest.GetUpperBound(i) != arySrc.GetUpperBound(i))
				{
					throw new ArrayTypeMismatchException();
				}
			}
			if (length > aryDest.Length)
			{
				length = aryDest.Length;
			}
			if (arySrc.Rank > 1)
			{
				int rank = arySrc.Rank;
				int length2 = arySrc.GetLength(rank - 1);
				int length3 = aryDest.GetLength(rank - 1);
				if (length3 == 0)
				{
					return aryDest;
				}
				int length4 = ((length2 > length3) ? length3 : length2);
				int num2 = unchecked(arySrc.Length / length2) - 1;
				for (int j = 0; j <= num2; j++)
				{
					Array.Copy(arySrc, j * length2, aryDest, j * length3, length4);
				}
			}
			else
			{
				Array.Copy(arySrc, aryDest, length);
			}
			return aryDest;
		}
	}
}
