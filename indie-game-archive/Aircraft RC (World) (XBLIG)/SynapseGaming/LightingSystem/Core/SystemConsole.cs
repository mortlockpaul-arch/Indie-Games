using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Z;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Tracks per-frame lighting and rendering statistics.
/// </summary>
public class SystemConsole
{
	private struct _0001CB(string message, float displayseconds)
	{
		public string Message = message;

		public float DisplaySeconds = displayseconds;

		public float RemainingSeconds = displayseconds;

		public Color GetFadeColor(Color initial)
		{
			return initial * MathHelper.Clamp(RemainingSeconds * 1.5f, 0f, 1f);
		}
	}

	private const int HCB = 10;

	private static bool HC_0002 = false;

	private static bool HC_0012 = false;

	private static Texture2D HCH;

	private static List<_0001CB> HC7 = new List<_0001CB>(32);

	private static Dictionary<string, SystemStatistic> HC_0001 = new Dictionary<string, SystemStatistic>(32);

	private static Z.D HCw = new Z.D();

	private static Color HCZ = new Color(0, 0, 0, 180);

	private static Vector2 HC_000F = default(Vector2);

	private static string HCy = "________________";

	private static string HC6 = "FrameRate";

	/// <summary>
	/// Dictionary of all statistics.
	/// </summary>
	public static Dictionary<string, SystemStatistic> Statistics => HC_0001;

	/// <summary>
	/// Gets a statistic by name, creating it if necessary.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="category">Category assign to the statistic if a new statistic object is created.</param>
	/// <returns></returns>
	public static SystemStatistic GetStatistic(string name, SystemStatisticCategory category)
	{
		if (HC_0001.TryGetValue(name, out var value))
		{
			return value;
		}
		value = new SystemStatistic(name, category);
		HC_0001.Add(name, value);
		return value;
	}

	/// <summary>
	/// Adds a message to the system console.
	/// </summary>
	/// <param name="message">Message to display.</param>
	/// <param name="displayseconds">Time in seconds the message is displayed.</param>
	public static void AddMessage(string message, int displayseconds)
	{
		HC7.Add(new _0001CB(message, displayseconds));
	}

	/// <summary>
	/// Ends statistic gathering for this frame and resets the AccumulationValue for all statistics.
	/// </summary>
	public static void Apply()
	{
		if (!HC_0002)
		{
			return;
		}
		foreach (KeyValuePair<string, SystemStatistic> item in HC_0001)
		{
			item.Value.R();
		}
		HC_0002 = false;
		HC_0012 = false;
	}

	/// <summary>
	/// Renders stats to the screen. This can be slow on some hardware, rendering several
	/// categories when trying to capture the frame rate is not recommended.
	/// </summary>
	/// <param name="categories">The statistic categories to render.</param>
	/// <param name="showstats">Determines if system stats are rendered.</param>
	/// <param name="showconsole">Determines if system console messages
	/// are rendered. The console is not displayed if no messages exist.</param>
	/// <param name="screenposition">Upper left corner to begin rendering.</param>
	/// <param name="scale">Text scale.</param>
	/// <param name="color">Text color.</param>
	/// <param name="gametime"></param>
	public static void Render(SystemStatisticCategory categories, bool showstats, bool showconsole, Vector2 screenposition, Vector2 scale, Color color, GameTime gametime)
	{
		if (!showstats && HC7.Count <= 0)
		{
			return;
		}
		Vector2 vector = screenposition;
		SpriteBatch spriteBatch = SunBurnCoreSystem.Instance._00025();
		SpriteFont spriteFont = SunBurnCoreSystem.Instance._0002n();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
		if (HCH == null || HCH.IsDisposed)
		{
			HCH = SunBurnCoreSystem.Instance._0002l("White");
		}
		spriteBatch.Draw(HCH, new Rectangle((int)screenposition.X - 10, (int)screenposition.Y - 10, (int)HC_000F.X + 20, (int)HC_000F.Y + 20), HCZ);
		if (showstats)
		{
			HCw._0002_0004(HC6, gametime, !HC_0012);
			HC_000F = HCw.b(spriteBatch, spriteFont, ref screenposition, scale, color);
			HC_0002 = true;
			HC_0012 = true;
			foreach (KeyValuePair<string, SystemStatistic> item in HC_0001)
			{
				if ((item.Value.Category & categories) != SystemStatisticCategory.None)
				{
					HCw._00029(item.Value.Name, item.Value.Value);
					Vector2 value = HCw.b(spriteBatch, spriteFont, ref screenposition, scale, color);
					HC_000F = Vector2.Max(HC_000F, value);
				}
			}
		}
		if (showconsole && HC7.Count > 0)
		{
			float num = (float)gametime.ElapsedGameTime.TotalSeconds;
			if (showstats)
			{
				HCw._0002E(HCy);
				screenposition.Y += HCw.b(spriteBatch, spriteFont, ref screenposition, scale, color).Y;
			}
			for (int i = 0; i < HC7.Count; i++)
			{
				_0001CB value2 = HC7[i];
				value2.RemainingSeconds -= num;
				if (value2.RemainingSeconds <= 0f)
				{
					HC7.RemoveAt(i);
					i--;
					continue;
				}
				HC7[i] = value2;
				HCw._0002E(value2.Message);
				Vector2 value = HCw.b(spriteBatch, spriteFont, ref screenposition, scale, value2.GetFadeColor(color));
				HC_000F = Vector2.Max(HC_000F, value);
			}
		}
		HC_000F.Y = screenposition.Y - vector.Y;
		spriteBatch.End();
	}

	/// <summary>
	/// Returns a string containing the names and values of all requested statistics.
	/// </summary>
	/// <param name="categories">Statistic categories to include.</param>
	/// <param name="gametime">Current game time used in frame rate calculation.</param>
	public static string ToString(SystemStatisticCategory categories, GameTime gametime)
	{
		string empty = string.Empty;
		HCw._0002_0004(HC6, gametime, !HC_0012);
		empty = empty + HCw.ToString() + "\r\n";
		HC_0002 = true;
		HC_0012 = true;
		foreach (KeyValuePair<string, SystemStatistic> item in HC_0001)
		{
			if ((item.Value.Category & categories) != SystemStatisticCategory.None)
			{
				HCw._00029(item.Key, item.Value.Value);
				empty = empty + HCw.ToString() + "\r\n";
			}
		}
		return empty;
	}

	/// <summary>
	/// Returns a string containing the names and values of all statistics. Because this method
	/// does not take the current game time the frame rate is likely to be inaccurate.
	/// </summary>
	/// <returns></returns>
	public new static string ToString()
	{
		HC_0002 = true;
		return ToString(SystemStatisticCategory.All, new GameTime());
	}
}
