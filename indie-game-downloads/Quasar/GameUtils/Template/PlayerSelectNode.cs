using Microsoft.Xna.Framework;
using Quasar.GameUtils.Template.Controls;

namespace Quasar.GameUtils.Template;

public class PlayerSelectNode : Element
{
	private const float POS_Y = 10f;

	private const float OFFSET_Y = 120f;

	private const float OFFSET_X = 240f;

	private PlayerSelect playerSelect;

	protected virtual PlayerSelectItem CreateItem(PlayerSelect playerSelect, PlayerIndex playerIndex)
	{
		return new PlayerSelectItem(playerSelect, playerIndex);
	}

	public PlayerSelectNode(PlayerSelect playerSelect)
	{
		this.playerSelect = playerSelect;
		transform.Translation = new Vector3(0f, 10f, 0f);
		for (int i = 0; i < 4; i++)
		{
			PlayerSelectItem playerSelectItem = CreateItem(playerSelect, (PlayerIndex)i);
			playerSelectItem.Transform.Translation = new Vector3((i % 2 == 0) ? (-240f) : 240f, (i / 2 == 0) ? 120f : (-120f), 0f);
			addChild(playerSelectItem);
		}
	}

	protected override void DoUpdate()
	{
		base.DoUpdate();
	}
}
