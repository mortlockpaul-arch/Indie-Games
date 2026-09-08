using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit;

internal static class MetadataSignatureHelper
{
	internal static BlobBuilder GetLocalSignature(List<LocalBuilder> locals, ModuleBuilderImpl module)
	{
		locals.Sort((LocalBuilder l1, LocalBuilder l2) => l1.LocalIndex - l2.LocalIndex);
		BlobBuilder blobBuilder = new BlobBuilder();
		LocalVariablesEncoder localVariablesEncoder = new BlobEncoder(blobBuilder).LocalVariableSignature(locals.Count);
		foreach (LocalBuilder local in locals)
		{
			WriteSignatureForType(localVariablesEncoder.AddVariable().Type(local.LocalType.IsByRef, local.IsPinned), local.LocalType.IsByRef ? local.LocalType.GetElementType() : local.LocalType, module);
		}
		return blobBuilder;
	}

	internal static BlobBuilder GetFieldSignature(Type fieldType, Type[] requiredCustomModifiers, Type[] optionalCustomModifiers, ModuleBuilderImpl module)
	{
		BlobBuilder blobBuilder = new BlobBuilder();
		WriteSignatureForType(new BlobEncoder(blobBuilder).Field().Type(), fieldType, module, requiredCustomModifiers, optionalCustomModifiers);
		return blobBuilder;
	}

	internal static BlobBuilder GetConstructorSignature(ParameterInfo[] parameters, ModuleBuilderImpl module)
	{
		BlobBuilder blobBuilder = new BlobBuilder();
		if (parameters == null)
		{
			parameters = Array.Empty<ParameterInfo>();
		}
		new BlobEncoder(blobBuilder).MethodSignature(SignatureCallingConvention.Default, 0, isInstanceMethod: true).Parameters(parameters.Length, out var returnType, out var parameters2);
		returnType.Void();
		WriteParametersSignature(module, GetParameterTypes(parameters), parameters2);
		return blobBuilder;
	}

	internal static BlobBuilder GetTypeSpecificationSignature(Type type, ModuleBuilderImpl module)
	{
		BlobBuilder blobBuilder = new BlobBuilder();
		WriteSignatureForType(new BlobEncoder(blobBuilder).TypeSpecificationSignature(), type, module);
		return blobBuilder;
	}

	internal static BlobBuilder GetMethodSpecificationSignature(Type[] genericArguments, ModuleBuilderImpl module)
	{
		BlobBuilder blobBuilder = new BlobBuilder();
		GenericTypeArgumentsEncoder genericTypeArgumentsEncoder = new BlobEncoder(blobBuilder).MethodSpecificationSignature(genericArguments.Length);
		foreach (Type type in genericArguments)
		{
			WriteSignatureForType(genericTypeArgumentsEncoder.AddArgument(), type, module);
		}
		return blobBuilder;
	}

	internal static BlobBuilder GetMethodSignature(ModuleBuilderImpl module, Type[] parameters, Type returnType, SignatureCallingConvention convention, int genParamCount = 0, bool isInstance = false, Type[] optionalParameterTypes = null, Type[] returnTypeRequiredModifiers = null, Type[] returnTypeOptionalModifiers = null, Type[][] parameterRequiredModifiers = null, Type[][] parameterOptionalModifiers = null)
	{
		BlobBuilder blobBuilder = new BlobBuilder();
		int parameterCount = ((parameters != null) ? parameters.Length : 0) + ((optionalParameterTypes != null) ? optionalParameterTypes.Length : 0);
		new BlobEncoder(blobBuilder).MethodSignature(convention, genParamCount, isInstance).Parameters(parameterCount, out var returnType2, out var parameters2);
		if ((object)returnType == null)
		{
			returnType = module.GetTypeFromCoreAssembly(CoreTypeId.Void);
		}
		WriteSignatureForType(returnType2.Type(), returnType, module, returnTypeRequiredModifiers, returnTypeOptionalModifiers);
		WriteParametersSignature(module, parameters, parameters2, parameterRequiredModifiers, parameterOptionalModifiers);
		if (optionalParameterTypes != null && optionalParameterTypes.Length != 0)
		{
			WriteParametersSignature(module, optionalParameterTypes, parameters2.StartVarArgs());
		}
		return blobBuilder;
	}

	internal static Type[] GetParameterTypes(ParameterInfo[] parameterInfos)
	{
		if (parameterInfos.Length == 0)
		{
			return Type.EmptyTypes;
		}
		Type[] array = new Type[parameterInfos.Length];
		for (int i = 0; i < parameterInfos.Length; i++)
		{
			array[i] = parameterInfos[i].GetModifiedParameterType();
		}
		return array;
	}

	private static void WriteCustomModifiers(CustomModifiersEncoder encoder, Type[] customModifiers, bool isOptional, ModuleBuilderImpl module)
	{
		for (int num = customModifiers.Length - 1; num >= 0; num--)
		{
			Type type = customModifiers[num];
			encoder.AddModifier(module.GetTypeHandle(type), isOptional);
		}
	}

	private static void WriteParametersSignature(ModuleBuilderImpl module, Type[] parameters, ParametersEncoder parameterEncoder, Type[][] requiredModifiers = null, Type[][] optionalModifiers = null)
	{
		if (parameters != null)
		{
			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterTypeEncoder parameterTypeEncoder = parameterEncoder.AddParameter();
				Type[] requiredModifiers2 = ((requiredModifiers != null && requiredModifiers.Length > i) ? requiredModifiers[i] : null);
				Type[] optionalModifiers2 = ((optionalModifiers != null && optionalModifiers.Length > i) ? optionalModifiers[i] : null);
				WriteSignatureForType(parameterTypeEncoder.Type(), parameters[i], module, requiredModifiers2, optionalModifiers2);
			}
		}
	}

	internal static BlobBuilder GetPropertySignature(PropertyBuilderImpl property, ModuleBuilderImpl module)
	{
		BlobBuilder blobBuilder = new BlobBuilder();
		new BlobEncoder(blobBuilder).PropertySignature(property.CallingConventions.HasFlag(CallingConventions.HasThis)).Parameters((property.ParameterTypes != null) ? property.ParameterTypes.Length : 0, out var returnType, out var parameters);
		WriteSignatureForType(returnType.Type(), property.PropertyType, module, property._returnTypeRequiredCustomModifiers, property._returnTypeOptionalCustomModifiers);
		WriteParametersSignature(module, property.ParameterTypes, parameters, property._parameterTypeRequiredCustomModifiers, property._parameterTypeOptionalCustomModifiers);
		return blobBuilder;
	}

	private static void WriteSignatureForType(SignatureTypeEncoder signature, Type type, ModuleBuilderImpl module, Type[] requiredModifiers = null, Type[] optionalModifiers = null)
	{
		WriteCustomModifiers(signature.CustomModifiers(), requiredModifiers ?? type.GetRequiredCustomModifiers(), isOptional: false, module);
		WriteCustomModifiers(signature.CustomModifiers(), optionalModifiers ?? type.GetOptionalCustomModifiers(), isOptional: true, module);
		if (type.IsArray)
		{
			Type elementType = type.GetElementType();
			if (type.GetArrayRank() == 1)
			{
				WriteSignatureForType(signature.SZArray(), elementType, module);
				return;
			}
			signature.Array(out var elementType2, out var arrayShape);
			WriteSignatureForType(elementType2, elementType, module);
			arrayShape.Shape(type.GetArrayRank(), ImmutableArray<int>.Empty, default(ImmutableArray<int>));
		}
		else if (type.IsPointer)
		{
			WriteSignatureForType(signature.Pointer(), type.GetElementType(), module);
		}
		else if (type.IsByRef)
		{
			signature.Builder.WriteByte(16);
			WriteSignatureForType(signature, type.GetElementType(), module);
		}
		else if (type.IsGenericType)
		{
			Type[] genericArguments = type.GetGenericArguments();
			GenericTypeArgumentsEncoder genericTypeArgumentsEncoder = signature.GenericInstantiation(module.GetTypeHandle(type.GetGenericTypeDefinition()), genericArguments.Length, type.IsValueType);
			Type[] array = genericArguments;
			foreach (Type type2 in array)
			{
				if (type2.IsGenericMethodParameter)
				{
					genericTypeArgumentsEncoder.AddArgument().GenericMethodTypeParameter(type2.GenericParameterPosition);
				}
				else if (type2.IsGenericParameter)
				{
					genericTypeArgumentsEncoder.AddArgument().GenericTypeParameter(type2.GenericParameterPosition);
				}
				else
				{
					WriteSignatureForType(genericTypeArgumentsEncoder.AddArgument(), type2, module);
				}
			}
		}
		else if (type.IsGenericMethodParameter)
		{
			signature.GenericMethodTypeParameter(type.GenericParameterPosition);
		}
		else if (type.IsGenericParameter)
		{
			signature.GenericTypeParameter(type.GenericParameterPosition);
		}
		else if (type.IsFunctionPointer)
		{
			WriteSignatureForFunctionPointerType(signature, type, module);
		}
		else
		{
			WriteSimpleSignature(signature, type, module);
		}
	}

	private static void WriteSignatureForFunctionPointerType(SignatureTypeEncoder signature, Type type, ModuleBuilderImpl module)
	{
		SignatureCallingConvention convention = SignatureCallingConvention.Default;
		FunctionPointerAttributes attributes = FunctionPointerAttributes.None;
		Type functionPointerReturnType = type.GetFunctionPointerReturnType();
		Type[] functionPointerParameterTypes = type.GetFunctionPointerParameterTypes();
		if (type.IsUnmanagedFunctionPointer)
		{
			convention = SignatureCallingConvention.Unmanaged;
			Type[] functionPointerCallingConventions = type.GetFunctionPointerCallingConventions();
			if (functionPointerCallingConventions != null && functionPointerCallingConventions.Length == 1)
			{
				switch (functionPointerCallingConventions[0].FullName)
				{
				case "System.Runtime.CompilerServices.CallConvCdecl":
					convention = SignatureCallingConvention.CDecl;
					break;
				case "System.Runtime.CompilerServices.CallConvStdcall":
					convention = SignatureCallingConvention.StdCall;
					break;
				case "System.Runtime.CompilerServices.CallConvThiscall":
					convention = SignatureCallingConvention.ThisCall;
					break;
				case "System.Runtime.CompilerServices.CallConvFastcall":
					convention = SignatureCallingConvention.FastCall;
					break;
				}
			}
		}
		signature.FunctionPointer(convention, attributes).Parameters(functionPointerParameterTypes.Length, out var returnType, out var parameters);
		WriteSignatureForType(returnType.Type(), functionPointerReturnType, module);
		Type[] array = functionPointerParameterTypes;
		foreach (Type type2 in array)
		{
			WriteSignatureForType(parameters.AddParameter().Type(), type2, module);
		}
	}

	private static void WriteSimpleSignature(SignatureTypeEncoder signature, Type type, ModuleBuilderImpl module)
	{
		type = type.UnderlyingSystemType;
		switch (module.GetTypeIdFromCoreTypes(type))
		{
		case CoreTypeId.Void:
			signature.Builder.WriteByte(1);
			break;
		case CoreTypeId.Boolean:
			signature.Boolean();
			break;
		case CoreTypeId.Byte:
			signature.Byte();
			break;
		case CoreTypeId.SByte:
			signature.SByte();
			break;
		case CoreTypeId.Char:
			signature.Char();
			break;
		case CoreTypeId.Int16:
			signature.Int16();
			break;
		case CoreTypeId.UInt16:
			signature.UInt16();
			break;
		case CoreTypeId.Int32:
			signature.Int32();
			break;
		case CoreTypeId.UInt32:
			signature.UInt32();
			break;
		case CoreTypeId.Int64:
			signature.Int64();
			break;
		case CoreTypeId.UInt64:
			signature.UInt64();
			break;
		case CoreTypeId.Single:
			signature.Single();
			break;
		case CoreTypeId.Double:
			signature.Double();
			break;
		case CoreTypeId.IntPtr:
			signature.IntPtr();
			break;
		case CoreTypeId.UIntPtr:
			signature.UIntPtr();
			break;
		case CoreTypeId.Object:
			signature.Object();
			break;
		case CoreTypeId.String:
			signature.String();
			break;
		case CoreTypeId.TypedReference:
			signature.TypedReference();
			break;
		default:
		{
			EntityHandle typeHandle = module.GetTypeHandle(type);
			signature.Type(typeHandle, type.IsValueType);
			break;
		}
		}
	}
}
