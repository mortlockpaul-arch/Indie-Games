using System;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectMercury.Renderers;

namespace Platformer1;

internal class Brick
{
	public const float PhysicsScaleDown = 0.2f;

	public const int PointValue = 30;

	public float PhysicsScaleUp = 5f;

	public PlatformerGame MainGame;

	public bool Active = true;

	public Texture2D texture;

	private Texture2D textureHide;

	public Texture2D textureBlood;

	private Color BloodColor;

	private SpriteEffects BloodFlip;

	private Vector2 origin;

	private Vector2 Hideorigin;

	private SoundEffect collectedSound;

	private float Density = 10000f;

	public CollisionCategory _collidesWith = CollisionCategory.All;

	public CollisionCategory _collisionCategory = CollisionCategory.Cat31;

	private Random random = new Random(354668);

	private World physicsSimulator;

	private float Rotation;

	public int LevelDataIndex;

	private string SharpString;

	private Color _borderColor = Color.Black;

	public Fixture SharpBody;

	public Fixture HideBody;

	private Fixture HideBodyBackend;

	public int ObjectType;

	public string ObjectSubType;

	public bool PopOutGo;

	public bool PopOutGoSaw;

	public bool PopOut;

	public bool DartShoot;

	public bool RayCastDartShoot;

	public Fixture RaycastDartHit;

	public bool DartShootGo;

	public bool FirstHit;

	private bool DartReset = true;

	private Color _color = Color.White;

	private double OldGameTime;

	private SpriteEffects TextureFlipEffects;

	private PlatformerGame mainGame;

	public SpriteFont Font;

	public string Data;

	public Vector2 cameraTransformOld;

	public float mass = 1f;

	private Vector2 AveragePlayerPosition;

	private Vector2 CamPosition;

	private string BrickString;

	private string BrickTypeString;

	private float Scale = 1f;

	public Fixture BrickBody;

	public Fixture ExplosionBody;

	public Fixture ExplosionFixtureB;

	public float BreakPoint = 0.01f;

	public float MaxImpulse = 500f;

	private FixedRevoluteJoint FixedRevJoint;

	private FixedAngleJoint FixedAngleJoint;

	public string ObjectTypeSub;

	private int _count = 2;

	private Vector2 _endPosition;

	private Vector2 PhysicsPosition;

	private int _radius = 100;

	private Vector2 _startPosition;

	private SoundEffect Explosion1;

	private SoundEffect Explosion2;

	private SoundEffect Explosion3;

	private SoundEffect Explosion4;

	private int ExplosionSound;

	private SoundEffect Freeze1;

	private SoundEffect Freeze2;

	private SoundEffect Freeze3;

	private SoundEffect Freeze4;

	private int FreezeSound;

	private SoundEffect Grinder1;

	private SoundEffect Grinder2;

	private SoundEffect Grinder3;

	private SoundEffect Grinder4;

	private int GrinderSound;

	private bool ActiveMine;

	private float ExplosionRandStr;

	public bool Explode;

	private Texture2D _ExplosionTexture;

	private Texture2D BlownMineTexture;

	private Vector2 _ExplosionOrigin;

	private double gameTimeOld;

	private bool Exploded;

	private int ExplosionFrame;

	public float ExplosionPower = 5000f;

	private bool MineTripped;

	private double ExplosionTimer;

	private bool Smoking;

	private float FreezeRandStr;

	public bool Freeze;

	public Fixture _FreezeBody;

	private Texture2D _FreezeTexture;

	private Vector2 _FreezeOrigin;

	private bool Freezed;

	private int FreezeFrame;

	private float GrinderRandStr;

	public bool Grinder;

	private Texture2D _GrinderTexture;

	private Vector2 _GrinderOrigin;

	private float GrinderTorqe;

	private bool Grinded;

	private int GrinderFrame;

	private bool VBrick;

	private RenderTarget2D _DecalRenderer;

	public readonly Color Color = Color.Yellow;

	private Vector2 basePosition;

	private float bounce;

	private Level level;

	private ContentManager content;

	public Level Level => level;

	public ContentManager Content => content;

	public Vector2 Position => basePosition + new Vector2(PhysicsPosition.X, PhysicsPosition.Y);

	public Circle BoundingCircle => new Circle(Position, 21.333334f);

	public Brick(ContentManager content, Level Level, PlatformerGame mainGame, Vector2 position, World physicsSimulator, string BrickType, int BrickMainType, float rot, Renderer renderer, int LevelDataIndex)
	{
		level = Level;
		ObjectSubType = BrickType;
		this.content = content;
		basePosition = position;
		Rotation = rot;
		this.LevelDataIndex = LevelDataIndex;
		MainGame = mainGame;
		ObjectType = BrickMainType;
		ObjectTypeSub = BrickType;
		this.mainGame = mainGame;
		this.content = Content;
		basePosition = position;
		Rotation = rot;
		TextureFlipEffects = SpriteEffects.None;
		renderer.LoadContent(content);
		if (BrickMainType == 2)
		{
			switch (BrickType)
			{
			case "0":
				LoadRegular(physicsSimulator, rot);
				break;
			case "Regular":
				LoadRegular(physicsSimulator, rot);
				break;
			case "1":
				LoadBigBrick(physicsSimulator, rot);
				break;
			case "2":
				LoadBrickBeam(physicsSimulator, rot);
				break;
			case "3":
				LoadBrickBall(physicsSimulator, rot);
				break;
			case "4":
				LoadStone(physicsSimulator, rot);
				break;
			case "5":
				LoadBigStone(physicsSimulator, rot);
				break;
			case "6":
				LoadStoneBeam(physicsSimulator, rot);
				break;
			case "7":
				LoadStoneBall(physicsSimulator, rot);
				break;
			default:
				LoadRegular(physicsSimulator, rot);
				break;
			}
		}
		else
		{
			switch (BrickType)
			{
			case "0":
				LoadMine(physicsSimulator, rot);
				break;
			case "1":
				LoadRegularSpike(physicsSimulator, rot);
				break;
			case "2":
				LoadGrinder(physicsSimulator, rot);
				break;
			default:
				LoadMine(physicsSimulator, rot);
				break;
			}
		}
	}

	private string LoadRegBrick(int variationCount)
	{
		if (level != null)
		{
			BrickString = "Bricks/0/" + level.random.Next(0, variationCount);
		}
		else
		{
			BrickString = "Bricks/0/0";
		}
		return BrickString;
	}

	private string LoadGrinderBrick(int variationCount)
	{
		if (level != null)
		{
			BrickString = "Bricks/Grinder/" + level.random.Next(0, variationCount);
		}
		else
		{
			BrickString = "Bricks/Grinder/0";
		}
		return BrickString;
	}

	public void LoadRegular(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>(LoadRegBrick(6));
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		BrickBody = FixtureFactory.CreateRectangle(physicsSimulator, 12.8f, 9.6f, Density);
		BrickBody.Body.Position = Position * 0.2f;
		BrickBody.Body.Rotation = Rotation;
		BrickBody.Body.BodyType = BodyType.Dynamic;
		BrickBody.Body.UserData = 9;
		BrickBody.UserData = 20;
		BrickBody.Body.SleepingAllowed = true;
		BrickBody.Friction = 1f;
		BrickBody.Restitution = 0f;
		Active = true;
	}

	public void LoadBigBrick(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>(LoadRegBrick(6));
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 2f;
		BrickBody = FixtureFactory.CreateRectangle(physicsSimulator, 25.6f, 19.2f, Density);
		BrickBody.Body.Position = Position * 0.2f;
		BrickBody.Body.Rotation = Rotation;
		BrickBody.Body.BodyType = BodyType.Dynamic;
		BrickBody.Body.UserData = 9;
		BrickBody.UserData = 20;
		BrickBody.Body.SleepingAllowed = true;
		BrickBody.Friction = 1f;
		BrickBody.Restitution = 0f;
		Active = true;
	}

	public void LoadBrickBeam(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>("Bricks/Beam/0");
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		BrickBody = FixtureFactory.CreateRectangle(physicsSimulator, 38.4f, 9.6f, Density);
		BrickBody.Body.Position = Position * 0.2f;
		BrickBody.Body.Rotation = Rotation;
		BrickBody.Body.BodyType = BodyType.Dynamic;
		BrickBody.Body.UserData = 9;
		BrickBody.UserData = 20;
		BrickBody.Body.SleepingAllowed = true;
		BrickBody.Friction = 1f;
		BrickBody.Restitution = 0f;
	}

	public void LoadBrickBall(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>("Bricks/Ball/0");
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		BrickBody = FixtureFactory.CreateCircle(physicsSimulator, 20f, Density);
		BrickBody.Body.Position = Position * 0.2f;
		BrickBody.Body.Rotation = Rotation;
		BrickBody.Body.BodyType = BodyType.Dynamic;
		BrickBody.Body.UserData = 9;
		BrickBody.UserData = 20;
		BrickBody.Body.SleepingAllowed = true;
		BrickBody.Friction = 1f;
		BrickBody.Restitution = 0f;
		Active = true;
	}

	public void LoadStone(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>("Bricks/Stone/0");
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		Density = 100000f;
		BrickBody = FixtureFactory.CreateRectangle(physicsSimulator, 12.8f, 9.6f, Density);
		BrickBody.Body.Position = Position * 0.2f;
		BrickBody.Body.Rotation = Rotation;
		BrickBody.Body.BodyType = BodyType.Dynamic;
		BrickBody.Body.UserData = 13;
		BrickBody.UserData = 20;
		BrickBody.Body.SleepingAllowed = true;
		BrickBody.Friction = 1f;
		BrickBody.Restitution = 0f;
		Active = true;
	}

	public void LoadBigStone(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>("Bricks/Stone/0");
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 2f;
		Density = 100000f;
		BrickBody = FixtureFactory.CreateRectangle(physicsSimulator, 25.6f, 19.2f, Density);
		BrickBody.Body.Position = Position * 0.2f;
		BrickBody.Body.Rotation = Rotation;
		BrickBody.Body.BodyType = BodyType.Dynamic;
		BrickBody.Body.UserData = 13;
		BrickBody.UserData = 20;
		BrickBody.Body.SleepingAllowed = true;
		BrickBody.Friction = 1f;
		BrickBody.Restitution = 0f;
		Active = true;
	}

	public void LoadStoneBall(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>("Bricks/Stone/Ball/0");
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		Density = 100000f;
		BrickBody = FixtureFactory.CreateCircle(physicsSimulator, 20f, Density);
		BrickBody.Body.Position = Position * 0.2f;
		BrickBody.Body.Rotation = Rotation;
		BrickBody.Body.BodyType = BodyType.Dynamic;
		BrickBody.Body.UserData = 13;
		BrickBody.UserData = 20;
		BrickBody.Body.SleepingAllowed = true;
		BrickBody.Friction = 1f;
		BrickBody.Restitution = 0f;
		Active = true;
	}

	public void LoadStoneBeam(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>("Bricks/Stone/Beams/0");
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		Density = 90000f;
		BrickBody = FixtureFactory.CreateRectangle(physicsSimulator, 51.2f, 9.6f, Density);
		BrickBody.Body.Position = Position * 0.2f;
		BrickBody.Body.Rotation = Rotation;
		BrickBody.Body.BodyType = BodyType.Dynamic;
		BrickBody.Body.UserData = 13;
		BrickBody.UserData = 20;
		BrickBody.Body.SleepingAllowed = true;
		BrickBody.Friction = 1f;
		BrickBody.Restitution = 0f;
		BrickBody.Body.IsBullet = true;
		BrickBody.Body.AngularDamping = 0.9f;
		BrickBody.Body.LinearDamping = 0.9f;
		BrickBody.CollidesWith = CollisionCategory.Cat2;
		BrickBody.CollidesWith = CollisionCategory.Cat17;
		BrickBody.CollisionCategories = CollisionCategory.Cat17;
		Active = true;
	}

	public void LoadBall(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>("Bricks/Ball/0");
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 0.45f;
		Density = 1E-06f;
		BrickBody = FixtureFactory.CreateCircle(physicsSimulator, 9.6f, Density);
		BrickBody.Body.Position = Position * 0.2f;
		BrickBody.Body.Rotation = Rotation;
		BrickBody.Body.BodyType = BodyType.Dynamic;
		BrickBody.Body.UserData = 999;
		BrickBody.UserData = 20;
		BrickBody.Body.SleepingAllowed = true;
		BrickBody.Friction = 1f;
		BrickBody.Restitution = 1f;
	}

	public void LoadMine(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>("Bricks/Mines/0");
		BlownMineTexture = Content.Load<Texture2D>("Bricks/Mines/Blown/0");
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		BrickBody = FixtureFactory.CreateRectangle(physicsSimulator, 12.8f, 9.6f, Density);
		BrickBody.Body.Position = Position * 0.2f;
		BrickBody.Body.Rotation = Rotation;
		BrickBody.Body.BodyType = BodyType.Dynamic;
		BrickBody.Body.UserData = 9;
		BrickBody.UserData = 20;
		BrickBody.Body.SleepingAllowed = true;
		BrickBody.Friction = 1f;
		BrickBody.Restitution = 0f;
		Fixture brickBody = BrickBody;
		brickBody.OnCollision = (CollisionEventHandler)Delegate.Combine(brickBody.OnCollision, new CollisionEventHandler(OnCollision_Mine));
		ExplosionBody = FixtureFactory.CreateCircle(physicsSimulator, 48f, 1E-06f);
		ExplosionBody.Body.Position = Position * 0.2f;
		ExplosionBody.Body.Rotation = Rotation;
		ExplosionBody.Body.BodyType = BodyType.Dynamic;
		ExplosionBody.Body.UserData = 9;
		ExplosionBody.UserData = 20;
		ExplosionBody.Body.Active = false;
		ExplosionBody.IsSensor = true;
		Fixture explosionBody = ExplosionBody;
		explosionBody.OnCollision = (CollisionEventHandler)Delegate.Combine(explosionBody.OnCollision, new CollisionEventHandler(OnCollision_Mine_Explosion));
		Explosion1 = Content.Load<SoundEffect>("SoundEffects/Grenade3");
		Explosion2 = Content.Load<SoundEffect>("SoundEffects/explosion");
		Explosion3 = Content.Load<SoundEffect>("SoundEffects/bomb-03");
		Explode = true;
		ActiveMine = true;
		Active = true;
	}

	public void LoadGrinder(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>(LoadGrinderBrick(6));
		BlownMineTexture = Content.Load<Texture2D>(LoadGrinderBrick(6));
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		Density = 1E-05f;
		BrickBody = FixtureFactory.CreateRectangle(physicsSimulator, 12.8f, 9.6f, Density);
		BrickBody.Body.Position = Position * 0.2f;
		BrickBody.Body.Rotation = Rotation;
		BrickBody.Body.BodyType = BodyType.Dynamic;
		BrickBody.Body.UserData = 98;
		BrickBody.UserData = 20;
		BrickBody.Body.SleepingAllowed = true;
		BrickBody.Friction = 0f;
		BrickBody.Restitution = 0.6f;
		Fixture brickBody = BrickBody;
		brickBody.OnCollision = (CollisionEventHandler)Delegate.Combine(brickBody.OnCollision, new CollisionEventHandler(OnCollision_Grinder));
		Explosion1 = Content.Load<SoundEffect>("SoundEffects/chainsaw");
		Explode = true;
		ActiveMine = true;
		Active = true;
	}

	public void LoadRegularSpike(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>("Bricks/Spike/0");
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		Density = 10f;
		BrickBody = FixtureFactory.CreateCircle(physicsSimulator, 6.4f, Density);
		BrickBody.Body.Position = Position * 0.2f;
		BrickBody.Body.Rotation = Rotation;
		BrickBody.Body.BodyType = BodyType.Dynamic;
		BrickBody.UserData = 20;
		BrickBody.Body.UserData = 97;
		BrickBody.Body.SleepingAllowed = true;
		BrickBody.Friction = 1f;
		BrickBody.Restitution = 0.1f;
		BrickBody.CollisionGroup = 98;
		Active = true;
	}

	private bool OnCollision_Ball(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		BrickBody.CollisionGroup = 0;
		return true;
	}

	private bool OnCollision_Mine(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (ActiveMine && fixtureB.Body != null && (int)fixtureB.Body.UserData == 8)
		{
			int num = random.Next(0, 100);
			if (num <= 33)
			{
				Explosion1.Play(mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			else if (num <= 66)
			{
				Explosion2.Play(mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			else if (num <= 100)
			{
				Explosion3.Play(mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			ExplosionBody.Body.Active = true;
			ActiveMine = false;
			texture = BlownMineTexture;
			if (mainGame.level != null)
			{
				mainGame.level.MineExplodeEffect[0].TriggerOffset = BrickBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				mainGame.level.MineExplodeEffect[1].TriggerOffset = BrickBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				mainGame.level.MineExplodeEffect[2].TriggerOffset = BrickBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				mainGame.level.MineExplodeEffect[3].TriggerOffset = BrickBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				mainGame.level.MineExplodeEffect.Trigger(new Vector2(0f, 0f));
			}
		}
		return true;
	}

	private bool OnCollision_Grinder(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (ActiveMine && fixtureB.Body != null && (int)fixtureB.Body.UserData == 8)
		{
			Explosion1.Play(mainGame.Sound_Effect_Volume, 0f, 0f);
			BrickBody.Body.UserData = 122;
			ActiveMine = false;
		}
		return true;
	}

	private bool OnCollision_Mine_Explosion(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		ExplosionFixtureB = fixtureB;
		return true;
	}

	public void RemoveAll(World _world)
	{
		Active = false;
		if (BrickBody != null && BrickBody.Body != null && BrickBody.Body.FixtureList != null)
		{
			_world.RemoveBody(BrickBody.Body);
		}
		if (ExplosionBody != null && ExplosionBody.Body != null && ExplosionBody.Body.FixtureList != null)
		{
			_world.RemoveBody(ExplosionBody.Body);
		}
	}

	public void ActiveAll_True(World _world)
	{
		if (BrickBody != null && BrickBody.Body != null && !BrickBody.Body.Active && BrickBody.Body.FixtureList != null)
		{
			BrickBody.Body.Active = true;
		}
		if (ExplosionBody != null && ExplosionBody.Body != null && !ExplosionBody.Body.Active && ExplosionBody.Body.FixtureList != null)
		{
			ExplosionBody.Body.Active = true;
		}
	}

	public void ActiveAll_False(World _world)
	{
		if (BrickBody != null && BrickBody.Body != null && BrickBody.Body.Active && BrickBody.Body.FixtureList != null)
		{
			BrickBody.Body.Active = false;
		}
		if (ExplosionBody != null && ExplosionBody.Body != null && ExplosionBody.Body.Active && ExplosionBody.Body.FixtureList != null)
		{
			ExplosionBody.Body.Active = false;
		}
	}

	public void Update(GameTime gameTime, World _world)
	{
		if (Active)
		{
			ActiveAll_True(_world);
			if (ObjectType == 2)
			{
				switch (ObjectSubType)
				{
				case "0":
					UpdateRegular(gameTime, _world);
					break;
				case "Regular":
					UpdateRegular(gameTime, _world);
					break;
				case "1":
					UpdateRegular(gameTime, _world);
					break;
				case "2":
					UpdateRegular(gameTime, _world);
					break;
				case "3":
					UpdateRegular(gameTime, _world);
					break;
				case "4":
					UpdateRegular(gameTime, _world);
					break;
				case "5":
					UpdateRegular(gameTime, _world);
					break;
				case "6":
					UpdateRegular(gameTime, _world);
					break;
				case "7":
					UpdateRegular(gameTime, _world);
					break;
				default:
					UpdateRegular(gameTime, _world);
					break;
				}
			}
			else
			{
				switch (ObjectTypeSub)
				{
				case "0":
					UpdateMine(gameTime, _world);
					break;
				case "1":
					UpdateRegularSpike(gameTime, _world);
					break;
				case "2":
					UpdateGrinder(gameTime, _world);
					break;
				default:
					UpdateMine(gameTime, _world);
					break;
				}
			}
		}
		else
		{
			ActiveAll_False(_world);
		}
	}

	private void UpdateRegular(GameTime gameTime, World _world)
	{
	}

	private void UpdateBall(GameTime gameTime, World _world)
	{
	}

	private void UpdateMine(GameTime gameTime, World _world)
	{
		random = new Random((int)gameTime.TotalGameTime.TotalMilliseconds);
		if (Explode && !ActiveMine && !Exploded)
		{
			if (ExplosionTimer + 500.0 > gameTime.TotalGameTime.TotalMilliseconds)
			{
				ExplosionBody.Body.Position = BrickBody.Body.Position;
				{
					foreach (Body body in _world.BodyList)
					{
						Vector2 point = body.Position;
						float num = 100000000f;
						if (ExplosionBody.TestPoint(ref point))
						{
							body.ApplyForce(new Vector2(1000f, 1000f) - new Vector2(BrickBody.Body.Position.X - body.Position.X, BrickBody.Body.Position.Y - body.Position.Y) * new Vector2(num, num));
						}
					}
					return;
				}
			}
			Exploded = true;
		}
		else
		{
			ExplosionTimer = gameTime.TotalGameTime.TotalMilliseconds;
		}
	}

	private void UpdateStone(GameTime gameTime, World _world)
	{
	}

	private void UpdateStoneBeam(GameTime gameTime, World _world)
	{
	}

	private void UpdateRegularSpike(GameTime gameTime, World _world)
	{
		Vector2 vector = new Vector2(0f, 0f);
		Vector2 vector2 = new Vector2(0f, 0f);
		Vector2 vector3 = new Vector2(0f, 0f);
		Vector2 vector4 = new Vector2(0f, 0f);
		if (mainGame.level == null)
		{
			return;
		}
		if (mainGame.Player1InGame && mainGame.level.Player1[mainGame.level.Player1Index] != null && mainGame.level.Player1[mainGame.level.Player1Index]._bodyBody != null)
		{
			vector = ((!(mainGame.level.Player1[mainGame.level.Player1Index]._bodyBody.Body.Position.X < BrickBody.Body.Position.X)) ? (BrickBody.Body.Position / mainGame.level.Player1[mainGame.level.Player1Index]._bodyBody.Body.Position) : (mainGame.level.Player1[mainGame.level.Player1Index]._bodyBody.Body.Position / BrickBody.Body.Position));
		}
		if (mainGame.Player2InGame && mainGame.level.Player1[mainGame.level.Player2Index] != null && mainGame.level.Player1[mainGame.level.Player2Index]._bodyBody != null)
		{
			vector2 = ((!(mainGame.level.Player1[mainGame.level.Player2Index]._bodyBody.Body.Position.X < BrickBody.Body.Position.X)) ? (BrickBody.Body.Position / mainGame.level.Player1[mainGame.level.Player2Index]._bodyBody.Body.Position) : (mainGame.level.Player1[mainGame.level.Player2Index]._bodyBody.Body.Position / BrickBody.Body.Position));
		}
		if (mainGame.Player3InGame && mainGame.level.Player1[mainGame.level.Player3Index] != null && mainGame.level.Player1[mainGame.level.Player3Index]._bodyBody != null)
		{
			vector3 = ((!(mainGame.level.Player1[mainGame.level.Player3Index]._bodyBody.Body.Position.X < BrickBody.Body.Position.X)) ? (BrickBody.Body.Position / mainGame.level.Player1[mainGame.level.Player3Index]._bodyBody.Body.Position) : (mainGame.level.Player1[mainGame.level.Player3Index]._bodyBody.Body.Position / BrickBody.Body.Position));
		}
		if (mainGame.Player4InGame && mainGame.level.Player1[mainGame.level.Player4Index] != null && mainGame.level.Player1[mainGame.level.Player4Index]._bodyBody != null)
		{
			vector4 = ((!(mainGame.level.Player1[mainGame.level.Player4Index]._bodyBody.Body.Position.X < BrickBody.Body.Position.X)) ? (BrickBody.Body.Position / mainGame.level.Player1[mainGame.level.Player4Index]._bodyBody.Body.Position) : (mainGame.level.Player1[mainGame.level.Player4Index]._bodyBody.Body.Position / BrickBody.Body.Position));
		}
		if (((vector.X > vector2.X) & (vector.X > vector3.X) & (vector.X > vector4.X)) && mainGame.level.Player1[mainGame.level.Player1Index] != null && mainGame.level.Player1[mainGame.level.Player1Index]._bodyBody != null)
		{
			if (mainGame.level.Player1[mainGame.level.Player1Index]._bodyBody.Body.Position.X > BrickBody.Body.Position.X)
			{
				TextureFlipEffects = SpriteEffects.None;
				BrickBody.Body.ApplyTorque(5000000f);
			}
			else
			{
				TextureFlipEffects = SpriteEffects.FlipHorizontally;
				BrickBody.Body.ApplyTorque(-5000000f);
			}
		}
		if (((vector2.X > vector.X) & (vector2.X > vector3.X) & (vector2.X > vector4.X)) && mainGame.level.Player1[mainGame.level.Player2Index] != null && mainGame.level.Player1[mainGame.level.Player2Index]._bodyBody != null)
		{
			if (mainGame.level.Player1[mainGame.level.Player2Index]._bodyBody.Body.Position.X > BrickBody.Body.Position.X)
			{
				TextureFlipEffects = SpriteEffects.None;
				BrickBody.Body.ApplyTorque(5000000f);
			}
			else
			{
				TextureFlipEffects = SpriteEffects.FlipHorizontally;
				BrickBody.Body.ApplyTorque(-5000000f);
			}
		}
		if (((vector3.X > vector.X) & (vector3.X > vector2.X) & (vector3.X > vector4.X)) && mainGame.level.Player1[mainGame.level.Player3Index] != null && mainGame.level.Player1[mainGame.level.Player3Index]._bodyBody != null)
		{
			if (mainGame.level.Player1[mainGame.level.Player3Index]._bodyBody.Body.Position.X > BrickBody.Body.Position.X)
			{
				TextureFlipEffects = SpriteEffects.None;
				BrickBody.Body.ApplyTorque(5000000f);
			}
			else
			{
				TextureFlipEffects = SpriteEffects.FlipHorizontally;
				BrickBody.Body.ApplyTorque(-5000000f);
			}
		}
		if (((vector4.X > vector.X) & (vector4.X > vector2.X) & (vector4.X > vector3.X)) && mainGame.level.Player1[mainGame.level.Player4Index] != null && mainGame.level.Player1[mainGame.level.Player4Index]._bodyBody != null)
		{
			if (mainGame.level.Player1[mainGame.level.Player4Index]._bodyBody.Body.Position.X > BrickBody.Body.Position.X)
			{
				TextureFlipEffects = SpriteEffects.None;
				BrickBody.Body.ApplyTorque(5000000f);
			}
			else
			{
				TextureFlipEffects = SpriteEffects.FlipHorizontally;
				BrickBody.Body.ApplyTorque(-5000000f);
			}
		}
	}

	private void UpdateGrinder(GameTime gameTime, World _world)
	{
		if (Explode && !ActiveMine && !Exploded)
		{
			if (ExplosionTimer + 10000.0 > gameTime.TotalGameTime.TotalMilliseconds)
			{
				BrickBody.Body.ApplyTorque(1000000f);
				BrickBody.Friction = 1f;
				BrickBody.Restitution = 0.6f;
			}
			else
			{
				Explode = true;
				ActiveMine = true;
				Active = true;
			}
		}
		else
		{
			ExplosionTimer = gameTime.TotalGameTime.TotalMilliseconds;
		}
	}

	public void OnCollected(Player1 collectedBy)
	{
	}

	public void Draw(GameTime gameTime, SpriteBatch spriteBatch, int i)
	{
		if (Active)
		{
			spriteBatch.Draw(texture, BrickBody.Body.Position * PhysicsScaleUp, null, Color.White, BrickBody.Body.Rotation, origin, Scale, TextureFlipEffects, 1f);
		}
	}

	public void DrawParticles(Vector2 cameraTransform, Renderer Renderer)
	{
	}
}
