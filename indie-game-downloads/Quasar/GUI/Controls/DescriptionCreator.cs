using System.Xml.Linq;

namespace Quasar.GUI.Controls;

public class DescriptionCreator : IControlCreator
{
	private static DescriptionCreator instance;

	public static DescriptionCreator Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new DescriptionCreator();
			}
			return instance;
		}
	}

	public Control ParseXml(XElement xe, Layout layout)
	{
		Description description = new Description(layout);
		description.parseXml(xe);
		return description;
	}
}
