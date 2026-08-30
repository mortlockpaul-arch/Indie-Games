using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace FarseerGames.FarseerPhysics.Collisions;

public class Geom : IEquatable<Geom>, IIsDisposable, IDisposable
{
	private bool _isSensor;

	private Matrix _matrix;

	private Matrix _matrixInverse;

	private bool _matrixInverseCached;

	private Vector2 _position;

	private Vector2 _positionOffset;

	private float _rotation;

	private float _rotationOffset;

	private bool _isDisposed;

	internal Body body;

	internal Vertices localVertices;

	internal Vertices worldVertices;

	internal int id;

	private Dictionary<int, bool> _collisionIgnores;

	public bool InSimulation;

	public CollisionCategory CollidesWith;

	public CollisionCategory CollisionCategories;

	public bool CollisionResponseEnabled;

	public float FrictionCoefficient;

	[XmlIgnore]
	[ContentSerializerIgnore]
	public CollisionEventHandler OnCollision;

	[XmlIgnore]
	[ContentSerializerIgnore]
	public SeparationEventHandler OnSeparation;

	public float RestitutionCoefficient;

	public int CollisionGroup;

	[XmlIgnore]
	[ContentSerializerIgnore]
	public AABB AABB;

	public bool CollisionEnabled;

	[XmlIgnore]
	[ContentSerializerIgnore]
	public object Tag;

	private static int _newId = -1;

	private Vector2 _localVertice;

	private Vector2 _vertice;

	private Vector2 _tempVector;

	private Matrix _matrixInverseTemp;

	public int Id => id;

	public int CollisionId { get; set; }

	public Vector2 Position
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _position;
		}
	}

	public float Rotation => _rotation;

	[XmlIgnore]
	[ContentSerializerIgnore]
	public Vertices LocalVertices => localVertices;

	[XmlIgnore]
	[ContentSerializerIgnore]
	public Vertices WorldVertices => worldVertices;

	[ContentSerializerIgnore]
	[XmlIgnore]
	public Matrix Matrix
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _matrix;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_matrix = value;
			_matrixInverseCached = false;
			Update();
		}
	}

	public Matrix MatrixInverse
	{
		get
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			if (!_matrixInverseCached)
			{
				Matrix.Invert(ref _matrix, ref _matrixInverse);
				_matrixInverseCached = true;
			}
			return _matrixInverse;
		}
	}

	public bool IsSensor
	{
		get
		{
			return _isSensor;
		}
		set
		{
			_isSensor = value;
			if (_isSensor)
			{
				body.IsStatic = true;
				CollisionResponseEnabled = false;
			}
			else
			{
				body.IsStatic = false;
				CollisionResponseEnabled = true;
			}
		}
	}

	[ContentSerializerIgnore]
	[XmlIgnore]
	public Body Body => body;

	public float GridCellSize { get; internal set; }

	[ContentSerializerIgnore]
	[XmlIgnore]
	public bool IsDisposed
	{
		get
		{
			return _isDisposed;
		}
		set
		{
			_isDisposed = value;
		}
	}

	public Geom()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		_matrix = Matrix.Identity;
		_matrixInverse = Matrix.Identity;
		_matrixInverseCached = true;
		_position = Vector2.Zero;
		_positionOffset = Vector2.Zero;
		_collisionIgnores = new Dictionary<int, bool>();
		InSimulation = true;
		CollidesWith = CollisionCategory.All;
		CollisionCategories = CollisionCategory.All;
		CollisionResponseEnabled = true;
		CollisionEnabled = true;
		_vertice = Vector2.Zero;
		base._002Ector();
		id = GetNextId();
	}

	public Geom(Body body, Vertices vertices, float collisionGridSize)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		_matrix = Matrix.Identity;
		_matrixInverse = Matrix.Identity;
		_matrixInverseCached = true;
		_position = Vector2.Zero;
		_positionOffset = Vector2.Zero;
		_collisionIgnores = new Dictionary<int, bool>();
		InSimulation = true;
		CollidesWith = CollisionCategory.All;
		CollisionCategories = CollisionCategory.All;
		CollisionResponseEnabled = true;
		CollisionEnabled = true;
		_vertice = Vector2.Zero;
		base._002Ector();
		Construct(body, vertices, Vector2.Zero, 0f, collisionGridSize);
	}

	public Geom(Body body, Vertices vertices, Vector2 offset, float rotationOffset, float collisionGridSize)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		_matrix = Matrix.Identity;
		_matrixInverse = Matrix.Identity;
		_matrixInverseCached = true;
		_position = Vector2.Zero;
		_positionOffset = Vector2.Zero;
		_collisionIgnores = new Dictionary<int, bool>();
		InSimulation = true;
		CollidesWith = CollisionCategory.All;
		CollisionCategories = CollisionCategory.All;
		CollisionResponseEnabled = true;
		CollisionEnabled = true;
		_vertice = Vector2.Zero;
		base._002Ector();
		Construct(body, vertices, offset, rotationOffset, collisionGridSize);
	}

	public Geom(Body body, Geom geometry)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		_matrix = Matrix.Identity;
		_matrixInverse = Matrix.Identity;
		_matrixInverseCached = true;
		_position = Vector2.Zero;
		_positionOffset = Vector2.Zero;
		_collisionIgnores = new Dictionary<int, bool>();
		InSimulation = true;
		CollidesWith = CollisionCategory.All;
		CollisionCategories = CollisionCategory.All;
		CollisionResponseEnabled = true;
		CollisionEnabled = true;
		_vertice = Vector2.Zero;
		base._002Ector();
		ConstructClone(body, geometry, geometry._positionOffset, geometry._rotationOffset);
	}

	public Geom(Body body, Geom geometry, Vector2 offset, float rotationOffset)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		_matrix = Matrix.Identity;
		_matrixInverse = Matrix.Identity;
		_matrixInverseCached = true;
		_position = Vector2.Zero;
		_positionOffset = Vector2.Zero;
		_collisionIgnores = new Dictionary<int, bool>();
		InSimulation = true;
		CollidesWith = CollisionCategory.All;
		CollisionCategories = CollisionCategory.All;
		CollisionResponseEnabled = true;
		CollisionEnabled = true;
		_vertice = Vector2.Zero;
		base._002Ector();
		ConstructClone(body, geometry, offset, rotationOffset);
	}

	private void Construct(Body bodyToSet, Vertices vertices, Vector2 positionOffset, float rotationOffset, float collisionGridSize)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		GridCellSize = collisionGridSize;
		id = GetNextId();
		_positionOffset = positionOffset;
		_rotationOffset = rotationOffset;
		SetVertices(vertices);
		SetBody(bodyToSet);
		if (PhysicsSimulator.NarrowPhaseCollider == NarrowPhaseCollider.DistanceGrid)
		{
			DistanceGrid.Instance.CreateDistanceGrid(this);
		}
	}

	private void ConstructClone(Body bodyToSet, Geom geometry, Vector2 positionOffset, float rotationOffset)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		id = GetNextId();
		_positionOffset = positionOffset;
		_rotationOffset = rotationOffset;
		RestitutionCoefficient = geometry.RestitutionCoefficient;
		FrictionCoefficient = geometry.FrictionCoefficient;
		GridCellSize = geometry.GridCellSize;
		CollisionGroup = geometry.CollisionGroup;
		CollisionEnabled = geometry.CollisionEnabled;
		CollisionResponseEnabled = geometry.CollisionResponseEnabled;
		CollisionCategories = geometry.CollisionCategories;
		CollidesWith = geometry.CollidesWith;
		SetVertices(geometry.localVertices);
		SetBody(bodyToSet);
		if (PhysicsSimulator.NarrowPhaseCollider == NarrowPhaseCollider.DistanceGrid)
		{
			DistanceGrid.Instance.Copy(geometry.id, id);
		}
	}

	public void SetVertices(Vertices vertices)
	{
		vertices.ForceCounterClockWiseOrder();
		localVertices = new Vertices(vertices);
		worldVertices = new Vertices(vertices);
		AABB.Update(ref vertices);
	}

	public void SetBody(Body bodyToSet)
	{
		if (body != null)
		{
			Body obj = body;
			obj.Updated = (UpdatedEventHandler)Delegate.Remove(obj.Updated, new UpdatedEventHandler(Update));
			body.Disposed -= BodyOnDisposed;
		}
		body = bodyToSet;
		bodyToSet.Updated = (UpdatedEventHandler)Delegate.Combine(bodyToSet.Updated, new UpdatedEventHandler(Update));
		bodyToSet.Disposed += BodyOnDisposed;
		Update(ref bodyToSet.position, ref bodyToSet.rotation);
	}

	public Vector2 GetWorldPosition(Vector2 localPosition)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return Vector2.Transform(localPosition, _matrix);
	}

	public float GetNearestDistance(ref Vector2 point)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		float num = float.MaxValue;
		int index = 0;
		for (int i = 0; i < localVertices.Count; i++)
		{
			float distanceToEdge = GetDistanceToEdge(ref point, i);
			if (distanceToEdge < num)
			{
				num = distanceToEdge;
				index = i;
			}
		}
		Feature nearestFeature = GetNearestFeature(ref point, index);
		Vector2 val = Vector2.Subtract(point, nearestFeature.Position);
		float num2 = Vector2.Dot(val, nearestFeature.Normal);
		if (num2 < 0f)
		{
			return 0f - nearestFeature.Distance;
		}
		return nearestFeature.Distance;
	}

	private float GetDistanceToEdge(ref Vector2 point, int index)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		Vector2 edge = localVertices.GetEdge(index);
		Vector2 val = Vector2.Subtract(point, localVertices[index]);
		float num = Vector2.Dot(val, edge);
		if (num < 0f)
		{
			return Math.Abs(((Vector2)(ref val)).Length());
		}
		float num2 = Vector2.Dot(edge, edge);
		if (num2 <= num)
		{
			return Vector2.Distance(point, localVertices[localVertices.NextIndex(index)]);
		}
		float num3 = num / num2;
		edge = Vector2.Multiply(edge, num3);
		Vector2 val2 = Vector2.Add(localVertices[index], edge);
		return Vector2.Distance(point, val2);
	}

	public Feature GetNearestFeature(ref Vector2 point, int index)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		Feature result = default(Feature);
		Vector2 edge = localVertices.GetEdge(index);
		Vector2 val = Vector2.Subtract(point, localVertices[index]);
		float num = Vector2.Dot(val, edge);
		if (num < 0f)
		{
			result.Position = localVertices[index];
			result.Normal = localVertices.GetVertexNormal(index);
			result.Distance = Math.Abs(((Vector2)(ref val)).Length());
			return result;
		}
		float num2 = Vector2.Dot(edge, edge);
		if (num2 <= num)
		{
			Vector2 val2 = Vector2.Subtract(point, localVertices[localVertices.NextIndex(index)]);
			result.Position = localVertices[localVertices.NextIndex(index)];
			result.Normal = localVertices.GetVertexNormal(localVertices.NextIndex(index));
			result.Distance = Math.Abs(((Vector2)(ref val2)).Length());
			return result;
		}
		float num3 = num / num2;
		edge = Vector2.Multiply(edge, num3);
		Vector2 val3 = Vector2.Add(localVertices[index], edge);
		Vector2 val4 = Vector2.Subtract(point, val3);
		result.Position = val3;
		result.Normal = localVertices.GetEdgeNormal(index);
		result.Distance = ((Vector2)(ref val4)).Length();
		return result;
	}

	public bool Collide(Vector2 position)
	{
		return Collide(ref position);
	}

	public bool Collide(ref Vector2 position)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (AABB.Contains(ref position))
		{
			Matrix matrixInverse = MatrixInverse;
			Vector2.Transform(ref position, ref matrixInverse, ref position);
			if (PhysicsSimulator.NarrowPhaseCollider == NarrowPhaseCollider.DistanceGrid)
			{
				return DistanceGrid.Instance.Intersect(this, ref position);
			}
			if (PhysicsSimulator.NarrowPhaseCollider == NarrowPhaseCollider.SAT)
			{
				return SAT.Instance.Intersect(this, ref position);
			}
		}
		return false;
	}

	private bool FastCollide(ref Vector2 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Matrix matrixInverse = MatrixInverse;
		Vector2.Transform(ref position, ref matrixInverse, ref position);
		if (PhysicsSimulator.NarrowPhaseCollider == NarrowPhaseCollider.DistanceGrid)
		{
			return DistanceGrid.Instance.Intersect(this, ref position);
		}
		if (PhysicsSimulator.NarrowPhaseCollider == NarrowPhaseCollider.SAT)
		{
			return SAT.Instance.Intersect(this, ref position);
		}
		return false;
	}

	public bool Collide(Geom geometry)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (AABB.Intersect(ref AABB, ref geometry.AABB))
		{
			int count = worldVertices.Count;
			for (int i = 0; i < count; i++)
			{
				_tempVector = worldVertices[i];
				if (geometry.FastCollide(ref _tempVector))
				{
					return true;
				}
			}
			count = geometry.worldVertices.Count;
			for (int j = 0; j < count; j++)
			{
				_tempVector = geometry.worldVertices[j];
				if (FastCollide(ref _tempVector))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void TransformToLocalCoordinates(ref Vector2 worldVertex, out Vector2 localVertex)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_matrixInverseTemp = MatrixInverse;
		Vector2.Transform(ref worldVertex, ref _matrixInverseTemp, ref localVertex);
	}

	public void TransformNormalToWorld(ref Vector2 localNormal, out Vector2 worldNormal)
	{
		Vector2.TransformNormal(ref localNormal, ref _matrix, ref worldNormal);
	}

	private void Update(ref Vector2 position, ref float rotation)
	{
		Matrix.CreateRotationZ(rotation + _rotationOffset, ref _matrix);
		float num = _positionOffset.X * _matrix.M11 + _positionOffset.Y * _matrix.M21 + _matrix.M41;
		float num2 = _positionOffset.X * _matrix.M12 + _positionOffset.Y * _matrix.M22 + _matrix.M42;
		_matrix.M41 = position.X + num;
		_matrix.M42 = position.Y + num2;
		_matrix.M44 = 1f;
		_position.X = _matrix.M41;
		_position.Y = _matrix.M42;
		_rotation = body.rotation + _rotationOffset;
		_matrixInverseCached = false;
		Update();
	}

	private void Update()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < localVertices.Count; i++)
		{
			_localVertice = localVertices[i];
			float x = _localVertice.X * _matrix.M11 + _localVertice.Y * _matrix.M21 + _matrix.M41;
			float y = _localVertice.X * _matrix.M12 + _localVertice.Y * _matrix.M22 + _matrix.M42;
			_vertice.X = x;
			_vertice.Y = y;
			worldVertices[i] = _vertice;
		}
		AABB.Update(ref worldVertices);
	}

	public void RestoreCollisionWith(Geom geometry)
	{
		if (_collisionIgnores.ContainsKey(geometry.id))
		{
			_collisionIgnores[geometry.id] = false;
		}
	}

	public void IgnoreCollisionWith(Geom geometry)
	{
		if (_collisionIgnores.ContainsKey(geometry.id))
		{
			_collisionIgnores[geometry.id] = true;
		}
		else
		{
			_collisionIgnores.Add(geometry.id, value: true);
		}
	}

	public bool IsGeometryIgnored(Geom geometry)
	{
		if (_collisionIgnores.ContainsKey(geometry.id))
		{
			return _collisionIgnores[geometry.id];
		}
		return false;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Geom other))
		{
			return false;
		}
		return Equals(other);
	}

	public static bool operator ==(Geom geometry1, Geom geometry2)
	{
		if (object.ReferenceEquals(geometry1, geometry2))
		{
			return true;
		}
		if ((object)geometry1 == null || (object)geometry2 == null)
		{
			return false;
		}
		return geometry1.Equals(geometry2);
	}

	public static bool operator !=(Geom geometry1, Geom geometry2)
	{
		return !(geometry1 == geometry2);
	}

	public static bool operator <(Geom geometry1, Geom geometry2)
	{
		return geometry1.id < geometry2.id;
	}

	public static bool operator >(Geom geometry1, Geom geometry2)
	{
		return geometry1.id > geometry2.id;
	}

	private static int GetNextId()
	{
		_newId++;
		return _newId;
	}

	private void BodyOnDisposed(object sender, EventArgs e)
	{
		Dispose();
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!IsDisposed && disposing)
		{
			if (PhysicsSimulator.NarrowPhaseCollider == NarrowPhaseCollider.DistanceGrid)
			{
				DistanceGrid.Instance.RemoveDistanceGrid(this);
			}
			if (body != null)
			{
				Body obj = body;
				obj.Updated = (UpdatedEventHandler)Delegate.Remove(obj.Updated, new UpdatedEventHandler(Update));
				body.Disposed -= BodyOnDisposed;
			}
		}
		IsDisposed = true;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public bool Equals(Geom other)
	{
		if ((object)other == null)
		{
			return false;
		}
		return id == other.id;
	}
}
