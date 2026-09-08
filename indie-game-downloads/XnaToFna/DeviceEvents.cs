using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using XnaToFna.ProxyForms;

namespace XnaToFna;

public static class DeviceEvents
{
	public enum Events
	{
		DBT_CONFIGCHANGECANCELED = 25,
		DBT_CONFIGCHANGED = 24,
		DBT_CUSTOMEVENT = 32774,
		DBT_DEVICEARRIVAL = 32768,
		DBT_DEVICEQUERYREMOVE = 32769,
		DBT_DEVICEQUERYREMOVEFAILED = 32770,
		DBT_DEVICEREMOVECOMPLETE = 32772,
		DBT_DEVICEREMOVEPENDING = 32771,
		DBT_DEVICETYPESPECIFIC = 32773,
		DBT_DEVNODES_CHANGED = 7,
		DBT_QUERYCHANGECONFIG = 23,
		DBT_USERDEFINED = 65535
	}

	public static bool[] IsGamepadConnected = new bool[0];

	public static void DeviceChange(Events e, IntPtr data)
	{
		PInvoke.CallHooks(Messages.WM_DEVICECHANGE, (IntPtr)(int)e, data, global: true, window: true, allWindows: true);
	}

	public static void GamepadConnected(int i)
	{
		DeviceChange(Events.DBT_DEVICEARRIVAL, IntPtr.Zero);
	}

	public static void GamepadDisconnected(int i)
	{
		DeviceChange(Events.DBT_DEVICEREMOVECOMPLETE, IntPtr.Zero);
	}

	public static void Update()
	{
		for (int i = 0; i < IsGamepadConnected.Length; i++)
		{
			bool isConnected = GamePad.GetState((PlayerIndex)i).IsConnected;
			if (isConnected && !IsGamepadConnected[i])
			{
				GamepadConnected(i);
			}
			else if (!isConnected && IsGamepadConnected[i])
			{
				GamepadDisconnected(i);
			}
			IsGamepadConnected[i] = isConnected;
		}
	}
}
