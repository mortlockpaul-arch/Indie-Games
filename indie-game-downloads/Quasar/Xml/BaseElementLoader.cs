using System.Xml.Linq;

namespace Quasar.Xml;

public abstract class BaseElementLoader
{
	public abstract Element Load(XElement xe, string currentPath);

	protected void LoadCommonData(Element e, XElement xe, string currentPath, bool loadChildren)
	{
		if (e == null)
		{
			return;
		}
		XAttribute xAttribute = xe.Attribute("name");
		if (xAttribute != null)
		{
			e.Name = xAttribute.Value;
		}
		e.Transform.FromXml(xe);
		XElement xElement = xe.Element("Behaviors");
		if (xElement != null)
		{
			foreach (XElement item in xElement.Elements())
			{
				Behavior behavior = Quasar.Xml.BehaviorLoader.Load(item);
				if (behavior != null)
				{
					e.addBehavior(behavior);
				}
			}
		}
		if (!loadChildren)
		{
			return;
		}
		XElement xElement2 = xe.Element("Children");
		if (xElement2 == null)
		{
			return;
		}
		foreach (XElement item2 in xElement2.Elements())
		{
			BaseElementLoader loader = ElementLoader.GetLoader(item2.Name.LocalName);
			if (loader != null)
			{
				Element element = loader.Load(item2, currentPath);
				if (element != null)
				{
					e.addChild(element);
				}
			}
		}
	}
}
