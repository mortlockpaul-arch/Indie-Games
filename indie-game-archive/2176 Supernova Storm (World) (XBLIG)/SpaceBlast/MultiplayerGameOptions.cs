using System.Collections.Generic;
using Microsoft.Xna.Framework.GamerServices;

namespace SpaceBlast;

internal class MultiplayerGameOptions : GameOptions
{
	public List<Gamer> m_Gamers = new List<Gamer>();
}
