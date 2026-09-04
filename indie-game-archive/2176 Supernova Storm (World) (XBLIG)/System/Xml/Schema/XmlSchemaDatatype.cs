namespace System.Xml.Schema;

public abstract class XmlSchemaDatatype
{
	public abstract XmlTokenizedType TokenizedType { get; }

	public virtual XmlTypeCode TypeCode
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public abstract Type ValueType { get; }

	public virtual XmlSchemaDatatypeVariety Variety
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public abstract object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr);

	public virtual object ChangeType(object value, Type targetType)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public virtual object ChangeType(object value, Type targetType, IXmlNamespaceResolver namespaceResolver)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public virtual bool IsDerivedFrom(XmlSchemaDatatype datatype)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}
}
