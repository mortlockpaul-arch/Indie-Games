using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Inventory.Items;

public class Flashlight02 : Item
{
	public const string ID = "Flashlight02";

	public Flashlight02(Game game, ContentManager CM)
		: base(game)
	{
		m_icon_large = CM.Load<Texture2D>("Inventory/Items/Flashlight01/flashlight");
		m_icon_medium = CM.Load<Texture2D>("Inventory/Items/Flashlight01/flashlight_medium");
		m_examine_model_from_item = "Flashlight01";
		m_name = "Flashlight";
		m_desc = "Flashlight with battery.";
		m_id = "Flashlight02";
	}

	public override void DrawExamineModel(SpriteBatch SB, RenderTarget2D RT, Color color)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (((Color)(ref color)).A >= byte.MaxValue)
		{
			SB.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
			SB.GraphicsDevice.RenderState.AlphaBlendEnable = false;
			SB.GraphicsDevice.RenderState.AlphaTestEnable = true;
			SB.Draw(RT.GetTexture(), Game.VIEW_RECT, color);
			SB.End();
		}
		else
		{
			base.DrawExamineModel(SB, RT, color);
		}
	}
}
