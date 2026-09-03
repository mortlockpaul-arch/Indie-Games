using System;
using Core;
using Core.Inventory;
using Microsoft.Xna.Framework;
using SGSCore;

namespace TheMare1.Inventory.Items;

public class DreamRoom9SledgeHandle : Item
{
	public const string ID = "DreamRoom9SledgeHandle";

	public DreamRoom9SledgeHandle(Core.Game game)
		: base(game)
	{
		m_icon = game.m_CL.LoadTexture("Inventory/Items/DreamRoom9SledgeHandle/sledge_handle_thumb");
		onLoadExamine(game.m_CL);
		m_name = m_game.m_language.GetString("Wooden handle");
		m_desc = m_game.m_language.GetString("It's the handle of a sledgehammer.");
		m_id = "DreamRoom9SledgeHandle";
		m_combine_id.Add("DreamRoom3SledgeTop");
		m_combine_result_id.Add("Sledge");
	}

	public override void onLoadExamine(SGSContentLoader CL)
	{
		try
		{
			base.onLoadExamine(CL);
			if (m_examine_anim == null)
			{
				m_examine_anim = new TextureAnimation(m_game, CL.m_CM, "Inventory/Items/DreamRoom9SledgeHandle/Animation/", 1u, reverse: false);
				m_examine_anim.UseCombinedFrames(314, 41, 179, 2048);
				m_examine_anim.m_positioned = true;
				m_examine_anim.m_dest_rect = new Rectangle((int)Math.Round(404.5), (int)Math.Round(329.0), 471, 62);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}
