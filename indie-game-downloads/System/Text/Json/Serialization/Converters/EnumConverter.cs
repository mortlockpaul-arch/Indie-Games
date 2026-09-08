using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;

namespace System.Text.Json.Serialization.Converters;

internal sealed class EnumConverter<T> : JsonPrimitiveConverter<T> where T : struct, Enum
{
	private sealed class EnumFieldInfo(ulong key, EnumFieldNameKind kind, string originalName, string jsonName)
	{
		private List<EnumFieldInfo> _conflictingFields;

		public EnumFieldNameKind Kind { get; } = kind;

		public ulong Key { get; } = key;

		public string OriginalName { get; } = originalName;

		public string JsonName { get; } = jsonName;

		public void AppendConflictingField(EnumFieldInfo other)
		{
			if (ConflictsWith(this, other))
			{
				return;
			}
			List<EnumFieldInfo> list = _conflictingFields ?? (_conflictingFields = new List<EnumFieldInfo>());
			foreach (EnumFieldInfo item in list)
			{
				if (ConflictsWith(item, other))
				{
					return;
				}
			}
			list.Add(other);
			static bool ConflictsWith(EnumFieldInfo current, EnumFieldInfo enumFieldInfo)
			{
				if (current.Kind == EnumFieldNameKind.Default)
				{
					return true;
				}
				if (enumFieldInfo.Kind == EnumFieldNameKind.Default)
				{
					return false;
				}
				return current.JsonName.Equals(enumFieldInfo.JsonName, StringComparison.Ordinal);
			}
		}

		public EnumFieldInfo GetMatchingField(ReadOnlySpan<char> input)
		{
			if (Kind == EnumFieldNameKind.Default || input.SequenceEqual(JsonName.AsSpan()))
			{
				return this;
			}
			List<EnumFieldInfo> conflictingFields = _conflictingFields;
			if (conflictingFields != null)
			{
				foreach (EnumFieldInfo item in conflictingFields)
				{
					if (item.Kind == EnumFieldNameKind.Default || input.SequenceEqual(item.JsonName.AsSpan()))
					{
						return item;
					}
				}
			}
			return null;
		}
	}

	private enum EnumFieldNameKind
	{
		Default,
		NamingPolicy,
		Attribute
	}

	private static readonly TypeCode s_enumTypeCode = System.Type.GetTypeCode(typeof(T));

	private static readonly bool s_isSignedEnum = (int)s_enumTypeCode % 2 == 1;

	private static readonly bool s_isFlagsEnum = typeof(T).IsDefined(typeof(FlagsAttribute), inherit: false);

	private readonly EnumConverterOptions _converterOptions;

	private readonly JsonNamingPolicy _namingPolicy;

	private readonly EnumFieldInfo[] _enumFieldInfo;

	private readonly Dictionary<string, EnumFieldInfo> _enumFieldInfoIndex;

	private readonly ConcurrentDictionary<ulong, JsonEncodedText> _nameCacheForWriting;

	private readonly ConcurrentDictionary<string, ulong> _nameCacheForReading;

	public EnumConverter(EnumConverterOptions converterOptions, JsonNamingPolicy namingPolicy, JsonSerializerOptions options)
	{
		_converterOptions = converterOptions;
		_namingPolicy = namingPolicy;
		_enumFieldInfo = ResolveEnumFields(namingPolicy);
		_enumFieldInfoIndex = new Dictionary<string, EnumFieldInfo>(StringComparer.OrdinalIgnoreCase);
		_nameCacheForWriting = new ConcurrentDictionary<ulong, JsonEncodedText>();
		_nameCacheForReading = new ConcurrentDictionary<string, ulong>(StringComparer.Ordinal);
		JavaScriptEncoder encoder = options.Encoder;
		EnumFieldInfo[] enumFieldInfo = _enumFieldInfo;
		foreach (EnumFieldInfo enumFieldInfo2 in enumFieldInfo)
		{
			AddToEnumFieldIndex(enumFieldInfo2);
			JsonEncodedText value = JsonEncodedText.Encode(enumFieldInfo2.JsonName, encoder);
			_nameCacheForWriting.TryAdd(enumFieldInfo2.Key, value);
			_nameCacheForReading.TryAdd(enumFieldInfo2.JsonName, enumFieldInfo2.Key);
		}
		if (namingPolicy == null)
		{
			return;
		}
		enumFieldInfo = _enumFieldInfo;
		foreach (EnumFieldInfo enumFieldInfo3 in enumFieldInfo)
		{
			if (enumFieldInfo3.Kind == EnumFieldNameKind.NamingPolicy)
			{
				AddToEnumFieldIndex(new EnumFieldInfo(enumFieldInfo3.Key, EnumFieldNameKind.Default, enumFieldInfo3.OriginalName, enumFieldInfo3.OriginalName));
			}
		}
		void AddToEnumFieldIndex(EnumFieldInfo fieldInfo)
		{
			if (!_enumFieldInfoIndex.TryAdd(fieldInfo.JsonName, fieldInfo))
			{
				_enumFieldInfoIndex[fieldInfo.JsonName].AppendConflictingField(fieldInfo);
			}
		}
	}

	public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		switch (reader.TokenType)
		{
		case JsonTokenType.String:
		{
			if ((_converterOptions & EnumConverterOptions.AllowStrings) != 0 && TryParseEnumFromString(ref reader, out var result))
			{
				return result;
			}
			break;
		}
		case JsonTokenType.Number:
			if ((_converterOptions & EnumConverterOptions.AllowNumbers) == 0)
			{
				break;
			}
			switch (s_enumTypeCode)
			{
			case TypeCode.Int32:
			{
				if (reader.TryGetInt32(out var value8))
				{
					return (T)(object)value8;
				}
				break;
			}
			case TypeCode.UInt32:
			{
				if (reader.TryGetUInt32(out var value4))
				{
					return (T)(object)value4;
				}
				break;
			}
			case TypeCode.Int64:
			{
				if (reader.TryGetInt64(out var value6))
				{
					return (T)(object)value6;
				}
				break;
			}
			case TypeCode.UInt64:
			{
				if (reader.TryGetUInt64(out var value2))
				{
					return (T)(object)value2;
				}
				break;
			}
			case TypeCode.Byte:
			{
				if (reader.TryGetByte(out var value7))
				{
					return (T)(object)value7;
				}
				break;
			}
			case TypeCode.SByte:
			{
				if (reader.TryGetSByte(out var value5))
				{
					return (T)(object)value5;
				}
				break;
			}
			case TypeCode.Int16:
			{
				if (reader.TryGetInt16(out var value3))
				{
					return (T)(object)value3;
				}
				break;
			}
			case TypeCode.UInt16:
			{
				if (reader.TryGetUInt16(out var value))
				{
					return (T)(object)value;
				}
				break;
			}
			}
			break;
		}
		ThrowHelper.ThrowJsonException();
		return default(T);
	}

	public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
	{
		EnumConverterOptions converterOptions = _converterOptions;
		if ((converterOptions & EnumConverterOptions.AllowStrings) != 0)
		{
			ulong key = ConvertToUInt64(value);
			if (_nameCacheForWriting.TryGetValue(key, out var value2))
			{
				writer.WriteStringValue(value2);
				return;
			}
			if (IsDefinedValueOrCombinationOfValues(key))
			{
				string value3 = FormatEnumAsString(key, value, null);
				if (_nameCacheForWriting.Count < 64)
				{
					value2 = JsonEncodedText.Encode(value3, options.Encoder);
					writer.WriteStringValue(value2);
					_nameCacheForWriting.TryAdd(key, value2);
				}
				else
				{
					writer.WriteStringValue(value3);
				}
				return;
			}
		}
		if ((converterOptions & EnumConverterOptions.AllowNumbers) == 0)
		{
			ThrowHelper.ThrowJsonException();
		}
		if (s_isSignedEnum)
		{
			writer.WriteNumberValue(ConvertToInt64(value));
		}
		else
		{
			writer.WriteNumberValue(ConvertToUInt64(value));
		}
	}

	internal override T ReadAsPropertyNameCore(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (!TryParseEnumFromString(ref reader, out var result))
		{
			ThrowHelper.ThrowJsonException();
		}
		return result;
	}

	internal override void WriteAsPropertyNameCore(Utf8JsonWriter writer, T value, JsonSerializerOptions options, bool isWritingExtensionDataProperty)
	{
		JsonNamingPolicy dictionaryKeyPolicy = options.DictionaryKeyPolicy;
		JsonNamingPolicy jsonNamingPolicy = ((dictionaryKeyPolicy != null && dictionaryKeyPolicy != _namingPolicy) ? dictionaryKeyPolicy : null);
		ulong num = ConvertToUInt64(value);
		if (jsonNamingPolicy == null && _nameCacheForWriting.TryGetValue(num, out var value2))
		{
			writer.WritePropertyName(value2);
		}
		else if (IsDefinedValueOrCombinationOfValues(num))
		{
			string text = FormatEnumAsString(num, value, jsonNamingPolicy);
			if (jsonNamingPolicy == null && _nameCacheForWriting.Count < 64)
			{
				value2 = JsonEncodedText.Encode(text, options.Encoder);
				writer.WritePropertyName(value2);
				_nameCacheForWriting.TryAdd(num, value2);
			}
			else
			{
				writer.WritePropertyName(text);
			}
		}
		else if (s_isSignedEnum)
		{
			writer.WritePropertyName(ConvertToInt64(value));
		}
		else
		{
			writer.WritePropertyName(num);
		}
	}

	private bool TryParseEnumFromString(ref Utf8JsonReader reader, out T result)
	{
		int valueLength = reader.ValueLength;
		char[] array = null;
		Span<char> span = ((valueLength > 128) ? ((Span<char>)(array = ArrayPool<char>.Shared.Rent(valueLength))) : stackalloc char[128]);
		Span<char> span2 = span;
		span2 = span2[..reader.CopyString(span2)];
		ReadOnlySpan<char> readOnlySpan = span2.Trim();
		ConcurrentDictionary<string, ulong>.AlternateLookup<ReadOnlySpan<char>> alternateLookup = _nameCacheForReading.GetAlternateLookup<ReadOnlySpan<char>>();
		bool flag;
		if (alternateLookup.TryGetValue(readOnlySpan, out var value))
		{
			result = ConvertFromUInt64(value);
			flag = true;
		}
		else
		{
			if (JsonHelpers.IntegerRegex.IsMatch(readOnlySpan))
			{
				if ((_converterOptions & EnumConverterOptions.AllowNumbers) != 0)
				{
					flag = Enum.TryParse<T>(readOnlySpan, out result);
				}
				else
				{
					result = default(T);
					flag = false;
				}
			}
			else
			{
				flag = TryParseNamedEnum(readOnlySpan, out result);
			}
			if (flag && _nameCacheForReading.Count < 64)
			{
				alternateLookup.TryAdd(readOnlySpan, ConvertToUInt64(result));
			}
		}
		if (array != null)
		{
			span2.Clear();
			ArrayPool<char>.Shared.Return(array);
		}
		return flag;
	}

	private bool TryParseNamedEnum(ReadOnlySpan<char> source, out T result)
	{
		Dictionary<string, EnumFieldInfo>.AlternateLookup<ReadOnlySpan<char>> alternateLookup = _enumFieldInfoIndex.GetAlternateLookup<ReadOnlySpan<char>>();
		ReadOnlySpan<char> readOnlySpan = source;
		ulong num = 0uL;
		while (true)
		{
			int num2 = readOnlySpan.IndexOf(',');
			ReadOnlySpan<char> readOnlySpan2;
			if (num2 == -1)
			{
				readOnlySpan2 = readOnlySpan;
				readOnlySpan = default(ReadOnlySpan<char>);
			}
			else
			{
				readOnlySpan2 = readOnlySpan.Slice(0, num2).TrimEnd();
				readOnlySpan = readOnlySpan.Slice(num2 + 1).TrimStart();
			}
			if (alternateLookup.TryGetValue(readOnlySpan2, out var value))
			{
				EnumFieldInfo matchingField = value.GetMatchingField(readOnlySpan2);
				if (matchingField != null)
				{
					num |= matchingField.Key;
					if (readOnlySpan.IsEmpty)
					{
						break;
					}
					continue;
				}
			}
			result = default(T);
			return false;
		}
		result = ConvertFromUInt64(num);
		return true;
	}

	private static ulong ConvertToUInt64(T value)
	{
		return s_enumTypeCode switch
		{
			TypeCode.Int32 => (ulong)(int)(object)value, 
			TypeCode.UInt32 => (uint)(object)value, 
			TypeCode.Int64 => (ulong)(long)(object)value, 
			TypeCode.UInt64 => (ulong)(object)value, 
			TypeCode.Int16 => (ulong)(short)(object)value, 
			TypeCode.UInt16 => (ushort)(object)value, 
			TypeCode.SByte => (ulong)(sbyte)(object)value, 
			_ => (byte)(object)value, 
		};
	}

	private static long ConvertToInt64(T value)
	{
		return s_enumTypeCode switch
		{
			TypeCode.Int32 => (int)(object)value, 
			TypeCode.Int64 => (long)(object)value, 
			TypeCode.Int16 => (short)(object)value, 
			_ => (sbyte)(object)value, 
		};
	}

	private static T ConvertFromUInt64(ulong value)
	{
		return s_enumTypeCode switch
		{
			TypeCode.Int32 => (T)(object)(int)value, 
			TypeCode.UInt32 => (T)(object)(uint)value, 
			TypeCode.Int64 => (T)(object)(long)value, 
			TypeCode.UInt64 => (T)(object)value, 
			TypeCode.Int16 => (T)(object)(short)value, 
			TypeCode.UInt16 => (T)(object)(ushort)value, 
			TypeCode.SByte => (T)(object)(sbyte)value, 
			_ => (T)(object)(byte)value, 
		};
	}

	private string FormatEnumAsString(ulong key, T value, JsonNamingPolicy dictionaryKeyPolicy)
	{
		EnumFieldInfo[] enumFieldInfo;
		if (s_isFlagsEnum)
		{
			Span<char> initialBuffer = stackalloc char[128];
			using System.Text.ValueStringBuilder valueStringBuilder = new System.Text.ValueStringBuilder(initialBuffer);
			ulong num = key;
			enumFieldInfo = _enumFieldInfo;
			foreach (EnumFieldInfo enumFieldInfo2 in enumFieldInfo)
			{
				ulong key2 = enumFieldInfo2.Key;
				if ((key2 == 0L) ? (key == 0) : ((num & key2) == key2))
				{
					num &= ~key2;
					string s = ((dictionaryKeyPolicy != null) ? ResolveAndValidateJsonName(enumFieldInfo2.OriginalName, dictionaryKeyPolicy, enumFieldInfo2.Kind) : enumFieldInfo2.JsonName);
					if (valueStringBuilder.Length > 0)
					{
						valueStringBuilder.Append(", ");
					}
					valueStringBuilder.Append(s);
					if (num == 0L)
					{
						break;
					}
				}
			}
			return valueStringBuilder.ToString();
		}
		enumFieldInfo = _enumFieldInfo;
		foreach (EnumFieldInfo enumFieldInfo3 in enumFieldInfo)
		{
			if (enumFieldInfo3.Key == key)
			{
				return ResolveAndValidateJsonName(enumFieldInfo3.OriginalName, dictionaryKeyPolicy, enumFieldInfo3.Kind);
			}
		}
		return null;
	}

	private bool IsDefinedValueOrCombinationOfValues(ulong key)
	{
		EnumFieldInfo[] enumFieldInfo;
		if (s_isFlagsEnum)
		{
			ulong num = key;
			enumFieldInfo = _enumFieldInfo;
			for (int i = 0; i < enumFieldInfo.Length; i++)
			{
				ulong key2 = enumFieldInfo[i].Key;
				if ((key2 == 0L) ? (key == 0) : ((num & key2) == key2))
				{
					num &= ~key2;
					if (num == 0L)
					{
						return true;
					}
				}
			}
			return false;
		}
		enumFieldInfo = _enumFieldInfo;
		for (int i = 0; i < enumFieldInfo.Length; i++)
		{
			if (enumFieldInfo[i].Key == key)
			{
				return true;
			}
		}
		return false;
	}

	internal override JsonSchema GetSchema(JsonNumberHandling numberHandling)
	{
		if ((_converterOptions & EnumConverterOptions.AllowStrings) != 0)
		{
			if (s_isFlagsEnum)
			{
				return new JsonSchema
				{
					Type = JsonSchemaType.String
				};
			}
			JsonArray jsonArray = new JsonArray();
			EnumFieldInfo[] enumFieldInfo = _enumFieldInfo;
			foreach (EnumFieldInfo enumFieldInfo2 in enumFieldInfo)
			{
				jsonArray.Add((JsonNode?)enumFieldInfo2.JsonName);
			}
			return new JsonSchema
			{
				Enum = jsonArray
			};
		}
		return new JsonSchema
		{
			Type = JsonSchemaType.Integer
		};
	}

	private static EnumFieldInfo[] ResolveEnumFields(JsonNamingPolicy namingPolicy)
	{
		string[] names = Enum.GetNames<T>();
		T[] values = Enum.GetValues<T>();
		Dictionary<string, string> dictionary = null;
		foreach (FieldInfo field in GetFields())
		{
			JsonStringEnumMemberNameAttribute customAttribute = field.GetCustomAttribute<JsonStringEnumMemberNameAttribute>();
			if (customAttribute != null)
			{
				(dictionary ?? (dictionary = new Dictionary<string, string>(StringComparer.Ordinal))).Add(field.Name, customAttribute.Name);
			}
		}
		EnumFieldInfo[] array = new EnumFieldInfo[names.Length];
		for (int i = 0; i < names.Length; i++)
		{
			string text = names[i];
			ulong key = ConvertToUInt64(values[i]);
			EnumFieldNameKind kind;
			if (dictionary != null && dictionary.TryGetValue(text, out var value))
			{
				text = value;
				kind = EnumFieldNameKind.Attribute;
			}
			else
			{
				kind = ((namingPolicy != null) ? EnumFieldNameKind.NamingPolicy : EnumFieldNameKind.Default);
			}
			string jsonName = ResolveAndValidateJsonName(text, namingPolicy, kind);
			array[i] = new EnumFieldInfo(key, kind, text, jsonName);
		}
		if (s_isFlagsEnum)
		{
			array = TopologicalSortEnumFields(array);
		}
		return array;
		static IEnumerable<FieldInfo> GetFields()
		{
			return typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public);
		}
	}

	private static string ResolveAndValidateJsonName(string name, JsonNamingPolicy namingPolicy, EnumFieldNameKind kind)
	{
		if (kind != EnumFieldNameKind.Attribute && namingPolicy != null)
		{
			name = namingPolicy.ConvertName(name);
		}
		if (string.IsNullOrEmpty(name) || char.IsWhiteSpace(name[0]) || char.IsWhiteSpace(name[name.Length - 1]) || (s_isFlagsEnum && name.AsSpan().IndexOf(',') >= 0))
		{
			ThrowHelper.ThrowInvalidOperationException_UnsupportedEnumIdentifier(typeof(T), name);
		}
		return name;
	}

	private static EnumFieldInfo[] TopologicalSortEnumFields(EnumFieldInfo[] enumFields)
	{
		if (enumFields.Length <= 1)
		{
			return enumFields;
		}
		(int, int)[] array = new(int, int)[enumFields.Length];
		for (int i = 0; i < enumFields.Length; i++)
		{
			array[i] = (-PopCount(enumFields[i].Key), i);
		}
		Array.Sort(array);
		EnumFieldInfo[] array2 = new EnumFieldInfo[enumFields.Length];
		for (int j = 0; j < array.Length; j++)
		{
			int item = array[j].Item2;
			array2[j] = enumFields[item];
		}
		return array2;
	}

	private static int PopCount(ulong value)
	{
		return (int)ulong.PopCount(value);
	}
}
