using Microsoft.Xna.Framework;
using Quasar.GUI.Controls;

namespace Quasar.GUI.Elements;

public class GroupControlElement : RenderItem
{
	private bool ignorePosition;

	private GroupControl groupControl;

	public bool IgnorePosition
	{
		set
		{
			ignorePosition = value;
		}
	}

	public GroupControlElement(GroupControl gc)
	{
		groupControl = gc;
	}

	protected override void DoUpdate()
	{
		if (!ignorePosition)
		{
			base.Transform.Translation = new Vector3(groupControl.Position, 0f);
		}
		base.DoUpdate();
	}
}
