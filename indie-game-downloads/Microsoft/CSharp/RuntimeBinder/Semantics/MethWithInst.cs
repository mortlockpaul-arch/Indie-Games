namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class MethWithInst : MethPropWithInst
{
	public MethWithInst(MethodSymbol meth, AggregateType ats)
		: this(meth, ats, null)
	{
	}

	public MethWithInst(MethodSymbol meth, AggregateType ats, TypeArray typeArgs)
	{
		Set(meth, ats, typeArgs);
	}

	public MethWithInst(MethPropWithInst mpwi)
	{
		Set(mpwi.Sym as MethodSymbol, mpwi.Ats, mpwi.TypeArgs);
	}
}
