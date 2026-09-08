using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.All)]
public class DefaultValueAttribute : Attribute
{
	private object _value;

	private static readonly object s_throwSentinel = (IsSupported ? null : new object());

	[FeatureSwitchDefinition("System.ComponentModel.DefaultValueAttribute.IsSupported")]
	[FeatureGuard(typeof(RequiresUnreferencedCodeAttribute))]
	internal static bool IsSupported
	{
		get
		{
			if (!AppContext.TryGetSwitch("System.ComponentModel.DefaultValueAttribute.IsSupported", out var isEnabled))
			{
				return true;
			}
			return isEnabled;
		}
	}

	public virtual object? Value
	{
		get
		{
			if (!IsSupported && _value == s_throwSentinel)
			{
				throw new ArgumentException(SR.RuntimeInstanceNotAllowed);
			}
			return _value;
		}
	}

	public DefaultValueAttribute(Type type, string? value)
	{
		if (!IsSupported)
		{
			_value = s_throwSentinel;
		}
		else
		{
			if (type == null)
			{
				return;
			}
			try
			{
				if (TryConvertFromInvariantString(type, value, out var conversionResult))
				{
					_value = conversionResult;
				}
				else if (type.IsSubclassOf(typeof(Enum)) && value != null)
				{
					_value = Enum.Parse(type, value, ignoreCase: true);
				}
				else if (type == typeof(TimeSpan) && value != null)
				{
					_value = TimeSpan.Parse(value);
				}
				else
				{
					_value = Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
				}
				[RequiresUnreferencedCode("DefaultValueAttribute usage of TypeConverter is not compatible with trimming.")]
				static bool TryConvertFromInvariantString([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type typeToConvert, string stringValue, out object reference)
				{
					reference = null;
					try
					{
						reference = ConvertFromInvariantString(null, typeToConvert, stringValue);
						[RequiresUnreferencedCode("DefaultValueAttribute usage of TypeConverter is not compatible with trimming.")]
						[UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "ConvertFromInvariantString")]
						static extern object ConvertFromInvariantString([UnsafeAccessorType("System.ComponentModel.TypeDescriptor, System.ComponentModel.TypeConverter")] object _, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, string stringValue);
					}
					catch
					{
						return false;
					}
					return true;
				}
			}
			catch
			{
			}
		}
	}

	public DefaultValueAttribute(char value)
	{
		_value = value;
	}

	public DefaultValueAttribute(byte value)
	{
		_value = value;
	}

	public DefaultValueAttribute(short value)
	{
		_value = value;
	}

	public DefaultValueAttribute(int value)
	{
		_value = value;
	}

	public DefaultValueAttribute(long value)
	{
		_value = value;
	}

	public DefaultValueAttribute(float value)
	{
		_value = value;
	}

	public DefaultValueAttribute(double value)
	{
		_value = value;
	}

	public DefaultValueAttribute(bool value)
	{
		_value = value;
	}

	public DefaultValueAttribute(string? value)
	{
		_value = value;
	}

	public DefaultValueAttribute(object? value)
	{
		_value = value;
	}

	[CLSCompliant(false)]
	public DefaultValueAttribute(sbyte value)
	{
		_value = value;
	}

	[CLSCompliant(false)]
	public DefaultValueAttribute(ushort value)
	{
		_value = value;
	}

	[CLSCompliant(false)]
	public DefaultValueAttribute(uint value)
	{
		_value = value;
	}

	[CLSCompliant(false)]
	public DefaultValueAttribute(ulong value)
	{
		_value = value;
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj == this)
		{
			return true;
		}
		if (!(obj is DefaultValueAttribute defaultValueAttribute))
		{
			return false;
		}
		if (Value == null)
		{
			return defaultValueAttribute.Value == null;
		}
		return Value.Equals(defaultValueAttribute.Value);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	protected void SetValue(object? value)
	{
		_value = value;
	}
}
