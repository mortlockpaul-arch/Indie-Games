using System.Collections.Generic;
using Eyehook.Framework;
using Loot.Core;
using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Loot.Screens;

public abstract class MenuBoxScreen : Screen
{
	protected enum Align
	{
		Left,
		Center
	}

	protected abstract class MenuItem
	{
		public virtual string Name => null;

		public virtual string Value => null;

		public virtual Align Align => Align.Left;

		public virtual void Increment()
		{
		}

		public virtual void Decrement()
		{
		}

		public virtual void Click()
		{
		}
	}

	protected class BlankMenuItem : MenuItem
	{
	}

	private List<MenuItem> MenuItems = new List<MenuItem>();

	private string title;

	private int currentOption;

	private Color defaultColor = new Color(0, 0, 0);

	private Color selectedColor = new Color(176, 0, 0);

	private int boxWidth;

	private int valueOffset;

	protected Rectangle BoxRect;

	protected MenuBoxScreen(string title, int boxWidth, int valueOffset)
		: base(modal: true)
	{
		this.title = title;
		this.boxWidth = boxWidth;
		this.valueOffset = valueOffset;
	}

	protected void SetCurrentOption(int i)
	{
		currentOption = i;
	}

	protected void ClearMenuItems()
	{
		MenuItems.Clear();
	}

	protected void AddMenuItem(MenuItem menuItem)
	{
		MenuItems.Add(menuItem);
		BoxRect = new Rectangle(640 - boxWidth / 2, 320 - MenuItems.Count * 20, boxWidth, 80 + MenuItems.Count * 40);
		if (title != null)
		{
			BoxRect.Y += 18;
		}
	}

	public override void update(GameTime gameTime)
	{
		if (MC.GamePadManager.isNewButtonDown(Buttons.A))
		{
			MenuItems[currentOption].Click();
		}
		else if (MC.GamePadManager.isNewDirLeft())
		{
			MenuItems[currentOption].Decrement();
		}
		else if (MC.GamePadManager.isNewDirRight())
		{
			MenuItems[currentOption].Increment();
		}
		else if (MC.GamePadManager.isNewDirDown())
		{
			PlaySound.MenuMove();
			do
			{
				currentOption++;
				if (currentOption >= MenuItems.Count)
				{
					currentOption = 0;
				}
			}
			while (MenuItems[currentOption] is BlankMenuItem);
		}
		else
		{
			if (!MC.GamePadManager.isNewDirUp())
			{
				return;
			}
			PlaySound.MenuMove();
			do
			{
				currentOption--;
				if (currentOption < 0)
				{
					currentOption = MenuItems.Count - 1;
				}
			}
			while (MenuItems[currentOption] is BlankMenuItem);
		}
	}

	public override void draw(GameTime gameTime)
	{
		base.spriteBatch.Begin();
		MenuBox.Draw(base.spriteBatch, BoxRect, title);
		for (int i = 0; i < MenuItems.Count; i++)
		{
			Color color = ((currentOption == i) ? selectedColor : defaultColor);
			string name = MenuItems[i].Name;
			string value = MenuItems[i].Value;
			Vector2 v = new Vector2(BoxRect.Left + 56, BoxRect.Top + 60 + i * 40);
			if (name == null)
			{
				continue;
			}
			if (MenuItems[i].Align == Align.Left)
			{
				Text.Draw(base.spriteBatch, v, name, color);
				if (value != null)
				{
					v.X += valueOffset;
					Text.Draw(base.spriteBatch, v, value, color);
				}
			}
			else
			{
				v.X = 640f;
				Text.DrawCentered(base.spriteBatch, v, name, color);
			}
		}
		base.spriteBatch.End();
	}
}
