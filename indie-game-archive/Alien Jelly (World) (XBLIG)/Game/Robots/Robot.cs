using System;
using GKEngine;
using GKEngine.Cameras;
using GKEngine.Entities;
using GKEngine.Utils;
using Game.Atoms;
using Game.Grids;
using Game.History;
using Game.Particles;
using Game.Physics;
using Game.QBits;
using Game.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Robots;

public class Robot : PhysicsItem
{
	public enum RobotType
	{
		Static,
		Guard,
		Move
	}

	private const string TRACER_LASER = "Eye_0";

	private const string TRACER_BOOSTER = "Booster_0";

	private const float BOOSTER_TIME = 1000f;

	private const float LASER_TIME = 1000f;

	private const float GLOW_TIME = 3000f;

	private const int DELAY_TICK = 5;

	private const float ALERT_RADIUS_MULTI = 1.3f;

	private const float ALERT_Y_DELTA = 0.5f;

	public static string[] TYPE_NAMES = new string[3] { "Static", "Guard", "Move" };

	public static string[] TYPE_MODEL_PATH = new string[3] { "Content/Models/Robots/Default/Model", "Content/Models/Robots/Default/Model", "Content/Models/Robots/Default/Model" };

	public static string LASER_MODEL_PATH = "Content/Models/Robots/Laser/Model";

	public static Range[] WAIT_TIME = new Range[1]
	{
		new Range(600f, 1000f)
	};

	public static Range[] MOVE_TIME = new Range[4]
	{
		new Range(600f, 1000f),
		new Range(500f, 800f),
		new Range(400f, 700f),
		new Range(200f, 500f)
	};

	public static int[] LASER_RADIUS = new int[3] { 120, 80, 40 };

	public static string TITLE = "Robot";

	public static string DESCRIPTION = "The robot is the bad guy. It will do its best to kill all the alien jellies it can find. This item has properties.";

	public static string PROPERTIES_DESCRIPTION = "This robot has the following properties that will help you control how it moves and thinks.";

	public static AtomProperty[] PROPERTIES = new AtomProperty[3]
	{
		new AtomProperty("Robot Type", "This option lets you define what kind of robot this is there by what actions it can perform.", new string[3] { "Stupid", "Guard", "Moving" }),
		new AtomProperty("Move Speed", "This option allows you to set how fast the robot moves.", new string[4] { "Slow", "Normal", "Fast", "Super Fast" }),
		new AtomProperty("Laser Range", "This option allows you to set the range or the robot's laser.", new string[3] { "Far", "Normal", "Short" })
	};

	public static int[] PROPERTIES_DEFAULT;

	private static Range SEEK_TIME;

	private static float SEEK_LEPR;

	private object _laser_collide;

	private Vector3 _proximity_qbit = default(Vector3);

	private Vector2 _proximity_qbit_XZ = default(Vector2);

	public RobotManager manager;

	public RobotType type;

	protected int renderStackIndex = 3;

	protected int renderStateIndex;

	private int delayTick;

	protected MaxModel model;

	protected MaxModelPart part;

	protected MaxModelPartRenderable[] partBooster;

	protected Effect effect;

	private EffectParameter effectRatio;

	private MaxModelRenderable modelLaser;

	private EffectParameter modelLaserEffectTime;

	private EffectParameter modelLaserEffectDistance;

	protected float[] boosterTimes;

	private Range boosterOffset = new Range(0f, 5f);

	public bool flipping;

	public int moveType;

	private Vector3 moveHeading;

	private bool waiting;

	private float waitTime;

	private float waitTimeTotal;

	private Quaternion directionStart;

	private Quaternion directionTarget;

	private float directionSeekingTime;

	private float directionSeekingTimeTotal;

	private QBit qbit;

	private Base3D laser = new Base3D();

	private bool lasering;

	private float laserTime;

	private float laserRatio;

	private int laserRadius = 100;

	private Ray laserRay = default(Ray);

	private float laserDistance = 1f;

	private Vector3 tracerLaser;

	private Vector3 tracerBooster;

	public ParticleEmitter emitter;

	public ParticleEmitter emitterSmoke;

	public ParticleEmitterSchema emitterSchemaExplode;

	public ParticleEmitterSchema emitterSchemaSmoke;

	private float glowTime;

	private bool _alerted;

	public int index => manager.robots.IndexOf(this);

	public bool busy
	{
		get
		{
			if (!moving && !dying && !lasering)
			{
				return flipping;
			}
			return true;
		}
	}

	public override bool physicsAlive
	{
		get
		{
			if (!dead && !dying && visible)
			{
				return !moving;
			}
			return false;
		}
	}

	public override int[] properties
	{
		set
		{
			if (value.Length > 0)
			{
				type = (RobotType)value[0];
				moveType = value[1];
				laserRadius = LASER_RADIUS[value[2]];
			}
		}
	}

	public bool alerted
	{
		get
		{
			return _alerted;
		}
		set
		{
			if (_alerted != value)
			{
				if (value)
				{
					if (waiting)
					{
						Wait_Complete();
					}
				}
				else
				{
					Direction_Seek();
				}
			}
			_alerted = value;
		}
	}

	public Robot(RobotManager oManager, int[] aProperties)
	{
		manager = oManager;
		scene = manager.universe.scene;
		properties = aProperties;
		manager.universe.physics.Add(this);
		Load();
	}

	public override void Load()
	{
		model = GameEngine.SceneContent.Load<MaxModel>(TYPE_MODEL_PATH[(int)type]);
		model.Build(this);
		Laser_Load();
		tracerLaser = new Vector3(model.tracers["Eye_0"].M41, model.tracers["Eye_0"].M42, model.tracers["Eye_0"].M43);
		tracerBooster = new Vector3(model.tracers["Booster_0"].M41, model.tracers["Booster_0"].M42, model.tracers["Booster_0"].M43);
		part = model.modelParts[0];
		effect = part.material.effect;
		effectRatio = effect.Parameters["Ratio"];
		Booster_Init();
		scene.lights.SetEffect(ref effect);
		scene.RenderStacks_FromName(GameMain.RENDERSTACK_SOLID).Add(guid.value, this);
		Particles_Set();
		base.Load();
	}

	public override void Dispose()
	{
		base.Dispose();
		Particles_Dispose();
		model = null;
		Booster_Dispose();
		Laser_Dispose();
	}

	public void Update(GameTime oGameTime)
	{
		if (dead || historyLocked)
		{
			return;
		}
		int milliseconds = oGameTime.ElapsedGameTime.Milliseconds;
		delayTick++;
		delayTick %= 5;
		glowTime += milliseconds;
		glowTime %= 3000f;
		if (effectRatio != null)
		{
			effectRatio.SetValue(glowTime / 3000f);
		}
		if (dying)
		{
			Death_Update(oGameTime);
			return;
		}
		Think(oGameTime);
		if (moving)
		{
			MoveTo_Update(milliseconds);
		}
		else if (lasering)
		{
			Laser_Update(oGameTime);
		}
		else if (waiting)
		{
			Wait_Update(milliseconds);
		}
		Direction_Update(oGameTime);
		Kill_Check();
		Booster_Update(oGameTime);
	}

	public void Reverse(GameTime oGameTime)
	{
		if (lasering)
		{
			Laser_Update(oGameTime);
		}
	}

	public void Start()
	{
		effect.Parameters["CamCull"].SetValue(value: false);
		Direction_Init();
		switch (type)
		{
		case RobotType.Static:
			Laser();
			break;
		case RobotType.Guard:
		case RobotType.Move:
			break;
		}
	}

	private void Booster_Init()
	{
		partBooster = new MaxModelPartRenderable[3];
		partBooster[0] = new MaxModelPartRenderable(scene, this, model.modelParts[1]);
		partBooster[1] = new MaxModelPartRenderable(scene, this, model.modelParts[2]);
		partBooster[2] = new MaxModelPartRenderable(scene, this, model.modelParts[3]);
		boosterTimes = new float[3];
		boosterTimes[0] = 0f;
		boosterTimes[1] = 0.33333f;
		boosterTimes[2] = 0.66666f;
		for (int i = 0; i < partBooster.Length; i++)
		{
			partBooster[i].part.hasLocal = true;
			partBooster[i].part.local = Matrix.Identity;
			scene.RenderStacks_FromName(GameMain.RENDERSTACK_ADD).Add(partBooster[i].guid.value, partBooster[i]);
		}
	}

	private void Booster_Update(GameTime oGameTime)
	{
		for (int i = 0; i < boosterTimes.Length; i++)
		{
			boosterTimes[i] += (float)oGameTime.ElapsedGameTime.Milliseconds / 1000f;
			boosterTimes[i] %= 1f;
			Booster_Lerp(i);
		}
	}

	private void Booster_Lerp(int xIndex)
	{
		partBooster[xIndex].part.local.M11 = (1f - boosterTimes[xIndex]) * 1.2f;
		partBooster[xIndex].part.local.M33 = (1f - boosterTimes[xIndex]) * 1.2f;
		partBooster[xIndex].part.local.M42 = boosterOffset.Lerp(boosterTimes[xIndex]) * -1f;
	}

	private void Booster_Dispose()
	{
		for (int i = 0; i < partBooster.Length; i++)
		{
			if (partBooster[i] != null)
			{
				scene.RenderStacks_FromName(GameMain.RENDERSTACK_ADD).Remove(partBooster[i].guid.value, partBooster[i]);
				partBooster[i].part = null;
				partBooster[i].Dispose();
				partBooster[i] = null;
			}
		}
		partBooster = null;
	}

	public void MoveTo(Vector3 vTo)
	{
		Event_Move_Start();
		moveTime = 0;
		moveTimeTotal = (int)MOVE_TIME[moveType].random;
		moveFrom = position;
		moveTo = vTo;
		moving = true;
	}

	public void MoveTo_Update(int elapsed)
	{
		moveTime += elapsed;
		if (moveTime >= moveTimeTotal)
		{
			MoveTo_Lerp(1f);
			SnapToGrid();
			moving = false;
			Event_Move_End();
		}
		else
		{
			float ratio = (float)moveTime / (float)moveTimeTotal;
			MoveTo_Lerp(ratio);
		}
	}

	protected void MoveTo_Lerp(float ratio)
	{
		float num = (float)Math.Sin((double)ratio * (Math.PI / 2.0));
		X = moveFrom.X + (moveTo.X - moveFrom.X) * num;
		Z = moveFrom.Z + (moveTo.Z - moveFrom.Z) * num;
		Y = moveFrom.Y + (moveTo.Y - moveFrom.Y) * num;
	}

	protected void MoveTo_Finalise()
	{
		if (moving)
		{
			if ((double)moveTime >= (double)moveTimeTotal * 0.5)
			{
				MoveTo_Lerp(1f);
			}
			else
			{
				MoveTo_Lerp(0f);
			}
			SnapToGrid();
			Event_Move_End();
			moving = false;
		}
	}

	public void Wait()
	{
		waitTime = 0f;
		waitTimeTotal = WAIT_TIME[0].random;
		waiting = true;
	}

	public void Wait_Update(int elapsed)
	{
		if (waiting && !flipping)
		{
			waitTime += elapsed;
			if (waitTime >= waitTimeTotal)
			{
				Wait_Complete();
			}
		}
	}

	public void Wait_Complete()
	{
		waiting = false;
	}

	private void Direction_Init()
	{
		directionStart = rotation;
		moveHeading = matrix.Forward;
	}

	private void Direction_Update(GameTime oGameTime)
	{
		if ((type != RobotType.Move && type != RobotType.Guard) || flipping)
		{
			return;
		}
		if (!alerted)
		{
			directionSeekingTime += (float)oGameTime.ElapsedGameTime.TotalMilliseconds;
			if (directionSeekingTime >= directionSeekingTimeTotal)
			{
				Direction_Seek();
			}
		}
		else
		{
			directionTarget = Quaternion.Normalize(Quaternion.CreateFromRotationMatrix(Matrix.CreateConstrainedBillboard(position, qbit.position, base.rotationMatrix.Up, null, null)));
		}
		if (lasering)
		{
			rotation = Quaternion.Lerp(rotation, directionTarget, 0.2f);
		}
		else
		{
			rotation = Quaternion.Lerp(rotation, directionTarget, SEEK_LEPR);
		}
	}

	private void Direction_Seek()
	{
		directionSeekingTime = 0f;
		directionSeekingTimeTotal = SEEK_TIME.random;
		directionTarget = rotation * Quaternion.CreateFromAxisAngle(Vector3.Up, (float)(GameEngine.random.NextDouble() * Math.PI * 2.0));
	}

	private void Think(GameTime oGameTime)
	{
		switch (type)
		{
		case RobotType.Guard:
			Think_ProximityCheck();
			break;
		case RobotType.Move:
			if (!moving)
			{
				Think_ProximityCheck();
			}
			Think_Move();
			break;
		case RobotType.Static:
			break;
		}
	}

	private void Think_ProximityCheck()
	{
		float num = float.MaxValue;
		QBit qBit = null;
		QBit qBit2 = null;
		for (int i = 0; i < manager.universe.qbits.qbits.Count; i++)
		{
			_proximity_qbit = Vector3.Transform(manager.universe.qbits.qbits[i].position, Matrix.Invert(matrix));
			float num2 = Math.Abs(_proximity_qbit.Y);
			_proximity_qbit_XZ.X = _proximity_qbit.X;
			_proximity_qbit_XZ.Y = _proximity_qbit.Z;
			float xDistance = _proximity_qbit_XZ.Length();
			if (!(num2 < Grid.SPACING.Y * 0.5f) || !(xDistance < (float)laserRadius * 1.3f))
			{
				continue;
			}
			if (xDistance < num)
			{
				qBit = manager.universe.qbits.qbits[i];
				num = xDistance;
			}
			if (xDistance < (float)laserRadius)
			{
				_temp_vector = manager.universe.qbits.qbits[i].position - position;
				Vector3.Normalize(ref _temp_vector, out laserRay.Direction);
				laserRay.Position = position;
				if (!PhysicsRayCheckNoQbits(this, ref laserRay, ref xDistance))
				{
					qBit2 = manager.universe.qbits.qbits[i];
					break;
				}
			}
		}
		if (qBit2 != null)
		{
			qbit = qBit2;
			alerted = true;
			if (!lasering)
			{
				Laser();
			}
			return;
		}
		if (lasering)
		{
			Laser_Stop();
		}
		if (qBit != null)
		{
			qbit = qBit;
			alerted = true;
		}
		else
		{
			alerted = false;
		}
	}

	private void Think_Move()
	{
		if (!alerted && !waiting && !moving && !lasering)
		{
			Vector3 vector = new Vector3(X + Grid.SPACING.X * moveHeading.X, Y + Grid.SPACING.Y * moveHeading.Y, Z + Grid.SPACING.Z * moveHeading.Z);
			Vector3 vPosition = new Vector3(X + Grid.SPACING.X * (moveHeading.X + matrix.Down.X), Y + Grid.SPACING.Y * (moveHeading.Y + matrix.Down.Y), Z + Grid.SPACING.Z * (moveHeading.Z + matrix.Down.Z));
			IGridable gridable = manager.universe.grid.At(vector);
			PhysicsItem physicsItem = physics.At(vector.X, vector.Y, vector.Z);
			IGridable gridable2 = manager.universe.grid.At(vPosition);
			PhysicsItem physicsItem2 = physics.At(vPosition.X, vPosition.Y, vPosition.Z);
			if (gridable == null && physicsItem == null && (gridable2 != null || physicsItem2 != null))
			{
				MoveTo(vector);
				return;
			}
			moveHeading.X *= -1f;
			moveHeading.Y *= -1f;
			moveHeading.Z *= -1f;
			Wait();
		}
	}

	private void Laser()
	{
		lasering = true;
		modelLaser.model.visible = true;
		laserTime = 0f;
		modelLaserEffectTime.SetValue(0);
	}

	private void Laser_Load()
	{
		modelLaser = new MaxModelRenderable(scene, GameEngine.SceneContent.Load<MaxModel>(LASER_MODEL_PATH).Clone());
		modelLaser.model.Build(laser);
		modelLaserEffectTime = modelLaser.model.modelParts[0].material.effect.Parameters["Time"];
		modelLaserEffectDistance = modelLaser.model.modelParts[0].material.effect.Parameters["Distance"];
		scene.RenderStacks_FromName(GameMain.RENDERSTACK_ADD).Add(modelLaser.guid.value, modelLaser);
		modelLaser.model.visible = false;
	}

	private void Laser_Dispose()
	{
		scene.RenderStacks_FromName(GameMain.RENDERSTACK_ADD).Remove(modelLaser.guid.value, modelLaser);
		modelLaser.Dispose();
		modelLaser = null;
	}

	private void Laser_Update(GameTime oGameTime)
	{
		laserTime += (float)oGameTime.ElapsedGameTime.TotalMilliseconds;
		laserTime %= 1000f;
		laserRatio = laserTime / 1000f;
		modelLaserEffectTime.SetValue(laserRatio);
		if (delayTick != 0)
		{
			return;
		}
		laserRay.Direction = matrix.Forward;
		laserRay.Position = position;
		laserDistance = 3000f;
		_laser_collide = PhysicsRayCast(this, ref laserRay, ref laserDistance);
		laserDistance -= 4f;
		modelLaserEffectDistance.SetValue(laserDistance);
		laser.scaleZ = laserDistance / 5f;
		laser.position = Vector3.Transform(tracerLaser, matrix);
		laser.rotation = Quaternion.Normalize(Quaternion.CreateFromRotationMatrix(Matrix.CreateBillboard(laserRay.Direction, Vector3.Zero, Vector3.Up, null)));
		if (!flipping && !historyLocked && !manager.universe.history.reversing && _laser_collide != null && _laser_collide is QBit)
		{
			QBit qBit = _laser_collide as QBit;
			if (!qBit.historyLocked && !qBit.dead && !qBit.dying)
			{
				qBit.Death();
			}
		}
	}

	private void Laser_Stop()
	{
		lasering = false;
		modelLaser.model.visible = false;
	}

	public void Kill_Check()
	{
		for (int i = 0; i < manager.universe.qbits.qbits.Count; i++)
		{
			QBit qBit = manager.universe.qbits.qbits[i];
			if (qBit.playable && Math.Abs(qBit.X - X) < Grid.SPACING.X * 0.5f && Math.Abs(qBit.Y - Y) < Grid.SPACING.Y * 0.5f && Math.Abs(qBit.Z - Z) < Grid.SPACING.Z * 0.5f)
			{
				qBit.Death();
				break;
			}
		}
	}

	public override void Death()
	{
		base.Death();
		(scene as PlayScene).audio.EventCues_Trigger("Robot Die");
		deathTime = 0f;
		Laser_Stop();
		emitter.matrix = matrix;
		emitter.Start(1000f, emitterSchemaExplode);
		emitterSmoke.matrix = matrix;
		emitterSmoke.Start(2000f, emitterSchemaSmoke);
		visible = false;
	}

	private void Death_Update(GameTime oGameTime)
	{
		deathTime += oGameTime.ElapsedGameTime.Milliseconds;
		if (deathTime >= deathTimeTotal)
		{
			Dead();
		}
		else
		{
			Death_Lerp(deathTime / deathTimeTotal);
		}
	}

	private void Death_Lerp(float xRatio)
	{
	}

	public override void Render(GameTime oGameTime)
	{
		if (visible)
		{
			Camera camera = scene.cameras.camera;
			part.Render(ref _matrix, camera);
		}
	}

	public void RenderEffect(ref Effect oEffect)
	{
		if (visible)
		{
			oEffect.Parameters["World"].SetValue(matrix);
			model.RenderEffect(ref oEffect);
		}
	}

	public static Robot FromAtom(RobotManager oManager, AtomDefinition oDef, int[] aProperties)
	{
		return new Robot(oManager, aProperties);
	}

	private void Particles_Set()
	{
		emitter = new ParticleEmitter(scene, ParticleEmitter.EffectType.Default, GameMain.RENDERSTACK_ADD);
		emitterSmoke = new ParticleEmitter(scene, ParticleEmitter.EffectType.Default, GameMain.RENDERSTACK_ALPHA_UNSORTED);
		emitterSchemaExplode = new ParticleEmitterSchema(50);
		emitterSchemaExplode.mode = ParticleEmitter.Mode.OneShot;
		emitterSchemaExplode.rotationStart = 0f;
		emitterSchemaExplode.rotationEnd = (float)Math.PI * 2f;
		emitterSchemaExplode.rotationTween = 1;
		emitterSchemaExplode.scaleStart = 30f;
		emitterSchemaExplode.scaleEnd = 5f;
		emitterSchemaExplode.scaleTween = 1;
		emitterSchemaExplode.textureStart = scene.library.texture2Ds["TextureDiffuse_Particles_Explosion_0"];
		emitterSchemaExplode.textureEnd = scene.library.texture2Ds["TextureDiffuse_Particles_Stars_0"];
		emitterSchemaExplode.textureTween = 0;
		emitterSchemaExplode.tintStart = new Color(255, 255, 255, 255);
		emitterSchemaExplode.tintEnd = new Color(255, 100, 100, 0);
		emitterSchemaExplode.tintTween = 1;
		emitterSchemaExplode.tween = 1;
		emitterSchemaExplode.Float_Constant(ref emitterSchemaExplode.data, 0u, 0f);
		emitterSchemaExplode.Vector_Constant(ref emitterSchemaExplode.positions, Vector3.Zero);
		emitterSchemaExplode.Vector_Random(ref emitterSchemaExplode.deltas, Vector3.Zero, 20f, 50f);
		emitterSchemaSmoke = new ParticleEmitterSchema(50);
		emitterSchemaSmoke.mode = ParticleEmitter.Mode.OneShot;
		emitterSchemaSmoke.rotationStart = 0f;
		emitterSchemaSmoke.rotationEnd = (float)Math.PI * 2f;
		emitterSchemaSmoke.rotationTween = 1;
		emitterSchemaSmoke.scaleStart = 10f;
		emitterSchemaSmoke.scaleEnd = 50f;
		emitterSchemaSmoke.scaleTween = 1;
		emitterSchemaSmoke.textureStart = scene.library.texture2Ds["TextureDiffuse_Particles_Clouds_0"];
		emitterSchemaSmoke.textureEnd = scene.library.texture2Ds["TextureDiffuse_Particles_Clouds_0"];
		emitterSchemaSmoke.textureTween = 1;
		emitterSchemaSmoke.tintStart = new Color(64, 64, 64, 255);
		emitterSchemaSmoke.tintEnd = new Color(0, 0, 0, 0);
		emitterSchemaSmoke.tintTween = 1;
		emitterSchemaSmoke.tween = 1;
		emitterSchemaSmoke.Float_Constant(ref emitterSchemaSmoke.data, 0u, 0f);
		emitterSchemaSmoke.Vector_Random(ref emitterSchemaSmoke.positions, Vector3.Zero, 5f, 15f);
		emitterSchemaSmoke.Vector_Random(ref emitterSchemaSmoke.deltas, new Vector3(0f, 40f, 0f), 0f, 30f);
	}

	private void Particles_Dispose()
	{
		emitter.Dispose();
		emitterSmoke.Dispose();
		emitter = null;
		emitterSmoke = null;
		emitterSchemaExplode = null;
		emitterSchemaSmoke = null;
	}

	public override void PhysicsStart()
	{
	}

	public override void PhysicsStop()
	{
	}

	public override void PhysicsUpdate(float elapsed)
	{
	}

	protected override void Event_Physics_Start()
	{
	}

	protected override void Event_Physics_End()
	{
	}

	public override void Event_Flip_Start()
	{
		if (!dead)
		{
			emitter.Stop();
			flipping = true;
			if (!historyLocked && !manager.universe.history.reversing)
			{
				MoveTo_Finalise();
			}
			if (lasering)
			{
				Laser_Stop();
			}
		}
		base.Event_Flip_Start();
	}

	public override void Event_Flip_End()
	{
		if (!dead)
		{
			flipping = false;
			moveHeading = Vector3.Transform(moveHeading, Quaternion.CreateFromAxisAngle(manager.universe.flippingAxis, (float)Math.PI / 2f * (float)manager.universe.flippingAmount));
			MathUtils.VectSnap(ref moveHeading);
			SnapToGrid();
			if (type == RobotType.Static)
			{
				Laser();
			}
			Direction_Seek();
		}
	}

	protected override void Event_Move_End()
	{
		physics.universe.history.Close(this, HistoryItem.Action.Move);
	}

	protected override void History_Reverse_Move_Lerp(ref HistoryItem oItem, float xRatio, GameTime oGameTime)
	{
		base.History_Reverse_Move_Lerp(ref oItem, xRatio, oGameTime);
	}

	public override void History_Event_Reverse_End(ref HistoryItem oItem)
	{
		base.History_Event_Reverse_End(ref oItem);
		if (oItem.action == HistoryItem.Action.Death && type == RobotType.Static)
		{
			Laser();
		}
	}

	static Robot()
	{
		int[] pROPERTIES_DEFAULT = new int[3];
		PROPERTIES_DEFAULT = pROPERTIES_DEFAULT;
		SEEK_TIME = new Range(1000f, 3000f);
		SEEK_LEPR = 0.05f;
	}
}
