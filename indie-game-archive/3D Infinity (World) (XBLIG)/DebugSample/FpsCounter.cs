using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DebugSample;

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
		debugManager = ((GameComponent)this).Game.Services.GetService(typeof(DebugManager)) as DebugManager;
		if (debugManager == null)
		{
			throw new InvalidOperationException("DebugManaerが登録されていません");
		}
		if (((GameComponent)this).Game.Services.GetService(typeof(IDebugCommandHost)) is IDebugCommandHost debugCommandHost)
		{
			debugCommandHost.RegisterCommand("fps", "FPS Counter", CommandExecute);
			((DrawableGameComponent)this).Visible = false;
		}
		Fps = 0f;
		sampleFrames = 0;
		stopwatch = Stopwatch.StartNew();
		stringBuilder.Length = 0;
		((DrawableGameComponent)this).Initialize();
	}

	private void CommandExecute(IDebugCommandHost host, string command, IList<string> arguments)
	{
		if (arguments.Count == 0)
		{
			((DrawableGameComponent)this).Visible = !((DrawableGameComponent)this).Visible;
		}
		foreach (string argument in arguments)
		{
			switch (argument.ToLower())
			{
			case "on":
				((DrawableGameComponent)this).Visible = true;
				break;
			case "off":
				((DrawableGameComponent)this).Visible = false;
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
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		sampleFrames++;
		SpriteBatch spriteBatch = debugManager.SpriteBatch;
		SpriteFont debugFont = debugManager.DebugFont;
		Vector2 val = debugFont.MeasureString("X");
		Rectangle val2 = default(Rectangle);
		((Rectangle)(ref val2))._002Ector(0, 0, (int)(val.X * 14f), (int)(val.Y * 1.3f));
		Layout layout = new Layout(spriteBatch.GraphicsDevice.Viewport);
		val2 = layout.Place(val2, 0.01f, 0.01f, Alignment.TopLeft);
		val = debugFont.MeasureString(stringBuilder);
		layout.ClientArea = val2;
		Vector2 val3 = layout.Place(val, 0f, 0.1f, Alignment.Center);
		spriteBatch.Begin();
		spriteBatch.Draw(debugManager.WhiteTexture, val2, new Color((byte)0, (byte)0, (byte)0, (byte)128));
		spriteBatch.DrawString(debugFont, stringBuilder, val3, Color.White);
		spriteBatch.End();
		((DrawableGameComponent)this).Draw(gameTime);
	}
}
