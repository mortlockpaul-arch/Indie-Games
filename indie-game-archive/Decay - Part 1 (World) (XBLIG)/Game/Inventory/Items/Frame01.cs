using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Inventory.Items;

public class Frame01 : Item
{
	public const string ID = "Frame01";

	public Frame01(Game game, ContentManager CM)
		: base(game)
	{
		m_icon_large = CM.Load<Texture2D>("Inventory/Items/Frame01/frame");
		m_icon_medium = CM.Load<Texture2D>("Inventory/Items/Frame01/frame_medium");
		LoadExamineModel(CM, "Inventory/Items/Frame01/Model/");
		m_name = "Frame";
		m_desc = "Looks like some odd building ...";
		m_id = "Frame01";
	}
}
