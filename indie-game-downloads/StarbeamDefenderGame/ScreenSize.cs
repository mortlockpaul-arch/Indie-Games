using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarbeamDefenderGame;

public class ScreenSize
{
	private Texture2D rectangletexture;

	private Texture2D backgroundtexture;

	private float sizemod;

	private float setmod;

	private int keyboarddelay;

	public float GetScreenSizeMod
	{
		get
		{
			return setmod;
		}
		set
		{
			setmod = value;
		}
	}

	public void LoadContent(ContentManager content, string contentrootfolder)
	{
		backgroundtexture = content.Load<Texture2D>(contentrootfolder + "scalebackground");
		rectangletexture = content.Load<Texture2D>(contentrootfolder + "ScreenSizeLayer");
	}

	public void Draw(SpriteBatch spritebatch)
	{
		Rectangle value = new Rectangle(0, 0, 1280, 720);
		spritebatch.Draw(destinationRectangle: new Rectangle(0, 0, 1280, 720), texture: backgroundtexture, sourceRectangle: value, color: Color.White);
		int num = (int)(1280f * sizemod);
		int num2 = (int)(720f * sizemod);
		value = new Rectangle(0, 0, 1280, 720);
		spritebatch.Draw(destinationRectangle: new Rectangle((1280 - num) / 2, (720 - num2) / 2, num, num2), texture: rectangletexture, sourceRectangle: value, color: Color.White);
	}

	public int DoGameUpdate(int timems, PlayerIndex controller)
	{
		keyboarddelay -= timems;
		if (keyboarddelay > 0)
		{
			return 0;
		}
		keyboarddelay = 0;
		GamePadState state = GamePad.GetState(controller);
		Vector2 vector = ((!((state.ThumbSticks.Left.X != 0f) | (state.ThumbSticks.Left.Y != 0f))) ? state.ThumbSticks.Right : state.ThumbSticks.Left);
		if (vector.X < -0.8f)
		{
			sizemod -= 0.01f;
			if (sizemod < 0.8f)
			{
				sizemod = 0.8f;
			}
			keyboarddelay = 50;
		}
		if (vector.X > 0.8f)
		{
			sizemod += 0.01f;
			if (sizemod > 1f)
			{
				sizemod = 1f;
			}
			keyboarddelay = 50;
		}
		if (vector.Y > 0.8f)
		{
			sizemod -= 0.01f;
			if (sizemod < 0.8f)
			{
				sizemod = 0.8f;
			}
			keyboarddelay = 50;
		}
		if (vector.Y < -0.8f)
		{
			sizemod += 0.01f;
			if (sizemod > 1f)
			{
				sizemod = 1f;
			}
			keyboarddelay = 50;
		}
		KeyboardState state2 = Keyboard.GetState();
		if (state.IsButtonDown(Buttons.A) | state2.IsKeyDown(Keys.Enter))
		{
			setmod = sizemod;
			return 1;
		}
		if (state2.IsKeyDown(Keys.Up))
		{
			sizemod += 0.01f;
			if (sizemod > 1f)
			{
				sizemod = 1f;
			}
			keyboarddelay = 50;
		}
		if (state2.IsKeyDown(Keys.Down))
		{
			sizemod -= 0.01f;
			if (sizemod < 0.8f)
			{
				sizemod = 0.8f;
			}
			keyboarddelay = 50;
		}
		return 0;
	}

	public void Reset()
	{
		keyboarddelay = 250;
	}

	public void Setup(float currentsizemod)
	{
		sizemod = currentsizemod;
		setmod = 1f;
		Reset();
	}
}
