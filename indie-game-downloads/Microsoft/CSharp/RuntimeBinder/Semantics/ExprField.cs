namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprField : ExprWithType
{
	public Expr OptionalObject { get; set; }

	public FieldWithType FieldWithType { get; }

	public ExprField(CType type, Expr optionalObject, FieldWithType field)
		: base(ExpressionKind.Field, type)
	{
		base.Flags = ((!field.Field().isReadOnly) ? EXPRFLAG.EXF_LVALUE : ((EXPRFLAG)0));
		OptionalObject = optionalObject;
		FieldWithType = field;
	}
}
