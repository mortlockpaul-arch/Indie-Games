using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class ConfirmMessage
{
	public const string CONFIRM_MESSAGE_EXECUTE = "ConfirmMessage";

	private static string message = "";

	public static bool ResetDelegate = true;

	public static bool valid = false;

	public static Vector2 TextOffset = Vector2.Zero;

	public static MenuEntry ExecuteDelegateEntry = new MenuEntry();

	public static event EventHandler<MenuEntry> ExecuteDelegate = null;

	public static bool IsExecuteDelegateNull()
	{
		return ExecuteDelegate == null;
	}

	public static void ExecuteBackDelegate()
	{
		if (!IsExecuteDelegateNull())
		{
			ExecuteDelegateEntry.text = "ConfirmMessage";
			ExecuteDelegate(null, ExecuteDelegateEntry);
			ExecuteDelegate = null;
		}
	}

	public static void AddMessage(string msg, EventHandler<MenuEntry> executeFunc)
	{
		message = msg;
		TextOffset = Menu.defaultFont.MeasureString(message) * 0.5f * 0.9f;
		valid = true;
		ExecuteDelegate += executeFunc;
	}

	public static bool Update(float eTime)
	{
		if (valid)
		{
			if (LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuSelect)
			{
				valid = false;
				ExecuteBackDelegate();
			}
			else if (LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuBack)
			{
				valid = false;
			}
			LevelBaseMenu.InputUpdate.menuInput = MenuInput.None;
			LevelBaseMenu.InputUpdate.menuInputContinuos = MenuInput.None;
			LevelBaseMenu.InputUpdate.menuInputRightStick = MenuInput.None;
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
			int num = graphicsDevice.Viewport.TitleSafeArea.Center.X - 256;
			int num2 = graphicsDevice.Viewport.TitleSafeArea.Center.Y - 64;
			titleSafeArea.X = num;
			titleSafeArea.Y = num2;
			titleSafeArea.Width = 512;
			titleSafeArea.Height = 128;
			zero.X = (float)graphicsDevice.Viewport.TitleSafeArea.Center.X - Menu.defaultFont.MeasureString(message).X * 0.5f;
			zero.Y = titleSafeArea.Y + 24;
			Menu.spriteBatch.Begin();
			Menu.spriteBatch.Draw(LevelBaseMenu.texBlack, titleSafeArea, new Color(0, 0, 0, 220));
			Menu.spriteBatch.DrawString(Menu.defaultFont, message, zero, Color.LightGray);
			titleSafeArea.Width = 38;
			titleSafeArea.Height = 38;
			titleSafeArea.X = num + 64;
			titleSafeArea.Y = num2 + 80;
			Menu.spriteBatch.Draw(Menu.aButton, titleSafeArea, Color.White);
			zero.X = titleSafeArea.X + 42;
			zero.Y = titleSafeArea.Y - 2;
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Yes", zero, Color.LightGray);
			titleSafeArea.X = num + 364;
			Menu.spriteBatch.Draw(Menu.bButton, titleSafeArea, Color.White);
			zero.X = titleSafeArea.X + 42;
			Menu.spriteBatch.DrawString(Menu.defaultFont, "No", zero, Color.LightGray);
			Menu.spriteBatch.End();
		}
	}
}
