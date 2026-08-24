using BEPUphysics;
using BEPUphysics.Collidables;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Rendering;

namespace AircraftRC;

public class Jeux(CustomPhysicsGame game)
{
	private DetectorVolume VolPorteV1;

	private DetectorVolume VolPorteV2;

	private DetectorVolume VolPorteV3;

	private DetectorVolume VolPorteV4;

	private DetectorVolume VolPorteV5;

	private DetectorVolume VolPorteV6;

	private DetectorVolume VolPorteV7;

	private DetectorVolume VolPorteV8;

	private DetectorVolume VolPorteV9;

	private DetectorVolume VolPorteV10;

	private Model volPV1;

	private Model volPV2;

	private Model volPV3;

	private Model volPV4;

	private Model volPV5;

	private Model volPV6;

	private Model volPV7;

	private Model volPV8;

	private Model volPV9;

	private Model volPV10;

	private Model volPR1;

	private Model volPR2;

	private Model volPR3;

	private Model volPR4;

	private Model volPR5;

	private Model volPR6;

	private Model volPR7;

	private Model volPR8;

	private Model volPR9;

	private Model volPR10;

	private SceneObject objVolPV1;

	private SceneObject objVolPV2;

	private SceneObject objVolPV3;

	private SceneObject objVolPV4;

	private SceneObject objVolPV5;

	private SceneObject objVolPV6;

	private SceneObject objVolPV7;

	private SceneObject objVolPV8;

	private SceneObject objVolPV9;

	private SceneObject objVolPV10;

	private SceneObject objVolPR1;

	private SceneObject objVolPR2;

	private SceneObject objVolPR3;

	private SceneObject objVolPR4;

	private SceneObject objVolPR5;

	private SceneObject objVolPR6;

	private SceneObject objVolPR7;

	private SceneObject objVolPR8;

	private SceneObject objVolPR9;

	private SceneObject objVolPR10;

	public int A;

	public int ReA;

	private Space jeuxSpace;

	private TerrainP terjeux;

	public bool couleurT;

	public bool couleurG;

	public float timerS;

	public int timecounterS;

	public int timecounter1S;

	public int timecounterM;

	public int timecounter1M = 15;

	public int timecounterH;

	public int timecounter1H;

	private SoundBank soundBankb;

	private AudioEmitter emitterp = new AudioEmitter();

	private AudioListener listenerp = new AudioListener();

	public bool finAficheR;

	public bool finAficheT;

	private float minC;

	private float minCHi;

	private float totalC;

	private float totalCHi;

	private Cue portes;

	public void Load(CustomPhysicsGame game)
	{
		A = 0;
		timecounterS = 0;
		timecounterM = 0;
		jeuxSpace = game.space;
		terjeux = game.terrain;
		soundBankb = game.soundBank;
		volPV1 = game.Content.Load<Model>("Models/Portes/PorteV1");
		volPV2 = game.Content.Load<Model>("Models/Portes/PorteV2");
		volPV3 = game.Content.Load<Model>("Models/Portes/PorteV3");
		volPV4 = game.Content.Load<Model>("Models/Portes/PorteV4");
		volPV5 = game.Content.Load<Model>("Models/Portes/PorteV5");
		volPV6 = game.Content.Load<Model>("Models/Portes/PorteV6");
		volPV7 = game.Content.Load<Model>("Models/Portes/PorteV7");
		volPV8 = game.Content.Load<Model>("Models/Portes/PorteV8");
		volPV9 = game.Content.Load<Model>("Models/Portes/PorteV9");
		volPV10 = game.Content.Load<Model>("Models/Portes/PorteV10");
		volPR1 = game.Content.Load<Model>("Models/Portes/PorteR1");
		volPR2 = game.Content.Load<Model>("Models/Portes/PorteR2");
		volPR3 = game.Content.Load<Model>("Models/Portes/PorteR3");
		volPR4 = game.Content.Load<Model>("Models/Portes/PorteR4");
		volPR5 = game.Content.Load<Model>("Models/Portes/PorteR5");
		volPR6 = game.Content.Load<Model>("Models/Portes/PorteR6");
		volPR7 = game.Content.Load<Model>("Models/Portes/PorteR7");
		volPR8 = game.Content.Load<Model>("Models/Portes/PorteR8");
		volPR9 = game.Content.Load<Model>("Models/Portes/PorteR9");
		volPR10 = game.Content.Load<Model>("Models/Portes/PorteR10");
		objVolPV1 = new SceneObject(volPV1);
		objVolPV2 = new SceneObject(volPV2);
		objVolPV3 = new SceneObject(volPV3);
		objVolPV4 = new SceneObject(volPV4);
		objVolPV5 = new SceneObject(volPV5);
		objVolPV6 = new SceneObject(volPV6);
		objVolPV7 = new SceneObject(volPV7);
		objVolPV8 = new SceneObject(volPV8);
		objVolPV9 = new SceneObject(volPV9);
		objVolPV10 = new SceneObject(volPV10);
		objVolPR1 = new SceneObject(volPR1);
		objVolPR2 = new SceneObject(volPR2);
		objVolPR3 = new SceneObject(volPR3);
		objVolPR4 = new SceneObject(volPR4);
		objVolPR5 = new SceneObject(volPR5);
		objVolPR6 = new SceneObject(volPR6);
		objVolPR7 = new SceneObject(volPR7);
		objVolPR8 = new SceneObject(volPR8);
		objVolPR9 = new SceneObject(volPR9);
		objVolPR10 = new SceneObject(volPR10);
		TriangleMesh.GetVerticesAndIndicesFromModel(volPV1, out var vertices, out var indices);
		VolPorteV1 = new DetectorVolume(new TriangleMesh(new StaticMeshData(vertices, indices)));
		TriangleMesh.GetVerticesAndIndicesFromModel(volPV2, out vertices, out indices);
		VolPorteV2 = new DetectorVolume(new TriangleMesh(new StaticMeshData(vertices, indices)));
		TriangleMesh.GetVerticesAndIndicesFromModel(volPV3, out vertices, out indices);
		VolPorteV3 = new DetectorVolume(new TriangleMesh(new StaticMeshData(vertices, indices)));
		TriangleMesh.GetVerticesAndIndicesFromModel(volPV4, out vertices, out indices);
		VolPorteV4 = new DetectorVolume(new TriangleMesh(new StaticMeshData(vertices, indices)));
		TriangleMesh.GetVerticesAndIndicesFromModel(volPV5, out vertices, out indices);
		VolPorteV5 = new DetectorVolume(new TriangleMesh(new StaticMeshData(vertices, indices)));
		TriangleMesh.GetVerticesAndIndicesFromModel(volPV6, out vertices, out indices);
		VolPorteV6 = new DetectorVolume(new TriangleMesh(new StaticMeshData(vertices, indices)));
		TriangleMesh.GetVerticesAndIndicesFromModel(volPV7, out vertices, out indices);
		VolPorteV7 = new DetectorVolume(new TriangleMesh(new StaticMeshData(vertices, indices)));
		TriangleMesh.GetVerticesAndIndicesFromModel(volPV8, out vertices, out indices);
		VolPorteV8 = new DetectorVolume(new TriangleMesh(new StaticMeshData(vertices, indices)));
		TriangleMesh.GetVerticesAndIndicesFromModel(volPV9, out vertices, out indices);
		VolPorteV9 = new DetectorVolume(new TriangleMesh(new StaticMeshData(vertices, indices)));
		TriangleMesh.GetVerticesAndIndicesFromModel(volPV10, out vertices, out indices);
		VolPorteV10 = new DetectorVolume(new TriangleMesh(new StaticMeshData(vertices, indices)));
		terjeux.sceneInterfaceScene.ObjectManager.Submit(objVolPV1);
		terjeux.sceneInterfaceScene.ObjectManager.Submit(objVolPV2);
		terjeux.sceneInterfaceScene.ObjectManager.Submit(objVolPV3);
		terjeux.sceneInterfaceScene.ObjectManager.Submit(objVolPV4);
		terjeux.sceneInterfaceScene.ObjectManager.Submit(objVolPV5);
		terjeux.sceneInterfaceScene.ObjectManager.Submit(objVolPV6);
		terjeux.sceneInterfaceScene.ObjectManager.Submit(objVolPV7);
		terjeux.sceneInterfaceScene.ObjectManager.Submit(objVolPV8);
		terjeux.sceneInterfaceScene.ObjectManager.Submit(objVolPV9);
		terjeux.sceneInterfaceScene.ObjectManager.Submit(objVolPV10);
		jeuxSpace.Add(VolPorteV1);
		jeuxSpace.Add(VolPorteV2);
		jeuxSpace.Add(VolPorteV3);
		jeuxSpace.Add(VolPorteV4);
		jeuxSpace.Add(VolPorteV5);
		jeuxSpace.Add(VolPorteV6);
		jeuxSpace.Add(VolPorteV7);
		jeuxSpace.Add(VolPorteV8);
		jeuxSpace.Add(VolPorteV9);
		jeuxSpace.Add(VolPorteV10);
		VolPorteV1.EntityBeganTouching += Toucher1;
		VolPorteV2.EntityBeganTouching += Toucher2;
		VolPorteV3.EntityBeganTouching += Toucher3;
		VolPorteV4.EntityBeganTouching += Toucher4;
		VolPorteV5.EntityBeganTouching += Toucher5;
		VolPorteV6.EntityBeganTouching += Toucher6;
		VolPorteV7.EntityBeganTouching += Toucher7;
		VolPorteV8.EntityBeganTouching += Toucher8;
		VolPorteV9.EntityBeganTouching += Toucher9;
		VolPorteV10.EntityBeganTouching += Toucher10;
	}

	public void Unload(CustomPhysicsGame game)
	{
		finAficheR = false;
		finAficheT = false;
		terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPV1);
		terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPV2);
		terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPV3);
		terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPV4);
		terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPV5);
		terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPV6);
		terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPV7);
		terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPV8);
		terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPV9);
		terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPV10);
		terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPR1);
		terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPR2);
		terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPR3);
		terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPR4);
		terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPR5);
		terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPR6);
		terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPR7);
		terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPR8);
		terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPR9);
		terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPR10);
		if (VolPorteV1.Space != null)
		{
			jeuxSpace.Remove(VolPorteV1);
		}
		if (VolPorteV2.Space != null)
		{
			jeuxSpace.Remove(VolPorteV2);
		}
		if (VolPorteV3.Space != null)
		{
			jeuxSpace.Remove(VolPorteV3);
		}
		if (VolPorteV4.Space != null)
		{
			jeuxSpace.Remove(VolPorteV4);
		}
		if (VolPorteV5.Space != null)
		{
			jeuxSpace.Remove(VolPorteV5);
		}
		if (VolPorteV6.Space != null)
		{
			jeuxSpace.Remove(VolPorteV6);
		}
		if (VolPorteV7.Space != null)
		{
			jeuxSpace.Remove(VolPorteV7);
		}
		if (VolPorteV8.Space != null)
		{
			jeuxSpace.Remove(VolPorteV8);
		}
		if (VolPorteV9.Space != null)
		{
			jeuxSpace.Remove(VolPorteV9);
		}
		if (VolPorteV10.Space != null)
		{
			jeuxSpace.Remove(VolPorteV10);
		}
	}

	private void Toucher1(DetectorVolume volume, Entity toucher)
	{
		checked
		{
			if (!finAficheR && !finAficheT)
			{
				A++;
			}
			PorteS();
			if (timecounterS >= 10)
			{
				timecounterS -= 10;
			}
			else
			{
				timecounterS = 0;
			}
			if (A > ReA && totalC <= totalCHi)
			{
				timecounter1S = timecounterS;
				timecounter1M = timecounterM;
			}
			terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPV1);
			if (VolPorteV1.Space != null)
			{
				jeuxSpace.Remove(VolPorteV1);
			}
			terjeux.sceneInterfaceScene.ObjectManager.Submit(objVolPR1);
		}
	}

	private void Toucher2(DetectorVolume volume, Entity toucher)
	{
		checked
		{
			if (!finAficheR && !finAficheT)
			{
				A++;
			}
			PorteS();
			if (timecounterS > 10)
			{
				timecounterS -= 10;
			}
			else
			{
				timecounterS = 0;
			}
			if (A > ReA && totalC <= totalCHi)
			{
				timecounter1S = timecounterS;
				timecounter1M = timecounterM;
			}
			terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPV2);
			if (VolPorteV2.Space != null)
			{
				jeuxSpace.Remove(VolPorteV2);
			}
			terjeux.sceneInterfaceScene.ObjectManager.Submit(objVolPR2);
		}
	}

	private void Toucher3(DetectorVolume volume, Entity toucher)
	{
		checked
		{
			if (!finAficheR && !finAficheT)
			{
				A++;
			}
			PorteS();
			if (timecounterS > 10)
			{
				timecounterS -= 10;
			}
			else
			{
				timecounterS = 0;
			}
			if (A > ReA && totalC <= totalCHi)
			{
				timecounter1S = timecounterS;
				timecounter1M = timecounterM;
			}
			terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPV3);
			if (VolPorteV3.Space != null)
			{
				jeuxSpace.Remove(VolPorteV3);
			}
			terjeux.sceneInterfaceScene.ObjectManager.Submit(objVolPR3);
		}
	}

	private void Toucher4(DetectorVolume volume, Entity toucher)
	{
		checked
		{
			if (!finAficheR && !finAficheT)
			{
				A++;
			}
			PorteS();
			if (timecounterS > 10)
			{
				timecounterS -= 10;
			}
			else
			{
				timecounterS = 0;
			}
			if (A > ReA && totalC <= totalCHi)
			{
				timecounter1S = timecounterS;
				timecounter1M = timecounterM;
			}
			terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPV4);
			if (VolPorteV4.Space != null)
			{
				jeuxSpace.Remove(VolPorteV4);
			}
			terjeux.sceneInterfaceScene.ObjectManager.Submit(objVolPR4);
		}
	}

	private void Toucher5(DetectorVolume volume, Entity toucher)
	{
		checked
		{
			if (!finAficheR && !finAficheT)
			{
				A++;
			}
			PorteS();
			if (timecounterS > 10)
			{
				timecounterS -= 10;
			}
			else
			{
				timecounterS = 0;
			}
			if (A > ReA && totalC <= totalCHi)
			{
				timecounter1S = timecounterS;
				timecounter1M = timecounterM;
			}
			terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPV5);
			if (VolPorteV5.Space != null)
			{
				jeuxSpace.Remove(VolPorteV5);
			}
			terjeux.sceneInterfaceScene.ObjectManager.Submit(objVolPR5);
		}
	}

	private void Toucher6(DetectorVolume volume, Entity toucher)
	{
		checked
		{
			if (!finAficheR && !finAficheT)
			{
				A++;
			}
			PorteS();
			if (timecounterS > 10)
			{
				timecounterS -= 10;
			}
			else
			{
				timecounterS = 0;
			}
			if (A > ReA && totalC <= totalCHi)
			{
				timecounter1S = timecounterS;
				timecounter1M = timecounterM;
			}
			terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPV6);
			if (VolPorteV6.Space != null)
			{
				jeuxSpace.Remove(VolPorteV6);
			}
			terjeux.sceneInterfaceScene.ObjectManager.Submit(objVolPR6);
		}
	}

	private void Toucher7(DetectorVolume volume, Entity toucher)
	{
		checked
		{
			if (!finAficheR && !finAficheT)
			{
				A++;
			}
			PorteS();
			if (timecounterS > 10)
			{
				timecounterS -= 10;
			}
			else
			{
				timecounterS = 0;
			}
			if (A > ReA && totalC <= totalCHi)
			{
				timecounter1S = timecounterS;
				timecounter1M = timecounterM;
			}
			terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPV7);
			if (VolPorteV7.Space != null)
			{
				jeuxSpace.Remove(VolPorteV7);
			}
			terjeux.sceneInterfaceScene.ObjectManager.Submit(objVolPR7);
		}
	}

	private void Toucher8(DetectorVolume volume, Entity toucher)
	{
		checked
		{
			if (!finAficheR && !finAficheT)
			{
				A++;
			}
			PorteS();
			if (timecounterS > 10)
			{
				timecounterS -= 10;
			}
			else
			{
				timecounterS = 0;
			}
			if (A > ReA && totalC <= totalCHi)
			{
				timecounter1S = timecounterS;
				timecounter1M = timecounterM;
			}
			terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPV8);
			if (VolPorteV8.Space != null)
			{
				jeuxSpace.Remove(VolPorteV8);
			}
			terjeux.sceneInterfaceScene.ObjectManager.Submit(objVolPR8);
		}
	}

	private void Toucher9(DetectorVolume volume, Entity toucher)
	{
		checked
		{
			if (!finAficheR && !finAficheT)
			{
				A++;
			}
			PorteS();
			if (timecounterS > 10)
			{
				timecounterS -= 10;
			}
			else
			{
				timecounterS = 0;
			}
			if (A > ReA && totalC <= totalCHi)
			{
				timecounter1S = timecounterS;
				timecounter1M = timecounterM;
			}
			terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPV9);
			if (VolPorteV9.Space != null)
			{
				jeuxSpace.Remove(VolPorteV9);
			}
			terjeux.sceneInterfaceScene.ObjectManager.Submit(objVolPR9);
		}
	}

	private void Toucher10(DetectorVolume volume, Entity toucher)
	{
		checked
		{
			if (!finAficheR && !finAficheT)
			{
				A++;
			}
			PorteS();
			if (timecounterS > 10)
			{
				timecounterS -= 10;
			}
			else
			{
				timecounterS = 0;
			}
			if (A > ReA && totalC <= totalCHi)
			{
				timecounter1S = timecounterS;
				timecounter1M = timecounterM;
			}
			terjeux.sceneInterfaceScene.ObjectManager.Remove(objVolPV10);
			if (VolPorteV10.Space != null)
			{
				jeuxSpace.Remove(VolPorteV10);
			}
			terjeux.sceneInterfaceScene.ObjectManager.Submit(objVolPR10);
		}
	}

	public void PorteS()
	{
		portes = soundBankb.GetCue("portes");
		portes.Apply3D(listenerp, emitterp);
		portes.Play();
	}

	public void Draw(CustomPhysicsGame game, GameTime gameTime)
	{
		checked
		{
			if (!finAficheR && !finAficheT && !game.activeSR)
			{
				timerS += (float)gameTime.ElapsedGameTime.TotalSeconds;
				timecounterS += (int)timerS;
				if (timerS >= 1f)
				{
					timerS = 0f;
				}
				if (timecounterS >= 60)
				{
					timecounterS = 0;
					timecounterM++;
				}
			}
			if (A >= ReA)
			{
				ReA = A;
			}
			if (A >= 10)
			{
				finAficheR = true;
			}
			if (timecounterM >= 15)
			{
				finAficheT = true;
			}
			if (A >= ReA)
			{
				couleurG = true;
			}
			else
			{
				couleurG = false;
			}
			float num = timecounterM * 60;
			float num2 = timecounter1M * 60;
			float num3 = num + (float)timecounterS;
			float num4 = num2 + (float)timecounter1S;
			if (num3 <= num4)
			{
				couleurT = true;
			}
			else
			{
				couleurT = false;
			}
		}
	}
}
