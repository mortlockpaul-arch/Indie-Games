using System.Net;

namespace System.Xml;

public abstract class XmlResolver
{
	public abstract ICredentials Credentials { set; }

	[Obsolete("This member is not present in the desktop .NET Framework.")]
	public virtual XmlNameTable NameTable
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public abstract object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn);

	public virtual Uri ResolveUri(Uri baseUri, string relativeUri)
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}
}
