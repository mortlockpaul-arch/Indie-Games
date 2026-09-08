namespace System.Runtime.InteropServices;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class TypeMapAssociationAttribute<TTypeMapGroup> : Attribute
{
	public TypeMapAssociationAttribute(Type source, Type proxy)
	{
	}
}
