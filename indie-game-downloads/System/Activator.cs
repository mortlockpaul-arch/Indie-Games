using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Runtime.Remoting;
using System.Threading;

namespace System;

public static class Activator
{
	[DebuggerHidden]
	[DebuggerStepThrough]
	public static object? CreateInstance([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type type, BindingFlags bindingAttr, Binder? binder, object?[]? args, CultureInfo? culture)
	{
		return CreateInstance(type, bindingAttr, binder, args, culture, null);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static object? CreateInstance([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type, params object?[]? args)
	{
		return CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, null, args, null, null);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static object? CreateInstance([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type, object?[]? args, object?[]? activationAttributes)
	{
		return CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, null, args, null, activationAttributes);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public static object? CreateInstance([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type type)
	{
		return CreateInstance(type, nonPublic: false);
	}

	[RequiresUnreferencedCode("Type and its constructor could be removed")]
	public static ObjectHandle? CreateInstanceFrom(string assemblyFile, string typeName)
	{
		return CreateInstanceFrom(assemblyFile, typeName, ignoreCase: false, BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, null, null, null, null);
	}

	[RequiresUnreferencedCode("Type and its constructor could be removed")]
	public static ObjectHandle? CreateInstanceFrom(string assemblyFile, string typeName, object?[]? activationAttributes)
	{
		return CreateInstanceFrom(assemblyFile, typeName, ignoreCase: false, BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, null, null, null, activationAttributes);
	}

	[RequiresUnreferencedCode("Type and its constructor could be removed")]
	public static ObjectHandle? CreateInstanceFrom(string assemblyFile, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder? binder, object?[]? args, CultureInfo? culture, object?[]? activationAttributes)
	{
		object obj = CreateInstance(Assembly.LoadFrom(assemblyFile).GetType(typeName, throwOnError: true, ignoreCase), bindingAttr, binder, args, culture, activationAttributes);
		if (obj == null)
		{
			return null;
		}
		return new ObjectHandle(obj);
	}

	public static object? CreateInstance([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type type, BindingFlags bindingAttr, Binder? binder, object?[]? args, CultureInfo? culture, object?[]? activationAttributes)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		if (type is TypeBuilder)
		{
			throw new NotSupportedException(SR.NotSupported_CreateInstanceWithTypeBuilder);
		}
		if ((bindingAttr & (BindingFlags)0xFF) == 0)
		{
			bindingAttr |= BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance;
		}
		if (activationAttributes != null && activationAttributes.Length != 0)
		{
			throw new PlatformNotSupportedException(SR.NotSupported_ActivAttr);
		}
		return ((type.UnderlyingSystemType as RuntimeType) ?? throw new ArgumentException(SR.Arg_MustBeType, "type")).CreateInstanceImpl(bindingAttr, binder, args, culture);
	}

	[RequiresUnreferencedCode("Type and its constructor could be removed")]
	public static ObjectHandle? CreateInstance(string assemblyName, string typeName)
	{
		StackCrawlMark stackMark = StackCrawlMark.LookForMyCaller;
		return CreateInstanceInternal(assemblyName, typeName, ignoreCase: false, BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, null, null, null, null, ref stackMark);
	}

	[RequiresUnreferencedCode("Type and its constructor could be removed")]
	public static ObjectHandle? CreateInstance(string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder? binder, object?[]? args, CultureInfo? culture, object?[]? activationAttributes)
	{
		StackCrawlMark stackMark = StackCrawlMark.LookForMyCaller;
		return CreateInstanceInternal(assemblyName, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, ref stackMark);
	}

	[RequiresUnreferencedCode("Type and its constructor could be removed")]
	public static ObjectHandle? CreateInstance(string assemblyName, string typeName, object?[]? activationAttributes)
	{
		StackCrawlMark stackMark = StackCrawlMark.LookForMyCaller;
		return CreateInstanceInternal(assemblyName, typeName, ignoreCase: false, BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, null, null, null, activationAttributes, ref stackMark);
	}

	public static object? CreateInstance([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type type, bool nonPublic)
	{
		return CreateInstance(type, nonPublic, wrapExceptions: true);
	}

	internal static object CreateInstance(Type type, bool nonPublic, bool wrapExceptions)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return ((type.UnderlyingSystemType as RuntimeType) ?? throw new ArgumentException(SR.Arg_MustBeType, "type")).CreateInstanceDefaultCtor(!nonPublic, wrapExceptions);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "Implementation detail of Activator that linker intrinsically recognizes")]
	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2072:UnrecognizedReflectionPattern", Justification = "Implementation detail of Activator that linker intrinsically recognizes")]
	private static ObjectHandle CreateInstanceInternal(string assemblyString, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, ref StackCrawlMark stackMark)
	{
		RuntimeAssembly runtimeAssembly = ((assemblyString != null) ? RuntimeAssembly.InternalLoad(new AssemblyName(assemblyString), ref stackMark, AssemblyLoadContext.CurrentContextualReflectionContext) : Assembly.GetExecutingAssembly(ref stackMark));
		object obj = CreateInstance(runtimeAssembly.GetType(typeName, throwOnError: true, ignoreCase), bindingAttr, binder, args, culture, activationAttributes);
		if (obj == null)
		{
			return null;
		}
		return new ObjectHandle(obj);
	}

	[Intrinsic]
	public static T CreateInstance<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>() where T : allows ref struct
	{
		RuntimeType runtimeType = (RuntimeType)typeof(T);
		if (!runtimeType.IsValueType)
		{
			object source = runtimeType.CreateInstanceOfT();
			return Unsafe.As<object, T>(ref source);
		}
		T source2 = default(T);
		runtimeType.CallDefaultStructConstructor(ref Unsafe.As<T, byte>(ref source2));
		return source2;
	}

	private static T CreateDefaultInstance<T>() where T : struct
	{
		return default(T);
	}
}
