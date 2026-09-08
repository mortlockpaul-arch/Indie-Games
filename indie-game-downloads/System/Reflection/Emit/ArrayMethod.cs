using System.Globalization;

namespace System.Reflection.Emit;

internal sealed class ArrayMethod : MethodInfo
{
	private readonly ModuleBuilder _module;

	private readonly Type _containingType;

	private readonly string _name;

	private readonly CallingConventions _callingConvention;

	private readonly Type _returnType;

	private readonly Type[] _parameterTypes;

	internal Type[] ParameterTypes => _parameterTypes;

	public override Module Module => _module;

	public override Type ReflectedType => _containingType;

	public override string Name => _name;

	public override Type DeclaringType => _containingType;

	public override MethodAttributes Attributes => MethodAttributes.PrivateScope;

	public override CallingConventions CallingConvention => _callingConvention;

	public override RuntimeMethodHandle MethodHandle
	{
		get
		{
			throw new NotSupportedException(System.SR.NotSupported_SymbolMethod);
		}
	}

	public override Type ReturnType => _returnType;

	public override ICustomAttributeProvider ReturnTypeCustomAttributes
	{
		get
		{
			throw new NotSupportedException(System.SR.NotSupported_SymbolMethod);
		}
	}

	internal ArrayMethod(ModuleBuilderImpl module, Type arrayClass, string methodName, CallingConventions callingConvention, Type returnType, Type[] parameterTypes)
	{
		_returnType = returnType ?? module.GetTypeFromCoreAssembly(CoreTypeId.Void);
		if (parameterTypes != null)
		{
			_parameterTypes = new Type[parameterTypes.Length];
			for (int i = 0; i < parameterTypes.Length; i++)
			{
				ArgumentNullException.ThrowIfNull(_parameterTypes[i] = parameterTypes[i], "parameterTypes");
			}
		}
		else
		{
			_parameterTypes = Type.EmptyTypes;
		}
		_module = module;
		_containingType = arrayClass;
		_name = methodName;
		_callingConvention = callingConvention;
	}

	public override ParameterInfo[] GetParameters()
	{
		throw new NotSupportedException(System.SR.NotSupported_SymbolMethod);
	}

	public override MethodImplAttributes GetMethodImplementationFlags()
	{
		throw new NotSupportedException(System.SR.NotSupported_SymbolMethod);
	}

	public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
	{
		throw new NotSupportedException(System.SR.NotSupported_SymbolMethod);
	}

	public override MethodInfo GetBaseDefinition()
	{
		return this;
	}

	public override object[] GetCustomAttributes(bool inherit)
	{
		throw new NotSupportedException(System.SR.NotSupported_SymbolMethod);
	}

	public override object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		throw new NotSupportedException(System.SR.NotSupported_SymbolMethod);
	}

	public override bool IsDefined(Type attributeType, bool inherit)
	{
		throw new NotSupportedException(System.SR.NotSupported_SymbolMethod);
	}
}
