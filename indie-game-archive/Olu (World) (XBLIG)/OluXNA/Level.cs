using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class Level
{
	public List<Zone> zones;

	public int activeZone;

	public int maxBeats;

	public int fogStart;

	public int fogEnd;

	public float tempo;

	public Vector3 baseColor;

	public Vector3 flashColor;

	public Vector3 effectColor;

	public string playerModelPath;

	public Zone ActiveZone => zones[activeZone];

	public Level()
	{
		zones = new List<Zone>();
		activeZone = 0;
		maxBeats = 16;
	}

	public void Update(GameTime gametime)
	{
		zones[activeZone].Update(gametime);
	}

	public void Draw(GameTime gametime)
	{
		zones[activeZone].Draw(gametime);
	}

	public void AddZone(int id)
	{
		zones.Add(new Zone(id));
	}

	public void LoadGraphics()
	{
		BaseGame.Get().maxBeat = maxBeats;
		BaseGame.BEAT = 60f / tempo / 4f;
		foreach (Zone zone in zones)
		{
			zone.LoadGraphics();
		}
		BaseGame.FOG_START = fogStart;
		BaseGame.FOG_END = fogEnd;
		BaseGame.Get().fogEffect.Parameters["xFogStart"].SetValue((float)BaseGame.FOG_START);
		BaseGame.Get().fogEffect.Parameters["xFogEnd"].SetValue((float)BaseGame.FOG_END);
	}

	public void LoadZone(int id)
	{
		LoadZone(id, clear: true);
	}

	public void LoadZone(int id, bool clear)
	{
		if (clear)
		{
			zones[activeZone].Finish();
		}
		activeZone = id;
		zones[activeZone].Start();
		for (int num = BaseGame.Get().enems.Count - 1; num >= 0; num--)
		{
			BaseGame.Get().enems[num].leave();
		}
		BaseGame.Get().actualEnem = 0;
	}
}
