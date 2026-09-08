using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.Input;

namespace Quasar.GameUtils.Template.Controls;

public class Credits : InteractiveControl
{
	public const string Type = "Credits";

	private List<CreditsItem> items = new List<CreditsItem>();

	private int currPage;

	private int itemsPerPage = 4;

	public override string ControlType => "Credits";

	public int CurrentPage => currPage;

	public int ItemsPerPage => itemsPerPage;

	public List<CreditsItem> Items => items;

	public int NumPages => (items.Count - 1) / itemsPerPage + 1;

	public event CreditsEvent OnChange;

	public event CreditsBoolEvent OnCancel;

	public event CreditsEvent OnCancelled;

	public override bool CheckClick(ClickType type, Vector2 position)
	{
		return false;
	}

	public Credits(int itemsPerPage, Layout layout)
		: base(layout)
	{
		this.itemsPerPage = itemsPerPage;
	}

	public List<CreditsItem> GetCurrentItems()
	{
		return items.GetRange(itemsPerPage * currPage, Math.Min(itemsPerPage, items.Count - itemsPerPage * currPage));
	}

	public void AddItem(string name, string gamertag, string image, CreditsFunctions functions)
	{
		items.Add(new CreditsItem(name, gamertag, functions, image));
	}

	public void AddItem(CreditsItem ci)
	{
		items.Add(ci);
	}

	public void AddItem(string name, string image, string longDesc)
	{
		items.Add(new CreditsItem(name, longDesc, image));
	}

	public override void Update()
	{
		int num = currPage;
		if ((InputManager.MenuRightRepeat() || InputManager.MenuInteract()) && currPage < NumPages - 1)
		{
			currPage++;
		}
		if (InputManager.MenuLeftRepeat() && currPage > 0)
		{
			currPage--;
		}
		if (num != currPage && OnChange != null)
		{
			OnChange();
		}
		if (InputManager.MenuCancel() && OnCancel != null && OnCancel() && OnCancelled != null)
		{
			OnCancelled();
		}
	}

	public override void Dispose()
	{
		OnCancel = null;
		OnCancelled = null;
		OnChange = null;
		base.Dispose();
	}
}
