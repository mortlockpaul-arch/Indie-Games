using System.IO;

namespace System.Xml.Serialization;

public class XmlSerializer
{
	public event XmlAttributeEventHandler UnknownAttribute
	{
		add
		{
		}
		remove
		{
		}
	}

	public event XmlElementEventHandler UnknownElement
	{
		add
		{
		}
		remove
		{
		}
	}

	public event XmlNodeEventHandler UnknownNode
	{
		add
		{
		}
		remove
		{
		}
	}

	public event UnreferencedObjectEventHandler UnreferencedObject
	{
		add
		{
		}
		remove
		{
		}
	}

	protected XmlSerializer()
	{
	}

	public XmlSerializer(Type type)
	{
	}

	public XmlSerializer(Type type, string defaultNamespace)
	{
	}

	public XmlSerializer(Type type, XmlAttributeOverrides overrides)
	{
	}

	public XmlSerializer(Type type, XmlRootAttribute root)
	{
	}

	public XmlSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace)
	{
	}

	public XmlSerializer(XmlTypeMapping xmlMapping)
	{
	}

	public XmlSerializer(Type type, Type[] extraTypes)
	{
	}

	public static XmlSerializer[] FromTypes(Type[] types)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public virtual bool CanDeserialize(XmlReader reader)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public void Serialize(TextWriter textWriter, object o)
	{
	}

	public void Serialize(TextWriter textWriter, object o, XmlSerializerNamespaces namespaces)
	{
	}

	public void Serialize(Stream stream, object o)
	{
	}

	public void Serialize(Stream stream, object o, XmlSerializerNamespaces namespaces)
	{
	}

	public void Serialize(XmlWriter xmlWriter, object o)
	{
	}

	public void Serialize(XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces)
	{
	}

	public void Serialize(XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces, string encodingStyle)
	{
	}

	public object Deserialize(Stream stream)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public object Deserialize(TextReader textReader)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public object Deserialize(XmlReader xmlReader)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public object Deserialize(XmlReader xmlReader, string encodingStyle)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}
}
