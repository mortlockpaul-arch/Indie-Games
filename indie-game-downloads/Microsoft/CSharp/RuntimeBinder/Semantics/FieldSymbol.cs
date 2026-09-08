using System.Reflection;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class FieldSymbol : VariableSymbol
{
	public new bool isStatic;

	public bool isReadOnly;

	public bool isEvent;

	public FieldInfo AssociatedFieldInfo;

	public void SetType(CType pType)
	{
		type = pType;
	}

	public new CType GetType()
	{
		return type;
	}
}
