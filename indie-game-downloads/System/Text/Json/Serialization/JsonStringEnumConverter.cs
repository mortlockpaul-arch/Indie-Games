using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Converters;

namespace System.Text.Json.Serialization;

public class JsonStringEnumConverter<TEnum> : JsonConverterFactory where TEnum : struct, Enum
{
	private readonly JsonNamingPolicy _namingPolicy;

	private readonly EnumConverterOptions _converterOptions;

	public JsonStringEnumConverter()
		: this((JsonNamingPolicy?)null, true)
	{
	}

	public JsonStringEnumConverter(JsonNamingPolicy? namingPolicy = null, bool allowIntegerValues = true)
	{
		_namingPolicy = namingPolicy;
		_converterOptions = ((!allowIntegerValues) ? EnumConverterOptions.AllowStrings : (EnumConverterOptions.AllowStrings | EnumConverterOptions.AllowNumbers));
	}

	public sealed override bool CanConvert(Type typeToConvert)
	{
		return typeToConvert == typeof(TEnum);
	}

	public sealed override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
	{
		if (typeToConvert != typeof(TEnum))
		{
			ThrowHelper.ThrowArgumentOutOfRangeException_JsonConverterFactory_TypeNotSupported(typeToConvert);
		}
		return EnumConverterFactory.Helpers.Create<TEnum>(_converterOptions, options, _namingPolicy);
	}
}
[RequiresDynamicCode("JsonStringEnumConverter cannot be statically analyzed and requires runtime code generation. Applications should use the generic JsonStringEnumConverter<TEnum> instead.")]
public class JsonStringEnumConverter : JsonConverterFactory
{
	private readonly JsonNamingPolicy _namingPolicy;

	private readonly EnumConverterOptions _converterOptions;

	public JsonStringEnumConverter()
		: this(null, true)
	{
	}

	public JsonStringEnumConverter(JsonNamingPolicy? namingPolicy = null, bool allowIntegerValues = true)
	{
		_namingPolicy = namingPolicy;
		_converterOptions = ((!allowIntegerValues) ? EnumConverterOptions.AllowStrings : (EnumConverterOptions.AllowStrings | EnumConverterOptions.AllowNumbers));
	}

	public sealed override bool CanConvert(Type typeToConvert)
	{
		return typeToConvert.IsEnum;
	}

	public sealed override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
	{
		if (!typeToConvert.IsEnum)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException_JsonConverterFactory_TypeNotSupported(typeToConvert);
		}
		return EnumConverterFactory.Create(typeToConvert, _converterOptions, _namingPolicy, options);
	}
}
