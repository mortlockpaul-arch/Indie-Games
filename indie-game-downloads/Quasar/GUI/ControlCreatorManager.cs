using System.Collections.Generic;
using System.Xml.Linq;
using Quasar.GUI.Controls;

namespace Quasar.GUI;

public class ControlCreatorManager
{
	private static ControlCreatorManager instance;

	private Dictionary<string, IControlCreator> factories = new Dictionary<string, IControlCreator>();

	public static ControlCreatorManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new ControlCreatorManager();
			}
			return instance;
		}
	}

	private ControlCreatorManager()
	{
		addFactory("Group", Group.GroupCreator.Instance);
		addFactory("ClickButton", ClickButton.ClickButtonCreator.Instance);
		addFactory("Label", Label.LabelCreator.Instance);
		addFactory("Description", DescriptionCreator.Instance);
		addFactory("ButtonInstructions", ButtonInstructionsCreator.Instance);
	}

	public Control ParseControl(XElement xe, Layout layout)
	{
		IControlCreator value = null;
		if (factories.TryGetValue(xe.Name.LocalName, out value))
		{
			return value.ParseXml(xe, layout);
		}
		return null;
	}

	public void addFactory(string name, IControlCreator factory)
	{
		factories.Add(name, factory);
	}
}
