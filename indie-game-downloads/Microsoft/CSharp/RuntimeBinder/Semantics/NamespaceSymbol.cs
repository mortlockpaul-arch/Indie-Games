using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class NamespaceSymbol : NamespaceOrAggregateSymbol
{
	public static readonly NamespaceSymbol Root = GetRootNamespaceSymbol();

	private static NamespaceSymbol GetRootNamespaceSymbol()
	{
		NamespaceSymbol namespaceSymbol = new NamespaceSymbol();
		namespaceSymbol.name = NameManager.GetPredefinedName(PredefinedName.PN_VOID);
		namespaceSymbol.setKind(SYMKIND.SK_NamespaceSymbol);
		return namespaceSymbol;
	}
}
