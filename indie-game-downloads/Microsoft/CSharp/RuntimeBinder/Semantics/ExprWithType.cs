using System;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal abstract class ExprWithType : Expr
{
	protected ExprWithType(ExpressionKind kind, CType type)
		: base(kind)
	{
		base.Type = type;
	}

	protected static bool TypesAreEqual(Type t1, Type t2)
	{
		if (!(t1 == t2))
		{
			return t1.IsEquivalentTo(t2);
		}
		return true;
	}
}
