using System;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectMercury.Renderers;

namespace Platformer1;

internal class Kinetics
{
	public const float PhysicsScaleDown = 0.2f;

	public const int PointValue = 30;

	public float PhysicsScaleUp = 5f;

	public PlatformerGame MainGame;

	public bool Active = true;

	public Texture2D texture;

	private Vector2 origin;

	private float Density = 1E-07f;

	public CollisionCategory _collidesWith = CollisionCategory.All;

	public CollisionCategory _collisionCategory = CollisionCategory.Cat31;

	private Random random = new Random(354668);

	private float Rotation;

	public int LevelDataIndex;

	private Color _borderColor = Color.Black;

	public int ObjectType;

	public string ObjectSubType;

	public bool PopOutGo;

	public bool PopOutGoSaw;

	public bool PopOut;

	public bool DartShoot;

	public bool RayCastDartShoot;

	public bool DartShootGo;

	public bool FirstHit;

	private Color _color = Color.White;

	private PlatformerGame mainGame;

	public float mass = 1f;

	private string KineticsString;

	private float Scale = 1f;

	public Fixture KineticBody;

	public float BreakPoint = 0.01f;

	public float MaxImpulse = 500f;

	public string ObjectTypeSub;

	private Vector2 PhysicsPosition;

	private bool ForceField;

	private bool ForceX;

	private bool BouncePad;

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

	private Vector2 PlatformTravelDistance;

	private Vector2 PlatformTravelDirection;

	private Vector2 PlatformTravelSpeed;

	private bool PlatformX;

	private bool PlatformXGoRight;

	private bool PlatformXGoLeft;

	private bool PlatformY;

	private bool PlatformYGoRight;

	private bool PlatformYGoLeft;

	private Fixture FixB;

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

	private bool VKinetics;

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

	public Kinetics(ContentManager content, Level Level, PlatformerGame mainGame, Vector2 position, World physicsSimulator, string KineticsType, float rot, Renderer renderer, int LevelDataIndex)
	{
		level = Level;
		ObjectType = 4;
		ObjectSubType = KineticsType;
		this.content = content;
		basePosition = position;
		Rotation = rot;
		this.LevelDataIndex = LevelDataIndex;
		ObjectTypeSub = KineticsType;
		MainGame = mainGame;
		this.mainGame = mainGame;
		this.content = Content;
		basePosition = position;
		Rotation = rot;
		renderer.LoadContent(content);
		switch (KineticsType)
		{
		case "0":
			LoadMovingPlatform_Horizontal_Short_Right_Fast(physicsSimulator, rot);
			break;
		case "1":
			LoadMovingPlatform_Horizontal_Short_Left_Fast(physicsSimulator, rot);
			break;
		case "2":
			LoadMovingPlatform_Vertical_Short_Up_Fast(physicsSimulator, rot);
			break;
		case "3":
			LoadMovingPlatform_Vertical_Short_Down_Fast(physicsSimulator, rot);
			break;
		case "4":
			LoadBouncePad(physicsSimulator, rot);
			break;
		case "5":
			LoadForceFieldPush(physicsSimulator, rot);
			break;
		case "6":
			LoadForceFieldX(physicsSimulator, rot);
			break;
		case "7":
			LoadMovingPlatform_Vertical_Short_Down_Fast(physicsSimulator, rot);
			break;
		default:
			LoadMovingPlatform_Horizontal_Short_Right_Fast(physicsSimulator, rot);
			break;
		}
	}

	private string LoadKinetics(int variationCount)
	{
		random.Next(variationCount);
		KineticsString = "Kineticss/Needles/1";
		return KineticsString;
	}

	public void LoadBouncePad(World physicsSimulator, float i)
	{
		BouncePad = true;
		texture = Content.Load<Texture2D>("Kinetics/Platforms/BouncePad");
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		PlatformTravelDistance = new Vector2(100f, 100f);
		PlatformTravelDirection = new Vector2(1f, 0f);
		PlatformTravelSpeed = new Vector2(10f, 10f);
		KineticBody = FixtureFactory.CreateRectangle(physicsSimulator, 40f, 4f, Density);
		KineticBody.Body.Position = Position * 0.2f;
		KineticBody.Body.Rotation = Rotation;
		KineticBody.Body.BodyType = BodyType.Static;
		KineticBody.Body.UserData = 302;
		KineticBody.UserData = Rotation;
		KineticBody.Body.SleepingAllowed = true;
		KineticBody.Friction = 1f;
		KineticBody.Restitution = 0.1f;
		KineticBody.CollisionCategories = CollisionCategory.Cat30;
		KineticBody.CollidesWith = CollisionCategory.All;
		Fixture kineticBody = KineticBody;
		kineticBody.OnCollision = (CollisionEventHandler)Delegate.Combine(kineticBody.OnCollision, new CollisionEventHandler(OnCollision_BouncePad));
		PlatformX = true;
		PlatformXGoRight = true;
		Active = true;
	}

	public void LoadForceFieldPush(World physicsSimulator, float i)
	{
		ForceField = true;
		texture = Content.Load<Texture2D>("Kinetics/Platforms/ForceField");
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		PlatformTravelDistance = new Vector2(100f, 100f);
		PlatformTravelDirection = new Vector2(1f, 0f);
		PlatformTravelSpeed = new Vector2(10f, 10f);
		KineticBody = FixtureFactory.CreateRectangle(physicsSimulator, 40f, 40f, Density);
		KineticBody.Body.Position = Position * 0.2f;
		KineticBody.Body.Rotation = Rotation;
		KineticBody.Body.BodyType = BodyType.Static;
		KineticBody.Body.UserData = 300;
		KineticBody.UserData = Rotation;
		KineticBody.Body.SleepingAllowed = true;
		KineticBody.IsSensor = true;
		KineticBody.Restitution = 0.1f;
		KineticBody.CollisionCategories = CollisionCategory.Cat30;
		KineticBody.CollisionGroup = 300;
		Fixture kineticBody = KineticBody;
		kineticBody.OnCollision = (CollisionEventHandler)Delegate.Combine(kineticBody.OnCollision, new CollisionEventHandler(OnCollision_ForcePush));
		PlatformX = true;
		PlatformXGoLeft = true;
		Active = true;
	}

	public void LoadForceFieldX(World physicsSimulator, float i)
	{
		ForceX = true;
		texture = Content.Load<Texture2D>("Kinetics/Platforms/ForceX");
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		PlatformTravelDistance = new Vector2(100f, 100f);
		PlatformTravelDirection = new Vector2(1f, 0f);
		PlatformTravelSpeed = new Vector2(10f, 10f);
		KineticBody = FixtureFactory.CreateRectangle(physicsSimulator, 40f, 40f, Density);
		KineticBody.Body.Position = Position * 0.2f;
		KineticBody.Body.Rotation = Rotation;
		KineticBody.Body.BodyType = BodyType.Static;
		KineticBody.Body.UserData = 301;
		KineticBody.UserData = Rotation;
		KineticBody.IsSensor = true;
		KineticBody.Restitution = 0.1f;
		KineticBody.CollisionCategories = CollisionCategory.Cat30;
		KineticBody.CollisionGroup = 301;
		Fixture kineticBody = KineticBody;
		kineticBody.OnCollision = (CollisionEventHandler)Delegate.Combine(kineticBody.OnCollision, new CollisionEventHandler(OnCollision_ForceX));
		PlatformX = true;
		PlatformXGoLeft = true;
		Active = true;
	}

	public void LoadMovingPlatform_Horizontal_Short_Right_Fast(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>("Kinetics/Platforms/0");
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		PlatformTravelDistance = new Vector2(100f, 100f);
		PlatformTravelDirection = new Vector2(1f, 0f);
		PlatformTravelSpeed = new Vector2(20f, 20f);
		KineticBody = FixtureFactory.CreateRectangle(physicsSimulator, 40f, 4f, Density);
		KineticBody.Body.Position = Position * 0.2f;
		KineticBody.Body.Rotation = Rotation;
		KineticBody.Body.BodyType = BodyType.Kinematic;
		KineticBody.Body.UserData = 1;
		KineticBody.UserData = 20;
		KineticBody.Friction = 1f;
		KineticBody.Restitution = 0.1f;
		KineticBody.CollisionCategories = CollisionCategory.Cat30;
		KineticBody.CollidesWith = CollisionCategory.All;
		Fixture kineticBody = KineticBody;
		kineticBody.OnCollision = (CollisionEventHandler)Delegate.Combine(kineticBody.OnCollision, new CollisionEventHandler(OnCollision_OneSided));
		PlatformX = true;
		PlatformXGoRight = true;
		Active = true;
	}

	public void LoadMovingPlatform_Horizontal_Short_Left_Fast(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>("Kinetics/Platforms/0");
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		PlatformTravelDistance = new Vector2(100f, 100f);
		PlatformTravelDirection = new Vector2(1f, 0f);
		PlatformTravelSpeed = new Vector2(20f, 20f);
		KineticBody = FixtureFactory.CreateRectangle(physicsSimulator, 40f, 4f, Density);
		KineticBody.Body.Position = Position * 0.2f;
		KineticBody.Body.Rotation = Rotation;
		KineticBody.Body.BodyType = BodyType.Kinematic;
		KineticBody.Body.UserData = 1;
		KineticBody.UserData = 20;
		KineticBody.Body.SleepingAllowed = true;
		KineticBody.Friction = 1f;
		KineticBody.Restitution = 0.1f;
		KineticBody.CollisionCategories = CollisionCategory.Cat30;
		Fixture kineticBody = KineticBody;
		kineticBody.OnCollision = (CollisionEventHandler)Delegate.Combine(kineticBody.OnCollision, new CollisionEventHandler(OnCollision_OneSided));
		PlatformX = true;
		PlatformXGoLeft = true;
		Active = true;
	}

	public void LoadMovingPlatform_Vertical_Short_Up(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>("Kinetics/Platforms/0");
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		PlatformTravelDistance = new Vector2(100f, 100f);
		PlatformTravelDirection = new Vector2(0f, 1f);
		PlatformTravelSpeed = new Vector2(10f, 10f);
		KineticBody = FixtureFactory.CreateRectangle(physicsSimulator, 40f, 4f, Density);
		KineticBody.Body.Position = Position * 0.2f;
		KineticBody.Body.Rotation = Rotation;
		KineticBody.Body.BodyType = BodyType.Kinematic;
		KineticBody.Body.UserData = 1;
		KineticBody.UserData = 20;
		KineticBody.Body.SleepingAllowed = true;
		KineticBody.Friction = 1f;
		KineticBody.Restitution = 0.1f;
		KineticBody.CollisionCategories = CollisionCategory.Cat30;
		Fixture kineticBody = KineticBody;
		kineticBody.OnCollision = (CollisionEventHandler)Delegate.Combine(kineticBody.OnCollision, new CollisionEventHandler(OnCollision_OneSided));
		PlatformY = true;
		PlatformYGoRight = true;
		Active = true;
	}

	public void LoadMovingPlatform_Vertical_Short_Down(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>("Kinetics/Platforms/0");
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		PlatformTravelDistance = new Vector2(100f, 100f);
		PlatformTravelDirection = new Vector2(0f, 1f);
		PlatformTravelSpeed = new Vector2(10f, 10f);
		KineticBody = FixtureFactory.CreateRectangle(physicsSimulator, 40f, 4f, Density);
		KineticBody.Body.Position = Position * 0.2f;
		KineticBody.Body.Rotation = Rotation;
		KineticBody.Body.BodyType = BodyType.Kinematic;
		KineticBody.Body.UserData = 1;
		KineticBody.UserData = 20;
		KineticBody.Body.SleepingAllowed = true;
		KineticBody.Friction = 1f;
		KineticBody.Restitution = 0.1f;
		KineticBody.CollisionCategories = CollisionCategory.Cat30;
		Fixture kineticBody = KineticBody;
		kineticBody.OnCollision = (CollisionEventHandler)Delegate.Combine(kineticBody.OnCollision, new CollisionEventHandler(OnCollision_OneSided));
		PlatformY = true;
		PlatformYGoLeft = true;
		Active = true;
	}

	public void LoadMovingPlatform_Vertical_Short_Up_Fast(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>("Kinetics/Platforms/0");
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		PlatformTravelDistance = new Vector2(100f, 100f);
		PlatformTravelDirection = new Vector2(0f, 1f);
		PlatformTravelSpeed = new Vector2(20f, 20f);
		KineticBody = FixtureFactory.CreateRectangle(physicsSimulator, 40f, 4f, Density);
		KineticBody.Body.Position = Position * 0.2f;
		KineticBody.Body.Rotation = Rotation;
		KineticBody.Body.BodyType = BodyType.Kinematic;
		KineticBody.Body.UserData = 1;
		KineticBody.UserData = 20;
		KineticBody.Body.SleepingAllowed = true;
		KineticBody.Friction = 1f;
		KineticBody.Restitution = 0.1f;
		KineticBody.CollisionCategories = CollisionCategory.Cat30;
		Fixture kineticBody = KineticBody;
		kineticBody.OnCollision = (CollisionEventHandler)Delegate.Combine(kineticBody.OnCollision, new CollisionEventHandler(OnCollision_OneSided));
		PlatformY = true;
		PlatformYGoRight = true;
		Active = true;
	}

	private bool OnCollision_OneSided(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if ((int)fixtureA.UserData == 20 && (int)fixtureB.Body.UserData == 8 && fixtureA.Body.LinearVelocity.Y > fixtureB.Body.LinearVelocity.Y)
		{
			FixB = fixtureB;
			contact.Enabled = false;
			return false;
		}
		return true;
	}

	private bool OnCollision_BouncePad(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if ((int)fixtureB.Body.UserData != 8)
		{
			FixB = fixtureB;
		}
		return true;
	}

	private bool OnCollision_ForcePush(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if ((int)fixtureB.Body.UserData != 8)
		{
			FixB = fixtureB;
			contact.Enabled = false;
			return false;
		}
		return true;
	}

	private bool OnCollision_ForceX(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if ((int)fixtureB.Body.UserData != 8)
		{
			FixB = fixtureB;
			contact.Enabled = false;
			return false;
		}
		return true;
	}

	public void LoadMovingPlatform_Vertical_Short_Down_Fast(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>("Kinetics/Platforms/0");
		origin = new Vector2(texture.Width / 2, texture.Height / 2);
		Scale = 1f;
		PlatformTravelDistance = new Vector2(100f, 100f);
		PlatformTravelDirection = new Vector2(0f, 1f);
		PlatformTravelSpeed = new Vector2(20f, 20f);
		KineticBody = FixtureFactory.CreateRectangle(physicsSimulator, 40f, 4f, Density);
		KineticBody.Body.Position = Position * 0.2f;
		KineticBody.Body.Rotation = Rotation;
		KineticBody.Body.BodyType = BodyType.Kinematic;
		KineticBody.Body.UserData = 1;
		KineticBody.UserData = 20;
		KineticBody.Body.SleepingAllowed = true;
		KineticBody.Friction = 1f;
		KineticBody.Restitution = 0.1f;
		KineticBody.CollisionCategories = CollisionCategory.Cat30;
		Fixture kineticBody = KineticBody;
		kineticBody.OnCollision = (CollisionEventHandler)Delegate.Combine(kineticBody.OnCollision, new CollisionEventHandler(OnCollision_OneSided));
		PlatformY = true;
		PlatformYGoLeft = true;
		Active = true;
	}

	public void RemoveAll(World _world)
	{
		Active = false;
		if (KineticBody != null && KineticBody.Body != null)
		{
			_world.RemoveBody(KineticBody.Body);
		}
	}

	public void ActiveAll_True(World _world)
	{
		if (KineticBody != null && KineticBody.Body != null && !KineticBody.Body.Active && KineticBody.Body.FixtureList != null)
		{
			KineticBody.Body.Active = true;
		}
	}

	public void ActiveAll_False(World _world)
	{
		if (KineticBody != null && KineticBody.Body != null && KineticBody.Body.Active && KineticBody.Body.FixtureList != null)
		{
			KineticBody.Body.Active = false;
		}
	}

	public void Update(GameTime gameTime, World _world)
	{
		if (Active)
		{
			ActiveAll_True(_world);
			random = new Random((int)gameTime.TotalGameTime.TotalMilliseconds);
			if (ForceField)
			{
				if (FixB != null)
				{
					Vector2 vector = new Vector2(0f, -10000f);
					Vector2 vector2 = new Vector2(10000000f, 10000000f);
					float num = (float)Math.Cos(Rotation);
					float num2 = (float)Math.Sin(Rotation);
					vector = new Vector2(vector.X * num - vector.Y * num2, vector.X * num2 + vector.Y * num);
					FixB.Body.ApplyForce(vector * vector2, FixB.Body.WorldCenter);
					FixB = null;
				}
				return;
			}
			if (ForceX)
			{
				if (FixB != null)
				{
					Vector2 linearVelocity = FixB.Body.LinearVelocity;
					Vector2 vector3 = new Vector2(100000000f, 100000000f);
					FixB.Body.ApplyForce(linearVelocity * vector3, FixB.Body.WorldCenter);
					FixB = null;
				}
				return;
			}
			if (BouncePad)
			{
				if (FixB != null)
				{
					Vector2 vector4 = new Vector2(0f, -10000f);
					float num3 = (float)Math.Cos(Rotation);
					float num4 = (float)Math.Sin(Rotation);
					vector4 = new Vector2(vector4.X * num3 - vector4.Y * num4, vector4.X * num4 + vector4.Y * num3);
					FixB.Body.LinearVelocity += vector4;
					FixB = null;
				}
				return;
			}
			if (FixB != null)
			{
				FixB.Body.ApplyForce(new Vector2(0f, -500f));
				FixB = null;
			}
			if (PlatformX)
			{
				if (PlatformXGoRight)
				{
					if (KineticBody.Body.Position.X - PlatformTravelDistance.X / 2f > Position.X * 0.2f)
					{
						PlatformXGoRight = false;
						PlatformXGoLeft = true;
					}
					else
					{
						KineticBody.Body.LinearVelocity = PlatformTravelDirection * PlatformTravelSpeed;
					}
				}
				if (PlatformXGoLeft)
				{
					if (KineticBody.Body.Position.X + PlatformTravelDistance.X / 2f < Position.X * 0.2f)
					{
						PlatformXGoLeft = false;
						PlatformXGoRight = true;
					}
					else
					{
						KineticBody.Body.LinearVelocity = -PlatformTravelDirection * PlatformTravelSpeed;
					}
				}
			}
			if (!PlatformY)
			{
				return;
			}
			if (PlatformYGoRight)
			{
				if (KineticBody.Body.Position.Y - PlatformTravelDistance.Y / 2f > Position.Y * 0.2f)
				{
					PlatformYGoRight = false;
					PlatformYGoLeft = true;
				}
				else
				{
					KineticBody.Body.LinearVelocity = PlatformTravelDirection * PlatformTravelSpeed;
				}
			}
			if (PlatformYGoLeft)
			{
				if (KineticBody.Body.Position.Y + PlatformTravelDistance.Y / 2f < Position.Y * 0.2f)
				{
					PlatformYGoLeft = false;
					PlatformYGoRight = true;
				}
				else
				{
					KineticBody.Body.LinearVelocity = -PlatformTravelDirection * PlatformTravelSpeed;
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
			spriteBatch.Draw(texture, KineticBody.Body.Position * PhysicsScaleUp, null, Color.White, KineticBody.Body.Rotation, origin, Scale, SpriteEffects.None, 1f);
		}
	}

	public void DrawParticles(Vector2 cameraTransform, Renderer Renderer)
	{
	}
}
