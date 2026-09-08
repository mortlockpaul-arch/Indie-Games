using System.Runtime.CompilerServices;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprBinOp : ExprOperator
{
	[CompilerGenerated]
	private bool _003CIsLifted_003Ek__BackingField;

	public Expr OptionalLeftChild { get; set; }

	public Expr OptionalRightChild { get; set; }

	public bool IsLifted
	{
		[CompilerGenerated]
		set
		{
			_003CIsLifted_003Ek__BackingField = value;
		}
	}

	public ExprBinOp(ExpressionKind kind, CType type, Expr left, Expr right)
		: base(kind, type)
	{
		base.Flags = EXPRFLAG.EXF_BINOP;
		OptionalLeftChild = left;
		OptionalRightChild = right;
	}

	public ExprBinOp(ExpressionKind kind, CType type, Expr left, Expr right, Expr call, MethPropWithInst userMethod)
		: base(kind, type, call, userMethod)
	{
		base.Flags = EXPRFLAG.EXF_BINOP;
		OptionalLeftChild = left;
		OptionalRightChild = right;
	}

	public void SetAssignment()
	{
		base.Flags |= EXPRFLAG.EXF_ASSGOP;
	}
}
