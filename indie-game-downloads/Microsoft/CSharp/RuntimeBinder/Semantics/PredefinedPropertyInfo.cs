using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class PredefinedPropertyInfo
{
	public PREDEFPROP property;

	public PredefinedName name;

	public PREDEFMETH getter;

	public PredefinedPropertyInfo(PREDEFPROP property, PredefinedName name, PREDEFMETH getter)
	{
		this.property = property;
		this.name = name;
		this.getter = getter;
	}
}
