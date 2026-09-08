using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Deep_waters;

public class ScreenFlashesTextures
{
	private Texture2D text;

	private Color currentcolor = new Color(0, 0, 0, 0);

	public bool isfadingin = true;

	private BlendState bstate = BlendState.AlphaBlend;

	private float alphatarget;

	private float speed;

	private float timeonscreen = 1600f;

	private float timepassed;

	private bool hasshown;

	public bool disposeme;

	public ScreenFlashesTextures(Texture2D t, float alpha, float vel, bool isfading, BlendState blendingstate)
	{
		text = t;
		alphatarget = alpha;
		currentcolor.A = 0;
		isfadingin = isfading;
		speed = vel;
		bstate = blendingstate;
	}

	public void Update(GameTime gameTime)
	{
		if ((float)(int)currentcolor.A > alphatarget - 16f * speed)
		{
			timepassed += gameTime.ElapsedGameTime.Milliseconds;
			if (timepassed > timeonscreen)
			{
				isfadingin = false;
			}
		}
		if (isfadingin)
		{
			if ((float)(int)currentcolor.A < alphatarget - 16f * speed)
			{
				currentcolor.A += (byte)((float)gameTime.ElapsedGameTime.Milliseconds * speed);
			}
			else
			{
				hasshown = true;
			}
			currentcolor.R = currentcolor.A;
			currentcolor.B = currentcolor.A;
			currentcolor.G = currentcolor.A;
			return;
		}
		timepassed = 0f;
		if (currentcolor.A > 0)
		{
			currentcolor.A -= (byte)((float)gameTime.ElapsedGameTime.Milliseconds * speed);
		}
		currentcolor.R = currentcolor.A;
		currentcolor.B = currentcolor.A;
		currentcolor.G = currentcolor.A;
		if (hasshown)
		{
			disposeme = true;
		}
	}

	public void Draw(GameTime gameTime, SpriteBatch sb)
	{
		sb.Draw(text, new Rectangle(0, 0, sb.GraphicsDevice.Viewport.Width, sb.GraphicsDevice.Viewport.Height), currentcolor);
	}
}
