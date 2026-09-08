using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace StarbeamDefenderGame;

public class AudioManager
{
	private SoundEffect explosionsound;

	private SoundEffect missilelaunch;

	private SoundEffect enemyblaster;

	private SoundEffect playerblaster;

	private SoundEffect shieldhit;

	private SoundEffectInstance[] sounds;

	private SoundEffect gamemusic;

	private SoundEffectInstance gamemusicinstance;

	private string musicfile;

	public void Initialise(ContentManager content)
	{
		sounds = new SoundEffectInstance[10];
		explosionsound = content.Load<SoundEffect>("SoundEffects/ExplosionSmall");
		missilelaunch = content.Load<SoundEffect>("SoundEffects/Missile");
		enemyblaster = content.Load<SoundEffect>("SoundEffects/EnemyBlaster");
		playerblaster = content.Load<SoundEffect>("SoundEffects/PlayerBlaster");
		shieldhit = content.Load<SoundEffect>("SoundEffects/Shield");
		for (int i = 0; i < 10; i++)
		{
			sounds[i] = explosionsound.CreateInstance();
		}
	}

	public void PlayMusic(string file, ContentManager content)
	{
		if (file == string.Empty)
		{
			gamemusicinstance.Stop();
		}
		else if (!(musicfile == file) || gamemusicinstance.State != SoundState.Playing)
		{
			if (gamemusic != null)
			{
				gamemusicinstance.Dispose();
				gamemusic.Dispose();
			}
			gamemusic = content.Load<SoundEffect>(file);
			musicfile = file;
			gamemusicinstance = gamemusic.CreateInstance();
			gamemusicinstance.IsLooped = true;
			gamemusicinstance.Play();
		}
	}

	private int GetNextFreeSoundIndex()
	{
		for (int i = 0; i < 10; i++)
		{
			if (sounds[i].State == SoundState.Stopped)
			{
				sounds[i].Dispose();
				return i;
			}
		}
		return -1;
	}

	public void PlayExplosionSound()
	{
		if (MediaPlayer.GameHasControl)
		{
			int nextFreeSoundIndex = GetNextFreeSoundIndex();
			if (nextFreeSoundIndex != -1)
			{
				sounds[nextFreeSoundIndex] = explosionsound.CreateInstance();
				sounds[nextFreeSoundIndex].IsLooped = false;
				sounds[nextFreeSoundIndex].Play();
			}
		}
	}

	public void PlayShieldHitSound()
	{
		if (MediaPlayer.GameHasControl)
		{
			int nextFreeSoundIndex = GetNextFreeSoundIndex();
			if (nextFreeSoundIndex != -1)
			{
				sounds[nextFreeSoundIndex] = shieldhit.CreateInstance();
				sounds[nextFreeSoundIndex].IsLooped = false;
				sounds[nextFreeSoundIndex].Play();
			}
		}
	}

	public void PlayMissileFireSound()
	{
		if (MediaPlayer.GameHasControl)
		{
			int nextFreeSoundIndex = GetNextFreeSoundIndex();
			if (nextFreeSoundIndex != -1)
			{
				sounds[nextFreeSoundIndex] = missilelaunch.CreateInstance();
				sounds[nextFreeSoundIndex].IsLooped = false;
				sounds[nextFreeSoundIndex].Play();
			}
		}
	}

	public void PlayEnemyBlasterSound()
	{
		if (MediaPlayer.GameHasControl)
		{
			int nextFreeSoundIndex = GetNextFreeSoundIndex();
			if (nextFreeSoundIndex != -1)
			{
				sounds[nextFreeSoundIndex] = enemyblaster.CreateInstance();
				sounds[nextFreeSoundIndex].IsLooped = false;
				sounds[nextFreeSoundIndex].Play();
			}
		}
	}

	public void PlayPlayerBlasterSound()
	{
		if (MediaPlayer.GameHasControl)
		{
			int nextFreeSoundIndex = GetNextFreeSoundIndex();
			if (nextFreeSoundIndex != -1)
			{
				sounds[nextFreeSoundIndex] = playerblaster.CreateInstance();
				sounds[nextFreeSoundIndex].IsLooped = false;
				sounds[nextFreeSoundIndex].Play();
			}
		}
	}
}
