using Quasar.GUI.Controls;

namespace Quasar.GUI.Elements;

public class GroupNode : Element
{
	private Group group;

	public Group Group => group;

	public GroupNode(Group group)
	{
		this.group = group;
		group.OnControlAdded += group_OnControlAdded;
	}

	private void group_OnControlAdded(Group arg1, GroupControl arg2)
	{
		addChild(TemplateManager.Templates[arg1.Layout.Template].CreateGroupControl(arg2, arg1));
	}

	protected override void DoUpdate()
	{
		base.Visible = group.Visible;
		base.DoUpdate();
	}
}
