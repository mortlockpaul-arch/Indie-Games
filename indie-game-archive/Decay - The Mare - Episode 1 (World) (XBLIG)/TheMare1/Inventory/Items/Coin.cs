using System;
using Core;
using Core.Inventory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SGSCore;

namespace TheMare1.Inventory.Items;

public class Coin : Item
{
	public const string ID = "Coin";

	public Coin(Core.Game game, ContentManager CM)
		: base(game)
	{
		m_icon = CM.Load<Texture2D>("Inventory/Items/Coin/coin_thumb");
		onLoadExamine(game.m_CL);
		m_name = m_game.m_language.GetString("Coin");
		m_pickup_name = m_game.m_language.GetString("Coin");
		m_desc = "";
		m_id = "Coin";
	}

	public override void onLoadExamine(SGSContentLoader CL)
	{
		try
		{
			base.onLoadExamine(CL);
			if (m_examine_anim == null)
			{
				m_examine_anim = new TextureAnimation(m_game, CL.m_CM, "Inventory/Items/Coin/Animation/", 2u, reverse: false);
				m_examine_anim.UseCombinedFrames(175, 174, 179, 2048);
				m_examine_anim.m_positioned = true;
				m_examine_anim.m_dest_rect = new Rectangle((int)Math.Round(531.0), (int)Math.Round(248.5), 218, 223);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}
