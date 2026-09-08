using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Microsoft.CSharp.RuntimeBinder;

/// <summary>Contains factory methods to create dynamic call site binders for CSharp.</summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class Binder
{
	/// <summary>Initializes a new CSharp binary operation binder.</summary>
	/// <param name="flags">The flags with which to initialize the binder.</param>
	/// <param name="operation">The binary operation kind.</param>
	/// <param name="context">The <see cref="T:System.Type" /> that indicates where this operation is used.</param>
	/// <param name="argumentInfo">The sequence of <see cref="T:Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo" /> instances for the arguments to this operation.</param>
	/// <returns>A new CSharp binary operation binder.</returns>
	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static CallSiteBinder BinaryOperation(CSharpBinderFlags flags, ExpressionType operation, Type? context, IEnumerable<CSharpArgumentInfo>? argumentInfo)
	{
		bool isChecked = (flags & CSharpBinderFlags.CheckedContext) != 0;
		bool num = (flags & CSharpBinderFlags.BinaryOperationLogical) != 0;
		CSharpBinaryOperationFlags cSharpBinaryOperationFlags = CSharpBinaryOperationFlags.None;
		if (num)
		{
			cSharpBinaryOperationFlags |= CSharpBinaryOperationFlags.LogicalOperation;
		}
		return new CSharpBinaryOperationBinder(operation, isChecked, cSharpBinaryOperationFlags, context, argumentInfo).TryGetExisting();
	}

	/// <summary>Initializes a new CSharp convert binder.</summary>
	/// <param name="flags">The flags with which to initialize the binder.</param>
	/// <param name="type">The type to convert to.</param>
	/// <param name="context">The <see cref="T:System.Type" /> that indicates where this operation is used.</param>
	/// <returns>A new CSharp convert binder.</returns>
	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static CallSiteBinder Convert(CSharpBinderFlags flags, Type type, Type? context)
	{
		CSharpConversionKind conversionKind = (((flags & CSharpBinderFlags.ConvertExplicit) != CSharpBinderFlags.None) ? CSharpConversionKind.ExplicitConversion : (((flags & CSharpBinderFlags.ConvertArrayIndex) != CSharpBinderFlags.None) ? CSharpConversionKind.ArrayCreationConversion : CSharpConversionKind.ImplicitConversion));
		bool isChecked = (flags & CSharpBinderFlags.CheckedContext) != 0;
		return new CSharpConvertBinder(type, conversionKind, isChecked, context).TryGetExisting();
	}

	/// <summary>Initializes a new CSharp get index binder.</summary>
	/// <param name="flags">The flags with which to initialize the binder.</param>
	/// <param name="context">The <see cref="T:System.Type" /> that indicates where this operation is used.</param>
	/// <param name="argumentInfo">The sequence of <see cref="T:Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo" /> instances for the arguments to this operation.</param>
	/// <returns>A new CSharp get index binder.</returns>
	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static CallSiteBinder GetIndex(CSharpBinderFlags flags, Type? context, IEnumerable<CSharpArgumentInfo>? argumentInfo)
	{
		return new CSharpGetIndexBinder(context, argumentInfo).TryGetExisting();
	}

	/// <summary>Initializes a new CSharp get member binder.</summary>
	/// <param name="flags">The flags with which to initialize the binder.</param>
	/// <param name="name">The name of the member to get.</param>
	/// <param name="context">The <see cref="T:System.Type" /> that indicates where this operation is used.</param>
	/// <param name="argumentInfo">The sequence of <see cref="T:Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo" /> instances for the arguments to this operation.</param>
	/// <returns>A new CSharp get member binder.</returns>
	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static CallSiteBinder GetMember(CSharpBinderFlags flags, string name, Type? context, IEnumerable<CSharpArgumentInfo>? argumentInfo)
	{
		bool resultIndexed = (flags & CSharpBinderFlags.ResultIndexed) != 0;
		return new CSharpGetMemberBinder(name, resultIndexed, context, argumentInfo).TryGetExisting();
	}

	/// <summary>Initializes a new CSharp invoke binder.</summary>
	/// <param name="flags">The flags with which to initialize the binder.</param>
	/// <param name="context">The <see cref="T:System.Type" /> that indicates where this operation is used.</param>
	/// <param name="argumentInfo">The sequence of <see cref="T:Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo" /> instances for the arguments to this operation.</param>
	/// <returns>A new CSharp invoke binder.</returns>
	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static CallSiteBinder Invoke(CSharpBinderFlags flags, Type? context, IEnumerable<CSharpArgumentInfo>? argumentInfo)
	{
		bool num = (flags & CSharpBinderFlags.ResultDiscarded) != 0;
		CSharpCallFlags cSharpCallFlags = CSharpCallFlags.None;
		if (num)
		{
			cSharpCallFlags |= CSharpCallFlags.ResultDiscarded;
		}
		return new CSharpInvokeBinder(cSharpCallFlags, context, argumentInfo).TryGetExisting();
	}

	/// <summary>Initializes a new CSharp invoke member binder.</summary>
	/// <param name="flags">The flags with which to initialize the binder.</param>
	/// <param name="name">The name of the member to invoke.</param>
	/// <param name="typeArguments">The list of type arguments specified for this invoke.</param>
	/// <param name="context">The <see cref="T:System.Type" /> that indicates where this operation is used.</param>
	/// <param name="argumentInfo">The sequence of <see cref="T:Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo" /> instances for the arguments to this operation.</param>
	/// <returns>A new CSharp invoke member binder.</returns>
	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static CallSiteBinder InvokeMember(CSharpBinderFlags flags, string name, IEnumerable<Type>? typeArguments, Type? context, IEnumerable<CSharpArgumentInfo>? argumentInfo)
	{
		bool num = (flags & CSharpBinderFlags.InvokeSimpleName) != 0;
		bool flag = (flags & CSharpBinderFlags.InvokeSpecialName) != 0;
		bool flag2 = (flags & CSharpBinderFlags.ResultDiscarded) != 0;
		CSharpCallFlags cSharpCallFlags = CSharpCallFlags.None;
		if (num)
		{
			cSharpCallFlags |= CSharpCallFlags.SimpleNameCall;
		}
		if (flag)
		{
			cSharpCallFlags |= CSharpCallFlags.EventHookup;
		}
		if (flag2)
		{
			cSharpCallFlags |= CSharpCallFlags.ResultDiscarded;
		}
		return new CSharpInvokeMemberBinder(cSharpCallFlags, name, context, typeArguments, argumentInfo).TryGetExisting();
	}

	/// <summary>Initializes a new CSharp invoke constructor binder.</summary>
	/// <param name="flags">The flags with which to initialize the binder.</param>
	/// <param name="context">The <see cref="T:System.Type" /> that indicates where this operation is used.</param>
	/// <param name="argumentInfo">The sequence of <see cref="T:Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo" /> instances for the arguments to this operation.</param>
	/// <returns>A new CSharp invoke constructor binder.</returns>
	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static CallSiteBinder InvokeConstructor(CSharpBinderFlags flags, Type? context, IEnumerable<CSharpArgumentInfo>? argumentInfo)
	{
		return new CSharpInvokeConstructorBinder(CSharpCallFlags.None, context, argumentInfo).TryGetExisting();
	}

	/// <summary>Initializes a new CSharp is event binder.</summary>
	/// <param name="flags">The flags with which to initialize the binder.</param>
	/// <param name="name">The name of the event to look for.</param>
	/// <param name="context">The <see cref="T:System.Type" /> that indicates where this operation is used.</param>
	/// <returns>A new CSharp is event binder.</returns>
	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static CallSiteBinder IsEvent(CSharpBinderFlags flags, string name, Type? context)
	{
		return new CSharpIsEventBinder(name, context).TryGetExisting();
	}

	/// <summary>Initializes a new CSharp set index binder.</summary>
	/// <param name="flags">The flags with which to initialize the binder.</param>
	/// <param name="context">The <see cref="T:System.Type" /> that indicates where this operation is used.</param>
	/// <param name="argumentInfo">The sequence of <see cref="T:Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo" /> instances for the arguments to this operation.</param>
	/// <returns>A new CSharp set index binder.</returns>
	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static CallSiteBinder SetIndex(CSharpBinderFlags flags, Type? context, IEnumerable<CSharpArgumentInfo>? argumentInfo)
	{
		bool isCompoundAssignment = (flags & CSharpBinderFlags.ValueFromCompoundAssignment) != 0;
		bool isChecked = (flags & CSharpBinderFlags.CheckedContext) != 0;
		return new CSharpSetIndexBinder(isCompoundAssignment, isChecked, context, argumentInfo).TryGetExisting();
	}

	/// <summary>Initializes a new CSharp set member binder.</summary>
	/// <param name="flags">The flags with which to initialize the binder.</param>
	/// <param name="name">The name of the member to set.</param>
	/// <param name="context">The <see cref="T:System.Type" /> that indicates where this operation is used.</param>
	/// <param name="argumentInfo">The sequence of <see cref="T:Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo" /> instances for the arguments to this operation.</param>
	/// <returns>A new CSharp set member binder.</returns>
	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static CallSiteBinder SetMember(CSharpBinderFlags flags, string name, Type? context, IEnumerable<CSharpArgumentInfo>? argumentInfo)
	{
		bool isCompoundAssignment = (flags & CSharpBinderFlags.ValueFromCompoundAssignment) != 0;
		bool isChecked = (flags & CSharpBinderFlags.CheckedContext) != 0;
		return new CSharpSetMemberBinder(name, isCompoundAssignment, isChecked, context, argumentInfo).TryGetExisting();
	}

	/// <summary>Initializes a new CSharp unary operation binder.</summary>
	/// <param name="flags">The flags with which to initialize the binder.</param>
	/// <param name="operation">The unary operation kind.</param>
	/// <param name="context">The <see cref="T:System.Type" /> that indicates where this operation is used.</param>
	/// <param name="argumentInfo">The sequence of <see cref="T:Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo" /> instances for the arguments to this operation.</param>
	/// <returns>A new CSharp unary operation binder.</returns>
	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static CallSiteBinder UnaryOperation(CSharpBinderFlags flags, ExpressionType operation, Type? context, IEnumerable<CSharpArgumentInfo>? argumentInfo)
	{
		bool isChecked = (flags & CSharpBinderFlags.CheckedContext) != 0;
		return new CSharpUnaryOperationBinder(operation, isChecked, context, argumentInfo).TryGetExisting();
	}
}
