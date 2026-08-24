using Microsoft.Xna.Framework.Audio;

namespace Game.Audio;

public class AudioEventCue
{
	public AudioManager manager;

	public string name;

	public bool active;

	public Cue cue;

	public AudioEventCue(AudioManager oManager, string xName)
	{
		manager = oManager;
		name = xName;
		cue = manager.soundBank.GetCue(name);
		active = false;
	}

	public virtual void Play()
	{
		if (cue.IsPaused)
		{
			cue.Resume();
		}
		else if (cue.IsPlaying)
		{
			cue.Stop(AudioStopOptions.Immediate);
			cue = manager.soundBank.GetCue(name);
			cue.Play();
		}
		else
		{
			cue = manager.soundBank.GetCue(name);
			cue.Play();
		}
	}

	public virtual void Dispose()
	{
		cue.Dispose();
		cue = null;
		manager = null;
	}
}
