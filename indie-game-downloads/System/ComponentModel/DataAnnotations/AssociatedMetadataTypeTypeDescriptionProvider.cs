using System.Diagnostics.CodeAnalysis;

namespace System.ComponentModel.DataAnnotations;

public class AssociatedMetadataTypeTypeDescriptionProvider : TypeDescriptionProvider
{
	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)]
	private readonly Type _associatedMetadataType;

	public AssociatedMetadataTypeTypeDescriptionProvider(Type type)
		: base(TypeDescriptor.GetProvider(type))
	{
	}

	public AssociatedMetadataTypeTypeDescriptionProvider(Type type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type associatedMetadataType)
		: this(type)
	{
		ArgumentNullException.ThrowIfNull(associatedMetadataType, "associatedMetadataType");
		_associatedMetadataType = associatedMetadataType;
	}

	public override ICustomTypeDescriptor GetTypeDescriptor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type objectType, object? instance)
	{
		return new AssociatedMetadataTypeTypeDescriptor(base.GetTypeDescriptor(objectType, instance), objectType, _associatedMetadataType);
	}
}
