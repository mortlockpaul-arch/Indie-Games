using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;

namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.All)]
public sealed class AmbientValueAttribute : Attribute
{
	private object _value;

	private static readonly object s_throwSentinel = (IDesignerHost.IsSupported ? null : new object());

	public object? Value
	{
		get
		{
			if (!IDesignerHost.IsSupported && _value == s_throwSentinel)
			{
				throw new ArgumentException(System.SR.RuntimeInstanceNotAllowed);
			}
			return _value;
		}
	}

	public AmbientValueAttribute([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type type, string value)
	{
		if (!IDesignerHost.IsSupported)
		{
			_value = s_throwSentinel;
			return;
		}
		try
		{
			_value = TypeDescriptorGetConverter(type).ConvertFromInvariantString(value);
			[RequiresUnreferencedCode("AmbientValueAttribute usage of TypeConverter is not compatible with trimming.")]
			static TypeConverter TypeDescriptorGetConverter([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type type2)
			{
				return TypeDescriptor.GetConverter(type2);
			}
		}
		catch
		{
		}
	}

	public AmbientValueAttribute(char value)
	{
		_value = value;
	}

	public AmbientValueAttribute(byte value)
	{
		_value = value;
	}

	public AmbientValueAttribute(short value)
	{
		_value = value;
	}

	public AmbientValueAttribute(int value)
	{
		_value = value;
	}

	public AmbientValueAttribute(long value)
	{
		_value = value;
	}

	public AmbientValueAttribute(float value)
	{
		_value = value;
	}

	public AmbientValueAttribute(double value)
	{
		_value = value;
	}

	public AmbientValueAttribute(bool value)
	{
		_value = value;
	}

	public AmbientValueAttribute(string? value)
	{
		_value = value;
	}

	public AmbientValueAttribute(object? value)
	{
		_value = value;
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj == this)
		{
			return true;
		}
		if (obj is AmbientValueAttribute ambientValueAttribute)
		{
			if (Value == null)
			{
				return ambientValueAttribute.Value == null;
			}
			return Value.Equals(ambientValueAttribute.Value);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
