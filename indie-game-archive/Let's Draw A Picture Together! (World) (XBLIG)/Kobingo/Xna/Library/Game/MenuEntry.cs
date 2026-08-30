using System;

namespace Kobingo.Xna.Library.Game;

public class MenuEntry
{
	public string Text { get; set; }

	public string Description { get; set; }

	public object Tag { get; set; }

	public string[] Values { get; private set; }

	public int SelectedIndex { get; set; }

	public string SelectedValue => Values[SelectedIndex];

	public bool Enabled { get; set; }

	public GameScreen GameScreen { get; private set; }

	public event EventHandler Selected;

	public MenuEntry(string text, string description, params string[] values)
	{
		Text = text;
		Description = description;
		Values = values;
		Enabled = true;
	}

	public MenuEntry(string text, string description, EventHandler selectedCallback)
		: this(text, description)
	{
		Selected = (EventHandler)Delegate.Combine(Selected, selectedCallback);
	}

	public MenuEntry(string text, string description, GameScreen gameScreen)
		: this(text, description)
	{
		GameScreen = gameScreen;
	}

	public virtual void Next()
	{
		if (Values.Length > 0)
		{
			SelectedIndex = (SelectedIndex + 1) % Values.Length;
		}
	}

	public virtual void Previous()
	{
		if (Values.Length > 0)
		{
			SelectedIndex = (SelectedIndex + 1) % Values.Length;
		}
	}

	public virtual void Select()
	{
		if (GameScreen != null)
		{
			GameScreen.Show();
		}
		if (Selected != null)
		{
			Selected(this, EventArgs.Empty);
		}
	}

	public override string ToString()
	{
		if (Values != null && Values.Length > 0)
		{
			return Text + ": " + SelectedValue;
		}
		return Text;
	}
}
