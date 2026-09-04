using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceBlast;

internal class MessageWindow : DrawableGameComponent
{
	private struct Message
	{
		public string msg;

		public double expires;
	}

	private const double constMessageLife = 8.0;

	private int m_XEdge;

	private int m_YBase;

	private List<Message> m_MessageList = new List<Message>();

	private ContentManager m_Content;

	private SpriteBatch m_SpriteBatch;

	private SpriteFont m_Font;

	private Game m_Game;

	public MessageWindow(Game game)
		: base(game)
	{
		m_Game = game;
	}

	protected override void LoadContent()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		m_Content = MainGame.ContentMan;
		m_SpriteBatch = new SpriteBatch(m_Game.GraphicsDevice);
		m_Font = m_Content.Load<SpriteFont>("Fonts/MessageWindowFont");
		if (MainGame.Is1080HD)
		{
			m_XEdge = 150;
			m_YBase = 850;
		}
		else
		{
			m_XEdge = 100;
			m_YBase = 500;
		}
		((DrawableGameComponent)this).LoadContent();
	}

	public override void Update(GameTime gameTime)
	{
		double rawTime = TimeManager.RawTime;
		while (m_MessageList.Count > 0 && m_MessageList[0].expires < rawTime)
		{
			m_MessageList.RemoveAt(0);
		}
		((GameComponent)this).Update(gameTime);
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		m_SpriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
		float num = m_YBase;
		foreach (Message message in m_MessageList)
		{
			m_SpriteBatch.DrawString(m_Font, message.msg, new Vector2((float)m_XEdge, num), Color.Yellow);
			num += 20f;
		}
		m_SpriteBatch.End();
		((DrawableGameComponent)this).Draw(gameTime);
	}

	public void AddMessage(string msg)
	{
		Message item = new Message
		{
			msg = msg,
			expires = TimeManager.RawTime + 8.0
		};
		if (m_MessageList.Count > 0)
		{
			item.expires = Math.Max(item.expires, m_MessageList[m_MessageList.Count - 1].expires + 2.0);
		}
		m_MessageList.Add(item);
	}
}
