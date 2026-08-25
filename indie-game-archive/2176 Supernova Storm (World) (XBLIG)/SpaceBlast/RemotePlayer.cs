using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace SpaceBlast;

internal class RemotePlayer : Player
{
	public RemotePlayer(byte playerid, Vector3 pos, Gamer gamer, ShipColor colour, ETeam team)
		: base(playerid, pos, gamer, colour, team)
	{
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
		if (MainGame.Instance.IsWithinAudibleRange(TheShip.Position))
		{
			MainGame.AudioMan.Play(Sound.Explosion, TheShip.Position);
		}
		base.Die(killedBy);
	}
}
