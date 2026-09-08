using Quasar.GUI.Controls;
using Quasar.GUI.Elements;

namespace Quasar.GameUtils.Template;

public class GameGroupNode : GroupNode
{
	public GameGroupNode(Group group)
		: base(group)
	{
		group.OnFocusChange += OnFocusChange;
	}

	private void OnFocusChange(Group group)
	{
		GameTemplate.MoveAudio.Start();
	}
}
