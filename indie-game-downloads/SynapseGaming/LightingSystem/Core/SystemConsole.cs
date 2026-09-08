using System.Collections.Generic;
using _0003;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Tracks per-frame lighting and rendering statistics.
/// </summary>
public class SystemConsole
{
	private struct DA_0018(string message, float displayseconds)
	{
		public string Message = message;

		public float DisplaySeconds = displayseconds;

		public float RemainingSeconds = displayseconds;

		public Color GetFadeColor(Color initial)
		{
			return initial * MathHelper.Clamp(RemainingSeconds * 1.5f, 0f, 1f);
		}
	}

	private const int _3A_0018 = 10;

	private static bool _3AL = false;

	private static bool _3A_0019 = false;

	private static Texture2D _3A3;

	private static List<DA_0018> _3A6 = new List<DA_0018>(32);

	private static Dictionary<string, SystemStatistic> _3AD = new Dictionary<string, SystemStatistic>(32);

	private static _0003.c _3A_0017 = new _0003.c();

	private static Color _3A_0003 = new Color(0, 0, 0, 180);

	private static Vector2 _3Al = default(Vector2);

	private static string _3At = "________________";

	private static string _3AF = "FrameRate";

	/// <summary>
	/// Dictionary of all statistics.
	/// </summary>
	public static Dictionary<string, SystemStatistic> Statistics => _3AD;

	/// <summary>
	/// Gets a statistic by name, creating it if necessary.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="category">Category assign to the statistic if a new statistic object is created.</param>
	/// <returns></returns>
	public static SystemStatistic GetStatistic(string name, SystemStatisticCategory category)
	{
		if (_3AD.TryGetValue(name, out var value))
		{
			return value;
		}
		value = new SystemStatistic(name, category);
		_3AD.Add(name, value);
		return value;
	}

	/// <summary>
	/// Adds a message to the system console.
	/// </summary>
	/// <param name="message">Message to display.</param>
	/// <param name="displayseconds">Time in seconds the message is displayed.</param>
	public static void AddMessage(string message, int displayseconds)
	{
		_3A6.Add(new DA_0018(message, displayseconds));
	}

	/// <summary>
	/// Ends statistic gathering for this frame and resets the AccumulationValue for all statistics.
	/// </summary>
	public static void Apply()
	{
		if (!_3AL)
		{
			return;
		}
		foreach (KeyValuePair<string, SystemStatistic> item in _3AD)
		{
			item.Value._0016();
		}
		_3AL = false;
		_3A_0019 = false;
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
		if (!showstats && _3A6.Count <= 0)
		{
			return;
		}
		Vector2 vector = screenposition;
		SpriteBatch spriteBatch = SunBurnCoreSystem.Instance.LQ();
		SpriteFont spriteFont = SunBurnCoreSystem.Instance.LS();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
		if (_3A3 == null || _3A3.IsDisposed)
		{
			_3A3 = SunBurnCoreSystem.Instance.Lw("White");
		}
		spriteBatch.Draw(_3A3, new Rectangle((int)screenposition.X - 10, (int)screenposition.Y - 10, (int)_3Al.X + 20, (int)_3Al.Y + 20), _3A_0003);
		if (showstats)
		{
			_3A_0017.L_0006(_3AF, gametime, !_3A_0019);
			_3Al = _3A_0017.e(spriteBatch, spriteFont, ref screenposition, scale, color);
			_3AL = true;
			_3A_0019 = true;
			foreach (KeyValuePair<string, SystemStatistic> item in _3AD)
			{
				if ((item.Value.Category & categories) != SystemStatisticCategory.None)
				{
					_3A_0017.Ld(item.Value.Name, item.Value.Value);
					Vector2 value = _3A_0017.e(spriteBatch, spriteFont, ref screenposition, scale, color);
					_3Al = Vector2.Max(_3Al, value);
				}
			}
		}
		if (showconsole && _3A6.Count > 0)
		{
			float num = (float)gametime.ElapsedGameTime.TotalSeconds;
			if (showstats)
			{
				_3A_0017.LV(_3At);
				screenposition.Y += _3A_0017.e(spriteBatch, spriteFont, ref screenposition, scale, color).Y;
			}
			for (int i = 0; i < _3A6.Count; i++)
			{
				DA_0018 value2 = _3A6[i];
				value2.RemainingSeconds -= num;
				if (value2.RemainingSeconds <= 0f)
				{
					_3A6.RemoveAt(i);
					i--;
					continue;
				}
				_3A6[i] = value2;
				_3A_0017.LV(value2.Message);
				Vector2 value = _3A_0017.e(spriteBatch, spriteFont, ref screenposition, scale, value2.GetFadeColor(color));
				_3Al = Vector2.Max(_3Al, value);
			}
		}
		_3Al.Y = screenposition.Y - vector.Y;
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
		_3A_0017.L_0006(_3AF, gametime, !_3A_0019);
		empty = empty + _3A_0017.ToString() + "\r\n";
		_3AL = true;
		_3A_0019 = true;
		foreach (KeyValuePair<string, SystemStatistic> item in _3AD)
		{
			if ((item.Value.Category & categories) != SystemStatisticCategory.None)
			{
				_3A_0017.Ld(item.Key, item.Value.Value);
				empty = empty + _3A_0017.ToString() + "\r\n";
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
		_3AL = true;
		return ToString(SystemStatisticCategory.All, new GameTime());
	}
}
