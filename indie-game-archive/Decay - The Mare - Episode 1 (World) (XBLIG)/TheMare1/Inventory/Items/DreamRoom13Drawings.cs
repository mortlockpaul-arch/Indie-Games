using System;
using Core;
using Core.Inventory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TheMare1.Inventory.Items;

public class DreamRoom13Drawings : Item
{
	public const string ID = "DreamRoom13Drawings";

	public DreamRoom13Drawings(Core.Game game, ContentManager CM)
		: base(game)
	{
		m_icon = CM.Load<Texture2D>("Inventory/Items/DreamRoom13Drawings/all_flowers");
		m_bundle_ids.Add("PaperArrow");
		m_bundle_ids.Add("PaperEye");
		m_bundle_ids.Add("PaperFlame");
		m_bundle_ids.Add("PaperTear");
		m_name = m_game.m_language.GetString("Drawings");
		m_desc = "";
		m_id = "DreamRoom13Drawings";
	}

	public override void DrawAskPickupImage(SpriteBatch SB, Color color)
	{
		try
		{
			if (m_icon != null)
			{
				Rectangle destinationRectangle = new Rectangle((int)Math.Round((float)(Core.Game.VIEW_RECT.Width - m_icon.Width) * 0.5f), (int)Math.Round((float)(Core.Game.VIEW_RECT.Height - m_icon.Height) * 0.5f), m_icon.Width, m_icon.Height);
				SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				SB.Draw(m_icon, destinationRectangle, color);
				SB.End();
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}
