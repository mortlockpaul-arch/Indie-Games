using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace OluXNA;

internal class VerifySaveLocationComponent : DrawableGameComponent
{
	private string text;

	private Vector2 textPos;

	private string text2;

	private Vector2 textPos2;

	private string buttonText;

	private Vector2 buttonPos;

	private Vector2 buttonOrig;

	private string buttonText2;

	private Vector2 buttonPos2;

	private Vector2 buttonOrig2;

	private IOModes ioSaveLoad;

	private bool reprompt;

	private bool allowChoice;

	private float goTime;

	private float activateTime;

	public VerifySaveLocationComponent(Game game, string _text, IOModes _ioMode)
		: base(game)
	{
		text = _text;
		ioSaveLoad = _ioMode;
		reprompt = false;
		allowChoice = true;
		activateTime = 0.05f;
		goTime = -0.01f;
	}

	public override void Initialize()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		textPos = new Vector2((float)BaseGame.WIDTH / 2f, (float)BaseGame.HEIGHT / 2f);
		textPos -= BaseGame.Get().hud.HUDfont.MeasureString(text) / 2f;
		buttonText = BaseGame.Get().hud.KeyMap[(Buttons)4096];
		buttonPos = new Vector2((float)BaseGame.WIDTH * 0.25f, (float)BaseGame.HEIGHT * 0.6f);
		buttonOrig = BaseGame.Get().hud.ControllerFont.MeasureString(buttonText);
		buttonOrig.X *= 1.5f;
		ref Vector2 reference = ref buttonOrig;
		reference.Y /= 2f;
		buttonText2 = BaseGame.Get().hud.KeyMap[(Buttons)16384];
		buttonPos2 = new Vector2((float)BaseGame.WIDTH * 0.25f, (float)BaseGame.HEIGHT * 0.7f);
		buttonOrig2 = BaseGame.Get().hud.ControllerFont.MeasureString(buttonText2);
		buttonOrig2.X *= 1.5f;
		ref Vector2 reference2 = ref buttonOrig2;
		reference2.Y /= 2f;
		text = BaseGame.WrapString(text, 0.8f * (float)BaseGame.WIDTH, 1f, BaseGame.Get().hud.HUDfont);
		textPos = new Vector2((float)BaseGame.WIDTH / 2f, (float)BaseGame.HEIGHT / 2f);
		textPos -= BaseGame.Get().hud.HUDfont.MeasureString(text) / 2f;
		((DrawableGameComponent)this).Initialize();
	}

	public override void Update(GameTime gameTime)
	{
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().input.Update();
		if (goTime > 0f)
		{
			goTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
		}
		if (activateTime > 0f)
		{
			activateTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
		}
		if (((GameComponent)this).Game.IsActive && !Guide.IsVisible)
		{
			if (((goTime < 0f && BaseGame.Get().input.PadPressed((Buttons)4096) && activateTime < 0f && allowChoice) || reprompt) && !BaseGame.Get().continueWithoutSaving)
			{
				goTime = 0.01f;
				if (reprompt)
				{
					goTime = 1f;
				}
				reprompt = false;
				allowChoice = false;
			}
			else if ((goTime < 0f && BaseGame.Get().input.PadPressed((Buttons)16384) && activateTime < 0f && allowChoice) || BaseGame.Get().continueWithoutSaving)
			{
				BaseGame.Get().continueWithoutSaving = true;
				RemoveRestart();
			}
			else if (goTime > 1.25f)
			{
				goTime = -0.01f;
				switch (ioSaveLoad)
				{
				case IOModes.LoadHS:
					Guide.BeginShowStorageDeviceSelector((AsyncCallback)GetDeviceLoadHS, (object)null);
					break;
				case IOModes.LoadPlayer:
					Guide.BeginShowStorageDeviceSelector(BaseGame.Get().input.ActivePlayerIndex, (AsyncCallback)GetDeviceLoadPlayer, (object)null);
					break;
				case IOModes.SaveHS:
					Guide.BeginShowStorageDeviceSelector((AsyncCallback)GetDeviceSaveHS, (object)null);
					break;
				case IOModes.SavePlayer:
					Guide.BeginShowStorageDeviceSelector(BaseGame.Get().input.ActivePlayerIndex, (AsyncCallback)GetDeviceSavePlayer, (object)null);
					break;
				}
			}
		}
		((GameComponent)this).Update(gameTime);
	}

	public void GetDeviceLoadHS(IAsyncResult result)
	{
		StorageDevice val = Guide.EndShowStorageDeviceSelector(result);
		if (val == null)
		{
			reprompt = true;
			return;
		}
		BaseGame.Get().globStorageDevice = val;
		BaseGame.Get().BeginLoadHS();
		if (BaseGame.Get().HSLoad)
		{
			RemoveRestart();
		}
		else
		{
			Remove();
		}
	}

	public void GetDeviceSaveHS(IAsyncResult result)
	{
		StorageDevice val = Guide.EndShowStorageDeviceSelector(result);
		if (val == null)
		{
			reprompt = true;
			return;
		}
		BaseGame.Get().globStorageDevice = val;
		BaseGame.Get().BeginSaveHS();
		if (BaseGame.Get().HSSaved)
		{
			RemoveRestart();
		}
		else
		{
			Remove();
		}
	}

	public void GetDeviceLoadPlayer(IAsyncResult result)
	{
		StorageDevice val = Guide.EndShowStorageDeviceSelector(result);
		if (val == null)
		{
			reprompt = true;
			return;
		}
		BaseGame.Get().storageDevice = val;
		BaseGame.Get().BeginLoadPlayer();
		if (BaseGame.Get().PlayerLoad)
		{
			RemoveRestart();
		}
		else
		{
			Remove();
		}
	}

	public void GetDeviceSavePlayer(IAsyncResult result)
	{
		StorageDevice val = Guide.EndShowStorageDeviceSelector(result);
		if (val == null)
		{
			reprompt = true;
			return;
		}
		BaseGame.Get().storageDevice = val;
		BaseGame.Get().BeginSavePlayer();
		if (BaseGame.Get().PlayerSaved)
		{
			RemoveRestart();
		}
		else
		{
			Remove();
		}
	}

	public void RemoveRestart()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		foreach (GameComponent item in (Collection<IGameComponent>)(object)((GameComponent)this).Game.Components)
		{
			GameComponent val = item;
			val.Enabled = true;
		}
		Remove();
	}

	public void Remove()
	{
		((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Remove((IGameComponent)(object)this);
		((GameComponent)this).Dispose();
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		SpriteBatch spriteBatch = BaseGame.Get().spriteBatch;
		spriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)3, (SaveStateMode)0);
		spriteBatch.Draw(BaseGame.Get().hud.blackTex, new Rectangle(0, 0, BaseGame.WIDTH, BaseGame.HEIGHT), (Rectangle?)null, new Color(1f, 1f, 1f, 0.6f), 0f, Vector2.Zero, (SpriteEffects)0, 0.01f);
		spriteBatch.DrawString(BaseGame.Get().hud.HUDfont, text, textPos, Color.White);
		spriteBatch.DrawString(BaseGame.Get().hud.ControllerFont, buttonText, buttonPos, Color.White);
		spriteBatch.DrawString(BaseGame.Get().hud.HUDfont, "Select storage device (or use hard drive)", buttonPos + buttonOrig, Color.White, 0f, Vector2.Zero, 0.6f, (SpriteEffects)0, 0f);
		spriteBatch.DrawString(BaseGame.Get().hud.ControllerFont, buttonText2, buttonPos2, Color.White);
		spriteBatch.DrawString(BaseGame.Get().hud.HUDfont, "Continue without saving", buttonPos2 + buttonOrig2, Color.White, 0f, Vector2.Zero, 0.6f, (SpriteEffects)0, 0f);
		spriteBatch.End();
		((DrawableGameComponent)this).Draw(gameTime);
	}
}
