using System;
using Core;
using Core.Inventory;
using Microsoft.Xna.Framework;
using SGSCore;

namespace TheMare1.Inventory.Items;

public class DreamRoom6Photo : Item
{
	public const string ID = "DreamRoom6Photo";

	private TextureAnimation m_examine_anim2;

	public DreamRoom6Photo(Core.Game game)
		: base(game)
	{
		m_icon = game.m_CL.LoadTexture("Inventory/Items/DreamRoom6Photo/photo_thumb");
		onLoadExamine(game.m_CL);
		m_name = m_game.m_language.GetString("Photo");
		m_desc = "";
		m_id = "DreamRoom6Photo";
	}

	public override void Clear()
	{
		base.Clear();
		if (m_examine_anim2 != null)
		{
			m_examine_anim2.Clear();
			m_examine_anim2 = null;
		}
	}

	public override void Reset()
	{
		try
		{
			base.Reset();
			if (m_game.m_game_data.GetState("DreamRoom6Photo.NoPaper") == "1")
			{
				HandleNoPaper();
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public override void onLoadExamine(SGSContentLoader CL)
	{
		try
		{
			base.onLoadExamine(CL);
			if (m_examine_anim == null)
			{
				m_examine_anim = new TextureAnimation(m_game, CL.m_CM, "Inventory/Items/DreamRoom6Photo/Animations/WithPaper/", 3u, reverse: false);
				m_examine_anim.UseCombinedFrames(192, 248, 179, 2048);
				m_examine_anim.m_positioned = true;
				m_examine_anim.m_dest_rect = new Rectangle((int)Math.Round(496.0), (int)Math.Round(174.0), 288, 372);
				m_examine_anim2 = new TextureAnimation(m_game, CL.m_CM, "Inventory/Items/DreamRoom6Photo/Animations/NoPaper/", 3u, reverse: false);
				m_examine_anim2.UseCombinedFrames(192, 248, 179, 2048);
				m_examine_anim2.m_positioned = true;
				m_examine_anim2.m_dest_rect = new Rectangle((int)Math.Round(496.0) - 1, (int)Math.Round(174.0), 287, 372);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public override void Update(TimeSpan elapsed)
	{
		try
		{
			base.Update(elapsed);
			if (m_examine_anim != null)
			{
				m_examine_use_text = "";
				if (!(m_game.m_game_data.GetState("DreamRoom6Photo.NoPaper") == "1") && m_examine_anim.m_current_frame >= 80 && m_examine_anim.m_current_frame <= 100)
				{
					m_examine_use_text = m_game.m_language.GetString("DETACH PAPER");
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	private void HandleNoPaper()
	{
		try
		{
			if (m_examine_anim != m_examine_anim2)
			{
				int current_frame = m_examine_anim.m_current_frame;
				if (m_examine_anim != null)
				{
					m_examine_anim.Clear();
					m_examine_anim = null;
				}
				m_examine_anim = m_examine_anim2;
				m_examine_anim.SetFrame(current_frame);
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
			base.onExamineUse();
			if (!(m_examine_use_text == ""))
			{
				m_examine_use_text = "";
				m_game.m_game_data.SetState("DreamRoom6Photo.NoPaper", "1");
				m_game.m_inventory.AddItem("DreamRoom6Paper", loading: false);
				m_game.m_inventory.ChangeExamineItem("DreamRoom6Paper");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}
