using System;

namespace Quasar.ContentPipeline;

public class XmlSource : IDisposable
{
	private string xmlCode;

	public string XmlCode => xmlCode;

	public XmlSource(string xmlCode)
	{
		this.xmlCode = xmlCode;
	}

	public void Dispose()
	{
		xmlCode = null;
	}
}
