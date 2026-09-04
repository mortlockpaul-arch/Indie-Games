using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace SpaceBlast;

internal class ReplayPlayer : Player
{
	public ReplayPlayer(byte playerid, Vector3 pos, Gamer gamer, ShipColor colour, ETeam team)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector(playerid, pos, gamer, colour, team);
	}

	public override void Terminate()
	{
	}
}
