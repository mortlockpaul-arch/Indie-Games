using Microsoft.Xna.Framework;

namespace EGEngine;

public class MessagePumpBase
{
	private static int MAX_MESSAGES = 16;

	private static int queueHead = 0;

	private static int queueTail = 0;

	private static MessageItem[] massageQueue = new MessageItem[MAX_MESSAGES];

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

	public virtual void Flush()
	{
	}

	public virtual void AddGamerMessage(string msg0, string msg1, string msg2, Color msgColor0, Color msgColor2)
	{
	}

	public virtual void Update(float eTime)
	{
	}

	public virtual void Draw()
	{
	}
}
