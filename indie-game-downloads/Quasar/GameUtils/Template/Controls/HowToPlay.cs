using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.Input;

namespace Quasar.GameUtils.Template.Controls;

public class HowToPlay : InteractiveControl
{
	public class HowToPlayItem
	{
		private string image;

		private string text;

		public string Image => image;

		public bool HasImage => image.Length > 0;

		public string Text => text;

		public HowToPlayItem(string text, string image)
		{
			this.image = image;
			this.text = text;
		}
	}

	public delegate void HowToPlayHandler();

	public const string Type = "HowToPlay";

	private List<HowToPlayItem> items = new List<HowToPlayItem>();

	private int currItem;

	private Vector2 position;

	private Vector2 size;

	public override string ControlType => "HowToPlay";

	public int CurrentIndex => currItem;

	public List<HowToPlayItem> Items => items;

	public HowToPlayItem CurrentItem => items[currItem];

	public Vector2 Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
		}
	}

	public Vector2 Size
	{
		get
		{
			return size;
		}
		set
		{
			size = value;
		}
	}

	public int ItemsCount => items.Count;

	public event HowToPlayHandler OnItemChanged;

	public event Action OnCancelled;

	public override bool CheckClick(ClickType type, Vector2 position)
	{
		return false;
	}

	private void InvokeEvent()
	{
		if (OnItemChanged != null)
		{
			OnItemChanged();
		}
	}

	public void MoveNext()
	{
		int num = Math.Min(currItem + 1, ItemsCount - 1);
		if (currItem != num)
		{
			currItem = num;
			InvokeEvent();
		}
	}

	public void MovePrevious()
	{
		int num = Math.Max(0, currItem - 1);
		if (currItem != num)
		{
			currItem = num;
			InvokeEvent();
		}
	}

	public HowToPlay(Layout layout)
		: base(layout)
	{
	}

	public void AddItem(string text, string image)
	{
		items.Add(new HowToPlayItem(text, image));
	}

	public void AddItem(HowToPlayItem howtoPlay)
	{
		items.Add(howtoPlay);
	}

	public override void Update()
	{
		if (InputManager.MenuInteract() || InputManager.MenuRightRepeat())
		{
			MoveNext();
		}
		if (InputManager.MenuLeftRepeat())
		{
			MovePrevious();
		}
		if (InputManager.MenuCancel() && OnCancelled != null)
		{
			OnCancelled();
		}
	}
}
