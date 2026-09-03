using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class MusicPart
{
	public int startBeat;

	public string cueName;

	public int maxMeasures;

	public int curMeasure;

	public int playMeasure;

	public bool done;

	public MusicPart(int _sB, string _cN)
	{
		startBeat = _sB;
		cueName = _cN;
		maxMeasures = (playMeasure = (curMeasure = 0));
	}

	public MusicPart(MusicPart other)
		: this(other.startBeat, other.cueName, other.playMeasure, other.maxMeasures)
	{
	}

	public MusicPart(int _sB, string _cN, int _pmeas, int _lmeas)
		: this(_sB, _cN)
	{
		playMeasure = _pmeas;
		maxMeasures = _lmeas;
		if (maxMeasures != 0)
		{
			curMeasure = -1;
		}
	}

	public MusicPart(Dictionary<string, string> attributes, XmlNode node)
		: this(int.Parse(attributes["beat"]), attributes["name"])
	{
		playMeasure = LevelLoader.GetIntFromAtt(attributes, "playmeas", 0);
		maxMeasures = LevelLoader.GetIntFromAtt(attributes, "loopmeas", 0);
		if (maxMeasures != 0)
		{
			curMeasure = -1;
		}
	}

	public void Update(GameTime gametime)
	{
		if (BaseGame.Get().OnExactBeat(0))
		{
			if (maxMeasures > 0)
			{
				curMeasure = (curMeasure + 1) % maxMeasures;
			}
			else if (maxMeasures < 0)
			{
				curMeasure++;
			}
		}
		if (curMeasure == playMeasure && BaseGame.Get().OnExactBeat(startBeat))
		{
			BaseGame.Get().BGPlayCue(cueName);
			if (maxMeasures < 0)
			{
				done = true;
			}
		}
	}
}
