using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using _7;

namespace System.Runtime.Serialization;

/// <summary>
/// Stores all the data needed to serialize or deserialize an object. This class
/// cannot be inherited.
/// </summary>
public class SerializationInfo : IEnumerable
{
	private CultureInfo HCB = new CultureInfo("en-US");

	private Dictionary<string, SerializationEntry> HC_0002 = new Dictionary<string, SerializationEntry>(16);

	/// <summary>
	/// Creates a new instance of the System.Runtime.Serialization.SerializationInfo
	/// class.
	/// </summary>
	/// <param name="unused1"></param>
	/// <param name="unused2"></param>
	public SerializationInfo(Type unused1, FormatterConverter unused2)
	{
	}

	/// <summary>
	/// Returns a System.Runtime.Serialization.SerializationInfoEnumerator used to
	/// iterate through the name-value pairs in the
	/// System.Runtime.Serialization.SerializationInfo store.
	/// </summary>
	/// <returns></returns>
	public IEnumerator GetEnumerator()
	{
		return new _7.B(HC_0002.GetEnumerator());
	}

	/// <summary>
	/// Adds the specified object into the System.Runtime.Serialization.SerializationInfo
	/// store, where it is associated with a specified name.
	/// </summary>
	/// <param name="name">The name to associate with the value, so it can be deserialized later.</param>
	/// <param name="value">The value to be serialized. Any children of this object will automatically
	/// be serialized.</param>
	public void AddValue(string name, object value)
	{
		HC_0002.Add(name, new SerializationEntry(name, value));
	}

	/// <summary>
	/// Retrieves a value from the System.Runtime.Serialization.SerializationInfo store.
	/// </summary>
	/// <param name="name">The name associated with the value to retrieve.</param>
	/// <param name="datatype">The System.Type of the value to retrieve. If the stored value cannot be converted
	/// to this type, the system will throw a System.InvalidCastException.</param>
	/// <returns></returns>
	public object GetValue(string name, Type datatype)
	{
		SerializationEntry serializationEntry = HC_0002[name];
		object value = serializationEntry.Value;
		if (!(value is string text))
		{
			return value;
		}
		if ((object)datatype == typeof(string))
		{
			return text;
		}
		if ((object)datatype == typeof(int))
		{
			return int.Parse(text, HCB);
		}
		if ((object)datatype == typeof(float))
		{
			return float.Parse(text, HCB);
		}
		if ((object)datatype == typeof(bool))
		{
			return bool.Parse(text);
		}
		throw new Exception("Unknown data conversion.");
	}
}
