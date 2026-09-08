using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Quasar.Elements;

namespace Quasar.Audios;

public class SpatializedPooledAudio : Audio
{
	private const int DEFAULT_INSTANCES = 3;

	private static float defaultDistance = 100f;

	private List<SpatializedAudio> audios;

	private List<Element> elements;

	private int currentAudio;

	private bool autoSpatialize = true;

	private float distanceLimitSq;

	public static float DefaultDistanceLimit
	{
		get
		{
			return defaultDistance;
		}
		set
		{
			defaultDistance = Math.Abs(value);
		}
	}

	public int Count => audios.Count;

	public int ActiveSounds
	{
		get
		{
			int num = 0;
			for (int i = 0; i < audios.Count; i++)
			{
				if (audios[i].Playing)
				{
					num++;
				}
			}
			return num;
		}
	}

	public float Usage => (float)ActiveSounds / (float)audios.Count;

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

	private SpatializedAudio CurrentAudio => audios[currentAudio];

	public Camera Camera
	{
		get
		{
			if (audios.Count > 0)
			{
				return audios[0].Camera;
			}
			return null;
		}
		set
		{
			foreach (SpatializedAudio audio in audios)
			{
				audio.Camera = value;
			}
		}
	}

	public override float Pan
	{
		get
		{
			return CurrentAudio.Pan;
		}
		set
		{
			CurrentAudio.Pan = value;
		}
	}

	public override bool Playing => CurrentAudio.Playing;

	public override float Speed
	{
		get
		{
			return CurrentAudio.Speed;
		}
		set
		{
			CurrentAudio.Speed = value;
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
			return false;
		}
		set
		{
		}
	}

	public float DistanceLimit
	{
		set
		{
			distanceLimitSq = Math.Abs(value);
			distanceLimitSq *= distanceLimitSq;
		}
	}

	public override float Volume
	{
		get
		{
			return CurrentAudio.Volume;
		}
		set
		{
			foreach (SpatializedAudio audio in audios)
			{
				audio.Volume = value;
			}
		}
	}

	public SpatializedPooledAudio(SoundEffect buffer, int instances)
	{
		audios = new List<SpatializedAudio>(instances);
		elements = new List<Element>(instances);
		for (int i = 0; i < instances; i++)
		{
			audios.Add(new SpatializedAudio(buffer));
			elements.Add(new Element());
		}
		currentAudio = 0;
		DistanceLimit = defaultDistance;
	}

	private void switchToNextAudio()
	{
		currentAudio = (currentAudio + 1) % audios.Count;
	}

	public SpatializedPooledAudio(SoundEffect buffer)
		: this(buffer, 3)
	{
	}

	public SpatializedPooledAudio(SoundEffect buffer, Camera cam)
		: this(buffer, 3)
	{
		Camera = cam;
	}

	public void Update()
	{
		for (int i = 0; i < audios.Count; i++)
		{
			if (audios[i].Playing)
			{
				audios[i].Update(elements[i]);
			}
		}
	}

	public override void Start()
	{
		CurrentAudio.Start();
		switchToNextAudio();
	}

	private bool updateDistance(Vector3 position)
	{
		if ((CurrentAudio.Camera.Transform.WorldTranslation - position).LengthSquared() <= distanceLimitSq)
		{
			elements[currentAudio].Transform.Translation = position;
			return true;
		}
		return true;
	}

	public void Start(Vector3 position)
	{
		if (updateDistance(position))
		{
			Start();
		}
	}

	public void Start(Vector3 position, float pitchVariation)
	{
		if (updateDistance(position))
		{
			Start(pitchVariation);
		}
	}

	public override void Stop()
	{
		if (audios == null)
		{
			return;
		}
		foreach (SpatializedAudio audio in audios)
		{
			audio.Stop();
		}
	}

	public override void Pause()
	{
		if (audios == null)
		{
			return;
		}
		foreach (SpatializedAudio audio in audios)
		{
			audio.Pause();
		}
	}

	public override void Dispose()
	{
		if (audios != null)
		{
			foreach (SpatializedAudio audio in audios)
			{
				audio.Dispose();
			}
			audios = null;
		}
		base.Dispose();
	}
}
