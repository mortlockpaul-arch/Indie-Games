using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class Versioned
{
	[RequiresUnreferencedCode("The method name cannot and type cannot be statically analyzed so it may be trimmed")]
	public static object CallByName(object Instance, string MethodName, CallType UseCallType, params object[] Arguments)
	{
		switch (UseCallType)
		{
		case CallType.Method:
			return NewLateBinding.LateCall(Instance, null, MethodName, Arguments, null, null, null, IgnoreReturn: false);
		case CallType.Get:
			return NewLateBinding.LateGet(Instance, null, MethodName, Arguments, null, null, null);
		case CallType.Let:
		case CallType.Set:
		{
			IDynamicMetaObjectProvider dynamicMetaObjectProvider = IDOUtils.TryCastToIDMOP(Instance);
			if (dynamicMetaObjectProvider != null)
			{
				IDOBinder.IDOSet(dynamicMetaObjectProvider, MethodName, null, Arguments);
			}
			else
			{
				NewLateBinding.LateSet(Instance, null, MethodName, Arguments, null, null, OptimisticSet: false, RValueBase: false, UseCallType);
			}
			return null;
		}
		default:
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "CallType"));
		}
	}

	public static bool IsNumeric(object Expression)
	{
		if (!(Expression is IConvertible convertible))
		{
			return false;
		}
		switch (convertible.GetTypeCode())
		{
		case TypeCode.SByte:
		case TypeCode.Byte:
		case TypeCode.Int16:
		case TypeCode.UInt16:
		case TypeCode.Int32:
		case TypeCode.UInt32:
		case TypeCode.Int64:
		case TypeCode.UInt64:
		case TypeCode.Single:
		case TypeCode.Double:
		case TypeCode.Decimal:
			return true;
		case TypeCode.Boolean:
			return true;
		case TypeCode.Char:
		case TypeCode.String:
		{
			string value = convertible.ToString(null);
			try
			{
				long i64Value = default(long);
				if (Utils.IsHexOrOctValue(value, ref i64Value))
				{
					return true;
				}
			}
			catch (FormatException)
			{
				return false;
			}
			double Result = default(double);
			return Conversions.TryParseDouble(value, ref Result);
		}
		default:
			return false;
		}
	}

	public static string TypeName(object Expression)
	{
		if (Expression == null)
		{
			return "Nothing";
		}
		Type type = Expression.GetType();
		return (!type.IsCOMObject || !string.Equals(type.Name, "__ComObject", StringComparison.Ordinal)) ? Utils.VBFriendlyNameOfType(type) : Information.TypeNameOfCOMObject(Expression, bThrowException: true);
	}

	public static string SystemTypeName(string VbName)
	{
		return Strings.Trim(VbName).ToUpperInvariant() switch
		{
			"BOOLEAN" => "System.Boolean", 
			"SBYTE" => "System.SByte", 
			"BYTE" => "System.Byte", 
			"SHORT" => "System.Int16", 
			"USHORT" => "System.UInt16", 
			"INTEGER" => "System.Int32", 
			"UINTEGER" => "System.UInt32", 
			"LONG" => "System.Int64", 
			"ULONG" => "System.UInt64", 
			"DECIMAL" => "System.Decimal", 
			"SINGLE" => "System.Single", 
			"DOUBLE" => "System.Double", 
			"DATE" => "System.DateTime", 
			"CHAR" => "System.Char", 
			"STRING" => "System.String", 
			"OBJECT" => "System.Object", 
			_ => null, 
		};
	}

	public static string VbTypeName(string SystemName)
	{
		SystemName = Strings.Trim(SystemName).ToUpperInvariant();
		if (Operators.CompareString(Strings.Left(SystemName, 7), "SYSTEM.", TextCompare: false) == 0)
		{
			SystemName = Strings.Mid(SystemName, 8);
		}
		return SystemName switch
		{
			"BOOLEAN" => "Boolean", 
			"SBYTE" => "SByte", 
			"BYTE" => "Byte", 
			"INT16" => "Short", 
			"UINT16" => "UShort", 
			"INT32" => "Integer", 
			"UINT32" => "UInteger", 
			"INT64" => "Long", 
			"UINT64" => "ULong", 
			"DECIMAL" => "Decimal", 
			"SINGLE" => "Single", 
			"DOUBLE" => "Double", 
			"DATETIME" => "Date", 
			"CHAR" => "Char", 
			"STRING" => "String", 
			"OBJECT" => "Object", 
			_ => null, 
		};
	}
}
