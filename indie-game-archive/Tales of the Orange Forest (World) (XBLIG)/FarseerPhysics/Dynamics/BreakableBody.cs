using System;
using System.Collections.Generic;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics;

public class BreakableBody
{
	public bool Broken;

	public Body MainBody;

	public List<Fixture> Parts = new List<Fixture>(8);

	public float Strength = 500f;

	private float[] _angularVelocitiesCache = new float[8];

	private bool _break;

	private Vector2[] _velocitiesCache = new Vector2[8];

	private World _world;

	public BreakableBody(List<Vertices> vertices, World world, float density)
	{
		_world = world;
		MainBody = _world.CreateBody();
		MainBody.BodyType = BodyType.Dynamic;
		foreach (Vertices vertex in vertices)
		{
			PolygonShape shape = new PolygonShape(vertex);
			Fixture fixture = MainBody.CreateFixture(shape, density);
			fixture.PostSolve = (Action<ContactConstraint>)Delegate.Combine(fixture.PostSolve, new Action<ContactConstraint>(PostSolve));
			Parts.Add(fixture);
		}
	}

	private void PostSolve(ContactConstraint contactConstraint)
	{
		if (!Broken)
		{
			float num = 0f;
			for (int i = 0; i < contactConstraint.Manifold.PointCount; i++)
			{
				num = Math.Max(num, contactConstraint.Manifold.Points[0].NormalImpulse);
				num = Math.Max(num, contactConstraint.Manifold.Points[1].NormalImpulse);
			}
			if (num > Strength)
			{
				_break = true;
			}
		}
	}

	public void Update()
	{
		if (_break)
		{
			Decompose();
			Broken = true;
			_break = false;
		}
		if (!Broken)
		{
			if (Parts.Count > _angularVelocitiesCache.Length)
			{
				_velocitiesCache = new Vector2[Parts.Count];
				_angularVelocitiesCache = new float[Parts.Count];
			}
			for (int i = 0; i < Parts.Count; i++)
			{
				ref Vector2 reference = ref _velocitiesCache[i];
				reference = Parts[i].Body.LinearVelocity;
				_angularVelocitiesCache[i] = Parts[i].Body.AngularVelocity;
			}
		}
	}

	private void Decompose()
	{
		for (int i = 0; i < Parts.Count; i++)
		{
			Fixture fixture = Parts[i];
			fixture.PostSolve = (Action<ContactConstraint>)Delegate.Remove(fixture.PostSolve, new Action<ContactConstraint>(PostSolve));
			Shape shape = fixture.Shape.Clone();
			MainBody.DestroyFixture(fixture);
			Body body = BodyFactory.CreateBody(_world);
			body.BodyType = BodyType.Dynamic;
			body.Position = MainBody.Position;
			body.Rotation = MainBody.Rotation;
			body.CreateFixture(shape, fixture.Density);
			body.AngularVelocity = _angularVelocitiesCache[i];
			body.LinearVelocity = _velocitiesCache[i];
		}
	}

	public void Break()
	{
		_break = true;
	}
}
