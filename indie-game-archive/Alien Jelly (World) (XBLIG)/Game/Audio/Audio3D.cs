using GKEngine.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Game.Audio;

public class Audio3D
{
	private Vector3 _temp_vector = default(Vector3);

	public GameAudio manager;

	public string name;

	public Base3D item;

	public Cue cue;

	public bool active;

	public bool trigger;

	public Audio3D(GameAudio oManager, string xName)
	{
		manager = oManager;
		name = xName;
		cue = manager.soundBank.GetCue(name);
		active = false;
	}

	public Audio3D(GameAudio oManager, string xName, Base3D oItem)
	{
		manager = oManager;
		name = xName;
		item = oItem;
		cue = manager.soundBank.GetCue(name);
		active = false;
	}

	public virtual void OneShot3D(Base3D oItem)
	{
		item = oItem;
		active = true;
		trigger = false;
		if (cue != null && !cue.IsStopped)
		{
			cue.Stop(AudioStopOptions.Immediate);
		}
		cue = manager.soundBank.GetCue(name);
		cue.Apply3D(manager.listener, manager.emitter);
		_temp_vector.X = manager.focus.position.X - item.position.X;
		_temp_vector.Y = manager.focus.position.Y - item.position.Y;
		_temp_vector.Z = manager.focus.position.Z - item.position.Z;
		float value = 1f - MathHelper.Clamp(_temp_vector.Length(), 0f, 500f) / 500f;
		cue.SetVariable("Volume", value);
		cue.Play();
	}

	public virtual void Play()
	{
		if (cue.IsPaused)
		{
			cue.Resume();
		}
		else if (cue.IsPlaying)
		{
			cue.Stop(AudioStopOptions.Immediate);
			cue = manager.soundBank.GetCue(name);
			cue.Play();
		}
		else
		{
			cue = manager.soundBank.GetCue(name);
			cue.Play();
		}
	}

	public virtual void Stop()
	{
		cue.Stop(AudioStopOptions.Immediate);
	}
}
