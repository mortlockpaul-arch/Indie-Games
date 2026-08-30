using System;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer1;

internal class Sharps
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

	private SpriteEffects HorizontalOrientation;

	private Level level;

	private int _count = 2;

	private Vector2 _endPosition;

	private Vector2 PhysicsPosition;

	private int _radius = 100;

	private Vector2 _startPosition;

	public readonly Color Color = Color.Yellow;

	private Vector2 basePosition;

	private float bounce;

	private ContentManager content;

	public Level Level => level;

	public ContentManager Content => content;

	public Vector2 Position => basePosition + new Vector2(PhysicsPosition.X, PhysicsPosition.Y);

	public Circle BoundingCircle => new Circle(Position, 21.333334f);

	public Sharps(ContentManager content, Level MainLevel, PlatformerGame mainGame, Vector2 position, World physicsSimulator, string BrickType, float rot, int LevelDataIndex)
	{
		level = MainLevel;
		ObjectType = 3;
		ObjectSubType = BrickType;
		this.content = content;
		basePosition = position;
		Rotation = rot;
		this.LevelDataIndex = LevelDataIndex;
		MainGame = mainGame;
		HorizontalOrientation = SpriteEffects.None;
		switch (BrickType)
		{
		case "3":
			LoadNeedle(physicsSimulator, rot);
			break;
		case "4":
			LoadNeedlePopOut(physicsSimulator, rot);
			break;
		case "5":
			LoadSawPopOut(physicsSimulator, rot);
			break;
		case "6":
			LoadDartShoot(physicsSimulator, rot);
			break;
		case "7":
			LoadSaw(physicsSimulator, rot);
			break;
		default:
			LoadNeedle(physicsSimulator, rot);
			break;
		}
	}

	private string LoadSharp(int variationCount)
	{
		random.Next(variationCount);
		SharpString = "Sharps/Needles/1";
		return SharpString;
	}

	public void LoadNeedle(World physicsSimulator, float i)
	{
		int num = random.Next(1, 10);
		BloodColor = Color.White;
		if (num > 5)
		{
			BloodFlip = SpriteEffects.FlipHorizontally;
		}
		else if (num < 5)
		{
			BloodFlip = SpriteEffects.None;
		}
		texture = Content.Load<Texture2D>(LoadSharp(2));
		textureBlood = Content.Load<Texture2D>("Sharps/NeedlesBlood/0");
		BloodColor.A = 0;
		origin = new Vector2((float)texture.Width / 2f, (float)texture.Height / 2f);
		SharpBody = FixtureFactory.CreateRectangle(physicsSimulator, 2f, 17f, Density);
		SharpBody.Body.Position = Position * 0.2f;
		SharpBody.Body.Rotation = Rotation;
		SharpBody.Friction = 1f;
		SharpBody.Body.SleepingAllowed = true;
		SharpBody.Body.BodyType = BodyType.Static;
		SharpBody.CollisionCategories = CollisionCategory.Cat13;
		SharpBody.CollisionGroup = 99;
		SharpBody.Body.UserData = 999;
		Fixture sharpBody = SharpBody;
		sharpBody.OnCollision = (CollisionEventHandler)Delegate.Combine(sharpBody.OnCollision, new CollisionEventHandler(OnCollision_body));
	}

	public void LoadNeedleShort(World physicsSimulator, float i)
	{
		int num = random.Next(1, 10);
		BloodColor = Color.White;
		if (num > 5)
		{
			BloodFlip = SpriteEffects.FlipHorizontally;
		}
		else if (num < 5)
		{
			BloodFlip = SpriteEffects.None;
		}
		texture = Content.Load<Texture2D>("Sharps/Needles/2");
		textureBlood = Content.Load<Texture2D>("Sharps/NeedlesBlood/2");
		BloodColor.A = 0;
		origin = new Vector2((float)texture.Width / 2f, (float)texture.Height / 2f);
		SharpBody = FixtureFactory.CreateRectangle(physicsSimulator, 2f, 9.6f, Density);
		SharpBody.Body.Position = Position * 0.2f;
		SharpBody.Body.Rotation = Rotation;
		SharpBody.Friction = 1f;
		SharpBody.Body.SleepingAllowed = true;
		SharpBody.Body.BodyType = BodyType.Static;
		SharpBody.CollisionCategories = CollisionCategory.Cat13;
		SharpBody.CollisionGroup = 99;
		SharpBody.Body.UserData = 99;
		Fixture sharpBody = SharpBody;
		sharpBody.OnCollision = (CollisionEventHandler)Delegate.Combine(sharpBody.OnCollision, new CollisionEventHandler(OnCollision_body));
	}

	public void LoadNeedlePopOut(World physicsSimulator, float i)
	{
		PopOut = true;
		int num = random.Next(1, 10);
		BloodColor = Color.White;
		if (num > 5)
		{
			BloodFlip = SpriteEffects.FlipHorizontally;
		}
		else if (num < 5)
		{
			BloodFlip = SpriteEffects.None;
		}
		texture = Content.Load<Texture2D>("Sharps/Needles/1");
		textureHide = Content.Load<Texture2D>("Sharps/Needles/Hide/1");
		textureBlood = Content.Load<Texture2D>("Sharps/NeedlesBlood/1");
		BloodColor.A = 0;
		origin = new Vector2((float)texture.Width / 2f, (float)texture.Height / 2f);
		Hideorigin = new Vector2((float)textureHide.Width / 2f, (float)textureHide.Height / 2f);
		HideBody = FixtureFactory.CreateRectangle(physicsSimulator, 4f, 18f, Density);
		HideBody.Body.Position = Position * 0.2f;
		HideBody.Friction = 1f;
		HideBody.Body.SleepingAllowed = true;
		HideBody.Body.BodyType = BodyType.Static;
		HideBody.CollisionCategories = CollisionCategory.Cat13;
		HideBody.CollisionGroup = 98;
		HideBody.Body.UserData = 90;
		Fixture hideBody = HideBody;
		hideBody.OnCollision = (CollisionEventHandler)Delegate.Combine(hideBody.OnCollision, new CollisionEventHandler(OnCollision_body_Needle_PopOut));
		HideBodyBackend = FixtureFactory.CreateCircle(physicsSimulator, 1f, Density);
		HideBodyBackend.Body.Position = Position * 0.2f + new Vector2(0f, 10f);
		HideBodyBackend.Body.SleepingAllowed = true;
		HideBodyBackend.Body.UserData = 90;
		HideBodyBackend.CollisionGroup = 98;
		HideBodyBackend.CollidesWith = CollisionCategory.None;
		HideBodyBackend.CollisionCategories = CollisionCategory.None;
		HideBodyBackend.Body.BodyType = BodyType.Dynamic;
		SharpBody = FixtureFactory.CreateRectangle(physicsSimulator, 2f, 17f, Density);
		SharpBody.Body.Position = Position * 0.2f;
		SharpBody.Body.Rotation = Rotation;
		SharpBody.Friction = 1f;
		SharpBody.Body.SleepingAllowed = true;
		SharpBody.Body.BodyType = BodyType.Kinematic;
		SharpBody.CollisionCategories = CollisionCategory.Cat13;
		SharpBody.CollisionGroup = 98;
		SharpBody.Body.UserData = 98;
		Fixture sharpBody = SharpBody;
		sharpBody.OnCollision = (CollisionEventHandler)Delegate.Combine(sharpBody.OnCollision, new CollisionEventHandler(OnCollision_body));
		RevoluteJoint joint = new RevoluteJoint(HideBodyBackend.Body, HideBody.Body, new Vector2(0f, 0f), new Vector2(0f, 10f));
		HideBody.Body.Rotation = Rotation;
		physicsSimulator.AddJoint(joint);
	}

	public void LoadSawPopOut(World physicsSimulator, float i)
	{
		PopOut = true;
		int num = random.Next(1, 10);
		BloodColor = Color.White;
		if (num > 5)
		{
			BloodFlip = SpriteEffects.FlipHorizontally;
		}
		else if (num < 5)
		{
			BloodFlip = SpriteEffects.None;
		}
		texture = Content.Load<Texture2D>("Sharps/Saw/0");
		textureHide = Content.Load<Texture2D>("Sharps/Saw/Hide/0");
		textureBlood = Content.Load<Texture2D>("Sharps/Saw/Blood/0");
		BloodColor.A = 0;
		origin = new Vector2((float)texture.Width / 2f, (float)texture.Height / 2f);
		Hideorigin = new Vector2((float)textureHide.Width / 2f, (float)textureHide.Height / 2f);
		HideBody = FixtureFactory.CreateRectangle(physicsSimulator, 17f, 17f, Density);
		HideBody.Body.Position = Position * 0.2f;
		HideBody.Friction = 1f;
		HideBody.Body.SleepingAllowed = true;
		HideBody.Body.BodyType = BodyType.Static;
		HideBody.CollisionCategories = CollisionCategory.Cat13;
		HideBody.CollisionGroup = 98;
		HideBody.Body.UserData = 90;
		Fixture hideBody = HideBody;
		hideBody.OnCollision = (CollisionEventHandler)Delegate.Combine(hideBody.OnCollision, new CollisionEventHandler(OnCollision_body_Saw_PopOut));
		HideBodyBackend = FixtureFactory.CreateCircle(physicsSimulator, 1f, Density);
		HideBodyBackend.Body.Position = Position * 0.2f + new Vector2(0f, 10f);
		HideBodyBackend.Body.SleepingAllowed = true;
		HideBodyBackend.Body.UserData = 90;
		HideBodyBackend.CollisionGroup = 98;
		HideBodyBackend.CollidesWith = CollisionCategory.None;
		HideBodyBackend.CollisionCategories = CollisionCategory.None;
		HideBodyBackend.Body.BodyType = BodyType.Dynamic;
		SharpBody = FixtureFactory.CreateCircle(physicsSimulator, 8f, Density);
		SharpBody.Body.Position = Position * 0.2f;
		SharpBody.Body.Rotation = Rotation;
		SharpBody.Friction = 1f;
		SharpBody.Restitution = 1f;
		SharpBody.Body.SleepingAllowed = true;
		SharpBody.Body.BodyType = BodyType.Kinematic;
		SharpBody.CollisionCategories = CollisionCategory.Cat13;
		SharpBody.CollisionGroup = 98;
		SharpBody.Body.UserData = 98;
		Fixture sharpBody = SharpBody;
		sharpBody.OnCollision = (CollisionEventHandler)Delegate.Combine(sharpBody.OnCollision, new CollisionEventHandler(OnCollision_body));
		RevoluteJoint joint = new RevoluteJoint(HideBodyBackend.Body, HideBody.Body, new Vector2(0f, 0f), new Vector2(0f, 10f));
		HideBody.Body.Rotation = Rotation;
		physicsSimulator.AddJoint(joint);
	}

	public void LoadDartShoot(World physicsSimulator, float i)
	{
		DartShoot = true;
		RayCastDartShoot = true;
		int num = random.Next(1, 10);
		BloodColor = Color.White;
		if (num > 5)
		{
			BloodFlip = SpriteEffects.FlipHorizontally;
		}
		else if (num < 5)
		{
			BloodFlip = SpriteEffects.None;
		}
		texture = Content.Load<Texture2D>("Sharps/Dart/0");
		textureHide = Content.Load<Texture2D>("Sharps/Dart/Hide/0");
		textureBlood = null;
		BloodColor.A = 0;
		origin = new Vector2((float)texture.Width / 2f, (float)texture.Height / 2f);
		Hideorigin = new Vector2((float)textureHide.Width / 2f, (float)textureHide.Height / 2f);
		HideBody = FixtureFactory.CreateRectangle(physicsSimulator, 4f, 18f, Density);
		HideBody.Body.Position = Position * 0.2f;
		HideBody.Friction = 1f;
		HideBody.Body.SleepingAllowed = true;
		HideBody.Body.BodyType = BodyType.Static;
		HideBody.CollisionCategories = CollisionCategory.Cat13;
		HideBody.CollisionGroup = 97;
		HideBody.Body.UserData = 90;
		Fixture hideBody = HideBody;
		hideBody.OnCollision = (CollisionEventHandler)Delegate.Combine(hideBody.OnCollision, new CollisionEventHandler(OnCollision_body_Dart_Shoot));
		HideBodyBackend = FixtureFactory.CreateCircle(physicsSimulator, 1f, Density);
		HideBodyBackend.Body.Position = Position * 0.2f + new Vector2(0f, 10f);
		HideBodyBackend.Body.SleepingAllowed = true;
		HideBodyBackend.Body.UserData = 90;
		HideBodyBackend.CollisionGroup = 97;
		HideBodyBackend.CollidesWith = CollisionCategory.None;
		HideBodyBackend.CollisionCategories = CollisionCategory.None;
		HideBodyBackend.Body.BodyType = BodyType.Dynamic;
		SharpBody = FixtureFactory.CreateRectangle(physicsSimulator, 2f, 7f, 1E-07f);
		SharpBody.Body.Position = Position * 0.2f;
		SharpBody.Body.Rotation = Rotation;
		SharpBody.Body.Mass = 1E-07f;
		SharpBody.Friction = 1f;
		SharpBody.Restitution = 1f;
		SharpBody.Body.IsBullet = true;
		SharpBody.Body.IgnoreGravity = true;
		SharpBody.Body.SleepingAllowed = true;
		SharpBody.Body.BodyType = BodyType.Dynamic;
		SharpBody.CollisionCategories = CollisionCategory.All;
		SharpBody.CollidesWith = CollisionCategory.All;
		SharpBody.CollisionGroup = 97;
		SharpBody.Body.UserData = 98;
		Fixture sharpBody = SharpBody;
		sharpBody.OnCollision = (CollisionEventHandler)Delegate.Combine(sharpBody.OnCollision, new CollisionEventHandler(OnCollision_body_Dart));
		RevoluteJoint joint = new RevoluteJoint(HideBodyBackend.Body, HideBody.Body, new Vector2(0f, 0f), new Vector2(0f, 10f));
		HideBody.Body.Rotation = Rotation;
		physicsSimulator.AddJoint(joint);
	}

	public void LoadSaw(World physicsSimulator, float i)
	{
		int num = random.Next(1, 10);
		BloodColor = Color.White;
		if (num > 5)
		{
			BloodFlip = SpriteEffects.FlipHorizontally;
		}
		else if (num < 5)
		{
			BloodFlip = SpriteEffects.None;
		}
		texture = Content.Load<Texture2D>("Sharps/Saw/0");
		textureBlood = Content.Load<Texture2D>("Sharps/Saw/Blood/0");
		BloodColor.A = 0;
		origin = new Vector2((float)texture.Width / 2f, (float)texture.Height / 2f);
		SharpBody = FixtureFactory.CreateCircle(physicsSimulator, 8f, Density);
		SharpBody.Body.Position = Position * 0.2f;
		SharpBody.Body.Rotation = Rotation;
		SharpBody.Friction = 1f;
		SharpBody.Restitution = 1f;
		SharpBody.Body.SleepingAllowed = true;
		SharpBody.Body.BodyType = BodyType.Kinematic;
		SharpBody.CollisionCategories = CollisionCategory.Cat13;
		SharpBody.CollisionGroup = 98;
		SharpBody.Body.UserData = 98;
		Fixture sharpBody = SharpBody;
		sharpBody.OnCollision = (CollisionEventHandler)Delegate.Combine(sharpBody.OnCollision, new CollisionEventHandler(OnCollision_body));
		if (Rotation > 0f)
		{
			HorizontalOrientation = SpriteEffects.None;
			SharpBody.Body.AngularVelocity = 10f;
		}
		else
		{
			HorizontalOrientation = SpriteEffects.FlipHorizontally;
			SharpBody.Body.AngularVelocity = -10f;
		}
	}

	private bool OnCollision_body_Needle_PopOut(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null && (int)fixtureB.Body.UserData == 8 && SharpBody.Body.BodyType == BodyType.Kinematic)
		{
			PopOutGo = true;
		}
		return true;
	}

	private bool OnCollision_body_Dart_Shoot(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null && (int)fixtureB.Body.UserData == 8 && SharpBody.Body.BodyType == BodyType.Dynamic)
		{
			DartShootGo = true;
		}
		return true;
	}

	private bool OnCollision_body_Saw_PopOut(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null && (int)fixtureB.Body.UserData == 8 && SharpBody.Body.BodyType == BodyType.Kinematic)
		{
			PopOutGoSaw = true;
		}
		return true;
	}

	private bool OnCollision_body_Dart(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			SharpBody.Body.Position = HideBody.Body.Position;
			SharpBody.Body.LinearVelocity = new Vector2(0f, 0f);
			SharpBody.Body.AngularVelocity = 0f;
			SharpBody.Body.Rotation = Rotation;
			DartReset = true;
		}
		return true;
	}

	private bool OnCollision_body(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null && (int)fixtureB.Body.UserData == 8 && BloodColor.A < 220)
		{
			BloodColor.A += 50;
		}
		return true;
	}

	public void RemoveAll(World _world)
	{
		Active = false;
		if (SharpBody != null && SharpBody.Body != null && SharpBody.Body.FixtureList != null)
		{
			SharpBody.Body.Active = false;
		}
		if (HideBody != null && HideBody.Body != null && HideBody.Body.FixtureList != null)
		{
			HideBody.Body.Active = false;
		}
		if (HideBodyBackend != null && HideBodyBackend.Body != null && HideBodyBackend.Body.FixtureList != null)
		{
			HideBodyBackend.Body.Active = false;
		}
	}

	public void ActiveAll_True(World _world)
	{
		if (SharpBody != null && SharpBody.Body != null && !SharpBody.Body.Active && SharpBody.Body.FixtureList != null)
		{
			SharpBody.Body.Active = true;
		}
		if (HideBody != null && HideBody.Body != null && !HideBody.Body.Active && HideBody.Body.FixtureList != null)
		{
			HideBody.Body.Active = true;
		}
		if (HideBodyBackend != null && HideBodyBackend.Body != null && !HideBodyBackend.Body.Active && HideBodyBackend.Body.FixtureList != null)
		{
			HideBodyBackend.Body.Active = true;
		}
	}

	public void ActiveAll_False(World _world)
	{
		if (SharpBody != null && SharpBody.Body != null && SharpBody.Body.Active && SharpBody.Body.FixtureList != null)
		{
			SharpBody.Body.Active = false;
		}
		if (HideBody != null && HideBody.Body != null && HideBody.Body.Active && HideBody.Body.FixtureList != null)
		{
			HideBody.Body.Active = false;
		}
		if (HideBodyBackend != null && HideBodyBackend.Body != null && HideBodyBackend.Body.Active && HideBodyBackend.Body.FixtureList != null)
		{
			HideBodyBackend.Body.Active = false;
		}
	}

	public void Update(GameTime gameTime, World _world)
	{
		if (Active)
		{
			ActiveAll_True(_world);
			if (PopOut)
			{
				if (PopOutGo)
				{
					if (gameTime.TotalGameTime.TotalMilliseconds < OldGameTime + 150.0)
					{
						int num = 10;
						SharpBody.Body.LinearVelocity = (HideBody.Body.Position - HideBodyBackend.Body.Position) * new Vector2(num, num);
						SharpBody.Body.AngularVelocity = 0f;
					}
					else
					{
						PopOutGo = false;
					}
				}
				else if (PopOutGoSaw)
				{
					if (gameTime.TotalGameTime.TotalMilliseconds < OldGameTime + 900.0)
					{
						int num2 = 1;
						SharpBody.Body.LinearVelocity = (HideBody.Body.Position - HideBodyBackend.Body.Position) * new Vector2(num2, num2);
						SharpBody.Body.AngularVelocity = 10f;
					}
					else
					{
						PopOutGoSaw = false;
					}
				}
				else
				{
					SharpBody.Body.Position = HideBody.Body.Position;
					SharpBody.Body.LinearVelocity = new Vector2(0f, 0f);
					SharpBody.Body.AngularVelocity = 0f;
					OldGameTime = gameTime.TotalGameTime.TotalMilliseconds;
				}
			}
			if (RayCastDartShoot)
			{
				_world.RayCast(delegate(Fixture f, Vector2 p, Vector2 n, float fr)
				{
					_ = f.Body;
					if (f != null)
					{
						if (f.Body.UserData != null)
						{
							if ((int)f.Body.UserData == 8)
							{
								DartShootGo = true;
								return fr;
							}
							if ((int)f.Body.UserData == 90)
							{
								return -1f;
							}
							return -1f;
						}
						return 1f;
					}
					return 1f;
				}, HideBody.Body.Position, (HideBody.Body.Position - HideBodyBackend.Body.Position) * new Vector2(100f, 100f));
				FirstHit = false;
			}
			if (DartShoot)
			{
				if (DartReset)
				{
					SharpBody.Body.Position = HideBody.Body.Position;
					SharpBody.Body.LinearVelocity = new Vector2(0f, 0f);
					SharpBody.Body.Rotation = Rotation;
				}
				else if (OldGameTime + 1000.0 < gameTime.TotalGameTime.TotalMilliseconds)
				{
					DartReset = true;
				}
				else
				{
					DartShootGo = false;
				}
				if (DartShootGo)
				{
					int num3 = 100;
					SharpBody.Body.ApplyLinearImpulse((HideBody.Body.Position - HideBodyBackend.Body.Position) * new Vector2(num3, num3), new Vector2(0f, 0f));
					DartShootGo = false;
					DartReset = false;
					OldGameTime = gameTime.TotalGameTime.TotalMilliseconds;
				}
			}
		}
		else
		{
			ActiveAll_False(_world);
		}
	}

	public void OnCollected(Player1 collectedBy)
	{
	}

	public void Draw(GameTime gameTime, SpriteBatch spriteBatch, int i)
	{
		if (Active)
		{
			if (DartShoot)
			{
				spriteBatch.Draw(texture, SharpBody.Body.Position * PhysicsScaleUp, null, _color, SharpBody.Body.Rotation, origin, 0.5f, HorizontalOrientation, 1f);
			}
			else
			{
				spriteBatch.Draw(texture, SharpBody.Body.Position * PhysicsScaleUp, null, _color, SharpBody.Body.Rotation, origin, 1f, HorizontalOrientation, 1f);
			}
			if (level != null && level.Blood && textureBlood != null)
			{
				spriteBatch.Draw(textureBlood, SharpBody.Body.Position * PhysicsScaleUp, null, BloodColor, SharpBody.Body.Rotation, origin, 1f, BloodFlip, 1f);
			}
			if (PopOut || DartShoot)
			{
				spriteBatch.Draw(textureHide, HideBody.Body.Position * PhysicsScaleUp, null, _color, HideBody.Body.Rotation, Hideorigin, 1f, HorizontalOrientation, 1f);
			}
		}
	}
}
