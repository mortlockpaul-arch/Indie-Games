using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

public static class SoundManager
{
	private static Dictionary<string, SoundEffect> pages;

	private static ContentManager content;

	private static List<AttackSound> m_SoundQueue;

	private static List<AttackSound> soundRemoveList;

	public static void Initialize(ContentManager c)
	{
		pages = new Dictionary<string, SoundEffect>(1000);
		content = c;
		m_SoundQueue = new List<AttackSound>(200);
		soundRemoveList = new List<AttackSound>(200);
	}

	public static SoundEffect GetSoundEffect(string filename)
	{
		if (pages.ContainsKey(filename))
		{
			return pages[filename];
		}
		pages[filename] = content.Load<SoundEffect>(filename);
		return pages[filename];
	}

	public static void AddSoundToPlay(SoundEffect sound, float volume, float pitch, int timeOffset)
	{
		AttackSound attackSound = new AttackSound(sound, volume, pitch, timeOffset);
		if (attackSound.Timer > 0)
		{
			m_SoundQueue.Add(attackSound);
		}
		else
		{
			attackSound.Play();
		}
	}

	public static void Update(TimeTracker gameTime)
	{
		soundRemoveList.Clear();
		foreach (AttackSound item in m_SoundQueue)
		{
			item.Timer -= gameTime.ElapsedMilli;
			if (item.Timer <= 0)
			{
				item.Play();
				soundRemoveList.Add(item);
			}
		}
		foreach (AttackSound soundRemove in soundRemoveList)
		{
			m_SoundQueue.Remove(soundRemove);
		}
	}
}
