using System;
using System.Collections.Generic;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Interfaces;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Controllers;

public sealed class FluidDragController : Controller
{
	public delegate void EntryEventHandler(Geom geom, Vertices verts);

	private float _area;

	private Vector2 _axis;

	private Vector2 _buoyancyForce;

	private Vector2 _centroid;

	private Vector2 _centroidVelocity;

	private float _dragArea;

	private IFluidContainer _fluidContainer;

	private Dictionary<Geom, bool> _geomInFluidList;

	private List<Geom> _geomList;

	private Vector2 _gravity;

	private Vector2 _linearDragForce;

	private float _max;

	private float _min;

	private float _partialMass;

	private float _rotationalDragTorque;

	private float _totalArea;

	private Vector2 _totalForce;

	private Vector2 _vert;

	private Vertices _vertices;

	public EntryEventHandler Entry;

	public float Density { get; set; }

	public float LinearDragCoefficient { get; set; }

	public float RotationalDragCoefficient { get; set; }

	public FluidDragController()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		_axis = Vector2.Zero;
		_buoyancyForce = Vector2.Zero;
		_centroid = Vector2.Zero;
		_gravity = Vector2.Zero;
		_linearDragForce = Vector2.Zero;
		base._002Ector();
		_geomList = new List<Geom>();
		_geomInFluidList = new Dictionary<Geom, bool>();
	}

	public void Initialize(IFluidContainer fluidContainer, float density, float linearDragCoefficient, float rotationalDragCoefficient, Vector2 gravity)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		_fluidContainer = fluidContainer;
		Density = density;
		LinearDragCoefficient = linearDragCoefficient;
		RotationalDragCoefficient = rotationalDragCoefficient;
		_gravity = gravity;
		_vertices = new Vertices();
	}

	public void AddGeom(Geom geom)
	{
		_geomList.Add(geom);
		_geomInFluidList.Add(geom, value: false);
	}

	public void RemoveGeom(Geom geom)
	{
		_geomList.Remove(geom);
		_geomInFluidList.Remove(geom);
	}

	public override void Validate()
	{
	}

	public void Reset()
	{
		_geomInFluidList.Clear();
		for (int i = 0; i < _geomList.Count; i++)
		{
			_geomInFluidList.Add(_geomList[i], value: false);
		}
	}

	public override void Update(float dt, float dtReal)
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < _geomList.Count; i++)
		{
			_totalArea = _geomList[i].localVertices.GetArea();
			if (!_fluidContainer.Intersect(ref _geomList[i].AABB))
			{
				continue;
			}
			FindVerticesInFluid(_geomList[i]);
			if ((float)_vertices.Count < (float)_geomList[i].LocalVertices.Count * 0.15f)
			{
				_geomInFluidList[_geomList[i]] = false;
			}
			_area = _vertices.GetArea();
			if ((double)_area < 1E-06)
			{
				continue;
			}
			_centroid = _vertices.GetCentroid(_area);
			_buoyancyForce = -_gravity * _area * Density;
			CalculateDrag(_geomList[i]);
			Vector2.Add(ref _buoyancyForce, ref _linearDragForce, ref _totalForce);
			_geomList[i].body.ApplyForceAtWorldPoint(ref _totalForce, ref _centroid);
			_geomList[i].body.ApplyTorque(_rotationalDragTorque);
			if (!_geomInFluidList[_geomList[i]])
			{
				_geomInFluidList[_geomList[i]] = true;
				if (Entry != null)
				{
					Entry(_geomList[i], _vertices);
				}
			}
		}
	}

	private void FindVerticesInFluid(Geom geom)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		_vertices.Clear();
		for (int i = 0; i < geom.worldVertices.Count; i++)
		{
			_vert = geom.worldVertices[i];
			if (_fluidContainer.Contains(ref _vert))
			{
				_vertices.Add(_vert);
			}
		}
	}

	private void CalculateDrag(Geom geom)
	{
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		geom.body.GetVelocityAtWorldPoint(ref _centroid, out _centroidVelocity);
		_axis.X = 0f - _centroidVelocity.Y;
		_axis.Y = _centroidVelocity.X;
		if (_axis.X != 0f || _axis.Y != 0f)
		{
			((Vector2)(ref _axis)).Normalize();
		}
		_vertices.ProjectToAxis(ref _axis, out _min, out _max);
		_dragArea = Math.Abs(_max - _min);
		_partialMass = geom.body.mass * (_area / _totalArea);
		_linearDragForce = -0.5f * Density * _dragArea * LinearDragCoefficient * _partialMass * _centroidVelocity;
		_rotationalDragTorque = (0f - geom.body.AngularVelocity) * RotationalDragCoefficient * _partialMass;
	}
}
