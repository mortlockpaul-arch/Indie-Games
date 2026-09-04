using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace XnaLibrary;

public class SettingsSerializer
{
	public static void Save<T>(string filename, T obj)
	{
		using FileStream fileStream = File.Open(filename, FileMode.Create);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
		xmlSerializer.Serialize(fileStream, obj);
		fileStream.Flush();
	}

	public static T Load<T>(string filename)
	{
		T result = default(T);
		if (!File.Exists(filename))
		{
			return result;
		}
		using FileStream stream = File.Open(filename, FileMode.OpenOrCreate);
		StreamReader streamReader = new StreamReader(stream);
		string xml = streamReader.ReadToEnd();
		return deserialize<T>(xml);
	}

	public static T Load<T>(string filename, string xpath)
	{
		T result = default(T);
		if (!File.Exists(filename))
		{
			return result;
		}
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.Load(filename);
		XmlNode xmlNode = xmlDocument.SelectSingleNode(xpath);
		if (xmlNode == null)
		{
			return result;
		}
		return deserialize<T>(xmlNode.OuterXml);
	}

	private static T deserialize<T>(string xml)
	{
		T val = default(T);
		using StringReader textReader = new StringReader(xml);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
		return (T)xmlSerializer.Deserialize(textReader);
	}
}
