using System.Collections.Generic;
using GKEngine.Entities;
using GKEngine.Scenes;
using Game.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Game.Audio;

public class GameAudio : AudioManager
{
	public const float DISTANCE = 500f;

	public static readonly Range VOLUME_SETTING = new Range(0f, 10f);

	public static readonly Range VOLUME_VALUE = new Range(0f, 2f);

	private Vector3 _temp_vector = default(Vector3);

	private Vector3 _temp_vector_update = default(Vector3);

	private Vector3 _temp_vector_getfree = default(Vector3);

	public bool populate;

	public AudioCategory categorySFX;

	public AudioCategory categoryMusic;

	public MusicManager music;

	public AudioEventCue soundRewind;

	private float _volumeSFX;

	private float _volumeMusic;

	public float volumeSFX
	{
		get
		{
			return _volumeSFX;
		}
		set
		{
			_volumeSFX = value;
			categorySFX.SetVolume(_volumeSFX);
		}
	}

	public float volumeMusic
	{
		get
		{
			return _volumeMusic;
		}
		set
		{
			_volumeMusic = value;
			categoryMusic.SetVolume(_volumeMusic);
		}
	}

	public GameAudio(Scene oScene, Base3D oFocus)
		: base(oScene, oFocus)
	{
	}

	public override void Init()
	{
		base.Init();
		music = new MusicManager(this);
		soundRewind = new AudioEventCue(this, "Sound_Rewind");
		categorySFX = audioEngine.GetCategory("SFX");
		categoryMusic = audioEngine.GetCategory("Music");
		FromSettings();
		foreach (KeyValuePair<string, List<Audio3D>> audio3DEvent in audio3DEvents)
		{
			int capacity = audio3DEvent.Value.Capacity;
			for (int i = 0; i < capacity; i++)
			{
				audio3DEvent.Value.Add(new Audio3D(this, audio3DEvent.Key));
			}
		}
		Init_EventCues();
	}

	private void Init_EventCues()
	{
		EventCues_Add(new AudioEventCue(this, "Sound_Over_0"));
		EventCues_Add(new AudioEventCue(this, "Sound_Click_0"));
		EventCues_Add(new AudioEventCue(this, "Sound_Squish"));
		EventCues_Add(new AudioEventCue(this, "Sound_Splat"));
		EventCues_Add(new AudioEventCue(this, "Sound_Collect"));
		EventCues_Add(new AudioEventCue(this, "Sound_Crate"));
		EventCues_Add(new AudioEventCue(this, "Portal_In"));
		EventCues_Add(new AudioEventCue(this, "Portal_Out"));
		EventCues_Add(new AudioEventCue(this, "Phase_In"));
		EventCues_Add(new AudioEventCue(this, "Phase_Out"));
		EventCues_Add(new AudioEventCue(this, "Speech_Bubble"));
		EventCues_Add(new AudioEventCue(this, "Switch"));
		EventCues_Add(new AudioEventCue(this, "Intro Speech"));
		EventCues_Add(new AudioEventCue(this, "Sound_Success"));
		EventCues_Add(new AudioEventCue(this, "Sound_Fail"));
		EventCues_Add(new AudioEventCue(this, "Sound_Button"));
		EventCues_Add(new AudioEventCue(this, "Sound_Place"));
		EventCues_Add(new AudioEventCue(this, "Collective Mass"));
		EventCues_Add(new AudioEventCue(this, "CM Buzz"));
		EventCues_Add(new AudioEventCue(this, "Exit"));
		EventCues_Add(new AudioEventCue(this, "Push"));
		EventCues_Add(new AudioEventCue(this, "Special Event"));
		EventCues_Add(new AudioEventCue(this, "Button"));
		EventCues_Add(new AudioEventCue(this, "Robot Die"));
		EventCues_Add(new AudioEventCue(this, "Build Snap"));
		EventCues_Add(new AudioEventCue(this, "Build Move"));
		EventCues_Add(new AudioEventCue(this, "Build Axis Change"));
		EventCues_Add(new AudioEventCue(this, "Build Other"));
		EventCues_Add(new AudioEventCue(this, "Menu In"));
		EventCues_Add(new AudioEventCue(this, "Menu Out"));
	}

	public override void Update(GameTime oGameTime)
	{
		music.Update(oGameTime);
		try
		{
			listener.Position = scene.cameras.camera.position;
			listener.Up = scene.cameras.camera.matrix.Up;
			listener.Forward = scene.cameras.camera.matrix.Forward;
			foreach (KeyValuePair<string, List<Audio3D>> audio3DEvent in audio3DEvents)
			{
				int capacity = audio3DEvent.Value.Capacity;
				for (int i = 0; i < capacity; i++)
				{
					if (audio3DEvent.Value[i].trigger)
					{
						audio3DEvent.Value[i].OneShot3D(audio3DEvent.Value[i].item);
					}
					else if (audio3DEvent.Value[i].active)
					{
						if (audio3DEvent.Value[i].cue.IsStopped)
						{
							audio3DEvent.Value[i].active = false;
						}
						emitter.Position = audio3DEvent.Value[i].item.position;
						emitter.Forward = audio3DEvent.Value[i].item.matrix.Forward;
						emitter.Up = audio3DEvent.Value[i].item.matrix.Up;
						audio3DEvent.Value[i].cue.Apply3D(listener, emitter);
						float value = 1f - MathHelper.Clamp(_temp_vector_update.Length(), 0f, 500f) / 500f;
						audio3DEvent.Value[i].cue.SetVariable("Volume", value);
					}
				}
			}
		}
		catch
		{
		}
		base.Update(oGameTime);
	}

	public override void Dispose()
	{
		try
		{
			foreach (KeyValuePair<string, List<Audio3D>> audio3DEvent in audio3DEvents)
			{
				int capacity = audio3DEvent.Value.Capacity;
				for (int i = 0; i < capacity; i++)
				{
					if (audio3DEvent.Value[i].active)
					{
						audio3DEvent.Value[i].cue.Stop(AudioStopOptions.Immediate);
					}
				}
			}
		}
		catch
		{
		}
		soundRewind.cue.Stop(AudioStopOptions.Immediate);
		soundRewind = null;
		music.Stop();
		music.Dispose();
		base.Dispose();
	}

	public void FromSettings()
	{
		if (DataManager.local != null)
		{
			volumeSFX = VOLUME_VALUE.Lerp(VOLUME_SETTING.Ratio(DataManager.local.settings.volumeFX));
			volumeMusic = VOLUME_VALUE.Lerp(VOLUME_SETTING.Ratio(DataManager.local.settings.volumeMusic));
		}
	}

	public Audio3D Audio3DEvent_Add(string xName, Base3D oItem)
	{
		Audio3D result = null;
		_temp_vector.X = focus.position.X - oItem.position.X;
		_temp_vector.Y = focus.position.Y - oItem.position.Y;
		_temp_vector.Z = focus.position.Z - oItem.position.Z;
		float num = _temp_vector.Length();
		if (num < 500f)
		{
			int num2 = Audio3DEvent_GetFree(xName, num);
			if (num2 > -1)
			{
				audio3DEvents[xName][num2].item = oItem;
				audio3DEvents[xName][num2].trigger = true;
				result = audio3DEvents[xName][num2];
			}
		}
		return result;
	}

	public int Audio3DEvent_GetFree(string xName, float xDistance)
	{
		int num = -1;
		int count = audio3DEvents[xName].Count;
		for (int i = 0; i < count; i++)
		{
			if (!audio3DEvents[xName][i].active)
			{
				num = i;
				break;
			}
		}
		if (num < 0)
		{
			int num2 = -1;
			float num3 = 0f;
			count = audio3DEvents[xName].Count;
			for (int j = 0; j < count; j++)
			{
				_temp_vector_getfree.X = focus.position.X - audio3DEvents[xName][j].item.position.X;
				_temp_vector_getfree.Y = focus.position.Y - audio3DEvents[xName][j].item.position.Y;
				_temp_vector_getfree.Z = focus.position.Z - audio3DEvents[xName][j].item.position.Z;
				if (_temp_vector_getfree.Length() > num3)
				{
					num3 = _temp_vector_getfree.Length();
					num2 = j;
				}
			}
			if (num2 > -1 && num3 >= xDistance)
			{
				num = num2;
			}
		}
		return num;
	}

	public void Audio3DEvent_Stop(string xName, Base3D oItem)
	{
		int count = audio3DEvents[xName].Count;
		for (int i = 0; i < count; i++)
		{
			if (audio3DEvents[xName][i].active && audio3DEvents[xName][i].item == oItem)
			{
				audio3DEvents[xName][i].cue.Stop(AudioStopOptions.Immediate);
				break;
			}
		}
	}
}
