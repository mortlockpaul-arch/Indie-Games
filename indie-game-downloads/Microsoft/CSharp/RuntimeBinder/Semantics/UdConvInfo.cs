namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal readonly struct UdConvInfo(MethWithType mwt, bool srcImplicit, bool dstImplicit)
{
	public readonly MethWithType Meth = mwt;

	public readonly bool SrcImplicit = srcImplicit;

	public readonly bool DstImplicit = dstImplicit;
}
