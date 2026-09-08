using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions;

internal static class CachedReflectionInfo
{
	[CompilerGenerated]
	private static ConstructorInfo _003CNullable_Boolean_Ctor_003Ek__BackingField;

	[CompilerGenerated]
	private static ConstructorInfo _003CDecimal_Ctor_Int32_003Ek__BackingField;

	[CompilerGenerated]
	private static ConstructorInfo _003CDecimal_Ctor_UInt32_003Ek__BackingField;

	[CompilerGenerated]
	private static ConstructorInfo _003CDecimal_Ctor_Int64_003Ek__BackingField;

	[CompilerGenerated]
	private static ConstructorInfo _003CDecimal_Ctor_UInt64_003Ek__BackingField;

	[CompilerGenerated]
	private static ConstructorInfo _003CDecimal_Ctor_Int32_Int32_Int32_Bool_Byte_003Ek__BackingField;

	[CompilerGenerated]
	private static FieldInfo _003CDecimal_One_003Ek__BackingField;

	[CompilerGenerated]
	private static FieldInfo _003CDecimal_MinusOne_003Ek__BackingField;

	[CompilerGenerated]
	private static FieldInfo _003CDecimal_MinValue_003Ek__BackingField;

	[CompilerGenerated]
	private static FieldInfo _003CDecimal_MaxValue_003Ek__BackingField;

	[CompilerGenerated]
	private static FieldInfo _003CDecimal_Zero_003Ek__BackingField;

	[CompilerGenerated]
	private static FieldInfo _003CDateTime_MinValue_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CMethodBase_GetMethodFromHandle_RuntimeMethodHandle_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CMethodBase_GetMethodFromHandle_RuntimeMethodHandle_RuntimeTypeHandle_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CMethodInfo_CreateDelegate_Type_Object_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CString_op_Equality_String_String_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CString_Equals_String_String_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CDictionaryOfStringInt32_Add_String_Int32_003Ek__BackingField;

	[CompilerGenerated]
	private static ConstructorInfo _003CDictionaryOfStringInt32_Ctor_Int32_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CType_GetTypeFromHandle_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CObject_GetType_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CDecimal_op_Implicit_Byte_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CDecimal_op_Implicit_SByte_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CDecimal_op_Implicit_Int16_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CDecimal_op_Implicit_UInt16_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CDecimal_op_Implicit_Int32_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CDecimal_op_Implicit_UInt32_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CDecimal_op_Implicit_Int64_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CDecimal_op_Implicit_UInt64_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CDecimal_op_Implicit_Char_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CMath_Pow_Double_Double_003Ek__BackingField;

	[CompilerGenerated]
	private static ConstructorInfo _003CClosure_ObjectArray_ObjectArray_003Ek__BackingField;

	[CompilerGenerated]
	private static FieldInfo _003CClosure_Constants_003Ek__BackingField;

	[CompilerGenerated]
	private static FieldInfo _003CClosure_Locals_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CRuntimeOps_CreateRuntimeVariables_ObjectArray_Int64Array_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CRuntimeOps_CreateRuntimeVariables_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CRuntimeOps_MergeRuntimeVariables_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CRuntimeOps_Quote_003Ek__BackingField;

	private static MethodInfo s_String_Format_String_ObjectArray;

	private static ConstructorInfo s_InvalidCastException_Ctor_String;

	private static MethodInfo s_CallSiteOps_SetNotMatched;

	private static MethodInfo s_CallSiteOps_GetMatch;

	private static MethodInfo s_CallSiteOps_ClearMatch;

	private static MethodInfo s_DynamicObject_TryGetMember;

	private static MethodInfo s_DynamicObject_TrySetMember;

	private static MethodInfo s_DynamicObject_TryDeleteMember;

	private static MethodInfo s_DynamicObject_TryGetIndex;

	private static MethodInfo s_DynamicObject_TrySetIndex;

	private static MethodInfo s_DynamicObject_TryDeleteIndex;

	private static MethodInfo s_DynamicObject_TryConvert;

	private static MethodInfo s_DynamicObject_TryInvoke;

	private static MethodInfo s_DynamicObject_TryInvokeMember;

	private static MethodInfo s_DynamicObject_TryBinaryOperation;

	private static MethodInfo s_DynamicObject_TryUnaryOperation;

	private static MethodInfo s_DynamicObject_TryCreateInstance;

	public static ConstructorInfo Nullable_Boolean_Ctor => _003CNullable_Boolean_Ctor_003Ek__BackingField ?? (_003CNullable_Boolean_Ctor_003Ek__BackingField = typeof(bool?).GetConstructor(new Type[1] { typeof(bool) }));

	public static ConstructorInfo Decimal_Ctor_Int32 => _003CDecimal_Ctor_Int32_003Ek__BackingField ?? (_003CDecimal_Ctor_Int32_003Ek__BackingField = typeof(decimal).GetConstructor(new Type[1] { typeof(int) }));

	public static ConstructorInfo Decimal_Ctor_UInt32 => _003CDecimal_Ctor_UInt32_003Ek__BackingField ?? (_003CDecimal_Ctor_UInt32_003Ek__BackingField = typeof(decimal).GetConstructor(new Type[1] { typeof(uint) }));

	public static ConstructorInfo Decimal_Ctor_Int64 => _003CDecimal_Ctor_Int64_003Ek__BackingField ?? (_003CDecimal_Ctor_Int64_003Ek__BackingField = typeof(decimal).GetConstructor(new Type[1] { typeof(long) }));

	public static ConstructorInfo Decimal_Ctor_UInt64 => _003CDecimal_Ctor_UInt64_003Ek__BackingField ?? (_003CDecimal_Ctor_UInt64_003Ek__BackingField = typeof(decimal).GetConstructor(new Type[1] { typeof(ulong) }));

	public static ConstructorInfo Decimal_Ctor_Int32_Int32_Int32_Bool_Byte => _003CDecimal_Ctor_Int32_Int32_Int32_Bool_Byte_003Ek__BackingField ?? (_003CDecimal_Ctor_Int32_Int32_Int32_Bool_Byte_003Ek__BackingField = typeof(decimal).GetConstructor(new Type[5]
	{
		typeof(int),
		typeof(int),
		typeof(int),
		typeof(bool),
		typeof(byte)
	}));

	public static FieldInfo Decimal_One => _003CDecimal_One_003Ek__BackingField ?? (_003CDecimal_One_003Ek__BackingField = typeof(decimal).GetField("One"));

	public static FieldInfo Decimal_MinusOne => _003CDecimal_MinusOne_003Ek__BackingField ?? (_003CDecimal_MinusOne_003Ek__BackingField = typeof(decimal).GetField("MinusOne"));

	public static FieldInfo Decimal_MinValue => _003CDecimal_MinValue_003Ek__BackingField ?? (_003CDecimal_MinValue_003Ek__BackingField = typeof(decimal).GetField("MinValue"));

	public static FieldInfo Decimal_MaxValue => _003CDecimal_MaxValue_003Ek__BackingField ?? (_003CDecimal_MaxValue_003Ek__BackingField = typeof(decimal).GetField("MaxValue"));

	public static FieldInfo Decimal_Zero => _003CDecimal_Zero_003Ek__BackingField ?? (_003CDecimal_Zero_003Ek__BackingField = typeof(decimal).GetField("Zero"));

	public static FieldInfo DateTime_MinValue => _003CDateTime_MinValue_003Ek__BackingField ?? (_003CDateTime_MinValue_003Ek__BackingField = typeof(DateTime).GetField("MinValue"));

	public static MethodInfo MethodBase_GetMethodFromHandle_RuntimeMethodHandle => _003CMethodBase_GetMethodFromHandle_RuntimeMethodHandle_003Ek__BackingField ?? (_003CMethodBase_GetMethodFromHandle_RuntimeMethodHandle_003Ek__BackingField = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[1] { typeof(RuntimeMethodHandle) }));

	public static MethodInfo MethodBase_GetMethodFromHandle_RuntimeMethodHandle_RuntimeTypeHandle => _003CMethodBase_GetMethodFromHandle_RuntimeMethodHandle_RuntimeTypeHandle_003Ek__BackingField ?? (_003CMethodBase_GetMethodFromHandle_RuntimeMethodHandle_RuntimeTypeHandle_003Ek__BackingField = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[2]
	{
		typeof(RuntimeMethodHandle),
		typeof(RuntimeTypeHandle)
	}));

	public static MethodInfo MethodInfo_CreateDelegate_Type_Object => _003CMethodInfo_CreateDelegate_Type_Object_003Ek__BackingField ?? (_003CMethodInfo_CreateDelegate_Type_Object_003Ek__BackingField = typeof(MethodInfo).GetMethod("CreateDelegate", new Type[2]
	{
		typeof(Type),
		typeof(object)
	}));

	public static MethodInfo String_op_Equality_String_String => _003CString_op_Equality_String_String_003Ek__BackingField ?? (_003CString_op_Equality_String_String_003Ek__BackingField = typeof(string).GetMethod("op_Equality", new Type[2]
	{
		typeof(string),
		typeof(string)
	}));

	public static MethodInfo String_Equals_String_String => _003CString_Equals_String_String_003Ek__BackingField ?? (_003CString_Equals_String_String_003Ek__BackingField = typeof(string).GetMethod("Equals", new Type[2]
	{
		typeof(string),
		typeof(string)
	}));

	public static MethodInfo DictionaryOfStringInt32_Add_String_Int32 => _003CDictionaryOfStringInt32_Add_String_Int32_003Ek__BackingField ?? (_003CDictionaryOfStringInt32_Add_String_Int32_003Ek__BackingField = typeof(Dictionary<string, int>).GetMethod("Add", new Type[2]
	{
		typeof(string),
		typeof(int)
	}));

	public static ConstructorInfo DictionaryOfStringInt32_Ctor_Int32 => _003CDictionaryOfStringInt32_Ctor_Int32_003Ek__BackingField ?? (_003CDictionaryOfStringInt32_Ctor_Int32_003Ek__BackingField = typeof(Dictionary<string, int>).GetConstructor(new Type[1] { typeof(int) }));

	public static MethodInfo Type_GetTypeFromHandle => _003CType_GetTypeFromHandle_003Ek__BackingField ?? (_003CType_GetTypeFromHandle_003Ek__BackingField = typeof(Type).GetMethod("GetTypeFromHandle"));

	public static MethodInfo Object_GetType => _003CObject_GetType_003Ek__BackingField ?? (_003CObject_GetType_003Ek__BackingField = typeof(object).GetMethod("GetType"));

	public static MethodInfo Decimal_op_Implicit_Byte => _003CDecimal_op_Implicit_Byte_003Ek__BackingField ?? (_003CDecimal_op_Implicit_Byte_003Ek__BackingField = typeof(decimal).GetMethod("op_Implicit", new Type[1] { typeof(byte) }));

	public static MethodInfo Decimal_op_Implicit_SByte => _003CDecimal_op_Implicit_SByte_003Ek__BackingField ?? (_003CDecimal_op_Implicit_SByte_003Ek__BackingField = typeof(decimal).GetMethod("op_Implicit", new Type[1] { typeof(sbyte) }));

	public static MethodInfo Decimal_op_Implicit_Int16 => _003CDecimal_op_Implicit_Int16_003Ek__BackingField ?? (_003CDecimal_op_Implicit_Int16_003Ek__BackingField = typeof(decimal).GetMethod("op_Implicit", new Type[1] { typeof(short) }));

	public static MethodInfo Decimal_op_Implicit_UInt16 => _003CDecimal_op_Implicit_UInt16_003Ek__BackingField ?? (_003CDecimal_op_Implicit_UInt16_003Ek__BackingField = typeof(decimal).GetMethod("op_Implicit", new Type[1] { typeof(ushort) }));

	public static MethodInfo Decimal_op_Implicit_Int32 => _003CDecimal_op_Implicit_Int32_003Ek__BackingField ?? (_003CDecimal_op_Implicit_Int32_003Ek__BackingField = typeof(decimal).GetMethod("op_Implicit", new Type[1] { typeof(int) }));

	public static MethodInfo Decimal_op_Implicit_UInt32 => _003CDecimal_op_Implicit_UInt32_003Ek__BackingField ?? (_003CDecimal_op_Implicit_UInt32_003Ek__BackingField = typeof(decimal).GetMethod("op_Implicit", new Type[1] { typeof(uint) }));

	public static MethodInfo Decimal_op_Implicit_Int64 => _003CDecimal_op_Implicit_Int64_003Ek__BackingField ?? (_003CDecimal_op_Implicit_Int64_003Ek__BackingField = typeof(decimal).GetMethod("op_Implicit", new Type[1] { typeof(long) }));

	public static MethodInfo Decimal_op_Implicit_UInt64 => _003CDecimal_op_Implicit_UInt64_003Ek__BackingField ?? (_003CDecimal_op_Implicit_UInt64_003Ek__BackingField = typeof(decimal).GetMethod("op_Implicit", new Type[1] { typeof(ulong) }));

	public static MethodInfo Decimal_op_Implicit_Char => _003CDecimal_op_Implicit_Char_003Ek__BackingField ?? (_003CDecimal_op_Implicit_Char_003Ek__BackingField = typeof(decimal).GetMethod("op_Implicit", new Type[1] { typeof(char) }));

	public static MethodInfo Math_Pow_Double_Double => _003CMath_Pow_Double_Double_003Ek__BackingField ?? (_003CMath_Pow_Double_Double_003Ek__BackingField = typeof(Math).GetMethod("Pow", new Type[2]
	{
		typeof(double),
		typeof(double)
	}));

	public static ConstructorInfo Closure_ObjectArray_ObjectArray => _003CClosure_ObjectArray_ObjectArray_003Ek__BackingField ?? (_003CClosure_ObjectArray_ObjectArray_003Ek__BackingField = typeof(Closure).GetConstructor(new Type[2]
	{
		typeof(object[]),
		typeof(object[])
	}));

	public static FieldInfo Closure_Constants => _003CClosure_Constants_003Ek__BackingField ?? (_003CClosure_Constants_003Ek__BackingField = typeof(Closure).GetField("Constants"));

	public static FieldInfo Closure_Locals => _003CClosure_Locals_003Ek__BackingField ?? (_003CClosure_Locals_003Ek__BackingField = typeof(Closure).GetField("Locals"));

	public static MethodInfo RuntimeOps_CreateRuntimeVariables_ObjectArray_Int64Array => _003CRuntimeOps_CreateRuntimeVariables_ObjectArray_Int64Array_003Ek__BackingField ?? (_003CRuntimeOps_CreateRuntimeVariables_ObjectArray_Int64Array_003Ek__BackingField = typeof(RuntimeOps).GetMethod("CreateRuntimeVariables", new Type[2]
	{
		typeof(object[]),
		typeof(long[])
	}));

	public static MethodInfo RuntimeOps_CreateRuntimeVariables => _003CRuntimeOps_CreateRuntimeVariables_003Ek__BackingField ?? (_003CRuntimeOps_CreateRuntimeVariables_003Ek__BackingField = typeof(RuntimeOps).GetMethod("CreateRuntimeVariables", Type.EmptyTypes));

	public static MethodInfo RuntimeOps_MergeRuntimeVariables => _003CRuntimeOps_MergeRuntimeVariables_003Ek__BackingField ?? (_003CRuntimeOps_MergeRuntimeVariables_003Ek__BackingField = typeof(RuntimeOps).GetMethod("MergeRuntimeVariables"));

	public static MethodInfo RuntimeOps_Quote => _003CRuntimeOps_Quote_003Ek__BackingField ?? (_003CRuntimeOps_Quote_003Ek__BackingField = typeof(RuntimeOps).GetMethod("Quote"));

	public static MethodInfo String_Format_String_ObjectArray => s_String_Format_String_ObjectArray ?? (s_String_Format_String_ObjectArray = typeof(string).GetMethod("Format", new Type[2]
	{
		typeof(string),
		typeof(object[])
	}));

	public static ConstructorInfo InvalidCastException_Ctor_String => s_InvalidCastException_Ctor_String ?? (s_InvalidCastException_Ctor_String = typeof(InvalidCastException).GetConstructor(new Type[1] { typeof(string) }));

	public static MethodInfo CallSiteOps_SetNotMatched => s_CallSiteOps_SetNotMatched ?? (s_CallSiteOps_SetNotMatched = typeof(CallSiteOps).GetMethod("SetNotMatched"));

	public static MethodInfo CallSiteOps_GetMatch => s_CallSiteOps_GetMatch ?? (s_CallSiteOps_GetMatch = typeof(CallSiteOps).GetMethod("GetMatch"));

	public static MethodInfo CallSiteOps_ClearMatch => s_CallSiteOps_ClearMatch ?? (s_CallSiteOps_ClearMatch = typeof(CallSiteOps).GetMethod("ClearMatch"));

	public static MethodInfo DynamicObject_TryGetMember => s_DynamicObject_TryGetMember ?? (s_DynamicObject_TryGetMember = typeof(DynamicObject).GetMethod("TryGetMember"));

	public static MethodInfo DynamicObject_TrySetMember => s_DynamicObject_TrySetMember ?? (s_DynamicObject_TrySetMember = typeof(DynamicObject).GetMethod("TrySetMember"));

	public static MethodInfo DynamicObject_TryDeleteMember => s_DynamicObject_TryDeleteMember ?? (s_DynamicObject_TryDeleteMember = typeof(DynamicObject).GetMethod("TryDeleteMember"));

	public static MethodInfo DynamicObject_TryGetIndex => s_DynamicObject_TryGetIndex ?? (s_DynamicObject_TryGetIndex = typeof(DynamicObject).GetMethod("TryGetIndex"));

	public static MethodInfo DynamicObject_TrySetIndex => s_DynamicObject_TrySetIndex ?? (s_DynamicObject_TrySetIndex = typeof(DynamicObject).GetMethod("TrySetIndex"));

	public static MethodInfo DynamicObject_TryDeleteIndex => s_DynamicObject_TryDeleteIndex ?? (s_DynamicObject_TryDeleteIndex = typeof(DynamicObject).GetMethod("TryDeleteIndex"));

	public static MethodInfo DynamicObject_TryConvert => s_DynamicObject_TryConvert ?? (s_DynamicObject_TryConvert = typeof(DynamicObject).GetMethod("TryConvert"));

	public static MethodInfo DynamicObject_TryInvoke => s_DynamicObject_TryInvoke ?? (s_DynamicObject_TryInvoke = typeof(DynamicObject).GetMethod("TryInvoke"));

	public static MethodInfo DynamicObject_TryInvokeMember => s_DynamicObject_TryInvokeMember ?? (s_DynamicObject_TryInvokeMember = typeof(DynamicObject).GetMethod("TryInvokeMember"));

	public static MethodInfo DynamicObject_TryBinaryOperation => s_DynamicObject_TryBinaryOperation ?? (s_DynamicObject_TryBinaryOperation = typeof(DynamicObject).GetMethod("TryBinaryOperation"));

	public static MethodInfo DynamicObject_TryUnaryOperation => s_DynamicObject_TryUnaryOperation ?? (s_DynamicObject_TryUnaryOperation = typeof(DynamicObject).GetMethod("TryUnaryOperation"));

	public static MethodInfo DynamicObject_TryCreateInstance => s_DynamicObject_TryCreateInstance ?? (s_DynamicObject_TryCreateInstance = typeof(DynamicObject).GetMethod("TryCreateInstance"));
}
