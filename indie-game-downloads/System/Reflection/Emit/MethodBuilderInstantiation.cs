using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.Reflection.Emit;

internal sealed class MethodBuilderInstantiation : MethodInfo
{
	internal readonly MethodInfo _method;

	private readonly Type[] _inst;

	public override MemberTypes MemberType => _method.MemberType;

	public override string Name => _method.Name;

	public override Type DeclaringType => _method.DeclaringType;

	public override Type ReflectedType => _method.ReflectedType;

	public override Module Module => _method.Module;

	public override RuntimeMethodHandle MethodHandle
	{
		get
		{
			throw new NotSupportedException(System.SR.NotSupported_DynamicModule);
		}
	}

	public override MethodAttributes Attributes => _method.Attributes;

	public override CallingConventions CallingConvention => _method.CallingConvention;

	public override bool IsGenericMethodDefinition => false;

	public override bool ContainsGenericParameters
	{
		get
		{
			for (int i = 0; i < _inst.Length; i++)
			{
				if (_inst[i].ContainsGenericParameters)
				{
					return true;
				}
			}
			if (DeclaringType != null && DeclaringType.ContainsGenericParameters)
			{
				return true;
			}
			return false;
		}
	}

	public override bool IsGenericMethod => true;

	public override Type ReturnType => _method.ReturnType;

	public override ParameterInfo ReturnParameter
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public override ICustomAttributeProvider ReturnTypeCustomAttributes
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	internal static MethodInfo MakeGenericMethod(MethodInfo method, Type[] inst)
	{
		if (!method.IsGenericMethodDefinition)
		{
			throw new InvalidOperationException();
		}
		return new System.Reflection.Emit.MethodBuilderInstantiation(method, inst);
	}

	internal MethodBuilderInstantiation(MethodInfo method, Type[] inst)
	{
		_method = method;
		_inst = inst;
	}

	public override object[] GetCustomAttributes(bool inherit)
	{
		return _method.GetCustomAttributes(inherit);
	}

	public override object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		return _method.GetCustomAttributes(attributeType, inherit);
	}

	public override bool IsDefined(Type attributeType, bool inherit)
	{
		return _method.IsDefined(attributeType, inherit);
	}

	public override ParameterInfo[] GetParameters()
	{
		return _method.GetParameters();
	}

	public override MethodImplAttributes GetMethodImplementationFlags()
	{
		return _method.GetMethodImplementationFlags();
	}

	public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
	{
		throw new NotSupportedException();
	}

	public override Type[] GetGenericArguments()
	{
		return _inst;
	}

	public override MethodInfo GetGenericMethodDefinition()
	{
		return _method;
	}

	[RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
	[RequiresUnreferencedCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
	public override MethodInfo MakeGenericMethod(params Type[] arguments)
	{
		throw new InvalidOperationException(System.SR.Format(System.SR.Arg_NotGenericMethodDefinition, this));
	}

	public override MethodInfo GetBaseDefinition()
	{
		throw new NotSupportedException();
	}
}
