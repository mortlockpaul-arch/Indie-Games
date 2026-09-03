using System;
using Core;
using Core.Inventory;
using Microsoft.Xna.Framework;
using SGSCore;

namespace TheMare1.Inventory.Items;

public class DreamRoom10PuppyKey : Item
{
	public const string ID = "DreamRoom10PuppyKey";

	public DreamRoom10PuppyKey(Core.Game game)
		: base(game)
	{
		m_icon = game.m_CL.LoadTexture("Inventory/Items/DreamRoom10PuppyKey/key_puppy_thumb");
		onLoadExamine(game.m_CL);
		m_name = m_game.m_language.GetString("Key");
		m_desc = "";
		m_id = "DreamRoom10PuppyKey";
	}

	public override void onLoadExamine(SGSContentLoader CL)
	{
		try
		{
			base.onLoadExamine(CL);
			if (m_examine_anim == null)
			{
				m_examine_anim = new TextureAnimation(m_game, CL.m_CM, "Inventory/Items/DreamRoom10PuppyKey/Animation/", 2u, reverse: false);
				m_examine_anim.UseCombinedFrames(226, 184, 179, 2048);
				m_examine_anim.m_positioned = true;
				m_examine_anim.m_dest_rect = new Rectangle((int)Math.Round(470.0), (int)Math.Round(222.0) + 40, 340, 276);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}
