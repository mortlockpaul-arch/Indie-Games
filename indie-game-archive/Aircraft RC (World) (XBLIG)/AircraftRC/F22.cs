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

public class F22
{
	public enum TrainState
	{
		sorti,
		rentre
	}

	private SmokeParticleSystem particules;

	private SmokeParticleSystem particules1;

	private ExplosionFireSmokeParticleSystem explosion;

	private ParticleSystemManager SmokePSmanager = new ParticleSystemManager();

	private FireParticleSystem particules2;

	private FireParticleSystem particules3;

	public Cue reacteur;

	public Cue reacteur2;

	private Cue crash;

	private Cue REST;

	private SoundBank soundBanka;

	private AudioEmitter emitteravion = new AudioEmitter();

	private AudioListener listeneravion = new AudioListener();

	private SceneObject objfuselage;

	private SceneObject objailesD;

	private SceneObject objailesG;

	private SceneObject objderiveD;

	private SceneObject objderiveG;

	private SceneObject objderiveMobD;

	private SceneObject objderiveMobG;

	private SceneObject objprofondeurD;

	private SceneObject objprofondeurG;

	private SceneObject objroue;

	private SceneObject objroue1;

	private SceneObject objroueA;

	private SceneObject objaileronG;

	private SceneObject objaileronD;

	private SceneObject objtraindroit;

	private SceneObject objtraingauche;

	private SceneObject objtrainavant;

	private SceneObject objtrapegauche;

	private SceneObject objtrapedroite;

	private SceneObject objtrapeAG;

	private SceneObject objtrapeAD;

	private SceneObject objtubes;

	private List<CompoundShapeEntry> avion = new List<CompoundShapeEntry>();

	private Entity<CompoundCollidable> AvionNeuf;

	private ConvexHullShape fuselageshape;

	private ConvexHullShape ailesBGshape;

	private ConvexHullShape ailesBDshape;

	private ConvexHullShape deriveDshape;

	private ConvexHullShape deriveGshape;

	private ConvexHullShape profondeurDshape;

	private ConvexHullShape profondeurGshape;

	private ConvexHull fuselage;

	private ConvexHull aileBG;

	private ConvexHull aileBD;

	private ConvexHull deriveD;

	private ConvexHull deriveG;

	private ConvexHull profondeurD;

	private ConvexHull profondeurG;

	public Vehicle Vehicle;

	private Entity sphere;

	private Entity pot;

	private Entity pot1;

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

	private float VitesseAV;

	private float vitesseacceleration;

	private float vitesseMaxi = 25000f;

	private float vitesseMini;

	private float vitesseMaxiA = 165f;

	public float accelerationDE;

	private float acceleration;

	public float Angle;

	public float Angle2;

	public float Angle3;

	public float AngleR;

	private float sensibilite = 705f;

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

	private float deriveRot;

	private float profondeurRot;

	private float aileronGRot;

	private float aileronDRot;

	private float trainDRot;

	private float trainGRot;

	private float trainARot;

	private float trapeDRot = 86f;

	private float trapeGRot = -86f;

	private float trapeADRot = 90f;

	private float trapeAGRot = -90f;

	private float XtrainG = 1.49f;

	private float XtrainD = -1.56f;

	public float Ytrain = -0.9f;

	private float YtrainA = -0.78f;

	private float ZtrainA = -6.58f;

	private float angleroue = 1.5708f;

	private float angleroue1 = 1.5708f;

	public TrainState trainState;

	public float altitude;

	private Vector3 Monte;

	private Vector3 DroiteS;

	private Vector3 Droite;

	private Vector3 DroiteP;

	private Model fuselageModel;

	private Model aileGModel;

	private Model aileDModel;

	private Model deriveDModel;

	private Model deriveGModel;

	private Model profondeurDModel;

	private Model profondeurGModel;

	private Model wheelModel;

	private Model roueAModel;

	private Model tubesModel;

	private Model avionpiecesModel;

	private Model trainsModel;

	private Model fuselageModelh;

	private Model aileDModelh;

	private Model aileGModelh;

	private Model deriveDModelh;

	private Model deriveGModelh;

	private Model profondeurDModelh;

	private Model profondeurGModelh;

	private Cylinder roue1;

	private Cylinder roue2;

	private Cylinder roueA;

	private Matrix wheelGraphicRotation;

	private Matrix OffsetTransformfuselage = Matrix.Identity;

	private Matrix OffsetTransformaileBG = Matrix.Identity;

	private Matrix OffsetTransformaileBD = Matrix.Identity;

	private Matrix OffsetTransformderiveD = Matrix.Identity;

	private Matrix OffsetTransformderiveG = Matrix.Identity;

	private Matrix OffsetTransformprofondeurD = Matrix.Identity;

	private Matrix OffsetTransformprofondeurG = Matrix.Identity;

	private Matrix OffsetTransformtubes = Matrix.Identity;

	private Matrix PositionderiveGmob = Matrix.CreateTranslation(-1.86f, -1.11f, 5f) * Matrix.CreateRotationZ(0.45f) * Matrix.CreateRotationX(-0.41f);

	private Matrix PositionderiveDmob = Matrix.CreateTranslation(1.82f, -1.15f, 5f) * Matrix.CreateRotationZ(-0.43f) * Matrix.CreateRotationX(-0.4f);

	private Matrix PositionprofondeurmobG = Matrix.CreateTranslation(0.1f, 0.05f, 5.5f);

	private Matrix PositionprofondeurmobD = Matrix.CreateTranslation(-0.1f, 0.05f, 5.5f);

	private Matrix PositionaileronGmob = Matrix.CreateTranslation(2.428f, 0.5f, 4.74f) * Matrix.CreateRotationY(0.31f) * Matrix.CreateRotationZ(-0.085f);

	private Matrix PositionaileronDmob = Matrix.CreateTranslation(-2.428f, 0.5f, 4.74f) * Matrix.CreateRotationY(-0.31f) * Matrix.CreateRotationZ(0.085f);

	private Matrix Positiontubes = Matrix.CreateTranslation(0f, 0f, 4.6f);

	private Matrix Positionpot = Matrix.CreateTranslation(-0.5f, 0.04f, 5.55f);

	private Matrix Positionpot1 = Matrix.CreateTranslation(0.5f, 0.04f, 5.5f);

	private Matrix PositiontrainD = Matrix.CreateTranslation(0.948f, -0.505f, 0f) * Matrix.CreateRotationX(-0f);

	private Matrix PositiontrainG = Matrix.CreateTranslation(-0.949f, -0.505f, 0f) * Matrix.CreateRotationX(-0f);

	private Matrix PositiontrainA = Matrix.CreateTranslation(-0f, -0.3f, -4.25f);

	private Matrix PositiontrapeD = Matrix.CreateTranslation(1.9f, 0.155f, -0f) * Matrix.CreateRotationX(-0f);

	private Matrix PositiontrapeG = Matrix.CreateTranslation(-1.91f, 0.155f, -0f) * Matrix.CreateRotationX(-0f);

	private Matrix PositiontrapeAD = Matrix.CreateTranslation(0.226f, -0.4125f, -0f) * Matrix.CreateRotationX(-0f);

	private Matrix PositiontrapeAG = Matrix.CreateTranslation(-0.248f, -0.4125f, -0f) * Matrix.CreateRotationX(-0f);

	private ModelMesh derivemobD;

	private ModelMesh derivemobG;

	private ModelMesh profondeurmob;

	private ModelMesh aileronGmob;

	private ModelMesh aileronDmob;

	private ModelMesh trainDroit;

	private ModelMesh trainGauche;

	private ModelMesh trainAvant;

	private ModelMesh trapeD;

	private ModelMesh trapeG;

	private ModelMesh trapeAD;

	private ModelMesh trapeAG;

	public F22(CustomPhysicsGame game)
	{
		particules = new SmokeParticleSystem(game);
		particules1 = new SmokeParticleSystem(game);
		explosion = new ExplosionFireSmokeParticleSystem(game);
		particules2 = new FireParticleSystem(game);
		particules3 = new FireParticleSystem(game);
		inputStateConfig = new ManetteConfig(game);
	}

	public void Load(CustomPhysicsGame game)
	{
		fuselageModelh = game.Content.Load<Model>("Models/f22/HullF");
		aileDModelh = game.Content.Load<Model>("Models/f22/HullAD");
		aileGModelh = game.Content.Load<Model>("Models/f22/HullAG");
		deriveDModelh = game.Content.Load<Model>("Models/f22/HullDD");
		deriveGModelh = game.Content.Load<Model>("Models/f22/HullDG");
		profondeurDModelh = game.Content.Load<Model>("Models/f22/HullPD");
		profondeurGModelh = game.Content.Load<Model>("Models/f22/HullPG");
		avionpiecesModel = game.Content.Load<Model>("Models/f22/22Pieces");
		trainsModel = game.Content.Load<Model>("Models/f22/22Train");
		fuselageModel = game.Content.Load<Model>("Models/f22/22fuse");
		aileDModel = game.Content.Load<Model>("Models/f22/22aileD");
		aileGModel = game.Content.Load<Model>("Models/f22/22aileG");
		deriveDModel = game.Content.Load<Model>("Models/f22/22DeriD");
		deriveGModel = game.Content.Load<Model>("Models/f22/22DeriG");
		profondeurDModel = game.Content.Load<Model>("Models/f22/22ProfondeurD");
		profondeurGModel = game.Content.Load<Model>("Models/f22/22ProfondeurG");
		tubesModel = game.Content.Load<Model>("Models/f22/22Tubes");
		wheelModel = game.Content.Load<Model>("Models/f22/22RoueArr");
		roueAModel = game.Content.Load<Model>("Models/f22/22RoueAV");
		derivemobD = avionpiecesModel.Meshes["DeriMobD"];
		derivemobG = avionpiecesModel.Meshes["DeriMobG"];
		aileronGmob = avionpiecesModel.Meshes["aileronD"];
		aileronDmob = avionpiecesModel.Meshes["aileronG"];
		trainDroit = trainsModel.Meshes["trainD"];
		trainGauche = trainsModel.Meshes["trainG"];
		trainAvant = trainsModel.Meshes["TrainAvant"];
		trapeD = trainsModel.Meshes["TrapeD"];
		trapeG = trainsModel.Meshes["TrapeG"];
		trapeAD = trainsModel.Meshes["TrapeAD"];
		trapeAG = trainsModel.Meshes["TrapeAG"];
		objtraindroit = new SceneObject(trainDroit);
		objtraingauche = new SceneObject(trainGauche);
		objtrainavant = new SceneObject(trainAvant);
		objtrapegauche = new SceneObject(trapeG);
		objtrapedroite = new SceneObject(trapeD);
		objtrapeAG = new SceneObject(trapeAG);
		objtrapeAD = new SceneObject(trapeAD);
		objfuselage = new SceneObject(fuselageModel);
		objailesD = new SceneObject(aileDModel);
		objailesG = new SceneObject(aileGModel);
		objderiveD = new SceneObject(deriveDModel);
		objderiveG = new SceneObject(deriveGModel);
		objderiveMobD = new SceneObject(derivemobD);
		objderiveMobG = new SceneObject(derivemobG);
		objprofondeurD = new SceneObject(profondeurDModel);
		objprofondeurG = new SceneObject(profondeurGModel);
		objroue = new SceneObject(wheelModel);
		objroue1 = new SceneObject(wheelModel);
		objroueA = new SceneObject(roueAModel);
		objaileronG = new SceneObject(aileronGmob);
		objaileronD = new SceneObject(aileronDmob);
		objtubes = new SceneObject(tubesModel);
		TriangleMesh.GetVerticesAndIndicesFromModel(fuselageModelh, out var vertices, out var indices);
		fuselageshape = new ConvexHullShape(vertices, out var center);
		fuselage = new ConvexHull(vertices, 200f);
		TriangleMesh.GetVerticesAndIndicesFromModel(aileDModelh, out vertices, out indices);
		ailesBGshape = new ConvexHullShape(vertices, out center);
		aileBG = new ConvexHull(vertices, 100f);
		TriangleMesh.GetVerticesAndIndicesFromModel(aileGModelh, out vertices, out indices);
		ailesBDshape = new ConvexHullShape(vertices, out center);
		aileBD = new ConvexHull(vertices, 100f);
		TriangleMesh.GetVerticesAndIndicesFromModel(deriveDModelh, out vertices, out indices);
		deriveDshape = new ConvexHullShape(vertices, out center);
		deriveD = new ConvexHull(vertices, 50f);
		TriangleMesh.GetVerticesAndIndicesFromModel(deriveGModelh, out vertices, out indices);
		deriveGshape = new ConvexHullShape(vertices, out center);
		deriveG = new ConvexHull(vertices, 50f);
		TriangleMesh.GetVerticesAndIndicesFromModel(profondeurDModelh, out vertices, out indices);
		profondeurDshape = new ConvexHullShape(vertices, out center);
		profondeurD = new ConvexHull(vertices, 30f);
		TriangleMesh.GetVerticesAndIndicesFromModel(profondeurGModelh, out vertices, out indices);
		profondeurGshape = new ConvexHullShape(vertices, out center);
		profondeurG = new ConvexHull(vertices, 30f);
		roue1 = new Cylinder(new Vector3(0f, 0f, 0f), 0.2f, 0.4f, 4f);
		roue2 = new Cylinder(new Vector3(0f, 0f, 0f), 0.2f, 0.4f, 4f);
		roueA = new Cylinder(new Vector3(0f, 0f, 0f), 0.1f, 0.26f, 3.5f);
		avion.Add(new CompoundShapeEntry(fuselageshape, new Vector3(0f, 0f, 0f), 250f));
		avion.Add(new CompoundShapeEntry(ailesBGshape, new Vector3(-2.72f, 0.08f, 2.1f), 120f));
		avion.Add(new CompoundShapeEntry(ailesBDshape, new Vector3(2.72f, 0.18f, 2.28f), 120f));
		avion.Add(new CompoundShapeEntry(deriveDshape, new Vector3(-1.6f, 1f, 3.8f), 50f));
		avion.Add(new CompoundShapeEntry(deriveGshape, new Vector3(1.6f, 1f, 4f), 50f));
		avion.Add(new CompoundShapeEntry(profondeurDshape, new Vector3(-2.1f, 0.1f, 5.8f), 15f));
		avion.Add(new CompoundShapeEntry(profondeurGshape, new Vector3(2.1f, 0.1f, 5.8f), 15f));
		AvionNeuf = new CompoundBody(avion, 620f);
		Vehicle = new Vehicle(AvionNeuf);
		owningSpace = game.space;
		AvionNeuf.CollisionInformation.Events.DetectingInitialCollision += HandleCollision;
		owningSpace.Add(Vehicle);
		sphere = new Sphere(new Vector3(0f, 0f, 0f), 0.3f);
		pot = new Sphere(new Vector3(0f, 0f, 0f), 0.1f);
		pot1 = new Sphere(new Vector3(0f, 0f, 0f), 0.1f);
		AvionNeuf.CollisionInformation.LocalPosition = new Vector3(0f, 0.35f, 2.3f);
		AvionNeuf.Position = new Vector3(0f, 1.5f, 0f);
		particules.AutoInitialize(game.GraphicsDevice, game.Content, null);
		SmokePSmanager.AddParticleSystem(particules);
		particules1.AutoInitialize(game.GraphicsDevice, game.Content, null);
		SmokePSmanager.AddParticleSystem(particules1);
		explosion.AutoInitialize(game.GraphicsDevice, game.Content, null);
		SmokePSmanager.AddParticleSystem(explosion);
		particules2.AutoInitialize(game.GraphicsDevice, game.Content, null);
		SmokePSmanager.AddParticleSystem(particules2);
		particules3.AutoInitialize(game.GraphicsDevice, game.Content, null);
		SmokePSmanager.AddParticleSystem(particules3);
		Vehicle.Body.LinearVelocity = Vector3.Zero;
		Vehicle.Body.AngularVelocity = Vector3.Zero;
		Vehicle.Body.Orientation = new Quaternion(0f, 0f, 0f, 1f);
		wheelGraphicRotation = Matrix.CreateFromAxisAngle(Vector3.Forward, 1.5708f);
		Vehicle.AddWheel(new Wheel(new RaycastWheelShape(0.41f, wheelGraphicRotation), new WheelSuspension(40000f, 50f, Vector3.Down, 0.7f, new Vector3(XtrainD, Ytrain, -0.5f)), new WheelDrivingMotor(0.1f, 30000f, 0f), new WheelBrake(0f, 0f, 0.2f), new WheelSlidingFriction(1f, 2f)));
		Vehicle.AddWheel(new Wheel(new RaycastWheelShape(0.41f, wheelGraphicRotation), new WheelSuspension(40000f, 50f, Vector3.Down, 0.7f, new Vector3(XtrainG, Ytrain, -0.5f)), new WheelDrivingMotor(0.1f, 30000f, 0f), new WheelBrake(0f, 0f, 0.2f), new WheelSlidingFriction(1f, 2f)));
		Vehicle.AddWheel(new Wheel(new RaycastWheelShape(0.26f, wheelGraphicRotation), new WheelSuspension(50000f, 100f, Vector3.Down, 0.8f, new Vector3(-0.01f, YtrainA, ZtrainA)), new WheelDrivingMotor(0.1f, 30000f, 0f), new WheelBrake(0f, 0f, 0.16f), new WheelSlidingFriction(1f, 1f)));
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
		OffsetTransformderiveD = Matrix.CreateTranslation(-deriveD.Position);
		OffsetTransformderiveG = Matrix.CreateTranslation(-deriveG.Position);
		OffsetTransformprofondeurG = Matrix.CreateTranslation(-profondeurG.Position);
		OffsetTransformprofondeurD = Matrix.CreateTranslation(-profondeurD.Position);
		soundBanka = game.soundBank;
	}

	public void Pause()
	{
		reacteur.Stop(AudioStopOptions.Immediate);
		reacteur2.Stop(AudioStopOptions.Immediate);
	}

	private void piecesVu(CustomPhysicsGame game)
	{
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objtraindroit);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objtraingauche);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objtrainavant);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objtrapegauche);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objtrapedroite);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objtrapeAG);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objtrapeAD);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objfuselage);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objailesD);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objailesG);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objderiveD);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objderiveG);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objderiveMobD);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objderiveMobG);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objprofondeurD);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objprofondeurG);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objtubes);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objroueA);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objaileronG);
		game.terrain.sceneInterfaceScene.ObjectManager.Submit(objaileronD);
	}

	private void Transneuf()
	{
		objroue.World = Vehicle.Wheels[0].Shape.WorldTransform;
		objroue1.World = Vehicle.Wheels[1].Shape.WorldTransform;
		objroueA.World = Vehicle.Wheels[2].Shape.WorldTransform;
		objfuselage.World = AvionNeuf.WorldTransform;
		objailesD.World = AvionNeuf.WorldTransform;
		objailesG.World = AvionNeuf.WorldTransform;
		objderiveD.World = AvionNeuf.WorldTransform;
		objderiveG.World = AvionNeuf.WorldTransform;
		objderiveMobD.World = Matrix.CreateRotationY(MathHelper.ToRadians(deriveRot)) * PositionderiveDmob * AvionNeuf.WorldTransform;
		objderiveMobG.World = Matrix.CreateRotationY(MathHelper.ToRadians(deriveRot)) * PositionderiveGmob * AvionNeuf.WorldTransform;
		objprofondeurD.World = Matrix.CreateRotationX(MathHelper.ToRadians(profondeurRot)) * PositionprofondeurmobD * AvionNeuf.WorldTransform;
		objprofondeurG.World = Matrix.CreateRotationX(MathHelper.ToRadians(profondeurRot)) * PositionprofondeurmobG * AvionNeuf.WorldTransform;
		objtubes.World = Matrix.CreateRotationX(MathHelper.ToRadians(profondeurRot / 10f)) * Positiontubes * AvionNeuf.WorldTransform;
		objaileronG.World = Matrix.CreateRotationX(MathHelper.ToRadians(aileronDRot)) * PositionaileronGmob * AvionNeuf.WorldTransform;
		objaileronD.World = Matrix.CreateRotationX(MathHelper.ToRadians(aileronGRot)) * PositionaileronDmob * AvionNeuf.WorldTransform;
		objtraindroit.World = Matrix.CreateRotationZ(MathHelper.ToRadians(trainDRot)) * PositiontrainD * AvionNeuf.WorldTransform;
		objtraingauche.World = Matrix.CreateRotationZ(MathHelper.ToRadians(trainGRot)) * PositiontrainG * AvionNeuf.WorldTransform;
		objtrainavant.World = Matrix.CreateRotationX(MathHelper.ToRadians(trainARot)) * PositiontrainA * AvionNeuf.WorldTransform;
		objtrapegauche.World = Matrix.CreateRotationZ(MathHelper.ToRadians(trapeGRot)) * PositiontrapeG * AvionNeuf.WorldTransform;
		objtrapedroite.World = Matrix.CreateRotationZ(MathHelper.ToRadians(trapeDRot)) * PositiontrapeD * AvionNeuf.WorldTransform;
		objtrapeAG.World = Matrix.CreateRotationZ(MathHelper.ToRadians(trapeAGRot)) * PositiontrapeAG * AvionNeuf.WorldTransform;
		objtrapeAD.World = Matrix.CreateRotationZ(MathHelper.ToRadians(trapeADRot)) * PositiontrapeAD * AvionNeuf.WorldTransform;
		pot.WorldTransform = Positionpot * AvionNeuf.WorldTransform;
		pot1.WorldTransform = Positionpot1 * AvionNeuf.WorldTransform;
		fuselage.Position = AvionNeuf.Position;
		aileBG.Position = AvionNeuf.Position;
		aileBD.Position = AvionNeuf.Position;
		deriveD.Position = AvionNeuf.Position;
		deriveG.Position = AvionNeuf.Position;
		profondeurD.Position = AvionNeuf.Position;
		profondeurG.Position = AvionNeuf.Position;
		roue1.Position = AvionNeuf.Position;
		roue2.Position = AvionNeuf.Position;
		roueA.Position = AvionNeuf.Position;
		fuselage.Orientation = AvionNeuf.Orientation;
		aileBG.Orientation = AvionNeuf.Orientation;
		aileBD.Orientation = AvionNeuf.Orientation;
		deriveD.Orientation = AvionNeuf.Orientation;
		deriveG.Orientation = AvionNeuf.Orientation;
		profondeurD.Orientation = AvionNeuf.Orientation;
		profondeurG.Orientation = AvionNeuf.Orientation;
		roue1.Orientation = AvionNeuf.Orientation;
		roue2.Orientation = AvionNeuf.Orientation;
		roueA.Orientation = AvionNeuf.Orientation;
	}

	private void Transcasse()
	{
		if (Ytrain <= 0.1f)
		{
			objroue.World = roue1.WorldTransform;
			objroue1.World = roue2.WorldTransform;
			objroueA.World = roueA.WorldTransform;
		}
		if (Ytrain >= 0.12f)
		{
			objroue.World = Vehicle.Wheels[0].Shape.WorldTransform * OffsetTransformfuselage * fuselage.WorldTransform;
			objroue1.World = Vehicle.Wheels[1].Shape.WorldTransform * OffsetTransformfuselage * fuselage.WorldTransform;
			objroueA.World = Vehicle.Wheels[2].Shape.WorldTransform * OffsetTransformfuselage * fuselage.WorldTransform;
		}
		objfuselage.World = OffsetTransformfuselage * fuselage.WorldTransform;
		objailesG.World = OffsetTransformaileBD * aileBD.WorldTransform;
		objailesD.World = OffsetTransformaileBG * aileBG.WorldTransform;
		objderiveD.World = OffsetTransformderiveD * deriveD.WorldTransform;
		objderiveG.World = OffsetTransformderiveG * deriveG.WorldTransform;
		objderiveMobD.World = PositionderiveDmob * OffsetTransformderiveD * deriveD.WorldTransform;
		objderiveMobG.World = PositionderiveGmob * OffsetTransformderiveG * deriveG.WorldTransform;
		objprofondeurD.World = PositionprofondeurmobD * OffsetTransformprofondeurD * profondeurD.WorldTransform;
		objprofondeurG.World = PositionprofondeurmobG * OffsetTransformprofondeurG * profondeurG.WorldTransform;
		objtubes.World = Positiontubes * fuselage.WorldTransform;
		objaileronG.World = PositionaileronGmob * OffsetTransformaileBG * aileBG.WorldTransform;
		objaileronD.World = PositionaileronDmob * OffsetTransformaileBD * aileBD.WorldTransform;
		objtraindroit.World = Matrix.CreateRotationZ(MathHelper.ToRadians(trainDRot)) * PositiontrainD * OffsetTransformfuselage * fuselage.WorldTransform;
		objtraingauche.World = Matrix.CreateRotationZ(MathHelper.ToRadians(trainGRot)) * PositiontrainG * OffsetTransformfuselage * fuselage.WorldTransform;
		objtrainavant.World = Matrix.CreateRotationX(MathHelper.ToRadians(trainARot)) * PositiontrainA * OffsetTransformfuselage * fuselage.WorldTransform;
		objtrapegauche.World = Matrix.CreateRotationZ(MathHelper.ToRadians(trapeGRot)) * PositiontrapeG * OffsetTransformaileBD * aileBD.WorldTransform;
		objtrapedroite.World = Matrix.CreateRotationZ(MathHelper.ToRadians(trapeDRot)) * PositiontrapeD * OffsetTransformaileBG * aileBG.WorldTransform;
		objtrapeAG.World = Matrix.CreateRotationZ(MathHelper.ToRadians(trapeAGRot)) * PositiontrapeAG * OffsetTransformfuselage * fuselage.WorldTransform;
		objtrapeAD.World = Matrix.CreateRotationZ(MathHelper.ToRadians(trapeADRot)) * PositiontrapeAD * OffsetTransformfuselage * fuselage.WorldTransform;
	}

	public void Moteur()
	{
		reacteur = soundBanka.GetCue("f-22");
		reacteur.SetVariable("accelerateur", pitch);
		reacteur.Apply3D(listeneravion, emitteravion);
		reacteur.Play();
		reacteur2 = soundBanka.GetCue("reacteur2");
		reacteur2.SetVariable("accelerateur", pitch);
		reacteur2.Apply3D(listeneravion, emitteravion);
		reacteur2.Play();
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
		if ((sender != null && VitesseAvion >= 29f) || accelerationDE <= -4f)
		{
			Collision1();
		}
	}

	public void Update(float dt, CustomPhysicsGame game, GameTime gameTime)
	{
		inputStateConfig.Update(game);
		game.space.Update(dt);
		game.camera.position = new Vector3(-33f, 15f, 12f);
		checked
		{
			if (inputStateConfig.ApressBis && VitesseAvion >= 4f)
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
				trainDRot += 0.5f;
				trainGRot -= 0.5f;
				trainARot -= 0.5f;
				if (trainDRot >= 100f)
				{
					trapeDRot -= 1.1f;
					trapeGRot += 1.1f;
				}
				if (trainARot <= -90f)
				{
					trapeADRot -= 0.5f;
					trapeAGRot += 0.5f;
				}
				Ytrain += 0.0049f;
				angleroue -= 0.008f;
				angleroue1 += 0.008f;
				ZtrainA += 0.0041f;
				YtrainA += 0.0024f;
				if (Ytrain >= 0.16f)
				{
					game.terrain.sceneInterfaceScene.ObjectManager.Remove(objroue);
					game.terrain.sceneInterfaceScene.ObjectManager.Remove(objroue1);
				}
				Vehicle.Wheels[0].Suspension.LocalAttachmentPoint = new Vector3(XtrainG, Ytrain, -0.5f);
				Vehicle.Wheels[1].Suspension.LocalAttachmentPoint = new Vector3(XtrainD, Ytrain, -0.5f);
				Vehicle.Wheels[2].Suspension.LocalAttachmentPoint = new Vector3(-0.01f, YtrainA, ZtrainA);
				Vehicle.Wheels[0].Shape.SpinAngle = 0f;
				Vehicle.Wheels[1].Shape.SpinAngle = 0f;
				Vehicle.Wheels[0].Shape.LocalGraphicTransform = (wheelGraphicRotation = Matrix.CreateFromAxisAngle(Vector3.Forward, angleroue));
				Vehicle.Wheels[1].Shape.LocalGraphicTransform = (wheelGraphicRotation = Matrix.CreateFromAxisAngle(Vector3.Forward, angleroue1));
				if (angleroue1 >= 3.1416f)
				{
					angleroue1 = 3.1416f;
				}
				if (angleroue <= 0f)
				{
					angleroue = 0f;
				}
				if (Ytrain >= 0.16f)
				{
					Ytrain = 0.16f;
				}
				if (YtrainA >= 0f)
				{
					YtrainA = 0f;
				}
				if (ZtrainA >= -6f)
				{
					ZtrainA = -6f;
				}
				if (trainDRot >= 105f)
				{
					trainDRot = 105f;
				}
				if (trainGRot <= -105f)
				{
					trainGRot = -105f;
				}
				if (trapeDRot <= 0f)
				{
					trapeDRot = 0f;
				}
				if (trapeGRot >= 0f)
				{
					trapeGRot = 0f;
				}
				if (trainARot <= -90f)
				{
					trainARot = -90f;
				}
				if (trapeADRot <= 0f)
				{
					trapeADRot = 0f;
				}
				if (trapeAGRot >= 0f)
				{
					trapeAGRot = 0f;
				}
			}
			if (trainState == TrainState.sorti)
			{
				if (trapeDRot >= 86f)
				{
					trainDRot -= 0.5f;
					trainGRot += 0.5f;
					Ytrain -= 0.0045f;
					game.terrain.sceneInterfaceScene.ObjectManager.Submit(objroue);
					game.terrain.sceneInterfaceScene.ObjectManager.Submit(objroue1);
					angleroue += 0.008f;
					angleroue1 -= 0.008f;
				}
				trapeDRot += 0.5f;
				trapeGRot -= 0.5f;
				trapeADRot += 0.5f;
				trapeAGRot -= 0.5f;
				if (trapeADRot >= 50f)
				{
					trainARot += 0.5f;
					YtrainA -= 0.006f;
					ZtrainA -= 0.0028f;
				}
				Vehicle.Wheels[0].Suspension.LocalAttachmentPoint = new Vector3(XtrainG, Ytrain, -0.42f);
				Vehicle.Wheels[1].Suspension.LocalAttachmentPoint = new Vector3(XtrainD, Ytrain, -0.42f);
				Vehicle.Wheels[2].Suspension.LocalAttachmentPoint = new Vector3(-0.01f, YtrainA, ZtrainA);
				Vehicle.Wheels[0].Shape.LocalGraphicTransform = (wheelGraphicRotation = Matrix.CreateFromAxisAngle(Vector3.Forward, angleroue));
				Vehicle.Wheels[1].Shape.LocalGraphicTransform = (wheelGraphicRotation = Matrix.CreateFromAxisAngle(Vector3.Forward, angleroue1));
				if (angleroue1 <= 1.5708f)
				{
					angleroue1 = 1.5708f;
				}
				if (angleroue >= 1.5708f)
				{
					angleroue = 1.5708f;
				}
				if (Ytrain <= -0.9f)
				{
					Ytrain = -0.9f;
				}
				if (XtrainD >= -1.56f)
				{
					XtrainD = -1.56f;
				}
				if (XtrainG <= 1.49f)
				{
					XtrainG = 1.49f;
				}
				if (YtrainA <= -0.78f)
				{
					YtrainA = -0.78f;
				}
				if (ZtrainA <= -6.58f)
				{
					ZtrainA = -6.58f;
				}
				if (trapeDRot >= 86f)
				{
					trapeDRot = 86f;
				}
				if (trapeGRot <= -86f)
				{
					trapeGRot = -86f;
				}
				if (trainARot >= 0f)
				{
					trainARot = 0f;
				}
				if (trainDRot <= 0f)
				{
					trainDRot = 0f;
				}
				if (trainGRot >= 0f)
				{
					trainGRot = 0f;
				}
				if (trapeADRot >= 86f)
				{
					trapeADRot = 86f;
				}
				if (trapeAGRot <= -86f)
				{
					trapeAGRot = -86f;
				}
			}
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
				particules1.Emitter.PositionData.Position = pot1.Position;
				particules2.Emitter.PositionData.Position = pot.Position;
				particules3.Emitter.PositionData.Position = pot1.Position;
				if (QuaFuel > 0f)
				{
					particules.Emitter.Enabled = true;
					particules1.Emitter.Enabled = true;
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
			game.space.Update(dt);
			Random random = new Random();
			particules.vitesse = new Vector3(random.Next(-3, 3), random.Next(3, 3), random.Next(3, 3));
			particules.nombrepar = acceleration / 50000f;
			particules.Emitter.ParticlesPerSecond = acceleration / 100f;
			particules1.vitesse = new Vector3(random.Next(-3, 3), random.Next(3, 3), random.Next(3, 3));
			particules1.nombrepar = acceleration / 50000f;
			particules1.Emitter.ParticlesPerSecond = acceleration / 100f;
			explosion.ExplosionColor = new Color(189, 145, 82);
			explosion.ExplosionParticleSize = 16;
			explosion.ExplosionIntensity = 1;
			explosion.vitesse = fuselage.LinearVelocity;
			particules2.Emitter.ParticlesPerSecond = 380f;
			particules3.Emitter.ParticlesPerSecond = 380f;
			particules2.diametre = 0.15f;
			particules3.diametre = 0.08f;
			particules2.InitialProperties.StartSizeMax = 2.2f;
			particules2.InitialProperties.EndSizeMax = 2.2f;
			particules3.InitialProperties.StartSizeMin = 2.2f;
			particules3.InitialProperties.EndSizeMin = 2.2f;
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
			float num3 = (float)Math.Acos(Vector3.Dot(sphere.OrientationMatrix.Down, Vehicle.Body.OrientationMatrix.Left)) * ((float)Math.PI * 2f);
			profondeurRot = 0f;
			Vehicle.Body.AngularDamping = 0.72f;
			float num4 = VitesseAvion * 10f;
			if (num4 >= 510f)
			{
				num4 = 510f;
			}
			Vector3 right = Vehicle.Body.OrientationMatrix.Right;
			if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M1 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M4 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M6)
			{
				Monte = -right * num4 * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.Y;
				profondeurRot += 30f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.Y;
				Vehicle.Body.ApplyAngularImpulse(ref Monte);
			}
			if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M2 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M3 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M5)
			{
				Monte = -right * num4 * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.Y;
				profondeurRot += 30f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.Y;
				Vehicle.Body.ApplyAngularImpulse(ref Monte);
			}
			deriveRot = 0f;
			float num5 = VitesseAvion * 8f;
			if (num5 >= 300f)
			{
				num5 = 300f;
			}
			Vector3 up = Vehicle.Body.OrientationMatrix.Up;
			if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M1 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M2 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M6)
			{
				DroiteS = -up * num5 * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
				deriveRot += 28f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
				if (altitude <= 1.4f)
				{
					DroiteS = -up * VitesseAvion * 70f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
					if (VitesseAvion <= 8f && VitesseAvion >= 0.03f)
					{
						DroiteS = -up * 700f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
					}
				}
				Vehicle.Body.ApplyAngularImpulse(ref DroiteS);
			}
			if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M3 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M4 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M5)
			{
				DroiteS = -up * num5 * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.X;
				deriveRot += 28f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.X;
				if (altitude <= 1.4f)
				{
					DroiteS = -up * VitesseAvion * 70f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.X;
					if (VitesseAvion <= 8f && VitesseAvion >= 0.03f)
					{
						DroiteS = -up * 700f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.X;
					}
				}
				Vehicle.Body.ApplyAngularImpulse(ref DroiteS);
			}
			aileronGRot = 0f;
			aileronDRot = 0f;
			float num6 = VitesseAvion * 8f;
			if (num6 >= 510f)
			{
				num6 = 510f;
			}
			Vector3 forward = Vehicle.Body.OrientationMatrix.Forward;
			if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M1 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M2 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M6)
			{
				Droite = forward * num6 * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.X;
				aileronGRot += 28f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.X;
				aileronDRot -= 28f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.X;
				Vehicle.Body.ApplyAngularImpulse(ref Droite);
			}
			if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M3 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M4 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M5)
			{
				Droite = forward * num6 * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
				aileronGRot += 28f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
				aileronDRot -= 28f * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.X;
				Vehicle.Body.ApplyAngularImpulse(ref Droite);
			}
			float num7 = AngleR * 20f;
			if (num7 >= 80f)
			{
				num7 = 80f;
			}
			if (num7 <= -80f)
			{
				num7 = -80f;
			}
			DroiteP = up * num7;
			Vehicle.Body.ApplyAngularImpulse(ref DroiteP);
			pot.Position = Vehicle.Body.Position;
			sphere.Position = Vehicle.Body.Position;
			Vector3 impulse = Monte / 5.8f;
			if (VitesseAvion >= 20f && num3 >= 5f && num3 <= 14f && altitude >= 5.75f && Angle2 <= 7f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse);
			}
			Vector3 impulse2 = -right * 180f;
			if (VitesseAvion <= 22f && altitude <= 3f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse2);
			}
			Vector3 impulse3 = right * 150f;
			if (VitesseAvion <= 1f && Angle2 <= 3f && altitude <= 3f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse3);
			}
			Vector3 impulse4 = -right * 1900f / Angle2;
			Vector3 impulse5 = right * 1800f / Angle2;
			Vector3 impulse6 = -up * 3200f / Angle2;
			Vector3 impulse7 = up * 3200f / Angle2;
			Vector3 impulse8 = -up * 1700f / Angle2;
			Vector3 impulse9 = up * 1700f / Angle2;
			Vector3 impulse10 = -right * 120f / Angle2;
			Vector3 impulse11 = right * 1100f / Angle2;
			if (Angle3 >= 0f && Angle2 >= 10f && num3 >= 4f && num3 <= 15f && altitude >= 3f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse11);
			}
			if (Angle2 >= 10f && num3 >= 4f && num3 <= 15f)
			{
				Vehicle.Body.ApplyAngularImpulse(ref impulse10);
			}
			if (VitesseAvion <= 25f && altitude >= 3f)
			{
				if (Angle3 <= 0f && Angle2 >= 11f && num3 >= 4f && num3 <= 15f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse4);
				}
				if (Angle3 >= 0f && Angle2 >= 11f && num3 >= 4f && num3 <= 15f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse5);
				}
				if (Angle <= -2.5f && Angle2 >= 11f && VitesseAvion <= 2f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse6);
				}
				if (Angle >= 2.5f && Angle2 >= 11f && VitesseAvion <= 2f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse7);
				}
				if (Angle3 <= 0f && num3 >= 4f && num3 <= 15f && Angle2 <= 11f && Angle2 >= 5f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse4);
				}
				if (Angle3 >= 0f && num3 >= 4f && num3 <= 15f && Angle2 <= 11f && Angle2 >= 5f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse5);
				}
				if (num3 <= 4f && Angle2 <= 11f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse7);
				}
				if (num3 >= 15f && Angle2 <= 11f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse6);
				}
			}
			if (VitesseAvion <= 35f && altitude >= 2f)
			{
				if (num3 <= 4f)
				{
					Vehicle.Body.ApplyAngularImpulse(ref impulse9);
				}
				if (num3 >= 15f)
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
				particules1.Emitter.Enabled = false;
				particules2.Emitter.Enabled = false;
				particules3.Emitter.Enabled = false;
				impulse12 = VitesseAvion * vector2;
				reacteur.Stop(AudioStopOptions.Immediate);
				reacteur2.Stop(AudioStopOptions.Immediate);
			}
			Vehicle.Body.ApplyLinearImpulse(ref impulse12);
			if (compteCrash >= 99)
			{
				compteCrash = 99;
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
			Vehicle.Body.LinearDamping = Angle2 / 89f;
			if (trainState == TrainState.sorti)
			{
				Vehicle.Body.LinearDamping = Angle2 / 79f;
			}
			if (VitesseAvion < 18f)
			{
				Vehicle.Body.LinearDamping = 0f;
			}
			RAcc = vitesseMaxi / AccAPP;
			variA = RAcc / sensibilite;
			moteurA = AccM / variA;
			bouttonA = AccB / variA;
			if ((game.manetteChoix == CustomPhysicsGame.ManetteChoix.M3 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M6) && inputStateConfig.Acceleration)
			{
				acceleration += 150f;
				reacteur.SetVariable("accelerateur", pitch += 0.18f);
				reacteur2.SetVariable("accelerateur", pitch += 0.18f);
				MDboutton -= 0.675f;
				if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M1 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M4)
				{
					acceleration -= sensibilite * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.Y;
					reacteur.SetVariable("accelerateur", pitch -= moteurA * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.Y);
					reacteur2.SetVariable("accelerateur", pitch -= moteurA * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.Y);
					MDboutton += bouttonA * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.Y;
				}
				if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M2 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M3)
				{
					acceleration -= sensibilite * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.Y;
					reacteur.SetVariable("accelerateur", pitch -= moteurA * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.Y);
					reacteur2.SetVariable("accelerateur", pitch -= moteurA * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.Y);
					MDboutton += bouttonA * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.Y;
				}
			}
			if ((game.manetteChoix == CustomPhysicsGame.ManetteChoix.M5 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M6) && inputStateConfig.Deceleration)
			{
				acceleration -= 150f;
				reacteur.SetVariable("accelerateur", pitch -= 0.18f);
				reacteur2.SetVariable("accelerateur", pitch -= 0.18f);
				MDboutton += 0.675f;
			}
			if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M1 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M4)
			{
				acceleration += sensibilite * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.Y;
				reacteur.SetVariable("accelerateur", pitch += moteurA * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.Y);
				reacteur2.SetVariable("accelerateur", pitch += moteurA * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.Y);
				MDboutton -= bouttonA * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Right.Y;
			}
			if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M2 || game.manetteChoix == CustomPhysicsGame.ManetteChoix.M3)
			{
				acceleration += sensibilite * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.Y;
				reacteur.SetVariable("accelerateur", pitch += moteurA * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.Y);
				reacteur2.SetVariable("accelerateur", pitch += moteurA * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.Y);
				MDboutton -= bouttonA * game.input.CurrentGamePadStates[game.pla].ThumbSticks.Left.Y;
			}
			if (VitesseAvion <= 24f && altitude >= 3f)
			{
				accelerationDE -= 0.3f;
			}
			else
			{
				accelerationDE += 0.1f;
			}
			if (Angle2 >= 16f)
			{
				accelerationDE -= 0.1f;
			}
			else
			{
				accelerationDE += 0.02f;
			}
			if (num3 <= 4f || num3 >= 15f)
			{
				accelerationDE -= 0.1f;
			}
			else
			{
				accelerationDE += 0.02f;
			}
			if (accelerationDE <= -750f)
			{
				accelerationDE = -750f;
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
			JaugeBoutton = VitesseAvion * 0.8f + 128f;
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
			reacteur.Apply3D(listeneravion, emitteravion);
			reacteur2.Apply3D(listeneravion, emitteravion);
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
				owningSpace.Remove(deriveD);
				owningSpace.Remove(deriveG);
				owningSpace.Remove(profondeurD);
				owningSpace.Remove(profondeurG);
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
				reacteur.Stop(AudioStopOptions.Immediate);
				reacteur2.Stop(AudioStopOptions.Immediate);
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
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objtraingauche);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objtrainavant);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objtrapegauche);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objtrapedroite);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objtrapeAG);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objtrapeAD);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objfuselage);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objailesD);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objailesG);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objderiveD);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objderiveG);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objderiveMobD);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objderiveMobG);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objprofondeurD);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objprofondeurG);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objroue);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objroue1);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objroueA);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objaileronG);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objaileronD);
			game.terrain.sceneInterfaceScene.ObjectManager.Remove(objtubes);
			owningSpace.Remove(Vehicle);
		}
		if (Avioncasse)
		{
			owningSpace.Remove(fuselage);
			owningSpace.Remove(aileBG);
			owningSpace.Remove(aileBD);
			owningSpace.Remove(deriveD);
			owningSpace.Remove(deriveG);
			owningSpace.Remove(profondeurD);
			owningSpace.Remove(profondeurG);
			owningSpace.Remove(roue1);
			owningSpace.Remove(roue2);
			owningSpace.Remove(roueA);
		}
	}

	public void Restart(CustomPhysicsGame game)
	{
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
		pitch = -12f;
		game.camera.fov = 45f;
		Vehicle.Body.LinearVelocity = Vector3.Zero;
		Vehicle.Body.AngularVelocity = Vector3.Zero;
		Vehicle.Body.Orientation = new Quaternion(0f, 0f, 0f, 1f);
		AvionNeuf.Position = new Vector3(0f, 1.5f, 0f);
		particules.Emitter.Enabled = true;
		particules1.Emitter.Enabled = true;
		particules2.Emitter.Enabled = true;
		particules3.Emitter.Enabled = true;
		Moteur();
		trainDRot = 0f;
		trainGRot = 0f;
		trainARot = 0f;
		trapeDRot = 86f;
		trapeGRot = -86f;
		trapeADRot = 90f;
		trapeAGRot = -90f;
		XtrainG = 1.49f;
		XtrainD = -1.56f;
		Ytrain = -0.9f;
		YtrainA = -0.78f;
		ZtrainA = -6.58f;
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
			particules1.Emitter.Enabled = false;
			particules2.Emitter.Enabled = false;
			particules3.Emitter.Enabled = false;
			explosion.Emitter.Enabled = true;
			if (altitude <= 8f)
			{
				explosion.Explode();
			}
			owningSpace.Remove(Vehicle);
			owningSpace.Add(fuselage);
			owningSpace.Add(aileBG);
			owningSpace.Add(aileBD);
			owningSpace.Add(deriveD);
			owningSpace.Add(deriveG);
			owningSpace.Add(profondeurD);
			owningSpace.Add(profondeurG);
			owningSpace.Add(roue1);
			owningSpace.Add(roue2);
			owningSpace.Add(roueA);
			fuselage.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			fuselage.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			aileBG.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			aileBG.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			aileBD.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			aileBD.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			deriveD.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			deriveD.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			deriveG.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			deriveG.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			profondeurD.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			profondeurD.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			profondeurG.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			profondeurG.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			roue1.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			roue1.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			roue2.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			roue2.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			roueA.LinearVelocity = AvionNeuf.LinearVelocity / 3f;
			roueA.AngularVelocity = AvionNeuf.AngularVelocity / 3f;
			Crash();
			reacteur.Stop(AudioStopOptions.Immediate);
			reacteur2.Stop(AudioStopOptions.Immediate);
		}
	}
}
