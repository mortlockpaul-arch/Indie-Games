using System;
using Core;
using Core.Inventory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SGSCore;

namespace TheMare1.Inventory.Items;

public class DreamRoom4Document : Item
{
	public const string ID = "DreamRoom4Document";

	public DreamRoom4Document(Core.Game game, ContentManager CM)
		: base(game)
	{
		m_icon = CM.Load<Texture2D>("Inventory/Items/DreamRoom4Document/text_thumb");
		onLoadExamine(game.m_CL);
		m_use_scrolling = true;
		m_min_scroll_y = (float)Core.Game.TS_AREA.Bottom - 1024f;
		m_name = m_game.m_language.GetString("Drawing");
		m_desc = "";
		m_id = "DreamRoom4Document";
	}

	public override void onLoadExamine(SGSContentLoader CL)
	{
		try
		{
			base.onLoadExamine(CL);
			if (m_examine_image == null)
			{
				m_examine_image = CL.m_CM.Load<Texture2D>("Inventory/Items/DreamRoom4Document/poetry_poetry");
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
		Rectangle destinationRectangle = new Rectangle((int)((float)Core.Game.VIEW_RECT.Width - 720f * num) / 2, (int)m_game.m_inventory.m_scroll_y, (int)(720f * num), (int)(1024f * num));
		SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
		SB.Draw(m_examine_image, destinationRectangle, color);
		SB.End();
	}
}
