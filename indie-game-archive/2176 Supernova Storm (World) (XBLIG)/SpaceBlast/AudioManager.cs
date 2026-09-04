using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace SpaceBlast;

internal class AudioManager
{
	private const int constTotalInstances = 100;

	private SoundEffect[] m_Sounds = (SoundEffect[])(object)new SoundEffect[13];

	private SoundEffectInstance[] m_SoundInstances = (SoundEffectInstance[])(object)new SoundEffectInstance[100];

	private int m_InstanceCounter;

	public void LoadContent()
	{
		try
		{
			m_Sounds[0] = MainGame.ContentMan.Load<SoundEffect>("Audio/BensPowerup2");
			m_Sounds[1] = MainGame.ContentMan.Load<SoundEffect>("Audio/Click");
			m_Sounds[2] = MainGame.ContentMan.Load<SoundEffect>("Audio/Cloak");
			m_Sounds[3] = MainGame.ContentMan.Load<SoundEffect>("Audio/DeCloak");
			m_Sounds[4] = MainGame.ContentMan.Load<SoundEffect>("Audio/engine_2");
			m_Sounds[5] = MainGame.ContentMan.Load<SoundEffect>("Audio/explosion3");
			m_Sounds[6] = MainGame.ContentMan.Load<SoundEffect>("Audio/Laser1");
			m_Sounds[7] = MainGame.ContentMan.Load<SoundEffect>("Audio/Laser2");
			m_Sounds[8] = MainGame.ContentMan.Load<SoundEffect>("Audio/PowerUpAppear3");
			m_Sounds[9] = MainGame.ContentMan.Load<SoundEffect>("Audio/PU_MegaDamage");
			m_Sounds[10] = MainGame.ContentMan.Load<SoundEffect>("Audio/tx0_fire1");
			m_Sounds[11] = MainGame.ContentMan.Load<SoundEffect>("Audio/EMP");
			m_Sounds[12] = MainGame.ContentMan.Load<SoundEffect>("Audio/Starburst");
			for (int i = 0; i < 100; i++)
			{
				m_SoundInstances[i] = m_Sounds[0].Play(0f);
				m_SoundInstances[i].Stop();
			}
		}
		catch (Exception)
		{
		}
	}

	public SoundEffectInstance CreateEngineSound()
	{
		SoundEffectInstance val = m_Sounds[4].Play(1f, 0f, 0f, true);
		val.Pause();
		return val;
	}

	public void Play(Sound sound)
	{
		int num = m_InstanceCounter++ % 100;
		m_SoundInstances[num].Dispose();
		m_SoundInstances[num] = m_Sounds[(int)sound].Play();
	}

	public void Play(Sound sound, Vector3 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = position - MainGame.Instance.LeftPlayer.TheShip.Position;
		float num = MathHelper.Clamp(1f - ((Vector3)(ref val)).LengthSquared() / 7E+09f, 0f, 1f);
		float num2 = val.X / 20000f;
		int num3 = m_InstanceCounter++ % 100;
		m_SoundInstances[num3].Dispose();
		m_SoundInstances[num3] = m_Sounds[(int)sound].Play(num, 0f, num2, false);
		if (MainGame.Instance.RightPlayer != null)
		{
			val = position - MainGame.Instance.RightPlayer.TheShip.Position;
			num = MathHelper.Clamp(1f - ((Vector3)(ref val)).LengthSquared() / 7E+09f, 0f, 1f);
			num2 = val.X / 20000f;
			num3 = m_InstanceCounter++ % 100;
			m_SoundInstances[num3].Dispose();
			m_SoundInstances[num3] = m_Sounds[(int)sound].Play(num, 0f, num2, false);
		}
	}

	public void Play(Sound sound, float volume)
	{
		int num = m_InstanceCounter++ % 100;
		m_SoundInstances[num].Dispose();
		m_SoundInstances[num] = m_Sounds[(int)sound].Play(volume);
	}

	public void PlayEngines(Player player, float speed, float thrust, ref Vector3 position)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Vector3 val = position - MainGame.Instance.LeftPlayer.TheShip.Position;
			float num = MathHelper.Clamp(1f - ((Vector3)(ref val)).LengthSquared() / 7E+09f, 0f, 1f);
			float num2 = 0f;
			if (MainGame.Instance.RightPlayer != null)
			{
				val = position - MainGame.Instance.RightPlayer.TheShip.Position;
				num2 = MathHelper.Clamp(1f - ((Vector3)(ref val)).LengthSquared() / 7E+09f, 0f, 1f);
			}
			if ((int)player.EngineSound.State != 0)
			{
				player.EngineSound.Resume();
			}
			player.EngineSound.Volume = thrust * MathHelper.Max(num, num2);
			player.EngineSound.Pitch = speed / 100f;
		}
		catch (Exception)
		{
		}
	}

	public void StopEngines(Player player)
	{
		player.EngineSound.Pause();
	}

	public void Reset()
	{
		foreach (Player value in MainGame.Players.PlayerMap.Values)
		{
			value.EngineSound.Pause();
		}
	}

	public void Update()
	{
	}
}
