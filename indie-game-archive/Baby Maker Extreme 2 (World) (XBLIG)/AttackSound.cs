using Microsoft.Xna.Framework.Audio;

public class AttackSound
{
	private const float volumeMod = 0.5f;

	private SoundEffect m_effect;

	private float m_volume;

	private float m_pitch;

	private int m_timer;

	public int Timer
	{
		get
		{
			return m_timer;
		}
		set
		{
			m_timer = value;
		}
	}

	public AttackSound(SoundEffect sound, float volume, float pitch, int time)
	{
		m_effect = sound;
		m_volume = volume;
		m_pitch = pitch;
		m_timer = time;
	}

	public void Play()
	{
		m_effect.Play(m_volume * 0.5f, m_pitch, 0f);
	}
}
