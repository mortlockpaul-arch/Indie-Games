using System.Diagnostics.CodeAnalysis;

namespace System.Net.Http.Headers;

/// <summary>Represents a product token value in a User-Agent header.</summary>
public class ProductHeaderValue : ICloneable
{
	private readonly string _name;

	private readonly string _version;

	/// <summary>Gets the name of the product token.</summary>
	/// <returns>The name of the product token.</returns>
	public string Name => _name;

	/// <summary>Gets the version of the product token.</summary>
	/// <returns>The version of the product token.</returns>
	public string? Version => _version;

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.ProductHeaderValue" /> class.</summary>
	/// <param name="name">The product name.</param>
	public ProductHeaderValue(string name)
		: this(name, null)
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.ProductHeaderValue" /> class.</summary>
	/// <param name="name">The product name value.</param>
	/// <param name="version">The product version value.</param>
	public ProductHeaderValue(string name, string? version)
	{
		HeaderUtilities.CheckValidToken(name, "name");
		if (!string.IsNullOrEmpty(version))
		{
			HeaderUtilities.CheckValidToken(version, "version");
			_version = version;
		}
		_name = name;
	}

	private ProductHeaderValue(ProductHeaderValue source)
	{
		_name = source._name;
		_version = source._version;
	}

	/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.ProductHeaderValue" /> object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override string ToString()
	{
		if (string.IsNullOrEmpty(_version))
		{
			return _name;
		}
		return _name + "/" + _version;
	}

	/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.ProductHeaderValue" /> object.</summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>
	///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (!(obj is ProductHeaderValue productHeaderValue))
		{
			return false;
		}
		if (string.Equals(_name, productHeaderValue._name, StringComparison.OrdinalIgnoreCase))
		{
			return string.Equals(_version, productHeaderValue._version, StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.ProductHeaderValue" /> object.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override int GetHashCode()
	{
		int num = StringComparer.OrdinalIgnoreCase.GetHashCode(_name);
		if (!string.IsNullOrEmpty(_version))
		{
			num ^= StringComparer.OrdinalIgnoreCase.GetHashCode(_version);
		}
		return num;
	}

	/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.ProductHeaderValue" /> instance.</summary>
	/// <param name="input">A string that represents product header value information.</param>
	/// <returns>A <see cref="T:System.Net.Http.Headers.ProductHeaderValue" /> instance.</returns>
	public static ProductHeaderValue Parse(string input)
	{
		int index = 0;
		return (ProductHeaderValue)GenericHeaderParser.SingleValueProductParser.ParseValue(input, null, ref index);
	}

	/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.ProductHeaderValue" /> information.</summary>
	/// <param name="input">The string to validate.</param>
	/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.ProductHeaderValue" /> version of the string.</param>
	/// <returns>
	///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.ProductHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
	public static bool TryParse([NotNullWhen(true)] string? input, [NotNullWhen(true)] out ProductHeaderValue? parsedValue)
	{
		int index = 0;
		parsedValue = null;
		if (GenericHeaderParser.SingleValueProductParser.TryParseValue(input, null, ref index, out var parsedValue2))
		{
			parsedValue = (ProductHeaderValue)parsedValue2;
			return true;
		}
		return false;
	}

	internal static int GetProductLength(string input, int startIndex, out ProductHeaderValue parsedValue)
	{
		parsedValue = null;
		if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
		{
			return 0;
		}
		int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
		if (tokenLength == 0)
		{
			return 0;
		}
		string name = input.Substring(startIndex, tokenLength);
		int num = startIndex + tokenLength;
		num += HttpRuleParser.GetWhitespaceLength(input, num);
		if (num == input.Length || input[num] != '/')
		{
			parsedValue = new ProductHeaderValue(name);
			return num - startIndex;
		}
		num++;
		num += HttpRuleParser.GetWhitespaceLength(input, num);
		int tokenLength2 = HttpRuleParser.GetTokenLength(input, num);
		if (tokenLength2 == 0)
		{
			return 0;
		}
		string version = input.Substring(num, tokenLength2);
		num += tokenLength2;
		num += HttpRuleParser.GetWhitespaceLength(input, num);
		parsedValue = new ProductHeaderValue(name, version);
		return num - startIndex;
	}

	/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.ProductHeaderValue" /> instance.</summary>
	/// <returns>A copy of the current instance.</returns>
	object ICloneable.Clone()
	{
		return new ProductHeaderValue(this);
	}
}
