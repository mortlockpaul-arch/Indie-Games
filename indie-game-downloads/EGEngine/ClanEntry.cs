using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

namespace EGEngine;

public class ClanEntry
{
	public string gamerTag;

	public NetworkGamer gamer;

	public bool[] render;

	public Vector2[] screenPos;

	public ClanEntry()
	{
		gamerTag = "";
		render = new bool[2];
		screenPos = new Vector2[2];
	}

	public ClanEntry(NetworkGamer e)
	{
		gamerTag = "";
		render = new bool[2];
		screenPos = new Vector2[2];
		gamer = e;
		gamerTag = e.Gamertag;
		render[0] = false;
		render[1] = false;
		ref Vector2 reference = ref screenPos[0];
		reference = new Vector2(40000f, 40000f);
		ref Vector2 reference2 = ref screenPos[1];
		reference2 = new Vector2(40000f, 40000f);
	}
}
