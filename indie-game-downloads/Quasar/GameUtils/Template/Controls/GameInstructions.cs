using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.Input;

namespace Quasar.GameUtils.Template.Controls;

public class GameInstructions : InteractiveControl
{
	public const string Type = "GameInstructions";

	private List<GameInstructionsItem> items = new List<GameInstructionsItem>();

	private int currPage;

	private int itemsPerPage = 3;

	public override string ControlType => "GameInstructions";

	public int CurrentPage => currPage;

	public int ItemsPerPage => itemsPerPage;

	public List<GameInstructionsItem> Items => items;

	public int NumPages => (items.Count - 1) / itemsPerPage + 1;

	public event GameInstructionsBoolEvent OnCancel;

	public event GameInstructionsEvent OnChange;

	public event GameInstructionsEvent OnCancelled;

	public override bool CheckClick(ClickType type, Vector2 position)
	{
		return false;
	}

	public GameInstructions(int itemsPerPage, Layout layout)
		: base(layout)
	{
		this.itemsPerPage = itemsPerPage;
	}

	public List<GameInstructionsItem> GetCurrentItems()
	{
		return items.GetRange(itemsPerPage * currPage, Math.Min(itemsPerPage, items.Count - itemsPerPage * currPage));
	}

	public void AddItem(string text, Texture2D image)
	{
		items.Add(new GameInstructionsItem(text, image));
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
