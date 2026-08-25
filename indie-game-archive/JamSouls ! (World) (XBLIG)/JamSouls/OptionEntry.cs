using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JamSouls;

internal class OptionEntry
{
	public const float TEXT_OFFSET = 50f;

	public Vector2 position;

	public Color color;

	public Sprite icon;

	public string text;

	public List<string> m_Entry = new List<string>();

	public int m_SelectedOption;

	public OptionEntry(Sprite texture, string t, Vector2 pos, int selected)
	{
		color = Color.Gray;
		icon = texture;
		text = t;
		position = pos;
		m_SelectedOption = selected;
	}

	public void Draw(ScreenManager screen)
	{
		Vector2 vector = position;
		string text = "";
		if (m_SelectedOption < m_Entry.Count)
		{
			text = m_Entry[m_SelectedOption];
		}
		Color textcolor = Color.Green;
		if (text == TextManager.GetText(TextID.OFF))
		{
			textcolor = Color.Red;
		}
		if (icon != null)
		{
			icon.Draw(position, Color.White, SpriteEffects.None, 1f);
			vector.X += 50f;
		}
		if (this.text != "")
		{
			screen.DrawText(screen.GoBoomMiddle, ref vector, this.text, ScreenManager.TextOrigin.top_Left, color);
		}
		vector.X += screen.GoBoomMiddle.MeasureString(this.text).X + 50f;
		screen.DrawText(screen.GoBoomMiddle, ref vector, text, ScreenManager.TextOrigin.top_Left, textcolor);
	}
}
