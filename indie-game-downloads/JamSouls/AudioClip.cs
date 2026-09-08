using Microsoft.Xna.Framework.Audio;

namespace JamSouls;

public class AudioClip
{
	private string name = "";

	private Cue m_cue;

	public AudioClip(string sound)
	{
		name = sound;
	}

	public void Play()
	{
		m_cue = AudioManager.m_SoundBank.GetCue(name);
		m_cue.Play();
	}

	public void Stop()
	{
		if (m_cue != null)
		{
			m_cue.Stop(AudioStopOptions.Immediate);
		}
	}

	public bool IsPlaying()
	{
		if (m_cue != null)
		{
			return m_cue.IsPlaying;
		}
		return false;
	}
}
