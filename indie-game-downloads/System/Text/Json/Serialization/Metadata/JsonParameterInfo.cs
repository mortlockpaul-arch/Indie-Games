using System.Reflection;

namespace System.Text.Json.Serialization.Metadata;

public abstract class JsonParameterInfo
{
	private ICustomAttributeProvider _attributeProvider;

	public Type DeclaringType => MatchingProperty.DeclaringType;

	public int Position { get; }

	public Type ParameterType => MatchingProperty.PropertyType;

	public string Name { get; }

	public bool HasDefaultValue { get; }

	public object? DefaultValue { get; }

	internal object? EffectiveDefaultValue { get; private protected init; }

	public bool IsNullable => MatchingProperty.IsSetNullable;

	public bool IsMemberInitializer { get; }

	public ICustomAttributeProvider? AttributeProvider
	{
		get
		{
			ICustomAttributeProvider customAttributeProvider = _attributeProvider;
			if (customAttributeProvider == null && MatchingProperty.DeclaringTypeInfo.ConstructorAttributeProvider is MethodBase methodBase)
			{
				ParameterInfo[] parameters = methodBase.GetParameters();
				if (Position < parameters.Length)
				{
					customAttributeProvider = (_attributeProvider = parameters[Position]);
				}
			}
			return customAttributeProvider;
		}
	}

	internal JsonPropertyInfo MatchingProperty { get; }

	internal JsonConverter EffectiveConverter => MatchingProperty.EffectiveConverter;

	internal bool IgnoreNullTokensOnRead => MatchingProperty.IgnoreNullTokensOnRead;

	internal JsonSerializerOptions Options => MatchingProperty.Options;

	internal byte[] JsonNameAsUtf8Bytes => MatchingProperty.NameAsUtf8Bytes;

	internal JsonNumberHandling? NumberHandling => MatchingProperty.EffectiveNumberHandling;

	internal JsonTypeInfo JsonTypeInfo => MatchingProperty.JsonTypeInfo;

	internal bool ShouldDeserialize => !MatchingProperty.IsIgnored;

	internal bool IsRequiredParameter
	{
		get
		{
			if (!HasDefaultValue)
			{
				return !IsMemberInitializer;
			}
			return false;
		}
	}

	internal JsonParameterInfo(JsonParameterInfoValues parameterInfoValues, JsonPropertyInfo matchingProperty)
	{
		Position = parameterInfoValues.Position;
		Name = parameterInfoValues.Name;
		HasDefaultValue = parameterInfoValues.HasDefaultValue;
		DefaultValue = (parameterInfoValues.HasDefaultValue ? parameterInfoValues.DefaultValue : null);
		MatchingProperty = matchingProperty;
		IsMemberInitializer = parameterInfoValues.IsMemberInitializer;
	}
}
internal sealed class JsonParameterInfo<T> : JsonParameterInfo
{
	public new JsonConverter<T> EffectiveConverter => MatchingProperty.EffectiveConverter;

	public new JsonPropertyInfo<T> MatchingProperty { get; }

	public new T EffectiveDefaultValue { get; }

	public JsonParameterInfo(JsonParameterInfoValues parameterInfoValues, JsonPropertyInfo<T> matchingPropertyInfo)
		: base(parameterInfoValues, matchingPropertyInfo)
	{
		if (parameterInfoValues != null && parameterInfoValues.HasDefaultValue)
		{
			object defaultValue = parameterInfoValues.DefaultValue;
			if (defaultValue != null)
			{
				EffectiveDefaultValue = (T)defaultValue;
			}
		}
		MatchingProperty = matchingPropertyInfo;
		base.EffectiveDefaultValue = EffectiveDefaultValue;
	}
}
