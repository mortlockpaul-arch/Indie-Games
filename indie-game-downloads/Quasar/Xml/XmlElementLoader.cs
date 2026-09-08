using System.Xml.Linq;

namespace Quasar.Xml;

internal class XmlElementLoader : BaseElementLoader
{
	public void Load(XElement xe, Element element, string currentPath)
	{
		LoadCommonData(element, xe, currentPath, loadChildren: true);
	}

	public override Element Load(XElement xe, string currentPath)
	{
		Element element = new Element();
		Load(xe, element, currentPath);
		return element;
	}
}
