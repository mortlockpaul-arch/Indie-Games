using System.IO;
using System.Xml.Schema;

namespace System.Xml;

public class XmlDocument : XmlNode
{
	public override string BaseURI
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public XmlElement DocumentElement
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public XmlImplementation Implementation
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public override string InnerXml
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
		set
		{
		}
	}

	public override bool IsReadOnly
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public override string LocalName
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public override string Name
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public XmlNameTable NameTable
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public override XmlNodeType NodeType
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public override XmlDocument OwnerDocument
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public override XmlNode ParentNode
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public bool PreserveWhitespace
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
		set
		{
		}
	}

	public override IXmlSchemaInfo SchemaInfo
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public XmlSchemaSet Schemas
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
		set
		{
		}
	}

	public virtual XmlResolver XmlResolver
	{
		set
		{
		}
	}

	public event XmlNodeChangedEventHandler NodeChanged
	{
		add
		{
		}
		remove
		{
		}
	}

	public event XmlNodeChangedEventHandler NodeChanging
	{
		add
		{
		}
		remove
		{
		}
	}

	public event XmlNodeChangedEventHandler NodeInserted
	{
		add
		{
		}
		remove
		{
		}
	}

	public event XmlNodeChangedEventHandler NodeInserting
	{
		add
		{
		}
		remove
		{
		}
	}

	public event XmlNodeChangedEventHandler NodeRemoved
	{
		add
		{
		}
		remove
		{
		}
	}

	public event XmlNodeChangedEventHandler NodeRemoving
	{
		add
		{
		}
		remove
		{
		}
	}

	public XmlDocument()
	{
	}

	public XmlDocument(XmlNameTable nt)
	{
	}

	protected internal XmlDocument(XmlImplementation imp)
	{
	}

	public override XmlNode CloneNode(bool deep)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public XmlAttribute CreateAttribute(string name)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public virtual XmlCDataSection CreateCDataSection(string data)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public virtual XmlComment CreateComment(string data)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public virtual XmlDocumentFragment CreateDocumentFragment()
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public XmlElement CreateElement(string name)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public virtual XmlEntityReference CreateEntityReference(string name)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public virtual XmlProcessingInstruction CreateProcessingInstruction(string target, string data)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public virtual XmlDeclaration CreateXmlDeclaration(string version, string encoding, string standalone)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public virtual XmlText CreateTextNode(string text)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public virtual XmlSignificantWhitespace CreateSignificantWhitespace(string text)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public virtual XmlWhitespace CreateWhitespace(string text)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public virtual XmlNodeList GetElementsByTagName(string name)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public XmlAttribute CreateAttribute(string qualifiedName, string namespaceURI)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public XmlElement CreateElement(string qualifiedName, string namespaceURI)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public virtual XmlNodeList GetElementsByTagName(string localName, string namespaceURI)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public virtual XmlNode ImportNode(XmlNode node, bool deep)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public virtual XmlAttribute CreateAttribute(string prefix, string localName, string namespaceURI)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	protected internal virtual XmlAttribute CreateDefaultAttribute(string prefix, string localName, string namespaceURI)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public virtual XmlElement CreateElement(string prefix, string localName, string namespaceURI)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public virtual XmlNode CreateNode(XmlNodeType type, string prefix, string name, string namespaceURI)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public virtual XmlNode CreateNode(string nodeTypeString, string name, string namespaceURI)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public virtual XmlNode CreateNode(XmlNodeType type, string name, string namespaceURI)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public virtual XmlNode ReadNode(XmlReader reader)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public virtual void Load(string filename)
	{
	}

	public virtual void Load(Stream inStream)
	{
	}

	public virtual void Load(TextReader txtReader)
	{
	}

	public virtual void Load(XmlReader reader)
	{
	}

	public virtual void LoadXml(string xml)
	{
	}

	public virtual void Save(string filename)
	{
	}

	public virtual void Save(Stream outStream)
	{
	}

	public virtual void Save(TextWriter writer)
	{
	}

	public virtual void Save(XmlWriter w)
	{
	}

	public override void WriteTo(XmlWriter w)
	{
	}

	public override void WriteContentTo(XmlWriter xw)
	{
	}

	public void Validate(ValidationEventHandler validationEventHandler)
	{
	}

	public void Validate(ValidationEventHandler validationEventHandler, XmlNode nodeToValidate)
	{
	}
}
