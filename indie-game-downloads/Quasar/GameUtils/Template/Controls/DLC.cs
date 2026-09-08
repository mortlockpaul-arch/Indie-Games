using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.Input;

namespace Quasar.GameUtils.Template.Controls;

public class DLC : InteractiveControl
{
	public class DLCItem
	{
		private int targetSales;

		private string title;

		private string description;

		private bool unlocked;

		public int TargetSales => targetSales;

		public string Title => title;

		public string Description => description;

		public bool Unlocked => unlocked;

		public DLCItem(string title, string description, int targetSales, bool unlocked)
		{
			this.title = title;
			this.description = description;
			this.targetSales = targetSales;
			this.unlocked = unlocked;
		}
	}

	public const string Type = "DLC";

	private List<DLCItem> items = new List<DLCItem>();

	private int currPage;

	private int itemsPerPage = 4;

	public override string ControlType => "DLC";

	public int CurrentPage => currPage;

	public int ItemsPerPage => itemsPerPage;

	public List<DLCItem> Items => items;

	public int NumPages => (items.Count - 1) / itemsPerPage + 1;

	public event Action OnChange;

	public event Func<bool> OnCancel;

	public event Action OnCancelled;

	public void AddItem(DLCItem item)
	{
		items.Add(item);
	}

	public override bool CheckClick(ClickType type, Vector2 position)
	{
		return false;
	}

	public DLC(int itemsPerPage, Layout layout)
		: base(layout)
	{
		this.itemsPerPage = itemsPerPage;
	}

	public List<DLCItem> GetCurrentItems()
	{
		return items.GetRange(itemsPerPage * currPage, Math.Min(itemsPerPage, items.Count - itemsPerPage * currPage));
	}

	public override void Update()
	{
		int num = currPage;
		if ((InputManager.MenuRightRepeat() || InputManager.MenuDownRepeat() || InputManager.MenuInteract()) && currPage < NumPages - 1)
		{
			currPage++;
		}
		if ((InputManager.MenuLeftRepeat() || InputManager.MenuUpRepeat()) && currPage > 0)
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
