using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus.DebugTools;

public class FpsCounter : DrawableGameComponent
{
	private DebugManager debugManager;

	private Stopwatch stopwatch;

	private int sampleFrames;

	private StringBuilder stringBuilder = new StringBuilder(16);

	public float Fps { get; private set; }

	public TimeSpan SampleSpan { get; set; }

	public FpsCounter(Game game)
		: base(game)
	{
		SampleSpan = TimeSpan.FromSeconds(1.0);
	}

	public override void Initialize()
	{
		debugManager = base.Game.Services.GetService(typeof(DebugManager)) as DebugManager;
		if (debugManager == null)
		{
			throw new InvalidOperationException("DebugManaer is not registered.");
		}
		if (base.Game.Services.GetService(typeof(IDebugCommandHost)) is IDebugCommandHost debugCommandHost)
		{
			debugCommandHost.RegisterCommand("fps", "FPS Counter", CommandExecute);
			base.Visible = true;
		}
		Fps = 0f;
		sampleFrames = 0;
		stopwatch = Stopwatch.StartNew();
		stringBuilder.Length = 0;
		base.Initialize();
	}

	private void CommandExecute(IDebugCommandHost host, string command, IList<string> arguments)
	{
		if (arguments.Count == 0)
		{
			base.Visible = !base.Visible;
		}
		foreach (string argument in arguments)
		{
			switch (argument.ToLower())
			{
			case "on":
				base.Visible = true;
				break;
			case "off":
				base.Visible = false;
				break;
			}
		}
	}

	public override void Update(GameTime gameTime)
	{
		if (stopwatch.Elapsed > SampleSpan)
		{
			Fps = (float)sampleFrames / (float)stopwatch.Elapsed.TotalSeconds;
			stopwatch.Reset();
			stopwatch.Start();
			sampleFrames = 0;
			stringBuilder.Length = 0;
			stringBuilder.Append("FPS: ");
			stringBuilder.AppendNumber(Fps);
		}
	}

	public override void Draw(GameTime gameTime)
	{
		if (MaximinusGame.DebugIncludeInTimeRuler)
		{
			MaximinusGame.Debug_TimeRuler_BeginMark("DEBUG fps", Color.White);
		}
		sampleFrames++;
		SpriteBatch spriteBatch = debugManager.SpriteBatch;
		SpriteFont debugFont = debugManager.debugFont;
		Vector2 vector = debugFont.MeasureString("X");
		Rectangle region = new Rectangle(0, 0, (int)(vector.X * 14f), (int)(vector.Y * 1.3f));
		Layout layout = new Layout(spriteBatch.GraphicsDevice.Viewport);
		region = layout.Place(region, 0.01f, 0.01f, Alignment.TopLeft);
		vector = debugFont.MeasureString(stringBuilder);
		layout.ClientArea = region;
		Vector2 position = layout.Place(vector, 0f, 0.1f, Alignment.Center);
		debugManager.SBBegin();
		spriteBatch.Draw(debugManager.WhiteTexture, region, null, new Color(0, 0, 0, 128), 0f, Vector2.Zero, SpriteEffects.None, 0.0001f);
		spriteBatch.DrawString(debugFont, stringBuilder, position, Color.White);
		debugManager.SBEnd();
		base.Draw(gameTime);
		if (MaximinusGame.DebugIncludeInTimeRuler)
		{
			MaximinusGame.Debug_TimeRuler_EndMark("DEBUG fps");
		}
	}
}
