using System.Diagnostics.CodeAnalysis;

namespace System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class MetadataTypeAttribute : Attribute
{
	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)]
	private readonly Type _metadataClassType;

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)]
	public Type MetadataClassType
	{
		get
		{
			if (_metadataClassType == null)
			{
				throw new InvalidOperationException(System.SR.MetadataTypeAttribute_TypeCannotBeNull);
			}
			return _metadataClassType;
		}
	}

	public MetadataTypeAttribute([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type metadataClassType)
	{
		_metadataClassType = metadataClassType;
	}
}
