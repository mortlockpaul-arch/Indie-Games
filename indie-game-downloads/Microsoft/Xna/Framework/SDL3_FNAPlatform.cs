using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Utilities;
using ObjCRuntime;
using SDL3;

namespace Microsoft.Xna.Framework;

internal static class SDL3_FNAPlatform
{
	private delegate void em_callback_func();

	private static string OSVersion;

	private static readonly bool UseScancodes = Environment.GetEnvironmentVariable("FNA_KEYBOARD_USE_SCANCODES") == "1";

	private static bool SupportsGlobalMouse;

	private static bool SupportsOrientations;

	private static List<Game> activeGames = new List<Game>();

	private static Game emscriptenGame;

	private static uint[] displayIds;

	private static bool micInit = false;

	private static nint[] INTERNAL_devices = new nint[GamePad.GAMEPAD_COUNT];

	private static Dictionary<uint, int> INTERNAL_instanceList = new Dictionary<uint, int>();

	private static string[] INTERNAL_guids = GenStringArray();

	private static GamePadState[] INTERNAL_states = new GamePadState[GamePad.GAMEPAD_COUNT];

	private static GamePadCapabilities[] INTERNAL_capabilities = new GamePadCapabilities[GamePad.GAMEPAD_COUNT];

	private static readonly GamePadType[] INTERNAL_gamepadType = new GamePadType[9]
	{
		GamePadType.Unknown,
		GamePadType.GamePad,
		GamePadType.Wheel,
		GamePadType.ArcadeStick,
		GamePadType.FlightStick,
		GamePadType.DancePad,
		GamePadType.Guitar,
		GamePadType.DrumKit,
		GamePadType.BigButtonPad
	};

	private static Dictionary<int, Keys> INTERNAL_keyMap = new Dictionary<int, Keys>
	{
		{
			97,
			Keys.A
		},
		{
			98,
			Keys.B
		},
		{
			99,
			Keys.C
		},
		{
			100,
			Keys.D
		},
		{
			101,
			Keys.E
		},
		{
			102,
			Keys.F
		},
		{
			103,
			Keys.G
		},
		{
			104,
			Keys.H
		},
		{
			105,
			Keys.I
		},
		{
			106,
			Keys.J
		},
		{
			107,
			Keys.K
		},
		{
			108,
			Keys.L
		},
		{
			109,
			Keys.M
		},
		{
			110,
			Keys.N
		},
		{
			111,
			Keys.O
		},
		{
			112,
			Keys.P
		},
		{
			113,
			Keys.Q
		},
		{
			114,
			Keys.R
		},
		{
			115,
			Keys.S
		},
		{
			116,
			Keys.T
		},
		{
			117,
			Keys.U
		},
		{
			118,
			Keys.V
		},
		{
			119,
			Keys.W
		},
		{
			120,
			Keys.X
		},
		{
			121,
			Keys.Y
		},
		{
			122,
			Keys.Z
		},
		{
			48,
			Keys.D0
		},
		{
			49,
			Keys.D1
		},
		{
			50,
			Keys.D2
		},
		{
			51,
			Keys.D3
		},
		{
			52,
			Keys.D4
		},
		{
			53,
			Keys.D5
		},
		{
			54,
			Keys.D6
		},
		{
			55,
			Keys.D7
		},
		{
			56,
			Keys.D8
		},
		{
			57,
			Keys.D9
		},
		{
			1073741922,
			Keys.NumPad0
		},
		{
			1073741913,
			Keys.NumPad1
		},
		{
			1073741914,
			Keys.NumPad2
		},
		{
			1073741915,
			Keys.NumPad3
		},
		{
			1073741916,
			Keys.NumPad4
		},
		{
			1073741917,
			Keys.NumPad5
		},
		{
			1073741918,
			Keys.NumPad6
		},
		{
			1073741919,
			Keys.NumPad7
		},
		{
			1073741920,
			Keys.NumPad8
		},
		{
			1073741921,
			Keys.NumPad9
		},
		{
			1073742040,
			Keys.OemClear
		},
		{
			1073741908,
			Keys.Divide
		},
		{
			1073741912,
			Keys.Enter
		},
		{
			1073741910,
			Keys.Subtract
		},
		{
			1073741909,
			Keys.Multiply
		},
		{
			1073741923,
			Keys.Decimal
		},
		{
			1073741911,
			Keys.Add
		},
		{
			1073741882,
			Keys.F1
		},
		{
			1073741883,
			Keys.F2
		},
		{
			1073741884,
			Keys.F3
		},
		{
			1073741885,
			Keys.F4
		},
		{
			1073741886,
			Keys.F5
		},
		{
			1073741887,
			Keys.F6
		},
		{
			1073741888,
			Keys.F7
		},
		{
			1073741889,
			Keys.F8
		},
		{
			1073741890,
			Keys.F9
		},
		{
			1073741891,
			Keys.F10
		},
		{
			1073741892,
			Keys.F11
		},
		{
			1073741893,
			Keys.F12
		},
		{
			1073741928,
			Keys.F13
		},
		{
			1073741929,
			Keys.F14
		},
		{
			1073741930,
			Keys.F15
		},
		{
			1073741931,
			Keys.F16
		},
		{
			1073741932,
			Keys.F17
		},
		{
			1073741933,
			Keys.F18
		},
		{
			1073741934,
			Keys.F19
		},
		{
			1073741935,
			Keys.F20
		},
		{
			1073741936,
			Keys.F21
		},
		{
			1073741937,
			Keys.F22
		},
		{
			1073741938,
			Keys.F23
		},
		{
			1073741939,
			Keys.F24
		},
		{
			32,
			Keys.Space
		},
		{
			1073741906,
			Keys.Up
		},
		{
			1073741905,
			Keys.Down
		},
		{
			1073741904,
			Keys.Left
		},
		{
			1073741903,
			Keys.Right
		},
		{
			1073742050,
			Keys.LeftAlt
		},
		{
			1073742054,
			Keys.RightAlt
		},
		{
			1073742048,
			Keys.LeftControl
		},
		{
			1073742052,
			Keys.RightControl
		},
		{
			1073742051,
			Keys.LeftWindows
		},
		{
			1073742055,
			Keys.RightWindows
		},
		{
			1073742049,
			Keys.LeftShift
		},
		{
			1073742053,
			Keys.RightShift
		},
		{
			1073741925,
			Keys.Apps
		},
		{
			1073741942,
			Keys.Apps
		},
		{
			47,
			Keys.OemQuestion
		},
		{
			92,
			Keys.OemPipe
		},
		{
			91,
			Keys.OemOpenBrackets
		},
		{
			93,
			Keys.OemCloseBrackets
		},
		{
			1073741881,
			Keys.CapsLock
		},
		{
			44,
			Keys.OemComma
		},
		{
			127,
			Keys.Delete
		},
		{
			1073741901,
			Keys.End
		},
		{
			8,
			Keys.Back
		},
		{
			13,
			Keys.Enter
		},
		{
			27,
			Keys.Escape
		},
		{
			1073741898,
			Keys.Home
		},
		{
			1073741897,
			Keys.Insert
		},
		{
			45,
			Keys.OemMinus
		},
		{
			1073741907,
			Keys.NumLock
		},
		{
			1073741899,
			Keys.PageUp
		},
		{
			1073741902,
			Keys.PageDown
		},
		{
			1073741896,
			Keys.Pause
		},
		{
			46,
			Keys.OemPeriod
		},
		{
			61,
			Keys.OemPlus
		},
		{
			1073741894,
			Keys.PrintScreen
		},
		{
			39,
			Keys.OemQuotes
		},
		{
			1073741895,
			Keys.Scroll
		},
		{
			59,
			Keys.OemSemicolon
		},
		{
			1073742082,
			Keys.Sleep
		},
		{
			9,
			Keys.Tab
		},
		{
			96,
			Keys.OemTilde
		},
		{
			1073741951,
			Keys.VolumeMute
		},
		{
			1073741952,
			Keys.VolumeUp
		},
		{
			1073741953,
			Keys.VolumeDown
		},
		{
			60,
			Keys.OemBackslash
		},
		{
			178,
			Keys.OemTilde
		},
		{
			233,
			Keys.None
		},
		{
			124,
			Keys.OemPipe
		},
		{
			43,
			Keys.OemPlus
		},
		{
			248,
			Keys.OemSemicolon
		},
		{
			230,
			Keys.OemQuotes
		},
		{
			0,
			Keys.None
		}
	};

	private static Dictionary<int, Keys> INTERNAL_scanMap = new Dictionary<int, Keys>
	{
		{
			4,
			Keys.A
		},
		{
			5,
			Keys.B
		},
		{
			6,
			Keys.C
		},
		{
			7,
			Keys.D
		},
		{
			8,
			Keys.E
		},
		{
			9,
			Keys.F
		},
		{
			10,
			Keys.G
		},
		{
			11,
			Keys.H
		},
		{
			12,
			Keys.I
		},
		{
			13,
			Keys.J
		},
		{
			14,
			Keys.K
		},
		{
			15,
			Keys.L
		},
		{
			16,
			Keys.M
		},
		{
			17,
			Keys.N
		},
		{
			18,
			Keys.O
		},
		{
			19,
			Keys.P
		},
		{
			20,
			Keys.Q
		},
		{
			21,
			Keys.R
		},
		{
			22,
			Keys.S
		},
		{
			23,
			Keys.T
		},
		{
			24,
			Keys.U
		},
		{
			25,
			Keys.V
		},
		{
			26,
			Keys.W
		},
		{
			27,
			Keys.X
		},
		{
			28,
			Keys.Y
		},
		{
			29,
			Keys.Z
		},
		{
			39,
			Keys.D0
		},
		{
			30,
			Keys.D1
		},
		{
			31,
			Keys.D2
		},
		{
			32,
			Keys.D3
		},
		{
			33,
			Keys.D4
		},
		{
			34,
			Keys.D5
		},
		{
			35,
			Keys.D6
		},
		{
			36,
			Keys.D7
		},
		{
			37,
			Keys.D8
		},
		{
			38,
			Keys.D9
		},
		{
			98,
			Keys.NumPad0
		},
		{
			89,
			Keys.NumPad1
		},
		{
			90,
			Keys.NumPad2
		},
		{
			91,
			Keys.NumPad3
		},
		{
			92,
			Keys.NumPad4
		},
		{
			93,
			Keys.NumPad5
		},
		{
			94,
			Keys.NumPad6
		},
		{
			95,
			Keys.NumPad7
		},
		{
			96,
			Keys.NumPad8
		},
		{
			97,
			Keys.NumPad9
		},
		{
			216,
			Keys.OemClear
		},
		{
			84,
			Keys.Divide
		},
		{
			88,
			Keys.Enter
		},
		{
			86,
			Keys.Subtract
		},
		{
			85,
			Keys.Multiply
		},
		{
			99,
			Keys.Decimal
		},
		{
			87,
			Keys.Add
		},
		{
			58,
			Keys.F1
		},
		{
			59,
			Keys.F2
		},
		{
			60,
			Keys.F3
		},
		{
			61,
			Keys.F4
		},
		{
			62,
			Keys.F5
		},
		{
			63,
			Keys.F6
		},
		{
			64,
			Keys.F7
		},
		{
			65,
			Keys.F8
		},
		{
			66,
			Keys.F9
		},
		{
			67,
			Keys.F10
		},
		{
			68,
			Keys.F11
		},
		{
			69,
			Keys.F12
		},
		{
			104,
			Keys.F13
		},
		{
			105,
			Keys.F14
		},
		{
			106,
			Keys.F15
		},
		{
			107,
			Keys.F16
		},
		{
			108,
			Keys.F17
		},
		{
			109,
			Keys.F18
		},
		{
			110,
			Keys.F19
		},
		{
			111,
			Keys.F20
		},
		{
			112,
			Keys.F21
		},
		{
			113,
			Keys.F22
		},
		{
			114,
			Keys.F23
		},
		{
			115,
			Keys.F24
		},
		{
			44,
			Keys.Space
		},
		{
			82,
			Keys.Up
		},
		{
			81,
			Keys.Down
		},
		{
			80,
			Keys.Left
		},
		{
			79,
			Keys.Right
		},
		{
			226,
			Keys.LeftAlt
		},
		{
			230,
			Keys.RightAlt
		},
		{
			224,
			Keys.LeftControl
		},
		{
			228,
			Keys.RightControl
		},
		{
			227,
			Keys.LeftWindows
		},
		{
			231,
			Keys.RightWindows
		},
		{
			225,
			Keys.LeftShift
		},
		{
			229,
			Keys.RightShift
		},
		{
			101,
			Keys.Apps
		},
		{
			118,
			Keys.Apps
		},
		{
			56,
			Keys.OemQuestion
		},
		{
			49,
			Keys.OemPipe
		},
		{
			47,
			Keys.OemOpenBrackets
		},
		{
			48,
			Keys.OemCloseBrackets
		},
		{
			57,
			Keys.CapsLock
		},
		{
			54,
			Keys.OemComma
		},
		{
			76,
			Keys.Delete
		},
		{
			77,
			Keys.End
		},
		{
			42,
			Keys.Back
		},
		{
			40,
			Keys.Enter
		},
		{
			41,
			Keys.Escape
		},
		{
			74,
			Keys.Home
		},
		{
			73,
			Keys.Insert
		},
		{
			45,
			Keys.OemMinus
		},
		{
			83,
			Keys.NumLock
		},
		{
			75,
			Keys.PageUp
		},
		{
			78,
			Keys.PageDown
		},
		{
			72,
			Keys.Pause
		},
		{
			55,
			Keys.OemPeriod
		},
		{
			46,
			Keys.OemPlus
		},
		{
			70,
			Keys.PrintScreen
		},
		{
			52,
			Keys.OemQuotes
		},
		{
			71,
			Keys.Scroll
		},
		{
			51,
			Keys.OemSemicolon
		},
		{
			258,
			Keys.Sleep
		},
		{
			43,
			Keys.Tab
		},
		{
			53,
			Keys.OemTilde
		},
		{
			127,
			Keys.VolumeMute
		},
		{
			128,
			Keys.VolumeUp
		},
		{
			129,
			Keys.VolumeDown
		},
		{
			100,
			Keys.OemBackslash
		},
		{
			0,
			Keys.None
		},
		{
			50,
			Keys.None
		}
	};

	private static Dictionary<int, SDL.SDL_Scancode> INTERNAL_xnaMap = new Dictionary<int, SDL.SDL_Scancode>
	{
		{
			65,
			SDL.SDL_Scancode.SDL_SCANCODE_A
		},
		{
			66,
			SDL.SDL_Scancode.SDL_SCANCODE_B
		},
		{
			67,
			SDL.SDL_Scancode.SDL_SCANCODE_C
		},
		{
			68,
			SDL.SDL_Scancode.SDL_SCANCODE_D
		},
		{
			69,
			SDL.SDL_Scancode.SDL_SCANCODE_E
		},
		{
			70,
			SDL.SDL_Scancode.SDL_SCANCODE_F
		},
		{
			71,
			SDL.SDL_Scancode.SDL_SCANCODE_G
		},
		{
			72,
			SDL.SDL_Scancode.SDL_SCANCODE_H
		},
		{
			73,
			SDL.SDL_Scancode.SDL_SCANCODE_I
		},
		{
			74,
			SDL.SDL_Scancode.SDL_SCANCODE_J
		},
		{
			75,
			SDL.SDL_Scancode.SDL_SCANCODE_K
		},
		{
			76,
			SDL.SDL_Scancode.SDL_SCANCODE_L
		},
		{
			77,
			SDL.SDL_Scancode.SDL_SCANCODE_M
		},
		{
			78,
			SDL.SDL_Scancode.SDL_SCANCODE_N
		},
		{
			79,
			SDL.SDL_Scancode.SDL_SCANCODE_O
		},
		{
			80,
			SDL.SDL_Scancode.SDL_SCANCODE_P
		},
		{
			81,
			SDL.SDL_Scancode.SDL_SCANCODE_Q
		},
		{
			82,
			SDL.SDL_Scancode.SDL_SCANCODE_R
		},
		{
			83,
			SDL.SDL_Scancode.SDL_SCANCODE_S
		},
		{
			84,
			SDL.SDL_Scancode.SDL_SCANCODE_T
		},
		{
			85,
			SDL.SDL_Scancode.SDL_SCANCODE_U
		},
		{
			86,
			SDL.SDL_Scancode.SDL_SCANCODE_V
		},
		{
			87,
			SDL.SDL_Scancode.SDL_SCANCODE_W
		},
		{
			88,
			SDL.SDL_Scancode.SDL_SCANCODE_X
		},
		{
			89,
			SDL.SDL_Scancode.SDL_SCANCODE_Y
		},
		{
			90,
			SDL.SDL_Scancode.SDL_SCANCODE_Z
		},
		{
			48,
			SDL.SDL_Scancode.SDL_SCANCODE_0
		},
		{
			49,
			SDL.SDL_Scancode.SDL_SCANCODE_1
		},
		{
			50,
			SDL.SDL_Scancode.SDL_SCANCODE_2
		},
		{
			51,
			SDL.SDL_Scancode.SDL_SCANCODE_3
		},
		{
			52,
			SDL.SDL_Scancode.SDL_SCANCODE_4
		},
		{
			53,
			SDL.SDL_Scancode.SDL_SCANCODE_5
		},
		{
			54,
			SDL.SDL_Scancode.SDL_SCANCODE_6
		},
		{
			55,
			SDL.SDL_Scancode.SDL_SCANCODE_7
		},
		{
			56,
			SDL.SDL_Scancode.SDL_SCANCODE_8
		},
		{
			57,
			SDL.SDL_Scancode.SDL_SCANCODE_9
		},
		{
			96,
			SDL.SDL_Scancode.SDL_SCANCODE_KP_0
		},
		{
			97,
			SDL.SDL_Scancode.SDL_SCANCODE_KP_1
		},
		{
			98,
			SDL.SDL_Scancode.SDL_SCANCODE_KP_2
		},
		{
			99,
			SDL.SDL_Scancode.SDL_SCANCODE_KP_3
		},
		{
			100,
			SDL.SDL_Scancode.SDL_SCANCODE_KP_4
		},
		{
			101,
			SDL.SDL_Scancode.SDL_SCANCODE_KP_5
		},
		{
			102,
			SDL.SDL_Scancode.SDL_SCANCODE_KP_6
		},
		{
			103,
			SDL.SDL_Scancode.SDL_SCANCODE_KP_7
		},
		{
			104,
			SDL.SDL_Scancode.SDL_SCANCODE_KP_8
		},
		{
			105,
			SDL.SDL_Scancode.SDL_SCANCODE_KP_9
		},
		{
			254,
			SDL.SDL_Scancode.SDL_SCANCODE_KP_CLEAR
		},
		{
			110,
			SDL.SDL_Scancode.SDL_SCANCODE_KP_PERIOD
		},
		{
			111,
			SDL.SDL_Scancode.SDL_SCANCODE_KP_DIVIDE
		},
		{
			106,
			SDL.SDL_Scancode.SDL_SCANCODE_KP_MULTIPLY
		},
		{
			109,
			SDL.SDL_Scancode.SDL_SCANCODE_KP_MINUS
		},
		{
			107,
			SDL.SDL_Scancode.SDL_SCANCODE_KP_PLUS
		},
		{
			112,
			SDL.SDL_Scancode.SDL_SCANCODE_F1
		},
		{
			113,
			SDL.SDL_Scancode.SDL_SCANCODE_F2
		},
		{
			114,
			SDL.SDL_Scancode.SDL_SCANCODE_F3
		},
		{
			115,
			SDL.SDL_Scancode.SDL_SCANCODE_F4
		},
		{
			116,
			SDL.SDL_Scancode.SDL_SCANCODE_F5
		},
		{
			117,
			SDL.SDL_Scancode.SDL_SCANCODE_F6
		},
		{
			118,
			SDL.SDL_Scancode.SDL_SCANCODE_F7
		},
		{
			119,
			SDL.SDL_Scancode.SDL_SCANCODE_F8
		},
		{
			120,
			SDL.SDL_Scancode.SDL_SCANCODE_F9
		},
		{
			121,
			SDL.SDL_Scancode.SDL_SCANCODE_F10
		},
		{
			122,
			SDL.SDL_Scancode.SDL_SCANCODE_F11
		},
		{
			123,
			SDL.SDL_Scancode.SDL_SCANCODE_F12
		},
		{
			124,
			SDL.SDL_Scancode.SDL_SCANCODE_F13
		},
		{
			125,
			SDL.SDL_Scancode.SDL_SCANCODE_F14
		},
		{
			126,
			SDL.SDL_Scancode.SDL_SCANCODE_F15
		},
		{
			127,
			SDL.SDL_Scancode.SDL_SCANCODE_F16
		},
		{
			128,
			SDL.SDL_Scancode.SDL_SCANCODE_F17
		},
		{
			129,
			SDL.SDL_Scancode.SDL_SCANCODE_F18
		},
		{
			130,
			SDL.SDL_Scancode.SDL_SCANCODE_F19
		},
		{
			131,
			SDL.SDL_Scancode.SDL_SCANCODE_F20
		},
		{
			132,
			SDL.SDL_Scancode.SDL_SCANCODE_F21
		},
		{
			133,
			SDL.SDL_Scancode.SDL_SCANCODE_F22
		},
		{
			134,
			SDL.SDL_Scancode.SDL_SCANCODE_F23
		},
		{
			135,
			SDL.SDL_Scancode.SDL_SCANCODE_F24
		},
		{
			32,
			SDL.SDL_Scancode.SDL_SCANCODE_SPACE
		},
		{
			38,
			SDL.SDL_Scancode.SDL_SCANCODE_UP
		},
		{
			40,
			SDL.SDL_Scancode.SDL_SCANCODE_DOWN
		},
		{
			37,
			SDL.SDL_Scancode.SDL_SCANCODE_LEFT
		},
		{
			39,
			SDL.SDL_Scancode.SDL_SCANCODE_RIGHT
		},
		{
			164,
			SDL.SDL_Scancode.SDL_SCANCODE_LALT
		},
		{
			165,
			SDL.SDL_Scancode.SDL_SCANCODE_RALT
		},
		{
			162,
			SDL.SDL_Scancode.SDL_SCANCODE_LCTRL
		},
		{
			163,
			SDL.SDL_Scancode.SDL_SCANCODE_RCTRL
		},
		{
			91,
			SDL.SDL_Scancode.SDL_SCANCODE_LGUI
		},
		{
			92,
			SDL.SDL_Scancode.SDL_SCANCODE_RGUI
		},
		{
			160,
			SDL.SDL_Scancode.SDL_SCANCODE_LSHIFT
		},
		{
			161,
			SDL.SDL_Scancode.SDL_SCANCODE_RSHIFT
		},
		{
			93,
			SDL.SDL_Scancode.SDL_SCANCODE_APPLICATION
		},
		{
			191,
			SDL.SDL_Scancode.SDL_SCANCODE_SLASH
		},
		{
			220,
			SDL.SDL_Scancode.SDL_SCANCODE_BACKSLASH
		},
		{
			219,
			SDL.SDL_Scancode.SDL_SCANCODE_LEFTBRACKET
		},
		{
			221,
			SDL.SDL_Scancode.SDL_SCANCODE_RIGHTBRACKET
		},
		{
			20,
			SDL.SDL_Scancode.SDL_SCANCODE_CAPSLOCK
		},
		{
			188,
			SDL.SDL_Scancode.SDL_SCANCODE_COMMA
		},
		{
			46,
			SDL.SDL_Scancode.SDL_SCANCODE_DELETE
		},
		{
			35,
			SDL.SDL_Scancode.SDL_SCANCODE_END
		},
		{
			8,
			SDL.SDL_Scancode.SDL_SCANCODE_BACKSPACE
		},
		{
			13,
			SDL.SDL_Scancode.SDL_SCANCODE_RETURN
		},
		{
			27,
			SDL.SDL_Scancode.SDL_SCANCODE_ESCAPE
		},
		{
			36,
			SDL.SDL_Scancode.SDL_SCANCODE_HOME
		},
		{
			45,
			SDL.SDL_Scancode.SDL_SCANCODE_INSERT
		},
		{
			189,
			SDL.SDL_Scancode.SDL_SCANCODE_MINUS
		},
		{
			144,
			SDL.SDL_Scancode.SDL_SCANCODE_NUMLOCKCLEAR
		},
		{
			33,
			SDL.SDL_Scancode.SDL_SCANCODE_PAGEUP
		},
		{
			34,
			SDL.SDL_Scancode.SDL_SCANCODE_PAGEDOWN
		},
		{
			19,
			SDL.SDL_Scancode.SDL_SCANCODE_PAUSE
		},
		{
			190,
			SDL.SDL_Scancode.SDL_SCANCODE_PERIOD
		},
		{
			187,
			SDL.SDL_Scancode.SDL_SCANCODE_EQUALS
		},
		{
			44,
			SDL.SDL_Scancode.SDL_SCANCODE_PRINTSCREEN
		},
		{
			222,
			SDL.SDL_Scancode.SDL_SCANCODE_APOSTROPHE
		},
		{
			145,
			SDL.SDL_Scancode.SDL_SCANCODE_SCROLLLOCK
		},
		{
			186,
			SDL.SDL_Scancode.SDL_SCANCODE_SEMICOLON
		},
		{
			95,
			SDL.SDL_Scancode.SDL_SCANCODE_SLEEP
		},
		{
			9,
			SDL.SDL_Scancode.SDL_SCANCODE_TAB
		},
		{
			192,
			SDL.SDL_Scancode.SDL_SCANCODE_GRAVE
		},
		{
			173,
			SDL.SDL_Scancode.SDL_SCANCODE_MUTE
		},
		{
			175,
			SDL.SDL_Scancode.SDL_SCANCODE_VOLUMEUP
		},
		{
			174,
			SDL.SDL_Scancode.SDL_SCANCODE_VOLUMEDOWN
		},
		{
			226,
			SDL.SDL_Scancode.SDL_SCANCODE_NONUSBACKSLASH
		},
		{
			0,
			SDL.SDL_Scancode.SDL_SCANCODE_UNKNOWN
		}
	};

	private unsafe static SDL.SDL_EventFilter win32OnPaint = Win32OnPaint;

	private static SDL.SDL_EventFilter prevEventFilter;

	public static string ProgramInit(LaunchParameters args)
	{
		try
		{
			OSVersion = SDL.SDL_GetPlatform();
		}
		catch (DllNotFoundException)
		{
			FNALoggerEXT.LogError("SDL3 was not found! Do you have fnalibs?");
			throw;
		}
		catch (BadImageFormatException inner)
		{
			string text = string.Format("This process is {0}-bit, the DLL is {1}-bit!", (IntPtr.Size == 4) ? "32" : "64", (IntPtr.Size == 4) ? "64" : "32");
			FNALoggerEXT.LogError(text);
			throw new BadImageFormatException(text, inner);
		}
		SDL.SDL_SetMainReady();
		string baseDirectory = GetBaseDirectory();
		string text2 = Path.Combine(baseDirectory, "gamecontrollerdb.txt");
		if (File.Exists(text2))
		{
			SDL.SDL_SetHint("SDL_GAMECONTROLLERCONFIG_FILE", text2);
		}
		if (Environment.GetEnvironmentVariable("FNA_NUKE_STEAM_INPUT") == "1")
		{
			SDL.SDL_SetHintWithPriority("SDL_GAMECONTROLLER_IGNORE_DEVICES", "0x28DE/0x11FF", SDL.SDL_HintPriority.SDL_HINT_OVERRIDE);
			SDL.SDL_SetHintWithPriority("SDL_GAMECONTROLLER_IGNORE_DEVICES_EXCEPT", "", SDL.SDL_HintPriority.SDL_HINT_OVERRIDE);
			SDL.SDL_SetHintWithPriority("SDL_GAMECONTROLLER_ALLOW_STEAM_VIRTUAL_GAMEPAD", "0", SDL.SDL_HintPriority.SDL_HINT_OVERRIDE);
		}
		if (args.TryGetValue("glprofile", out var value))
		{
			switch (value)
			{
			case "es3":
				SDL.SDL_SetHintWithPriority("FNA3D_OPENGL_FORCE_ES3", "1", SDL.SDL_HintPriority.SDL_HINT_OVERRIDE);
				break;
			case "core":
				SDL.SDL_SetHintWithPriority("FNA3D_OPENGL_FORCE_CORE_PROFILE", "1", SDL.SDL_HintPriority.SDL_HINT_OVERRIDE);
				break;
			case "compatibility":
				SDL.SDL_SetHintWithPriority("FNA3D_OPENGL_FORCE_COMPATIBILITY_PROFILE", "1", SDL.SDL_HintPriority.SDL_HINT_OVERRIDE);
				break;
			}
		}
		if (args.TryGetValue("angle", out value) && value == "1")
		{
			SDL.SDL_SetHintWithPriority("FNA3D_OPENGL_FORCE_ES3", "1", SDL.SDL_HintPriority.SDL_HINT_OVERRIDE);
			SDL.SDL_SetHintWithPriority("SDL_OPENGL_ES_DRIVER", "1", SDL.SDL_HintPriority.SDL_HINT_OVERRIDE);
		}
		if (args.TryGetValue("forcemailboxvsync", out value) && value == "1")
		{
			SDL.SDL_SetHintWithPriority("FNA3D_VULKAN_FORCE_MAILBOX_VSYNC", "1", SDL.SDL_HintPriority.SDL_HINT_OVERRIDE);
		}
		if (args.TryGetValue("audiodriver", out value))
		{
			SDL.SDL_SetHintWithPriority("SDL_AUDIO_DRIVER", value, SDL.SDL_HintPriority.SDL_HINT_OVERRIDE);
		}
		if (!SDL.SDL_Init(SDL.SDL_InitFlags.SDL_INIT_VIDEO | SDL.SDL_InitFlags.SDL_INIT_GAMEPAD))
		{
			throw new Exception("SDL_Init failed: " + SDL.SDL_GetError());
		}
		string text3 = SDL.SDL_GetCurrentVideoDriver();
		SupportsGlobalMouse = OSVersion.Equals("Windows") || OSVersion.Equals("macOS") || text3.Equals("x11");
		if (Environment.GetEnvironmentVariable("FNA_MOUSE_DISABLE_GLOBAL_ACCESS") == "1")
		{
			SupportsGlobalMouse = false;
		}
		if (SupportsGlobalMouse)
		{
			SDL.SDL_SetHint("SDL_MOUSE_EMULATE_WARP_WITH_RELATIVE", "0");
		}
		SupportsOrientations = OSVersion.Equals("iOS") || OSVersion.Equals("Android");
		if (OSVersion.Equals("Windows"))
		{
			SDL.SDL_SetHint("SDL_VIDEO_MINIMIZE_ON_FOCUS_LOSS", "1");
		}
		string value2 = SDL.SDL_GetHint("SDL_JOYSTICK_ALLOW_BACKGROUND_EVENTS");
		if (string.IsNullOrEmpty(value2))
		{
			SDL.SDL_SetHint("SDL_JOYSTICK_ALLOW_BACKGROUND_EVENTS", "1");
		}
		SDL.SDL_SetHint("SDL_ORIENTATIONS", "LandscapeLeft LandscapeRight Portrait");
		SDL.SDL_Event[] array = new SDL.SDL_Event[1];
		SDL.SDL_PumpEvents();
		while (SDL.SDL_PeepEvents(array, 1, SDL.SDL_EventAction.SDL_GETEVENT, 1619u, 1619u) == 1)
		{
			INTERNAL_AddInstance(array[0].gdevice.which);
		}
		if (OSVersion.Equals("Windows") && SDL.SDL_GetHint("FNA_WIN32_IGNORE_WM_PAINT") != "1")
		{
			SDL.SDL_GetEventFilter(out prevEventFilter, out var userdata);
			SDL.SDL_SetEventFilter(win32OnPaint, userdata);
		}
		return baseDirectory;
	}

	public static void ProgramExit(object sender, EventArgs e)
	{
		SDL.SDL_QuitSubSystem(SDL.SDL_InitFlags.SDL_INIT_VIDEO | SDL.SDL_InitFlags.SDL_INIT_GAMEPAD);
	}

	public static nint Malloc(int size)
	{
		return SDL.SDL_malloc((nuint)size);
	}

	public static void SetEnv(string name, string value)
	{
		SDL.SDL_SetHintWithPriority(name, value, SDL.SDL_HintPriority.SDL_HINT_OVERRIDE);
	}

	public static GameWindow CreateWindow()
	{
		SDL.SDL_WindowFlags sDL_WindowFlags = (SDL.SDL_WindowFlags)(0x608uL | (ulong)FNA3D.FNA3D_PrepareWindowAttributes());
		if ((sDL_WindowFlags & SDL.SDL_WindowFlags.SDL_WINDOW_VULKAN) == SDL.SDL_WindowFlags.SDL_WINDOW_VULKAN)
		{
			string text = SDL.SDL_GetHint("FNA3D_VULKAN_PIPELINE_CACHE_FILE_NAME");
			if (text == null)
			{
				text = ((!OSVersion.Equals("Windows") && !OSVersion.Equals("macOS") && !OSVersion.Equals("Linux") && !OSVersion.Equals("FreeBSD") && !OSVersion.Equals("OpenBSD") && !OSVersion.Equals("NetBSD")) ? string.Empty : "FNA3D_Vulkan_PipelineCache.blob");
				SDL.SDL_SetHint("FNA3D_VULKAN_PIPELINE_CACHE_FILE_NAME", text);
			}
		}
		if (Environment.GetEnvironmentVariable("FNA_GRAPHICS_ENABLE_HIGHDPI") == "1")
		{
			sDL_WindowFlags |= SDL.SDL_WindowFlags.SDL_WINDOW_HIGH_PIXEL_DENSITY;
		}
		string defaultWindowTitle = AssemblyHelper.GetDefaultWindowTitle();
		nint num = SDL.SDL_CreateWindow(defaultWindowTitle, GraphicsDeviceManager.DefaultBackBufferWidth, GraphicsDeviceManager.DefaultBackBufferHeight, sDL_WindowFlags);
		if (num == IntPtr.Zero)
		{
			throw new NoSuitableGraphicsDeviceException(SDL.SDL_GetError());
		}
		INTERNAL_SetIcon(num, defaultWindowTitle);
		SDL.SDL_DisableScreenSaver();
		OnIsMouseVisibleChanged(visible: false);
		sDL_WindowFlags = SDL.SDL_GetWindowFlags(num);
		if ((sDL_WindowFlags & SDL.SDL_WindowFlags.SDL_WINDOW_HIGH_PIXEL_DENSITY) == 0)
		{
			Environment.SetEnvironmentVariable("FNA_GRAPHICS_ENABLE_HIGHDPI", "0");
		}
		return new FNAWindow(UnwrapWindow(num), "\\\\.\\DISPLAY" + SDL.SDL_GetDisplayForWindow(num), defaultWindowTitle);
	}

	public static void DisposeWindow(GameWindow window)
	{
		SDL.SDL_SetHintWithPriority("SDL_VIDEO_MINIMIZE_ON_FOCUS_LOSS", "0", SDL.SDL_HintPriority.SDL_HINT_OVERRIDE);
		if (Mouse.WindowHandle == window.Handle)
		{
			Mouse.WindowHandle = IntPtr.Zero;
		}
		if (TouchPanel.WindowHandle == window.Handle)
		{
			TouchPanel.WindowHandle = IntPtr.Zero;
		}
		if (TextInputEXT.WindowHandle == window.Handle)
		{
			TextInputEXT.WindowHandle = IntPtr.Zero;
		}
		SDL.SDL_DestroyWindow(WrapWindow(window.Handle));
	}

	public unsafe static void ApplyWindowChanges(nint window, int clientWidth, int clientHeight, bool wantsFullscreen, string screenDeviceName, ref string resultDeviceName)
	{
		bool flag = false;
		ScaleForWindow(window, invert: false, ref clientWidth, ref clientHeight);
		nint window2 = WrapWindow(window);
		if (!wantsFullscreen)
		{
			bool flag2 = false;
			if ((SDL.SDL_GetWindowFlags(window2) & SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) != 0)
			{
				SDL.SDL_SetWindowFullscreen(window2, false);
				flag2 = true;
			}
			else
			{
				SDL.SDL_GetWindowSize(window2, out var w, out var h);
				flag2 = clientWidth != w || clientHeight != h;
			}
			if (flag2)
			{
				SDL.SDL_SetWindowSize(window2, clientWidth, clientHeight);
				flag = true;
			}
		}
		int num = 0;
		for (int i = 0; i < GraphicsAdapter.Adapters.Count; i++)
		{
			if (screenDeviceName == GraphicsAdapter.Adapters[i].DeviceName)
			{
				num = i;
				break;
			}
		}
		if (resultDeviceName != screenDeviceName)
		{
			SDL.SDL_SetWindowFullscreen(window2, false);
			resultDeviceName = screenDeviceName;
			flag = true;
		}
		if (flag)
		{
			int num2 = (int)(0x2FFF0000 | displayIds[num]);
			SDL.SDL_SetWindowPosition(window2, num2, num2);
		}
		if (wantsFullscreen)
		{
			if ((SDL.SDL_GetWindowFlags(window2) & SDL.SDL_WindowFlags.SDL_WINDOW_HIDDEN) != 0)
			{
				SDL.SDL_DisplayMode* ptr = (SDL.SDL_DisplayMode*)SDL.SDL_GetCurrentDisplayMode(SDL.SDL_GetDisplayForWindow(window2));
				SDL.SDL_SetWindowSize(window2, ptr->w, ptr->h);
			}
			SDL.SDL_SetWindowFullscreen(window2, true);
		}
		if (Mouse.WindowHandle == window)
		{
			Rectangle windowBounds = GetWindowBounds(window);
			Mouse.INTERNAL_WindowWidth = windowBounds.Width;
			Mouse.INTERNAL_WindowHeight = windowBounds.Height;
		}
	}

	public static void ScaleForWindow(nint window, bool invert, ref int w, ref int h)
	{
		nint window2 = WrapWindow(window);
		SDL.SDL_GetWindowSize(window2, out var w2, out var h2);
		FNA3D.FNA3D_GetDrawableSize(window2, out var w3, out var h3);
		if (w2 != 0 && h2 != 0 && w3 != 0 && h3 != 0 && (w2 != w3 || h2 != h3))
		{
			if (invert)
			{
				w = (int)((float)w * ((float)w3 / (float)w2));
				h = (int)((float)h * ((float)h3 / (float)h2));
			}
			else
			{
				w = (int)((float)w / ((float)w3 / (float)w2));
				h = (int)((float)h / ((float)h3 / (float)h2));
			}
		}
	}

	public unsafe static Rectangle GetWindowBounds(nint window)
	{
		nint window2 = WrapWindow(window);
		Rectangle result = default(Rectangle);
		if ((SDL.SDL_GetWindowFlags(window2) & SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) != 0)
		{
			SDL.SDL_DisplayMode* ptr = (SDL.SDL_DisplayMode*)SDL.SDL_GetCurrentDisplayMode(SDL.SDL_GetDisplayForWindow(window2));
			result.X = 0;
			result.Y = 0;
			result.Width = ptr->w;
			result.Height = ptr->h;
		}
		else
		{
			SDL.SDL_GetWindowPosition(window2, out result.X, out result.Y);
			SDL.SDL_GetWindowSize(window2, out result.Width, out result.Height);
		}
		return result;
	}

	public static bool GetWindowResizable(nint window)
	{
		return (SDL.SDL_GetWindowFlags(WrapWindow(window)) & SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE) != 0;
	}

	public static void SetWindowResizable(nint window, bool resizable)
	{
		SDL.SDL_SetWindowResizable(WrapWindow(window), resizable);
	}

	public static bool GetWindowBorderless(nint window)
	{
		return (SDL.SDL_GetWindowFlags(WrapWindow(window)) & SDL.SDL_WindowFlags.SDL_WINDOW_BORDERLESS) != 0;
	}

	public static void SetWindowBorderless(nint window, bool borderless)
	{
		SDL.SDL_SetWindowBordered(WrapWindow(window), !borderless);
	}

	public static void SetWindowTitle(nint window, string title)
	{
		SDL.SDL_SetWindowTitle(WrapWindow(window), title);
	}

	public static bool IsScreenKeyboardShown(nint window)
	{
		return SDL.SDL_ScreenKeyboardShown(WrapWindow(window));
	}

	private static void INTERNAL_SetIcon(nint window, string title)
	{
		string empty = string.Empty;
		try
		{
			empty = INTERNAL_GetIconName(title + ".png");
			if (!string.IsNullOrEmpty(empty))
			{
				nint num;
				nint num2;
				using (Stream stream = TitleContainer.OpenStream(empty))
				{
					num = FNA3D.ReadImageStream(stream, out var width, out var height, out var _);
					num2 = SDL.SDL_CreateSurfaceFrom(width, height, SDL.SDL_PixelFormat.SDL_PIXELFORMAT_ABGR8888, num, width * 4);
				}
				SDL.SDL_SetWindowIcon(window, num2);
				SDL.SDL_DestroySurface(num2);
				FNA3D.FNA3D_Image_Free(num);
				return;
			}
		}
		catch (DllNotFoundException)
		{
		}
		empty = INTERNAL_GetIconName(title + ".bmp");
		if (!string.IsNullOrEmpty(empty))
		{
			nint num3 = SDL.SDL_LoadBMP(empty);
			SDL.SDL_SetWindowIcon(window, num3);
			SDL.SDL_DestroySurface(num3);
		}
	}

	private static string INTERNAL_GetIconName(string title)
	{
		string text = Path.Combine(TitleLocation.Path, title);
		if (File.Exists(text))
		{
			return text;
		}
		text = Path.Combine(TitleLocation.Path, INTERNAL_StripBadChars(title));
		if (File.Exists(text))
		{
			return text;
		}
		return string.Empty;
	}

	private static string INTERNAL_StripBadChars(string path)
	{
		char[] collection = new char[9] { '<', '>', ':', '"', '/', '\\', '|', '?', '*' };
		List<char> list = new List<char>();
		list.AddRange(Path.GetInvalidFileNameChars());
		list.AddRange(collection);
		string text = path;
		foreach (char item in list)
		{
			text = text.Replace(item.ToString(), "");
		}
		return text;
	}

	public static void SetTextInputRectangle(nint window, Rectangle rectangle)
	{
		SDL.SDL_Rect rect = default(SDL.SDL_Rect);
		rect.x = rectangle.X;
		rect.y = rectangle.Y;
		rect.w = rectangle.Width;
		rect.h = rectangle.Height;
		SDL.SDL_SetTextInputArea(WrapWindow(window), ref rect, 0);
	}

	public static nint WrapWindow(nint handle)
	{
		return handle;
	}

	public static nint UnwrapWindow(nint handle)
	{
		return handle;
	}

	private static DisplayOrientation INTERNAL_ConvertOrientation(SDL.SDL_DisplayOrientation orientation)
	{
		switch (orientation)
		{
		case SDL.SDL_DisplayOrientation.SDL_ORIENTATION_LANDSCAPE:
			return DisplayOrientation.LandscapeLeft;
		case SDL.SDL_DisplayOrientation.SDL_ORIENTATION_LANDSCAPE_FLIPPED:
			return DisplayOrientation.LandscapeRight;
		case SDL.SDL_DisplayOrientation.SDL_ORIENTATION_PORTRAIT:
		case SDL.SDL_DisplayOrientation.SDL_ORIENTATION_PORTRAIT_FLIPPED:
			return DisplayOrientation.Portrait;
		default:
			throw new NotSupportedException("FNA does not support this device orientation.");
		}
	}

	private static void INTERNAL_HandleOrientationChange(DisplayOrientation orientation, GraphicsDevice graphicsDevice, GraphicsAdapter graphicsAdapter, FNAWindow window)
	{
		int backBufferWidth = graphicsDevice.PresentationParameters.BackBufferWidth;
		int backBufferHeight = graphicsDevice.PresentationParameters.BackBufferHeight;
		int num = Math.Min(backBufferWidth, backBufferHeight);
		int num2 = Math.Max(backBufferWidth, backBufferHeight);
		if (orientation == DisplayOrientation.Portrait)
		{
			graphicsDevice.PresentationParameters.BackBufferWidth = num;
			graphicsDevice.PresentationParameters.BackBufferHeight = num2;
		}
		else
		{
			graphicsDevice.PresentationParameters.BackBufferWidth = num2;
			graphicsDevice.PresentationParameters.BackBufferHeight = num;
		}
		graphicsDevice.PresentationParameters.DisplayOrientation = orientation;
		window.CurrentOrientation = orientation;
		graphicsDevice.Reset(graphicsDevice.PresentationParameters, graphicsAdapter);
		window.INTERNAL_OnOrientationChanged();
	}

	public static bool SupportsOrientationChanges()
	{
		return SupportsOrientations;
	}

	public static GraphicsAdapter RegisterGame(Game game)
	{
		nint window = WrapWindow(game.Window.Handle);
		SDL.SDL_ShowWindow(window);
		activeGames.Add(game);
		return FetchDisplayAdapter(window);
	}

	public static void UnregisterGame(Game game)
	{
		activeGames.Remove(game);
	}

	public unsafe static void PollEvents(Game game, ref GraphicsAdapter currentAdapter, bool[] textInputControlDown, ref bool textInputSuppress)
	{
		char* ptr = stackalloc char[32];
		SDL.SDL_Event @event;
		while ((bool)SDL.SDL_PollEvent(out @event))
		{
			if (@event.type == 768)
			{
				Keys keys = ToXNAKey(ref @event.key.key, ref @event.key.scancode);
				if (Keyboard.keys.IsKeyUp(keys))
				{
					Keyboard.keys.AddPressedKey((int)keys);
					if (FNAPlatform.TextInputBindings.TryGetValue(keys, out var value))
					{
						textInputControlDown[value] = true;
						TextInputEXT.OnTextInput(FNAPlatform.TextInputCharacters[value]);
					}
					else if ((Keyboard.keys.IsKeyDown(Keys.LeftControl) || Keyboard.keys.IsKeyDown(Keys.RightControl)) && Keyboard.keys.IsKeyUp(Keys.LeftAlt) && keys == Keys.V)
					{
						textInputControlDown[6] = true;
						TextInputEXT.OnTextInput(FNAPlatform.TextInputCharacters[6]);
						textInputSuppress = true;
					}
				}
				else if ((bool)@event.key.repeat)
				{
					if (FNAPlatform.TextInputBindings.TryGetValue(keys, out var value2))
					{
						TextInputEXT.OnTextInput(FNAPlatform.TextInputCharacters[value2]);
					}
					else if ((Keyboard.keys.IsKeyDown(Keys.LeftControl) || Keyboard.keys.IsKeyDown(Keys.RightControl)) && keys == Keys.V)
					{
						TextInputEXT.OnTextInput(FNAPlatform.TextInputCharacters[6]);
					}
				}
			}
			else if (@event.type == 769)
			{
				Keys keys2 = ToXNAKey(ref @event.key.key, ref @event.key.scancode);
				if (Keyboard.keys.IsKeyDown(keys2))
				{
					Keyboard.keys.RemovePressedKey((int)keys2);
					if (FNAPlatform.TextInputBindings.TryGetValue(keys2, out var value3))
					{
						textInputControlDown[value3] = false;
					}
					else if ((Keyboard.keys.IsKeyUp(Keys.LeftControl) && Keyboard.keys.IsKeyUp(Keys.RightControl) && textInputControlDown[6]) || keys2 == Keys.V)
					{
						textInputControlDown[6] = false;
						textInputSuppress = false;
					}
				}
			}
			else if (@event.type == 1025)
			{
				Mouse.INTERNAL_onClicked(@event.button.button - 1);
			}
			else if (@event.type == 1027)
			{
				Mouse.INTERNAL_MouseWheel += @event.wheel.y * 120f;
			}
			else if (@event.type == 1792)
			{
				TouchPanel.TouchDeviceExists = true;
				TouchPanel.INTERNAL_onTouchEvent((int)@event.tfinger.fingerID, TouchLocationState.Pressed, @event.tfinger.x, @event.tfinger.y, 0f, 0f);
			}
			else if (@event.type == 1794)
			{
				TouchPanel.INTERNAL_onTouchEvent((int)@event.tfinger.fingerID, TouchLocationState.Moved, @event.tfinger.x, @event.tfinger.y, @event.tfinger.dx, @event.tfinger.dy);
			}
			else if (@event.type == 1793 || @event.type == 1795)
			{
				TouchPanel.INTERNAL_onTouchEvent((int)@event.tfinger.fingerID, TouchLocationState.Released, @event.tfinger.x, @event.tfinger.y, 0f, 0f);
			}
			else if (@event.type >= 514 && @event.type <= 538)
			{
				if (@event.type == 526)
				{
					game.IsActive = true;
					if (SDL.SDL_GetCurrentVideoDriver() == "x11")
					{
						SDL.SDL_SetWindowFullscreen(WrapWindow(game.Window.Handle), game.GraphicsDevice.PresentationParameters.IsFullScreen);
					}
					SDL.SDL_DisableScreenSaver();
				}
				else if (@event.type == 527)
				{
					game.IsActive = false;
					if (SDL.SDL_GetCurrentVideoDriver() == "x11")
					{
						SDL.SDL_SetWindowFullscreen(WrapWindow(game.Window.Handle), false);
					}
					SDL.SDL_EnableScreenSaver();
				}
				else if (@event.type == 519)
				{
					Rectangle windowBounds = GetWindowBounds(Mouse.WindowHandle);
					Mouse.INTERNAL_WindowWidth = windowBounds.Width;
					Mouse.INTERNAL_WindowHeight = windowBounds.Height;
				}
				else if (@event.type == 518)
				{
					SDL.SDL_WindowFlags sDL_WindowFlags = SDL.SDL_GetWindowFlags(WrapWindow(game.Window.Handle));
					if ((sDL_WindowFlags & SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE) != (SDL.SDL_WindowFlags)0uL && (sDL_WindowFlags & (SDL.SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS | SDL.SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS)) != 0)
					{
						((FNAWindow)game.Window).INTERNAL_ClientSizeChanged();
					}
				}
				else if (@event.type == 516)
				{
					game.RedrawWindow();
				}
				else if (@event.type == 531)
				{
					GraphicsAdapter graphicsAdapter = FetchDisplayAdapter(WrapWindow(game.Window.Handle));
					if (graphicsAdapter != currentAdapter)
					{
						currentAdapter = graphicsAdapter;
						game.GraphicsDevice.Reset(game.GraphicsDevice.PresentationParameters, currentAdapter);
					}
				}
				else if (@event.type == 524)
				{
					SDL.SDL_DisableScreenSaver();
				}
				else if (@event.type == 525)
				{
					SDL.SDL_EnableScreenSaver();
				}
				else if (@event.type == 535)
				{
					if (game.Services.INTERNAL_GetService(typeof(IGraphicsDeviceManager)) is GraphicsDeviceManager graphicsDeviceManager)
					{
						graphicsDeviceManager.IsFullScreen = true;
					}
				}
				else if (@event.type == 536 && game.Services.INTERNAL_GetService(typeof(IGraphicsDeviceManager)) is GraphicsDeviceManager graphicsDeviceManager2)
				{
					graphicsDeviceManager2.IsFullScreen = false;
				}
			}
			else if (@event.type >= 337 && @event.type <= 344)
			{
				GraphicsAdapter.AdaptersChanged();
				currentAdapter = FetchDisplayAdapter(WrapWindow(game.Window.Handle));
				if (@event.type == 337)
				{
					if (SupportsOrientationChanges())
					{
						DisplayOrientation orientation = INTERNAL_ConvertOrientation((SDL.SDL_DisplayOrientation)@event.display.data1);
						INTERNAL_HandleOrientationChange(orientation, game.GraphicsDevice, currentAdapter, (FNAWindow)game.Window);
					}
				}
				else
				{
					game.GraphicsDevice.QuietlyUpdateAdapter(currentAdapter);
				}
			}
			else if (@event.type == 1619)
			{
				INTERNAL_AddInstance(@event.gdevice.which);
			}
			else if (@event.type == 1620)
			{
				INTERNAL_RemoveInstance(@event.gdevice.which);
			}
			else if (@event.type == 771 && !textInputSuppress)
			{
				int num = MeasureStringLength(@event.text.text);
				if (num > 0)
				{
					int chars = Encoding.UTF8.GetChars(@event.text.text, num, ptr, num);
					for (int i = 0; i < chars; i++)
					{
						TextInputEXT.OnTextInput(ptr[i]);
					}
				}
			}
			else if (@event.type == 770)
			{
				int num2 = MeasureStringLength(@event.edit.text);
				if (num2 > 0)
				{
					int chars2 = Encoding.UTF8.GetChars(@event.edit.text, num2, ptr, num2);
					string text = new string(ptr, 0, chars2);
					TextInputEXT.OnTextEditing(text, @event.edit.start, @event.edit.length);
				}
				else
				{
					TextInputEXT.OnTextEditing(null, 0, 0);
				}
			}
			else if (@event.type == 256)
			{
				game.RunApplication = false;
				break;
			}
		}
	}

	private unsafe static int MeasureStringLength(byte* ptr)
	{
		int num = 0;
		while (*ptr != 0)
		{
			ptr++;
			num++;
		}
		return num;
	}

	public static bool NeedsPlatformMainLoop()
	{
		return SDL.SDL_GetPlatform().Equals("Emscripten");
	}

	public static void RunPlatformMainLoop(Game game)
	{
		if (SDL.SDL_GetPlatform().Equals("Emscripten"))
		{
			emscriptenGame = game;
			emscripten_set_main_loop(RunEmscriptenMainLoop, 0, 1);
			return;
		}
		throw new NotSupportedException("Cannot run the main loop of an unknown platform");
	}

	[DllImport("__Native", CallingConvention = CallingConvention.Cdecl)]
	private static extern void emscripten_set_main_loop(em_callback_func func, int fps, int simulate_infinite_loop);

	[DllImport("__Native", CallingConvention = CallingConvention.Cdecl)]
	private static extern void emscripten_cancel_main_loop();

	[MonoPInvokeCallback(typeof(em_callback_func))]
	private static void RunEmscriptenMainLoop()
	{
		emscriptenGame.RunOneFrame();
		if (!emscriptenGame.RunApplication)
		{
			emscriptenGame.Exit();
			emscripten_cancel_main_loop();
		}
	}

	private static GraphicsAdapter FetchDisplayAdapter(nint window, bool retry = true)
	{
		uint num = SDL.SDL_GetDisplayForWindow(window);
		int num2 = -1;
		for (int i = 0; i < displayIds.Length; i++)
		{
			if (num == displayIds[i])
			{
				num2 = i;
				break;
			}
		}
		if ((uint)num2 > (uint)GraphicsAdapter.Adapters.Count)
		{
			FNALoggerEXT.LogWarn("SDL3 Window ID and Display ID desync'd");
			if (retry)
			{
				GraphicsAdapter.AdaptersChanged();
				return FetchDisplayAdapter(window, retry: false);
			}
			FNALoggerEXT.LogWarn("SDL3 Window ID and Display ID desync'd really badly");
			return GraphicsAdapter.DefaultAdapter;
		}
		return GraphicsAdapter.Adapters[num2];
	}

	public unsafe static GraphicsAdapter[] GetGraphicsAdapters()
	{
		uint* ptr = (uint*)SDL.SDL_GetDisplays(out var count);
		GraphicsAdapter[] array = new GraphicsAdapter[count];
		displayIds = new uint[count];
		for (int i = 0; i < array.Length; i++)
		{
			List<DisplayMode> list = new List<DisplayMode>();
			SDL.SDL_DisplayMode** ptr2 = (SDL.SDL_DisplayMode**)SDL.SDL_GetFullscreenDisplayModes(ptr[i], out var count2);
			for (int num = count2 - 1; num >= 0; num--)
			{
				bool flag = false;
				foreach (DisplayMode item in list)
				{
					if (ptr2[num]->w == item.Width && ptr2[num]->h == item.Height)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					list.Add(new DisplayMode(ptr2[num]->w, ptr2[num]->h, SurfaceFormat.Color));
				}
			}
			SDL.SDL_free((nint)ptr2);
			array[i] = new GraphicsAdapter(new DisplayModeCollection(list), "\\\\.\\DISPLAY" + (i + 1), SDL.SDL_GetDisplayName(ptr[i]));
			displayIds[i] = ptr[i];
		}
		SDL.SDL_free((nint)ptr);
		return array;
	}

	public unsafe static DisplayMode GetCurrentDisplayMode(int adapterIndex)
	{
		SDL.SDL_DisplayMode* ptr = (SDL.SDL_DisplayMode*)SDL.SDL_GetCurrentDisplayMode(displayIds[adapterIndex]);
		return new DisplayMode(ptr->w, ptr->h, SurfaceFormat.Color);
	}

	public static nint GetMonitorHandle(int adapterIndex)
	{
		return new IntPtr((int)displayIds[adapterIndex]);
	}

	public static void GetMouseState(nint window, out int x, out int y, out ButtonState left, out ButtonState middle, out ButtonState right, out ButtonState x1, out ButtonState x2)
	{
		SDL.SDL_MouseButtonFlags sDL_MouseButtonFlags;
		float x3;
		float y2;
		if (GetRelativeMouseMode(window))
		{
			sDL_MouseButtonFlags = SDL.SDL_GetRelativeMouseState(out x3, out y2);
		}
		else if (SupportsGlobalMouse)
		{
			sDL_MouseButtonFlags = SDL.SDL_GetGlobalMouseState(out x3, out y2);
			int x4 = 0;
			int y3 = 0;
			SDL.SDL_GetWindowPosition(WrapWindow(window), out x4, out y3);
			x3 -= (float)x4;
			y2 -= (float)y3;
		}
		else
		{
			sDL_MouseButtonFlags = SDL.SDL_GetMouseState(out x3, out y2);
		}
		x = (int)x3;
		y = (int)y2;
		left = (ButtonState)(sDL_MouseButtonFlags & SDL.SDL_MouseButtonFlags.SDL_BUTTON_LMASK);
		middle = (ButtonState)((uint)(sDL_MouseButtonFlags & SDL.SDL_MouseButtonFlags.SDL_BUTTON_MMASK) >> 1);
		right = (ButtonState)((uint)(sDL_MouseButtonFlags & SDL.SDL_MouseButtonFlags.SDL_BUTTON_RMASK) >> 2);
		x1 = (ButtonState)((uint)(sDL_MouseButtonFlags & SDL.SDL_MouseButtonFlags.SDL_BUTTON_X1MASK) >> 3);
		x2 = (ButtonState)((uint)(sDL_MouseButtonFlags & SDL.SDL_MouseButtonFlags.SDL_BUTTON_X2MASK) >> 4);
	}

	public static void WarpMouseInWindow(nint window, int x, int y)
	{
		SDL.SDL_WarpMouseInWindow(WrapWindow(window), x, y);
	}

	public static void OnIsMouseVisibleChanged(bool visible)
	{
		if (visible)
		{
			SDL.SDL_ShowCursor();
		}
		else
		{
			SDL.SDL_HideCursor();
		}
	}

	public static bool GetRelativeMouseMode(nint window)
	{
		return SDL.SDL_GetWindowRelativeMouseMode(WrapWindow(window));
	}

	public static void SetRelativeMouseMode(nint window, bool enable)
	{
		SDL.SDL_SetWindowRelativeMouseMode(WrapWindow(window), enable);
		if (enable)
		{
			SDL.SDL_GetRelativeMouseState(out float x, out x);
		}
	}

	private static string GetBaseDirectory()
	{
		if (Environment.GetEnvironmentVariable("FNA_SDL_FORCE_BASE_PATH") != "1" && (OSVersion.Equals("Windows") || OSVersion.Equals("macOS") || OSVersion.Equals("Linux") || OSVersion.Equals("FreeBSD") || OSVersion.Equals("OpenBSD") || OSVersion.Equals("NetBSD")))
		{
			return AppDomain.CurrentDomain.BaseDirectory;
		}
		string text = SDL.SDL_GetBasePath();
		if (string.IsNullOrEmpty(text))
		{
			text = AppDomain.CurrentDomain.BaseDirectory;
		}
		if (string.IsNullOrEmpty(text))
		{
			text = Environment.CurrentDirectory;
		}
		return text;
	}

	public static string GetStorageRoot()
	{
		string text = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName).Replace(".vshost", "");
		if (OSVersion.Equals("Windows"))
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "SavedGames", text);
		}
		if (OSVersion.Equals("macOS"))
		{
			string environmentVariable = Environment.GetEnvironmentVariable("HOME");
			if (string.IsNullOrEmpty(environmentVariable))
			{
				return ".";
			}
			return Path.Combine(environmentVariable, "Library/Application Support", text);
		}
		if (OSVersion.Equals("Linux") || OSVersion.Equals("FreeBSD") || OSVersion.Equals("OpenBSD") || OSVersion.Equals("NetBSD"))
		{
			string text2 = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
			if (string.IsNullOrEmpty(text2))
			{
				text2 = Environment.GetEnvironmentVariable("HOME");
				if (string.IsNullOrEmpty(text2))
				{
					return ".";
				}
				text2 += "/.local/share";
			}
			return Path.Combine(text2, text);
		}
		return SDL.SDL_GetPrefPath(null, text);
	}

	public static DriveInfo GetDriveInfo(string storageRoot)
	{
		try
		{
			return new DriveInfo(MonoPathRootWorkaround(storageRoot));
		}
		catch (Exception ex)
		{
			FNALoggerEXT.LogError("Failed to get DriveInfo: " + ex.ToString());
			return null;
		}
	}

	private static string MonoPathRootWorkaround(string storageRoot)
	{
		if (Environment.OSVersion.Platform == PlatformID.Win32NT)
		{
			return Path.GetPathRoot(storageRoot);
		}
		if (storageRoot == null)
		{
			return null;
		}
		if (storageRoot.Trim().Length == 0)
		{
			throw new ArgumentException("The specified path is not of a legal form.");
		}
		if (!Path.IsPathRooted(storageRoot) && !storageRoot.Contains(":"))
		{
			return string.Empty;
		}
		int num = -1;
		int num2 = 0;
		string[] logicalDrives = Environment.GetLogicalDrives();
		for (int i = 0; i < logicalDrives.Length; i++)
		{
			if (!string.IsNullOrEmpty(logicalDrives[i]))
			{
				string text = logicalDrives[i];
				if (text[text.Length - 1] != Path.DirectorySeparatorChar)
				{
					text += Path.DirectorySeparatorChar;
				}
				if (storageRoot.StartsWith(text) && text.Length > num2)
				{
					num = i;
					num2 = text.Length;
				}
			}
		}
		if (num >= 0)
		{
			return logicalDrives[num];
		}
		return Path.GetPathRoot(storageRoot);
	}

	public unsafe static nint ReadToPointer(string path, out nint size)
	{
		nint result = SDL.SDL_LoadFile(path, out var datasize);
		size = (nint)((UIntPtr)datasize).ToPointer();
		return result;
	}

	public static void FreeFilePointer(nint file)
	{
		SDL.SDL_free(file);
	}

	public static void ShowRuntimeError(GameWindow gameWindow, string message)
	{
		SDL.SDL_ShowSimpleMessageBox(SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR, gameWindow.Title, message, gameWindow.Handle);
	}

	public unsafe static Microphone[] GetMicrophones()
	{
		if (!micInit)
		{
			SDL.SDL_InitSubSystem(SDL.SDL_InitFlags.SDL_INIT_AUDIO);
			micInit = true;
		}
		uint* ptr = (uint*)SDL.SDL_GetAudioRecordingDevices(out var count);
		if (count < 1)
		{
			return new Microphone[0];
		}
		Microphone[] array = new Microphone[count + 1];
		SDL.SDL_AudioSpec spec = default(SDL.SDL_AudioSpec);
		spec.freq = 44100;
		spec.format = SDL.SDL_AudioFormat.SDL_AUDIO_S16LE;
		spec.channels = 1;
		array[0] = new Microphone(SDL.SDL_OpenAudioDeviceStream(4294967294u, ref spec, null, IntPtr.Zero), "Default Device");
		for (int i = 0; i < count; i++)
		{
			nint num = SDL.SDL_OpenAudioDeviceStream(ptr[i], ref spec, null, IntPtr.Zero);
			array[i + 1] = new Microphone(num, SDL.SDL_GetAudioDeviceName(SDL.SDL_GetAudioStreamDevice(num)));
		}
		SDL.SDL_free((nint)ptr);
		return array;
	}

	public unsafe static int GetMicrophoneSamples(nint handle, byte[] buffer, int offset, int count)
	{
		fixed (byte* buf = &buffer[offset])
		{
			return SDL.SDL_GetAudioStreamData(handle, (nint)buf, count);
		}
	}

	public static int GetMicrophoneQueuedBytes(nint handle)
	{
		return SDL.SDL_GetAudioStreamQueued(handle);
	}

	public static void StartMicrophone(nint handle)
	{
		SDL.SDL_ResumeAudioStreamDevice(handle);
	}

	public static void StopMicrophone(nint handle)
	{
		SDL.SDL_PauseAudioStreamDevice(handle);
	}

	public static GamePadCapabilities GetGamePadCapabilities(int index)
	{
		if (INTERNAL_devices[index] == IntPtr.Zero)
		{
			return default(GamePadCapabilities);
		}
		return INTERNAL_capabilities[index];
	}

	public static GamePadState GetGamePadState(int index, GamePadDeadZone deadZoneMode)
	{
		nint num = INTERNAL_devices[index];
		if (num == IntPtr.Zero)
		{
			return default(GamePadState);
		}
		Vector2 leftPosition = new Vector2((float)SDL.SDL_GetGamepadAxis(num, SDL.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTX) / 32767f, (float)SDL.SDL_GetGamepadAxis(num, SDL.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTY) / -32767f);
		Vector2 rightPosition = new Vector2((float)SDL.SDL_GetGamepadAxis(num, SDL.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTX) / 32767f, (float)SDL.SDL_GetGamepadAxis(num, SDL.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTY) / -32767f);
		float leftTrigger = (float)SDL.SDL_GetGamepadAxis(num, SDL.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFT_TRIGGER) / 32767f;
		float rightTrigger = (float)SDL.SDL_GetGamepadAxis(num, SDL.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHT_TRIGGER) / 32767f;
		Buttons buttons = (Buttons)0;
		if ((bool)SDL.SDL_GetGamepadButton(num, SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_SOUTH))
		{
			buttons |= Buttons.A;
		}
		if ((bool)SDL.SDL_GetGamepadButton(num, SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_EAST))
		{
			buttons |= Buttons.B;
		}
		if ((bool)SDL.SDL_GetGamepadButton(num, SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_WEST))
		{
			buttons |= Buttons.X;
		}
		if ((bool)SDL.SDL_GetGamepadButton(num, SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_NORTH))
		{
			buttons |= Buttons.Y;
		}
		if ((bool)SDL.SDL_GetGamepadButton(num, SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_BACK))
		{
			buttons |= Buttons.Back;
		}
		if ((bool)SDL.SDL_GetGamepadButton(num, SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_GUIDE))
		{
			buttons |= Buttons.BigButton;
		}
		if ((bool)SDL.SDL_GetGamepadButton(num, SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_START))
		{
			buttons |= Buttons.Start;
		}
		if ((bool)SDL.SDL_GetGamepadButton(num, SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_STICK))
		{
			buttons |= Buttons.LeftStick;
		}
		if ((bool)SDL.SDL_GetGamepadButton(num, SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_STICK))
		{
			buttons |= Buttons.RightStick;
		}
		if ((bool)SDL.SDL_GetGamepadButton(num, SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_SHOULDER))
		{
			buttons |= Buttons.LeftShoulder;
		}
		if ((bool)SDL.SDL_GetGamepadButton(num, SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_SHOULDER))
		{
			buttons |= Buttons.RightShoulder;
		}
		ButtonState upValue = ButtonState.Released;
		ButtonState downValue = ButtonState.Released;
		ButtonState leftValue = ButtonState.Released;
		ButtonState rightValue = ButtonState.Released;
		if ((bool)SDL.SDL_GetGamepadButton(num, SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_UP))
		{
			buttons |= Buttons.DPadUp;
			upValue = ButtonState.Pressed;
		}
		if ((bool)SDL.SDL_GetGamepadButton(num, SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_DOWN))
		{
			buttons |= Buttons.DPadDown;
			downValue = ButtonState.Pressed;
		}
		if ((bool)SDL.SDL_GetGamepadButton(num, SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_LEFT))
		{
			buttons |= Buttons.DPadLeft;
			leftValue = ButtonState.Pressed;
		}
		if ((bool)SDL.SDL_GetGamepadButton(num, SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_RIGHT))
		{
			buttons |= Buttons.DPadRight;
			rightValue = ButtonState.Pressed;
		}
		if ((bool)SDL.SDL_GetGamepadButton(num, SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_MISC1))
		{
			buttons |= Buttons.Misc1EXT;
		}
		if ((bool)SDL.SDL_GetGamepadButton(num, SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_PADDLE1))
		{
			buttons |= Buttons.Paddle1EXT;
		}
		if ((bool)SDL.SDL_GetGamepadButton(num, SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_PADDLE1))
		{
			buttons |= Buttons.Paddle2EXT;
		}
		if ((bool)SDL.SDL_GetGamepadButton(num, SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_PADDLE2))
		{
			buttons |= Buttons.Paddle3EXT;
		}
		if ((bool)SDL.SDL_GetGamepadButton(num, SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_PADDLE2))
		{
			buttons |= Buttons.Paddle4EXT;
		}
		if ((bool)SDL.SDL_GetGamepadButton(num, SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_TOUCHPAD))
		{
			buttons |= Buttons.TouchPadEXT;
		}
		GamePadState gamePadState = new GamePadState(new GamePadThumbSticks(leftPosition, rightPosition, deadZoneMode), new GamePadTriggers(leftTrigger, rightTrigger, deadZoneMode), new GamePadButtons(buttons), new GamePadDPad(upValue, downValue, leftValue, rightValue));
		gamePadState.IsConnected = true;
		gamePadState.PacketNumber = INTERNAL_states[index].PacketNumber;
		if (gamePadState != INTERNAL_states[index])
		{
			gamePadState.PacketNumber++;
			INTERNAL_states[index] = gamePadState;
		}
		return gamePadState;
	}

	public static bool SetGamePadVibration(int index, float leftMotor, float rightMotor)
	{
		nint num = INTERNAL_devices[index];
		if (num == IntPtr.Zero)
		{
			return false;
		}
		return SDL.SDL_RumbleGamepad(num, (ushort)(MathHelper.Clamp(leftMotor, 0f, 1f) * 65535f), (ushort)(MathHelper.Clamp(rightMotor, 0f, 1f) * 65535f), 0u);
	}

	public static bool SetGamePadTriggerVibration(int index, float leftTrigger, float rightTrigger)
	{
		nint num = INTERNAL_devices[index];
		if (num == IntPtr.Zero)
		{
			return false;
		}
		return SDL.SDL_RumbleGamepadTriggers(num, (ushort)(MathHelper.Clamp(leftTrigger, 0f, 1f) * 65535f), (ushort)(MathHelper.Clamp(rightTrigger, 0f, 1f) * 65535f), 0u);
	}

	public static string GetGamePadGUID(int index)
	{
		return INTERNAL_guids[index];
	}

	public static void SetGamePadLightBar(int index, Color color)
	{
		nint num = INTERNAL_devices[index];
		if (num != IntPtr.Zero)
		{
			SDL.SDL_SetGamepadLED(num, color.R, color.G, color.B);
		}
	}

	public unsafe static bool GetGamePadGyro(int index, out Vector3 gyro)
	{
		nint num = INTERNAL_devices[index];
		if (num == IntPtr.Zero)
		{
			gyro = Vector3.Zero;
			return false;
		}
		if (!SDL.SDL_GamepadSensorEnabled(num, SDL.SDL_SensorType.SDL_SENSOR_GYRO))
		{
			SDL.SDL_SetGamepadSensorEnabled(num, SDL.SDL_SensorType.SDL_SENSOR_GYRO, true);
		}
		float* ptr = stackalloc float[3];
		if (!SDL.SDL_GetGamepadSensorData(num, SDL.SDL_SensorType.SDL_SENSOR_GYRO, ptr, 3))
		{
			gyro = Vector3.Zero;
			return false;
		}
		gyro.X = *ptr;
		gyro.Y = ptr[1];
		gyro.Z = ptr[2];
		return true;
	}

	public unsafe static bool GetGamePadAccelerometer(int index, out Vector3 accel)
	{
		nint num = INTERNAL_devices[index];
		if (num == IntPtr.Zero)
		{
			accel = Vector3.Zero;
			return false;
		}
		if (!SDL.SDL_GamepadSensorEnabled(num, SDL.SDL_SensorType.SDL_SENSOR_ACCEL))
		{
			SDL.SDL_SetGamepadSensorEnabled(num, SDL.SDL_SensorType.SDL_SENSOR_ACCEL, true);
		}
		float* ptr = stackalloc float[3];
		if (!SDL.SDL_GetGamepadSensorData(num, SDL.SDL_SensorType.SDL_SENSOR_ACCEL, ptr, 3))
		{
			accel = Vector3.Zero;
			return false;
		}
		accel.X = *ptr;
		accel.Y = ptr[1];
		accel.Z = ptr[2];
		return true;
	}

	private static void INTERNAL_AddInstance(uint dev)
	{
		int num = -1;
		for (int i = 0; i < INTERNAL_devices.Length; i++)
		{
			if (INTERNAL_devices[i] == IntPtr.Zero)
			{
				num = i;
				break;
			}
		}
		if (num == -1)
		{
			return;
		}
		SDL.SDL_ClearError();
		INTERNAL_devices[num] = SDL.SDL_OpenGamepad(dev);
		nint joystick = SDL.SDL_GetGamepadJoystick(INTERNAL_devices[num]);
		uint key = SDL.SDL_GetJoystickID(joystick);
		if (INTERNAL_instanceList.ContainsKey(key))
		{
			INTERNAL_devices[num] = IntPtr.Zero;
			return;
		}
		INTERNAL_instanceList.Add(key, num);
		INTERNAL_states[num] = default(GamePadState);
		INTERNAL_states[num].IsConnected = true;
		bool flag = SDL.SDL_RumbleGamepad(INTERNAL_devices[num], 0, 0, 0u);
		bool hasTriggerVibrationMotorsEXT = SDL.SDL_RumbleGamepadTriggers(INTERNAL_devices[num], 0, 0, 0u);
		uint props = SDL.SDL_GetGamepadProperties(INTERNAL_devices[num]);
		GamePadCapabilities gamePadCapabilities = new GamePadCapabilities
		{
			IsConnected = true,
			GamePadType = INTERNAL_gamepadType[(int)SDL.SDL_GetJoystickType(joystick)],
			HasAButton = SDL.SDL_GamepadHasButton(INTERNAL_devices[num], SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_SOUTH),
			HasBButton = SDL.SDL_GamepadHasButton(INTERNAL_devices[num], SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_EAST),
			HasXButton = SDL.SDL_GamepadHasButton(INTERNAL_devices[num], SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_WEST),
			HasYButton = SDL.SDL_GamepadHasButton(INTERNAL_devices[num], SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_NORTH),
			HasBackButton = SDL.SDL_GamepadHasButton(INTERNAL_devices[num], SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_BACK),
			HasBigButton = SDL.SDL_GamepadHasButton(INTERNAL_devices[num], SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_GUIDE),
			HasStartButton = SDL.SDL_GamepadHasButton(INTERNAL_devices[num], SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_START),
			HasLeftStickButton = SDL.SDL_GamepadHasButton(INTERNAL_devices[num], SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_STICK),
			HasRightStickButton = SDL.SDL_GamepadHasButton(INTERNAL_devices[num], SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_STICK),
			HasLeftShoulderButton = SDL.SDL_GamepadHasButton(INTERNAL_devices[num], SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_SHOULDER),
			HasRightShoulderButton = SDL.SDL_GamepadHasButton(INTERNAL_devices[num], SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_SHOULDER),
			HasDPadUpButton = SDL.SDL_GamepadHasButton(INTERNAL_devices[num], SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_UP),
			HasDPadDownButton = SDL.SDL_GamepadHasButton(INTERNAL_devices[num], SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_DOWN),
			HasDPadLeftButton = SDL.SDL_GamepadHasButton(INTERNAL_devices[num], SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_LEFT),
			HasDPadRightButton = SDL.SDL_GamepadHasButton(INTERNAL_devices[num], SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_RIGHT),
			HasLeftXThumbStick = SDL.SDL_GamepadHasAxis(INTERNAL_devices[num], SDL.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTX),
			HasLeftYThumbStick = SDL.SDL_GamepadHasAxis(INTERNAL_devices[num], SDL.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTY),
			HasRightXThumbStick = SDL.SDL_GamepadHasAxis(INTERNAL_devices[num], SDL.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTX),
			HasRightYThumbStick = SDL.SDL_GamepadHasAxis(INTERNAL_devices[num], SDL.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTY),
			HasLeftTrigger = SDL.SDL_GamepadHasAxis(INTERNAL_devices[num], SDL.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFT_TRIGGER),
			HasRightTrigger = SDL.SDL_GamepadHasAxis(INTERNAL_devices[num], SDL.SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHT_TRIGGER),
			HasLeftVibrationMotor = flag,
			HasRightVibrationMotor = flag,
			HasVoiceSupport = false,
			HasLightBarEXT = SDL.SDL_GetBooleanProperty(props, "SDL.joystick.cap.rgb_led", false),
			HasTriggerVibrationMotorsEXT = hasTriggerVibrationMotorsEXT,
			HasMisc1EXT = SDL.SDL_GamepadHasButton(INTERNAL_devices[num], SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_MISC1),
			HasPaddle1EXT = SDL.SDL_GamepadHasButton(INTERNAL_devices[num], SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_PADDLE1),
			HasPaddle2EXT = SDL.SDL_GamepadHasButton(INTERNAL_devices[num], SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_PADDLE1),
			HasPaddle3EXT = SDL.SDL_GamepadHasButton(INTERNAL_devices[num], SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_PADDLE2),
			HasPaddle4EXT = SDL.SDL_GamepadHasButton(INTERNAL_devices[num], SDL.SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_PADDLE2),
			HasTouchPadEXT = (SDL.SDL_GetNumGamepadTouchpads(INTERNAL_devices[num]) > 0),
			HasGyroEXT = SDL.SDL_GamepadHasSensor(INTERNAL_devices[num], SDL.SDL_SensorType.SDL_SENSOR_GYRO),
			HasAccelerometerEXT = SDL.SDL_GamepadHasSensor(INTERNAL_devices[num], SDL.SDL_SensorType.SDL_SENSOR_ACCEL)
		};
		INTERNAL_capabilities[num] = gamePadCapabilities;
		ushort num2 = SDL.SDL_GetJoystickVendor(joystick);
		ushort num3 = SDL.SDL_GetJoystickProduct(joystick);
		if (num2 == 0 && num3 == 0)
		{
			INTERNAL_guids[num] = "xinput";
		}
		else
		{
			INTERNAL_guids[num] = $"{num2 & 0xFF:x2}{num2 >> 8:x2}{num3 & 0xFF:x2}{num3 >> 8:x2}";
		}
		if (num2 == 10462)
		{
			SDL.SDL_GamepadType sDL_GamepadType = SDL.SDL_GetGamepadType(INTERNAL_devices[num]);
			if (sDL_GamepadType == SDL.SDL_GamepadType.SDL_GAMEPAD_TYPE_XBOX360 || sDL_GamepadType == SDL.SDL_GamepadType.SDL_GAMEPAD_TYPE_XBOXONE)
			{
				INTERNAL_guids[num] = "xinput";
			}
			else
			{
				switch (sDL_GamepadType)
				{
				case SDL.SDL_GamepadType.SDL_GAMEPAD_TYPE_PS4:
					INTERNAL_guids[num] = "4c05c405";
					break;
				case SDL.SDL_GamepadType.SDL_GAMEPAD_TYPE_PS5:
					INTERNAL_guids[num] = "4c05e60c";
					break;
				}
			}
		}
		string text = SDL.SDL_GetGamepadMapping(INTERNAL_devices[num]);
		string text2 = ((!string.IsNullOrEmpty(text)) ? ("Mapping: " + text) : "Mapping not found");
		FNALoggerEXT.LogInfo("Controller " + num + ": " + SDL.SDL_GetGamepadName(INTERNAL_devices[num]) + ", GUID: " + INTERNAL_guids[num] + ", " + text2);
	}

	private static void INTERNAL_RemoveInstance(uint dev)
	{
		if (INTERNAL_instanceList.TryGetValue(dev, out var value))
		{
			INTERNAL_instanceList.Remove(dev);
			SDL.SDL_CloseGamepad(INTERNAL_devices[value]);
			INTERNAL_devices[value] = IntPtr.Zero;
			INTERNAL_states[value] = default(GamePadState);
			INTERNAL_guids[value] = string.Empty;
			SDL.SDL_ClearError();
			FNALoggerEXT.LogInfo("Removed device, player: " + value);
		}
	}

	private static string[] GenStringArray()
	{
		string[] array = new string[GamePad.GAMEPAD_COUNT];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = string.Empty;
		}
		return array;
	}

	public static TouchPanelCapabilities GetTouchCapabilities()
	{
		SDL.SDL_free(SDL.SDL_GetTouchDevices(out var count));
		bool flag = count > 0;
		return new TouchPanelCapabilities(flag, flag ? 4 : 0);
	}

	public unsafe static void UpdateTouchPanelState()
	{
		nint num = SDL.SDL_GetTouchFingers(GetTouchDeviceId(0), out var count);
		for (int i = 0; i < 8; i++)
		{
			if (i >= count)
			{
				TouchPanel.SetFinger(i, -1, Vector2.Zero);
				continue;
			}
			SDL.SDL_Finger* ptr = *(SDL.SDL_Finger**)(num + (nint)i * (nint)sizeof(SDL.SDL_Finger*));
			TouchPanel.SetFinger(i, (int)ptr->id, new Vector2((float)Math.Round(ptr->x * (float)TouchPanel.DisplayWidth), (float)Math.Round(ptr->y * (float)TouchPanel.DisplayHeight)));
		}
		SDL.SDL_free(num);
	}

	public static int GetNumTouchFingers()
	{
		SDL.SDL_free(SDL.SDL_GetTouchFingers(GetTouchDeviceId(0), out var count));
		return count;
	}

	private unsafe static ulong GetTouchDeviceId(int index)
	{
		nint num = SDL.SDL_GetTouchDevices(out var count);
		ulong result = ((index >= 0 && index < count) ? (*(ulong*)(num + (nint)index * (nint)8)) : 0);
		SDL.SDL_free(num);
		return result;
	}

	public static bool IsTextInputActive(nint window)
	{
		return SDL.SDL_TextInputActive(WrapWindow(window));
	}

	public static void StartTextInput(nint window)
	{
		SDL.SDL_StartTextInput(WrapWindow(window));
	}

	public static void StopTextInput(nint window)
	{
		SDL.SDL_StopTextInput(WrapWindow(window));
	}

	private static Keys ToXNAKey(ref uint sym, ref SDL.SDL_Scancode scancode)
	{
		Keys value;
		if (UseScancodes)
		{
			if (INTERNAL_scanMap.TryGetValue((int)scancode, out value))
			{
				return value;
			}
		}
		else if (INTERNAL_keyMap.TryGetValue((int)sym, out value))
		{
			return value;
		}
		FNALoggerEXT.LogWarn("KEY/SCANCODE MISSING FROM SDL3->XNA DICTIONARY: " + sym + " " + scancode);
		return Keys.None;
	}

	public static Keys GetKeyFromScancode(Keys scancode)
	{
		if (UseScancodes)
		{
			return scancode;
		}
		if (INTERNAL_xnaMap.TryGetValue((int)scancode, out var value))
		{
			uint key = SDL.SDL_GetKeyFromScancode(value, SDL.SDL_Keymod.SDL_KMOD_NONE, true);
			if (INTERNAL_keyMap.TryGetValue((int)key, out var value2))
			{
				return value2;
			}
			FNALoggerEXT.LogWarn("KEYCODE MISSING FROM SDL3->XNA DICTIONARY: " + key);
		}
		else
		{
			FNALoggerEXT.LogWarn("SCANCODE MISSING FROM XNA->SDL3 DICTIONARY: " + scancode);
		}
		return Keys.None;
	}

	private unsafe static bool Win32OnPaint(nint userdata, SDL.SDL_Event* evt)
	{
		if (evt->type == 516)
		{
			foreach (Game activeGame in activeGames)
			{
				if (activeGame.Window != null && evt->window.windowID == SDL.SDL_GetWindowID(activeGame.Window.Handle))
				{
					activeGame.RedrawWindow();
					return false;
				}
			}
		}
		if (prevEventFilter != null)
		{
			return prevEventFilter(userdata, evt);
		}
		return true;
	}
}
