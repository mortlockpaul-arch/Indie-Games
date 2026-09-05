using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Renderer;

namespace PlayObjects;

public static class SoundEffects
{
	private static List<SoundEffect> m_cheers;

	private static List<SoundEffect> m_riffs;

	private static List<SoundEffect> m_exclaims;

	private static int waitCounter;

	public static void Initialize()
	{
		m_cheers = new List<SoundEffect>();
		m_cheers.Add(SoundManager.GetSoundEffect("sounds/freesound/cheer/18666__Halleck__curtain_call_16_1"));
		m_cheers.Add(SoundManager.GetSoundEffect("sounds/freesound/cheer/18666__Halleck__curtain_call_16_2"));
		m_cheers.Add(SoundManager.GetSoundEffect("sounds/freesound/cheer/18666__Halleck__curtain_call_16_3"));
		m_cheers.Add(SoundManager.GetSoundEffect("sounds/freesound/cheer/18666__Halleck__curtain_call_16_4"));
		m_cheers.Add(SoundManager.GetSoundEffect("sounds/freesound/cheer/18666__Halleck__curtain_call_16_5"));
		m_exclaims = new List<SoundEffect>();
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/extreme1"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/extreme2"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/extreme3"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/radical1"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/rockout1"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/yeah1"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/yeah2"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/rockroll1"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/bodacious1"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/sick1"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/sick2"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/radical2"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/radical3"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/rockout2"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/rockout3"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/extreme4"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/gnarly1"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/gnarly2"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/gnarly3"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/rockroll2"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/bodacious2"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/bodacious3"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/leet"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/leet2"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/uberleet1"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/pwned1"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/awesome1"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/awesome2"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/thebomb1"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/thebomb2"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/thebomb3"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/coolbeans1"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/copasetic1"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/fresh1"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/funkyfresh1"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/funkalicious1"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/fresh2"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/tothemax1"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/totheextreme1"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/tubular1"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/tubular2"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/wicked1"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/wicked2"));
		m_exclaims.Add(SoundManager.GetSoundEffect("sounds/exclaims/awesome3"));
		m_riffs = new List<SoundEffect>();
		m_riffs.Add(SoundManager.GetSoundEffect("sounds/freesound/slides/11767__SpeedY"));
		m_riffs.Add(SoundManager.GetSoundEffect("sounds/freesound/slides/11770__SpeedY"));
		m_riffs.Add(SoundManager.GetSoundEffect("sounds/freesound/slides/11780__SpeedY"));
		m_riffs.Add(SoundManager.GetSoundEffect("sounds/freesound/slides/11782__SpeedY"));
		m_riffs.Add(SoundManager.GetSoundEffect("sounds/freesound/slides/11790__SpeedY"));
		m_riffs.Add(SoundManager.GetSoundEffect("sounds/freesound/slides/11806__SpeedY"));
		m_riffs.Add(SoundManager.GetSoundEffect("sounds/freesound/slides/11923__SpeedY"));
		m_riffs.Add(SoundManager.GetSoundEffect("sounds/freesound/slides/11925__SpeedY"));
		waitCounter = (int)SceneRenderer.GetRand(3f, 8f);
	}

	public static void PlayCollisionSound()
	{
		SoundManager.AddSoundToPlay(m_riffs[(int)SceneRenderer.GetRand(0f, m_riffs.Count)], 0.7f, 0f, 0);
		waitCounter--;
		if (waitCounter <= 0)
		{
			SoundManager.AddSoundToPlay(m_cheers[(int)SceneRenderer.GetRand(0f, m_cheers.Count)], 1f, 0f, 0);
			if (SceneRenderer.GetRand(0f, 1f) < 0.2f)
			{
				SoundManager.AddSoundToPlay(m_exclaims[(int)SceneRenderer.GetRand(0f, m_exclaims.Count)], 0.7f, 0f, 200);
			}
			waitCounter = (int)SceneRenderer.GetRand(3f, 8f);
		}
	}
}
