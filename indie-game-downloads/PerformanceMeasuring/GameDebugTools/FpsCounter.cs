using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;

namespace PerformanceMeasuring.GameDebugTools;

public class FpsCounter : DrawableGameComponent
{
	private DebugManager debugManager;

	private Stopwatch stopwatch;

	private int sampleFrames;

	private StringBuilder stringBuilder = new StringBuilder(16);

	private string extraMessage;

	private int collectionsDetected;

	private long totalMemory;

	private long lastGCMemory;

	public float Fps { get; private set; }

	public TimeSpan SampleSpan { get; set; }

	public string ExtraMessage
	{
		get
		{
			return extraMessage;
		}
		set
		{
			extraMessage = value;
		}
	}

	public FpsCounter(Game game)
		: base(game)
	{
		SampleSpan = TimeSpan.FromSeconds(1.0);
		totalMemory = GC.GetTotalMemory(forceFullCollection: false);
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
	}

	public override void Draw(GameTime gameTime)
	{
		long num = GC.GetTotalMemory(forceFullCollection: false);
		if (num < totalMemory)
		{
			collectionsDetected++;
			lastGCMemory = num;
		}
		totalMemory = num;
		if (stopwatch.Elapsed > SampleSpan)
		{
			Fps = (float)sampleFrames / (float)stopwatch.Elapsed.TotalSeconds;
			stopwatch.Reset();
			stopwatch.Start();
			sampleFrames = 0;
			stringBuilder.Length = 0;
			stringBuilder.Append("FPS: ");
			StringBuilderExtensions.AppendNumber(stringBuilder, Fps);
			stringBuilder.Append("  GC: ");
			StringBuilderExtensions.AppendNumber(stringBuilder, collectionsDetected);
			stringBuilder.Append("  RC: ");
			StringBuilderExtensions.AppendNumber(stringBuilder, RenderStats.RenderCalls);
			stringBuilder.Append("  CO: ");
			StringBuilderExtensions.AppendNumber(stringBuilder, RenderStats.CulledObjects);
			stringBuilder.Append("  MEM: ");
			stringBuilder.AppendNumber((float)num / 1048576f, 2, AppendNumberOptions.NumberGroup);
			stringBuilder.Append("MB  GC MEM: ");
			stringBuilder.AppendNumber((float)(num - lastGCMemory) / 1024f, 2, AppendNumberOptions.NumberGroup);
			stringBuilder.Append("KB");
			stringBuilder.Append(extraMessage);
		}
		sampleFrames++;
		SpriteBatch spriteBatch = debugManager.SpriteBatch;
		SpriteFont debugFont = debugManager.DebugFont;
		Vector2 vector = debugFont.MeasureString(stringBuilder);
		Rectangle destinationRectangle = new Rectangle(35, 35, (int)(vector.X + 15f), (int)(vector.Y * 1.3f));
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
		spriteBatch.Draw(debugManager.WhiteTexture, destinationRectangle, new Color(0, 0, 0, 128));
		spriteBatch.DrawString(debugFont, stringBuilder, new Vector2(40f, 40f), Color.White);
		spriteBatch.End();
		base.Draw(gameTime);
	}
}
