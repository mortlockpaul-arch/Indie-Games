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

internal class Weapon
{
	public const float PhysicsScaleDown = 0.2f;

	public const float PhysicsScaleUp = 5f;

	public const int PointValue = 30;

	public bool Active = true;

	public Texture2D texture;

	private Texture2D textureHide;

	public Texture2D textureBlood;

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

	private PlatformerGame mainGame;

	public SpriteFont Font;

	public string Data;

	public Vector2 cameraTransformOld;

	public float mass = 1f;

	private Vector2 AveragePlayerPosition;

	private Vector2 CamPosition;

	private string WeaponString;

	private string WeaponTypeString;

	private float Scale = 1f;

	public Fixture WeaponBody;

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

	private bool VWeapon;

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

	public Weapon(ContentManager content, Level Level, PlatformerGame mainGame, Vector2 position, World physicsSimulator, string WeaponType, float rot, Renderer renderer)
	{
		level = Level;
		ObjectType = 2;
		ObjectSubType = WeaponType;
		this.content = content;
		basePosition = position;
		Rotation = rot;
		ObjectType = 2;
		ObjectTypeSub = WeaponType;
		this.mainGame = mainGame;
		this.content = Content;
		basePosition = position;
		Rotation = rot;
		renderer.LoadContent(content);
		switch (WeaponType)
		{
		case "0":
			LoadSword(physicsSimulator, rot);
			break;
		case "1":
			LoadAx(physicsSimulator, rot);
			break;
		case "2":
			LoadBattleAx(physicsSimulator, rot);
			break;
		case "3":
			LoadSpear(physicsSimulator, rot);
			break;
		case "4":
			LoadLongBow(physicsSimulator, rot);
			break;
		case "5":
			LoadCrossBow(physicsSimulator, rot);
			break;
		case "6":
			LoadDualingPistol(physicsSimulator, rot);
			break;
		case "7":
			LoadMuskeet(physicsSimulator, rot);
			break;
		case "8":
			LoadBlunderbus(physicsSimulator, rot);
			break;
		default:
			LoadSword(physicsSimulator, rot);
			break;
		}
	}

	private string LoadRegWeapon(int variationCount)
	{
		if (level != null)
		{
			WeaponString = "Weapons/0/" + level.random.Next(0, variationCount);
		}
		else
		{
			WeaponString = "Weapons/0/0";
		}
		return WeaponString;
	}

	private string LoadGrinderWeapon(int variationCount)
	{
		if (level != null)
		{
			WeaponString = "Weapons/Grinder/" + level.random.Next(0, variationCount);
		}
		else
		{
			WeaponString = "Weapons/Grinder/0";
		}
		return WeaponString;
	}

	public void LoadSword(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>(LoadRegWeapon(6));
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		WeaponBody = FixtureFactory.CreateRectangle(physicsSimulator, 12.8f, 9.6f, Density);
		WeaponBody.Body.Position = Position * 0.2f;
		WeaponBody.Body.Rotation = Rotation;
		WeaponBody.Body.BodyType = BodyType.Dynamic;
		WeaponBody.Body.UserData = 9;
		WeaponBody.UserData = 20;
		WeaponBody.Body.SleepingAllowed = true;
		WeaponBody.Friction = 1f;
		WeaponBody.Restitution = 0f;
		Active = true;
	}

	public void LoadAx(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>(LoadRegWeapon(6));
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		WeaponBody = FixtureFactory.CreateRectangle(physicsSimulator, 12.8f, 9.6f, Density);
		WeaponBody.Body.Position = Position * 0.2f;
		WeaponBody.Body.Rotation = Rotation;
		WeaponBody.Body.BodyType = BodyType.Dynamic;
		WeaponBody.Body.UserData = 9;
		WeaponBody.UserData = 20;
		WeaponBody.Body.SleepingAllowed = true;
		WeaponBody.Friction = 1f;
		WeaponBody.Restitution = 0f;
		Active = true;
	}

	public void LoadBattleAx(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>(LoadRegWeapon(6));
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		WeaponBody = FixtureFactory.CreateRectangle(physicsSimulator, 12.8f, 9.6f, Density);
		WeaponBody.Body.Position = Position * 0.2f;
		WeaponBody.Body.Rotation = Rotation;
		WeaponBody.Body.BodyType = BodyType.Dynamic;
		WeaponBody.Body.UserData = 9;
		WeaponBody.UserData = 20;
		WeaponBody.Body.SleepingAllowed = true;
		WeaponBody.Friction = 1f;
		WeaponBody.Restitution = 0f;
		Active = true;
	}

	public void LoadSpear(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>(LoadRegWeapon(6));
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		WeaponBody = FixtureFactory.CreateRectangle(physicsSimulator, 12.8f, 9.6f, Density);
		WeaponBody.Body.Position = Position * 0.2f;
		WeaponBody.Body.Rotation = Rotation;
		WeaponBody.Body.BodyType = BodyType.Dynamic;
		WeaponBody.Body.UserData = 9;
		WeaponBody.UserData = 20;
		WeaponBody.Body.SleepingAllowed = true;
		WeaponBody.Friction = 1f;
		WeaponBody.Restitution = 0f;
		Active = true;
	}

	public void LoadLongBow(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>(LoadRegWeapon(6));
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		WeaponBody = FixtureFactory.CreateRectangle(physicsSimulator, 12.8f, 9.6f, Density);
		WeaponBody.Body.Position = Position * 0.2f;
		WeaponBody.Body.Rotation = Rotation;
		WeaponBody.Body.BodyType = BodyType.Dynamic;
		WeaponBody.Body.UserData = 9;
		WeaponBody.UserData = 20;
		WeaponBody.Body.SleepingAllowed = true;
		WeaponBody.Friction = 1f;
		WeaponBody.Restitution = 0f;
		Active = true;
	}

	public void LoadCrossBow(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>(LoadRegWeapon(6));
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		WeaponBody = FixtureFactory.CreateRectangle(physicsSimulator, 12.8f, 9.6f, Density);
		WeaponBody.Body.Position = Position * 0.2f;
		WeaponBody.Body.Rotation = Rotation;
		WeaponBody.Body.BodyType = BodyType.Dynamic;
		WeaponBody.Body.UserData = 9;
		WeaponBody.UserData = 20;
		WeaponBody.Body.SleepingAllowed = true;
		WeaponBody.Friction = 1f;
		WeaponBody.Restitution = 0f;
		Active = true;
	}

	public void LoadDualingPistol(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>(LoadRegWeapon(6));
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		WeaponBody = FixtureFactory.CreateRectangle(physicsSimulator, 12.8f, 9.6f, Density);
		WeaponBody.Body.Position = Position * 0.2f;
		WeaponBody.Body.Rotation = Rotation;
		WeaponBody.Body.BodyType = BodyType.Dynamic;
		WeaponBody.Body.UserData = 9;
		WeaponBody.UserData = 20;
		WeaponBody.Body.SleepingAllowed = true;
		WeaponBody.Friction = 1f;
		WeaponBody.Restitution = 0f;
		Active = true;
	}

	public void LoadMuskeet(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>(LoadRegWeapon(6));
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		WeaponBody = FixtureFactory.CreateRectangle(physicsSimulator, 12.8f, 9.6f, Density);
		WeaponBody.Body.Position = Position * 0.2f;
		WeaponBody.Body.Rotation = Rotation;
		WeaponBody.Body.BodyType = BodyType.Dynamic;
		WeaponBody.Body.UserData = 9;
		WeaponBody.UserData = 20;
		WeaponBody.Body.SleepingAllowed = true;
		WeaponBody.Friction = 1f;
		WeaponBody.Restitution = 0f;
		Active = true;
	}

	public void LoadBlunderbus(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>(LoadRegWeapon(6));
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		WeaponBody = FixtureFactory.CreateRectangle(physicsSimulator, 12.8f, 9.6f, Density);
		WeaponBody.Body.Position = Position * 0.2f;
		WeaponBody.Body.Rotation = Rotation;
		WeaponBody.Body.BodyType = BodyType.Dynamic;
		WeaponBody.Body.UserData = 9;
		WeaponBody.UserData = 20;
		WeaponBody.Body.SleepingAllowed = true;
		WeaponBody.Friction = 1f;
		WeaponBody.Restitution = 0f;
		Active = true;
	}

	private bool OnCollision_Ball(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		WeaponBody.CollisionGroup = 0;
		return true;
	}

	private bool OnCollision_Mine(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (ActiveMine && fixtureB.Body != null && (int)fixtureB.Body.UserData == 8)
		{
			int num = random.Next(0, 100);
			if (num <= 33)
			{
				Explosion1.Play();
			}
			else if (num <= 66)
			{
				Explosion2.Play();
			}
			else if (num <= 100)
			{
				Explosion3.Play();
			}
			ExplosionBody.Body.Active = true;
			ActiveMine = false;
			texture = BlownMineTexture;
			if (mainGame.level != null)
			{
				mainGame.level.MineExplodeEffect[0].TriggerOffset = WeaponBody.Body.Position * new Vector2(5f, 5f);
				mainGame.level.MineExplodeEffect[1].TriggerOffset = WeaponBody.Body.Position * new Vector2(5f, 5f);
				mainGame.level.MineExplodeEffect[2].TriggerOffset = WeaponBody.Body.Position * new Vector2(5f, 5f);
				mainGame.level.MineExplodeEffect[3].TriggerOffset = WeaponBody.Body.Position * new Vector2(5f, 5f);
				mainGame.level.MineExplodeEffect.Trigger(new Vector2(0f, 0f));
			}
		}
		return true;
	}

	private bool OnCollision_Grinder(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (ActiveMine && fixtureB.Body != null && (int)fixtureB.Body.UserData == 8)
		{
			Explosion1.Play();
			WeaponBody.Body.UserData = 122;
			ActiveMine = false;
		}
		return true;
	}

	private bool OnCollision_Mine_Explosion(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		ExplosionFixtureB = fixtureB;
		return true;
	}

	public void Update(GameTime gameTime, World _world)
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
			UpdateBall(gameTime, _world);
			break;
		case "2":
			UpdateMine(gameTime, _world);
			break;
		case "3":
			UpdateStone(gameTime, _world);
			break;
		case "4":
			UpdateStoneBeam(gameTime, _world);
			break;
		case "5":
			UpdateRegularSpike(gameTime, _world);
			break;
		case "6":
			UpdateGrinder(gameTime, _world);
			break;
		default:
			UpdateRegular(gameTime, _world);
			break;
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
				ExplosionBody.Body.Position = WeaponBody.Body.Position;
				{
					foreach (Body body in _world.BodyList)
					{
						Vector2 point = body.Position;
						float num = 100000000f;
						if (ExplosionBody.TestPoint(ref point))
						{
							body.ApplyForce(new Vector2(1000f, 1000f) - new Vector2(WeaponBody.Body.Position.X - body.Position.X, WeaponBody.Body.Position.Y - body.Position.Y) * new Vector2(num, num));
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
	}

	private void UpdateGrinder(GameTime gameTime, World _world)
	{
		if (Explode && !ActiveMine && !Exploded)
		{
			if (ExplosionTimer + 10000.0 > gameTime.TotalGameTime.TotalMilliseconds)
			{
				WeaponBody.Body.ApplyTorque(10000f);
				WeaponBody.Friction = 1f;
				WeaponBody.Restitution = 0.6f;
			}
			else
			{
				Exploded = true;
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
			spriteBatch.Draw(texture, WeaponBody.Body.Position * 5f, null, Color.White, WeaponBody.Body.Rotation, origin, Scale, SpriteEffects.None, 1f);
		}
	}

	public void DrawParticles(Vector2 cameraTransform, Renderer Renderer)
	{
	}
}
