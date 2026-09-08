using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Template;
using Quasar.Input;

namespace Quasar.GameUtils.OtherGames;

public class OtherGames
{
	public enum CameraPositions
	{
		Front,
		Back,
		Screenshot1,
		Screenshot2,
		Screenshot3,
		Screenshot4,
		Description,
		QR,
		Count
	}

	public class OtherGamesItem
	{
		private string boxArt;

		private List<string> screenshots;

		private string name;

		private int month;

		private int year;

		private int price;

		private string description;

		private string qr;

		private Vector3 color;

		public string BoxArt => boxArt;

		public int ScreenshotCount => screenshots.Count;

		public string Name => name;

		public int Month => month;

		public int Year => year;

		public int Price => price;

		public string Description => description;

		public string QR => qr;

		public Vector3 Color => color;

		public string Screenshot(int index)
		{
			return screenshots[index];
		}

		public OtherGamesItem(string name, string description, int month, int year, int price, string boxart, string qr, List<string> screenshots, Vector3 color)
		{
			this.name = name;
			this.description = description;
			boxArt = boxart;
			this.price = price;
			this.month = month;
			this.year = year;
			this.screenshots = screenshots;
			this.qr = qr;
			this.color = color;
		}
	}

	private CameraPositions currentCameraPosition;

	private List<OtherGamesItem> items = new List<OtherGamesItem>();

	private int currItem;

	public CameraPositions CurrentCameraPosition => currentCameraPosition;

	public bool IsShowingFront => currentCameraPosition == CameraPositions.Front;

	public int CurrentItemIndex => currItem;

	public List<OtherGamesItem> Items => items;

	public event Action OnCancel;

	public void AddItem(string name, string description, int month, int year, int price, string boxart, string qr, List<string> screenshots, Vector3 color)
	{
		items.Add(new OtherGamesItem(name, description, month, year, price, boxart, qr, screenshots, color));
	}

	public void AddItem(OtherGamesItem oti)
	{
		items.Add(oti);
	}

	private void ShowBack()
	{
		currentCameraPosition = CameraPositions.Back;
		GameTemplate.SelectorAudio.Start();
	}

	private void ShowFront()
	{
		currentCameraPosition = CameraPositions.Front;
		GameTemplate.InteractionAudio.Start();
	}

	private void NextItem(bool forceItem)
	{
		if (IsShowingFront || forceItem)
		{
			currItem = (currItem + 1) % Items.Count;
		}
		else
		{
			currentCameraPosition = (CameraPositions)(1 + (int)currentCameraPosition % 7);
		}
		GameTemplate.MoveAudio.Start();
	}

	private void PrevItem(bool forceItem)
	{
		if (IsShowingFront || forceItem)
		{
			currItem = (currItem - 1 + Items.Count) % Items.Count;
		}
		else
		{
			currentCameraPosition = (CameraPositions)(1 + (int)(currentCameraPosition - 2 + 8 - 1) % 7);
		}
		GameTemplate.MoveAudio.Start();
	}

	public void Update()
	{
		if (InputManager.MenuInteract())
		{
			if (IsShowingFront)
			{
				ShowBack();
			}
			else
			{
				NextItem(forceItem: false);
			}
		}
		if (InputManager.MenuCancel())
		{
			if (IsShowingFront)
			{
				if (OnCancel != null)
				{
					OnCancel();
				}
			}
			else
			{
				ShowFront();
			}
		}
		if (InputManager.MenuLeftRepeat())
		{
			PrevItem(forceItem: false);
		}
		if (InputManager.MenuRightRepeat())
		{
			NextItem(forceItem: false);
		}
		if (InputManager.MenuPrevPage())
		{
			PrevItem(forceItem: true);
		}
		if (InputManager.MenuNextPage())
		{
			NextItem(forceItem: true);
		}
	}
}
