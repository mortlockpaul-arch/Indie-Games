using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace SpaceBlast;

internal class ReplayPlayer : Player
{
	public ReplayPlayer(byte playerid, Vector3 pos, Gamer gamer, ShipColor colour, ETeam team)
		: base(playerid, pos, gamer, colour, team)
	{
	}

	public override void Terminate()
	{
	}
}
