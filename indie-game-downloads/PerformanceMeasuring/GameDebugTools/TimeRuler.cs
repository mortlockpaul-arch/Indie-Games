using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace PerformanceMeasuring.GameDebugTools;

public class TimeRuler : DrawableGameComponent
{
	private const int MaxBars = 8;

	private const int MaxSamples = 4096;

	private const int MaxNestCall = 32;

	private const int MaxSampleFrames = 4;

	private const int LogSnapDuration = 120;

	private const int BarHeight = 8;

	private const int BarPadding = 2;

	private const int AutoAdjustDelay = 0;

	private Vector2 position;

	private int currentMarks;

	public bool ShowLog { get; set; }

	public int TargetSampleFrames { get; set; }

	public Vector2 Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
		}
	}

	public int Width { get; set; }

	public TimeRuler(Game game)
		: base(game)
	{
		base.Game.Services.AddService(typeof(TimeRuler), this);
	}

	public override void Initialize()
	{
		base.Initialize();
	}

	protected override void LoadContent()
	{
		Width = (int)((float)base.GraphicsDevice.Viewport.Width * 0.98f);
		position = new Layout(base.GraphicsDevice.Viewport).Place(new Vector2(Width, 8f), 0f, 0.01f, Alignment.BottomCenter);
		base.LoadContent();
	}

	[Conditional("PROFILE")]
	public void StartFrame()
	{
	}

	[Conditional("PROFILE")]
	public void BeginMark(string markerName, Color color)
	{
	}

	[Conditional("PROFILE")]
	private void BeginMark(int barIndex, string markerName, Color color)
	{
	}

	[Conditional("PROFILE")]
	public void EndMark(string markerName)
	{
	}

	[Conditional("PROFILE")]
	public void EndMark(int barIndex, string markerName)
	{
	}

	public float GetAverageTime(int barIndex, string markerName)
	{
		return 0f;
	}

	[Conditional("PROFILE")]
	public void ResetLog()
	{
	}

	public override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
	}

	[Conditional("PROFILE")]
	public void Draw(Vector2 position, int width)
	{
	}
}
