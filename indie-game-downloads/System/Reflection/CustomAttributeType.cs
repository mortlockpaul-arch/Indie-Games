namespace System.Reflection;

internal sealed class CustomAttributeType
{
	public CustomAttributeEncoding EncodedType { get; }

	public CustomAttributeEncoding EncodedEnumType { get; }

	public CustomAttributeEncoding EncodedArrayType { get; }

	public Type EnumType { get; }

	public CustomAttributeType(CustomAttributeEncoding encodedType, CustomAttributeEncoding encodedArrayType, CustomAttributeEncoding encodedEnumType, Type enumType)
	{
		EncodedType = encodedType;
		EncodedArrayType = encodedArrayType;
		EncodedEnumType = encodedEnumType;
		EnumType = enumType;
	}

	public CustomAttributeType(RuntimeType parameterType)
	{
		CustomAttributeEncoding customAttributeEncoding = RuntimeCustomAttributeData.TypeToCustomAttributeEncoding(parameterType);
		CustomAttributeEncoding customAttributeEncoding2 = CustomAttributeEncoding.Undefined;
		CustomAttributeEncoding customAttributeEncoding3 = CustomAttributeEncoding.Undefined;
		Type type = null;
		if (customAttributeEncoding == CustomAttributeEncoding.Array)
		{
			parameterType = (RuntimeType)parameterType.GetElementType();
			customAttributeEncoding2 = RuntimeCustomAttributeData.TypeToCustomAttributeEncoding(parameterType);
		}
		if (customAttributeEncoding == CustomAttributeEncoding.Enum || customAttributeEncoding2 == CustomAttributeEncoding.Enum)
		{
			type = parameterType;
			customAttributeEncoding3 = RuntimeCustomAttributeData.TypeToCustomAttributeEncoding((RuntimeType)Enum.GetUnderlyingType(parameterType));
		}
		EncodedType = customAttributeEncoding;
		EncodedArrayType = customAttributeEncoding2;
		EncodedEnumType = customAttributeEncoding3;
		EnumType = type;
	}
}
