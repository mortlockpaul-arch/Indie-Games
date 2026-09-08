using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EGEngine;

public class Menu
{
	public struct sbSpriteBatch
	{
		private float startX;

		private float startY;

		private float scaleX;

		private float scaleY;

		private Vector2 posA;

		private Vector2 posB;

		private Rectangle recA;

		private Rectangle recB;

		public SpriteBatch sb;

		public void Begin()
		{
			sb.Begin();
		}

		public void Begin(SpriteSortMode sm, BlendState bs, SamplerState ss, DepthStencilState ds, RasterizerState rs)
		{
			sb.Begin(sm, bs, ss, ds, rs);
		}

		public void End()
		{
			sb.End();
		}

		public void Draw(Texture2D t, Rectangle d, Rectangle s, Color c)
		{
			sb.Draw(t, d, s, c);
		}

		public void Draw(Texture2D t, Rectangle a, Color c)
		{
			sb.Draw(t, a, c);
		}

		public void Draw(Texture2D t, Rectangle a, Rectangle? b, Color c, float d, Vector2 e, SpriteEffects f, int g)
		{
			sb.Draw(t, a, b, c, d, e, f, g);
		}

		public void Draw(Texture2D t, Vector2 a, Rectangle? b, Color c, float d, Vector2 e, Vector2 f, SpriteEffects g, int h)
		{
			if (b.HasValue)
			{
				sb.Draw(t, a, recA, c, d, e, f, g, h);
			}
			else
			{
				sb.Draw(t, a, b, c, d, e, f, g, h);
			}
		}

		public void DrawString(SpriteFont f, string s, Vector2 p, Color c)
		{
			sb.DrawString(f, s, p, c);
		}

		public void DrawString(SpriteFont a, string b, Vector2 c, Color d, float e, Vector2 f, float g, SpriteEffects h, int i)
		{
			sb.DrawString(a, b, c, d, e, f, g, h, i);
		}
	}

	private static bool initialized;

	public static bool HackSetViewPort;

	public GameMenus menuId;

	public byte transitionAlpha;

	public float currentTime;

	public float transitionTime;

	public float transitionDelta;

	public MenuState state;

	public Texture2D defaultBackground;

	public static Texture2D AvRMenu;

	public int SelectedEntry;

	public int menuListCountOverride;

	public List<MenuEntry> menuEntryList;

	public MenuMgr Manager;

	public bool drawMenuBackdrop;

	public static PlayerBase ActivePlayer;

	public static Texture2D backgraoundTexture;

	public static Texture2D logoTexture;

	public static Texture2D aButton;

	public static Texture2D bButton;

	public static Texture2D xButton;

	public static Texture2D yButton;

	public static Texture2D dpRight;

	public static Texture2D dpLeft;

	public static Texture2D dpUp;

	public static Texture2D dpDown;

	public static Texture2D dPad;

	public static Texture2D backButton;

	public static Texture2D startButton;

	public static Texture2D rightStick;

	public static Texture2D rightTrigger;

	public static Texture2D rightShoulder;

	public static Texture2D leftStick;

	public static Texture2D leftTrigger;

	public static Texture2D leftShoulder;

	public static Texture2D texGradientVertical;

	public static Texture2D titleTexture;

	public static Texture2D multiplayTexture;

	public static Rectangle titleSafeArea;

	public static Rectangle menuGradientRec;

	public static Color menuGradientColor;

	public static SpriteFont systemFont;

	public static SpriteFont defaultFont;

	public static SpriteFont defaultBigFont;

	public static SpriteFont LogoFont;

	public static Cue menuMusic;

	public static Cue levelBackgroundMusic;

	public static Cue levelAmbiencedMusic;

	public static MenuEntry BackDelegateEntry;

	public static string BACK_DELEGATE_ENTRY;

	public Color bgTextureColor;

	public Color buttonColor;

	public static sbSpriteBatch spriteBatch;

	private Random logoRand;

	private float fgMove;

	private float bgMove;

	private float lx;

	private float ly;

	private float dx;

	private float dy;

	private static Vector2 tmpTextPos;

	private static Rectangle tmpButtonRec;

	public static float AllMusicVolume;

	private static float LevelMusicVolume;

	private static float AmbientMusicVolume;

	private static float gameSetting;

	public bool IsActive => state != MenuState.Hidden;

	public MenuState State
	{
		get
		{
			return state;
		}
		set
		{
			state = value;
			currentTime = 0f;
		}
	}

	public Rectangle TitleSafeArea
	{
		set
		{
			titleSafeArea = value;
		}
	}

	public event EventHandler<MenuEntry> ExitMenuDelegate;

	public event EventHandler<MenuEntry> BackMenuDelegate;

	public bool IsBackDelegateNull()
	{
		return BackMenuDelegate == null;
	}

	public void ExecuteBackDelegate()
	{
		if (!IsBackDelegateNull())
		{
			BackMenuDelegate(this, BackDelegateEntry);
		}
	}

	public Color TransitionThisColor(Color e)
	{
		e.A = (byte)((float)(int)e.A * transitionDelta);
		e.R = (byte)((float)(int)e.R * transitionDelta);
		e.G = (byte)((float)(int)e.G * transitionDelta);
		e.B = (byte)((float)(int)e.B * transitionDelta);
		return e;
	}

	public Menu()
	{
		menuId = GameMenus.Invalid;
		state = MenuState.Hidden;
		menuEntryList = new List<MenuEntry>();
		drawMenuBackdrop = true;
		bgTextureColor = Color.White;
		buttonColor = Color.White;
		logoRand = new Random();
		lx = 1f;
		ly = 1f;
		dx = 1f;
		dy = 1f;
	}

	public Menu(GameMenus id)
	{
		menuId = GameMenus.Invalid;
		state = MenuState.Hidden;
		menuEntryList = new List<MenuEntry>();
		drawMenuBackdrop = true;
		bgTextureColor = Color.White;
		buttonColor = Color.White;
		logoRand = new Random();
		lx = 1f;
		ly = 1f;
		dx = 1f;
		dy = 1f;
		menuId = id;
	}

	public Menu(EventHandler<MenuEntry> exitmenudDelegate)
	{
		menuId = GameMenus.Invalid;
		state = MenuState.Hidden;
		menuEntryList = new List<MenuEntry>();
		drawMenuBackdrop = true;
		bgTextureColor = Color.White;
		buttonColor = Color.White;
		logoRand = new Random();
		lx = 1f;
		ly = 1f;
		dx = 1f;
		dy = 1f;
	}

	public virtual void LoadContent()
	{
		if (!initialized)
		{
			initialized = true;
			spriteBatch = default(sbSpriteBatch);
			spriteBatch.sb = new SpriteBatch(EndGameEngine.GraphicMgr.GraphicsDevice);
			systemFont = EndGameEngine.ContentMgr.Load<SpriteFont>("fonts\\systemFont");
			systemFont.LineSpacing = (int)((float)systemFont.LineSpacing * 0.5f);
			defaultFont = EndGameEngine.GameAssetMgr.Load<SpriteFont>("fonts\\default");
			defaultFont.LineSpacing = (int)((float)defaultFont.LineSpacing * 1.2f);
			_ = defaultFont.Spacing;
			defaultBigFont = EndGameEngine.GameAssetMgr.Load<SpriteFont>("fonts\\defaultBig");
			defaultBigFont.LineSpacing = (int)((float)defaultBigFont.LineSpacing * 0.5f);
			LogoFont = EndGameEngine.GameAssetMgr.Load<SpriteFont>("fonts\\LogoFont");
			LogoFont.LineSpacing = (int)((float)LogoFont.LineSpacing * 0.4f);
			TitleSafeArea = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea;
			aButton = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\engine\\abutton");
			bButton = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\engine\\bbutton");
			xButton = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\engine\\xbutton");
			yButton = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\engine\\ybutton");
			dpRight = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\engine\\dpright");
			dpLeft = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\engine\\dpleft");
			dpUp = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\engine\\dpup");
			dpDown = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\engine\\dpdown");
			dPad = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\engine\\dPad");
			rightStick = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\engine\\rightStick");
			rightTrigger = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\engine\\rightTrigger");
			rightShoulder = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\engine\\rightbutton");
			leftStick = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\engine\\leftStick");
			leftTrigger = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\engine\\leftTrigger");
			leftShoulder = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\engine\\leftbutton");
			startButton = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\engine\\startButton");
			backButton = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\engine\\backButton");
			if (EndGameEngine.GameSettings.GameName.Contains("ToyPlane"))
			{
				texGradientVertical = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\gradientvertYellow");
			}
			else
			{
				texGradientVertical = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\gradientvert");
			}
			titleTexture = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\Title");
			multiplayTexture = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\CampFire");
			if (EndGameEngine.GameSettings.GameName.Contains("_AvR_"))
			{
				AvRMenu = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\menus\\scrollMenu");
			}
			menuGradientColor = new Color(100, 100, 100, 160);
			menuGradientRec = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea;
			menuGradientRec.X += 64;
			menuGradientRec.Width = 300;
			BackDelegateEntry.text = BACK_DELEGATE_ENTRY;
		}
	}

	public virtual void UnLoadContent()
	{
	}

	public virtual void Update(float eTime)
	{
		UpdateTransition(eTime);
		int num = ((menuListCountOverride > 0) ? menuListCountOverride : menuEntryList.Count);
		for (int i = 0; i < num; i++)
		{
			menuEntryList[i].Update(eTime, transitionDelta);
		}
		HandleInput();
	}

	public void UpdateTransition(float eTime)
	{
		currentTime += eTime;
		if (state == MenuState.TransitionOn)
		{
			transitionDelta = currentTime / transitionTime;
			transitionDelta = ((transitionDelta > 1f) ? 1f : transitionDelta);
			transitionDelta = ((transitionDelta < 0f) ? 0f : transitionDelta);
			transitionAlpha = (byte)(transitionDelta * 255f);
			if (currentTime >= transitionTime)
			{
				state = MenuState.Active;
				transitionAlpha = byte.MaxValue;
			}
		}
		else if (state == MenuState.TransitionOff)
		{
			transitionDelta = 1f - currentTime / transitionTime;
			transitionDelta = ((transitionDelta > 1f) ? 1f : transitionDelta);
			transitionDelta = ((transitionDelta < 0f) ? 0f : transitionDelta);
			transitionAlpha = (byte)(transitionDelta * 255f);
			if (currentTime >= transitionTime)
			{
				state = MenuState.Hidden;
				transitionAlpha = 0;
			}
		}
		bgTextureColor.R = (byte)((float)(int)menuGradientColor.R * transitionDelta);
		bgTextureColor.G = (byte)((float)(int)menuGradientColor.G * transitionDelta);
		bgTextureColor.B = (byte)((float)(int)menuGradientColor.B * transitionDelta);
		bgTextureColor.A = (byte)((float)(int)menuGradientColor.A * transitionDelta);
		buttonColor.R = (byte)(220f * transitionDelta);
		buttonColor.G = (byte)(220f * transitionDelta);
		buttonColor.B = (byte)(220f * transitionDelta);
		buttonColor.A = (byte)(220f * transitionDelta);
	}

	public virtual void Draw()
	{
		int num = ((menuListCountOverride > 0) ? menuListCountOverride : menuEntryList.Count);
		for (int i = 0; i < num; i++)
		{
			menuEntryList[i].Draw();
		}
	}

	public virtual void DrawBackground()
	{
	}

	public void DrawMenuTwoTexShake(Texture2D bg, Texture2D fg, Color c)
	{
		fgMove += 0.8f;
		bgMove += 0.01f;
		float num = (float)Math.Cos(bgMove);
		float num2 = (float)Math.Sin(bgMove);
		float num3 = (float)Math.Sin(fgMove);
		float num4 = (float)Math.Cos(fgMove);
		spriteBatch.Draw(bg, new Vector2(num * 20f - 20f, num2 * 20f - 20f), null, c, 0f, Vector2.Zero, new Vector2(1.05f, 1.05f), SpriteEffects.None, 0);
		spriteBatch.Draw(fg, new Rectangle(128, 0, 1024, 648), null, c, 0f, new Vector2(num3 * 0.15f, num4 * 0.15f), SpriteEffects.None, 0);
	}

	public virtual void MakeActive(MenuMgr e)
	{
		Manager = e;
		transitionDelta = 0f;
		transitionTime = 0.25f;
		State = MenuState.TransitionOn;
		EndGameEngine.enableClearTarget = true;
		ResetMenuEntries();
		ref Color reference = ref bgTextureColor;
		ref Color reference2 = ref bgTextureColor;
		ref Color reference3 = ref bgTextureColor;
		byte b = (bgTextureColor.A = 0);
		byte b3 = b;
		b = (reference3.B = b3);
		byte b5 = b;
		b = (reference2.G = b5);
		byte r = b;
		reference.R = r;
		ref Color reference4 = ref buttonColor;
		ref Color reference5 = ref buttonColor;
		ref Color reference6 = ref buttonColor;
		b = (buttonColor.A = 0);
		byte b8 = b;
		b = (reference6.B = b8);
		byte b10 = b;
		b = (reference5.G = b10);
		byte r2 = b;
		reference4.R = r2;
	}

	public virtual void ResetMenuEntries()
	{
		int count = menuEntryList.Count;
		for (int i = 0; i < count; i++)
		{
			MenuEntry menuEntry = menuEntryList[i];
			menuEntry.isSelected = false;
			ref Color diffuse = ref menuEntry.diffuse;
			ref Color diffuse2 = ref menuEntry.diffuse;
			ref Color diffuse3 = ref menuEntry.diffuse;
			byte b = (menuEntry.diffuse.A = 0);
			byte b3 = b;
			b = (diffuse3.B = b3);
			byte b5 = b;
			b = (diffuse2.G = b5);
			byte r = b;
			diffuse.R = r;
			ref Color shadow = ref menuEntry.shadow;
			ref Color shadow2 = ref menuEntry.shadow;
			ref Color shadow3 = ref menuEntry.shadow;
			b = (menuEntry.shadow.A = 0);
			byte b8 = b;
			b = (shadow3.B = b8);
			byte b10 = b;
			b = (shadow2.G = b10);
			byte r2 = b;
			shadow.R = r2;
			ref Color diffuseSelected = ref menuEntry.diffuseSelected;
			ref Color diffuseSelected2 = ref menuEntry.diffuseSelected;
			ref Color diffuseSelected3 = ref menuEntry.diffuseSelected;
			b = (menuEntry.diffuseSelected.A = 0);
			byte b13 = b;
			b = (diffuseSelected3.B = b13);
			byte b15 = b;
			b = (diffuseSelected2.G = b15);
			byte r3 = b;
			diffuseSelected.R = r3;
		}
		SelectedEntry = 0;
		if (menuEntryList.Count > 0)
		{
			menuEntryList[SelectedEntry].isSelected = true;
		}
	}

	public virtual void HandleInput()
	{
		int num = ((menuListCountOverride > 0) ? menuListCountOverride : menuEntryList.Count);
		if (state != MenuState.Active || num < 1 || HandleBackInput())
		{
			return;
		}
		if (ActivePlayer != null)
		{
			if (ActivePlayer.menuInput == MenuInput.MenuSelect && menuEntryList[SelectedEntry].strikeOutText == null)
			{
				ActivePlayer.menuInput = MenuInput.None;
				menuEntryList[SelectedEntry].TrySelected();
			}
			else if (ActivePlayer.menuInput == MenuInput.MenuUp)
			{
				SelectedEntry--;
				if (SelectedEntry < 0)
				{
					SelectedEntry = num - 1;
				}
			}
			else if (ActivePlayer.menuInput == MenuInput.MenuDown)
			{
				SelectedEntry++;
				if (SelectedEntry >= num)
				{
					SelectedEntry = 0;
				}
			}
		}
		else if (LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuSelect && menuEntryList[SelectedEntry].strikeOutText == null)
		{
			LevelBaseMenu.InputUpdate.menuInput = MenuInput.None;
			menuEntryList[SelectedEntry].TrySelected();
		}
		else if (LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuUp)
		{
			SelectedEntry--;
			if (SelectedEntry < 0)
			{
				SelectedEntry = num - 1;
			}
		}
		else if (LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuDown)
		{
			SelectedEntry++;
			if (SelectedEntry >= num)
			{
				SelectedEntry = 0;
			}
		}
		for (int i = 0; i < num; i++)
		{
			if (i == SelectedEntry)
			{
				menuEntryList[i].isSelected = true;
			}
			else
			{
				menuEntryList[i].isSelected = false;
			}
		}
	}

	public bool HandleBackInput()
	{
		if (ActivePlayer != null)
		{
			if (ActivePlayer.menuInput == MenuInput.MenuBack && BackMenuDelegate != null)
			{
				BackMenuDelegate(this, BackDelegateEntry);
				ActivePlayer.menuInput = MenuInput.None;
				return true;
			}
		}
		else if (LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuBack && BackMenuDelegate != null)
		{
			BackMenuDelegate(this, BackDelegateEntry);
			LevelBaseMenu.InputUpdate.menuInput = MenuInput.None;
			return true;
		}
		return false;
	}

	public void TryExitMenuDelegate(MenuEntry e)
	{
		if (ExitMenuDelegate != null)
		{
			ExitMenuDelegate(this, e);
		}
	}

	public void DrawButtonControl(Viewport view, bool drawSelect, bool drawBack, bool drawReady)
	{
		tmpTextPos.X = 200f;
		tmpTextPos.Y = 600f;
		tmpButtonRec.X = (int)tmpTextPos.X;
		tmpButtonRec.Y = (int)tmpTextPos.Y;
		tmpButtonRec.Width = 32;
		tmpButtonRec.Height = 32;
		if (drawSelect)
		{
			tmpTextPos.X += 38f;
			tmpTextPos.Y += 4f;
			spriteBatch.Draw(aButton, tmpButtonRec, buttonColor);
			spriteBatch.DrawString(defaultFont, "Select", tmpTextPos, buttonColor, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
		}
		if (drawBack)
		{
			if (drawSelect)
			{
				tmpTextPos.X += 160f;
				tmpButtonRec.X += 160;
			}
			else
			{
				tmpTextPos.X += 38f;
				tmpTextPos.Y += 4f;
			}
			spriteBatch.Draw(bButton, tmpButtonRec, buttonColor);
			spriteBatch.DrawString(defaultFont, "Back", tmpTextPos, buttonColor, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
		}
		if (drawReady)
		{
			tmpTextPos.X += 160f;
			tmpButtonRec.X += 160;
			spriteBatch.Draw(xButton, tmpButtonRec, buttonColor);
			spriteBatch.DrawString(defaultFont, "Ready", tmpTextPos, buttonColor, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
		}
	}

	public static void DrawRespawnButton(Viewport view)
	{
		tmpTextPos.X = view.TitleSafeArea.Center.X - 180;
		tmpTextPos.Y = view.TitleSafeArea.Bottom - 36;
		tmpButtonRec.X = (int)tmpTextPos.X + 110;
		tmpButtonRec.Y = (int)tmpTextPos.Y - 2;
		tmpButtonRec.Width = 38;
		tmpButtonRec.Height = 38;
		spriteBatch.Draw(xButton, tmpButtonRec, Color.White);
		Vector2 vector = default(Vector2);
		vector = new Vector2(-3f, -3f);
		spriteBatch.DrawString(defaultFont, "PRESS", tmpTextPos, Color.Black, 0f, vector, 1f, SpriteEffects.None, 0);
		spriteBatch.DrawString(defaultFont, "PRESS", tmpTextPos, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
		tmpTextPos.X += 160f;
		spriteBatch.DrawString(defaultFont, "TO SPAWN", tmpTextPos, Color.Black, 0f, vector, 1f, SpriteEffects.None, 0);
		spriteBatch.DrawString(defaultFont, "TO SPAWN", tmpTextPos, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
	}

	public static void DrawEnterLoadoutButton(Viewport view, string msg)
	{
		tmpTextPos.X = 560f;
		tmpTextPos.Y = 600f;
		tmpButtonRec.X = (int)tmpTextPos.X + 110;
		tmpButtonRec.Y = (int)tmpTextPos.Y;
		tmpButtonRec.Width = 32;
		tmpButtonRec.Height = 32;
		spriteBatch.Draw(xButton, tmpButtonRec, Color.White);
		Vector2 vector = default(Vector2);
		vector = new Vector2(-3f, -3f);
		spriteBatch.DrawString(defaultFont, "PRESS", tmpTextPos, Color.Black, 0f, vector, 1f, SpriteEffects.None, 0);
		spriteBatch.DrawString(defaultFont, "PRESS", tmpTextPos, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
		tmpTextPos.X += 160f;
		spriteBatch.DrawString(defaultFont, msg, tmpTextPos, Color.Black, 0f, vector, 1f, SpriteEffects.None, 0);
		spriteBatch.DrawString(defaultFont, msg, tmpTextPos, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
	}

	public static void DrawButton(Rectangle rec, Buttons? button, Color clr)
	{
		if (!button.HasValue)
		{
			spriteBatch.Draw(dPad, rec, clr);
		}
		else if (button == Buttons.A)
		{
			spriteBatch.Draw(aButton, rec, clr);
		}
		else if (button == Buttons.B)
		{
			spriteBatch.Draw(bButton, rec, clr);
		}
		else if (button == Buttons.X)
		{
			spriteBatch.Draw(xButton, rec, clr);
		}
		else if (button == Buttons.Y)
		{
			spriteBatch.Draw(yButton, rec, clr);
		}
		else if (button == Buttons.DPadUp)
		{
			spriteBatch.Draw(dpUp, rec, clr);
		}
		else if (button == Buttons.DPadDown)
		{
			spriteBatch.Draw(dpDown, rec, clr);
		}
		else if (button == Buttons.DPadRight)
		{
			spriteBatch.Draw(dpRight, rec, clr);
		}
		else if (button == Buttons.DPadLeft)
		{
			spriteBatch.Draw(dpLeft, rec, clr);
		}
		else if (button == Buttons.RightStick)
		{
			spriteBatch.Draw(rightStick, rec, clr);
		}
		else if (button == Buttons.RightTrigger)
		{
			spriteBatch.Draw(rightTrigger, rec, clr);
		}
		else if (button == Buttons.LeftStick)
		{
			spriteBatch.Draw(leftStick, rec, clr);
		}
		else if (button == Buttons.LeftTrigger)
		{
			spriteBatch.Draw(leftTrigger, rec, clr);
		}
		else if (button == Buttons.RightShoulder)
		{
			spriteBatch.Draw(rightShoulder, rec, clr);
		}
		else if (button == Buttons.LeftShoulder)
		{
			spriteBatch.Draw(leftShoulder, rec, clr);
		}
		else if (button == Buttons.Start)
		{
			spriteBatch.Draw(startButton, rec, clr);
		}
		else if (button == Buttons.Back)
		{
			spriteBatch.Draw(backButton, rec, clr);
		}
	}

	public static void DrawBuyWeaponButton(Viewport view, string text, Color color, bool drawButton)
	{
		tmpTextPos.X = view.TitleSafeArea.Center.X;
		tmpTextPos.Y = view.TitleSafeArea.Center.Y + 48;
		if (drawButton)
		{
			tmpTextPos.X -= defaultFont.MeasureString(text).X * 0.5f + 74f;
			tmpButtonRec.X = (int)tmpTextPos.X + 110;
			tmpButtonRec.Y = (int)tmpTextPos.Y - 2;
			tmpButtonRec.Width = 38;
			tmpButtonRec.Height = 38;
		}
		else
		{
			tmpTextPos.X -= defaultFont.MeasureString(text).X * 0.5f;
		}
		spriteBatch.Begin();
		if (drawButton)
		{
			spriteBatch.Draw(xButton, tmpButtonRec, Color.White);
			Vector2 vector = default(Vector2);
			vector = new Vector2(-3f, -3f);
			spriteBatch.DrawString(defaultFont, "PRESS", tmpTextPos, Color.Black, 0f, vector, 1f, SpriteEffects.None, 0);
			spriteBatch.DrawString(defaultFont, "PRESS", tmpTextPos, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
			tmpTextPos.X += 160f;
			spriteBatch.DrawString(defaultFont, text, tmpTextPos, Color.Black, 0f, vector, 1f, SpriteEffects.None, 0);
			spriteBatch.DrawString(defaultFont, text, tmpTextPos, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
		}
		else
		{
			Vector2 vector2 = default(Vector2);
			spriteBatch.DrawString(f: new Vector2(-3f, -3f), a: defaultFont, b: text, c: tmpTextPos, d: Color.Black, e: 0f, g: 1f, h: SpriteEffects.None, i: 0);
			spriteBatch.DrawString(defaultFont, text, tmpTextPos, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
		}
		spriteBatch.End();
	}

	public static void PlaySelect()
	{
		EndGameEngine.SoundBnk.GetCue("MenuSelect").Play();
	}

	public static void PlayQuickSelect()
	{
		EndGameEngine.SoundBnk.GetCue("MenuQuickSelect").Play();
	}

	public static void PlayInvalidSelect()
	{
		EndGameEngine.SoundBnk.GetCue("MenuInvalidSelect").Play();
	}

	public static void PlayMusic(BackgroundMusic e)
	{
	}

	public static void StopMusic(BackgroundMusic e)
	{
		if (e == BackgroundMusic.Start)
		{
			if (!levelBackgroundMusic.IsPaused)
			{
				levelBackgroundMusic.Pause();
			}
			if (!menuMusic.IsPaused)
			{
				menuMusic.Pause();
			}
		}
	}

	public static void UpdateVolume()
	{
		SetVolume(BackgroundMusic.LevelBackground, gameSetting);
	}

	public static void SetVolume(BackgroundMusic e, float v)
	{
	}

	static Menu()
	{
		initialized = false;
		HackSetViewPort = false;
		ActivePlayer = null;
		backgraoundTexture = null;
		logoTexture = null;
		menuMusic = null;
		levelBackgroundMusic = null;
		levelAmbiencedMusic = null;
		BackDelegateEntry = new MenuEntry();
		BACK_DELEGATE_ENTRY = "12XF-3G9O-479E-FGX4";
		tmpTextPos = Vector2.Zero;
		tmpButtonRec = default(Rectangle);
		AllMusicVolume = 150000f;
		LevelMusicVolume = 1500f;
		AmbientMusicVolume = 250000f;
		gameSetting = 1f;
	}
}
