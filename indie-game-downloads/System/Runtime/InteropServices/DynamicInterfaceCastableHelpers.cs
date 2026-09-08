using System.Diagnostics;

namespace System.Runtime.InteropServices;

internal static class DynamicInterfaceCastableHelpers
{
	[StackTraceHidden]
	internal static bool IsInterfaceImplemented(IDynamicInterfaceCastable castable, RuntimeType interfaceType, bool throwIfNotImplemented)
	{
		bool num = castable.IsInterfaceImplemented(new RuntimeTypeHandle(interfaceType), throwIfNotImplemented);
		if (!num & throwIfNotImplemented)
		{
			throw new InvalidCastException(SR.Format(SR.InvalidCast_FromTo, castable.GetType(), interfaceType));
		}
		return num;
	}

	[StackTraceHidden]
	internal static RuntimeType GetInterfaceImplementation(IDynamicInterfaceCastable castable, RuntimeType interfaceType)
	{
		RuntimeTypeHandle interfaceImplementation = castable.GetInterfaceImplementation(new RuntimeTypeHandle(interfaceType));
		if (interfaceImplementation.Equals(default(RuntimeTypeHandle)))
		{
			throw new InvalidCastException(SR.Format(SR.InvalidCast_FromTo, castable.GetType(), interfaceType));
		}
		RuntimeType runtimeType = interfaceImplementation.GetRuntimeType();
		if (!runtimeType.IsActualInterface)
		{
			throw new InvalidOperationException(SR.Format(SR.IDynamicInterfaceCastable_NotInterface, runtimeType.ToString()));
		}
		if (!runtimeType.IsDefined(typeof(DynamicInterfaceCastableImplementationAttribute), inherit: false))
		{
			throw new InvalidOperationException(SR.Format(SR.IDynamicInterfaceCastable_MissingImplementationAttribute, runtimeType, "DynamicInterfaceCastableImplementationAttribute"));
		}
		if (!runtimeType.IsAssignableTo(interfaceType))
		{
			throw new InvalidOperationException(SR.Format(SR.IDynamicInterfaceCastable_DoesNotImplementRequested, runtimeType, interfaceType));
		}
		return runtimeType;
	}
}
