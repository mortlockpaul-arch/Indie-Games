using System;
using System.ComponentModel;
using System.Reflection;

namespace Microsoft.Xna.Framework.Design;

internal abstract class MemberPropertyDescriptor : PropertyDescriptor
{
	private readonly MemberInfo member;

	public override Type ComponentType => member.DeclaringType;

	public override bool IsReadOnly => false;

	public MemberPropertyDescriptor(MemberInfo member)
		: base(member.Name, (Attribute[])member.GetCustomAttributes(typeof(Attribute), inherit: true))
	{
		this.member = member;
	}

	public override bool CanResetValue(object component)
	{
		return false;
	}

	public override void ResetValue(object component)
	{
	}

	public override bool ShouldSerializeValue(object component)
	{
		return true;
	}

	public override int GetHashCode()
	{
		return member.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		return obj is MemberPropertyDescriptor memberPropertyDescriptor && (object)member == memberPropertyDescriptor.member;
	}
}
