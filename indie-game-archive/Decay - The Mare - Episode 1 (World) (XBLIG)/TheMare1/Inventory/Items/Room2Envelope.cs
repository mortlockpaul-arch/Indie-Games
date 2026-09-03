using System;
using Core;
using Core.Inventory;
using Microsoft.Xna.Framework;
using SGSCore;

namespace TheMare1.Inventory.Items;

public class Room2Envelope : Item
{
	public const string ID = "Room2Envelope";

	public Room2Envelope(Core.Game game)
		: base(game)
	{
		m_icon = game.m_CL.LoadTexture("Inventory/Items/Envelope/envelope_thumb");
		onLoadExamine(game.m_CL);
		m_name = m_game.m_language.GetString("Envelope");
		m_desc = m_game.m_language.GetString("A bloodstained envelope.");
		m_id = "Room2Envelope";
		m_examine_use_text = m_game.m_language.GetString("OPEN");
	}

	public override void onLoadExamine(SGSContentLoader CL)
	{
		try
		{
			base.onLoadExamine(CL);
			if (m_examine_anim == null)
			{
				m_examine_anim = new TextureAnimation(m_game, CL.m_CM, "Inventory/Items/Envelope/Animation/", 3u, reverse: false);
				m_examine_anim.UseCombinedFrames(224, 208, 179, 2048);
				m_examine_anim.m_positioned = true;
				m_examine_anim.m_dest_rect = new Rectangle((int)Math.Round(472.0), (int)Math.Round(204.0) - 20, 336, 312);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public override void onExamineUse()
	{
		try
		{
			m_game.m_inventory.ReplaceExamineItem("Room2BloodyKey");
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}
