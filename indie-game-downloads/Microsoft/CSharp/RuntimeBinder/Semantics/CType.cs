using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal abstract class CType
{
	[ExcludeFromCodeCoverage(Justification = "Should only be called through override")]
	public virtual Type AssociatedSystemType
	{
		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		get
		{
			throw Error.InternalCompilerError();
		}
	}

	public TypeKind TypeKind { get; }

	public virtual CType BaseOrParameterOrElementType => null;

	public virtual FUNDTYPE FundamentalType => FUNDTYPE.FT_NONE;

	public virtual ConstValKind ConstValKind => ConstValKind.Int;

	public virtual bool IsDelegateType => false;

	public virtual bool IsSimpleType => false;

	public virtual bool IsSimpleOrEnum => false;

	public virtual bool IsSimpleOrEnumOrString => false;

	public virtual bool IsNumericType => false;

	public virtual bool IsStructType => false;

	public virtual bool IsEnumType => false;

	public virtual bool IsInterfaceType => false;

	public virtual bool IsClassType => false;

	[ExcludeFromCodeCoverage(Justification = "Should only be called through override")]
	public virtual AggregateType UnderlyingEnumType
	{
		get
		{
			throw Error.InternalCompilerError();
		}
	}

	public virtual bool IsPredefined => false;

	[ExcludeFromCodeCoverage(Justification = "Should only be called through override")]
	public virtual PredefinedType PredefinedType
	{
		get
		{
			throw Error.InternalCompilerError();
		}
	}

	public virtual bool IsStaticClass => false;

	public virtual bool IsValueType => false;

	public virtual bool IsNonNullableValueType => false;

	public virtual bool IsReferenceType => false;

	private protected CType(TypeKind kind)
	{
		TypeKind = kind;
	}

	public CType GetNakedType(bool fStripNub)
	{
		CType cType = this;
		while (true)
		{
			TypeKind typeKind = cType.TypeKind;
			if ((uint)(typeKind - 5) > 2u && (typeKind != TypeKind.TK_NullableType || !fStripNub))
			{
				break;
			}
			cType = cType.BaseOrParameterOrElementType;
		}
		return cType;
	}

	public virtual CType StripNubs()
	{
		return this;
	}

	public virtual CType StripNubs(out bool wasNullable)
	{
		wasNullable = false;
		return this;
	}

	public virtual bool IsUnsafe()
	{
		return false;
	}

	public virtual bool IsPredefType(PredefinedType pt)
	{
		return false;
	}

	[ExcludeFromCodeCoverage(Justification = "Should only be called through override")]
	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	public virtual AggregateType GetAts()
	{
		return null;
	}
}
