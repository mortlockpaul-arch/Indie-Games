#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DebugSample;

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
		public Marker[] Markers = new Marker[1024];

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

		public bool Initialized;
	}

	private const int MaxBars = 8;

	private const int MaxSamples = 1024;

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

	private Vector2 position;

	public bool ShowLog { get; set; }

	public int TargetSampleFrames { get; set; }

	public Vector2 Position
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return position;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			position = value;
		}
	}

	public int Width { get; set; }

	public TimeRuler(Game game)
		: base(game)
	{
		((GameComponent)this).Game.Services.AddService(typeof(TimeRuler), (object)this);
	}

	public override void Initialize()
	{
		debugManager = ((GameComponent)this).Game.Services.GetService(typeof(DebugManager)) as DebugManager;
		if (debugManager == null)
		{
			throw new InvalidOperationException("DebugManagerが登録されていません");
		}
		if (((GameComponent)this).Game.Services.GetService(typeof(IDebugCommandHost)) is IDebugCommandHost debugCommandHost)
		{
			debugCommandHost.RegisterCommand("tr", "TimeRuler", CommandExecute);
			((DrawableGameComponent)this).Visible = false;
			((GameComponent)this).Enabled = false;
		}
		logs = new FrameLog[2];
		for (int i = 0; i < logs.Length; i++)
		{
			logs[i] = new FrameLog();
		}
		int num = (TargetSampleFrames = 1);
		sampleFrames = num;
		((DrawableGameComponent)this).Initialize();
	}

	protected override void LoadContent()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		Viewport viewport = ((DrawableGameComponent)this).GraphicsDevice.Viewport;
		Width = (int)((float)((Viewport)(ref viewport)).Width * 0.8f);
		position = new Layout(((DrawableGameComponent)this).GraphicsDevice.Viewport).Place(new Vector2((float)Width, 8f), 0f, 0.01f, Alignment.BottomCenter);
		((DrawableGameComponent)this).LoadContent();
	}

	private void CommandExecute(IDebugCommandHost host, string command, IList<string> arguments)
	{
		if (arguments.Count == 0)
		{
			((DrawableGameComponent)this).Visible = !((DrawableGameComponent)this).Visible;
		}
		char[] separator = new char[1] { ':' };
		foreach (string argument in arguments)
		{
			string text = argument.ToLower();
			string[] array = text.Split(separator);
			switch (array[0])
			{
			case "on":
				((DrawableGameComponent)this).Visible = true;
				break;
			case "off":
				((DrawableGameComponent)this).Visible = false;
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
	}

	[Conditional("TRACE")]
	public void StartFrame()
	{
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		lock (this)
		{
			prevLog = logs[frameCount++ & 1];
			curLog = logs[frameCount & 1];
			float endTime = (float)stopwatch.Elapsed.TotalMilliseconds;
			for (int i = 0; i < prevLog.Bars.Length; i++)
			{
				MarkerCollection markerCollection = prevLog.Bars[i];
				MarkerCollection markerCollection2 = curLog.Bars[i];
				for (int j = 0; j < markerCollection.NestCount; j++)
				{
					int num = markerCollection.MarkerNests[j];
					markerCollection.Markers[num].EndTime = endTime;
					markerCollection2.MarkerNests[j] = j;
					markerCollection2.Markers[j].MarkerId = markerCollection.Markers[num].MarkerId;
					markerCollection2.Markers[j].BeginTime = 0f;
					markerCollection2.Markers[j].EndTime = -1f;
					markerCollection2.Markers[j].Color = markerCollection.Markers[num].Color;
				}
				for (int k = 0; k < markerCollection.MarkCount; k++)
				{
					float num2 = markerCollection.Markers[k].EndTime - markerCollection.Markers[k].BeginTime;
					int markerId = markerCollection.Markers[k].MarkerId;
					MarkerInfo markerInfo = markers[markerId];
					if (!markerInfo.Logs[i].Initialized)
					{
						markerInfo.Logs[i].Min = num2;
						markerInfo.Logs[i].Max = num2;
						markerInfo.Logs[i].Avg = num2;
						markerInfo.Logs[i].Initialized = true;
						continue;
					}
					markerInfo.Logs[i].Min = Math.Min(markerInfo.Logs[i].Min, num2);
					markerInfo.Logs[i].Max = Math.Min(markerInfo.Logs[i].Max, num2);
					markerInfo.Logs[i].Avg += num2;
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
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		BeginMark(0, markerName, color);
	}

	[Conditional("TRACE")]
	public void BeginMark(int barIndex, string markerName, Color color)
	{
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		lock (this)
		{
			if (barIndex < 0 || barIndex >= 8)
			{
				throw new ArgumentOutOfRangeException("barIndex");
			}
			MarkerCollection markerCollection = curLog.Bars[barIndex];
			if (markerCollection.MarkCount >= 1024)
			{
				throw new OverflowException("サンプル数がMaxSampleを超えました。\nTimeRuler.MaxSmpaleの値を大きくするか、サンプル数を少なくしてください。");
			}
			if (markerCollection.NestCount >= 32)
			{
				throw new OverflowException("ネスト数がMaxNestCallを超えました。\nTimeRuler.MaxNestCallの値を大きくするか、ネスト呼び出し数を減らしてください。");
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
				throw new InvalidOperationException("EndMarkを呼び出す前に、BeginMarkメソッドを呼んでください。");
			}
			if (!markerNameToIdMap.TryGetValue(markerName, out var value))
			{
				throw new InvalidOperationException($"マーカー名「{markerName}」は登録されていません。BeginMarkで使った名前と同じ名前か確認してください。");
			}
			int num = markerCollection.MarkerNests[--markerCollection.NestCount];
			if (markerCollection.Markers[num].MarkerId != value)
			{
				throw new InvalidOperationException("BeginMark/EndMarkの呼び出し順序が不正です。BeginMark(A), BeginMark(B), EndMark(B), EndMark(A)ののようには呼べますが、BeginMark(A), BeginMark(B), EndMark(A), EndMark(B)のようには呼べません。");
			}
			markerCollection.Markers[num].EndTime = (float)stopwatch.Elapsed.TotalMilliseconds;
		}
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
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Draw(position, Width);
		((DrawableGameComponent)this).Draw(gameTime);
	}

	[Conditional("TRACE")]
	public void Draw(Vector2 position, int width)
	{
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		SpriteBatch spriteBatch = debugManager.SpriteBatch;
		SpriteFont debugFont = debugManager.DebugFont;
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
		spriteBatch.Begin();
		Rectangle val = default(Rectangle);
		((Rectangle)(ref val))._002Ector((int)position.X, num6, width, num);
		spriteBatch.Draw(whiteTexture, val, new Color((byte)0, (byte)0, (byte)0, (byte)128));
		val.Height = 8;
		MarkerCollection[] bars2 = prevLog.Bars;
		foreach (MarkerCollection markerCollection2 in bars2)
		{
			val.Y = num6 + 2;
			if (markerCollection2.MarkCount > 0)
			{
				for (int k = 0; k < markerCollection2.MarkCount; k++)
				{
					float beginTime = markerCollection2.Markers[k].BeginTime;
					float endTime = markerCollection2.Markers[k].EndTime;
					int num7 = (int)(position.X + beginTime * num4);
					int num8 = (int)(position.X + endTime * num4);
					val.X = num7;
					val.Width = Math.Max(num8 - num7, 1);
					spriteBatch.Draw(whiteTexture, val, markerCollection2.Markers[k].Color);
				}
			}
			num6 += 10;
		}
		((Rectangle)(ref val))._002Ector((int)position.X, num5, 1, num);
		for (float num9 = 1f; num9 < num3; num9++)
		{
			val.X = (int)(position.X + num9 * num4);
			spriteBatch.Draw(whiteTexture, val, Color.Gray);
		}
		for (int l = 0; l <= sampleFrames; l++)
		{
			val.X = (int)(position.X + 16.666666f * (float)l * num4);
			spriteBatch.Draw(whiteTexture, val, Color.White);
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
			Vector2 val2 = debugFont.MeasureString(logString);
			((Rectangle)(ref val))._002Ector((int)position.X, num6, (int)val2.X, (int)val2.Y);
			spriteBatch.Draw(whiteTexture, val, new Color((byte)0, (byte)0, (byte)0, (byte)128));
			spriteBatch.DrawString(debugFont, logString, new Vector2(position.X, (float)num6), Color.White);
		}
		spriteBatch.End();
	}
}
