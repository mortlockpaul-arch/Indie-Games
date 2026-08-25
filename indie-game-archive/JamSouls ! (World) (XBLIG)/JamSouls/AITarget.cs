using Microsoft.Xna.Framework;

namespace JamSouls;

public class AITarget
{
	public object TargetObject;

	public AITarget(object target)
	{
		TargetObject = target;
	}

	public Vector2 GetPosition()
	{
		if ((object)TargetObject.GetType().BaseType == typeof(Player))
		{
			Player player = (Player)TargetObject;
			return player.GetPosition();
		}
		if ((object)TargetObject.GetType().BaseType == typeof(PowerUp))
		{
			PowerUp powerUp = (PowerUp)TargetObject;
			return powerUp.m_MiddlePosition;
		}
		if ((object)TargetObject.GetType().BaseType == typeof(ScenaricEntitie))
		{
			ScenaricEntitie scenaricEntitie = (ScenaricEntitie)TargetObject;
			return scenaricEntitie.GetPosition();
		}
		if ((object)TargetObject.GetType() == typeof(Soul))
		{
			Soul soul = (Soul)TargetObject;
			return soul.GetPosition();
		}
		return Vector2.Zero;
	}
}
