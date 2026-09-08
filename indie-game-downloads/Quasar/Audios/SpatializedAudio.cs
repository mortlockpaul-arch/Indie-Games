using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Quasar.Elements;

namespace Quasar.Audios;

public class SpatializedAudio : Audio
{
	private SoundEffect buffer;

	private SoundEffectInstance instance;

	private bool instanceError;

	private AudioEmitter emitter;

	private AudioListener listener;

	private Camera camera;

	private Vector3 velocity;

	private bool autoSpatialize = true;

	private bool loop;

	private float speed = 1f;

	private float volume = 1f;

	public static float DistanceScale
	{
		get
		{
			return SoundEffect.DistanceScale;
		}
		set
		{
			SoundEffect.DistanceScale = value;
		}
	}

	public Camera Camera
	{
		get
		{
			return camera;
		}
		set
		{
			camera = value;
		}
	}

	public Vector3 Velocity
	{
		get
		{
			return velocity;
		}
		set
		{
			velocity = value;
		}
	}

	public override float Pan
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public bool AutoSpatialize
	{
		get
		{
			return autoSpatialize;
		}
		set
		{
			autoSpatialize = value;
		}
	}

	public override bool Loop
	{
		get
		{
			return loop;
		}
		set
		{
			loop = value;
		}
	}

	public override float Speed
	{
		get
		{
			return speed;
		}
		set
		{
			speed = value;
			if (instance != null)
			{
				instance.Pitch = speed - 1f;
			}
		}
	}

	public override bool Playing
	{
		get
		{
			if (instance == null)
			{
				return false;
			}
			return instance.State == SoundState.Playing;
		}
	}

	public override float Volume
	{
		get
		{
			return volume;
		}
		set
		{
			volume = value;
			if (instance != null)
			{
				instance.Volume = value * Audio.SoundVolume;
			}
		}
	}

	public SpatializedAudio(SoundEffect buffer)
	{
		this.buffer = buffer;
		instance = null;
		listener = new AudioListener();
		emitter = new AudioEmitter();
	}

	public SpatializedAudio(SoundEffect buffer, Camera camera)
		: this(buffer)
	{
		this.camera = camera;
	}

	public void Update(Element element)
	{
		if (Playing && AutoSpatialize)
		{
			Camera camera = ((this.camera != null) ? this.camera : Scene.CurrentInstance.Camera);
			if (camera != null)
			{
				listener.Forward = -camera.Transform.ZVector;
				listener.Up = camera.Transform.WorldY;
				listener.Position = camera.Transform.WorldTranslation;
			}
			emitter.Position = element.Transform.WorldTranslation;
			instance.Apply3D(listener, emitter);
		}
	}

	public override void Start()
	{
		if (instance == null)
		{
			if (!instanceError)
			{
				try
				{
					instance = buffer.CreateInstance();
					instance.Volume = volume * Audio.SoundVolume;
					instance.IsLooped = Loop;
					instance.Pitch = speed - 1f;
					instance.Pan = 0f;
					instance.Apply3D(listener, emitter);
					instance.Play();
				}
				catch (Exception)
				{
					instanceError = true;
				}
			}
		}
		else
		{
			instance.Volume = volume * Audio.SoundVolume;
			instance.Pitch = speed - 1f;
			instance.Apply3D(listener, emitter);
			instance.Resume();
		}
	}

	public override void Stop()
	{
		if (instance != null && !instance.IsDisposed)
		{
			instance.Stop();
		}
	}

	public override void Pause()
	{
		if (instance != null && !instance.IsDisposed)
		{
			instance.Pause();
		}
	}

	~SpatializedAudio()
	{
		Dispose();
	}

	public override void Dispose()
	{
		if (instance != null)
		{
			instance.Dispose();
		}
		instance = null;
		buffer = null;
		base.Dispose();
	}
}
