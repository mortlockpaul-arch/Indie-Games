using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Storage;

namespace DepthAttack.CS;

public class BGM(Game game) : DrawableGameComponent(game)
{
	private const string cstrBgm00 = "Stage01C1_0206A";

	private const string cstrBgm01 = "Stage01C1_0206C";

	private const string cstrBgm02 = "Stage04C1_0209A";

	private const string cstrBgm03 = "Stage04C1_0209B";

	private const string cstrSEKettei00 = "BGM\\SE\\Kettei00";

	private const string cstrSECancel00 = "BGM\\SE\\Cancel00";

	private const string cstrSETamaHoming00 = "BGM\\SE\\Tama03";

	private const string cstrSETamaVulcan00 = "BGM\\SE\\Tama07";

	private const string cstrSERankIn00 = "BGM\\SE\\RankIn_B9_15262";

	private const string cstrSECPUTama00 = "BGM\\SE\\Hit01";

	private const string cstrSEBakuhatu00 = "BGM\\SE\\Hit00";

	private const string cstrSEBakuhatu01 = "BGM\\SE\\BakuhatuA1_08145";

	private const string cstrSEBakuhatu02 = "BGM\\SE\\BakuhatuB9_06090";

	private const string cstrSEBakuhatu03 = "BGM\\SE\\BakuhatuB9_02030";

	private const string pcStrContainerSaveOptionName = "Option_00.dat";

	public float fltSEVolume;

	public float fltBGMVolume;

	private AudioEngine engine;

	private SoundBank soundBank;

	private WaveBank waveBank;

	private AudioCategory audiioCategory;

	private Cue[] bgm = new Cue[4];

	public bool[] pflgBgm = new bool[4];

	public bool[] pflgSEKetteiStart = new bool[1];

	public bool[] pflgSECancelStart = new bool[1];

	private SoundEffect[] seKettei = new SoundEffect[1];

	private SoundEffect[] seCancel = new SoundEffect[1];

	private SoundEffect[] seRankIn = new SoundEffect[1];

	public bool[] pflgSERankIn = new bool[1];

	public bool[] pflgSEPlayerTama = new bool[2];

	private SoundEffect[] sePlayerTama = new SoundEffect[2];

	public bool[] pflgSECPUTama = new bool[1];

	private SoundEffect[] seCPUTama = new SoundEffect[1];

	public bool[] pflgSEBakuhatu = new bool[4];

	private SoundEffect[] seBakuhatu = new SoundEffect[4];

	public override void Initialize()
	{
		fltSEVolume = 0.5f;
		fltBGMVolume = 0.5f;
		base.Initialize();
	}

	public void pflgBGMON(int intBGMNo)
	{
		for (int i = 0; i < pflgBgm.Length; i++)
		{
			pflgBgm[i] = false;
		}
		pflgBgm[intBGMNo] = true;
	}

	public void pflgBGMOFF()
	{
		for (int i = 0; i < pflgBgm.Length; i++)
		{
			pflgBgm[i] = false;
		}
	}

	public override void Update(GameTime gameTime)
	{
		for (int i = 0; i < pflgBgm.Length; i++)
		{
			if (pflgBgm[i])
			{
				if (!bgm[i].IsPlaying)
				{
					if (bgm[i].IsStopped)
					{
						bgm[i] = soundBank.GetCue("Stage01C1_0206A");
						bgm[i].Resume();
					}
					else
					{
						bgm[i].Play();
					}
				}
			}
			else if (bgm[i].IsPlaying)
			{
				bgm[i].Stop(AudioStopOptions.Immediate);
			}
		}
		for (int i = 0; i < pflgSEKetteiStart.Length; i++)
		{
			if (pflgSEKetteiStart[i])
			{
				seKettei[i].Play(fltSEVolume, 0f, 0f);
				pflgSEKetteiStart[i] = false;
			}
		}
		for (int i = 0; i < pflgSECancelStart.Length; i++)
		{
			if (pflgSECancelStart[i])
			{
				seCancel[i].Play(fltSEVolume, 0f, 0f);
				pflgSECancelStart[i] = false;
			}
		}
		for (int i = 0; i < pflgSERankIn.Length; i++)
		{
			if (pflgSERankIn[i])
			{
				seRankIn[i].Play(fltSEVolume, 0f, 0f);
				pflgSERankIn[i] = false;
			}
		}
		for (int i = 0; i < pflgSEPlayerTama.Length; i++)
		{
			if (pflgSEPlayerTama[i])
			{
				sePlayerTama[i].Play(fltSEVolume, 0f, 0f);
				pflgSEPlayerTama[i] = false;
			}
		}
		for (int i = 0; i < pflgSECPUTama.Length; i++)
		{
			if (pflgSECPUTama[i])
			{
				seCPUTama[i].Play(fltSEVolume, 0f, 0f);
				pflgSECPUTama[i] = false;
			}
		}
		for (int i = 0; i < pflgSEBakuhatu.Length; i++)
		{
			if (pflgSEBakuhatu[i])
			{
				seBakuhatu[i].Play(fltSEVolume, 0f, 0f);
				pflgSEBakuhatu[i] = false;
			}
		}
		base.Update(gameTime);
	}

	public void BGMSetVolume()
	{
		audiioCategory.SetVolume(fltBGMVolume);
	}

	protected override void LoadContent()
	{
		engine = new AudioEngine("Content\\BGM\\DA_BGM.xgs");
		soundBank = new SoundBank(engine, "Content\\BGM\\Sound Bank.xsb");
		waveBank = new WaveBank(engine, "Content\\BGM\\Wave Bank.xwb");
		bgm[0] = soundBank.GetCue("Stage01C1_0206A");
		bgm[1] = soundBank.GetCue("Stage01C1_0206C");
		bgm[2] = soundBank.GetCue("Stage04C1_0209A");
		bgm[3] = soundBank.GetCue("Stage04C1_0209B");
		audiioCategory = engine.GetCategory("Music");
		audiioCategory.SetVolume(fltBGMVolume);
		seKettei[0] = base.Game.Content.Load<SoundEffect>("BGM\\SE\\Kettei00");
		seCancel[0] = base.Game.Content.Load<SoundEffect>("BGM\\SE\\Cancel00");
		seRankIn[0] = base.Game.Content.Load<SoundEffect>("BGM\\SE\\RankIn_B9_15262");
		sePlayerTama[0] = base.Game.Content.Load<SoundEffect>("BGM\\SE\\Tama03");
		sePlayerTama[1] = base.Game.Content.Load<SoundEffect>("BGM\\SE\\Tama07");
		seCPUTama[0] = base.Game.Content.Load<SoundEffect>("BGM\\SE\\Hit01");
		seBakuhatu[0] = base.Game.Content.Load<SoundEffect>("BGM\\SE\\Hit00");
		seBakuhatu[1] = base.Game.Content.Load<SoundEffect>("BGM\\SE\\BakuhatuA1_08145");
		seBakuhatu[2] = base.Game.Content.Load<SoundEffect>("BGM\\SE\\BakuhatuB9_06090");
		seBakuhatu[3] = base.Game.Content.Load<SoundEffect>("BGM\\SE\\BakuhatuB9_02030");
		base.LoadContent();
	}

	protected override void UnloadContent()
	{
		base.UnloadContent();
	}

	public void volumeRead()
	{
		if (Game1.pStorageDevice == null)
		{
			return;
		}
		IAsyncResult asyncResult = Game1.pStorageDevice.BeginOpenContainer("DepthAttack00", null, null);
		try
		{
			asyncResult.AsyncWaitHandle.WaitOne();
			StorageContainer storageContainer = Game1.pStorageDevice.EndOpenContainer(asyncResult);
			asyncResult.AsyncWaitHandle.Close();
			if (!storageContainer.FileExists("Option_00.dat"))
			{
				storageContainer.Dispose();
				return;
			}
			Stream stream = storageContainer.OpenFile("Option_00.dat", FileMode.Open);
			byte[] array = new byte[4];
			stream.Read(array, 0, 4);
			fltBGMVolume = BitConverter.ToSingle(array, 0);
			stream.Read(array, 0, 4);
			fltSEVolume = BitConverter.ToSingle(array, 0);
			stream.Close();
			storageContainer.Dispose();
		}
		catch
		{
			Game1.pStorageDevice = null;
		}
	}

	public void volumeSave()
	{
		if (Game1.pStorageDevice != null)
		{
			IAsyncResult asyncResult = Game1.pStorageDevice.BeginOpenContainer("DepthAttack00", null, null);
			try
			{
				asyncResult.AsyncWaitHandle.WaitOne();
				StorageContainer storageContainer = Game1.pStorageDevice.EndOpenContainer(asyncResult);
				asyncResult.AsyncWaitHandle.Close();
				Stream stream = storageContainer.OpenFile("Option_00.dat", FileMode.Create);
				byte[] array = new byte[4];
				array = BitConverter.GetBytes(fltBGMVolume);
				stream.Write(array, 0, 4);
				array = BitConverter.GetBytes(fltSEVolume);
				stream.Write(array, 0, 4);
				stream.Close();
				storageContainer.Dispose();
			}
			catch
			{
				Game1.pStorageDevice = null;
			}
		}
	}
}
