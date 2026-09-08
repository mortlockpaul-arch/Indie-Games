using System.Diagnostics.CodeAnalysis;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal abstract class Expr
{
	private CType _type;

	internal object RuntimeObject { get; set; }

	internal CType RuntimeObjectActualType { get; set; }

	public ExpressionKind Kind { get; }

	public EXPRFLAG Flags { get; set; }

	public bool IsOptionalArgument { get; set; }

	public string ErrorString { get; set; }

	public CType Type
	{
		get
		{
			return _type;
		}
		protected set
		{
			_type = value;
		}
	}

	[ExcludeFromCodeCoverage(Justification = "Should only be called through override")]
	public virtual object Object
	{
		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		get
		{
			throw Error.InternalCompilerError();
		}
	}

	protected Expr(ExpressionKind kind)
	{
		Kind = kind;
	}
}
