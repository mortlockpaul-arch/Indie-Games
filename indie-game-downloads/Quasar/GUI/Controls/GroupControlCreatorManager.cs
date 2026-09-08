using System.Collections.Generic;
using System.Xml.Linq;
using Quasar.GUI.Controls.GroupControls;

namespace Quasar.GUI.Controls;

public class GroupControlCreatorManager
{
	private static GroupControlCreatorManager instance;

	private Dictionary<string, IGroupControlCreator> factories = new Dictionary<string, IGroupControlCreator>();

	public static GroupControlCreatorManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GroupControlCreatorManager();
			}
			return instance;
		}
	}

	private GroupControlCreatorManager()
	{
		addFactory("Button", ButtonCreator.Instance);
		addFactory("Selector", SelectorCreator.Instance);
	}

	public GroupControl ParseControl(XElement xe, Group group, Layout layout)
	{
		IGroupControlCreator value = null;
		if (factories.TryGetValue(xe.Name.LocalName, out value))
		{
			return value.ParseXml(xe, group, layout);
		}
		return null;
	}

	public void addFactory(string name, IGroupControlCreator factory)
	{
		factories.Add(name, factory);
	}
}
