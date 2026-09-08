using Microsoft.Xna.Framework;

namespace Deep_waters;

internal class OptionsMenuScreen : MenuScreen
{
	private MenuEntry screenwidth;

	private MenuEntry screenheight;

	private MenuEntry MusicVolume;

	private MenuEntry SfxVolume;

	public OptionsMenuScreen(Vector2 pos)
		: base("Options", pos)
	{
		screenwidth = new MenuEntry(string.Empty);
		screenheight = new MenuEntry(string.Empty);
		MusicVolume = new MenuEntry(string.Empty);
		SfxVolume = new MenuEntry(string.Empty);
		SetMenuEntryText();
		MenuEntry menuEntry = new MenuEntry("Back");
		menuEntry.Selected += base.OnCancelOptions;
		base.MenuEntries.Add(screenwidth);
		base.MenuEntries.Add(screenheight);
		base.MenuEntries.Add(MusicVolume);
		base.MenuEntries.Add(SfxVolume);
		base.MenuEntries.Add(menuEntry);
	}

	private void SetMenuEntryText()
	{
		screenwidth.Text = "Screen width: " + currentsettings.percX;
		screenheight.Text = "Screen Heigth: " + currentsettings.percY;
		MusicVolume.Text = "BGM Volume: " + currentsettings.music;
		SfxVolume.Text = "SFX Volume: " + currentsettings.sfx;
	}

	private void useless(object sender, PlayerIndexEventArgs e)
	{
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		SetMenuEntryText();
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		if (screenwidth.isSelected)
		{
			currentsettings.percX += value;
			currentsettings.percX = (int)MathHelper.Clamp(currentsettings.percX, 50f, 100f);
		}
		if (screenheight.isSelected)
		{
			currentsettings.percY += value;
			currentsettings.percY = (int)MathHelper.Clamp(currentsettings.percY, 50f, 100f);
		}
		if (MusicVolume.isSelected)
		{
			currentsettings.music += value;
		}
		if (SfxVolume.isSelected)
		{
			currentsettings.sfx += value;
		}
	}

	public override void HandleInput(InputState input)
	{
		base.HandleInput(input);
		if (input.IsMenuLeft(base.ControllingPlayer) || input.IsMenuRight(base.ControllingPlayer))
		{
			if (input.IsMenuLeft(base.ControllingPlayer))
			{
				value = -1;
			}
			if (input.IsMenuRight(base.ControllingPlayer))
			{
				value = 1;
			}
		}
		else if (input.CurrentGamePadStates[(int)base.ControllingPlayer.Value].ThumbSticks.Left.X == 0f)
		{
			value = 0;
		}
	}
}
