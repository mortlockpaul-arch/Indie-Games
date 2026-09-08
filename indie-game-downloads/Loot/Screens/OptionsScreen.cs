using Eyehook.Framework;
using Loot.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Loot.Screens;

public class OptionsScreen : MenuBoxScreen
{
	private class Tips : MenuItem
	{
		public override string Name => "Tips";

		public override string Value => (!Profile.Preferences.Tips) ? "Off" : "On";

		private void toggle()
		{
			PlaySound.MenuClick();
			Profile.Preferences.Tips = !Profile.Preferences.Tips;
		}

		public override void Increment()
		{
			toggle();
		}

		public override void Decrement()
		{
			toggle();
		}

		public override void Click()
		{
			toggle();
		}
	}

	private class TextFX : MenuItem
	{
		public override string Name => "Floaty Text";

		public override string Value => (!Profile.Preferences.TextFX) ? "Off" : "On";

		private void toggle()
		{
			PlaySound.MenuClick();
			Profile.Preferences.TextFX = !Profile.Preferences.TextFX;
		}

		public override void Increment()
		{
			toggle();
		}

		public override void Decrement()
		{
			toggle();
		}

		public override void Click()
		{
			toggle();
		}
	}

	private class HPBar : MenuItem
	{
		public override string Name => "Life Bars";

		public override string Value => (!Profile.Preferences.HPBar) ? "Off" : "On";

		private void toggle()
		{
			PlaySound.MenuClick();
			Profile.Preferences.HPBar = !Profile.Preferences.HPBar;
		}

		public override void Increment()
		{
			toggle();
		}

		public override void Decrement()
		{
			toggle();
		}

		public override void Click()
		{
			toggle();
		}
	}

	private class VibrateItem : MenuItem
	{
		public override string Name => "Vibration";

		public override string Value => (!Profile.Preferences.Vibrate) ? "Off" : "On";

		private void toggle()
		{
			PlaySound.MenuClick();
			Profile.Preferences.Vibrate = !Profile.Preferences.Vibrate;
		}

		public override void Increment()
		{
			toggle();
		}

		public override void Decrement()
		{
			toggle();
		}

		public override void Click()
		{
			toggle();
		}
	}

	private class WobbleItem : MenuItem
	{
		public override string Name => "Fog Wobble";

		public override string Value => (!Profile.Preferences.Wobble) ? "Off" : "On";

		private void toggle()
		{
			PlaySound.MenuClick();
			Profile.Preferences.Wobble = !Profile.Preferences.Wobble;
		}

		public override void Increment()
		{
			toggle();
		}

		public override void Decrement()
		{
			toggle();
		}

		public override void Click()
		{
			toggle();
		}
	}

	private class Gore : MenuItem
	{
		public override string Name => "Gore";

		public override string Value => (!Profile.Preferences.Gore) ? "Off" : "On";

		private void toggle()
		{
			PlaySound.MenuClick();
			Profile.Preferences.Gore = !Profile.Preferences.Gore;
		}

		public override void Increment()
		{
			toggle();
		}

		public override void Decrement()
		{
			toggle();
		}

		public override void Click()
		{
			toggle();
		}
	}

	private class MusicVolume : MenuItem
	{
		private string[] values = new string[11]
		{
			"Off", "1", "2", "3", "4", "5", "6", "7", "8", "9",
			"Max"
		};

		public override string Name => "Music Vol.";

		public override string Value => values[Profile.Preferences.MusicVolume];

		public override void Increment()
		{
			PlaySound.MenuClick();
			Profile.Preferences.SetMusicVolume(Profile.Preferences.MusicVolume + 1);
		}

		public override void Decrement()
		{
			PlaySound.MenuClick();
			Profile.Preferences.SetMusicVolume(Profile.Preferences.MusicVolume - 1);
		}
	}

	private class EffectsVolume : MenuItem
	{
		private string[] values = new string[11]
		{
			"Off", "1", "2", "3", "4", "5", "6", "7", "8", "9",
			"Max"
		};

		public override string Name => "Effects Vol.";

		public override string Value => values[Profile.Preferences.EffectsVolume];

		private void playCoin()
		{
			PlaySound.Coin();
		}

		public override void Increment()
		{
			Profile.Preferences.SetEffectsVolume(Profile.Preferences.EffectsVolume + 1);
			playCoin();
		}

		public override void Decrement()
		{
			Profile.Preferences.SetEffectsVolume(Profile.Preferences.EffectsVolume - 1);
			playCoin();
		}
	}

	private class SaveItem : MenuItem
	{
		private Screen parent;

		public override string Name => "Save Changes";

		public override Align Align => Align.Center;

		public SaveItem(Screen parent)
		{
			this.parent = parent;
		}

		public override void Click()
		{
			PlaySound.MenuClick();
			Save.Preferences();
			MC.ScreenManager.removeScreen(parent);
		}
	}

	private class Discard : MenuItem
	{
		private Screen parent;

		private Preferences orig;

		public override string Name => "Discard Changes";

		public override Align Align => Align.Center;

		public Discard(Screen parent, Preferences orig)
		{
			this.parent = parent;
			this.orig = orig;
		}

		public override void Click()
		{
			PlaySound.MenuClick();
			Profile.SetPreferences(orig);
			Profile.Preferences.SetMusicVolume(Profile.Preferences.MusicVolume);
			Profile.Preferences.SetEffectsVolume(Profile.Preferences.EffectsVolume);
			MC.ScreenManager.removeScreen(parent);
		}
	}

	private readonly Preferences orig;

	private bool confirm;

	private Color dim = Color.Black * 0.25f;

	public OptionsScreen()
		: base("Options", 472, 312)
	{
		orig = new Preferences(Profile.Preferences);
		AddMenuItem(new BlankMenuItem());
		AddMenuItem(new Tips());
		AddMenuItem(new TextFX());
		AddMenuItem(new WobbleItem());
		AddMenuItem(new Gore());
		AddMenuItem(new HPBar());
		AddMenuItem(new VibrateItem());
		AddMenuItem(new BlankMenuItem());
		AddMenuItem(new MusicVolume());
		AddMenuItem(new EffectsVolume());
		AddMenuItem(new BlankMenuItem());
		AddMenuItem(new SaveItem(this));
		SetCurrentOption(1);
		confirm = false;
	}

	public override void update(GameTime gameTime)
	{
		if (MC.GamePadManager.isNewButtonDown(Buttons.B))
		{
			if (confirm)
			{
				PlaySound.Fail();
				return;
			}
			PlaySound.MenuCancel();
			if (Profile.Preferences.Equals(orig))
			{
				MC.ScreenManager.removeScreen(this);
				return;
			}
			ClearMenuItems();
			AddMenuItem(new SaveItem(this));
			AddMenuItem(new Discard(this, orig));
			SetCurrentOption(0);
			confirm = true;
		}
		else
		{
			base.update(gameTime);
		}
	}

	public override void draw(GameTime gameTime)
	{
		base.spriteBatch.Begin();
		Pixel.Draw(base.spriteBatch, base.viewportRect, dim);
		base.spriteBatch.End();
		base.draw(gameTime);
	}
}
