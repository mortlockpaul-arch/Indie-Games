using System.IO;
using System.Text;

namespace System.Xml;

public class XmlTextWriter : XmlWriter
{
	public Stream BaseStream
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public Formatting Formatting
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
		set
		{
		}
	}

	public char IndentChar
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
		set
		{
		}
	}

	public int Indentation
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
		set
		{
		}
	}

	public bool Namespaces
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
		set
		{
		}
	}

	public char QuoteChar
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
		set
		{
		}
	}

	public override WriteState WriteState
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public override string XmlLang
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public override XmlSpace XmlSpace
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public XmlTextWriter(Stream w, Encoding encoding)
	{
	}

	public XmlTextWriter(string filename, Encoding encoding)
	{
	}

	public XmlTextWriter(TextWriter w)
	{
	}

	public override void WriteStartDocument()
	{
	}

	public override void WriteStartDocument(bool standalone)
	{
	}

	public override void WriteEndDocument()
	{
	}

	public override void WriteDocType(string name, string pubid, string sysid, string subset)
	{
	}

	public override void WriteStartElement(string prefix, string localName, string ns)
	{
	}

	public override void WriteEndElement()
	{
	}

	public override void WriteFullEndElement()
	{
	}

	public override void WriteStartAttribute(string prefix, string localName, string ns)
	{
	}

	public override void WriteEndAttribute()
	{
	}

	public override void WriteCData(string text)
	{
	}

	public override void WriteComment(string text)
	{
	}

	public override void WriteProcessingInstruction(string name, string text)
	{
	}

	public override void WriteEntityRef(string name)
	{
	}

	public override void WriteCharEntity(char ch)
	{
	}

	public override void WriteWhitespace(string ws)
	{
	}

	public override void WriteString(string text)
	{
	}

	public override void WriteSurrogateCharEntity(char lowChar, char highChar)
	{
	}

	public override void WriteChars(char[] buffer, int index, int count)
	{
	}

	public override void WriteRaw(char[] buffer, int index, int count)
	{
	}

	public override void WriteRaw(string data)
	{
	}

	public override void WriteBase64(byte[] buffer, int index, int count)
	{
	}

	public override void WriteBinHex(byte[] buffer, int index, int count)
	{
	}

	public override void Close()
	{
	}

	public override void Flush()
	{
	}

	public override void WriteName(string name)
	{
	}

	public override void WriteQualifiedName(string localName, string ns)
	{
	}

	public override string LookupPrefix(string ns)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public override void WriteNmToken(string name)
	{
	}
}
