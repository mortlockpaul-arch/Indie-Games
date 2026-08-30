using Microsoft.Xna.Framework;

namespace Maximinus;

public class FrameRate
{
	public int currentFrame;

	private string instantanous;

	private string worst;

	private string average;

	public float instantanousValue;

	public float averageValue;

	public float worstValue;

	private double TotalMillisecInInterval;

	private static int interval = 20;

	private bool extraInfo;

	private Vector2 position;

	private Drawing2D draw2D;

	private string message
	{
		get
		{
			if (extraInfo)
			{
				return "inst  " + instantanous + Utils.newLine + "avg  " + average + Utils.newLine + "wrst " + worst;
			}
			return "fps " + average;
		}
	}

	public FrameRate(Drawing2D draw2D, bool extraInfo)
		: this(draw2D, extraInfo, Menus.AlignX.Right)
	{
	}

	public FrameRate(Drawing2D draw2D, bool extraInfo, Menus.AlignX alignX)
	{
		this.draw2D = draw2D;
		this.extraInfo = extraInfo;
		currentFrame = 0;
		instantanous = "";
		worst = "";
		average = "";
		worstValue = interval;
		switch (alignX)
		{
		case Menus.AlignX.Right:
			position = new Vector2(draw2D.ScreenSize.X * 0.8f, draw2D.ScreenSize.Y * 0.075f);
			break;
		case Menus.AlignX.Center:
			position = new Vector2(draw2D.ScreenSize.X * 0.5f, draw2D.ScreenSize.Y * 0.075f);
			break;
		case Menus.AlignX.Left:
			position = new Vector2(draw2D.ScreenSize.X * 0.1f, draw2D.ScreenSize.Y * 0.075f);
			break;
		}
	}

	public void render(GameTime gameTime)
	{
		Update(gameTime);
		draw2D.DrawString(message, position, Color.Yellow);
	}

	public void Update(GameTime gameTime)
	{
		currentFrame++;
		if (worst == "" || currentFrame % 60 == 0)
		{
			worstValue = Utils.TargetFPS;
			worst = worstValue.ToString("00.0");
		}
		instantanousValue = (float)(1000.0 / gameTime.ElapsedGameTime.TotalMilliseconds);
		instantanous = instantanousValue.ToString("00.0");
		if (instantanousValue < worstValue)
		{
			worstValue = instantanousValue;
			worst = worstValue.ToString("00.0");
		}
		TotalMillisecInInterval += gameTime.ElapsedGameTime.TotalMilliseconds;
		if (currentFrame % interval == interval - 1)
		{
			averageValue = (float)((double)(1000 * interval) / TotalMillisecInInterval);
			average = averageValue.ToString("00.0");
			TotalMillisecInInterval = 0.0;
		}
	}
}
