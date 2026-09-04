using System.Net;

namespace System.Xml;

public class XmlUrlResolver : XmlResolver
{
	public override ICredentials Credentials
	{
		set
		{
		}
	}

	public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}

	public override Uri ResolveUri(Uri baseUri, string relativeUri)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}
}
