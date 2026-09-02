using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Inventory.Items;

public class Pincett01 : Item
{
	public const string ID = "Pincett01";

	public Pincett01(Game game, ContentManager CM)
		: base(game)
	{
		m_icon_large = CM.Load<Texture2D>("Inventory/Items/Pincett01/pincett");
		m_icon_medium = CM.Load<Texture2D>("Inventory/Items/Pincett01/pincett_medium");
		LoadExamineModel(CM, "Inventory/Items/Pincett01/Model/");
		m_name = "Tweezer";
		m_desc = "A tweezer, found in the bathroom.";
		m_id = "Pincett01";
	}
}
