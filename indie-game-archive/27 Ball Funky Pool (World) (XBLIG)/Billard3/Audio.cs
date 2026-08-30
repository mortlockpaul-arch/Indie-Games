using Maximinus;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Billard3;

public class Audio
{
	public enum SFXID
	{
		BallBall,
		BallCue,
		BallTrou,
		BallBande,
		Menu,
		COUNT
	}

	private const float songVolMax = 0.5f;

	private static SoundEffect[] SFX = new SoundEffect[5];

	private static SoundEffectInstance song;

	public static bool SongStatus = !BillardGame.DebugRecord && !BillardGame.DisableMusic;

	public static void LoadContent(ContentManager Content)
	{
		SFX[0] = Content.Load<SoundEffect>("sound/ballball");
		SFX[1] = Content.Load<SoundEffect>("sound/ballcue");
		SFX[2] = Content.Load<SoundEffect>("sound/balltrou");
		SFX[3] = Content.Load<SoundEffect>("sound/ballbande");
		SFX[4] = Content.Load<SoundEffect>("sound/menu");
		song = Content.Load<SoundEffect>("sound/song").CreateInstance();
		song.IsLooped = true;
		song.Volume = 0f;
		song.Play();
	}

	public static void Update()
	{
		if (SongStatus)
		{
			song.Volume = Utils.incrementRatio(song.Volume / 0.5f, 30) * 0.5f;
		}
		else
		{
			song.Volume = Utils.decrementRatio(song.Volume / 0.5f, 30) * 0.5f;
		}
	}

	public static void PlaySFX(SFXID id)
	{
		PlaySFX(id, 1f);
	}

	public static void PlaySFX(SFXID id, float volume)
	{
		SoundEffect soundEffect = SFX[(int)id];
		soundEffect.Play(volume, 0f, 0f);
	}
}
