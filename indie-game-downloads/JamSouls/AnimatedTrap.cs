using Microsoft.Xna.Framework;

namespace JamSouls;

public class AnimatedTrap : ScenaricEntitie
{
	public AnimatedSprite m_TrapAnim;

	private GameState m_state;

	public TriggerTrap m_Trigger;

	private Vector2 m_location;

	public AnimatedTrap(GameState state, AnimatedSprite TrapAnim, int x, int y, string name)
	{
		m_TrapAnim = TrapAnim;
		m_TrapAnim.m_TotalLoop = 1;
		m_TrapAnim.m_bInfiniteLoop = false;
		m_TrapAnim.SetPosition(new Vector2(x, y));
		m_state = state;
		Name = name;
		TypeId = SCENARIC.TYPE_ANIM;
		InitEntity();
		m_bVisible = true;
		m_location = new Vector2(x, y);
	}

	public void SetTrigger(TriggerTrap ttrap)
	{
		m_Trigger = ttrap;
	}

	public override void Update(GameTime gameTime)
	{
		if (m_Trigger != null && m_Trigger.m_Triggered > 0f)
		{
			for (int i = 0; i < m_state.m_Players.Count; i++)
			{
				Player player = m_state.m_Players[i];
				Vector2 position = player.GetPosition();
				if (player.m_Tag != 1 && !player.m_bSpecialEnable && player.m_bIsOnGround && position.Y > m_location.Y && position.X > m_location.X && position.X < m_location.X + (float)m_TrapAnim.GetFrameWidth())
				{
					player.m_Tag = 1;
					if (GameContext.GameMode == GAME_MODE.DEATHMATCH)
					{
						player.DecreaseScore(1);
					}
				}
				m_TrapAnim.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
			}
		}
		else
		{
			m_TrapAnim.Reset();
		}
	}

	public override void SetPosition(Vector2 pos)
	{
		m_TrapAnim.SetPosition(pos);
	}

	public override Vector2 GetPosition()
	{
		return m_TrapAnim.m_FixedPos;
	}

	public override void Draw()
	{
		if (m_bVisible)
		{
			m_TrapAnim.DrawFixed(m_SpriteEffect, Color.White, m_zOrder);
		}
	}
}
