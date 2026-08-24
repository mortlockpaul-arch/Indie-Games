using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace AircraftRC;

public class Ambiance
{
	private Cue oiseau1;

	private Cue oiseau2;

	private Cue oiseau3;

	private Cue oiseau4;

	private Cue oiseau5;

	private Cue oiseau6;

	private Cue oiseau7;

	private AudioEmitter emitterambi1 = new AudioEmitter();

	private AudioEmitter emitterambi2 = new AudioEmitter();

	private AudioEmitter emitterambi3 = new AudioEmitter();

	private AudioEmitter emitterambi4 = new AudioEmitter();

	private AudioEmitter emitterambi5 = new AudioEmitter();

	private AudioEmitter emitterambi6 = new AudioEmitter();

	private AudioEmitter emitterambi7 = new AudioEmitter();

	private AudioListener listenerambi = new AudioListener();

	private Random o1;

	private float Volume = 100f;

	public Ambiance(CustomPhysicsGame game)
	{
		o1 = new Random();
	}

	public void Oiseau1(CustomPhysicsGame game)
	{
		oiseau1 = game.soundBank.GetCue("oiseau1");
		oiseau1.SetVariable("Volume", Volume);
		oiseau1.Apply3D(listenerambi, emitterambi1);
		oiseau1.Play();
	}

	public void Oiseau2(CustomPhysicsGame game)
	{
		oiseau2 = game.soundBank.GetCue("oiseau2");
		oiseau2.SetVariable("Volume", Volume);
		oiseau2.Apply3D(listenerambi, emitterambi2);
		oiseau2.Play();
	}

	public void Oiseau3(CustomPhysicsGame game)
	{
		oiseau3 = game.soundBank.GetCue("oiseau3");
		oiseau3.SetVariable("Volume", Volume);
		oiseau3.Apply3D(listenerambi, emitterambi3);
		oiseau3.Play();
	}

	public void Oiseau4(CustomPhysicsGame game)
	{
		oiseau4 = game.soundBank.GetCue("oiseau4");
		oiseau4.SetVariable("Volume", Volume);
		oiseau4.Apply3D(listenerambi, emitterambi4);
		oiseau4.Play();
	}

	public void Oiseau5(CustomPhysicsGame game)
	{
		oiseau5 = game.soundBank.GetCue("oiseau5");
		oiseau5.SetVariable("Volume", Volume);
		oiseau5.Apply3D(listenerambi, emitterambi5);
		oiseau5.Play();
	}

	public void Oiseau6(CustomPhysicsGame game)
	{
		oiseau6 = game.soundBank.GetCue("oiseau6");
		oiseau6.SetVariable("Volume", Volume);
		oiseau6.Apply3D(listenerambi, emitterambi6);
		oiseau6.Play();
	}

	public void Oiseau7(CustomPhysicsGame game)
	{
		oiseau7 = game.soundBank.GetCue("oiseau7");
		oiseau7.SetVariable("Volume", Volume);
		oiseau7.Apply3D(listenerambi, emitterambi7);
		oiseau7.Play();
	}

	public void UpdateSons(CustomPhysicsGame game)
	{
		int num = o1.Next(0, 9500);
		int num2 = o1.Next(0, 10500);
		int num3 = o1.Next(0, 9500);
		int num4 = o1.Next(0, 10500);
		int num5 = o1.Next(0, 9500);
		int num6 = o1.Next(0, 10500);
		int num7 = o1.Next(0, 9500);
		if (num == 1226)
		{
			Oiseau1(game);
		}
		if (num2 == 851)
		{
			Oiseau2(game);
		}
		if (num3 == 1112)
		{
			Oiseau3(game);
		}
		if (num4 == 5602)
		{
			Oiseau4(game);
		}
		if (num5 == 1333)
		{
			Oiseau5(game);
		}
		if (num6 == 283)
		{
			Oiseau6(game);
		}
		if (num7 == 3283)
		{
			Oiseau7(game);
		}
		emitterambi1.Position = new Vector3(o1.Next(10, 100), o1.Next(10, 100), o1.Next(10, 100));
		emitterambi2.Position = new Vector3(o1.Next(10, 100), o1.Next(10, 100), o1.Next(10, 100));
		emitterambi3.Position = new Vector3(o1.Next(10, 100), o1.Next(10, 100), o1.Next(10, 100));
		emitterambi4.Position = new Vector3(o1.Next(10, 100), o1.Next(10, 100), o1.Next(10, 100));
		emitterambi5.Position = new Vector3(o1.Next(10, 100), o1.Next(10, 100), o1.Next(10, 100));
		emitterambi6.Position = new Vector3(o1.Next(10, 100), o1.Next(10, 100), o1.Next(10, 100));
		emitterambi7.Position = new Vector3(o1.Next(10, 100), o1.Next(10, 100), o1.Next(10, 100));
	}
}
