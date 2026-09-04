using System.Collections;

namespace System.Xml.Schema;

public sealed class XmlSchemaValidator
{
	public IXmlLineInfo LineInfoProvider
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
		set
		{
		}
	}

	public Uri SourceUri
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
		set
		{
		}
	}

	public object ValidationEventSender
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
		set
		{
		}
	}

	public XmlResolver XmlResolver
	{
		set
		{
		}
	}

	public event ValidationEventHandler ValidationEventHandler
	{
		add
		{
		}
		remove
		{
		}
	}

	public XmlSchemaValidator(XmlNameTable nameTable, XmlSchemaSet schemas, IXmlNamespaceResolver namespaceResolver, XmlSchemaValidationFlags validationFlags)
	{
	}

	public void AddSchema(XmlSchema schema)
	{
	}

	public void Initialize()
	{
	}

	public void Initialize(XmlSchemaObject partialValidationType)
	{
	}

	public void ValidateElement(string localName, string namespaceUri, XmlSchemaInfo schemaInfo)
	{
	}

	public void ValidateElement(string localName, string namespaceUri, XmlSchemaInfo schemaInfo, string xsiType, string xsiNil, string xsiSchemaLocation, string xsiNoNamespaceSchemaLocation)
	{
	}

	public object ValidateAttribute(string localName, string namespaceUri, string attributeValue, XmlSchemaInfo schemaInfo)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public object ValidateAttribute(string localName, string namespaceUri, XmlValueGetter attributeValue, XmlSchemaInfo schemaInfo)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public void GetUnspecifiedDefaultAttributes(ArrayList defaultAttributes)
	{
	}

	public void ValidateEndOfAttributes(XmlSchemaInfo schemaInfo)
	{
	}

	public void ValidateText(string elementValue)
	{
	}

	public void ValidateText(XmlValueGetter elementValue)
	{
	}

	public void ValidateWhitespace(string elementValue)
	{
	}

	public void ValidateWhitespace(XmlValueGetter elementValue)
	{
	}

	public object ValidateEndElement(XmlSchemaInfo schemaInfo)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public object ValidateEndElement(XmlSchemaInfo schemaInfo, object typedValue)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public void SkipToEndElement(XmlSchemaInfo schemaInfo)
	{
	}

	public void EndValidation()
	{
	}

	public XmlSchemaParticle[] GetExpectedParticles()
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public XmlSchemaAttribute[] GetExpectedAttributes()
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}
}
