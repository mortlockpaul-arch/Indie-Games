using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace StarbeamDefenderGame;

public class StartMenu
{
	private int flash;

	private int flashtimer;

	private bool completed;

	private SpriteFont menufont;

	private StorageDevice sdevice;

	private bool showingstorageselect;

	private IAsyncResult storageresult;

	private Texture2D backgroundtexture;

	private PlayerIndex playeronecontroller;

	public StorageDevice StorageAccessDevice => sdevice;

	public PlayerIndex MasterControllerIndex => playeronecontroller;

	public void LoadContent(ContentManager content, string backgroundfile)
	{
		menufont = content.Load<SpriteFont>("Fonts/Main");
		backgroundtexture = content.Load<Texture2D>(backgroundfile);
	}

	private bool IsKeyDown(PlayerIndex controllerindex, Buttons button, Keys key)
	{
		GamePadState state = GamePad.GetState(controllerindex);
		KeyboardState state2 = Keyboard.GetState();
		if (state.IsButtonDown(button))
		{
			return true;
		}
		if (state2.IsKeyDown(key))
		{
			return true;
		}
		return false;
	}

	public int DoGameUpdate(int timems)
	{
		if (showingstorageselect)
		{
			return 0;
		}
		if (completed)
		{
			return 1;
		}
		flashtimer -= timems;
		if (flashtimer < 0)
		{
			flashtimer += 500;
			flash++;
			if (flash > 1)
			{
				flash = 0;
			}
		}
		for (int i = 0; i < 4; i++)
		{
			if (IsKeyDown((PlayerIndex)i, Buttons.Start, Keys.Enter) | IsKeyDown((PlayerIndex)i, Buttons.A, Keys.Space))
			{
				playeronecontroller = (PlayerIndex)i;
				showingstorageselect = true;
				completed = false;
				storageresult = StorageDevice.BeginShowSelector(DeviceSelected, null);
				break;
			}
		}
		return 0;
	}

	private void DeviceSelected(IAsyncResult result)
	{
		sdevice = StorageDevice.EndShowSelector(result);
		showingstorageselect = false;
		completed = true;
	}

	public void Draw(SpriteBatch spritebatch)
	{
		Rectangle value = new Rectangle(0, 0, 1280, 720);
		spritebatch.Draw(destinationRectangle: new Rectangle(0, 0, 1280, 720), texture: backgroundtexture, sourceRectangle: value, color: Color.White);
		if (flash == 0)
		{
			Vector2 vector = menufont.MeasureString("Press Start");
			spritebatch.DrawString(menufont, "Press Start", new Vector2(640f - vector.X / 2f, 300f), Color.Red);
		}
		else
		{
			Vector2 vector = menufont.MeasureString("Press Start");
			spritebatch.DrawString(menufont, "Press Start", new Vector2(640f - vector.X / 2f, 300f), Color.Green);
		}
	}
}
