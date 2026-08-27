using System;
using System.Collections.Generic;

namespace EGEngine;

public class MenuMgr
{
	public List<Menu> MenuList = new List<Menu>();

	private static List<ScheduledTask> TaskList = new List<ScheduledTask>();

	public void LoadContent()
	{
		AddMenu(new StartMenu(GameMenus.Start));
		MakeActive(GameMenus.Start);
	}

	public void AddMenu(Menu m)
	{
		m.LoadContent();
		MenuList.Add(m);
	}

	public void MakeActive(GameMenus id)
	{
		TransitionActiveOff();
		for (int i = 0; i < MenuList.Count; i++)
		{
			if (MenuList[i].menuId == id)
			{
				MenuList[i].MakeActive(this);
				break;
			}
		}
	}

	public void HideAll()
	{
		for (int i = 0; i < MenuList.Count; i++)
		{
			MenuList[i].State = MenuState.Hidden;
		}
	}

	public void TransitionActiveOff()
	{
		for (int i = 0; i < MenuList.Count; i++)
		{
			if (MenuList[i].IsActive)
			{
				MenuList[i].State = MenuState.TransitionOff;
			}
		}
	}

	public void SetBackMenuFunction(GameMenus id, EventHandler<MenuEntry> e)
	{
		for (int i = 0; i < MenuList.Count; i++)
		{
			if (MenuList[i].menuId == id)
			{
				MenuList[i].BackMenuDelegate += e;
				break;
			}
		}
	}

	public void Update(float eTime)
	{
		for (int i = 0; i < MenuList.Count; i++)
		{
			if (MenuList[i].IsActive)
			{
				MenuList[i].Update(eTime);
			}
		}
	}

	public void Draw()
	{
		if (TaskList.Count > 0)
		{
			for (int i = 0; i < TaskList.Count; i++)
			{
				TaskList[i].RunTask();
			}
			TaskList.Clear();
		}
		int count = MenuList.Count;
		for (int j = 0; j < count; j++)
		{
			if (MenuList[j].IsActive)
			{
				MenuList[j].Draw();
			}
		}
	}

	public bool IsActive(GameMenus m)
	{
		int count = MenuList.Count;
		for (int i = 0; i < count; i++)
		{
			if (MenuList[i].menuId == m)
			{
				return MenuList[i].IsActive;
			}
		}
		return false;
	}

	public Menu GetMenu(GameMenus m)
	{
		int count = MenuList.Count;
		for (int i = 0; i < count; i++)
		{
			if (MenuList[i].menuId == m)
			{
				return MenuList[i];
			}
		}
		return null;
	}

	public static void AddTask(ScheduledTask e)
	{
		TaskList.Add(e);
	}
}
