using System.Diagnostics.CodeAnalysis;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprCast : ExprWithType
{
	public Expr Argument { get; set; }

	public bool IsBoxingCast => (base.Flags & (EXPRFLAG.EXF_CTOR | EXPRFLAG.EXF_UNREALIZEDGOTO)) != 0;

	public override object Object
	{
		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		get
		{
			Expr argument;
			for (argument = Argument; argument is ExprCast exprCast; argument = exprCast.Argument)
			{
			}
			return argument.Object;
		}
	}

	public ExprCast(EXPRFLAG flags, CType type, Expr argument)
		: base(ExpressionKind.Cast, type)
	{
		Argument = argument;
		base.Flags = flags;
	}
}
