using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.Net.Http.Headers;

/// <summary>Represents a value which can either be a product or a comment in a User-Agent header.</summary>
public class ProductInfoHeaderValue : ICloneable
{
	private readonly ProductHeaderValue _product;

	private readonly string _comment;

	/// <summary>Gets the product from the <see cref="T:System.Net.Http.Headers.ProductInfoHeaderValue" /> object.</summary>
	/// <returns>The product value from this <see cref="T:System.Net.Http.Headers.ProductInfoHeaderValue" />.</returns>
	public ProductHeaderValue? Product => _product;

	/// <summary>Gets the comment from the <see cref="T:System.Net.Http.Headers.ProductInfoHeaderValue" /> object.</summary>
	/// <returns>The comment value this <see cref="T:System.Net.Http.Headers.ProductInfoHeaderValue" />.</returns>
	public string? Comment => _comment;

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.ProductInfoHeaderValue" /> class.</summary>
	/// <param name="productName">The product name value.</param>
	/// <param name="productVersion">The product version value.</param>
	public ProductInfoHeaderValue(string productName, string? productVersion)
		: this(new ProductHeaderValue(productName, productVersion))
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.ProductInfoHeaderValue" /> class.</summary>
	/// <param name="product">A <see cref="T:System.Net.Http.Headers.ProductInfoHeaderValue" /> object used to initialize the new instance.</param>
	public ProductInfoHeaderValue(ProductHeaderValue product)
	{
		ArgumentNullException.ThrowIfNull(product, "product");
		_product = product;
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.ProductInfoHeaderValue" /> class.</summary>
	/// <param name="comment">A comment value.</param>
	public ProductInfoHeaderValue(string comment)
	{
		HeaderUtilities.CheckValidComment(comment, "comment");
		_comment = comment;
	}

	private ProductInfoHeaderValue(ProductInfoHeaderValue source)
	{
		_product = source._product;
		_comment = source._comment;
	}

	/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.ProductInfoHeaderValue" /> object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override string ToString()
	{
		if (_product == null)
		{
			return _comment;
		}
		return _product.ToString();
	}

	/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.ProductInfoHeaderValue" /> object.</summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>
	///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (!(obj is ProductInfoHeaderValue productInfoHeaderValue))
		{
			return false;
		}
		if (_product == null)
		{
			return string.Equals(_comment, productInfoHeaderValue._comment, StringComparison.Ordinal);
		}
		return _product.Equals(productInfoHeaderValue._product);
	}

	/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.ProductInfoHeaderValue" /> object.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override int GetHashCode()
	{
		if (_product == null)
		{
			return _comment.GetHashCode();
		}
		return _product.GetHashCode();
	}

	/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.ProductInfoHeaderValue" /> instance.</summary>
	/// <param name="input">A string that represents product info header value information.</param>
	/// <returns>A <see cref="T:System.Net.Http.Headers.ProductInfoHeaderValue" /> instance.</returns>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
	/// <exception cref="T:System.FormatException">
	///   <paramref name="input" /> is not valid product info header value information.</exception>
	public static ProductInfoHeaderValue Parse(string input)
	{
		int index = 0;
		object obj = ProductInfoHeaderParser.SingleValueParser.ParseValue(input, null, ref index);
		if (index < input.Length)
		{
			throw new FormatException(System.SR.Format(CultureInfo.InvariantCulture, System.SR.net_http_headers_invalid_value, input.Substring(index)));
		}
		return (ProductInfoHeaderValue)obj;
	}

	/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.ProductInfoHeaderValue" /> information.</summary>
	/// <param name="input">The string to validate.</param>
	/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.ProductInfoHeaderValue" /> version of the string.</param>
	/// <returns>
	///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.ProductInfoHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
	public static bool TryParse([NotNullWhen(true)] string input, [NotNullWhen(true)] out ProductInfoHeaderValue? parsedValue)
	{
		int index = 0;
		parsedValue = null;
		if (ProductInfoHeaderParser.SingleValueParser.TryParseValue(input, null, ref index, out var parsedValue2))
		{
			if (index < input.Length)
			{
				return false;
			}
			parsedValue = (ProductInfoHeaderValue)parsedValue2;
			return true;
		}
		return false;
	}

	internal static int GetProductInfoLength(string input, int startIndex, out ProductInfoHeaderValue parsedValue)
	{
		parsedValue = null;
		if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
		{
			return 0;
		}
		int num = startIndex;
		if (input[num] == '(')
		{
			if (HttpRuleParser.GetCommentLength(input, num, out var length) != HttpParseResult.Parsed)
			{
				return 0;
			}
			string comment = input.Substring(num, length);
			num += length;
			num += HttpRuleParser.GetWhitespaceLength(input, num);
			parsedValue = new ProductInfoHeaderValue(comment);
		}
		else
		{
			int productLength = ProductHeaderValue.GetProductLength(input, num, out var parsedValue2);
			if (productLength == 0)
			{
				return 0;
			}
			num += productLength;
			parsedValue = new ProductInfoHeaderValue(parsedValue2);
		}
		return num - startIndex;
	}

	/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.ProductInfoHeaderValue" /> instance.</summary>
	/// <returns>A copy of the current instance.</returns>
	object ICloneable.Clone()
	{
		return new ProductInfoHeaderValue(this);
	}
}
