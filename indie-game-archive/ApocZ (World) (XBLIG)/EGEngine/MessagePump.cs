using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class MessagePump
{
	private static int MAX_MESSAGES = 16;

	private static int queueHead = 0;

	private static int queueTail = 0;

	private static MessageItem[] massageQueue = new MessageItem[MAX_MESSAGES];

	private static Vector2 msgPosition;

	private static Vector2 msgShadowPos = new Vector2(1f, 1f);

	private static Vector2 msgPos1 = new Vector2(-180f, 0f);

	private static Vector2 msgPos2 = new Vector2(-320f, 0f);

	private static GraphicsDevice tmpDevice;

	public static void Flush()
	{
		for (int i = 0; i < MAX_MESSAGES; i++)
		{
			massageQueue[i].timer = -1f;
		}
	}

	public static void AddGamerMessage(string msg0, string msg1, string msg2, Color msgColor0, Color msgColor2)
	{
		massageQueue[queueTail].timer = 12f;
		massageQueue[queueTail].message = null;
		massageQueue[queueTail].msgOffset1 = Menu.defaultFont.MeasureString(msg0).X * 1f;
		massageQueue[queueTail].msgOffset2 = Menu.defaultFont.MeasureString(msg1).X * 1f + massageQueue[queueTail].msgOffset1;
		massageQueue[queueTail].msgColor0 = msgColor0;
		massageQueue[queueTail].msgColor2 = msgColor2;
		massageQueue[queueTail].msgColor0.A = 100;
		massageQueue[queueTail].msgColor2.A = 100;
		massageQueue[queueTail].msgPart0 = msg0;
		massageQueue[queueTail].msgPart1 = msg1;
		massageQueue[queueTail].msgPart2 = msg2;
		queueTail++;
		if (queueTail >= MAX_MESSAGES)
		{
			queueTail = 0;
		}
	}

	public static void AddMessage(string msg)
	{
		if (msg.Contains("NullReference"))
		{
			massageQueue[queueTail].timer = 12f;
		}
		massageQueue[queueTail].timer = 12f;
		massageQueue[queueTail].message = msg;
		queueTail++;
		if (queueTail >= MAX_MESSAGES)
		{
			queueTail = 0;
		}
	}

	public static void Update(float eTime)
	{
		int num = queueHead;
		for (int i = 0; i < MAX_MESSAGES; i++)
		{
			massageQueue[num].timer -= eTime;
			num++;
			if (num >= MAX_MESSAGES)
			{
				num = 0;
			}
		}
		if (massageQueue[queueHead].timer < 0f)
		{
			queueHead = ((queueHead + 1 < MAX_MESSAGES) ? (queueHead + 1) : 0);
		}
	}

	public static void Draw()
	{
		float g = 0.65f;
		if (LevelBaseMenu.LoadState == LevelLoadState.Loading)
		{
			return;
		}
		int num = queueHead;
		tmpDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
		msgPosition.X = tmpDevice.Viewport.TitleSafeArea.Left;
		msgPosition.Y = tmpDevice.Viewport.TitleSafeArea.Top;
		Menu.spriteBatch.Begin();
		for (int i = 0; i < MAX_MESSAGES; i++)
		{
			if (massageQueue[num].timer > 0f)
			{
				if (massageQueue[num].message != null)
				{
					Menu.spriteBatch.DrawString(Menu.defaultFont, massageQueue[num].message, msgPosition + msgShadowPos, Color.Black, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
					Menu.spriteBatch.DrawString(Menu.defaultFont, massageQueue[num].message, msgPosition, Color.LightGray, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
				}
				else if (massageQueue[num].msgPart0 != null)
				{
					msgPos1.X = 0f - massageQueue[num].msgOffset1;
					msgPos2.X = 0f - massageQueue[num].msgOffset2;
					Menu.spriteBatch.DrawString(Menu.defaultFont, massageQueue[num].msgPart0, msgPosition, massageQueue[num].msgColor0, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
					Menu.spriteBatch.DrawString(Menu.defaultFont, massageQueue[num].msgPart1, msgPosition, Color.LightGray, 0f, msgPos1, g, SpriteEffects.None, 0);
					Menu.spriteBatch.DrawString(Menu.defaultFont, massageQueue[num].msgPart2, msgPosition, massageQueue[num].msgColor2, 0f, msgPos2, g, SpriteEffects.None, 0);
				}
				msgPosition.Y += 16f;
			}
			num++;
			if (num >= MAX_MESSAGES)
			{
				num = 0;
			}
		}
		Menu.spriteBatch.End();
	}
}
