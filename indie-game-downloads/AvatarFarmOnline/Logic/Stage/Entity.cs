using System;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage;

internal class Entity
{
	private float radius;

	protected Body entityBody;

	protected Fixture entityFixture;

	private AvatarFarmOnline.Logic.Stage.Stage stage;

	protected Vector2 orientation = new Vector2((float)Math.PI / 4f, -0.25f);

	public float Radius => radius;

	public Body Body => entityBody;

	public Fixture Fixture => entityFixture;

	public bool Enabled
	{
		get
		{
			return entityBody.Enabled;
		}
		set
		{
			SetEnabled(value);
		}
	}

	public AvatarFarmOnline.Logic.Stage.Stage Stage => stage;

	public Vector2 Speed => entityBody.LinearVelocity;

	public bool Moving => !entityBody.LinearVelocity.ZeroLength();

	public Vector2 Orientation => orientation;

	public Vector2 Position => entityBody.Position;

	public Vector3 WorldPosition => AvatarFarmOnline.Logic.GameGlobals.WorldPosition(Position);

	public Entity(AvatarFarmOnline.Logic.Stage.Stage stage, float radius)
	{
		this.stage = stage;
		this.radius = radius;
		InitPhysics(stage.World, radius);
	}

	protected virtual void InitPhysics(World world, float radius)
	{
		entityBody = BodyFactory.CreateBody(world, Vector2.Zero);
		entityBody.IgnoreGravity = true;
		entityBody.BodyType = BodyType.Dynamic;
		entityBody.SleepingAllowed = false;
		entityBody.FixedRotation = true;
		Vertices vertices = PolygonTools.CreateEllipse(radius, radius, 12);
		PolygonShape shape = new PolygonShape(vertices, 1f);
		entityFixture = entityBody.CreateFixture(shape);
		entityFixture.UserData = this;
		entityFixture.IsSensor = false;
	}

	public void SetPosition(Vector2 position)
	{
		entityBody.Position = position;
	}

	public void SetOrientation(Vector2 orientation)
	{
		this.orientation = orientation;
	}

	protected virtual void SetEnabled(bool value)
	{
		if (Enabled != value)
		{
			entityBody.Enabled = value;
		}
	}

	public virtual void Dispose()
	{
	}

	public virtual void Update()
	{
	}
}
