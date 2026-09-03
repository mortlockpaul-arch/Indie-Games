using System;
using System.Collections.Generic;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SGSCore;

namespace TheMare1;

public class Credits : Core.Credits
{
	private bool m_game_finished;

	private List<string> m_text1 = new List<string>();

	private List<string> m_text2 = new List<string>();

	public Credits(Core.Game game, SGSContentLoader CL)
		: base(game, CL)
	{
		m_text1.Add("Decay: The Mare - Episode 1 by");
		m_text2.Add("Shining Gate Software");
		m_text1.Add("Music, Graphics and Design by");
		m_text2.Add("Johannes Rae");
		m_text1.Add("Produced and Developed by");
		m_text2.Add("Fredrik Westlund");
		m_text1.Add("Voice Acting by");
		m_text2.Add("Charlie Sly");
		m_text1.Add("Additional QA by");
		m_text2.Add("Charlie Sly and Robert Ottone");
		m_text1.Add("Special Thanks to");
		m_text2.Add("");
		m_text1.Add("Melinda Sjoberg");
		m_text2.Add("");
		m_text1.Add("Anna-Maria Taawo Westlund");
		m_text2.Add("");
		m_text1.Add("Peter Sjoberg");
		m_text2.Add("");
		m_text1.Add("Morris Sjoberg");
		m_text2.Add("");
		m_text1.Add("Danilo Almqvist");
		m_text2.Add("");
		m_text1.Add("Andreas Karlsson");
		m_text2.Add("");
		m_last_index = m_text1.Count - 1;
	}

	public override void Reset()
	{
		base.Reset();
		TheMare1 theMare = (TheMare1)m_game;
		if (theMare != null && theMare.m_exit_game)
		{
			m_game_finished = true;
			if (m_game.m_game_data != null)
			{
				m_game.m_game_data.SetState("Music", "3");
			}
			m_game.PlayMusic(3);
		}
	}

	protected override void onClose()
	{
		if (m_game_finished)
		{
			m_game_finished = false;
			if (m_game.m_game_data != null)
			{
				m_game.m_game_data.SetState("Music", "2");
			}
			m_game.PlayMusic(2);
			m_game.FadeInMusic();
		}
		base.onClose();
	}

	public override void Draw(SpriteBatch SB)
	{
		try
		{
			if (SB != null)
			{
				Vector2 zero = Vector2.Zero;
				Vector2 zero2 = Vector2.Zero;
				string text = "";
				string text2 = "";
				text = m_text1[m_text_index];
				text2 = m_text2[m_text_index];
				SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				if (text2 != "")
				{
					zero = m_font.MeasureString(text);
					SB.DrawString(position: new Vector2(((float)Core.Game.VIEW_RECT.Width - zero.X) / 2f, (float)(Core.Game.VIEW_RECT.Height / 2) - zero.Y), spriteFont: m_font, text: text, color: Color.White * m_text_alpha);
					zero = m_font.MeasureString(text2);
					SB.DrawString(position: new Vector2(((float)Core.Game.VIEW_RECT.Width - zero.X) / 2f, Core.Game.VIEW_RECT.Height / 2), spriteFont: m_font, text: text2, color: Color.White * m_text_alpha);
				}
				else
				{
					zero = m_font.MeasureString(text);
					SB.DrawString(position: new Vector2(((float)Core.Game.VIEW_RECT.Width - zero.X) / 2f, ((float)Core.Game.VIEW_RECT.Height - zero.Y) / 2f), spriteFont: m_font, text: text, color: Color.White * m_text_alpha);
				}
				SB.End();
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}
