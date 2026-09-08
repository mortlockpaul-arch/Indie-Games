using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Microsoft.CSharp.RuntimeBinder.ComInterop;

namespace Microsoft.CSharp.RuntimeBinder;

internal sealed class DynamicMetaObjectProviderDebugView
{
	[DebuggerDisplay("{value}", Name = "{name, nq}", Type = "{type, nq}")]
	internal sealed class DynamicProperty
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string name;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly object value;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string type;

		public DynamicProperty(string name, object value)
		{
			this.name = name;
			this.value = value;
			type = ((value == null) ? "<null>" : value.GetType().ToString());
		}
	}

	[Serializable]
	internal sealed class DynamicDebugViewEmptyException : Exception
	{
		public string Empty => System.SR.EmptyDynamicView;

		public DynamicDebugViewEmptyException()
		{
		}

		[Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
		private DynamicDebugViewEmptyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private IList<KeyValuePair<string, object>> results;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly object obj;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static readonly ParameterExpression parameter = Expression.Parameter(typeof(object), "debug");

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static readonly Type ComObjectType = Type.GetType("System.__ComObject, System.Private.CoreLib");

	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	internal DynamicProperty[] Items
	{
		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
		get
		{
			if (results == null || results.Count == 0)
			{
				results = QueryDynamicObject(obj);
				if (results == null || results.Count == 0)
				{
					throw new DynamicDebugViewEmptyException();
				}
			}
			DynamicProperty[] array = new DynamicProperty[results.Count];
			for (int i = 0; i < results.Count; i++)
			{
				array[i] = new DynamicProperty(results[i].Key, results[i].Value);
			}
			return array;
		}
	}

	public DynamicMetaObjectProviderDebugView(object arg)
	{
		obj = arg;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static object TryEvalBinaryOperators<T1, T2>(T1 arg1, T2 arg2, CSharpArgumentInfoFlags arg1Flags, CSharpArgumentInfoFlags arg2Flags, ExpressionType opKind, Type accessibilityContext)
	{
		CSharpArgumentInfo cSharpArgumentInfo = CSharpArgumentInfo.Create(arg1Flags, null);
		CSharpArgumentInfo cSharpArgumentInfo2 = CSharpArgumentInfo.Create(arg2Flags, null);
		CallSite<Func<CallSite, T1, T2, object>> callSite = CallSite<Func<CallSite, T1, T2, object>>.Create(new CSharpBinaryOperationBinder(opKind, isChecked: false, CSharpBinaryOperationFlags.None, accessibilityContext, new CSharpArgumentInfo[2] { cSharpArgumentInfo, cSharpArgumentInfo2 }));
		return callSite.Target(callSite, arg1, arg2);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static object TryEvalUnaryOperators<T>(T obj, ExpressionType oper, Type accessibilityContext)
	{
		if (oper == ExpressionType.IsTrue || oper == ExpressionType.IsFalse)
		{
			CallSite<Func<CallSite, T, bool>> callSite = CallSite<Func<CallSite, T, bool>>.Create(new CSharpUnaryOperationBinder(oper, isChecked: false, accessibilityContext, new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			return callSite.Target(callSite, obj);
		}
		CallSite<Func<CallSite, T, object>> callSite2 = CallSite<Func<CallSite, T, object>>.Create(new CSharpUnaryOperationBinder(oper, isChecked: false, accessibilityContext, new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		return callSite2.Target(callSite2, obj);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static K TryEvalCast<T, K>(T obj, Type type, CSharpBinderFlags kind, Type accessibilityContext)
	{
		CallSite<Func<CallSite, T, K>> callSite = CallSite<Func<CallSite, T, K>>.Create(Binder.Convert(kind, type, accessibilityContext));
		return callSite.Target(callSite, obj);
	}

	private static void CreateDelegateSignatureAndArgumentInfos(object[] args, Type[] argTypes, CSharpArgumentInfoFlags[] argFlags, out Type[] delegateSignatureTypes, out CSharpArgumentInfo[] argInfos)
	{
		int num = args.Length;
		delegateSignatureTypes = new Type[num + 2];
		delegateSignatureTypes[0] = typeof(CallSite);
		argInfos = new CSharpArgumentInfo[num];
		for (int i = 0; i < num; i++)
		{
			if (argTypes[i] != null)
			{
				delegateSignatureTypes[i + 1] = argTypes[i];
			}
			else if (args[i] != null)
			{
				delegateSignatureTypes[i + 1] = args[i].GetType();
			}
			else
			{
				delegateSignatureTypes[i + 1] = typeof(object);
			}
			argInfos[i] = CSharpArgumentInfo.Create(argFlags[i], null);
		}
		delegateSignatureTypes[num + 1] = typeof(object);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static object CreateDelegateAndInvoke(Type[] delegateSignatureTypes, CallSiteBinder binder, object[] args)
	{
		CallSite callSite = CallSite.Create(Expression.GetDelegateType(delegateSignatureTypes), binder);
		Delegate obj = (Delegate)callSite.GetType().GetField("Target").GetValue(callSite);
		object[] array = new object[args.Length + 1];
		array[0] = callSite;
		args.CopyTo(array, 1);
		return obj.DynamicInvoke(array);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static object TryEvalMethodVarArgs(object[] methodArgs, Type[] argTypes, CSharpArgumentInfoFlags[] argFlags, string methodName, Type accessibilityContext, Type[] typeArguments)
	{
		CreateDelegateSignatureAndArgumentInfos(methodArgs, argTypes, argFlags, out var delegateSignatureTypes, out var argInfos);
		return CreateDelegateAndInvoke(binder: (!string.IsNullOrEmpty(methodName)) ? ((CallSiteBinder)new CSharpInvokeMemberBinder(CSharpCallFlags.ResultDiscarded, methodName, accessibilityContext, typeArguments, argInfos)) : ((CallSiteBinder)new CSharpInvokeBinder(CSharpCallFlags.ResultDiscarded, accessibilityContext, argInfos)), delegateSignatureTypes: delegateSignatureTypes, args: methodArgs);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static object TryGetMemberValue<T>(T obj, string propName, Type accessibilityContext, bool isResultIndexed)
	{
		CallSite<Func<CallSite, T, object>> callSite = CallSite<Func<CallSite, T, object>>.Create(new CSharpGetMemberBinder(propName, isResultIndexed, accessibilityContext, new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		return callSite.Target(callSite, obj);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static object TryGetMemberValueVarArgs(object[] propArgs, Type[] argTypes, CSharpArgumentInfoFlags[] argFlags, Type accessibilityContext)
	{
		CreateDelegateSignatureAndArgumentInfos(propArgs, argTypes, argFlags, out var delegateSignatureTypes, out var argInfos);
		CallSiteBinder binder = new CSharpGetIndexBinder(accessibilityContext, argInfos);
		return CreateDelegateAndInvoke(delegateSignatureTypes, binder, propArgs);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static object TrySetMemberValue<TObject, TValue>(TObject obj, string propName, TValue value, CSharpArgumentInfoFlags valueFlags, Type accessibilityContext)
	{
		CSharpArgumentInfo cSharpArgumentInfo = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null);
		CSharpArgumentInfo cSharpArgumentInfo2 = CSharpArgumentInfo.Create(valueFlags, null);
		CallSite<Func<CallSite, TObject, TValue, object>> callSite = CallSite<Func<CallSite, TObject, TValue, object>>.Create(new CSharpSetMemberBinder(propName, isCompoundAssignment: false, isChecked: false, accessibilityContext, new CSharpArgumentInfo[2] { cSharpArgumentInfo, cSharpArgumentInfo2 }));
		return callSite.Target(callSite, obj, value);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static object TrySetMemberValueVarArgs(object[] propArgs, Type[] argTypes, CSharpArgumentInfoFlags[] argFlags, Type accessibilityContext)
	{
		CreateDelegateSignatureAndArgumentInfos(propArgs, argTypes, argFlags, out var delegateSignatureTypes, out var argInfos);
		CallSiteBinder binder = new CSharpSetIndexBinder(isCompoundAssignment: false, isChecked: false, accessibilityContext, argInfos);
		return CreateDelegateAndInvoke(delegateSignatureTypes, binder, propArgs);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal static object TryGetMemberValue(object obj, string name, bool ignoreException)
	{
		bool ignoreCase = false;
		object obj2 = null;
		CallSite<Func<CallSite, object, object>> callSite = CallSite<Func<CallSite, object, object>>.Create(new GetMemberValueBinder(name, ignoreCase));
		try
		{
			return callSite.Target(callSite, obj);
		}
		catch (DynamicBindingFailedException)
		{
			if (ignoreException)
			{
				return null;
			}
			throw;
		}
		catch (MissingMemberException)
		{
			if (ignoreException)
			{
				return System.SR.GetValueonWriteOnlyProperty;
			}
			throw;
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static IList<KeyValuePair<string, object>> QueryDynamicObject(object obj)
	{
		if (obj is IDynamicMetaObjectProvider dynamicMetaObjectProvider)
		{
			List<string> list = new List<string>(dynamicMetaObjectProvider.GetMetaObject(parameter).GetDynamicMemberNames());
			list.Sort();
			List<KeyValuePair<string, object>> list2 = new List<KeyValuePair<string, object>>();
			{
				foreach (string item in list)
				{
					object value;
					if ((value = TryGetMemberValue(obj, item, ignoreException: true)) != null)
					{
						list2.Add(new KeyValuePair<string, object>(item, value));
					}
				}
				return list2;
			}
		}
		if (obj != null && ComObjectType.IsAssignableFrom(obj.GetType()))
		{
			IList<string> dynamicDataMemberNames = ComBinder.GetDynamicDataMemberNames(obj);
			return ComBinder.GetDynamicDataMembers(obj, dynamicDataMemberNames.OrderBy((string n) => n));
		}
		return Array.Empty<KeyValuePair<string, object>>();
	}
}
