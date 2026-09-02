using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Inventory.Items;

public class Key01 : Item
{
	public const string ID = "Key01";

	public Key01(Game game, ContentManager CM)
		: base(game)
	{
		m_icon_large = CM.Load<Texture2D>("Inventory/Items/Key01/key");
		m_icon_medium = CM.Load<Texture2D>("Inventory/Items/Key01/key_medium");
		LoadExamineModel(CM, "Inventory/Items/Key01/Model/");
		m_name = "Rusty Key";
		m_desc = "It's a small rusty key ...";
		m_id = "Key01";
	}
}
