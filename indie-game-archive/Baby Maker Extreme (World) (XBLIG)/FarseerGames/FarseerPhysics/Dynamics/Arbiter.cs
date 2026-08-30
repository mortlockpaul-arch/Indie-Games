using System;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Mathematics;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Dynamics;

public class Arbiter : IEquatable<Arbiter>
{
	private float _frictionCoefficientCombined;

	private ContactList _newContactList;

	private ContactList _mergedContactList;

	private PhysicsSimulator _physicsSimulator;

	private Vector2 _vec1;

	private Vector2 _vec2;

	public Geom GeometryA;

	public Geom GeometryB;

	private int i;

	private int j;

	private Contact indexContact;

	private int contactCount;

	private float _float1;

	private float _float2;

	private float _kNormal;

	private float _kTangent;

	private float _min;

	private Vector2 _r1;

	private Vector2 _r2;

	private float _restitution;

	private float _rn1;

	private float _rn2;

	private float _rt1;

	private float _rt2;

	private Contact _contact;

	private Vector2 _dv;

	private Vector2 _impulse;

	private Vector2 _impulseBias;

	private float _maxTangentImpulse;

	private float _normalImpulse;

	private float _normalImpulseBias;

	private float _normalImpulseBiasOriginal;

	private float _normalVelocityBias;

	private float _oldNormalImpulse;

	private float _oldTangentImpulse;

	private Vector2 _tangent;

	private float _tangentImpulse;

	private float _vn;

	private float _vt;

	public ContactList ContactList { get; private set; }

	public Arbiter()
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
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		_vec1 = Vector2.Zero;
		_vec2 = Vector2.Zero;
		_dv = Vector2.Zero;
		_impulse = Vector2.Zero;
		_impulseBias = Vector2.Zero;
		_tangent = Vector2.Zero;
		base._002Ector();
		ContactList = new ContactList(PhysicsSimulator.MaxContactsToDetect);
		_newContactList = new ContactList(PhysicsSimulator.MaxContactsToDetect);
		_mergedContactList = new ContactList(PhysicsSimulator.MaxContactsToDetect);
	}

	internal void ConstructArbiter(Geom geometry1, Geom geometry2, PhysicsSimulator physicsSimulator)
	{
		_physicsSimulator = physicsSimulator;
		if (geometry1 < geometry2)
		{
			GeometryA = geometry1;
			GeometryB = geometry2;
		}
		else
		{
			GeometryA = geometry2;
			GeometryB = geometry1;
		}
		switch (_physicsSimulator.FrictionType)
		{
		case FrictionType.Average:
			_frictionCoefficientCombined = (GeometryA.FrictionCoefficient + GeometryB.FrictionCoefficient) / 2f;
			break;
		case FrictionType.Minimum:
			_frictionCoefficientCombined = Math.Min(GeometryA.FrictionCoefficient, GeometryB.FrictionCoefficient);
			break;
		default:
			_frictionCoefficientCombined = (GeometryA.FrictionCoefficient + GeometryB.FrictionCoefficient) / 2f;
			break;
		}
	}

	internal void PreStepImpulse(ref float inverseDt)
	{
		for (i = 0; i < ContactList.Count; i++)
		{
			Contact value = ContactList[i];
			Vector2.Subtract(ref value.Position, ref GeometryA.body.position, ref _r1);
			Vector2.Subtract(ref value.Position, ref GeometryB.body.position, ref _r2);
			Vector2.Dot(ref _r1, ref value.Normal, ref _rn1);
			Vector2.Dot(ref _r2, ref value.Normal, ref _rn2);
			float num = GeometryA.body.inverseMass + GeometryB.body.inverseMass;
			Vector2.Dot(ref _r1, ref _r1, ref _float1);
			Vector2.Dot(ref _r2, ref _r2, ref _float2);
			_kNormal = num + GeometryA.body.inverseMomentOfInertia * (_float1 - _rn1 * _rn1) + GeometryB.body.inverseMomentOfInertia * (_float2 - _rn2 * _rn2);
			value.massNormal = 1f / _kNormal;
			_float1 = 1f;
			Calculator.Cross(ref value.Normal, ref _float1, out _tangent);
			Vector2.Dot(ref _r1, ref _tangent, ref _rt1);
			Vector2.Dot(ref _r2, ref _tangent, ref _rt2);
			_kTangent = GeometryA.body.inverseMass + GeometryB.body.inverseMass;
			Vector2.Dot(ref _r1, ref _r1, ref _float1);
			Vector2.Dot(ref _r2, ref _r2, ref _float2);
			_kTangent += GeometryA.body.inverseMomentOfInertia * (_float1 - _rt1 * _rt1) + GeometryB.body.InverseMomentOfInertia * (_float2 - _rt2 * _rt2);
			value.massTangent = 1f / _kTangent;
			_min = Math.Min(0f, _physicsSimulator.AllowedPenetration + value.Separation);
			value.normalVelocityBias = (0f - _physicsSimulator.BiasFactor) * inverseDt * _min;
			_restitution = (GeometryA.RestitutionCoefficient + GeometryB.RestitutionCoefficient) * 0.5f;
			GeometryA.body.GetVelocityAtWorldOffset(ref _r1, out _vec1);
			GeometryB.body.GetVelocityAtWorldOffset(ref _r2, out _vec2);
			Vector2.Subtract(ref _vec2, ref _vec1, ref _dv);
			Vector2.Dot(ref _dv, ref value.Normal, ref _vn);
			value.bounceVelocity = _vn * _restitution;
			Vector2.Multiply(ref value.Normal, value.normalImpulse, ref _vec1);
			Vector2.Multiply(ref _tangent, value.tangentImpulse, ref _vec2);
			Vector2.Add(ref _vec1, ref _vec2, ref _impulse);
			GeometryB.body.ApplyImpulseAtWorldOffset(ref _impulse, ref _r2);
			Vector2.Multiply(ref _impulse, -1f, ref _impulse);
			GeometryA.body.ApplyImpulseAtWorldOffset(ref _impulse, ref _r1);
			value.normalImpulseBias = 0f;
			ContactList[i] = value;
		}
	}

	internal void ApplyImpulse()
	{
		for (i = 0; i < ContactList.Count; i++)
		{
			_contact = ContactList[i];
			_contact.r1.X = _contact.Position.X - GeometryA.body.position.X;
			_contact.r1.Y = _contact.Position.Y - GeometryA.body.position.Y;
			_contact.r2.X = _contact.Position.X - GeometryB.body.position.X;
			_contact.r2.Y = _contact.Position.Y - GeometryB.body.position.Y;
			GeometryA.body.GetVelocityAtWorldOffset(ref _contact.r1, out _vec1);
			GeometryB.body.GetVelocityAtWorldOffset(ref _contact.r2, out _vec2);
			_dv.X = _vec2.X - _vec1.X;
			_dv.Y = _vec2.Y - _vec1.Y;
			_vn = _dv.X * _contact.Normal.X + _dv.Y * _contact.Normal.Y;
			_normalImpulse = _contact.massNormal * (0f - (_vn + _contact.bounceVelocity));
			_oldNormalImpulse = _contact.normalImpulse;
			_contact.normalImpulse = Math.Max(_oldNormalImpulse + _normalImpulse, 0f);
			_normalImpulse = _contact.normalImpulse - _oldNormalImpulse;
			_impulse.X = _contact.Normal.X * _normalImpulse;
			_impulse.Y = _contact.Normal.Y * _normalImpulse;
			GeometryB.body.ApplyImpulseAtWorldOffset(ref _impulse, ref _contact.r2);
			_impulse.X *= -1f;
			_impulse.Y *= -1f;
			GeometryA.body.ApplyImpulseAtWorldOffset(ref _impulse, ref _contact.r1);
			GeometryA.body.GetVelocityBiasAtWorldOffset(ref _contact.r1, out _vec1);
			GeometryB.body.GetVelocityBiasAtWorldOffset(ref _contact.r2, out _vec2);
			_dv.X = _vec2.X - _vec1.X;
			_dv.Y = _vec2.Y - _vec1.Y;
			_normalVelocityBias = _dv.X * _contact.Normal.X + _dv.Y * _contact.Normal.Y;
			_normalImpulseBias = _contact.massNormal * (0f - _normalVelocityBias + _contact.normalVelocityBias);
			_normalImpulseBiasOriginal = _contact.normalImpulseBias;
			_contact.normalImpulseBias = Math.Max(_normalImpulseBiasOriginal + _normalImpulseBias, 0f);
			_normalImpulseBias = _contact.normalImpulseBias - _normalImpulseBiasOriginal;
			_impulseBias.X = _contact.Normal.X * _normalImpulseBias;
			_impulseBias.Y = _contact.Normal.Y * _normalImpulseBias;
			GeometryB.body.ApplyBiasImpulseAtWorldOffset(ref _impulseBias, ref _contact.r2);
			_impulseBias.X *= -1f;
			_impulseBias.Y *= -1f;
			GeometryA.body.ApplyBiasImpulseAtWorldOffset(ref _impulseBias, ref _contact.r1);
			GeometryA.body.GetVelocityAtWorldOffset(ref _contact.r1, out _vec1);
			GeometryB.body.GetVelocityAtWorldOffset(ref _contact.r2, out _vec2);
			_dv.X = _vec2.X - _vec1.X;
			_dv.Y = _vec2.Y - _vec1.Y;
			_maxTangentImpulse = _frictionCoefficientCombined * _contact.normalImpulse;
			_float1 = 1f;
			_tangent.X = _float1 * _contact.Normal.Y;
			_tangent.Y = (0f - _float1) * _contact.Normal.X;
			_vt = _dv.X * _tangent.X + _dv.Y * _tangent.Y;
			_tangentImpulse = _contact.massTangent * (0f - _vt);
			_oldTangentImpulse = _contact.tangentImpulse;
			_contact.tangentImpulse = Calculator.Clamp(_oldTangentImpulse + _tangentImpulse, 0f - _maxTangentImpulse, _maxTangentImpulse);
			_tangentImpulse = _contact.tangentImpulse - _oldTangentImpulse;
			_impulse.X = _tangent.X * _tangentImpulse;
			_impulse.Y = _tangent.Y * _tangentImpulse;
			GeometryB.body.ApplyImpulseAtWorldOffset(ref _impulse, ref _contact.r2);
			_impulse.X *= -1f;
			_impulse.Y *= -1f;
			GeometryA.body.ApplyImpulseAtWorldOffset(ref _impulse, ref _contact.r1);
			ContactList[i] = _contact;
		}
	}

	internal void Reset()
	{
		GeometryA = null;
		GeometryB = null;
		ContactList.Clear();
		_newContactList.Clear();
		_mergedContactList.Clear();
	}

	internal void Collide()
	{
		_newContactList.Clear();
		if (PhysicsSimulator.NarrowPhaseCollider == NarrowPhaseCollider.DistanceGrid)
		{
			DistanceGrid.Instance.Collide(GeometryA, GeometryB, _newContactList);
		}
		else if (PhysicsSimulator.NarrowPhaseCollider == NarrowPhaseCollider.SAT)
		{
			SAT.Instance.Collide(GeometryA, GeometryB, _newContactList);
		}
		contactCount = _newContactList.Count;
		for (i = 1; i < contactCount; i++)
		{
			indexContact = _newContactList[i];
			j = i;
			while (j > 0 && _newContactList[j - 1].Separation > indexContact.Separation)
			{
				_newContactList[j] = _newContactList[j - 1];
				j--;
			}
			_newContactList[j] = indexContact;
		}
		if (contactCount > _physicsSimulator.MaxContactsToResolve)
		{
			_newContactList.RemoveRange(_physicsSimulator.MaxContactsToResolve, contactCount - _physicsSimulator.MaxContactsToResolve);
		}
		if (GeometryA.OnCollision != null && _newContactList.Count > 0 && !GeometryA.OnCollision(GeometryA, GeometryB, _newContactList))
		{
			_newContactList.Clear();
		}
		if (GeometryB.OnCollision != null && _newContactList.Count > 0 && !GeometryB.OnCollision(GeometryB, GeometryA, _newContactList))
		{
			_newContactList.Clear();
		}
		_mergedContactList.Clear();
		for (i = 0; i < _newContactList.Count; i++)
		{
			int num = ContactList.IndexOfSafe(_newContactList[i]);
			if (num > -1)
			{
				Contact item = _newContactList[i];
				item.normalImpulse = ContactList[num].normalImpulse;
				item.tangentImpulse = ContactList[num].tangentImpulse;
				_mergedContactList.Add(item);
			}
			else
			{
				_mergedContactList.Add(_newContactList[i]);
			}
		}
		ContactList.Clear();
		for (i = 0; i < _mergedContactList.Count; i++)
		{
			ContactList.Add(_mergedContactList[i]);
		}
	}

	internal bool ContainsInvalidGeom()
	{
		if (!GeometryA.IsDisposed && !GeometryB.IsDisposed && (!GeometryA.body.isStatic || !GeometryB.body.isStatic) && GeometryA.body.Enabled && GeometryB.body.Enabled && (GeometryA.CollisionGroup != GeometryB.CollisionGroup || GeometryA.CollisionGroup == 0 || GeometryB.CollisionGroup == 0))
		{
			return ((GeometryA.CollisionCategories & GeometryB.CollidesWith) == 0) & ((GeometryB.CollisionCategories & GeometryA.CollidesWith) == 0);
		}
		return true;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Arbiter))
		{
			return false;
		}
		return Equals((Arbiter)obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public static bool operator ==(Arbiter arbiter1, Arbiter arbiter2)
	{
		return arbiter1.Equals(arbiter2);
	}

	public static bool operator !=(Arbiter arbiter1, Arbiter arbiter2)
	{
		return !arbiter1.Equals(arbiter2);
	}

	public bool Equals(Arbiter other)
	{
		if (GeometryA == other.GeometryA)
		{
			return GeometryB == other.GeometryB;
		}
		return false;
	}
}
