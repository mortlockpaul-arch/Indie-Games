using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Inventory.Items;

public class Polygrip01 : Item
{
	public const string ID = "Polygrip01";

	public Polygrip01(Game game, ContentManager CM)
		: base(game)
	{
		m_icon_large = CM.Load<Texture2D>("Inventory/Items/Polygrip01/polygrip");
		m_icon_medium = CM.Load<Texture2D>("Inventory/Items/Polygrip01/polygrip_medium");
		LoadExamineModel(CM, "Inventory/Items/Polygrip01/Model/");
		m_name = "Slip Joint Pliers";
		m_desc = "This tool will get a nice grip.";
		m_id = "Polygrip01";
	}
}
