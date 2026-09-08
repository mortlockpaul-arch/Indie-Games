using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.GameUtils.Stats;
using Quasar.Input;

namespace Quasar.GameUtils.Template.Controls;

public class Stats : InteractiveControl
{
	public const string Type = "Stats";

	private List<StatProgress> items = new List<StatProgress>();

	private int currPage;

	private int itemsPerPage = 8;

	private PlayerIndex playerIndex;

	public override string ControlType => "Stats";

	public int CurrentPage => currPage;

	public int ItemsPerPage => itemsPerPage;

	public List<StatProgress> Items => items;

	public PlayerIndex PlayerIndex => playerIndex;

	public int NumPages => (items.Count - 1) / itemsPerPage + 1;

	public event Action OnChange;

	public event Func<bool> OnCancel;

	public event Action OnCancelled;

	public Stats(int itemsPerPage, Layout layout, PlayerIndex playerIndex)
		: base(layout)
	{
		this.playerIndex = playerIndex;
		this.itemsPerPage = itemsPerPage;
		PlayerStatProgress playerProgress = StatsManager.Instance.GetPlayerProgress(playerIndex);
		items = new List<StatProgress>(playerProgress.Awards.Values);
	}

	public override bool CheckClick(ClickType type, Vector2 position)
	{
		return false;
	}

	public List<StatProgress> GetCurrentItems()
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
