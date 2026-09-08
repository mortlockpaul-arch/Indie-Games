namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal abstract class ParentSymbol : Symbol
{
	public Symbol firstChild;

	private Symbol _lastChild;

	public void AddToChildList(Symbol sym)
	{
		if (_lastChild == null)
		{
			firstChild = (_lastChild = sym);
		}
		else
		{
			_lastChild.nextChild = sym;
			_lastChild = sym;
			sym.nextChild = null;
		}
		sym.parent = this;
	}
}
