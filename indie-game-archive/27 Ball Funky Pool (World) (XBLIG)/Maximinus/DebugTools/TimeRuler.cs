#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus.DebugTools;

public class TimeRuler : DrawableGameComponent
{
	private struct Marker
	{
		public int MarkerId;

		public float BeginTime;

		public float EndTime;

		public Color Color;
	}

	private class MarkerCollection
	{
		public Marker[] Markers = new Marker[5120];

		public int MarkCount;

		public int[] MarkerNests = new int[32];

		public int NestCount;
	}

	private class FrameLog
	{
		public MarkerCollection[] Bars;

		public FrameLog()
		{
			Bars = new MarkerCollection[8];
			for (int i = 0; i < 8; i++)
			{
				Bars[i] = new MarkerCollection();
			}
		}
	}

	private class MarkerInfo
	{
		public string Name;

		public MarkerLog[] Logs = new MarkerLog[8];

		public MarkerInfo(string name)
		{
			Name = name;
		}
	}

	private struct MarkerLog
	{
		public float SnapMin;

		public float SnapMax;

		public float SnapAvg;

		public float Min;

		public float Max;

		public float Avg;

		public int Samples;

		public Color Color;

		public bool Initialized;
	}

	private const int MaxBars = 8;

	private const int MaxSamples = 5120;

	private const int MaxNestCall = 32;

	private const int MaxSampleFrames = 4;

	private const int LogSnapDuration = 120;

	private const int BarHeight = 8;

	private const int BarPadding = 2;

	private const int AutoAdjustDelay = 30;

	private DebugManager debugManager;

	private FrameLog[] logs;

	private FrameLog prevLog;

	private FrameLog curLog;

	private int frameCount;

	private Stopwatch stopwatch = new Stopwatch();

	private List<MarkerInfo> markers = new List<MarkerInfo>();

	private Dictionary<string, int> markerNameToIdMap = new Dictionary<string, int>();

	private int frameAdjust;

	private int sampleFrames;

	private StringBuilder logString = new StringBuilder(512);

	private int updateCount;

	private Vector2 position;

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
		debugManager = base.Game.Services.GetService(typeof(DebugManager)) as DebugManager;
		if (debugManager == null)
		{
			throw new InvalidOperationException("DebugManager is not registered.");
		}
		if (base.Game.Services.GetService(typeof(IDebugCommandHost)) is IDebugCommandHost debugCommandHost)
		{
			debugCommandHost.RegisterCommand("tr", "TimeRuler", CommandExecute);
			base.Visible = true;
		}
		logs = new FrameLog[2];
		for (int i = 0; i < logs.Length; i++)
		{
			logs[i] = new FrameLog();
		}
		int num = (TargetSampleFrames = 1);
		sampleFrames = num;
		base.Enabled = false;
		base.Initialize();
	}

	protected override void LoadContent()
	{
		Width = (int)((float)base.GraphicsDevice.Viewport.Width * 0.8f);
		position = new Layout(base.GraphicsDevice.Viewport).Place(new Vector2(Width, 8f), 0f, 0.01f, Alignment.BottomCenter);
		base.LoadContent();
	}

	private void CommandExecute(IDebugCommandHost host, string command, IList<string> arguments)
	{
		bool flag = base.Visible;
		if (arguments.Count == 0)
		{
			base.Visible = !base.Visible;
		}
		char[] separator = new char[1] { ':' };
		foreach (string argument in arguments)
		{
			string text = argument.ToLower();
			string[] array = text.Split(separator);
			switch (array[0])
			{
			case "on":
				base.Visible = true;
				break;
			case "off":
				base.Visible = false;
				break;
			case "reset":
				ResetLog();
				break;
			case "log":
				if (array.Length > 1)
				{
					if (string.Compare(array[1], "on") == 0)
					{
						ShowLog = true;
					}
					if (string.Compare(array[1], "off") == 0)
					{
						ShowLog = false;
					}
				}
				else
				{
					ShowLog = !ShowLog;
				}
				break;
			case "frame":
			{
				int val = int.Parse(array[1]);
				val = Math.Max(val, 1);
				val = Math.Min(val, 4);
				TargetSampleFrames = val;
				break;
			}
			case "/?":
			case "--help":
				host.Echo("tr [log|on|off|reset|frame]");
				host.Echo("Options:");
				host.Echo("       on     Display TimeRuler.");
				host.Echo("       off    Hide TimeRuler.");
				host.Echo("       log    Show/Hide marker log.");
				host.Echo("       reset  Reset marker log.");
				host.Echo("       frame:sampleFrames");
				host.Echo("              Change target sample frame count");
				break;
			}
		}
		if (base.Visible != flag)
		{
			Interlocked.Exchange(ref updateCount, 0);
		}
	}

	[Conditional("TRACE")]
	public void StartFrame()
	{
		lock (this)
		{
			int num = Interlocked.Increment(ref updateCount);
			if (base.Visible && 1 < num && num < 4)
			{
				return;
			}
			prevLog = logs[frameCount++ & 1];
			curLog = logs[frameCount & 1];
			float endTime = (float)stopwatch.Elapsed.TotalMilliseconds;
			for (int i = 0; i < prevLog.Bars.Length; i++)
			{
				MarkerCollection markerCollection = prevLog.Bars[i];
				MarkerCollection markerCollection2 = curLog.Bars[i];
				for (int j = 0; j < markerCollection.NestCount; j++)
				{
					int num2 = markerCollection.MarkerNests[j];
					markerCollection.Markers[num2].EndTime = endTime;
					markerCollection2.MarkerNests[j] = j;
					markerCollection2.Markers[j].MarkerId = markerCollection.Markers[num2].MarkerId;
					markerCollection2.Markers[j].BeginTime = 0f;
					markerCollection2.Markers[j].EndTime = -1f;
					markerCollection2.Markers[j].Color = markerCollection.Markers[num2].Color;
				}
				for (int k = 0; k < markerCollection.MarkCount; k++)
				{
					float num3 = markerCollection.Markers[k].EndTime - markerCollection.Markers[k].BeginTime;
					int markerId = markerCollection.Markers[k].MarkerId;
					MarkerInfo markerInfo = markers[markerId];
					markerInfo.Logs[i].Color = markerCollection.Markers[k].Color;
					if (!markerInfo.Logs[i].Initialized)
					{
						markerInfo.Logs[i].Min = num3;
						markerInfo.Logs[i].Max = num3;
						markerInfo.Logs[i].Avg = num3;
						markerInfo.Logs[i].Initialized = true;
						continue;
					}
					markerInfo.Logs[i].Min = Math.Min(markerInfo.Logs[i].Min, num3);
					markerInfo.Logs[i].Max = Math.Min(markerInfo.Logs[i].Max, num3);
					markerInfo.Logs[i].Avg += num3;
					markerInfo.Logs[i].Avg *= 0.5f;
					if (markerInfo.Logs[i].Samples++ >= 120)
					{
						markerInfo.Logs[i].SnapMin = markerInfo.Logs[i].Min;
						markerInfo.Logs[i].SnapMax = markerInfo.Logs[i].Max;
						markerInfo.Logs[i].SnapAvg = markerInfo.Logs[i].Avg;
						markerInfo.Logs[i].Samples = 0;
					}
				}
				markerCollection2.MarkCount = markerCollection.NestCount;
				markerCollection2.NestCount = markerCollection.NestCount;
			}
			stopwatch.Reset();
			stopwatch.Start();
		}
	}

	[Conditional("TRACE")]
	public void BeginMark(string markerName, Color color)
	{
		BeginMark(0, markerName, color);
	}

	[Conditional("TRACE")]
	public void BeginMark(int barIndex, string markerName, Color color)
	{
		lock (this)
		{
			if (barIndex < 0 || barIndex >= 8)
			{
				throw new ArgumentOutOfRangeException("barIndex");
			}
			MarkerCollection markerCollection = curLog.Bars[barIndex];
			if (markerCollection.MarkCount >= 5120)
			{
				throw new OverflowException("Exceeded sample count.\nEither set larger number to TimeRuler.MaxSample orlower sample count.");
			}
			if (markerCollection.NestCount >= 32)
			{
				throw new OverflowException("Exceeded nest count.\nEither set larget number to TimeRuler.MaxNestCall orlower nest calls.");
			}
			if (!markerNameToIdMap.TryGetValue(markerName, out var value))
			{
				value = markers.Count;
				markerNameToIdMap.Add(markerName, value);
				markers.Add(new MarkerInfo(markerName));
			}
			markerCollection.MarkerNests[markerCollection.NestCount++] = markerCollection.MarkCount;
			markerCollection.Markers[markerCollection.MarkCount].MarkerId = value;
			markerCollection.Markers[markerCollection.MarkCount].Color = color;
			markerCollection.Markers[markerCollection.MarkCount].BeginTime = (float)stopwatch.Elapsed.TotalMilliseconds;
			markerCollection.Markers[markerCollection.MarkCount].EndTime = -1f;
			markerCollection.MarkCount++;
		}
	}

	[Conditional("TRACE")]
	public void EndMark(string markerName)
	{
		EndMark(0, markerName);
	}

	[Conditional("TRACE")]
	public void EndMark(int barIndex, string markerName)
	{
		lock (this)
		{
			if (barIndex < 0 || barIndex >= 8)
			{
				throw new ArgumentOutOfRangeException("barIndex");
			}
			MarkerCollection markerCollection = curLog.Bars[barIndex];
			if (markerCollection.NestCount <= 0)
			{
				throw new InvalidOperationException("Call BeingMark method before call EndMark method.");
			}
			if (!markerNameToIdMap.TryGetValue(markerName, out var value))
			{
				throw new InvalidOperationException($"Maker '{markerName}' is not registered.Make sure you specifed same name as you used for BeginMark method.");
			}
			int num = markerCollection.MarkerNests[--markerCollection.NestCount];
			if (markerCollection.Markers[num].MarkerId != value)
			{
				throw new InvalidOperationException("Incorrect call order of BeginMark/EndMark method.You call it like BeginMark(A), BeginMark(B), EndMark(B), EndMark(A) But you can't call it like BeginMark(A), BeginMark(B), EndMark(A), EndMark(B).");
			}
			markerCollection.Markers[num].EndTime = (float)stopwatch.Elapsed.TotalMilliseconds;
		}
	}

	public float GetAverageTime(int barIndex, string markerName)
	{
		if (barIndex < 0 || barIndex >= 8)
		{
			throw new ArgumentOutOfRangeException("barIndex");
		}
		float result = 0f;
		if (markerNameToIdMap.TryGetValue(markerName, out var value))
		{
			result = markers[value].Logs[barIndex].Avg;
		}
		return result;
	}

	[Conditional("TRACE")]
	public void ResetLog()
	{
		lock (this)
		{
			foreach (MarkerInfo marker in markers)
			{
				for (int i = 0; i < marker.Logs.Length; i++)
				{
					marker.Logs[i].Initialized = false;
					marker.Logs[i].SnapMin = 0f;
					marker.Logs[i].SnapMax = 0f;
					marker.Logs[i].SnapAvg = 0f;
					marker.Logs[i].Min = 0f;
					marker.Logs[i].Max = 0f;
					marker.Logs[i].Avg = 0f;
					marker.Logs[i].Samples = 0;
				}
			}
		}
	}

	public override void Draw(GameTime gameTime)
	{
		if (MaximinusGame.DebugIncludeInTimeRuler)
		{
			MaximinusGame.Debug_TimeRuler_BeginMark("DEBUG ruler", Color.White);
		}
		Draw(position, Width);
		base.Draw(gameTime);
		if (MaximinusGame.DebugIncludeInTimeRuler)
		{
			MaximinusGame.Debug_TimeRuler_EndMark("DEBUG ruler");
		}
	}

	[Conditional("TRACE")]
	public void Draw(Vector2 position, int width)
	{
		if (!base.Visible)
		{
			return;
		}
		Interlocked.Exchange(ref updateCount, 0);
		SpriteBatch spriteBatch = debugManager.SpriteBatch;
		SpriteFont debugFont = debugManager.debugFont;
		Texture2D whiteTexture = debugManager.WhiteTexture;
		int num = 0;
		float num2 = 0f;
		MarkerCollection[] bars = prevLog.Bars;
		foreach (MarkerCollection markerCollection in bars)
		{
			if (markerCollection.MarkCount > 0)
			{
				num += 12;
				num2 = Math.Max(num2, markerCollection.Markers[markerCollection.MarkCount - 1].EndTime);
			}
		}
		float num3 = (float)sampleFrames * 16.666666f;
		if (num2 > num3)
		{
			frameAdjust = Math.Max(0, frameAdjust) + 1;
		}
		else
		{
			frameAdjust = Math.Min(0, frameAdjust) - 1;
		}
		if (Math.Abs(frameAdjust) > 30)
		{
			sampleFrames = Math.Min(4, sampleFrames);
			sampleFrames = Math.Max(TargetSampleFrames, (int)(num2 / 16.666666f) + 1);
			frameAdjust = 0;
		}
		float num4 = (float)width / num3;
		int num5 = (int)position.Y - (num - 8);
		int num6 = num5;
		debugManager.SBBegin();
		Rectangle destinationRectangle = new Rectangle((int)position.X, num6, width, num);
		spriteBatch.Draw(whiteTexture, destinationRectangle, null, new Color(0, 0, 0, 128), 0f, Vector2.Zero, SpriteEffects.None, 0.0003f);
		destinationRectangle.Height = 8;
		MarkerCollection[] bars2 = prevLog.Bars;
		foreach (MarkerCollection markerCollection2 in bars2)
		{
			destinationRectangle.Y = num6 + 2;
			if (markerCollection2.MarkCount > 0)
			{
				for (int k = 0; k < markerCollection2.MarkCount; k++)
				{
					float beginTime = markerCollection2.Markers[k].BeginTime;
					float endTime = markerCollection2.Markers[k].EndTime;
					int num7 = (int)(position.X + beginTime * num4);
					int num8 = (int)(position.X + endTime * num4);
					destinationRectangle.X = num7;
					destinationRectangle.Width = Math.Max(num8 - num7, 1);
					spriteBatch.Draw(whiteTexture, destinationRectangle, null, markerCollection2.Markers[k].Color, 0f, Vector2.Zero, SpriteEffects.None, 0.0001f);
				}
			}
			num6 += 10;
		}
		destinationRectangle = new Rectangle((int)position.X, num5, 1, num);
		for (float num9 = 1f; num9 < num3; num9++)
		{
			destinationRectangle.X = (int)(position.X + num9 * num4);
			spriteBatch.Draw(whiteTexture, destinationRectangle, Color.Gray);
		}
		for (int l = 0; l <= sampleFrames; l++)
		{
			destinationRectangle.X = (int)(position.X + 16.666666f * (float)l * num4);
			spriteBatch.Draw(whiteTexture, destinationRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.0001f);
		}
		if (ShowLog)
		{
			num6 = num5 - debugFont.LineSpacing;
			logString.Length = 0;
			foreach (MarkerInfo marker in markers)
			{
				for (int m = 0; m < 8; m++)
				{
					if (marker.Logs[m].Initialized)
					{
						if (logString.Length > 0)
						{
							logString.Append("\n");
						}
						logString.Append(" Bar ");
						logString.AppendNumber(m);
						logString.Append(" ");
						logString.Append(marker.Name);
						logString.Append(" Avg.:");
						logString.AppendNumber(marker.Logs[m].SnapAvg);
						logString.Append("ms ");
						num6 -= debugFont.LineSpacing;
					}
				}
			}
			Vector2 vector = debugFont.MeasureString(logString);
			destinationRectangle = new Rectangle((int)position.X, num6, (int)vector.X + 12, (int)vector.Y);
			spriteBatch.Draw(whiteTexture, destinationRectangle, null, new Color(0, 0, 0, 128), 0f, Vector2.Zero, SpriteEffects.None, 0.0001f);
			spriteBatch.DrawString(debugFont, logString, new Vector2(position.X + 12f, num6), Color.White);
			num6 += (int)((float)debugFont.LineSpacing * 0.3f);
			destinationRectangle = new Rectangle((int)position.X + 4, num6, 10, 10);
			Rectangle destinationRectangle2 = new Rectangle((int)position.X + 5, num6 + 1, 8, 8);
			foreach (MarkerInfo marker2 in markers)
			{
				for (int n = 0; n < 8; n++)
				{
					if (marker2.Logs[n].Initialized)
					{
						destinationRectangle.Y = num6;
						destinationRectangle2.Y = num6 + 1;
						spriteBatch.Draw(whiteTexture, destinationRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.0002f);
						spriteBatch.Draw(whiteTexture, destinationRectangle2, marker2.Logs[n].Color);
						num6 += debugFont.LineSpacing;
					}
				}
			}
		}
		debugManager.SBEnd();
	}
}
