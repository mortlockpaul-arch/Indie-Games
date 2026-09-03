using System;
using Core;
using Core.Inventory;
using Microsoft.Xna.Framework;
using SGSCore;

namespace TheMare1.Inventory.Items;

public class Sledge : Item
{
	public const string ID = "Sledge";

	public Sledge(Core.Game game)
		: base(game)
	{
		m_icon = game.m_CL.LoadTexture("Inventory/Items/Sledge/sledge_both_thumb");
		onLoadExamine(game.m_CL);
		m_name = m_game.m_language.GetString("Sledgehammer");
		m_desc = "";
		m_id = "Sledge";
	}

	public override void onLoadExamine(SGSContentLoader CL)
	{
		try
		{
			base.onLoadExamine(CL);
			if (m_examine_anim == null)
			{
				m_examine_anim = new TextureAnimation(m_game, CL.m_CM, "Inventory/Items/Sledge/Animation/", 3u, reverse: false);
				m_examine_anim.UseCombinedFrames(365, 123, 179, 2048);
				m_examine_anim.m_positioned = true;
				m_examine_anim.m_dest_rect = new Rectangle((int)Math.Round(366.0), (int)Math.Round(268.0), 548, 184);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}
