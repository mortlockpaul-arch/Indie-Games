using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprNamedArgumentSpecification : Expr
{
	private Expr _value;

	public Name Name { get; }

	public Expr Value
	{
		get
		{
			return _value;
		}
		set
		{
			base.Type = (_value = value).Type;
		}
	}

	public ExprNamedArgumentSpecification(Name name, Expr value)
		: base(ExpressionKind.NamedArgumentSpecification)
	{
		Name = name;
		Value = value;
	}
}
