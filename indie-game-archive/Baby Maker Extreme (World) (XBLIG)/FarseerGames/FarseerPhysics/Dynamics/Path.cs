using System;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Dynamics.Joints;
using FarseerGames.FarseerPhysics.Dynamics.Springs;
using FarseerGames.FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Dynamics;

public class Path
{
	private const float _controlPointSize = 6f;

	private const float _precision = 0.0005f;

	private GenericList<Body> _bodies;

	private Vertices _controlPoints;

	private GenericList<Geom> _geoms;

	private GenericList<Spring> _springs;

	private float _height;

	private GenericList<Joint> _joints;

	private bool _loop;

	private float _mass;

	private bool _recalculate = true;

	private float _width;

	private float _linkWidth;

	public GenericList<Body> Bodies => _bodies;

	public GenericList<Joint> Joints => _joints;

	public GenericList<Geom> Geoms => _geoms;

	public GenericList<Spring> Springs => _springs;

	public Vertices ControlPoints => _controlPoints;

	public Path(float width, float height, float mass, bool endless)
		: this(width, height, width, mass, endless)
	{
	}

	public Path(float width, float height, float linkWidth, float mass, bool endless)
	{
		_width = width;
		_linkWidth = linkWidth;
		_height = height;
		_loop = endless;
		_mass = mass;
		_geoms = new GenericList<Geom>(8);
		_bodies = new GenericList<Body>(8);
		_joints = new GenericList<Joint>(8);
		_springs = new GenericList<Spring>(8);
		_controlPoints = new Vertices();
	}

	public void LinkBodies(LinkType type, float min, float max, float springConstant, float dampingConstant)
	{
		LinkBodies(type, min, max, springConstant, dampingConstant, 1f);
	}

	public void LinkBodies(LinkType type, float min, float max, float springConstant, float dampingConstant, float springRestLengthFactor)
	{
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0872: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_092b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0824: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		Vector2 initialAnchorPosition = default(Vector2);
		for (int i = 0; i < _bodies.Count; i++)
		{
			if (i >= _bodies.Count - 1)
			{
				continue;
			}
			float num = ((!(_bodies[i].position.X < _bodies[i + 1].position.X)) ? ((_bodies[i + 1].position.X - _bodies[i].position.X) * 0.5f) : (Math.Abs(_bodies[i].position.X - _bodies[i + 1].position.X) * 0.5f));
			float num2 = ((!(_bodies[i].position.Y < _bodies[i + 1].position.Y)) ? ((_bodies[i + 1].position.Y - _bodies[i].position.Y) * 0.5f) : (Math.Abs(_bodies[i].position.Y - _bodies[i + 1].position.Y) * 0.5f));
			((Vector2)(ref initialAnchorPosition))._002Ector(_bodies[i].position.X + num, _bodies[i].position.Y + num2);
			switch (type)
			{
			case LinkType.RevoluteJoint:
			{
				RevoluteJoint revoluteJoint = JointFactory.Instance.CreateRevoluteJoint(_bodies[i], _bodies[i + 1], initialAnchorPosition);
				revoluteJoint.BiasFactor = 0.2f;
				revoluteJoint.Softness = 0.01f;
				_joints.Add(revoluteJoint);
				break;
			}
			case LinkType.LinearSpring:
			{
				LinearSpring linearSpring = ((!(_bodies[i].Position.X < _bodies[i + 1].Position.X)) ? SpringFactory.Instance.CreateLinearSpring(_bodies[i], new Vector2((0f - _width) / 2f, 0f), _bodies[i + 1], new Vector2(_width / 2f, 0f), springConstant, dampingConstant) : SpringFactory.Instance.CreateLinearSpring(_bodies[i], new Vector2(_width / 2f, 0f), _bodies[i + 1], new Vector2((0f - _width) / 2f, 0f), springConstant, dampingConstant));
				if (i >= 1)
				{
					LinearSpring linearSpring2 = (LinearSpring)_springs[i - 1];
					linearSpring.RestLength = Vector2.Distance(linearSpring2.AttachPoint2, linearSpring.AttachPoint1) * springRestLengthFactor;
				}
				_springs.Add(linearSpring);
				break;
			}
			case LinkType.PinJoint:
			{
				PinJoint pinJoint = ((!(_bodies[i].Position.X < _bodies[i + 1].Position.X)) ? JointFactory.Instance.CreatePinJoint(_bodies[i], new Vector2((0f - _width) / 2f, 0f), _bodies[i + 1], new Vector2(_width / 2f, 0f)) : JointFactory.Instance.CreatePinJoint(_bodies[i], new Vector2(_width / 2f, 0f), _bodies[i + 1], new Vector2((0f - _width) / 2f, 0f)));
				pinJoint.BiasFactor = 0.2f;
				pinJoint.Softness = 0.01f;
				if (i >= 1)
				{
					PinJoint pinJoint2 = (PinJoint)_joints[i - 1];
					pinJoint.TargetDistance = Vector2.Distance(pinJoint2.Anchor2, pinJoint.Anchor1);
				}
				_joints.Add(pinJoint);
				break;
			}
			case LinkType.SliderJoint:
			{
				SliderJoint sliderJoint = ((!(_bodies[i].Position.X < _bodies[i + 1].Position.X)) ? JointFactory.Instance.CreateSliderJoint(_bodies[i], new Vector2((0f - _width) / 2f, 0f), _bodies[i + 1], new Vector2(_width / 2f, 0f), min, max) : JointFactory.Instance.CreateSliderJoint(_bodies[i], new Vector2(_width / 2f, 0f), _bodies[i + 1], new Vector2((0f - _width) / 2f, 0f), min, max));
				sliderJoint.BiasFactor = 0.2f;
				sliderJoint.Softness = 0.01f;
				_joints.Add(sliderJoint);
				break;
			}
			}
		}
		if (_loop)
		{
			float num = ((!(_bodies[0].position.X < _bodies[_bodies.Count - 1].position.X)) ? ((_bodies[_bodies.Count - 1].position.X - _bodies[0].position.X) * 0.5f) : (Math.Abs(_bodies[0].position.X - _bodies[_bodies.Count - 1].position.X) * 0.5f));
			float num2 = ((!(_bodies[0].position.Y < _bodies[_bodies.Count - 1].position.Y)) ? ((_bodies[_bodies.Count - 1].position.Y - _bodies[0].position.Y) * 0.5f) : (Math.Abs(_bodies[0].position.Y - _bodies[_bodies.Count - 1].position.Y) * 0.5f));
			((Vector2)(ref initialAnchorPosition))._002Ector(_bodies[0].position.X + num, _bodies[0].position.Y + num2);
			switch (type)
			{
			case LinkType.RevoluteJoint:
			{
				RevoluteJoint revoluteJoint = JointFactory.Instance.CreateRevoluteJoint(_bodies[0], _bodies[_bodies.Count - 1], initialAnchorPosition);
				revoluteJoint.BiasFactor = 0.2f;
				revoluteJoint.Softness = 0.01f;
				_joints.Add(revoluteJoint);
				break;
			}
			case LinkType.LinearSpring:
			{
				LinearSpring linearSpring = SpringFactory.Instance.CreateLinearSpring(_bodies[0], new Vector2((0f - _width) / 2f, 0f), _bodies[_bodies.Count - 1], new Vector2(_width / 2f, 0f), springConstant, dampingConstant);
				linearSpring.RestLength = _width;
				_springs.Add(linearSpring);
				break;
			}
			case LinkType.PinJoint:
			{
				PinJoint pinJoint = JointFactory.Instance.CreatePinJoint(_bodies[0], new Vector2((0f - _width) / 2f, 0f), _bodies[_bodies.Count - 1], new Vector2(_width / 2f, 0f));
				pinJoint.BiasFactor = 0.2f;
				pinJoint.Softness = 0.01f;
				pinJoint.TargetDistance = _width;
				_joints.Add(pinJoint);
				break;
			}
			case LinkType.SliderJoint:
			{
				SliderJoint sliderJoint = JointFactory.Instance.CreateSliderJoint(_bodies[0], new Vector2((0f - _width) / 2f, 0f), _bodies[_bodies.Count - 1], new Vector2(_width / 2f, 0f), min, max);
				sliderJoint.BiasFactor = 0.2f;
				sliderJoint.Softness = 0.01f;
				_joints.Add(sliderJoint);
				break;
			}
			}
		}
	}

	public void AddToPhysicsSimulator(PhysicsSimulator physicsSimulator)
	{
		foreach (Body body in _bodies)
		{
			physicsSimulator.Add(body);
		}
		foreach (Geom geom in _geoms)
		{
			physicsSimulator.Add(geom);
		}
		foreach (Joint joint in _joints)
		{
			physicsSimulator.Add(joint);
		}
		foreach (Spring spring in _springs)
		{
			physicsSimulator.Add(spring);
		}
	}

	public void RemoveFromPhysicsSimulator(PhysicsSimulator physicsSimulator)
	{
		foreach (Body body in _bodies)
		{
			physicsSimulator.Remove(body);
		}
		foreach (Geom geom in _geoms)
		{
			physicsSimulator.Remove(geom);
		}
		foreach (Joint joint in _joints)
		{
			physicsSimulator.Remove(joint);
		}
		foreach (Spring spring in _springs)
		{
			physicsSimulator.Remove(spring);
		}
	}

	public void CreateGeoms(PhysicsSimulator physicsSimulator, int collisionGroup)
	{
		CreateGeoms(collisionGroup);
		foreach (Geom geom in _geoms)
		{
			physicsSimulator.Add(geom);
		}
	}

	public void CreateGeoms(int collisionGroup)
	{
		foreach (Body body in _bodies)
		{
			Geom geom = GeomFactory.Instance.CreateRectangleGeom(body, _width, _height);
			geom.CollisionGroup = collisionGroup;
			_geoms.Add(geom);
		}
	}

	public void CreateGeoms(CollisionCategory collisionCategory, CollisionCategory collidesWith, PhysicsSimulator physicsSimulator)
	{
		CreateGeoms(collisionCategory, collidesWith);
		foreach (Geom geom in _geoms)
		{
			physicsSimulator.Add(geom);
		}
	}

	public void CreateGeoms(CollisionCategory collisionCategory, CollisionCategory collidesWith)
	{
		foreach (Body body in _bodies)
		{
			Geom geom = GeomFactory.Instance.CreateRectangleGeom(body, _width, _height);
			geom.CollisionCategories = collisionCategory;
			geom.CollidesWith = collidesWith;
			_geoms.Add(geom);
		}
	}

	public int PointInControlPoint(Vector2 point)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		foreach (Vector2 controlPoint in _controlPoints)
		{
			Vector2 min = new Vector2(controlPoint.X - 3f, controlPoint.Y - 3f);
			Vector2 max = new Vector2(controlPoint.X + 3f, controlPoint.Y + 3f);
			if (new AABB(ref min, ref max).Contains(ref point))
			{
				return _controlPoints.IndexOf(controlPoint);
			}
		}
		return -1;
	}

	public void MoveControlPoint(Vector2 position, int index)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_controlPoints[index] = position;
		_recalculate = true;
	}

	public void Add(Vector2 controlPoint)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_controlPoints.Add(controlPoint);
		_recalculate = true;
	}

	public void Add(Body body)
	{
		_bodies.Add(body);
	}

	public void Add(Geom geom)
	{
		_geoms.Add(geom);
	}

	public void Add(Joint joint)
	{
		_joints.Add(joint);
	}

	public void Add(Spring spring)
	{
		_springs.Add(spring);
	}

	public void Remove(Vector2 controlPoint)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_controlPoints.Remove(controlPoint);
		_recalculate = true;
	}

	public void Remove(int index)
	{
		_controlPoints.RemoveAt(index);
		_recalculate = true;
	}

	public void Update()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		Vector2 val = default(Vector2);
		Vector2 val2 = default(Vector2);
		if (!_recalculate)
		{
			return;
		}
		Curve val3 = new Curve();
		Curve val4 = new Curve();
		float num2 = 1f / (float)_controlPoints.Count;
		float num3;
		for (int i = 0; i < _controlPoints.Count; i++)
		{
			num3 = num2 * (float)(i + 1);
			val3.Keys.Add(new CurveKey(num3, _controlPoints[i].X));
			val4.Keys.Add(new CurveKey(num3, _controlPoints[i].Y));
		}
		num3 = 0f;
		val3.ComputeTangents((CurveTangent)2);
		val4.ComputeTangents((CurveTangent)2);
		while (num < _linkWidth / 2f)
		{
			num3 += 0.0005f;
			((Vector2)(ref val))._002Ector(val3.Evaluate(num3), val4.Evaluate(num3));
			num = Vector2.Distance(_controlPoints[0], val);
		}
		while (num < _linkWidth)
		{
			num3 += 0.0005f;
			((Vector2)(ref val2))._002Ector(val3.Evaluate(num3), val4.Evaluate(num3));
			num = Vector2.Distance(_controlPoints[0], val2);
		}
		Body body = BodyFactory.Instance.CreateRectangleBody(_width, _height, _mass);
		body.Position = val;
		body.Rotation = Vertices.FindNormalAngle(Vertices.FindVertexNormal(_controlPoints[0], val, val2));
		_bodies.Add(body);
		Vector2 val5 = val;
		while (num3 < 1f)
		{
			num = 0f;
			while (num < _linkWidth && num3 < 1f)
			{
				num3 += 0.0005f;
				((Vector2)(ref val))._002Ector(val3.Evaluate(num3), val4.Evaluate(num3));
				num = Vector2.Distance(val, val5);
			}
			num = 0f;
			while (num < _linkWidth && num3 < 1f)
			{
				num3 += 0.0005f;
				((Vector2)(ref val2))._002Ector(val3.Evaluate(num3), val4.Evaluate(num3));
				num = Vector2.Distance(val, val2);
			}
			body = BodyFactory.Instance.CreateRectangleBody(_width, _height, _mass);
			body.Position = val;
			body.Rotation = Vertices.FindNormalAngle(Vertices.FindVertexNormal(val5, val, val2));
			_bodies.Add(body);
			val5 = val;
		}
		MoveControlPoint(val2, _controlPoints.Count - 1);
		_recalculate = false;
	}
}
