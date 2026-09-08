using System.Reflection;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal class PropertySymbol : MethodOrPropertySymbol
{
	public MethodSymbol GetterMethod { get; set; }

	public MethodSymbol SetterMethod { get; set; }

	public PropertyInfo AssociatedPropertyInfo { get; set; }

	public bool Bogus { get; set; }
}
