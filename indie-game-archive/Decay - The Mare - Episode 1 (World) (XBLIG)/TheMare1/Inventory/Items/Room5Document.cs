using System;
using Core;
using Core.Inventory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SGSCore;

namespace TheMare1.Inventory.Items;

public class Room5Document : Item
{
	public const string ID = "Room5Document";

	public Room5Document(Core.Game game, ContentManager CM)
		: base(game)
	{
		m_icon = CM.Load<Texture2D>("Inventory/Items/Room5Document/namelist_document");
		onLoadExamine(game.m_CL);
		m_use_scrolling = true;
		m_min_scroll_y = (float)Core.Game.TS_AREA.Bottom - 1280f;
		m_name = m_game.m_language.GetString("Note");
		m_desc = "";
		m_id = "Room5Document";
	}

	public override void onLoadExamine(SGSContentLoader CL)
	{
		try
		{
			base.onLoadExamine(CL);
			if (m_examine_image == null)
			{
				m_examine_image = CL.m_CM.Load<Texture2D>("Inventory/Items/Room5Document/document_namelist");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public override void DrawExamineImage(SpriteBatch SB, Color color)
	{
		float num = 1f;
		Rectangle destinationRectangle = new Rectangle((int)((float)Core.Game.VIEW_RECT.Width - 724f * num) / 2, (int)m_game.m_inventory.m_scroll_y, (int)(724f * num), (int)(1280f * num));
		SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
		SB.Draw(m_examine_image, destinationRectangle, color);
		SB.End();
	}
}
