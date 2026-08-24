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

public class AC130
{
	public enum TrainState
	{
		sorti,
		rentre
	}

	private SmokeParticleSystem particules;

	private SmokeParticleSystem particules2;

	private SmokeParticleSystem particules3;

	private SmokeParticleSystem particules4;

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

	private SceneObject objhelice1;

	private SceneObject objroue;

	private SceneObject objroue1;

	private SceneObject objroueA;

	private SceneObject objaileronG;

	private SceneObject objaileronD;

	private SceneObject objheliceRon1;

	private SceneObject objheliceRon2;

	private SceneObject objheliceRon3;

	private SceneObject objheliceRon4;

	private SceneObject objtraindroit;

	private SceneObject objtrappeDP;

	private SceneObject objtrappeDG;

	private SceneObject objtraingauche;

	private SceneObject objtrappeGP;

	private SceneObject objtrappeGG;

	private SceneObject objtrainavant;

	private SceneObject objtrape1g;

	private SceneObject objtrape2g;

	private SceneObject objhelice2;

	private SceneObject objhelice3;

	private SceneObject objhelice4;

	private SceneObject objroue2;

	private SceneObject objroue3;

	private SceneObject objcables;

	private List<CompoundShapeEntry> avion = new List<CompoundShapeEntry>();

	private Entity<CompoundCollidable> AvionNeuf;

	private ConvexHullShape fuselageshape;

	private ConvexHullShape ailesBGshape;

	private ConvexHullShape ailesBDshape;

	private ConvexHullShape deriveshape;

	private ConvexHullShape profondeurshapeG;

	private ConvexHullShape profondeurshapeD;

	private ConvexHullShape heliceshape;

	private ConvexHullShape heliceshape1;

	private ConvexHullShape heliceshape2;

	private ConvexHullShape heliceshape3;

	private ConvexHull fuselage;

	private ConvexHull aileBG;

	private ConvexHull aileBD;

	private ConvexHull derive;

	private ConvexHull profondeurD;

	private ConvexHull profondeurG;

	private ConvexHull helice;

	private ConvexHull helice1;

	private ConvexHull helice2;

	private ConvexHull helice3;

	public Vehicle Vehicle;

	private Entity sphere;

	private Entity pot;

	private Entity pot2;

	private Entity pot3;

	private Entity pot4;

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

	public float vitesseacceleration;

	private float vitesseMaxi = 13000f;

	private float vitesseMini;

	private float vitesseMaxiA = 80f;

	private float accelerationDE;

	private float acceleration;

	public float Angle2;

	private float deriveRot;

	private float profondeurRot;

	private float aileronGRot;

	private float aileronDRot;

	private float helicerotation;

	private float trapperotDP = 80f;

	private float trapperotDG = -50f;

	private float trapperotGP = 80f;

	private float trapperotGG = -50f;

	private float trainRotA;

	private float trapeArrAA = -0.3f;

	private float trapeArrAR = 0.5f;

	private float trapeDesAA = -0.01f;

	private float trapeDesAR = -0.03f;

	public float Ytrain = -1.1f;

	private float YtrainA = -0.9f;

	private float ZtrainA = -6.24f;

	private float YT;

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

	public TrainState trainState;

	private Model fuselageModel;

	private Model ailesDModel;

	private Model ailesGModel;

	private Model deriveModel;

	private Model profondeurGModel;

	private Model profondeurDModel;

	private Model heliceModel;

	private Model wheelModel;

	private Model roueAModel;

	private Model cablesModel;

	private Model fuselageModelh;

	private Model ailesBDModelh;

	private Model ailesBGModelh;

	private Model deriveModelh;

	private Model profondeurModelhG;

	private Model profondeurModelhD;

	private Model heliceModelh;

	private Cylinder roue1;

	private Cylinder roue2;

	private Cylinder roue3;

	private Cylinder roue4;

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

	private Matrix OffsetTransformhelice1 = Matrix.Identity;

	private Matrix OffsetTransformhelice2 = Matrix.Identity;

	private Matrix OffsetTransformhelice3 = Matrix.Identity;

	private Matrix OffsetTransformhelice4 = Matrix.Identity;

	private Matrix OffsetTransformtrain = Matrix.Identity;

	private Matrix Positionderivemob = Matrix.CreateTranslation(0f, 0f, 7.1f);

	private Matrix PositionprofondeurmobD = Matrix.CreateTranslation(0f, 0.35f, 6.79f);

	private Matrix PositionprofondeurmobG = Matrix.CreateTranslation(0f, 0.35f, 6.79f);

	private Matrix PositionaileronDmob = Matrix.CreateTranslation(6.546f, 0.35f, 1.47f) * Matrix.CreateRotationY(0.09f) * Matrix.CreateRotationZ(0.019f);

	private Matrix PositionaileronGmob = Matrix.CreateTranslation(-6.546f, 0.35f, 1.47f) * Matrix.CreateRotationY(-0.09f) * Matrix.CreateRotationZ(-0.019f);

	private Matrix PositionHelirot1 = Matrix.CreateTranslation(2f, 0.28f, -0f);

	private Matrix PositionHelirot2 = Matrix.CreateTranslation(-2f, 0.28f, -0f);

	private Matrix PositionHelirot3 = Matrix.CreateTranslation(4.03f, 0.3f, -0f);

	private Matrix PositionHelirot4 = Matrix.CreateTranslation(-4.03f, 0.3f, -0f);

	private Matrix Positionpot = Matrix.CreateTranslation(-2f, 0.2f, 1.5f);

	private Matrix Positionpot2 = Matrix.CreateTranslation(2f, 0.2f, 1.5f);

	private Matrix Positionpot3 = Matrix.CreateTranslation(-4.03f, 0.2f, 1.5f);

	private Matrix Positionpot4 = Matrix.CreateTranslation(4.03f, 0.2f, 1.5f);

	private Matrix PositiontrainD = Matrix.CreateTranslation(0f, -0f, 0f);

	private Matrix PositiontrappeDP = Matrix.CreateTranslation(0.635f, -1.08f, 0f);

	private Matrix PositiontrappeDG = Matrix.CreateTranslation(1.02f, -0.89f, 0f);

	private Matrix PositiontrainG = Matrix.CreateTranslation(-0f, -0f, 0f);

	private Matrix PositiontrappeGP = Matrix.CreateTranslation(-0.638f, -1.08f, 0f);

	private Matrix PositiontrappeGG = Matrix.CreateTranslation(-1.03f, -0.89f, 0f);

	private Matrix PositiontrainA = Matrix.CreateTranslation(-0f, -0.936f, -4.259f);

	private Matrix PositiontrapeD = Matrix.CreateTranslation(-0f, -0f, 0f);

	private Matrix PositiontrapeG = Matrix.CreateTranslation(0f, -0f, 0f);

	private Matrix Positionhelice1 = Matrix.CreateTranslation(2f, 0.29f, 0f);

	private Matrix Positionhelice2 = Matrix.CreateTranslation(-2f, 0.29f, 0f);

	private Matrix Positionhelice3 = Matrix.CreateTranslation(4.03f, 0.29f, 0f);

	private Matrix Positionhelice4 = Matrix.CreateTranslation(-4.03f, 0.29f, 0f);

	public AC130(CustomPhysicsGame game)
	{
		particules = new SmokeParticleSystem(game);
		particules2 = new SmokeParticleSystem(game);
		particules3 = new SmokeParticleSystem(game);
		particules4 = new SmokeParticleSystem(game);
		explosion = new ExplosionFireSmokeParticleSystem(game);
		inputStateConfig = new ManetteConfig(game);
	}

	public void Load(CustomPhysicsGame game)
	{
		fuselageModelh = game.Content.Load<Model>("Models/ac130/Fhull");
		ailesBGModelh = game.Content.Load<Model>("Models/ac130/AGhull");
		ailesBDModelh = game.Content.Load<Model>("Models/ac130/ADhull");
		deriveModelh = game.Content.Load<Model>("Models/ac130/Dhull");
		profondeurModelhD = game.Content.Load<Model>("Models/ac130/PDhull");
		profondeurModelhG = game.Content.Load<Model>("Models/ac130/PGhull");
		heliceModelh = game.Content.Load<Model>("Models/ac130/Hhull");
		Model model = game.Content.Load<Model>("Models/ac130/AcheliceRon");
		Model model2 = game.Content.Load<Model>("Models/ac130/ACpieces");
		Model model3 = game.Content.Load<Model>("Models/ac130/Atrain");
		fuselageModel = game.Content.Load<Model>("Models/ac130/Acfuselage");
		ailesGModel = game.Content.Load<Model>("Models/ac130/AcailleG");
		ailesDModel = game.Content.Load<Model>("Models/ac130/AcailleD");
		deriveModel = game.Content.Load<Model>("Models/ac130/Acderive");
		profondeurDModel = game.Content.Load<Model>("Models/ac130/AcprofondeurD");
		profondeurGModel = game.Content.Load<Model>("Models/ac130/AcprofondeurG");
		heliceModel = game.Content.Load<Model>("Models/ac130/Achelice");
		cablesModel = game.Content.Load<Model>("Models/ac130/Accables");
		wheelModel = game.Content.Load<Model>("Models/ac130/AroueArr");
		roueAModel = game.Content.Load<Model>("Models/ac130/AroueAV");
		ModelMesh mesh = model2.Meshes["deriveMob"];
		ModelMesh mesh2 = model2.Meshes["profonMobD"];
		ModelMesh mesh3 = model2.Meshes["profonMobG"];
		ModelMesh mesh4 = model2.Meshes["aileronD"];
		ModelMesh mesh5 = model2.Meshes["aileronG"];
		ModelMesh mesh6 = model3.Meshes["AtrainD"];
		ModelMesh mesh7 = model3.Meshes["AtrappeDP"];
		ModelMesh mesh8 = model3.Meshes["AtrappeDG"];
		ModelMesh mesh9 = model3.Meshes["AtrainG"];
		ModelMesh mesh10 = model3.Meshes["AtrappeGP"];
		ModelMesh mesh11 = model3.Meshes["AtrappeGG"];
		ModelMesh mesh12 = model3.Meshes["AtrainA"];
		ModelMesh mesh13 = model3.Meshes["AtrappeAA"];
		ModelMesh mesh14 = model3.Meshes["AtrappeAR"];
		objfuselage = new SceneObject(fuselageModel);
		objtraindroit = new SceneObject(mesh6);
		objtrappeDP = new SceneObject(mesh7);
		objtrappeDG = new SceneObject(mesh8);
		objtraingauche = new SceneObject(mesh9);
		objtrappeGP = new SceneObject(mesh10);
		objtrappeGG = new SceneObject(mesh11);
		objtrainavant = new SceneObject(mesh12);
		objtrape1g = new SceneObject(mesh13);
		objtrape2g = new SceneObject(mesh14);
		objailesBD = new SceneObject(ailesDModel);
		objailesBG = new SceneObject(ailesGModel);
		objderive = new SceneObject(deriveModel);
		objderiveMob = new SceneObject(mesh);
		objprofondeurD = new SceneObject(profondeurDModel);
		objprofondeurG = new SceneObject(profondeurGModel);
		objprofondeurMobD = new SceneObject(mesh2);
		objprofondeurMobG = new SceneObject(mesh3);
		objcables = new SceneObject(cablesModel);
		objhelice1 = new SceneObject(heliceModel);
		objhelice2 = new SceneObject(heliceModel);
		objhelice3 = new SceneObject(heliceModel);
		objhelice4 = new SceneObject(heliceModel);
		objroue = new SceneObject(wheelModel);
		objroue1 = new SceneObject(wheelModel);
		objroue2 = new SceneObject(wheelModel);
		objroue3 = new SceneObject(wheelModel);
		objroueA = new SceneObject(roueAModel);
		objaileronG = new SceneObject(mesh5);
		objaileronD = new SceneObject(mesh4);
		objheliceRon1 = new SceneObject(model);
		objheliceRon2 = new SceneObject(model);
		objheliceRon3 = new SceneObject(model);
		objheliceRon4 = new SceneObject(model);
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
		TriangleMesh.GetVerticesAndIndicesFromModel(heliceModelh, out vertices, out indices);
		heliceshape1 = new ConvexHullShape(vertices, out center);
		helice1 = new ConvexHull(vertices, 30f);
		TriangleMesh.GetVerticesAndIndicesFromModel(heliceModelh, out vertices, out indices);
		heliceshape2 = new ConvexHullShape(vertices, out center);
		helice2 = new ConvexHull(vertices, 30f);
		TriangleMesh.GetVerticesAndIndicesFromModel(heliceModelh, out vertices, out indices);
		heliceshape3 = new ConvexHullShape(vertices, out center);
		helice3 = new ConvexHull(vertices, 30f);
		roue1 = new Cylinder(new Vector3(0f, 0f, 0f), 0.25f, 0.5f, 5f);
		roue2 = new Cylinder(new Vector3(0f, 0f, 0f), 0.25f, 0.5f, 5f);
		roue3 = new Cylinder(new Vector3(0f, 0f, 0f), 0.25f, 0.5f, 5f);
		roue4 = new Cylinder(new Vector3(0f, 0f, 0f), 0.25f, 0.5f, 5f);
		roueA = new Cylinder(new Vector3(0f, 0f, 0f), 0.15f, 0.08f, 3f);
		avion.Add(new CompoundShapeEntry(fuselageshape, new Vector3(0f, 0f, 0f), 200f));
		avion.Add(new CompoundShapeEntry(ailesBGshape, new Vector3(-3.9f, 0.58f, -0.46f), 100f));
		avion.Add(new CompoundShapeEntry(ailesBDshape, new Vector3(3.9f, 0.58f, -0.46f), 100f));
		avion.Add(new CompoundShapeEntry(deriveshape, new Vector3(0f, 1.8f, 5.5f), 30f));
		avion.Add(new CompoundShapeEntry(profondeurshapeD, new Vector3(1.4f, 0.65f, 5.9f), 30f));
		avion.Add(new CompoundShapeEntry(profondeurshapeG, new Vector3(-1.4f, 0.65f, 5.9f), 30f));
		avion.Add(new CompoundShapeEntry(heliceshape, new Vector3(2.05f, -0.17f, -2.63f), 30f));
		avion.Add(new CompoundShapeEntry(heliceshape1, new Vector3(-2.05f, -0.17f, -2.63f), 30f));
		AvionNeuf = new CompoundBody(avion, 600f);
		Vehicle = new Vehicle(AvionNeuf);
		owningSpace = game.space;
		AvionNeuf.CollisionInformation.Events.DetectingInitialCollision += HandleCollision;
		owningSpace.Add(Vehicle);
		sphere = new Sphere(new Vector3(0f, 0f, 0f), 0.3f);
		pot = new Sphere(new Vector3(0f, 0f, 0f), 0.1f);
		pot2 = new Sphere(new Vector3(0f, 0f, 0f), 0.1f);
		pot3 = new Sphere(new Vector3(0f, 0f, 0f), 0.1f);
		pot4 = new Sphere(new Vector3(0f, 0f, 0f), 0.1f);
		AvionNeuf.CollisionInformation.LocalPosition = new Vector3(0f, 0.24f, 1.98f);
		AvionNeuf.Position = new Vector3(0f, 1.4f, 0f);
		particules.AutoInitialize(game.GraphicsDevice, game.Content, null);
		SmokePSmanager.AddParticleSystem(particules);
		particules2.AutoInitialize(game.GraphicsDevice, game.Content, null);
		SmokePSmanager.AddParticleSystem(particules2);
		particules3.AutoInitialize(game.GraphicsDevice, game.Content, null);
		SmokePSmanager.AddParticleSystem(particules3);
		particules4.AutoInitialize(game.GraphicsDevice, game.Content, null);
		SmokePSmanager.AddParticleSystem(particules4);
		explosion.AutoInitialize(game.GraphicsDevice, game.Content, null);
		SmokePSmanager.AddParticleSystem(explosion);
		Vehicle.Body.LinearVelocity = Vector3.Zero;
		Vehicle.Body.AngularVelocity = Vector3.Zero;
		Vehicle.Body.Orientation = new Quaternion(0f, 0f, 0f, 1f);
		wheelGraphicRotation = Matrix.CreateFromAxisAngle(Vector3.Forward, 1.5708f);
		Vehicle.AddWheel(new Wheel(new RaycastWheelShape(0.31f, wheelGraphicRotation), new WheelSuspension(30000f, 50f, Vector3.Down, 0.7f, new Vector3(0.84f, Ytrain, -0.62f)), new WheelDrivingMotor(0.1f, 30000f, 0f), new WheelBrake(0f, 0f, 0.04f), new WheelSlidingFriction(1f, 2f)));
		Vehicle.AddWheel(new Wheel(new RaycastWheelShape(0.31f, wheelGraphicRotation), new WheelSuspension(30000f, 50f, Vector3.Down, 0.7f, new Vector3(-0.84f, Ytrain, -0.62f)), new WheelDrivingMotor(0.1f, 30000f, 0f), new WheelBrake(0f, 0f, 0.04f), new WheelSlidingFriction(1f, 2f)));
		Vehicle.AddWheel(new Wheel(new RaycastWheelShape(0.31f, wheelGraphicRotation), new WheelSuspension(30000f, 50f, Vector3.Down, 0.7f, new Vector3(0.84f, Ytrain, -1.62f)), new WheelDrivingMotor(0.1f, 30000f, 0f), new WheelBrake(0f, 0f, 0.04f), new WheelSlidingFriction(1f, 2f)));
		Vehicle.AddWheel(new Wheel(new RaycastWheelShape(0.31f, wheelGraphicRotation), new WheelSuspension(30000f, 50f, Vector3.Down, 0.7f, new Vector3(-0.84f, Ytrain, -1.62f)), new WheelDrivingMotor(0.1f, 30000f, 0f), new WheelBrake(0f, 0f, 0.04f), new WheelSlidingFriction(1f, 2f)));
		Vehicle.AddWheel(new Wheel(new RaycastWheelShape(0.24f, wheelGraphicRotation), new WheelSuspension(50000f, 100f, Vector3.Down, 0.8f, new Vector3(-0.14f, YtrainA, ZtrainA)), new WheelDrivingMotor(0.1f, 30000f, 0f), new WheelBrake(0f, 0f, 0.033f), new WheelSlidingFriction(1f, 1f)));
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
		OffsetTransformhelice1 = Matrix.CreateTranslation(-helice.Position);
		OffsetTransformhelice2 = Matrix.CreateTranslation(-helice1.Position);
		OffsetTransformhelice3 = Matrix.CreateTranslation(-helice2.Position);
		OffsetTransformhelice4 = Matrix.CreateTranslation(-helice3.Position);
		soundBanka = game.soundBank;
	}

	public void Pause()
	{
		moteur.Stop(AudioStopOptions.Immediate);
	}

	private void piecesVu(CustomPhysicsGame game)
	{
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objtraindroit);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objtrappeDP);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objtrappeDG);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objtraingauche);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objtrappeGP);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objtrappeGG);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objtrainavant);
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
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objheliceRon1);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objheliceRon2);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objheliceRon3);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objheliceRon4);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objroue);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objroue1);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objroue2);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objroue3);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objroueA);
		if (Avioncasse)
		{
			game.terrain.sceneInterfaceScene.ObjectManager.Submit(objhelice1);
			game.terrain.sceneInterfaceScene.ObjectManager.Submit(objhelice2);
			game.terrain.sceneInterfaceScene.ObjectManager.Submit(objhelice3);
			game.terrain.sceneInterfaceScene.ObjectManager.Submit(objhelice4);
		}
	}

	private void Transneuf()
	{
		objroue.World = Vehicle.Wheels[0].Shape.WorldTransform;
		objroue1.World = Vehicle.Wheels[1].Shape.WorldTransform;
		objroue2.World = Vehicle.Wheels[2].Shape.WorldTransform;
		objroue3.World = Vehicle.Wheels[3].Shape.WorldTransform;
		objroueA.World = Vehicle.Wheels[4].Shape.WorldTransform;
		objfuselage.World = AvionNeuf.WorldTransform;
		objailesBD.World = AvionNeuf.WorldTransform;
		objailesBG.World = AvionNeuf.WorldTransform;
		objderive.World = AvionNeuf.WorldTransform;
		objderiveMob.World = Matrix.CreateRotationY(MathHelper.ToRadians(deriveRot)) * Positionderivemob * AvionNeuf.WorldTransform;
		objprofondeurD.World = AvionNeuf.WorldTransform;
		objprofondeurG.World = AvionNeuf.WorldTransform;
		objcables.World = AvionNeuf.WorldTransform;
		objprofondeurMobD.World = Matrix.CreateRotationX(MathHelper.ToRadians(profondeurRot)) * PositionprofondeurmobD * AvionNeuf.WorldTransform;
		objprofondeurMobG.World = Matrix.CreateRotationX(MathHelper.ToRadians(profondeurRot)) * PositionprofondeurmobG * AvionNeuf.WorldTransform;
		objheliceRon1.World = Matrix.CreateRotationZ(helicerotation) * PositionHelirot1 * AvionNeuf.WorldTransform;
		objheliceRon2.World = Matrix.CreateRotationZ(helicerotation) * PositionHelirot2 * AvionNeuf.WorldTransform;
		objheliceRon3.World = Matrix.CreateRotationZ(helicerotation) * PositionHelirot3 * AvionNeuf.WorldTransform;
		objheliceRon4.World = Matrix.CreateRotationZ(helicerotation) * PositionHelirot4 * AvionNeuf.WorldTransform;
		objaileronG.World = Matrix.CreateRotationX(MathHelper.ToRadians(aileronDRot)) * PositionaileronGmob * AvionNeuf.WorldTransform;
		objaileronD.World = Matrix.CreateRotationX(MathHelper.ToRadians(aileronGRot)) * PositionaileronDmob * AvionNeuf.WorldTransform;
		helice.WorldTransform = Matrix.CreateRotationZ(rot) * Positionhelice1 * AvionNeuf.WorldTransform;
		helice1.WorldTransform = Matrix.CreateRotationZ(rot) * Positionhelice2 * AvionNeuf.WorldTransform;
		helice2.WorldTransform = Matrix.CreateRotationZ(rot) * Positionhelice3 * AvionNeuf.WorldTransform;
		helice3.WorldTransform = Matrix.CreateRotationZ(rot) * Positionhelice4 * AvionNeuf.WorldTransform;
		objhelice1.World = OffsetTransformhelice1 * helice.WorldTransform;
		objhelice2.World = OffsetTransformhelice2 * helice1.WorldTransform;
		objhelice3.World = OffsetTransformhelice3 * helice2.WorldTransform;
		objhelice4.World = OffsetTransformhelice4 * helice3.WorldTransform;
		pot.WorldTransform = Positionpot * AvionNeuf.WorldTransform;
		pot2.WorldTransform = Positionpot2 * AvionNeuf.WorldTransform;
		pot3.WorldTransform = Positionpot3 * AvionNeuf.WorldTransform;
		pot4.WorldTransform = Positionpot4 * AvionNeuf.WorldTransform;
		objtraindroit.World = Matrix.CreateTranslation(0f, YT, 0f) * PositiontrainD * AvionNeuf.WorldTransform;
		objtrappeDP.World = Matrix.CreateRotationZ(MathHelper.ToRadians(0f - trapperotDP)) * PositiontrappeDP * AvionNeuf.WorldTransform;
		objtrappeDG.World = Matrix.CreateRotationZ(MathHelper.ToRadians(0f - trapperotDG)) * PositiontrappeDG * AvionNeuf.WorldTransform;
		objtraingauche.World = Matrix.CreateTranslation(0f, YT, 0f) * PositiontrainG * AvionNeuf.WorldTransform;
		objtrappeGP.World = Matrix.CreateRotationZ(MathHelper.ToRadians(trapperotGP)) * PositiontrappeGP * AvionNeuf.WorldTransform;
		objtrappeGG.World = Matrix.CreateRotationZ(MathHelper.ToRadians(trapperotGG)) * PositiontrappeGG * AvionNeuf.WorldTransform;
		objtrainavant.World = Matrix.CreateRotationX(MathHelper.ToRadians(trainRotA)) * PositiontrainA * AvionNeuf.WorldTransform;
		objtrape1g.World = Matrix.CreateTranslation(0f, trapeDesAA, trapeArrAA) * PositiontrapeD * AvionNeuf.WorldTransform;
		objtrape2g.World = Matrix.CreateTranslation(0f, trapeDesAR, trapeArrAR) * PositiontrapeG * AvionNeuf.WorldTransform;
		profondeurD.Position = AvionNeuf.Position;
		fuselage.Position = AvionNeuf.Position;
		aileBG.Position = AvionNeuf.Position;
		aileBD.Position = AvionNeuf.Position;
		derive.Position = AvionNeuf.Position;
		profondeurG.Position = AvionNeuf.Position;
		roue1.Position = AvionNeuf.Position;
		roue2.Position = AvionNeuf.Position;
		roue3.Position = AvionNeuf.Position;
		roue4.Position = AvionNeuf.Position;
		roueA.Position = AvionNeuf.Position;
		profondeurD.Orientation = AvionNeuf.Orientation;
		fuselage.Orientation = AvionNeuf.Orientation;
		aileBG.Orientation = AvionNeuf.Orientation;
		aileBD.Orientation = AvionNeuf.Orientation;
		derive.Orientation = AvionNeuf.Orientation;
		profondeurG.Orientation = AvionNeuf.Orientation;
		roue1.Orientation = AvionNeuf.Orientation;
		roue2.Orientation = AvionNeuf.Orientation;
		roue3.Orientation = AvionNeuf.Orientation;
		roue4.Orientation = AvionNeuf.Orientation;
		roueA.Orientation = AvionNeuf.Orientation;
	}

	private void Transcasse()
	{
		if (trainState == TrainState.sorti)
		{
			objroue.World = roue1.WorldTransform;
			objroue1.World = roue2.WorldTransform;
			objroue2.World = roue3.WorldTransform;
			objroue3.World = roue4.WorldTransform;
			objroueA.World = roueA.WorldTransform;
		}
		if (trainState == TrainState.rentre)
		{
			objroue.World = Vehicle.Wheels[0].Shape.WorldTransform * OffsetTransformfuselage * fuselage.WorldTransform;
			objroue1.World = Vehicle.Wheels[1].Shape.WorldTransform * OffsetTransformfuselage * fuselage.WorldTransform;
			objroue2.World = Vehicle.Wheels[2].Shape.WorldTransform * OffsetTransformfuselage * fuselage.WorldTransform;
			objroue3.World = Vehicle.Wheels[3].Shape.WorldTransform * OffsetTransformfuselage * fuselage.WorldTransform;
			objroueA.World = Vehicle.Wheels[4].Shape.WorldTransform * OffsetTransformfuselage * fuselage.WorldTransform;
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
		objhelice1.World = OffsetTransformhelice1 * helice.WorldTransform;
		objhelice2.World = OffsetTransformhelice2 * helice1.WorldTransform;
		objhelice3.World = OffsetTransformhelice3 * helice2.WorldTransform;
		objhelice4.World = OffsetTransformhelice4 * helice3.WorldTransform;
		objaileronD.World = PositionaileronDmob * OffsetTransformaileBD * aileBD.WorldTransform;
		objaileronG.World = PositionaileronGmob * OffsetTransformaileBG * aileBG.WorldTransform;
		objtraingauche.World = Matrix.CreateTranslation(0f, YT, 0f) * PositiontrainG * OffsetTransformfuselage * fuselage.WorldTransform;
		objtrappeGP.World = Matrix.CreateRotationZ(MathHelper.ToRadians(trapperotGP)) * PositiontrappeGP * OffsetTransformfuselage * fuselage.WorldTransform;
		objtrappeGG.World = Matrix.CreateRotationZ(MathHelper.ToRadians(trapperotGG)) * PositiontrappeGG * OffsetTransformfuselage * fuselage.WorldTransform;
		objtraindroit.World = Matrix.CreateTranslation(0f, YT, 0f) * PositiontrainD * OffsetTransformfuselage * fuselage.WorldTransform;
		objtrappeDP.World = Matrix.CreateRotationZ(MathHelper.ToRadians(0f - trapperotDP)) * PositiontrappeDP * OffsetTransformfuselage * fuselage.WorldTransform;
		objtrappeDG.World = Matrix.CreateRotationZ(MathHelper.ToRadians(0f - trapperotDG)) * PositiontrappeDG * OffsetTransformfuselage * fuselage.WorldTransform;
		objtrainavant.World = Matrix.CreateRotationX(MathHelper.ToRadians(trainRotA)) * PositiontrainA * OffsetTransformfuselage * fuselage.WorldTransform;
		objtrape1g.World = Matrix.CreateTranslation(0f, trapeDesAA, trapeArrAA) * PositiontrapeD * OffsetTransformfuselage * fuselage.WorldTransform;
		objtrape2g.World = Matrix.CreateTranslation(0f, trapeDesAR, trapeArrAR) * PositiontrapeG * OffsetTransformfuselage * fuselage.WorldTransform;
		objheliceRon1.World = Matrix.CreateTranslation(0f, -20f, 0f);
		objheliceRon2.World = Matrix.CreateTranslation(0f, -20f, 0f);
		objheliceRon3.World = Matrix.CreateTranslation(0f, -20f, 0f);
		objheliceRon4.World = Matrix.CreateTranslation(0f, -20f, 0f);
	}

	public void Moteur()
	{
		moteur = soundBanka.GetCue("ac");
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
		if ((sender != null && VitesseAvion >= 20f) || accelerationDE <= -3f)
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
				if (Ytrain >= -0.6f)
				{
					trapperotDP -= 0.5f;
					trapperotDG += 0.5f;
					trapperotGP -= 0.5f;
					trapperotGG += 0.5f;
				}
				if (trainRotA <= -70f)
				{
					trapeDesAA += 0.0005f;
					trapeDesAR += 0.0005f;
					trapeArrAA += 0.005f;
					trapeArrAR -= 0.005f;
				}
				trainRotA -= 0.6f;
				YtrainA += 0.0015f;
				ZtrainA += 0.0036f;
				Ytrain += 0.0057f;
				YT += 0.003f;
				Vehicle.Wheels[0].Suspension.LocalAttachmentPoint = new Vector3(0.84f, Ytrain, -1.1f);
				Vehicle.Wheels[1].Suspension.LocalAttachmentPoint = new Vector3(-0.84f, Ytrain, -1.1f);
				Vehicle.Wheels[2].Suspension.LocalAttachmentPoint = new Vector3(0.84f, Ytrain, -1.67f);
				Vehicle.Wheels[3].Suspension.LocalAttachmentPoint = new Vector3(-0.84f, Ytrain, -1.67f);
				Vehicle.Wheels[4].Suspension.LocalAttachmentPoint = new Vector3(-0f, YtrainA, ZtrainA);
				if (YtrainA >= -0.5f)
				{
					YtrainA = -0.5f;
				}
				if (ZtrainA >= -5.9f)
				{
					ZtrainA = -5.9f;
				}
				if (Ytrain >= -0.6f)
				{
					Ytrain = -0.6f;
				}
				if (YT >= 0.3f)
				{
					YT = 0.3f;
				}
				if (trapperotDP <= 0f)
				{
					trapperotDP = 0f;
				}
				if (trapperotDG >= 0f)
				{
					trapperotDG = 0f;
				}
				if (trapperotGP <= 0f)
				{
					trapperotGP = 0f;
				}
				if (trapperotGG >= 0f)
				{
					trapperotGG = 0f;
				}
				if (trainRotA <= -70f)
				{
					trainRotA = -70f;
				}
				if (trapeDesAA >= 0f)
				{
					trapeDesAA = 0f;
				}
				if (trapeDesAR >= 0f)
				{
					trapeDesAR = 0f;
				}
				if (trapeArrAA >= 0f)
				{
					trapeArrAA = 0f;
				}
				if (trapeArrAR <= 0f)
				{
					trapeArrAR = 0f;
				}
			}
			if (trainState == TrainState.sorti)
			{
				trapeDesAA -= 0.0005f;
				trapeDesAR -= 0.0005f;
				trapeArrAA -= 0.005f;
				trapeArrAR += 0.005f;
				trapperotDP += 0.5f;
				trapperotDG -= 0.5f;
				trapperotGP += 0.5f;
				trapperotGG -= 0.5f;
				if (trapeArrAA <= -0.3f)
				{
					trainRotA += 0.5f;
					YtrainA -= 0.0035f;
					ZtrainA -= 0.0022f;
				}
				if (trapperotDP >= 80f)
				{
					YT -= 0.003f;
					Ytrain -= 0.0055f;
				}
				Vehicle.Wheels[0].Suspension.LocalAttachmentPoint = new Vector3(0.84f, Ytrain, -1.1f);
				Vehicle.Wheels[1].Suspension.LocalAttachmentPoint = new Vector3(-0.84f, Ytrain, -1.1f);
				Vehicle.Wheels[2].Suspension.LocalAttachmentPoint = new Vector3(0.84f, Ytrain, -1.67f);
				Vehicle.Wheels[3].Suspension.LocalAttachmentPoint = new Vector3(-0.84f, Ytrain, -1.67f);
				Vehicle.Wheels[4].Suspension.LocalAttachmentPoint = new Vector3(-0f, YtrainA, ZtrainA);
				if (YtrainA <= -0.9f)
				{
					YtrainA = -0.9f;
				}
				if (ZtrainA <= -6.24f)
				{
					ZtrainA = -6.24f;
				}
				if (Ytrain <= -1.1f)
				{
					Ytrain = -1.1f;
				}
				if (YT <= 0f)
				{
					YT = 0f;
				}
				if (trapperotDP >= 80f)
				{
					trapperotDP = 80f;
				}
				if (trapperotDG <= -50f)
				{
					trapperotDG = -50f;
				}
				if (trapperotGP >= 80f)
				{
					trapperotGP = 80f;
				}
				if (trapperotGG <= -50f)
				{
					trapperotGG = -50f;
				}
				if (trainRotA >= 0f)
				{
					trainRotA = 0f;
				}
				if (trapeDesAA <= -0.01f)
				{
					trapeDesAA = -0.01f;
				}
				if (trapeDesAR <= -0.03f)
				{
					trapeDesAR = -0.03f;
				}
				if (trapeArrAA <= -0.3f)
				{
					trapeArrAA = -0.3f;
				}
				if (trapeArrAR >= 0.5f)
				{
					trapeArrAR = 0.5f;
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
				particules2.Emitter.PositionData.Position = pot2.Position;
				particules3.Emitter.PositionData.Position = pot3.Position;
				particules4.Emitter.PositionData.Position = pot4.Position;
				if (QuaFuel > 0f)
				{
					particules.Emitter.Enabled = true;
					particules2.Emitter.Enabled = true;
					particules3.Emitter.Enabled = true;
					particules4.Emitter.Enabled = true;
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
			particules2.vitesse = new Vector3(random.Next(-3, 3), random.Next(3, 3), random.Next(3, 3));
			particules2.nombrepar = acceleration / 46000f;
			particules2.Emitter.ParticlesPerSecond = acceleration / 100f;
			particules3.vitesse = new Vector3(random.Next(-3, 3), random.Next(3, 3), random.Next(3, 3));
			particules3.nombrepar = acceleration / 46000f;
			particules3.Emitter.ParticlesPerSecond = acceleration / 100f;
			particules4.vitesse = new Vector3(random.Next(-3, 3), random.Next(3, 3), random.Next(3, 3));
			particules4.nombrepar = acceleration / 46000f;
			particules4.Emitter.ParticlesPerSecond = acceleration / 100f;
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
			Vehicle.Body.AngularDamping = 0.9f;
			float num7 = VitesseAvion * 10f;
			Vector3 right = Vehicle.Body.OrientationMatrix.Right;
			if (num7 >= 320f)
			{
				num7 = 320f;
			}
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
			float num8 = VitesseAvion * 10f;
			if (num8 >= 120f)
			{
				num8 = 120f;
			}
			Vector3 up = Vehicle.Body.OrientationMatrix.Up;
			if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M1 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M2 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M6)
			{
				DroiteS = -up * num8 * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
				deriveRot += 28f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
				if (altitude <= 1.8f)
				{
					DroiteS = -up * VitesseAvion * 80f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
					if (VitesseAvion <= 7f && VitesseAvion >= 0.2f)
					{
						DroiteS = -up * 500f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
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
					DroiteS = -up * VitesseAvion * 80f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.X;
					if (VitesseAvion <= 7f && VitesseAvion >= 0.2f)
					{
						DroiteS = -up * 500f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.X;
					}
				}
				Vehicle.Body.ApplyAngularImpulse(ref DroiteS);
			}
			aileronGRot = 0f;
			aileronDRot = 0f;
			float num9 = VitesseAvion * 12f;
			if (num9 >= 350f)
			{
				num9 = 320f;
				Vehicle.Body.LinearDamping = Angle2 / 75f;
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
			if (num10 >= 35f)
			{
				num10 = 35f;
			}
			if (num10 <= -35f)
			{
				num10 = -35f;
			}
			DroiteP = up * num10;
			Vehicle.Body.ApplyAngularImpulse(ref DroiteP);
			sphere.Position = Vehicle.Body.Position;
			Vector3 impulse = Monte / 2.5f;
			if (VitesseAvion >= 30f && num6 >= 5f && num6 <= 14f && altitude >= 5.75f && Angle2 <= 8.5f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse);
			}
			Vector3 impulse2 = -right * 100f;
			if (VitesseAvion <= 21f && altitude <= 3f && Angle2 >= 10.6f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse2);
			}
			Vector3 impulse3 = right * 250f;
			if (VitesseAvion <= 2f && altitude <= 3f && trainState == TrainState.rentre)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse3);
			}
			Vector3 impulse4 = -right * 3400f / Angle2;
			Vector3 impulse5 = right * 3400f / Angle2;
			Vector3 impulse6 = -up * 5600f / Angle2;
			Vector3 impulse7 = up * 5600f / Angle2;
			Vector3 impulse8 = -up * 1000f / Angle2;
			Vector3 impulse9 = up * 1000f / Angle2;
			Vector3 impulse10 = -right * 300f / Angle2;
			Vector3 impulse11 = right * 1300f / Angle2;
			if (num4 >= 0f && Angle2 >= 8.5f && num6 >= 4f && num6 <= 15f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse11);
			}
			if (Angle2 >= 10f && num6 >= 4f && num6 <= 15f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse10);
			}
			if (VitesseAvion <= 21f && altitude >= 3f)
			{
				if (num4 <= 0f && Angle2 >= 11f && num6 >= 4f && num6 <= 15f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse4);
				}
				if (num4 >= 0f && Angle2 >= 11f && num6 >= 4f && num6 <= 15f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse5);
				}
				if (num3 <= -0.01f && Angle2 >= 11f && VitesseAvion <= 1f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse6);
				}
				if (num3 >= 0.01f && Angle2 >= 11f && VitesseAvion <= 1f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse7);
				}
				if (num4 <= 0f && num6 >= 4f && num6 <= 15f && Angle2 <= 11f && Angle2 >= 5f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse4);
				}
				if (num4 >= 0f && num6 >= 4f && num6 <= 15f && Angle2 <= 11f && Angle2 >= 5f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse5);
				}
				if (num6 <= 4f && Angle2 <= 11f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse7);
				}
				if (num6 >= 15f && Angle2 <= 11f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse6);
				}
			}
			if (VitesseAvion <= 30f && altitude >= 2f)
			{
				if (num6 <= 5f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse9);
				}
				if (num6 >= 16f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse8);
				}
			}
			vitesseacceleration = accelerationDE + acceleration * dt;
			Vector3 impulse12 = vitesseacceleration * vector2;
			if (vitesseacceleration <= vitesseMini)
			{
				impulse12 = VitesseAvion * vector2;
			}
			if (VitesseAvion >= vitesseMaxiA)
			{
				impulse12 = 0f * vector2;
			}
			if (QuaFuel <= 0f)
			{
				particules.Emitter.Enabled = false;
				particules2.Emitter.Enabled = false;
				particules3.Emitter.Enabled = false;
				particules4.Emitter.Enabled = false;
				impulse12 = VitesseAvion * vector2;
				moteur.Stop(AudioStopOptions.Immediate);
				game.terrain.sceneInterfaceScene.ObjectManager.Remove(objheliceRon1);
				game.terrain.sceneInterfaceScene.ObjectManager.Remove(objheliceRon2);
				game.terrain.sceneInterfaceScene.ObjectManager.Remove(objheliceRon3);
				game.terrain.sceneInterfaceScene.ObjectManager.Remove(objheliceRon4);
				game.terrain.sceneInterfaceScene.ObjectManager.Submit(objhelice1);
				game.terrain.sceneInterfaceScene.ObjectManager.Submit(objhelice2);
				game.terrain.sceneInterfaceScene.ObjectManager.Submit(objhelice3);
				game.terrain.sceneInterfaceScene.ObjectManager.Submit(objhelice4);
			}
			Vehicle.Body.ApplyLinearImpulse(ref impulse12);
			if (compteCrash >= 99)
			{
				compteCrash = 99;
			}
			if (Avioncasse)
			{
				game.terrain.sceneInterfaceScene.ObjectManager.Submit(objhelice1);
				game.terrain.sceneInterfaceScene.ObjectManager.Submit(objhelice2);
				game.terrain.sceneInterfaceScene.ObjectManager.Submit(objhelice3);
				game.terrain.sceneInterfaceScene.ObjectManager.Submit(objhelice4);
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
			Vehicle.Body.LinearDamping = Angle2 / 86f;
			if (trainState == TrainState.sorti)
			{
				Vehicle.Body.LinearDamping = Angle2 / 80f;
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
			if (num6 <= 4f || num6 >= 15f)
			{
				accelerationDE -= 0.3f;
			}
			else
			{
				accelerationDE += 0.1f;
			}
			if (accelerationDE <= -200f)
			{
				accelerationDE = -200f;
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
			if (tempcrash >= 30f)
			{
				owningSpace.Remove(fuselage);
				owningSpace.Remove(aileBG);
				owningSpace.Remove(aileBD);
				owningSpace.Remove(derive);
				owningSpace.Remove(profondeurD);
				owningSpace.Remove(profondeurG);
				owningSpace.Remove(helice);
				owningSpace.Remove(helice1);
				owningSpace.Remove(helice2);
				owningSpace.Remove(helice3);
				owningSpace.Remove(roue1);
				owningSpace.Remove(roue2);
				owningSpace.Remove(roue3);
				owningSpace.Remove(roue4);
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
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objtrappeDP);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objtrappeDG);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objtraingauche);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objtrappeGP);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objtrappeGG);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objtrainavant);
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
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objroue2);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objroue3);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objroueA);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objaileronG);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objaileronD);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objheliceRon1);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objheliceRon2);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objheliceRon3);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objheliceRon4);
			if (objhelice1 != null)
			{
				game.terrain.sceneInterfaceScene.ObjectManager.Remove(objhelice1);
			}
			if (objhelice2 != null)
			{
				game.terrain.sceneInterfaceScene.ObjectManager.Remove(objhelice2);
			}
			if (objhelice3 != null)
			{
				game.terrain.sceneInterfaceScene.ObjectManager.Remove(objhelice3);
			}
			if (objhelice4 != null)
			{
				game.terrain.sceneInterfaceScene.ObjectManager.Remove(objhelice4);
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
			owningSpace.Remove(helice1);
			owningSpace.Remove(helice2);
			owningSpace.Remove(helice3);
			owningSpace.Remove(roue1);
			owningSpace.Remove(roue2);
			owningSpace.Remove(roue3);
			owningSpace.Remove(roue4);
			owningSpace.Remove(roueA);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objhelice1);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objhelice2);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objhelice3);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objhelice4);
		}
	}

	public void Restart(CustomPhysicsGame game)
	{
		if (objhelice1 != null)
		{
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objhelice1);
		}
		if (objheliceRon1 == null)
		{
			game.terrain.sceneInterfaceScene.ObjectManager.Submit(objheliceRon1);
		}
		if (objhelice2 != null)
		{
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objhelice2);
		}
		if (objheliceRon2 == null)
		{
			game.terrain.sceneInterfaceScene.ObjectManager.Submit(objheliceRon2);
		}
		if (objhelice3 != null)
		{
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objhelice3);
		}
		if (objheliceRon3 == null)
		{
			game.terrain.sceneInterfaceScene.ObjectManager.Submit(objheliceRon3);
		}
		if (objhelice4 != null)
		{
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objhelice4);
		}
		if (objheliceRon4 == null)
		{
			game.terrain.sceneInterfaceScene.ObjectManager.Submit(objheliceRon4);
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
		JaugeBoutton = 128f;
		pitch = -11.8f;
		game.camera.fov = 30f;
		Vehicle.Body.LinearVelocity = Vector3.Zero;
		Vehicle.Body.AngularVelocity = Vector3.Zero;
		Vehicle.Body.Orientation = new Quaternion(0.1f, 0f, 0f, 1f);
		AvionNeuf.Position = new Vector3(0f, 1.4f, 0f);
		particules.Emitter.Enabled = true;
		particules2.Emitter.Enabled = true;
		particules3.Emitter.Enabled = true;
		particules4.Emitter.Enabled = true;
		Moteur();
		trapperotDP = 80f;
		trapperotDG = -50f;
		trapperotGP = 80f;
		trapperotGG = -50f;
		trainRotA = 0f;
		trapeArrAA = -0.3f;
		trapeArrAR = 0.5f;
		trapeDesAA = -0.01f;
		trapeDesAR = -0.03f;
		Ytrain = -1.1f;
		ZtrainA = -6.24f;
		YtrainA = -0.9f;
		YT = 0f;
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
			particules2.Emitter.Enabled = false;
			particules3.Emitter.Enabled = false;
			particules4.Emitter.Enabled = false;
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
			owningSpace.Add(helice1);
			owningSpace.Add(helice2);
			owningSpace.Add(helice3);
			owningSpace.Add(roue1);
			owningSpace.Add(roue2);
			owningSpace.Add(roue3);
			owningSpace.Add(roue4);
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
			helice.AngularVelocity = AvionNeuf.AngularVelocity / 3f + new Vector3(100f, 0f, 0f);
			helice1.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			helice1.AngularVelocity = AvionNeuf.AngularVelocity / 3f + new Vector3(100f, 0f, 0f);
			helice2.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			helice2.AngularVelocity = AvionNeuf.AngularVelocity / 3f + new Vector3(100f, 0f, 0f);
			helice3.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			helice3.AngularVelocity = AvionNeuf.AngularVelocity / 3f + new Vector3(100f, 0f, 0f);
			roue1.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			roue1.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			roue2.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			roue2.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			roue3.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			roue3.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			roue4.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			roue4.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			roueA.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			roueA.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			Crash();
			moteur.Stop(AudioStopOptions.Immediate);
		}
	}
}
