using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Quasar.Global;

public static class CompatHooks
{
	private static readonly ConcurrentDictionary<string, byte> Seen = new ConcurrentDictionary<string, byte>(StringComparer.Ordinal);

	private static readonly ConcurrentDictionary<int, bool> PreviousKeyStates = new ConcurrentDictionary<int, bool>();

	private static readonly ConcurrentDictionary<int, bool> PreviousStageKeyboardStates = new ConcurrentDictionary<int, bool>();

	private static readonly ConcurrentDictionary<string, bool> PreviousGamePadStates = new ConcurrentDictionary<string, bool>(StringComparer.Ordinal);

	private static readonly ConcurrentDictionary<string, bool> PreviousMenuStates = new ConcurrentDictionary<string, bool>(StringComparer.Ordinal);

	private static readonly ConcurrentDictionary<string, long> NextMenuRepeatTicks = new ConcurrentDictionary<string, long>(StringComparer.Ordinal);

	private static readonly ConcurrentDictionary<int, long> NextRepeatTicks = new ConcurrentDictionary<int, long>();

	private static readonly ConcurrentDictionary<int, object> CompatPlayerBodies = new ConcurrentDictionary<int, object>();

	private static readonly ConcurrentDictionary<int, object> CompatPlayerOverlays = new ConcurrentDictionary<int, object>();

	private static readonly ConcurrentDictionary<int, object> CompatPlayerStageMeshes = new ConcurrentDictionary<int, object>();

	private static readonly ConcurrentDictionary<int, object> CompatPlayerStageActors = new ConcurrentDictionary<int, object>();

	private static readonly ConcurrentDictionary<int, Vector2> CompatCameraOffsets = new ConcurrentDictionary<int, Vector2>();

	private static readonly ConcurrentDictionary<int, byte> OfficialAvatarLoadRepairs = new ConcurrentDictionary<int, byte>();

	private static object CachedProxyTexture;

	private static object CompatMenuBackground;

	private static object CompatMenuPreview;

	private static BasicEffect CompatAvatarEffect;

	private static VertexPositionColor[] CompatAvatarVertices;

	private static int[] CompatAvatarIndices;

	private static DateTime CompatAvatarObjStamp = DateTime.MinValue;

	private static bool CompatAvatarTriedObj;

	private static readonly string RuntimeDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserData");

	private static readonly string CompatPlayerModelConfigPath = Path.Combine(RuntimeDataDir, "player-model.txt");

	private static readonly string CompatPlayerModelNotesPath = Path.Combine(RuntimeDataDir, "player-model-readme.txt");

	private static readonly string CompatAvatarObjPath = Path.Combine(RuntimeDataDir, "player-avatar.strb.obj");

	private static readonly string CompatAvatarObjNotesPath = Path.Combine(RuntimeDataDir, "player-avatar-obj-readme.txt");

	private static readonly string CompatPlayerModelSampleDirName = "Custom";

	private static readonly string CompatPlayerModelSampleFileName = "PlayerAvatar.xnb";

	private static readonly string CompatPlayerModelDefaultAsset = "Animals/PigModel";

	private static readonly string LogPath = Path.Combine(RuntimeDataDir, "compat-hooks.log");

	private static readonly string ExceptionLogPath = Path.Combine(RuntimeDataDir, "compat-exceptions.log");

	private static readonly string CommandPath = Path.Combine(RuntimeDataDir, "avatarfarm-runtime-commands.txt");

	private static readonly string SceneFilterPath = Path.Combine(RuntimeDataDir, "avatarfarm-scene-filter.txt");

	private static readonly string OverlayXPath = Path.Combine(RuntimeDataDir, "avatarfarm-overlay-x.txt");

	private static readonly string OverlayZPath = Path.Combine(RuntimeDataDir, "avatarfarm-overlay-z.txt");

	private static readonly string OverlayScalePath = Path.Combine(RuntimeDataDir, "avatarfarm-overlay-scale.txt");

	private static readonly string CameraYawPath = Path.Combine(RuntimeDataDir, "avatarfarm-camera-yaw.txt");

	private static readonly string CameraPitchPath = Path.Combine(RuntimeDataDir, "avatarfarm-camera-pitch.txt");

	private static readonly string CameraDistancePath = Path.Combine(RuntimeDataDir, "avatarfarm-camera-distance.txt");

	private static readonly object CommandLock = new object();

	private static object registeredLayout;

	private static MethodInfo registeredLayoutUpdate;

	private static object registeredGroup;

	private static readonly ArrayList registeredGroups = new ArrayList();

	private static readonly ArrayList registeredMenuButtons = new ArrayList();

	private static int activeMenuButton;

	private static object baseGameInstance;

	private static MethodInfo baseGameMainLoop;

	private static int updatingLayout;

	private static int nextLayoutUpdateTicks;

	private static int nextStageKeyboardTicks;

	private static int queuedMoveX;

	private static int queuedMoveY;

	private static int queuedMoveExpireTick;

	private static object menuBackgroundSection;

	private static int loadingRecoveryStarted;

	private static int renderCameraTick;

	private static float FreeCameraYaw;

	private static float FreeCameraPitch = -0.18f;

	private static float FreeCameraDistance = 5.8f;

	private static int Captured;

	private static int ForcedCameraMode;

	private static bool UseCompatFarmMesh = false;

	private static bool UseCompatPlayerSync = true;

	private static bool UseCompatPlayerProxy = false;

	private static bool UseCompatPlayerOverlay = false;

	private static bool UseCompatPlayerStageActor = false;

	private static bool ForceCompatPlayerProxy = false;

	private static bool UseCompatStageClear = false;

	private static bool PreferCompatPlayerStageMesh = false;

	private static bool UseCompatGeneratedAvatar = false;

	private static int fixedGuiViewportApplied;

	private static int LastMouseWheel;

	private static int MouseWheelPulse;

	private static int MouseWheelPulseExpires;

	private static int RuntimeDirectoryReady;

	private static int HlslDiagnosticBudget = 20;

	private static readonly object MainThreadPreloadLock = new object();

	private static string[] MainThreadPreloadAssets;

	private static int MainThreadPreloadIndex;

	private static int MainThreadPreloadFailureCount;

	private static int MainThreadPreloadFailureLogBudget = 16;

	private static int MainThreadPreloadComplete;

	private static int AudioRuntimeValidationComplete;

	private static int AudioRuntimeValidationAttempts;

	private static int NextAudioRuntimeValidationTick;

	private static readonly bool EnableRenderDiagnostics = string.Equals(Environment.GetEnvironmentVariable("AVATARFARM_RENDER_DIAGNOSTICS"), "1", StringComparison.Ordinal);

	private const uint GL_RGBA = 6408u;

	private const uint GL_UNSIGNED_BYTE = 5121u;

	private const uint GL_VIEWPORT = 2978u;

	public static GraphicsDevice GraphicsDevice
	{
		get
		{
			try
			{
				return Engine.Device;
			}
			catch
			{
				return null;
			}
		}
	}

	[DllImport("opengl32.dll")]
	private static extern void glGetIntegerv(uint pname, int[] data);

	[DllImport("opengl32.dll")]
	private static extern void glReadPixels(int x, int y, int width, int height, uint format, uint type, byte[] pixels);

	public static bool IsVirtualKeyDown(int virtualKey)
	{
		try
		{
			KeyboardState state = Keyboard.GetState();
			Keys keys = VirtualKeyToXnaKey(virtualKey);
			if (keys != Keys.None && state.IsKeyDown(keys))
			{
				Mark("inprocess-keydown-" + keys);
				return true;
			}
			MouseState state2 = Mouse.GetState();
			UpdateMouseWheelPulse(state2.ScrollWheelValue);
			if ((virtualKey == 13 || virtualKey == 32 || virtualKey == 69) && state2.LeftButton == ButtonState.Pressed)
			{
				return true;
			}
			if (virtualKey == 27 && state2.RightButton == ButtonState.Pressed)
			{
				return true;
			}
			if (virtualKey == 72 && state2.MiddleButton == ButtonState.Pressed)
			{
				return true;
			}
			if (virtualKey == 38 && MouseWheelPulse > 0)
			{
				return true;
			}
			if (virtualKey == 40 && MouseWheelPulse < 0)
			{
				return true;
			}
		}
		catch
		{
		}
		return false;
	}

	private static Keys VirtualKeyToXnaKey(int virtualKey)
	{
		if (virtualKey >= 65 && virtualKey <= 90)
		{
			return (Keys)(65 + virtualKey - 65);
		}
		if (virtualKey >= 48 && virtualKey <= 57)
		{
			return (Keys)(48 + virtualKey - 48);
		}
		return virtualKey switch
		{
			8 => Keys.Back, 
			9 => Keys.Tab, 
			13 => Keys.Enter, 
			16 => Keys.LeftShift, 
			17 => Keys.LeftControl, 
			18 => Keys.LeftAlt, 
			27 => Keys.Escape, 
			32 => Keys.Space, 
			33 => Keys.PageUp, 
			34 => Keys.PageDown, 
			35 => Keys.End, 
			36 => Keys.Home, 
			37 => Keys.Left, 
			38 => Keys.Up, 
			39 => Keys.Right, 
			40 => Keys.Down, 
			45 => Keys.Insert, 
			46 => Keys.Delete, 
			_ => Keys.None, 
		};
	}

	private static void UpdateMouseWheelPulse(int wheelValue)
	{
		int num = Environment.TickCount & 0x7FFFFFFF;
		int num2 = Volatile.Read(ref LastMouseWheel);
		if (num2 == 0)
		{
			Interlocked.CompareExchange(ref LastMouseWheel, wheelValue, 0);
			num2 = wheelValue;
		}
		if (wheelValue != num2)
		{
			Interlocked.Exchange(ref MouseWheelPulse, Math.Sign(wheelValue - num2));
			Interlocked.Exchange(ref MouseWheelPulseExpires, num + 45);
			Interlocked.Exchange(ref LastMouseWheel, wheelValue);
		}
		else if (Volatile.Read(ref MouseWheelPulseExpires) != 0 && num >= Volatile.Read(ref MouseWheelPulseExpires))
		{
			Interlocked.Exchange(ref MouseWheelPulse, 0);
			Interlocked.Exchange(ref MouseWheelPulseExpires, 0);
		}
	}

	public static bool WasVirtualKeyPressed(int virtualKey)
	{
		if (TryConsumeCommand(virtualKey))
		{
			Mark("command-pressed-" + virtualKey);
			return true;
		}
		bool flag = IsVirtualKeyDown(virtualKey) || IsSdlKeyDown(virtualKey) || IsGamePadVirtualKeyDown(virtualKey);
		PreviousKeyStates.TryGetValue(virtualKey, out var value);
		PreviousKeyStates[virtualKey] = flag;
		return flag && !value;
	}

	public static bool IsCompatKeyDown(int virtualKey)
	{
		return IsVirtualKeyDown(virtualKey) || IsSdlKeyDown(virtualKey) || IsGamePadVirtualKeyDown(virtualKey);
	}

	public static Vector2 GetCompatMovement()
	{
		float num = 0f;
		float num2 = 0f;
		if (IsKeyboardKeyDownOnly(37) || IsKeyboardKeyDownOnly(65))
		{
			num--;
		}
		if (IsKeyboardKeyDownOnly(39) || IsKeyboardKeyDownOnly(68))
		{
			num++;
		}
		if (IsKeyboardKeyDownOnly(38) || IsKeyboardKeyDownOnly(87))
		{
			num2++;
		}
		if (IsKeyboardKeyDownOnly(40) || IsKeyboardKeyDownOnly(83))
		{
			num2--;
		}
		Vector2 gamePadMovement = GetGamePadMovement();
		num += gamePadMovement.X;
		num2 += gamePadMovement.Y;
		int num3 = Environment.TickCount & 0x7FFFFFFF;
		int num4 = Volatile.Read(ref queuedMoveExpireTick);
		if (num3 < num4)
		{
			num += (float)Volatile.Read(ref queuedMoveX);
			num2 += (float)Volatile.Read(ref queuedMoveY);
		}
		else if (num4 != 0)
		{
			Interlocked.Exchange(ref queuedMoveX, 0);
			Interlocked.Exchange(ref queuedMoveY, 0);
			Interlocked.Exchange(ref queuedMoveExpireTick, 0);
		}
		Vector2 result = new Vector2(num, num2);
		if (result.LengthSquared() > 1f)
		{
			result.Normalize();
		}
		return result;
	}

	public static Vector2 GetCompatAim()
	{
		try
		{
			GamePadState compatGamePadState = GetCompatGamePadState();
			if (!compatGamePadState.IsConnected)
			{
				return Vector2.Zero;
			}
			return ApplyRadialDeadZone(compatGamePadState.ThumbSticks.Right, 0.14f, 1.85f, clampToUnit: false);
		}
		catch (Exception ex)
		{
			Mark("gamepad-aim-failed-" + DescribeException(ex));
			return Vector2.Zero;
		}
	}

	public static bool WasMenuInteractPressed()
	{
		Mark("menu-interact-polled");
		GamePadState compatGamePadState = GetCompatGamePadState();
		bool flag = IsVirtualKeyDown(13) || IsVirtualKeyDown(32) || (compatGamePadState.IsConnected && compatGamePadState.Buttons.A == ButtonState.Pressed);
		if (flag)
		{
			Mark("menu-interact-down");
		}
		return WasMenuSemanticPressed("INTERACT", flag);
	}

	public static bool WasMenuStartPressed()
	{
		Mark("menu-start-polled");
		GamePadState compatGamePadState = GetCompatGamePadState();
		bool flag = IsKeyboardKeyDownOnly(9) || (compatGamePadState.IsConnected && compatGamePadState.Buttons.Start == ButtonState.Pressed);
		if (flag)
		{
			Mark("menu-start-down");
		}
		return WasMenuSemanticPressed("START", flag);
	}

	public static bool WasMenuCancelPressed()
	{
		GamePadState compatGamePadState = GetCompatGamePadState();
		bool flag = IsVirtualKeyDown(27) || (compatGamePadState.IsConnected && compatGamePadState.Buttons.B == ButtonState.Pressed);
		if (flag)
		{
			Mark("menu-cancel-down");
		}
		return WasMenuSemanticPressed("CANCEL", flag);
	}

	public static bool WasMenuBackPressed()
	{
		GamePadState compatGamePadState = GetCompatGamePadState();
		bool flag = IsKeyboardKeyDownOnly(8) || (compatGamePadState.IsConnected && compatGamePadState.Buttons.Back == ButtonState.Pressed);
		if (flag)
		{
			Mark("menu-back-down");
		}
		return WasMenuSemanticPressed("BACK", flag);
	}

	public static bool WasMenuDirectionPressed(int virtualKey)
	{
		return WasMenuSemanticPressed("DIRECTION-" + virtualKey, IsMenuDirectionDown(virtualKey));
	}

	public static bool WasMenuDirectionRepeat(int virtualKey)
	{
		string key = "REPEAT-" + virtualKey;
		bool flag = IsMenuDirectionDown(virtualKey);
		PreviousMenuStates.TryGetValue(key, out var value);
		PreviousMenuStates[key] = flag;
		if (!flag)
		{
			NextMenuRepeatTicks[key] = 0L;
			return false;
		}
		long num = Environment.TickCount & 0x7FFFFFFF;
		if (!value)
		{
			NextMenuRepeatTicks[key] = num + 550;
			return true;
		}
		NextMenuRepeatTicks.TryGetValue(key, out var value2);
		if (value2 == 0L || num < value2)
		{
			return false;
		}
		NextMenuRepeatTicks[key] = num + 125;
		return true;
	}

	private static bool WasMenuSemanticPressed(string name, bool down)
	{
		PreviousMenuStates.TryGetValue(name, out var value);
		PreviousMenuStates[name] = down;
		return down && !value;
	}

	private static bool IsMenuDirectionDown(int virtualKey)
	{
		if (IsVirtualKeyDown(virtualKey) || (virtualKey == 37 && IsKeyboardKeyDownOnly(65)) || (virtualKey == 38 && IsKeyboardKeyDownOnly(87)) || (virtualKey == 39 && IsKeyboardKeyDownOnly(68)) || (virtualKey == 40 && IsKeyboardKeyDownOnly(83)))
		{
			return true;
		}
		GamePadState compatGamePadState = GetCompatGamePadState();
		if (!compatGamePadState.IsConnected)
		{
			return false;
		}
		return virtualKey switch
		{
			37 => compatGamePadState.DPad.Left == ButtonState.Pressed || compatGamePadState.ThumbSticks.Left.X < -0.45f, 
			38 => compatGamePadState.DPad.Up == ButtonState.Pressed || compatGamePadState.ThumbSticks.Left.Y > 0.45f, 
			39 => compatGamePadState.DPad.Right == ButtonState.Pressed || compatGamePadState.ThumbSticks.Left.X > 0.45f, 
			40 => compatGamePadState.DPad.Down == ButtonState.Pressed || compatGamePadState.ThumbSticks.Left.Y < -0.45f, 
			_ => false, 
		};
	}

	private static bool IsKeyboardKeyDownOnly(int virtualKey)
	{
		try
		{
			Keys keys = VirtualKeyToXnaKey(virtualKey);
			return keys != Keys.None && Keyboard.GetState().IsKeyDown(keys);
		}
		catch
		{
			return false;
		}
	}

	private static bool WasStageKeyboardOrMousePressed(int virtualKey)
	{
		bool flag = IsVirtualKeyDown(virtualKey) || IsSdlKeyDown(virtualKey);
		PreviousStageKeyboardStates.TryGetValue(virtualKey, out var value);
		PreviousStageKeyboardStates[virtualKey] = flag;
		return flag && !value;
	}

	private static Vector2 ApplyRadialDeadZone(Vector2 value, float deadZone, float responseScale, bool clampToUnit)
	{
		float num = value.Length();
		if (num <= deadZone || num <= 0.0001f)
		{
			return Vector2.Zero;
		}
		float num2 = Math.Min(1f, (num - deadZone) / (1f - deadZone));
		Vector2 result = value * (num2 / num) * responseScale;
		if (clampToUnit && result.LengthSquared() > 1f)
		{
			result.Normalize();
		}
		return result;
	}

	public static bool WasCompatKeyRepeat(int virtualKey)
	{
		Mark("repeat-call-" + virtualKey);
		if (TryConsumeCommand(virtualKey))
		{
			Mark("command-repeat-" + virtualKey);
			return true;
		}
		if (!IsCompatKeyDown(virtualKey))
		{
			NextRepeatTicks[virtualKey] = 0L;
			return false;
		}
		Mark("repeat-down-" + virtualKey);
		long num = Environment.TickCount & 0x7FFFFFFF;
		NextRepeatTicks.TryGetValue(virtualKey, out var value);
		if (value != 0L && num < value)
		{
			return false;
		}
		NextRepeatTicks[virtualKey] = num + 180;
		return true;
	}

	private static bool TryConsumeCommand(int virtualKey)
	{
		string text = VirtualKeyToCommand(virtualKey);
		if (text == null)
		{
			return false;
		}
		try
		{
			lock (CommandLock)
			{
				if (!File.Exists(CommandPath))
				{
					return false;
				}
				Mark("command-file-seen");
				string[] array = ReadCommandLines();
				Mark("command-file-lines-" + array.Length);
				for (int i = 0; i < array.Length; i++)
				{
					Mark("command-line-" + array[i].Trim());
					if (CommandMatches(array[i], text))
					{
						string[] array2 = new string[array.Length - 1];
						if (i > 0)
						{
							Array.Copy(array, 0, array2, 0, i);
						}
						if (i + 1 < array.Length)
						{
							Array.Copy(array, i + 1, array2, i, array.Length - i - 1);
						}
						WriteCommandLines(array2);
						return true;
					}
				}
			}
		}
		catch
		{
		}
		return false;
	}

	private static bool CommandMatches(string line, string wanted)
	{
		if (line == null)
		{
			return false;
		}
		string text = line.Trim();
		if (text.Length == 0)
		{
			return false;
		}
		if (string.Equals(text, wanted, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		return wanted == "ENTER" && string.Equals(text, "INTERACT", StringComparison.OrdinalIgnoreCase);
	}

	private static string[] ReadCommandLines()
	{
		using FileStream stream = new FileStream(CommandPath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
		using StreamReader streamReader = new StreamReader(stream);
		string text = streamReader.ReadToEnd();
		if (text.Length == 0)
		{
			return new string[0];
		}
		return text.Replace("\r\n", "\n").Replace('\r', '\n').Split(new char[1] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
	}

	private static void WriteCommandLines(string[] lines)
	{
		string value = ((lines == null || lines.Length == 0) ? "" : string.Join(Environment.NewLine, lines));
		using FileStream stream = new FileStream(CommandPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
		using StreamWriter streamWriter = new StreamWriter(stream);
		streamWriter.Write(value);
	}

	private static string VirtualKeyToCommand(int virtualKey)
	{
		return virtualKey switch
		{
			13 => "ENTER", 
			27 => "ESC", 
			32 => "SPACE", 
			37 => "LEFT", 
			38 => "UP", 
			39 => "RIGHT", 
			40 => "DOWN", 
			_ => null, 
		};
	}

	private static bool IsSdlKeyDown(int virtualKey)
	{
		return IsVirtualKeyDown(virtualKey);
	}

	private static int VirtualKeyToSdlScancode(int virtualKey)
	{
		return virtualKey switch
		{
			13 => 40, 
			27 => 41, 
			32 => 44, 
			37 => 80, 
			38 => 82, 
			39 => 79, 
			40 => 81, 
			_ => -1, 
		};
	}

	private static GamePadState GetCompatGamePadState()
	{
		try
		{
			GamePadState state = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.None);
			if (state.IsConnected)
			{
				return state;
			}
			for (int i = 1; i < 4; i++)
			{
				state = GamePad.GetState((PlayerIndex)i, GamePadDeadZone.None);
				if (state.IsConnected)
				{
					Mark("gamepad-connected-index-" + i);
					return state;
				}
			}
		}
		catch (Exception ex)
		{
			Mark("gamepad-state-failed-" + DescribeException(ex));
		}
		return default(GamePadState);
	}

	private static bool IsGamePadVirtualKeyDown(int virtualKey)
	{
		try
		{
			GamePadState compatGamePadState = GetCompatGamePadState();
			if (!compatGamePadState.IsConnected)
			{
				return false;
			}
			switch (virtualKey)
			{
			case 13:
			case 32:
				return compatGamePadState.Buttons.A == ButtonState.Pressed || compatGamePadState.Buttons.Start == ButtonState.Pressed;
			case 27:
				return compatGamePadState.Buttons.B == ButtonState.Pressed || compatGamePadState.Buttons.Back == ButtonState.Pressed;
			case 37:
				return compatGamePadState.DPad.Left == ButtonState.Pressed || compatGamePadState.ThumbSticks.Left.X < -0.35f;
			case 38:
				return compatGamePadState.DPad.Up == ButtonState.Pressed || compatGamePadState.ThumbSticks.Left.Y > 0.35f;
			case 39:
				return compatGamePadState.DPad.Right == ButtonState.Pressed || compatGamePadState.ThumbSticks.Left.X > 0.35f;
			case 40:
				return compatGamePadState.DPad.Down == ButtonState.Pressed || compatGamePadState.ThumbSticks.Left.Y < -0.35f;
			case 69:
				return compatGamePadState.Buttons.A == ButtonState.Pressed;
			case 80:
				return compatGamePadState.Buttons.Y == ButtonState.Pressed;
			case 88:
				return compatGamePadState.Buttons.X == ButtonState.Pressed;
			case 67:
				return compatGamePadState.Buttons.RightStick == ButtonState.Pressed;
			case 72:
				return compatGamePadState.Buttons.LeftStick == ButtonState.Pressed;
			case 74:
				return compatGamePadState.Buttons.LeftShoulder == ButtonState.Pressed || compatGamePadState.ThumbSticks.Right.X < -0.35f;
			case 76:
				return compatGamePadState.Buttons.RightShoulder == ButtonState.Pressed || compatGamePadState.ThumbSticks.Right.X > 0.35f;
			case 73:
				return compatGamePadState.ThumbSticks.Right.Y > 0.35f;
			case 75:
				return compatGamePadState.ThumbSticks.Right.Y < -0.35f;
			case 85:
				return compatGamePadState.Triggers.Left > 0.35f;
			case 79:
				return compatGamePadState.Triggers.Right > 0.35f;
			default:
				return false;
			}
		}
		catch
		{
			return false;
		}
	}

	private static Vector2 GetGamePadMovement()
	{
		try
		{
			GamePadState compatGamePadState = GetCompatGamePadState();
			if (!compatGamePadState.IsConnected)
			{
				return Vector2.Zero;
			}
			Vector2 result = ApplyRadialDeadZone(compatGamePadState.ThumbSticks.Left, 0.14f, 1f, clampToUnit: true);
			Vector2 zero = Vector2.Zero;
			if (compatGamePadState.DPad.Left == ButtonState.Pressed)
			{
				zero.X--;
			}
			if (compatGamePadState.DPad.Right == ButtonState.Pressed)
			{
				zero.X++;
			}
			if (compatGamePadState.DPad.Up == ButtonState.Pressed)
			{
				zero.Y++;
			}
			if (compatGamePadState.DPad.Down == ButtonState.Pressed)
			{
				zero.Y--;
			}
			if (zero.LengthSquared() > 1f)
			{
				zero.Normalize();
			}
			result += zero;
			if (result.LengthSquared() > 1f)
			{
				result.Normalize();
			}
			return result;
		}
		catch
		{
			return Vector2.Zero;
		}
	}

	private static bool WasGamePadButtonPressed(string name, bool down)
	{
		PreviousGamePadStates.TryGetValue(name, out var value);
		PreviousGamePadStates[name] = down;
		return down && !value;
	}

	private static string PollGamePadStageCommand()
	{
		try
		{
			GamePadState compatGamePadState = GetCompatGamePadState();
			if (!compatGamePadState.IsConnected)
			{
				return null;
			}
			if (WasGamePadButtonPressed("A", compatGamePadState.Buttons.A == ButtonState.Pressed))
			{
				return "ACTION";
			}
			if (WasGamePadButtonPressed("X", compatGamePadState.Buttons.X == ButtonState.Pressed))
			{
				return "RECYCLE";
			}
			if (WasGamePadButtonPressed("Y", compatGamePadState.Buttons.Y == ButtonState.Pressed))
			{
				return "PLANT";
			}
			if (WasGamePadButtonPressed("B", compatGamePadState.Buttons.B == ButtonState.Pressed))
			{
				return "ESC";
			}
			if (WasGamePadButtonPressed("START", compatGamePadState.Buttons.Start == ButtonState.Pressed))
			{
				return "ENTER";
			}
			if (WasGamePadButtonPressed("BACK", compatGamePadState.Buttons.Back == ButtonState.Pressed))
			{
				return "ESC";
			}
			if (WasGamePadButtonPressed("RS", compatGamePadState.Buttons.RightStick == ButtonState.Pressed))
			{
				return "CAMERA";
			}
			if (WasGamePadButtonPressed("LS", compatGamePadState.Buttons.LeftStick == ButtonState.Pressed))
			{
				return "CAMRESET";
			}
			if (WasGamePadButtonPressed("LB", compatGamePadState.Buttons.LeftShoulder == ButtonState.Pressed))
			{
				return "CAMLEFT";
			}
			if (WasGamePadButtonPressed("RB", compatGamePadState.Buttons.RightShoulder == ButtonState.Pressed))
			{
				return "CAMRIGHT";
			}
			if (WasGamePadButtonPressed("LT", compatGamePadState.Triggers.Left > 0.35f))
			{
				return "CAMIN";
			}
			if (WasGamePadButtonPressed("RT", compatGamePadState.Triggers.Right > 0.35f))
			{
				return "CAMOUT";
			}
			if (WasGamePadButtonPressed("RSU", compatGamePadState.ThumbSticks.Right.Y > 0.45f))
			{
				return "CAMUP";
			}
			if (WasGamePadButtonPressed("RSD", compatGamePadState.ThumbSticks.Right.Y < -0.45f))
			{
				return "CAMDOWN";
			}
			if (WasGamePadButtonPressed("RSL", compatGamePadState.ThumbSticks.Right.X < -0.45f))
			{
				return "CAMLEFT";
			}
			if (WasGamePadButtonPressed("RSR", compatGamePadState.ThumbSticks.Right.X > 0.45f))
			{
				return "CAMRIGHT";
			}
		}
		catch (Exception ex)
		{
			Mark("gamepad-stage-command-failed-" + DescribeException(ex));
		}
		return null;
	}

	public static void Mark(string name)
	{
		if (name != null && name.IndexOf("afterDraw", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			CaptureBackbufferOnce(name);
		}
		if (!Seen.TryAdd(name, 0))
		{
			return;
		}
		try
		{
			EnsureRuntimeDirectory();
			string contents = $"{DateTime.UtcNow:O} pid={Process.GetCurrentProcess().Id} tid={Thread.CurrentThread.ManagedThreadId} {name}{Environment.NewLine}";
			File.AppendAllText(LogPath, contents);
		}
		catch
		{
		}
	}

	private static void EnsureRuntimeDirectory()
	{
		if (Interlocked.Exchange(ref RuntimeDirectoryReady, 1) != 0)
		{
			return;
		}
		Directory.CreateDirectory(RuntimeDataDir);
		try
		{
			if (File.Exists(LogPath) && new FileInfo(LogPath).Length > 2097152)
			{
				string text = Path.Combine(RuntimeDataDir, "compat-hooks.previous.log");
				if (File.Exists(text))
				{
					File.Delete(text);
				}
				File.Move(LogPath, text);
			}
		}
		catch
		{
		}
	}

	public static void MarkRenderItem(object item)
	{
		if (item == null)
		{
			Mark("RenderItem.null");
			return;
		}
		ApplyForcedStageCameraDuringRender();
		string elementName = GetElementName(item);
		string fullName = item.GetType().FullName;
		Mark("RenderItem." + (string.IsNullOrEmpty(elementName) ? "<unnamed>" : elementName) + "." + fullName);
	}

	private static void ApplyForcedStageCameraDuringRender()
	{
		try
		{
			int num = Environment.TickCount & 0x7FFFFFFF;
			if (num != Volatile.Read(ref renderCameraTick))
			{
				object currentGameSection = GetCurrentGameSection();
				if (currentGameSection != null && currentGameSection.GetType().FullName.IndexOf(".Sections.StageSection", StringComparison.Ordinal) >= 0)
				{
					Interlocked.Exchange(ref renderCameraTick, num);
					ApplyForcedStageCamera(currentGameSection);
				}
			}
		}
		catch
		{
		}
	}

	public static void MarkType(string label, object value)
	{
		string text = ((value != null) ? value.GetType().FullName : "<null>");
		Mark(label + ":" + text);
	}

	public static void MarkFloats(string label, float a, float b, float c, float d)
	{
		Mark(string.Format(CultureInfo.InvariantCulture, "{0}:{1},{2},{3},{4}", label, a, b, c, d));
	}

	public static void RenderMeshSafely(object mesh, object transform, object owner)
	{
		if (mesh == null || transform == null)
		{
			return;
		}
		try
		{
			MethodInfo methodInfo = mesh.GetType().GetMethod("Render", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[1] { transform.GetType() }, null);
			if (methodInfo == null)
			{
				MethodInfo[] methods = mesh.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				for (int i = 0; i < methods.Length; i++)
				{
					if (methods[i].Name == "Render" && methods[i].GetParameters().Length == 1)
					{
						methodInfo = methods[i];
						break;
					}
				}
			}
			if (methodInfo != null)
			{
				methodInfo.Invoke(mesh, new object[1] { transform });
				LogFinalShaderBindingOnce(mesh);
			}
		}
		catch (Exception ex)
		{
			string text = ((owner == null) ? "null" : owner.GetType().FullName);
			string fullName = mesh.GetType().FullName;
			Mark("mesh-render-failed-" + text + "-" + fullName + "-" + DescribeException(ex));
		}
	}

	private static void LogFinalShaderBindingOnce(object mesh)
	{
		if (!EnableRenderDiagnostics)
		{
			return;
		}
		try
		{
			object obj = InvokeProperty(mesh, "Shader");
			Effect effect = ((obj == null) ? null : (GetField(obj, "effect") as Effect));
			if (effect == null)
			{
				return;
			}
			string text = string.Empty;
			int num = 0;
			foreach (EffectParameter parameter in effect.Parameters)
			{
				if (num < 24)
				{
					if (text.Length > 0)
					{
						text += ",";
					}
					text = text + parameter.Name + ":" + parameter.Semantic;
				}
				num++;
			}
			bool flag = text.IndexOf("BaseTexture", StringComparison.OrdinalIgnoreCase) >= 0 && text.IndexOf("FloatParameter0", StringComparison.OrdinalIgnoreCase) >= 0 && text.IndexOf("FloatParameter1", StringComparison.OrdinalIgnoreCase) >= 0;
			if (text.IndexOf("ShaderIndex", StringComparison.OrdinalIgnoreCase) >= 0 || (!flag && Interlocked.Decrement(ref HlslDiagnosticBudget) < 0))
			{
				return;
			}
			string text2 = (flag ? "final-shader-binding" : "hlsl-shader-binding") + "-params-" + text;
			Texture2D texture2D = null;
			EffectParameter effectParameter = effect.Parameters["BaseTexture"];
			if (effectParameter != null)
			{
				try
				{
					texture2D = effectParameter.GetValueTexture2D();
				}
				catch
				{
				}
			}
			object obj3 = InvokeProperty(mesh, "FirstMaterial");
			ICollection collection = ((obj3 == null) ? null : InvokeProperty(obj3, "Textures")) as ICollection;
			Mark(text2 + "-technique-" + ((effect.CurrentTechnique == null) ? "null" : effect.CurrentTechnique.Name) + "-materialTextures-" + (collection?.Count ?? (-1)) + "-bound-" + ((texture2D == null) ? "null" : (texture2D.Width + "x" + texture2D.Height)) + "-parameterCount-" + num);
		}
		catch (Exception ex)
		{
			Mark("final-shader-binding-failed-" + DescribeException(ex));
		}
	}

	public static void DrawEngineSafely(object engine)
	{
		try
		{
			if (engine == null)
			{
				return;
			}
			PumpMainThreadAssetPreload(1);
			ValidateAudioRuntime();
			EnsureMenuBackgroundScene();
			GraphicsDevice graphicsDevice = GetField(engine, "device") as GraphicsDevice;
			BlendState blendState = GetField(engine, "alphaBlendState") as BlendState;
			if (graphicsDevice != null)
			{
				EnsureFixedViewportAndGui(graphicsDevice);
				graphicsDevice.BlendState = blendState ?? BlendState.AlphaBlend;
				graphicsDevice.DepthStencilState = DepthStencilState.Default;
				graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			}
			else
			{
				Mark("draw-safe-no-device");
			}
			object field = GetField(engine, "game");
			if (!(InvokeProperty(field, "RenderProcesses") is IList list))
			{
				Mark("draw-safe-no-render-processes");
				return;
			}
			bool flag = false;
			for (int i = 0; i < list.Count; i++)
			{
				flag = RenderCompatProcessAt(list, i, graphicsDevice, flag);
			}
			if (!flag && graphicsDevice != null && UseCompatGeneratedAvatar)
			{
				DrawCompatGeneratedAvatarForCurrentSection(graphicsDevice);
				flag = true;
			}
		}
		catch (Exception ex)
		{
			Mark("draw-safe-failed-" + DescribeException(ex));
		}
	}

	private static void ValidateAudioRuntime()
	{
		if (Volatile.Read(ref AudioRuntimeValidationComplete) != 0)
		{
			return;
		}
		int num = Environment.TickCount & 0x7FFFFFFF;
		if (num < Volatile.Read(ref NextAudioRuntimeValidationTick))
		{
			return;
		}
		Interlocked.Exchange(ref NextAudioRuntimeValidationTick, num + 500);
		try
		{
			Type type = Type.GetType("Quasar.GameUtils.Audio.XACTJukebox, QuasarGameUtils");
			if (type == null)
			{
				return;
			}
			PropertyInfo property = type.GetProperty("HasInstance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (!(property != null) || !Convert.ToBoolean(property.GetValue(null, null)))
			{
				return;
			}
			PropertyInfo property2 = type.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			object target = ((property2 == null) ? null : property2.GetValue(null, null));
			object field = GetField(target, "audioEngine");
			object field2 = GetField(target, "soundBank");
			object field3 = GetField(target, "waveBank");
			object field4 = GetField(target, "currentSong");
			object obj = InvokeProperty(target, "CurrentSongList");
			object obj2 = ((field4 == null) ? null : InvokeProperty(field4, "IsPlaying"));
			Type type2 = Type.GetType("Quasar.Audio, Quasar");
			object obj3 = ((type2 == null) ? null : InvokeStaticProperty(type2, "SoundVolume"));
			object obj4 = ((type2 == null) ? null : InvokeStaticProperty(type2, "MusicVolume"));
			if (field != null && field2 != null && field3 != null && field4 != null)
			{
				Mark("audio-runtime-ready-song-" + (obj ?? "<unknown>")?.ToString() + "-playing-" + (obj2 ?? "<unknown>")?.ToString() + "-sound-" + (obj3 ?? "<unknown>")?.ToString() + "-music-" + (obj4 ?? "<unknown>"));
				Interlocked.Exchange(ref AudioRuntimeValidationComplete, 1);
				return;
			}
			int num2 = Interlocked.Increment(ref AudioRuntimeValidationAttempts);
			if (num2 >= 60)
			{
				Mark("audio-runtime-not-ready-engine-" + (field != null) + "-soundbank-" + (field2 != null) + "-wavebank-" + (field3 != null) + "-cue-" + (field4 != null));
				Interlocked.Exchange(ref AudioRuntimeValidationComplete, 1);
			}
		}
		catch (Exception ex)
		{
			if (Interlocked.Increment(ref AudioRuntimeValidationAttempts) >= 60)
			{
				Mark("audio-runtime-validation-failed-" + DescribeException(ex));
				Interlocked.Exchange(ref AudioRuntimeValidationComplete, 1);
			}
		}
	}

	private static object InvokeStaticProperty(Type type, string name)
	{
		PropertyInfo property = type.GetProperty(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		return (property == null) ? null : property.GetValue(null, null);
	}

	private static void PumpMainThreadAssetPreload(int maximumAssets)
	{
		if (Volatile.Read(ref MainThreadPreloadComplete) != 0 || maximumAssets <= 0)
		{
			return;
		}
		lock (MainThreadPreloadLock)
		{
			if (MainThreadPreloadComplete != 0)
			{
				return;
			}
			if (MainThreadPreloadAssets == null)
			{
				MainThreadPreloadAssets = BuildMainThreadPreloadList();
				Mark("main-thread-preload-planned-" + MainThreadPreloadAssets.Length);
			}
			int num = 0;
			while (num < maximumAssets && MainThreadPreloadIndex < MainThreadPreloadAssets.Length)
			{
				string text = MainThreadPreloadAssets[MainThreadPreloadIndex++];
				num++;
				try
				{
					Engine.ContentManager.Load<object>(text);
				}
				catch (Exception ex)
				{
					MainThreadPreloadFailureCount++;
					if (MainThreadPreloadFailureLogBudget > 0)
					{
						MainThreadPreloadFailureLogBudget--;
						Mark("main-thread-preload-skip-" + text.Replace('/', '_') + "-" + DescribeException(ex));
					}
				}
			}
			if (MainThreadPreloadIndex >= MainThreadPreloadAssets.Length)
			{
				MainThreadPreloadComplete = 1;
				Mark("main-thread-preload-complete-" + MainThreadPreloadAssets.Length + "-skipped-" + MainThreadPreloadFailureCount);
			}
		}
	}

	private static void FinishMainThreadAssetPreload()
	{
		while (Volatile.Read(ref MainThreadPreloadComplete) == 0)
		{
			PumpMainThreadAssetPreload(24);
		}
	}

	private static string[] BuildMainThreadPreloadList()
	{
		string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content");
		if (!Directory.Exists(text))
		{
			Mark("main-thread-preload-content-missing");
			return new string[0];
		}
		List<string> list = new List<string>();
		HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		string[] array = new string[11]
		{
			"Fonts", "Textures/GUI", "Textures/Backgrounds", "Textures/HUD", "Textures/ShopIcons", "Animations", "Avatar", "Models", "Particles", "Sounds",
			"Textures"
		};
		for (int i = 0; i < array.Length; i++)
		{
			AddPreloadAssets(text, array[i], list, seen);
		}
		string[] files = Directory.GetFiles(text, "*.xnb", SearchOption.AllDirectories);
		Array.Sort(files, StringComparer.OrdinalIgnoreCase);
		for (int j = 0; j < files.Length; j++)
		{
			AddPreloadAsset(text, files[j], list, seen);
		}
		return list.ToArray();
	}

	private static void AddPreloadAssets(string contentRoot, string relativeRoot, List<string> assets, HashSet<string> seen)
	{
		string path = Path.Combine(contentRoot, relativeRoot.Replace('/', Path.DirectorySeparatorChar));
		if (Directory.Exists(path))
		{
			string[] files = Directory.GetFiles(path, "*.xnb", SearchOption.AllDirectories);
			Array.Sort(files, StringComparer.OrdinalIgnoreCase);
			for (int i = 0; i < files.Length; i++)
			{
				AddPreloadAsset(contentRoot, files[i], assets, seen);
			}
		}
	}

	private static void AddPreloadAsset(string contentRoot, string path, List<string> assets, HashSet<string> seen)
	{
		string path2 = path.Substring(contentRoot.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
		path2 = Path.ChangeExtension(path2, null).Replace('\\', '/');
		if (!path2.StartsWith("Shaders/", StringComparison.OrdinalIgnoreCase) && !path2.StartsWith("Avatar/AnimationData/", StringComparison.OrdinalIgnoreCase) && seen.Add(path2))
		{
			assets.Add(path2);
		}
	}

	private static bool RenderCompatProcessAt(IList renderProcesses, int i, GraphicsDevice device, bool drewCompatAvatar)
	{
		try
		{
			object obj = null;
			try
			{
				obj = renderProcesses[i];
			}
			catch
			{
				return drewCompatAvatar;
			}
			if (obj == null)
			{
				return drewCompatAvatar;
			}
			LogRenderProcessStateOnce(obj, i, device);
			EnsureFixedViewportAndGui(device);
			MethodInfo method = obj.GetType().GetMethod("Render", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method != null)
			{
				method.Invoke(obj, null);
			}
			LogStageRenderTargetMetricsOnce(obj, i);
			EnsureFixedViewportAndGui(device);
			if (!drewCompatAvatar && device != null && UseCompatGeneratedAvatar && ShouldDrawCompatAvatarAfterRenderProcess(obj, i, renderProcesses.Count))
			{
				DrawCompatGeneratedAvatarForCurrentSection(device);
				drewCompatAvatar = true;
			}
		}
		catch (Exception ex)
		{
			Mark("draw-safe-render-process-" + i + "-failed-" + DescribeException(ex));
		}
		return drewCompatAvatar;
	}

	private static void LogStageRenderTargetMetricsOnce(object renderProcess, int index)
	{
		if (!EnableRenderDiagnostics)
		{
			return;
		}
		try
		{
			object currentGameSection = GetCurrentGameSection();
			string text = ((currentGameSection == null) ? string.Empty : currentGameSection.GetType().FullName);
			if (text.IndexOf(".Sections.StageSection", StringComparison.Ordinal) < 0)
			{
				return;
			}
			string text2 = "stage-target-metrics-" + index + "-" + renderProcess.GetType().FullName;
			if (!Seen.TryAdd(text2, 0))
			{
				return;
			}
			if (InvokeProperty(renderProcess, "RenderTarget") is Texture2D { Width: >0, Height: >0 } texture2D)
			{
				Microsoft.Xna.Framework.Color[] array = new Microsoft.Xna.Framework.Color[texture2D.Width * texture2D.Height];
				texture2D.GetData(array);
				long num = 0L;
				long num2 = 0L;
				long num3 = 0L;
				long num4 = 0L;
				long num5 = 0L;
				long num6 = 0L;
				int num7 = Math.Max(1, Math.Min(texture2D.Width, texture2D.Height) / 80);
				for (int i = num7 / 2; i < texture2D.Height; i += num7)
				{
					int num8 = i * texture2D.Width;
					for (int j = num7 / 2; j < texture2D.Width; j += num7)
					{
						Microsoft.Xna.Framework.Color color = array[num8 + j];
						num += color.R;
						num2 += color.G;
						num3 += color.B;
						if (color.R + color.G + color.B < 30)
						{
							num4++;
						}
						if (color.R > 245 && color.G > 245 && color.B > 245)
						{
							num5++;
						}
						num6++;
					}
				}
				if (num6 == 0)
				{
					Mark(text2 + "-empty");
					return;
				}
				Mark(text2 + "-size-" + texture2D.Width + "x" + texture2D.Height + "-rgb-" + num / num6 + "," + num2 / num6 + "," + num3 / num6 + "-darkpermille-" + num4 * 1000 / num6 + "-whitepermille-" + num5 * 1000 / num6);
			}
			else
			{
				Mark(text2 + "-no-target");
			}
		}
		catch (Exception ex)
		{
			Mark("stage-target-metrics-" + index + "-" + renderProcess.GetType().FullName + "-failed-" + DescribeException(ex));
		}
	}

	private static void LogRenderProcessStateOnce(object renderProcess, int index, GraphicsDevice device)
	{
		try
		{
			string fullName = renderProcess.GetType().FullName;
			string key = "render-state-" + index + "-" + fullName;
			if (!Seen.TryAdd(key, 0))
			{
				return;
			}
			string text = "backbuffer";
			if (device != null)
			{
				RenderTargetBinding[] renderTargets = device.GetRenderTargets();
				if (renderTargets != null && renderTargets.Length != 0)
				{
					text = ((!(renderTargets[0].RenderTarget is Texture2D { Width: var width } texture2D)) ? renderTargets[0].RenderTarget.GetType().FullName : (width + "x" + texture2D.Height));
				}
				Viewport viewport = device.Viewport;
				Mark("render-state-" + index + "-" + fullName + "-target-" + text + "-viewport-" + viewport.X + "," + viewport.Y + "," + viewport.Width + "," + viewport.Height);
				LogRenderProcessSourcesOnce(renderProcess, index);
			}
			else
			{
				Mark("render-state-" + index + "-" + fullName + "-no-device");
			}
		}
		catch (Exception ex)
		{
			Mark("render-state-" + index + "-failed-" + DescribeException(ex));
		}
	}

	private static void LogRenderProcessSourcesOnce(object renderProcess, int processIndex)
	{
		try
		{
			object field = GetField(renderProcess, "finalRenderPass");
			IEnumerable enumerable = ((field == null) ? null : (GetField(field, "sources") as IEnumerable));
			if (enumerable == null)
			{
				Mark("render-sources-" + processIndex + "-none");
				return;
			}
			int num = 0;
			foreach (object item in enumerable)
			{
				object obj = InvokeProperty(item, "Scene");
				object obj2 = InvokeProperty(item, "Camera");
				object obj3 = InvokeProperty(item, "Viewport");
				Mark("render-source-" + processIndex + "-" + num + "-scene-" + ((obj == null) ? "null" : obj.GetType().FullName) + "-camera-" + ((obj2 == null) ? "null" : obj2.GetType().FullName) + "-viewport-" + ((obj3 == null) ? "default" : obj3.ToString()));
				num++;
			}
			if (num == 0)
			{
				Mark("render-sources-" + processIndex + "-empty");
			}
		}
		catch (Exception ex)
		{
			Mark("render-sources-" + processIndex + "-failed-" + DescribeException(ex));
		}
	}

	public static void RegisterLayout(object layout)
	{
		if (layout != null && registeredLayout != layout)
		{
			registeredLayout = layout;
			registeredLayoutUpdate = layout.GetType().GetMethod("Update", Type.EmptyTypes);
			Mark("registered-layout-" + layout.GetType().FullName);
		}
	}

	private static bool ShouldDrawCompatAvatarAfterRenderProcess(object renderProcess, int index, int count)
	{
		try
		{
			object currentGameSection = GetCurrentGameSection();
			if (currentGameSection == null || currentGameSection.GetType().FullName.IndexOf(".Sections.StageSection", StringComparison.Ordinal) < 0)
			{
				return false;
			}
			string text = ((renderProcess == null) ? "null" : renderProcess.GetType().FullName);
			Mark("render-process-" + index + "-of-" + count + "-" + text);
			string text2 = ((text == null) ? "" : text.ToLowerInvariant());
			if (text2.IndexOf("layout") >= 0 || text2.IndexOf("hud") >= 0 || text2.IndexOf("gui") >= 0 || text2.IndexOf("message") >= 0 || text2.IndexOf("shop") >= 0)
			{
				return false;
			}
			return index == Math.Max(0, count - 1);
		}
		catch
		{
			return index == 0;
		}
	}

	public static void RegisterGroup(object group)
	{
		if (group == null)
		{
			return;
		}
		registeredGroup = group;
		lock (registeredGroups)
		{
			if (!registeredGroups.Contains(group))
			{
				registeredGroups.Add(group);
			}
		}
		Mark("registered-group-" + group.GetType().FullName);
	}

	public static void RegisterMenuButton(object item)
	{
		if (item == null)
		{
			return;
		}
		lock (registeredMenuButtons)
		{
			if (!registeredMenuButtons.Contains(item))
			{
				registeredMenuButtons.Add(item);
				object field = GetField(item, "button");
				object field2 = GetField(field, "text");
				Mark("registered-menu-button-" + registeredMenuButtons.Count + "-" + field2);
			}
		}
	}

	public static void StartOfflineFarm()
	{
		try
		{
			FinishMainThreadAssetPreload();
			EnsurePlayerStorageDevice();
			EnsurePlayerStorageContainer();
			Type type = Type.GetType("Quasar.GameUtils.Logic.Mode.GameManager, QuasarGameUtils");
			Type type2 = Type.GetType("AvatarFarmOnline.Logic.Mode.Farm.FarmGameSetup, AvatarFarmOnline") ?? Type.GetType("AvatarFarm2.Logic.Mode.Farm.FarmGameSetup, AvatarFarmOnline");
			if (type == null || type2 == null)
			{
				Mark("offline-start-types-missing");
				return;
			}
			MethodInfo method = type.GetMethod("get_PersistentData", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			object obj = ((method == null) ? null : method.Invoke(null, null));
			if (obj == null)
			{
				MethodInfo method2 = type.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				MethodInfo method3 = type.GetMethod("SetSetup", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				MethodInfo method4 = type.GetMethod("StartGame", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				Type type3 = Type.GetType("Microsoft.Xna.Framework.PlayerIndex, FNA");
				object obj2 = ((type3 == null) ? ((object)0) : Enum.ToObject(type3, 0));
				object obj3 = Activator.CreateInstance(type2, obj2);
				if (method2 != null)
				{
					method2.Invoke(null, null);
				}
				method3.Invoke(null, new object[1] { obj3 });
				method4.Invoke(null, null);
				obj = method.Invoke(null, null);
			}
			object obj4 = InvokeProperty(obj, "FarmManager");
			object obj5 = InvokeProperty(obj4, "Farms");
			IList list = obj5 as IList;
			object obj6 = null;
			if (list != null && list.Count > 0)
			{
				obj6 = list[0];
			}
			else if (obj4 != null)
			{
				MethodInfo method5 = obj4.GetType().GetMethod("CreateFarm", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (method5 != null)
				{
					object[] array = new object[2] { "My Farm", null };
					if (method5.Invoke(obj4, array) is int num && num != 0)
					{
						obj6 = array[1];
					}
				}
			}
			if (obj6 == null)
			{
				Mark("offline-start-no-farm");
				return;
			}
			ForceSaveFarmFiles(obj4, obj6);
			ValidateFarmLoad(obj6);
			MethodInfo method6 = obj.GetType().GetMethod("SetFarm", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			method6.Invoke(obj, new object[1] { obj6 });
			if (!StartStageDirect())
			{
				SetNextGameSectionId(12);
			}
			Mark("offline-start-ok");
		}
		catch (Exception ex)
		{
			Mark("offline-start-failed-" + DescribeException(ex));
		}
	}

	private static bool StartStageDirect()
	{
		try
		{
			Type type = Type.GetType("Quasar.GameUtils.Logic.Mode.GameManager, QuasarGameUtils");
			Type type2 = Type.GetType("Quasar.GameUtils.Game.BaseGame, QuasarGameUtils");
			Type type3 = Type.GetType("AvatarFarmOnline.Sections.StageSection, AvatarFarmOnline") ?? Type.GetType("AvatarFarm2.Sections.StageSection, AvatarFarmOnline");
			if (type == null || type2 == null || type3 == null)
			{
				Mark("direct-stage-types-missing");
				return false;
			}
			MethodInfo method = type.GetMethod("PrepareNextRound", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (method != null)
			{
				method.Invoke(null, null);
			}
			MethodInfo method2 = type.GetMethod("get_CurrentGame", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			object obj = ((method2 == null) ? null : method2.Invoke(null, null));
			MethodInfo methodInfo = obj?.GetType().GetMethod("LoadStage", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			object obj2 = ((methodInfo == null) ? null : methodInfo.Invoke(obj, null));
			if (obj2 == null)
			{
				Mark("direct-stage-no-stage");
				return false;
			}
			ConstructorInfo constructorInfo = null;
			ConstructorInfo[] constructors = type3.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (ConstructorInfo constructorInfo2 in constructors)
			{
				ParameterInfo[] parameters = constructorInfo2.GetParameters();
				if (parameters.Length == 1 && parameters[0].ParameterType.IsInstanceOfType(obj2))
				{
					constructorInfo = constructorInfo2;
					break;
				}
			}
			object obj3 = ((constructorInfo == null) ? null : constructorInfo.Invoke(new object[1] { obj2 }));
			if (obj3 == null)
			{
				Mark("direct-stage-no-section");
				return false;
			}
			MethodInfo method3 = obj3.GetType().GetMethod("InitScenes", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method3 != null)
			{
				method3.Invoke(obj3, null);
				ApplyStageSceneFilter(obj3);
			}
			MethodInfo method4 = obj.GetType().GetMethod("StartGame", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method4 != null)
			{
				method4.Invoke(obj, null);
				Mark("direct-stage-startgame");
			}
			ForceCameraState(obj2, 0, "normal");
			MethodInfo method5 = type2.GetMethod("get_Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			object obj4 = ((method5 == null) ? null : method5.Invoke(null, null));
			MethodInfo method6 = type2.GetMethod("set_NextGameSection", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (obj4 == null || method6 == null)
			{
				Mark("direct-stage-no-basegame-setter");
				return false;
			}
			method6.Invoke(obj4, new object[1] { obj3 });
			Mark("direct-stage-ok");
			return true;
		}
		catch (Exception ex)
		{
			Mark("direct-stage-failed-" + DescribeException(ex));
			try
			{
				File.AppendAllText(ExceptionLogPath, DateTime.UtcNow.ToString("O") + " direct stage " + ex?.ToString() + Environment.NewLine);
			}
			catch
			{
			}
			return false;
		}
	}

	private static void ForceCameraState(object stage, int stateValue, string label)
	{
		try
		{
			object obj = InvokeProperty(stage, "LocalPlayer");
			if (obj == null)
			{
				return;
			}
			MethodInfo method = obj.GetType().GetMethod("SetCameraState", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method == null)
			{
				return;
			}
			Type parameterType = method.GetParameters()[0].ParameterType;
			object obj2 = Enum.ToObject(parameterType, stateValue);
			method.Invoke(obj, new object[1] { obj2 });
			if (stateValue == 0)
			{
				MethodInfo method2 = obj.GetType().GetMethod("SetOrientation", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				object obj3 = InvokeProperty(obj, "Orientation");
				object obj4 = ((obj3 == null) ? null : Activator.CreateInstance(obj3.GetType(), -0.75f, -0.25f));
				if (method2 != null && obj4 != null)
				{
					method2.Invoke(obj, new object[1] { obj4 });
				}
			}
			Mark("direct-stage-camera-" + label);
		}
		catch (Exception ex)
		{
			Mark("direct-stage-camera-failed-" + DescribeException(ex));
		}
	}

	private static void ApplyStageSceneFilter(object section)
	{
		try
		{
			object field = GetField(section, "stageScene");
			if (field == null)
			{
				Mark("scene-filter-no-stage-scene");
				return;
			}
			string[] array = ReadSceneFilterLines();
			object obj = FindStageSceneBackgroundRoot(field);
			if (obj == null)
			{
				Mark("scene-filter-no-background-root");
				DumpSceneElements(field, "scene-filter-stage-scene", 2);
				if (array.Length != 0)
				{
					ApplySceneElementFilter(field, array);
				}
			}
			else
			{
				DumpElementNames(obj, "scene-filter-background-root", 1);
				if (array.Length == 0)
				{
					Mark("scene-filter-none");
					return;
				}
				ApplyElementFilter(obj, array, childList: true);
				ApplySceneElementFilter(field, array);
			}
		}
		catch (Exception ex)
		{
			Mark("scene-filter-failed-" + DescribeException(ex));
		}
	}

	private static string[] ReadSceneFilterLines()
	{
		try
		{
			if (!File.Exists(SceneFilterPath))
			{
				return new string[0];
			}
			string[] array = File.ReadAllLines(SceneFilterPath);
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < array.Length; i++)
			{
				string text = ((array[i] == null) ? "" : array[i].Trim().ToUpperInvariant());
				if (text.Length > 0)
				{
					arrayList.Add(text);
				}
			}
			string[] array2 = new string[arrayList.Count];
			arrayList.CopyTo(array2);
			return array2;
		}
		catch
		{
			return new string[0];
		}
	}

	private static string[] MergeFilters(string[] first, string[] second)
	{
		ArrayList arrayList = new ArrayList();
		if (first != null)
		{
			for (int i = 0; i < first.Length; i++)
			{
				if (!string.IsNullOrEmpty(first[i]) && !arrayList.Contains(first[i]))
				{
					arrayList.Add(first[i]);
				}
			}
		}
		if (second != null)
		{
			for (int j = 0; j < second.Length; j++)
			{
				if (!string.IsNullOrEmpty(second[j]) && !arrayList.Contains(second[j]))
				{
					arrayList.Add(second[j]);
				}
			}
		}
		string[] array = new string[arrayList.Count];
		arrayList.CopyTo(array);
		return array;
	}

	private static bool ShouldFilterSceneElement(string[] filters, object element)
	{
		if (filters == null || filters.Length == 0 || element == null)
		{
			return false;
		}
		string elementName = GetElementName(element);
		string text = (string.IsNullOrEmpty(elementName) ? "" : elementName.ToUpperInvariant());
		string text2 = element.GetType().Name.ToUpperInvariant();
		string text3 = element.GetType().FullName.ToUpperInvariant();
		foreach (string text4 in filters)
		{
			if (text4 == text || text4 == text2 || text4 == text3)
			{
				return true;
			}
		}
		return false;
	}

	private static object FindStageSceneBackgroundRoot(object stageScene)
	{
		IList sceneElementList = GetSceneElementList(stageScene);
		if (sceneElementList == null)
		{
			return null;
		}
		for (int i = 0; i < sceneElementList.Count; i++)
		{
			object obj = sceneElementList[i];
			if (obj == null)
			{
				continue;
			}
			IList childList = GetChildList(obj);
			if (childList == null || childList.Count == 0)
			{
				continue;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			for (int j = 0; j < childList.Count; j++)
			{
				string elementName = GetElementName(childList[j]);
				if (string.Equals(elementName, "Sky", StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
				}
				if (string.Equals(elementName, "Floor", StringComparison.OrdinalIgnoreCase))
				{
					flag2 = true;
				}
				if (string.Equals(elementName, "Tree", StringComparison.OrdinalIgnoreCase))
				{
					flag3 = true;
				}
			}
			if (flag | flag2 | flag3)
			{
				return obj;
			}
		}
		return null;
	}

	private static void ApplySceneElementFilter(object scene, string[] filters)
	{
		IList sceneElementList = GetSceneElementList(scene);
		if (sceneElementList == null)
		{
			Mark("scene-filter-no-scene-elements");
			return;
		}
		MethodInfo method = scene.GetType().GetMethod("Remove", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (method == null)
		{
			Mark("scene-filter-no-scene-remove");
			return;
		}
		for (int num = sceneElementList.Count - 1; num >= 0; num--)
		{
			object obj = sceneElementList[num];
			if (!ShouldFilterSceneElement(filters, obj))
			{
				ApplyRecursiveElementFilter(obj, filters);
			}
			else
			{
				method.Invoke(scene, new object[1] { obj });
				Mark("scene-filter-removed-scene-" + DescribeElement(obj));
			}
		}
	}

	private static bool RemoveSceneElement(object scene, object element, string label)
	{
		try
		{
			if (scene == null || element == null)
			{
				return false;
			}
			MethodInfo method = scene.GetType().GetMethod("Remove", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method == null)
			{
				Mark(label + "-remove-missing");
				return false;
			}
			method.Invoke(scene, new object[1] { element });
			Mark(label + "-removed-" + DescribeElement(element));
			return true;
		}
		catch (Exception ex)
		{
			Mark(label + "-remove-failed-" + DescribeException(ex));
			return false;
		}
	}

	private static void ApplyElementFilter(object root, string[] filters, bool childList)
	{
		IList list = (childList ? GetChildList(root) : null);
		if (list == null)
		{
			Mark("scene-filter-no-children");
			return;
		}
		MethodInfo method = root.GetType().GetMethod("removeChild", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (method == null)
		{
			Mark("scene-filter-no-remove-child");
			return;
		}
		for (int num = list.Count - 1; num >= 0; num--)
		{
			object obj = list[num];
			if (ShouldFilterSceneElement(filters, obj))
			{
				method.Invoke(root, new object[1] { obj });
				Mark("scene-filter-removed-child-" + DescribeElement(obj));
			}
		}
	}

	private static void ApplyRecursiveElementFilter(object root, string[] filters)
	{
		IList childList = GetChildList(root);
		if (childList == null)
		{
			return;
		}
		MethodInfo method = root.GetType().GetMethod("removeChild", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (method == null)
		{
			return;
		}
		for (int num = childList.Count - 1; num >= 0; num--)
		{
			object obj = childList[num];
			if (ShouldFilterSceneElement(filters, obj))
			{
				method.Invoke(root, new object[1] { obj });
				Mark("scene-filter-removed-recursive-" + DescribeElement(obj));
			}
			else
			{
				ApplyRecursiveElementFilter(obj, filters);
			}
		}
	}

	private static void DumpSceneElements(object scene, string label, int depth)
	{
		IList sceneElementList = GetSceneElementList(scene);
		if (sceneElementList != null)
		{
			for (int i = 0; i < sceneElementList.Count; i++)
			{
				DumpElementNamesRecursive(sceneElementList[i], label, 0, depth);
			}
		}
	}

	private static void DumpElementNames(object element, string label, int depth)
	{
		if (element != null && depth >= 0)
		{
			DumpElementNamesRecursive(element, label, 0, depth);
		}
	}

	private static void DumpElementNamesRecursive(object element, string label, int level, int maxDepth)
	{
		if (element == null || level > maxDepth)
		{
			return;
		}
		string text = new string('.', level);
		string elementName = GetElementName(element);
		string fullName = element.GetType().FullName;
		Mark(label + "-" + text + ((elementName.Length == 0) ? "<unnamed>" : elementName) + "-" + fullName);
		IList childList = GetChildList(element);
		if (childList != null)
		{
			for (int i = 0; i < childList.Count; i++)
			{
				DumpElementNamesRecursive(childList[i], label, level + 1, maxDepth);
			}
		}
	}

	private static string GetElementName(object element)
	{
		if (element == null)
		{
			return "";
		}
		FieldInfo fieldInfo = GetFieldInfo(element.GetType(), "Name");
		object obj = ((fieldInfo == null) ? null : fieldInfo.GetValue(element));
		return (obj == null) ? "" : obj.ToString();
	}

	private static string DescribeElement(object element)
	{
		if (element == null)
		{
			return "null";
		}
		string elementName = GetElementName(element);
		return ((elementName.Length == 0) ? "<unnamed>" : elementName) + "-" + element.GetType().Name;
	}

	private static void LogElementTransform(object element, string label)
	{
		try
		{
			object obj = InvokeProperty(element, "Transform");
			if (obj == null)
			{
				Mark(label + "-no-transform");
				return;
			}
			object value = InvokeProperty(obj, "Translation");
			object value2 = InvokeProperty(obj, "Scale");
			object value3 = InvokeProperty(obj, "Rotation");
			Mark(label + "-t-" + FormatObject(value) + "-s-" + FormatObject(value2) + "-r-" + FormatObject(value3));
		}
		catch (Exception ex)
		{
			Mark(label + "-failed-" + DescribeException(ex));
		}
	}

	private static string FormatObject(object value)
	{
		if (value == null)
		{
			return "null";
		}
		return value.ToString().Replace(" ", "");
	}

	private static IList GetChildList(object element)
	{
		return (InvokeProperty(element, "Children") as IList) ?? (GetField(element, "children") as IList);
	}

	private static IList GetSceneElementList(object scene)
	{
		return (InvokeProperty(scene, "Elements") as IList) ?? (GetField(scene, "elements") as IList);
	}

	private static void ValidateFarmLoad(object farm)
	{
		try
		{
			if (farm == null)
			{
				Mark("validate-load-no-farm");
				return;
			}
			EnsurePlayerStorageDevice();
			EnsurePlayerStorageContainer();
			MethodInfo method = farm.GetType().GetMethod("LoadFarm", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method == null)
			{
				Mark("validate-load-no-method");
				return;
			}
			object obj = method.Invoke(farm, null);
			Mark("validate-load-ok-" + ((obj == null) ? "null" : obj.GetType().FullName));
		}
		catch (Exception ex)
		{
			Mark("validate-load-failed-" + DescribeException(ex));
			try
			{
				File.AppendAllText(ExceptionLogPath, DateTime.UtcNow.ToString("O") + " validate load " + ex?.ToString() + Environment.NewLine);
			}
			catch
			{
			}
		}
	}

	private static void ForceSaveFarmFiles(object farmManager, object farm)
	{
		try
		{
			EnsurePlayerStorageDevice();
			EnsurePlayerStorageContainer();
			object playerStorageContainer = GetPlayerStorageContainer();
			if (playerStorageContainer == null)
			{
				Mark("force-save-no-container");
				return;
			}
			ProbeContainerWrite(playerStorageContainer);
			if (farmManager != null)
			{
				MethodInfo method = farmManager.GetType().GetMethod("Save", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[2]
				{
					playerStorageContainer.GetType(),
					typeof(object)
				}, null);
				if (method != null)
				{
					method.Invoke(farmManager, new object[2] { playerStorageContainer, null });
					Mark("force-save-farm-manager");
				}
				else
				{
					Mark("force-save-no-manager-save");
				}
			}
			Type type = Type.GetType("AvatarFarmOnline.Logic.Stage.FarmData, AvatarFarmOnline") ?? Type.GetType("AvatarFarm2.Logic.Stage.FarmData, AvatarFarmOnline");
			if (type == null || farm == null)
			{
				Mark("force-save-farmdata-type-missing");
				return;
			}
			object obj = Activator.CreateInstance(type, nonPublic: true);
			FieldInfo fieldInfo = GetFieldInfo(type, "localHeader");
			if (fieldInfo != null)
			{
				fieldInfo.SetValue(obj, farm);
			}
			MethodInfo method2 = type.GetMethod("Init", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
			if (method2 != null)
			{
				method2.Invoke(obj, null);
			}
			MethodInfo method3 = type.GetMethod("Save", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[2]
			{
				playerStorageContainer.GetType(),
				typeof(object)
			}, null);
			if (method3 != null)
			{
				method3.Invoke(obj, new object[2] { playerStorageContainer, null });
				Mark("force-save-farm-data");
			}
			else
			{
				Mark("force-save-no-farmdata-save");
			}
		}
		catch (Exception ex)
		{
			Mark("force-save-failed-" + DescribeException(ex));
			try
			{
				File.AppendAllText(ExceptionLogPath, DateTime.UtcNow.ToString("O") + " force save " + ex?.ToString() + Environment.NewLine);
			}
			catch
			{
			}
		}
	}

	private static void ProbeContainerWrite(object container)
	{
		try
		{
			FieldInfo fieldInfo = FindField(container.GetType(), "storagePath", "_rootPath", "rootPath", "root");
			object obj = ((fieldInfo == null) ? null : fieldInfo.GetValue(container));
			Mark("storage-container-root-" + ((obj == null) ? "null" : obj.ToString().Replace('\\', '/')));
			MethodInfo method = container.GetType().GetMethod("OpenFile", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[2]
			{
				typeof(string),
				typeof(FileMode)
			}, null);
			if (method == null)
			{
				Mark("storage-probe-no-openfile");
				return;
			}
			using (Stream stream = (Stream)method.Invoke(container, new object[2]
			{
				"compat-test.txt",
				FileMode.Create
			}))
			{
				byte[] bytes = Encoding.ASCII.GetBytes("ok");
				stream.Write(bytes, 0, bytes.Length);
			}
			Mark("storage-probe-write-ok");
		}
		catch (Exception ex)
		{
			Mark("storage-probe-write-failed-" + DescribeException(ex));
			try
			{
				File.AppendAllText(ExceptionLogPath, DateTime.UtcNow.ToString("O") + " storage probe " + ex?.ToString() + Environment.NewLine);
			}
			catch
			{
			}
		}
	}

	private static object GetPlayerStorageContainer()
	{
		Type type = Type.GetType("Quasar.GameUtils.Storage.StorageManager, QuasarGameUtils");
		if (type == null)
		{
			return null;
		}
		MethodInfo method = type.GetMethod("get_Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		object obj = ((method == null) ? null : method.Invoke(null, null));
		FieldInfo fieldInfo = GetFieldInfo(type, "playerContainers");
		Array array = ((obj == null || fieldInfo == null) ? null : (fieldInfo.GetValue(obj) as Array));
		return (array != null && array.Length > 0) ? array.GetValue(0) : null;
	}

	private static void EnsurePlayerStorageContainer()
	{
		try
		{
			Type type = Type.GetType("Quasar.GameUtils.Storage.StorageManager, QuasarGameUtils");
			Type type2 = Type.GetType("Quasar.GameUtils.Game.BaseGame, QuasarGameUtils");
			if (type == null || type2 == null)
			{
				Mark("storage-container-types-missing");
				return;
			}
			MethodInfo method = type.GetMethod("get_Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			object obj = ((method == null) ? null : method.Invoke(null, null));
			if (obj == null)
			{
				Mark("storage-container-no-manager");
				return;
			}
			FieldInfo fieldInfo = GetFieldInfo(type, "playerContainers");
			Array array = ((fieldInfo == null) ? null : (fieldInfo.GetValue(obj) as Array));
			if (array == null || array.Length == 0)
			{
				Mark("storage-container-array-missing");
				return;
			}
			string text = "Avatar Farm Online";
			FieldInfo field = type.GetField("containerName", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			object obj2 = ((field == null) ? null : field.GetValue(null));
			if (obj2 is string && ((string)obj2).Length > 0)
			{
				text = (string)obj2;
			}
			string text2 = Path.Combine(RuntimeDataDir, "Saves", text, "Player1");
			object value = array.GetValue(0);
			if (value != null)
			{
				FieldInfo fieldInfo2 = FindField(value.GetType(), "storagePath", "_rootPath", "rootPath", "root");
				string text3 = ((fieldInfo2 == null) ? null : (fieldInfo2.GetValue(value) as string));
				if (string.Equals(Path.GetFullPath(text3 ?? string.Empty).TrimEnd('\\'), Path.GetFullPath(text2).TrimEnd('\\'), StringComparison.OrdinalIgnoreCase))
				{
					Mark("storage-container-existing-portable");
					return;
				}
				array.SetValue(null, 0);
				Mark("storage-container-replacing-nonportable-" + (text3 ?? "unknown").Replace('\\', '/'));
			}
			MethodInfo method2 = type2.GetMethod("get_Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			object obj3 = ((method2 == null) ? null : method2.Invoke(null, null));
			FieldInfo fieldInfo3 = GetFieldInfo(type2, "playerStorageDevices");
			Array array2 = ((obj3 == null || fieldInfo3 == null) ? null : (fieldInfo3.GetValue(obj3) as Array));
			object obj4 = ((array2 == null || array2.Length == 0) ? null : array2.GetValue(0));
			if (obj4 == null)
			{
				Mark("storage-container-no-device");
				return;
			}
			Type elementType = array.GetType().GetElementType();
			object uninitializedObject = FormatterServices.GetUninitializedObject(elementType);
			FieldInfo fieldInfo4 = FindField(elementType, "<StorageDevice>k__BackingField", "_device", "StorageDevice", "device");
			FieldInfo fieldInfo5 = FindField(elementType, "_playerIndex", "playerIndex");
			FieldInfo fieldInfo6 = FindField(elementType, "<DisplayName>k__BackingField", "_displayName", "DisplayName", "displayName");
			FieldInfo fieldInfo7 = FindField(elementType, "storagePath", "_rootPath", "rootPath", "root");
			FieldInfo fieldInfo8 = FindField(elementType, "<IsDisposed>k__BackingField", "_isDisposed", "IsDisposed", "isDisposed", "disposed");
			Directory.CreateDirectory(text2);
			if (fieldInfo4 != null)
			{
				fieldInfo4.SetValue(uninitializedObject, obj4);
			}
			if (fieldInfo5 != null)
			{
				object value2 = (fieldInfo5.FieldType.IsEnum ? Enum.ToObject(fieldInfo5.FieldType, 0) : ((object)0));
				fieldInfo5.SetValue(uninitializedObject, value2);
			}
			if (fieldInfo6 != null)
			{
				fieldInfo6.SetValue(uninitializedObject, text);
			}
			if (fieldInfo7 == null)
			{
				throw new MissingFieldException(elementType.FullName, "storagePath");
			}
			fieldInfo7.SetValue(uninitializedObject, text2);
			if (fieldInfo8 != null)
			{
				fieldInfo8.SetValue(uninitializedObject, false);
			}
			Mark("storage-container-built-portable-fields");
			array.SetValue(uninitializedObject, 0);
			FieldInfo fieldInfo9 = FindField(uninitializedObject.GetType(), "storagePath", "_rootPath", "rootPath", "root");
			object obj5 = ((fieldInfo9 == null) ? null : fieldInfo9.GetValue(uninitializedObject));
			if (obj5 == null)
			{
				Mark("storage-container-fields-" + ListFieldNames(uninitializedObject.GetType()));
			}
			Mark("storage-container-installed-root-" + ((obj5 == null) ? "null" : obj5.ToString().Replace('\\', '/')));
			Mark("storage-container-installed");
		}
		catch (Exception ex)
		{
			Mark("storage-container-failed-" + DescribeException(ex));
			try
			{
				File.AppendAllText(ExceptionLogPath, DateTime.UtcNow.ToString("O") + " storage container " + ex?.ToString() + Environment.NewLine);
			}
			catch
			{
			}
		}
	}

	private static string ListFieldNames(Type type)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (type != null)
			{
				FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				for (int i = 0; i < fields.Length; i++)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(fields[i].Name);
				}
				type = type.BaseType;
			}
			string text = stringBuilder.ToString();
			return (text.Length > 160) ? text.Substring(0, 160) : text;
		}
		catch
		{
			return "list-failed";
		}
	}

	private static FieldInfo FindField(Type type, params string[] names)
	{
		while (type != null)
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < names.Length; i++)
			{
				for (int j = 0; j < fields.Length; j++)
				{
					if (string.Equals(fields[j].Name, names[i], StringComparison.OrdinalIgnoreCase))
					{
						return fields[j];
					}
				}
			}
			for (int k = 0; k < names.Length; k++)
			{
				for (int l = 0; l < fields.Length; l++)
				{
					if (fields[l].Name.IndexOf(names[k], StringComparison.OrdinalIgnoreCase) >= 0)
					{
						return fields[l];
					}
				}
			}
			type = type.BaseType;
		}
		return null;
	}

	private static void EnsurePlayerStorageDevice()
	{
		try
		{
			Type type = Type.GetType("Quasar.GameUtils.Game.BaseGame, QuasarGameUtils");
			if (type == null)
			{
				Mark("storage-device-types-missing");
				return;
			}
			MethodInfo method = type.GetMethod("get_Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			object obj = ((method == null) ? null : method.Invoke(null, null));
			if (obj == null)
			{
				Mark("storage-device-no-basegame");
				return;
			}
			FieldInfo fieldInfo = GetFieldInfo(type, "playerStorageDevices");
			FieldInfo fieldInfo2 = GetFieldInfo(type, "gamerStorageDevices");
			Array array = ((fieldInfo == null) ? null : (fieldInfo.GetValue(obj) as Array));
			Array array2 = ((fieldInfo2 == null) ? null : (fieldInfo2.GetValue(obj) as Array));
			if (array == null || array.Length == 0)
			{
				Mark("storage-device-array-missing");
				return;
			}
			if (array.GetValue(0) == null)
			{
				Type elementType = array.GetType().GetElementType();
				if (elementType == null)
				{
					Mark("storage-device-type-missing");
					return;
				}
				object uninitializedObject = FormatterServices.GetUninitializedObject(elementType);
				FieldInfo field = elementType.GetField("deviceIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				FieldInfo field2 = elementType.GetField("playerIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (field != null)
				{
					field.SetValue(uninitializedObject, 0u);
				}
				if (field2 != null)
				{
					field2.SetValue(uninitializedObject, 0);
				}
				if (uninitializedObject != null)
				{
					array.SetValue(uninitializedObject, 0);
					Mark("storage-device-installed");
				}
			}
			else
			{
				Mark("storage-device-existing");
			}
			if (array2 != null && array2.Length > 0)
			{
				array2.SetValue(null, 0);
			}
		}
		catch (Exception ex)
		{
			Mark("storage-device-failed-" + DescribeException(ex));
		}
	}

	private static string DescribeException(Exception ex)
	{
		if (ex == null)
		{
			return "null";
		}
		Exception ex2 = ex;
		while (ex2.InnerException != null)
		{
			ex2 = ex2.InnerException;
		}
		string text = ((ex2.Message == null) ? "" : ex2.Message.Replace('\r', ' ').Replace('\n', ' '));
		if (text.Length > 120)
		{
			text = text.Substring(0, 120);
		}
		return ex2.GetType().Name + "-" + text;
	}

	private static object InvokeProperty(object target, string name)
	{
		if (target == null)
		{
			return null;
		}
		MethodInfo method = target.GetType().GetMethod("get_" + name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		return (method == null) ? null : method.Invoke(target, null);
	}

	private static object InvokeIndexer(object target, object key)
	{
		if (target == null)
		{
			return null;
		}
		MethodInfo method = target.GetType().GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		return (method == null) ? null : method.Invoke(target, new object[1] { key });
	}

	private static void SetNextGameSectionId(int id)
	{
		Type type = Type.GetType("Quasar.GameUtils.Game.BaseGame, QuasarGameUtils");
		if (!(type == null))
		{
			MethodInfo method = type.GetMethod("get_Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			object obj = ((method == null) ? null : method.Invoke(null, null));
			MethodInfo method2 = type.GetMethod("set_NextGameSectionId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (obj != null && method2 != null)
			{
				method2.Invoke(obj, new object[1] { id });
			}
		}
	}

	public static void UpdateRegisteredLayout()
	{
	}

	private static void RecoverFromLoadingSection()
	{
		try
		{
			object currentGameSection = GetCurrentGameSection();
			string text = ((currentGameSection == null) ? "" : currentGameSection.GetType().FullName);
			if (text.IndexOf(".Sections.LoadingSection", StringComparison.Ordinal) >= 0 && Interlocked.Exchange(ref loadingRecoveryStarted, 1) == 0)
			{
				Mark("loading-section-recovery-start");
				StartOfflineFarm();
			}
		}
		catch (Exception ex)
		{
			Mark("loading-section-recovery-failed-" + DescribeException(ex));
		}
	}

	public static void ProcessStageCommandFile()
	{
		try
		{
			object currentGameSection = GetCurrentGameSection();
			if (currentGameSection == null || currentGameSection.GetType().FullName.IndexOf(".Sections.StageSection", StringComparison.Ordinal) < 0)
			{
				return;
			}
			SyncStageScenes(currentGameSection);
			if (UseCompatFarmMesh)
			{
				EnsureGenericFarmTileMesh(currentGameSection);
				NormalizeFarmTileMesh(currentGameSection);
			}
			if (UseCompatPlayerSync)
			{
				SyncPlayerItems(currentGameSection);
			}
			ApplyForcedStageCamera(currentGameSection);
			if (!File.Exists(CommandPath))
			{
				ProcessStageKeyboard(currentGameSection);
				return;
			}
			bool flag = false;
			lock (CommandLock)
			{
				string[] array = ReadCommandLines();
				ArrayList arrayList = new ArrayList();
				for (int i = 0; i < array.Length; i++)
				{
					string text = ((array[i] == null) ? "" : array[i].Trim().ToUpperInvariant());
					if (text == "X")
					{
						text = "RECYCLE";
					}
					switch (text)
					{
					default:
						if (!(text == "DOWN"))
						{
							switch (text)
							{
							default:
								if (!(text == "CAMRESET"))
								{
									if (text.Length > 0)
									{
										arrayList.Add(array[i]);
									}
									break;
								}
								goto case "ENTER";
							case "ENTER":
							case "SPACE":
							case "ESC":
							case "ACTION":
							case "SHOP":
							case "PLANT":
							case "RECYCLE":
							case "CAMERA":
							case "CAM1":
							case "CAM2":
							case "CAM3":
							case "CAM4":
							case "CAMLEFT":
							case "CAMRIGHT":
							case "CAMUP":
							case "CAMDOWN":
							case "CAMIN":
							case "CAMOUT":
								ApplyStageCommand(currentGameSection, text);
								flag = true;
								break;
							}
							break;
						}
						goto case "LEFT";
					case "LEFT":
					case "RIGHT":
					case "UP":
						QueueCompatMovement(text);
						flag = true;
						break;
					}
				}
				string[] array2 = new string[arrayList.Count];
				arrayList.CopyTo(array2);
				WriteCommandLines(array2);
			}
			if (!flag)
			{
				ProcessStageKeyboard(currentGameSection);
			}
		}
		catch (Exception ex)
		{
			Mark("stage-command-failed-" + DescribeException(ex));
		}
	}

	private static void EnsureMenuBackgroundScene()
	{
		try
		{
			object currentGameSection = GetCurrentGameSection();
			string text = ((currentGameSection == null) ? "" : currentGameSection.GetType().FullName);
			if (text.IndexOf(".Sections.StageSection", StringComparison.Ordinal) >= 0)
			{
				menuBackgroundSection = null;
				return;
			}
			bool flag = text.IndexOf(".Sections.MainMenuSection", StringComparison.Ordinal) >= 0;
			bool flag2 = text.IndexOf(".Sections.AvatarFarmSplashSection", StringComparison.Ordinal) >= 0;
			bool flag3 = text.IndexOf(".Sections.HowToPlaySection", StringComparison.Ordinal) >= 0;
			if ((flag || flag2 || flag3) && menuBackgroundSection != currentGameSection)
			{
				Type type = Type.GetType("AvatarFarmOnline.Scenes.BGScene, AvatarFarmOnline");
				MethodInfo methodInfo = ((type == null) ? null : type.GetMethod("get_Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));
				object obj = ((methodInfo == null) ? null : methodInfo.Invoke(null, null));
				if (obj == null)
				{
					Mark("menu-bg-no-scene");
					return;
				}
				EnsureMenuMainRenderSource(currentGameSection, obj);
				menuBackgroundSection = currentGameSection;
				Mark(flag2 ? "splash-bg-real-scene" : (flag3 ? "how-to-play-bg-real-scene" : "menu-bg-real-scene"));
			}
		}
		catch (Exception ex)
		{
			Mark("menu-bg-failed-" + DescribeException(ex));
		}
	}

	private static void ForceMenuBackgroundCamera(object bgScene)
	{
		try
		{
			object obj = InvokeProperty(bgScene, "Camera");
			object obj2 = InvokeProperty(obj, "Transform");
			PropertyInfo propertyInfo = obj2?.GetType().GetProperty("Matrix", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			FieldInfo fieldInfo = ((obj == null) ? null : GetFieldInfo(obj.GetType(), "projection"));
			MethodInfo methodInfo = obj?.GetType().GetMethod("UpdateMatrix", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (obj == null || obj2 == null || propertyInfo == null || fieldInfo == null || methodInfo == null)
			{
				Mark("menu-bg-camera-missing");
				return;
			}
			float num = 0f;
			try
			{
				num = (float)(DateTime.UtcNow.Ticks % 10000000) / 10000000f;
			}
			catch
			{
			}
			Vector3 cameraPosition = new Vector3((float)Math.Sin((double)num * Math.PI * 2.0) * 2f, 1.2f, (float)Math.Cos((double)num * Math.PI * 2.0) * 2f);
			Matrix matrix = Matrix.Invert(Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up));
			propertyInfo.SetValue(obj2, matrix, null);
			fieldInfo.SetValue(obj, Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, GetEngineAspectRatio(), 0.1f, 10000f));
			methodInfo.Invoke(obj, null);
			MarkFloats("menu-bg-camera-pos", cameraPosition.X, cameraPosition.Y, cameraPosition.Z, 0f);
		}
		catch (Exception ex)
		{
			Mark("menu-bg-camera-failed-" + DescribeException(ex));
		}
	}

	private static void EnsureMenuMainRenderSource(object section, object bgScene)
	{
		try
		{
			object field = GetField(section, "mainRenderPass");
			if (field != null)
			{
				MethodInfo method = field.GetType().GetMethod("insertSource", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[2]
				{
					typeof(int),
					bgScene.GetType().BaseType
				}, null);
				if (method == null)
				{
					method = field.GetType().GetMethod("insertSource", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[2]
					{
						typeof(int),
						bgScene.GetType()
					}, null);
				}
				if (method != null)
				{
					method.Invoke(field, new object[2] { 0, bgScene });
					Mark("menu-bg-mainrender-inserted");
				}
				else
				{
					MethodInfo methodInfo = FindSingleArgMethod(field.GetType(), "addSource");
					if (methodInfo != null)
					{
						methodInfo.Invoke(field, new object[1] { bgScene });
						Mark("menu-bg-mainrender-added");
					}
				}
			}
			object field2 = GetField(section, "fadeMesh");
			if (field2 != null)
			{
				MethodInfo method2 = field2.GetType().GetMethod("set_Alpha", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (method2 != null)
				{
					method2.Invoke(field2, new object[1] { 0f });
					Mark("menu-bg-fade-alpha-zero");
				}
			}
		}
		catch (Exception ex)
		{
			Mark("menu-bg-mainrender-failed-" + DescribeException(ex));
		}
	}

	private static void EnsureMenuFlatBackground(object section)
	{
		try
		{
			if (CompatMenuBackground != null || section == null)
			{
				return;
			}
			object obj = FindFirstSceneByName(section, "Quasar.GUI.LayoutScene");
			if (obj == null)
			{
				Mark("menu-flat-bg-no-layout-scene");
				return;
			}
			object textureByName = GetTextureByName("GroundTiles");
			object obj2 = CreateCompatProxyTexturedPlane(textureByName, new Vector2(1500f, 900f), new Vector3(450f, -40f, -10f), 0f, 0.92f) ?? CreateCompatProxyPlane(new Vector2(1500f, 900f), new Vector4(0.42f, 0.66f, 0.43f, 1f), new Vector3(450f, -40f, -10f), 0f);
			if (obj2 == null)
			{
				Mark("menu-flat-bg-create-failed");
				return;
			}
			SetFieldValue(obj2, "Name", "CompatMenuFlatBackground");
			object obj3 = InvokeProperty(obj2, "Transform");
			if (obj3 != null)
			{
				SetPropertyValue(obj3, "Translation", new Vector3(450f, -40f, -10f));
				TrySetProperty(obj3, "Scale", new Vector3(1f, 1f, 1f));
			}
			MethodInfo method = obj.GetType().GetMethod("Insert", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[2]
			{
				typeof(int),
				obj2.GetType().BaseType
			}, null);
			if (method == null)
			{
				method = obj.GetType().GetMethod("Insert", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[2]
				{
					typeof(int),
					obj2.GetType()
				}, null);
			}
			if (method != null)
			{
				method.Invoke(obj, new object[2] { 0, obj2 });
				CompatMenuBackground = obj2;
				Mark("menu-flat-bg-inserted");
				return;
			}
			MethodInfo method2 = obj.GetType().GetMethod("Add", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[1] { obj2.GetType().BaseType }, null);
			if (method2 == null)
			{
				method2 = obj.GetType().GetMethod("Add", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[1] { obj2.GetType() }, null);
			}
			if (method2 != null)
			{
				method2.Invoke(obj, new object[1] { obj2 });
				CompatMenuBackground = obj2;
				Mark("menu-flat-bg-added");
			}
			else
			{
				Mark("menu-flat-bg-no-add");
			}
		}
		catch (Exception ex)
		{
			Mark("menu-flat-bg-failed-" + DescribeException(ex));
		}
	}

	private static object FindFirstSceneByName(object section, string fullName)
	{
		try
		{
			if (!(GetField(section, "scenes") is IList list))
			{
				return null;
			}
			for (int i = 0; i < list.Count; i++)
			{
				object obj = list[i];
				if (obj != null && string.Equals(obj.GetType().FullName, fullName, StringComparison.Ordinal))
				{
					return obj;
				}
			}
		}
		catch
		{
		}
		return null;
	}

	private static void EnsureCompatMenuPreview()
	{
		try
		{
			if (CompatMenuPreview != null || registeredLayout == null)
			{
				return;
			}
			EnsureCompatPlayerModelBootstrap();
			object obj = CreateCompatStageMesh(ResolveCompatPlayerModelAsset());
			if (obj == null)
			{
				return;
			}
			Type type = Type.GetType("Quasar.RenderItem, Quasar");
			object obj2 = ((type == null) ? null : Activator.CreateInstance(type, obj));
			if (obj2 == null)
			{
				return;
			}
			object obj3 = InvokeProperty(obj2, "Transform");
			if (obj3 != null)
			{
				SetPropertyValue(obj3, "Translation", new Vector3(260f, 40f, 0f));
				SetPropertyValue(obj3, "Rotation", Quaternion.CreateFromAxisAngle(Vector3.Up, -0.35f));
				try
				{
					SetPropertyValue(obj3, "Scale", new Vector3(220f, 220f, 220f));
				}
				catch
				{
				}
			}
			TrySetProperty(obj2, "Visible", true);
			TrySetProperty(obj2, "Active", true);
			TrySetProperty(obj2, "Alpha", 1f);
			AddCompatProxyChild(FindAddChildMethod(registeredLayout), registeredLayout, obj2);
			CompatMenuPreview = obj2;
			Mark("compat-menu-preview-installed");
		}
		catch (Exception ex)
		{
			Mark("compat-menu-preview-failed-" + DescribeException(ex));
		}
	}

	private static void SyncStageScenes(object section)
	{
		try
		{
			object field = GetField(section, "stageScene");
			object camera = ((field == null) ? null : InvokeProperty(field, "GameCamera"));
			SetSceneCamera(field, camera);
			EnsureFixedViewportAndGui(null);
		}
		catch (Exception ex)
		{
			Mark("sync-scenes-failed-" + DescribeException(ex));
		}
	}

	private static void EnsureFixedViewportAndGui(GraphicsDevice device)
	{
		try
		{
			bool flag = true;
			if (device != null)
			{
				int num = ((device.PresentationParameters == null) ? device.Viewport.Width : device.PresentationParameters.BackBufferWidth);
				int num2 = ((device.PresentationParameters == null) ? device.Viewport.Height : device.PresentationParameters.BackBufferHeight);
				RenderTargetBinding[] renderTargets = device.GetRenderTargets();
				if (renderTargets != null && renderTargets.Length != 0 && renderTargets[0].RenderTarget is Texture2D texture2D)
				{
					num = texture2D.Width;
					num2 = texture2D.Height;
					flag = false;
				}
				if (num <= 0)
				{
					num = 1280;
				}
				if (num2 <= 0)
				{
					num2 = 720;
				}
				device.Viewport = new Viewport(0, 0, num, num2);
				device.ScissorRectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, num, num2);
			}
			Type type = Type.GetType("Quasar.Global.Engine, Quasar");
			if (type == null)
			{
				return;
			}
			MethodInfo method = type.GetMethod("SetGUIWidth", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (method != null)
			{
				method.Invoke(null, new object[1] { 1280 });
			}
			object obj = null;
			PropertyInfo property = type.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (property != null)
			{
				obj = property.GetValue(null, null);
			}
			if (obj == null)
			{
				FieldInfo fieldInfo = GetFieldInfo(type, "instance");
				if (fieldInfo != null)
				{
					obj = fieldInfo.GetValue(null);
				}
			}
			MethodInfo methodInfo = ((obj == null) ? null : type.GetMethod("UpdateViewport", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
			if (methodInfo != null && ((device == null) | flag))
			{
				methodInfo.Invoke(obj, null);
			}
			if (Interlocked.Exchange(ref fixedGuiViewportApplied, 1) == 0)
			{
				Mark("fixed-gui-viewport-1280x720");
			}
		}
		catch (Exception ex)
		{
			Mark("fixed-gui-viewport-failed-" + DescribeException(ex));
		}
	}

	private static void NormalizeFarmTileMesh(object section)
	{
		try
		{
			object field = GetField(section, "stage");
			object field2 = GetField(section, "stageScene");
			object farmItem = ((field2 == null) ? null : GetField(field2, "farmItem"));
			object obj = ((field == null) ? null : InvokeProperty(field, "FarmData"));
			object obj2 = ((obj == null) ? null : InvokeProperty(obj, "FarmSize"));
			object obj3 = ((obj == null) ? null : InvokeProperty(obj, "Tiles"));
			object farmRenderMesh = GetFarmRenderMesh(farmItem);
			if (farmRenderMesh == null || obj2 == null || obj3 == null)
			{
				return;
			}
			MethodInfo method = farmRenderMesh.GetType().GetMethod("UpdateTileData", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			FieldInfo fieldInfo = GetFieldInfo(obj2.GetType(), "X");
			FieldInfo fieldInfo2 = GetFieldInfo(obj2.GetType(), "Y");
			MethodInfo method2 = obj3.GetType().GetMethod("Get", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[2]
			{
				typeof(int),
				typeof(int)
			}, null);
			int[] tileIndicesArray = GetTileIndicesArray(farmRenderMesh);
			if (tileIndicesArray == null || method == null || fieldInfo == null || fieldInfo2 == null || method2 == null)
			{
				return;
			}
			int num = Convert.ToInt32(fieldInfo.GetValue(obj2));
			int num2 = Convert.ToInt32(fieldInfo2.GetValue(obj2));
			bool flag = false;
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num; j++)
				{
					object obj4 = method2.Invoke(obj3, new object[2] { j, i });
					int num3 = 0;
					if (obj4 != null && !Convert.ToBoolean(InvokeProperty(obj4, "IsEmpty")))
					{
						num3 = 2;
					}
					int num4 = i * num + j;
					if (num4 < tileIndicesArray.Length && tileIndicesArray[num4] != num3)
					{
						tileIndicesArray[num4] = num3;
						flag = true;
					}
				}
			}
			if (flag)
			{
				method.Invoke(farmRenderMesh, null);
			}
		}
		catch (Exception ex)
		{
			Mark("normalize-farmtilemesh-failed-" + DescribeException(ex));
		}
	}

	private static void SyncPlayerItems(object section)
	{
		try
		{
			object field = GetField(section, "stageScene");
			object obj = ((field == null) ? null : InvokeProperty(field, "GameCamera"));
			object farmItem = ((field == null) ? null : GetField(field, "farmItem"));
			object farmRenderMesh = GetFarmRenderMesh(farmItem);
			bool flag = farmRenderMesh != null && string.Equals(farmRenderMesh.GetType().FullName, "Quasar.Meshes.TileMesh", StringComparison.Ordinal);
			object obj2 = ((field == null) ? null : GetField(field, "playerItems"));
			if (!(obj2 is IList list))
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				object obj3 = list[i];
				if (obj3 == null)
				{
					continue;
				}
				object obj4 = InvokeProperty(obj3, "Player");
				object obj5 = ((obj4 == null) ? null : InvokeProperty(obj4, "WorldPosition"));
				object obj6 = InvokeProperty(obj3, "Transform");
				if (obj5 == null || obj6 == null)
				{
					continue;
				}
				float x = Convert.ToSingle(GetFieldInfo(obj5.GetType(), "X").GetValue(obj5));
				float y = Convert.ToSingle(GetFieldInfo(obj5.GetType(), "Y").GetValue(obj5));
				float num = Convert.ToSingle(GetFieldInfo(obj5.GetType(), "Z").GetValue(obj5));
				if (flag)
				{
					num = 0f - num;
				}
				PropertyInfo property = obj6.GetType().GetProperty("Translation", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				PropertyInfo property2 = obj6.GetType().GetProperty("Rotation", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (property != null && property.CanWrite)
				{
					property.SetValue(obj6, new Vector3(x, y, num), null);
				}
				object obj7 = ((obj4 == null) ? null : InvokeProperty(obj4, "Rotation"));
				if (property2 != null && property2.CanWrite && obj7 != null)
				{
					float angle = Convert.ToSingle(obj7);
					property2.SetValue(obj6, Quaternion.CreateFromAxisAngle(Vector3.Up, angle), null);
				}
				object field2 = GetField(obj3, "avatar");
				object field3 = GetField(obj3, "loadedAvatar");
				if (field2 == null && field3 == null)
				{
					RepairOfficialAvatarLoad(obj3);
					field2 = GetField(obj3, "avatar");
					field3 = GetField(obj3, "loadedAvatar");
				}
				object field4 = GetField(obj3, "meshItem");
				if (PreferCompatPlayerStageMesh)
				{
					EnsureCompatPlayerStageMesh(obj3, field4);
					RefreshCompatPlayerStageMesh(obj3, field4);
				}
				if (UseCompatPlayerStageActor)
				{
					SyncCompatPlayerStageActor(section, obj3, new Vector3(x, y, num), obj7);
				}
				object obj8 = ((field4 == null) ? null : InvokeProperty(field4, "Transform"));
				if (obj8 != null)
				{
					PropertyInfo property3 = obj8.GetType().GetProperty("Translation", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					PropertyInfo property4 = obj8.GetType().GetProperty("Rotation", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					PropertyInfo property5 = obj8.GetType().GetProperty("Scale", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (property3 != null && property3.CanWrite)
					{
						property3.SetValue(obj8, Vector3.Zero, null);
					}
					if (property4 != null && property4.CanWrite)
					{
						property4.SetValue(obj8, Quaternion.CreateFromAxisAngle(Vector3.Up, -(float)Math.PI / 2f), null);
					}
					if (property5 != null && property5.CanWrite)
					{
						property5.SetValue(obj8, Vector3.One, null);
					}
				}
				object obj9 = ((field4 == null) ? null : InvokeProperty(field4, "Meshes"));
				int num2 = 0;
				if (obj9 is ICollection collection)
				{
					num2 = collection.Count;
				}
				Mark("playeritem-state-avatar-" + ((field2 != null) ? "1" : "0") + "-loaded-" + ((field3 != null) ? "1" : "0") + "-meshitem-" + ((field4 != null) ? "1" : "0") + "-meshes-" + num2.ToString(CultureInfo.InvariantCulture));
				if (obj != null)
				{
					LogProjectedPoint(obj, "player-world", new Vector3(x, y, num));
					if (UseCompatPlayerOverlay)
					{
						SyncCompatPlayerOverlay(section, obj3, obj, new Vector3(x, y, num));
					}
				}
				if (!PreferCompatPlayerStageMesh && (ForceCompatPlayerProxy || field2 == null || field3 == null || num2 == 0))
				{
					EnsureCompatPlayerProxy(obj3);
				}
			}
		}
		catch (Exception ex)
		{
			Mark("sync-playeritems-failed-" + DescribeException(ex));
		}
	}

	private static void RepairOfficialAvatarLoad(object playerItem)
	{
		if (playerItem == null)
		{
			return;
		}
		int hashCode = RuntimeHelpers.GetHashCode(playerItem);
		if (!OfficialAvatarLoadRepairs.TryAdd(hashCode, 0))
		{
			return;
		}
		try
		{
			MethodInfo method = playerItem.GetType().GetMethod("LoadAvatar", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			MethodInfo method2 = playerItem.GetType().GetMethod("OnLoadAvatarFinished", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method == null || method2 == null)
			{
				throw new MissingMethodException(playerItem.GetType().FullName, (method == null) ? "LoadAvatar" : "OnLoadAvatarFinished");
			}
			method.Invoke(playerItem, new object[1]);
			object field = GetField(playerItem, "loadedAvatar");
			if (field == null)
			{
				throw new InvalidOperationException("The original avatar loader returned no avatar.");
			}
			method2.Invoke(playerItem, new object[1]);
			Mark("official-avatar-load-repaired");
		}
		catch (Exception ex)
		{
			OfficialAvatarLoadRepairs.TryRemove(hashCode, out var _);
			Mark("official-avatar-load-repair-failed-" + DescribeException(ex));
			try
			{
				EnsureRuntimeDirectory();
				File.AppendAllText(ExceptionLogPath, DateTime.UtcNow.ToString("O") + " official avatar load repair" + Environment.NewLine + ex?.ToString() + Environment.NewLine + Environment.NewLine);
			}
			catch
			{
			}
		}
	}

	private static void SyncCompatPlayerStageActor(object section, object playerItem, Vector3 worldPoint, object rotationValue)
	{
		try
		{
			if (section == null || playerItem == null)
			{
				return;
			}
			int hashCode = RuntimeHelpers.GetHashCode(playerItem);
			if (!CompatPlayerStageActors.TryGetValue(hashCode, out var value) || value == null)
			{
				value = CreateCompatPlayerStageActor(section);
				if (value == null)
				{
					return;
				}
				CompatPlayerStageActors[hashCode] = value;
				Mark("compat-player-stage-actor-added");
			}
			object obj = InvokeProperty(value, "Transform");
			if (obj != null)
			{
				SetPropertyValue(obj, "Translation", worldPoint + new Vector3(0f, 1.6f, 0f));
				if (rotationValue != null)
				{
					float angle = Convert.ToSingle(rotationValue);
					SetPropertyValue(obj, "Rotation", Quaternion.CreateFromAxisAngle(Vector3.Up, angle));
				}
				try
				{
					SetPropertyValue(obj, "Scale", new Vector3(1.8f, 1.8f, 1.8f));
				}
				catch
				{
				}
				TrySetProperty(value, "Visible", true);
				TrySetProperty(value, "Active", true);
				TrySetProperty(value, "Alpha", 1f);
			}
		}
		catch (Exception ex)
		{
			Mark("compat-player-stage-actor-failed-" + DescribeException(ex));
		}
	}

	private static object CreateCompatPlayerStageActor(object section)
	{
		try
		{
			EnsureCompatPlayerModelBootstrap();
			string a = ResolveCompatPlayerModelAsset();
			object obj = null;
			obj = CreateCompatProxyWorldRect(new Vector2(1.8f, 3.2f), new Vector3(0f, 0f, 0f), 0f, 0.98f, GetShaderByName("SimpleMultiply"));
			if (obj == null)
			{
				obj = CreateCompatCubeStageActor();
			}
			if (obj == null && !string.Equals(a, CompatPlayerModelDefaultAsset, StringComparison.OrdinalIgnoreCase))
			{
				obj = CreateCompatModelRenderItem(CompatPlayerModelDefaultAsset);
			}
			if (obj == null)
			{
				return null;
			}
			object obj2 = InvokeProperty(obj, "Transform");
			if (obj2 != null)
			{
				try
				{
					SetPropertyValue(obj2, "Scale", new Vector3(1.8f, 1.8f, 1.8f));
				}
				catch
				{
				}
			}
			object field = GetField(section, "stageScene");
			MethodInfo methodInfo = ((field == null) ? null : FindSingleArgMethod(field.GetType(), "Add"));
			if (methodInfo == null)
			{
				Mark("compat-player-stage-actor-no-add");
				return null;
			}
			methodInfo.Invoke(field, new object[1] { obj });
			Mark("compat-player-stage-elements-count-" + (GetSceneElementList(field)?.Count ?? (-1)));
			DumpSceneElements(field, "compat-stage-scene", 1);
			return obj;
		}
		catch (Exception ex)
		{
			Mark("compat-player-stage-actor-create-failed-" + DescribeException(ex));
			return null;
		}
	}

	private static object CreateCompatModelRenderItem(string assetName)
	{
		if (string.IsNullOrEmpty(assetName))
		{
			return null;
		}
		try
		{
			Type type = Type.GetType("Quasar.Xml.ModelLoader, Quasar");
			MethodInfo methodInfo = ((type == null) ? null : type.GetMethod("LoadModelDefinition", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[1] { typeof(string) }, null));
			object obj = ((methodInfo == null) ? null : methodInfo.Invoke(null, new object[1] { assetName }));
			if (obj == null)
			{
				return null;
			}
			TrySetProperty(obj, "Visible", true);
			TrySetProperty(obj, "Active", true);
			TrySetProperty(obj, "Alpha", 1f);
			Mark("compat-player-modelitem-loaded-" + assetName.Replace('/', '_'));
			return obj;
		}
		catch (Exception ex)
		{
			Mark("compat-player-modelitem-failed-" + assetName.Replace('/', '_') + "-" + DescribeException(ex));
			return null;
		}
	}

	private static void SyncCompatPlayerOverlay(object section, object playerItem, object camera, Vector3 worldPoint)
	{
		try
		{
			if (section == null || playerItem == null || camera == null)
			{
				return;
			}
			int hashCode = RuntimeHelpers.GetHashCode(playerItem);
			if (!CompatPlayerOverlays.TryGetValue(hashCode, out var value) || value == null)
			{
				value = CreateCompatOverlayRectangle();
				if (value == null)
				{
					return;
				}
				object field = GetField(section, "HUDScene");
				if (field == null)
				{
					field = GetField(section, "stageScene");
				}
				MethodInfo methodInfo = field?.GetType().GetMethod("Add", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[1] { value.GetType().BaseType }, null);
				if (methodInfo == null && field != null)
				{
					MethodInfo[] methods = field.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					foreach (MethodInfo methodInfo2 in methods)
					{
						if (string.Equals(methodInfo2.Name, "Add", StringComparison.Ordinal))
						{
							ParameterInfo[] parameters = methodInfo2.GetParameters();
							if (parameters != null && parameters.Length == 1)
							{
								methodInfo = methodInfo2;
								break;
							}
						}
					}
				}
				if (methodInfo == null)
				{
					Mark("compat-player-overlay-no-scene-add");
					return;
				}
				methodInfo.Invoke(field, new object[1] { value });
				CompatPlayerOverlays[hashCode] = value;
				Mark("compat-player-overlay-added");
			}
			MethodInfo method = camera.GetType().GetMethod("ProjectPoint", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[1] { typeof(Vector3) }, null);
			object obj = ((method == null) ? null : method.Invoke(camera, new object[1] { worldPoint + new Vector3(0f, 1.15f, 0f) }));
			if (obj != null && TryReadVector4(obj, out var x, out var y, out var _, out var w) && !(Math.Abs(w) <= 0.0001f))
			{
				object obj2 = InvokeProperty(value, "Transform");
				if (obj2 != null)
				{
					float num = ReadOverlayScale();
					x /= w;
					y /= w;
					SetPropertyValue(obj2, "Translation", new Vector3(x * 640f + ReadOverlayX(), y * -360f + ReadOverlayZ(), 0f));
					SetPropertyValue(obj2, "Rotation", Quaternion.Identity);
					TrySetProperty(obj2, "Scale", new Vector3(num, num, 1f));
				}
			}
		}
		catch (Exception ex)
		{
			Mark("compat-player-overlay-failed-" + DescribeException(ex));
		}
	}

	private static void EnsureCompatPlayerStageMesh(object playerItem, object meshItem)
	{
		try
		{
			if (playerItem == null || meshItem == null)
			{
				return;
			}
			EnsureCompatPlayerModelBootstrap();
			int hashCode = RuntimeHelpers.GetHashCode(playerItem);
			if (CompatPlayerStageMeshes.TryGetValue(hashCode, out var value) && value != null)
			{
				return;
			}
			string text = ResolveCompatPlayerModelAsset();
			object obj = CreateCompatStageMesh(text);
			if (obj == null && !string.Equals(text, CompatPlayerModelDefaultAsset, StringComparison.OrdinalIgnoreCase))
			{
				obj = CreateCompatStageMesh(CompatPlayerModelDefaultAsset);
			}
			if (obj != null)
			{
				if (!AttachCompatMeshToRenderItem(meshItem, obj))
				{
					Mark("compat-player-stage-mesh-attach-failed");
					return;
				}
				CompatPlayerStageMeshes[hashCode] = obj;
				Mark("compat-player-stage-mesh-installed-" + text.Replace('/', '_'));
			}
		}
		catch (Exception ex)
		{
			Mark("compat-player-stage-mesh-failed-" + DescribeException(ex));
		}
	}

	private static void RefreshCompatPlayerStageMesh(object playerItem, object meshItem)
	{
		try
		{
			if (playerItem != null && meshItem != null && CompatPlayerStageMeshes.TryGetValue(RuntimeHelpers.GetHashCode(playerItem), out var value) && value != null)
			{
				object obj = InvokeProperty(meshItem, "Meshes");
				if (obj is IList list && (list.Count != 1 || list[0] != value))
				{
					list.Clear();
					list.Add(value);
				}
			}
		}
		catch (Exception ex)
		{
			Mark("compat-player-stage-mesh-refresh-failed-" + DescribeException(ex));
		}
	}

	private static void EnsureCompatPlayerModelBootstrap()
	{
		try
		{
			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string text = Path.Combine(baseDirectory, "Content", "Models", CompatPlayerModelSampleDirName);
			string text2 = Path.Combine(text, CompatPlayerModelSampleFileName);
			string text3 = Path.Combine(baseDirectory, "Content", "Models", "Animals", "PigModel.xnb");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			if (!File.Exists(text2) && File.Exists(text3))
			{
				File.Copy(text3, text2, overwrite: true);
			}
			if (!File.Exists(CompatPlayerModelConfigPath))
			{
				File.WriteAllText(CompatPlayerModelConfigPath, "Custom/PlayerAvatar" + Environment.NewLine);
			}
			if (!File.Exists(CompatPlayerModelNotesPath))
			{
				File.WriteAllText(CompatPlayerModelNotesPath, "Player model asset selector for Avatar Farm Online" + Environment.NewLine + "Default suggested value:" + Environment.NewLine + "Custom/PlayerAvatar" + Environment.NewLine + Environment.NewLine + "Examples of built-in assets:" + Environment.NewLine + "Animals/PigModel" + Environment.NewLine + "Animals/CowModel" + Environment.NewLine + "Tools/TractorModel" + Environment.NewLine + Environment.NewLine + "Drop-in replacement:" + Environment.NewLine + "Place an .xnb model at Content\\Models\\Custom\\PlayerAvatar.xnb and keep player-model.txt set to Custom/PlayerAvatar." + Environment.NewLine);
			}
		}
		catch (Exception ex)
		{
			Mark("compat-player-model-bootstrap-failed-" + DescribeException(ex));
		}
	}

	private static string ResolveCompatPlayerModelAsset()
	{
		try
		{
			if (File.Exists(CompatPlayerModelConfigPath))
			{
				string text = File.ReadAllText(CompatPlayerModelConfigPath).Trim().Replace('\\', '/');
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
		}
		catch (Exception ex)
		{
			Mark("compat-player-model-config-failed-" + DescribeException(ex));
		}
		return CompatPlayerModelDefaultAsset;
	}

	private static object CreateCompatStageMesh(string assetName)
	{
		if (string.IsNullOrEmpty(assetName))
		{
			return null;
		}
		try
		{
			Type type = Type.GetType("Quasar.Meshes.XMesh, Quasar");
			if (type == null)
			{
				return null;
			}
			object obj = Activator.CreateInstance(type, assetName);
			if (obj == null)
			{
				return null;
			}
			TrySetProperty(obj, "Alpha", 1f);
			TrySetProperty(obj, "Ambient", new Vector3(1f, 1f, 1f));
			TrySetProperty(obj, "Diffuse", new Vector3(1f, 1f, 1f));
			object shaderByName = GetShaderByName("SimpleMultiply");
			if (shaderByName != null)
			{
				TrySetProperty(obj, "Shader", shaderByName);
			}
			try
			{
				Type type2 = Type.GetType("Quasar.Mesh, Quasar");
				FieldInfo fieldInfo = ((type2 == null) ? null : type2.GetField("boundingSphere", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
				if (fieldInfo != null)
				{
					fieldInfo.SetValue(obj, new BoundingSphere(Vector3.Zero, 10000f));
				}
			}
			catch
			{
			}
			return obj;
		}
		catch (Exception ex)
		{
			Mark("compat-player-stage-mesh-create-failed-" + assetName.Replace('/', '_') + "-" + DescribeException(ex));
			return null;
		}
	}

	private static object CreateCompatCubeStageActor()
	{
		try
		{
			Type type = Type.GetType("Quasar.Meshes.Cube, Quasar");
			Type type2 = Type.GetType("Quasar.RenderItem, Quasar");
			if (type == null || type2 == null)
			{
				return null;
			}
			object obj = Activator.CreateInstance(type, new object[0]);
			if (obj == null)
			{
				return null;
			}
			TrySetProperty(obj, "Alpha", 1f);
			object shaderByName = GetShaderByName("Simple");
			if (shaderByName == null)
			{
				shaderByName = GetShaderByName("BaseNoNormal");
			}
			if (shaderByName != null)
			{
				TrySetProperty(obj, "Shader", shaderByName);
			}
			object obj2 = InvokeProperty(obj, "FirstMaterial");
			if (obj2 != null)
			{
				TrySetProperty(obj2, "Ambient", new Vector3(1f, 0.08f, 0.08f));
				TrySetProperty(obj2, "Diffuse", new Vector3(1f, 0.08f, 0.08f));
				TrySetProperty(obj2, "Specular", new Vector3(0f, 0f, 0f));
			}
			object obj3 = Activator.CreateInstance(type2, obj);
			if (obj3 == null)
			{
				return null;
			}
			SetFieldValue(obj3, "Name", "CompatPlayerActor");
			TrySetProperty(obj3, "Visible", true);
			TrySetProperty(obj3, "Active", true);
			TrySetProperty(obj3, "Alpha", 1f);
			return obj3;
		}
		catch (Exception ex)
		{
			Mark("compat-player-cube-actor-failed-" + DescribeException(ex));
			return null;
		}
	}

	private static bool AttachCompatMeshToRenderItem(object renderItem, object mesh)
	{
		if (renderItem == null || mesh == null)
		{
			return false;
		}
		try
		{
			object obj = InvokeProperty(renderItem, "Meshes");
			if (obj is IList list)
			{
				list.Clear();
				list.Add(mesh);
			}
			else
			{
				MethodInfo method = renderItem.GetType().GetMethod("addMesh", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (method == null)
				{
					return false;
				}
				method.Invoke(renderItem, new object[1] { mesh });
			}
			object obj2 = InvokeProperty(renderItem, "Transform");
			if (obj2 != null)
			{
				SetPropertyValue(obj2, "Translation", new Vector3(0f, 0f, 0f));
				SetPropertyValue(obj2, "Rotation", Quaternion.CreateFromAxisAngle(Vector3.Up, -(float)Math.PI / 2f));
				try
				{
					SetPropertyValue(obj2, "Scale", new Vector3(1.35f, 1.35f, 1.35f));
				}
				catch
				{
				}
			}
			TrySetProperty(renderItem, "Visible", true);
			TrySetProperty(renderItem, "Active", true);
			TrySetProperty(renderItem, "Alpha", 1f);
			TrySetProperty(renderItem, "Ambient", new Vector3(1f, 1f, 1f));
			TrySetProperty(renderItem, "Diffuse", new Vector3(1f, 1f, 1f));
			return true;
		}
		catch (Exception ex)
		{
			Mark("compat-player-stage-mesh-attach-ex-" + DescribeException(ex));
			return false;
		}
	}

	private static bool TryReadVector4(object value, out float x, out float y, out float z, out float w)
	{
		x = (y = (z = (w = 0f)));
		if (value == null)
		{
			return false;
		}
		FieldInfo fieldInfo = GetFieldInfo(value.GetType(), "X");
		FieldInfo fieldInfo2 = GetFieldInfo(value.GetType(), "Y");
		FieldInfo fieldInfo3 = GetFieldInfo(value.GetType(), "Z");
		FieldInfo fieldInfo4 = GetFieldInfo(value.GetType(), "W");
		if (fieldInfo == null || fieldInfo2 == null || fieldInfo3 == null || fieldInfo4 == null)
		{
			return false;
		}
		x = Convert.ToSingle(fieldInfo.GetValue(value));
		y = Convert.ToSingle(fieldInfo2.GetValue(value));
		z = Convert.ToSingle(fieldInfo3.GetValue(value));
		w = Convert.ToSingle(fieldInfo4.GetValue(value));
		return true;
	}

	private static object CreateCompatOverlayRectangle()
	{
		object compatProxyTexture = GetCompatProxyTexture();
		object obj = CreateCompatProxyTexturedPlane(compatProxyTexture, new Vector2(110f, 220f), Vector3.Zero, 0f, 1f);
		if (obj == null)
		{
			obj = CreateCompatProxyPlane(new Vector2(110f, 220f), new Vector4(1f, 0.1f, 0.1f, 1f), Vector3.Zero, 0f);
		}
		if (obj != null)
		{
			SetFieldValue(obj, "Name", "CompatPlayerOverlay");
			TrySetProperty(obj, "Visible", true);
			TrySetProperty(obj, "Active", true);
			SetPropertyValue(obj, "Alpha", 1f);
		}
		return obj;
	}

	private static float ReadOverlayX()
	{
		return ReadFloatFile(OverlayXPath, 480f);
	}

	private static float ReadOverlayZ()
	{
		return ReadFloatFile(OverlayZPath, 0f);
	}

	private static float ReadOverlayScale()
	{
		float num = ReadFloatFile(OverlayScalePath, 0.75f);
		if (num < 0.1f)
		{
			return 0.1f;
		}
		if (num > 4f)
		{
			return 4f;
		}
		return num;
	}

	private static float ReadFloatFile(string path, float fallback)
	{
		try
		{
			if (File.Exists(path))
			{
				string s = File.ReadAllText(path).Trim();
				if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
				{
					return result;
				}
			}
		}
		catch
		{
		}
		return fallback;
	}

	private static void SyncCompatPlayerProxyPlacement(object playerItem, object camera, Vector3 worldPoint)
	{
		try
		{
			if (playerItem == null || camera == null)
			{
				return;
			}
			int hashCode = RuntimeHelpers.GetHashCode(playerItem);
			if (!CompatPlayerBodies.TryGetValue(hashCode, out var value) || value == null)
			{
				return;
			}
			object obj = null;
			MethodInfo method = camera.GetType().GetMethod("ProjectPoint", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[1] { typeof(Vector3) }, null);
			if (method != null)
			{
				obj = method.Invoke(camera, new object[1] { worldPoint + new Vector3(0f, 1.15f, 0f) });
			}
			if (obj == null || !TryReadVector4(obj, out var x, out var y, out var _, out var w) || Math.Abs(w) <= 0.0001f)
			{
				return;
			}
			x /= w;
			y /= w;
			object obj2 = InvokeProperty(value, "Transform");
			if (obj2 != null)
			{
				float x2 = x * 640f;
				float y2 = y * -360f;
				SetPropertyValue(obj2, "Translation", new Vector3(x2, y2, 0f));
				SetPropertyValue(obj2, "Rotation", Quaternion.Identity);
				try
				{
					SetPropertyValue(obj2, "Scale", new Vector3(120f, 220f, 1f));
				}
				catch
				{
				}
				SetPropertyValue(value, "Alpha", 0.98f);
			}
		}
		catch (Exception ex)
		{
			Mark("compat-player-proxy-place-failed-" + DescribeException(ex));
		}
	}

	private static void DrawCompatGeneratedAvatarForCurrentSection(GraphicsDevice device)
	{
		try
		{
			object currentGameSection = GetCurrentGameSection();
			if (currentGameSection == null || currentGameSection.GetType().FullName.IndexOf(".Sections.StageSection", StringComparison.Ordinal) < 0)
			{
				return;
			}
			object field = GetField(currentGameSection, "stageScene");
			object obj = ((field == null) ? null : InvokeProperty(field, "GameCamera"));
			object field2 = GetField(currentGameSection, "stage");
			object obj2 = ((field2 == null) ? null : InvokeProperty(field2, "LocalPlayer"));
			object obj3 = ((field2 == null) ? null : InvokeProperty(field2, "FarmData"));
			object obj4 = ((obj2 == null) ? null : InvokeProperty(obj2, "WorldPosition"));
			if (obj != null && obj2 != null && obj3 != null && obj4 != null && TryGetCameraMatrices(obj, out var view, out var projection))
			{
				float x = ReadFieldSingle(obj4, "X", 0f);
				float num = ReadFieldSingle(obj4, "Y", 0f);
				float num2 = ReadFieldSingle(obj4, "Z", 0f);
				bool flag = false;
				try
				{
					object field3 = GetField(field, "farmItem");
					object farmRenderMesh = GetFarmRenderMesh(field3);
					flag = farmRenderMesh != null && string.Equals(farmRenderMesh.GetType().FullName, "Quasar.Meshes.TileMesh", StringComparison.Ordinal);
				}
				catch
				{
				}
				float z = (flag ? (0f - num2) : num2);
				float rotation = 0f;
				object obj6 = InvokeProperty(obj2, "Rotation");
				if (obj6 != null)
				{
					rotation = Convert.ToSingle(obj6);
				}
				DrawCompatGeneratedAvatar(device, new Vector3(x, num + 0.15f, z), rotation, view, projection);
				Mark("compat-generated-avatar-drawn");
			}
		}
		catch (Exception ex)
		{
			Mark("compat-generated-avatar-failed-" + DescribeException(ex));
		}
	}

	private static bool TryGetCameraMatrices(object camera, out Matrix view, out Matrix projection)
	{
		view = Matrix.Identity;
		projection = Matrix.Identity;
		try
		{
			object obj = InvokeProperty(camera, "Transform");
			PropertyInfo propertyInfo = obj?.GetType().GetProperty("Matrix", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			FieldInfo fieldInfo = GetFieldInfo(camera.GetType(), "projection");
			if (propertyInfo == null || fieldInfo == null)
			{
				return false;
			}
			object value = propertyInfo.GetValue(obj, null);
			object value2 = fieldInfo.GetValue(camera);
			if (!(value is Matrix) || !(value2 is Matrix))
			{
				return false;
			}
			view = Matrix.Invert((Matrix)value);
			projection = (Matrix)value2;
			return true;
		}
		catch
		{
			return false;
		}
	}

	private static float ReadFieldSingle(object target, string name, float fallback)
	{
		try
		{
			FieldInfo fieldInfo = ((target == null) ? null : GetFieldInfo(target.GetType(), name));
			return (fieldInfo == null) ? fallback : Convert.ToSingle(fieldInfo.GetValue(target));
		}
		catch
		{
			return fallback;
		}
	}

	private static void DrawCompatGeneratedAvatar(GraphicsDevice device, Vector3 position, float rotation, Matrix view, Matrix projection)
	{
		EnsureCompatGeneratedAvatarMesh();
		if (CompatAvatarVertices == null || CompatAvatarIndices == null || CompatAvatarVertices.Length == 0 || CompatAvatarIndices.Length == 0)
		{
			return;
		}
		if (CompatAvatarEffect == null || CompatAvatarEffect.GraphicsDevice != device)
		{
			if (CompatAvatarEffect != null)
			{
				CompatAvatarEffect.Dispose();
			}
			CompatAvatarEffect = new BasicEffect(device);
			CompatAvatarEffect.VertexColorEnabled = true;
			CompatAvatarEffect.LightingEnabled = false;
		}
		DepthStencilState depthStencilState = device.DepthStencilState;
		BlendState blendState = device.BlendState;
		RasterizerState rasterizerState = device.RasterizerState;
		try
		{
			device.DepthStencilState = DepthStencilState.Default;
			device.BlendState = BlendState.Opaque;
			device.RasterizerState = RasterizerState.CullNone;
			CompatAvatarEffect.World = Matrix.CreateScale(0.68f) * Matrix.CreateRotationY(rotation + (float)Math.PI) * Matrix.CreateTranslation(position);
			CompatAvatarEffect.View = view;
			CompatAvatarEffect.Projection = projection;
			foreach (EffectPass pass in CompatAvatarEffect.CurrentTechnique.Passes)
			{
				pass.Apply();
				device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, CompatAvatarVertices, 0, CompatAvatarVertices.Length, CompatAvatarIndices, 0, CompatAvatarIndices.Length / 3);
			}
		}
		finally
		{
			device.DepthStencilState = depthStencilState;
			device.BlendState = blendState;
			device.RasterizerState = rasterizerState;
		}
	}

	private static void EnsureCompatGeneratedAvatarMesh()
	{
		EnsureCompatAvatarObjNotes();
		try
		{
			if (File.Exists(CompatAvatarObjPath))
			{
				DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(CompatAvatarObjPath);
				if (!CompatAvatarTriedObj || lastWriteTimeUtc != CompatAvatarObjStamp)
				{
					CompatAvatarTriedObj = true;
					CompatAvatarObjStamp = lastWriteTimeUtc;
					if (TryLoadCompatAvatarObj(CompatAvatarObjPath, out var vertices, out var indices))
					{
						CompatAvatarVertices = vertices;
						CompatAvatarIndices = indices;
						Mark("compat-generated-avatar-obj-loaded-" + vertices.Length + "-" + indices.Length / 3);
						return;
					}
					Mark("compat-generated-avatar-obj-invalid");
					CompatAvatarVertices = null;
					CompatAvatarIndices = null;
				}
				else if (CompatAvatarVertices != null && CompatAvatarIndices != null)
				{
					return;
				}
			}
			else
			{
				CompatAvatarTriedObj = false;
				CompatAvatarObjStamp = DateTime.MinValue;
			}
		}
		catch (Exception ex)
		{
			Mark("compat-generated-avatar-obj-failed-" + DescribeException(ex));
		}
		if (CompatAvatarVertices == null || CompatAvatarIndices == null)
		{
			List<VertexPositionColor> list = new List<VertexPositionColor>();
			List<int> list2 = new List<int>();
			AddAvatarTaper(list, list2, new Vector3(0f, 1.08f, 0f), 0.62f, 0.17f, 0.24f, 18, new Microsoft.Xna.Framework.Color(63, 116, 196));
			AddAvatarSphere(list, list2, new Vector3(0f, 1.57f, -0.01f), new Vector3(0.17f, 0.22f, 0.16f), 18, 10, new Microsoft.Xna.Framework.Color(225, 174, 132));
			AddAvatarSphere(list, list2, new Vector3(0f, 1.66f, -0.01f), new Vector3(0.175f, 0.09f, 0.155f), 18, 5, new Microsoft.Xna.Framework.Color(82, 48, 34));
			AddAvatarSphere(list, list2, new Vector3(-0.06f, 1.58f, -0.14f), new Vector3(0.016f, 0.02f, 0.008f), 8, 4, new Microsoft.Xna.Framework.Color(24, 32, 48));
			AddAvatarSphere(list, list2, new Vector3(0.06f, 1.58f, -0.14f), new Vector3(0.016f, 0.02f, 0.008f), 8, 4, new Microsoft.Xna.Framework.Color(24, 32, 48));
			AddAvatarCylinder(list, list2, new Vector3(-0.07f, 0.77f, 0f), new Vector3(-0.08f, 0.09f, 0f), 0.065f, 0.055f, 14, new Microsoft.Xna.Framework.Color(45, 62, 91));
			AddAvatarCylinder(list, list2, new Vector3(0.07f, 0.77f, 0f), new Vector3(0.08f, 0.09f, 0f), 0.065f, 0.055f, 14, new Microsoft.Xna.Framework.Color(45, 62, 91));
			AddAvatarBox(list, list2, new Vector3(-0.08f, 0.04f, -0.04f), new Vector3(0.085f, 0.04f, 0.14f), new Microsoft.Xna.Framework.Color(32, 32, 35));
			AddAvatarBox(list, list2, new Vector3(0.08f, 0.04f, -0.04f), new Vector3(0.085f, 0.04f, 0.14f), new Microsoft.Xna.Framework.Color(32, 32, 35));
			AddAvatarCylinder(list, list2, new Vector3(-0.25f, 1.29f, 0f), new Vector3(-0.36f, 0.72f, -0.03f), 0.045f, 0.04f, 12, new Microsoft.Xna.Framework.Color(225, 174, 132));
			AddAvatarCylinder(list, list2, new Vector3(0.25f, 1.29f, 0f), new Vector3(0.36f, 0.72f, -0.03f), 0.045f, 0.04f, 12, new Microsoft.Xna.Framework.Color(225, 174, 132));
			AddAvatarSphere(list, list2, new Vector3(-0.37f, 0.66f, -0.03f), new Vector3(0.05f, 0.06f, 0.04f), 10, 6, new Microsoft.Xna.Framework.Color(225, 174, 132));
			AddAvatarSphere(list, list2, new Vector3(0.37f, 0.66f, -0.03f), new Vector3(0.05f, 0.06f, 0.04f), 10, 6, new Microsoft.Xna.Framework.Color(225, 174, 132));
			CompatAvatarVertices = list.ToArray();
			CompatAvatarIndices = list2.ToArray();
			Mark("compat-generated-avatar-mesh-ready-" + CompatAvatarVertices.Length + "-" + CompatAvatarIndices.Length / 3);
		}
	}

	private static void EnsureCompatAvatarObjNotes()
	{
		try
		{
			if (!File.Exists(CompatAvatarObjNotesPath))
			{
				File.WriteAllText(CompatAvatarObjNotesPath, "Drop-in OBJ avatar replacement for Avatar Farm Online" + Environment.NewLine + "Path: " + CompatAvatarObjPath + Environment.NewLine + Environment.NewLine + "Use a small triangulated OBJ. Vertex colors are supported when v lines include RGB values:" + Environment.NewLine + "v x y z r g b" + Environment.NewLine + Environment.NewLine + "Coordinates are in farm world units; a roughly 1.7-unit tall model fits the current camera." + Environment.NewLine);
			}
		}
		catch
		{
		}
	}

	private static bool TryLoadCompatAvatarObj(string path, out VertexPositionColor[] vertices, out int[] indices)
	{
		vertices = null;
		indices = null;
		try
		{
			List<VertexPositionColor> list = new List<VertexPositionColor>();
			List<int> list2 = new List<int>();
			string[] array = File.ReadAllLines(path);
			for (int i = 0; i < array.Length; i++)
			{
				string text = ((array[i] == null) ? "" : array[i].Trim());
				if (text.Length == 0 || text.StartsWith("#", StringComparison.Ordinal))
				{
					continue;
				}
				string[] array2 = text.Split(new char[2] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
				if (array2.Length >= 4 && string.Equals(array2[0], "v", StringComparison.OrdinalIgnoreCase))
				{
					if (TryParseInvariant(array2[1], out var value) && TryParseInvariant(array2[2], out var value2) && TryParseInvariant(array2[3], out var value3))
					{
						Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(78, 132, 220);
						if (array2.Length >= 7 && TryParseInvariant(array2[4], out var value4) && TryParseInvariant(array2[5], out var value5) && TryParseInvariant(array2[6], out var value6))
						{
							color = new Microsoft.Xna.Framework.Color(ObjColorByte(value4), ObjColorByte(value5), ObjColorByte(value6));
						}
						list.Add(new VertexPositionColor(new Vector3(value, value2, value3), color));
					}
				}
				else
				{
					if (array2.Length < 4 || !string.Equals(array2[0], "f", StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					List<int> list3 = new List<int>();
					for (int j = 1; j < array2.Length; j++)
					{
						if (TryParseObjIndex(array2[j], list.Count, out var index))
						{
							list3.Add(index);
						}
					}
					for (int k = 1; k + 1 < list3.Count; k++)
					{
						list2.Add(list3[0]);
						list2.Add(list3[k]);
						list2.Add(list3[k + 1]);
					}
				}
			}
			if (list.Count < 3 || list2.Count < 3)
			{
				return false;
			}
			vertices = list.ToArray();
			indices = list2.ToArray();
			return true;
		}
		catch
		{
			return false;
		}
	}

	private static bool TryParseObjIndex(string token, int vertexCount, out int index)
	{
		index = 0;
		if (string.IsNullOrEmpty(token))
		{
			return false;
		}
		int num = token.IndexOf('/');
		string s = ((num >= 0) ? token.Substring(0, num) : token);
		if (!int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) || result == 0)
		{
			return false;
		}
		index = ((result > 0) ? (result - 1) : (vertexCount + result));
		return index >= 0 && index < vertexCount;
	}

	private static bool TryParseInvariant(string text, out float value)
	{
		return float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
	}

	private static byte ObjColorByte(float value)
	{
		if (value <= 1f)
		{
			value *= 255f;
		}
		if (value < 0f)
		{
			value = 0f;
		}
		if (value > 255f)
		{
			value = 255f;
		}
		return (byte)value;
	}

	private static void AddAvatarTaper(List<VertexPositionColor> vertices, List<int> indices, Vector3 center, float height, float bottomRadius, float topRadius, int segments, Microsoft.Xna.Framework.Color color)
	{
		AddAvatarCylinder(vertices, indices, center + new Vector3(0f, (0f - height) * 0.5f, 0f), center + new Vector3(0f, height * 0.5f, 0f), bottomRadius, topRadius, segments, color);
	}

	private static void AddAvatarCylinder(List<VertexPositionColor> vertices, List<int> indices, Vector3 start, Vector3 end, float startRadius, float endRadius, int segments, Microsoft.Xna.Framework.Color color)
	{
		Vector3 vector = end - start;
		float num = vector.Length();
		if (!(num <= 0.0001f))
		{
			Vector3 vector2 = vector / num;
			Vector3 vector3 = Vector3.Cross(vector2, Vector3.Forward);
			if (vector3.LengthSquared() < 0.0001f)
			{
				vector3 = Vector3.Cross(vector2, Vector3.Right);
			}
			vector3.Normalize();
			Vector3 vector4 = Vector3.Cross(vector3, vector2);
			vector4.Normalize();
			int count = vertices.Count;
			for (int i = 0; i < segments; i++)
			{
				float num2 = (float)Math.PI * 2f * (float)i / (float)segments;
				Vector3 vector5 = vector3 * (float)Math.Cos(num2) + vector4 * (float)Math.Sin(num2);
				vertices.Add(new VertexPositionColor(start + vector5 * startRadius, color));
				vertices.Add(new VertexPositionColor(end + vector5 * endRadius, color));
			}
			for (int j = 0; j < segments; j++)
			{
				int num3 = (j + 1) % segments;
				AddAvatarTri(indices, count + j * 2, count + num3 * 2, count + j * 2 + 1);
				AddAvatarTri(indices, count + j * 2 + 1, count + num3 * 2, count + num3 * 2 + 1);
			}
		}
	}

	private static void AddAvatarSphere(List<VertexPositionColor> vertices, List<int> indices, Vector3 center, Vector3 radius, int slices, int stacks, Microsoft.Xna.Framework.Color color)
	{
		int count = vertices.Count;
		for (int i = 0; i <= stacks; i++)
		{
			float num = (float)i / (float)stacks;
			float num2 = (float)Math.PI * num;
			for (int j = 0; j <= slices; j++)
			{
				float num3 = (float)j / (float)slices;
				float num4 = (float)Math.PI * 2f * num3;
				Vector3 vector = new Vector3((float)(Math.Sin(num2) * Math.Cos(num4)), (float)Math.Cos(num2), (float)(Math.Sin(num2) * Math.Sin(num4)));
				vertices.Add(new VertexPositionColor(center + new Vector3(vector.X * radius.X, vector.Y * radius.Y, vector.Z * radius.Z), color));
			}
		}
		for (int k = 0; k < stacks; k++)
		{
			for (int l = 0; l < slices; l++)
			{
				int num5 = count + k * (slices + 1) + l;
				int num6 = num5 + 1;
				int num7 = num5 + slices + 1;
				int c = num7 + 1;
				AddAvatarTri(indices, num5, num7, num6);
				AddAvatarTri(indices, num6, num7, c);
			}
		}
	}

	private static void AddAvatarBox(List<VertexPositionColor> vertices, List<int> indices, Vector3 center, Vector3 half, Microsoft.Xna.Framework.Color color)
	{
		Vector3[] array = new Vector3[8]
		{
			center + new Vector3(0f - half.X, 0f - half.Y, 0f - half.Z),
			center + new Vector3(half.X, 0f - half.Y, 0f - half.Z),
			center + new Vector3(half.X, half.Y, 0f - half.Z),
			center + new Vector3(0f - half.X, half.Y, 0f - half.Z),
			center + new Vector3(0f - half.X, 0f - half.Y, half.Z),
			center + new Vector3(half.X, 0f - half.Y, half.Z),
			center + new Vector3(half.X, half.Y, half.Z),
			center + new Vector3(0f - half.X, half.Y, half.Z)
		};
		int[,] array2 = new int[6, 4]
		{
			{ 0, 1, 2, 3 },
			{ 5, 4, 7, 6 },
			{ 4, 0, 3, 7 },
			{ 1, 5, 6, 2 },
			{ 3, 2, 6, 7 },
			{ 4, 5, 1, 0 }
		};
		for (int i = 0; i < 6; i++)
		{
			int count = vertices.Count;
			for (int j = 0; j < 4; j++)
			{
				vertices.Add(new VertexPositionColor(array[array2[i, j]], color));
			}
			AddAvatarTri(indices, count, count + 1, count + 2);
			AddAvatarTri(indices, count, count + 2, count + 3);
		}
	}

	private static void AddAvatarTri(List<int> indices, int a, int b, int c)
	{
		indices.Add(a);
		indices.Add(b);
		indices.Add(c);
	}

	private static void EnsureCompatPlayerProxy(object playerItem)
	{
		try
		{
			if (!UseCompatPlayerProxy || playerItem == null)
			{
				return;
			}
			int hashCode = RuntimeHelpers.GetHashCode(playerItem);
			if (CompatPlayerBodies.ContainsKey(hashCode))
			{
				return;
			}
			object obj = TryCreateAndAttachCompatPlayerProxy(playerItem);
			if (obj != null)
			{
				CompatPlayerBodies[hashCode] = obj;
				Mark("compat-player-proxy-installed");
				return;
			}
			object obj2 = InvokeProperty(playerItem, "Children");
			IList list = obj2 as IList;
			object obj3 = null;
			if (list != null && list.Count > 0)
			{
				object obj4 = null;
				if (list.Count > 1)
				{
					obj4 = ResolveCompatPlayerProxyNodeRecursive(list[1], 2);
					if (obj4 != null)
					{
						Mark("compat-player-proxy-prefer-child1-" + SafeTypeName(obj4));
					}
				}
				if (obj4 == null)
				{
					obj4 = ResolveCompatPlayerProxyNode(list);
				}
				if (obj4 != null)
				{
					object obj5 = CreateCompatProxyTexturedPlane(GetCompatProxyTexture(), new Vector2(1.6f, 3.1f), Vector3.Zero, 0f, 0.98f) ?? CreateCompatProxyPlane(new Vector2(1.5f, 3f), new Vector4(0.95f, 0.84f, 0.42f, 0.98f), Vector3.Zero, 0f);
					object obj6 = ((obj5 == null) ? null : InvokeProperty(obj5, "Mesh"));
					if (obj6 != null)
					{
						try
						{
							SetPropertyValue(obj4, "Mesh", obj6);
							Mark("compat-player-proxy-mesh-swapped");
						}
						catch
						{
						}
					}
					object obj8 = InvokeProperty(obj4, "Transform");
					if (obj8 != null)
					{
						SetPropertyValue(obj8, "Translation", new Vector3(0f, 6f, 0f));
						SetPropertyValue(obj8, "Rotation", Quaternion.Identity);
						try
						{
							SetPropertyValue(obj8, "Scale", new Vector3(4f, 4f, 4f));
						}
						catch
						{
						}
						SetPropertyValue(obj4, "Alpha", 0.98f);
						object obj10 = InvokeProperty(obj4, "Mesh");
						if (obj10 != null)
						{
							object obj11 = InvokeProperty(obj10, "FirstMaterial");
							if (obj11 != null)
							{
								object compatProxyTexture = GetCompatProxyTexture();
								if (compatProxyTexture != null)
								{
									TrySetTexture(obj11, compatProxyTexture);
								}
								SetFieldValue(obj11, "Ambient", new Vector3(0.98f, 0.88f, 0.6f));
								SetFieldValue(obj11, "Diffuse", new Vector3(0.98f, 0.88f, 0.6f));
								SetFieldValue(obj11, "Specular", Vector3.Zero);
								TryInvoke(obj11, "SetForcedAlpha", true);
							}
						}
						if (obj3 == null)
						{
							obj3 = obj4;
						}
					}
				}
			}
			else
			{
				Mark("compat-player-proxy-no-children");
			}
			if (obj3 == null)
			{
				obj3 = TryCreateAndAttachCompatPlayerProxy(playerItem);
			}
			if (obj3 == null)
			{
				Mark("compat-player-proxy-null-node");
				return;
			}
			CompatPlayerBodies[hashCode] = obj3;
			Mark("compat-player-proxy-installed");
		}
		catch (Exception ex)
		{
			Mark("compat-player-proxy-failed-" + DescribeException(ex));
		}
	}

	private static object TryCreateAndAttachCompatPlayerProxy(object playerItem)
	{
		if (playerItem == null)
		{
			return null;
		}
		MethodInfo methodInfo = FindAddChildMethod(playerItem);
		if (methodInfo == null)
		{
			Mark("compat-player-proxy-no-addchild");
		}
		object shaderByName = GetShaderByName("SimpleMultiply");
		object obj = CreateCompatProxyWorldRect(new Vector2(1.6f, 3.1f), new Vector3(0f, 1.55f, 0f), 0f, 0.98f, shaderByName);
		if (obj == null)
		{
			object compatProxyTexture = GetCompatProxyTexture();
			obj = CreateCompatProxyTexturedPlane(compatProxyTexture, new Vector2(1.5f, 3f), new Vector3(0f, 1.5f, 0f), 0f, 0.98f);
		}
		if (obj == null)
		{
			obj = CreateCompatProxyPlane(new Vector2(1.4f, 2.8f), new Vector4(0.95f, 0.84f, 0.42f, 0.98f), new Vector3(0f, 1.45f, 0f), 0f);
		}
		if (obj == null)
		{
			Mark("compat-player-proxy-create-failed");
			return null;
		}
		AddCompatProxyChild(methodInfo, playerItem, obj);
		Mark("compat-player-proxy-added-child-" + SafeTypeName(obj));
		return obj;
	}

	private static MethodInfo FindAddChildMethod(object parent)
	{
		if (parent == null)
		{
			return null;
		}
		MethodInfo[] methods = parent.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (MethodInfo methodInfo in methods)
		{
			if (string.Equals(methodInfo.Name, "AddChild", StringComparison.Ordinal))
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
				if (parameters != null && parameters.Length == 1)
				{
					return methodInfo;
				}
			}
		}
		return null;
	}

	private static object ResolveCompatPlayerProxyNode(IList childList)
	{
		if (childList == null || childList.Count == 0)
		{
			return null;
		}
		for (int i = 0; i < childList.Count; i++)
		{
			object obj = ResolveCompatPlayerProxyNodeRecursive(childList[i], 2);
			if (obj != null && !IsShadowLikeNode(obj))
			{
				Mark("compat-player-proxy-node-" + i + "-" + SafeTypeName(obj));
				return obj;
			}
		}
		for (int j = 0; j < childList.Count; j++)
		{
			object obj2 = ResolveCompatPlayerProxyNodeRecursive(childList[j], 2);
			if (obj2 != null)
			{
				Mark("compat-player-proxy-fallback-node-" + j + "-" + SafeTypeName(obj2));
				return obj2;
			}
		}
		return null;
	}

	private static object ResolveCompatPlayerProxyNodeRecursive(object node, int depth)
	{
		if (node == null)
		{
			return null;
		}
		if (HasMeshAndTransform(node))
		{
			return node;
		}
		if (depth <= 0)
		{
			return null;
		}
		if (!(InvokeProperty(node, "Children") is IList list))
		{
			return null;
		}
		for (int i = 0; i < list.Count; i++)
		{
			object node2 = list[i];
			object obj = ResolveCompatPlayerProxyNodeRecursive(node2, depth - 1);
			if (obj != null && !IsShadowLikeNode(obj))
			{
				return obj;
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			object obj2 = ResolveCompatPlayerProxyNodeRecursive(list[j], depth - 1);
			if (obj2 != null)
			{
				return obj2;
			}
		}
		return null;
	}

	private static bool HasMeshAndTransform(object node)
	{
		if (node == null)
		{
			return false;
		}
		object obj = InvokeProperty(node, "Mesh");
		object obj2 = InvokeProperty(node, "Transform");
		return obj != null && obj2 != null;
	}

	private static bool IsShadowLikeNode(object node)
	{
		string text = SafeTypeName(node);
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		text = text.ToLowerInvariant();
		return text.Contains("shadow") || text.Contains("blob");
	}

	private static string SafeTypeName(object obj)
	{
		try
		{
			return (obj == null) ? "null" : obj.GetType().FullName;
		}
		catch
		{
			return "unknown";
		}
	}

	private static object GetCompatProxyTexture()
	{
		if (CachedProxyTexture != null)
		{
			return CachedProxyTexture;
		}
		try
		{
			string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Models", "Custom", "PlayerAvatar.png");
			if (File.Exists(path))
			{
				using FileStream stream = File.OpenRead(path);
				Texture2D texture2D = Texture2D.FromStream(Engine.Device, stream);
				if (texture2D != null)
				{
					CachedProxyTexture = texture2D;
					Mark("compat-player-proxy-texture-custom");
					return texture2D;
				}
			}
			Type type = Type.GetType("Quasar.Textures.TextureManager, Quasar");
			MethodInfo methodInfo = ((type == null) ? null : type.GetMethod("get_Textures", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));
			object obj = ((methodInfo == null) ? null : methodInfo.Invoke(null, null));
			if (obj == null)
			{
				return null;
			}
			string[] array = new string[3] { "GroundTiles", "blobShadow", "default" };
			for (int i = 0; i < array.Length; i++)
			{
				try
				{
					object obj2 = InvokeIndexer(obj, array[i]);
					if (obj2 != null)
					{
						CachedProxyTexture = obj2;
						Mark("compat-player-proxy-texture-" + array[i]);
						return obj2;
					}
				}
				catch
				{
				}
			}
		}
		catch (Exception ex)
		{
			Mark("compat-player-proxy-texture-failed-" + DescribeException(ex));
		}
		return null;
	}

	private static object GetTextureByName(string name)
	{
		try
		{
			Type type = Type.GetType("Quasar.Textures.TextureManager, Quasar");
			MethodInfo methodInfo = ((type == null) ? null : type.GetMethod("get_Textures", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));
			object obj = ((methodInfo == null) ? null : methodInfo.Invoke(null, null));
			return (obj == null) ? null : InvokeIndexer(obj, name);
		}
		catch
		{
			return null;
		}
	}

	private static void AddCompatProxyChild(MethodInfo addChild, object parent, object child)
	{
		if (parent != null && child != null)
		{
			if (addChild != null)
			{
				addChild.Invoke(parent, new object[1] { child });
			}
			else if (InvokeProperty(parent, "Children") is IList list && !list.Contains(child))
			{
				list.Add(child);
			}
		}
	}

	private static object CreateCompatProxyPlane(Vector2 size, Vector4 color, Vector3 translation, float yaw)
	{
		Type type = Type.GetType("Quasar.Items._2D.Rectangle, Quasar");
		if (type == null)
		{
			return null;
		}
		object obj = Activator.CreateInstance(type, size, color);
		if (obj == null)
		{
			return null;
		}
		SetPropertyValue(obj, "Alpha", color.W);
		object obj2 = InvokeProperty(obj, "Transform");
		if (obj2 != null)
		{
			SetPropertyValue(obj2, "Translation", translation);
			SetPropertyValue(obj2, "Rotation", Quaternion.CreateFromAxisAngle(Vector3.Up, yaw));
		}
		object obj3 = InvokeProperty(obj, "Mesh");
		if (obj3 != null)
		{
			object obj4 = GetShaderByName("SimpleNoZWrite") ?? GetShaderByName("SimpleMultiply") ?? GetShaderByName("Simple");
			if (obj4 != null)
			{
				TrySetProperty(obj3, "Shader", obj4);
			}
			object obj5 = InvokeProperty(obj3, "FirstMaterial");
			if (obj5 != null)
			{
				SetFieldValue(obj5, "Ambient", new Vector3(color.X, color.Y, color.Z));
				SetFieldValue(obj5, "Diffuse", new Vector3(color.X, color.Y, color.Z));
				SetFieldValue(obj5, "Specular", Vector3.Zero);
			}
		}
		return obj;
	}

	private static object CreateCompatProxyTexturedPlane(object texture, Vector2 size, Vector3 translation, float yaw, float alpha)
	{
		if (texture == null)
		{
			return null;
		}
		Type type = Type.GetType("Quasar.Items._2D.Rectangle, Quasar");
		if (type == null)
		{
			return null;
		}
		object obj = Activator.CreateInstance(type, texture, size);
		if (obj == null)
		{
			return null;
		}
		SetPropertyValue(obj, "Alpha", alpha);
		object obj2 = InvokeProperty(obj, "Transform");
		if (obj2 != null)
		{
			SetPropertyValue(obj2, "Translation", translation);
			SetPropertyValue(obj2, "Rotation", Quaternion.CreateFromAxisAngle(Vector3.Up, yaw));
		}
		object obj3 = InvokeProperty(obj, "Mesh");
		if (obj3 != null)
		{
			object obj4 = InvokeProperty(obj3, "FirstMaterial");
			if (obj4 != null)
			{
				TrySetTexture(obj4, texture);
				SetFieldValue(obj4, "Ambient", new Vector3(0.92f, 0.74f, 0.34f));
				SetFieldValue(obj4, "Diffuse", new Vector3(0.96f, 0.8f, 0.42f));
				SetFieldValue(obj4, "Specular", Vector3.Zero);
			}
		}
		return obj;
	}

	private static object CreateCompatProxyWorldRect(Vector2 size, Vector3 translation, float yaw, float alpha, object shader)
	{
		Type type = Type.GetType("Quasar.Items._2D.Rectangle, Quasar");
		if (type == null)
		{
			return null;
		}
		object compatProxyTexture = GetCompatProxyTexture();
		if (compatProxyTexture == null)
		{
			return null;
		}
		object obj = Activator.CreateInstance(type, compatProxyTexture, size);
		if (obj == null)
		{
			return null;
		}
		SetPropertyValue(obj, "Alpha", alpha);
		object obj2 = InvokeProperty(obj, "Transform");
		if (obj2 != null)
		{
			Quaternion quaternion = Quaternion.CreateFromAxisAngle(Vector3.UnitX, 0f);
			Quaternion quaternion2 = Quaternion.CreateFromAxisAngle(Vector3.Up, yaw);
			SetPropertyValue(obj2, "Translation", translation);
			SetPropertyValue(obj2, "Rotation", quaternion2 * quaternion);
		}
		object obj3 = InvokeProperty(obj, "Mesh");
		if (obj3 != null)
		{
			TrySetProperty(obj3, "Shader", shader ?? GetShaderByName("SimpleMultiply"));
			object obj4 = InvokeProperty(obj3, "FirstMaterial");
			if (obj4 != null)
			{
				TryInvoke(obj4, "SetForcedAlpha", true);
				TrySetTexture(obj4, compatProxyTexture);
				SetFieldValue(obj4, "Ambient", new Vector3(0.92f, 0.78f, 0.55f));
				SetFieldValue(obj4, "Diffuse", new Vector3(0.92f, 0.78f, 0.55f));
				SetFieldValue(obj4, "Specular", Vector3.Zero);
			}
		}
		return obj;
	}

	private static void TrySetTexture(object material, object texture)
	{
		if (material == null || texture == null)
		{
			return;
		}
		try
		{
			MethodInfo method = material.GetType().GetMethod("set_Texture", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method != null)
			{
				method.Invoke(material, new object[1] { texture });
				return;
			}
		}
		catch
		{
		}
		try
		{
			object obj2 = InvokeProperty(material, "Textures");
			if (obj2 is IList list)
			{
				if (list.Count == 0)
				{
					list.Add(texture);
				}
				else
				{
					list[0] = texture;
				}
			}
		}
		catch
		{
		}
	}

	private static void EnsureGenericFarmTileMesh(object section)
	{
		try
		{
			object field = GetField(section, "stage");
			object field2 = GetField(section, "stageScene");
			object obj = ((field2 == null) ? null : GetField(field2, "farmItem"));
			if (obj == null)
			{
				return;
			}
			object farmRenderMesh = GetFarmRenderMesh(obj);
			if (farmRenderMesh != null && string.Equals(farmRenderMesh.GetType().FullName, "Quasar.Meshes.TileMesh", StringComparison.Ordinal))
			{
				return;
			}
			object obj2 = ((field == null) ? null : InvokeProperty(field, "FarmData"));
			object obj3 = ((obj2 == null) ? null : InvokeProperty(obj2, "FarmSize"));
			object field3 = GetField(obj, "tileMesh");
			object obj4 = ((field3 == null) ? null : InvokeProperty(field3, "FirstMaterial"));
			object obj5 = ((obj4 == null) ? null : InvokeProperty(obj4, "Textures"));
			object obj6 = null;
			if (obj5 is IList { Count: >0 } list)
			{
				obj6 = list[0];
			}
			if (obj3 == null || obj6 == null)
			{
				return;
			}
			Type type = Type.GetType("Quasar.Meshes.TileMesh, Quasar");
			Type type2 = Type.GetType("Microsoft.Xna.Framework.Vector2, FNA");
			Type type3 = obj3.GetType();
			if (!(type == null) && !(type2 == null) && !(type3 == null))
			{
				object obj7 = Activator.CreateInstance(type2, 2f);
				object obj8 = Activator.CreateInstance(type3, 2);
				object obj9 = Activator.CreateInstance(type, obj3, obj7, obj8, obj6);
				SetPropertyValue(obj9, "UseXZCoords", true);
				object obj10 = InvokeProperty(obj9, "FirstMaterial");
				if (obj10 != null)
				{
					SetFieldValue(obj10, "Shininess", 10f);
					SetFieldValue(obj10, "Ambient", new Vector3(1f));
					SetFieldValue(obj10, "Diffuse", new Vector3(1f));
					SetFieldValue(obj10, "Specular", new Vector3(0f));
				}
				MethodInfo method = obj.GetType().GetMethod("clearMeshes", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				MethodInfo method2 = obj.GetType().GetMethod("addMesh", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (!(method == null) && !(method2 == null))
				{
					method.Invoke(obj, null);
					method2.Invoke(obj, new object[1] { obj9 });
					Mark("farmtilemesh-generic-installed");
				}
			}
		}
		catch (Exception ex)
		{
			Mark("farmtilemesh-generic-failed-" + DescribeException(ex));
		}
	}

	private static object GetFarmRenderMesh(object farmItem)
	{
		if (farmItem == null)
		{
			return null;
		}
		object obj = InvokeProperty(farmItem, "Meshes");
		if (obj is IList { Count: >0 } list)
		{
			return list[0];
		}
		return null;
	}

	private static int[] GetTileIndicesArray(object tileMesh)
	{
		if (tileMesh == null)
		{
			return null;
		}
		FieldInfo fieldInfo = GetFieldInfo(tileMesh.GetType(), "TileIndices");
		if (fieldInfo != null)
		{
			return fieldInfo.GetValue(tileMesh) as int[];
		}
		PropertyInfo property = tileMesh.GetType().GetProperty("TileIndices", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (property != null)
		{
			return property.GetValue(tileMesh, null) as int[];
		}
		return null;
	}

	private static void SetPropertyValue(object instance, string name, object value)
	{
		if (instance != null)
		{
			PropertyInfo property = instance.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (property != null && property.CanWrite)
			{
				property.SetValue(instance, value, null);
			}
		}
	}

	private static void TrySetProperty(object instance, string name, object value)
	{
		try
		{
			SetPropertyValue(instance, name, value);
		}
		catch
		{
		}
	}

	private static object TryInvoke(object instance, string name, params object[] args)
	{
		if (instance == null)
		{
			return null;
		}
		try
		{
			Type[] array = new Type[(args != null) ? args.Length : 0];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = ((args[i] == null) ? typeof(object) : args[i].GetType());
			}
			MethodInfo methodInfo = instance.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, array, null);
			if (methodInfo == null)
			{
				MethodInfo[] methods = instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				for (int j = 0; j < methods.Length; j++)
				{
					if (methods[j].Name == name && methods[j].GetParameters().Length == array.Length)
					{
						methodInfo = methods[j];
						break;
					}
				}
			}
			return (methodInfo == null) ? null : methodInfo.Invoke(instance, args);
		}
		catch
		{
			return null;
		}
	}

	private static object GetShaderByName(string name)
	{
		try
		{
			Type type = Type.GetType("Quasar.Shaders.ShaderManager, Quasar");
			MethodInfo methodInfo = ((type == null) ? null : type.GetMethod("get_Shaders", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));
			object obj = ((methodInfo == null) ? null : methodInfo.Invoke(null, null));
			return (obj == null) ? null : InvokeIndexer(obj, name);
		}
		catch
		{
			return null;
		}
	}

	private static void SetFieldValue(object instance, string name, object value)
	{
		if (instance != null)
		{
			FieldInfo fieldInfo = GetFieldInfo(instance.GetType(), name);
			if (fieldInfo != null)
			{
				fieldInfo.SetValue(instance, value);
			}
		}
	}

	private static void SetSceneEnabled(object scene, bool enabled)
	{
		if (scene != null)
		{
			PropertyInfo property = scene.GetType().GetProperty("Enabled", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (property != null && property.CanWrite)
			{
				property.SetValue(scene, enabled, null);
			}
		}
	}

	private static void SetSceneCamera(object scene, object camera)
	{
		if (scene != null && camera != null)
		{
			PropertyInfo property = scene.GetType().GetProperty("Camera", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (property != null && property.CanWrite)
			{
				property.SetValue(scene, camera, null);
			}
		}
	}

	private static void TuneStageRenderPass(object section)
	{
		try
		{
			if (UseCompatStageClear)
			{
				object obj = InvokeProperty(section, "MainRenderPass");
				if (obj == null)
				{
					obj = GetField(section, "mainRenderPass");
				}
				if (obj != null)
				{
					SetPropertyValue(obj, "MustClearColor", true);
					SetPropertyValue(obj, "BackgroundColor", new Microsoft.Xna.Framework.Color(100, 160, 110));
				}
			}
		}
		catch (Exception ex)
		{
			Mark("tune-stage-renderpass-failed-" + DescribeException(ex));
		}
	}

	private static void ProcessStageKeyboard(object section)
	{
		int num = Environment.TickCount & 0x7FFFFFFF;
		if (num < nextStageKeyboardTicks)
		{
			return;
		}
		object field = GetField(section, "stage");
		object obj = ((field == null) ? null : InvokeProperty(field, "IsInPlayableState"));
		object obj2 = ((field == null) ? null : InvokeProperty(field, "IsShowingMessage"));
		if (obj is bool && (bool)obj && (!(obj2 is int num2) || num2 == 0))
		{
			string text = null;
			if (WasStageKeyboardOrMousePressed(32))
			{
				text = "SPACE";
			}
			if (WasStageKeyboardOrMousePressed(69))
			{
				text = "ACTION";
			}
			if (WasStageKeyboardOrMousePressed(81))
			{
				text = "SHOP";
			}
			if (WasStageKeyboardOrMousePressed(80))
			{
				text = "PLANT";
			}
			if (WasStageKeyboardOrMousePressed(88))
			{
				text = "RECYCLE";
			}
			if (WasStageKeyboardOrMousePressed(67))
			{
				text = "CAMERA";
			}
			if (WasStageKeyboardOrMousePressed(49))
			{
				text = "CAM1";
			}
			if (WasStageKeyboardOrMousePressed(50))
			{
				text = "CAM2";
			}
			if (WasStageKeyboardOrMousePressed(51))
			{
				text = "CAM3";
			}
			if (WasStageKeyboardOrMousePressed(52))
			{
				text = "CAM4";
			}
			if (WasStageKeyboardOrMousePressed(74))
			{
				text = "CAMLEFT";
			}
			if (WasStageKeyboardOrMousePressed(76))
			{
				text = "CAMRIGHT";
			}
			if (WasStageKeyboardOrMousePressed(73))
			{
				text = "CAMUP";
			}
			if (WasStageKeyboardOrMousePressed(75))
			{
				text = "CAMDOWN";
			}
			if (WasStageKeyboardOrMousePressed(85))
			{
				text = "CAMIN";
			}
			if (WasStageKeyboardOrMousePressed(79))
			{
				text = "CAMOUT";
			}
			if (WasStageKeyboardOrMousePressed(72))
			{
				text = "CAMRESET";
			}
			if (text != null)
			{
				nextStageKeyboardTicks = num + 110;
				Mark("stage-command-input-" + text);
				ApplyStageCommand(section, text);
			}
		}
	}

	private static void QueueCompatMovement(string command)
	{
		int value = 0;
		int value2 = 0;
		switch (command)
		{
		default:
			return;
		case "LEFT":
			value = -1;
			break;
		case "RIGHT":
			value = 1;
			break;
		case "UP":
			value2 = 1;
			break;
		case "DOWN":
			value2 = -1;
			break;
		}
		Interlocked.Exchange(ref queuedMoveX, value);
		Interlocked.Exchange(ref queuedMoveY, value2);
		Interlocked.Exchange(ref queuedMoveExpireTick, (Environment.TickCount & 0x7FFFFFFF) + 180);
	}

	private static object GetCurrentGameSection()
	{
		Type type = Type.GetType("Quasar.GameUtils.Game.BaseGame, QuasarGameUtils");
		if (type == null)
		{
			return null;
		}
		MethodInfo method = type.GetMethod("get_Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		object target = ((method == null) ? null : method.Invoke(null, null));
		return InvokeProperty(target, "CurrentGameSection");
	}

	private static void ApplyStageCommand(object section, string command)
	{
		object field = GetField(section, "stage");
		if (field == null)
		{
			return;
		}
		switch (command)
		{
		case "ENTER":
		{
			MethodInfo method4 = field.GetType().GetMethod("Resume", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method4 != null)
			{
				method4.Invoke(field, null);
				Mark("stage-command-resume");
			}
			break;
		}
		case "ESC":
		{
			bool flag2 = false;
			object obj5 = InvokeProperty(field, "IsPaused");
			if (obj5 is bool)
			{
				flag2 = (bool)obj5;
			}
			string name = (flag2 ? "Resume" : "Pause");
			MethodInfo method3 = field.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method3 != null)
			{
				method3.Invoke(field, null);
				Mark(flag2 ? "stage-command-resume" : "stage-command-pause");
			}
			break;
		}
		default:
			if (!(command == "CAMRESET"))
			{
				object obj = InvokeProperty(field, "LocalPlayer");
				if (obj == null)
				{
					break;
				}
				switch (command)
				{
				case "SHOP":
				{
					MethodInfo method2 = obj.GetType().GetMethod("Shop", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[1] { typeof(bool) }, null);
					if (method2 != null)
					{
						method2.Invoke(obj, new object[1] { false });
						Mark("stage-command-shop");
					}
					break;
				}
				default:
					if (!(command == "RECYCLE"))
					{
						object obj2 = InvokeProperty(obj, "Position");
						if (obj2 == null)
						{
							break;
						}
						Type type = obj2.GetType();
						FieldInfo field2 = type.GetField("X", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						FieldInfo field3 = type.GetField("Y", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						if (!(field2 == null) && !(field3 == null))
						{
							float x = Convert.ToSingle(field2.GetValue(obj2));
							float y = Convert.ToSingle(field3.GetValue(obj2));
							float yaw = 0f;
							if (command == "LEFT")
							{
								x -= 1.5f;
								yaw = (float)Math.PI / 2f;
							}
							if (command == "RIGHT")
							{
								x += 1.5f;
								yaw = -(float)Math.PI / 2f;
							}
							if (command == "UP")
							{
								y -= 1.5f;
								yaw = 0f;
							}
							if (command == "DOWN")
							{
								y += 1.5f;
								yaw = (float)Math.PI;
							}
							ClampStagePosition(field, ref x, ref y);
							object obj3 = Activator.CreateInstance(type, x, y);
							TrySetPlayerFacing(obj, yaw);
							MethodInfo method = obj.GetType().GetMethod("SetPosition", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
							if (method != null)
							{
								method.Invoke(obj, new object[1] { obj3 });
								Mark("stage-command-move-" + command + "-" + x.ToString("0.0", CultureInfo.InvariantCulture) + "-" + y.ToString("0.0", CultureInfo.InvariantCulture));
							}
						}
						break;
					}
					goto case "SPACE";
				case "SPACE":
				case "ACTION":
				case "PLANT":
					try
					{
						object obj4 = InvokeProperty(obj, "HighlightedTile");
						if (obj4 == null)
						{
							Mark("stage-command-action-no-tile");
							break;
						}
						object farmData = InvokeProperty(field, "FarmData");
						string text = ((command == "SPACE" || command == "ACTION") ? ChooseStageAction(farmData, obj4) : command);
						bool flag = ((!(text == "PLANT")) ? WorkTile(obj, obj4, text, null, 0, out var failReason) : TryPlantAny(obj, obj4, out failReason));
						Mark("stage-command-work-" + text + "-" + flag + "-" + ((failReason == null) ? "null" : failReason.ToString()));
						break;
					}
					catch (Exception ex)
					{
						Mark("stage-command-action-failed-" + DescribeException(ex));
						break;
					}
				}
				break;
			}
			goto case "CAMERA";
		case "CAMERA":
		case "CAM1":
		case "CAM2":
		case "CAM3":
		case "CAM4":
		case "CAMLEFT":
		case "CAMRIGHT":
		case "CAMUP":
		case "CAMDOWN":
		case "CAMIN":
		case "CAMOUT":
			ApplyCameraCommand(field, command);
			break;
		}
	}

	private static void TrySetPlayerFacing(object player, float yaw)
	{
		try
		{
			if (player != null)
			{
				object obj = InvokeProperty(player, "Orientation");
				Type type = ((obj == null) ? Type.GetType("Microsoft.Xna.Framework.Vector2, FNA") : obj.GetType());
				object obj2 = Activator.CreateInstance(type, yaw, -0.18f);
				MethodInfo method = player.GetType().GetMethod("SetOrientation", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (method != null)
				{
					method.Invoke(player, new object[1] { obj2 });
				}
				FieldInfo fieldInfo = GetFieldInfo(player.GetType(), "rotation");
				if (fieldInfo != null)
				{
					fieldInfo.SetValue(player, yaw);
				}
			}
		}
		catch (Exception ex)
		{
			Mark("player-facing-failed-" + DescribeException(ex));
		}
	}

	private static void ForcePlayerCameraState(object player, int stateValue)
	{
		try
		{
			if (player == null)
			{
				return;
			}
			MethodInfo method = player.GetType().GetMethod("SetCameraState", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (!(method == null))
			{
				ParameterInfo[] parameters = method.GetParameters();
				if (parameters.Length == 1)
				{
					object obj = Enum.ToObject(parameters[0].ParameterType, stateValue);
					method.Invoke(player, new object[1] { obj });
				}
			}
		}
		catch (Exception ex)
		{
			Mark("force-player-camera-state-failed-" + DescribeException(ex));
		}
	}

	private static void SetPlayerCameraOrientation(object player, float yaw, float pitch)
	{
		try
		{
			if (player == null)
			{
				return;
			}
			if (pitch < -1f)
			{
				pitch = -1f;
			}
			if (pitch > 0.2f)
			{
				pitch = 0.2f;
			}
			object obj = InvokeProperty(player, "Orientation");
			Type type = ((obj == null) ? Type.GetType("Microsoft.Xna.Framework.Vector2, FNA") : obj.GetType());
			if (!(type == null))
			{
				object obj2 = Activator.CreateInstance(type, yaw, pitch);
				MethodInfo method = player.GetType().GetMethod("SetOrientation", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (method != null)
				{
					method.Invoke(player, new object[1] { obj2 });
				}
			}
		}
		catch (Exception ex)
		{
			Mark("player-camera-orientation-failed-" + DescribeException(ex));
		}
	}

	private static void ApplyCameraCommand(object stage, string command)
	{
		try
		{
			ReadFreeCameraSettings();
			bool flag = true;
			switch (command)
			{
			case "CAMLEFT":
				FreeCameraYaw -= 0.18f;
				break;
			case "CAMRIGHT":
				FreeCameraYaw += 0.18f;
				break;
			case "CAMUP":
				FreeCameraPitch = Math.Max(-1.05f, FreeCameraPitch - 0.08f);
				break;
			case "CAMDOWN":
				FreeCameraPitch = Math.Min(0.45f, FreeCameraPitch + 0.08f);
				break;
			case "CAMIN":
				FreeCameraDistance = Math.Max(2.5f, FreeCameraDistance - 0.55f);
				break;
			case "CAMOUT":
				FreeCameraDistance = Math.Min(16f, FreeCameraDistance + 0.55f);
				break;
			case "CAMRESET":
				FreeCameraYaw = 0f;
				FreeCameraPitch = -0.18f;
				FreeCameraDistance = 5.8f;
				ForcedCameraMode = 1;
				break;
			default:
				flag = false;
				break;
			}
			if (flag)
			{
				ForcedCameraMode = 1;
				WriteFreeCameraSettings();
				MarkFloats("camera-free", FreeCameraYaw, FreeCameraPitch, FreeCameraDistance, 0f);
				return;
			}
			int num = ForcedCameraMode;
			switch (command)
			{
			case "CAMERA":
				num++;
				if (num > 3)
				{
					num = 1;
				}
				break;
			case "CAM1":
				num = 1;
				break;
			case "CAM2":
				num = 2;
				break;
			case "CAM3":
				num = 3;
				break;
			case "CAM4":
				num = 0;
				break;
			}
			ForcedCameraMode = num;
			Mark("camera-command-" + command + "-mode-" + num);
		}
		catch (Exception ex)
		{
			Mark("camera-command-failed-" + DescribeException(ex));
		}
	}

	private static void ReadFreeCameraSettings()
	{
		FreeCameraYaw = ReadFloatFile(CameraYawPath, FreeCameraYaw);
		FreeCameraPitch = ReadFloatFile(CameraPitchPath, FreeCameraPitch);
		FreeCameraDistance = ReadFloatFile(CameraDistancePath, FreeCameraDistance);
		if (FreeCameraDistance < 2.5f || FreeCameraDistance > 16f)
		{
			FreeCameraDistance = 5.8f;
		}
	}

	private static void WriteFreeCameraSettings()
	{
		WriteFloatFile(CameraYawPath, FreeCameraYaw);
		WriteFloatFile(CameraPitchPath, FreeCameraPitch);
		WriteFloatFile(CameraDistancePath, FreeCameraDistance);
	}

	private static void WriteFloatFile(string path, float value)
	{
		try
		{
			File.WriteAllText(path, value.ToString("0.###", CultureInfo.InvariantCulture));
		}
		catch
		{
		}
	}

	private static void ApplyForcedStageCamera(object section)
	{
		try
		{
			object field = GetField(section, "stageScene");
			if (field == null)
			{
				Mark("forced-camera-no-stage-scene");
				return;
			}
			object obj = InvokeProperty(field, "GameCamera");
			object field2 = GetField(section, "stage");
			object obj2 = InvokeProperty(field2, "LocalPlayer");
			object obj3 = InvokeProperty(field2, "FarmData");
			if (obj == null || obj2 == null || obj3 == null)
			{
				Mark("forced-camera-missing-" + (obj == null) + "-" + (obj2 == null) + "-" + (obj3 == null));
			}
			else
			{
				if (ForcedCameraMode == 0)
				{
					return;
				}
				object obj4 = InvokeProperty(obj, "Transform");
				PropertyInfo propertyInfo = obj4?.GetType().GetProperty("Matrix", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				FieldInfo fieldInfo = GetFieldInfo(obj.GetType(), "projection");
				MethodInfo method = obj.GetType().GetMethod("UpdateMatrix", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (obj4 == null || propertyInfo == null || !propertyInfo.CanWrite || fieldInfo == null || method == null)
				{
					Mark("forced-camera-missing-transform-" + (obj4 == null) + "-" + (propertyInfo == null) + "-" + (fieldInfo == null) + "-" + (method == null));
					return;
				}
				object obj5 = InvokeProperty(obj3, "FarmExtents");
				object obj6 = InvokeProperty(obj2, "WorldPosition");
				if (obj5 == null || obj6 == null)
				{
					Mark("forced-camera-missing-pos-" + (obj5 == null) + "-" + (obj6 == null));
					return;
				}
				FieldInfo fieldInfo2 = GetFieldInfo(obj5.GetType(), "X");
				FieldInfo fieldInfo3 = GetFieldInfo(obj5.GetType(), "Y");
				FieldInfo fieldInfo4 = GetFieldInfo(obj6.GetType(), "X");
				FieldInfo fieldInfo5 = GetFieldInfo(obj6.GetType(), "Y");
				FieldInfo fieldInfo6 = GetFieldInfo(obj6.GetType(), "Z");
				if (fieldInfo2 == null || fieldInfo3 == null || fieldInfo4 == null || fieldInfo5 == null || fieldInfo6 == null)
				{
					Mark("forced-camera-missing-fields");
					return;
				}
				float num = Convert.ToSingle(fieldInfo2.GetValue(obj5));
				float num2 = Convert.ToSingle(fieldInfo3.GetValue(obj5));
				float x = Convert.ToSingle(fieldInfo4.GetValue(obj6));
				float num3 = Convert.ToSingle(fieldInfo5.GetValue(obj6));
				float num4 = Convert.ToSingle(fieldInfo6.GetValue(obj6));
				float num5 = Math.Max(8f, Math.Max(num, num2));
				bool flag = false;
				try
				{
					object field3 = GetField(field, "farmItem");
					object farmRenderMesh = GetFarmRenderMesh(field3);
					flag = farmRenderMesh != null && string.Equals(farmRenderMesh.GetType().FullName, "Quasar.Meshes.TileMesh", StringComparison.Ordinal);
				}
				catch
				{
				}
				float z = (flag ? (0f - num4) : num4);
				Vector3 vector = new Vector3(x, num3 + 0.9f, z);
				Vector3 vector2 = (flag ? new Vector3(num * 0.5f, 0.8f, 0f - num2 * 0.5f) : new Vector3(num * 0.5f, 0.8f, num2 * 0.5f));
				float num6 = 0f;
				float num7 = -0.25f;
				try
				{
					object obj8 = InvokeProperty(obj2, "Orientation");
					if (obj8 != null)
					{
						FieldInfo fieldInfo7 = GetFieldInfo(obj8.GetType(), "X");
						FieldInfo fieldInfo8 = GetFieldInfo(obj8.GetType(), "Y");
						if (fieldInfo7 != null)
						{
							num6 = Convert.ToSingle(fieldInfo7.GetValue(obj8));
						}
						if (fieldInfo8 != null)
						{
							num7 = Convert.ToSingle(fieldInfo8.GetValue(obj8));
						}
					}
				}
				catch
				{
				}
				if (num6 == 0f)
				{
					object obj10 = InvokeProperty(obj2, "Rotation");
					if (obj10 != null)
					{
						num6 = Convert.ToSingle(obj10);
					}
				}
				ReadFreeCameraSettings();
				if (ForcedCameraMode == 1)
				{
					ForcePlayerCameraState(obj2, 0);
					SetPlayerCameraOrientation(obj2, FreeCameraYaw, FreeCameraPitch);
					MarkFloats("native-follow-camera", FreeCameraYaw, FreeCameraPitch, FreeCameraDistance, 0f);
					return;
				}
				Vector3 cameraPosition;
				Vector3 cameraUpVector;
				switch (ForcedCameraMode)
				{
				case 1:
				{
					ForcePlayerCameraState(obj2, 0);
					vector = new Vector3(x, num3 + 1.15f, z);
					float num8 = num6 + FreeCameraYaw;
					float num9 = Math.Max(2.5f, Math.Min(16f, FreeCameraDistance));
					float y = Math.Max(0.65f, 2.25f + FreeCameraPitch * 3.5f);
					Vector3 vector3 = new Vector3((float)Math.Sin(num8) * (0f - num9), y, (float)Math.Cos(num8) * num9);
					Vector3 vector4 = new Vector3((float)Math.Sin(num8) * 1.8f, FreeCameraPitch, (float)Math.Cos(num8) * -1.8f);
					cameraPosition = vector + vector3;
					vector += vector4;
					cameraUpVector = Vector3.Up;
					break;
				}
				case 2:
					vector = vector2;
					cameraPosition = vector2 + new Vector3((0f - num5) * 0.65f, num5 * 1.05f, num5 * 0.75f);
					cameraUpVector = Vector3.Up;
					break;
				default:
					vector = vector2;
					cameraPosition = vector2 + new Vector3(0f, num5 * 1.55f, 0.05f);
					cameraUpVector = new Vector3(0f, 0f, -1f);
					break;
				}
				Matrix matrix = Matrix.CreateLookAt(cameraPosition, vector, cameraUpVector);
				propertyInfo.SetValue(obj4, matrix, null);
				fieldInfo.SetValue(obj, Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, GetEngineAspectRatio(), 0.1f, 10000f));
				method.Invoke(obj, null);
				LogProjectedPoint(obj, "forced-camera-center", vector2);
				LogProjectedPoint(obj, "forced-camera-target", vector);
				MarkFloats("forced-camera-pos", cameraPosition.X, cameraPosition.Y, cameraPosition.Z, num5);
				Mark("forced-camera-applied-" + ForcedCameraMode);
			}
		}
		catch (Exception ex)
		{
			Mark("forced-camera-failed-" + DescribeException(ex));
		}
	}

	private static float GetEngineAspectRatio()
	{
		try
		{
			Type type = Type.GetType("Quasar.Global.Engine, Quasar");
			MethodInfo methodInfo = ((type == null) ? null : type.GetMethod("get_AspectRatio", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));
			object obj = ((methodInfo == null) ? null : methodInfo.Invoke(null, null));
			float num = ((obj == null) ? 1.7777778f : Convert.ToSingle(obj));
			if (num > 0.1f && num < 10f)
			{
				return num;
			}
		}
		catch
		{
		}
		return 1.7777778f;
	}

	private static Vector2 InvokeGameMathVectorFromAngle(float angle, float length)
	{
		try
		{
			Type type = Type.GetType("Quasar.Global.GameMath, Quasar");
			MethodInfo methodInfo = ((type == null) ? null : type.GetMethod("VectorFromAngle", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[2]
			{
				typeof(float),
				typeof(float)
			}, null));
			if (((methodInfo == null) ? null : methodInfo.Invoke(null, new object[2] { angle, length })) is Vector2 result)
			{
				return result;
			}
		}
		catch
		{
		}
		return new Vector2((float)Math.Sin(angle) * length, (float)Math.Cos(angle) * length);
	}

	private static Vector3 ReadVector3Property(object target, string propertyName, Vector3 fallback)
	{
		try
		{
			if (InvokeProperty(target, propertyName) is Vector3 result)
			{
				return result;
			}
		}
		catch
		{
		}
		return fallback;
	}

	private static void LogProjectedPoint(object camera, string label, Vector3 point)
	{
		if (!string.Equals(Environment.GetEnvironmentVariable("AVATARFARM_VERBOSE_PROJECTION"), "1", StringComparison.Ordinal))
		{
			return;
		}
		try
		{
			MethodInfo method = camera.GetType().GetMethod("ProjectPoint", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[1] { typeof(Vector3) }, null);
			object obj = ((method == null) ? null : method.Invoke(camera, new object[1] { point }));
			if (obj == null)
			{
				return;
			}
			FieldInfo fieldInfo = GetFieldInfo(obj.GetType(), "X");
			FieldInfo fieldInfo2 = GetFieldInfo(obj.GetType(), "Y");
			FieldInfo fieldInfo3 = GetFieldInfo(obj.GetType(), "Z");
			FieldInfo fieldInfo4 = GetFieldInfo(obj.GetType(), "W");
			if (!(fieldInfo == null) && !(fieldInfo2 == null) && !(fieldInfo3 == null) && !(fieldInfo4 == null))
			{
				float num = Convert.ToSingle(fieldInfo.GetValue(obj));
				float num2 = Convert.ToSingle(fieldInfo2.GetValue(obj));
				float num3 = Convert.ToSingle(fieldInfo3.GetValue(obj));
				float num4 = Convert.ToSingle(fieldInfo4.GetValue(obj));
				if (Math.Abs(num4) > 0.0001f)
				{
					MarkFloats(label, num / num4, num2 / num4, num3 / num4, num4);
				}
				else
				{
					MarkFloats(label + "-clip", num, num2, num3, num4);
				}
			}
		}
		catch
		{
		}
	}

	private static bool TryPlantAny(object player, object tile, out object failReason)
	{
		string[] array = new string[10] { "Carrot", "Clover", "Daisy", "Lettuce", "Potato", "Onion", "Broccoli", "Spinach", "Tomato", "Wheat" };
		failReason = null;
		for (int i = 0; i < array.Length; i++)
		{
			object plantDefinition = GetPlantDefinition(array[i]);
			if (plantDefinition != null)
			{
				if (WorkTile(player, tile, "PLANT", plantDefinition, 0, out var failReason2))
				{
					Mark("stage-command-plant-picked-" + array[i]);
					failReason = failReason2;
					return true;
				}
				string a = ((failReason2 == null) ? "" : failReason2.ToString());
				if (!string.Equals(a, "BuildingNeeded", StringComparison.OrdinalIgnoreCase) && !string.Equals(a, "IncorrectSeason", StringComparison.OrdinalIgnoreCase) && !string.Equals(a, "XpLevelNeeded", StringComparison.OrdinalIgnoreCase) && !string.Equals(a, "Trialmode", StringComparison.OrdinalIgnoreCase))
				{
					failReason = failReason2;
					return false;
				}
			}
		}
		failReason = "NoPlantCandidate";
		return false;
	}

	private static string ChooseStageAction(object farmData, object tile)
	{
		try
		{
			if (tile == null)
			{
				return "PLOW";
			}
			if (Convert.ToBoolean(InvokeProperty(tile, "IsEmpty")))
			{
				return "PLOW";
			}
			object target = InvokeProperty(tile, "Contents");
			object obj = InvokeProperty(target, "Type");
			switch ((obj == null) ? "" : obj.ToString())
			{
			case "Land":
			{
				object obj3 = InvokeProperty(target, "State");
				string text2 = ((obj3 == null) ? "" : obj3.ToString());
				return (text2 == "Plowed") ? "PLANT" : "PLOW";
			}
			case "Plant":
			{
				object obj4 = InvokeProperty(target, "State");
				string text3 = ((obj4 == null) ? "" : obj4.ToString());
				return (text3 == "Growing") ? "WATER" : "GATHER";
			}
			case "Tree":
				return "GATHERTREE";
			case "Animal":
			{
				object obj2 = InvokeProperty(target, "State");
				string text = ((obj2 == null) ? "" : obj2.ToString());
				return (text == "ReadyToGather") ? "GATHERANIMAL" : "FEEDANIMAL";
			}
			case "Building":
				return "GATHERBUILDING";
			case "Tool":
				return "REFILL";
			}
		}
		catch (Exception ex)
		{
			Mark("stage-command-choose-failed-" + DescribeException(ex));
		}
		return "PLOW";
	}

	private static bool WorkTile(object player, object tile, string action, object item, int rotation, out object failReason)
	{
		failReason = null;
		MethodInfo methodInfo = tile?.GetType().GetMethod("Work", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (methodInfo == null)
		{
			Mark("stage-command-no-work");
			return false;
		}
		ParameterInfo[] parameters = methodInfo.GetParameters();
		Type parameterType = parameters[1].ParameterType;
		Type elementType = parameters[4].ParameterType.GetElementType();
		object obj = Enum.Parse(parameterType, NormalizeWorkAction(action), ignoreCase: true);
		object[] array = new object[5]
		{
			player,
			obj,
			item,
			rotation,
			Enum.ToObject(elementType, 0)
		};
		object obj2 = methodInfo.Invoke(tile, array);
		failReason = array[4];
		return obj2 is bool && (bool)obj2;
	}

	private static string NormalizeWorkAction(string action)
	{
		return action switch
		{
			"PLOW" => "Plow", 
			"WATER" => "Water", 
			"GATHER" => "Gather", 
			"GATHERTREE" => "GatherTree", 
			"GATHERBUILDING" => "GatherBuilding", 
			"GATHERANIMAL" => "GatherAnimal", 
			"FEEDANIMAL" => "FeedAnimal", 
			"REFILL" => "Refill", 
			"RECYCLE" => "Recycle", 
			"PLANT" => "Plant", 
			_ => "Plow", 
		};
	}

	private static object GetPlantDefinition(string id)
	{
		try
		{
			Type type = Type.GetType("AvatarFarmOnline.Logic.Stage.ItemDefinitionManager, AvatarFarmOnline") ?? Type.GetType("AvatarFarm2.Logic.Stage.ItemDefinitionManager, AvatarFarmOnline");
			MethodInfo methodInfo = ((type == null) ? null : type.GetMethod("get_Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));
			object obj = ((methodInfo == null) ? null : methodInfo.Invoke(null, null));
			MethodInfo methodInfo2 = obj?.GetType().GetMethod("GetPlant", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			return (methodInfo2 == null) ? null : methodInfo2.Invoke(obj, new object[1] { id });
		}
		catch (Exception ex)
		{
			Mark("stage-command-plantdef-failed-" + DescribeException(ex));
			return null;
		}
	}

	private static void ClampStagePosition(object stage, ref float x, ref float y)
	{
		try
		{
			object target = InvokeProperty(stage, "FarmData");
			object obj = InvokeProperty(target, "FarmExtents");
			if (obj == null)
			{
				x = ClampFloat(0.5f, 39.5f, x);
				y = ClampFloat(0.5f, 39.5f, y);
				return;
			}
			FieldInfo field = obj.GetType().GetField("X", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			FieldInfo field2 = obj.GetType().GetField("Y", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			float num = ((field == null) ? 40f : Convert.ToSingle(field.GetValue(obj)));
			float num2 = ((field2 == null) ? 40f : Convert.ToSingle(field2.GetValue(obj)));
			x = ClampFloat(0.5f, Math.Max(0.5f, num - 0.5f), x);
			y = ClampFloat(0.5f, Math.Max(0.5f, num2 - 0.5f), y);
		}
		catch
		{
			x = ClampFloat(0.5f, 39.5f, x);
			y = ClampFloat(0.5f, 39.5f, y);
		}
	}

	private static float ClampFloat(float min, float max, float value)
	{
		if (value < min)
		{
			return min;
		}
		if (value > max)
		{
			return max;
		}
		return value;
	}

	private static void ReloadBaseGameSection()
	{
		try
		{
			if (baseGameInstance == null || baseGameMainLoop == null)
			{
				Type type = Type.GetType("Quasar.GameUtils.Game.BaseGame, QuasarGameUtils");
				if (type == null)
				{
					return;
				}
				MethodInfo method = type.GetMethod("get_Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (method == null)
				{
					return;
				}
				baseGameInstance = method.Invoke(null, null);
				baseGameMainLoop = type.GetMethod("ReloadSection", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				Mark("basegame-reload-ready");
			}
			if (baseGameInstance != null && baseGameMainLoop != null)
			{
				baseGameMainLoop.Invoke(baseGameInstance, null);
			}
		}
		catch (Exception ex)
		{
			try
			{
				File.AppendAllText(ExceptionLogPath, DateTime.UtcNow.ToString("O") + " basegame reload " + ex?.ToString() + Environment.NewLine);
			}
			catch
			{
			}
			Mark("basegame-reload-failed-" + DescribeException(ex));
		}
	}

	private static void PulseBaseGameMainLoop()
	{
		try
		{
			Type type = Type.GetType("Quasar.GameUtils.Game.BaseGame, QuasarGameUtils");
			if (!(type == null))
			{
				MethodInfo method = type.GetMethod("get_Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				object obj = ((method == null) ? null : method.Invoke(null, null));
				MethodInfo method2 = type.GetMethod("MainLoop", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (obj != null && method2 != null)
				{
					method2.Invoke(obj, null);
					LogBaseGameState(obj);
					Mark("basegame-mainloop-pulse-ok");
				}
			}
		}
		catch (Exception ex)
		{
			try
			{
				File.AppendAllText(ExceptionLogPath, DateTime.UtcNow.ToString("O") + " basegame mainloop " + ex?.ToString() + Environment.NewLine);
			}
			catch
			{
			}
			Mark("basegame-mainloop-pulse-failed-" + DescribeException(ex));
		}
	}

	private static void LogBaseGameState(object instance)
	{
		try
		{
			object obj = InvokeProperty(instance, "CurrentGameSection");
			object obj2 = InvokeProperty(instance, "NextGameSection");
			Mark("basegame-current-" + ((obj == null) ? "null" : obj.GetType().FullName));
			Mark("basegame-next-" + ((obj2 == null) ? "null" : obj2.GetType().FullName));
			LogExceptionSection(obj2);
		}
		catch
		{
		}
	}

	private static void LogExceptionSection(object section)
	{
		if (section != null && !(section.GetType().FullName != "Quasar.GameUtils.Game.ExceptionSection"))
		{
			object field = GetField(section, "message");
			object field2 = GetField(section, "stackTrace");
			try
			{
				File.AppendAllText(ExceptionLogPath, DateTime.UtcNow.ToString("O") + " exception-section " + field?.ToString() + Environment.NewLine + field2?.ToString() + Environment.NewLine);
			}
			catch
			{
			}
			Mark("exception-section-" + field);
		}
	}

	private static void DriveRegisteredLayoutCommands()
	{
		string text;
		while ((text = TryConsumeAnyCommand()) != null)
		{
			Mark("layout-direct-command-" + text);
			if (text == "UP" || text == "LEFT")
			{
				MoveRegisteredGroup(-1);
			}
			else if (text == "DOWN" || text == "RIGHT")
			{
				MoveRegisteredGroup(1);
			}
			else if (text == "ENTER" || text == "SPACE" || text == "INTERACT")
			{
				InvokeRegisteredGroup();
			}
			else if (text == "ESC" || text == "BACK" || text == "CANCEL")
			{
				CancelRegisteredLayout();
			}
		}
	}

	private static void CancelRegisteredLayout()
	{
		if (registeredLayout == null)
		{
			Mark("layout-direct-cancel-no-layout");
			return;
		}
		try
		{
			FieldInfo fieldInfo = GetFieldInfo(registeredLayout.GetType(), "OnCancel");
			Delegate obj = ((fieldInfo == null) ? null : (fieldInfo.GetValue(registeredLayout) as Delegate));
			if ((object)obj == null)
			{
				Mark("layout-direct-cancel-no-handler");
				return;
			}
			Type[] genericArguments = obj.GetType().GetGenericArguments();
			Type enumType = ((genericArguments.Length > 1) ? genericArguments[1] : typeof(PlayerIndex));
			object obj2 = Enum.ToObject(enumType, 0);
			obj.DynamicInvoke(registeredLayout, obj2);
			Mark("layout-direct-cancelled");
		}
		catch (Exception ex)
		{
			Mark("layout-direct-cancel-failed-" + DescribeException(ex));
		}
	}

	private static string PollGamePadMenuCommand()
	{
		try
		{
			GamePadState compatGamePadState = GetCompatGamePadState();
			if (!compatGamePadState.IsConnected)
			{
				return null;
			}
			if (WasGamePadButtonPressed("MENU_A", compatGamePadState.Buttons.A == ButtonState.Pressed || compatGamePadState.Buttons.Start == ButtonState.Pressed))
			{
				return "ENTER";
			}
			if (WasGamePadButtonPressed("MENU_B", compatGamePadState.Buttons.B == ButtonState.Pressed || compatGamePadState.Buttons.Back == ButtonState.Pressed))
			{
				return "ESC";
			}
			if (WasGamePadButtonPressed("MENU_UP", compatGamePadState.DPad.Up == ButtonState.Pressed || compatGamePadState.ThumbSticks.Left.Y > 0.45f))
			{
				return "UP";
			}
			if (WasGamePadButtonPressed("MENU_DOWN", compatGamePadState.DPad.Down == ButtonState.Pressed || compatGamePadState.ThumbSticks.Left.Y < -0.45f))
			{
				return "DOWN";
			}
			if (WasGamePadButtonPressed("MENU_LEFT", compatGamePadState.DPad.Left == ButtonState.Pressed || compatGamePadState.ThumbSticks.Left.X < -0.45f))
			{
				return "LEFT";
			}
			if (WasGamePadButtonPressed("MENU_RIGHT", compatGamePadState.DPad.Right == ButtonState.Pressed || compatGamePadState.ThumbSticks.Left.X > 0.45f))
			{
				return "RIGHT";
			}
		}
		catch (Exception ex)
		{
			Mark("gamepad-menu-command-failed-" + DescribeException(ex));
		}
		return null;
	}

	private static string TryConsumeAnyCommand()
	{
		try
		{
			lock (CommandLock)
			{
				if (!File.Exists(CommandPath))
				{
					return null;
				}
				string[] array = File.ReadAllLines(CommandPath);
				for (int i = 0; i < array.Length; i++)
				{
					string text = ((array[i] == null) ? null : array[i].Trim().ToUpperInvariant());
					if (!string.IsNullOrEmpty(text))
					{
						string[] array2 = new string[array.Length - 1];
						if (i > 0)
						{
							Array.Copy(array, 0, array2, 0, i);
						}
						if (i + 1 < array.Length)
						{
							Array.Copy(array, i + 1, array2, i, array.Length - i - 1);
						}
						File.WriteAllLines(CommandPath, array2);
						return text;
					}
				}
			}
		}
		catch
		{
		}
		return null;
	}

	private static object GetRegisteredGroup()
	{
		if (registeredGroup != null)
		{
			object bestRegisteredGroup = GetBestRegisteredGroup();
			if (bestRegisteredGroup != null)
			{
				return bestRegisteredGroup;
			}
			return registeredGroup;
		}
		if (registeredLayout == null)
		{
			return null;
		}
		object field = GetField(registeredLayout, "activeControl");
		if (field != null)
		{
			Mark("layout-active-type-" + field.GetType().FullName);
		}
		if (IsGroup(field))
		{
			return field;
		}
		object field2 = GetField(registeredLayout, "controlList");
		if (field2 is IEnumerable enumerable)
		{
			foreach (object item in enumerable)
			{
				if (item != null)
				{
					Mark("layout-list-type-" + item.GetType().FullName);
				}
				if (IsGroup(item))
				{
					return item;
				}
			}
		}
		object field3 = GetField(registeredLayout, "controls");
		if (field3 is IDictionary dictionary)
		{
			foreach (DictionaryEntry item2 in dictionary)
			{
				if (item2.Value != null)
				{
					Mark("layout-dict-type-" + item2.Value.GetType().FullName);
				}
				if (IsGroup(item2.Value))
				{
					return item2.Value;
				}
			}
		}
		return null;
	}

	private static object GetBestRegisteredGroup()
	{
		object obj = null;
		int num = 0;
		lock (registeredGroups)
		{
			foreach (object registeredGroup in registeredGroups)
			{
				int num2 = ((GetField(registeredGroup, "controls") is IList list) ? list.Count : 0);
				Mark("registered-group-count-" + num2);
				if (num2 > num)
				{
					obj = registeredGroup;
					num = num2;
				}
			}
		}
		return (num > 0) ? obj : null;
	}

	private static bool IsGroup(object value)
	{
		return value != null && value.GetType().FullName == "Quasar.GUI.Controls.Group";
	}

	private static void MoveRegisteredGroup(int delta)
	{
		object obj = GetRegisteredGroup();
		if (obj == null)
		{
			Mark("layout-direct-no-group");
			return;
		}
		IList list = GetField(obj, "controls") as IList;
		FieldInfo fieldInfo = GetFieldInfo(obj.GetType(), "activeControl");
		if (list == null || fieldInfo == null || list.Count == 0)
		{
			Mark("layout-direct-bad-group");
			MoveRegisteredMenuButtons(delta);
			return;
		}
		int num = (int)fieldInfo.GetValue(obj);
		int i;
		for (i = num + delta; i < 0; i += list.Count)
		{
		}
		i %= list.Count;
		SetFocused(SafeListGet(list, num), focused: false);
		fieldInfo.SetValue(obj, i);
		SetFocused(SafeListGet(list, i), focused: true);
		Mark("layout-direct-focus-" + i);
	}

	private static void InvokeRegisteredGroup()
	{
		object obj = GetRegisteredGroup();
		if (obj == null)
		{
			Mark("layout-direct-invoke-no-group");
			return;
		}
		IList list = GetField(obj, "controls") as IList;
		FieldInfo fieldInfo = GetFieldInfo(obj.GetType(), "activeControl");
		if (list == null || fieldInfo == null || list.Count == 0)
		{
			Mark("layout-direct-invoke-bad-group");
			InvokeRegisteredMenuButton();
			return;
		}
		int index = (int)fieldInfo.GetValue(obj);
		object obj2 = SafeListGet(list, index);
		if (obj2 != null)
		{
			FieldInfo fieldInfo2 = GetFieldInfo(obj2.GetType(), "OnInteraction");
			Delegate obj3 = ((fieldInfo2 == null) ? null : (fieldInfo2.GetValue(obj2) as Delegate));
			if ((object)obj3 == null)
			{
				Mark("layout-direct-no-interaction");
				return;
			}
			Type enumType = obj3.GetType().GetGenericArguments()[1];
			object obj4 = Enum.ToObject(enumType, 0);
			obj3.DynamicInvoke(obj2, obj4);
			Mark("layout-direct-invoked-" + index);
		}
	}

	private static void MoveRegisteredMenuButtons(int delta)
	{
		lock (registeredMenuButtons)
		{
			if (registeredMenuButtons.Count == 0)
			{
				Mark("menu-button-none");
				return;
			}
			SetMenuButtonFocused(activeMenuButton, focused: false);
			activeMenuButton += delta;
			while (activeMenuButton < 0)
			{
				activeMenuButton += registeredMenuButtons.Count;
			}
			activeMenuButton %= registeredMenuButtons.Count;
			SetMenuButtonFocused(activeMenuButton, focused: true);
			Mark("menu-button-focus-" + activeMenuButton);
		}
	}

	private static void InvokeRegisteredMenuButton()
	{
		lock (registeredMenuButtons)
		{
			if (registeredMenuButtons.Count == 0)
			{
				Mark("menu-button-invoke-none");
				return;
			}
			object target = registeredMenuButtons[activeMenuButton];
			object field = GetField(target, "button");
			if (field == null)
			{
				Mark("menu-button-invoke-no-button");
				return;
			}
			FieldInfo fieldInfo = GetFieldInfo(field.GetType(), "OnInteraction");
			Delegate obj = ((fieldInfo == null) ? null : (fieldInfo.GetValue(field) as Delegate));
			if ((object)obj == null)
			{
				Mark("menu-button-invoke-no-interaction");
				return;
			}
			Type enumType = obj.GetType().GetGenericArguments()[1];
			object obj2 = Enum.ToObject(enumType, 0);
			obj.DynamicInvoke(field, obj2);
			Mark("menu-button-invoked-" + activeMenuButton);
		}
	}

	private static void SetMenuButtonFocused(int index, bool focused)
	{
		if (index >= 0 && index < registeredMenuButtons.Count)
		{
			object field = GetField(registeredMenuButtons[index], "button");
			SetFocused(field, focused);
			MethodInfo method = registeredMenuButtons[index].GetType().GetMethod("DoUpdate", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method != null)
			{
				method.Invoke(registeredMenuButtons[index], null);
			}
			SetMenuButtonTextColor(registeredMenuButtons[index], focused);
		}
	}

	private static void SetMenuButtonTextColor(object item, bool focused)
	{
		try
		{
			object field = GetField(item, "text");
			if (field == null)
			{
				return;
			}
			MethodInfo method = field.GetType().GetMethod("get_FirstMaterial", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			object obj = ((method == null) ? null : method.Invoke(field, null));
			if (obj == null)
			{
				return;
			}
			Type type = Type.GetType("Quasar.Global.GameMath, Quasar");
			if (type == null)
			{
				return;
			}
			MethodInfo method2 = type.GetMethod("RGBToVector", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[3]
			{
				typeof(byte),
				typeof(byte),
				typeof(byte)
			}, null);
			if (!(method2 == null))
			{
				object value = (focused ? method2.Invoke(null, new object[3]
				{
					byte.MaxValue,
					byte.MaxValue,
					byte.MaxValue
				}) : method2.Invoke(null, new object[3]
				{
					(byte)150,
					(byte)220,
					(byte)60
				}));
				FieldInfo fieldInfo = GetFieldInfo(obj.GetType(), "Diffuse");
				if (fieldInfo != null)
				{
					fieldInfo.SetValue(obj, value);
				}
			}
		}
		catch (Exception ex)
		{
			Mark("menu-button-color-failed-" + ex.GetType().Name);
		}
	}

	private static object SafeListGet(IList list, int index)
	{
		if (list == null || index < 0 || index >= list.Count)
		{
			return null;
		}
		return list[index];
	}

	private static MethodInfo FindSingleArgMethod(Type type, string name)
	{
		if (type == null)
		{
			return null;
		}
		MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (MethodInfo methodInfo in methods)
		{
			if (string.Equals(methodInfo.Name, name, StringComparison.Ordinal))
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
				if (parameters != null && parameters.Length == 1)
				{
					return methodInfo;
				}
			}
		}
		return null;
	}

	private static void SetFocused(object control, bool focused)
	{
		if (control == null)
		{
			return;
		}
		MethodInfo method = control.GetType().GetMethod("set_Focused", new Type[1] { typeof(bool) });
		if (method != null)
		{
			method.Invoke(control, new object[1] { focused });
			return;
		}
		FieldInfo fieldInfo = GetFieldInfo(control.GetType(), "focused");
		if (fieldInfo != null)
		{
			fieldInfo.SetValue(control, focused);
		}
	}

	private static object GetField(object target, string name)
	{
		if (target == null)
		{
			return null;
		}
		FieldInfo fieldInfo = GetFieldInfo(target.GetType(), name);
		return (fieldInfo == null) ? null : fieldInfo.GetValue(target);
	}

	private static FieldInfo GetFieldInfo(Type type, string name)
	{
		while (type != null)
		{
			FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field != null)
			{
				return field;
			}
			type = type.BaseType;
		}
		return null;
	}

	private static void CaptureBackbufferOnce(string name)
	{
		if (Interlocked.Exchange(ref Captured, 1) != 0)
		{
			return;
		}
		try
		{
			Directory.CreateDirectory(Path.GetDirectoryName(LogPath));
			int[] array = new int[4];
			glGetIntegerv(2978u, array);
			int num = array[2];
			int num2 = array[3];
			if (num <= 0 || num2 <= 0 || num > 8192 || num2 > 8192)
			{
				return;
			}
			byte[] array2 = new byte[num * num2 * 4];
			glReadPixels(0, 0, num, num2, 6408u, 5121u, array2);
			using Bitmap bitmap = new Bitmap(num, num2, PixelFormat.Format32bppArgb);
			for (int i = 0; i < num2; i++)
			{
				int num3 = num2 - 1 - i;
				for (int j = 0; j < num; j++)
				{
					int num4 = (num3 * num + j) * 4;
					bitmap.SetPixel(j, i, System.Drawing.Color.FromArgb(array2[num4 + 3], array2[num4], array2[num4 + 1], array2[num4 + 2]));
				}
			}
			bitmap.Save(Path.Combine(RuntimeDataDir, "internal-backbuffer.png"), ImageFormat.Png);
		}
		catch (Exception ex)
		{
			File.AppendAllText(LogPath, DateTime.UtcNow.ToString("O") + " capture failed " + ex?.ToString() + Environment.NewLine);
		}
	}
}
