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

public class Corsair
{
	public enum TrainState
	{
		sorti,
		rentre
	}

	private SmokeParticleSystem particules;

	private ExplosionFireSmokeParticleSystem explosion;

	private ParticleSystemManager SmokePSmanager = new ParticleSystemManager();

	public Cue moteur;

	private Cue crash;

	private Cue REST;

	private SoundBank soundBanka;

	private AudioEmitter emitteravion = new AudioEmitter();

	private AudioListener listeneravion = new AudioListener();

	private SceneObject objfuselage;

	private SceneObject objailesBG;

	private SceneObject objailesBD;

	private SceneObject objderive;

	private SceneObject objderiveMob;

	private SceneObject objprofondeurG;

	private SceneObject objprofondeurD;

	private SceneObject objprofondeurMobG;

	private SceneObject objprofondeurMobD;

	private SceneObject objhelice;

	private SceneObject objcables;

	private SceneObject objroue;

	private SceneObject objroue1;

	private SceneObject objroueA;

	private SceneObject objaileronG;

	private SceneObject objaileronD;

	private SceneObject objhelice2;

	private SceneObject objtraindroit;

	private SceneObject objtraingauche;

	private SceneObject objtrape1d;

	private SceneObject objtrape2d;

	private SceneObject objtrape1g;

	private SceneObject objtrape2g;

	private List<CompoundShapeEntry> avion = new List<CompoundShapeEntry>();

	private Entity<CompoundCollidable> AvionNeuf;

	private ConvexHullShape fuselageshape;

	private ConvexHullShape ailesBGshape;

	private ConvexHullShape ailesBDshape;

	private ConvexHullShape deriveshape;

	private ConvexHullShape profondeurshapeG;

	private ConvexHullShape profondeurshapeD;

	private ConvexHullShape heliceshape;

	private ConvexHullShape cablesshape;

	private ConvexHull fuselage;

	private ConvexHull aileBG;

	private ConvexHull aileBD;

	private ConvexHull derive;

	private ConvexHull profondeurD;

	private ConvexHull profondeurG;

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

	private float pitch = -11.8f;

	private float pitchMaxi = 12f;

	private float pitchMini = -11.8f;

	public float VitesseAvion;

	public float VitesseAV;

	public float altitude;

	private Vector3 Monte;

	private Vector3 DroiteS;

	private Vector3 Droite;

	private Vector3 DroiteP;

	public float vitesseacceleration;

	private float vitesseMaxi = 19000f;

	private float vitesseMini;

	private float vitesseMaxiA = 95f;

	private float accelerationDE;

	private float acceleration;

	public float Angle2;

	private float deriveRot;

	private float profondeurRot;

	private float aileronGRot;

	private float aileronDRot;

	private float helicerotation;

	private float trainRot;

	private float traperotD;

	private float traperotG;

	private float sensibilite = 700f;

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

	private float angleroue = 1.5708f;

	private float angleroue1 = 1.5708f;

	public float Ytrain = -1.09f;

	private float Ztrain = -1.47f;

	private float YtrainA = 0.2f;

	public TrainState trainState;

	private Model fuselageModel;

	private Model ailesDModel;

	private Model ailesGModel;

	private Model deriveModel;

	private Model profondeurGModel;

	private Model profondeurDModel;

	private Model heliceModel;

	private Model cablesModel;

	private Model wheelModel;

	private Model roueAModel;

	private Model fuselageModelh;

	private Model ailesBDModelh;

	private Model ailesBGModelh;

	private Model deriveModelh;

	private Model profondeurModelhG;

	private Model profondeurModelhD;

	private Model heliceModelh;

	private Model cablesModelh;

	private Cylinder roue1;

	private Cylinder roue2;

	private Cylinder roueA;

	private Matrix wheelGraphicRotation;

	private Matrix OffsetTransformfuselage = Matrix.Identity;

	private Matrix OffsetTransformProfD = Matrix.Identity;

	private Matrix OffsetTransformProfG = Matrix.Identity;

	private Matrix OffsetTransformaileBG = Matrix.Identity;

	private Matrix OffsetTransformaileBD = Matrix.Identity;

	private Matrix OffsetTransformderive = Matrix.Identity;

	private Matrix OffsetTransformprofondeurmobD = Matrix.Identity;

	private Matrix OffsetTransformprofondeurmobG = Matrix.Identity;

	private Matrix OffsetTransformhelice = Matrix.Identity;

	private Matrix OffsetTransformcables = Matrix.Identity;

	private Matrix Positionderivemob = Matrix.CreateTranslation(-0f, 0.98f, 3.87f);

	private Matrix PositionprofondeurmobD = Matrix.CreateTranslation(-0f, 0.3f, 4.5f);

	private Matrix PositionprofondeurmobG = Matrix.CreateTranslation(-0f, 0.3f, 4.5f);

	private Matrix PositionaileronDmob = Matrix.CreateTranslation(2.53f, -0.74f, 1.53f) * Matrix.CreateRotationY(0.14f) * Matrix.CreateRotationZ(0.2f);

	private Matrix PositionaileronGmob = Matrix.CreateTranslation(-2.53f, -0.74f, 1.53f) * Matrix.CreateRotationY(-0.14f) * Matrix.CreateRotationZ(-0.2f);

	private Matrix Positionpot = Matrix.CreateTranslation(-0.7f, 0.3f, 0.4f);

	private Matrix Positiontrain = Matrix.CreateTranslation(-0f, -0.51f, 0f);

	private Matrix Positiontrape1G = Matrix.CreateTranslation(-1.4f, -0.5f, 0f);

	private Matrix Positiontrape2G = Matrix.CreateTranslation(-0.9f, -0.5f, 0f);

	private Matrix Positiontrape1D = Matrix.CreateTranslation(1.4f, -0.5f, 0f);

	private Matrix Positiontrape2D = Matrix.CreateTranslation(0.9f, -0.5f, 0f);

	private Matrix Positionhelice2 = Matrix.CreateTranslation(0f, 0f, -1.55f);

	public Corsair(CustomPhysicsGame game)
	{
		particules = new SmokeParticleSystem(game);
		explosion = new ExplosionFireSmokeParticleSystem(game);
		inputStateConfig = new ManetteConfig(game);
	}

	public void Load(CustomPhysicsGame game)
	{
		fuselageModelh = game.Content.Load<Model>("Models/corsair/hullfuselqgeC");
		ailesBGModelh = game.Content.Load<Model>("Models/corsair/hullaileGC");
		ailesBDModelh = game.Content.Load<Model>("Models/corsair/hullaileDC");
		deriveModelh = game.Content.Load<Model>("Models/corsair/hullderive");
		profondeurModelhD = game.Content.Load<Model>("Models/corsair/hullprofondeurD");
		profondeurModelhG = game.Content.Load<Model>("Models/corsair/hullprofondeurG");
		heliceModelh = game.Content.Load<Model>("Models/corsair/hullhelice");
		cablesModelh = game.Content.Load<Model>("Models/spad/cablesh");
		Model model = game.Content.Load<Model>("Models/corsair/heliceRot");
		Model model2 = game.Content.Load<Model>("Models/corsair/avionpiecesC");
		Model model3 = game.Content.Load<Model>("Models/corsair/trains");
		fuselageModel = game.Content.Load<Model>("Models/corsair/fuselageC");
		ailesGModel = game.Content.Load<Model>("Models/corsair/AileGC");
		ailesDModel = game.Content.Load<Model>("Models/corsair/AileDC");
		deriveModel = game.Content.Load<Model>("Models/corsair/deriveC");
		profondeurDModel = game.Content.Load<Model>("Models/corsair/ProfondeurDC");
		profondeurGModel = game.Content.Load<Model>("Models/corsair/ProfondeurGC");
		heliceModel = game.Content.Load<Model>("Models/corsair/helice");
		cablesModel = game.Content.Load<Model>("Models/corsair/cablesC");
		wheelModel = game.Content.Load<Model>("Models/corsair/RoueC");
		roueAModel = game.Content.Load<Model>("Models/corsair/RoueAC");
		ModelMesh mesh = model2.Meshes["deriveM"];
		ModelMesh mesh2 = model2.Meshes["profondeurMD"];
		ModelMesh mesh3 = model2.Meshes["profondeurMG"];
		ModelMesh mesh4 = model2.Meshes["AileronD"];
		ModelMesh mesh5 = model2.Meshes["AileronG"];
		ModelMesh mesh6 = model3.Meshes["trainD"];
		ModelMesh mesh7 = model3.Meshes["trape1D"];
		ModelMesh mesh8 = model3.Meshes["trape2D"];
		ModelMesh mesh9 = model3.Meshes["trainG"];
		ModelMesh mesh10 = model3.Meshes["trape1G"];
		ModelMesh mesh11 = model3.Meshes["trape2G"];
		objfuselage = new SceneObject(fuselageModel);
		objtraindroit = new SceneObject(mesh6);
		objtrape1d = new SceneObject(mesh7);
		objtrape2d = new SceneObject(mesh8);
		objtraingauche = new SceneObject(mesh9);
		objtrape1g = new SceneObject(mesh10);
		objtrape2g = new SceneObject(mesh11);
		objailesBD = new SceneObject(ailesDModel);
		objailesBG = new SceneObject(ailesGModel);
		objderive = new SceneObject(deriveModel);
		objderiveMob = new SceneObject(mesh);
		objprofondeurD = new SceneObject(profondeurDModel);
		objprofondeurG = new SceneObject(profondeurGModel);
		objprofondeurMobD = new SceneObject(mesh2);
		objprofondeurMobG = new SceneObject(mesh3);
		objhelice = new SceneObject(heliceModel);
		objcables = new SceneObject(cablesModel);
		objroue = new SceneObject(wheelModel);
		objroue1 = new SceneObject(wheelModel);
		objroueA = new SceneObject(roueAModel);
		objaileronG = new SceneObject(mesh5);
		objaileronD = new SceneObject(mesh4);
		objhelice2 = new SceneObject(model);
		TriangleMesh.GetVerticesAndIndicesFromModel(fuselageModelh, out var vertices, out var indices);
		fuselageshape = new ConvexHullShape(vertices, out var center);
		fuselage = new ConvexHull(vertices, 200f);
		TriangleMesh.GetVerticesAndIndicesFromModel(ailesBGModelh, out vertices, out indices);
		ailesBGshape = new ConvexHullShape(vertices, out center);
		aileBG = new ConvexHull(vertices, 50f);
		TriangleMesh.GetVerticesAndIndicesFromModel(ailesBDModelh, out vertices, out indices);
		ailesBDshape = new ConvexHullShape(vertices, out center);
		aileBD = new ConvexHull(vertices, 50f);
		TriangleMesh.GetVerticesAndIndicesFromModel(deriveModelh, out vertices, out indices);
		deriveshape = new ConvexHullShape(vertices, out center);
		derive = new ConvexHull(vertices, 30f);
		TriangleMesh.GetVerticesAndIndicesFromModel(profondeurModelhD, out vertices, out indices);
		profondeurshapeD = new ConvexHullShape(vertices, out center);
		profondeurD = new ConvexHull(vertices, 50f);
		TriangleMesh.GetVerticesAndIndicesFromModel(profondeurModelhG, out vertices, out indices);
		profondeurshapeG = new ConvexHullShape(vertices, out center);
		profondeurG = new ConvexHull(vertices, 30f);
		TriangleMesh.GetVerticesAndIndicesFromModel(heliceModelh, out vertices, out indices);
		heliceshape = new ConvexHullShape(vertices, out center);
		helice = new ConvexHull(vertices, 30f);
		TriangleMesh.GetVerticesAndIndicesFromModel(cablesModelh, out vertices, out indices);
		cablesshape = new ConvexHullShape(vertices, out center);
		cables = new ConvexHull(vertices, 10f);
		roue1 = new Cylinder(new Vector3(0f, 0f, 0f), 0.25f, 0.5f, 5f);
		roue2 = new Cylinder(new Vector3(0f, 0f, 0f), 0.25f, 0.5f, 5f);
		roueA = new Cylinder(new Vector3(0f, 0f, 0f), 0.15f, 0.08f, 3f);
		avion.Add(new CompoundShapeEntry(fuselageshape, new Vector3(0f, 0f, 0f), 200f));
		avion.Add(new CompoundShapeEntry(ailesBGshape, new Vector3(-1.81f, -0.35f, -0.26f), 100f));
		avion.Add(new CompoundShapeEntry(ailesBDshape, new Vector3(1.76f, -0.29f, -0.29f), 100f));
		avion.Add(new CompoundShapeEntry(deriveshape, new Vector3(0f, 0.6f, 2.79f), 30f));
		avion.Add(new CompoundShapeEntry(profondeurshapeD, new Vector3(0.8f, 0.05f, 3.1f), 30f));
		avion.Add(new CompoundShapeEntry(profondeurshapeG, new Vector3(-0.7f, 0.04f, 3.1f), 30f));
		avion.Add(new CompoundShapeEntry(heliceshape, new Vector3(0.05f, -0.17f, -2.63f), 30f));
		avion.Add(new CompoundShapeEntry(cablesshape, new Vector3(0f, 1.05f, 1.1f), 10f));
		AvionNeuf = new CompoundBody(avion, 600f);
		Vehicle = new Vehicle(AvionNeuf);
		owningSpace = game.space;
		AvionNeuf.CollisionInformation.Events.DetectingInitialCollision += HandleCollision;
		owningSpace.Add(Vehicle);
		sphere = new Sphere(new Vector3(0f, 0f, 0f), 0.3f);
		pot = new Sphere(new Vector3(0f, 0f, 0f), 0.1f);
		AvionNeuf.CollisionInformation.LocalPosition = new Vector3(0f, 0.08f, 1.48f);
		AvionNeuf.Position = new Vector3(0f, 1.4f, 0f);
		particules.AutoInitialize(game.GraphicsDevice, game.Content, null);
		explosion.AutoInitialize(game.GraphicsDevice, game.Content, null);
		SmokePSmanager.AddParticleSystem(particules);
		SmokePSmanager.AddParticleSystem(explosion);
		Vehicle.Body.LinearVelocity = Vector3.Zero;
		Vehicle.Body.AngularVelocity = Vector3.Zero;
		Vehicle.Body.Orientation = new Quaternion(0.1f, 0f, 0f, 1f);
		wheelGraphicRotation = Matrix.CreateFromAxisAngle(Vector3.Forward, 1.5708f);
		Vehicle.AddWheel(new Wheel(new RaycastWheelShape(0.376f, wheelGraphicRotation), new WheelSuspension(40000f, 50f, Vector3.Down, 0.6f, new Vector3(-1.14f, Ytrain, Ztrain)), new WheelDrivingMotor(0.1f, 30000f, 0f), new WheelBrake(0f, 0f, 0.068f), new WheelSlidingFriction(1f, 2f)));
		Vehicle.AddWheel(new Wheel(new RaycastWheelShape(0.376f, wheelGraphicRotation), new WheelSuspension(40000f, 50f, Vector3.Down, 0.6f, new Vector3(1.14f, Ytrain, Ztrain)), new WheelDrivingMotor(0.1f, 30000f, 0f), new WheelBrake(0f, 0f, 0.068f), new WheelSlidingFriction(1f, 2f)));
		Vehicle.AddWheel(new Wheel(new RaycastWheelShape(0.2f, wheelGraphicRotation), new WheelSuspension(50000f, 100f, Vector3.Down, 0.8f, new Vector3(0f, YtrainA, 2.45f)), new WheelDrivingMotor(0.1f, 30000f, 0f), new WheelBrake(0f, 0f, 0.06f), new WheelSlidingFriction(1f, 1f)));
		foreach (Wheel wheel in Vehicle.Wheels)
		{
			wheel.Shape.FreezeWheelsWhileBraking = true;
			wheel.Suspension.SolverSettings.MaximumIterations = 1;
			wheel.Brake.SolverSettings.MaximumIterations = 1;
			wheel.SlidingFriction.SolverSettings.MaximumIterations = 1;
			wheel.DrivingMotor.SolverSettings.MaximumIterations = 0;
		}
		OffsetTransformfuselage = Matrix.CreateTranslation(-fuselage.Position);
		OffsetTransformaileBG = Matrix.CreateTranslation(-aileBG.Position);
		OffsetTransformaileBD = Matrix.CreateTranslation(-aileBD.Position);
		OffsetTransformderive = Matrix.CreateTranslation(-derive.Position);
		OffsetTransformProfD = Matrix.CreateTranslation(-profondeurD.Position);
		OffsetTransformProfG = Matrix.CreateTranslation(-profondeurG.Position);
		OffsetTransformprofondeurmobD = Matrix.CreateTranslation(-profondeurD.Position);
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
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objtraindroit);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objtrape1d);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objtrape2d);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objtraingauche);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objtrape1g);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objtrape2g);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objfuselage);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objailesBD);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objailesBG);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objderive);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objderiveMob);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objprofondeurD);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objprofondeurG);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objprofondeurMobD);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objprofondeurMobG);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objcables);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objaileronG);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objaileronD);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objhelice2);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objroueA);
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
		objfuselage.World = AvionNeuf.WorldTransform;
		objailesBD.World = AvionNeuf.WorldTransform;
		objailesBG.World = AvionNeuf.WorldTransform;
		objderive.World = AvionNeuf.WorldTransform;
		objderiveMob.World = Matrix.CreateRotationY(MathHelper.ToRadians(deriveRot)) * Positionderivemob * AvionNeuf.WorldTransform;
		objprofondeurD.World = AvionNeuf.WorldTransform;
		objprofondeurG.World = AvionNeuf.WorldTransform;
		objprofondeurMobD.World = Matrix.CreateRotationX(MathHelper.ToRadians(profondeurRot)) * PositionprofondeurmobD * AvionNeuf.WorldTransform;
		objprofondeurMobG.World = Matrix.CreateRotationX(MathHelper.ToRadians(profondeurRot)) * PositionprofondeurmobG * AvionNeuf.WorldTransform;
		objcables.World = AvionNeuf.WorldTransform;
		objaileronG.World = Matrix.CreateRotationX(MathHelper.ToRadians(aileronDRot)) * PositionaileronGmob * AvionNeuf.WorldTransform;
		objaileronD.World = Matrix.CreateRotationX(MathHelper.ToRadians(aileronGRot)) * PositionaileronDmob * AvionNeuf.WorldTransform;
		objhelice2.World = Matrix.CreateRotationZ(helicerotation) * AvionNeuf.WorldTransform;
		helice.WorldTransform = Matrix.CreateRotationZ(rot) * Positionhelice2 * AvionNeuf.WorldTransform;
		objhelice.World = OffsetTransformhelice * helice.WorldTransform;
		pot.WorldTransform = Positionpot * AvionNeuf.WorldTransform;
		objtraindroit.World = Matrix.CreateRotationX(MathHelper.ToRadians(trainRot)) * Positiontrain * AvionNeuf.WorldTransform;
		objtrape1d.World = Matrix.CreateRotationZ(MathHelper.ToRadians(traperotD)) * Positiontrape1D * AvionNeuf.WorldTransform;
		objtrape2d.World = Matrix.CreateRotationZ(MathHelper.ToRadians(traperotG)) * Positiontrape2D * AvionNeuf.WorldTransform;
		objtraingauche.World = Matrix.CreateRotationX(MathHelper.ToRadians(trainRot)) * Positiontrain * AvionNeuf.WorldTransform;
		objtrape1g.World = Matrix.CreateRotationZ(MathHelper.ToRadians(traperotG)) * Positiontrape1G * AvionNeuf.WorldTransform;
		objtrape2g.World = Matrix.CreateRotationZ(MathHelper.ToRadians(traperotD)) * Positiontrape2G * AvionNeuf.WorldTransform;
		profondeurD.Position = AvionNeuf.Position;
		fuselage.Position = AvionNeuf.Position;
		aileBG.Position = AvionNeuf.Position;
		aileBD.Position = AvionNeuf.Position;
		derive.Position = AvionNeuf.Position;
		profondeurG.Position = AvionNeuf.Position;
		cables.Position = AvionNeuf.Position;
		roue1.Position = AvionNeuf.Position;
		roue2.Position = AvionNeuf.Position;
		roueA.Position = AvionNeuf.Position;
		profondeurD.Orientation = AvionNeuf.Orientation;
		fuselage.Orientation = AvionNeuf.Orientation;
		aileBG.Orientation = AvionNeuf.Orientation;
		aileBD.Orientation = AvionNeuf.Orientation;
		derive.Orientation = AvionNeuf.Orientation;
		profondeurG.Orientation = AvionNeuf.Orientation;
		cables.Orientation = AvionNeuf.Orientation;
		roue1.Orientation = AvionNeuf.Orientation;
		roue2.Orientation = AvionNeuf.Orientation;
		roueA.Orientation = AvionNeuf.Orientation;
	}

	private void Transcasse()
	{
		if (Ytrain <= -0.22f)
		{
			objroue.World = roue1.WorldTransform;
			objroue1.World = roue2.WorldTransform;
			objroueA.World = roueA.WorldTransform;
		}
		if (Ytrain >= -0.2f)
		{
			objroue.World = Vehicle.Wheels[0].Shape.WorldTransform * OffsetTransformaileBG * aileBG.WorldTransform;
			objroue1.World = Vehicle.Wheels[1].Shape.WorldTransform * OffsetTransformaileBD * aileBD.WorldTransform;
			objroueA.World = roueA.WorldTransform;
		}
		objfuselage.World = OffsetTransformfuselage * fuselage.WorldTransform;
		objailesBG.World = OffsetTransformaileBG * aileBG.WorldTransform;
		objailesBD.World = OffsetTransformaileBD * aileBD.WorldTransform;
		objderive.World = OffsetTransformderive * derive.WorldTransform;
		objderiveMob.World = Positionderivemob * OffsetTransformderive * derive.WorldTransform;
		objprofondeurD.World = OffsetTransformProfD * profondeurD.WorldTransform;
		objprofondeurG.World = OffsetTransformProfG * profondeurG.WorldTransform;
		objprofondeurMobD.World = PositionprofondeurmobD * OffsetTransformProfD * profondeurD.WorldTransform;
		objprofondeurMobG.World = PositionprofondeurmobG * OffsetTransformProfG * profondeurG.WorldTransform;
		objhelice.World = OffsetTransformhelice * helice.WorldTransform;
		objcables.World = OffsetTransformcables * cables.WorldTransform;
		objaileronD.World = PositionaileronDmob * OffsetTransformaileBD * aileBD.WorldTransform;
		objaileronG.World = PositionaileronGmob * OffsetTransformaileBG * aileBG.WorldTransform;
		objtraingauche.World = Matrix.CreateRotationX(MathHelper.ToRadians(trainRot)) * Positiontrain * OffsetTransformaileBG * aileBG.WorldTransform;
		objtraindroit.World = Matrix.CreateRotationX(MathHelper.ToRadians(trainRot)) * Positiontrain * OffsetTransformaileBD * aileBD.WorldTransform;
		objtrape1d.World = Matrix.CreateRotationZ(MathHelper.ToRadians(traperotD)) * Positiontrape1D * OffsetTransformaileBD * aileBD.WorldTransform;
		objtrape2d.World = Matrix.CreateRotationZ(MathHelper.ToRadians(traperotG)) * Positiontrape2D * OffsetTransformaileBD * aileBD.WorldTransform;
		objtrape1g.World = Matrix.CreateRotationZ(MathHelper.ToRadians(traperotG)) * Positiontrape1G * OffsetTransformaileBG * aileBG.WorldTransform;
		objtrape2g.World = Matrix.CreateRotationZ(MathHelper.ToRadians(traperotD)) * Positiontrape1G * OffsetTransformaileBG * aileBG.WorldTransform;
		objhelice2.World = Matrix.CreateTranslation(0f, -20f, 0f);
	}

	public void Moteur()
	{
		moteur = soundBanka.GetCue("corsair");
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
		if ((sender != null && VitesseAvion >= 26f) || accelerationDE <= -4f)
		{
			Collision1();
		}
	}

	public void Update(float dt, CustomPhysicsGame game, GameTime gameTime)
	{
		inputStateConfig.Update(game);
		game.space.Update();
		game.camera.position = new Vector3(-33f, 15f, 12f);
		checked
		{
			if (inputStateConfig.ApressBis && VitesseAvion >= 3f)
			{
				if (trainState == TrainState.sorti)
				{
					trainState++;
				}
				else
				{
					trainState--;
				}
			}
			if (trainState == TrainState.rentre)
			{
				trainRot -= 0.5f;
				if (Ytrain >= -0.4f)
				{
					traperotD -= 0.5f;
					traperotG += 0.5f;
				}
				YtrainA += 0.002f;
				Ytrain += 0.00379f;
				Ztrain += 0.00414f;
				Vehicle.Wheels[0].Suspension.LocalAttachmentPoint = new Vector3(-1.14f, Ytrain, Ztrain);
				Vehicle.Wheels[1].Suspension.LocalAttachmentPoint = new Vector3(1.14f, Ytrain, Ztrain);
				Vehicle.Wheels[2].Suspension.LocalAttachmentPoint = new Vector3(0f, YtrainA, 2.45f);
				angleroue += 0.007f;
				angleroue1 -= 0.007f;
				Vehicle.Wheels[0].Shape.SpinAngle = 0f;
				Vehicle.Wheels[1].Shape.SpinAngle = 0f;
				if (Ytrain >= -0.22f)
				{
					game.terrain.sceneInterfaceScene.ObjectManager.Remove(objroue);
					game.terrain.sceneInterfaceScene.ObjectManager.Remove(objroue1);
				}
				Vehicle.Wheels[0].Shape.LocalGraphicTransform = (wheelGraphicRotation = Matrix.CreateFromAxisAngle(Vector3.Forward, angleroue));
				Vehicle.Wheels[1].Shape.LocalGraphicTransform = (wheelGraphicRotation = Matrix.CreateFromAxisAngle(Vector3.Forward, angleroue1));
				if (angleroue >= 3.1416f)
				{
					angleroue = 3.1416f;
				}
				if (angleroue1 <= 0f)
				{
					angleroue1 = 0f;
				}
				if (YtrainA >= 0.38f)
				{
					YtrainA = 0.38f;
				}
				if (Ytrain >= -0.19f)
				{
					Ytrain = -0.19f;
				}
				if (Ztrain >= -0.75f)
				{
					Ztrain = -0.75f;
				}
				if (trainRot <= -95f)
				{
					trainRot = -95f;
				}
				if (traperotD <= -80f)
				{
					traperotD = -80f;
				}
				if (traperotG >= 80f)
				{
					traperotG = 80f;
				}
			}
			if (trainState == TrainState.sorti)
			{
				YtrainA -= 0.002f;
				traperotD += 0.5f;
				traperotG -= 0.5f;
				if (traperotD >= -10f)
				{
					trainRot += 0.5f;
					Ytrain -= 0.0061f;
					Ztrain -= 0.00384f;
					angleroue -= 0.007f;
					angleroue1 += 0.007f;
				}
				if (Ytrain <= -0.9f)
				{
					game.terrain.sceneInterfaceScene.ObjectManager.Submit(objroue);
					game.terrain.sceneInterfaceScene.ObjectManager.Submit(objroue1);
				}
				Vehicle.Wheels[0].Suspension.LocalAttachmentPoint = new Vector3(-1.14f, Ytrain, Ztrain);
				Vehicle.Wheels[1].Suspension.LocalAttachmentPoint = new Vector3(1.14f, Ytrain, Ztrain);
				Vehicle.Wheels[2].Suspension.LocalAttachmentPoint = new Vector3(0f, YtrainA, 2.45f);
				Vehicle.Wheels[0].Shape.LocalGraphicTransform = (wheelGraphicRotation = Matrix.CreateFromAxisAngle(Vector3.Forward, angleroue));
				Vehicle.Wheels[1].Shape.LocalGraphicTransform = (wheelGraphicRotation = Matrix.CreateFromAxisAngle(Vector3.Forward, angleroue1));
				if (angleroue <= 1.5708f)
				{
					angleroue = 1.5708f;
				}
				if (angleroue1 >= 1.5708f)
				{
					angleroue1 = 1.5708f;
				}
				if (YtrainA <= 0.2f)
				{
					YtrainA = 0.2f;
				}
				if (Ytrain <= -1.09f)
				{
					Ytrain = -1.09f;
				}
				if (Ztrain <= -1.47f)
				{
					Ztrain = -1.47f;
				}
				if (trainRot >= 0f)
				{
					trainRot = 0f;
				}
				if (traperotD >= 0f)
				{
					traperotD = 0f;
				}
				if (traperotG <= 0f)
				{
					traperotG = 0f;
				}
			}
			if (!Avionloin)
			{
				temploin = 1f;
			}
			if (!Avioncasse)
			{
				tempcrash = 0f;
				Transneuf();
				piecesVu(game);
				game.camera.Target = Vehicle.Body.Position;
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
			game.space.Update();
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
			float num3 = (float)Math.Sin(Vector3.Dot(sphere.OrientationMatrix.Down, Vehicle.Body.OrientationMatrix.Left)) * ((float)Math.PI * 2f);
			Angle2 = (float)Math.Acos(Vector3.Dot(sphere.OrientationMatrix.Down, Vehicle.Body.OrientationMatrix.Forward)) * ((float)Math.PI * 2f);
			float num4 = (float)Math.Atan(Vector3.Dot(sphere.WorldTransform.Up, Vehicle.Body.OrientationMatrix.Down)) * ((float)Math.PI * 2f);
			float num5 = (float)Math.Atan(Vector3.Dot(sphere.OrientationMatrix.Down, Vehicle.Body.OrientationMatrix.Left)) * ((float)Math.PI * 2f);
			float num6 = (float)Math.Acos(Vector3.Dot(sphere.OrientationMatrix.Down, Vehicle.Body.OrientationMatrix.Left)) * ((float)Math.PI * 2f);
			profondeurRot = 0f;
			Vehicle.Body.AngularDamping = 0.72f;
			float num7 = VitesseAvion * 8f;
			if (num7 >= 160f)
			{
				num7 = 160f;
			}
			Vector3 right = Vehicle.Body.OrientationMatrix.Right;
			if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M1 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M4 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M6)
			{
				Monte = -right * num7 * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.Y;
				profondeurRot += 30f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.Y;
				Vehicle.Body.ApplyAngularImpulse(ref Monte);
			}
			if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M2 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M3 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M5)
			{
				Monte = -right * num7 * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.Y;
				profondeurRot += 30f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.Y;
				Vehicle.Body.ApplyAngularImpulse(ref Monte);
			}
			deriveRot = 0f;
			float num8 = VitesseAvion * 8f;
			if (num8 >= 110f)
			{
				num8 = 110f;
			}
			Vector3 up = Vehicle.Body.OrientationMatrix.Up;
			if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M1 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M2 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M6)
			{
				DroiteS = -up * num8 * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
				deriveRot += 28f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
				if (altitude <= 1.8f)
				{
					DroiteS = -up * VitesseAvion * 20f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
					if (VitesseAvion <= 6f && VitesseAvion >= 0.6f)
					{
						DroiteS = -up * 300f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
					}
				}
				Vehicle.Body.ApplyAngularImpulse(ref DroiteS);
			}
			if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M3 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M4 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M5)
			{
				DroiteS = -up * num8 * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.X;
				deriveRot += 28f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.X;
				if (altitude <= 1.8f)
				{
					DroiteS = -up * VitesseAvion * 20f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.X;
					if (VitesseAvion <= 6f && VitesseAvion >= 0.6f)
					{
						DroiteS = -up * 300f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.X;
					}
				}
				Vehicle.Body.ApplyAngularImpulse(ref DroiteS);
			}
			aileronGRot = 0f;
			aileronDRot = 0f;
			float num9 = VitesseAvion * 8f;
			if (num9 >= 200f)
			{
				num9 = 200f;
			}
			Vector3 forward = Vehicle.Body.OrientationMatrix.Forward;
			if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M1 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M2 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M6)
			{
				Droite = forward * num9 * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.X;
				aileronGRot += 28f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.X;
				aileronDRot -= 28f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.X;
				Vehicle.Body.ApplyAngularImpulse(ref Droite);
			}
			if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M3 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M4 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M5)
			{
				Droite = forward * num9 * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
				aileronGRot += 28f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
				aileronDRot -= 28f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
				Vehicle.Body.ApplyAngularImpulse(ref Droite);
			}
			float num10 = num5 * 20f;
			if (num10 >= 31f)
			{
				num10 = 31f;
			}
			if (num10 <= -31f)
			{
				num10 = -31f;
			}
			DroiteP = up * num10;
			Vehicle.Body.ApplyAngularImpulse(ref DroiteP);
			pot.Position = Vehicle.Body.Position;
			sphere.Position = Vehicle.Body.Position;
			Vector3 impulse = Monte / 4f;
			if (VitesseAvion >= 30f && num6 >= 5f && num6 <= 14f && altitude >= 5.75f && Angle2 <= 8.5f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse);
			}
			Vector3 impulse2 = new Vector3(0f, -400f, 0f);
			if (altitude <= 2.5f && VitesseAvion <= 15f && VitesseAvion >= 1f)
			{
				Vehicle.Body.ApplyLinearImpulse(ref impulse2);
			}
			Vector3 impulse3 = -right * 150f;
			Vector3 impulse4 = right * 80f;
			if (altitude <= 3f && Angle2 >= 10.8f && VitesseAvion <= 8f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse3);
			}
			if (altitude <= 3f && Angle2 <= 10.7f && VitesseAvion <= 10f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse4);
			}
			Vector3 impulse5 = right * 250f;
			if (VitesseAvion <= 3f && altitude <= 3f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse5);
			}
			Vector3 impulse6 = -right * 1800f / Angle2;
			Vector3 impulse7 = right * 1800f / Angle2;
			Vector3 impulse8 = -up * 3000f / Angle2;
			Vector3 impulse9 = up * 3000f / Angle2;
			Vector3 impulse10 = -up * 1400f / Angle2;
			Vector3 impulse11 = up * 1400f / Angle2;
			Vector3 impulse12 = -right * 80f / Angle2;
			Vector3 impulse13 = right * 1100f / Angle2;
			if (num4 >= 0f && Angle2 >= 10f && num6 >= 4f && num6 <= 15f && altitude >= 4f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse13);
			}
			if (Angle2 >= 11.9f && num6 >= 4f && num6 <= 15f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse12);
			}
			if (VitesseAvion <= 22f && altitude >= 3f)
			{
				if (num4 <= 0f && Angle2 >= 11f && num6 >= 4f && num6 <= 15f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse6);
				}
				if (num4 >= 0f && Angle2 >= 11f && num6 >= 4f && num6 <= 15f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse7);
				}
				if (num3 <= -2.5f && Angle2 >= 12f && VitesseAvion <= 2f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse8);
				}
				if (num3 >= 2.5f && Angle2 >= 12f && VitesseAvion <= 2f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse9);
				}
				if (num4 <= 0f && num6 >= 4f && num6 <= 15f && Angle2 <= 11f && Angle2 >= 5f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse6);
				}
				if (num4 >= 0f && num6 >= 4f && num6 <= 15f && Angle2 <= 11f && Angle2 >= 5f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse7);
				}
				if (num6 <= 4f && Angle2 <= 11f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse9);
				}
				if (num6 >= 15f && Angle2 <= 11f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse8);
				}
			}
			if (VitesseAvion <= 32f && altitude >= 2f)
			{
				if (num6 <= 4f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse11);
				}
				if (num6 >= 15f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse10);
				}
			}
			vitesseacceleration = accelerationDE + acceleration * dt;
			Vector3 impulse14 = vitesseacceleration * vector2;
			if (vitesseacceleration <= vitesseMini)
			{
				impulse14 = VitesseAvion * vector2;
			}
			if (VitesseAvion >= vitesseMaxiA)
			{
				impulse14 = 0f * vector2;
			}
			if (QuaFuel <= 0f)
			{
				particules.Emitter.Enabled = false;
				impulse14 = VitesseAvion * vector2;
				moteur.Stop(AudioStopOptions.Immediate);
				game.terrain.sceneInterfaceScene.ObjectManager.Remove(objhelice2);
				game.terrain.sceneInterfaceScene.ObjectManager.Submit(objhelice);
			}
			Vehicle.Body.ApplyLinearImpulse(ref impulse14);
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
			if (trainState == TrainState.sorti)
			{
				Vehicle.Body.LinearDamping = Angle2 / 76f;
			}
			if (VitesseAvion < 18f)
			{
				Vehicle.Body.LinearDamping = 0f;
			}
			RAcc = vitesseMaxi / AccAPP;
			variA = RAcc / sensibilite;
			moteurA = AccM / variA;
			bouttonA = AccB / variA;
			if ((game.manetteChoix == CustomPhysicsGame.ManetteChoix.M5 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M6) && inputStateConfig.Acceleration)
			{
				acceleration += 100f;
				moteur.SetVariable("accelerateur", pitch += 0.16f);
				MDboutton -= 0.6f;
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
				acceleration -= 100f;
				moteur.SetVariable("accelerateur", pitch -= 0.16f);
				MDboutton += 0.6f;
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
			if (VitesseAvion <= 22f && altitude >= 3f)
			{
				accelerationDE -= 0.1f;
			}
			else
			{
				accelerationDE += 0.02f;
			}
			if (Angle2 >= 16f)
			{
				accelerationDE -= 0.1f;
			}
			else
			{
				accelerationDE += 0.02f;
			}
			if (num6 <= 4f || num6 >= 15f)
			{
				accelerationDE -= 0.1f;
			}
			else
			{
				accelerationDE += 0.02f;
			}
			if (accelerationDE <= -50f)
			{
				accelerationDE = -50f;
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
			JaugeBoutton = VitesseAvion * 0.85f + 128f;
			if (JaugeBoutton >= 199f)
			{
				JaugeBoutton = 199f;
			}
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
			else
			{
				GamePad.SetVibration(game.menu.player, 0f, 0f);
			}
			if (tempcrash >= 30f)
			{
				owningSpace.Remove(fuselage);
				owningSpace.Remove(aileBG);
				owningSpace.Remove(aileBD);
				owningSpace.Remove(derive);
				owningSpace.Remove(profondeurD);
				owningSpace.Remove(profondeurG);
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
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objtraindroit);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objtrape1d);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objtrape2d);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objtraingauche);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objtrape1g);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objtrape2g);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objfuselage);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objailesBD);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objailesBG);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objderive);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objderiveMob);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objprofondeurD);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objprofondeurG);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objprofondeurMobD);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objprofondeurMobG);
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
			owningSpace.Remove(fuselage);
			owningSpace.Remove(aileBG);
			owningSpace.Remove(aileBD);
			owningSpace.Remove(derive);
			owningSpace.Remove(profondeurD);
			owningSpace.Remove(profondeurG);
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
		trainState = TrainState.sorti;
		acceleration = 0f;
		accelerationDE = 0f;
		vitesseacceleration = 0f;
		VitesseAvion = 0f;
		MDboutton = 636f;
		JaugeBoutton = 40f;
		pitch = -11.8f;
		game.camera.fov = 45f;
		Vehicle.Body.LinearVelocity = Vector3.Zero;
		Vehicle.Body.AngularVelocity = Vector3.Zero;
		Vehicle.Body.Orientation = new Quaternion(0.1f, 0f, 0f, 1f);
		AvionNeuf.Position = new Vector3(0f, 1.4f, 0f);
		particules.Emitter.Enabled = true;
		Moteur();
		trainRot = 0f;
		traperotD = 0f;
		traperotG = 0f;
		Ytrain = -1.12f;
		Ztrain = -1.47f;
		angleroue = 1.5708f;
		angleroue1 = 1.5708f;
	}

	private void Collision1()
	{
		QuaFuel = 18000f;
		JaugeFuel = 383f;
		checked
		{
			if (CC)
			{
				compteCrash++;
			}
			Avioncasse = true;
			particules.Emitter.Enabled = false;
			explosion.Emitter.Enabled = true;
			if (altitude <= 8f)
			{
				explosion.Explode();
			}
			owningSpace.Remove(Vehicle);
			owningSpace.Add(fuselage);
			owningSpace.Add(aileBG);
			owningSpace.Add(aileBD);
			owningSpace.Add(derive);
			owningSpace.Add(profondeurD);
			owningSpace.Add(profondeurG);
			owningSpace.Add(helice);
			owningSpace.Add(cables);
			owningSpace.Add(roue1);
			owningSpace.Add(roue2);
			owningSpace.Add(roueA);
			fuselage.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			fuselage.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			aileBG.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			aileBG.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			aileBD.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			aileBD.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			derive.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			derive.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			profondeurD.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			profondeurD.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			profondeurG.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			profondeurG.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			helice.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			helice.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			helice.AngularVelocity += new Vector3(200f, 0f, 100f);
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
