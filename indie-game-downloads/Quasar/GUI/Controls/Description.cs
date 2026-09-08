using Quasar.Language;

namespace Quasar.GUI.Controls;

public class Description : Control
{
	public const string Type = "Description";

	public override string ControlType => "Description";

	public string Text
	{
		get
		{
			if (base.Layout.ActiveControl is Group { ActiveControl: { } activeControl } && activeControl.GetParameter("Description", out var value))
			{
				return LanguageManager.Texts[value];
			}
			return "";
		}
	}

	public Description(Layout layout)
		: base(layout)
	{
	}
}
