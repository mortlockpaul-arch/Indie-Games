using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using _0001;
using R;

namespace SynapseGaming.LightingSystem.Serialization;

/// <summary>
/// Provides helper methods for serializing objects and hierarchies of objects to xml or file.
/// </summary>
public class SerializationHelper
{
	internal const string HCB = "root";

	internal const string HC_0002 = "List";

	internal const string HC_0012 = "classes";

	internal const string HCH = "fullname";

	internal const string HC7 = "assembly";

	internal const string HC_0001 = "fullyqualifiedname";

	private static R.B HCw = new R.B();

	/// <summary>
	/// Serializes a field or enumeration. The type must include the [Serialize] attribute and
	/// if a custom class should ideally implement the IFullSerializable interface.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="field">Field or enumeration to serialize.</param>
	/// <param name="info">SerializationInfo object used to store the serialized name and data.</param>
	/// <param name="name">Name stored with the serialized data.</param>
	public static void SerializeFieldOrEnum<T>(ref T field, SerializationInfo info, string name)
	{
		try
		{
			info.AddValue(name, field);
		}
		catch
		{
		}
	}

	/// <summary>
	/// Deserializes a field.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="field">Variable used to store the deserialized data.</param>
	/// <param name="info">SerializationInfo object containing the serialized data.</param>
	/// <param name="name">Name used to retrieve the serialized data.</param>
	/// <param name="usedefault">Determines if a default value should be applied to the field when data cannot be deserialized.</param>
	public static void DeserializeField<T>(ref T field, SerializationInfo info, string name, bool usedefault)
	{
		try
		{
			field = (T)info.GetValue(name, typeof(T));
		}
		catch
		{
			if (usedefault)
			{
				field = default(T);
			}
		}
	}

	/// <summary>
	/// Deserializes a field.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="info">SerializationInfo object containing the serialized data.</param>
	/// <param name="name">Name used to retrieve the serialized data.</param>
	public static T DeserializeField<T>(SerializationInfo info, string name)
	{
		try
		{
			return (T)info.GetValue(name, typeof(T));
		}
		catch
		{
		}
		return default(T);
	}

	/// <summary>
	/// Deserializes an enumeration.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="field">Variable used to store the deserialized data.</param>
	/// <param name="info">SerializationInfo object containing the serialized data.</param>
	/// <param name="name">Name used to retrieve the serialized data.</param>
	/// <param name="isflag">Determines if the enumeration type is a flag and can contain more than one value.</param>
	public static void DeserializeEnum<T>(ref T field, SerializationInfo info, string name, bool isflag)
	{
		try
		{
			string value = (string)info.GetValue(name, typeof(string));
			field = (T)Enum.Parse(typeof(T), value, ignoreCase: false);
		}
		catch
		{
			if (!isflag)
			{
				field = default(T);
			}
		}
	}

	/// <summary>
	/// Deserializes an enumeration.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="info">SerializationInfo object containing the serialized data.</param>
	/// <param name="name">Name used to retrieve the serialized data.</param>
	public static T DeserializeEnum<T>(SerializationInfo info, string name)
	{
		try
		{
			string value = (string)info.GetValue(name, typeof(string));
			return (T)Enum.Parse(typeof(T), value, ignoreCase: false);
		}
		catch
		{
		}
		return default(T);
	}

	/// <summary>
	/// Deserializes an object from xml.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="xml"></param>
	/// <returns></returns>
	public static T LoadFromXml<T>(string xml)
	{
		using MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
		global::_0001._0001 obj = new global::_0001._0001();
		obj.Load(stream);
		return (T)HCw._7F(obj);
	}
}
