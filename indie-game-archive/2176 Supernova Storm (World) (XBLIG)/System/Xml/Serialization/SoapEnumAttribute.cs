namespace System.Xml.Serialization;

[AttributeUsage(AttributeTargets.Field)]
public class SoapEnumAttribute : Attribute
{
	public string Name
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
		set
		{
		}
	}

	public SoapEnumAttribute()
	{
	}

	public SoapEnumAttribute(string name)
	{
	}
}
