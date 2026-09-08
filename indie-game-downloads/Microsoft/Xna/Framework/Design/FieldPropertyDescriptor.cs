using System;
using System.Reflection;

namespace Microsoft.Xna.Framework.Design;

internal sealed class FieldPropertyDescriptor : MemberPropertyDescriptor
{
	private readonly FieldInfo field;

	public override Type PropertyType => field.FieldType;

	public FieldPropertyDescriptor(FieldInfo field)
		: base(field)
	{
		this.field = field;
	}

	public override object GetValue(object component)
	{
		return field.GetValue(component);
	}

	public override void SetValue(object component, object value)
	{
		field.SetValue(component, value);
	}
}
