using Quasar.GUI;

namespace AvatarFarmOnline.Template.Controls;

internal class JoinGameStatus : Control
{
	public const string Type = "JoinInvited";

	public override string ControlType => "JoinInvited";

	public JoinGameStatus(Layout layout)
		: base(layout)
	{
	}
}
