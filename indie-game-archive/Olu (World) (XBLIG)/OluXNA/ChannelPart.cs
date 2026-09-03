using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class ChannelPart
{
	public int startBeat;

	public int channelNum;

	public int maxMeasures;

	public int curMeasure;

	public int playMeasure;

	public float value;

	public bool done;

	public ChannelPart(Dictionary<string, string> attributes, XmlNode node)
	{
		startBeat = LevelLoader.GetIntFromAtt(attributes, "beat", 0);
		channelNum = LevelLoader.GetIntFromAtt(attributes, "channel", 0);
		playMeasure = LevelLoader.GetIntFromAtt(attributes, "playmeas", 0);
		maxMeasures = LevelLoader.GetIntFromAtt(attributes, "loopmeas", 0);
		value = LevelLoader.GetFloatFromAtt(attributes, "value", 1f);
		if (maxMeasures != 0)
		{
			curMeasure = -1;
		}
		done = false;
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
			BaseGame.Get().channels[channelNum] = value;
			if (maxMeasures < 0)
			{
				done = true;
			}
		}
	}
}
