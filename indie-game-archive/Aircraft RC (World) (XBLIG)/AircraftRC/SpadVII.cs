using System;
using System.Collections.Generic;
using BEPUphysics;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.Vehicle;
using DPSF;
using DPSF.ParticleSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SynapseGaming.LightingSystem.Rendering;

namespace AircraftRC;

public class SpadVII
{
	private SmokeParticleSystem particules;

	private ExplosionFireSmokeParticleSystem explosion;

	private ParticleSystemManager SmokePSmanager = new ParticleSystemManager();

	public Cue moteur;

	private Cue crash;

	private Cue REST;

	private SoundBank soundBanka;

	private AudioEmitter emitteravion = new AudioEmitter();

	private AudioListener listeneravion = new AudioListener();

	private SceneObject objtrain;

	private SceneObject objfuselage;

	private SceneObject objailesH;

	private SceneObject objailesBG;

	private SceneObject objailesBD;

	private SceneObject objderive;

	private SceneObject objderiveMob;

	private SceneObject objprofondeur;

	private SceneObject objprofondeurMob;

	private SceneObject objhelice;

	private SceneObject objcables;

	private SceneObject objroue;

	private SceneObject objroue1;

	private SceneObject objroueA;

	private SceneObject objaileronG;

	private SceneObject objaileronD;

	private SceneObject objhelice2;

	private List<CompoundShapeEntry> avion = new List<CompoundShapeEntry>();

	private Entity<CompoundCollidable> AvionNeuf;

	private ConvexHullShape trainshape;

	private ConvexHullShape fuselageshape;

	private ConvexHullShape ailesHshape;

	private ConvexHullShape ailesBGshape;

	private ConvexHullShape ailesBDshape;

	private ConvexHullShape deriveshape;

	private ConvexHullShape profondeurshape;

	private ConvexHullShape heliceshape;

	private ConvexHullShape cablesshape;

	private ConvexHull fuselage;

	private ConvexHull aileH;

	private ConvexHull aileBG;

	private ConvexHull aileBD;

	private ConvexHull train;

	private ConvexHull derive;

	private ConvexHull profondeur;

	private ConvexHull helice;

	private ConvexHull cables;

	public Vehicle Vehicle;

	private Entity sphere;

	private Entity pot;

	private ManetteConfig inputStateConfig;

	private Space owningSpace;

	private bool CC = true;

	public bool Avioncasse;

	public bool Avionloin;

	public bool AvionloinA;

	public float tempcrash;

	public float temploin = 1f;

	public float MDboutton = 636f;

	public float BouttonMaxi = 514f;

	public float BouttonMini = 603f;

	public float JaugeBoutton = 128f;

	private float pitch = -12f;

	private float pitchMaxi = 12f;

	private float pitchMini = -12f;

	public float VitesseAvion;

	public float VitesseAV;

	public float vitesseacceleration;

	private float vitesseMaxi = 12000f;

	private float vitesseMini;

	private float vitesseMaxiA = 65f;

	private float accelerationDE;

	private float acceleration;

	public float Angle;

	public float Angle3;

	public float Angle2;

	public float AngleR;

	public float AngleU;

	private float deriveRot;

	private float profondeurRot;

	private float aileronGRot;

	private float aileronDRot;

	private float helicerotation;

	public float altitude;

	private float sensibilite = 680f;

	private float moteurA;

	private float bouttonA;

	private float AccAPP = 60f;

	private float AccM = 0.4f;

	private float AccB = 1.5f;

	private float RAcc;

	private float variA;

	public int compteCrash;

	public float QuaFuel = 18000f;

	public float JaugeFuel = 383f;

	public float tt;

	public float rot = -400f;

	private Vector3 Monte;

	private Vector3 DroiteS;

	private Vector3 Droite;

	private Vector3 DroiteP;

	private Model trainModel;

	private Model fuselageModel;

	private Model ailesHModel;

	private Model ailesBDModel;

	private Model ailesBGModel;

	private Model deriveModel;

	private Model profondeurModel;

	private Model heliceModel;

	private Model cablesModel;

	private Model wheelModel;

	private Model roueAModel;

	private Model trainModelh;

	private Model fuselageModelh;

	private Model ailesHModelh;

	private Model ailesBDModelh;

	private Model ailesBGModelh;

	private Model deriveModelh;

	private Model profondeurModelh;

	private Model heliceModelh;

	private Model cablesModelh;

	private Cylinder roue1;

	private Cylinder roue2;

	private Cylinder roueA;

	private Matrix OffsetTransformtrain = Matrix.Identity;

	private Matrix OffsetTransformfuselage = Matrix.Identity;

	private Matrix OffsetTransformaileH = Matrix.Identity;

	private Matrix OffsetTransformaileBG = Matrix.Identity;

	private Matrix OffsetTransformaileBD = Matrix.Identity;

	private Matrix OffsetTransformderive = Matrix.Identity;

	private Matrix OffsetTransformprofondeur = Matrix.Identity;

	private Matrix OffsetTransformhelice = Matrix.Identity;

	private Matrix OffsetTransformcables = Matrix.Identity;

	private Matrix Positionderivemob = Matrix.CreateTranslation(-0.055f, 0.69f, 4.8f);

	private Matrix Positionprofondeurmob = Matrix.CreateTranslation(-0.072f, 0.475f, 4.576f);

	private Matrix PositionaileronGmob = Matrix.CreateTranslation(3.345f, 0.986f, 0.45f);

	private Matrix PositionaileronDmob = Matrix.CreateTranslation(-3.469f, 1.005f, 0.47f);

	private Matrix Positionhelice = Matrix.CreateTranslation(-0.066f, 0.09f, 0f);

	private Matrix Positionhelice2 = Matrix.CreateTranslation(-0.066f, 0.09f, -1.5f);

	private Matrix Positionpot = Matrix.CreateTranslation(-0.6f, 0.4f, 0.1f);

	public SpadVII(CustomPhysicsGame game)
	{
		particules = new SmokeParticleSystem(game);
		explosion = new ExplosionFireSmokeParticleSystem(game);
		inputStateConfig = new ManetteConfig(game);
	}

	public void Load(CustomPhysicsGame game)
	{
		trainModelh = game.Content.Load<Model>("Models/spad/trainh");
		fuselageModelh = game.Content.Load<Model>("Models/spad/fuselageh");
		ailesHModelh = game.Content.Load<Model>("Models/spad/aileHh");
		ailesBGModelh = game.Content.Load<Model>("Models/spad/aileBGh");
		ailesBDModelh = game.Content.Load<Model>("Models/spad/aileBDh");
		deriveModelh = game.Content.Load<Model>("Models/spad/deriveh");
		profondeurModelh = game.Content.Load<Model>("Models/spad/profondeurh");
		heliceModelh = game.Content.Load<Model>("Models/spad/heliceh");
		cablesModelh = game.Content.Load<Model>("Models/spad/cablesh");
		Model model = game.Content.Load<Model>("Models/spad/helice2");
		Model model2 = game.Content.Load<Model>("Models/spad/avionpieces");
		trainModel = game.Content.Load<Model>("Models/spad/train");
		fuselageModel = game.Content.Load<Model>("Models/spad/fuselage");
		ailesHModel = game.Content.Load<Model>("Models/spad/aileH");
		ailesBGModel = game.Content.Load<Model>("Models/spad/aileBG");
		ailesBDModel = game.Content.Load<Model>("Models/spad/aileBD");
		deriveModel = game.Content.Load<Model>("Models/spad/derive");
		profondeurModel = game.Content.Load<Model>("Models/spad/profondeur");
		heliceModel = game.Content.Load<Model>("Models/spad/helice");
		cablesModel = game.Content.Load<Model>("Models/spad/cables");
		wheelModel = game.Content.Load<Model>("Models/spad/roues");
		roueAModel = game.Content.Load<Model>("Models/spad/rouesA");
		ModelMesh mesh = model2.Meshes["derive"];
		ModelMesh mesh2 = model2.Meshes["ProfondeurAR"];
		ModelMesh mesh3 = model2.Meshes["AileronD0"];
		ModelMesh mesh4 = model2.Meshes["AileronG0"];
		objtrain = new SceneObject(trainModel);
		objfuselage = new SceneObject(fuselageModel);
		objailesH = new SceneObject(ailesHModel);
		objailesBD = new SceneObject(ailesBDModel);
		objailesBG = new SceneObject(ailesBGModel);
		objderive = new SceneObject(deriveModel);
		objderiveMob = new SceneObject(mesh);
		objprofondeur = new SceneObject(profondeurModel);
		objprofondeurMob = new SceneObject(mesh2);
		objhelice = new SceneObject(heliceModel);
		objcables = new SceneObject(cablesModel);
		objroue = new SceneObject(wheelModel);
		objroue1 = new SceneObject(wheelModel);
		objroueA = new SceneObject(roueAModel);
		objaileronG = new SceneObject(mesh3);
		objaileronD = new SceneObject(mesh4);
		objhelice2 = new SceneObject(model);
		TriangleMesh.GetVerticesAndIndicesFromModel(trainModelh, out var vertices, out var indices);
		trainshape = new ConvexHullShape(vertices, out var center);
		train = new ConvexHull(vertices, 50f);
		TriangleMesh.GetVerticesAndIndicesFromModel(fuselageModelh, out vertices, out indices);
		fuselageshape = new ConvexHullShape(vertices, out center);
		fuselage = new ConvexHull(vertices, 200f);
		TriangleMesh.GetVerticesAndIndicesFromModel(ailesHModelh, out vertices, out indices);
		ailesHshape = new ConvexHullShape(vertices, out center);
		aileH = new ConvexHull(vertices, 100f);
		TriangleMesh.GetVerticesAndIndicesFromModel(ailesBGModelh, out vertices, out indices);
		ailesBGshape = new ConvexHullShape(vertices, out center);
		aileBG = new ConvexHull(vertices, 50f);
		TriangleMesh.GetVerticesAndIndicesFromModel(ailesBDModelh, out vertices, out indices);
		ailesBDshape = new ConvexHullShape(vertices, out center);
		aileBD = new ConvexHull(vertices, 50f);
		TriangleMesh.GetVerticesAndIndicesFromModel(deriveModelh, out vertices, out indices);
		deriveshape = new ConvexHullShape(vertices, out center);
		derive = new ConvexHull(vertices, 30f);
		TriangleMesh.GetVerticesAndIndicesFromModel(profondeurModelh, out vertices, out indices);
		profondeurshape = new ConvexHullShape(vertices, out center);
		profondeur = new ConvexHull(vertices, 30f);
		TriangleMesh.GetVerticesAndIndicesFromModel(heliceModelh, out vertices, out indices);
		heliceshape = new ConvexHullShape(vertices, out center);
		helice = new ConvexHull(vertices, 30f);
		TriangleMesh.GetVerticesAndIndicesFromModel(cablesModelh, out vertices, out indices);
		cablesshape = new ConvexHullShape(vertices, out center);
		cables = new ConvexHull(vertices, 10f);
		roue1 = new Cylinder(new Vector3(0f, 0f, 0f), 0.2f, 0.6f, 5f);
		roue2 = new Cylinder(new Vector3(0f, 0f, 0f), 0.2f, 0.6f, 5f);
		roueA = new Cylinder(new Vector3(0f, 0f, 0f), 0.1f, 0.08f, 3f);
		avion.Add(new CompoundShapeEntry(fuselageshape, new Vector3(0f, 0f, 0f), 150f));
		avion.Add(new CompoundShapeEntry(ailesHshape, new Vector3(-0.65f, 0.33f, -0.95f), 80f));
		avion.Add(new CompoundShapeEntry(ailesBGshape, new Vector3(-2.3f, -0.25f, -0.95f), 50f));
		avion.Add(new CompoundShapeEntry(ailesBDshape, new Vector3(2.9f, -0.28f, -0.97f), 50f));
		avion.Add(new CompoundShapeEntry(trainshape, new Vector3(0f, -1.1f, -1.4f), 50f));
		avion.Add(new CompoundShapeEntry(deriveshape, new Vector3(0.029f, 0.4f, 3.73f), 30f));
		avion.Add(new CompoundShapeEntry(profondeurshape, new Vector3(-0.12f, 0.217f, 3.53f), 30f));
		avion.Add(new CompoundShapeEntry(heliceshape, new Vector3(-0.02f, -0.2f, -2.4f), 30f));
		avion.Add(new CompoundShapeEntry(cablesshape, new Vector3(0.28f, 0.1f, -0.9f), 10f));
		AvionNeuf = new CompoundBody(avion, 500f);
		AvionNeuf.CollisionInformation.LocalPosition = new Vector3(-0.14f, 0.25f, 0.7f);
		AvionNeuf.Position = new Vector3(0f, 1.5f, 0f);
		AvionNeuf.CollisionInformation.Events.DetectingInitialCollision += HandleCollision;
		Vehicle = new Vehicle(AvionNeuf);
		owningSpace = game.space;
		owningSpace.Add(Vehicle);
		sphere = new Sphere(new Vector3(0f, 0f, 0f), 0.3f);
		pot = new Sphere(new Vector3(0f, 0f, 0f), 0.1f);
		particules.AutoInitialize(game.GraphicsDevice, game.Content, null);
		explosion.AutoInitialize(game.GraphicsDevice, game.Content, null);
		SmokePSmanager.AddParticleSystem(particules);
		SmokePSmanager.AddParticleSystem(explosion);
		Vehicle.Body.LinearVelocity = Vector3.Zero;
		Vehicle.Body.AngularVelocity = Vector3.Zero;
		Vehicle.Body.Orientation = new Quaternion(0.11f, 0f, 0f, 1f);
		Matrix localGraphicTransform = Matrix.CreateFromAxisAngle(Vector3.Forward, (float)Math.PI / 2f);
		Vehicle.AddWheel(new Wheel(new RaycastWheelShape(0.376f, localGraphicTransform), new WheelSuspension(30000f, 50f, Vector3.Down, 0.6f, new Vector3(-1f, -1.46f, -1.4f)), new WheelDrivingMotor(0.1f, 30000f, 0f), new WheelBrake(0f, 0f, 0.1f), new WheelSlidingFriction(1f, 2f)));
		Vehicle.AddWheel(new Wheel(new RaycastWheelShape(0.376f, localGraphicTransform), new WheelSuspension(30000f, 50f, Vector3.Down, 0.6f, new Vector3(1.1f, -1.46f, -1.4f)), new WheelDrivingMotor(0.1f, 30000f, 0f), new WheelBrake(0f, 0f, 0.1f), new WheelSlidingFriction(1f, 2f)));
		Vehicle.AddWheel(new Wheel(new RaycastWheelShape(0.3f, localGraphicTransform), new WheelSuspension(50000f, 100f, Vector3.Down, 0.8f, new Vector3(0.11f, -0.08f, 3.8f)), new WheelDrivingMotor(0.1f, 30000f, 0f), new WheelBrake(0f, 0f, 0.09f), new WheelSlidingFriction(1f, 1f)));
		foreach (Wheel wheel in Vehicle.Wheels)
		{
			wheel.Shape.FreezeWheelsWhileBraking = true;
			wheel.Suspension.SolverSettings.MaximumIterations = 1;
			wheel.Brake.SolverSettings.MaximumIterations = 1;
			wheel.SlidingFriction.SolverSettings.MaximumIterations = 1;
			wheel.DrivingMotor.SolverSettings.MaximumIterations = 0;
		}
		OffsetTransformtrain = Matrix.CreateTranslation(-train.Position);
		OffsetTransformfuselage = Matrix.CreateTranslation(-fuselage.Position);
		OffsetTransformaileH = Matrix.CreateTranslation(-aileH.Position);
		OffsetTransformaileBG = Matrix.CreateTranslation(-aileBG.Position);
		OffsetTransformaileBD = Matrix.CreateTranslation(-aileBD.Position);
		OffsetTransformderive = Matrix.CreateTranslation(-derive.Position);
		OffsetTransformprofondeur = Matrix.CreateTranslation(-profondeur.Position);
		OffsetTransformhelice = Matrix.CreateTranslation(-helice.Position);
		OffsetTransformcables = Matrix.CreateTranslation(-cables.Position);
		soundBanka = game.soundBank;
	}

	public void Pause()
	{
		moteur.Stop(AudioStopOptions.Immediate);
	}

	private void piecesVu(CustomPhysicsGame game)
	{
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objtrain);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objfuselage);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objailesH);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objailesBD);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objailesBG);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objderive);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objderiveMob);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objprofondeur);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objprofondeurMob);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objcables);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objroue);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objroue1);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objroueA);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objaileronG);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objaileronD);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objhelice2);
		if (Avioncasse)
		{
			game.terrain.sceneInterfaceScene.ObjectManager.Submit(objhelice);
		}
	}

	private void Transneuf()
	{
		objroue.World = Vehicle.Wheels[0].Shape.WorldTransform;
		objroue1.World = Vehicle.Wheels[1].Shape.WorldTransform;
		objroueA.World = Vehicle.Wheels[2].Shape.WorldTransform;
		objtrain.World = AvionNeuf.WorldTransform;
		objfuselage.World = AvionNeuf.WorldTransform;
		objailesH.World = AvionNeuf.WorldTransform;
		objailesBD.World = AvionNeuf.WorldTransform;
		objailesBG.World = AvionNeuf.WorldTransform;
		objderive.World = AvionNeuf.WorldTransform;
		objderiveMob.World = Matrix.CreateRotationY(MathHelper.ToRadians(deriveRot)) * Positionderivemob * AvionNeuf.WorldTransform;
		objprofondeur.World = AvionNeuf.WorldTransform;
		objprofondeurMob.World = Matrix.CreateRotationX(MathHelper.ToRadians(profondeurRot)) * Positionprofondeurmob * AvionNeuf.WorldTransform;
		objcables.World = AvionNeuf.WorldTransform;
		objaileronG.World = Matrix.CreateRotationX(MathHelper.ToRadians(aileronDRot)) * PositionaileronGmob * AvionNeuf.WorldTransform;
		objaileronD.World = Matrix.CreateRotationX(MathHelper.ToRadians(aileronGRot)) * PositionaileronDmob * AvionNeuf.WorldTransform;
		objhelice2.World = Matrix.CreateRotationZ(helicerotation) * Positionhelice * AvionNeuf.WorldTransform;
		helice.WorldTransform = Matrix.CreateRotationZ(rot) * Positionhelice2 * AvionNeuf.WorldTransform;
		objhelice.World = OffsetTransformhelice * helice.WorldTransform;
		pot.WorldTransform = Positionpot * AvionNeuf.WorldTransform;
		train.Position = AvionNeuf.Position;
		fuselage.Position = AvionNeuf.Position;
		aileH.Position = AvionNeuf.Position;
		aileBG.Position = AvionNeuf.Position;
		aileBD.Position = AvionNeuf.Position;
		derive.Position = AvionNeuf.Position;
		profondeur.Position = AvionNeuf.Position;
		cables.Position = AvionNeuf.Position;
		roue1.Position = AvionNeuf.Position;
		roue2.Position = AvionNeuf.Position;
		roueA.Position = AvionNeuf.Position;
		train.Orientation = AvionNeuf.Orientation;
		fuselage.Orientation = AvionNeuf.Orientation;
		aileH.Orientation = AvionNeuf.Orientation;
		aileBG.Orientation = AvionNeuf.Orientation;
		aileBD.Orientation = AvionNeuf.Orientation;
		derive.Orientation = AvionNeuf.Orientation;
		profondeur.Orientation = AvionNeuf.Orientation;
		cables.Orientation = AvionNeuf.Orientation;
		roue1.Orientation = AvionNeuf.Orientation;
		roue2.Orientation = AvionNeuf.Orientation;
		roueA.Orientation = AvionNeuf.Orientation;
	}

	private void Transcasse()
	{
		objroue.World = roue1.WorldTransform;
		objroue1.World = roue2.WorldTransform;
		objroueA.World = roueA.WorldTransform;
		objtrain.World = OffsetTransformtrain * train.WorldTransform;
		objfuselage.World = OffsetTransformfuselage * fuselage.WorldTransform;
		objailesH.World = OffsetTransformaileH * aileH.WorldTransform;
		objailesBG.World = OffsetTransformaileBG * aileBG.WorldTransform;
		objailesBD.World = OffsetTransformaileBD * aileBD.WorldTransform;
		objderive.World = OffsetTransformderive * derive.WorldTransform;
		objderiveMob.World = Positionderivemob * OffsetTransformderive * derive.WorldTransform;
		objprofondeur.World = OffsetTransformprofondeur * profondeur.WorldTransform;
		objprofondeurMob.World = Positionprofondeurmob * OffsetTransformprofondeur * profondeur.WorldTransform;
		objhelice.World = OffsetTransformhelice * helice.WorldTransform;
		objcables.World = OffsetTransformcables * cables.WorldTransform;
		objaileronG.World = PositionaileronGmob * OffsetTransformaileH * aileH.WorldTransform;
		objaileronD.World = PositionaileronDmob * OffsetTransformaileH * aileH.WorldTransform;
		objhelice2.World = Matrix.CreateTranslation(0f, -20f, 0f);
	}

	public void Moteur()
	{
		moteur = soundBanka.GetCue("spad7");
		moteur.SetVariable("accelerateur", pitch);
		moteur.Apply3D(listeneravion, emitteravion);
		moteur.Play();
	}

	public void Crash()
	{
		crash = soundBanka.GetCue("Crash");
		crash.Apply3D(listeneravion, emitteravion);
		crash.Play();
	}

	public void RestartSound()
	{
		REST = soundBanka.GetCue("restart");
		REST.Apply3D(listeneravion, emitteravion);
		REST.Play();
	}

	public void HandleCollision(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
	{
		if ((sender != null && VitesseAvion >= 22f) || accelerationDE <= -3f)
		{
			Collision1();
		}
	}

	public void Update(float dt, CustomPhysicsGame game, GameTime gameTime)
	{
		inputStateConfig.Update(game);
		game.space.Update(dt);
		game.camera.position = new Vector3(-33f, 15f, 12f);
		if (!Avionloin)
		{
			temploin = 1f;
		}
		if (!Avioncasse)
		{
			tempcrash = 0f;
			piecesVu(game);
			game.camera.Target = Vehicle.Body.Position;
			Transneuf();
			particules.Emitter.PositionData.Position = pot.Position;
			if (QuaFuel > 0f)
			{
				particules.Emitter.Enabled = true;
			}
		}
		if (Avioncasse)
		{
			tempcrash += 0.1f;
			Transcasse();
			game.camera.Target = fuselage.Position;
			explosion.Emitter.PositionData.Position = new Vector3(fuselage.Position.X, 0f, fuselage.Position.Z);
		}
		explosion.Emitter.PositionData.Position = new Vector3(fuselage.Position.X, 0f, fuselage.Position.Z);
		Vector3 vector = Vehicle.Body.LinearVelocity - Vehicle.Body.WorldTransform.Forward * Vector3.Dot(Vehicle.Body.WorldTransform.Forward, Vehicle.Body.LinearVelocity);
		Vector3 vector2 = Vehicle.Body.OrientationMatrix.Forward - vector;
		VitesseAvion = Vector3.Dot(Vehicle.Body.WorldTransform.Forward, Vehicle.Body.LinearVelocity);
		VitesseAV = Vector3.Dot(Vehicle.Body.WorldTransform.Down, Vehicle.Body.LinearVelocity);
		helicerotation += 1f + VitesseAvion;
		game.space.Update(dt);
		Random random = new Random();
		particules.vitesse = new Vector3(random.Next(-3, 3), random.Next(3, 3), random.Next(3, 3));
		particules.nombrepar = acceleration / 46000f;
		particules.Emitter.ParticlesPerSecond = acceleration / 100f;
		explosion.ExplosionColor = new Color(189, 145, 82);
		explosion.ExplosionParticleSize = 16;
		explosion.ExplosionIntensity = 1;
		explosion.vitesse = fuselage.LinearVelocity;
		Matrix identity = Matrix.Identity;
		SmokePSmanager.SetWorldViewProjectionMatricesForAllParticleSystems(identity, game.camera.View, game.camera.Projection);
		SmokePSmanager.SetCameraPositionForAllParticleSystems(game.camera.position);
		SmokePSmanager.UpdateAllParticleSystems((float)gameTime.ElapsedGameTime.TotalSeconds);
		altitude = Vehicle.Body.Position.Y;
		float x = Vehicle.Body.Position.X;
		float z = Vehicle.Body.Position.Z;
		float num = 5f;
		float num2 = 45f;
		if (altitude >= 300f || x >= 300f || z >= 300f || x <= -300f || z <= -300f)
		{
			game.camera.fov -= 0.1f;
		}
		else
		{
			game.camera.fov += 0.12f;
		}
		if (game.inputStateConfig.ZoomPlus)
		{
			game.camera.fov -= 0.3f;
		}
		if (game.inputStateConfig.ZoomMoins)
		{
			game.camera.fov += 0.3f;
		}
		if (game.camera.fov <= num)
		{
			game.camera.fov = num;
		}
		if (game.camera.fov >= num2)
		{
			game.camera.fov = num2;
		}
		if (altitude >= 2500f || x >= 2500f || z >= 2500f || x <= -2500f || (z <= -2500f && !Avioncasse))
		{
			AvionloinA = true;
		}
		else
		{
			AvionloinA = false;
		}
		if (altitude >= 2800f || x >= 2800f || z >= 2800f || x <= -2800f || (z <= -2800f && !Avioncasse))
		{
			Avionloin = true;
			temploin += 0.1f;
			if (temploin >= 30f && !Avioncasse)
			{
				Restart(game);
			}
		}
		Angle = (float)Math.Sin(Vector3.Dot(sphere.OrientationMatrix.Down, Vehicle.Body.OrientationMatrix.Left)) * ((float)Math.PI * 2f);
		Angle2 = (float)Math.Acos(Vector3.Dot(sphere.OrientationMatrix.Down, Vehicle.Body.OrientationMatrix.Forward)) * ((float)Math.PI * 2f);
		Angle3 = (float)Math.Atan(Vector3.Dot(sphere.WorldTransform.Up, Vehicle.Body.OrientationMatrix.Down)) * ((float)Math.PI * 2f);
		AngleR = (float)Math.Atan(Vector3.Dot(sphere.OrientationMatrix.Down, Vehicle.Body.OrientationMatrix.Left)) * ((float)Math.PI * 2f);
		AngleU = (float)Math.Acos(Vector3.Dot(sphere.OrientationMatrix.Down, Vehicle.Body.OrientationMatrix.Left)) * ((float)Math.PI * 2f);
		profondeurRot = 0f;
		Vehicle.Body.AngularDamping = 0.75f;
		float num3 = VitesseAvion * 10f;
		Vector3 right = Vehicle.Body.OrientationMatrix.Right;
		if (num3 >= 200f)
		{
			num3 = 200f;
		}
		if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M1 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M4 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M6)
		{
			Monte = -right * num3 * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.Y;
			profondeurRot += 30f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.Y;
			Vehicle.Body.ApplyAngularImpulse(ref Monte);
		}
		if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M2 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M3 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M5)
		{
			Monte = -right * num3 * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.Y;
			profondeurRot += 30f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.Y;
			Vehicle.Body.ApplyAngularImpulse(ref Monte);
		}
		deriveRot = 0f;
		float num4 = VitesseAvion * 10f;
		if (num4 >= 190f)
		{
			num4 = 190f;
		}
		Vector3 up = Vehicle.Body.OrientationMatrix.Up;
		if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M1 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M2 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M6)
		{
			DroiteS = -up * num4 * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
			deriveRot += 28f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
			if (altitude <= 1.7f)
			{
				DroiteS = -up * VitesseAvion * 40f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
				if (VitesseAvion <= 6f && VitesseAvion >= 0.1f)
				{
					DroiteS = -up * 330f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
				}
			}
			Vehicle.Body.ApplyAngularImpulse(ref DroiteS);
		}
		if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M3 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M4 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M5)
		{
			DroiteS = -up * num4 * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.X;
			deriveRot += 28f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.X;
			if (altitude <= 1.8f && Angle2 >= 10.9f)
			{
				DroiteS = -up * VitesseAvion * 40f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.X;
				if (VitesseAvion <= 6f && VitesseAvion >= 0.1f)
				{
					DroiteS = -up * 330f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.X;
				}
			}
			Vehicle.Body.ApplyAngularImpulse(ref DroiteS);
		}
		aileronGRot = 0f;
		aileronDRot = 0f;
		float num5 = VitesseAvion * 10f;
		if (num5 >= 320f)
		{
			num5 = 320f;
			Vehicle.Body.LinearDamping = Angle2 / 75f;
		}
		Vector3 forward = Vehicle.Body.OrientationMatrix.Forward;
		if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M1 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M2 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M6)
		{
			Droite = forward * num5 * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.X;
			aileronGRot += 28f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.X;
			aileronDRot -= 28f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.X;
			Vehicle.Body.ApplyAngularImpulse(ref Droite);
		}
		if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M3 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M4 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M5)
		{
			Droite = forward * num5 * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
			aileronGRot += 28f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
			aileronDRot -= 28f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
			Vehicle.Body.ApplyAngularImpulse(ref Droite);
		}
		float num6 = AngleR * 20f;
		if (num6 >= 35f)
		{
			num6 = 35f;
		}
		if (num6 <= -35f)
		{
			num6 = -35f;
		}
		DroiteP = up * num6;
		Vehicle.Body.ApplyAngularImpulse(ref DroiteP);
		pot.Position = Vehicle.Body.Position;
		sphere.Position = Vehicle.Body.Position;
		Vector3 impulse = Monte / 4.5f;
		if (VitesseAvion >= 22f && AngleU >= 5f && AngleU <= 14f && altitude >= 5.75f && Angle2 <= 8f)
		{
			Vehicle.Body.ApplyAngularImpulse(ref impulse);
		}
		Vector3 impulse2 = -right * 100f;
		if (VitesseAvion >= 5f && VitesseAvion <= 23f)
		{
			Vector3 impulse3 = new Vector3(0f, -150f, 0f);
			Vehicle.Body.ApplyLinearImpulse(ref impulse3);
			if (altitude <= 2f && Angle2 >= 10f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse2);
			}
		}
		Vector3 impulse4 = right * 150f;
		if (VitesseAvion <= 2f && altitude <= 2f)
		{
			Vehicle.Body.ApplyAngularImpulse(ref impulse4);
		}
		Vector3 impulse5 = -right * 2800f / Angle2;
		Vector3 impulse6 = right * 2200f / Angle2;
		Vector3 impulse7 = -up * 3000f / Angle2;
		Vector3 impulse8 = up * 3000f / Angle2;
		Vector3 impulse9 = -up * 1000f / Angle2;
		Vector3 impulse10 = up * 1000f / Angle2;
		Vector3 impulse11 = -right * 60f / Angle2;
		Vector3 impulse12 = right * 900f / Angle2;
		if (Angle3 >= 0f && Angle2 >= 8.5f && AngleU >= 4f && AngleU <= 15f)
		{
			Vehicle.Body.ApplyAngularImpulse(ref impulse12);
		}
		if (Angle2 >= 10f && AngleU >= 4f && AngleU <= 15f)
		{
			Vehicle.Body.ApplyAngularImpulse(ref impulse11);
		}
		if (VitesseAvion <= 21f && altitude >= 2f)
		{
			if (Angle3 <= 0f && Angle2 >= 11f && AngleU >= 4f && AngleU <= 15f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse5);
			}
			if (Angle3 >= 0f && Angle2 >= 11f && AngleU >= 4f && AngleU <= 15f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse6);
			}
			if (Angle <= -2.5f && Angle2 >= 11f && VitesseAvion <= 2f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse7);
			}
			if (Angle >= 2.5f && Angle2 >= 11f && VitesseAvion <= 2f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse8);
			}
			if (Angle3 <= 0f && AngleU >= 5f && AngleU <= 16f && Angle2 <= 11f && Angle2 >= 5f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse5);
			}
			if (Angle3 >= 0f && AngleU >= 5f && AngleU <= 16f && Angle2 <= 11f && Angle2 >= 5f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse6);
			}
			if (AngleU <= 5f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse8);
			}
			if (AngleU >= 16f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse7);
			}
		}
		if (VitesseAvion <= 30f && altitude >= 2f)
		{
			if (AngleU <= 5f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse10);
			}
			if (AngleU >= 16f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse9);
			}
		}
		vitesseacceleration = accelerationDE + acceleration * dt;
		Vector3 impulse13 = ((!(VitesseAvion <= 10f)) ? (vitesseacceleration * vector2) : (vitesseacceleration * vector2 + vector));
		if (vitesseacceleration <= vitesseMini)
		{
			impulse13 = VitesseAvion * vector2;
		}
		if (VitesseAvion >= vitesseMaxiA)
		{
			impulse13 = 0f * vector2;
		}
		if (QuaFuel <= 0f)
		{
			particules.Emitter.Enabled = false;
			impulse13 = VitesseAvion * vector2;
			moteur.Stop(AudioStopOptions.Immediate);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objhelice2);
			game.terrain.sceneInterfaceScene.ObjectManager.Submit(objhelice);
		}
		Vehicle.Body.ApplyLinearImpulse(ref impulse13);
		if (compteCrash >= 99)
		{
			compteCrash = 99;
		}
		if (Avioncasse)
		{
			game.terrain.sceneInterfaceScene.ObjectManager.Submit(objhelice);
		}
		if (game.gamemode == CustomPhysicsGame.GameMode.M0)
		{
			CC = true;
		}
		if (game.gamemode == CustomPhysicsGame.GameMode.M1)
		{
			CC = false;
		}
		if (!game.terrain.FuelZActi && !Avioncasse && game.gamemode == CustomPhysicsGame.GameMode.M0)
		{
			QuaFuel--;
			JaugeFuel -= 0.007111111f;
		}
		if (JaugeFuel <= 255f)
		{
			JaugeFuel = 255f;
		}
		if (QuaFuel <= 0f)
		{
			rot += 1f * tt;
			QuaFuel = 0f;
			tt++;
		}
		if (QuaFuel >= 1f)
		{
			tt = 0f;
			rot = -400f;
		}
		if (game.terrain.FuelZActi && !Avioncasse && game.gamemode == CustomPhysicsGame.GameMode.M0)
		{
			QuaFuel += 28f;
			JaugeFuel += 0.1991111f;
		}
		if (JaugeFuel >= 383f)
		{
			JaugeFuel = 383f;
		}
		if (QuaFuel >= 18000f)
		{
			QuaFuel = 18000f;
		}
		if (rot >= 0f)
		{
			rot = 0f;
		}
		if (rot <= -400f)
		{
			rot = -400f;
		}
		if (tt >= 150f)
		{
			tt = 150f;
		}
		Vehicle.Body.LinearDamping = Angle2 / 85f;
		if (VitesseAvion < 15f)
		{
			Vehicle.Body.LinearDamping = 0f;
		}
		RAcc = vitesseMaxi / AccAPP;
		variA = RAcc / sensibilite;
		moteurA = AccM / variA;
		bouttonA = AccB / variA;
		if ((game.manetteChoix == CustomPhysicsGame.ManetteChoix.M5 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M6) && inputStateConfig.Acceleration)
		{
			acceleration += 75f;
			moteur.SetVariable("accelerateur", pitch += 0.16363f);
			MDboutton -= 0.61364f;
			if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M1 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M4)
			{
				acceleration -= sensibilite * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.Y;
				moteur.SetVariable("accelerateur", pitch -= moteurA * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.Y);
				MDboutton += bouttonA * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.Y;
			}
			if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M2 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M3)
			{
				acceleration -= sensibilite * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.Y;
				moteur.SetVariable("accelerateur", pitch -= moteurA * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.Y);
				MDboutton += bouttonA * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.Y;
			}
		}
		if ((game.manetteChoix == CustomPhysicsGame.ManetteChoix.M5 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M6) && inputStateConfig.Deceleration)
		{
			acceleration -= 75f;
			moteur.SetVariable("accelerateur", pitch -= 0.16363f);
			MDboutton += 0.61364f;
		}
		if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M1 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M4)
		{
			acceleration += sensibilite * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.Y;
			moteur.SetVariable("accelerateur", pitch += moteurA * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.Y);
			MDboutton -= bouttonA * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.Y;
		}
		if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M2 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M3)
		{
			acceleration += sensibilite * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.Y;
			moteur.SetVariable("accelerateur", pitch += moteurA * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.Y);
			MDboutton -= bouttonA * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.Y;
		}
		if (VitesseAvion <= 20f && altitude >= 3f)
		{
			accelerationDE -= 0.4f;
		}
		else
		{
			accelerationDE += 0.1f;
		}
		if (Angle2 >= 16f)
		{
			accelerationDE -= 0.3f;
		}
		else
		{
			accelerationDE += 0.1f;
		}
		if (AngleU <= 4f || AngleU >= 15f)
		{
			accelerationDE -= 0.3f;
		}
		else
		{
			accelerationDE += 0.1f;
		}
		if (accelerationDE <= -700f)
		{
			accelerationDE = -700f;
		}
		if (accelerationDE >= 0f)
		{
			accelerationDE = 0f;
		}
		if (acceleration >= vitesseMaxi)
		{
			acceleration = vitesseMaxi;
		}
		if (acceleration <= vitesseMini)
		{
			acceleration = vitesseMini;
		}
		JaugeBoutton = VitesseAvion + 128f;
		if (MDboutton <= BouttonMaxi)
		{
			MDboutton = BouttonMaxi;
		}
		if (MDboutton >= BouttonMini)
		{
			MDboutton = BouttonMini;
		}
		if (pitch >= pitchMaxi)
		{
			pitch = pitchMaxi;
		}
		if (pitch <= pitchMini)
		{
			pitch = pitchMini;
		}
		emitteravion.Position = Vehicle.Body.Position;
		moteur.Apply3D(listeneravion, emitteravion);
		game.audioEngine.Update();
		if (tempcrash > 0.1f && tempcrash <= 3f)
		{
			GamePad.SetVibration(game.menu.player, 0.5f, 0.7f);
		}
		if (tempcrash >= 3.1f)
		{
			GamePad.SetVibration(game.menu.player, 0f, 0f);
		}
		checked
		{
			if (tempcrash >= 30f)
			{
				owningSpace.Remove(train);
				owningSpace.Remove(fuselage);
				owningSpace.Remove(aileH);
				owningSpace.Remove(aileBG);
				owningSpace.Remove(aileBD);
				owningSpace.Remove(derive);
				owningSpace.Remove(profondeur);
				owningSpace.Remove(helice);
				owningSpace.Remove(cables);
				owningSpace.Remove(roue1);
				owningSpace.Remove(roue2);
				owningSpace.Remove(roueA);
				owningSpace.Add(Vehicle);
				Restart(game);
				if (!game.jeux.finAficheR && !game.jeux.finAficheT)
				{
					game.jeux.timecounterS += 5;
				}
			}
			if (!Avioncasse && inputStateConfig.Ypress)
			{
				moteur.Stop(AudioStopOptions.Immediate);
				Restart(game);
				if (!game.jeux.finAficheR && !game.jeux.finAficheT)
				{
					game.jeux.timecounterS += 10;
				}
			}
		}
	}

	public void Draw()
	{
		SmokePSmanager.DrawAllParticleSystems();
	}

	public void Remove(CustomPhysicsGame game)
	{
		if (!Avioncasse)
		{
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objtrain);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objfuselage);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objailesH);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objailesBD);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objailesBG);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objderive);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objderiveMob);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objprofondeur);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objprofondeurMob);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objcables);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objroue);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objroue1);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objroueA);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objaileronG);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objaileronD);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objhelice2);
			if (objhelice != null)
			{
				game.terrain.sceneInterfaceScene.ObjectManager.Remove(objhelice);
			}
			owningSpace.Remove(Vehicle);
		}
		if (Avioncasse)
		{
			owningSpace.Remove(train);
			owningSpace.Remove(fuselage);
			owningSpace.Remove(aileH);
			owningSpace.Remove(aileBG);
			owningSpace.Remove(aileBD);
			owningSpace.Remove(derive);
			owningSpace.Remove(profondeur);
			owningSpace.Remove(helice);
			owningSpace.Remove(cables);
			owningSpace.Remove(roue1);
			owningSpace.Remove(roue2);
			owningSpace.Remove(roueA);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objhelice);
		}
	}

	public void Restart(CustomPhysicsGame game)
	{
		if (objhelice != null)
		{
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objhelice);
		}
		if (objhelice == null)
		{
			game.terrain.sceneInterfaceScene.ObjectManager.Submit(objhelice2);
		}
		if (QuaFuel <= 0f)
		{
			QuaFuel = 15000f;
			JaugeFuel = 383f;
		}
		RestartSound();
		Avioncasse = false;
		Avionloin = false;
		acceleration = 0f;
		accelerationDE = 0f;
		vitesseacceleration = 0f;
		VitesseAvion = 0f;
		MDboutton = 636f;
		JaugeBoutton = 128f;
		pitch = -12f;
		game.camera.fov = 45f;
		Vehicle.Body.LinearVelocity = Vector3.Zero;
		Vehicle.Body.AngularVelocity = Vector3.Zero;
		Vehicle.Body.Orientation = new Quaternion(0.11f, 0f, 0f, 1f);
		AvionNeuf.Position = new Vector3(0f, 1.5f, 0f);
		particules.Emitter.Enabled = true;
		Moteur();
	}

	private void Collision1()
	{
		QuaFuel = 18000f;
		JaugeFuel = 383f;
		Avioncasse = true;
		checked
		{
			if (CC)
			{
				compteCrash++;
			}
			particules.Emitter.Enabled = false;
			explosion.Emitter.Enabled = true;
			if (altitude <= 8f)
			{
				explosion.Explode();
			}
			owningSpace.Remove(Vehicle);
			owningSpace.Add(train);
			owningSpace.Add(fuselage);
			owningSpace.Add(aileH);
			owningSpace.Add(aileBG);
			owningSpace.Add(aileBD);
			owningSpace.Add(derive);
			owningSpace.Add(profondeur);
			owningSpace.Add(helice);
			owningSpace.Add(cables);
			owningSpace.Add(roue1);
			owningSpace.Add(roue2);
			owningSpace.Add(roueA);
			train.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			train.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			fuselage.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			fuselage.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			aileH.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			aileH.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			aileBG.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			aileBG.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			aileBD.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			aileBD.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			derive.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			derive.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			profondeur.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			profondeur.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			helice.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			helice.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			helice.AngularVelocity += new Vector3(0f, 0f, 200f);
			cables.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			cables.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			roue1.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			roue1.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			roue2.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			roue2.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			roueA.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			roueA.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			Crash();
			moteur.Stop(AudioStopOptions.Immediate);
		}
	}
}
