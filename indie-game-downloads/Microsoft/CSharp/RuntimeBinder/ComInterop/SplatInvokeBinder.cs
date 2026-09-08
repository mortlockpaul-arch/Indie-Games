using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
internal sealed class SplatInvokeBinder : CallSiteBinder
{
	private static readonly SplatInvokeBinder s_instance = new SplatInvokeBinder();

	internal static SplatInvokeBinder Instance => s_instance;

	private SplatInvokeBinder()
	{
	}

	public override Expression Bind(object[] args, ReadOnlyCollection<ParameterExpression> parameters, LabelTarget returnLabel)
	{
		int num = ((object[])args[1]).Length;
		ParameterExpression array = parameters[1];
		ReadOnlyCollectionBuilder<Expression> readOnlyCollectionBuilder = new ReadOnlyCollectionBuilder<Expression>(num + 1);
		Type[] array2 = new Type[num + 3];
		readOnlyCollectionBuilder.Add(parameters[0]);
		array2[0] = typeof(CallSite);
		array2[1] = typeof(object);
		for (int i = 0; i < num; i++)
		{
			readOnlyCollectionBuilder.Add(Expression.ArrayAccess(array, Expression.Constant(i)));
			array2[i + 2] = typeof(object).MakeByRefType();
		}
		array2[^1] = typeof(object);
		return Expression.IfThen(Expression.Equal(Expression.ArrayLength(array), Expression.Constant(num)), Expression.Return(returnLabel, Expression.MakeDynamic(Expression.GetDelegateType(array2), new ComInvokeAction(new CallInfo(num)), readOnlyCollectionBuilder)));
	}
}
