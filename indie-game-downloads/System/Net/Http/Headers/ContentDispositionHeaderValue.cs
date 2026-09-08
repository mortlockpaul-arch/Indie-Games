using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace System.Net.Http.Headers;

/// <summary>Represents the value of the Content-Disposition header.</summary>
public class ContentDispositionHeaderValue : ICloneable
{
	private UnvalidatedObjectCollection<NameValueHeaderValue> _parameters;

	private string _dispositionType;

	/// <summary>The disposition type for a content body part.</summary>
	/// <returns>The disposition type.</returns>
	public string DispositionType
	{
		get
		{
			return _dispositionType;
		}
		set
		{
			HeaderUtilities.CheckValidToken(value, "value");
			_dispositionType = value;
		}
	}

	/// <summary>A set of parameters included the Content-Disposition header.</summary>
	/// <returns>A collection of parameters.</returns>
	public ICollection<NameValueHeaderValue> Parameters => _parameters ?? (_parameters = new UnvalidatedObjectCollection<NameValueHeaderValue>());

	/// <summary>The name for a content body part.</summary>
	/// <returns>The name for the content body part.</returns>
	public string? Name
	{
		get
		{
			return GetName("name");
		}
		set
		{
			SetName("name", value);
		}
	}

	/// <summary>A suggestion for how to construct a filename for   storing the message payload to be used if the entity is   detached and stored in a separate file.</summary>
	/// <returns>A suggested filename.</returns>
	public string? FileName
	{
		get
		{
			return GetName("filename");
		}
		set
		{
			SetName("filename", value);
		}
	}

	/// <summary>A suggestion for how to construct filenames for   storing message payloads to be used if the entities are    detached and stored in a separate files.</summary>
	/// <returns>A suggested filename of the form filename*.</returns>
	public string? FileNameStar
	{
		get
		{
			return GetName("filename*");
		}
		set
		{
			SetName("filename*", value);
		}
	}

	/// <summary>The date at which   the file was created.</summary>
	/// <returns>The file creation date.</returns>
	public DateTimeOffset? CreationDate
	{
		get
		{
			return GetDate("creation-date");
		}
		set
		{
			SetDate("creation-date", value);
		}
	}

	/// <summary>The date at   which the file was last modified.</summary>
	/// <returns>The file modification date.</returns>
	public DateTimeOffset? ModificationDate
	{
		get
		{
			return GetDate("modification-date");
		}
		set
		{
			SetDate("modification-date", value);
		}
	}

	/// <summary>The date the file was last read.</summary>
	/// <returns>The last read date.</returns>
	public DateTimeOffset? ReadDate
	{
		get
		{
			return GetDate("read-date");
		}
		set
		{
			SetDate("read-date", value);
		}
	}

	/// <summary>The approximate size, in bytes, of the file.</summary>
	/// <returns>The approximate size, in bytes.</returns>
	public long? Size
	{
		get
		{
			NameValueHeaderValue nameValueHeaderValue = NameValueHeaderValue.Find(_parameters, "size");
			if (nameValueHeaderValue != null && ulong.TryParse(nameValueHeaderValue.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
			{
				return (long)result;
			}
			return null;
		}
		set
		{
			NameValueHeaderValue nameValueHeaderValue = NameValueHeaderValue.Find(_parameters, "size");
			if (!value.HasValue)
			{
				if (nameValueHeaderValue != null)
				{
					_parameters.Remove(nameValueHeaderValue);
				}
				return;
			}
			ArgumentOutOfRangeException.ThrowIfNegative(value.GetValueOrDefault(), "value.GetValueOrDefault()");
			if (nameValueHeaderValue != null)
			{
				nameValueHeaderValue.Value = value.Value.ToString(CultureInfo.InvariantCulture);
				return;
			}
			string value2 = value.Value.ToString(CultureInfo.InvariantCulture);
			Parameters.Add(new NameValueHeaderValue("size", value2));
		}
	}

	private ContentDispositionHeaderValue()
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" /> class.</summary>
	/// <param name="source">A <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" />.</param>
	protected ContentDispositionHeaderValue(ContentDispositionHeaderValue source)
	{
		_dispositionType = source._dispositionType;
		_parameters = source._parameters.Clone();
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" /> class.</summary>
	/// <param name="dispositionType">A string that contains a <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" />.</param>
	public ContentDispositionHeaderValue(string dispositionType)
	{
		HeaderUtilities.CheckValidToken(dispositionType, "dispositionType");
		_dispositionType = dispositionType;
	}

	/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" /> object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override string ToString()
	{
		StringBuilder stringBuilder = System.Text.StringBuilderCache.Acquire();
		stringBuilder.Append(_dispositionType);
		NameValueHeaderValue.ToString(_parameters, ';', leadingSeparator: true, stringBuilder);
		return System.Text.StringBuilderCache.GetStringAndRelease(stringBuilder);
	}

	/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" /> object.</summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>
	///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (!(obj is ContentDispositionHeaderValue contentDispositionHeaderValue))
		{
			return false;
		}
		if (string.Equals(_dispositionType, contentDispositionHeaderValue._dispositionType, StringComparison.OrdinalIgnoreCase))
		{
			return HeaderUtilities.AreEqualCollections(_parameters, contentDispositionHeaderValue._parameters);
		}
		return false;
	}

	/// <summary>Serves as a hash function for an  <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" /> object.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override int GetHashCode()
	{
		return StringComparer.OrdinalIgnoreCase.GetHashCode(_dispositionType) ^ NameValueHeaderValue.GetHashCode(_parameters);
	}

	/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" /> instance.</summary>
	/// <returns>A copy of the current instance.</returns>
	object ICloneable.Clone()
	{
		return new ContentDispositionHeaderValue(this);
	}

	/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" /> instance.</summary>
	/// <param name="input">A string that represents content disposition header value information.</param>
	/// <returns>An <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" /> instance.</returns>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
	/// <exception cref="T:System.FormatException">
	///   <paramref name="input" /> is not valid content disposition header value information.</exception>
	public static ContentDispositionHeaderValue Parse(string input)
	{
		int index = 0;
		return (ContentDispositionHeaderValue)GenericHeaderParser.ContentDispositionParser.ParseValue(input, null, ref index);
	}

	/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" /> information.</summary>
	/// <param name="input">The string to validate.</param>
	/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" /> version of the string.</param>
	/// <returns>
	///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
	public static bool TryParse([NotNullWhen(true)] string? input, [NotNullWhen(true)] out ContentDispositionHeaderValue? parsedValue)
	{
		int index = 0;
		parsedValue = null;
		if (GenericHeaderParser.ContentDispositionParser.TryParseValue(input, null, ref index, out var parsedValue2))
		{
			parsedValue = (ContentDispositionHeaderValue)parsedValue2;
			return true;
		}
		return false;
	}

	internal static int GetDispositionTypeLength(string input, int startIndex, out object parsedValue)
	{
		parsedValue = null;
		if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
		{
			return 0;
		}
		int dispositionTypeExpressionLength = GetDispositionTypeExpressionLength(input, startIndex, out var dispositionType);
		if (dispositionTypeExpressionLength == 0)
		{
			return 0;
		}
		int num = startIndex + dispositionTypeExpressionLength;
		num += HttpRuleParser.GetWhitespaceLength(input, num);
		ContentDispositionHeaderValue contentDispositionHeaderValue = new ContentDispositionHeaderValue();
		contentDispositionHeaderValue._dispositionType = dispositionType;
		if (num < input.Length && input[num] == ';')
		{
			num++;
			int nameValueListLength = NameValueHeaderValue.GetNameValueListLength(input, num, ';', (UnvalidatedObjectCollection<NameValueHeaderValue>)contentDispositionHeaderValue.Parameters);
			if (nameValueListLength == 0)
			{
				return 0;
			}
			parsedValue = contentDispositionHeaderValue;
			return num + nameValueListLength - startIndex;
		}
		parsedValue = contentDispositionHeaderValue;
		return num - startIndex;
	}

	private static int GetDispositionTypeExpressionLength(string input, int startIndex, out string dispositionType)
	{
		dispositionType = null;
		int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
		if (tokenLength == 0)
		{
			return 0;
		}
		dispositionType = input.Substring(startIndex, tokenLength);
		return tokenLength;
	}

	private DateTimeOffset? GetDate(string parameter)
	{
		NameValueHeaderValue nameValueHeaderValue = NameValueHeaderValue.Find(_parameters, parameter);
		if (nameValueHeaderValue != null)
		{
			ReadOnlySpan<char> readOnlySpan = nameValueHeaderValue.Value.AsSpan();
			if (IsQuoted(readOnlySpan))
			{
				readOnlySpan = readOnlySpan.Slice(1, readOnlySpan.Length - 2);
			}
			if (HttpDateParser.TryParse(readOnlySpan, out var result))
			{
				return result;
			}
		}
		return null;
	}

	private void SetDate(string parameter, DateTimeOffset? date)
	{
		NameValueHeaderValue nameValueHeaderValue = NameValueHeaderValue.Find(_parameters, parameter);
		if (!date.HasValue)
		{
			if (nameValueHeaderValue != null)
			{
				_parameters.Remove(nameValueHeaderValue);
			}
			return;
		}
		string value = $"\"{date.GetValueOrDefault():r}\"";
		if (nameValueHeaderValue != null)
		{
			nameValueHeaderValue.Value = value;
		}
		else
		{
			Parameters.Add(new NameValueHeaderValue(parameter, value));
		}
	}

	private string GetName(string parameter)
	{
		NameValueHeaderValue nameValueHeaderValue = NameValueHeaderValue.Find(_parameters, parameter);
		if (nameValueHeaderValue != null)
		{
			string output;
			if (parameter.EndsWith('*'))
			{
				if (TryDecode5987(nameValueHeaderValue.Value, out output))
				{
					return output;
				}
				return null;
			}
			if (TryDecodeMime(nameValueHeaderValue.Value, out output))
			{
				return output;
			}
			if (!IsQuoted(nameValueHeaderValue.Value.AsSpan()))
			{
				return nameValueHeaderValue.Value;
			}
			return nameValueHeaderValue.Value.Substring(1, nameValueHeaderValue.Value.Length - 2);
		}
		return null;
	}

	private void SetName(string parameter, string value)
	{
		NameValueHeaderValue nameValueHeaderValue = NameValueHeaderValue.Find(_parameters, parameter);
		if (string.IsNullOrEmpty(value))
		{
			if (nameValueHeaderValue != null)
			{
				_parameters.Remove(nameValueHeaderValue);
			}
			return;
		}
		string value2 = ((!parameter.EndsWith('*')) ? EncodeAndQuoteMime(value) : HeaderUtilities.Encode5987(value));
		if (nameValueHeaderValue != null)
		{
			nameValueHeaderValue.Value = value2;
		}
		else
		{
			Parameters.Add(new NameValueHeaderValue(parameter, value2));
		}
	}

	private static string EncodeAndQuoteMime(string input)
	{
		string text = input;
		bool flag = false;
		if (IsQuoted(text.AsSpan()))
		{
			text = text.Substring(1, text.Length - 2);
			flag = true;
		}
		if (text.Contains('"'))
		{
			throw new ArgumentException(System.SR.Format(CultureInfo.InvariantCulture, System.SR.net_http_headers_invalid_value, input));
		}
		if (!Ascii.IsValid(text.AsSpan()))
		{
			flag = true;
			text = EncodeMime(text);
		}
		else if (!flag && HttpRuleParser.GetTokenLength(text, 0) != text.Length)
		{
			flag = true;
		}
		if (flag)
		{
			text = "\"" + text + "\"";
		}
		return text;
	}

	private static bool IsQuoted(ReadOnlySpan<char> value)
	{
		if (value.Length > 1 && value[0] == '"')
		{
			return value[value.Length - 1] == '"';
		}
		return false;
	}

	private static string EncodeMime(string input)
	{
		string text = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
		return "=?utf-8?B?" + text + "?=";
	}

	private static bool TryDecodeMime(string input, [NotNullWhen(true)] out string output)
	{
		output = null;
		if (!IsQuoted(input.AsSpan()) || input.Length < 10)
		{
			return false;
		}
		Span<Range> destination = stackalloc Range[6];
		ReadOnlySpan<char> source = input.AsSpan();
		if (source.Split(destination, '?') == 5)
		{
			Range range = destination[0];
			if (source[range.Start..range.End].SequenceEqual("\"=".AsSpan()))
			{
				range = destination[4];
				if (source[range.Start..range.End].SequenceEqual("=\"".AsSpan()))
				{
					range = destination[2];
					if (source[range.Start..range.End].Equals("b".AsSpan(), StringComparison.OrdinalIgnoreCase))
					{
						try
						{
							range = destination[1];
							Encoding encoding = Encoding.GetEncoding(input[range.Start..range.End]);
							range = destination[3];
							byte[] array = Convert.FromBase64String(input[range.Start..range.End]);
							output = encoding.GetString(array, 0, array.Length);
							return true;
						}
						catch (ArgumentException)
						{
						}
						catch (FormatException)
						{
						}
						return false;
					}
				}
			}
		}
		return false;
	}

	private static bool TryDecode5987(string input, out string output)
	{
		output = null;
		int num = input.IndexOf('\'');
		if (num == -1)
		{
			return false;
		}
		int num2 = input.LastIndexOf('\'');
		if (num == num2 || input.IndexOf('\'', num + 1) != num2)
		{
			return false;
		}
		string name = input.Substring(0, num);
		string text = input.Substring(num2 + 1);
		StringBuilder stringBuilder = new StringBuilder();
		try
		{
			Encoding encoding = Encoding.GetEncoding(name);
			byte[] array = new byte[text.Length];
			int num3 = 0;
			for (int i = 0; i < text.Length; i++)
			{
				if (Uri.IsHexEncoding(text, i))
				{
					array[num3++] = (byte)Uri.HexUnescape(text, ref i);
					i--;
					continue;
				}
				if (num3 > 0)
				{
					stringBuilder.Append(encoding.GetString(array, 0, num3));
					num3 = 0;
				}
				stringBuilder.Append(text[i]);
			}
			if (num3 > 0)
			{
				stringBuilder.Append(encoding.GetString(array, 0, num3));
			}
		}
		catch (ArgumentException)
		{
			return false;
		}
		output = stringBuilder.ToString();
		return true;
	}
}
