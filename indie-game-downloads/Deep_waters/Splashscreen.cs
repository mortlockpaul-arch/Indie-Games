using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace Deep_waters;

internal class Splashscreen : GameScreen
{
	private float timepassed;

	private bool changescreen;

	private bool dontsave;

	private bool execActionOne;

	private bool execActionTwo;

	private int response = 50;

	private bool guideshowonce;

	private int mytimer;

	private bool requesthasshown;

	private bool hasbenexecuted;

	public override void LoadContent()
	{
		base.LoadContent();
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		if (Guide.IsVisible)
		{
			return;
		}
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		if (!base.IsActive || Guide.IsVisible)
		{
			return;
		}
		if (!execActionTwo)
		{
			if (!hasbenexecuted)
			{
				if (execActionOne)
				{
					if (mytimer >= 1000)
					{
						ActionOne();
					}
					else
					{
						mytimer += gameTime.ElapsedGameTime.Milliseconds;
					}
					return;
				}
				if (base.ScreenManager.storageManager.device == null && !dontsave)
				{
					execActionOne = true;
				}
			}
			else if (!base.ScreenManager.storageManager.loadedSettingsrequest && base.ScreenManager.storageManager.device == null && !dontsave)
			{
				execActionTwo = true;
			}
		}
		if (execActionTwo)
		{
			if (mytimer >= 1000)
			{
				ActionTwo();
			}
			else
			{
				mytimer += gameTime.ElapsedGameTime.Milliseconds;
			}
		}
		else if (timepassed >= 3000f)
		{
			if (!changescreen)
			{
				base.ScreenManager.AddScreen(new BackgroundScreen("blank"), null);
				base.ScreenManager.AddScreen(new adscreen(), null);
				changescreen = true;
			}
		}
		else
		{
			timepassed += gameTime.ElapsedGameTime.Milliseconds;
		}
	}

	private void ActionTwo()
	{
		if (!Guide.IsVisible)
		{
			Guide.BeginShowMessageBox("Save disabled", "you haven't selected a storage device, the save option has been disabled, the progress and preferences will not be saved,", new string[2] { "OK", "Cancel" }, 0, MessageBoxIcon.Alert, confirmwarning, base.ControllingPlayer);
			execActionTwo = false;
			hasbenexecuted = false;
		}
	}

	private void ActionOne()
	{
		if (!Guide.IsVisible)
		{
			base.ScreenManager.storageManager.device = null;
			base.ScreenManager.storageManager.result = null;
			base.ScreenManager.storageManager.loadedSettingsrequest = false;
			base.ScreenManager.loadsettings = true;
			base.ScreenManager.storageManager.requestloadsettings(PlayerIndex.One);
			execActionOne = false;
			hasbenexecuted = true;
			mytimer = 0;
		}
	}

	public void confirmwarning(IAsyncResult result)
	{
		int? num = Guide.EndShowMessageBox(result);
		if (!num.HasValue)
		{
			mytimer = 0;
			execActionOne = true;
			return;
		}
		int? num2 = num;
		if (num2.GetValueOrDefault() == 0 && num2.HasValue)
		{
			mytimer = 0;
			dontsave = true;
			base.ScreenManager.settings.enablesaving = false;
			hasbenexecuted = false;
		}
		else
		{
			mytimer = 0;
			execActionOne = true;
		}
	}

	public override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
	}
}
