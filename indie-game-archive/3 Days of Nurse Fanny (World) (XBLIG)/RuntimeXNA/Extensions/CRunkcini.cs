using System;
using Microsoft.Xna.Framework.Storage;
using RuntimeXNA.Actions;
using RuntimeXNA.Expressions;
using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Extensions;

internal class CRunkcini : CRunExtension
{
	public const int INI_UTF8 = 8;

	private int saveCounter;

	private CIni ini;

	private short iniFlags;

	private string iniName;

	private string iniCurrentGroup;

	private string iniCurrentItem;

	private object stateobj;

	public override int getNumberOfConditions()
	{
		return 0;
	}

	private void cleanName()
	{
		int num = iniName.LastIndexOf('\\');
		if (num < 0)
		{
			num = iniName.LastIndexOf('/');
		}
		if (num >= 0 && num + 1 < iniName.Length)
		{
			iniName = iniName.Substring(num + 1);
		}
	}

	public override bool createRunObject(CFile file, CCreateObjectInfo cob, int version)
	{
		iniFlags = file.readAShort();
		iniName = file.readAString();
		if (iniName.Length == 0)
		{
			iniName = "Default.ini";
		}
		cleanName();
		ini = new CIni(this, iniFlags);
		saveCounter = 0;
		iniCurrentGroup = "Group";
		iniCurrentItem = "Item";
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
		return false;
	}

	private void GetDevice(IAsyncResult result)
	{
		ho.hoAdRunHeader.rhApp.storageDevice = StorageDevice.EndShowSelector(result);
	}

	public override void destroyRunObject(bool bFast)
	{
		ini.saveIni();
	}

	public override int handleRunObject()
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
		if (saveCounter > 0)
		{
			saveCounter--;
			if (saveCounter <= 0)
			{
				saveCounter = 0;
				ini.saveIni();
			}
		}
		return 0;
	}

	public override void action(int num, CActExtension act)
	{
		switch (num)
		{
		case 0:
			SetCurrentGroup(act);
			break;
		case 1:
			SetCurrentItem(act);
			break;
		case 2:
			SetValue(act);
			break;
		case 3:
			SavePosition(act);
			break;
		case 4:
			LoadPosition(act);
			break;
		case 5:
			SetString(act);
			break;
		case 6:
			SetCurrentFile(act);
			break;
		case 7:
			SetValueItem(act);
			break;
		case 8:
			SetValueGroupItem(act);
			break;
		case 9:
			SetStringItem(act);
			break;
		case 10:
			SetStringGroupItem(act);
			break;
		case 11:
			DeleteItem(act);
			break;
		case 12:
			DeleteGroupItem(act);
			break;
		case 13:
			DeleteGroup(act);
			break;
		}
	}

	private void SetCurrentGroup(CActExtension act)
	{
		iniCurrentGroup = act.getParamExpString(rh, 0);
	}

	private void SetCurrentItem(CActExtension act)
	{
		iniCurrentItem = act.getParamExpString(rh, 0);
	}

	private void SetValue(CActExtension act)
	{
		string name = act.getParamExpression(rh, 0).ToString();
		ini.writePrivateProfileString(iniCurrentGroup, iniCurrentItem, name, iniName);
		saveCounter = 50;
	}

	private void SavePosition(CActExtension act)
	{
		CObject paramObject = act.getParamObject(rh, 0);
		string name = paramObject.hoX + "," + paramObject.hoY;
		string keyName = "pos." + paramObject.hoOiList.oilName;
		ini.writePrivateProfileString(iniCurrentGroup, keyName, name, iniName);
		saveCounter = 50;
	}

	private void LoadPosition(CActExtension act)
	{
		CObject paramObject = act.getParamObject(rh, 0);
		string keyName = "pos." + paramObject.hoOiList.oilName;
		string privateProfileString = ini.getPrivateProfileString(iniCurrentGroup, keyName, "X", iniName);
		if (string.Compare(privateProfileString, "X") != 0)
		{
			int num = privateProfileString.IndexOf(",");
			string value = privateProfileString.Substring(0, num);
			string value2 = privateProfileString.Substring(num + 1);
			try
			{
				paramObject.hoX = Convert.ToInt32(value, 10);
			}
			catch (FormatException ex)
			{
				ex.GetType();
			}
			catch (ArgumentOutOfRangeException ex2)
			{
				ex2.GetType();
			}
			try
			{
				paramObject.hoY = Convert.ToInt32(value2, 10);
			}
			catch (FormatException ex3)
			{
				ex3.GetType();
			}
			catch (ArgumentOutOfRangeException ex4)
			{
				ex4.GetType();
			}
			paramObject.roc.rcChanged = true;
			paramObject.roc.rcCheckCollides = true;
		}
	}

	private void SetString(CActExtension act)
	{
		string paramExpString = act.getParamExpString(rh, 0);
		ini.writePrivateProfileString(iniCurrentGroup, iniCurrentItem, paramExpString, iniName);
		saveCounter = 50;
	}

	private void SetCurrentFile(CActExtension act)
	{
		iniName = act.getParamExpString(rh, 0);
		cleanName();
	}

	private void SetValueItem(CActExtension act)
	{
		string paramExpString = act.getParamExpString(rh, 0);
		string name = act.getParamExpression(rh, 1).ToString();
		ini.writePrivateProfileString(iniCurrentGroup, paramExpString, name, iniName);
		saveCounter = 50;
	}

	private void SetValueGroupItem(CActExtension act)
	{
		string paramExpString = act.getParamExpString(rh, 0);
		string paramExpString2 = act.getParamExpString(rh, 1);
		string name = act.getParamExpression(rh, 2).ToString();
		ini.writePrivateProfileString(paramExpString, paramExpString2, name, iniName);
		saveCounter = 50;
	}

	private void SetStringItem(CActExtension act)
	{
		string paramExpString = act.getParamExpString(rh, 0);
		string paramExpString2 = act.getParamExpString(rh, 1);
		ini.writePrivateProfileString(iniCurrentGroup, paramExpString, paramExpString2, iniName);
		saveCounter = 50;
	}

	private void SetStringGroupItem(CActExtension act)
	{
		string paramExpString = act.getParamExpString(rh, 0);
		string paramExpString2 = act.getParamExpString(rh, 1);
		string paramExpString3 = act.getParamExpString(rh, 2);
		ini.writePrivateProfileString(paramExpString, paramExpString2, paramExpString3, iniName);
		saveCounter = 50;
	}

	private void DeleteItem(CActExtension act)
	{
		ini.deleteItem(iniCurrentGroup, act.getParamExpString(rh, 0), iniName);
		saveCounter = 50;
	}

	private void DeleteGroupItem(CActExtension act)
	{
		ini.deleteItem(act.getParamExpString(rh, 0), act.getParamExpString(rh, 1), iniName);
		saveCounter = 50;
	}

	private void DeleteGroup(CActExtension act)
	{
		ini.deleteGroup(act.getParamExpString(rh, 0), iniName);
		saveCounter = 50;
	}

	public override CValue expression(int num)
	{
		return num switch
		{
			0 => GetValue(), 
			1 => GetString(), 
			2 => GetValueItem(), 
			3 => GetValueGroupItem(), 
			4 => GetStringItem(), 
			5 => GetStringGroupItem(), 
			_ => null, 
		};
	}

	private CValue GetValue()
	{
		string privateProfileString = ini.getPrivateProfileString(iniCurrentGroup, iniCurrentItem, "", iniName);
		int i = 0;
		if (privateProfileString.Length > 0)
		{
			try
			{
				i = Convert.ToInt32(privateProfileString, 10);
			}
			catch (FormatException ex)
			{
				ex.GetType();
			}
			catch (ArgumentOutOfRangeException ex2)
			{
				ex2.GetType();
			}
		}
		return new CValue(i);
	}

	private CValue GetString()
	{
		string privateProfileString = ini.getPrivateProfileString(iniCurrentGroup, iniCurrentItem, "", iniName);
		return new CValue(privateProfileString);
	}

	private CValue GetValueItem()
	{
		string keyName = ho.getExpParam().getString();
		string privateProfileString = ini.getPrivateProfileString(iniCurrentGroup, keyName, "", iniName);
		int i = 0;
		if (privateProfileString.Length > 0)
		{
			try
			{
				i = Convert.ToInt32(privateProfileString, 10);
			}
			catch (FormatException ex)
			{
				ex.GetType();
			}
			catch (ArgumentOutOfRangeException ex2)
			{
				ex2.GetType();
			}
		}
		return new CValue(i);
	}

	private CValue GetValueGroupItem()
	{
		string sectionName = ho.getExpParam().getString();
		string keyName = ho.getExpParam().getString();
		string privateProfileString = ini.getPrivateProfileString(sectionName, keyName, "", iniName);
		int i = 0;
		if (privateProfileString.Length > 0)
		{
			try
			{
				i = Convert.ToInt32(privateProfileString, 10);
			}
			catch (FormatException ex)
			{
				ex.GetType();
			}
			catch (ArgumentOutOfRangeException ex2)
			{
				ex2.GetType();
			}
		}
		return new CValue(i);
	}

	private CValue GetStringItem()
	{
		string keyName = ho.getExpParam().getString();
		string privateProfileString = ini.getPrivateProfileString(iniCurrentGroup, keyName, "", iniName);
		return new CValue(privateProfileString);
	}

	private CValue GetStringGroupItem()
	{
		string sectionName = ho.getExpParam().getString();
		string keyName = ho.getExpParam().getString();
		string privateProfileString = ini.getPrivateProfileString(sectionName, keyName, "", iniName);
		return new CValue(privateProfileString);
	}
}
