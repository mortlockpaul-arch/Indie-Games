using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit;

internal sealed class TypeBuilderImpl : TypeBuilder
{
	private readonly ModuleBuilderImpl _module;

	private readonly string _name;

	private readonly string _namespace;

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
	private Type _typeParent;

	private readonly TypeBuilderImpl _declaringType;

	private GenericTypeParameterBuilderImpl[] _typeParameters;

	private TypeAttributes _attributes;

	private PackingSize _packingSize;

	private int _typeSize;

	private Type _enumUnderlyingType;

	private bool _isCreated;

	internal bool _isHiddenGlobalType;

	internal TypeDefinitionHandle _handle;

	internal int _firstFieldToken;

	internal int _firstMethodToken;

	internal int _firstPropertyToken;

	internal int _firstEventToken;

	internal readonly List<MethodBuilderImpl> _methodDefinitions = new List<MethodBuilderImpl>();

	internal readonly List<FieldBuilderImpl> _fieldDefinitions = new List<FieldBuilderImpl>();

	internal readonly List<ConstructorBuilderImpl> _constructorDefinitions = new List<ConstructorBuilderImpl>();

	internal List<Type> _interfaces;

	internal readonly List<PropertyBuilderImpl> _propertyDefinitions = new List<PropertyBuilderImpl>();

	internal readonly List<EventBuilderImpl> _eventDefinitions = new List<EventBuilderImpl>();

	internal List<CustomAttributeWrapper> _customAttributes;

	internal Dictionary<Type, List<(MethodInfo ifaceMethod, MethodInfo targetMethod)>> _methodOverrides;

	[CompilerGenerated]
	private string _003CFullName_003Ek__BackingField;

	protected override PackingSize PackingSizeCore => _packingSize;

	protected override int SizeCore => _typeSize;

	public override string Name => _name;

	public override Type DeclaringType => _declaringType;

	public override Type ReflectedType => _declaringType;

	public override bool IsGenericTypeDefinition => IsGenericType;

	public override bool IsConstructedGenericType => false;

	public override bool IsGenericType => _typeParameters != null;

	public override Type[] GenericTypeParameters
	{
		get
		{
			Type[] typeParameters = _typeParameters;
			return typeParameters ?? Type.EmptyTypes;
		}
	}

	public override string AssemblyQualifiedName
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public override string FullName => _003CFullName_003Ek__BackingField ?? (_003CFullName_003Ek__BackingField = System.Reflection.Emit.TypeNameBuilder.ToString(this, System.Reflection.Emit.TypeNameBuilder.Format.FullName));

	public override string Namespace => _namespace;

	public override Assembly Assembly => _module.Assembly;

	public override Module Module => _module;

	public override Type UnderlyingSystemType => this;

	public override bool IsSZArray => false;

	public override Guid GUID
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public override Type BaseType => _typeParent;

	public override int MetadataToken => MetadataTokens.GetToken(_handle);

	internal TypeBuilderImpl(ModuleBuilderImpl module)
	{
		_name = "<Module>";
		_module = module;
		_isHiddenGlobalType = true;
		_handle = MetadataTokens.TypeDefinitionHandle(1);
	}

	internal TypeBuilderImpl(string fullName, TypeAttributes typeAttributes, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type parent, ModuleBuilderImpl module, Type[] interfaces, PackingSize packingSize, int typeSize, TypeBuilderImpl enclosingType)
	{
		_name = fullName;
		_module = module;
		_attributes = typeAttributes;
		_packingSize = packingSize;
		_typeSize = typeSize;
		SetParent(parent);
		_declaringType = enclosingType;
		int num = _name.LastIndexOf('.');
		if (num != -1)
		{
			_namespace = _name.Substring(0, num);
			string name = _name;
			int num2 = num + 1;
			_name = name.Substring(num2, name.Length - num2);
		}
		if (interfaces != null)
		{
			_interfaces = new List<Type>(interfaces.Length);
			foreach (Type type in interfaces)
			{
				ArgumentNullException.ThrowIfNull(type, "interfaces");
				_interfaces.Add(type);
			}
		}
	}

	internal ModuleBuilderImpl GetModuleBuilder()
	{
		return _module;
	}

	protected override void AddInterfaceImplementationCore([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type interfaceType)
	{
		ThrowIfCreated();
		if (_interfaces == null)
		{
			_interfaces = new List<Type>();
		}
		_interfaces.Add(interfaceType);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2083:DynamicallyAccessedMembers", Justification = "Not sure how to handle warning on 'this'")]
	[return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
	protected override TypeInfo CreateTypeInfoCore()
	{
		if (_isCreated)
		{
			return this;
		}
		if (_isHiddenGlobalType)
		{
			_isCreated = true;
			return null;
		}
		if (_constructorDefinitions.Count == 0 && (_attributes & TypeAttributes.ClassSemanticsMask) == 0 && !base.IsValueType && (_attributes & (TypeAttributes.Abstract | TypeAttributes.Sealed)) != (TypeAttributes.Abstract | TypeAttributes.Sealed))
		{
			DefineDefaultConstructor(MethodAttributes.Public);
		}
		ValidateMethods();
		_isCreated = true;
		return this;
	}

	private void ValidateMethods()
	{
		for (int i = 0; i < _methodDefinitions.Count; i++)
		{
			MethodBuilderImpl methodBuilderImpl = _methodDefinitions[i];
			if ((methodBuilderImpl.Attributes & MethodAttributes.Abstract) != MethodAttributes.PrivateScope)
			{
				if ((_attributes & TypeAttributes.Abstract) == 0)
				{
					throw new InvalidOperationException(System.SR.InvalidOperation_BadTypeAttributesNotAbstract);
				}
				ILGeneratorImpl iLGeneratorImpl = methodBuilderImpl.ILGeneratorImpl;
				if (iLGeneratorImpl != null && iLGeneratorImpl.ILOffset > 0)
				{
					throw new InvalidOperationException(System.SR.Format(System.SR.InvalidOperation_BadMethodBody, methodBuilderImpl.Name));
				}
			}
		}
	}

	internal void ThrowIfCreated()
	{
		if (_isCreated)
		{
			throw new InvalidOperationException(System.SR.InvalidOperation_TypeHasBeenCreated);
		}
	}

	protected override ConstructorBuilder DefineConstructorCore(MethodAttributes attributes, CallingConventions callingConvention, Type[] parameterTypes, Type[][] requiredCustomModifiers, Type[][] optionalCustomModifiers)
	{
		if ((_attributes & TypeAttributes.ClassSemanticsMask) == TypeAttributes.ClassSemanticsMask && (attributes & MethodAttributes.Static) != MethodAttributes.Static)
		{
			throw new InvalidOperationException(System.SR.InvalidOperation_ConstructorNotAllowedOnInterface);
		}
		ThrowIfCreated();
		string name = (((attributes & MethodAttributes.Static) != MethodAttributes.PrivateScope) ? ConstructorInfo.TypeConstructorName : ConstructorInfo.ConstructorName);
		attributes |= MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
		ConstructorBuilderImpl constructorBuilderImpl = new ConstructorBuilderImpl(name, attributes, callingConvention, parameterTypes, requiredCustomModifiers, optionalCustomModifiers, _module, this);
		_constructorDefinitions.Add(constructorBuilderImpl);
		return constructorBuilderImpl;
	}

	protected override ConstructorBuilder DefineDefaultConstructorCore(MethodAttributes attributes)
	{
		if ((_attributes & TypeAttributes.ClassSemanticsMask) == TypeAttributes.ClassSemanticsMask)
		{
			throw new InvalidOperationException(System.SR.InvalidOperation_ConstructorNotAllowedOnInterface);
		}
		return DefineDefaultConstructorInternal(attributes);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2075:UnrecognizedReflectionPattern", Justification = "GetConstructor is only called on a TypeBuilderInstantiation which is not subject to trimming")]
	private ConstructorBuilderImpl DefineDefaultConstructorInternal(MethodAttributes attributes)
	{
		ConstructorInfo constructorInfo = ((!_typeParent.IsConstructedGenericType || (!(_typeParent.GetGenericTypeDefinition() is TypeBuilderImpl) && !_module.ContainsTypeBuilder(_typeParent.GetGenericArguments()))) ? _typeParent.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) : TypeBuilder.GetConstructor(_typeParent, _typeParent.GetGenericTypeDefinition().GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null)));
		if (constructorInfo == null)
		{
			throw new NotSupportedException(System.SR.NotSupported_NoParentDefaultConstructor);
		}
		ConstructorBuilderImpl obj = (ConstructorBuilderImpl)DefineConstructorCore(attributes, CallingConventions.Standard, null, null, null);
		ILGenerator iLGenerator = obj.GetILGenerator();
		iLGenerator.Emit(OpCodes.Ldarg_0);
		iLGenerator.Emit(OpCodes.Call, constructorInfo);
		iLGenerator.Emit(OpCodes.Ret);
		obj._isDefaultConstructor = true;
		return obj;
	}

	protected override EventBuilder DefineEventCore(string name, EventAttributes attributes, Type eventtype)
	{
		ArgumentNullException.ThrowIfNull(eventtype, "eventtype");
		ThrowIfCreated();
		EventBuilderImpl eventBuilderImpl = new EventBuilderImpl(name, attributes, eventtype, this);
		_eventDefinitions.Add(eventBuilderImpl);
		return eventBuilderImpl;
	}

	protected override FieldBuilder DefineFieldCore(string fieldName, Type type, Type[] requiredCustomModifiers, Type[] optionalCustomModifiers, FieldAttributes attributes)
	{
		ThrowIfCreated();
		if (_enumUnderlyingType == null && IsEnum && (attributes & FieldAttributes.Static) == 0)
		{
			_enumUnderlyingType = type;
		}
		FieldBuilderImpl fieldBuilderImpl = new FieldBuilderImpl(this, fieldName, type, attributes, requiredCustomModifiers, optionalCustomModifiers);
		_fieldDefinitions.Add(fieldBuilderImpl);
		return fieldBuilderImpl;
	}

	protected override GenericTypeParameterBuilder[] DefineGenericParametersCore(params string[] names)
	{
		if (_typeParameters != null)
		{
			throw new InvalidOperationException();
		}
		GenericTypeParameterBuilderImpl[] array = new GenericTypeParameterBuilderImpl[names.Length];
		for (int i = 0; i < names.Length; i++)
		{
			string text = names[i];
			ArgumentNullException.ThrowIfNull(text, "names");
			array[i] = new GenericTypeParameterBuilderImpl(text, i, this);
		}
		return _typeParameters = array;
	}

	protected override FieldBuilder DefineInitializedDataCore(string name, byte[] data, FieldAttributes attributes)
	{
		if (data.Length == 0 || data.Length >= 4128768)
		{
			throw new ArgumentException(System.SR.Argument_BadSizeForData, "Length");
		}
		return DefineDataHelper(name, data, data.Length, attributes);
	}

	protected override MethodBuilder DefineMethodCore(string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] returnTypeRequiredCustomModifiers, Type[] returnTypeOptionalCustomModifiers, Type[] parameterTypes, Type[][] parameterTypeRequiredCustomModifiers, Type[][] parameterTypeOptionalCustomModifiers)
	{
		ThrowIfCreated();
		MethodBuilderImpl methodBuilderImpl = new MethodBuilderImpl(name, attributes, callingConvention, returnType, returnTypeRequiredCustomModifiers, returnTypeOptionalCustomModifiers, parameterTypes, parameterTypeRequiredCustomModifiers, parameterTypeOptionalCustomModifiers, _module, this);
		_methodDefinitions.Add(methodBuilderImpl);
		return methodBuilderImpl;
	}

	protected override void DefineMethodOverrideCore(MethodInfo methodInfoBody, MethodInfo methodInfoDeclaration)
	{
		ThrowIfCreated();
		if ((object)methodInfoBody.DeclaringType != this)
		{
			throw new ArgumentException(System.SR.ArgumentException_BadMethodImplBody);
		}
		Type declaringType = methodInfoDeclaration.DeclaringType;
		if (_methodOverrides == null)
		{
			_methodOverrides = new Dictionary<Type, List<(MethodInfo, MethodInfo)>>();
		}
		if (_methodOverrides.TryGetValue(declaringType, out List<(MethodInfo, MethodInfo)> value))
		{
			if (value.Exists(((MethodInfo ifaceMethod, MethodInfo targetMethod) pair) => pair.ifaceMethod.Equals(methodInfoDeclaration)))
			{
				throw new ArgumentException(System.SR.Format(System.SR.Argument_MethodOverridden, methodInfoBody.Name, FullName), "methodInfoDeclaration");
			}
			value.Add((methodInfoDeclaration, methodInfoBody));
		}
		else
		{
			value = new List<(MethodInfo, MethodInfo)>();
			value.Add((methodInfoDeclaration, methodInfoBody));
			_methodOverrides.Add(declaringType, value);
		}
	}

	private bool IsInterfaceImplemented(Type interfaceType)
	{
		if (_interfaces != null)
		{
			foreach (Type @interface in _interfaces)
			{
				if (interfaceType.IsAssignableFrom(@interface))
				{
					return true;
				}
			}
		}
		if (_typeParent != null)
		{
			Type[] interfaces = _typeParent.GetInterfaces();
			foreach (Type c in interfaces)
			{
				if (interfaceType.IsAssignableFrom(c))
				{
					return true;
				}
			}
		}
		return false;
	}

	protected override TypeBuilder DefineNestedTypeCore(string name, TypeAttributes attr, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type parent, Type[] interfaces, PackingSize packSize, int typeSize)
	{
		return _module.DefineNestedType(name, attr, parent, interfaces, packSize, typeSize, this);
	}

	[RequiresUnreferencedCode("P/Invoke marshalling may dynamically access members that could be trimmed.")]
	protected override MethodBuilder DefinePInvokeMethodCore(string name, string dllName, string entryName, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] returnTypeRequiredCustomModifiers, Type[] returnTypeOptionalCustomModifiers, Type[] parameterTypes, Type[][] parameterTypeRequiredCustomModifiers, Type[][] parameterTypeOptionalCustomModifiers, CallingConvention nativeCallConv, CharSet nativeCharSet)
	{
		if ((attributes & MethodAttributes.Abstract) != MethodAttributes.PrivateScope)
		{
			throw new ArgumentException(System.SR.Argument_BadPInvokeMethod);
		}
		if ((_attributes & TypeAttributes.ClassSemanticsMask) == TypeAttributes.ClassSemanticsMask)
		{
			throw new ArgumentException(System.SR.Argument_BadPInvokeOnInterface);
		}
		ThrowIfCreated();
		attributes |= MethodAttributes.PinvokeImpl;
		MethodBuilderImpl methodBuilderImpl = new MethodBuilderImpl(name, attributes, callingConvention, returnType, returnTypeRequiredCustomModifiers, returnTypeOptionalCustomModifiers, parameterTypes, parameterTypeRequiredCustomModifiers, parameterTypeOptionalCustomModifiers, _module, this);
		methodBuilderImpl.CreateDllImportData(dllName, entryName, nativeCallConv, nativeCharSet);
		if (_methodDefinitions.Contains(methodBuilderImpl))
		{
			throw new ArgumentException(System.SR.Argument_MethodRedefined);
		}
		_methodDefinitions.Add(methodBuilderImpl);
		return methodBuilderImpl;
	}

	protected override PropertyBuilder DefinePropertyCore(string name, PropertyAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] returnTypeRequiredCustomModifiers, Type[] returnTypeOptionalCustomModifiers, Type[] parameterTypes, Type[][] parameterTypeRequiredCustomModifiers, Type[][] parameterTypeOptionalCustomModifiers)
	{
		PropertyBuilderImpl propertyBuilderImpl = new PropertyBuilderImpl(name, attributes, callingConvention, returnType, returnTypeRequiredCustomModifiers, returnTypeOptionalCustomModifiers, parameterTypes, parameterTypeRequiredCustomModifiers, parameterTypeOptionalCustomModifiers, this);
		_propertyDefinitions.Add(propertyBuilderImpl);
		return propertyBuilderImpl;
	}

	protected override ConstructorBuilder DefineTypeInitializerCore()
	{
		ThrowIfCreated();
		return new ConstructorBuilderImpl(ConstructorInfo.TypeConstructorName, MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, null, null, null, _module, this);
	}

	protected override FieldBuilder DefineUninitializedDataCore(string name, int size, FieldAttributes attributes)
	{
		if (size <= 0 || size >= 4128768)
		{
			throw new ArgumentException(System.SR.Argument_BadSizeForData, "size");
		}
		return DefineDataHelper(name, new byte[size], size, attributes);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2072:DynamicallyAccessedMembers", Justification = "The members of 'ValueType' are not referenced in this context")]
	private FieldBuilder DefineDataHelper(string name, byte[] data, int size, FieldAttributes attributes)
	{
		ArgumentException.ThrowIfNullOrEmpty(name, "name");
		ThrowIfCreated();
		string text = $"$ArrayType${size}";
		TypeBuilderImpl typeBuilderImpl = _module.FindTypeBuilderWithName(text, ignoreCase: false);
		if (typeBuilderImpl == null)
		{
			TypeAttributes attr = TypeAttributes.Public | TypeAttributes.ExplicitLayout | TypeAttributes.Sealed;
			typeBuilderImpl = (TypeBuilderImpl)_module.DefineType(text, attr, _module.GetTypeFromCoreAssembly(CoreTypeId.ValueType), PackingSize.Size1, size);
			typeBuilderImpl.CreateType();
		}
		FieldBuilder fieldBuilder = DefineField(name, typeBuilderImpl, attributes | FieldAttributes.Static | FieldAttributes.HasFieldRVA);
		((FieldBuilderImpl)fieldBuilder).SetData(data);
		return fieldBuilder;
	}

	protected override bool IsCreatedCore()
	{
		return _isCreated;
	}

	protected override void SetCustomAttributeCore(ConstructorInfo con, ReadOnlySpan<byte> binaryAttribute)
	{
		switch (con.ReflectedType.FullName)
		{
		case "System.Runtime.InteropServices.StructLayoutAttribute":
			ParseStructLayoutAttribute(con, binaryAttribute);
			return;
		case "System.Runtime.CompilerServices.SpecialNameAttribute":
			_attributes |= TypeAttributes.SpecialName;
			return;
		case "System.SerializableAttribute":
			_attributes |= TypeAttributes.Serializable;
			return;
		case "System.Runtime.InteropServices.ComImportAttribute":
			_attributes |= TypeAttributes.Import;
			return;
		case "System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeImportAttribute":
			_attributes |= TypeAttributes.WindowsRuntime;
			return;
		case "System.Security.SuppressUnmanagedCodeSecurityAttribute":
			_attributes |= TypeAttributes.HasSecurity;
			break;
		}
		if (_customAttributes == null)
		{
			_customAttributes = new List<CustomAttributeWrapper>();
		}
		_customAttributes.Add(new CustomAttributeWrapper(con, binaryAttribute));
	}

	internal void SetCustomAttribute(ConstructorInfo con, ReadOnlySpan<byte> binaryAttribute)
	{
		SetCustomAttributeCore(con, binaryAttribute);
	}

	private void ParseStructLayoutAttribute(ConstructorInfo con, ReadOnlySpan<byte> binaryAttribute)
	{
		CustomAttributeInfo customAttributeInfo = CustomAttributeInfo.DecodeCustomAttribute(con, binaryAttribute);
		LayoutKind layoutKind = (LayoutKind)customAttributeInfo._ctorArgs[0];
		_attributes &= ~TypeAttributes.LayoutMask;
		TypeAttributes attributes = _attributes;
		_attributes = (TypeAttributes)((int)attributes | (layoutKind switch
		{
			LayoutKind.Auto => 0, 
			LayoutKind.Explicit => 16, 
			LayoutKind.Sequential => 8, 
			_ => 0, 
		}));
		for (int i = 0; i < customAttributeInfo._namedParamNames.Length; i++)
		{
			string text = customAttributeInfo._namedParamNames[i];
			int num = (int)customAttributeInfo._namedParamValues[i];
			switch (text)
			{
			case "CharSet":
				switch ((CharSet)num)
				{
				case CharSet.None:
				case CharSet.Ansi:
					_attributes &= ~TypeAttributes.StringFormatMask;
					break;
				case CharSet.Unicode:
					_attributes &= ~TypeAttributes.AutoClass;
					_attributes |= TypeAttributes.UnicodeClass;
					break;
				case CharSet.Auto:
					_attributes &= ~TypeAttributes.UnicodeClass;
					_attributes |= TypeAttributes.AutoClass;
					break;
				}
				break;
			case "Pack":
				_packingSize = (PackingSize)num;
				break;
			case "Size":
				_typeSize = num;
				break;
			default:
				throw new ArgumentException(System.SR.Format(System.SR.Argument_UnknownNamedType, con.DeclaringType, text), "binaryAttribute");
			}
		}
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2074:DynamicallyAccessedMembers", Justification = "System.Object type is preserved via ModulBuilderImpl.s_coreTypes")]
	protected override void SetParentCore([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type parent)
	{
		ThrowIfCreated();
		if (parent != null)
		{
			if (parent.IsInterface)
			{
				throw new ArgumentException(System.SR.Argument_CannotSetParentToInterface);
			}
			_typeParent = parent;
		}
		else if ((_attributes & TypeAttributes.ClassSemanticsMask) != TypeAttributes.ClassSemanticsMask)
		{
			_typeParent = _module.GetTypeFromCoreAssembly(CoreTypeId.Object);
		}
		else
		{
			if ((_attributes & TypeAttributes.Abstract) == 0)
			{
				throw new InvalidOperationException(System.SR.InvalidOperation_BadInterfaceNotAbstract);
			}
			_typeParent = null;
		}
	}

	public override Type[] GetGenericArguments()
	{
		Type[] typeParameters = _typeParameters;
		return typeParameters ?? Type.EmptyTypes;
	}

	public override Type GetGenericTypeDefinition()
	{
		if (IsGenericTypeDefinition)
		{
			return this;
		}
		throw new InvalidOperationException();
	}

	public override bool IsDefined(Type attributeType, bool inherit)
	{
		throw new NotImplementedException();
	}

	public override object[] GetCustomAttributes(bool inherit)
	{
		throw new NotImplementedException();
	}

	public override object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		throw new NotImplementedException();
	}

	public override Type GetElementType()
	{
		throw new NotSupportedException(System.SR.NotSupported_DynamicModule);
	}

	public override Type GetEnumUnderlyingType()
	{
		if (IsEnum)
		{
			if (_enumUnderlyingType == null)
			{
				throw new InvalidOperationException(System.SR.InvalidOperation_NoUnderlyingTypeOnEnum);
			}
			return _enumUnderlyingType;
		}
		throw new ArgumentException(System.SR.Argument_MustBeEnum);
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
	public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
	{
		throw new NotSupportedException();
	}

	protected override bool IsArrayImpl()
	{
		return false;
	}

	protected override bool IsByRefImpl()
	{
		return false;
	}

	protected override bool IsPointerImpl()
	{
		return false;
	}

	protected override bool IsPrimitiveImpl()
	{
		return false;
	}

	protected override bool IsValueTypeImpl()
	{
		return IsSubclassOf(_module.GetTypeFromCoreAssembly(CoreTypeId.ValueType));
	}

	protected override bool HasElementTypeImpl()
	{
		return false;
	}

	protected override TypeAttributes GetAttributeFlagsImpl()
	{
		return _attributes;
	}

	protected override bool IsCOMObjectImpl()
	{
		return (GetAttributeFlagsImpl() & TypeAttributes.Import) != 0;
	}

	internal void ThrowIfNotCreated()
	{
		if (!_isCreated)
		{
			throw new NotSupportedException(System.SR.NotSupported_TypeNotYetCreated);
		}
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
	protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] _)
	{
		ThrowIfNotCreated();
		ArgumentNullException.ThrowIfNull(types, "types");
		for (int i = 0; i < types.Length; i++)
		{
			ArgumentNullException.ThrowIfNull(types[i], "types");
		}
		foreach (ConstructorBuilderImpl constructorDefinition in _constructorDefinitions)
		{
			if (MatchesTheFilter(constructorDefinition._methodBuilder, GetBindingFlags(constructorDefinition._methodBuilder), bindingAttr, callConvention, types))
			{
				return constructorDefinition;
			}
		}
		return null;
	}

	internal static BindingFlags GetBindingFlags(MethodInfo method)
	{
		return (BindingFlags)((((method.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public) ? 16 : 32) | (((method.Attributes & MethodAttributes.Static) != MethodAttributes.PrivateScope) ? 8 : 4));
	}

	private static bool MatchesTheFilter(MethodBuilderImpl method, BindingFlags methodFlags, BindingFlags bindingFlags, CallingConventions callConv, Type[] argumentTypes)
	{
		if ((bindingFlags & methodFlags) != methodFlags)
		{
			return false;
		}
		if ((callConv & CallingConventions.Any) == 0)
		{
			if ((callConv & CallingConventions.VarArgs) != 0 && (method.CallingConvention & CallingConventions.VarArgs) == 0)
			{
				return false;
			}
			if ((callConv & CallingConventions.Standard) != 0 && (method.CallingConvention & CallingConventions.Standard) == 0)
			{
				return false;
			}
		}
		if (argumentTypes == null)
		{
			return true;
		}
		Type[] array = method.ParameterTypes ?? Type.EmptyTypes;
		if (argumentTypes.Length != array.Length)
		{
			return false;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (!argumentTypes[i].Equals(array[i]))
			{
				return false;
			}
		}
		return true;
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
	private ConstructorInfo[] GetConstructors(string name, BindingFlags bindingAttr)
	{
		ThrowIfNotCreated();
		ConstructorInfo[] constructors = GetConstructors(bindingAttr);
		List<ConstructorInfo> list = new List<ConstructorInfo>();
		ConstructorInfo[] array = constructors;
		foreach (ConstructorInfo constructorInfo in array)
		{
			if (name == constructorInfo.Name)
			{
				list.Add(constructorInfo);
			}
		}
		return list.ToArray();
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
	public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
	{
		ThrowIfNotCreated();
		List<ConstructorInfo> list = new List<ConstructorInfo>();
		foreach (ConstructorBuilderImpl constructorDefinition in _constructorDefinitions)
		{
			if (MatchesTheFilter(constructorDefinition._methodBuilder, GetBindingFlags(constructorDefinition._methodBuilder), bindingAttr, CallingConventions.Any, constructorDefinition._methodBuilder.ParameterTypes))
			{
				list.Add(constructorDefinition);
			}
		}
		return list.ToArray();
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents)]
	public override EventInfo[] GetEvents()
	{
		throw new NotSupportedException();
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)]
	public override EventInfo[] GetEvents(BindingFlags bindingAttr)
	{
		throw new NotSupportedException();
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)]
	public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
	{
		throw new NotSupportedException();
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
	private MethodInfo[] GetMethods(string name, BindingFlags bindingAttr)
	{
		ArgumentNullException.ThrowIfNull(name, "name");
		MethodInfo[] methods = GetMethods(bindingAttr);
		List<MethodInfo> list = new List<MethodInfo>();
		MethodInfo[] array = methods;
		foreach (MethodInfo methodInfo in array)
		{
			if (name == methodInfo.Name)
			{
				list.Add(methodInfo);
			}
		}
		return list.ToArray();
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
	public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
	{
		ThrowIfNotCreated();
		List<MethodInfo> list = new List<MethodInfo>();
		foreach (MethodBuilderImpl methodDefinition in _methodDefinitions)
		{
			if (!methodDefinition.IsConstructor && MatchesTheFilter(methodDefinition, GetBindingFlags(methodDefinition), bindingAttr, CallingConventions.Any, methodDefinition.ParameterTypes))
			{
				list.Add(methodDefinition);
			}
		}
		if (!bindingAttr.HasFlag(BindingFlags.DeclaredOnly) && _typeParent != null)
		{
			list.AddRange(_typeParent.GetMethods(bindingAttr));
		}
		return list.ToArray();
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
	protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
	{
		ThrowIfNotCreated();
		MethodInfo methodInfo = null;
		foreach (MethodBuilderImpl methodDefinition in _methodDefinitions)
		{
			if (name.Equals(methodDefinition.Name) && MatchesTheFilter(methodDefinition, GetBindingFlags(methodDefinition), bindingAttr, callConvention, types))
			{
				if (methodInfo != null)
				{
					throw new AmbiguousMatchException(System.SR.Format(System.SR.AmbiguousMatch_MemberInfo, methodDefinition.DeclaringType, methodDefinition.Name));
				}
				methodInfo = methodDefinition;
			}
		}
		if (methodInfo == null && !bindingAttr.HasFlag(BindingFlags.DeclaredOnly) && _typeParent != null)
		{
			methodInfo = ((types != null) ? _typeParent.GetMethod(name, bindingAttr, binder, callConvention, types, modifiers) : _typeParent.GetMethod(name, bindingAttr));
		}
		return methodInfo;
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)]
	public override FieldInfo GetField(string name, BindingFlags bindingFlags)
	{
		ArgumentNullException.ThrowIfNull(name, "name");
		ThrowIfNotCreated();
		FieldInfo fieldInfo = null;
		StringComparison comparisonType = (((bindingFlags & BindingFlags.IgnoreCase) != BindingFlags.Default) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		foreach (FieldBuilderImpl fieldDefinition in _fieldDefinitions)
		{
			BindingFlags bindingFlags2 = GetBindingFlags(fieldDefinition);
			if (name.Equals(fieldDefinition.Name, comparisonType) && (bindingFlags & bindingFlags2) == bindingFlags2)
			{
				if (fieldInfo != null && (object)fieldDefinition.DeclaringType == fieldInfo.DeclaringType)
				{
					throw new AmbiguousMatchException(System.SR.Format(System.SR.AmbiguousMatch_MemberInfo, fieldDefinition.DeclaringType, fieldDefinition.Name));
				}
				fieldInfo = fieldDefinition;
			}
		}
		if (fieldInfo == null && !bindingFlags.HasFlag(BindingFlags.DeclaredOnly) && _typeParent != null)
		{
			fieldInfo = _typeParent.GetField(name, bindingFlags);
		}
		return fieldInfo;
	}

	private static BindingFlags GetBindingFlags(FieldBuilderImpl field)
	{
		return (BindingFlags)((((field.Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Public) ? 16 : 32) | (((field.Attributes & FieldAttributes.Static) != FieldAttributes.PrivateScope) ? 8 : 4));
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)]
	private FieldInfo[] GetFields(string name, BindingFlags bindingAttr)
	{
		ArgumentNullException.ThrowIfNull(name, "name");
		ThrowIfNotCreated();
		FieldInfo[] fields = GetFields(bindingAttr);
		List<FieldInfo> list = new List<FieldInfo>();
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			if (name == fieldInfo.Name)
			{
				list.Add(fieldInfo);
			}
		}
		return list.ToArray();
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)]
	public override FieldInfo[] GetFields(BindingFlags bindingAttr)
	{
		ThrowIfNotCreated();
		List<FieldInfo> list = new List<FieldInfo>(_fieldDefinitions.Count);
		foreach (FieldBuilderImpl fieldDefinition in _fieldDefinitions)
		{
			BindingFlags bindingFlags = GetBindingFlags(fieldDefinition);
			if ((bindingAttr & bindingFlags) == bindingFlags)
			{
				list.Add(fieldDefinition);
			}
		}
		if (!bindingAttr.HasFlag(BindingFlags.DeclaredOnly) && _typeParent != null)
		{
			list.AddRange(_typeParent.GetFields(bindingAttr));
		}
		return list.ToArray();
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
	[return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
	public override Type GetInterface(string name, bool ignoreCase)
	{
		ArgumentNullException.ThrowIfNull(name, "name");
		ThrowIfNotCreated();
		Type[] interfaces = GetInterfaces();
		Type type = null;
		StringComparison comparisonType = (ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		foreach (Type type2 in interfaces)
		{
			if (name.Equals(type2.Name, comparisonType))
			{
				if (type != null)
				{
					throw new AmbiguousMatchException(System.SR.Format(System.SR.AmbiguousMatch_MemberInfo, type2.DeclaringType, type2.Name));
				}
				type = type2;
			}
		}
		return type;
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
	public override Type[] GetInterfaces()
	{
		ThrowIfNotCreated();
		List<Type> list = _interfaces ?? new List<Type>();
		if (_typeParent != null)
		{
			list.AddRange(_typeParent.GetInterfaces());
		}
		return list.ToArray();
	}

	internal static BindingFlags GetBindingFlags(PropertyBuilderImpl property)
	{
		MethodInfo getMethod = property.GetMethod;
		MethodInfo setMethod = property.SetMethod;
		BindingFlags result = BindingFlags.Default;
		if (getMethod != null)
		{
			result = GetBindingFlags(getMethod);
		}
		else if (setMethod != null)
		{
			result = GetBindingFlags(setMethod);
		}
		return result;
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
	private PropertyInfo[] GetProperties(string name, BindingFlags bindingAttr)
	{
		ArgumentNullException.ThrowIfNull(name, "name");
		PropertyInfo[] properties = GetProperties(bindingAttr);
		List<PropertyInfo> list = new List<PropertyInfo>();
		PropertyInfo[] array = properties;
		foreach (PropertyInfo propertyInfo in array)
		{
			if (name == propertyInfo.Name)
			{
				list.Add(propertyInfo);
			}
		}
		return list.ToArray();
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
	public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
	{
		ThrowIfNotCreated();
		List<PropertyInfo> list = new List<PropertyInfo>(_propertyDefinitions.Count);
		foreach (PropertyBuilderImpl propertyDefinition in _propertyDefinitions)
		{
			BindingFlags bindingFlags = GetBindingFlags(propertyDefinition);
			if ((bindingAttr & bindingFlags) == bindingFlags)
			{
				list.Add(propertyDefinition);
			}
		}
		if (!bindingAttr.HasFlag(BindingFlags.DeclaredOnly) && _typeParent != null)
		{
			list.AddRange(_typeParent.GetProperties(bindingAttr));
		}
		return list.ToArray();
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
	protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
	{
		ArgumentNullException.ThrowIfNull(name, "name");
		List<PropertyInfo> propertyCandidates = GetPropertyCandidates(name, bindingAttr, types);
		if (propertyCandidates.Count == 0)
		{
			return null;
		}
		if (types == null || types.Length == 0)
		{
			PropertyInfo propertyInfo = propertyCandidates[0];
			if (propertyCandidates.Count == 1)
			{
				if ((object)returnType != null && !returnType.IsEquivalentTo(propertyInfo.PropertyType))
				{
					return null;
				}
				return propertyInfo;
			}
			if ((object)returnType == null)
			{
				throw new AmbiguousMatchException(System.SR.Format(System.SR.AmbiguousMatch_MemberInfo, propertyInfo.DeclaringType, propertyInfo.Name));
			}
		}
		if (binder == null)
		{
			binder = Type.DefaultBinder;
		}
		return binder.SelectProperty(bindingAttr, propertyCandidates.ToArray(), returnType, types, modifiers);
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
	private List<PropertyInfo> GetPropertyCandidates(string name, BindingFlags bindingAttr, Type[] types)
	{
		PropertyInfo[] properties = GetProperties(bindingAttr);
		List<PropertyInfo> list = new List<PropertyInfo>(properties.Length);
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (propertyInfo.Name == name && (types == null || propertyInfo.GetIndexParameters().Length == types.Length))
			{
				list.Add(propertyInfo);
			}
		}
		return list;
	}

	private static BindingFlags GetBindingFlags(TypeBuilderImpl type)
	{
		if (!type.IsPublic && !type.IsNestedPublic)
		{
			return BindingFlags.NonPublic;
		}
		return BindingFlags.Public;
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.NonPublicNestedTypes)]
	private Type[] GetNestedTypes(string name, BindingFlags bindingAttr)
	{
		ArgumentNullException.ThrowIfNull(name, "name");
		ThrowIfNotCreated();
		Type[] nestedTypes = GetNestedTypes(bindingAttr);
		List<Type> list = new List<Type>();
		Type[] array = nestedTypes;
		foreach (Type type in array)
		{
			if (type.Name == name)
			{
				list.Add(type);
			}
		}
		return list.ToArray();
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.NonPublicNestedTypes)]
	public override Type[] GetNestedTypes(BindingFlags bindingAttr)
	{
		ThrowIfNotCreated();
		List<Type> list = new List<Type>();
		foreach (TypeBuilderImpl nestedTypeBuilder in _module.GetNestedTypeBuilders(this))
		{
			BindingFlags bindingFlags = GetBindingFlags(nestedTypeBuilder);
			if ((bindingAttr & bindingFlags) == bindingFlags)
			{
				list.Add(nestedTypeBuilder);
			}
		}
		return list.ToArray();
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.NonPublicNestedTypes)]
	public override Type GetNestedType(string name, BindingFlags bindingAttr)
	{
		ArgumentNullException.ThrowIfNull(name, "name");
		Type[] nestedTypes = GetNestedTypes(bindingAttr);
		Type type = null;
		Type[] array = nestedTypes;
		foreach (Type type2 in array)
		{
			if (type2.Name == name)
			{
				if (type != null)
				{
					throw new AmbiguousMatchException(System.SR.Format(System.SR.AmbiguousMatch_MemberInfo, type2.DeclaringType, type2.Name));
				}
				type = type2;
			}
		}
		return type;
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields | DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.NonPublicNestedTypes | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties | DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)]
	public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
	{
		ArgumentNullException.ThrowIfNull(name, "name");
		MethodInfo[] array = Array.Empty<MethodInfo>();
		ConstructorInfo[] array2 = Array.Empty<ConstructorInfo>();
		PropertyInfo[] array3 = Array.Empty<PropertyInfo>();
		EventInfo[] array4 = Array.Empty<EventInfo>();
		FieldInfo[] array5 = Array.Empty<FieldInfo>();
		Type[] array6 = Array.Empty<Type>();
		int num = 0;
		if ((type & MemberTypes.Method) != 0)
		{
			array = GetMethods(name, bindingAttr);
			if (type == MemberTypes.Method)
			{
				return array;
			}
			num += array.Length;
		}
		if ((type & MemberTypes.Constructor) != 0)
		{
			array2 = GetConstructors(name, bindingAttr);
			if (type == MemberTypes.Constructor)
			{
				return array2;
			}
			num += array2.Length;
		}
		if ((type & MemberTypes.Property) != 0)
		{
			array3 = GetProperties(name, bindingAttr);
			if (type == MemberTypes.Property)
			{
				return array3;
			}
			num += array3.Length;
		}
		if ((type & MemberTypes.Field) != 0)
		{
			array5 = GetFields(name, bindingAttr);
			if (type == MemberTypes.Field)
			{
				return array5;
			}
			num += array5.Length;
		}
		if ((type & (MemberTypes.TypeInfo | MemberTypes.NestedType)) != 0)
		{
			array6 = GetNestedTypes(name, bindingAttr);
			if (type == MemberTypes.NestedType || type == MemberTypes.TypeInfo)
			{
				return array6;
			}
			num += array6.Length;
		}
		MemberInfo[] array7;
		if (type != (MemberTypes.Constructor | MemberTypes.Method))
		{
			array7 = new MemberInfo[num];
		}
		else
		{
			MemberInfo[] array8 = new MethodBase[num];
			array7 = array8;
		}
		MemberInfo[] array9 = array7;
		int num2 = 0;
		array.CopyTo(array9, num2);
		num2 += array.Length;
		array2.CopyTo(array9, num2);
		num2 += array2.Length;
		array3.CopyTo(array9, num2);
		num2 += array3.Length;
		array4.CopyTo(array9, num2);
		num2 += array4.Length;
		array5.CopyTo(array9, num2);
		num2 += array5.Length;
		array6.CopyTo(array9, num2);
		return array9;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2085:DynamicallyAccessedMembers", Justification = "Methods are loaded from this TypeBuilder")]
	public override InterfaceMapping GetInterfaceMap([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)] Type interfaceType)
	{
		ThrowIfNotCreated();
		ArgumentNullException.ThrowIfNull(interfaceType, "interfaceType");
		ValidateInterfaceType(interfaceType);
		MethodInfo[] methods = interfaceType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		InterfaceMapping result = new InterfaceMapping
		{
			InterfaceType = interfaceType,
			TargetType = this,
			InterfaceMethods = new MethodInfo[methods.Length],
			TargetMethods = new MethodInfo[methods.Length]
		};
		for (int i = 0; i < methods.Length; i++)
		{
			MethodInfo methodInfo = methods[i];
			MethodInfo methodImpl = GetMethodImpl(methodInfo.Name, GetBindingFlags(methodInfo), null, methodInfo.CallingConvention, GetParameterTypes(methodInfo.GetParameters()), null);
			result.InterfaceMethods[i] = methodInfo;
			result.TargetMethods[i] = methodImpl;
		}
		if (_methodOverrides == null)
		{
			return result;
		}
		if (_methodOverrides.TryGetValue(interfaceType, out List<(MethodInfo, MethodInfo)> value))
		{
			for (int j = 0; j < methods.Length; j++)
			{
				for (int k = 0; k < value.Count; k++)
				{
					if (result.InterfaceMethods[j].Equals(value[k].Item1))
					{
						result.TargetMethods[j] = value[k].Item2;
					}
				}
			}
		}
		return result;
	}

	private static Type[] GetParameterTypes(ParameterInfo[] parameterInfos)
	{
		Type[] array = new Type[parameterInfos.Length];
		for (int i = 0; i < parameterInfos.Length; i++)
		{
			array[i] = parameterInfos[i].ParameterType;
		}
		return array;
	}

	private void ValidateInterfaceType(Type interfaceType)
	{
		if (!interfaceType.IsInterface)
		{
			throw new ArgumentException(System.SR.Argument_MustBeInterface, "interfaceType");
		}
		if (!IsInterfaceImplemented(interfaceType))
		{
			throw new ArgumentException(System.SR.Argument_InterfaceNotFound, "interfaceType");
		}
		if (IsGenericParameter)
		{
			throw new InvalidOperationException(System.SR.Argument_GenericParameter);
		}
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields | DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.NonPublicNestedTypes | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties | DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)]
	public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
	{
		ThrowIfNotCreated();
		MethodInfo[] methods = GetMethods(bindingAttr);
		ConstructorInfo[] constructors = GetConstructors(bindingAttr);
		PropertyInfo[] properties = GetProperties(bindingAttr);
		FieldInfo[] fields = GetFields(bindingAttr);
		Type[] nestedTypes = GetNestedTypes(bindingAttr);
		MemberInfo[] array = new MemberInfo[methods.Length + constructors.Length + properties.Length + fields.Length + nestedTypes.Length];
		int num = 0;
		methods.CopyTo(array, num);
		num += methods.Length;
		constructors.CopyTo(array, num);
		num += constructors.Length;
		properties.CopyTo(array, num);
		num += properties.Length;
		fields.CopyTo(array, num);
		num += fields.Length;
		nestedTypes.CopyTo(array, num);
		return array;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2070:UnrecognizedReflectionPattern", Justification = "The GetInterfaces technically requires all interfaces to be preservedBut in this case it acts only on TypeBuilder which is never trimmed (as it's runtime created).")]
	public override bool IsAssignableFrom([NotNullWhen(true)] Type c)
	{
		if (AreTypesEqual(this, c))
		{
			return true;
		}
		if (c is TypeBuilderImpl typeBuilderImpl)
		{
			if (typeBuilderImpl.IsSubclassOf(this))
			{
				return true;
			}
			if (!base.IsInterface)
			{
				return false;
			}
			Type[] interfaces = typeBuilderImpl.GetInterfaces();
			for (int i = 0; i < interfaces.Length; i++)
			{
				if (AreTypesEqual(interfaces[i], this))
				{
					return true;
				}
				if (interfaces[i].IsSubclassOf(this))
				{
					return true;
				}
			}
		}
		return false;
	}

	internal static bool AreTypesEqual(Type t1, Type t2)
	{
		if (t1 == t2)
		{
			return true;
		}
		if (t1 is TypeBuilderImpl typeBuilderImpl && t2 is TypeBuilderImpl typeBuilderImpl2 && (object)typeBuilderImpl == typeBuilderImpl2)
		{
			return true;
		}
		return false;
	}

	public override bool IsSubclassOf(Type c)
	{
		Type type = this;
		if (AreTypesEqual(type, c))
		{
			return false;
		}
		type = type.BaseType;
		while (type != null)
		{
			if (AreTypesEqual(type, c))
			{
				return true;
			}
			type = type.BaseType;
		}
		return false;
	}
}
