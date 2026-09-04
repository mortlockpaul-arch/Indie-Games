using System.IO;
using System.Text;

namespace System.Xml;

public abstract class XmlWriter : IDisposable
{
	public virtual XmlWriterSettings Settings
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public abstract WriteState WriteState { get; }

	public virtual string XmlLang
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public virtual XmlSpace XmlSpace
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public abstract void WriteStartDocument();

	public abstract void WriteStartDocument(bool standalone);

	public abstract void WriteEndDocument();

	public abstract void WriteDocType(string name, string pubid, string sysid, string subset);

	public void WriteStartElement(string localName, string ns)
	{
	}

	public abstract void WriteStartElement(string prefix, string localName, string ns);

	public void WriteStartElement(string localName)
	{
	}

	public abstract void WriteEndElement();

	public abstract void WriteFullEndElement();

	public void WriteAttributeString(string localName, string ns, string value)
	{
	}

	public void WriteAttributeString(string localName, string value)
	{
	}

	public void WriteAttributeString(string prefix, string localName, string ns, string value)
	{
	}

	public void WriteStartAttribute(string localName, string ns)
	{
	}

	public abstract void WriteStartAttribute(string prefix, string localName, string ns);

	public void WriteStartAttribute(string localName)
	{
	}

	public abstract void WriteEndAttribute();

	public abstract void WriteCData(string text);

	public abstract void WriteComment(string text);

	public abstract void WriteProcessingInstruction(string name, string text);

	public abstract void WriteEntityRef(string name);

	public abstract void WriteCharEntity(char ch);

	public abstract void WriteWhitespace(string ws);

	public abstract void WriteString(string text);

	public abstract void WriteSurrogateCharEntity(char lowChar, char highChar);

	public abstract void WriteChars(char[] buffer, int index, int count);

	public abstract void WriteRaw(char[] buffer, int index, int count);

	public abstract void WriteRaw(string data);

	public abstract void WriteBase64(byte[] buffer, int index, int count);

	public virtual void WriteBinHex(byte[] buffer, int index, int count)
	{
	}

	public abstract void Close();

	public abstract void Flush();

	public abstract string LookupPrefix(string ns);

	public virtual void WriteNmToken(string name)
	{
	}

	public virtual void WriteName(string name)
	{
	}

	public virtual void WriteQualifiedName(string localName, string ns)
	{
	}

	public virtual void WriteValue(object value)
	{
	}

	public virtual void WriteValue(string value)
	{
	}

	public virtual void WriteValue(bool value)
	{
	}

	public virtual void WriteValue(DateTime value)
	{
	}

	public virtual void WriteValue(double value)
	{
	}

	public virtual void WriteValue(float value)
	{
	}

	public virtual void WriteValue(decimal value)
	{
	}

	public virtual void WriteValue(int value)
	{
	}

	public virtual void WriteValue(long value)
	{
	}

	public virtual void WriteAttributes(XmlReader reader, bool defattr)
	{
	}

	public virtual void WriteNode(XmlReader reader, bool defattr)
	{
	}

	public void WriteElementString(string localName, string value)
	{
	}

	public void WriteElementString(string localName, string ns, string value)
	{
	}

	public void WriteElementString(string prefix, string localName, string ns, string value)
	{
	}

	void IDisposable.Dispose()
	{
	}

	protected virtual void Dispose(bool disposing)
	{
	}

	public static XmlWriter Create(string outputFileName)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public static XmlWriter Create(string outputFileName, XmlWriterSettings settings)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public static XmlWriter Create(Stream output)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public static XmlWriter Create(Stream output, XmlWriterSettings settings)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public static XmlWriter Create(TextWriter output)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public static XmlWriter Create(TextWriter output, XmlWriterSettings settings)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public static XmlWriter Create(StringBuilder output)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public static XmlWriter Create(StringBuilder output, XmlWriterSettings settings)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public static XmlWriter Create(XmlWriter output)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public static XmlWriter Create(XmlWriter output, XmlWriterSettings settings)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}
}
