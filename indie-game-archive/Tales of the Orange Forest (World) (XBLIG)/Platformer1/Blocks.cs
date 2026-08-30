using System;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer1;

internal class Blocks
{
	public const float PhysicsScaleDown = 0.2f;

	public const int PointValue = 30;

	public float PhysicsScaleUp = 5f;

	public PlatformerGame MainGame;

	public bool Active = true;

	public Texture2D texture;

	private Color BloodColor;

	private Vector2 origin;

	private float Density = 10000f;

	public CollisionCategory _collidesWith = CollisionCategory.All;

	public CollisionCategory _collisionCategory = CollisionCategory.Cat31;

	private Random random = new Random(354668);

	private World physicsSimulator;

	private float Rotation;

	public int LevelDataIndex;

	private string Blockstring;

	private Color _borderColor = Color.Black;

	public Fixture BlockBody;

	private bool IsChain;

	private bool IsBridge;

	private bool IsBig;

	public Fixture[] ChainBodys;

	public RevoluteJoint[] ChainJoints;

	public RevoluteJoint[] ChainJoints2;

	public int ChainCount;

	public Texture2D ChainTexture;

	private Vector2 ChainOrigin;

	public Fixture BlockBody2;

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

	private Level level;

	private int _count = 2;

	private Vector2 _endPosition;

	private Vector2 PhysicsPosition;

	private int _radius = 100;

	private Vector2 _startPosition;

	private Vector2 basePosition;

	private ContentManager content;

	public Level Level => level;

	public ContentManager Content => content;

	public Vector2 Position => basePosition + new Vector2(PhysicsPosition.X, PhysicsPosition.Y);

	public Blocks(ContentManager content, Level MainLevel, PlatformerGame mainGame, Vector2 position, World physicsSimulator, string BrickType, int BlockType, float rot, int LevelDataIndex)
	{
		level = MainLevel;
		ObjectType = 1;
		ObjectSubType = BrickType;
		this.content = content;
		basePosition = position;
		Rotation = rot;
		this.LevelDataIndex = LevelDataIndex;
		MainGame = mainGame;
		switch (BrickType)
		{
		case "0":
			LoadRegular(physicsSimulator, rot);
			break;
		case "1":
			LoadBigBlock(physicsSimulator, rot);
			break;
		case "2":
			LoadBeam(physicsSimulator, rot);
			break;
		case "3":
			LoadLongBeam(physicsSimulator, rot);
			break;
		case "4":
			LoadBall(physicsSimulator, rot);
			break;
		case "5":
			LoadBigBall(physicsSimulator, rot);
			break;
		case "6":
			LoadChainBridge(physicsSimulator, rot);
			break;
		case "7":
			LoadArrow(physicsSimulator, rot);
			break;
		default:
			LoadRegular(physicsSimulator, rot);
			break;
		}
	}

	private string LoadBlock(int variationCount)
	{
		random.Next(variationCount);
		Blockstring = "Blocks/Needles/1";
		return Blockstring;
	}

	public void LoadRegular(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>("Blocks/0");
		origin = new Vector2((float)texture.Width / 2f, (float)texture.Height / 2f);
		BlockBody = FixtureFactory.CreateRectangle(physicsSimulator, 12.8f, 14f, Density, Position * 0.2f);
		BlockBody.Body.Rotation = Rotation;
		BlockBody.Friction = 1f;
		BlockBody.Body.SleepingAllowed = true;
		BlockBody.Body.BodyType = BodyType.Static;
		BlockBody.CollisionCategories = CollisionCategory.Cat30;
		BlockBody.CollisionGroup = 365;
		BlockBody.Body.UserData = 1;
		BlockBody.UserData = 1;
	}

	public void LoadBigBlock(World physicsSimulator, float i)
	{
		IsBig = true;
		texture = Content.Load<Texture2D>("Blocks/0");
		origin = new Vector2((float)texture.Width / 2f, (float)texture.Height / 2f);
		BlockBody = FixtureFactory.CreateRectangle(physicsSimulator, 25.6f, 25.6f, Density, Position * 0.2f);
		BlockBody.Body.Rotation = Rotation;
		BlockBody.Friction = 1f;
		BlockBody.Body.SleepingAllowed = true;
		BlockBody.Body.BodyType = BodyType.Static;
		BlockBody.CollisionCategories = CollisionCategory.Cat30;
		BlockBody.CollisionGroup = 365;
		BlockBody.Body.UserData = 1;
		BlockBody.UserData = 1;
	}

	public void LoadBeam(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>("Blocks/Beams/0");
		origin = new Vector2((float)texture.Width / 2f, (float)texture.Height / 2f);
		BlockBody = FixtureFactory.CreateRectangle(physicsSimulator, 64f, 12.8f, Density);
		BlockBody.Body.Position = Position * 0.2f;
		BlockBody.Body.Rotation = Rotation;
		BlockBody.Friction = 1f;
		BlockBody.Body.SleepingAllowed = true;
		BlockBody.Body.BodyType = BodyType.Static;
		BlockBody.CollisionCategories = CollisionCategory.Cat30;
		BlockBody.CollisionGroup = 365;
		BlockBody.Body.UserData = 1;
	}

	public void LoadLongBeam(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>("Blocks/Beams/1");
		origin = new Vector2((float)texture.Width / 2f, (float)texture.Height / 2f);
		BlockBody = FixtureFactory.CreateRectangle(physicsSimulator, 128f, 12.8f, Density);
		BlockBody.Body.Position = Position * 0.2f;
		BlockBody.Body.Rotation = Rotation;
		BlockBody.Friction = 1f;
		BlockBody.Body.SleepingAllowed = true;
		BlockBody.Body.BodyType = BodyType.Static;
		BlockBody.CollisionCategories = CollisionCategory.Cat30;
		BlockBody.CollisionGroup = 365;
		BlockBody.Body.UserData = 1;
	}

	public void LoadBall(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>("Blocks/Ball/0");
		origin = new Vector2((float)texture.Width / 2f, (float)texture.Height / 2f);
		BlockBody = FixtureFactory.CreateCircle(physicsSimulator, 20f, Density, Position * 0.2f);
		BlockBody.Body.Rotation = Rotation;
		BlockBody.Friction = 1f;
		BlockBody.Body.SleepingAllowed = true;
		BlockBody.Body.BodyType = BodyType.Static;
		BlockBody.CollisionCategories = CollisionCategory.Cat30;
		BlockBody.CollisionGroup = 365;
		BlockBody.Body.UserData = 1;
	}

	public void LoadBigBall(World physicsSimulator, float i)
	{
		IsBig = true;
		texture = Content.Load<Texture2D>("Blocks/Ball/0");
		origin = new Vector2((float)texture.Width / 2f, (float)texture.Height / 2f);
		BlockBody = FixtureFactory.CreateCircle(physicsSimulator, 40f, Density, Position * 0.2f);
		BlockBody.Body.Rotation = Rotation;
		BlockBody.Friction = 1f;
		BlockBody.Body.SleepingAllowed = true;
		BlockBody.Body.BodyType = BodyType.Static;
		BlockBody.CollisionCategories = CollisionCategory.Cat30;
		BlockBody.CollisionGroup = 365;
		BlockBody.Body.UserData = 1;
	}

	public void LoadChain(World physicsSimulator, float i)
	{
		ChainCount = 30;
		float density = 10f;
		IsChain = true;
		ChainBodys = new Fixture[ChainCount];
		ChainJoints = new RevoluteJoint[ChainCount];
		texture = Content.Load<Texture2D>("Blocks/0");
		origin = new Vector2((float)texture.Width / 2f, (float)texture.Height / 2f);
		BlockBody = FixtureFactory.CreateRectangle(physicsSimulator, 12.8f, 12.8f, Density);
		BlockBody.Body.Position = Position * 0.2f;
		BlockBody.Body.Rotation = Rotation;
		BlockBody.Friction = 1f;
		BlockBody.Body.SleepingAllowed = true;
		BlockBody.Body.BodyType = BodyType.Static;
		BlockBody.CollisionCategories = CollisionCategory.Cat30;
		BlockBody.Body.UserData = 1;
		BlockBody.CollisionGroup = 365;
		ChainTexture = Content.Load<Texture2D>("Blocks/Chains/0");
		ChainOrigin = new Vector2((float)ChainTexture.Width / 2f, (float)ChainTexture.Height / 2f);
		for (int j = 0; j < ChainCount; j++)
		{
			if (j == 0)
			{
				ChainBodys[j] = FixtureFactory.CreateCircle(physicsSimulator, 4f, density);
				ChainBodys[j].Body.Position = (Position + new Vector2(0f, 12.8f)) * 0.2f;
				ChainBodys[j].Body.Rotation = Rotation;
				ChainBodys[j].Friction = 0f;
				ChainBodys[j].Restitution = 0f;
				ChainBodys[j].Body.SleepingAllowed = true;
				ChainBodys[j].Body.BodyType = BodyType.Dynamic;
				ChainBodys[j].Body.UserData = 9;
				ChainJoints[j] = new RevoluteJoint(BlockBody.Body, ChainBodys[j].Body, new Vector2(0f, 0f), new Vector2(0f, -12.8f));
				ChainJoints[j].CollideConnected = true;
				physicsSimulator.AddJoint(ChainJoints[j]);
			}
			else
			{
				ChainBodys[j] = FixtureFactory.CreateCircle(physicsSimulator, 4f, density);
				ChainBodys[j].Body.Position = (Position + new Vector2(0f, 4f * (float)j)) * 0.2f;
				ChainBodys[j].Body.Rotation = Rotation;
				ChainBodys[j].Friction = 0f;
				ChainBodys[j].Restitution = 0f;
				ChainBodys[j].Body.SleepingAllowed = true;
				ChainBodys[j].Body.BodyType = BodyType.Dynamic;
				ChainBodys[j].Body.UserData = 9;
				ChainJoints[j] = new RevoluteJoint(ChainBodys[j - 1].Body, ChainBodys[j].Body, new Vector2(0f, 2f), new Vector2(0f, -2f));
				ChainJoints[j].CollideConnected = true;
				physicsSimulator.AddJoint(ChainJoints[j]);
			}
		}
	}

	public void LoadChainBridge(World physicsSimulator, float i)
	{
		ChainCount = 30;
		float density = 10f;
		IsChain = true;
		IsBridge = true;
		ChainBodys = new Fixture[ChainCount];
		ChainJoints = new RevoluteJoint[ChainCount + 1];
		ChainJoints2 = new RevoluteJoint[ChainCount + 1];
		texture = Content.Load<Texture2D>("Blocks/0");
		origin = new Vector2((float)texture.Width / 2f, (float)texture.Height / 2f);
		BlockBody = FixtureFactory.CreateRectangle(physicsSimulator, 12.8f, 12.8f, Density);
		BlockBody.Body.Position = Position * 0.2f;
		BlockBody.Body.Rotation = Rotation;
		BlockBody.Friction = 1f;
		BlockBody.Body.SleepingAllowed = true;
		BlockBody.Body.BodyType = BodyType.Static;
		BlockBody.CollisionCategories = CollisionCategory.Cat30;
		BlockBody.Body.UserData = 1;
		BlockBody.CollisionGroup = 365;
		ChainTexture = Content.Load<Texture2D>("Blocks/Chains/Bridge/0");
		ChainOrigin = new Vector2((float)ChainTexture.Width / 2f, (float)ChainTexture.Height / 2f);
		for (int j = 0; j < ChainCount; j++)
		{
			if (j == 0)
			{
				ChainBodys[j] = FixtureFactory.CreateRectangle(physicsSimulator, 10f, 6f, density);
				ChainBodys[j].Body.Position = (Position + new Vector2(0f, 12.8f)) * 0.2f;
				ChainBodys[j].Body.Rotation = Rotation;
				ChainBodys[j].Friction = 0f;
				ChainBodys[j].Restitution = 0f;
				ChainBodys[j].Body.SleepingAllowed = true;
				ChainBodys[j].Body.BodyType = BodyType.Dynamic;
				ChainBodys[j].Body.UserData = 9;
				ChainBodys[j].CollisionGroup = 111;
				ChainJoints[j] = new RevoluteJoint(BlockBody.Body, ChainBodys[j].Body, new Vector2(0f, 0f), new Vector2(0f, -12.8f));
				ChainJoints[j].CollideConnected = false;
				physicsSimulator.AddJoint(ChainJoints[j]);
				ChainJoints2[j] = new RevoluteJoint(BlockBody.Body, ChainBodys[j].Body, new Vector2(0f, 0f), new Vector2(0f, -12.8f));
				ChainJoints2[j].CollideConnected = false;
				physicsSimulator.AddJoint(ChainJoints2[j]);
			}
			else
			{
				ChainBodys[j] = FixtureFactory.CreateRectangle(physicsSimulator, 10f, 6f, density);
				ChainBodys[j].Body.Position = (Position + new Vector2(0f, 6f * (float)j)) * 0.2f;
				ChainBodys[j].Body.Rotation = Rotation;
				ChainBodys[j].Friction = 1f;
				ChainBodys[j].Restitution = 0f;
				ChainBodys[j].Body.SleepingAllowed = true;
				ChainBodys[j].Body.BodyType = BodyType.Dynamic;
				ChainBodys[j].CollisionGroup = 111;
				ChainBodys[j].Body.UserData = 9;
				ChainJoints[j] = new RevoluteJoint(ChainBodys[j - 1].Body, ChainBodys[j].Body, new Vector2(-1f, 0f), new Vector2(1f, 0f));
				ChainJoints[j].CollideConnected = false;
				physicsSimulator.AddJoint(ChainJoints[j]);
				ChainJoints2[j] = new RevoluteJoint(ChainBodys[j - 1].Body, ChainBodys[j].Body, new Vector2(-1f, 0f), new Vector2(1f, 0f));
				ChainJoints2[j].CollideConnected = false;
				physicsSimulator.AddJoint(ChainJoints2[j]);
			}
		}
		BlockBody2 = FixtureFactory.CreateRectangle(physicsSimulator, 12.8f, 12.8f, Density);
		BlockBody2.Body.Position = (Position + new Vector2(1000f, 0f)) * 0.2f;
		BlockBody2.Body.Rotation = Rotation;
		BlockBody2.Friction = 1f;
		BlockBody2.Body.SleepingAllowed = true;
		BlockBody2.Body.BodyType = BodyType.Static;
		BlockBody2.CollisionCategories = CollisionCategory.Cat30;
		BlockBody2.Body.UserData = 1;
		ChainJoints[ChainCount] = new RevoluteJoint(BlockBody2.Body, ChainBodys[ChainCount - 1].Body, new Vector2(0f, 0f), new Vector2(0f, -12.8f));
		ChainJoints[ChainCount].CollideConnected = true;
		physicsSimulator.AddJoint(ChainJoints[ChainCount]);
	}

	public void LoadArrow(World physicsSimulator, float i)
	{
		texture = Content.Load<Texture2D>("Blocks/Arrow");
		origin = new Vector2((float)texture.Width / 2f, (float)texture.Height / 2f);
		BlockBody = FixtureFactory.CreateRectangle(physicsSimulator, 10f, 10f, Density, Position * 0.2f);
		BlockBody.Body.Rotation = Rotation;
		BlockBody.Friction = 1f;
		BlockBody.Body.SleepingAllowed = true;
		BlockBody.Body.BodyType = BodyType.Static;
		BlockBody.CollisionCategories = CollisionCategory.None;
		BlockBody.CollidesWith = CollisionCategory.None;
		BlockBody.CollisionGroup = 0;
		BlockBody.Body.UserData = 1;
		BlockBody.UserData = 1;
		BlockBody.Body.Active = false;
	}

	private bool OnCollision_body_Needle_PopOut(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null && (int)fixtureB.Body.UserData == 8 && BlockBody.Body.BodyType == BodyType.Kinematic)
		{
			PopOutGo = true;
		}
		return true;
	}

	private bool OnCollision_body_Dart_Shoot(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null && (int)fixtureB.Body.UserData == 8 && BlockBody.Body.BodyType == BodyType.Dynamic)
		{
			DartShootGo = true;
		}
		return true;
	}

	private bool OnCollision_body_Saw_PopOut(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null && (int)fixtureB.Body.UserData == 8 && BlockBody.Body.BodyType == BodyType.Kinematic)
		{
			PopOutGoSaw = true;
		}
		return true;
	}

	private bool OnCollision_body_Dart(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			BlockBody.Body.Position = HideBody.Body.Position;
			BlockBody.Body.LinearVelocity = new Vector2(0f, 0f);
			BlockBody.Body.AngularVelocity = 0f;
			BlockBody.Body.Rotation = Rotation;
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
		if (BlockBody != null && BlockBody.Body != null && BlockBody.Body.FixtureList != null)
		{
			_world.RemoveBody(BlockBody.Body);
		}
		for (int i = 0; i < ChainCount; i++)
		{
			if (ChainBodys[i] != null && ChainBodys[i].Body != null && ChainBodys[i].Body.FixtureList != null)
			{
				_world.RemoveBody(ChainBodys[i].Body);
			}
		}
		if (BlockBody2 != null && BlockBody2.Body != null && BlockBody2.Body.FixtureList != null)
		{
			_world.RemoveBody(BlockBody2.Body);
		}
	}

	public void ActiveAll_True(World _world)
	{
		if (BlockBody != null && BlockBody.Body != null && !BlockBody.Body.Active && BlockBody.Body.FixtureList != null)
		{
			BlockBody.Body.Active = true;
		}
		for (int i = 0; i < ChainCount; i++)
		{
			if (ChainBodys[i] != null && ChainBodys[i].Body != null && ChainBodys[i].Body.FixtureList != null && !ChainBodys[i].Body.Active)
			{
				ChainBodys[i].Body.Active = true;
			}
		}
		if (BlockBody2 != null && BlockBody2.Body != null && !BlockBody2.Body.Active && BlockBody2.Body.FixtureList != null)
		{
			BlockBody2.Body.Active = true;
		}
	}

	public void ActiveAll_False(World _world)
	{
		if (BlockBody != null && BlockBody.Body != null && BlockBody.Body.Active && BlockBody.Body.FixtureList != null)
		{
			BlockBody.Body.Active = false;
		}
		for (int i = 0; i < ChainCount; i++)
		{
			if (ChainBodys[i] != null && ChainBodys[i].Body != null && ChainBodys[i].Body.FixtureList != null && ChainBodys[i].Body.Active)
			{
				ChainBodys[i].Body.Active = false;
			}
		}
		if (BlockBody2 != null && BlockBody2.Body != null && BlockBody2.Body.Active && BlockBody2.Body.FixtureList != null)
		{
			BlockBody2.Body.Active = false;
		}
	}

	public void Update(GameTime gameTime, World _world)
	{
		if (Active)
		{
			ActiveAll_True(_world);
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
		if (!Active)
		{
			return;
		}
		if (!IsBig)
		{
			spriteBatch.Draw(texture, BlockBody.Body.Position * PhysicsScaleUp, null, Color.White, BlockBody.Body.Rotation, origin, 1f, SpriteEffects.None, 1f);
		}
		else
		{
			spriteBatch.Draw(texture, BlockBody.Body.Position * PhysicsScaleUp, null, Color.White, BlockBody.Body.Rotation, origin, 2f, SpriteEffects.None, 1f);
		}
		if (IsChain)
		{
			for (int j = 0; j < ChainCount; j++)
			{
				spriteBatch.Draw(ChainTexture, ChainBodys[j].Body.Position * PhysicsScaleUp, null, Color.White, ChainBodys[j].Body.Rotation, ChainOrigin, 1f, SpriteEffects.None, 1f);
			}
		}
		if (IsBridge)
		{
			spriteBatch.Draw(texture, BlockBody2.Body.Position * PhysicsScaleUp, null, Color.White, BlockBody2.Body.Rotation, origin, 1f, SpriteEffects.None, 1f);
		}
	}
}
