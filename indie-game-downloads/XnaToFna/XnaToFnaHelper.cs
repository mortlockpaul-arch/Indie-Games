using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using XnaToFna.ProxyForms;

namespace XnaToFna;

public static class XnaToFnaHelper
{
	public static XnaToFnaGame Game;

	public static int MaximumGamepadCount;

	public static MulticastDelegate fna_ApplyWindowChanges;

	public static void MainHook(string[] args)
	{
	}

	public static void Initialize(XnaToFnaGame game)
	{
		Game = game;
		TextInputEXT.TextInput += KeyboardEvents.CharEntered;
		if (Environment.GetEnvironmentVariable("FNADROID") != "1")
		{
			TextInputEXT.StartTextInput();
		}
		game.Window.ClientSizeChanged += SDLWindowSizeChanged;
		string environmentVariable = Environment.GetEnvironmentVariable("FNA_GAMEPAD_NUM_GAMEPADS");
		if (string.IsNullOrEmpty(environmentVariable) || !int.TryParse(environmentVariable, out MaximumGamepadCount) || MaximumGamepadCount < 0)
		{
			MaximumGamepadCount = Enum.GetNames(typeof(PlayerIndex)).Length;
		}
		DeviceEvents.IsGamepadConnected = new bool[MaximumGamepadCount];
		PlatformHook("ApplyWindowChanges");
	}

	public static void Log(string s)
	{
		Console.Write("[XnaToFnaHelper] ");
		Console.WriteLine(s);
	}

	public static IntPtr GetProxyFormHandle(this GameWindow window)
	{
		if (GameForm.Instance == null)
		{
			Log("[ProxyForms] Creating game ProxyForms.GameForm");
			GameForm.Instance = new GameForm();
		}
		return GameForm.Instance.Handle;
	}

	public static bool get_IsTrialMode()
	{
		return Environment.GetEnvironmentVariable("XNATOFNA_ISTRIALMODE") != "0";
	}

	public static void ApplyChanges(GraphicsDeviceManager self)
	{
		string environmentVariable = Environment.GetEnvironmentVariable("XNATOFNA_DISPLAY_FULLSCREEN");
		if (environmentVariable == "0")
		{
			self.IsFullScreen = false;
		}
		else if (environmentVariable == "1")
		{
			self.IsFullScreen = true;
		}
		if (int.TryParse(Environment.GetEnvironmentVariable("XNATOFNA_DISPLAY_WIDTH") ?? "", out var result))
		{
			self.PreferredBackBufferWidth = result;
		}
		if (int.TryParse(Environment.GetEnvironmentVariable("XNATOFNA_DISPLAY_HEIGHT") ?? "", out var result2))
		{
			self.PreferredBackBufferHeight = result2;
		}
		string[] array = (Environment.GetEnvironmentVariable("XNATOFNA_DISPLAY_SIZE") ?? "").Split('x');
		if (array.Length == 2)
		{
			if (int.TryParse(array[0], out result))
			{
				self.PreferredBackBufferWidth = result;
			}
			if (int.TryParse(array[1], out result2))
			{
				self.PreferredBackBufferHeight = result2;
			}
		}
		self.ApplyChanges();
	}

	public static void PreUpdate(GameTime time)
	{
		KeyboardEvents.Update();
		MouseEvents.Update();
		DeviceEvents.Update();
	}

	public static T GetService<T>() where T : class
	{
		return (T)Game.Services.GetService(typeof(T));
	}

	public static B GetService<A, B>() where A : class where B : class, A
	{
		return Game.Services.GetService(typeof(A)) as B;
	}

	public static void PlatformHook(string name)
	{
		Type typeFromHandle = typeof(XnaToFnaHelper);
		Assembly assembly = Assembly.GetAssembly(typeof(Game));
		FieldInfo field = assembly.GetType("Microsoft.Xna.Framework.FNAPlatform").GetField(name);
		typeFromHandle.GetField($"fna_{name}").SetValue(null, field.GetValue(null));
		field.SetValue(null, Delegate.CreateDelegate(assembly.GetType($"Microsoft.Xna.Framework.FNAPlatform+{name}Func"), typeFromHandle.GetMethod(name)));
	}

	public static void SDLWindowSizeChanged(object sender, EventArgs e)
	{
		GameForm.Instance?.SDLWindowSizeChanged(sender, e);
	}

	public static void ApplyWindowChanges(IntPtr window, int clientWidth, int clientHeight, bool wantsFullscreen, string screenDeviceName, ref string resultDeviceName)
	{
		object[] array = new object[6] { window, clientWidth, clientHeight, wantsFullscreen, screenDeviceName, resultDeviceName };
		fna_ApplyWindowChanges.DynamicInvoke(array);
		resultDeviceName = (string)array[5];
		GameForm.Instance?.SDLWindowChanged(window, clientWidth, clientHeight, wantsFullscreen, screenDeviceName, ref resultDeviceName);
		_ = resultDeviceName != screenDeviceName;
	}
}
