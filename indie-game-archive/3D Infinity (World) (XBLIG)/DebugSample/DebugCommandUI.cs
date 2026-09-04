using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DebugSample;

public class DebugCommandUI : DrawableGameComponent, IDebugCommandHost, IDebugEchoListner, IDebugCommandExecutioner
{
	private enum State
	{
		Closed,
		Opening,
		Opened,
		Closing
	}

	private class CommandInfo
	{
		public string command;

		public string description;

		public DebugCommandExecute callback;

		public CommandInfo(string command, string description, DebugCommandExecute callback)
		{
			this.command = command;
			this.description = description;
			this.callback = callback;
		}
	}

	private const int MaxLineCount = 20;

	private const int MaxCommandHistory = 32;

	private const string Cursor = "▂";

	public string DefaultPrompt = "CMD>";

	private DebugManager debugManager;

	private State state;

	private float stateTransition;

	private List<IDebugEchoListner> listenrs = new List<IDebugEchoListner>();

	private Stack<IDebugCommandExecutioner> executioners = new Stack<IDebugCommandExecutioner>();

	private Dictionary<string, CommandInfo> commandTable = new Dictionary<string, CommandInfo>();

	private string commandLine = string.Empty;

	private int cursorIndex;

	private Queue<string> lines = new Queue<string>();

	private List<string> commandHistory = new List<string>();

	private int commandHistoryIndex;

	private KeyboardState prevKeyState;

	private Keys pressedKey;

	private float keyRepeatTimer;

	private float keyRepeatStartDuration = 0.3f;

	private float keyRepeatDuration = 0.03f;

	public string Prompt { get; set; }

	public bool Focused => state != State.Closed;

	public DebugCommandUI(Game game)
		: base(game)
	{
		Prompt = DefaultPrompt;
		((GameComponent)this).Game.Services.AddService(typeof(IDebugCommandHost), (object)this);
		DebugCommandExecute callback = delegate
		{
			int num = 0;
			foreach (CommandInfo value in commandTable.Values)
			{
				num = Math.Max(num, value.command.Length);
			}
			string format = $"{{0,-{num}}}    {{1}}";
			foreach (CommandInfo value2 in commandTable.Values)
			{
				Echo(string.Format(format, value2.command, value2.description));
			}
		};
		RegisterCommand("help", "Show Command helps", callback);
		RegisterCommand("cls", "Clear Screen", delegate
		{
			lines.Clear();
		});
		RegisterCommand("echo", "Display Messages", delegate(IDebugCommandHost host, string command, IList<string> args)
		{
			Echo(command.Substring(5));
		});
	}

	public override void Initialize()
	{
		debugManager = ((GameComponent)this).Game.Services.GetService(typeof(DebugManager)) as DebugManager;
		if (debugManager == null)
		{
			throw new InvalidOperationException("DebugManagerが見つかりません。");
		}
		((DrawableGameComponent)this).Initialize();
	}

	public void RegisterCommand(string command, string description, DebugCommandExecute callback)
	{
		string key = command.ToLower();
		if (commandTable.ContainsKey(key))
		{
			throw new InvalidOperationException($"{command}は既に登録されています");
		}
		commandTable.Add(key, new CommandInfo(command, description, callback));
	}

	public void UnregisterCommand(string command)
	{
		string key = command.ToLower();
		if (!commandTable.ContainsKey(key))
		{
			throw new InvalidOperationException($"{command}は登録されていません");
		}
		commandTable.Remove(command);
	}

	public void ExecuteCommand(string command)
	{
		if (executioners.Count != 0)
		{
			executioners.Peek().ExecuteCommand(command);
			return;
		}
		char[] array = new char[1] { ' ' };
		Echo(Prompt + command);
		command = command.TrimStart(array);
		List<string> list = new List<string>(command.Split(array));
		string text = list[0];
		list.RemoveAt(0);
		if (commandTable.TryGetValue(text.ToLower(), out var value))
		{
			try
			{
				value.callback(this, command, list);
			}
			catch (Exception ex)
			{
				EchoError("Unhandled Exception occured");
				string[] array2 = ex.Message.Split('\n');
				string[] array3 = array2;
				foreach (string text2 in array3)
				{
					EchoError(text2);
				}
			}
		}
		else
		{
			Echo("Unknown Command");
		}
		commandHistory.Add(command);
		while (commandHistory.Count > 32)
		{
			commandHistory.RemoveAt(0);
		}
		commandHistoryIndex = commandHistory.Count;
	}

	public void RegisterEchoListner(IDebugEchoListner listner)
	{
		listenrs.Add(listner);
	}

	public void UnregisterEchoListner(IDebugEchoListner listner)
	{
		listenrs.Remove(listner);
	}

	public void Echo(DebugCommandMessage messageType, string text)
	{
		lines.Enqueue(text);
		while (lines.Count >= 20)
		{
			lines.Dequeue();
		}
		foreach (IDebugEchoListner listenr in listenrs)
		{
			listenr.Echo(messageType, text);
		}
	}

	public void Echo(string text)
	{
		Echo(DebugCommandMessage.Standard, text);
	}

	public void EchoWarning(string text)
	{
		Echo(DebugCommandMessage.Warning, text);
	}

	public void EchoError(string text)
	{
		Echo(DebugCommandMessage.Error, text);
	}

	public void PushExecutioner(IDebugCommandExecutioner executioner)
	{
		executioners.Push(executioner);
	}

	public void PopExecutioner()
	{
		executioners.Pop();
	}

	public override void Update(GameTime gameTime)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		KeyboardState val = Keyboard.GetState();
		float num = (float)gameTime.ElapsedGameTime.TotalSeconds;
		switch (state)
		{
		case State.Closed:
			if (((KeyboardState)(ref val)).IsKeyDown((Keys)9))
			{
				state = State.Opening;
			}
			break;
		case State.Opening:
			stateTransition += num * 8f;
			if (stateTransition > 1f)
			{
				stateTransition = 1f;
				state = State.Opened;
			}
			break;
		case State.Opened:
			ProcessKeyInputs(num);
			break;
		case State.Closing:
			stateTransition -= num * 8f;
			if (stateTransition < 0f)
			{
				stateTransition = 0f;
				state = State.Closed;
			}
			break;
		}
		prevKeyState = val;
		((GameComponent)this).Update(gameTime);
	}

	public void ProcessKeyInputs(float dt)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Invalid comparison between Unknown and I4
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected I4, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected I4, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Invalid comparison between Unknown and I4
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Invalid comparison between Unknown and I4
		KeyboardState val = Keyboard.GetState();
		Keys[] pressedKeys = ((KeyboardState)(ref val)).GetPressedKeys();
		bool shitKeyPressed = ((KeyboardState)(ref val)).IsKeyDown((Keys)160) || ((KeyboardState)(ref val)).IsKeyDown((Keys)161);
		Keys[] array = pressedKeys;
		foreach (Keys val2 in array)
		{
			if (!IsKeyPressed(val2, dt))
			{
				continue;
			}
			if (KeyboardUtils.KeyToString(val2, shitKeyPressed, out var character))
			{
				commandLine = commandLine.Insert(cursorIndex, new string(character, 1));
				cursorIndex++;
				continue;
			}
			Keys val3 = val2;
			if ((int)val3 <= 13)
			{
				switch (val3 - 8)
				{
				case 0:
					if (cursorIndex > 0)
					{
						commandLine = commandLine.Remove(--cursorIndex, 1);
					}
					continue;
				case 1:
					state = State.Closing;
					continue;
				}
				if ((int)val3 == 13)
				{
					ExecuteCommand(commandLine);
					commandLine = string.Empty;
					cursorIndex = 0;
				}
				continue;
			}
			switch (val3 - 37)
			{
			default:
				if ((int)val3 == 46 && cursorIndex < commandLine.Length)
				{
					commandLine = commandLine.Remove(cursorIndex, 1);
				}
				break;
			case 0:
				if (cursorIndex > 0)
				{
					cursorIndex--;
				}
				break;
			case 2:
				if (cursorIndex < commandLine.Length)
				{
					cursorIndex++;
				}
				break;
			case 1:
				if (commandHistory.Count > 0)
				{
					commandHistoryIndex = Math.Max(0, commandHistoryIndex - 1);
					commandLine = commandHistory[commandHistoryIndex];
					cursorIndex = commandLine.Length;
				}
				break;
			case 3:
				if (commandHistory.Count > 0)
				{
					commandHistoryIndex = Math.Min(commandHistory.Count - 1, commandHistoryIndex + 1);
					commandLine = commandHistory[commandHistoryIndex];
					cursorIndex = commandLine.Length;
				}
				break;
			}
		}
	}

	private bool IsKeyPressed(Keys key, float dt)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (((KeyboardState)(ref prevKeyState)).IsKeyUp(key))
		{
			keyRepeatTimer = keyRepeatStartDuration;
			pressedKey = key;
			return true;
		}
		if (key == pressedKey)
		{
			keyRepeatTimer -= dt;
			if (keyRepeatTimer <= 0f)
			{
				keyRepeatTimer += keyRepeatDuration;
				return true;
			}
		}
		return false;
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		if (state == State.Closed)
		{
			return;
		}
		SpriteFont debugFont = debugManager.DebugFont;
		SpriteBatch spriteBatch = debugManager.SpriteBatch;
		Texture2D whiteTexture = debugManager.WhiteTexture;
		Viewport viewport = ((DrawableGameComponent)this).GraphicsDevice.Viewport;
		float num = ((Viewport)(ref viewport)).Width;
		Viewport viewport2 = ((DrawableGameComponent)this).GraphicsDevice.Viewport;
		float num2 = ((Viewport)(ref viewport2)).Height;
		float num3 = num2 * 0.1f;
		float num4 = num * 0.1f;
		Rectangle val = new Rectangle
		{
			X = (int)num4,
			Y = (int)num3,
			Width = (int)(num * 0.8f),
			Height = 20 * debugFont.LineSpacing
		};
		Matrix val2 = Matrix.CreateTranslation(new Vector3(0f, (float)(-val.Height) * (1f - stateTransition), 0f));
		spriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)1, val2);
		spriteBatch.Draw(whiteTexture, val, new Color((byte)0, (byte)0, (byte)0, (byte)200));
		Vector2 val3 = default(Vector2);
		((Vector2)(ref val3))._002Ector(num4, num3);
		foreach (string line in lines)
		{
			spriteBatch.DrawString(debugFont, line, val3, Color.White);
			val3.Y += (float)debugFont.LineSpacing;
		}
		string text = Prompt + commandLine.Substring(0, cursorIndex);
		Vector2 val4 = val3 + debugFont.MeasureString(text);
		val4.Y = val3.Y;
		spriteBatch.DrawString(debugFont, $"{Prompt}{commandLine}", val3, Color.White);
		spriteBatch.DrawString(debugFont, "▂", val4, Color.White);
		spriteBatch.End();
	}
}
