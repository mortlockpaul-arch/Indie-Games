using System;
using Core;
using Core.Inventory;
using Microsoft.Xna.Framework;
using SGSCore;

namespace TheMare1.Inventory.Items;

public class Room2BloodyKey : Item
{
	public const string ID = "Room2BloodyKey";

	public Room2BloodyKey(Core.Game game)
		: base(game)
	{
		m_icon = game.m_CL.LoadTexture("Inventory/Items/Room2BloodyKey/key_blood_thumb");
		onLoadExamine(game.m_CL);
		m_name = m_game.m_language.GetString("Key");
		m_desc = m_game.m_language.GetString("A bloodstained key.");
		m_id = "Room2BloodyKey";
	}

	public override void onLoadExamine(SGSContentLoader CL)
	{
		try
		{
			base.onLoadExamine(CL);
			if (m_examine_anim == null)
			{
				m_examine_anim = new TextureAnimation(m_game, CL.m_CM, "Inventory/Items/Room2BloodyKey/Animation/", 2u, reverse: false);
				m_examine_anim.UseCombinedFrames(213, 105, 179, 2048);
				m_examine_anim.m_positioned = true;
				m_examine_anim.m_dest_rect = new Rectangle((int)Math.Round(480.0), (int)Math.Round(281.0) + 40, 320, 158);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}
