using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Inventory.Items;

public class Note02 : Item
{
	public const string ID = "Note02";

	public Note02(Game game, ContentManager CM)
		: base(game)
	{
		m_icon_large = CM.Load<Texture2D>("Inventory/Items/Note02/letter_leftroom");
		m_icon_medium = CM.Load<Texture2D>("Inventory/Items/Note02/letter_leftroom_medium");
		m_examine_image = CM.Load<Texture2D>("Inventory/Items/Note02/note_bloodspurting");
		m_name = "Note";
		m_desc = "";
		m_id = "Note02";
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
}
