using System.Xml.Linq;

namespace Quasar.GUI.Controls;

public class ButtonInstructionsCreator : IControlCreator
{
	private static ButtonInstructionsCreator instance;

	public static ButtonInstructionsCreator Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new ButtonInstructionsCreator();
			}
			return instance;
		}
	}

	public Control ParseXml(XElement xe, Layout layout)
	{
		ButtonInstructions buttonInstructions = new ButtonInstructions(layout);
		buttonInstructions.parseXml(xe);
		return buttonInstructions;
	}
}
