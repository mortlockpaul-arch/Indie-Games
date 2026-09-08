using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Text;

namespace Microsoft.Xna.Framework.Design;

public class MathTypeConverter : ExpandableObjectConverter
{
	protected PropertyDescriptorCollection propertyDescriptions;

	protected bool supportStringConvert = true;

	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		return (supportStringConvert && sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType);
	}

	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
	}

	public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
	{
		return true;
	}

	public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
	{
		return propertyDescriptions;
	}

	public override bool GetPropertiesSupported(ITypeDescriptorContext context)
	{
		return true;
	}

	internal static string ConvertToString<T>(CultureInfo culture, params T[] array) where T : struct, IConvertible
	{
		if (culture == null)
		{
			culture = CultureInfo.CurrentCulture;
		}
		string text = culture.TextInfo.ListSeparator + " ";
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString(culture));
			stringBuilder.Append(text);
		}
		stringBuilder.Length -= text.Length;
		return stringBuilder.ToString();
	}
}
