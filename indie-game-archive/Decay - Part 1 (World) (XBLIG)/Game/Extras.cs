using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SGSCore;

namespace Game;

public class Extras
{
	private Texture2D m_LS;

	private Texture2D m_b_button;

	private SpriteFont m_font;

	private SpriteFont m_font2;

	private List<Texture2D> m_images;

	private int m_current_image;

	private Game m_game;

	public Extras(Game game, SGSContentLoader CL)
	{
		m_game = game;
		m_LS = CL.LoadTexture("HUD/LS");
		m_b_button = CL.LoadTexture("HUD/b_button");
		m_font = CL.LoadFont("Fonts/SpriteFont2");
		m_font2 = CL.LoadFont("Fonts/SpriteFont1");
		m_images = new List<Texture2D>();
		m_images.Add(CL.LoadTexture("Extras/extras_fallen"));
		m_images.Add(CL.LoadTexture("Extras/extras_wb"));
		m_images.Add(CL.LoadTexture("Extras/extras_tegel"));
		m_images.Add(CL.LoadTexture("Extras/extras_girl"));
		m_images.Add(CL.LoadTexture("Extras/extras_korri"));
		m_images.Add(CL.LoadTexture("Extras/extras_owhite"));
		m_images.Add(CL.LoadTexture("Extras/extras_tv"));
	}

	public virtual void Clear()
	{
		m_game = null;
		m_LS = null;
		m_b_button = null;
		m_font = null;
		m_font2 = null;
		for (int i = 0; i < m_images.Count; i++)
		{
			m_images[i] = null;
		}
		m_images.Clear();
		m_images = null;
	}

	public virtual void Reset()
	{
		m_current_image = 0;
	}

	public virtual void Update(TimeSpan elapsed)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Invalid comparison between Unknown and I4
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Invalid comparison between Unknown and I4
		KeyboardState state = Keyboard.GetState();
		GamePadState state2 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadDPad dPad = ((GamePadState)(ref state2)).DPad;
		if ((int)((GamePadDPad)(ref dPad)).Left != 1)
		{
			GamePadState state3 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadThumbSticks thumbSticks = ((GamePadState)(ref state3)).ThumbSticks;
			if (!(((GamePadThumbSticks)(ref thumbSticks)).Left.X < -0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)37))
			{
				m_game.m_left_pressed = false;
				goto IL_00a6;
			}
		}
		if (!m_game.m_left_pressed)
		{
			m_game.m_left_pressed = true;
			m_current_image--;
			if (m_current_image < 0)
			{
				m_current_image = m_images.Count - 1;
			}
		}
		goto IL_00a6;
		IL_0147:
		GamePadState state4 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadButtons buttons = ((GamePadState)(ref state4)).Buttons;
		if ((int)((GamePadButtons)(ref buttons)).B == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)66))
		{
			if (!m_game.m_b_pressed)
			{
				m_game.m_b_pressed = true;
				m_game.onExtrasClosed();
			}
		}
		else
		{
			m_game.m_b_pressed = false;
		}
		return;
		IL_00a6:
		GamePadState state5 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadDPad dPad2 = ((GamePadState)(ref state5)).DPad;
		if ((int)((GamePadDPad)(ref dPad2)).Right != 1)
		{
			GamePadState state6 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref state6)).ThumbSticks;
			if (!(((GamePadThumbSticks)(ref thumbSticks2)).Left.X > 0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)39))
			{
				m_game.m_right_pressed = false;
				goto IL_0147;
			}
		}
		if (!m_game.m_right_pressed)
		{
			m_game.m_right_pressed = true;
			m_current_image++;
			if (m_current_image >= m_images.Count)
			{
				m_current_image = 0;
			}
		}
		goto IL_0147;
	}

	public virtual void Draw(SpriteBatch SB)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		if (SB != null)
		{
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_images[m_current_image], new Vector2((float)((Game.VIEW_RECT.Width - m_images[m_current_image].Width) / 2), (float)((Game.VIEW_RECT.Height - m_images[m_current_image].Height) / 2 - 60)), Color.White);
			SB.End();
			Vector2 zero = Vector2.Zero;
			Vector2 zero2 = Vector2.Zero;
			string text = "";
			switch (m_current_image)
			{
			case 0:
				text = "At first, the name of this project was Fallen Angel.\n\rLater on we decided to change it to Decay.";
				break;
			case 1:
				text = "A snapshot of the whiteboard, after an early brainstorm meeting.";
				break;
			case 2:
				text = "This building can be seen from our office. In the game\n\rit can be seen on the frame that has a message from Emily.";
				break;
			case 3:
				text = "Concept art for the \"woman's painting\".";
				break;
			case 4:
				text = "Concept art for the hallway.";
				break;
			case 5:
				text = "Concept art for Mr. O. White.";
				break;
			case 6:
				text = "Concept art for the TV.";
				break;
			}
			zero2 = m_font.MeasureString(text);
			zero.X = ((float)Game.VIEW_RECT.Width - zero2.X) / 2f;
			zero.Y = (float)(((Rectangle)(ref Game.TS_AREA)).Bottom - 40) - zero2.Y;
			SB.Begin();
			SB.DrawString(m_font, text, zero, Color.White);
			zero2 = m_font2.MeasureString("CHANGE IMAGE");
			zero = Vector2.Zero;
			zero.X = ((Rectangle)(ref Game.TS_AREA)).Left;
			zero.Y = ((Rectangle)(ref Game.TS_AREA)).Bottom - m_LS.Height + 7;
			SB.Draw(m_LS, zero, Color.White);
			zero.X += (float)(m_LS.Width + 10);
			zero.Y = (float)((Rectangle)(ref Game.TS_AREA)).Bottom - zero2.Y;
			SB.DrawString(m_font2, "CHANGE IMAGE", zero, Color.White);
			zero2 = m_font2.MeasureString("BACK");
			zero.X = (float)((Rectangle)(ref Game.TS_AREA)).Right - zero2.X;
			zero.Y = (float)((Rectangle)(ref Game.TS_AREA)).Bottom - zero2.Y;
			SB.DrawString(m_font2, "BACK", zero, Color.White);
			zero.X -= (float)(m_b_button.Width + 10);
			SB.Draw(m_b_button, zero, Color.White);
			SB.End();
		}
	}
}
