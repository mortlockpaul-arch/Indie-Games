using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

public class Menu
{
	public List<MenuItem> items;

	public int activeItem;

	private string upCue;

	private string downCue;

	public MenuItem ActiveItem => items[activeItem];

	public Menu()
		: this("moveUp", "moveDown")
	{
	}

	public Menu(string _upCue, string _downCue)
	{
		items = new List<MenuItem>();
		upCue = _upCue;
		downCue = _downCue;
	}

	public void Draw(GameTime gametime)
	{
		foreach (MenuItem item in items)
		{
			item.Draw(gametime);
		}
	}

	public void Update(GameTime gametime)
	{
		foreach (MenuItem item in items)
		{
			item.Update(gametime);
		}
	}

	public MenuItem AddItem(MenuItem mi)
	{
		items.Add(mi);
		if (items.Count > 1)
		{
			items[items.Count - 2].next = items[items.Count - 1];
			items[items.Count - 1].prev = items[items.Count - 2];
		}
		else
		{
			items[0].selected = true;
		}
		items[items.Count - 1].next = items[0];
		items[0].prev = items[items.Count - 1];
		return items[items.Count - 1];
	}

	public MenuItem Add(string _name, Vector2 _pos, Color _normal, Color _select, string _command)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return AddItem(new MenuItem(this, _name, _pos, _normal, _select, _command));
	}

	public MenuItem AddDisabled(string _name, Vector2 _pos, Color _normal, Color _select, string _command)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return AddItem(new MenuItemDisabled(this, _name, _pos, _normal, _select, _command));
	}

	public MenuItem Add(string _name, Vector2 _pos, Color _normal, Color _select, string _command, OptionChangedEvent updateFunc, string option1, string option2)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		MenuItemOption menuItemOption = (MenuItemOption)AddItem(new MenuItemOption(this, _name, _pos, _normal, _select, _command, updateFunc));
		menuItemOption.options = new List<string>();
		menuItemOption.options.Add(option1);
		menuItemOption.options.Add(option2);
		return menuItemOption;
	}

	public void MoveUp()
	{
		items[activeItem].selected = false;
		BaseGame.Get().PlayCue(upCue);
		MenuItem prev = items[activeItem].prev;
		while (prev is MenuItemDisabled)
		{
			prev = prev.prev;
		}
		activeItem = items.IndexOf(prev);
		prev.selected = true;
	}

	public void MoveDown()
	{
		items[activeItem].selected = false;
		BaseGame.Get().PlayCue(downCue);
		MenuItem next = items[activeItem].next;
		while (next is MenuItemDisabled)
		{
			next = next.next;
		}
		activeItem = items.IndexOf(next);
		next.selected = true;
	}
}
