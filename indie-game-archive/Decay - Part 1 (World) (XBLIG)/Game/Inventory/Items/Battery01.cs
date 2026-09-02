using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Inventory.Items;

public class Battery01 : Item
{
	public const string ID = "Battery01";

	public Battery01(Game game, ContentManager CM)
		: base(game)
	{
		m_icon_large = CM.Load<Texture2D>("Inventory/Items/Battery01/battery");
		m_icon_medium = CM.Load<Texture2D>("Inventory/Items/Battery01/battery_medium");
		LoadExamineModel(CM, "Inventory/Items/Battery01/Model/");
		m_name = "Battery";
		m_desc = "It's a battery ...";
		m_id = "Battery01";
		m_combine_id.Add("Flashlight01");
		m_combine_result_id.Add("Flashlight02");
	}
}
