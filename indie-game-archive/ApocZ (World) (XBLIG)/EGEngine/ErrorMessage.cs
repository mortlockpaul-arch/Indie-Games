using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EGEngine;

public class ErrorMessage
{
	public static bool valid = false;

	public static bool RevertToMainMenu = false;

	public static bool AttemptSignIn = false;

	private static int maxLineLength = 64;

	private static string message = "";

	public static void AddMessage(string msg, bool backToMain)
	{
		AddMessage(msg, backToMain, atemptSignIn: false);
	}

	public static void AddMessage(string msg, bool backToMain, bool atemptSignIn)
	{
		RevertToMainMenu = backToMain;
		AttemptSignIn = atemptSignIn;
		message = "";
		if (msg.Length > maxLineLength + 5)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int startIndex = 0;
			while (num < msg.Length)
			{
				if (msg[num] == ' ')
				{
					num4 = num;
				}
				num++;
				num3++;
				if (num3 > maxLineLength)
				{
					message = message + msg.Substring(num2, num4 - num2) + "\n";
					num3 = 0;
					num2 = num4;
					startIndex = num2 + (num4 - num2);
				}
			}
			message += msg.Substring(startIndex);
		}
		else
		{
			message = msg;
		}
		valid = true;
		Thread.Sleep(100);
	}

	public static bool Update(float eTime)
	{
		if (valid && LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuSelect)
		{
			valid = false;
			LevelBaseMenu.InputUpdate.menuInput = MenuInput.None;
			LevelBaseMenu.InputUpdate.menuInputContinuos = MenuInput.None;
			LevelBaseMenu.InputUpdate.menuInputRightStick = MenuInput.None;
			GamePad.GetState(EndGameEngine.controllingPlayer.Value);
			LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].lastGamePadState = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].currentGamePadState;
			InputBase.playerGamePad[(int)EndGameEngine.controllingPlayer.Value].lastGamePadState = InputBase.playerGamePad[(int)EndGameEngine.controllingPlayer.Value].currentGamePadState;
			if (AttemptSignIn)
			{
				SignedInGamer signedInGamer = Gamer.SignedInGamers[EndGameEngine.controllingPlayer.Value];
				if ((signedInGamer == null || !signedInGamer.IsSignedInToLive) && !Guide.IsVisible)
				{
					Guide.ShowSignIn(1, onlineOnly: true);
				}
			}
			if (RevertToMainMenu)
			{
				EndGameEngine.menuMgr.MakeActive(GameMenus.MainMenu);
			}
			Thread.Sleep(100);
		}
		return valid;
	}

	public static void Draw()
	{
		if (LevelBaseMenu.LoadState != LevelLoadState.Loading && valid)
		{
			GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
			Vector2 zero = Vector2.Zero;
			Rectangle titleSafeArea = graphicsDevice.Viewport.TitleSafeArea;
			titleSafeArea.X += 64;
			titleSafeArea.Y += 128;
			titleSafeArea.Width -= 128;
			titleSafeArea.Height -= 256;
			zero.X = titleSafeArea.X + 32;
			zero.Y = titleSafeArea.Y + 32;
			Menu.spriteBatch.Begin();
			Menu.spriteBatch.Draw(LevelBaseMenu.texBlack, titleSafeArea, new Color(0, 255, 0, 180));
			Menu.spriteBatch.DrawString(Menu.defaultFont, message, zero, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
			Menu.spriteBatch.Draw(a: new Rectangle(titleSafeArea.X + 412, titleSafeArea.Y + titleSafeArea.Height - 64, 32, 32), t: Menu.aButton, c: Color.White);
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Press        To Continue", new Vector2(titleSafeArea.X + 314, titleSafeArea.Y + titleSafeArea.Height - 64), Color.White);
			Menu.spriteBatch.End();
		}
	}
}
