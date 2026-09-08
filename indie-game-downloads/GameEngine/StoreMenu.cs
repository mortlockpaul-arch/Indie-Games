using DataContent;
using EGEngine;

namespace GameEngine;

public class StoreMenu : GameMenuScreenCls
{
	public StoreMenu()
	{
		Entry entry = new Entry();
		entry.cost = 20;
		entry.message = "$" + entry.cost + " Sleeping Bag";
		entry.messagesize = Menu.defaultFont.MeasureString(entry.message);
		entry.SelectedFunction += BuySleepingBagFunc;
		MenuEntries.Add(entry);
		entry = new Entry();
		entry.cost = 100;
		entry.message = "$" + entry.cost + " Camping Supplies";
		entry.messagesize = Menu.defaultFont.MeasureString(entry.message);
		entry.SelectedFunction += BuyCampingSuppliesFunc;
		MenuEntries.Add(entry);
		entry = new Entry();
		entry.cost = 400;
		entry.message = "$" + entry.cost + " HD Cam Recorder";
		entry.messagesize = Menu.defaultFont.MeasureString(entry.message);
		entry.SelectedFunction += BuyHDCamRecorderFunc;
		MenuEntries.Add(entry);
		entry = new Entry();
		entry.cost = 800;
		entry.message = "$" + entry.cost + " Night Vision Goggles";
		entry.messagesize = Menu.defaultFont.MeasureString(entry.message);
		entry.SelectedFunction += BuyNightVisionGogglesFunc;
		MenuEntries.Add(entry);
		entry = new Entry();
		entry.cost = 1200;
		entry.message = "$" + entry.cost + " Thermal Camera";
		entry.messagesize = Menu.defaultFont.MeasureString(entry.message);
		entry.SelectedFunction += BuyThermalCameraFunc;
		MenuEntries.Add(entry);
		entry = new Entry();
		entry.cost = 2000;
		entry.message = "$" + entry.cost + " 30 Cal. Hunting Rifle";
		entry.messagesize = Menu.defaultFont.MeasureString(entry.message);
		entry.SelectedFunction += BuyHuntingRifleFunc;
		MenuEntries.Add(entry);
		entry = new Entry();
		entry.cost = 4500;
		entry.message = "$" + entry.cost + " 4x4 Off Road Vehicle";
		entry.messagesize = Menu.defaultFont.MeasureString(entry.message);
		entry.SelectedFunction += BuyOffRoadVehicleFunc;
		MenuEntries.Add(entry);
		entry = new Entry();
		entry.message = "Exit";
		entry.messagesize = Menu.defaultFont.MeasureString(entry.message);
		entry.SelectedFunction += ExitFunc;
		MenuEntries.Add(entry);
		TextJustify = 1;
	}

	public void Update(PlayerBase playerRef, int qIndex, bool canAccessMenu)
	{
		if (!canAccessMenu)
		{
			base.Update(playerRef, qIndex);
		}
	}

	public void DrawPost(PlayerBase playerRef, int qIndex, bool canAccessMenu)
	{
		if (!canAccessMenu)
		{
			base.DrawPost(playerRef, qIndex);
		}
	}

	private void BuyOffRoadVehicleFunc(object obj, Entry e)
	{
		if (SimpleZombieAI.TotalMonies >= e.cost)
		{
			e.valid = false;
			NextSelected();
			SimpleZombieAI.OffRoadVehicle = true;
			SimpleZombieAI.TotalMonies -= e.cost;
			GenericMessages.Add("This Vehicle Will Get You to Good Hunting Areas", 6);
			LevelObjectives.IssueCallbackFunc(6);
		}
	}

	private void BuyHuntingRifleFunc(object obj, Entry e)
	{
		if (SimpleZombieAI.TotalMonies >= e.cost)
		{
			e.valid = false;
			NextSelected();
			SimpleZombieAI.HuntingRifle = true;
			SimpleZombieAI.TotalMonies -= e.cost;
			GenericMessages.Add("This Will Do The Job", 6);
			LevelObjectives.IssueCallbackFunc(6);
			LevelObjectives.Add("Kill A Deer To Use As Bait For Bigfoot", float.MaxValue, GameObjectives.GenericCallBackFunc);
			LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].fpsWeapon.SetWeapon(WeaponType.USA);
		}
	}

	private void BuyThermalCameraFunc(object obj, Entry e)
	{
		if (SimpleZombieAI.TotalMonies >= e.cost)
		{
			e.valid = false;
			NextSelected();
			SimpleZombieAI.ThermalCamera = true;
			SimpleZombieAI.TotalMonies -= e.cost;
			GenericMessages.Add("Nothing Can Hide From This Camera $$$", 6);
			LevelObjectives.IssueCallbackFunc(6);
			LevelObjectives.Add("Get Thermal Video Of Bigfoot", float.MaxValue, GameObjectives.GenericCallBackFunc);
			LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].fpsWeapon.SetWeapon(WeaponType.ThermalCamera);
		}
	}

	private void BuyNightVisionGogglesFunc(object obj, Entry e)
	{
		if (SimpleZombieAI.TotalMonies >= e.cost)
		{
			e.valid = false;
			NextSelected();
			SimpleZombieAI.NVGoogles = true;
			SimpleZombieAI.TotalMonies -= e.cost;
			GenericMessages.Add("You Can Track At Night With NV Goggles", 6);
			LevelObjectives.IssueCallbackFunc(6);
		}
	}

	private void BuyHDCamRecorderFunc(object obj, Entry e)
	{
		if (SimpleZombieAI.TotalMonies >= e.cost)
		{
			e.valid = false;
			NextSelected();
			SimpleZombieAI.HDCamRecorder = true;
			SimpleZombieAI.TotalMonies -= e.cost;
			GenericMessages.Add("You Can Film Video Now", 6);
			LevelObjectives.IssueCallbackFunc(6);
			LevelObjectives.Add("Get Video Of Bigfoot", float.MaxValue, GameObjectives.GenericCallBackFunc);
			LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].fpsWeapon.SetWeapon(WeaponType.HDCamera);
		}
	}

	private void BuyCampingSuppliesFunc(object obj, Entry e)
	{
		if (SimpleZombieAI.TotalMonies >= e.cost)
		{
			e.valid = false;
			NextSelected();
			SimpleZombieAI.CampingSupplies = true;
			SimpleZombieAI.TotalMonies -= e.cost;
			GenericMessages.Add("You Can Hunt Indefinatley Now", 6);
			if (!SimpleZombieAI.HDCamRecorder && !SimpleZombieAI.ThermalCamera)
			{
				LevelObjectives.IssueCallbackFunc(6);
				LevelObjectives.Add("Get Cell Phone Picture Of Bigfoot", float.MaxValue, GameObjectives.GenericCallBackFunc);
			}
		}
	}

	private void BuySleepingBagFunc(object obj, Entry e)
	{
		if (SimpleZombieAI.TotalMonies >= e.cost)
		{
			e.valid = false;
			NextSelected();
			SimpleZombieAI.SleepingBag = true;
			SimpleZombieAI.TotalMonies -= e.cost;
			GenericMessages.Add("You Can Hunt At Night Now", 6);
			if (!SimpleZombieAI.HDCamRecorder && !SimpleZombieAI.ThermalCamera)
			{
				LevelObjectives.IssueCallbackFunc(6);
				LevelObjectives.Add("Get Cell Phone Picture Of Bigfoot", float.MaxValue, GameObjectives.GenericCallBackFunc);
			}
		}
	}

	private void ExitFunc(object obj, Entry e)
	{
		Timer = 1f;
		State = GMSCState.TransitionOff;
	}
}
