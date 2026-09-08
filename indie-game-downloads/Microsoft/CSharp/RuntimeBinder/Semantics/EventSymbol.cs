using System.Reflection;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class EventSymbol : Symbol
{
	public EventInfo AssociatedEventInfo;

	public new bool isStatic;

	public bool isOverride;

	public CType type;

	public MethodSymbol methAdd;

	public MethodSymbol methRemove;
}
