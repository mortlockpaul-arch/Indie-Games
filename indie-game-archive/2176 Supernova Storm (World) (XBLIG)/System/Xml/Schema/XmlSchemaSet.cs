using System.Collections;

namespace System.Xml.Schema;

public class XmlSchemaSet
{
	public XmlSchemaCompilationSettings CompilationSettings
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
		set
		{
		}
	}

	public int Count
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public XmlSchemaObjectTable GlobalAttributes
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public XmlSchemaObjectTable GlobalElements
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public XmlSchemaObjectTable GlobalTypes
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public bool IsCompiled
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

	public XmlSchemaSet()
	{
	}

	public XmlSchemaSet(XmlNameTable nameTable)
	{
	}

	public XmlSchema Add(string targetNamespace, string schemaUri)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public XmlSchema Add(string targetNamespace, XmlReader schemaDocument)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public void Add(XmlSchemaSet schemas)
	{
	}

	public XmlSchema Add(XmlSchema schema)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public XmlSchema Remove(XmlSchema schema)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public bool RemoveRecursive(XmlSchema schemaToRemove)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public bool Contains(string targetNamespace)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public bool Contains(XmlSchema schema)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public void Compile()
	{
	}

	public XmlSchema Reprocess(XmlSchema schema)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public void CopyTo(XmlSchema[] schemas, int index)
	{
	}

	public ICollection Schemas()
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public ICollection Schemas(string targetNamespace)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}
}
