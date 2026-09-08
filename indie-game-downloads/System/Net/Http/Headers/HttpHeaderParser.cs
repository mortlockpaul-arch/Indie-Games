using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace System.Net.Http.Headers;

internal abstract class HttpHeaderParser
{
	public static readonly byte[] DefaultSeparatorBytes = ", "u8.ToArray();

	public bool SupportsMultipleValues { get; }

	public string Separator { get; }

	public byte[] SeparatorBytes { get; }

	public virtual IEqualityComparer Comparer => null;

	protected HttpHeaderParser(bool supportsMultipleValues)
	{
		SupportsMultipleValues = supportsMultipleValues;
		Separator = ", ";
		SeparatorBytes = DefaultSeparatorBytes;
	}

	protected HttpHeaderParser(bool supportsMultipleValues, string separator)
		: this(supportsMultipleValues)
	{
		if (supportsMultipleValues)
		{
			Separator = separator;
			SeparatorBytes = Encoding.ASCII.GetBytes(separator);
		}
	}

	public abstract bool TryParseValue(string value, object storeValue, ref int index, [NotNullWhen(true)] out object parsedValue);

	public object ParseValue(string value, object storeValue, ref int index)
	{
		if (!TryParseValue(value, storeValue, ref index, out var parsedValue))
		{
			throw new FormatException(System.SR.Format(CultureInfo.InvariantCulture, System.SR.net_http_headers_invalid_value, (value == null) ? "<null>" : value.Substring(index)));
		}
		return parsedValue;
	}

	public virtual string ToString(object value)
	{
		return value.ToString();
	}
}
