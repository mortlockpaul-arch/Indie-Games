using System;
using Core;
using Core.Inventory;
using Microsoft.Xna.Framework;
using SGSCore;

namespace TheMare1.Inventory.Items;

public class DreamRoom3SledgeTop : Item
{
	public const string ID = "DreamRoom3SledgeTop";

	public DreamRoom3SledgeTop(Core.Game game)
		: base(game)
	{
		m_icon = game.m_CL.LoadTexture("Inventory/Items/DreamRoom3SledgeTop/sledge_top_thumb");
		onLoadExamine(game.m_CL);
		m_name = m_game.m_language.GetString("Metal block");
		m_desc = m_game.m_language.GetString("It's the head of a sledgehammer.");
		m_id = "DreamRoom3SledgeTop";
		m_combine_id.Add("DreamRoom9SledgeHandle");
		m_combine_result_id.Add("Sledge");
	}

	public override void onLoadExamine(SGSContentLoader CL)
	{
		try
		{
			base.onLoadExamine(CL);
			if (m_examine_anim == null)
			{
				m_examine_anim = new TextureAnimation(m_game, CL.m_CM, "Inventory/Items/DreamRoom3SledgeTop/Animation/", 2u, reverse: false);
				m_examine_anim.UseCombinedFrames(189, 139, 179, 2048);
				m_examine_anim.m_positioned = true;
				m_examine_anim.m_dest_rect = new Rectangle((int)Math.Round(498.0), (int)Math.Round(256.0), 284, 208);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}
