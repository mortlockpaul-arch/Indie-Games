using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Game.Audio;

public class MusicTrack
{
	public MusicManager manager;

	public string title;

	public AudioEventCue[] tracks;

	public MusicTrack(MusicManager oManager, string xTitle, AudioEventCue[] aTracks)
	{
		manager = oManager;
		title = xTitle;
		tracks = aTracks;
	}

	public void Update(GameTime oGameTime)
	{
	}

	public void Dispose()
	{
	}

	public void Stop()
	{
		for (uint num = 0u; num < tracks.Length; num++)
		{
			tracks[num].cue.Stop(AudioStopOptions.Immediate);
		}
	}

	public void Play()
	{
		for (uint num = 0u; num < tracks.Length; num++)
		{
			tracks[num].Play();
		}
	}

	public void Pause()
	{
		for (uint num = 0u; num < tracks.Length; num++)
		{
			tracks[num].cue.Pause();
		}
	}

	public void Resume()
	{
		for (uint num = 0u; num < tracks.Length; num++)
		{
			tracks[num].cue.Resume();
		}
	}
}
