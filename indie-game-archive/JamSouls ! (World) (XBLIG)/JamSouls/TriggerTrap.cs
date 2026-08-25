using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JamSouls;

public class TriggerTrap : ScenaricEntitie
{
	private const float MIN_TRIGGER_TIME = 250f;

	public Texture2D m_UpTexture;

	public Texture2D m_DnTexture;

	private Vector2 Location;

	public float m_Triggered;

	private GameState m_state;

	private Rectangle sourceRectangle;

	public Rectangle m_TriggerRect = default(Rectangle);

	public TriggerTrap(GameState state, Texture2D Sprite1, Texture2D Sprite2, int x, int y, string name)
	{
		m_UpTexture = Sprite1;
		m_DnTexture = Sprite2;
		Location.X = x;
		Location.Y = y;
		m_state = state;
		Name = name;
		TypeId = SCENARIC.TYPE_LAYER;
		sourceRectangle = new Rectangle(0, 0, m_UpTexture.Width, m_UpTexture.Height);
		InitEntity();
		m_bVisible = true;
	}

	public override void Update(GameTime gameTime)
	{
		for (int i = 0; i < m_state.m_Players.Count; i++)
		{
			Vector2 bottomPosition = m_state.m_Players[i].GetBottomPosition();
			if (m_state.m_Players[i].m_bIsOnGround && m_TriggerRect.Contains((int)bottomPosition.X, (int)bottomPosition.Y))
			{
				m_Triggered = 250f;
			}
		}
		if (m_Triggered > 0f)
		{
			m_Triggered -= gameTime.ElapsedGameTime.Milliseconds;
		}
	}

	public override void SetPosition(Vector2 pos)
	{
		Location = pos;
	}

	public override Vector2 GetPosition()
	{
		return Location;
	}

	public override void Draw()
	{
		if (m_bVisible)
		{
			if (m_Triggered > 0f)
			{
				m_state.ScreenManager.SpriteBatch.Draw(m_DnTexture, Location, sourceRectangle, Color.White, 0f, Vector2.Zero, 1f, m_SpriteEffect, m_zOrder);
			}
			else
			{
				m_state.ScreenManager.SpriteBatch.Draw(m_UpTexture, Location, sourceRectangle, Color.White, 0f, Vector2.Zero, 1f, m_SpriteEffect, m_zOrder);
			}
		}
	}
}
