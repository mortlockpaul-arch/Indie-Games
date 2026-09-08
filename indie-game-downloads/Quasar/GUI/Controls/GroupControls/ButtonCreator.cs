using System.Xml.Linq;

namespace Quasar.GUI.Controls.GroupControls;

public class ButtonCreator : IGroupControlCreator
{
	private static ButtonCreator instance;

	public static ButtonCreator Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new ButtonCreator();
			}
			return instance;
		}
	}

	public GroupControl ParseXml(XElement xe, Group group, Layout layout)
	{
		Button button = new Button(layout, group);
		button.parseXml(xe);
		return button;
	}
}
