using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal sealed class VarEnumSelector
{
	private static readonly Dictionary<VarEnum, Type> s_comToManagedPrimitiveTypes = CreateComToManagedPrimitiveTypes();

	private static readonly IList<IList<VarEnum>> s_comPrimitiveTypeFamilies = CreateComPrimitiveTypeFamilies();

	internal VariantBuilder[] VariantBuilders { get; }

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal VarEnumSelector(Type[] explicitArgTypes)
	{
		VariantBuilders = new VariantBuilder[explicitArgTypes.Length];
		for (int i = 0; i < explicitArgTypes.Length; i++)
		{
			VariantBuilders[i] = GetVariantBuilder(explicitArgTypes[i]);
		}
	}

	internal static Type GetManagedMarshalType(VarEnum varEnum)
	{
		if (varEnum == VarEnum.VT_CY)
		{
			return typeof(CurrencyWrapper);
		}
		if (varEnum.IsPrimitiveType())
		{
			return s_comToManagedPrimitiveTypes[varEnum];
		}
		switch (varEnum)
		{
		case VarEnum.VT_EMPTY:
		case VarEnum.VT_NULL:
		case VarEnum.VT_DISPATCH:
		case VarEnum.VT_VARIANT:
		case VarEnum.VT_UNKNOWN:
			return typeof(object);
		case VarEnum.VT_ERROR:
			return typeof(ErrorWrapper);
		default:
			throw Error.UnexpectedVarEnum(varEnum);
		}
	}

	private static Dictionary<VarEnum, Type> CreateComToManagedPrimitiveTypes()
	{
		return new Dictionary<VarEnum, Type>
		{
			{
				VarEnum.VT_I1,
				typeof(sbyte)
			},
			{
				VarEnum.VT_I2,
				typeof(short)
			},
			{
				VarEnum.VT_I4,
				typeof(int)
			},
			{
				VarEnum.VT_I8,
				typeof(long)
			},
			{
				VarEnum.VT_UI1,
				typeof(byte)
			},
			{
				VarEnum.VT_UI2,
				typeof(ushort)
			},
			{
				VarEnum.VT_UI4,
				typeof(uint)
			},
			{
				VarEnum.VT_UI8,
				typeof(ulong)
			},
			{
				VarEnum.VT_INT,
				typeof(int)
			},
			{
				VarEnum.VT_UINT,
				typeof(uint)
			},
			{
				VarEnum.VT_BOOL,
				typeof(bool)
			},
			{
				VarEnum.VT_R4,
				typeof(float)
			},
			{
				VarEnum.VT_R8,
				typeof(double)
			},
			{
				VarEnum.VT_DECIMAL,
				typeof(decimal)
			},
			{
				VarEnum.VT_DATE,
				typeof(DateTime)
			},
			{
				VarEnum.VT_BSTR,
				typeof(string)
			},
			{
				VarEnum.VT_CY,
				typeof(CurrencyWrapper)
			},
			{
				VarEnum.VT_ERROR,
				typeof(ErrorWrapper)
			}
		};
	}

	private static IList<IList<VarEnum>> CreateComPrimitiveTypeFamilies()
	{
		return new VarEnum[11][]
		{
			new VarEnum[4]
			{
				VarEnum.VT_I8,
				VarEnum.VT_I4,
				VarEnum.VT_I2,
				VarEnum.VT_I1
			},
			new VarEnum[4]
			{
				VarEnum.VT_UI8,
				VarEnum.VT_UI4,
				VarEnum.VT_UI2,
				VarEnum.VT_UI1
			},
			new VarEnum[1] { VarEnum.VT_INT },
			new VarEnum[1] { VarEnum.VT_UINT },
			new VarEnum[1] { VarEnum.VT_BOOL },
			new VarEnum[1] { VarEnum.VT_DATE },
			new VarEnum[2]
			{
				VarEnum.VT_R8,
				VarEnum.VT_R4
			},
			new VarEnum[1] { VarEnum.VT_DECIMAL },
			new VarEnum[1] { VarEnum.VT_BSTR },
			new VarEnum[1] { VarEnum.VT_CY },
			new VarEnum[1] { VarEnum.VT_ERROR }
		};
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private static List<VarEnum> GetConversionsToComPrimitiveTypeFamilies(Type argumentType)
	{
		List<VarEnum> list = new List<VarEnum>();
		foreach (IList<VarEnum> s_comPrimitiveTypeFamily in s_comPrimitiveTypeFamilies)
		{
			foreach (VarEnum item in s_comPrimitiveTypeFamily)
			{
				Type destination = s_comToManagedPrimitiveTypes[item];
				if (TypeUtils.IsImplicitlyConvertible(argumentType, destination, considerUserDefined: true))
				{
					list.Add(item);
					break;
				}
			}
		}
		return list;
	}

	private static void CheckForAmbiguousMatch(Type argumentType, List<VarEnum> compatibleComTypes)
	{
		if (compatibleComTypes.Count <= 1)
		{
			return;
		}
		string text = "";
		for (int i = 0; i < compatibleComTypes.Count; i++)
		{
			string name = s_comToManagedPrimitiveTypes[compatibleComTypes[i]].Name;
			if (i == compatibleComTypes.Count - 1)
			{
				text += " and ";
			}
			else if (i != 0)
			{
				text += ", ";
			}
			text += name;
		}
		throw Error.AmbiguousConversion(argumentType.Name, text);
	}

	private static bool TryGetPrimitiveComType(Type argumentType, out VarEnum primitiveVarEnum)
	{
		switch (Type.GetTypeCode(argumentType))
		{
		case TypeCode.Boolean:
			primitiveVarEnum = VarEnum.VT_BOOL;
			return true;
		case TypeCode.Char:
			primitiveVarEnum = VarEnum.VT_UI2;
			return true;
		case TypeCode.SByte:
			primitiveVarEnum = VarEnum.VT_I1;
			return true;
		case TypeCode.Byte:
			primitiveVarEnum = VarEnum.VT_UI1;
			return true;
		case TypeCode.Int16:
			primitiveVarEnum = VarEnum.VT_I2;
			return true;
		case TypeCode.UInt16:
			primitiveVarEnum = VarEnum.VT_UI2;
			return true;
		case TypeCode.Int32:
			primitiveVarEnum = VarEnum.VT_I4;
			return true;
		case TypeCode.UInt32:
			primitiveVarEnum = VarEnum.VT_UI4;
			return true;
		case TypeCode.Int64:
			primitiveVarEnum = VarEnum.VT_I8;
			return true;
		case TypeCode.UInt64:
			primitiveVarEnum = VarEnum.VT_UI8;
			return true;
		case TypeCode.Single:
			primitiveVarEnum = VarEnum.VT_R4;
			return true;
		case TypeCode.Double:
			primitiveVarEnum = VarEnum.VT_R8;
			return true;
		case TypeCode.Decimal:
			primitiveVarEnum = VarEnum.VT_DECIMAL;
			return true;
		case TypeCode.DateTime:
			primitiveVarEnum = VarEnum.VT_DATE;
			return true;
		case TypeCode.String:
			primitiveVarEnum = VarEnum.VT_BSTR;
			return true;
		default:
			if (argumentType == typeof(CurrencyWrapper))
			{
				primitiveVarEnum = VarEnum.VT_CY;
				return true;
			}
			if (argumentType == typeof(ErrorWrapper))
			{
				primitiveVarEnum = VarEnum.VT_ERROR;
				return true;
			}
			if (argumentType == typeof(nint))
			{
				primitiveVarEnum = VarEnum.VT_INT;
				return true;
			}
			if (argumentType == typeof(nuint))
			{
				primitiveVarEnum = VarEnum.VT_UINT;
				return true;
			}
			primitiveVarEnum = VarEnum.VT_VOID;
			return false;
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private static bool TryGetPrimitiveComTypeViaConversion(Type argumentType, out VarEnum primitiveVarEnum)
	{
		List<VarEnum> conversionsToComPrimitiveTypeFamilies = GetConversionsToComPrimitiveTypeFamilies(argumentType);
		CheckForAmbiguousMatch(argumentType, conversionsToComPrimitiveTypeFamilies);
		if (conversionsToComPrimitiveTypeFamilies.Count == 1)
		{
			primitiveVarEnum = conversionsToComPrimitiveTypeFamilies[0];
			return true;
		}
		primitiveVarEnum = VarEnum.VT_VOID;
		return false;
	}

	private static VarEnum GetComType(ref Type argumentType)
	{
		if (argumentType == typeof(Missing))
		{
			return VarEnum.VT_RECORD;
		}
		if (argumentType.IsArray)
		{
			return VarEnum.VT_ARRAY;
		}
		if (argumentType == typeof(UnknownWrapper))
		{
			return VarEnum.VT_UNKNOWN;
		}
		if (argumentType == typeof(DispatchWrapper))
		{
			return VarEnum.VT_DISPATCH;
		}
		if (argumentType == typeof(VariantWrapper))
		{
			return VarEnum.VT_VARIANT;
		}
		if (argumentType == typeof(BStrWrapper))
		{
			return VarEnum.VT_BSTR;
		}
		if (argumentType == typeof(ErrorWrapper))
		{
			return VarEnum.VT_ERROR;
		}
		if (argumentType == typeof(CurrencyWrapper))
		{
			return VarEnum.VT_CY;
		}
		if (argumentType.IsEnum)
		{
			argumentType = Enum.GetUnderlyingType(argumentType);
			return GetComType(ref argumentType);
		}
		if (argumentType.IsNullableType())
		{
			argumentType = TypeUtils.GetNonNullableType(argumentType);
			return GetComType(ref argumentType);
		}
		if (argumentType.IsGenericType)
		{
			return VarEnum.VT_UNKNOWN;
		}
		if (TryGetPrimitiveComType(argumentType, out var primitiveVarEnum))
		{
			return primitiveVarEnum;
		}
		return VarEnum.VT_RECORD;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private static VariantBuilder GetVariantBuilder(Type argumentType)
	{
		if (argumentType == null)
		{
			return new VariantBuilder(VarEnum.VT_EMPTY, new NullArgBuilder());
		}
		if (argumentType == typeof(DBNull))
		{
			return new VariantBuilder(VarEnum.VT_NULL, new NullArgBuilder());
		}
		ArgBuilder simpleArgBuilder;
		if (argumentType.IsByRef)
		{
			Type argumentType2 = argumentType.GetElementType();
			VarEnum varEnum = ((!(argumentType2 == typeof(object)) && !(argumentType2 == typeof(DBNull))) ? GetComType(ref argumentType2) : VarEnum.VT_VARIANT);
			simpleArgBuilder = GetSimpleArgBuilder(argumentType2, varEnum);
			return new VariantBuilder(varEnum | VarEnum.VT_BYREF, simpleArgBuilder);
		}
		VarEnum elementVarEnum = GetComType(ref argumentType);
		simpleArgBuilder = GetByValArgBuilder(argumentType, ref elementVarEnum);
		return new VariantBuilder(elementVarEnum, simpleArgBuilder);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private static ArgBuilder GetByValArgBuilder(Type elementType, ref VarEnum elementVarEnum)
	{
		if (elementVarEnum == VarEnum.VT_RECORD)
		{
			if (TryGetPrimitiveComTypeViaConversion(elementType, out var primitiveVarEnum))
			{
				elementVarEnum = primitiveVarEnum;
				Type managedMarshalType = GetManagedMarshalType(elementVarEnum);
				return new ConversionArgBuilder(elementType, GetSimpleArgBuilder(managedMarshalType, elementVarEnum));
			}
			if (typeof(IConvertible).IsAssignableFrom(elementType))
			{
				return new ConvertibleArgBuilder();
			}
		}
		return GetSimpleArgBuilder(elementType, elementVarEnum);
	}

	private static SimpleArgBuilder GetSimpleArgBuilder(Type elementType, VarEnum elementVarEnum)
	{
		switch (elementVarEnum)
		{
		case VarEnum.VT_BSTR:
			return new StringArgBuilder(elementType);
		case VarEnum.VT_BOOL:
			return new BoolArgBuilder(elementType);
		case VarEnum.VT_DATE:
			return new DateTimeArgBuilder(elementType);
		case VarEnum.VT_CY:
			return new CurrencyArgBuilder(elementType);
		case VarEnum.VT_DISPATCH:
			return new DispatchArgBuilder(elementType);
		case VarEnum.VT_UNKNOWN:
			return new UnknownArgBuilder(elementType);
		case VarEnum.VT_VARIANT:
		case VarEnum.VT_RECORD:
		case VarEnum.VT_ARRAY:
			return new VariantArgBuilder(elementType);
		case VarEnum.VT_ERROR:
			return new ErrorArgBuilder(elementType);
		default:
		{
			Type managedMarshalType = GetManagedMarshalType(elementVarEnum);
			if (elementType == managedMarshalType)
			{
				return new SimpleArgBuilder(elementType);
			}
			return new ConvertArgBuilder(elementType, managedMarshalType);
		}
		}
	}
}
