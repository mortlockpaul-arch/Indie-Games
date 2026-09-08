using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class TypeParameterType : CType
{
	public TypeParameterSymbol Symbol { get; }

	public ParentSymbol OwningSymbol => Symbol.parent;

	public Name Name => Symbol.name;

	public bool Covariant => Symbol.Covariant;

	public bool Invariant => Symbol.Invariant;

	public bool Contravariant => Symbol.Contravariant;

	public override bool IsValueType => Symbol.IsValueType();

	public override bool IsReferenceType => Symbol.IsReferenceType();

	public override bool IsNonNullableValueType => Symbol.IsNonNullableValueType();

	public bool HasNewConstraint => Symbol.HasNewConstraint();

	public bool HasRefConstraint => Symbol.HasRefConstraint();

	public bool HasValConstraint => Symbol.HasValConstraint();

	public bool IsMethodTypeParameter => Symbol.IsMethodTypeParameter();

	public int IndexInOwnParameters => Symbol.GetIndexInOwnParameters();

	public int IndexInTotalParameters => Symbol.GetIndexInTotalParameters();

	public TypeArray Bounds => Symbol.GetBounds();

	public override Type AssociatedSystemType
	{
		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		get
		{
			return (IsMethodTypeParameter ? ((MethodInfo)((MethodSymbol)OwningSymbol).AssociatedMemberInfo).GetGenericArguments() : ((AggregateSymbol)OwningSymbol).AssociatedSystemType.GetGenericArguments())[IndexInOwnParameters];
		}
	}

	public override FUNDTYPE FundamentalType => FUNDTYPE.FT_VAR;

	public TypeParameterType(TypeParameterSymbol symbol)
		: base(TypeKind.TK_TypeParameterType)
	{
		Symbol = symbol;
		symbol.SetTypeParameterType(this);
	}
}
