using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class StartMenu : Menu
{
	public static MenuEntry StartMessage = new MenuEntry();

	public static bool StartControllerSelected = false;

	public static Texture2D BackGroundOverride = null;

	public static bool ApocThemeMusicRampUp = true;

	public static float ApocThemeMusicVolume = 0f;

	public static Cue ApocThemeMusic = null;

	private Vector2 msgPos = Vector2.Zero;

	public static void PlayThemeMusic(bool e)
	{
		if (e)
		{
			if (ApocThemeMusic == null)
			{
				ApocThemeMusic = EndGameEngine.SoundBnk.GetCue("ApocThemeTest");
				ApocThemeMusic.Play();
				ApocThemeMusic.SetVariable("Distance", ApocThemeMusicVolume);
			}
			else if (!ApocThemeMusic.IsPlaying)
			{
				ApocThemeMusic.Dispose();
				ApocThemeMusic = EndGameEngine.SoundBnk.GetCue("ApocThemeTest");
				ApocThemeMusic.Play();
				ApocThemeMusicRampUp = true;
				ApocThemeMusic.SetVariable("Distance", ApocThemeMusicVolume);
			}
		}
		else if (ApocThemeMusic != null)
		{
			ApocThemeMusicRampUp = false;
			if (ApocThemeMusicVolume >= 20000f)
			{
				ApocThemeMusic.Dispose();
			}
		}
	}

	public StartMenu(GameMenus id)
		: base(id)
	{
	}

	public override void LoadContent()
	{
		base.LoadContent();
		StartMessage.entryType = MenuEntryType.Text;
		StartMessage.entryAttribute = (MenuEntryAttribute)6;
		StartMessage.scale = 1f;
		StartMessage.text = "Press Start";
		StartMessage.diffuse = Color.White;
		StartMessage.shadow = Color.Black;
		StartMessage.position.X = Menu.titleSafeArea.X + Menu.titleSafeArea.Width / 2;
		StartMessage.position.X -= Menu.defaultFont.MeasureString("Press Start").X * 0.5f;
		StartMessage.position.Y = Menu.titleSafeArea.Y + Menu.titleSafeArea.Height - 128;
		StartMessage.size = Vector2.Zero;
		StartMessage.Build();
	}

	public override void Update(float eTime)
	{
		StartMessage.Update(eTime, transitionDelta);
		base.Update(eTime);
		EndGameEngine.GameSettings.GameName.Contains("ApocalypseZ");
	}

	public override void Draw()
	{
		EndGameEngine.GraphicMgr.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
		Viewport viewport = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport;
		base.Draw();
		Color white = Color.White;
		white.R = (byte)(255f * transitionDelta);
		white.G = (byte)(255f * transitionDelta);
		white.B = (byte)(255f * transitionDelta);
		white.A = (byte)(255f * transitionDelta);
		Menu.spriteBatch.Begin();
		if (EndGameEngine.GameSettings.GameName.Contains("ApocalypseZ"))
		{
			Menu.spriteBatch.Draw(Menu.titleTexture, EndGameEngine.DefualtViewport.Bounds, white);
			string text = "Visit Our Webpage 'www.ApocZ.com'";
			string text2 = "And Forums 'www.ApocZ.com/forum' For Latest News And Support";
			Vector2 zero = Vector2.Zero;
			byte b = (white.B = (byte)(180f * transitionDelta));
			byte r = (white.G = b);
			white.R = r;
			zero.X = 640f - Menu.defaultFont.MeasureString(text).X * 0.5f * 0.8f;
			zero.Y = viewport.TitleSafeArea.Bottom - 52;
			Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero, white, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
			zero.X = 640f - Menu.defaultFont.MeasureString(text2).X * 0.5f * 0.8f;
			zero.Y = viewport.TitleSafeArea.Bottom - 24;
			Menu.spriteBatch.DrawString(Menu.defaultFont, text2, zero, white, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
		}
		Menu.spriteBatch.End();
		StartMessage.Draw();
	}

	public override void DrawBackground()
	{
		if (BackGroundOverride == null)
		{
			int num = (int)((float)Menu.titleSafeArea.Width * 0.5333f);
			int num2 = (int)((float)Menu.titleSafeArea.Height * 0.4741f);
			Rectangle a = new Rectangle(Menu.titleSafeArea.X + (Menu.titleSafeArea.Width - num), Menu.titleSafeArea.Y + (Menu.titleSafeArea.Height - num2), num, num2);
			Menu.spriteBatch.Begin();
			Menu.spriteBatch.Draw(defaultBackground, a, new Color(255, 255, 255, transitionAlpha));
			Menu.spriteBatch.End();
		}
		else
		{
			GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
			Rectangle a2 = new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
			Menu.spriteBatch.Begin();
			Menu.spriteBatch.Draw(BackGroundOverride, a2, new Color(255, 255, 255, transitionAlpha));
			Menu.spriteBatch.End();
		}
	}

	public override void MakeActive(MenuMgr e)
	{
		base.MakeActive(e);
	}
}
