namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprFieldInfo : ExprWithType
{
	public FieldSymbol Field { get; }

	public AggregateType FieldType { get; }

	public ExprFieldInfo(FieldSymbol field, AggregateType fieldType, CType type)
		: base(ExpressionKind.FieldInfo, type)
	{
		Field = field;
		FieldType = fieldType;
	}
}
