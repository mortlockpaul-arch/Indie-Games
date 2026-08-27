using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class MediaEmitterClass
{
	private static int MaxMediaEmitters = 16;

	private static MediaStruct[] list = new MediaStruct[MaxMediaEmitters];

	private static Color btColor = new Color(200, 200, 200, 255);

	private static Color btShade = Color.Black;

	private static Vector2 btOffset = new Vector2(-3f, -3f);

	public static void LoadContent()
	{
		for (int i = 0; i < MaxMediaEmitters; i++)
		{
			list[i].Render = true;
			list[i].Flags = 0u;
		}
	}

	public static void Add(ref MediaStruct e)
	{
		for (int i = 0; i < MaxMediaEmitters; i++)
		{
			if (list[i].Flags == 0)
			{
				list[i].Flags = e.Flags;
				if ((list[i].Flags & 1) != 0)
				{
					list[i].BallonText = e.BallonText;
					list[i].Position.X = 640f;
					list[i].Position.Y = 320f;
					list[i].Position3D = e.Position3D;
					list[i].Direction.X = ((float)EndGameEngine.randGenerator.NextDouble() - 0.5f) * 2f;
					list[i].Direction.Y = -2f;
				}
				if ((list[i].Flags & 8) != 0)
				{
					list[i].BallonText = e.BallonText;
					list[i].Position.X = 640f;
					list[i].Position.Y = 320f;
					list[i].Position3D = e.Position3D;
					list[i].Direction.X = ((float)EndGameEngine.randGenerator.NextDouble() - 0.5f) * 2f;
					list[i].Direction.Y = ((float)EndGameEngine.randGenerator.NextDouble() - 0.5f) * 2f;
				}
				list[i].Offset.X = Menu.defaultFont.MeasureString(e.BallonText).X * 0.5f;
				list[i].SoundName = e.SoundName;
				list[i].Scale = 0.25f;
				list[i].Timer = e.Timer;
				list[i].TimerDelay = e.TimerDelay;
				break;
			}
		}
	}

	public static void Update(float eTimeMS, int qIndex)
	{
		for (int i = 0; i < MaxMediaEmitters; i++)
		{
			if (list[i].Flags == 0)
			{
				continue;
			}
			if ((list[i].Flags & 4) != 0)
			{
				list[i].TimerDelay -= eTimeMS;
				if (list[i].TimerDelay <= 0f)
				{
					list[i].Flags &= 4294967291u;
				}
				continue;
			}
			if ((list[i].Flags & 1) != 0)
			{
				list[i].Position += list[i].Direction;
				list[i].Scale += 0.01f;
			}
			if ((list[i].Flags & 8) != 0)
			{
				list[i].Position3D.X += list[i].Direction.X;
				list[i].Position3D.Z += list[i].Direction.Y;
				list[i].Position3D.Y += 1.8f;
				list[i].Scale += 0.01f;
				list[i].Render = math.Position3DToScreen2D(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value], ref list[i].Position3D, ref list[i].Position, qIndex);
			}
			if ((list[i].Flags & 2) != 0)
			{
				list[i].Flags &= 4294967293u;
				if (!list[i].SoundCue.IsDisposed)
				{
					list[i].SoundCue.Stop(AudioStopOptions.Immediate);
					list[i].SoundCue.Dispose();
				}
				list[i].SoundCue = EndGameEngine.SoundBnk.GetCue(list[i].SoundName);
				list[i].SoundCue.Play();
			}
			list[i].Timer -= eTimeMS;
			if (list[i].Timer <= 0f)
			{
				list[i].Flags = 0u;
			}
		}
	}

	public static void Draw(int qIndex)
	{
		Menu.spriteBatch.Begin();
		for (int i = 0; i < MaxMediaEmitters; i++)
		{
			if (list[i].Flags != 0 && list[i].Render && (list[i].Flags & 4) == 0 && ((list[i].Flags & 1) != 0 || (list[i].Flags & 8) != 0))
			{
				float num = ((list[i].Timer > 1f) ? 1f : list[i].Timer);
				btColor.A = (byte)(num * 255f);
				btColor.R = (byte)(num * 200f);
				btColor.G = (byte)(num * 200f);
				btColor.B = (byte)(num * 200f);
				btShade.A = (byte)(num * 255f);
				Menu.spriteBatch.DrawString(Menu.defaultFont, list[i].BallonText, list[i].Position, btShade, 0f, list[i].Offset + btOffset, list[i].Scale, SpriteEffects.None, 0);
				Menu.spriteBatch.DrawString(Menu.defaultFont, list[i].BallonText, list[i].Position, btColor, 0f, list[i].Offset, list[i].Scale, SpriteEffects.None, 0);
			}
		}
		Menu.spriteBatch.End();
	}
}
