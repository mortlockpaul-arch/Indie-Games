using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Reflection;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class NewLateBinding
{
	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static object LateCall(object Instance, Type Type, string MemberName, object[] Arguments, string[] ArgumentNames, Type[] TypeArguments, bool[] CopyBack, bool IgnoreReturn)
	{
		if (Arguments == null)
		{
			Arguments = Symbols.NoArguments;
		}
		if (ArgumentNames == null)
		{
			ArgumentNames = Symbols.NoArgumentNames;
		}
		if (TypeArguments == null)
		{
			TypeArguments = Symbols.NoTypeArguments;
		}
		Symbols.Container container = (((object)Type == null) ? new Symbols.Container(Instance) : new Symbols.Container(Type));
		if (container.IsCOMObject && !container.IsWindowsRuntimeObject)
		{
			return LateBinding.InternalLateCall(Instance, Type, MemberName, Arguments, ArgumentNames, CopyBack, IgnoreReturn);
		}
		IDynamicMetaObjectProvider dynamicMetaObjectProvider = IDOUtils.TryCastToIDMOP(Instance);
		if (dynamicMetaObjectProvider != null && TypeArguments == Symbols.NoTypeArguments)
		{
			return IDOBinder.IDOCall(dynamicMetaObjectProvider, MemberName, Arguments, ArgumentNames, CopyBack, IgnoreReturn);
		}
		return ObjectLateCall(Instance, Type, MemberName, Arguments, ArgumentNames, TypeArguments, CopyBack, IgnoreReturn);
	}

	[Obsolete("FallbackCall has been deprecated and is not supported.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static object FallbackCall(object Instance, string MemberName, object[] Arguments, string[] ArgumentNames, bool IgnoreReturn)
	{
		return ObjectLateCall(Instance, null, MemberName, Arguments, ArgumentNames, Symbols.NoTypeArguments, IDOBinder.GetCopyBack(), IgnoreReturn);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	private static object ObjectLateCall(object instance, Type type, string memberName, object[] arguments, string[] argumentNames, Type[] typeArguments, bool[] copyBack, bool ignoreReturn)
	{
		Symbols.Container baseReference = (((object)type == null) ? new Symbols.Container(instance) : new Symbols.Container(type));
		BindingFlags bindingFlags = BindingFlags.InvokeMethod | BindingFlags.GetProperty;
		if (ignoreReturn)
		{
			bindingFlags |= BindingFlags.IgnoreReturn;
		}
		OverloadResolution.ResolutionFailure failure = default(OverloadResolution.ResolutionFailure);
		return CallMethod(baseReference, memberName, arguments, argumentNames, typeArguments, copyBack, bindingFlags, reportErrors: true, ref failure);
	}

	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	internal static bool CanBindCall(object instance, string memberName, object[] arguments, string[] argumentNames, bool ignoreReturn)
	{
		Symbols.Container container = new Symbols.Container(instance);
		BindingFlags bindingFlags = BindingFlags.InvokeMethod | BindingFlags.GetProperty;
		if (ignoreReturn)
		{
			bindingFlags |= BindingFlags.IgnoreReturn;
		}
		MemberInfo[] members = container.GetMembers(ref memberName, reportErrors: false);
		if (members == null || members.Length == 0)
		{
			return false;
		}
		OverloadResolution.ResolutionFailure failure = default(OverloadResolution.ResolutionFailure);
		ResolveCall(container, memberName, members, arguments, argumentNames, Symbols.NoTypeArguments, bindingFlags, reportErrors: false, ref failure);
		return failure == OverloadResolution.ResolutionFailure.None;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static object LateCallInvokeDefault(object Instance, object[] Arguments, string[] ArgumentNames, bool ReportErrors)
	{
		return InternalLateInvokeDefault(Instance, Arguments, ArgumentNames, ReportErrors, IDOBinder.GetCopyBack());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static object LateGetInvokeDefault(object Instance, object[] Arguments, string[] ArgumentNames, bool ReportErrors)
	{
		if (IDOUtils.TryCastToIDMOP(Instance) != null || (Arguments != null && Arguments.Length > 0))
		{
			return InternalLateInvokeDefault(Instance, Arguments, ArgumentNames, ReportErrors, IDOBinder.GetCopyBack());
		}
		return Instance;
	}

	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	private static object InternalLateInvokeDefault(object instance, object[] arguments, string[] argumentNames, bool reportErrors, bool[] copyBack)
	{
		IDynamicMetaObjectProvider dynamicMetaObjectProvider = IDOUtils.TryCastToIDMOP(instance);
		if (dynamicMetaObjectProvider != null)
		{
			return IDOBinder.IDOInvokeDefault(dynamicMetaObjectProvider, arguments, argumentNames, reportErrors, copyBack);
		}
		return ObjectLateInvokeDefault(instance, arguments, argumentNames, reportErrors, copyBack);
	}

	[Obsolete("FallbackInvokeDefault1 has been deprecated and is not supported.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static object FallbackInvokeDefault1(object Instance, object[] Arguments, string[] ArgumentNames, bool ReportErrors)
	{
		return IDOBinder.IDOFallbackInvokeDefault((IDynamicMetaObjectProvider)Instance, Arguments, ArgumentNames, ReportErrors, IDOBinder.GetCopyBack());
	}

	[Obsolete("FallbackInvokeDefault2 has been deprecated and is not supported.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static object FallbackInvokeDefault2(object Instance, object[] Arguments, string[] ArgumentNames, bool ReportErrors)
	{
		return ObjectLateInvokeDefault(Instance, Arguments, ArgumentNames, ReportErrors, IDOBinder.GetCopyBack());
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	private static object ObjectLateInvokeDefault(object instance, object[] arguments, string[] argumentNames, bool reportErrors, bool[] copyBack)
	{
		Symbols.Container container = new Symbols.Container(instance);
		OverloadResolution.ResolutionFailure failure = default(OverloadResolution.ResolutionFailure);
		object result = InternalLateIndexGet(instance, arguments, argumentNames, reportErrors || arguments.Length != 0 || container.IsArray, ref failure, copyBack);
		if (failure != OverloadResolution.ResolutionFailure.None)
		{
			return instance;
		}
		return result;
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static object LateIndexGet(object Instance, object[] Arguments, string[] ArgumentNames)
	{
		return InternalLateInvokeDefault(Instance, Arguments, ArgumentNames, reportErrors: true, null);
	}

	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	private static object LateIndexGet(object instance, object[] arguments, string[] argumentNames, bool[] copyBack)
	{
		return InternalLateInvokeDefault(instance, arguments, argumentNames, reportErrors: true, copyBack);
	}

	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	private static object InternalLateIndexGet(object instance, object[] arguments, string[] argumentNames, bool reportErrors, ref OverloadResolution.ResolutionFailure failure, bool[] copyBack)
	{
		failure = OverloadResolution.ResolutionFailure.None;
		if (arguments == null)
		{
			arguments = Symbols.NoArguments;
		}
		if (argumentNames == null)
		{
			argumentNames = Symbols.NoArgumentNames;
		}
		Symbols.Container container = new Symbols.Container(instance);
		if (container.IsCOMObject && !container.IsWindowsRuntimeObject)
		{
			return LateBinding.LateIndexGet(instance, arguments, argumentNames);
		}
		if (container.IsArray)
		{
			if (argumentNames.Length > 0)
			{
				failure = OverloadResolution.ResolutionFailure.InvalidArgument;
				if (reportErrors)
				{
					throw new ArgumentException(System.SR.Argument_InvalidNamedArgs);
				}
				return null;
			}
			ResetCopyback(copyBack);
			return container.GetArrayValue(arguments);
		}
		return CallMethod(container, "", arguments, argumentNames, Symbols.NoTypeArguments, copyBack, BindingFlags.InvokeMethod | BindingFlags.GetProperty, reportErrors, ref failure);
	}

	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	internal static bool CanBindInvokeDefault(object instance, object[] arguments, string[] argumentNames, bool reportErrors)
	{
		Symbols.Container container = new Symbols.Container(instance);
		reportErrors = reportErrors || arguments.Length != 0 || container.IsArray;
		if (!reportErrors)
		{
			return true;
		}
		if (container.IsArray)
		{
			return argumentNames.Length == 0;
		}
		return CanBindCall(instance, "", arguments, argumentNames, ignoreReturn: false);
	}

	internal static void ResetCopyback(bool[] copyBack)
	{
		checked
		{
			if (copyBack != null)
			{
				int num = copyBack.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					copyBack[i] = false;
				}
			}
		}
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static object LateGet(object Instance, Type Type, string MemberName, object[] Arguments, string[] ArgumentNames, Type[] TypeArguments, bool[] CopyBack)
	{
		if (Arguments == null)
		{
			Arguments = Symbols.NoArguments;
		}
		if (ArgumentNames == null)
		{
			ArgumentNames = Symbols.NoArgumentNames;
		}
		if (TypeArguments == null)
		{
			TypeArguments = Symbols.NoTypeArguments;
		}
		Symbols.Container container = (((object)Type == null) ? new Symbols.Container(Instance) : new Symbols.Container(Type));
		if (container.IsCOMObject && !container.IsWindowsRuntimeObject)
		{
			return LateBinding.LateGet(Instance, Type, MemberName, Arguments, ArgumentNames, CopyBack);
		}
		_ = 256;
		_ = 4096;
		IDynamicMetaObjectProvider dynamicMetaObjectProvider = IDOUtils.TryCastToIDMOP(Instance);
		if (dynamicMetaObjectProvider != null && TypeArguments == Symbols.NoTypeArguments)
		{
			return IDOBinder.IDOGet(dynamicMetaObjectProvider, MemberName, Arguments, ArgumentNames, CopyBack);
		}
		return ObjectLateGet(Instance, Type, MemberName, Arguments, ArgumentNames, TypeArguments, CopyBack);
	}

	[Obsolete("FallbackGet has been deprecated and is not supported.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static object FallbackGet(object Instance, string MemberName, object[] Arguments, string[] ArgumentNames)
	{
		return ObjectLateGet(Instance, null, MemberName, Arguments, ArgumentNames, Symbols.NoTypeArguments, IDOBinder.GetCopyBack());
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	private static object ObjectLateGet(object instance, Type type, string memberName, object[] arguments, string[] argumentNames, Type[] typeArguments, bool[] copyBack)
	{
		Symbols.Container container = (((object)type == null) ? new Symbols.Container(instance) : new Symbols.Container(type));
		BindingFlags bindingFlags = BindingFlags.InvokeMethod | BindingFlags.GetProperty;
		MemberInfo[] members = container.GetMembers(ref memberName, reportErrors: true);
		if (members[0].MemberType == MemberTypes.Field)
		{
			if (typeArguments.Length > 0)
			{
				throw new ArgumentException(System.SR.Argument_InvalidValue);
			}
			object fieldValue = container.GetFieldValue((FieldInfo)members[0]);
			if (arguments.Length == 0)
			{
				return fieldValue;
			}
			return LateIndexGet(fieldValue, arguments, argumentNames, copyBack);
		}
		if (argumentNames.Length > arguments.Length || (copyBack != null && copyBack.Length != arguments.Length))
		{
			throw new ArgumentException(System.SR.Argument_InvalidValue);
		}
		OverloadResolution.ResolutionFailure failure = default(OverloadResolution.ResolutionFailure);
		Symbols.Method targetProcedure = ResolveCall(container, memberName, members, arguments, argumentNames, typeArguments, bindingFlags, reportErrors: false, ref failure);
		if (failure == OverloadResolution.ResolutionFailure.None)
		{
			return container.InvokeMethod(targetProcedure, arguments, copyBack, bindingFlags);
		}
		if (arguments.Length > 0 && members.Length == 1 && IsZeroArgumentCall(members[0]))
		{
			targetProcedure = ResolveCall(container, memberName, members, Symbols.NoArguments, Symbols.NoArgumentNames, typeArguments, bindingFlags, reportErrors: false, ref failure);
			if (failure == OverloadResolution.ResolutionFailure.None)
			{
				object obj = container.InvokeMethod(targetProcedure, Symbols.NoArguments, null, bindingFlags);
				if (obj == null)
				{
					throw new MissingMemberException(System.SR.Format(System.SR.IntermediateLateBoundNothingResult1, targetProcedure.ToString(), container.VBFriendlyName));
				}
				obj = InternalLateIndexGet(obj, arguments, argumentNames, reportErrors: false, ref failure, copyBack);
				if (failure == OverloadResolution.ResolutionFailure.None)
				{
					return obj;
				}
			}
		}
		ResolveCall(container, memberName, members, arguments, argumentNames, typeArguments, bindingFlags, reportErrors: true, ref failure);
		throw new InternalErrorException();
	}

	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	internal static bool CanBindGet(object instance, string memberName, object[] arguments, string[] argumentNames)
	{
		Symbols.Container container = new Symbols.Container(instance);
		BindingFlags lookupFlags = BindingFlags.InvokeMethod | BindingFlags.GetProperty;
		MemberInfo[] members = container.GetMembers(ref memberName, reportErrors: false);
		if (members == null || members.Length == 0)
		{
			return false;
		}
		if (members[0].MemberType == MemberTypes.Field)
		{
			return true;
		}
		OverloadResolution.ResolutionFailure failure = default(OverloadResolution.ResolutionFailure);
		ResolveCall(container, memberName, members, arguments, argumentNames, Symbols.NoTypeArguments, lookupFlags, reportErrors: false, ref failure);
		if (failure == OverloadResolution.ResolutionFailure.None)
		{
			return true;
		}
		if (arguments.Length > 0 && members.Length == 1 && IsZeroArgumentCall(members[0]))
		{
			ResolveCall(container, memberName, members, Symbols.NoArguments, Symbols.NoArgumentNames, Symbols.NoTypeArguments, lookupFlags, reportErrors: false, ref failure);
			if (failure == OverloadResolution.ResolutionFailure.None)
			{
				return true;
			}
		}
		return false;
	}

	internal static bool IsZeroArgumentCall(MemberInfo member)
	{
		if (member.MemberType != MemberTypes.Method || ((MethodInfo)member).GetParameters().Length != 0)
		{
			if (member.MemberType == MemberTypes.Property)
			{
				return ((PropertyInfo)member).GetIndexParameters().Length == 0;
			}
			return false;
		}
		return true;
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static void LateIndexSetComplex(object Instance, object[] Arguments, string[] ArgumentNames, bool OptimisticSet, bool RValueBase)
	{
		IDynamicMetaObjectProvider dynamicMetaObjectProvider = IDOUtils.TryCastToIDMOP(Instance);
		if (dynamicMetaObjectProvider != null)
		{
			IDOBinder.IDOIndexSetComplex(dynamicMetaObjectProvider, Arguments, ArgumentNames, OptimisticSet, RValueBase);
		}
		else
		{
			ObjectLateIndexSetComplex(Instance, Arguments, ArgumentNames, OptimisticSet, RValueBase);
		}
	}

	[Obsolete("FallbackIndexSetComplex has been deprecated and is not supported.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static void FallbackIndexSetComplex(object Instance, object[] Arguments, string[] ArgumentNames, bool OptimisticSet, bool RValueBase)
	{
		ObjectLateIndexSetComplex(Instance, Arguments, ArgumentNames, OptimisticSet, RValueBase);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	internal static void ObjectLateIndexSetComplex(object instance, object[] arguments, string[] argumentNames, bool optimisticSet, bool rValueBase)
	{
		if (arguments == null)
		{
			arguments = Symbols.NoArguments;
		}
		if (argumentNames == null)
		{
			argumentNames = Symbols.NoArgumentNames;
		}
		Symbols.Container container = new Symbols.Container(instance);
		if (container.IsArray)
		{
			if (argumentNames.Length > 0)
			{
				throw new ArgumentException(System.SR.Argument_InvalidNamedArgs);
			}
			container.SetArrayValue(arguments);
			return;
		}
		if (argumentNames.Length > arguments.Length)
		{
			throw new ArgumentException(System.SR.Argument_InvalidValue);
		}
		if (arguments.Length < 1)
		{
			throw new ArgumentException(System.SR.Argument_InvalidValue);
		}
		string memberName = "";
		if (container.IsCOMObject && !container.IsWindowsRuntimeObject)
		{
			LateBinding.LateIndexSetComplex(instance, arguments, argumentNames, optimisticSet, rValueBase);
			return;
		}
		BindingFlags bindingFlags = BindingFlags.SetProperty;
		MemberInfo[] members = container.GetMembers(ref memberName, reportErrors: true);
		OverloadResolution.ResolutionFailure failure = default(OverloadResolution.ResolutionFailure);
		Symbols.Method targetProcedure = ResolveCall(container, memberName, members, arguments, argumentNames, Symbols.NoTypeArguments, bindingFlags, reportErrors: false, ref failure);
		if (failure == OverloadResolution.ResolutionFailure.None)
		{
			if (rValueBase && container.IsValueType)
			{
				throw new Exception(System.SR.Format(System.SR.RValueBaseForValueType, container.VBFriendlyName, container.VBFriendlyName));
			}
			container.InvokeMethod(targetProcedure, arguments, null, bindingFlags);
		}
		else if (!optimisticSet)
		{
			ResolveCall(container, memberName, members, arguments, argumentNames, Symbols.NoTypeArguments, bindingFlags, reportErrors: true, ref failure);
			throw new InternalErrorException();
		}
	}

	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	internal static bool CanIndexSetComplex(object instance, object[] arguments, string[] argumentNames, bool optimisticSet, bool rValueBase)
	{
		Symbols.Container container = new Symbols.Container(instance);
		if (container.IsArray)
		{
			return argumentNames.Length == 0;
		}
		string memberName = "";
		BindingFlags lookupFlags = BindingFlags.SetProperty;
		MemberInfo[] members = container.GetMembers(ref memberName, reportErrors: false);
		if (members == null || members.Length == 0)
		{
			return false;
		}
		OverloadResolution.ResolutionFailure failure = default(OverloadResolution.ResolutionFailure);
		ResolveCall(container, memberName, members, arguments, argumentNames, Symbols.NoTypeArguments, lookupFlags, reportErrors: false, ref failure);
		if (failure == OverloadResolution.ResolutionFailure.None)
		{
			if (rValueBase && container.IsValueType)
			{
				return false;
			}
			return true;
		}
		return optimisticSet;
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static void LateIndexSet(object Instance, object[] Arguments, string[] ArgumentNames)
	{
		IDynamicMetaObjectProvider dynamicMetaObjectProvider = IDOUtils.TryCastToIDMOP(Instance);
		if (dynamicMetaObjectProvider != null)
		{
			IDOBinder.IDOIndexSet(dynamicMetaObjectProvider, Arguments, ArgumentNames);
		}
		else
		{
			ObjectLateIndexSet(Instance, Arguments, ArgumentNames);
		}
	}

	[Obsolete("FallbackIndexSet has been deprecated and is not supported.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static void FallbackIndexSet(object Instance, object[] Arguments, string[] ArgumentNames)
	{
		ObjectLateIndexSet(Instance, Arguments, ArgumentNames);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	private static void ObjectLateIndexSet(object Instance, object[] Arguments, string[] ArgumentNames)
	{
		ObjectLateIndexSetComplex(Instance, Arguments, ArgumentNames, optimisticSet: false, rValueBase: false);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static void LateSetComplex(object Instance, Type Type, string MemberName, object[] Arguments, string[] ArgumentNames, Type[] TypeArguments, bool OptimisticSet, bool RValueBase)
	{
		IDynamicMetaObjectProvider dynamicMetaObjectProvider = IDOUtils.TryCastToIDMOP(Instance);
		if (dynamicMetaObjectProvider != null && TypeArguments == null)
		{
			IDOBinder.IDOSetComplex(dynamicMetaObjectProvider, MemberName, Arguments, ArgumentNames, OptimisticSet, RValueBase);
		}
		else
		{
			ObjectLateSetComplex(Instance, Type, MemberName, Arguments, ArgumentNames, TypeArguments, OptimisticSet, RValueBase);
		}
	}

	[Obsolete("FallbackSetComplex has been deprecated and is not supported.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static void FallbackSetComplex(object Instance, string MemberName, object[] Arguments, bool OptimisticSet, bool RValueBase)
	{
		ObjectLateSetComplex(Instance, null, MemberName, Arguments, Array.Empty<string>(), Symbols.NoTypeArguments, OptimisticSet, RValueBase);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	internal static void ObjectLateSetComplex(object instance, Type type, string memberName, object[] arguments, string[] argumentNames, Type[] typeArguments, bool optimisticSet, bool rValueBase)
	{
		LateSet(instance, type, memberName, arguments, argumentNames, typeArguments, optimisticSet, rValueBase, (CallType)0);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static void LateSet(object Instance, Type Type, string MemberName, object[] Arguments, string[] ArgumentNames, Type[] TypeArguments)
	{
		IDynamicMetaObjectProvider dynamicMetaObjectProvider = IDOUtils.TryCastToIDMOP(Instance);
		if (dynamicMetaObjectProvider != null && TypeArguments == null)
		{
			IDOBinder.IDOSet(dynamicMetaObjectProvider, MemberName, ArgumentNames, Arguments);
		}
		else
		{
			ObjectLateSet(Instance, Type, MemberName, Arguments, ArgumentNames, TypeArguments);
		}
	}

	[Obsolete("FallbackSet has been deprecated and is not supported.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static void FallbackSet(object Instance, string MemberName, object[] Arguments)
	{
		ObjectLateSet(Instance, null, MemberName, Arguments, Symbols.NoArgumentNames, Symbols.NoTypeArguments);
	}

	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	internal static void ObjectLateSet(object instance, Type type, string memberName, object[] arguments, string[] argumentNames, Type[] typeArguments)
	{
		LateSet(instance, type, memberName, arguments, argumentNames, typeArguments, OptimisticSet: false, RValueBase: false, (CallType)0);
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	public static void LateSet(object Instance, Type Type, string MemberName, object[] Arguments, string[] ArgumentNames, Type[] TypeArguments, bool OptimisticSet, bool RValueBase, CallType CallType)
	{
		if (Arguments == null)
		{
			Arguments = Symbols.NoArguments;
		}
		if (ArgumentNames == null)
		{
			ArgumentNames = Symbols.NoArgumentNames;
		}
		if (TypeArguments == null)
		{
			TypeArguments = Symbols.NoTypeArguments;
		}
		Symbols.Container container = (((object)Type == null) ? new Symbols.Container(Instance) : new Symbols.Container(Type));
		if (container.IsCOMObject && !container.IsWindowsRuntimeObject)
		{
			try
			{
				LateBinding.InternalLateSet(Instance, ref Type, MemberName, Arguments, ArgumentNames, OptimisticSet, CallType);
				if (RValueBase & Type.IsValueType)
				{
					throw new Exception(Utils.GetResourceString(System.SR.RValueBaseForValueType, container.VBFriendlyName, container.VBFriendlyName));
				}
				return;
			}
			catch (MissingMemberException) when (OptimisticSet)
			{
				return;
			}
		}
		MemberInfo[] members = container.GetMembers(ref MemberName, !OptimisticSet);
		if ((members.Length == 0) & OptimisticSet)
		{
			return;
		}
		if (members[0].MemberType == MemberTypes.Field)
		{
			if (TypeArguments.Length > 0)
			{
				throw new ArgumentException(System.SR.Argument_InvalidValue);
			}
			if (Arguments.Length == 1)
			{
				if (RValueBase && container.IsValueType)
				{
					throw new Exception(System.SR.Format(System.SR.RValueBaseForValueType, container.VBFriendlyName, container.VBFriendlyName));
				}
				container.SetFieldValue((FieldInfo)members[0], Arguments[0]);
			}
			else
			{
				LateIndexSetComplex(container.GetFieldValue((FieldInfo)members[0]), Arguments, ArgumentNames, OptimisticSet, RValueBase: true);
			}
			return;
		}
		BindingFlags bindingFlags = BindingFlags.SetProperty;
		if (ArgumentNames.Length > Arguments.Length)
		{
			throw new ArgumentException(System.SR.Argument_InvalidValue);
		}
		OverloadResolution.ResolutionFailure failure = default(OverloadResolution.ResolutionFailure);
		if (TypeArguments.Length == 0)
		{
			Symbols.Method targetProcedure = ResolveCall(container, MemberName, members, Arguments, ArgumentNames, Symbols.NoTypeArguments, bindingFlags, reportErrors: false, ref failure);
			if (failure == OverloadResolution.ResolutionFailure.None)
			{
				if (RValueBase && container.IsValueType)
				{
					throw new Exception(System.SR.Format(System.SR.RValueBaseForValueType, container.VBFriendlyName, container.VBFriendlyName));
				}
				container.InvokeMethod(targetProcedure, Arguments, null, bindingFlags);
				return;
			}
		}
		BindingFlags bindingFlags2 = BindingFlags.InvokeMethod | BindingFlags.GetProperty;
		if (failure == OverloadResolution.ResolutionFailure.None || failure == OverloadResolution.ResolutionFailure.MissingMember)
		{
			Symbols.Method targetProcedure = ResolveCall(container, MemberName, members, Symbols.NoArguments, Symbols.NoArgumentNames, TypeArguments, bindingFlags2, reportErrors: false, ref failure);
			if (failure == OverloadResolution.ResolutionFailure.None)
			{
				LateIndexSetComplex(container.InvokeMethod(targetProcedure, Symbols.NoArguments, null, bindingFlags2) ?? throw new MissingMemberException(System.SR.Format(System.SR.IntermediateLateBoundNothingResult1, targetProcedure.ToString(), container.VBFriendlyName)), Arguments, ArgumentNames, OptimisticSet, RValueBase: true);
				return;
			}
		}
		if (OptimisticSet)
		{
			return;
		}
		if (TypeArguments.Length == 0)
		{
			ResolveCall(container, MemberName, members, Arguments, ArgumentNames, TypeArguments, bindingFlags, reportErrors: true, ref failure);
		}
		else
		{
			ResolveCall(container, MemberName, members, Symbols.NoArguments, Symbols.NoArgumentNames, TypeArguments, bindingFlags2, reportErrors: true, ref failure);
		}
		throw new InternalErrorException();
	}

	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	internal static bool CanBindSet(object instance, string memberName, object value, bool optimisticSet, bool rValueBase)
	{
		Symbols.Container container = new Symbols.Container(instance);
		object[] array = new object[1] { value };
		MemberInfo[] members = container.GetMembers(ref memberName, reportErrors: false);
		if (members == null || members.Length == 0)
		{
			return false;
		}
		if (members[0].MemberType == MemberTypes.Field)
		{
			if (array.Length == 1 && rValueBase && container.IsValueType)
			{
				return false;
			}
			return true;
		}
		OverloadResolution.ResolutionFailure failure = default(OverloadResolution.ResolutionFailure);
		ResolveCall(container, memberName, members, array, Symbols.NoArgumentNames, Symbols.NoTypeArguments, BindingFlags.SetProperty, reportErrors: false, ref failure);
		if (failure == OverloadResolution.ResolutionFailure.None)
		{
			if (rValueBase && container.IsValueType)
			{
				return false;
			}
			return true;
		}
		BindingFlags lookupFlags = BindingFlags.InvokeMethod | BindingFlags.GetProperty;
		if (failure == OverloadResolution.ResolutionFailure.MissingMember)
		{
			ResolveCall(container, memberName, members, Symbols.NoArguments, Symbols.NoArgumentNames, Symbols.NoTypeArguments, lookupFlags, reportErrors: false, ref failure);
			if (failure == OverloadResolution.ResolutionFailure.None)
			{
				return true;
			}
		}
		return optimisticSet;
	}

	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	private static object CallMethod(Symbols.Container baseReference, string methodName, object[] arguments, string[] argumentNames, Type[] typeArguments, bool[] copyBack, BindingFlags invocationFlags, bool reportErrors, ref OverloadResolution.ResolutionFailure failure)
	{
		failure = OverloadResolution.ResolutionFailure.None;
		if (argumentNames.Length > arguments.Length || (copyBack != null && copyBack.Length != arguments.Length))
		{
			failure = OverloadResolution.ResolutionFailure.InvalidArgument;
			if (reportErrors)
			{
				throw new ArgumentException(System.SR.Argument_InvalidValue);
			}
			return null;
		}
		if (Symbols.HasFlag(invocationFlags, BindingFlags.SetProperty) && arguments.Length < 1)
		{
			failure = OverloadResolution.ResolutionFailure.InvalidArgument;
			if (reportErrors)
			{
				throw new ArgumentException(System.SR.Argument_InvalidValue);
			}
			return null;
		}
		MemberInfo[] members = baseReference.GetMembers(ref methodName, reportErrors);
		if (members == null || members.Length == 0)
		{
			failure = OverloadResolution.ResolutionFailure.MissingMember;
			if (reportErrors)
			{
				members = baseReference.GetMembers(ref methodName, reportErrors: true);
			}
			return null;
		}
		Symbols.Method targetProcedure = ResolveCall(baseReference, methodName, members, arguments, argumentNames, typeArguments, invocationFlags, reportErrors, ref failure);
		if (failure == OverloadResolution.ResolutionFailure.None)
		{
			return baseReference.InvokeMethod(targetProcedure, arguments, copyBack, invocationFlags);
		}
		return null;
	}

	internal static MethodInfo MatchesPropertyRequirements(Symbols.Method targetProcedure, BindingFlags flags)
	{
		PropertyInfo propertyInfo = targetProcedure.AsProperty();
		if (Symbols.HasFlag(flags, BindingFlags.SetProperty))
		{
			return HasIsExternalInitModifier(propertyInfo.GetSetMethod()) ? null : propertyInfo.GetSetMethod();
		}
		return propertyInfo.GetGetMethod();
	}

	internal static Exception ReportPropertyMismatch(Symbols.Method targetProcedure, BindingFlags flags)
	{
		PropertyInfo propertyInfo = targetProcedure.AsProperty();
		if (Symbols.HasFlag(flags, BindingFlags.SetProperty))
		{
			return new MissingMemberException(System.SR.Format(System.SR.NoSetProperty1, propertyInfo.Name));
		}
		return new MissingMemberException(System.SR.Format(System.SR.NoGetProperty1, propertyInfo.Name));
	}

	private static bool HasIsExternalInitModifier(MethodInfo method)
	{
		Type[] array = method?.ReturnParameter.GetRequiredCustomModifiers();
		if (array != null)
		{
			Type[] array2 = array;
			foreach (Type type in array2)
			{
				if (Operators.CompareString(type.Name, "IsExternalInit", TextCompare: false) == 0 && !type.IsNested && Operators.CompareString(type.Namespace, "System.Runtime.CompilerServices", TextCompare: false) == 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	internal static Symbols.Method ResolveCall(Symbols.Container baseReference, string methodName, MemberInfo[] members, object[] arguments, string[] argumentNames, Type[] typeArguments, BindingFlags lookupFlags, bool reportErrors, ref OverloadResolution.ResolutionFailure failure)
	{
		failure = OverloadResolution.ResolutionFailure.None;
		if (members[0].MemberType != MemberTypes.Method && members[0].MemberType != MemberTypes.Property)
		{
			failure = OverloadResolution.ResolutionFailure.InvalidTarget;
			if (reportErrors)
			{
				throw new ArgumentException(System.SR.Format(System.SR.ExpressionNotProcedure, methodName, baseReference.VBFriendlyName));
			}
			return null;
		}
		int num = arguments.Length;
		object obj = null;
		checked
		{
			if (Symbols.HasFlag(lookupFlags, BindingFlags.SetProperty))
			{
				if (arguments.Length == 0)
				{
					failure = OverloadResolution.ResolutionFailure.InvalidArgument;
					if (reportErrors)
					{
						throw new InvalidCastException(System.SR.Format(System.SR.PropertySetMissingArgument1, methodName));
					}
					return null;
				}
				object[] array = arguments;
				arguments = new object[num - 2 + 1];
				Array.Copy(array, arguments, arguments.Length);
				obj = array[num - 1];
			}
			Symbols.Method method = OverloadResolution.ResolveOverloadedCall(methodName, members, arguments, argumentNames, typeArguments, lookupFlags, reportErrors, ref failure, baseReference);
			if (failure != OverloadResolution.ResolutionFailure.None)
			{
				return null;
			}
			if (!method.ArgumentsValidated && !OverloadResolution.CanMatchArguments(method, arguments, argumentNames, typeArguments, rejectNarrowingConversions: false, null))
			{
				failure = OverloadResolution.ResolutionFailure.InvalidArgument;
				if (reportErrors)
				{
					string text = "";
					List<string> list = new List<string>();
					OverloadResolution.CanMatchArguments(method, arguments, argumentNames, typeArguments, rejectNarrowingConversions: false, list);
					foreach (string item in list)
					{
						text = text + "\r\n    " + item;
					}
					text = System.SR.Format(System.SR.MatchArgumentFailure2, method.ToString(), text);
					throw new InvalidCastException(text);
				}
				return null;
			}
			if (method.IsProperty)
			{
				if ((object)MatchesPropertyRequirements(method, lookupFlags) == null)
				{
					failure = OverloadResolution.ResolutionFailure.InvalidTarget;
					if (reportErrors)
					{
						throw ReportPropertyMismatch(method, lookupFlags);
					}
					return null;
				}
			}
			else if (Symbols.HasFlag(lookupFlags, BindingFlags.SetProperty))
			{
				failure = OverloadResolution.ResolutionFailure.InvalidTarget;
				if (reportErrors)
				{
					throw new MissingMemberException(System.SR.Format(System.SR.MethodAssignment1, method.AsMethod().Name));
				}
				return null;
			}
			if (Symbols.HasFlag(lookupFlags, BindingFlags.SetProperty))
			{
				ParameterInfo[] parameters = GetCallTarget(method, lookupFlags).GetParameters();
				ParameterInfo parameter = parameters[parameters.Length - 1];
				object argument = obj;
				bool requiresNarrowingConversion = false;
				bool allNarrowingIsFromObject = false;
				if (!OverloadResolution.CanPassToParameter(method, argument, parameter, isExpandedParamArray: false, rejectNarrowingConversions: false, null, ref requiresNarrowingConversion, ref allNarrowingIsFromObject))
				{
					failure = OverloadResolution.ResolutionFailure.InvalidArgument;
					if (reportErrors)
					{
						string text2 = "";
						List<string> list2 = new List<string>();
						object argument2 = obj;
						allNarrowingIsFromObject = false;
						requiresNarrowingConversion = false;
						OverloadResolution.CanPassToParameter(method, argument2, parameter, isExpandedParamArray: false, rejectNarrowingConversions: false, list2, ref allNarrowingIsFromObject, ref requiresNarrowingConversion);
						foreach (string item2 in list2)
						{
							text2 = text2 + "\r\n    " + item2;
						}
						text2 = System.SR.Format(System.SR.MatchArgumentFailure2, method.ToString(), text2);
						throw new InvalidCastException(text2);
					}
					return null;
				}
			}
			return method;
		}
	}

	internal static MethodBase GetCallTarget(Symbols.Method targetProcedure, BindingFlags flags)
	{
		if (targetProcedure.IsMethod)
		{
			return targetProcedure.AsMethod();
		}
		if (targetProcedure.IsProperty)
		{
			return MatchesPropertyRequirements(targetProcedure, flags);
		}
		return null;
	}

	[RequiresUnreferencedCode("Late binding is dynamic and cannot be statically analyzed. The referenced types and members may be trimmed")]
	internal static object[] ConstructCallArguments(Symbols.Method targetProcedure, object[] arguments, BindingFlags lookupFlags)
	{
		ParameterInfo[] parameters = GetCallTarget(targetProcedure, lookupFlags).GetParameters();
		checked
		{
			object[] array = new object[parameters.Length - 1 + 1];
			int num = arguments.Length;
			object argument = null;
			if (Symbols.HasFlag(lookupFlags, BindingFlags.SetProperty))
			{
				object[] array2 = arguments;
				arguments = new object[num - 2 + 1];
				Array.Copy(array2, arguments, arguments.Length);
				argument = array2[num - 1];
			}
			OverloadResolution.MatchArguments(targetProcedure, arguments, array);
			if (Symbols.HasFlag(lookupFlags, BindingFlags.SetProperty))
			{
				ParameterInfo parameterInfo = parameters[parameters.Length - 1];
				array[parameters.Length - 1] = OverloadResolution.PassToParameter(argument, parameterInfo, parameterInfo.ParameterType);
			}
			return array;
		}
	}
}
