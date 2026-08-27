using Microsoft.Xna.Framework;

namespace EGEngine;

public class AudioVideoMenu : Menu
{
	private enum MenuIndex
	{
		AudioIndex,
		VideoIndex,
		NumOfIndex
	}

	public const float MusicHigh = 0f;

	public const float MusicMid = 0.5f;

	public const float MusicLow = 1f;

	public const float BrightnessHigh = 2f;

	public const float BrightnessMid = 1f;

	public const float BrightnessLow = 0.5f;

	public static float MusicVolume = 0.5f;

	public static float Brightness = 1f;

	protected static MusicEntry[] MusicList;

	protected static float RealMusicVolume = 0f;

	private static Color diffuse = Color.Black;

	private static Rectangle texRec;

	private static float levelBarOffset;

	public virtual void LoadMusic()
	{
	}

	public virtual void PauseAllMusic()
	{
		if (MusicList == null)
		{
			return;
		}
		for (int i = 0; i < MusicList.Length; i++)
		{
			if (MusicList[i].Instance.IsPlaying && !MusicList[i].Instance.IsPaused)
			{
				MusicList[i].Instance.Pause();
			}
		}
	}

	public virtual void PlayMusic(int index)
	{
		if (MusicVolume == 1f)
		{
			RealMusicVolume = 10000000f;
		}
		else
		{
			RealMusicVolume = MusicVolume * MusicVolume * MusicVolume * 1000000f;
		}
		if (MusicList == null)
		{
			return;
		}
		for (int i = 0; i < MusicList.Length; i++)
		{
			if (i != index && MusicList[i].Instance.IsPlaying && !MusicList[i].Instance.IsPaused)
			{
				MusicList[i].Instance.Pause();
			}
		}
		if (!MusicList[index].Instance.IsPlaying)
		{
			MusicList[index].Instance.Play();
		}
		else if (MusicList[index].Instance.IsPaused)
		{
			MusicList[index].Instance.Resume();
		}
		MusicList[index].Instance.SetVariable("Distance", RealMusicVolume);
	}

	public virtual void SetVolume()
	{
		RealMusicVolume = MusicVolume * 20000f;
	}

	public AudioVideoMenu()
	{
	}

	public AudioVideoMenu(GameMenus id)
		: base(id)
	{
	}

	public override void LoadContent()
	{
		base.LoadContent();
		SetupAudioVideoMenu();
		texRec = Menu.titleSafeArea;
		levelBarOffset = 0f;
		for (int i = 0; i < menuEntryList.Count; i++)
		{
			if (menuEntryList[i].size.X > levelBarOffset)
			{
				levelBarOffset = menuEntryList[i].size.X;
			}
		}
		levelBarOffset += 36f;
	}

	public override void Update(float eTime)
	{
		base.Update(eTime);
		_ = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
		if (menuEntryList[0].isSelected)
		{
			float num = 0.01f;
			if (LevelBaseMenu.InputUpdate.menuInputContinuos == MenuInput.MenuRight)
			{
				MusicVolume = ((MusicVolume - num > 0f) ? (MusicVolume - num) : 0f);
				SetVolume();
				Menu.PlayQuickSelect();
			}
			else if (LevelBaseMenu.InputUpdate.menuInputContinuos == MenuInput.MenuLeft)
			{
				MusicVolume = ((MusicVolume + num < 1f) ? (MusicVolume + num) : 1f);
				SetVolume();
				Menu.PlayQuickSelect();
			}
		}
		if (menuEntryList[1].isSelected)
		{
			if (LevelBaseMenu.InputUpdate.menuInputContinuos == MenuInput.MenuRight)
			{
				Brightness = ((Brightness + 0.01f < 2f) ? (Brightness + 0.01f) : 2f);
				LevelOutside.VideoBrightness = ((LevelOutside.VideoBrightness + 0.01f < 2f) ? (LevelOutside.VideoBrightness + 0.01f) : 2f);
				Menu.PlayQuickSelect();
			}
			else if (LevelBaseMenu.InputUpdate.menuInputContinuos == MenuInput.MenuLeft)
			{
				Brightness = ((Brightness - 0.01f > 0.5f) ? (Brightness - 0.01f) : 0.5f);
				LevelOutside.VideoBrightness = ((LevelOutside.VideoBrightness - 0.01f > 0.5f) ? (LevelOutside.VideoBrightness - 0.01f) : 0.5f);
				Menu.PlayQuickSelect();
			}
		}
	}

	public override void Draw()
	{
		Menu.spriteBatch.Begin();
		bgTextureColor.R = (byte)(200f * transitionDelta);
		bgTextureColor.G = (byte)(200f * transitionDelta);
		bgTextureColor.B = (byte)(200f * transitionDelta);
		bgTextureColor.A = (byte)(200f * transitionDelta);
		Menu.spriteBatch.Draw(LevelBaseMenu.texBlack, EndGameEngine.DefualtViewport.TitleSafeArea, bgTextureColor);
		bgTextureColor.R = (byte)(40f * transitionDelta);
		bgTextureColor.G = (byte)(40f * transitionDelta);
		bgTextureColor.B = (byte)(40f * transitionDelta);
		bgTextureColor.A = (byte)(40f * transitionDelta);
		int height = (int)((float)EndGameEngine.DefualtViewport.TitleSafeArea.Width / (float)Menu.titleTexture.Width * (float)Menu.titleTexture.Height);
		Menu.spriteBatch.Draw(d: new Rectangle(EndGameEngine.DefualtViewport.TitleSafeArea.X, EndGameEngine.DefualtViewport.TitleSafeArea.Y, EndGameEngine.DefualtViewport.TitleSafeArea.Width, height), s: new Rectangle(4, 4, Menu.titleTexture.Width - 8, Menu.titleTexture.Height - 8), t: Menu.titleTexture, c: bgTextureColor);
		DrawButtonControl(EndGameEngine.GraphicMgr.GraphicsDevice.Viewport, drawSelect: false, drawBack: true, drawReady: false);
		DrawMusicVolume();
		DrawVideoBrightness();
		Menu.spriteBatch.End();
		base.Draw();
	}

	private void DrawMusicVolume()
	{
		Vector2 zero = Vector2.Zero;
		zero = menuEntryList[0].position + menuEntryList[0].textOffset;
		zero.X += levelBarOffset;
		zero.Y += 2f;
		Rectangle a = new Rectangle(0, 0, 200, (int)menuEntryList[0].textHeight - 8);
		Rectangle a2 = new Rectangle(0, 0, 0, (int)menuEntryList[0].textHeight - 12);
		a.X = (int)zero.X;
		a.Y = (int)zero.Y + 4;
		a2.X = (int)zero.X + 2;
		a2.Y = (int)zero.Y + 6;
		a2.Width += (int)((1f - MusicVolume / 1f) * 200f);
		a2.Width -= 4;
		if (menuEntryList[0].isSelected)
		{
			byte b = (diffuse.A = (byte)(255f * transitionDelta));
			byte b3 = (diffuse.B = b);
			byte r = (diffuse.G = b3);
			diffuse.R = r;
		}
		else
		{
			byte b6 = (diffuse.A = (byte)(100f * transitionDelta));
			byte b8 = (diffuse.B = b6);
			byte r2 = (diffuse.G = b8);
			diffuse.R = r2;
		}
		Menu.spriteBatch.Draw(LevelBaseMenu.texBlack, a, diffuse);
		Menu.spriteBatch.Draw(LevelBaseMenu.texBrown, a2, diffuse);
	}

	private void DrawVideoBrightness()
	{
		Vector2 zero = Vector2.Zero;
		zero = menuEntryList[1].position + menuEntryList[1].textOffset;
		zero.X += levelBarOffset;
		zero.Y += 2f;
		Rectangle a = new Rectangle(0, 0, 200, (int)menuEntryList[1].textHeight - 8);
		Rectangle a2 = new Rectangle(0, 0, 0, (int)menuEntryList[1].textHeight - 12);
		a.X = (int)zero.X;
		a.Y = (int)zero.Y + 4;
		a2.X = (int)zero.X + 2;
		a2.Y = (int)zero.Y + 6;
		a2.Width += (int)((LevelOutside.VideoBrightness - 0.5f) * 133f);
		a2.Width -= 4;
		if (menuEntryList[1].isSelected)
		{
			byte b = (diffuse.A = (byte)(255f * transitionDelta));
			byte b3 = (diffuse.B = b);
			byte r = (diffuse.G = b3);
			diffuse.R = r;
		}
		else
		{
			byte b6 = (diffuse.A = (byte)(100f * transitionDelta));
			byte b8 = (diffuse.B = b6);
			byte r2 = (diffuse.G = b8);
			diffuse.R = r2;
		}
		Menu.spriteBatch.Draw(LevelBaseMenu.texBlack, a, diffuse);
		Menu.spriteBatch.Draw(LevelBaseMenu.texBrown, a2, diffuse);
	}

	private void DrawAudioVideoMenu()
	{
	}

	public override void DrawBackground()
	{
	}

	public override void MakeActive(MenuMgr e)
	{
		base.MakeActive(e);
		Menu.PlaySelect();
	}

	private void SetupAudioVideoMenu()
	{
		MenuEntry menuEntry = new MenuEntry();
		MenuEntry menuEntry2 = new MenuEntry();
		Vector2 zero = Vector2.Zero;
		zero.X = Menu.titleSafeArea.Left + 80;
		zero.Y = Menu.titleSafeArea.Top + 80;
		menuEntryList.Add(menuEntry.Set("Music Volume", MenuTextJustify.Left, zero, MusicVolumeFunc, EndGameEngine.GameAssetMgr));
		menuEntry.isSelected = true;
		zero.Y += menuEntry.textHeight;
		menuEntryList.Add(menuEntry2.Set("Brightness", MenuTextJustify.Left, zero, VideoBrightnessFunc, EndGameEngine.GameAssetMgr));
		menuEntry2.isSelected = true;
		zero.Y += menuEntry2.textHeight;
	}

	private void MusicVolumeFunc(object sender, MenuEntry e)
	{
	}

	private void VideoBrightnessFunc(object sender, MenuEntry e)
	{
	}
}
