using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Maximinus.DebugTools;

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

	private const string Cursor = "_";

	public const string DefaultPrompt = "CMD>";

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
		Prompt = "CMD>";
		base.Game.Services.AddService(typeof(IDebugCommandHost), this);
		base.DrawOrder = int.MaxValue;
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
		debugManager = base.Game.Services.GetService(typeof(DebugManager)) as DebugManager;
		if (debugManager == null)
		{
			throw new InvalidOperationException("Coudn't find DebugManager.");
		}
		base.Initialize();
	}

	public void RegisterCommand(string command, string description, DebugCommandExecute callback)
	{
		string key = command.ToLower();
		if (commandTable.ContainsKey(key))
		{
			throw new InvalidOperationException($"Command \"{command}\" is already registered.");
		}
		commandTable.Add(key, new CommandInfo(command, description, callback));
	}

	public void UnregisterCommand(string command)
	{
		string key = command.ToLower();
		if (!commandTable.ContainsKey(key))
		{
			throw new InvalidOperationException($"Command \"{command}\" is not registered.");
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
				EchoError("Unhandled Exception occurred");
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

	public void Show()
	{
		if (state == State.Closed)
		{
			stateTransition = 0f;
			state = State.Opening;
		}
	}

	public void Hide()
	{
		if (state == State.Opened)
		{
			stateTransition = 1f;
			state = State.Closing;
		}
	}

	public override void Update(GameTime gameTime)
	{
		KeyboardState keyboardState = Keyboard.GetState();
		float num = (float)gameTime.ElapsedGameTime.TotalSeconds;
		switch (state)
		{
		case State.Closed:
			if (keyboardState.IsKeyDown(Keys.Tab))
			{
				Show();
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
		prevKeyState = keyboardState;
		base.Update(gameTime);
	}

	public void ProcessKeyInputs(float dt)
	{
		KeyboardState keyboardState = Keyboard.GetState();
		Keys[] pressedKeys = keyboardState.GetPressedKeys();
		bool shitKeyPressed = keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);
		Keys[] array = pressedKeys;
		foreach (Keys keys in array)
		{
			if (!IsKeyPressed(keys, dt))
			{
				continue;
			}
			if (KeyboardUtils.KeyToString(keys, shitKeyPressed, out var character))
			{
				commandLine = commandLine.Insert(cursorIndex, new string(character, 1));
				cursorIndex++;
				continue;
			}
			switch (keys)
			{
			case Keys.Back:
				if (cursorIndex > 0)
				{
					commandLine = commandLine.Remove(--cursorIndex, 1);
				}
				break;
			case Keys.Delete:
				if (cursorIndex < commandLine.Length)
				{
					commandLine = commandLine.Remove(cursorIndex, 1);
				}
				break;
			case Keys.Left:
				if (cursorIndex > 0)
				{
					cursorIndex--;
				}
				break;
			case Keys.Right:
				if (cursorIndex < commandLine.Length)
				{
					cursorIndex++;
				}
				break;
			case Keys.Enter:
				ExecuteCommand(commandLine);
				commandLine = string.Empty;
				cursorIndex = 0;
				break;
			case Keys.Up:
				if (commandHistory.Count > 0)
				{
					commandHistoryIndex = Math.Max(0, commandHistoryIndex - 1);
					commandLine = commandHistory[commandHistoryIndex];
					cursorIndex = commandLine.Length;
				}
				break;
			case Keys.Down:
				if (commandHistory.Count > 0)
				{
					commandHistoryIndex = Math.Min(commandHistory.Count - 1, commandHistoryIndex + 1);
					commandLine = commandHistory[commandHistoryIndex];
					cursorIndex = commandLine.Length;
				}
				break;
			case Keys.Tab:
				Hide();
				break;
			}
		}
	}

	private bool IsKeyPressed(Keys key, float dt)
	{
		if (prevKeyState.IsKeyUp(key))
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
		if (state == State.Closed)
		{
			return;
		}
		if (MaximinusGame.DebugIncludeInTimeRuler)
		{
			MaximinusGame.Debug_TimeRuler_BeginMark("DEBUG command", Color.White);
		}
		SpriteFont debugFont = debugManager.debugFont;
		SpriteBatch spriteBatch = debugManager.SpriteBatch;
		Texture2D whiteTexture = debugManager.WhiteTexture;
		float num = base.GraphicsDevice.Viewport.Width;
		float num2 = base.GraphicsDevice.Viewport.Height;
		float num3 = num2 * 0.1f;
		float num4 = num * 0.1f;
		Rectangle destinationRectangle = new Rectangle
		{
			X = (int)num4,
			Y = (int)num3,
			Width = (int)(num * 0.8f),
			Height = 20 * debugFont.LineSpacing
		};
		Matrix transformMatrix = Matrix.CreateTranslation(new Vector3(0f, (float)(-destinationRectangle.Height) * (1f - stateTransition), 0f));
		spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, transformMatrix);
		spriteBatch.Draw(whiteTexture, destinationRectangle, new Color(0, 0, 0, 200));
		Vector2 vector = new Vector2(num4, num3);
		foreach (string line in lines)
		{
			spriteBatch.DrawString(debugFont, line, vector, Color.White);
			vector.Y += debugFont.LineSpacing;
		}
		string text = Prompt + commandLine.Substring(0, cursorIndex);
		Vector2 position = vector + debugFont.MeasureString(text);
		position.Y = vector.Y;
		spriteBatch.DrawString(debugFont, $"{Prompt}{commandLine}", vector, Color.White);
		spriteBatch.DrawString(debugFont, "_", position, Color.White);
		spriteBatch.End();
		if (MaximinusGame.DebugIncludeInTimeRuler)
		{
			MaximinusGame.Debug_TimeRuler_EndMark("DEBUG command");
		}
	}
}
