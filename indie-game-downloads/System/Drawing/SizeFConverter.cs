using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace System.Drawing;

public class SizeFConverter : TypeConverter
{
	private static readonly string[] s_propertySort = new string[2] { "Width", "Height" };

	public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
	{
		if (!(sourceType == typeof(string)))
		{
			return base.CanConvertFrom(context, sourceType);
		}
		return true;
	}

	public override bool CanConvertTo(ITypeDescriptorContext? context, [NotNullWhen(true)] Type? destinationType)
	{
		if (!(destinationType == typeof(InstanceDescriptor)))
		{
			return base.CanConvertTo(context, destinationType);
		}
		return true;
	}

	public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
	{
		if (value is string text)
		{
			ReadOnlySpan<char> source = text.AsSpan().Trim();
			if (source.Length == 0)
			{
				return null;
			}
			if (culture == null)
			{
				culture = CultureInfo.CurrentCulture;
			}
			string listSeparator = culture.TextInfo.ListSeparator;
			Span<Range> destination = stackalloc Range[3];
			if (source.Split(destination, listSeparator.AsSpan()) != 2)
			{
				throw new ArgumentException(System.SR.Format(System.SR.TextParseFailedFormat, source.ToString(), "Width" + listSeparator + " Height"));
			}
			TypeConverter converterTrimUnsafe = TypeDescriptor.GetConverterTrimUnsafe(typeof(float));
			CultureInfo? culture2 = culture;
			Range range = destination[0];
			float width = (float)converterTrimUnsafe.ConvertFromString(context, culture2, text[range.Start..range.End]);
			CultureInfo? culture3 = culture;
			range = destination[1];
			float height = (float)converterTrimUnsafe.ConvertFromString(context, culture3, text[range.Start..range.End]);
			return new SizeF(width, height);
		}
		return base.ConvertFrom(context, culture, value);
	}

	public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
	{
		ArgumentNullException.ThrowIfNull(destinationType, "destinationType");
		if (value is SizeF sizeF)
		{
			if (destinationType == typeof(string))
			{
				if (culture == null)
				{
					culture = CultureInfo.CurrentCulture;
				}
				string listSeparator = culture.TextInfo.ListSeparator;
				TypeConverter converterTrimUnsafe = TypeDescriptor.GetConverterTrimUnsafe(typeof(float));
				string text = converterTrimUnsafe.ConvertToString(context, culture, sizeF.Width);
				string text2 = converterTrimUnsafe.ConvertToString(context, culture, sizeF.Height);
				return text + listSeparator + " " + text2;
			}
			if (destinationType == typeof(InstanceDescriptor))
			{
				ConstructorInfo constructor = typeof(SizeF).GetConstructor(new Type[2]
				{
					typeof(float),
					typeof(float)
				});
				if (constructor != null)
				{
					return new InstanceDescriptor(constructor, new object[2] { sizeF.Width, sizeF.Height });
				}
			}
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override object CreateInstance(ITypeDescriptorContext? context, IDictionary propertyValues)
	{
		ArgumentNullException.ThrowIfNull(propertyValues, "propertyValues");
		object obj = propertyValues["Width"];
		object obj2 = propertyValues["Height"];
		if (obj == null || obj2 == null || !(obj is float) || !(obj2 is float))
		{
			throw new ArgumentException(System.SR.PropertyValueInvalidEntry);
		}
		return new SizeF((float)obj, (float)obj2);
	}

	public override bool GetCreateInstanceSupported(ITypeDescriptorContext? context)
	{
		return true;
	}

	[RequiresUnreferencedCode("The Type of value cannot be statically discovered. The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
	public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
	{
		return TypeDescriptor.GetProperties(typeof(SizeF), attributes).Sort(s_propertySort);
	}

	public override bool GetPropertiesSupported(ITypeDescriptorContext? context)
	{
		return true;
	}
}
