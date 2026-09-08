using System.Xml.Linq;

namespace Quasar.GUI.Controls.GroupControls;

public class SelectorCreator : IGroupControlCreator
{
	private static SelectorCreator instance;

	public static SelectorCreator Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new SelectorCreator();
			}
			return instance;
		}
	}

	public GroupControl ParseXml(XElement xe, Group group, Layout layout)
	{
		Selector selector = new Selector(layout, group);
		selector.parseXml(xe);
		return selector;
	}
}
