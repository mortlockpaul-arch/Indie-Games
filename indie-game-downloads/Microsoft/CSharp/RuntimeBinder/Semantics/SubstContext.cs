using System;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class SubstContext
{
	public readonly CType[] ClassTypes;

	public readonly CType[] MethodTypes;

	public readonly bool DenormMeth;

	public bool IsNop => (ClassTypes.Length == 0) & (MethodTypes.Length == 0);

	public SubstContext(TypeArray typeArgsCls, TypeArray typeArgsMeth, bool denormMeth)
	{
		ClassTypes = typeArgsCls?.Items ?? Array.Empty<CType>();
		MethodTypes = typeArgsMeth?.Items ?? Array.Empty<CType>();
		DenormMeth = denormMeth;
	}

	public SubstContext(AggregateType type)
		: this(type, null, denormMeth: false)
	{
	}

	public SubstContext(AggregateType type, TypeArray typeArgsMeth)
		: this(type, typeArgsMeth, denormMeth: false)
	{
	}

	private SubstContext(AggregateType type, TypeArray typeArgsMeth, bool denormMeth)
		: this(type?.TypeArgsAll, typeArgsMeth, denormMeth)
	{
	}
}
