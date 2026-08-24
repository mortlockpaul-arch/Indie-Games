using System;
using GKEngine;
using GKEngine.Entities;
using GKEngine.Utils;
using Game.Atoms;
using Game.Entities;
using Game.Grids;
using Game.History;
using Game.Interactable;
using Game.Particles;
using Game.Physics;
using Game.Robots;
using Game.Scenes;
using Game.Scenes.Play.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.QBits;

public class QBit : PhysicsItem
{
	public enum QBitType
	{
		Null = -1,
		Red,
		Green,
		Blue,
		Yellow
	}

	private const string PATH_MODEL = "Content/Models/QBits/Default/Model";

	private const string PATH_MODEL_BRAIN = "Content/Models/QBits/Brain/Model";

	private const float BRAIN_TRAIL_DISTANCE = 20f;

	private const int MOVE_TIME_HOP = 500;

	private const int MOVE_TIME_ROLL = 200;

	private const int MOVE_TIME_PORTAL = 500;

	private const string BILLBOARD_EXIT = "Materials/Sequence/Warp_0";

	private const string BILLBOARD_PORTAL = "Materials/Sequence/Warp_1";

	public static Vector3[] DIRECTIONS = new Vector3[4]
	{
		new Vector3(0f, 0f, -1f),
		new Vector3(-1f, 0f, 0f),
		new Vector3(0f, 0f, 1f),
		new Vector3(1f, 0f, 0f)
	};

	public static string[] TYPE_NAMES = new string[4] { "Red", "Green", "Blue", "Yellow" };

	public static Color[] TYPE_COLORS = new Color[4]
	{
		new Color(212, 0, 64),
		new Color(0, 212, 64),
		new Color(0, 102, 255),
		new Color(212, 190, 0)
	};

	public static Color[] TYPE_COLORS_SHADOW = new Color[4]
	{
		new Color(32, 0, 0),
		new Color(0, 32, 0),
		new Color(0, 0, 32),
		new Color(32, 32, 0)
	};

	public static Color[] TYPE_COLOR_PARTICLE = new Color[4]
	{
		new Color(255, 128, 128),
		new Color(128, 255, 128),
		new Color(128, 128, 255),
		new Color(255, 255, 128)
	};

	public static Color[] TYPE_COLOR_ATOM = new Color[4]
	{
		new Color(255, 0, 0),
		new Color(0, 255, 0),
		new Color(0, 102, 255),
		new Color(255, 200, 0)
	};

	public static Range[] EYES_TIME = new Range[2]
	{
		new Range(3000f, 5000f),
		new Range(100f, 110f)
	};

	private Atom _temp_atom;

	public QBitManager manager;

	public QBitType type;

	protected MaxModel model;

	protected MaxModelPart part;

	protected Effect effect;

	protected EffectParameter effectBones;

	protected EffectParameter effectTime;

	protected EffectParameter effectSticky;

	public Matrix[] skinTransforms;

	protected MaxModelRenderable brain;

	protected Base3D brainOrientation;

	protected MaxModelPart eyeLeft;

	protected MaxModelPart eyeRight;

	protected EffectParameter eyeLeftIndexParam;

	protected EffectParameter eyeRightIndexParam;

	protected int eyeLeftIndex;

	protected int eyeRightIndex;

	protected float eyeLeftTime;

	protected float eyeRightTime;

	protected float eyeLeftTimeTotal;

	protected float eyeRightTimeTotal;

	protected QBitCorner[] corners = new QBitCorner[8];

	public QBitSpeech speech;

	public bool sticky;

	public bool exiting;

	public bool home;

	private Player _player;

	protected float moveHopHeight = 20f;

	protected int moveType;

	protected Vector3 moveDir = default(Vector3);

	protected Vector3 moveAxis = default(Vector3);

	protected AtomSwitch moveSwitch;

	public bool moveSwitchHit;

	public bool moveSwitchSound;

	protected float exitTime;

	protected float exitTimeTotal = 1000f;

	public ParticleEmitter emitter;

	public ParticleEmitter emitterBubbles;

	public ParticleEmitter[] emitterScore;

	private int emitterScoreIndex;

	public ParticleEmitterSchema emitterSchemaSwitch;

	public ParticleEmitterSchema emitterSchemaBubbles;

	public ParticleEmitterSchema emitterSchemaScore;

	private Billboard billboardExit;

	private Billboard billboardPortal;

	public int index => manager.qbits.IndexOf(this);

	public bool busy
	{
		get
		{
			if (!moving && !physicsActive && !exiting)
			{
				return dying;
			}
			return true;
		}
	}

	public bool playable
	{
		get
		{
			if (!dead && !dying && !exiting)
			{
				return !home;
			}
			return false;
		}
	}

	public Player player
	{
		get
		{
			return _player;
		}
		set
		{
			_player = value;
			if (player != null)
			{
				effect.Parameters["Selected"].SetValue(value: true);
			}
			else
			{
				effect.Parameters["Selected"].SetValue(value: false);
			}
		}
	}

	public override bool physicsAlive => playable;

	public QBit(QBitManager oManager, QBitType oType)
	{
		manager = oManager;
		scene = manager.universe.scene;
		type = oType;
		brainOrientation = new Base3D();
		manager.universe.physics.Add(this);
		Load();
	}

	public override void Load()
	{
		model = GameEngine.SceneContent.Load<MaxModel>("Content/Models/QBits/Default/Model").Clone();
		model.Build(this);
		Corners_Set();
		part = model.modelParts[0];
		effect = part.material.effect;
		effectBones = effect.Parameters["Bones"];
		effectTime = effect.Parameters["Time"];
		effectSticky = effect.Parameters["Sticky"];
		effect.Parameters["Color"].SetValue(TYPE_COLORS[(int)type].ToVector4());
		effect.Parameters["ShadowColor"].SetValue(TYPE_COLORS_SHADOW[(int)type].ToVector3());
		Brain_Init();
		Speech_Load();
		manager.renderStack.Add(guid.value, this);
		Particles_Set();
		Billboards_Load();
		SetLighting();
		base.Load();
	}

	public override void Dispose()
	{
		base.Dispose();
		moveSwitch = null;
		corners = null;
		manager = null;
		effect = null;
		effectBones = null;
		skinTransforms = null;
		Billboards_Dispose();
		Speech_Dispose();
		emitter.Dispose();
		emitterBubbles.Dispose();
		emitterScore[0].Dispose();
		emitterScore[1].Dispose();
		emitterScore = null;
		emitter = null;
		emitterBubbles = null;
		emitterSchemaSwitch = null;
		emitterSchemaBubbles = null;
		emitterSchemaScore = null;
		model.Dispose();
		model = null;
		part = null;
		Brain_Dispose();
	}

	public void Update(GameTime oGameTime)
	{
		if (!dead && !home && !historyLocked)
		{
			int milliseconds = oGameTime.ElapsedGameTime.Milliseconds;
			if (dying)
			{
				Death_Update(oGameTime);
			}
			else if (exiting)
			{
				Exit_Update(oGameTime);
			}
			else
			{
				if (moving)
				{
					Move_Update(milliseconds);
				}
				Corners_Update(oGameTime);
				Corners_Populate();
				Death_Check();
			}
			emitterBubbles.position = position;
			Brain_Update(oGameTime);
		}
		Billboards_Update(oGameTime);
		Speech_Update(oGameTime);
	}

	public void SetLighting()
	{
		scene.lights.SetEffect(ref model);
		scene.lights.SetEffect(ref brain.model);
	}

	protected int AngleFromYaw(float xYaw)
	{
		int num = (int)Math.Round(xYaw / ((float)Math.PI / 2f));
		if (num < 0)
		{
			return 4 - Math.Abs(num) % 4;
		}
		return num % 4;
	}

	protected float YawFromAngle(int xAngle)
	{
		return (float)Math.PI / 2f * (float)xAngle;
	}

	public QBit QBitAbove()
	{
		QBit result = null;
		for (int i = 0; i < manager.qbits.Count; i++)
		{
			if (manager.qbits[i] != this && X > manager.qbits[i].X - Grid.SPACING.X && X < manager.qbits[i].X + Grid.SPACING.X && Z > manager.qbits[i].Z - Grid.SPACING.Z && Z < manager.qbits[i].Z + Grid.SPACING.Z && Y + Grid.SPACING.Y > manager.qbits[i].Y - Grid.SPACING.Y && Y + Grid.SPACING.Y < manager.qbits[i].Y + Grid.SPACING.Y)
			{
				result = manager.qbits[i];
				break;
			}
		}
		return result;
	}

	public bool ExactCollide(float xX, float xY, float xZ)
	{
		bool flag = false;
		int digits = 1;
		xX = (float)Math.Round(xX, digits);
		xY = (float)Math.Round(xY, digits);
		xZ = (float)Math.Round(xZ, digits);
		for (int i = 0; i < manager.atoms.lengthAtoms; i++)
		{
			Atom atom = manager.atoms.atoms[i];
			if (!atom.definition.playGrid)
			{
				continue;
			}
			for (int j = 0; j < atom.area.Length; j++)
			{
				if ((float)Math.Round(atom.X + (float)atom.area[j].X * Grid.SPACING.X, digits) == xX && (float)Math.Round(atom.Y + (float)atom.area[j].Y * Grid.SPACING.Y, digits) == xY && (float)Math.Round(atom.Z + (float)atom.area[j].Z * Grid.SPACING.Z, digits) == xZ)
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			for (int i = 0; i < manager.qbits.Count; i++)
			{
				if (manager.qbits[i] != this && manager.qbits[i].X == xX && manager.qbits[i].Y == xY && manager.qbits[i].Z == xZ && !manager.qbits[i].dead && !manager.qbits[i].home)
				{
					flag = true;
					break;
				}
			}
		}
		return flag;
	}

	private void Check_CollideItems()
	{
		for (int i = 0; i < manager.universe.atoms.exits.Count; i++)
		{
			if (Math.Abs(X - manager.universe.atoms.exits[i].X) < Grid.SPACING.X * 0.5f && Math.Abs(Y - manager.universe.atoms.exits[i].Y) < Grid.SPACING.Y * 0.5f && Math.Abs(Z - manager.universe.atoms.exits[i].Z) < Grid.SPACING.Z * 0.5f && ((manager.universe.atoms.exits[i].type > 0 && manager.universe.atoms.exits[i].type - 1 == (int)type) || manager.universe.atoms.exits[i].type == 0))
			{
				Exit();
				break;
			}
		}
		for (int i = 0; i < manager.universe.atoms.collects.Count; i++)
		{
			if (!manager.universe.atoms.collects[i].collected && Math.Abs(X - manager.universe.atoms.collects[i].atom.X) < Grid.SPACING.X * 0.5f && Math.Abs(Y - manager.universe.atoms.collects[i].atom.Y) < Grid.SPACING.Y * 0.5f && Math.Abs(Z - manager.universe.atoms.collects[i].atom.Z) < Grid.SPACING.Z * 0.5f)
			{
				Event_Collect(manager.universe.atoms.collects[i]);
				manager.universe.atoms.collects[i].Collect();
				break;
			}
		}
	}

	private bool Check_HasUnder(int xX, int xY, int xZ)
	{
		bool flag = false;
		flag = manager.grid.NextTop(xX, xY, xZ) > manager.grid.fromY;
		if (!flag)
		{
			flag = physics.NextTop(xX, xY, xZ) > manager.grid.fromY;
		}
		if (!flag)
		{
			flag = manager.atoms.Triggers_FallInto(xX, xY, xZ) != null;
		}
		return flag;
	}

	public bool Check_FallToDeath()
	{
		bool flag = false;
		if (!sticky)
		{
			int num = manager.grid.NextTop(base.gridX, base.gridY - 1, base.gridZ);
			int num2 = physics.NextTop(base.gridX, base.gridY - 1, base.gridZ);
			int num3 = manager.PhasedUnder(this);
			if (num <= manager.grid.fromY && num3 <= manager.grid.fromY)
			{
				flag = true;
			}
			if (!flag && num > num2 && num > num3)
			{
				IGridable gridable = manager.atoms.grid.At(base.gridX, num - 1, base.gridZ);
				if (gridable is Atom && (gridable as Atom).definition.type == AtomDefinition.Type.Pain)
				{
					flag = true;
				}
			}
		}
		return flag;
	}

	public override void PhysicsStart()
	{
		if (!sticky)
		{
			base.PhysicsStart();
		}
	}

	private void Brain_Init()
	{
		float num = 1f;
		brain = new MaxModelRenderable(scene, GameEngine.SceneContent.Load<MaxModel>("Content/Models/QBits/Brain/Model").Clone());
		brain.model.Build(brainOrientation);
		brain.model.modelParts[0].material.effect.Parameters["RimColor"].SetValue(TYPE_COLORS[(int)type].ToVector3());
		scene.RenderStacks_FromName(GameMain.RENDERSTACK_SOLID).Add(brain.guid.value, brain);
		eyeLeft = brain.model.PartFromName("Eye_0");
		eyeRight = brain.model.PartFromName("Eye_1");
		eyeLeftIndexParam = eyeLeft.material.effect.Parameters["Index"];
		eyeRightIndexParam = eyeRight.material.effect.Parameters["Index"];
		num = 0.7f + (float)GameEngine.random.NextDouble() * 0.6f;
		eyeLeft.hasLocal = true;
		eyeLeft.local = Matrix.CreateScale(num);
		eyeRight.hasLocal = true;
		eyeRight.local = Matrix.CreateScale(2f - num);
	}

	public void Brain_Update(GameTime oGameTime)
	{
		float amount = Math.Min((float)oGameTime.ElapsedGameTime.Milliseconds / 100f, 1f);
		float amount2 = Math.Min((float)oGameTime.ElapsedGameTime.Milliseconds / 200f, 1f);
		brainOrientation.X = X;
		Base3D base3D = brainOrientation;
		float y = (brainOrientation.Y = MathHelper.Lerp(brainOrientation.Y, Y, amount));
		base3D.Y = y;
		brainOrientation.Z = Z;
		if (moving)
		{
			if (moveDir != Vector3.Zero)
			{
				Quaternion quaternion = Quaternion.Normalize(Quaternion.CreateFromRotationMatrix(Matrix.CreateBillboard(moveDir, Vector3.Zero, Vector3.Up, Vector3.Forward)));
				brainOrientation.rotation = Quaternion.Lerp(brainOrientation.rotation, quaternion, amount2);
			}
		}
		else
		{
			Quaternion quaternion2 = Quaternion.Normalize(Quaternion.CreateFromRotationMatrix(Matrix.CreateBillboard(manager.universe.scene.cameras.camera.position, brainOrientation.position, Vector3.Up, Vector3.Forward)));
			brainOrientation.rotation = Quaternion.Lerp(brainOrientation.rotation, quaternion2, amount2);
		}
		Brain_Eyes_Update(oGameTime);
		if (visible != brain.model.visible)
		{
			brain.model.visible = visible;
		}
	}

	public void Brain_Eyes_Update(GameTime oGameTime)
	{
		eyeLeftTime += oGameTime.ElapsedGameTime.Milliseconds;
		eyeRightTime += oGameTime.ElapsedGameTime.Milliseconds;
		if (eyeLeftTime >= eyeLeftTimeTotal)
		{
			eyeLeftIndex = ((eyeLeftIndex == 0) ? 1 : 0);
			eyeLeftIndexParam.SetValue(eyeLeftIndex);
			eyeLeftTime = 0f;
			eyeLeftTimeTotal = EYES_TIME[eyeLeftIndex].random;
		}
		if (eyeRightTime >= eyeRightTimeTotal)
		{
			eyeRightIndex = ((eyeRightIndex == 0) ? 1 : 0);
			eyeRightIndexParam.SetValue(eyeRightIndex);
			eyeRightTime = 0f;
			eyeRightTimeTotal = EYES_TIME[eyeRightIndex].random;
		}
	}

	private void Brain_Dispose()
	{
		scene.RenderStacks_FromName(GameMain.RENDERSTACK_SOLID).Remove(brain.guid.value, brain);
		brain.Dispose();
		brain = null;
		eyeLeft = null;
		eyeRight = null;
		eyeLeftIndexParam = null;
		eyeRightIndexParam = null;
	}

	public void Brain_SetPosition()
	{
		brainOrientation.X = X;
		brainOrientation.Y = Y;
		brainOrientation.Z = Z;
	}

	public void Move(Vector3 vDir, int xMoveType, bool xPush)
	{
		bool flag = false;
		IGridable gridable = null;
		PhysicsItem physicsItem = null;
		IGridable gridable2 = null;
		PhysicsItem physicsItem2 = null;
		if (busy || speech.visible)
		{
			return;
		}
		SnapToGrid();
		MathUtils.UnitSnapXZ(ref vDir);
		physicsItem = physics.At(X, Y + Grid.SPACING.Y, Z);
		if (!(vDir.Length() > 0f) || physicsItem != null)
		{
			return;
		}
		gridable = manager.grid.At(base.gridX + (int)vDir.X, base.gridY + (int)vDir.Y, base.gridZ + (int)vDir.Z);
		flag = Check_HasUnder(base.gridX + (int)vDir.X, base.gridY + (int)vDir.Y, base.gridZ + (int)vDir.Z);
		physicsItem = physics.At(X + vDir.X * Grid.SPACING.X, Y + vDir.Y * Grid.SPACING.Y, Z + vDir.Z * Grid.SPACING.Z);
		if (xPush && physicsItem is Crate && !physicsItem.physicsActive && (manager.grid.At(physicsItem.gridX, physicsItem.gridY - 1, physicsItem.gridZ) != null || physics.At(physicsItem.X, physicsItem.Y - Grid.SPACING.Y, physicsItem.Z) != null))
		{
			if (base.gridX + (int)(vDir.X * 2f) < manager.grid.fromX || base.gridX + (int)(vDir.X * 2f) > manager.grid.toX || base.gridY + (int)(vDir.Y * 2f) < manager.grid.fromY || base.gridY + (int)(vDir.Y * 2f) > manager.grid.toY || base.gridZ + (int)(vDir.Z * 2f) < manager.grid.fromZ || base.gridZ + (int)(vDir.Z * 2f) > manager.grid.toZ)
			{
				xPush = false;
			}
			else
			{
				gridable2 = manager.grid.At(base.gridX + (int)(vDir.X * 2f), base.gridY + (int)(vDir.Y * 2f), base.gridZ + (int)(vDir.Z * 2f));
				physicsItem2 = physics.At(X + vDir.X * Grid.SPACING.X * 2f, Y + vDir.Y * Grid.SPACING.Y * 2f, Z + vDir.Z * Grid.SPACING.Z * 2f);
			}
		}
		bool flag2 = false;
		if (gridable == null && flag && (physicsItem == null || physicsItem is Robot || (xPush && physicsItem is Crate && gridable2 == null && physicsItem2 == null && !physicsItem.physicsActive)))
		{
			flag2 = true;
		}
		else if (base.gridY + (int)vDir.Y < manager.grid.toY && !xPush)
		{
			vDir.Y++;
			gridable = manager.grid.At(base.gridX + (int)vDir.X, base.gridY + (int)vDir.Y, base.gridZ + (int)vDir.Z);
			flag = Check_HasUnder(base.gridX + (int)vDir.X, base.gridY + (int)vDir.Y, base.gridZ + (int)vDir.Z);
			physicsItem = physics.At(X + vDir.X * Grid.SPACING.X, Y + vDir.Y * Grid.SPACING.Y, Z + vDir.Z * Grid.SPACING.Z);
			if (gridable == null && flag && (physicsItem == null || physicsItem is Robot))
			{
				flag2 = true;
			}
		}
		if (flag2)
		{
			SnapToGrid();
			moveType = xMoveType;
			Move_Start(base.gridX + (int)vDir.X, base.gridY + (int)vDir.Y, base.gridZ + (int)vDir.Z);
			if (xPush && physicsItem is Crate)
			{
				(scene as PlayScene).audio.EventCues_Trigger("Push");
				(physicsItem as Crate).Move(base.gridX + (int)(vDir.X * 2f), base.gridY + (int)(vDir.Y * 2f), base.gridZ + (int)(vDir.Z * 2f), 200);
			}
		}
	}

	public void Move_Start(int xX, int xY, int xZ)
	{
		if (!busy && QBitAbove() == null)
		{
			if (sticky)
			{
				Sticky_Unstick();
			}
			Event_Move_Start();
			moveTime = 0;
			moveFrom = position;
			moveTo.X = (float)xX * Grid.SPACING.X;
			moveTo.Y = (float)xY * Grid.SPACING.Y;
			moveTo.Z = (float)xZ * Grid.SPACING.Z;
			moveDir.X = Math.Sign(Math.Round(moveTo.X - moveFrom.X));
			moveDir.Y = Math.Sign(Math.Round(moveTo.Y - moveFrom.Y));
			moveDir.Z = Math.Sign(Math.Round(moveTo.Z - moveFrom.Z));
			moveSwitch = manager.atoms.Triggers_Intersect(moveTo) as AtomSwitch;
			moveSwitchHit = true;
			if (moveSwitch != null && Vector3.Dot(moveSwitch.matrix.Up, Vector3.Up) > 0.9f)
			{
				moveType = 0;
				moveTimeTotal = 500;
				moveSwitchSound = false;
			}
			else
			{
				moveType = 1;
				moveTimeTotal = 200;
				moveAxis.X = moveDir.Z;
				moveAxis.Y = 0f;
				moveAxis.Z = moveDir.X * -1f;
				moveAxis = Vector3.Transform(moveAxis, Quaternion.Inverse(rotation));
				moveRotationFrom = rotation;
				moveRotationTo = rotation * Quaternion.CreateFromAxisAngle(moveAxis, (float)Math.PI / 2f);
			}
			moving = true;
		}
	}

	public void Move_Portal(int xX, int xY, int xZ)
	{
		manager.universe.scene.audio.EventCues_Trigger("Portal_In");
		Event_Move_Start();
		moveTime = 0;
		moveFrom = position;
		moveTo.X = (float)xX * Grid.SPACING.X;
		moveTo.Y = (float)xY * Grid.SPACING.Y;
		moveTo.Z = (float)xZ * Grid.SPACING.Z;
		moveRotationFrom = rotation;
		moveType = 2;
		moveTimeTotal = 500;
		billboardPortal.GotoAndPlay(position, 100f, 0, 49, 500f, xLoop: false, xHideOnComplete: false);
		visible = false;
		moving = true;
	}

	public void Move_Update(int elapsed)
	{
		moveTime += elapsed;
		if (moveTime >= moveTimeTotal)
		{
			Move_Done();
			return;
		}
		float ratio = (float)moveTime / (float)moveTimeTotal;
		Move_Lerp(ratio);
	}

	protected void Move_Lerp(float ratio)
	{
		switch (moveType)
		{
		case 0:
			Move_Lerp_Hop(ratio, ref moveFrom, ref moveTo, moveSwitch);
			break;
		case 1:
			Move_Lerp_Roll(ratio, ref moveFrom, ref moveTo, ref moveRotationFrom, ref moveRotationTo);
			break;
		case 2:
			Move_Lerp_Portal(ratio, ref moveFrom, ref moveTo);
			break;
		}
	}

	protected void Move_Lerp_Hop(float ratio, ref Vector3 vFrom, ref Vector3 vTo, AtomSwitch oMoveSwitch)
	{
		X = vFrom.X + (vTo.X - vFrom.X) * ratio;
		Z = vFrom.Z + (vTo.Z - vFrom.Z) * ratio;
		Y = vFrom.Y + (vTo.Y - vFrom.Y) * ratio;
		Y += (float)Math.Sin(Math.PI * (double)ratio) * moveHopHeight;
		if (moveSwitch != null && ratio >= 0.6f)
		{
			moveSwitch.Button_Lerp(1f - (ratio - 0.6f) / 0.4f);
			if (!moveSwitchSound)
			{
				(scene as PlayScene).audio.EventCues_Trigger("Button");
				moveSwitchSound = true;
			}
		}
	}

	protected void Move_Lerp_Roll(float ratio, ref Vector3 vFrom, ref Vector3 vTo, ref Quaternion qFrom, ref Quaternion qTo)
	{
		X = vFrom.X + (vTo.X - vFrom.X) * ratio;
		Z = vFrom.Z + (vTo.Z - vFrom.Z) * ratio;
		float num = (float)Math.Sin(0.7853981852531433 + Math.PI / 2.0 * (double)ratio);
		num -= (float)Math.Sin(Math.PI / 4.0);
		Y = vFrom.Y + (vTo.Y - vFrom.Y) * ratio;
		Y += num * Grid.SPACING.Y;
		rotation = Quaternion.Lerp(qFrom, qTo, ratio);
	}

	protected void Move_Lerp_Portal(float ratio, ref Vector3 vFrom, ref Vector3 vTo)
	{
		X = vFrom.X + (vTo.X - vFrom.X) * ratio;
		Z = vFrom.Z + (vTo.Z - vFrom.Z) * ratio;
		Y = vFrom.Y + (vTo.Y - vFrom.Y) * ratio;
	}

	protected void Move_Done()
	{
		visible = true;
		if (moveType == 2)
		{
			billboardPortal.GotoAndPlay(position, 100f, 0, 49, 500f, xLoop: false, xHideOnComplete: true);
			manager.universe.scene.audio.EventCues_Trigger("Portal_Out");
		}
		Move_Lerp(1f);
		SnapToGrid();
		Corners_SetVelocity((moveTo - moveFrom) / moveTime);
		Event_Move_End();
		moving = false;
		if (!ExactCollide(X, Y - Grid.SPACING.Y, Z) && moveType != 2)
		{
			velocity.Y = (moveTo - moveFrom).Length() / (float)moveTime * -0.9f;
		}
		PhysicsItem physicsItem = physics.At(X, Y - Grid.SPACING.Y, Z);
		if (physicsItem is Robot)
		{
			(physicsItem as Robot).Death();
		}
	}

	private void Sticky_Stick()
	{
		bool flag = false;
		if (sticky)
		{
			return;
		}
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				int num = -1;
				if (num <= 1)
				{
					manager.grid.At(base.gridX + i, base.gridY + j, base.gridZ + num);
					flag = true;
				}
				if (flag)
				{
					break;
				}
			}
			if (flag)
			{
				break;
			}
		}
		if (flag)
		{
			manager.universe.scene.audio.EventCues_Trigger("Phase_In");
			physics.universe.history.Open(this, HistoryItem.Action.Flag);
			sticky = true;
			physics.universe.history.Close(this, HistoryItem.Action.Flag);
			Corners_Stop();
			Sticky_SetEffect();
		}
	}

	private void Sticky_Unstick()
	{
		if (sticky)
		{
			manager.universe.scene.audio.EventCues_Trigger("Phase_Out");
			physics.universe.history.Open(this, HistoryItem.Action.Flag);
			sticky = false;
			physics.universe.history.Close(this, HistoryItem.Action.Flag);
			Sticky_SetEffect();
			Corners_Spaz(new Range(0.1f, 0.5f));
		}
	}

	public void Sticky_Toggle()
	{
		if (sticky)
		{
			Sticky_Unstick();
		}
		else
		{
			Sticky_Stick();
		}
	}

	private void Sticky_SetEffect()
	{
		effectSticky.SetValue(sticky ? 1 : 0);
	}

	public void Corners_Populate()
	{
		for (int i = 1; i < skinTransforms.Length; i++)
		{
			ref Matrix reference = ref skinTransforms[i];
			reference = corners[i - 1].matrix;
		}
	}

	private void Corners_Set()
	{
		skinTransforms = new Matrix[model.bones.Count];
		skinTransforms[0] = default(Matrix);
		for (int i = 1; i < model.bones.Count; i++)
		{
			Base3D base3D = new Base3D(model.bones[i].bind * model.bones[0].bind);
			corners[i - 1] = new QBitCorner(this, base3D.position);
			ref Matrix reference = ref skinTransforms[i];
			reference = corners[i - 1].matrix;
		}
	}

	private void Corners_Update(GameTime oGameTime)
	{
		for (int i = 0; i < corners.Length; i++)
		{
			corners[i].Update(oGameTime);
		}
	}

	private void Corners_SetVelocity(Vector3 vVelocity)
	{
		Vector3 vector = default(Vector3);
		for (int i = 0; i < corners.Length; i++)
		{
			if (corners[i].IsTop())
			{
				vector = Vector3.Normalize(vVelocity);
				float num = (float)GameEngine.random.NextDouble() + 0.5f;
				corners[i].Simulation_Start(vector * (vVelocity.Length() * num));
			}
		}
	}

	public void Corners_Lean(Vector3 vDir)
	{
		if (busy)
		{
			return;
		}
		for (int i = 0; i < corners.Length; i++)
		{
			if (corners[i].IsTop())
			{
				corners[i].Lean(vDir * 20f);
			}
		}
	}

	public void Corners_Release()
	{
		for (int i = 0; i < corners.Length; i++)
		{
			if (corners[i].IsTop())
			{
				corners[i].Release();
			}
		}
	}

	public void Corners_Stop()
	{
		for (int i = 0; i < corners.Length; i++)
		{
			corners[i].Simulation_Stop();
		}
	}

	private void Corners_Spaz(Range oSpazAmount)
	{
		for (int i = 0; i < corners.Length; i++)
		{
			corners[i].Simulation_Start(GameMain.instance.GetRandUnitVecor() * oSpazAmount.random);
		}
	}

	private void Corners_Sicky()
	{
		for (int i = 0; i < corners.Length; i++)
		{
			if (corners[i].IsTop())
			{
				corners[i].position *= 0.5f;
			}
		}
	}

	public void Death_Check()
	{
		if (Y < (float)(manager.grid.fromY - 10) * Grid.SPACING.Y)
		{
			PhysicsStop();
			Death();
		}
	}

	public override void Death()
	{
		if (!dying && !manager.universe.history.reversing)
		{
			Particles_Bubbles_Stop();
			base.Death();
			(scene as PlayScene).audio.EventCues_Trigger("Sound_Splat");
			manager.universe.scene.postSplat.SetTint(TYPE_COLORS[(int)type]);
			manager.universe.scene.postSplat.Anim_In();
			deathTime = 0f;
			visible = false;
		}
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

	protected override void Dead()
	{
		base.Dead();
		PlayScene playScene = scene as PlayScene;
		if (!playScene.universe.Level_EndCheck(this))
		{
			playScene.universe.players.ReassignPlayer(this);
		}
	}

	public void Exit()
	{
		(scene as PlayScene).audio.EventCues_Trigger("Exit");
		Particles_Bubbles_Stop();
		physics.universe.history.Close(this, HistoryItem.Action.Physics);
		if (moving)
		{
			moving = false;
			physics.universe.history.Close(this, HistoryItem.Action.Move);
		}
		physicsCheckActive = false;
		physicsActive = false;
		exitTime = 0f;
		visible = false;
		billboardExit.GotoAndPlay(position, 100f, 1, 49, 1000f, xLoop: false, xHideOnComplete: true);
		manager.universe.history.Open(this, HistoryItem.Action.Exit);
		exiting = true;
	}

	private void Exit_Update(GameTime oGameTime)
	{
		exitTime += oGameTime.ElapsedGameTime.Milliseconds;
		if (exitTime >= exitTimeTotal)
		{
			Home();
		}
		else
		{
			Exit_Lerp(exitTime / exitTimeTotal);
		}
	}

	private void Exit_Lerp(float xRatio)
	{
	}

	private void Home()
	{
		manager.universe.history.Close(this, HistoryItem.Action.Exit);
		exiting = false;
		home = true;
		position = PhysicsItem.FARAWAY;
		visible = false;
		if (!manager.universe.Level_EndCheck(this))
		{
			manager.universe.players.ReassignPlayer(this);
		}
	}

	public override void Render(GameTime oGameTime)
	{
		if (visible)
		{
			effectBones.SetValue(skinTransforms);
			effectTime.SetValue((float)oGameTime.TotalGameTime.TotalMilliseconds);
			model.Render(scene.cameras.camera);
		}
	}

	public void RenderEffect(ref Effect oEffect)
	{
		if (visible)
		{
			oEffect.Parameters["Bones"].SetValue(skinTransforms);
			model.RenderEffect(ref oEffect);
		}
	}

	public override void History_Set(ref HistoryItemData oItem, HistoryItem.Action oAction)
	{
		base.History_Set(ref oItem, oAction);
		if (oAction == HistoryItem.Action.Move)
		{
			oItem.index = moveType;
		}
		if (oAction == HistoryItem.Action.Exit)
		{
			oItem.position = position;
			oItem.rotation = rotation;
		}
		if (oAction == HistoryItem.Action.Flag)
		{
			oItem.flag = sticky;
			oItem.position = position;
		}
	}

	public override void History_Reverse(ref HistoryItem oItem, float xRatio, GameTime oGameTime)
	{
		base.History_Reverse(ref oItem, xRatio, oGameTime);
		if (oItem.action == HistoryItem.Action.Exit)
		{
			History_Reverse_Exit_Lerp(ref oItem, xRatio, oGameTime);
		}
		_ = oItem.action;
		_ = 9;
	}

	protected override void History_Reverse_Move_Lerp(ref HistoryItem oItem, float xRatio, GameTime oGameTime)
	{
		switch (oItem.start.index)
		{
		case 0:
			Move_Lerp_Hop(1f - xRatio, ref oItem.start.position, ref oItem.end.position, moveSwitch);
			break;
		case 1:
			Move_Lerp_Roll(1f - xRatio, ref oItem.start.position, ref oItem.end.position, ref oItem.start.rotation, ref oItem.end.rotation);
			break;
		case 2:
			Move_Lerp_Portal(1f - xRatio, ref oItem.start.position, ref oItem.end.position);
			break;
		}
		History_Refresh(oGameTime);
	}

	protected override void History_Reverse_Physics_Lerp(ref HistoryItem oItem, float xRatio, GameTime oGameTime)
	{
		base.History_Reverse_Physics_Lerp(ref oItem, xRatio, oGameTime);
		History_Refresh(oGameTime);
	}

	protected override void History_Reverse_Death_Lerp(ref HistoryItem oItem, float xRatio, GameTime oGameTime)
	{
		base.History_Reverse_Death_Lerp(ref oItem, xRatio, oGameTime);
		History_Refresh(oGameTime);
	}

	protected void History_Reverse_Exit_Lerp(ref HistoryItem oItem, float xRatio, GameTime oGameTime)
	{
		position = oItem.start.position;
		rotation = oItem.start.rotation;
		visible = true;
		scaleX = xRatio;
		scaleY = xRatio;
		scaleZ = xRatio;
		History_Refresh(oGameTime);
	}

	public override void History_Event_Reverse_End(ref HistoryItem oItem)
	{
		base.History_Event_Reverse_End(ref oItem);
		if (oItem.action == HistoryItem.Action.Exit)
		{
			exiting = false;
			home = false;
			visible = true;
			SnapToGrid();
			Particles_Bubbles_Start();
		}
		else if (oItem.action == HistoryItem.Action.Death)
		{
			Particles_Bubbles_Start();
		}
		else if (oItem.action == HistoryItem.Action.Flag)
		{
			sticky = oItem.start.flag;
			position = oItem.start.position;
			physicsActive = !sticky;
			physicsCheckActive = sticky;
			Sticky_SetEffect();
		}
	}

	public override bool History_IsNotInteruptable(HistoryItem.Action oAction)
	{
		if (!base.History_IsNotInteruptable(oAction))
		{
			return oAction == HistoryItem.Action.Exit;
		}
		return true;
	}

	protected void History_Refresh(GameTime oGameTime)
	{
		Corners_Populate();
		if (player != null)
		{
			player.QBit_Update(oGameTime);
			brainOrientation.position = _position;
			brainOrientation.rotation = _rotation;
			emitterBubbles.position = _position;
			player.manager.camera.Refresh();
		}
	}

	public static QBit FromAtom(QBitManager oManager, AtomQBitDefinition oDef)
	{
		return new QBit(oManager, oDef.qbitType);
	}

	public void Billboards_Load()
	{
		billboardExit = new Billboard(scene, "Materials/Sheets/Exit", 8, 50, scene.RenderStacks_FromName(GameMain.RENDERSTACK_ADD));
		billboardPortal = new Billboard(scene, "Materials/Sheets/Portal", 8, 50, scene.RenderStacks_FromName(GameMain.RENDERSTACK_ADD));
	}

	public void Billboards_Dispose()
	{
		billboardExit.Dispose();
		billboardPortal.Dispose();
		billboardExit = null;
		billboardPortal = null;
	}

	public void Billboards_Update(GameTime oGameTime)
	{
		billboardExit.Update(oGameTime.ElapsedGameTime);
		billboardPortal.Update(oGameTime.ElapsedGameTime);
	}

	public void Speech_Load()
	{
		speech = new QBitSpeech(this);
		speech.Load();
	}

	public void Speech_Dispose()
	{
		speech.Dispose();
		speech = null;
	}

	public void Speech_Update(GameTime oGameTime)
	{
		speech.Update(oGameTime);
		if (dead || dying || physicsFalling || exiting || home)
		{
			speech.Halt();
		}
	}

	public void Particles_Set()
	{
		emitter = new ParticleEmitter(scene, ParticleEmitter.EffectType.Default, GameMain.RENDERSTACK_ADD);
		emitterBubbles = new ParticleEmitter(scene, ParticleEmitter.EffectType.Default, GameMain.RENDERSTACK_ADD_FIRST);
		emitterScore = new ParticleEmitter[2]
		{
			new ParticleEmitter(scene, ParticleEmitter.EffectType.Default, GameMain.RENDERSTACK_ADD),
			new ParticleEmitter(scene, ParticleEmitter.EffectType.Default, GameMain.RENDERSTACK_ADD)
		};
		emitterScoreIndex = 0;
		emitterSchemaSwitch = new ParticleEmitterSchema(50);
		emitterSchemaSwitch.mode = ParticleEmitter.Mode.OneShot;
		emitterSchemaSwitch.rotationStart = 0f;
		emitterSchemaSwitch.rotationEnd = (float)Math.PI * 2f;
		emitterSchemaSwitch.rotationTween = 0;
		emitterSchemaSwitch.scaleStart = 0f;
		emitterSchemaSwitch.scaleEnd = 20f;
		emitterSchemaSwitch.scaleTween = 1;
		emitterSchemaSwitch.textureStart = scene.library.texture2Ds["TextureDiffuse_Particles_Ember_0"];
		emitterSchemaSwitch.textureEnd = scene.library.texture2Ds["TextureDiffuse_Particles_Ember_1"];
		emitterSchemaSwitch.textureTween = 1;
		emitterSchemaSwitch.tintStart = TYPE_COLOR_PARTICLE[(int)type];
		emitterSchemaSwitch.tintEnd = new Color(0, 0, 0, 0);
		emitterSchemaSwitch.tintTween = 0;
		emitterSchemaSwitch.tween = 2;
		emitterSchemaSwitch.Float_Random(ref emitterSchemaSwitch.data, 0u, 0f, 0f);
		emitterSchemaSwitch.Float_Random(ref emitterSchemaSwitch.data, 1u, 1f, 1.5f);
		emitterSchemaSwitch.Float_Random(ref emitterSchemaSwitch.data, 2u, 0.3f, 1f);
		emitterSchemaSwitch.Vector_Constant(ref emitterSchemaSwitch.positions, Vector3.Zero);
		emitterSchemaSwitch.Vector_Random(ref emitterSchemaSwitch.deltas, new Vector3(0f, 20f, 0f), 15f, 40f);
		emitterSchemaBubbles = new ParticleEmitterSchema(20);
		emitterSchemaBubbles.mode = ParticleEmitter.Mode.Loop;
		emitterSchemaBubbles.rotationStart = 0f;
		emitterSchemaBubbles.rotationEnd = 0f;
		emitterSchemaBubbles.rotationTween = -1;
		emitterSchemaBubbles.scaleStart = 1f;
		emitterSchemaBubbles.scaleEnd = 2f;
		emitterSchemaBubbles.scaleTween = 0;
		emitterSchemaBubbles.textureStart = GameEngine.SceneContent.Load<Texture2D>("Content/Materials/Common/TextureDiffuse_Particles_Bubble_0");
		emitterSchemaBubbles.textureEnd = emitterSchemaBubbles.textureStart;
		emitterSchemaBubbles.textureTween = -1;
		emitterSchemaBubbles.tintStart = TYPE_COLOR_ATOM[(int)type];
		emitterSchemaBubbles.tintEnd = TYPE_COLOR_ATOM[(int)type];
		emitterSchemaBubbles.tintEnd.A = 128;
		emitterSchemaBubbles.tintTween = 0;
		emitterSchemaBubbles.tween = 4;
		emitterSchemaBubbles.Float_Random(ref emitterSchemaBubbles.data, 0u, 0f, 0f);
		emitterSchemaBubbles.Float_Random(ref emitterSchemaBubbles.data, 1u, 0.5f, 1f);
		emitterSchemaBubbles.Float_Random(ref emitterSchemaBubbles.data, 2u, 0.3f, 1f);
		emitterSchemaBubbles.Vector_Random_XZ(ref emitterSchemaBubbles.positions, new Vector3(0f, -10f, 0f), new Range(-9f, 9f), new Range(-9f, 9f));
		emitterSchemaBubbles.Vector_RandomRay(ref emitterSchemaBubbles.deltas, Vector3.Zero, Vector3.Up, 18f);
		emitterSchemaScore = new ParticleEmitterSchema(100);
		emitterSchemaScore.mode = ParticleEmitter.Mode.OneShot;
		emitterSchemaScore.rotationStart = 0f;
		emitterSchemaScore.rotationEnd = (float)Math.PI * 2f;
		emitterSchemaScore.rotationTween = 0;
		emitterSchemaScore.scaleStart = 1f;
		emitterSchemaScore.scaleEnd = 9f;
		emitterSchemaScore.scaleTween = 2;
		emitterSchemaScore.textureStart = scene.library.texture2Ds["TextureDiffuse_Particles_Stars_2"];
		emitterSchemaScore.textureEnd = scene.library.texture2Ds["TextureDiffuse_Particles_Stars_3"];
		emitterSchemaScore.textureTween = 0;
		emitterSchemaScore.tintStart = new Color(255, 255, 255, 255);
		emitterSchemaScore.tintEnd = new Color(255, 255, 255, 0);
		emitterSchemaScore.tintTween = 1;
		emitterSchemaScore.tween = 2;
		emitterSchemaScore.Float_Random(ref emitterSchemaScore.data, 0u, 0f, 1f);
		emitterSchemaScore.Float_Random(ref emitterSchemaScore.data, 1u, 1f, 3f);
		emitterSchemaScore.Float_Random(ref emitterSchemaScore.data, 2u, 0.5f, 3f);
		emitterSchemaScore.Vector_Constant(ref emitterSchemaScore.positions, Vector3.Zero);
		emitterSchemaScore.Vector_Random(ref emitterSchemaScore.deltas, new Vector3(0f, 20f, 0f), 20f, 30f);
		Particles_Bubbles_Start();
	}

	private void Particles_Bubbles_Start()
	{
		emitterBubbles.Start(5000f, emitterSchemaBubbles);
	}

	private void Particles_Bubbles_Stop()
	{
		emitterBubbles.Stop();
	}

	public void Particles_Switch_Start()
	{
		emitter.position = position;
		emitter.Start(750f, emitterSchemaSwitch);
	}

	protected override void Event_Move_End()
	{
		_temp_atom = manager.universe.atoms.Triggers_Intersect(physicsPreviousPosition);
		if (_temp_atom is AtomSwitch)
		{
			(_temp_atom as AtomSwitch).StateCheckIfOn();
		}
		manager.At(X, Y, Z, this)?.Death();
		base.Event_Move_End();
		PhysicsStart();
	}

	protected override void Event_Physics_End()
	{
		base.Event_Physics_End();
		Corners_SetVelocity(velocityCollide);
		PhysicsItem physicsItem = physics.At(X, Y - Grid.SPACING.Y, Z);
		manager.Event_Physics_End(this);
		(scene as PlayScene).audio.EventCues_Trigger("Sound_Squish");
		if (physicsItem is Robot)
		{
			(physicsItem as Robot).Death();
			PhysicsStart();
		}
	}

	public override void Event_Physics_Update(float elapsed)
	{
		base.Event_Physics_Update(elapsed);
		Check_CollideItems();
		manager.atoms.Event_QBit_Moved(this);
	}

	public void Event_Collect(ICollectable oCollect)
	{
		emitterScoreIndex = ((emitterScoreIndex == 0) ? 1 : 0);
		(scene as PlayScene).audio.EventCues_Trigger("Sound_Collect");
		AtomCollectDefinition atomCollectDefinition = oCollect.atom.definition as AtomCollectDefinition;
		emitterSchemaScore.tintStart = Color.White;
		emitterSchemaScore.tintEnd = AtomCollect.COLORS[oCollect.type];
		float[] array = new float[4] { 10f, 15f, 30f, 7f };
		emitterSchemaScore.scaleStart = 0f;
		emitterSchemaScore.scaleEnd = array[oCollect.type];
		emitterSchemaScore.Float_Random(ref emitterSchemaScore.data, 0u, 0f, 1f);
		emitterSchemaScore.Float_Random(ref emitterSchemaScore.data, 1u, 1f, 3f);
		emitterSchemaScore.Float_Random(ref emitterSchemaScore.data, 2u, 0.5f, 3f);
		float[] array2 = new float[4] { 500f, 750f, 1100f, 500f };
		emitterScore[emitterScoreIndex].position = position;
		emitterScore[emitterScoreIndex].Start(array2[oCollect.type], emitterSchemaScore);
		manager.universe.players.ui.scoreItems.Add(atomCollectDefinition.value, oCollect.atom.position);
		manager.universe.jems++;
	}

	public override void Event_Flip_Update()
	{
		base.Event_Flip_Update();
		Corners_Populate();
		emitterBubbles.position = position;
		brainOrientation.position = position;
	}

	public override void Event_Flip_End()
	{
		base.Event_Flip_End();
		Corners_Populate();
		SnapToGrid();
		moveSwitchHit = false;
	}

	protected override void Event_Physics_Start()
	{
		base.Event_Physics_Start();
	}

	public void Event_Switched(Player pPlayer)
	{
		player = null;
	}
}
