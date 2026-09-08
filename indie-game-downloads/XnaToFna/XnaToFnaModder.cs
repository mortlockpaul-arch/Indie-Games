using Mono.Cecil;
using MonoMod;

namespace XnaToFna;

public class XnaToFnaModder : MonoModder
{
	public XnaToFnaUtil XTF;

	public XnaToFnaModder(XnaToFnaUtil xtf)
	{
		XTF = xtf;
	}

	public override void Log(string text)
	{
		if (!text.StartsWith("[MapDependency]"))
		{
			XTF.Log("[MonoMod] " + text);
		}
	}

	public override IMetadataTokenProvider Relinker(IMetadataTokenProvider mtp, IGenericParameterProvider context)
	{
		return ((MonoModder)this).PostRelinker(((MonoModder)this).MainRelinker(mtp, context), context);
	}
}
