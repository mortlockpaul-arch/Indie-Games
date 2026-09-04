namespace System.Xml;

public abstract class XmlCharacterData : XmlLinkedNode
{
	public virtual string Data
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
		set
		{
		}
	}

	public override string InnerText
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
		set
		{
		}
	}

	public virtual int Length
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public override string Value
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
		set
		{
		}
	}

	protected internal XmlCharacterData(string data, XmlDocument doc)
	{
	}

	public virtual string Substring(int offset, int count)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public virtual void AppendData(string strData)
	{
	}

	public virtual void InsertData(int offset, string strData)
	{
	}

	public virtual void DeleteData(int offset, int count)
	{
	}

	public virtual void ReplaceData(int offset, int count, string strData)
	{
	}
}
