using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;
using RuntimeXNA.Actions;
using RuntimeXNA.Conditions;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Extensions;

internal class CRunXNA : CRunExtension
{
	private const int CND_TRIAL = 0;

	private const int CND_BACK = 1;

	private const int CND_DEACTIVATED = 2;

	private const int CND_REACTIVATED = 3;

	private const int CND_MUSICPLAYING = 4;

	private const int CND_LAST = 5;

	private const int ACT_OPENURL = 0;

	private const int ACT_VIBRATE = 1;

	private const int ACT_SETPLAYER = 2;

	private const int ACT_SETDEVICESELECTOR = 3;

	private const int ACT_OPENDEVICESELECTOR = 4;

	private const int FLAG_SIMULATE_TRIAL = 1;

	private object stateobj;

	private int flags;

	public override int getNumberOfConditions()
	{
		return 5;
	}

	public override bool createRunObject(CFile file, CCreateObjectInfo cob, int version)
	{
		flags = file.readAInt();
		if ((flags & 1) != 0)
		{
			Guide.SimulateTrialMode = true;
		}
		else
		{
			Guide.SimulateTrialMode = false;
		}
		return true;
	}

	public override void destroyRunObject(bool bFast)
	{
	}

	public override bool condition(int num, CCndExtension cnd)
	{
		return num switch
		{
			0 => Guide.IsTrialMode, 
			1 => cndBackPressed(), 
			2 => true, 
			3 => true, 
			4 => false, 
			_ => false, 
		};
	}

	public bool cndBackPressed()
	{
		return false;
	}

	public override void action(int num, CActExtension act)
	{
		switch (num)
		{
		case 0:
			openURL(act);
			break;
		case 1:
			vibrate(act);
			break;
		case 2:
			setPlayer(act);
			break;
		case 3:
			setDeviceSelector(act);
			break;
		case 4:
			openDeviceSelector(act);
			break;
		}
	}

	private void setDeviceSelector(CActExtension act)
	{
		switch (act.getParamExpression(rh, 0))
		{
		case 1:
			rh.deviceSelectorPlayer = PlayerIndex.One;
			break;
		case 2:
			rh.deviceSelectorPlayer = PlayerIndex.Two;
			break;
		case 3:
			rh.deviceSelectorPlayer = PlayerIndex.Three;
			break;
		case 4:
			rh.deviceSelectorPlayer = PlayerIndex.Four;
			break;
		}
	}

	private void openDeviceSelector(CActExtension act)
	{
		if (ho.hoAdRunHeader.rhApp.storageDevice == null)
		{
			stateobj = "Please choose a device";
			try
			{
				StorageDevice.BeginShowSelector(rh.deviceSelectorPlayer, GetDevice, stateobj);
			}
			catch (Exception ex)
			{
				ex.GetType();
			}
		}
	}

	private void GetDevice(IAsyncResult result)
	{
		ho.hoAdRunHeader.rhApp.storageDevice = StorageDevice.EndShowSelector(result);
	}

	private void openURL(CActExtension act)
	{
	}

	private void vibrate(CActExtension act)
	{
	}

	private void setPlayer(CActExtension act)
	{
		int paramExpression = act.getParamExpression(rh, 0);
		int paramExpression2 = act.getParamExpression(rh, 1);
		if (paramExpression >= 1 && paramExpression <= 4)
		{
			ho.hoAdRunHeader.rhApp.getCtrlType()[paramExpression - 1] = (short)(paramExpression2 | 0x80);
		}
	}
}
