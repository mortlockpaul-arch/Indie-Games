using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class ArrayType : CType
{
	public int Rank { get; }

	public bool IsSZArray { get; }

	public CType ElementType { get; }

	public CType BaseElementType
	{
		get
		{
			CType elementType;
			for (elementType = ElementType; elementType is ArrayType arrayType; elementType = arrayType.ElementType)
			{
			}
			return elementType;
		}
	}

	public override bool IsReferenceType => true;

	public override Type AssociatedSystemType
	{
		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		get
		{
			Type associatedSystemType = ElementType.AssociatedSystemType;
			if (!IsSZArray)
			{
				return associatedSystemType.MakeArrayType(Rank);
			}
			return associatedSystemType.MakeArrayType();
		}
	}

	public override CType BaseOrParameterOrElementType => ElementType;

	public override FUNDTYPE FundamentalType => FUNDTYPE.FT_REF;

	public override ConstValKind ConstValKind => ConstValKind.IntPtr;

	public ArrayType(CType elementType, int rank, bool isSZArray)
		: base(TypeKind.TK_ArrayType)
	{
		Rank = rank;
		IsSZArray = isSZArray;
		ElementType = elementType;
	}

	public override bool IsUnsafe()
	{
		return BaseElementType is PointerType;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	public override AggregateType GetAts()
	{
		return SymbolLoader.GetPredefindType(PredefinedType.PT_ARRAY);
	}
}
