using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprMemberGroup : ExprWithType
{
	public Name Name { get; }

	public TypeArray TypeArgs { get; }

	public SYMKIND SymKind { get; }

	public Expr OptionalObject { get; set; }

	public CMemberLookupResults MemberLookupResults { get; }

	public CType ParentType { get; }

	public bool IsDelegate => (base.Flags & EXPRFLAG.EXF_GOTONOTBLOCKED) != 0;

	public ExprMemberGroup(EXPRFLAG flags, Name name, TypeArray typeArgs, SYMKIND symKind, CType parentType, Expr optionalObject, CMemberLookupResults memberLookupResults)
		: base(ExpressionKind.MemberGroup, MethodGroupType.Instance)
	{
		base.Flags = flags;
		Name = name;
		TypeArgs = typeArgs ?? TypeArray.Empty;
		SymKind = symKind;
		ParentType = parentType;
		OptionalObject = optionalObject;
		MemberLookupResults = memberLookupResults;
	}
}
