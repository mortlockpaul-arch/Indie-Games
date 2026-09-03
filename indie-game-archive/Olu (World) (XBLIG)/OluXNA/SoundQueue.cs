using System.Collections;
using Microsoft.Xna.Framework.Audio;

namespace OluXNA;

internal class SoundQueue
{
	public ArrayList cues;

	protected int tic;

	private SoundBank sB;

	private static AudioEngine engine;

	private static WaveBank wavebank;

	public SoundQueue()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		base._002Ector();
		engine = new AudioEngine("Content\\Sounds\\Sounds.xgs");
		wavebank = new WaveBank(engine, "Content\\Sounds\\Wave Bank.xwb");
		sB = new SoundBank(engine, "Content\\Sounds\\Sound Bank.xsb");
		tic = 0;
		cues = new ArrayList();
	}

	public bool CueExists(string _cueName, Beats _beatPlay)
	{
		bool result = false;
		SoundPart other = new SoundPart(_cueName, _beatPlay);
		foreach (SoundPart cue in cues)
		{
			if (cue.isEqual(other))
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public bool CueExists(SoundPart _cue)
	{
		bool result = false;
		foreach (SoundPart cue in cues)
		{
			if (cue.isEqual(_cue))
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public void AddCue(string _cueName, Beats _beatPlay)
	{
		if (!CueExists(_cueName, _beatPlay))
		{
			cues.Add(new SoundPart(_cueName, _beatPlay));
		}
	}

	public void AddCue(SoundPart _cue)
	{
		if (!CueExists(_cue))
		{
			cues.Add(_cue);
		}
	}

	public void increment()
	{
		tic++;
		tic %= BaseGame.Get().maxBeat;
		for (int i = 0; i < cues.Count; i++)
		{
			SoundPart soundPart = (SoundPart)cues[i];
			if (tic % (int)soundPart.beatPlay == 0)
			{
				sB.PlayCue(soundPart.cueName);
				cues.Remove(soundPart);
				i--;
			}
		}
		engine.Update();
	}
}
