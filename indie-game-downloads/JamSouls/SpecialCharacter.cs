using Microsoft.Xna.Framework;

namespace JamSouls;

public abstract class SpecialCharacter
{
	public Player m_Player;

	public float m_SpecialTime;

	public abstract void InitSpecial();

	public abstract void Update(GameTime gameTime);

	public virtual void Draw()
	{
	}

	public virtual void StopSpecial()
	{
		m_Player.m_bSpecialEnable = false;
		int num = (int)(m_Player.m_CurrentAnim - 6);
		if (num >= 0 && num < 14)
		{
			m_Player.SetAnimation((Player.AnimStates)num);
		}
		else
		{
			m_Player.SetAnimation(Player.AnimStates.STAND);
		}
		m_Player.m_GameStateInstance.m_bAllowSoulSpawn = true;
		Vector2 impulse = new Vector2(0f, -100f);
		m_Player.GetBody().ApplyLinearImpulse(ref impulse);
	}
}
