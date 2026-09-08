using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace MS.Internal.Xml.Linq.ComponentModel;

internal sealed class XTypeDescriptionProvider<T> : TypeDescriptionProvider
{
	public XTypeDescriptionProvider()
		: base(TypeDescriptor.GetProvider(typeof(T)))
	{
	}

	public override ICustomTypeDescriptor GetTypeDescriptor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type type, object instance)
	{
		return new XTypeDescriptor<T>(base.GetTypeDescriptor(type, instance));
	}
}
