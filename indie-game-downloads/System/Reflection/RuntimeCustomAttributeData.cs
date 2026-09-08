using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Reflection;

internal sealed class RuntimeCustomAttributeData : CustomAttributeData
{
	private ConstructorInfo m_ctor;

	private readonly RuntimeModule m_scope;

	private readonly CustomAttributeCtorParameter[] m_ctorParams;

	private readonly CustomAttributeNamedParameter[] m_namedParams;

	private IList<CustomAttributeTypedArgument> m_typedCtorArgs;

	private IList<CustomAttributeNamedArgument> m_namedArgs;

	public override ConstructorInfo Constructor => m_ctor;

	public override IList<CustomAttributeTypedArgument> ConstructorArguments
	{
		get
		{
			if (m_typedCtorArgs == null)
			{
				if (m_ctorParams.Length != 0)
				{
					CustomAttributeTypedArgument[] array = new CustomAttributeTypedArgument[m_ctorParams.Length];
					for (int i = 0; i < array.Length; i++)
					{
						CustomAttributeEncodedArgument encodedArgument = m_ctorParams[i].EncodedArgument;
						array[i] = new CustomAttributeTypedArgument(m_scope, encodedArgument);
					}
					m_typedCtorArgs = Array.AsReadOnly(array);
				}
				else
				{
					m_typedCtorArgs = Array.Empty<CustomAttributeTypedArgument>();
				}
			}
			return m_typedCtorArgs;
		}
	}

	public override IList<CustomAttributeNamedArgument> NamedArguments
	{
		get
		{
			if (m_namedArgs == null)
			{
				int num = 0;
				if (m_namedParams != null)
				{
					CustomAttributeNamedParameter[] namedParams = m_namedParams;
					foreach (CustomAttributeNamedParameter customAttributeNamedParameter in namedParams)
					{
						if (customAttributeNamedParameter.EncodedArgument != null && customAttributeNamedParameter.EncodedArgument.CustomAttributeType.EncodedType != CustomAttributeEncoding.Undefined)
						{
							num++;
						}
					}
				}
				if (num != 0)
				{
					CustomAttributeNamedArgument[] array = new CustomAttributeNamedArgument[num];
					int num2 = 0;
					CustomAttributeNamedParameter[] namedParams = m_namedParams;
					foreach (CustomAttributeNamedParameter customAttributeNamedParameter2 in namedParams)
					{
						if (customAttributeNamedParameter2.EncodedArgument != null && customAttributeNamedParameter2.EncodedArgument.CustomAttributeType.EncodedType != CustomAttributeEncoding.Undefined)
						{
							array[num2++] = new CustomAttributeNamedArgument(customAttributeNamedParameter2.MemberInfo, new CustomAttributeTypedArgument(m_scope, customAttributeNamedParameter2.EncodedArgument));
						}
					}
					m_namedArgs = Array.AsReadOnly(array);
				}
				else
				{
					m_namedArgs = Array.Empty<CustomAttributeNamedArgument>();
				}
			}
			return m_namedArgs;
		}
	}

	internal static IList<CustomAttributeData> GetCustomAttributesInternal(RuntimeType target)
	{
		IList<CustomAttributeData> customAttributes = GetCustomAttributes(target.GetRuntimeModule(), target.MetadataToken);
		RuntimeType.ListBuilder<Attribute> pcas = default(RuntimeType.ListBuilder<Attribute>);
		PseudoCustomAttribute.GetCustomAttributes(target, (RuntimeType)typeof(object), ref pcas);
		if (pcas.Count <= 0)
		{
			return customAttributes;
		}
		return GetCombinedList(customAttributes, ref pcas);
	}

	internal static IList<CustomAttributeData> GetCustomAttributesInternal(RuntimeFieldInfo target)
	{
		IList<CustomAttributeData> customAttributes = GetCustomAttributes(target.GetRuntimeModule(), target.MetadataToken);
		RuntimeType.ListBuilder<Attribute> pcas = default(RuntimeType.ListBuilder<Attribute>);
		PseudoCustomAttribute.GetCustomAttributes(target, (RuntimeType)typeof(object), ref pcas);
		if (pcas.Count <= 0)
		{
			return customAttributes;
		}
		return GetCombinedList(customAttributes, ref pcas);
	}

	internal static IList<CustomAttributeData> GetCustomAttributesInternal(RuntimeMethodInfo target)
	{
		IList<CustomAttributeData> customAttributes = GetCustomAttributes(target.GetRuntimeModule(), target.MetadataToken);
		RuntimeType.ListBuilder<Attribute> pcas = default(RuntimeType.ListBuilder<Attribute>);
		PseudoCustomAttribute.GetCustomAttributes(target, (RuntimeType)typeof(object), ref pcas);
		if (pcas.Count <= 0)
		{
			return customAttributes;
		}
		return GetCombinedList(customAttributes, ref pcas);
	}

	internal static IList<CustomAttributeData> GetCustomAttributesInternal(RuntimeConstructorInfo target)
	{
		return GetCustomAttributes(target.GetRuntimeModule(), target.MetadataToken);
	}

	internal static IList<CustomAttributeData> GetCustomAttributesInternal(RuntimeEventInfo target)
	{
		return GetCustomAttributes(target.GetRuntimeModule(), target.MetadataToken);
	}

	internal static IList<CustomAttributeData> GetCustomAttributesInternal(RuntimePropertyInfo target)
	{
		return GetCustomAttributes(target.GetRuntimeModule(), target.MetadataToken);
	}

	internal static IList<CustomAttributeData> GetCustomAttributesInternal(RuntimeModule target)
	{
		if (target.IsResource())
		{
			return new List<CustomAttributeData>();
		}
		return GetCustomAttributes(target, target.MetadataToken);
	}

	internal static IList<CustomAttributeData> GetCustomAttributesInternal(RuntimeAssembly target)
	{
		return GetCustomAttributes((RuntimeModule)target.ManifestModule, RuntimeAssembly.GetToken(target));
	}

	internal static IList<CustomAttributeData> GetCustomAttributesInternal(RuntimeParameterInfo target)
	{
		RuntimeType.ListBuilder<Attribute> pcas = default(RuntimeType.ListBuilder<Attribute>);
		IList<CustomAttributeData> customAttributes = GetCustomAttributes(target.GetRuntimeModule(), target.MetadataToken);
		PseudoCustomAttribute.GetCustomAttributes(target, (RuntimeType)typeof(object), ref pcas);
		if (pcas.Count <= 0)
		{
			return customAttributes;
		}
		return GetCombinedList(customAttributes, ref pcas);
	}

	private static ReadOnlyCollection<CustomAttributeData> GetCombinedList(IList<CustomAttributeData> customAttributes, ref RuntimeType.ListBuilder<Attribute> pseudoAttributes)
	{
		CustomAttributeData[] array = new CustomAttributeData[customAttributes.Count + pseudoAttributes.Count];
		customAttributes.CopyTo(array, pseudoAttributes.Count);
		for (int i = 0; i < pseudoAttributes.Count; i++)
		{
			array[i] = new RuntimeCustomAttributeData(pseudoAttributes[i]);
		}
		return Array.AsReadOnly(array);
	}

	internal static CustomAttributeEncoding TypeToCustomAttributeEncoding(RuntimeType type)
	{
		if (type == typeof(int))
		{
			return CustomAttributeEncoding.Int32;
		}
		if (type.IsActualEnum)
		{
			return CustomAttributeEncoding.Enum;
		}
		if (type == typeof(string))
		{
			return CustomAttributeEncoding.String;
		}
		if (type == typeof(Type))
		{
			return CustomAttributeEncoding.Type;
		}
		if (type == typeof(object))
		{
			return CustomAttributeEncoding.Object;
		}
		if (type.IsArray)
		{
			return CustomAttributeEncoding.Array;
		}
		if (type == typeof(char))
		{
			return CustomAttributeEncoding.Char;
		}
		if (type == typeof(bool))
		{
			return CustomAttributeEncoding.Boolean;
		}
		if (type == typeof(byte))
		{
			return CustomAttributeEncoding.Byte;
		}
		if (type == typeof(sbyte))
		{
			return CustomAttributeEncoding.SByte;
		}
		if (type == typeof(short))
		{
			return CustomAttributeEncoding.Int16;
		}
		if (type == typeof(ushort))
		{
			return CustomAttributeEncoding.UInt16;
		}
		if (type == typeof(uint))
		{
			return CustomAttributeEncoding.UInt32;
		}
		if (type == typeof(long))
		{
			return CustomAttributeEncoding.Int64;
		}
		if (type == typeof(ulong))
		{
			return CustomAttributeEncoding.UInt64;
		}
		if (type == typeof(float))
		{
			return CustomAttributeEncoding.Float;
		}
		if (type == typeof(double))
		{
			return CustomAttributeEncoding.Double;
		}
		if (type == typeof(Enum))
		{
			return CustomAttributeEncoding.Object;
		}
		if (type.IsClass)
		{
			return CustomAttributeEncoding.Object;
		}
		if (type.IsActualInterface)
		{
			return CustomAttributeEncoding.Object;
		}
		if (type.IsActualValueType)
		{
			return CustomAttributeEncoding.Undefined;
		}
		throw new ArgumentException(SR.Argument_InvalidKindOfTypeForCA, "type");
	}

	private static IList<CustomAttributeData> GetCustomAttributes(RuntimeModule module, int tkTarget)
	{
		CustomAttributeRecord[] customAttributeRecords = GetCustomAttributeRecords(module, tkTarget);
		if (customAttributeRecords.Length == 0)
		{
			return Array.Empty<CustomAttributeData>();
		}
		CustomAttributeData[] array = new CustomAttributeData[customAttributeRecords.Length];
		for (int i = 0; i < customAttributeRecords.Length; i++)
		{
			array[i] = new RuntimeCustomAttributeData(module, customAttributeRecords[i].tkCtor, in customAttributeRecords[i].blob);
		}
		return Array.AsReadOnly(array);
	}

	internal static CustomAttributeRecord[] GetCustomAttributeRecords(RuntimeModule module, int targetToken)
	{
		MetadataImport metadataImport = module.MetadataImport;
		metadataImport.EnumCustomAttributes(targetToken, out var result);
		if (result.Length == 0)
		{
			return Array.Empty<CustomAttributeRecord>();
		}
		CustomAttributeRecord[] array = new CustomAttributeRecord[result.Length];
		for (int i = 0; i < array.Length; i++)
		{
			metadataImport.GetCustomAttributeProps(result[i], out array[i].tkCtor.Value, out array[i].blob);
		}
		GC.KeepAlive(module);
		return array;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2075:UnrecognizedReflectionPattern", Justification = "Property setters and fields which are accessed by any attribute instantiation which is present in the code linker has analyzed.As such enumerating all fields and properties may return different results after trimmingbut all those which are needed to actually have data will be there.")]
	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern", Justification = "We're getting a MethodBase of a constructor that we found in the metadata. The attribute constructor won't be trimmed.")]
	private RuntimeCustomAttributeData(RuntimeModule scope, MetadataToken caCtorToken, in ConstArray blob)
	{
		m_scope = scope;
		m_ctor = (RuntimeConstructorInfo)RuntimeType.GetMethodBase(m_scope, caCtorToken);
		if (m_ctor.DeclaringType.IsGenericType)
		{
			MetadataImport metadataImport = m_scope.MetadataImport;
			Type type = m_scope.ResolveType(metadataImport.GetParentToken(caCtorToken), null, null);
			m_ctor = (RuntimeConstructorInfo)m_scope.ResolveMethod(caCtorToken, type.GenericTypeArguments, null).MethodHandle.GetMethodInfo();
		}
		ReadOnlySpan<ParameterInfo> parametersAsSpan = m_ctor.GetParametersAsSpan();
		if (parametersAsSpan.Length != 0)
		{
			m_ctorParams = new CustomAttributeCtorParameter[parametersAsSpan.Length];
			for (int i = 0; i < parametersAsSpan.Length; i++)
			{
				m_ctorParams[i] = new CustomAttributeCtorParameter(new CustomAttributeType((RuntimeType)parametersAsSpan[i].ParameterType));
			}
		}
		else
		{
			m_ctorParams = Array.Empty<CustomAttributeCtorParameter>();
		}
		FieldInfo[] fields = m_ctor.DeclaringType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		PropertyInfo[] properties = m_ctor.DeclaringType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		m_namedParams = new CustomAttributeNamedParameter[properties.Length + fields.Length];
		int num = 0;
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			m_namedParams[num++] = new CustomAttributeNamedParameter(fieldInfo, CustomAttributeEncoding.Field, new CustomAttributeType((RuntimeType)fieldInfo.FieldType));
		}
		PropertyInfo[] array2 = properties;
		foreach (PropertyInfo propertyInfo in array2)
		{
			m_namedParams[num++] = new CustomAttributeNamedParameter(propertyInfo, CustomAttributeEncoding.Property, new CustomAttributeType((RuntimeType)propertyInfo.PropertyType));
		}
		CustomAttributeEncodedArgument.ParseAttributeArguments(blob, m_ctorParams, m_namedParams, m_scope);
	}

	internal RuntimeCustomAttributeData(Attribute attribute)
	{
		if (attribute is DllImportAttribute dllImport)
		{
			Init(dllImport);
		}
		else if (attribute is FieldOffsetAttribute fieldOffset)
		{
			Init(fieldOffset);
		}
		else if (attribute is MarshalAsAttribute marshalAs)
		{
			Init(marshalAs);
		}
		else if (attribute is TypeForwardedToAttribute forwardedTo)
		{
			Init(forwardedTo);
		}
		else
		{
			Init(attribute);
		}
	}

	private void Init(DllImportAttribute dllImport)
	{
		Type typeFromHandle = typeof(DllImportAttribute);
		m_ctor = typeFromHandle.GetConstructors(BindingFlags.Instance | BindingFlags.Public)[0];
		m_typedCtorArgs = Array.AsReadOnly(new CustomAttributeTypedArgument[1]
		{
			new CustomAttributeTypedArgument(dllImport.Value)
		});
		m_namedArgs = Array.AsReadOnly(new CustomAttributeNamedArgument[8]
		{
			new CustomAttributeNamedArgument(typeFromHandle.GetField("EntryPoint"), dllImport.EntryPoint),
			new CustomAttributeNamedArgument(typeFromHandle.GetField("CharSet"), dllImport.CharSet),
			new CustomAttributeNamedArgument(typeFromHandle.GetField("ExactSpelling"), dllImport.ExactSpelling),
			new CustomAttributeNamedArgument(typeFromHandle.GetField("SetLastError"), dllImport.SetLastError),
			new CustomAttributeNamedArgument(typeFromHandle.GetField("PreserveSig"), dllImport.PreserveSig),
			new CustomAttributeNamedArgument(typeFromHandle.GetField("CallingConvention"), dllImport.CallingConvention),
			new CustomAttributeNamedArgument(typeFromHandle.GetField("BestFitMapping"), dllImport.BestFitMapping),
			new CustomAttributeNamedArgument(typeFromHandle.GetField("ThrowOnUnmappableChar"), dllImport.ThrowOnUnmappableChar)
		});
	}

	private void Init(FieldOffsetAttribute fieldOffset)
	{
		m_ctor = typeof(FieldOffsetAttribute).GetConstructors(BindingFlags.Instance | BindingFlags.Public)[0];
		m_typedCtorArgs = Array.AsReadOnly(new CustomAttributeTypedArgument[1]
		{
			new CustomAttributeTypedArgument(fieldOffset.Value)
		});
		m_namedArgs = Array.Empty<CustomAttributeNamedArgument>();
	}

	private void Init(MarshalAsAttribute marshalAs)
	{
		Type typeFromHandle = typeof(MarshalAsAttribute);
		m_ctor = typeFromHandle.GetConstructors(BindingFlags.Instance | BindingFlags.Public)[0];
		m_typedCtorArgs = Array.AsReadOnly(new CustomAttributeTypedArgument[1]
		{
			new CustomAttributeTypedArgument(marshalAs.Value)
		});
		int num = 3;
		if (marshalAs.MarshalType != null)
		{
			num++;
		}
		if ((object)marshalAs.MarshalTypeRef != null)
		{
			num++;
		}
		if (marshalAs.MarshalCookie != null)
		{
			num++;
		}
		num++;
		num++;
		if ((object)marshalAs.SafeArrayUserDefinedSubType != null)
		{
			num++;
		}
		CustomAttributeNamedArgument[] array = new CustomAttributeNamedArgument[num];
		num = 0;
		array[num++] = new CustomAttributeNamedArgument(typeFromHandle.GetField("ArraySubType"), marshalAs.ArraySubType);
		array[num++] = new CustomAttributeNamedArgument(typeFromHandle.GetField("SizeParamIndex"), marshalAs.SizeParamIndex);
		array[num++] = new CustomAttributeNamedArgument(typeFromHandle.GetField("SizeConst"), marshalAs.SizeConst);
		array[num++] = new CustomAttributeNamedArgument(typeFromHandle.GetField("IidParameterIndex"), marshalAs.IidParameterIndex);
		array[num++] = new CustomAttributeNamedArgument(typeFromHandle.GetField("SafeArraySubType"), marshalAs.SafeArraySubType);
		if (marshalAs.MarshalType != null)
		{
			array[num++] = new CustomAttributeNamedArgument(typeFromHandle.GetField("MarshalType"), marshalAs.MarshalType);
		}
		if ((object)marshalAs.MarshalTypeRef != null)
		{
			array[num++] = new CustomAttributeNamedArgument(typeFromHandle.GetField("MarshalTypeRef"), marshalAs.MarshalTypeRef);
		}
		if (marshalAs.MarshalCookie != null)
		{
			array[num++] = new CustomAttributeNamedArgument(typeFromHandle.GetField("MarshalCookie"), marshalAs.MarshalCookie);
		}
		if ((object)marshalAs.SafeArrayUserDefinedSubType != null)
		{
			array[num++] = new CustomAttributeNamedArgument(typeFromHandle.GetField("SafeArrayUserDefinedSubType"), marshalAs.SafeArrayUserDefinedSubType);
		}
		m_namedArgs = Array.AsReadOnly(array);
	}

	private void Init(TypeForwardedToAttribute forwardedTo)
	{
		Type typeFromHandle = typeof(TypeForwardedToAttribute);
		Type[] types = new Type[1] { typeof(Type) };
		m_ctor = typeFromHandle.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, types, null);
		CustomAttributeTypedArgument[] array = new CustomAttributeTypedArgument[1]
		{
			new CustomAttributeTypedArgument(typeof(Type), forwardedTo.Destination)
		};
		m_typedCtorArgs = Array.AsReadOnly(array);
		m_namedArgs = Array.Empty<CustomAttributeNamedArgument>();
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2075:UnrecognizedReflectionPattern", Justification = "The pca object had to be created by the single ctor on the Type. So the ctor couldn't have been trimmed.")]
	private void Init(object pca)
	{
		Type type = pca.GetType();
		m_ctor = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)[0];
		m_typedCtorArgs = Array.Empty<CustomAttributeTypedArgument>();
		m_namedArgs = Array.Empty<CustomAttributeNamedArgument>();
	}
}
