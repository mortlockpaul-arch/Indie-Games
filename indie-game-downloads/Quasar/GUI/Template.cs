using System;
using Microsoft.Xna.Framework;
using Quasar.GUI.Controls;
using Quasar.GUI.Controls.GroupControls;
using Quasar.GUI.Elements;

namespace Quasar.GUI;

public abstract class Template : IDisposable
{
	public abstract GroupControlElement CreateGroupControl(GroupControl control, Group group);

	public virtual LayoutElement CreateElement(Layout layout)
	{
		LayoutElement layoutElement = new LayoutElement(layout);
		foreach (Control control in layout.Controls)
		{
			Element element = CreateControl(control);
			if (element != null)
			{
				layoutElement.addChild(element);
			}
		}
		return layoutElement;
	}

	public virtual GroupNode CreateGroup(Group group)
	{
		GroupNode groupNode = new GroupNode(group);
		foreach (GroupControl control in group.Controls)
		{
			groupNode.addChild(CreateGroupControl(control, group));
		}
		return groupNode;
	}

	public abstract DialogNode CreateDialog(Layout layout, Dialog dialog);

	public abstract TextInputNode CreateTextInputDialog(Layout layout, TextInputDialog dialog);

	public virtual Element CreateControl(Control control)
	{
		string controlType;
		if ((controlType = control.ControlType) != null && controlType == "Group")
		{
			return CreateGroup((Group)control);
		}
		return null;
	}

	public virtual Vector2 GetButtonSize(Button button)
	{
		return new Vector2(256f, 60f);
	}

	public virtual void Dispose()
	{
	}
}
