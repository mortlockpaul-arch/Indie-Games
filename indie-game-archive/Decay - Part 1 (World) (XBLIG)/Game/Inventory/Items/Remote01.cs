using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Inventory.Items;

public class Remote01 : Item
{
	public const string ID = "Remote01";

	public Remote01(Game game, ContentManager CM)
		: base(game)
	{
		m_icon_large = CM.Load<Texture2D>("Inventory/Items/Remote01/remote");
		m_icon_medium = CM.Load<Texture2D>("Inventory/Items/Remote01/remote_medium");
		LoadExamineModel(CM, "Inventory/Items/Remote01/Model/");
		m_name = "Remote";
		m_desc = "A television remote controller ...";
		m_id = "Remote01";
	}
}
