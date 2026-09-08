using Loot.Core;

namespace Loot.Screens;

public class DialogOption
{
	public DialogCallback Callback;

	public string Text;

	public FormattedText FormattedText;

	public DialogOption(string text)
	{
		Text = text;
		Callback = null;
	}

	public DialogOption(string text, DialogCallback callback)
	{
		Text = text;
		Callback = callback;
	}

	public void Format(int maxWidth)
	{
		FormattedText = new FormattedText(Text, maxWidth);
	}
}
