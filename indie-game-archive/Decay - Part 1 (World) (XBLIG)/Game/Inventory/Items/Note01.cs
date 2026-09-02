using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Inventory.Items;

public class Note01 : Item
{
	public const string ID = "Note01";

	public Note01(Game game, ContentManager CM)
		: base(game)
	{
		m_icon_large = CM.Load<Texture2D>("Inventory/Items/Note01/letter_rightroom");
		m_icon_medium = CM.Load<Texture2D>("Inventory/Items/Note01/letter_rightroom_medium");
		m_examine_image = CM.Load<Texture2D>("Inventory/Items/Note01/note_whydididoit");
		m_use_scrolling = true;
		m_min_scroll_y = (float)((Rectangle)(ref Game.TS_AREA)).Bottom - 861.60004f;
		m_name = "Letter";
		m_desc = "";
		m_id = "Note01";
	}

	public override void DrawAskPickupImage(SpriteBatch SB, Color color)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (m_icon_large != null)
		{
			Rectangle val = default(Rectangle);
			((Rectangle)(ref val))._002Ector((Game.VIEW_RECT.Width - m_icon_large.Width * 2) / 2, (Game.VIEW_RECT.Height - m_icon_large.Height * 2) / 2, m_icon_large.Width * 2, m_icon_large.Height * 2);
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_icon_large, val, color);
			SB.End();
		}
	}

	public override void DrawExamineImage(SpriteBatch SB, Color color)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		float num = 1.2f;
		Rectangle val = default(Rectangle);
		((Rectangle)(ref val))._002Ector((int)((float)Game.VIEW_RECT.Width - 616f * num) / 2, (int)m_game.m_inventory.m_scroll_y, (int)(616f * num), (int)(718f * num));
		SB.Begin((SpriteBlendMode)1);
		SB.Draw(m_examine_image, val, color);
		SB.End();
	}
}
