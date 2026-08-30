using System.Collections.Generic;
using FarseerGames.FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Controllers;

public class GravityController : Controller
{
	private float _strength;

	private PhysicsSimulator _simulator;

	private float _radius;

	public GravityType GravityType { get; set; }

	public List<Vector2> PointList { get; set; }

	public List<Body> BodyList { get; set; }

	public GravityController(PhysicsSimulator simulator, List<Body> bodies, List<Vector2> points, float strength, float radius)
	{
		GravityType = GravityType.Linear;
		_simulator = simulator;
		_strength = strength;
		if (GravityType == GravityType.DistanceSquared)
		{
			_strength *= 100f;
		}
		_radius = radius;
		PointList = points;
		BodyList = bodies;
	}

	public GravityController(PhysicsSimulator simulator, List<Body> bodies, float strength, float radius)
	{
		GravityType = GravityType.Linear;
		_simulator = simulator;
		_strength = strength;
		if (GravityType == GravityType.DistanceSquared)
		{
			_strength *= 100f;
		}
		_radius = radius;
		BodyList = bodies;
	}

	public GravityController(PhysicsSimulator simulator, List<Vector2> points, float strength, float radius)
	{
		GravityType = GravityType.Linear;
		_simulator = simulator;
		_strength = strength;
		if (GravityType == GravityType.DistanceSquared)
		{
			_strength *= 100f;
		}
		_radius = radius;
		PointList = points;
	}

	public override void Validate()
	{
	}

	public override void Update(float dt, float dtReal)
	{
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		foreach (Body body in _simulator.BodyList)
		{
			if (body.IsDisposed || !body.Enabled || body.IgnoreGravity)
			{
				continue;
			}
			if (BodyList != null)
			{
				foreach (Body body2 in BodyList)
				{
					if (body == body2 || (body.isStatic && body2.isStatic) || body2.IsDisposed || !body2.Enabled)
					{
						continue;
					}
					Vector2 val = body2.position - body.position;
					Vector2 val2 = val;
					((Vector2)(ref val2)).Normalize();
					float num = ((Vector2)(ref val)).Length();
					if (!(num > _radius))
					{
						Vector2 val3 = Vector2.Multiply(val2, _strength);
						if (GravityType == GravityType.DistanceSquared)
						{
							val3 = Vector2.Divide(val3, num * num);
						}
						else if (GravityType == GravityType.Linear)
						{
							val3 = Vector2.Divide(val3, num);
						}
						Vector2 amount = body.mass * val3;
						body.ApplyForce(ref amount);
					}
				}
			}
			if (PointList == null)
			{
				continue;
			}
			foreach (Vector2 point in PointList)
			{
				Vector2 val4 = point - body.position;
				Vector2 val5 = val4;
				((Vector2)(ref val5)).Normalize();
				float num2 = ((Vector2)(ref val4)).Length();
				if (!(num2 > _radius))
				{
					Vector2 val6 = Vector2.Multiply(val5, _strength);
					if (GravityType == GravityType.DistanceSquared)
					{
						val6 = Vector2.Divide(val6, num2 * num2);
					}
					else if (GravityType == GravityType.Linear)
					{
						val6 = Vector2.Divide(val6, num2);
					}
					Vector2 amount2 = body.mass * val6;
					body.ApplyForce(ref amount2);
				}
			}
		}
	}
}
