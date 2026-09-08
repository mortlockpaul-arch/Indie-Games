using Microsoft.Xna.Framework;
using Quasar.Global;

namespace Quasar.GUI.Controls;

public abstract class GroupControl : Control
{
	protected bool focused;

	protected Vector2 size;

	protected Vector2 position;

	private Group group;

	public bool Focused
	{
		get
		{
			return focused;
		}
		set
		{
			focused = value;
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

	public Layout2D.LayoutData LayoutData => new Layout2D.LayoutData(position, size);

	public Group Group => group;

	public abstract void Update();

	public abstract bool CheckClick(ClickType type, Vector2 position);

	public GroupControl(Layout layout, Group group)
		: base(layout)
	{
		this.group = group;
	}
}
