using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Reflection;

internal static class PseudoCustomAttribute
{
	private static readonly HashSet<RuntimeType> s_pca = CreatePseudoCustomAttributeHashSet();

	private static HashSet<RuntimeType> CreatePseudoCustomAttributeHashSet()
	{
		Type[] obj = new Type[11]
		{
			typeof(FieldOffsetAttribute),
			typeof(SerializableAttribute),
			typeof(MarshalAsAttribute),
			typeof(ComImportAttribute),
			typeof(NonSerializedAttribute),
			typeof(InAttribute),
			typeof(OutAttribute),
			typeof(OptionalAttribute),
			typeof(DllImportAttribute),
			typeof(PreserveSigAttribute),
			typeof(TypeForwardedToAttribute)
		};
		HashSet<RuntimeType> hashSet = new HashSet<RuntimeType>(obj.Length);
		Type[] array = obj;
		for (int i = 0; i < array.Length; i++)
		{
			RuntimeType item = (RuntimeType)array[i];
			hashSet.Add(item);
		}
		return hashSet;
	}

	internal static void GetCustomAttributes(RuntimeType type, RuntimeType caType, ref RuntimeType.ListBuilder<Attribute> pcas)
	{
		bool flag = caType == typeof(object) || caType == typeof(Attribute);
		if (flag || s_pca.Contains(caType))
		{
			if ((flag || caType == typeof(SerializableAttribute)) && (type.Attributes & TypeAttributes.Serializable) != TypeAttributes.NotPublic)
			{
				pcas.Add(new SerializableAttribute());
			}
			if ((flag || caType == typeof(ComImportAttribute)) && (type.Attributes & TypeAttributes.Import) != TypeAttributes.NotPublic)
			{
				pcas.Add(new ComImportAttribute());
			}
		}
	}

	internal static bool IsDefined(RuntimeType type, RuntimeType caType)
	{
		bool flag = caType == typeof(object) || caType == typeof(Attribute);
		if (!flag && !s_pca.Contains(caType))
		{
			return false;
		}
		if ((flag || caType == typeof(SerializableAttribute)) && (type.Attributes & TypeAttributes.Serializable) != TypeAttributes.NotPublic)
		{
			return true;
		}
		if ((flag || caType == typeof(ComImportAttribute)) && (type.Attributes & TypeAttributes.Import) != TypeAttributes.NotPublic)
		{
			return true;
		}
		return false;
	}

	internal static void GetCustomAttributes(RuntimeMethodInfo method, RuntimeType caType, ref RuntimeType.ListBuilder<Attribute> pcas)
	{
		bool flag = caType == typeof(object) || caType == typeof(Attribute);
		if (!flag && !s_pca.Contains(caType))
		{
			return;
		}
		if (flag || caType == typeof(DllImportAttribute))
		{
			Attribute dllImportCustomAttribute = GetDllImportCustomAttribute(method);
			if (dllImportCustomAttribute != null)
			{
				pcas.Add(dllImportCustomAttribute);
			}
		}
		if ((flag || caType == typeof(PreserveSigAttribute)) && (method.GetMethodImplementationFlags() & MethodImplAttributes.PreserveSig) != MethodImplAttributes.IL)
		{
			pcas.Add(new PreserveSigAttribute());
		}
	}

	internal static bool IsDefined(RuntimeMethodInfo method, RuntimeType caType)
	{
		bool flag = caType == typeof(object) || caType == typeof(Attribute);
		if (!flag && !s_pca.Contains(caType))
		{
			return false;
		}
		if ((flag || caType == typeof(DllImportAttribute)) && (method.Attributes & MethodAttributes.PinvokeImpl) != MethodAttributes.PrivateScope)
		{
			return true;
		}
		if ((flag || caType == typeof(PreserveSigAttribute)) && (method.GetMethodImplementationFlags() & MethodImplAttributes.PreserveSig) != MethodImplAttributes.IL)
		{
			return true;
		}
		return false;
	}

	internal static void GetCustomAttributes(RuntimeParameterInfo parameter, RuntimeType caType, ref RuntimeType.ListBuilder<Attribute> pcas)
	{
		bool flag = caType == typeof(object) || caType == typeof(Attribute);
		if (!flag && !s_pca.Contains(caType))
		{
			return;
		}
		if ((flag || caType == typeof(InAttribute)) && parameter.IsIn)
		{
			pcas.Add(new InAttribute());
		}
		if ((flag || caType == typeof(OutAttribute)) && parameter.IsOut)
		{
			pcas.Add(new OutAttribute());
		}
		if ((flag || caType == typeof(OptionalAttribute)) && parameter.IsOptional)
		{
			pcas.Add(new OptionalAttribute());
		}
		if (flag || caType == typeof(MarshalAsAttribute))
		{
			Attribute marshalAsCustomAttribute = GetMarshalAsCustomAttribute(parameter);
			if (marshalAsCustomAttribute != null)
			{
				pcas.Add(marshalAsCustomAttribute);
			}
		}
	}

	internal static bool IsDefined(RuntimeParameterInfo parameter, RuntimeType caType)
	{
		bool flag = caType == typeof(object) || caType == typeof(Attribute);
		if (!flag && !s_pca.Contains(caType))
		{
			return false;
		}
		if ((flag || caType == typeof(InAttribute)) && parameter.IsIn)
		{
			return true;
		}
		if ((flag || caType == typeof(OutAttribute)) && parameter.IsOut)
		{
			return true;
		}
		if ((flag || caType == typeof(OptionalAttribute)) && parameter.IsOptional)
		{
			return true;
		}
		if ((flag || caType == typeof(MarshalAsAttribute)) && GetMarshalAsCustomAttribute(parameter) != null)
		{
			return true;
		}
		return false;
	}

	internal static void GetCustomAttributes(RuntimeFieldInfo field, RuntimeType caType, ref RuntimeType.ListBuilder<Attribute> pcas)
	{
		bool flag = caType == typeof(object) || caType == typeof(Attribute);
		if (!flag && !s_pca.Contains(caType))
		{
			return;
		}
		if (flag || caType == typeof(MarshalAsAttribute))
		{
			Attribute marshalAsCustomAttribute = GetMarshalAsCustomAttribute(field);
			if (marshalAsCustomAttribute != null)
			{
				pcas.Add(marshalAsCustomAttribute);
			}
		}
		if (flag || caType == typeof(FieldOffsetAttribute))
		{
			Attribute marshalAsCustomAttribute = GetFieldOffsetCustomAttribute(field);
			if (marshalAsCustomAttribute != null)
			{
				pcas.Add(marshalAsCustomAttribute);
			}
		}
		if ((flag || caType == typeof(NonSerializedAttribute)) && (field.Attributes & FieldAttributes.NotSerialized) != FieldAttributes.PrivateScope)
		{
			pcas.Add(new NonSerializedAttribute());
		}
	}

	internal static bool IsDefined(RuntimeFieldInfo field, RuntimeType caType)
	{
		bool flag = caType == typeof(object) || caType == typeof(Attribute);
		if (!flag && !s_pca.Contains(caType))
		{
			return false;
		}
		if ((flag || caType == typeof(MarshalAsAttribute)) && GetMarshalAsCustomAttribute(field) != null)
		{
			return true;
		}
		if ((flag || caType == typeof(FieldOffsetAttribute)) && GetFieldOffsetCustomAttribute(field) != null)
		{
			return true;
		}
		if ((flag || caType == typeof(NonSerializedAttribute)) && (field.Attributes & FieldAttributes.NotSerialized) != FieldAttributes.PrivateScope)
		{
			return true;
		}
		return false;
	}

	private static DllImportAttribute GetDllImportCustomAttribute(RuntimeMethodInfo method)
	{
		if ((method.Attributes & MethodAttributes.PinvokeImpl) == 0)
		{
			return null;
		}
		RuntimeModule runtimeModule = method.Module.ModuleHandle.GetRuntimeModule();
		MetadataImport metadataImport = runtimeModule.MetadataImport;
		int metadataToken = method.MetadataToken;
		metadataImport.GetPInvokeMap(metadataToken, out var attributes, out var importName, out var importDll);
		GC.KeepAlive(runtimeModule);
		CharSet charSet = CharSet.None;
		switch (attributes & PInvokeAttributes.CharSetMask)
		{
		case PInvokeAttributes.CharSetNotSpec:
			charSet = CharSet.None;
			break;
		case PInvokeAttributes.CharSetAnsi:
			charSet = CharSet.Ansi;
			break;
		case PInvokeAttributes.CharSetUnicode:
			charSet = CharSet.Unicode;
			break;
		case PInvokeAttributes.CharSetMask:
			charSet = CharSet.Auto;
			break;
		}
		CallingConvention callingConvention = CallingConvention.Cdecl;
		switch (attributes & PInvokeAttributes.CallConvMask)
		{
		case PInvokeAttributes.CallConvWinapi:
			callingConvention = CallingConvention.Winapi;
			break;
		case PInvokeAttributes.CallConvCdecl:
			callingConvention = CallingConvention.Cdecl;
			break;
		case PInvokeAttributes.CallConvStdcall:
			callingConvention = CallingConvention.StdCall;
			break;
		case PInvokeAttributes.CallConvThiscall:
			callingConvention = CallingConvention.ThisCall;
			break;
		case PInvokeAttributes.CallConvFastcall:
			callingConvention = CallingConvention.FastCall;
			break;
		}
		return new DllImportAttribute(importDll)
		{
			EntryPoint = importName,
			CharSet = charSet,
			SetLastError = ((attributes & PInvokeAttributes.SupportsLastError) != 0),
			ExactSpelling = ((attributes & PInvokeAttributes.NoMangle) != 0),
			PreserveSig = ((method.GetMethodImplementationFlags() & MethodImplAttributes.PreserveSig) != 0),
			CallingConvention = callingConvention,
			BestFitMapping = ((attributes & PInvokeAttributes.BestFitMask) == PInvokeAttributes.BestFitEnabled),
			ThrowOnUnmappableChar = ((attributes & PInvokeAttributes.ThrowOnUnmappableCharMask) == PInvokeAttributes.ThrowOnUnmappableCharEnabled)
		};
	}

	private static MarshalAsAttribute GetMarshalAsCustomAttribute(RuntimeParameterInfo parameter)
	{
		return GetMarshalAsCustomAttribute(parameter.MetadataToken, parameter.GetRuntimeModule());
	}

	private static MarshalAsAttribute GetMarshalAsCustomAttribute(RuntimeFieldInfo field)
	{
		return GetMarshalAsCustomAttribute(field.MetadataToken, field.GetRuntimeModule());
	}

	private static MarshalAsAttribute GetMarshalAsCustomAttribute(int token, RuntimeModule scope)
	{
		ConstArray fieldMarshal = scope.MetadataImport.GetFieldMarshal(token);
		if (fieldMarshal.Length == 0)
		{
			return null;
		}
		return MetadataImport.GetMarshalAs(fieldMarshal, scope);
	}

	private static FieldOffsetAttribute GetFieldOffsetCustomAttribute(RuntimeFieldInfo field)
	{
		if ((object)field.DeclaringType != null)
		{
			RuntimeModule runtimeModule = field.GetRuntimeModule();
			if (runtimeModule.MetadataImport.GetFieldOffset(field.DeclaringType.MetadataToken, field.MetadataToken, out var offset))
			{
				return new FieldOffsetAttribute(offset);
			}
			GC.KeepAlive(runtimeModule);
		}
		return null;
	}

	internal static StructLayoutAttribute GetStructLayoutCustomAttribute(RuntimeType type)
	{
		if (type.IsActualInterface || type.HasElementType || type.IsGenericParameter)
		{
			return null;
		}
		LayoutKind layoutKind = LayoutKind.Auto;
		switch (type.Attributes & TypeAttributes.LayoutMask)
		{
		case TypeAttributes.ExplicitLayout:
			layoutKind = LayoutKind.Explicit;
			break;
		case TypeAttributes.NotPublic:
			layoutKind = LayoutKind.Auto;
			break;
		case TypeAttributes.SequentialLayout:
			layoutKind = LayoutKind.Sequential;
			break;
		}
		CharSet charSet = CharSet.None;
		switch (type.Attributes & TypeAttributes.StringFormatMask)
		{
		case TypeAttributes.NotPublic:
			charSet = CharSet.Ansi;
			break;
		case TypeAttributes.AutoClass:
			charSet = CharSet.Auto;
			break;
		case TypeAttributes.UnicodeClass:
			charSet = CharSet.Unicode;
			break;
		}
		RuntimeModule runtimeModule = type.GetRuntimeModule();
		runtimeModule.MetadataImport.GetClassLayout(type.MetadataToken, out var packSize, out var classSize);
		GC.KeepAlive(runtimeModule);
		return new StructLayoutAttribute(layoutKind)
		{
			Pack = packSize,
			Size = classSize,
			CharSet = charSet
		};
	}
}
