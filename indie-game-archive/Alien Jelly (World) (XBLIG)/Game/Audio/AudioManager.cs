using System.Collections.Generic;
using GKEngine.Entities;
using GKEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Game.Audio;

public class AudioManager
{
	public static AudioEngine AUDIOENGINE;

	public static WaveBank WAVEBANK;

	public static SoundBank SOUNDBANK;

	public static string PATH;

	public AudioEngine audioEngine;

	public bool active;

	public Scene scene;

	public WaveBank waveBank;

	public SoundBank soundBank;

	public List<AudioEventCue> eventCues = new List<AudioEventCue>();

	private int eventCuesCount;

	public AudioListener listener = new AudioListener();

	public AudioEmitter emitter = new AudioEmitter();

	protected Dictionary<string, List<Audio3D>> audio3DEvents = new Dictionary<string, List<Audio3D>>();

	public Base3D focus;

	public AudioManager(Scene oScene, Base3D oFocus)
	{
		scene = oScene;
		focus = oFocus;
		Init();
	}

	public virtual void Init()
	{
		audioEngine = AUDIOENGINE;
		waveBank = WAVEBANK;
		soundBank = SOUNDBANK;
		active = true;
	}

	public virtual void Update(GameTime oGameTime)
	{
		if (!active || audioEngine == null || audioEngine.IsDisposed)
		{
			return;
		}
		audioEngine.Update();
		for (int i = 0; i < eventCuesCount; i++)
		{
			if (eventCues[i].active)
			{
				eventCues[i].Play();
				eventCues[i].active = false;
			}
		}
	}

	public virtual void Dispose()
	{
		for (int i = 0; i < eventCues.Count; i++)
		{
			eventCues[i].Dispose();
		}
		eventCues.Clear();
		audio3DEvents.Clear();
		eventCues = null;
		audio3DEvents = null;
		listener = null;
		emitter = null;
		soundBank = null;
		waveBank = null;
		audioEngine = null;
	}

	public virtual void SetScene(Scene oScene)
	{
		scene = oScene;
	}

	public virtual void SetFocus(Base3D oBase)
	{
		focus = oBase;
	}

	public virtual void EventCues_Add(AudioEventCue oEventCue)
	{
		eventCues.Add(oEventCue);
		eventCuesCount = eventCues.Count;
	}

	public virtual void EventCues_Trigger(string xName)
	{
		for (int i = 0; i < eventCuesCount; i++)
		{
			if (eventCues[i].name == xName)
			{
				eventCues[i].active = true;
				break;
			}
		}
	}

	public static void Initialise(string pPath)
	{
		PATH = pPath;
		AUDIOENGINE = new AudioEngine(PATH + "/Xbox/Audio.xgs");
		WAVEBANK = new WaveBank(AUDIOENGINE, PATH + "/Xbox/Wave Bank.xwb");
		SOUNDBANK = new SoundBank(AUDIOENGINE, PATH + "/Xbox/Sound Bank.xsb");
	}

	public static void Unload()
	{
		if (SOUNDBANK != null)
		{
			SOUNDBANK.Dispose();
		}
		if (WAVEBANK != null)
		{
			WAVEBANK.Dispose();
		}
		if (AUDIOENGINE != null)
		{
			AUDIOENGINE.Dispose();
		}
		SOUNDBANK = null;
		WAVEBANK = null;
		AUDIOENGINE = null;
	}
}
