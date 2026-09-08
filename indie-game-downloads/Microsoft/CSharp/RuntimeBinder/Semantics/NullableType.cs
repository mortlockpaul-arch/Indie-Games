using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class NullableType : CType
{
	private AggregateType _ats;

	public CType UnderlyingType { get; }

	public override bool IsValueType => true;

	public override bool IsStructType => true;

	public override Type AssociatedSystemType
	{
		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		get
		{
			return typeof(Nullable<>).MakeGenericType(UnderlyingType.AssociatedSystemType);
		}
	}

	public override CType BaseOrParameterOrElementType => UnderlyingType;

	public override FUNDTYPE FundamentalType => FUNDTYPE.FT_STRUCT;

	[ExcludeFromCodeCoverage(Justification = "Should be unreachable. Overload exists just to catch it being hit during debug.")]
	public override ConstValKind ConstValKind => ConstValKind.Decimal;

	public NullableType(CType underlyingType)
		: base(TypeKind.TK_NullableType)
	{
		UnderlyingType = underlyingType;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	public override AggregateType GetAts()
	{
		return _ats ?? (_ats = TypeManager.GetAggregate(TypeManager.GetNullable(), TypeArray.Allocate(UnderlyingType)));
	}

	public override CType StripNubs()
	{
		return UnderlyingType;
	}

	public override CType StripNubs(out bool wasNullable)
	{
		wasNullable = true;
		return UnderlyingType;
	}
}
