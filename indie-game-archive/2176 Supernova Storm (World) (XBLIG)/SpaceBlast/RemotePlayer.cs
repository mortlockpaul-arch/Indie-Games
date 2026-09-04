using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace SpaceBlast;

internal class RemotePlayer : Player
{
	public RemotePlayer(byte playerid, Vector3 pos, Gamer gamer, ShipColor colour, ETeam team)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector(playerid, pos, gamer, colour, team);
	}

	public override void Terminate()
	{
	}

	public override void Update()
	{
		TheShip.UpdateRemoteShip();
		base.Update();
	}

	public override void Die(Player killedBy)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (MainGame.Instance.IsWithinAudibleRange(TheShip.Position))
		{
			MainGame.AudioMan.Play(Sound.Explosion, TheShip.Position);
		}
		base.Die(killedBy);
	}
}
