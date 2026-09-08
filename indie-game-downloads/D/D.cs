using System.IO;
using System.Xml.Linq;

namespace D;

internal class D
{
	public _6 DocumentElement;

	public void Load(Stream stream)
	{
		XDocument xDocument = XDocument.Load(stream);
		DocumentElement = new _6();
		DocumentElement.Load(xDocument.Root);
	}
}
