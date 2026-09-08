using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;

namespace JamSouls;

internal class DataScreen : GameScreen
{
	private ContentManager content;

	private IAsyncResult m_StorageSelectResult;

	private bool m_Request;

	private static StorageDevice m_StorageDevice;

	private Vector2 MESSAGE_POS = new Vector2(640f, 360f);

	private Texture2D m_WhiteBorderTex;

	private Rectangle m_WhiteBorder;

	private Texture2D m_BlackBorderTex;

	private Rectangle m_BlackBorder;

	private string m_Message = "";

	private bool m_LoadData;

	public DataScreen(bool bLoad)
	{
		base.TransitionOnTime = TimeSpan.FromSeconds(1.5);
		base.TransitionOffTime = TimeSpan.FromSeconds(1.5);
		base.IsPopup = true;
		m_WhiteBorder = new Rectangle(0, 0, 410, 110);
		m_WhiteBorder.X = 640 - m_WhiteBorder.Width / 2;
		m_WhiteBorder.Y = 360 - m_WhiteBorder.Height / 2;
		m_BlackBorder = new Rectangle(0, 0, 400, 100);
		m_BlackBorder.X = 640 - m_BlackBorder.Width / 2;
		m_BlackBorder.Y = 360 - m_BlackBorder.Height / 2;
		m_LoadData = bLoad;
		if (bLoad)
		{
			m_Message = TextManager.GetText(TextID.LOADING_DATA);
		}
		else
		{
			m_Message = TextManager.GetText(TextID.SAVING_DATA);
		}
		m_Request = true;
		SaveHandler.ResetState();
	}

	public override void LoadContent()
	{
		if (content == null)
		{
			content = new ContentManager(base.ScreenManager.Game.Services, "Content");
		}
		base.ScreenManager.Game.ResetElapsedTime();
		m_WhiteBorderTex = new Texture2D(base.ScreenManager.GraphicsDevice, 1, 1);
		m_WhiteBorderTex.SetData(new Color[1] { Color.White });
		m_BlackBorderTex = new Texture2D(base.ScreenManager.GraphicsDevice, 1, 1);
		m_BlackBorderTex.SetData(new Color[1] { Color.Black });
	}

	public override void UnloadContent()
	{
		content.Unload();
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		if (base.IsExiting)
		{
			return;
		}
		if (m_StorageDevice == null || !m_StorageDevice.IsConnected)
		{
			m_StorageDevice = null;
			m_Request = false;
			if (m_StorageSelectResult == null && !Guide.IsVisible)
			{
				m_StorageSelectResult = StorageDevice.BeginShowSelector(base.ControllingPlayer.Value, null, null);
			}
			if (m_StorageSelectResult != null && m_StorageSelectResult.IsCompleted)
			{
				m_StorageDevice = StorageDevice.EndShowSelector(m_StorageSelectResult);
				m_Request = true;
			}
		}
		if (!m_Request)
		{
			return;
		}
		if (m_StorageDevice != null && m_StorageDevice.IsConnected)
		{
			if (m_LoadData)
			{
				if (!SaveHandler.IsLoadRequestDone())
				{
					SaveHandler.LoadGame(m_StorageDevice);
				}
			}
			else if (!SaveHandler.IsSaveRequestDone())
			{
				SaveHandler.SaveGame(m_StorageDevice);
			}
		}
		else
		{
			SaveHandler.CancelSave();
			SaveHandler.CancelLoad();
			m_StorageDevice = null;
		}
		ExitScreen();
	}

	public override void Draw(GameTime gameTime)
	{
		if (m_Request)
		{
			SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
			spriteBatch.Begin();
			spriteBatch.Draw(m_WhiteBorderTex, m_WhiteBorder, Color.White);
			spriteBatch.Draw(m_BlackBorderTex, m_BlackBorder, Color.White);
			base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref MESSAGE_POS, m_Message, ScreenManager.TextOrigin.center_center, Color.White);
			spriteBatch.End();
		}
	}
}
