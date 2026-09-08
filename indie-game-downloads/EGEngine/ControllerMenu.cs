using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class ControllerMenu : Menu
{
	public static float ControllerSensitivityHigh = 2.25f;

	public static float ControllerSensitivityMid = 1.125f;

	public static float ControllerSensitivityLow = 0.75f;

	private MenuEntry ResumeMenu;

	private MenuEntry ExitMatchMenu;

	private Rectangle texRec;

	private Color shadow;

	private Color diffuse;

	public ControllerMenu(GameMenus id)
		: base(id)
	{
		ResumeMenu = new MenuEntry();
		ExitMatchMenu = new MenuEntry();
		texRec = new Rectangle(200, 88, 880, 500);
		shadow = Color.Black;
		diffuse = Color.Black;
	}

	public override void LoadContent()
	{
		base.LoadContent();
		defaultBackground = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\controller_layout");
		SetupControllerMenu();
	}

	public override void Update(float eTime)
	{
		base.Update(eTime);
		PlayerBase playerBase = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
		if (menuEntryList[1].isSelected)
		{
			if (LevelBaseMenu.InputUpdate.menuInputContinuos == MenuInput.MenuRight)
			{
				playerBase.PlayerControllerSensitivity = ((playerBase.PlayerControllerSensitivity + 0.007f < ControllerSensitivityHigh) ? (playerBase.PlayerControllerSensitivity + 0.007f) : ControllerSensitivityHigh);
			}
			else if (LevelBaseMenu.InputUpdate.menuInputContinuos == MenuInput.MenuLeft)
			{
				playerBase.PlayerControllerSensitivity = ((playerBase.PlayerControllerSensitivity - 0.007f > ControllerSensitivityLow) ? (playerBase.PlayerControllerSensitivity - 0.007f) : ControllerSensitivityLow);
			}
		}
	}

	public override void Draw()
	{
		_ = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport;
		EndGameEngine.GraphicMgr.GraphicsDevice.Viewport = EndGameEngine.DefualtViewport;
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
		Rectangle rectangle = default(Rectangle);
		rectangle = new Rectangle(EndGameEngine.DefualtViewport.TitleSafeArea.X, EndGameEngine.DefualtViewport.TitleSafeArea.Y, EndGameEngine.DefualtViewport.TitleSafeArea.Width, height);
		Rectangle rectangle2 = default(Rectangle);
		Menu.spriteBatch.Draw(s: new Rectangle(4, 4, Menu.titleTexture.Width - 8, Menu.titleTexture.Height - 8), t: Menu.titleTexture, d: rectangle, c: bgTextureColor);
		DrawButtonControl(EndGameEngine.GraphicMgr.GraphicsDevice.Viewport, drawSelect: false, drawBack: true, drawReady: false);
		bgTextureColor.R = (byte)(255f * transitionDelta);
		bgTextureColor.G = (byte)(255f * transitionDelta);
		bgTextureColor.B = (byte)(255f * transitionDelta);
		bgTextureColor.A = (byte)(255f * transitionDelta);
		Rectangle rectangle3 = default(Rectangle);
		rectangle3 = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea;
		rectangle3.Y -= 32;
		rectangle3.Height = 512;
		Menu.spriteBatch.Draw(SurvivalGuideMenu.ControllerTex, rectangle3, bgTextureColor);
		DrawInvertY();
		DrawSensitivity();
		Menu.spriteBatch.End();
		base.Draw();
	}

	private void DrawAimAssist()
	{
		int index = 0;
		Vector2 zero = Vector2.Zero;
		zero = menuEntryList[index].position + menuEntryList[index].textOffset;
		zero.X += 160f;
		zero.Y += 2f;
		string s = " : Off";
		if (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].AimAssist)
		{
			s = " : On";
		}
		ref Color reference = ref shadow;
		ref Color reference2 = ref shadow;
		byte b = (shadow.B = 0);
		byte b3 = b;
		b = (reference2.G = b3);
		byte r = b;
		reference.R = r;
		shadow.A = (byte)(255f * transitionDelta);
		Menu.spriteBatch.DrawString(Menu.defaultFont, s, zero, shadow);
		zero.X -= 2f;
		zero.Y -= 2f;
		if (menuEntryList[index].isSelected)
		{
			ref Color reference3 = ref diffuse;
			ref Color reference4 = ref diffuse;
			ref Color reference5 = ref diffuse;
			b = (diffuse.A = (byte)(255f * transitionDelta));
			byte b6 = b;
			b = (reference5.B = b6);
			byte b8 = b;
			b = (reference4.G = b8);
			byte r2 = b;
			reference3.R = r2;
		}
		else
		{
			ref Color reference6 = ref diffuse;
			ref Color reference7 = ref diffuse;
			b = (diffuse.B = (byte)(169f * transitionDelta));
			byte b11 = b;
			b = (reference7.G = b11);
			byte r3 = b;
			reference6.R = r3;
			diffuse.A = (byte)(169f * transitionDelta);
		}
		Menu.spriteBatch.DrawString(Menu.defaultFont, s, zero, diffuse);
	}

	private void DrawInvertY()
	{
		int index = 0;
		Vector2 zero = Vector2.Zero;
		zero = menuEntryList[index].position + menuEntryList[index].textOffset;
		zero.X += 160f;
		zero.Y += 2f;
		string s = " : Not Inverted";
		if (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].InvertY < 0f)
		{
			s = " : Inverted";
		}
		ref Color reference = ref shadow;
		ref Color reference2 = ref shadow;
		byte b = (shadow.B = 0);
		byte b3 = b;
		b = (reference2.G = b3);
		byte r = b;
		reference.R = r;
		shadow.A = (byte)(255f * transitionDelta);
		Menu.spriteBatch.DrawString(Menu.defaultFont, s, zero, shadow);
		zero.X -= 2f;
		zero.Y -= 2f;
		if (menuEntryList[index].isSelected)
		{
			ref Color reference3 = ref diffuse;
			ref Color reference4 = ref diffuse;
			ref Color reference5 = ref diffuse;
			b = (diffuse.A = (byte)(255f * transitionDelta));
			byte b6 = b;
			b = (reference5.B = b6);
			byte b8 = b;
			b = (reference4.G = b8);
			byte r2 = b;
			reference3.R = r2;
		}
		else
		{
			ref Color reference6 = ref diffuse;
			ref Color reference7 = ref diffuse;
			b = (diffuse.B = (byte)(169f * transitionDelta));
			byte b11 = b;
			b = (reference7.G = b11);
			byte r3 = b;
			reference6.R = r3;
			diffuse.A = (byte)(169f * transitionDelta);
		}
		Menu.spriteBatch.DrawString(Menu.defaultFont, s, zero, diffuse);
	}

	private void DrawSensitivity()
	{
		int index = 1;
		Vector2 zero = Vector2.Zero;
		zero = menuEntryList[index].position + menuEntryList[index].textOffset;
		zero.X += 176f;
		zero.Y += 2f;
		Rectangle rectangle = default(Rectangle);
		rectangle = new Rectangle(0, 0, 200, (int)menuEntryList[index].textHeight - 8);
		Rectangle rectangle2 = default(Rectangle);
		rectangle2 = new Rectangle(0, 0, 0, (int)menuEntryList[index].textHeight - 12);
		rectangle.X = (int)zero.X;
		rectangle.Y = (int)zero.Y + 4;
		rectangle2.X = (int)zero.X + 2;
		rectangle2.Y = (int)zero.Y + 6;
		rectangle2.Width += (int)((LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].PlayerControllerSensitivity - ControllerSensitivityLow) * 200f);
		rectangle2.Width -= 4;
		if (menuEntryList[index].isSelected)
		{
			ref Color reference = ref diffuse;
			ref Color reference2 = ref diffuse;
			ref Color reference3 = ref diffuse;
			byte b = (diffuse.A = (byte)(255f * transitionDelta));
			byte b3 = b;
			b = (reference3.B = b3);
			byte b5 = b;
			b = (reference2.G = b5);
			byte r = b;
			reference.R = r;
		}
		else
		{
			ref Color reference4 = ref diffuse;
			ref Color reference5 = ref diffuse;
			ref Color reference6 = ref diffuse;
			byte b = (diffuse.A = (byte)(100f * transitionDelta));
			byte b8 = b;
			b = (reference6.B = b8);
			byte b10 = b;
			b = (reference5.G = b10);
			byte r2 = b;
			reference4.R = r2;
		}
		Menu.spriteBatch.Draw(LevelBaseMenu.texBlack, rectangle, diffuse);
		Menu.spriteBatch.Draw(LevelBaseMenu.texBrown, rectangle2, diffuse);
	}

	private void DrawControllerMenu()
	{
	}

	public override void DrawBackground()
	{
	}

	public override void MakeActive(MenuMgr e)
	{
		base.MakeActive(e);
	}

	private void SetupControllerMenu()
	{
		new MenuEntry();
		MenuEntry menuEntry = new MenuEntry();
		MenuEntry menuEntry2 = new MenuEntry();
		Vector2 zero = Vector2.Zero;
		zero.X = Menu.titleSafeArea.Center.X - 180;
		zero.Y = Menu.titleSafeArea.Bottom - 100;
		menuEntryList.Add(menuEntry.Set("Invert Y", MenuTextJustify.Left, zero, InvertYFunc, EndGameEngine.GameAssetMgr));
		menuEntry.isSelected = true;
		zero.Y += menuEntry.textHeight;
		menuEntryList.Add(menuEntry2.Set("Sensitivity", MenuTextJustify.Left, zero, SensitivityFunc, EndGameEngine.GameAssetMgr));
		menuEntry2.isSelected = true;
		zero.Y += menuEntry2.textHeight;
	}

	private void AimnAssistFunc(object sender, MenuEntry e)
	{
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].AimAssist = !LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].AimAssist;
	}

	private void InvertYFunc(object sender, MenuEntry e)
	{
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].InvertY = ((LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].InvertY > 0f) ? (-1f) : 1f);
	}

	private void SensitivityFunc(object sender, MenuEntry e)
	{
	}
}
