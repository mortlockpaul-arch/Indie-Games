using System.Diagnostics.CodeAnalysis;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprTypeOf : ExprWithType
{
	public CType SourceType { get; }

	public override object Object
	{
		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		get
		{
			return SourceType.AssociatedSystemType;
		}
	}

	public ExprTypeOf(CType type, CType sourceType)
		: base(ExpressionKind.TypeOf, type)
	{
		base.Flags = EXPRFLAG.EXF_CANTBENULL;
		SourceType = sourceType;
	}
}
