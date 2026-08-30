using System;
using System.Collections.Generic;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer1;

internal class Lands
{
	public const float PhysicsScaleDown = 0.2f;

	public const float PhysicsScaleUp = 5f;

	public const int PointValue = 30;

	public bool Active = true;

	public Texture2D texture;

	private Color BloodColor;

	private Vector2 origin;

	public CollisionCategory _collidesWith = CollisionCategory.All;

	public CollisionCategory _collisionCategory = CollisionCategory.Cat31;

	private Random random = new Random(354668);

	private World _world;

	private float Rotation;

	public int LevelDataIndex;

	private string Landstring;

	private Color _borderColor = Color.Black;

	public Body LandBody;

	public List<Fixture> LandFixtures = new List<Fixture>();

	private bool IsChain;

	private bool IsBridge;

	public Fixture[] ChainBodys;

	public RevoluteJoint[] ChainJoints;

	public RevoluteJoint[] ChainJoints2;

	public int ChainCount;

	public Texture2D ChainTexture;

	private Vector2 ChainOrigin;

	public Fixture LandBody2;

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

	public readonly Color Color = Color.Yellow;

	private Vector2 basePosition;

	private float bounce;

	private Vector2 vertOld;

	private Vector2 Translation;

	private ContentManager content;

	public Level Level => level;

	public ContentManager Content => content;

	public Vector2 Position => basePosition + new Vector2(PhysicsPosition.X, PhysicsPosition.Y);

	public Circle BoundingCircle => new Circle(Position, 21.333334f);

	public Lands(ContentManager content, Level MainLevel, Vector2 position, World _world, string LandType, float rot, int LevelDataIndex)
	{
		level = MainLevel;
		ObjectType = 1;
		ObjectSubType = LandType;
		this.content = content;
		basePosition = position;
		Rotation = rot;
		this.LevelDataIndex = LevelDataIndex;
		switch (LandType)
		{
		case "0":
			LoadRegular(_world, rot, 0f);
			break;
		case "1":
			LoadRegular(_world, rot, 1f);
			break;
		case "2":
			LoadRegular(_world, rot, 2f);
			break;
		case "3":
			LoadRegular(_world, rot, 3f);
			break;
		case "4":
			LoadRegular(_world, rot, 4f);
			break;
		default:
			LoadRegular(_world, rot, 0f);
			break;
		}
	}

	private string LoadLand(int variationCount)
	{
		random.Next(variationCount);
		Landstring = "Lands/Needles/1";
		return Landstring;
	}

	public void LoadRegular(World _world, float i, float j)
	{
		texture = content.Load<Texture2D>("Lands/Regular/" + j);
		origin = new Vector2(100f, 0f);
		LandBody = BodyFactory.CreateBody(_world);
		LandBody.Position = basePosition * 0.2f;
		LandBody.BodyType = BodyType.Static;
		LandBody.UserData = 1;
		uint[] data = new uint[texture.Width * texture.Height];
		texture.GetData(data);
		Vertices vertices = PolygonTools.CreatePolygon(data, texture.Width, texture.Height);
		Translation = new Vector2(-100f, 0f);
		vertices.Translate(ref Translation);
		Vector2 value = new Vector2(0.2f, 0.2f);
		vertices.Scale(ref value);
		vertOld = vertices.GetCentroid();
		foreach (Vector2 item in vertices)
		{
			Vertices vertices2 = PolygonTools.CreateEdge(vertOld, item);
			PolygonShape shape = new PolygonShape(vertices2);
			LandBody.CreateFixture(shape);
			vertOld = item;
		}
		foreach (Fixture fixture in LandBody.FixtureList)
		{
			fixture.UserData = 1;
		}
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
		if (LandBody != null && LandBody.FixtureList != null)
		{
			_world.RemoveBody(LandBody);
		}
	}

	public void ActiveAll_True(World _world)
	{
		if (LandBody != null && !LandBody.Active)
		{
			LandBody.Active = true;
		}
	}

	public void ActiveAll_False(World _world)
	{
		if (LandBody != null && LandBody.Active)
		{
			LandBody.Active = false;
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
		if (Active)
		{
			spriteBatch.Draw(texture, LandBody.Position * 5f, null, Color.White, LandBody.Rotation, origin, 1f, SpriteEffects.None, 1f);
		}
	}
}
