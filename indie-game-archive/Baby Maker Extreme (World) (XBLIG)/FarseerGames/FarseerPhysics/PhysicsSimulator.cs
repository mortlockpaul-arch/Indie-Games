using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Controllers;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Dynamics.Joints;
using FarseerGames.FarseerPhysics.Dynamics.Springs;
using FarseerGames.FarseerPhysics.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace FarseerGames.FarseerPhysics;

public class PhysicsSimulator
{
	private IBroadPhaseCollider _broadPhaseCollider;

	internal Pool<Arbiter> arbiterPool;

	private List<Body> _bodyAddList;

	private List<Body> _bodyRemoveList;

	private List<Controller> _controllerAddList;

	private List<Controller> _controllerRemoveList;

	private List<Geom> _geomAddList;

	private List<Geom> _geomRemoveList;

	private List<Joint> _jointAddList;

	private List<Joint> _jointRemoveList;

	private List<Spring> _springAddList;

	private List<Spring> _springRemoveList;

	private Stopwatch _sw;

	public static NarrowPhaseCollider NarrowPhaseCollider = NarrowPhaseCollider.DistanceGrid;

	public bool Enabled;

	public float BiasFactor;

	public static int MaxContactsToDetect = 10;

	public int MaxContactsToResolve;

	public FrictionType FrictionType;

	public float AllowedPenetration;

	public Vector2 Gravity;

	public int Iterations;

	public bool EnableDiagnostics;

	private int _tempCount;

	public IBroadPhaseCollider BroadPhaseCollider
	{
		get
		{
			return _broadPhaseCollider;
		}
		set
		{
			if (GeomList.Count > 0)
			{
				throw new Exception("The GeomList must be empty when setting the broad phase collider type");
			}
			_broadPhaseCollider = value;
		}
	}

	[ContentSerializerIgnore]
	[XmlIgnore]
	public GenericList<Geom> GeomList { get; internal set; }

	[XmlIgnore]
	[ContentSerializerIgnore]
	public GenericList<Body> BodyList { get; internal set; }

	[ContentSerializerIgnore]
	[XmlIgnore]
	public GenericList<Controller> ControllerList { get; internal set; }

	[ContentSerializerIgnore]
	[XmlIgnore]
	public GenericList<Spring> SpringList { get; internal set; }

	[XmlIgnore]
	[ContentSerializerIgnore]
	public GenericList<Joint> JointList { get; internal set; }

	[XmlIgnore]
	[ContentSerializerIgnore]
	public ArbiterList ArbiterList { get; internal set; }

	[ContentSerializerIgnore]
	[XmlIgnore]
	public float CleanUpTime { get; internal set; }

	[ContentSerializerIgnore]
	[XmlIgnore]
	public float BroadPhaseCollisionTime { get; internal set; }

	[ContentSerializerIgnore]
	[XmlIgnore]
	public float NarrowPhaseCollisionTime { get; internal set; }

	[XmlIgnore]
	[ContentSerializerIgnore]
	public float ApplyForcesTime { get; internal set; }

	[ContentSerializerIgnore]
	[XmlIgnore]
	public float ApplyImpulsesTime { get; internal set; }

	[ContentSerializerIgnore]
	[XmlIgnore]
	public float UpdatePositionsTime { get; internal set; }

	[ContentSerializerIgnore]
	[XmlIgnore]
	public float UpdateTime { get; internal set; }

	public PhysicsSimulator()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		_sw = new Stopwatch();
		Enabled = true;
		BiasFactor = 0.8f;
		MaxContactsToResolve = 4;
		AllowedPenetration = 0.05f;
		Iterations = 5;
		base._002Ector();
		ConstructPhysicsSimulator(Vector2.Zero);
	}

	public PhysicsSimulator(Vector2 gravity)
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		_sw = new Stopwatch();
		Enabled = true;
		BiasFactor = 0.8f;
		MaxContactsToResolve = 4;
		AllowedPenetration = 0.05f;
		Iterations = 5;
		base._002Ector();
		UpdateTime = -1f;
		UpdatePositionsTime = -1f;
		ApplyImpulsesTime = -1f;
		ApplyForcesTime = -1f;
		NarrowPhaseCollisionTime = -1f;
		BroadPhaseCollisionTime = -1f;
		CleanUpTime = -1f;
		ConstructPhysicsSimulator(gravity);
	}

	private void ConstructPhysicsSimulator(Vector2 gravity)
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		GeomList = new GenericList<Geom>(32);
		_geomAddList = new List<Geom>(32);
		_geomRemoveList = new List<Geom>(32);
		BodyList = new GenericList<Body>(32);
		_bodyAddList = new List<Body>(32);
		_bodyRemoveList = new List<Body>(32);
		ControllerList = new GenericList<Controller>(8);
		_controllerAddList = new List<Controller>(8);
		_controllerRemoveList = new List<Controller>(8);
		JointList = new GenericList<Joint>(32);
		_jointAddList = new List<Joint>(32);
		_jointRemoveList = new List<Joint>(32);
		SpringList = new GenericList<Spring>(32);
		_springAddList = new List<Spring>(32);
		_springRemoveList = new List<Spring>(32);
		_broadPhaseCollider = new SelectiveSweepCollider(this);
		Gravity = gravity;
		ArbiterList = new ArbiterList(128);
		arbiterPool = new Pool<Arbiter>(128);
	}

	public void Add(Geom geometry)
	{
		if (geometry == null)
		{
			throw new ArgumentNullException("geometry", "Can't add null geometry");
		}
		if (!_geomAddList.Contains(geometry))
		{
			_geomAddList.Add(geometry);
		}
	}

	public void Remove(Geom geometry)
	{
		if (geometry == null)
		{
			throw new ArgumentNullException("geometry", "Can't remove null geometry");
		}
		_geomRemoveList.Add(geometry);
	}

	public void Add(Body body)
	{
		if (body == null)
		{
			throw new ArgumentNullException("body", "Can't add null body");
		}
		if (!_bodyAddList.Contains(body))
		{
			_bodyAddList.Add(body);
		}
	}

	public void Remove(Body body)
	{
		if (body == null)
		{
			throw new ArgumentNullException("body", "Can't remove null body");
		}
		_bodyRemoveList.Add(body);
	}

	public void Add(Controller controller)
	{
		if (controller == null)
		{
			throw new ArgumentNullException("controller", "Can't add null controller");
		}
		if (!_controllerAddList.Contains(controller))
		{
			_controllerAddList.Add(controller);
		}
	}

	public void Remove(Controller controller)
	{
		if (controller == null)
		{
			throw new ArgumentNullException("controller", "Can't remove null controller");
		}
		_controllerRemoveList.Add(controller);
	}

	public void Add(Joint joint)
	{
		if (joint == null)
		{
			throw new ArgumentNullException("joint", "Can't add null joint");
		}
		if (!_jointAddList.Contains(joint))
		{
			_jointAddList.Add(joint);
		}
	}

	public void Remove(Joint joint)
	{
		if (joint == null)
		{
			throw new ArgumentNullException("joint", "Can't remove null joint");
		}
		_jointRemoveList.Add(joint);
	}

	public void Add(Spring spring)
	{
		if (spring == null)
		{
			throw new ArgumentNullException("spring", "Can't add null spring");
		}
		if (!_springAddList.Contains(spring))
		{
			_springAddList.Add(spring);
		}
	}

	public void Remove(Spring spring)
	{
		if (spring == null)
		{
			throw new ArgumentNullException("spring", "Can't remove null spring");
		}
		_springRemoveList.Add(spring);
	}

	public void Clear()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		ConstructPhysicsSimulator(Gravity);
	}

	public void Update(float dt)
	{
		Update(dt, 0f);
	}

	public void ProcessAddedAndRemoved()
	{
		ProcessAddedItems();
		ProcessRemovedItems();
		ProcessDisposedItems();
	}

	public void Update(float dt, float dtReal)
	{
		if (EnableDiagnostics)
		{
			_sw.Start();
		}
		ProcessAddedItems();
		ProcessRemovedItems();
		ProcessDisposedItems();
		ArbiterList.PrepareForBroadphaseCollision(GeomList);
		if (EnableDiagnostics)
		{
			CleanUpTime = _sw.ElapsedTicks;
		}
		if (dt != 0f && Enabled)
		{
			DoBroadPhaseCollision();
			if (EnableDiagnostics)
			{
				BroadPhaseCollisionTime = (float)_sw.ElapsedTicks - CleanUpTime;
			}
			DoNarrowPhaseCollision();
			if (EnableDiagnostics)
			{
				NarrowPhaseCollisionTime = (float)_sw.ElapsedTicks - BroadPhaseCollisionTime - CleanUpTime;
			}
			ApplyForces(dt, dtReal);
			if (EnableDiagnostics)
			{
				ApplyForcesTime = (float)_sw.ElapsedTicks - NarrowPhaseCollisionTime - BroadPhaseCollisionTime - CleanUpTime;
			}
			ApplyImpulses(dt);
			if (EnableDiagnostics)
			{
				ApplyImpulsesTime = (float)_sw.ElapsedTicks - ApplyForcesTime - NarrowPhaseCollisionTime - BroadPhaseCollisionTime - CleanUpTime;
			}
			UpdatePositions(dt);
			if (EnableDiagnostics)
			{
				UpdatePositionsTime = (float)_sw.ElapsedTicks - ApplyImpulsesTime - ApplyForcesTime - NarrowPhaseCollisionTime - BroadPhaseCollisionTime - CleanUpTime;
			}
			if (EnableDiagnostics)
			{
				_sw.Stop();
				UpdateTime = _sw.ElapsedTicks;
				CleanUpTime = 1000f * CleanUpTime / (float)Stopwatch.Frequency;
				BroadPhaseCollisionTime = 1000f * BroadPhaseCollisionTime / (float)Stopwatch.Frequency;
				NarrowPhaseCollisionTime = 1000f * NarrowPhaseCollisionTime / (float)Stopwatch.Frequency;
				ApplyForcesTime = 1000f * ApplyForcesTime / (float)Stopwatch.Frequency;
				ApplyImpulsesTime = 1000f * ApplyImpulsesTime / (float)Stopwatch.Frequency;
				UpdatePositionsTime = 1000f * UpdatePositionsTime / (float)Stopwatch.Frequency;
				UpdateTime = 1000f * UpdateTime / (float)Stopwatch.Frequency;
				_sw.Reset();
			}
		}
	}

	public Geom Collide(float x, float y)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return Collide(new Vector2(x, y));
	}

	public Geom Collide(Vector2 point)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		foreach (Geom geom in GeomList)
		{
			if (geom.Collide(point))
			{
				return geom;
			}
		}
		return null;
	}

	public List<Geom> CollideAll(float x, float y)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return CollideAll(new Vector2(x, y));
	}

	public List<Geom> CollideAll(Vector2 point)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		List<Geom> list = new List<Geom>();
		foreach (Geom geom in GeomList)
		{
			if (geom.Collide(point))
			{
				list.Add(geom);
			}
		}
		return list;
	}

	private void DoBroadPhaseCollision()
	{
		_broadPhaseCollider.Update();
	}

	private void DoNarrowPhaseCollision()
	{
		for (int i = 0; i < ArbiterList.Count; i++)
		{
			ArbiterList[i].Collide();
		}
		ArbiterList.RemoveContactCountEqualsZero(arbiterPool);
	}

	private void ApplyForces(float dt, float dtReal)
	{
		for (int i = 0; i < ControllerList.Count; i++)
		{
			if (ControllerList[i].Enabled && !ControllerList[i].IsDisposed)
			{
				ControllerList[i].Update(dt, dtReal);
			}
		}
		for (int j = 0; j < SpringList.Count; j++)
		{
			if (SpringList[j].Enabled && !SpringList[j].IsDisposed)
			{
				SpringList[j].Update(dt);
			}
		}
		for (int k = 0; k < BodyList.Count; k++)
		{
			Body body = BodyList[k];
			if (body.Enabled && !body.isStatic && !body.IsDisposed)
			{
				body.ApplyImpulses();
				if (!body.IgnoreGravity)
				{
					body.force.X = body.force.X + Gravity.X * body.mass;
					body.force.Y = body.force.Y + Gravity.Y * body.mass;
				}
				body.IntegrateVelocity(dt);
				body.ClearForce();
				body.ClearTorque();
			}
		}
	}

	private void ApplyImpulses(float dt)
	{
		float inverseDt = 1f / dt;
		for (int i = 0; i < JointList.Count; i++)
		{
			if (JointList[i].Enabled && !JointList[i].IsDisposed)
			{
				JointList[i].PreStep(inverseDt);
			}
		}
		for (int j = 0; j < ArbiterList.Count; j++)
		{
			if (ArbiterList[j].GeometryA.CollisionResponseEnabled && ArbiterList[j].GeometryB.CollisionResponseEnabled)
			{
				ArbiterList[j].PreStepImpulse(ref inverseDt);
			}
		}
		for (int k = 0; k < Iterations; k++)
		{
			for (int l = 0; l < JointList.Count; l++)
			{
				if (JointList[l].Enabled && !JointList[l].IsDisposed)
				{
					JointList[l].Update();
				}
			}
			for (int m = 0; m < ArbiterList.Count; m++)
			{
				if (ArbiterList[m].GeometryA.CollisionResponseEnabled && ArbiterList[m].GeometryB.CollisionResponseEnabled)
				{
					ArbiterList[m].ApplyImpulse();
				}
			}
		}
	}

	private void UpdatePositions(float dt)
	{
		for (int i = 0; i < BodyList.Count; i++)
		{
			if (BodyList[i].Enabled && !BodyList[i].isStatic && !BodyList[i].IsDisposed)
			{
				BodyList[i].IntegratePosition(dt);
			}
		}
	}

	private void ProcessAddedItems()
	{
		_tempCount = _geomAddList.Count;
		for (int i = 0; i < _tempCount; i++)
		{
			if (!GeomList.Contains(_geomAddList[i]))
			{
				_geomAddList[i].InSimulation = true;
				GeomList.Add(_geomAddList[i]);
				_broadPhaseCollider.Add(_geomAddList[i]);
			}
		}
		_geomAddList.Clear();
		_tempCount = _bodyAddList.Count;
		for (int j = 0; j < _tempCount; j++)
		{
			if (!BodyList.Contains(_bodyAddList[j]))
			{
				BodyList.Add(_bodyAddList[j]);
			}
		}
		_bodyAddList.Clear();
		_tempCount = _controllerAddList.Count;
		for (int k = 0; k < _tempCount; k++)
		{
			if (!ControllerList.Contains(_controllerAddList[k]))
			{
				ControllerList.Add(_controllerAddList[k]);
			}
		}
		_controllerAddList.Clear();
		_tempCount = _jointAddList.Count;
		for (int l = 0; l < _tempCount; l++)
		{
			if (!JointList.Contains(_jointAddList[l]))
			{
				JointList.Add(_jointAddList[l]);
			}
		}
		_jointAddList.Clear();
		_tempCount = _springAddList.Count;
		for (int m = 0; m < _tempCount; m++)
		{
			if (!SpringList.Contains(_springAddList[m]))
			{
				SpringList.Add(_springAddList[m]);
			}
		}
		_springAddList.Clear();
	}

	private void ProcessRemovedItems()
	{
		_tempCount = _geomRemoveList.Count;
		for (int i = 0; i < _tempCount; i++)
		{
			_geomRemoveList[i].InSimulation = false;
			GeomList.Remove(_geomRemoveList[i]);
			for (int num = ArbiterList.Count; num > 0; num--)
			{
				if (ArbiterList[num - 1].GeometryA == _geomRemoveList[i] || ArbiterList[num - 1].GeometryB == _geomRemoveList[i])
				{
					arbiterPool.Insert(ArbiterList[num - 1]);
					ArbiterList.Remove(ArbiterList[num - 1]);
				}
			}
		}
		if (_geomRemoveList.Count > 0)
		{
			_broadPhaseCollider.ProcessRemovedGeoms();
		}
		_geomRemoveList.Clear();
		_tempCount = _bodyRemoveList.Count;
		for (int j = 0; j < _tempCount; j++)
		{
			BodyList.Remove(_bodyRemoveList[j]);
		}
		_bodyRemoveList.Clear();
		_tempCount = _controllerRemoveList.Count;
		for (int k = 0; k < _tempCount; k++)
		{
			ControllerList.Remove(_controllerRemoveList[k]);
		}
		_controllerRemoveList.Clear();
		int count = _jointRemoveList.Count;
		for (int l = 0; l < count; l++)
		{
			JointList.Remove(_jointRemoveList[l]);
		}
		_jointRemoveList.Clear();
		_tempCount = _springRemoveList.Count;
		for (int m = 0; m < _tempCount; m++)
		{
			SpringList.Remove(_springRemoveList[m]);
		}
		_springRemoveList.Clear();
	}

	private void ProcessDisposedItems()
	{
		for (int i = 0; i < ControllerList.Count; i++)
		{
			ControllerList[i].Validate();
		}
		for (int j = 0; j < JointList.Count; j++)
		{
			JointList[j].Validate();
		}
		for (int k = 0; k < SpringList.Count; k++)
		{
			SpringList[k].Validate();
		}
		_tempCount = GeomList.RemoveDisposed();
		if (_tempCount > 0)
		{
			_broadPhaseCollider.ProcessDisposedGeoms();
		}
		BodyList.RemoveDisposed();
		ControllerList.RemoveDisposed();
		SpringList.RemoveDisposed();
		JointList.RemoveDisposed();
		ArbiterList.CleanArbiterList(arbiterPool);
	}
}
