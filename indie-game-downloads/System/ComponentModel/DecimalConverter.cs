using System.ComponentModel.Design.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.ComponentModel;

public class DecimalConverter : BaseNumberConverter
{
	internal override bool AllowHex => false;

	internal override Type TargetType => typeof(decimal);

	public override bool CanConvertTo(ITypeDescriptorContext? context, [NotNullWhen(true)] Type? destinationType)
	{
		if (!(destinationType == typeof(InstanceDescriptor)))
		{
			return base.CanConvertTo(context, destinationType);
		}
		return true;
	}

	public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
	{
		if (destinationType == typeof(InstanceDescriptor) && value is decimal d)
		{
			return new InstanceDescriptor(typeof(decimal).GetConstructor(new Type[1] { typeof(int[]) }), new object[1] { decimal.GetBits(d) });
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	internal override object FromString(string value, int radix)
	{
		return Convert.ToDecimal(value, CultureInfo.CurrentCulture);
	}

	internal override object FromString(string value, NumberFormatInfo formatInfo)
	{
		return decimal.Parse(value, NumberStyles.Float, formatInfo);
	}

	internal override string ToString(object value, NumberFormatInfo formatInfo)
	{
		return ((decimal)value).ToString("G", formatInfo);
	}
}
