using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Maximinus;

public class Audio : ObjUpdate
{
	public struct EngineState(bool a, float p, float v)
	{
		public bool Alive = a;

		public float Pitch = p;

		public float Volume = v;
	}

	public static Audio Instance;

	public EngineState[] engineStates;

	private EngineSound[] engineSounds;

	public Audio(int howManyEngines, string pathEngineSound)
	{
		if (Instance == null)
		{
			Instance = this;
			SoundEffect soundEffect = MaximinusGame.ContentManager.Load<SoundEffect>(pathEngineSound);
			engineStates = new EngineState[howManyEngines];
			engineSounds = new EngineSound[howManyEngines];
			for (int i = 0; i < howManyEngines; i++)
			{
				ref EngineState reference = ref engineStates[i];
				reference = new EngineState(a: true, 0f, 1f);
				engineSounds[i] = new EngineSound(soundEffect.CreateInstance(), 1f);
			}
			return;
		}
		throw new Exception("multiple audio engines not supported");
	}

	public override void Update(GameTime gameTime)
	{
		for (int i = 0; i < engineSounds.Length; i++)
		{
			engineSounds[i].UpdateNew(gameTime, engineStates[i].Alive, engineStates[i].Pitch, engineStates[i].Volume);
		}
	}
}
