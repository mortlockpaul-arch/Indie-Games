using System.Collections.Generic;
using System.Xml.Linq;
using Quasar.Input;
using Quasar.Language;

namespace Quasar.GUI.Controls;

public class ButtonInstructions : Control
{
	public struct ButtonInstructionsItem
	{
		public InputManager.MenuInputCodes button;

		public InputManager.MenuInputCodes button2;

		public bool isPaired;

		public ButtonInstructionsItem(InputManager.MenuInputCodes button)
		{
			this.button = button;
			button2 = InputManager.MenuInputCodes.Interact;
			isPaired = false;
		}

		public ButtonInstructionsItem(InputManager.MenuInputCodes button, InputManager.MenuInputCodes button2)
		{
			this.button = button;
			this.button2 = button2;
			isPaired = true;
		}
	}

	public delegate void ButtonInstructionsHandler(ButtonInstructions aButton);

	private const int InstructionsTypes = 12;

	public const string Type = "ButtonInstructions";

	private List<KeyValuePair<ButtonInstructionsItem, string>> instructions = new List<KeyValuePair<ButtonInstructionsItem, string>>();

	public override string ControlType => "ButtonInstructions";

	public List<KeyValuePair<ButtonInstructionsItem, string>> Instructions => instructions;

	public event ButtonInstructionsHandler OnChange;

	public ButtonInstructions(Layout layout)
		: base(layout)
	{
	}

	public void ClearInstructions()
	{
		instructions.Clear();
		if (OnChange != null)
		{
			OnChange(this);
		}
	}

	public void AddInstructions(InputManager.MenuInputCodes button, string text)
	{
		instructions.Add(new KeyValuePair<ButtonInstructionsItem, string>(new ButtonInstructionsItem(button), text));
		if (OnChange != null)
		{
			OnChange(this);
		}
	}

	public void AddInstructions(InputManager.MenuInputCodes button, InputManager.MenuInputCodes button2, string text)
	{
		instructions.Add(new KeyValuePair<ButtonInstructionsItem, string>(new ButtonInstructionsItem(button, button2), text));
		if (OnChange != null)
		{
			OnChange(this);
		}
	}

	public void AddInstructions(List<KeyValuePair<ButtonInstructionsItem, string>> list)
	{
		instructions.AddRange(list);
		if (OnChange != null)
		{
			OnChange(this);
		}
	}

	public override void parseXml(XElement xe)
	{
		foreach (XAttribute item in xe.Attributes())
		{
			for (int i = 0; i < 12; i++)
			{
				if (item.Name.LocalName == ((InputManager.MenuInputCodes)i).ToString())
				{
					instructions.Add(new KeyValuePair<ButtonInstructionsItem, string>(new ButtonInstructionsItem((InputManager.MenuInputCodes)i), LanguageManager.Texts[item.Value]));
				}
			}
		}
		base.parseXml(xe);
	}

	public override void Dispose()
	{
		OnChange = null;
		base.Dispose();
	}
}
