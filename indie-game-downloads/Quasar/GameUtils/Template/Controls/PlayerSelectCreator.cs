using System.Xml.Linq;
using Quasar.GUI;

namespace Quasar.GameUtils.Template.Controls;

public class PlayerSelectCreator : IControlCreator
{
	private static PlayerSelectCreator instance;

	public static PlayerSelectCreator Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new PlayerSelectCreator();
			}
			return instance;
		}
	}

	public Control ParseXml(XElement xe, Layout layout)
	{
		PlayerSelect playerSelect = new PlayerSelect(layout);
		playerSelect.parseXml(xe);
		return playerSelect;
	}
}
