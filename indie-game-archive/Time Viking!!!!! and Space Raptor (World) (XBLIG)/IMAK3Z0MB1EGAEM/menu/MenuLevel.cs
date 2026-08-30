using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Viking_x86;

namespace IMAK3Z0MB1EGAEM.menu;

public class MenuLevel
{
	public string[] item;

	public int sel;

	public string title;

	private GamePadState pgs;

	public int grace;

	public virtual void Update(int idx)
	{
		GamePadState state = GamePad.GetState((PlayerIndex)idx);
		if (grace <= 0)
		{
			if ((state.Buttons.Start == ButtonState.Pressed && pgs.Buttons.Start == ButtonState.Released) || (state.Buttons.A == ButtonState.Pressed && pgs.Buttons.A == ButtonState.Released))
			{
				Accept();
			}
			if (state.Buttons.B == ButtonState.Pressed && pgs.Buttons.B == ButtonState.Released)
			{
				Cancel();
			}
			if ((state.ThumbSticks.Left.Y > 0.3f && pgs.ThumbSticks.Left.Y <= 0.3f) || (state.DPad.Up == ButtonState.Pressed && pgs.DPad.Up == ButtonState.Released))
			{
				sel = (sel + (item.Length - 1)) % item.Length;
			}
			if ((state.ThumbSticks.Left.Y < -0.3f && pgs.ThumbSticks.Left.Y >= -0.3f) || (state.DPad.Down == ButtonState.Pressed && pgs.DPad.Down == ButtonState.Released))
			{
				sel = (sel + 1) % item.Length;
			}
		}
		else
		{
			grace--;
		}
		pgs = state;
	}

	public virtual void Draw(Vector2 orig)
	{
		orig = new Vector2(640f, 150f);
		Text.DrawString(title, orig, 11f, Color.Gray, Text.Justify.Center);
		for (int i = 0; i < item.Length; i++)
		{
			Text.DrawString(item[i], orig + new Vector2(0f, (float)(i + 2) * 40f), 6f, (sel == i) ? new Color(Rand.GetRandomFloat(0.3f, 1f), Rand.GetRandomFloat(0.3f, 1f), Rand.GetRandomFloat(0.3f, 1f), 1f) : Color.White, Text.Justify.Center);
		}
	}

	public virtual void Accept()
	{
	}

	public virtual void Cancel()
	{
	}
}
