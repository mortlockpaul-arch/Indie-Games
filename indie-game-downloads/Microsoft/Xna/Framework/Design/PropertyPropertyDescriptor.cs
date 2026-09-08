using System;
using System.Reflection;

namespace Microsoft.Xna.Framework.Design;

internal sealed class PropertyPropertyDescriptor : MemberPropertyDescriptor
{
	private readonly PropertyInfo property;

	public override Type PropertyType => property.PropertyType;

	public PropertyPropertyDescriptor(PropertyInfo property)
		: base(property)
	{
		this.property = property;
	}

	public override object GetValue(object component)
	{
		return property.GetValue(component, null);
	}

	public override void SetValue(object component, object value)
	{
		property.SetValue(component, value, null);
	}
}
