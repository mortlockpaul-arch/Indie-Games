using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Converters;
using System.Threading;

namespace System.Text.Json.Nodes;

[DebuggerDisplay("JsonObject[{Count}]")]
[DebuggerTypeProxy(typeof(DebugView))]
public sealed class JsonObject : JsonNode, IDictionary<string, JsonNode?>, ICollection<KeyValuePair<string, JsonNode?>>, IEnumerable<KeyValuePair<string, JsonNode?>>, IEnumerable, IList<KeyValuePair<string, JsonNode?>>
{
	[ExcludeFromCodeCoverage]
	private sealed class DebugView
	{
		[DebuggerDisplay("{Display,nq}")]
		private struct DebugViewProperty
		{
			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public JsonNode Value;

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public string PropertyName;

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public string Display
			{
				get
				{
					if (Value == null)
					{
						return PropertyName + " = null";
					}
					if (Value is JsonValue)
					{
						return PropertyName + " = " + Value.ToJsonString();
					}
					if (Value is JsonObject jsonObject)
					{
						return $"{PropertyName} = JsonObject[{jsonObject.Count}]";
					}
					JsonArray jsonArray = (JsonArray)Value;
					return $"{PropertyName} = JsonArray[{jsonArray.Count}]";
				}
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly JsonObject _node;

		public string Json => _node.ToJsonString();

		public string Path => _node.GetPath();

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		private DebugViewProperty[] Items
		{
			get
			{
				DebugViewProperty[] array = new DebugViewProperty[_node.Count];
				int num = 0;
				foreach (KeyValuePair<string, JsonNode> item in _node)
				{
					array[num].PropertyName = item.Key;
					array[num].Value = item.Value;
					num++;
				}
				return array;
			}
		}

		public DebugView(JsonObject node)
		{
			_node = node;
		}
	}

	private JsonElement? _jsonElement;

	private OrderedDictionary<string, JsonNode> _dictionary;

	internal override JsonElement? UnderlyingElement => _jsonElement;

	private OrderedDictionary<string, JsonNode?> Dictionary => _dictionary ?? InitializeDictionary();

	public int Count => Dictionary.Count;

	ICollection<string> IDictionary<string, JsonNode?>.Keys => Dictionary.Keys;

	ICollection<JsonNode?> IDictionary<string, JsonNode?>.Values => Dictionary.Values;

	bool ICollection<KeyValuePair<string, JsonNode?>>.IsReadOnly => false;

	KeyValuePair<string, JsonNode?> IList<KeyValuePair<string, JsonNode?>>.this[int index]
	{
		get
		{
			return GetAt(index);
		}
		set
		{
			SetAt(index, value.Key, value.Value);
		}
	}

	public JsonObject(JsonNodeOptions? options = null)
		: base(options)
	{
	}

	public JsonObject(IEnumerable<KeyValuePair<string, JsonNode?>> properties, JsonNodeOptions? options = null)
		: this(options)
	{
		OrderedDictionary<string, JsonNode> orderedDictionary = CreateDictionary(options, (properties is ICollection<KeyValuePair<string, JsonNode>> collection) ? collection.Count : 0);
		foreach (KeyValuePair<string, JsonNode> property in properties)
		{
			orderedDictionary.Add(property.Key, property.Value);
			property.Value?.AssignParent(this);
		}
		_dictionary = orderedDictionary;
	}

	public static JsonObject? Create(JsonElement element, JsonNodeOptions? options = null)
	{
		return element.ValueKind switch
		{
			JsonValueKind.Null => null, 
			JsonValueKind.Object => new JsonObject(element, options), 
			_ => throw new InvalidOperationException(System.SR.Format(System.SR.NodeElementWrongType, "Object")), 
		};
	}

	internal JsonObject(JsonElement element, JsonNodeOptions? options = null)
		: this(options)
	{
		_jsonElement = element;
	}

	private protected override JsonNode GetItem(int index)
	{
		return GetAt(index).Value;
	}

	private protected override void SetItem(int index, JsonNode value)
	{
		SetAt(index, value);
	}

	internal override JsonNode DeepCloneCore()
	{
		GetUnderlyingRepresentation(out var dictionary, out var jsonElement);
		if (dictionary == null)
		{
			if (!jsonElement.HasValue)
			{
				return new JsonObject(base.Options);
			}
			return new JsonObject(jsonElement.Value.Clone(), base.Options);
		}
		JsonObject jsonObject = new JsonObject(base.Options)
		{
			_dictionary = CreateDictionary(base.Options, Count)
		};
		foreach (KeyValuePair<string, JsonNode> item in dictionary)
		{
			jsonObject.Add(item.Key, item.Value?.DeepCloneCore());
		}
		return jsonObject;
	}

	internal string GetPropertyName(JsonNode node)
	{
		KeyValuePair<string, JsonNode>? keyValuePair = FindValue(node);
		if (!keyValuePair.HasValue)
		{
			return string.Empty;
		}
		return keyValuePair.Value.Key;
	}

	public bool TryGetPropertyValue(string propertyName, out JsonNode? jsonNode)
	{
		int index;
		return TryGetPropertyValue(propertyName, out jsonNode, out index);
	}

	public bool TryGetPropertyValue(string propertyName, out JsonNode? jsonNode, out int index)
	{
		ArgumentNullException.ThrowIfNull(propertyName, "propertyName");
		return Dictionary.TryGetValue(propertyName, out jsonNode, out index);
	}

	public override void WriteTo(Utf8JsonWriter writer, JsonSerializerOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(writer, "writer");
		GetUnderlyingRepresentation(out var dictionary, out var jsonElement);
		if (dictionary == null && jsonElement.HasValue)
		{
			jsonElement.Value.WriteTo(writer);
			return;
		}
		writer.WriteStartObject();
		foreach (KeyValuePair<string, JsonNode> item in Dictionary)
		{
			writer.WritePropertyName(item.Key);
			if (item.Value == null)
			{
				writer.WriteNullValue();
			}
			else
			{
				item.Value.WriteTo(writer, options);
			}
		}
		writer.WriteEndObject();
	}

	private protected override JsonValueKind GetValueKindCore()
	{
		return JsonValueKind.Object;
	}

	internal override bool DeepEqualsCore(JsonNode node)
	{
		if (!(node is JsonArray))
		{
			if (!(node is JsonValue jsonValue))
			{
				if (node is JsonObject jsonObject)
				{
					OrderedDictionary<string, JsonNode> dictionary = Dictionary;
					OrderedDictionary<string, JsonNode> dictionary2 = jsonObject.Dictionary;
					if (dictionary.Count != dictionary2.Count)
					{
						return false;
					}
					foreach (KeyValuePair<string, JsonNode> item in dictionary)
					{
						if (!dictionary2.TryGetValue(item.Key, out var value) || !JsonNode.DeepEquals(item.Value, value))
						{
							return false;
						}
					}
					return true;
				}
				return false;
			}
			return jsonValue.DeepEqualsCore(this);
		}
		return false;
	}

	internal JsonNode GetItem(string propertyName)
	{
		ArgumentNullException.ThrowIfNull(propertyName, "propertyName");
		if (TryGetPropertyValue(propertyName, out JsonNode jsonNode))
		{
			return jsonNode;
		}
		return null;
	}

	internal override void GetPath(ref System.Text.ValueStringBuilder path, JsonNode child)
	{
		base.Parent?.GetPath(ref path, this);
		if (child != null)
		{
			string key = FindValue(child).Value.Key;
			if (key.AsSpan().ContainsSpecialCharacters())
			{
				path.Append("['");
				path.Append(key);
				path.Append("']");
			}
			else
			{
				path.Append('.');
				path.Append(key);
			}
		}
	}

	internal void SetItem(string propertyName, JsonNode value)
	{
		ArgumentNullException.ThrowIfNull(propertyName, "propertyName");
		OrderedDictionary<string, JsonNode> dictionary = Dictionary;
		if (!dictionary.TryAdd(propertyName, value, out var index))
		{
			JsonNode value2 = dictionary.GetAt(index).Value;
			if (value == value2)
			{
				return;
			}
			DetachParent(value2);
			dictionary.SetAt(index, value);
		}
		value?.AssignParent(this);
	}

	private void DetachParent(JsonNode item)
	{
		if (item != null)
		{
			item.Parent = null;
		}
	}

	private KeyValuePair<string, JsonNode>? FindValue(JsonNode value)
	{
		foreach (KeyValuePair<string, JsonNode> item in Dictionary)
		{
			if (item.Value == value)
			{
				return item;
			}
		}
		return null;
	}

	public void Add(string propertyName, JsonNode? value)
	{
		ArgumentNullException.ThrowIfNull(propertyName, "propertyName");
		Dictionary.Add(propertyName, value);
		value?.AssignParent(this);
	}

	public bool TryAdd(string propertyName, JsonNode? value)
	{
		int index;
		return TryAdd(propertyName, value, out index);
	}

	public bool TryAdd(string propertyName, JsonNode? value, out int index)
	{
		if (propertyName == null)
		{
			ThrowHelper.ThrowArgumentNullException("propertyName");
		}
		bool num = Dictionary.TryAdd(propertyName, value, out index);
		if (num)
		{
			value?.AssignParent(this);
		}
		return num;
	}

	public void Add(KeyValuePair<string, JsonNode?> property)
	{
		Add(property.Key, property.Value);
	}

	public void Clear()
	{
		OrderedDictionary<string, JsonNode> dictionary = _dictionary;
		if (dictionary == null)
		{
			_jsonElement = null;
			return;
		}
		foreach (JsonNode value in dictionary.Values)
		{
			DetachParent(value);
		}
		dictionary.Clear();
	}

	public bool ContainsKey(string propertyName)
	{
		ArgumentNullException.ThrowIfNull(propertyName, "propertyName");
		return Dictionary.ContainsKey(propertyName);
	}

	public bool Remove(string propertyName)
	{
		ArgumentNullException.ThrowIfNull(propertyName, "propertyName");
		bool num = Dictionary.Remove(propertyName, out JsonNode value);
		if (num)
		{
			DetachParent(value);
		}
		return num;
	}

	bool ICollection<KeyValuePair<string, JsonNode?>>.Contains(KeyValuePair<string, JsonNode> item)
	{
		return ((ICollection<KeyValuePair<string, JsonNode>>)Dictionary).Contains(item);
	}

	void ICollection<KeyValuePair<string, JsonNode?>>.CopyTo(KeyValuePair<string, JsonNode>[] array, int index)
	{
		((ICollection<KeyValuePair<string, JsonNode>>)Dictionary).CopyTo(array, index);
	}

	public IEnumerator<KeyValuePair<string, JsonNode?>> GetEnumerator()
	{
		return Dictionary.GetEnumerator();
	}

	bool ICollection<KeyValuePair<string, JsonNode?>>.Remove(KeyValuePair<string, JsonNode> item)
	{
		return Remove(item.Key);
	}

	bool IDictionary<string, JsonNode?>.TryGetValue(string propertyName, out JsonNode jsonNode)
	{
		ArgumentNullException.ThrowIfNull(propertyName, "propertyName");
		return Dictionary.TryGetValue(propertyName, out jsonNode);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return Dictionary.GetEnumerator();
	}

	private OrderedDictionary<string, JsonNode> InitializeDictionary()
	{
		GetUnderlyingRepresentation(out var dictionary, out var jsonElement);
		if (dictionary == null)
		{
			dictionary = CreateDictionary(base.Options);
			if (jsonElement.HasValue)
			{
				foreach (JsonProperty item in jsonElement.Value.EnumerateObject())
				{
					JsonNode jsonNode = JsonNodeConverter.Create(item.Value, base.Options);
					if (jsonNode != null)
					{
						jsonNode.Parent = this;
					}
					dictionary.Add(item.Name, jsonNode);
				}
			}
			_dictionary = dictionary;
			Interlocked.MemoryBarrier();
			_jsonElement = null;
		}
		return dictionary;
	}

	private static OrderedDictionary<string, JsonNode> CreateDictionary(JsonNodeOptions? options, int capacity = 0)
	{
		StringComparer comparer = ((options.HasValue && options.GetValueOrDefault().PropertyNameCaseInsensitive) ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
		return new OrderedDictionary<string, JsonNode>(capacity, comparer);
	}

	private void GetUnderlyingRepresentation(out OrderedDictionary<string, JsonNode> dictionary, out JsonElement? jsonElement)
	{
		jsonElement = _jsonElement;
		Interlocked.MemoryBarrier();
		dictionary = _dictionary;
	}

	public KeyValuePair<string, JsonNode?> GetAt(int index)
	{
		return Dictionary.GetAt(index);
	}

	public void SetAt(int index, string propertyName, JsonNode? value)
	{
		ArgumentNullException.ThrowIfNull(propertyName, "propertyName");
		OrderedDictionary<string, JsonNode?> dictionary = Dictionary;
		KeyValuePair<string, JsonNode> at = dictionary.GetAt(index);
		dictionary.SetAt(index, propertyName, value);
		DetachParent(at.Value);
		value?.AssignParent(this);
	}

	public void SetAt(int index, JsonNode? value)
	{
		OrderedDictionary<string, JsonNode?> dictionary = Dictionary;
		KeyValuePair<string, JsonNode> at = dictionary.GetAt(index);
		dictionary.SetAt(index, value);
		DetachParent(at.Value);
		value?.AssignParent(this);
	}

	public int IndexOf(string propertyName)
	{
		ArgumentNullException.ThrowIfNull(propertyName, "propertyName");
		return Dictionary.IndexOf(propertyName);
	}

	public void Insert(int index, string propertyName, JsonNode? value)
	{
		ArgumentNullException.ThrowIfNull(propertyName, "propertyName");
		Dictionary.Insert(index, propertyName, value);
		value?.AssignParent(this);
	}

	public void RemoveAt(int index)
	{
		KeyValuePair<string, JsonNode> at = Dictionary.GetAt(index);
		Dictionary.RemoveAt(index);
		DetachParent(at.Value);
	}

	int IList<KeyValuePair<string, JsonNode?>>.IndexOf(KeyValuePair<string, JsonNode> item)
	{
		return ((IList<KeyValuePair<string, JsonNode>>)Dictionary).IndexOf(item);
	}

	void IList<KeyValuePair<string, JsonNode?>>.Insert(int index, KeyValuePair<string, JsonNode> item)
	{
		Insert(index, item.Key, item.Value);
	}

	void IList<KeyValuePair<string, JsonNode?>>.RemoveAt(int index)
	{
		RemoveAt(index);
	}
}
