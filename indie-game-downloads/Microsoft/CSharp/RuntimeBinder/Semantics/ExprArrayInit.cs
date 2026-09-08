namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprArrayInit : ExprWithType
{
	public Expr OptionalArguments { get; set; }

	public Expr OptionalArgumentDimensions { get; set; }

	public int[] DimensionSizes { get; }

	public bool GeneratedForParamArray { get; set; }

	public ExprArrayInit(CType type, Expr arguments, Expr argumentDimensions, int[] dimensionSizes)
		: base(ExpressionKind.ArrayInit, type)
	{
		OptionalArguments = arguments;
		OptionalArgumentDimensions = argumentDimensions;
		DimensionSizes = dimensionSizes;
	}
}
