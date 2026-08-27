using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class LevelObjectives : EventArgs
{
	private static LevelObjectives objRef = new LevelObjectives();

	public string objMsg = "";

	public bool objActive;

	public float objLife;

	public int objCallbackMsg;

	private static Color tmpColor = Color.Black;

	private static Color tmpShadow = Color.Black;

	private static Vector2 msgPos = Vector2.Zero;

	public event EventHandler<LevelObjectives> objCallbackFunc;

	public static void Clear()
	{
		objRef.objActive = false;
		objRef.objLife = 0f;
	}

	public static void Add(string msg, float life, EventHandler<LevelObjectives> objhandler)
	{
		objRef.objActive = true;
		objRef.objMsg = msg;
		objRef.objLife = life;
		objRef.objCallbackMsg = 0;
		objRef.objCallbackFunc = objhandler;
	}

	public static void IssueCallbackFunc(int msgType)
	{
		if (objRef.objActive)
		{
			objRef.objCallbackMsg = msgType;
			objRef.objCallbackFunc(null, objRef);
		}
	}

	public static void Update()
	{
		if (objRef.objActive)
		{
			objRef.objLife -= 0.03f;
			if (objRef.objLife <= 0f)
			{
				objRef.objCallbackMsg = 1;
				objRef.objCallbackFunc(null, objRef);
			}
		}
	}

	public static void DrawPost(int qIndex, PlayerBase playerRef)
	{
		if (objRef.objActive)
		{
			Viewport viewport = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport;
			if (objRef.objLife > 0f)
			{
				msgPos.X = viewport.TitleSafeArea.Left;
				msgPos.Y = viewport.TitleSafeArea.Top;
				Menu.spriteBatch.Begin();
				tmpColor.R = 240;
				tmpColor.G = 0;
				tmpColor.B = 0;
				tmpColor.A = byte.MaxValue;
				Menu.spriteBatch.DrawString(Menu.defaultFont, "OBJECTIVE:", msgPos, tmpShadow, 0f, new Vector2(-3f, -3f), 0.95f, SpriteEffects.None, 0);
				Menu.spriteBatch.DrawString(Menu.defaultFont, "OBJECTIVE: ", msgPos, tmpColor, 0f, Vector2.Zero, 0.95f, SpriteEffects.None, 0);
				tmpColor.R = 211;
				tmpColor.G = 211;
				tmpColor.B = 211;
				tmpColor.A = byte.MaxValue;
				msgPos.X += Menu.defaultFont.MeasureString("OBJECTIVE: ").X;
				Menu.spriteBatch.DrawString(Menu.defaultFont, objRef.objMsg, msgPos, tmpShadow, 0f, new Vector2(-2f, -2f), 0.95f, SpriteEffects.None, 0);
				Menu.spriteBatch.DrawString(Menu.defaultFont, objRef.objMsg, msgPos, tmpColor, 0f, Vector2.Zero, 0.95f, SpriteEffects.None, 0);
				Menu.spriteBatch.End();
			}
		}
	}
}
