using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit;

internal sealed class MethodBuilderImpl : MethodBuilder
{
	private Type _returnType;

	internal Type[] _parameterTypes;

	private readonly ModuleBuilderImpl _module;

	private readonly string _name;

	private readonly CallingConventions _callingConventions;

	private readonly TypeBuilderImpl _declaringType;

	private MethodAttributes _attributes;

	private MethodImplAttributes _methodImplFlags;

	private GenericTypeParameterBuilderImpl[] _typeParameters;

	private ILGeneratorImpl _ilGenerator;

	private bool _initLocals;

	internal Type[] _returnTypeRequiredModifiers;

	internal Type[] _returnTypeOptionalCustomModifiers;

	internal Type[][] _parameterTypeRequiredCustomModifiers;

	internal Type[][] _parameterTypeOptionalCustomModifiers;

	internal DllImportData _dllImportData;

	internal List<CustomAttributeWrapper> _customAttributes;

	internal ParameterBuilderImpl[] _parameterBuilders;

	internal MethodDefinitionHandle _handle;

	internal int ParameterCount
	{
		get
		{
			if (_parameterTypes != null)
			{
				return _parameterTypes.Length;
			}
			return 0;
		}
	}

	internal Type[] ParameterTypes => _parameterTypes;

	internal ILGeneratorImpl ILGeneratorImpl => _ilGenerator;

	public new bool IsConstructor
	{
		get
		{
			if ((_attributes & (MethodAttributes.SpecialName | MethodAttributes.RTSpecialName)) != (MethodAttributes.SpecialName | MethodAttributes.RTSpecialName))
			{
				return false;
			}
			if (!_name.Equals(ConstructorInfo.ConstructorName))
			{
				return _name.Equals(ConstructorInfo.TypeConstructorName);
			}
			return true;
		}
	}

	protected override bool InitLocalsCore
	{
		get
		{
			ThrowIfGeneric();
			return _initLocals;
		}
		set
		{
			ThrowIfGeneric();
			_initLocals = value;
		}
	}

	public override string Name => _name;

	public override MethodAttributes Attributes => _attributes;

	public override CallingConventions CallingConvention => _callingConventions;

	public override Type DeclaringType
	{
		get
		{
			if (!_declaringType._isHiddenGlobalType)
			{
				return _declaringType;
			}
			return null;
		}
	}

	public override Module Module => _module;

	public override bool ContainsGenericParameters => _typeParameters != null;

	public override bool IsGenericMethod => _typeParameters != null;

	public override bool IsGenericMethodDefinition => _typeParameters != null;

	public override bool IsSecurityCritical => true;

	public override bool IsSecuritySafeCritical => false;

	public override bool IsSecurityTransparent => false;

	public override int MetadataToken
	{
		get
		{
			if (!(_handle == default(MethodDefinitionHandle)))
			{
				return MetadataTokens.GetToken(_handle);
			}
			return 0;
		}
	}

	public override RuntimeMethodHandle MethodHandle
	{
		get
		{
			throw new NotSupportedException(System.SR.NotSupported_DynamicModule);
		}
	}

	public override Type ReflectedType => DeclaringType;

	public override ParameterInfo ReturnParameter
	{
		get
		{
			if (_parameterBuilders == null || _parameterBuilders[0] == null)
			{
				return new ParameterInfoWrapper(new ParameterBuilderImpl(this, 0, ParameterAttributes.Retval, null), _returnType);
			}
			return new ParameterInfoWrapper(_parameterBuilders[0], _returnType);
		}
	}

	public override Type ReturnType => _returnType;

	public override ICustomAttributeProvider ReturnTypeCustomAttributes
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal MethodBuilderImpl(string name, MethodAttributes attributes, CallingConventions callingConventions, Type returnType, Type[] returnTypeRequiredCustomModifiers, Type[] returnTypeOptionalCustomModifiers, Type[] parameterTypes, Type[][] parameterTypeRequiredCustomModifiers, Type[][] parameterTypeOptionalCustomModifiers, ModuleBuilderImpl module, TypeBuilderImpl declaringType)
	{
		_module = module;
		_returnType = returnType ?? _module.GetTypeFromCoreAssembly(CoreTypeId.Void);
		_name = name;
		_attributes = attributes;
		if ((attributes & MethodAttributes.Static) == 0)
		{
			callingConventions |= CallingConventions.HasThis;
		}
		else if ((attributes & (MethodAttributes.Virtual | MethodAttributes.Abstract)) == MethodAttributes.Virtual)
		{
			throw new ArgumentException(System.SR.Argument_NoStaticVirtual);
		}
		_callingConventions = callingConventions;
		_declaringType = declaringType;
		_returnTypeRequiredModifiers = returnTypeRequiredCustomModifiers;
		_returnTypeOptionalCustomModifiers = returnTypeOptionalCustomModifiers;
		if (parameterTypes != null)
		{
			_parameterTypeRequiredCustomModifiers = parameterTypeRequiredCustomModifiers;
			_parameterTypeOptionalCustomModifiers = parameterTypeOptionalCustomModifiers;
			_parameterTypes = new Type[parameterTypes.Length];
			_parameterBuilders = new ParameterBuilderImpl[parameterTypes.Length + 1];
			for (int i = 0; i < parameterTypes.Length; i++)
			{
				ArgumentNullException.ThrowIfNull(_parameterTypes[i] = parameterTypes[i], "parameterTypes");
			}
		}
		_methodImplFlags = MethodImplAttributes.IL;
		_initLocals = true;
	}

	internal void CreateDllImportData(string dllName, string entryName, CallingConvention nativeCallConv, CharSet nativeCharSet)
	{
		_dllImportData = DllImportData.Create(dllName, entryName, nativeCallConv, nativeCharSet);
	}

	internal BlobBuilder GetMethodSignatureBlob()
	{
		return MetadataSignatureHelper.GetMethodSignature(_module, _parameterTypes, _returnType, ModuleBuilderImpl.GetSignatureConvention(_callingConventions), GetGenericArguments().Length, !base.IsStatic, null, _returnTypeRequiredModifiers, _returnTypeOptionalCustomModifiers, _parameterTypeRequiredCustomModifiers, _parameterTypeOptionalCustomModifiers);
	}

	private void ThrowIfGeneric()
	{
		if (IsGenericMethod && !IsGenericMethodDefinition)
		{
			throw new InvalidOperationException();
		}
	}

	protected override GenericTypeParameterBuilder[] DefineGenericParametersCore(params string[] names)
	{
		if (_typeParameters != null)
		{
			throw new InvalidOperationException(System.SR.InvalidOperation_GenericParametersAlreadySet);
		}
		GenericTypeParameterBuilderImpl[] array = new GenericTypeParameterBuilderImpl[names.Length];
		for (int i = 0; i < names.Length; i++)
		{
			string name = names[i];
			ArgumentNullException.ThrowIfNull(names, "names");
			array[i] = new GenericTypeParameterBuilderImpl(name, i, this, _declaringType);
		}
		return _typeParameters = array;
	}

	protected override ParameterBuilder DefineParameterCore(int position, ParameterAttributes attributes, string strParamName)
	{
		_declaringType.ThrowIfCreated();
		if (position > 0 && (_parameterTypes == null || position > _parameterTypes.Length))
		{
			throw new ArgumentOutOfRangeException(System.SR.ArgumentOutOfRange_ParamSequence);
		}
		if (_parameterBuilders == null)
		{
			_parameterBuilders = new ParameterBuilderImpl[1];
		}
		attributes &= ~ParameterAttributes.ReservedMask;
		ParameterBuilderImpl parameterBuilderImpl = new ParameterBuilderImpl(this, position, attributes, strParamName);
		_parameterBuilders[position] = parameterBuilderImpl;
		return parameterBuilderImpl;
	}

	protected override ILGenerator GetILGeneratorCore(int size)
	{
		if (IsGenericMethod && !IsGenericMethodDefinition)
		{
			throw new InvalidOperationException();
		}
		if ((_methodImplFlags & MethodImplAttributes.CodeTypeMask) != MethodImplAttributes.IL || (_methodImplFlags & MethodImplAttributes.ManagedMask) != MethodImplAttributes.IL || (_attributes & MethodAttributes.PinvokeImpl) != MethodAttributes.PrivateScope)
		{
			throw new InvalidOperationException(System.SR.InvalidOperation_ShouldNotHaveMethodBody);
		}
		if ((_attributes & MethodAttributes.Abstract) != MethodAttributes.PrivateScope)
		{
			throw new InvalidOperationException(System.SR.InvalidOperation_ShouldNotHaveMethodBody);
		}
		return _ilGenerator ?? (_ilGenerator = new ILGeneratorImpl(this, size));
	}

	internal void SetCustomAttribute(ConstructorInfo con, ReadOnlySpan<byte> binaryAttribute)
	{
		SetCustomAttributeCore(con, binaryAttribute);
	}

	protected override void SetCustomAttributeCore(ConstructorInfo con, ReadOnlySpan<byte> binaryAttribute)
	{
		switch (con.ReflectedType.FullName)
		{
		case "System.Runtime.CompilerServices.MethodImplAttribute":
		{
			int num = BinaryPrimitives.ReadUInt16LittleEndian(binaryAttribute.Slice(2));
			_methodImplFlags |= (MethodImplAttributes)num;
			return;
		}
		case "System.Runtime.InteropServices.DllImportAttribute":
		{
			_dllImportData = DllImportData.Create(CustomAttributeInfo.DecodeCustomAttribute(con, binaryAttribute), out var preserveSig);
			_attributes |= MethodAttributes.PinvokeImpl;
			if (preserveSig)
			{
				_methodImplFlags |= MethodImplAttributes.PreserveSig;
			}
			return;
		}
		case "System.Runtime.InteropServices.PreserveSigAttribute":
			_methodImplFlags |= MethodImplAttributes.PreserveSig;
			return;
		case "System.Runtime.CompilerServices.SpecialNameAttribute":
			_attributes |= MethodAttributes.SpecialName;
			return;
		case "System.Security.SuppressUnmanagedCodeSecurityAttribute":
			_attributes |= MethodAttributes.HasSecurity;
			break;
		}
		if (_customAttributes == null)
		{
			_customAttributes = new List<CustomAttributeWrapper>();
		}
		_customAttributes.Add(new CustomAttributeWrapper(con, binaryAttribute));
	}

	protected override void SetImplementationFlagsCore(MethodImplAttributes attributes)
	{
		_declaringType.ThrowIfCreated();
		_methodImplFlags = attributes;
	}

	protected override void SetSignatureCore(Type returnType, Type[] returnTypeRequiredCustomModifiers, Type[] returnTypeOptionalCustomModifiers, Type[] parameterTypes, Type[][] parameterTypeRequiredCustomModifiers, Type[][] parameterTypeOptionalCustomModifiers)
	{
		if (returnType != null)
		{
			_returnType = returnType;
			_returnTypeOptionalCustomModifiers = returnTypeOptionalCustomModifiers;
			_returnTypeRequiredModifiers = returnTypeRequiredCustomModifiers;
		}
		if (parameterTypes != null)
		{
			_parameterTypes = new Type[parameterTypes.Length];
			_parameterBuilders = new ParameterBuilderImpl[parameterTypes.Length + 1];
			for (int i = 0; i < parameterTypes.Length; i++)
			{
				ArgumentNullException.ThrowIfNull(_parameterTypes[i] = parameterTypes[i], "parameterTypes");
			}
			_parameterTypeOptionalCustomModifiers = parameterTypeOptionalCustomModifiers;
			_parameterTypeRequiredCustomModifiers = parameterTypeRequiredCustomModifiers;
		}
	}

	public override MethodInfo GetBaseDefinition()
	{
		return this;
	}

	public override object[] GetCustomAttributes(bool inherit)
	{
		throw new NotSupportedException(System.SR.NotSupported_DynamicModule);
	}

	public override object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		throw new NotSupportedException(System.SR.NotSupported_DynamicModule);
	}

	public override Type[] GetGenericArguments()
	{
		Type[] typeParameters = _typeParameters;
		return typeParameters ?? Type.EmptyTypes;
	}

	public override MethodInfo GetGenericMethodDefinition()
	{
		if (IsGenericMethod)
		{
			return this;
		}
		throw new InvalidOperationException();
	}

	public override MethodImplAttributes GetMethodImplementationFlags()
	{
		return _methodImplFlags;
	}

	public override ParameterInfo[] GetParameters()
	{
		if (_parameterTypes == null)
		{
			return Array.Empty<ParameterInfo>();
		}
		if (_parameterBuilders == null)
		{
			_parameterBuilders = new ParameterBuilderImpl[_parameterTypes.Length + 1];
		}
		ParameterInfo[] array = new ParameterInfo[_parameterTypes.Length];
		for (int i = 0; i < _parameterTypes.Length; i++)
		{
			if (_parameterBuilders[i + 1] == null)
			{
				array[i] = new ParameterInfoWrapper(new ParameterBuilderImpl(this, i, ParameterAttributes.None, null), _parameterTypes[i]);
			}
			else
			{
				array[i] = new ParameterInfoWrapper(_parameterBuilders[i + 1], _parameterTypes[i]);
			}
		}
		return array;
	}

	public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
	{
		throw new NotSupportedException(System.SR.NotSupported_DynamicModule);
	}

	public override bool IsDefined(Type attributeType, bool inherit)
	{
		throw new NotSupportedException(System.SR.NotSupported_DynamicModule);
	}

	[RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
	[RequiresUnreferencedCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
	public override MethodInfo MakeGenericMethod(params Type[] typeArguments)
	{
		return System.Reflection.Emit.MethodBuilderInstantiation.MakeGenericMethod(this, typeArguments);
	}
}
