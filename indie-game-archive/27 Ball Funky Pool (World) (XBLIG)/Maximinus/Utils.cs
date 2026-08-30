using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Maximinus;

public class Utils
{
	public class EventArgsInteger : EventArgs
	{
		public int value;

		public EventArgsInteger(int i)
		{
			value = i;
		}
	}

	public class EventArgs2Integers : EventArgs
	{
		public int value1;

		public int value2;

		public EventArgs2Integers(int v1, int v2)
		{
			value1 = v1;
			value2 = v2;
		}
	}

	public class Input
	{
		public enum PressOrRelease
		{
			Press,
			Release
		}

		public enum ActionMenu
		{
			MENU_UP,
			MENU_DOWN,
			MENU_LEFT,
			MENU_RIGHT,
			MENU_ACTIVATE,
			MENU_BACK,
			MENU_BUTTON_X,
			NONE
		}

		protected enum DPadButton
		{
			Up,
			Down,
			Left,
			Right
		}

		protected const int debugWinPadIndex = 0;

		public const float PadDeadZone = 0.2f;

		protected bool padIndexFound;

		protected PlayerIndex playerIndex;

		protected static ActionMenu[] ActionMenuArray = new ActionMenu[7]
		{
			ActionMenu.MENU_UP,
			ActionMenu.MENU_DOWN,
			ActionMenu.MENU_LEFT,
			ActionMenu.MENU_RIGHT,
			ActionMenu.MENU_ACTIVATE,
			ActionMenu.MENU_BACK,
			ActionMenu.MENU_BUTTON_X
		};

		private ActionMenu currentJoystickMenuAction;

		private ActionMenu previousJoystickMenuAction;

		protected int frameIntervaleForRepeat = 20;

		protected GamePadState padState;

		protected GamePadState padPreviousState;

		protected GamePadState[] padPreviousStatesForFindPad = new GamePadState[4];

		protected double timerAfterFindPadMs;

		public bool PadIndexFound => padIndexFound;

		public PlayerIndex PlayerIndex => playerIndex;

		public string PlayerIndexString => playerIndex.ToString();

		protected Vector2 currentStick => padState.ThumbSticks.Left;

		public static bool IsActionLeftRight(ActionMenu action)
		{
			if (action != ActionMenu.MENU_LEFT)
			{
				return action == ActionMenu.MENU_RIGHT;
			}
			return true;
		}

		protected DPadButton DPadMapping_Menu(ActionMenu action)
		{
			return action switch
			{
				ActionMenu.MENU_UP => DPadButton.Up, 
				ActionMenu.MENU_DOWN => DPadButton.Down, 
				ActionMenu.MENU_LEFT => DPadButton.Left, 
				ActionMenu.MENU_RIGHT => DPadButton.Right, 
				_ => throw new Exception("no DPad mapping for this action " + action), 
			};
		}

		protected Buttons padMapping_Menu(ActionMenu action)
		{
			return action switch
			{
				ActionMenu.MENU_ACTIVATE => Buttons.A, 
				ActionMenu.MENU_BACK => Buttons.B, 
				ActionMenu.MENU_BUTTON_X => Buttons.X, 
				_ => throw new Exception("no pad mapping for this action " + action), 
			};
		}

		private static bool isActionMenuOnDPad(ActionMenu action)
		{
			switch (action)
			{
			case ActionMenu.MENU_UP:
			case ActionMenu.MENU_DOWN:
			case ActionMenu.MENU_LEFT:
			case ActionMenu.MENU_RIGHT:
				return true;
			default:
				return false;
			}
		}

		protected bool ButtonEventTriggered(ActionMenu action)
		{
			return ButtonEventTriggered(currentJoystickMenuAction, previousJoystickMenuAction, padState, padPreviousState, action);
		}

		public bool XboxButtonIsPushed(Buttons button)
		{
			return isPressed(button);
		}

		protected bool ButtonEventTriggered(ActionMenu joystickCurr, ActionMenu joystickPrev, GamePadState stateCurr, GamePadState statePrev, ActionMenu action)
		{
			if (action == ActionMenu.NONE)
			{
				return false;
			}
			if (joystickPrev == action && joystickCurr == ActionMenu.NONE)
			{
				return true;
			}
			if (isActionMenuOnDPad(action))
			{
				return justReleased(stateCurr, statePrev, DPadMapping_Menu(action));
			}
			return justReleased(stateCurr, statePrev, padMapping_Menu(action));
		}

		protected bool ButtonEventTriggered(ActionMenu action, bool allowRepeatScroll, bool allowRepeatActivate, int slowerRepeatRatio, int currentFrame)
		{
			if (action == ActionMenu.MENU_ACTIVATE)
			{
				if (allowRepeatActivate)
				{
					if (currentFrame % (frameIntervaleForRepeat * slowerRepeatRatio) == 0)
					{
						return isPressed(padMapping_Menu(ActionMenu.MENU_ACTIVATE));
					}
					return false;
				}
				return ButtonEventTriggered(ActionMenu.MENU_ACTIVATE);
			}
			if (!allowRepeatScroll)
			{
				return ButtonEventTriggered(action);
			}
			switch (action)
			{
			case ActionMenu.MENU_UP:
			case ActionMenu.MENU_DOWN:
			case ActionMenu.MENU_LEFT:
			case ActionMenu.MENU_RIGHT:
				if (currentFrame % (frameIntervaleForRepeat * (slowerRepeatRatio + 1)) == 0)
				{
					if (currentJoystickMenuAction == action)
					{
						return true;
					}
					return isPressed(DPadMapping_Menu(action));
				}
				return false;
			default:
				return ButtonEventTriggered(action);
			}
		}

		public static ActionMenu joystickAnalogMenuIsPressed(Vector2 stick)
		{
			ActionMenu result = ActionMenu.NONE;
			if (stick == Vector2.Zero)
			{
				return result;
			}
			float x = stick.X;
			float y = stick.Y;
			float num = stick.Length();
			Vector2.Normalize(stick);
			x = stick.X;
			y = stick.Y;
			if (num > 0.33f)
			{
				float num2 = MathHelper.ToDegrees((float)Math.Acos(x));
				if (Math.Abs(num2 - 0f) < 30f)
				{
					result = ActionMenu.MENU_RIGHT;
				}
				else if (Math.Abs(Math.Abs(num2) - 180f) < 30f)
				{
					result = ActionMenu.MENU_LEFT;
				}
				else
				{
					num2 = MathHelper.ToDegrees((float)Math.Asin(y));
					if (Math.Abs(num2 - 90f) < 30f)
					{
						result = ActionMenu.MENU_UP;
					}
					else if (Math.Abs(num2 - -90f) < 30f)
					{
						result = ActionMenu.MENU_DOWN;
					}
				}
			}
			return result;
		}

		public Input()
		{
			padIndexFound = false;
			playerIndex = (PlayerIndex)(-1);
			currentJoystickMenuAction = ActionMenu.NONE;
		}

		public void InitializePost()
		{
		}

		protected bool CheckPlayerIndexFoundIfCondition(GameTime gameTime, bool condition)
		{
			if (!padIndexFound)
			{
				if (condition)
				{
					findPadIndex(gameTime);
				}
				return false;
			}
			return true;
		}

		protected virtual bool GetCurrentState()
		{
			currentJoystickMenuAction = joystickAnalogMenuIsPressed(currentStick);
			padState = padGetState(playerIndex);
			return padState.IsConnected;
		}

		public static GamePadState padGetState(PlayerIndex playerIndex)
		{
			return GamePad.GetState(playerIndex, GamePadDeadZone.Circular);
		}

		protected bool isPressed(Buttons b)
		{
			return isPressed(padState, b);
		}

		protected bool isPressed(GamePadState state, Buttons b)
		{
			return state.IsButtonDown(b);
		}

		protected bool wasPressed(Buttons b)
		{
			return padPreviousState.IsButtonDown(b);
		}

		protected bool justReleased(Buttons b)
		{
			return justReleased(padState, padPreviousState, b);
		}

		protected bool justReleased(GamePadState current, GamePadState previous, Buttons b)
		{
			if (b == (Buttons)(-1))
			{
				return false;
			}
			if (previous.IsButtonDown(b))
			{
				return !current.IsButtonDown(b);
			}
			return false;
		}

		protected bool justPressed(Buttons b)
		{
			if (!padPreviousState.IsButtonDown(b))
			{
				return padState.IsButtonDown(b);
			}
			return false;
		}

		protected bool justReleased(DPadButton button)
		{
			return justReleased(padState, padPreviousState, button);
		}

		protected bool justReleased(GamePadState current, GamePadState previous, DPadButton button)
		{
			switch (button)
			{
			case DPadButton.Up:
				if (previous.DPad.Up == ButtonState.Pressed)
				{
					return current.DPad.Up == ButtonState.Released;
				}
				return false;
			case DPadButton.Down:
				if (previous.DPad.Down == ButtonState.Pressed)
				{
					return current.DPad.Down == ButtonState.Released;
				}
				return false;
			case DPadButton.Left:
				if (previous.DPad.Left == ButtonState.Pressed)
				{
					return current.DPad.Left == ButtonState.Released;
				}
				return false;
			case DPadButton.Right:
				if (previous.DPad.Right == ButtonState.Pressed)
				{
					return current.DPad.Right == ButtonState.Released;
				}
				return false;
			default:
				return false;
			}
		}

		protected bool isPressed(DPadButton button)
		{
			return isPressed(padState, button);
		}

		protected bool isPressed(GamePadState state, DPadButton button)
		{
			return button switch
			{
				DPadButton.Up => state.DPad.Up == ButtonState.Pressed, 
				DPadButton.Down => state.DPad.Down == ButtonState.Pressed, 
				DPadButton.Left => state.DPad.Left == ButtonState.Pressed, 
				DPadButton.Right => state.DPad.Right == ButtonState.Pressed, 
				_ => false, 
			};
		}

		protected void findPadIndex(GameTime gameTime)
		{
			bool flag = false;
			for (PlayerIndex playerIndex = PlayerIndex.One; playerIndex <= PlayerIndex.Four; playerIndex++)
			{
				padPreviousState = padPreviousStatesForFindPad[(int)playerIndex];
				padState = padGetState(playerIndex);
				ref GamePadState reference = ref padPreviousStatesForFindPad[(int)playerIndex];
				reference = padState;
				if (!flag && padPreviousState.Buttons.A == ButtonState.Pressed && padState.Buttons.A == ButtonState.Released)
				{
					flag = true;
				}
				if (!flag && padPreviousState.Buttons.Start == ButtonState.Pressed && padState.Buttons.Start == ButtonState.Released)
				{
					flag = true;
				}
				if (flag)
				{
					this.playerIndex = playerIndex;
					break;
				}
			}
			if (flag)
			{
				padIndexFound = true;
				timerAfterFindPadMs = gameTime.TotalGameTime.TotalMilliseconds + 100.00000149011612;
			}
		}

		protected void UpdatePreviousStates()
		{
			padPreviousState = padState;
			previousJoystickMenuAction = currentJoystickMenuAction;
		}
	}

	public class InitializeGraphics
	{
		private static bool IsWidthSupported(DisplayModeCollection supportedModes, int preferredW)
		{
			foreach (DisplayMode supportedMode in supportedModes)
			{
				if (supportedMode.Width >= preferredW)
				{
					return true;
				}
			}
			return false;
		}

		public static void InitializeDevice(GraphicsDeviceManager device, int prefferedW, string gameName, float useThisRatio, bool antiAliasing, bool fullscreen)
		{
			_ = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
			_ = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
			if (prefferedW > 1280 && !IsWidthSupported(GraphicsAdapter.DefaultAdapter.SupportedDisplayModes, prefferedW))
			{
				prefferedW = 1280;
			}
			device.PreferredBackBufferWidth = prefferedW;
			device.PreferredBackBufferHeight = (int)((float)prefferedW / useThisRatio) + 1;
			device.PreferMultiSampling = antiAliasing;
			device.PreparingDeviceSettings += delegate(object sender, PreparingDeviceSettingsEventArgs e)
			{
				PresentationParameters presentationParameters = e.GraphicsDeviceInformation.PresentationParameters;
				try
				{
					presentationParameters.MultiSampleCount = ((!antiAliasing) ? 8 : 0);
				}
				catch (Exception)
				{
					try
					{
						presentationParameters.MultiSampleCount = 4;
					}
					catch (Exception)
					{
						try
						{
							presentationParameters.MultiSampleCount = 2;
						}
						catch (Exception)
						{
							presentationParameters.MultiSampleCount = 0;
						}
					}
				}
			};
		}
	}

	public class Record
	{
		public bool IsWritingToDisk => false;

		public Record(GraphicsDevice d, string gameName, int width, int height)
		{
		}

		public void EventStartStop()
		{
		}

		public void Draw()
		{
		}

		private void RecordThread()
		{
		}

		private void WriteAndFreeMemory()
		{
		}
	}

	public class StringDrawer
	{
		public enum Type
		{
			SpriteFont,
			BitmapFont
		}

		private Type type;

		private bool initialized;

		private SpriteFont fontS;

		private BitmapFont fontB;

		private SpriteBatch batch;

		public Type getType => type;

		public int LineSpacing
		{
			get
			{
				if (type == Type.SpriteFont)
				{
					return fontS.LineSpacing;
				}
				if (type == Type.BitmapFont)
				{
					return (int)((float)fontB.LineHeight * 1.5f);
				}
				throw new Exception("type unknonw " + type);
			}
		}

		public SpriteFont SpriteFont
		{
			get
			{
				if (type != Type.SpriteFont)
				{
					throw new Exception("type not SpriteFont : " + type);
				}
				return fontS;
			}
		}

		public void Reset(SpriteFont font, SpriteBatch batch)
		{
			type = Type.SpriteFont;
			fontS = font;
			this.batch = batch;
			initialized = true;
		}

		public void Reset(BitmapFont font, GraphicsDevice device)
		{
			Reset(font, device, null);
		}

		public void Reset(BitmapFont font, GraphicsDevice device, SpriteBatch batch)
		{
			type = Type.BitmapFont;
			fontB = font;
			if (batch != null)
			{
				fontB.SpriteBatchOverride(batch);
			}
			fontB.Reset(device);
			initialized = true;
		}

		public void UpdateSpriteBatch(SpriteBatch batch)
		{
			if (!initialized)
			{
				throw new Exception("not initialized");
			}
			this.batch = batch;
			if (type == Type.BitmapFont)
			{
				fontB.SpriteBatchOverride(batch);
			}
		}

		public void Draw(string s, Vector2 pos, Color col, int lineSpacingOverride)
		{
			Draw(s, pos, col, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f, lineSpacingOverride);
		}

		public void Draw(string s, Vector2 pos, Color col)
		{
			Draw(s, pos, col, -1);
		}

		public void Draw(string s, Vector2 pos, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
		{
			Draw(s, pos, color, rotation, origin, scale, effects, layerDepth, -1);
		}

		public void Draw(string s, Vector2 pos, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth, int lineSpacingOverride)
		{
			if (s.Contains(newLine))
			{
				string[] array = splitString(s, newLineChar);
				Vector2 pos2 = pos;
				string[] array2 = array;
				foreach (string s2 in array2)
				{
					Draw(s2, pos2, color, rotation, origin, scale, effects, layerDepth, lineSpacingOverride);
					pos2.Y += ((lineSpacingOverride != -1) ? lineSpacingOverride : LineSpacing);
				}
				return;
			}
			if (s.Contains(newLine))
			{
				throw new Exception("BitmapFont does not support multiple lines");
			}
			if (!initialized)
			{
				throw new Exception("not initialized");
			}
			if (type == Type.SpriteFont)
			{
				batch.DrawString(fontS, s, pos, color, rotation, origin, scale, effects, layerDepth);
				return;
			}
			if (type == Type.BitmapFont)
			{
				Vector2 v = pos - origin;
				fontB.DrawString(v, color, s);
				return;
			}
			throw new Exception("type unknonw " + type);
		}

		public Vector2 MeasureString(string s)
		{
			return MeasureString(s, -1);
		}

		public Vector2 MeasureString(string s, int lineSpacingOverride)
		{
			if (!initialized)
			{
				throw new Exception("not initialized");
			}
			if (type == Type.SpriteFont)
			{
				try
				{
					return fontS.MeasureString(s);
				}
				catch (Exception ex)
				{
					throw new Exception(ex.Message, ex);
				}
			}
			if (type == Type.BitmapFont)
			{
				int num = ((lineSpacingOverride != -1) ? lineSpacingOverride : LineSpacing);
				return new Vector2(fontB.MeasureStringWidth(s), fontB.LineHeight * (1 + StringCountCharOccurences(s, newLineChar)) + (num - fontB.LineHeight) * StringCountCharOccurences(s, newLineChar));
			}
			throw new Exception("type unknonw " + type);
		}

		public Vector2 FontDecalageForHalfHeight(string str)
		{
			if (!initialized)
			{
				throw new Exception("not initialized");
			}
			string[] array = str.Split(newLine.ToCharArray());
			int num = ((!str.Contains(newLine)) ? ((int)MeasureString(str).Y) : ((int)MeasureString(array[0]).Y));
			float num2 = ((type == Type.SpriteFont) ? 0.5f : 0.5f);
			return new Vector2(0f, (int)((float)num * num2));
		}

		public bool ContainsChar(char c)
		{
			if (type == Type.SpriteFont)
			{
				return fontS.Characters.Contains(c);
			}
			if (type == Type.BitmapFont)
			{
				return true;
			}
			throw new Exception("type unknown " + type);
		}
	}

	public class StringWithDrawer
	{
		public string s;

		public StringDrawer drawer;

		public StringWithDrawer()
		{
			s = "";
			drawer = null;
		}

		public StringWithDrawer(string s, StringDrawer d)
		{
			this.s = s;
			drawer = d;
		}
	}

	public class StringWithMultipleDrawer
	{
		private List<StringWithDrawer> list;

		public Vector2 ScreenEstate
		{
			get
			{
				Vector2 zero = Vector2.Zero;
				foreach (StringWithDrawer item in list)
				{
					Vector2 vector = item.drawer.MeasureString(item.s);
					zero.X += vector.X;
					zero.Y = Math.Max(zero.Y, vector.Y);
				}
				return zero;
			}
		}

		public void Initialize(List<StringWithDrawer> list)
		{
			this.list = list;
		}

		public void Initialize_3items(string item1, string item2, string item3, StringDrawer drawer1, StringDrawer drawer2, StringDrawer drawer3)
		{
			list = new List<StringWithDrawer>();
			list.Add(new StringWithDrawer(item1, drawer1));
			list.Add(new StringWithDrawer(item2, drawer2));
			list.Add(new StringWithDrawer(item3, drawer3));
		}

		public void Initialize_2items(string item1, string item2, StringDrawer drawer1, StringDrawer drawer2)
		{
			list = new List<StringWithDrawer>();
			list.Add(new StringWithDrawer(item1, drawer1));
			list.Add(new StringWithDrawer(item2, drawer2));
		}

		public void Draw(Vector2 startPos, Color color)
		{
			Vector2 pos = startPos;
			foreach (StringWithDrawer item in list)
			{
				item.drawer.Draw(item.s, pos, color);
				pos.X += item.drawer.MeasureString(item.s).X;
			}
		}

		public void WrapWidthOfLastItem(int Wmax)
		{
			StringWithDrawer stringWithDrawer = list[list.Count - 1];
			int num = Wmax;
			for (int i = 0; i < list.Count - 2; i++)
			{
				num -= (int)list[i].drawer.MeasureString(list[i].s).X;
			}
			stringWithDrawer.s = WrapStringWidth(stringWithDrawer.s, stringWithDrawer.drawer, num);
		}

		public static Vector2 ScreenEstateLines(List<StringWithMultipleDrawer> lines)
		{
			Vector2 zero = Vector2.Zero;
			foreach (StringWithMultipleDrawer line in lines)
			{
				Vector2 screenEstate = line.ScreenEstate;
				zero.X = Math.Max(zero.X, screenEstate.X);
				zero.Y += screenEstate.Y;
			}
			return zero;
		}
	}

	public class Textures
	{
		public enum TexSize
		{
			HD,
			SD,
			Independant
		}

		public static Texture2D ReplaceTransparencyWithColor(Texture2D t, GraphicsDevice d, Color c)
		{
			Texture2D texture2D = new Texture2D(d, t.Width, t.Height, mipMap: false, t.Format);
			int num = t.Width * t.Height;
			Color[] array = new Color[num];
			Color[] array2 = new Color[num];
			t.GetData(array);
			for (int i = 0; i < num; i++)
			{
				if (array[i].A == 0)
				{
					ref Color reference = ref array2[i];
					reference = Color.White;
				}
				else
				{
					ref Color reference2 = ref array2[i];
					reference2 = array[i];
				}
			}
			texture2D.SetData(array2);
			return texture2D;
		}

		public static Texture2D Invert(Texture2D t, GraphicsDevice d)
		{
			Texture2D texture2D = new Texture2D(d, t.Width, t.Height, mipMap: false, t.Format);
			if (t.LevelCount != 1 || t.Format != SurfaceFormat.Color)
			{
				return texture2D;
			}
			Color[] array = new Color[t.Width * t.Height];
			Color[] array2 = new Color[t.Width * t.Height];
			t.GetData(array2);
			for (int i = 0; i < t.Width * t.Height; i++)
			{
				ref Color reference = ref array[i];
				reference = new Color((byte)(255 - array2[i].R), (byte)(255 - array2[i].G), (byte)(255 - array2[i].B), array2[i].A);
			}
			texture2D.SetData(array);
			return texture2D;
		}

		public static int WidthFromHeightAndRatio(Texture2D t, int h)
		{
			return (int)((float)h * ((float)t.Width / (float)t.Height));
		}

		public static int HeightFromWidthAndRatio(Texture2D t, int w)
		{
			return (int)((float)w / ((float)t.Width / (float)t.Height));
		}

		public static string TexSizeString(TexSize subDir)
		{
			return subDir switch
			{
				TexSize.HD => "HD/", 
				TexSize.SD => "SD/", 
				TexSize.Independant => "common/", 
				_ => throw new Exception("TexSize unknown " + subDir), 
			};
		}

		public static bool isSizePowerOfTwo(Texture2D tex)
		{
			if (isIntegerPowerOfTwo(tex.Width))
			{
				return isIntegerPowerOfTwo(tex.Height);
			}
			return false;
		}

		public static Texture2D LoadTex(ContentManager content, string subDir, TexSize texSize, string name)
		{
			string text = subDir + ((subDir == "") ? "" : "/");
			string text2 = text + TexSizeString(texSize) + name;
			try
			{
				return content.Load<Texture2D>(text2);
			}
			catch (Exception)
			{
				string text3 = "textures/failsafe";
				try
				{
					return content.Load<Texture2D>(text3);
				}
				catch (Exception innerException)
				{
					throw new Exception("failed load textures '" + text2 + "' then '" + text3 + "' ", innerException);
				}
			}
		}

		public static Texture2D FromSheet(GraphicsDevice dev, Color[] sheet, Vector2 sheetSize, Rectangle rect)
		{
			Texture2D texture2D = new Texture2D(dev, rect.Width, rect.Height);
			Color[] array = new Color[rect.Width * rect.Height];
			int num = 0;
			for (int i = rect.Y; i < rect.Y + rect.Height; i++)
			{
				for (int j = rect.X; j < rect.X + rect.Width; j++)
				{
					ref Color reference = ref array[num++];
					reference = sheet[i * (int)sheetSize.X + j];
				}
			}
			texture2D.SetData(array);
			return texture2D;
		}
	}

	public class UI
	{
		public class VirtualKeyboard
		{
			public enum Action
			{
				MENU_UP,
				MENU_DOWN,
				MENU_LEFT,
				MENU_RIGHT,
				MENU_BACK,
				ACTIVATE,
				BUTTON_X
			}

			private string value;

			private bool finished;

			private GraphicsDevice device;

			private StringDrawer stringDrawer;

			private Texture2D texKeySmall;

			private Texture2D texKeySmallSelected;

			private Texture2D texKeyLarge;

			private Texture2D texKeyLargeSelected;

			private Texture2D texReturn;

			private int nbLettersName;

			private static char charReturn = '@';

			private int keySize = -1;

			private int pad = -1;

			private int selectLine;

			private int selectColumn;

			private static char[] lettersLine0 = new char[10] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };

			private static char[] lettersLine1 = new char[10] { 'Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P' };

			private static char[] lettersLine2 = new char[9] { 'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L' };

			private static char[] lettersLine3 = new char[7] { 'Z', 'X', 'C', 'V', 'B', 'N', 'M' };

			private static char[] lettersLine4 = new char[2] { ' ', charReturn };

			private static char[][] letters = new char[5][] { lettersLine0, lettersLine1, lettersLine2, lettersLine3, lettersLine4 };

			private static int maxLettersByLine = countMaxLettersByLine();

			public bool ValueLengthIsNotZero => value.Length != 0;

			public int Width => pad + (keySize + pad) * maxLettersByLine;

			public int Height => 3 * stringDrawer.LineSpacing + pad + (keySize + pad) * letters.Length;

			public string Value => value;

			private string Message => newLine + value + newLine;

			public bool Finished => finished;

			public VirtualKeyboard(Texture2D texKeySmall, Texture2D texKeyLarge, Texture2D texReturn)
			{
				this.texKeySmall = texKeySmall;
				this.texKeyLarge = texKeyLarge;
				this.texReturn = texReturn;
				value = "";
			}

			public void SetDefaultValue(string defaultValue)
			{
				value = defaultValue;
			}

			public void HandleInput(Input.ActionMenu action)
			{
				if (device == null || finished)
				{
					return;
				}
				switch (action)
				{
				case Input.ActionMenu.MENU_UP:
					selectLine--;
					if (selectLine == -1)
					{
						selectLine = letters.Length - 1;
					}
					if (selectColumn >= letters[selectLine].Length)
					{
						selectColumn = letters[selectLine].Length - 1;
					}
					break;
				case Input.ActionMenu.MENU_DOWN:
					selectLine++;
					if (selectLine == letters.Length)
					{
						selectLine = 0;
					}
					if (selectColumn >= letters[selectLine].Length)
					{
						selectColumn = letters[selectLine].Length - 1;
					}
					break;
				case Input.ActionMenu.MENU_LEFT:
					selectColumn--;
					if (selectColumn == -1)
					{
						selectColumn = letters[selectLine].Length - 1;
					}
					break;
				case Input.ActionMenu.MENU_RIGHT:
					selectColumn++;
					if (selectColumn == letters[selectLine].Length)
					{
						selectColumn = 0;
					}
					break;
				case Input.ActionMenu.MENU_ACTIVATE:
					if (letters[selectLine][selectColumn] == charReturn)
					{
						finished = true;
					}
					else if (value.Length < nbLettersName)
					{
						value += letters[selectLine][selectColumn];
					}
					else
					{
						finished = true;
					}
					break;
				case Input.ActionMenu.MENU_BACK:
					if (value.Length != 0)
					{
						value = StringRemoveLastChar(value);
					}
					break;
				case Input.ActionMenu.MENU_BUTTON_X:
					finished = true;
					break;
				}
			}

			public void render(GameTime gameTime, int screenWidth, int screenHeight, int X, int Y, byte alpha, SpriteBatch begunSpriteBatch)
			{
				Color color = ColorWithAlpha(Color.White, alpha);
				Color color2 = ColorWithAlpha(Color.Black, alpha);
				stringDrawer.Draw(Message, new Vector2((float)(screenWidth / 2) - stringDrawer.MeasureString(value).X / 2f, Y + pad), color2);
				int num = Y + pad + 3 * stringDrawer.LineSpacing;
				int num2 = (int)((float)keySize * 0.33f);
				int num3 = 0;
				int num4 = 0;
				char[][] array = letters;
				foreach (char[] array2 in array)
				{
					int num5 = X + pad + num2 * num3++;
					int num6 = 0;
					char[] array3 = array2;
					foreach (char c in array3)
					{
						Rectangle destinationRectangle = new Rectangle(num5, num, keySize, keySize);
						Vector2 pos = new Vector2((float)num5 + (float)pad * 0.75f, num + pad / 2);
						bool flag = num6 == selectColumn && num4 == selectLine;
						if (letters[num4][num6] != ' ')
						{
							if (letters[num4][num6] != charReturn)
							{
								begunSpriteBatch.Draw(flag ? texKeySmallSelected : texKeySmall, destinationRectangle, null, color, 0f, Vector2.Zero, SpriteEffects.None, 1f);
								stringDrawer.Draw(c.ToString(), pos, flag ? color : color2);
							}
							else
							{
								Rectangle destinationRectangle2 = new Rectangle(destinationRectangle.X + 5 * (keySize + pad), destinationRectangle.Y, destinationRectangle.Width, destinationRectangle.Height);
								begunSpriteBatch.Draw(flag ? texKeySmallSelected : texKeySmall, destinationRectangle2, null, color, 0f, Vector2.Zero, SpriteEffects.None, 1f);
								begunSpriteBatch.Draw(texReturn, destinationRectangle2, null, color, 0f, Vector2.Zero, SpriteEffects.None, 1f);
							}
						}
						else
						{
							begunSpriteBatch.Draw(flag ? texKeyLargeSelected : texKeyLarge, new Rectangle(destinationRectangle.X + 2 * (keySize + pad), destinationRectangle.Y, Textures.WidthFromHeightAndRatio(texKeyLarge, destinationRectangle.Height), destinationRectangle.Height), null, color, 0f, Vector2.Zero, SpriteEffects.None, 1f);
						}
						num5 += keySize + pad;
						num6++;
					}
					num += keySize + pad;
					num4++;
				}
			}

			public void Initialize(GraphicsDevice device, StringDrawer stringDrawer, int nbLettersName)
			{
				this.device = device;
				this.stringDrawer = stringDrawer;
				this.nbLettersName = nbLettersName;
				bool flag = true;
				string text = value;
				foreach (char c in text)
				{
					flag = flag && stringDrawer.ContainsChar(c);
				}
				if (!flag)
				{
					value = "";
				}
				int num = (int)stringDrawer.MeasureString("A").Y;
				keySize = (int)((float)num * 1.5f);
				pad = (int)((float)keySize * 0.33f);
				selectLine = 2;
				selectColumn = 0;
				texKeySmallSelected = Textures.Invert(texKeySmall, device);
				texKeyLargeSelected = Textures.Invert(texKeyLarge, device);
				finished = false;
			}

			private static int countMaxLettersByLine()
			{
				int num = 0;
				char[][] array = letters;
				foreach (char[] array2 in array)
				{
					num = Math.Max(num, array2.Length);
				}
				return num;
			}
		}
	}

	public static readonly float SQRT2 = (float)Math.Sqrt(2.0);

	private static Vector2[] IsVisibleData = new Vector2[4];

	public static readonly Color ColorTransparentWhite = new Color(Color.White.R, Color.White.G, Color.White.B, 0);

	public static int TargetFPS = 60;

	private static Random random = new Random((int)DateTime.Now.Ticks);

	public static string newLine = "\n";

	public static char newLineChar = '\n';

	public static float ratioMileToKm = 1.609344f;

	private static CultureInfo ParsingCulture = new CultureInfo("en-US");

	public static bool IsXboxGuideActive => Guide.IsVisible;

	public static Random Random => random;

	public static bool RandomBool => random.Next(2) == 0;

	public static float RandomRatio => (float)random.NextDouble();

	public static string DateTimeString
	{
		get
		{
			DateTime now = DateTime.Now;
			return now.Year + "-" + now.Month + "-" + now.Day + "-" + now.Hour + "h" + now.Minute + "m" + now.Second + "s";
		}
	}

	public static Color ColorVariationOf(Color baseColor, Vector3 variance)
	{
		return new Color(baseColor.ToVector3() + Vector3.UnitX * RandomBetween(0f - variance.X, variance.X) + Vector3.UnitY * RandomBetween(0f - variance.X, variance.X) + Vector3.UnitZ * RandomBetween(0f - variance.X, variance.X));
	}

	public static int Modulo(int value, int mod)
	{
		if (value >= 0)
		{
			return value % mod;
		}
		do
		{
			value += mod;
		}
		while (value < 0);
		return value;
	}

	public static void Rectangle_GetCorners(Rectangle rec, Point[] ret)
	{
		ref Point reference = ref ret[0];
		reference = new Point(rec.X, rec.Y);
		ref Point reference2 = ref ret[1];
		reference2 = ret[0];
		ret[1].X += rec.Width;
		ref Point reference3 = ref ret[2];
		reference3 = ret[0];
		ret[2].Y += rec.Height;
		ref Point reference4 = ref ret[3];
		reference4 = ret[0];
		ret[3].X += rec.Width;
		ret[3].Y += rec.Height;
	}

	public static string StringVec3(Vector3 v)
	{
		return "[X " + v.X.ToString("000.000") + " - " + v.Y.ToString("000.000") + " - " + v.Z.ToString("000.000") + "]";
	}

	public static Rectangle RectangleFromCenterAndSize(Point center, Point size)
	{
		return new Rectangle(center.X - size.X / 2, center.Y - size.Y / 2, size.X, size.Y);
	}

	public static Vector2 MoveInCircle(GameTime gameTime, float speed)
	{
		double num = gameTime.TotalGameTime.TotalSeconds * (double)speed;
		float x = (float)Math.Cos(num);
		float y = (float)Math.Sin(num);
		return new Vector2(x, y);
	}

	public static bool IsVisible(Vector2 pos, float scale, float alpha, int texX, int texY, bool canRotate, Point screenSize)
	{
		if (alpha == 0f)
		{
			return false;
		}
		float num = scale * (canRotate ? SQRT2 : 1f);
		bool flag = false;
		int num2 = (canRotate ? Math.Max(texX / 2, texY / 2) : (texX / 2));
		int num3 = (canRotate ? Math.Max(texX / 2, texY / 2) : (texY / 2));
		ref Vector2 reference = ref IsVisibleData[0];
		reference = new Vector2(pos.X - num * (float)num2, pos.Y - num * (float)num3);
		ref Vector2 reference2 = ref IsVisibleData[1];
		reference2 = new Vector2(pos.X - num * (float)num2, pos.Y + num * (float)num3);
		ref Vector2 reference3 = ref IsVisibleData[2];
		reference3 = new Vector2(pos.X + num * (float)num2, pos.Y - num * (float)num3);
		ref Vector2 reference4 = ref IsVisibleData[3];
		reference4 = new Vector2(pos.X + num * (float)num2, pos.Y + num * (float)num3);
		Vector2[] isVisibleData = IsVisibleData;
		for (int i = 0; i < isVisibleData.Length; i++)
		{
			Vector2 vector = isVisibleData[i];
			flag |= vector.X >= 0f && vector.X < (float)screenSize.X && vector.Y >= 0f && vector.Y < (float)screenSize.Y;
		}
		if (!flag)
		{
			ref Vector2 reference5 = ref IsVisibleData[0];
			reference5 = new Vector2(pos.X, pos.Y - num * (float)num3);
			ref Vector2 reference6 = ref IsVisibleData[1];
			reference6 = new Vector2(pos.X, pos.Y + num * (float)num3);
			ref Vector2 reference7 = ref IsVisibleData[2];
			reference7 = new Vector2(pos.X + num * (float)num2, pos.Y);
			ref Vector2 reference8 = ref IsVisibleData[3];
			reference8 = new Vector2(pos.X - num * (float)num2, pos.Y);
			Vector2[] isVisibleData2 = IsVisibleData;
			for (int j = 0; j < isVisibleData2.Length; j++)
			{
				Vector2 vector2 = isVisibleData2[j];
				flag |= vector2.X >= 0f && vector2.X < (float)screenSize.X && vector2.Y >= 0f && vector2.Y < (float)screenSize.Y;
			}
		}
		return flag;
	}

	public static int IndexOnTime_Increasing(int nbFramesPeriod, int maxIndex)
	{
		return (int)((float)MaximinusGame.CurrentFrame / (float)nbFramesPeriod) % maxIndex;
	}

	public static int IndexOnTime_Decreasing(int nbFramesPeriod, int maxIndex)
	{
		return maxIndex - 1 - IndexOnTime_Increasing(nbFramesPeriod, maxIndex);
	}

	public static Point Vec2ToPoint(Vector2 v)
	{
		return new Point((int)v.X, (int)v.Y);
	}

	public static Vector2 PointToVec2(Point v)
	{
		return new Vector2(v.X, v.Y);
	}

	public static float PowerCurve(float value, float power)
	{
		return (float)Math.Pow(Math.Abs(value), power) * (float)Math.Sign(value);
	}

	public static float PowerCurveInverse(float value, int power)
	{
		float num = value;
		int num2 = 0;
		while (num2 < power)
		{
			num2++;
			num = (float)Math.Sqrt(num);
		}
		return num;
	}

	public static Color ColorFromHexaString(string hexaStr)
	{
		string text = hexaStr;
		while (text.Length < 6)
		{
			text = "0" + text;
		}
		int r = Convert.ToInt32(new string(new char[2]
		{
			text[0],
			text[1]
		}), 16);
		int g = Convert.ToInt32(new string(new char[2]
		{
			text[2],
			text[3]
		}), 16);
		int b = Convert.ToInt32(new string(new char[2]
		{
			text[4],
			text[5]
		}), 16);
		return new Color(r, g, b);
	}

	public static Color ColorFromPackedValue(uint packedValue)
	{
		Color result = new Color(0, 0, 0);
		result.PackedValue = packedValue;
		return result;
	}

	public static string DebugOutStr(string s)
	{
		return "";
	}

	public static void DebugOut(string s)
	{
	}

	public static void assertStatic(bool b, string s)
	{
	}

	public static float PointDistance(Point P0, Point P1)
	{
		return Vector2.Distance(new Vector2(P0.X, P0.Y), new Vector2(P1.X, P1.Y));
	}

	public static void Swap(ref int i0, ref int i1)
	{
		int num = i0;
		i0 = i1;
		i1 = num;
	}

	public static Color ColorWithAlpha(Color c, int A)
	{
		return new Color(c.R, c.G, c.B, A);
	}

	public static Color ColorWithAlpha(Color c, float A)
	{
		return c * A;
	}

	public static Vector3 CatmullRom3D(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float amount)
	{
		return new Vector3
		{
			X = MathHelper.CatmullRom(v1.X, v2.X, v3.X, v4.X, amount),
			Y = MathHelper.CatmullRom(v1.Y, v2.Y, v3.Y, v4.Y, amount),
			Z = MathHelper.CatmullRom(v1.Z, v2.Z, v3.Z, v4.Z, amount)
		};
	}

	public static double AngleRadInTwoPiRange(double angleRad)
	{
		return AngleRadInPiRange(angleRad) + Math.PI;
	}

	public static double AngleRadInPiRange(double angleRad)
	{
		double num;
		for (num = angleRad; num > Math.PI; num -= Math.PI * 2.0)
		{
		}
		for (; num < -Math.PI; num += Math.PI * 2.0)
		{
		}
		return num;
	}

	public static string Format_Vector3_XY(Vector3 v, string format)
	{
		return Format_Vector2(new Vector2(v.X, v.Y), format);
	}

	public static Vector3 Vector2to3(Vector2 v, float Z)
	{
		return new Vector3(v.X, v.Y, Z);
	}

	public static string Format_Vector2(Vector2 v, string format)
	{
		return '[' + v.X.ToString(format) + '/' + v.Y.ToString(format) + ']';
	}

	public static void ShowMarketPlace_DontRaiseException(PlayerIndex playerIndex)
	{
		try
		{
			Guide.ShowMarketplace(playerIndex);
		}
		catch (Exception)
		{
		}
	}

	public static bool IntToBool(int i)
	{
		if (i != 0)
		{
			return true;
		}
		return false;
	}

	public static int BoolToInt(bool b)
	{
		if (!b)
		{
			return 0;
		}
		return 1;
	}

	public static void RandomReset()
	{
		random = new Random((int)DateTime.Now.Ticks);
	}

	public static float RandomBetween(float min, float max)
	{
		return min + (float)random.NextDouble() * (max - min);
	}

	public static bool OneChanceOutOf(int nbchances)
	{
		return random.Next(nbchances) == 0;
	}

	public static int ElapsedMilliseconds(GameTime gameTime, bool isFixedTimeStep)
	{
		return gameTime.ElapsedGameTime.Milliseconds;
	}

	public static int TotalMilliseconds(GameTime gameTime, bool isFixedTimeStep)
	{
		return gameTime.TotalGameTime.Milliseconds;
	}

	public static string ReplaceChar(string s, char oldChar, string newChar)
	{
		if (!s.Contains(oldChar))
		{
			return s;
		}
		string text = "";
		char[] array = s.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			text = ((array[i] != oldChar) ? (text + array[i]) : (text + newChar));
		}
		return text;
	}

	public static string RemoveTrailingWhiteChars(string s)
	{
		string text = "";
		bool flag = false;
		foreach (char c in s)
		{
			if (flag)
			{
				text += ' ';
				flag = false;
			}
			if (c == ' ')
			{
				flag = true;
			}
			else
			{
				text += c;
			}
		}
		return text;
	}

	public static double MileToKm(double m)
	{
		return m * (double)ratioMileToKm;
	}

	public static double KmToMile(double k)
	{
		return k / (double)ratioMileToKm;
	}

	public static bool isIntegerPowerOfTwo(int i)
	{
		if (i % 2 != 0)
		{
			return false;
		}
		if (i == 2)
		{
			return true;
		}
		return isIntegerPowerOfTwo(i / 2);
	}

	public static string StringRemoveLastChar(string s)
	{
		string text = "";
		for (int i = 0; i < s.Length - 1; i++)
		{
			text += s[i];
		}
		return text;
	}

	public static string formatDistance(Units units, double distanceMiles)
	{
		double num = distanceMiles;
		if (units == Units.Km)
		{
			num = MileToKm(num);
		}
		return formatIntegerWithSpaces((int)num);
	}

	public static string formatSpeed(float speedMilliSec)
	{
		return (speedMilliSec / 1000f).ToString("0.0");
	}

	public static float ParseFloat(string s)
	{
		float num = 0f;
		return Convert.ToSingle(s, ParsingCulture);
	}

	public static string formatIntegerWithSpaces(int I)
	{
		string text = I.ToString();
		string text2 = "";
		int num = 0;
		for (int num2 = text.Length - 1; num2 >= 0; num2--)
		{
			text2 = text[num2] + text2;
			if (num % 3 == 2 && num2 != 0)
			{
				text2 = " " + text2;
			}
			num++;
		}
		return text2;
	}

	public static string IntToBinaryString(int i)
	{
		string text = "";
		int num = i;
		while (num != 0)
		{
			if (num % 2 == 0)
			{
				text = "0" + text;
				num /= 2;
			}
			else
			{
				text = "1" + text;
				num = (num - 1) / 2;
			}
		}
		return text;
	}

	public static float SmoothStepRatio(float ratio)
	{
		return SmoothStepRatio(ratio, 1);
	}

	public static float SmoothStepRatio(float ratio, int nbSteps)
	{
		if (nbSteps < 1)
		{
			throw new Exception("recursion error");
		}
		if (nbSteps == 1)
		{
			return MathHelper.SmoothStep(0f, 1f, ratio);
		}
		return SmoothStepRatio(MathHelper.SmoothStep(0f, 1f, ratio), nbSteps - 1);
	}

	public static Rectangle SmoothStep(Rectangle r1, Rectangle r2, float r)
	{
		return new Rectangle((int)MathHelper.SmoothStep(r1.X, r2.X, r), (int)MathHelper.SmoothStep(r1.Y, r2.Y, r), (int)MathHelper.SmoothStep(r1.Width, r2.Width, r), (int)MathHelper.SmoothStep(r1.Height, r2.Height, r));
	}

	public static Vector2 SmoothStep(Vector2 v1, Vector2 v2, float r)
	{
		return new Vector2(MathHelper.SmoothStep(v1.X, v2.X, r), MathHelper.SmoothStep(v1.Y, v2.Y, r));
	}

	public static Vector3 SmoothStep(Vector3 v1, Vector3 v2, float r)
	{
		return new Vector3(MathHelper.SmoothStep(v1.X, v2.X, r), MathHelper.SmoothStep(v1.Y, v2.Y, r), MathHelper.SmoothStep(v1.Z, v2.Z, r));
	}

	public static string StrTail(string s, int n)
	{
		string[] array = s.Split(newLine.ToCharArray());
		if (array.Length < n)
		{
			return s;
		}
		int num = 0;
		string text = "";
		while (num < n)
		{
			text = text + array[num++] + newLine;
		}
		return text;
	}

	public static string WrapStringWidth(string s, StringDrawer drawer, int width)
	{
		string text = "";
		int num = 0;
		string[] array = splitStringWhiteSpace(s);
		foreach (string text2 in array)
		{
			string text3 = " ";
			int num2 = (int)drawer.MeasureString(text3).X;
			if (text2 == newLine)
			{
				num = 0;
				text += text2;
				continue;
			}
			int num3 = (int)drawer.MeasureString(text2 + text3).X;
			if (num3 <= width)
			{
				if (num + num3 <= width)
				{
					text += text2;
					if (text2 != newLine)
					{
						text += text3;
					}
					num += num3 + num2;
				}
				else
				{
					text = text + newLine + text2 + text3;
					num = num3 + num2;
				}
				continue;
			}
			char[] array2 = text2.ToCharArray();
			foreach (char c in array2)
			{
				string text4 = c.ToString();
				int num4 = (int)drawer.MeasureString(text4).X;
				if (num + num4 > width)
				{
					text += newLine;
					num = 0;
				}
				else
				{
					num += num4;
				}
				text += text4;
			}
		}
		return text;
	}

	public static string WrapStringHeight(string s, StringDrawer drawer, int height, int startLine)
	{
		string text = "";
		string[] array = s.Split(newLine.ToCharArray());
		int num = array.Count();
		if (startLine > num - 1)
		{
			throw new Exception("startLine > nbLines");
		}
		int i = startLine;
		int num2 = 0;
		bool flag = true;
		for (; i < num; i++)
		{
			if (!flag)
			{
				break;
			}
			int num3 = drawer.LineSpacing;
			if (num3 == 0)
			{
				num3 = (int)drawer.MeasureString(" ").Y;
			}
			num2 += num3;
			if (num2 > height)
			{
				flag = false;
				continue;
			}
			if (text != "")
			{
				text += newLine;
			}
			string text2 = array[i];
			if (text2 == "")
			{
				text2 = " ";
			}
			text += text2;
		}
		return text;
	}

	public static bool WrapStringHeight_IsItNecessary(string s, StringDrawer drawer, int height, int startLine)
	{
		string[] array = s.Split(newLine.ToCharArray());
		if (startLine > array.Count() - 1)
		{
			throw new Exception("startLine > nbLines");
		}
		int i = startLine;
		string text = "";
		for (; i < array.Count(); i++)
		{
			if (text != "")
			{
				text += newLine;
			}
			string text2 = array[i];
			text = ((!(text2 == "")) ? (text + text2) : (text + newLine));
		}
		try
		{
			return drawer.MeasureString(text).Y > (float)height;
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message, ex);
		}
	}

	public static int StringCountCharOccurences(string s, char separator)
	{
		int num = 0;
		foreach (char c in s)
		{
			if (c == separator)
			{
				num++;
			}
		}
		return num;
	}

	public static string[] splitString(string s, char separator)
	{
		return splitString(s, separator, StringCountCharOccurences(s, separator) + 1);
	}

	public static string[] splitString(string s, char separator, int nbItems)
	{
		string[] array = new string[nbItems];
		try
		{
			int num = 0;
			int num2 = 0;
			string text = "";
			while (num2 < s.Length && num < nbItems)
			{
				while (num2 < s.Length && s[num2] != separator)
				{
					text += s[num2++];
				}
				num2++;
				array[num++] = text;
				text = "";
			}
			if (s[s.Length - 1] == newLineChar)
			{
				array[nbItems - 1] = "";
			}
		}
		catch (Exception ex)
		{
			throw new Exception("Utils.splitString() " + ex);
		}
		return array;
	}

	public static string[] splitStringWhiteSpace(string s)
	{
		List<string> list = new List<string>();
		string text = "";
		char[] array = (s + newLine).ToCharArray();
		foreach (char c in array)
		{
			string text2 = c.ToString();
			if (text2 == newLine)
			{
				list.Add(text);
				list.Add(newLine);
				text = "";
			}
			else if (text2 != " ")
			{
				text += text2;
			}
			else
			{
				list.Add(text);
				text = "";
			}
		}
		if (list[list.Count - 1] == newLine)
		{
			list.RemoveAt(list.Count - 1);
		}
		return list.ToArray();
	}

	public static float incrementRatio(float oldValue, int nbSteps)
	{
		if (oldValue >= 1f)
		{
			return MathHelper.Clamp(oldValue, 0f, 1f);
		}
		float old = oldValue + 1f / (float)nbSteps;
		return clampRatio(old);
	}

	public static float clampRatio(float old)
	{
		return (float)clampRatio((double)old);
	}

	public static double clampRatio(double old)
	{
		return MathHelper.Clamp((float)old, 0f, 1f);
	}

	public static float decrementRatio(float oldValue, int nbSteps)
	{
		if (oldValue <= 0f)
		{
			return MathHelper.Clamp(oldValue, 0f, 1f);
		}
		float old = oldValue - 1f / (float)nbSteps;
		return clampRatio(old);
	}

	public static Point LerpPoint(Point p1, Point p2, float amount)
	{
		return new Point((int)MathHelper.Lerp(p1.X, p2.X, amount), (int)MathHelper.Lerp(p1.Y, p2.Y, amount));
	}

	public static Vector3 LerpVector3(Vector3 v1, Vector3 v2, float amount)
	{
		return new Vector3(MathHelper.Lerp(v1.X, v2.X, amount), MathHelper.Lerp(v1.Y, v2.Y, amount), MathHelper.Lerp(v1.Z, v2.Z, amount));
	}

	public static Vector2 LerpVector2(Vector2 v1, Vector2 v2, float amount)
	{
		return new Vector2(MathHelper.Lerp(v1.X, v2.X, amount), MathHelper.Lerp(v1.Y, v2.Y, amount));
	}

	public static Vector3 SmoothStepVector3(Vector3 v1, Vector3 v2, float amount)
	{
		return new Vector3(MathHelper.SmoothStep(v1.X, v2.X, amount), MathHelper.SmoothStep(v1.Y, v2.Y, amount), MathHelper.SmoothStep(v1.Z, v2.Z, amount));
	}

	public static Color SmoothStepColor(Color c1, Color c2, float amount)
	{
		return new Color((byte)MathHelper.SmoothStep((int)c1.R, (int)c2.R, amount), (byte)MathHelper.SmoothStep((int)c1.G, (int)c2.G, amount), (byte)MathHelper.SmoothStep((int)c1.B, (int)c2.B, amount), (byte)MathHelper.SmoothStep((int)c1.A, (int)c2.A, amount));
	}

	public static Color ApplyColorToMask(Color blendColor, Color maskColor)
	{
		return ColorWithAlpha(LerpColor(Color.Black, blendColor, maskColor.ToVector3().X), maskColor.A);
	}

	public static Color Lerp3Color(Color c1, Color c2, Color c3, float amount)
	{
		if (amount < 0.5f)
		{
			return LerpColor(c1, c2, amount * 2f);
		}
		return LerpColor(c2, c3, (amount - 0.5f) * 2f);
	}

	public static Color LerpColors(List<Color> colors, float amount)
	{
		if (amount >= 1f)
		{
			return colors[colors.Count - 1];
		}
		if (amount <= 0f)
		{
			return colors[0];
		}
		int num = colors.Count - 1;
		int num2 = (int)(amount * (float)num);
		float amount2 = (amount - (float)num2 / (float)num) * (float)num;
		return LerpColor(colors[num2], colors[num2 + 1], amount2);
	}

	public static Color LerpColor(Color c1, Color c2, float amount)
	{
		return new Color((byte)MathHelper.Lerp((int)c1.R, (int)c2.R, amount), (byte)MathHelper.Lerp((int)c1.G, (int)c2.G, amount), (byte)MathHelper.Lerp((int)c1.B, (int)c2.B, amount), (byte)MathHelper.Lerp((int)c1.A, (int)c2.A, amount));
	}

	public static float SmoothStep(float ratio)
	{
		return MathHelper.Lerp(0f, 1f, ratio);
	}

	public static float SmoothStepInverse(float ratio)
	{
		return ratio + ratio - MathHelper.SmoothStep(0f, 1f, ratio);
	}

	public static float SmoothStepInverse_Lessened(float ratio)
	{
		return ratio + ratio - SmoothStep_Lessened(ratio, 0.66f);
	}

	public static float SmoothStep_Lessened(float ratio, float lessenRatio)
	{
		return MathHelper.Lerp(MathHelper.SmoothStep(0f, 1f, ratio), ratio, lessenRatio);
	}

	public static float WrapAngleRad(float rad)
	{
		if ((double)rad < -Math.PI)
		{
			return rad + (float)Math.PI * 2f;
		}
		if ((double)rad > Math.PI)
		{
			return rad - (float)Math.PI * 2f;
		}
		return rad;
	}

	public static Rectangle LerpRectangle(Rectangle r1, Rectangle r2, float amount)
	{
		return new Rectangle((int)MathHelper.Lerp(r1.X, r2.X, amount), (int)MathHelper.Lerp(r1.Y, r2.Y, amount), (int)MathHelper.Lerp(r1.Width, r2.Width, amount), (int)MathHelper.Lerp(r1.Height, r2.Height, amount));
	}

	public static Color ColorWhitePulsing(GameTime gameTime, float aMin, float aMax)
	{
		float num = (float)Math.Cos(MathHelper.ToRadians((float)(gameTime.TotalGameTime.TotalMilliseconds / 4.0 % 360.0)));
		num++;
		num /= 2f;
		num = MathHelper.Lerp(aMin, aMax, num);
		return ColorWithAlpha(Color.White, num);
	}

	public static Color ColorWhitePulsing(GameTime gameTime)
	{
		return ColorWhitePulsing(gameTime, 0.33f, 0.66f);
	}
}
