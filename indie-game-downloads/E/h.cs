using System;
using System.Runtime.CompilerServices;
using System.Threading;
using _000E;
using _0014;
using Microsoft.Xna.Framework;
using N;
using T;
using W;
using Y;
using Z;
using d;
using m;
using n;
using q;
using r;
using s;
using y;

namespace E;

internal class h : Y.h, W.b, Z.b, m.b, m.h, W._6, r.h, T.h, q.h
{
	internal Vector3 a5h;

	internal Quaternion a5b = Quaternion.Identity;

	internal N._7 a56 = N._7.Identity;

	internal Vector3 a5a;

	internal Vector3 a57;

	internal Vector3 a5_0006;

	internal Vector3 a5v;

	internal bool a5B;

	private bool a5X = true;

	internal N._7 a5_0018;

	internal N._7 a5W;

	internal N._7 a5_0002;

	internal N._7 a5_000E;

	internal float a5y;

	internal float a5r;

	internal float a5_0001;

	private Action<h> a5_000F;

	protected s.b collisionInformation;

	protected internal d.v locker = new d.v();

	internal T._6 a5Z;

	private Action<T._6> a5u;

	internal Z._0006 a5L;

	private Action<_000E.h> a5z;

	private W._0006 a5Y;

	private r.a a5m;

	private m._7 a5T = _0014._6.DefaultPositionUpdateMode;

	private float a5q;

	private float a5E;

	private float a5_0013 = 0.15f;

	private float a5x = 0.03f;

	[CompilerGenerated]
	private n._6 a5Q;

	[CompilerGenerated]
	private bool a5_0012;

	[CompilerGenerated]
	private m._6 a5g;

	[CompilerGenerated]
	private object a5P;

	public Vector3 Position
	{
		get
		{
			return a5h;
		}
		set
		{
			a5h = value;
			a5L.Activate();
		}
	}

	public Quaternion Orientation
	{
		get
		{
			return a5b;
		}
		set
		{
			Quaternion.Normalize(ref value, out a5b);
			N._7.CreateFromQuaternion(ref a5b, out a56);
			N._7.MultiplyTransposed(ref a56, ref a5_000E, out var result);
			N._7.Multiply(ref result, ref a56, out a5_0018);
			N._7.MultiplyTransposed(ref a56, ref a5_0002, out result);
			N._7.Multiply(ref result, ref a56, out a5W);
			a5L.Activate();
		}
	}

	public N._7 OrientationMatrix
	{
		get
		{
			return a56;
		}
		set
		{
			N._7.CreateQuaternion(ref value, out a5b);
			Orientation = a5b;
		}
	}

	public Matrix WorldTransform
	{
		get
		{
			N._7.ToMatrix4X4(ref a56, out var result);
			result.Translation = a5h;
			return result;
		}
		set
		{
			Quaternion.CreateFromRotationMatrix(ref value, out a5b);
			Orientation = a5b;
			a5h = value.Translation;
			a5L.Activate();
		}
	}

	public Vector3 AngularVelocity
	{
		get
		{
			return a5_0006;
		}
		set
		{
			a5_0006 = value;
			N._7.Transform(ref value, ref a5W, out a5v);
			a5L.Activate();
		}
	}

	public Vector3 AngularMomentum
	{
		get
		{
			if (_0014._6.ConserveAngularMomentum)
			{
				return a5v;
			}
			N._7.Transform(ref a5_0006, ref a5W, out var result);
			return result;
		}
		set
		{
			a5v = value;
			N._7.Transform(ref value, ref a5_0018, out a5_0006);
			a5L.Activate();
		}
	}

	public Vector3 LinearVelocity
	{
		get
		{
			return a5a;
		}
		set
		{
			a5a = value;
			Vector3.Multiply(ref a5a, a5y, out a57);
			a5L.Activate();
		}
	}

	public Vector3 LinearMomentum
	{
		get
		{
			return a57;
		}
		set
		{
			a57 = value;
			Vector3.Multiply(ref a57, a5r, out a5a);
			a5L.Activate();
		}
	}

	public n.B MotionState
	{
		get
		{
			n.B result = default(n.B);
			result.Position = a5h;
			result.Orientation = a5b;
			result.LinearVelocity = a5a;
			result.AngularVelocity = a5_0006;
			return result;
		}
		set
		{
			Position = value.Position;
			Orientation = value.Orientation;
			LinearVelocity = value.LinearVelocity;
			AngularVelocity = value.AngularVelocity;
		}
	}

	public bool IsDynamic => a5B;

	public bool IsAffectedByGravity
	{
		get
		{
			return a5X;
		}
		set
		{
			a5X = value;
		}
	}

	public n._6 BufferedStates
	{
		[CompilerGenerated]
		get
		{
			return a5Q;
		}
		[CompilerGenerated]
		private set
		{
			a5Q = obj;
		}
	}

	public N._7 InertiaTensorInverse => a5_0018;

	public N._7 InertiaTensor => a5W;

	public N._7 LocalInertiaTensor
	{
		get
		{
			return a5_0002;
		}
		set
		{
			a5_0002 = value;
			N._7._6_0015(ref a5_0002, out a5_000E);
			N._7.MultiplyTransposed(ref a56, ref a5_000E, out var result);
			N._7.Multiply(ref result, ref a56, out a5_0018);
			N._7.MultiplyTransposed(ref a56, ref a5_0002, out result);
			N._7.Multiply(ref result, ref a56, out a5W);
		}
	}

	public N._7 LocalInertiaTensorInverse
	{
		get
		{
			return a5_000E;
		}
		set
		{
			a5_000E = value;
			N._7._6_0015(ref a5_000E, out a5_0002);
			N._7.MultiplyTransposed(ref a56, ref a5_000E, out var result);
			N._7.Multiply(ref result, ref a56, out a5_0018);
			N._7.MultiplyTransposed(ref a56, ref a5_0002, out result);
			N._7.Multiply(ref result, ref a56, out a5W);
		}
	}

	public float Mass
	{
		get
		{
			return a5y;
		}
		set
		{
			if (value <= 0f || float.IsNaN(value) || float.IsInfinity(value))
			{
				BecomeKinematic();
			}
			else if (a5B)
			{
				N._7.Multiply(ref a5_0002, value * a5r, out var result);
				BecomeDynamic(value, result);
			}
			else
			{
				BecomeDynamic(value);
			}
		}
	}

	public float InverseMass
	{
		get
		{
			return a5r;
		}
		set
		{
			if (value > 0f)
			{
				Mass = 1f / value;
			}
			else
			{
				Mass = 0f;
			}
		}
	}

	public float Volume
	{
		get
		{
			return a5_0001;
		}
		set
		{
			a5_0001 = value;
		}
	}

	public s.b CollisionInformation
	{
		get
		{
			return collisionInformation;
		}
		protected set
		{
			if (collisionInformation != null)
			{
				collisionInformation.Shape.ShapeChanged -= a5z;
			}
			collisionInformation = value;
			if (collisionInformation != null)
			{
				collisionInformation.Shape.ShapeChanged += a5z;
			}
		}
	}

	public d.v Locker => locker;

	public T._6 Material
	{
		get
		{
			return a5Z;
		}
		set
		{
			if (a5Z != null)
			{
				a5Z.MaterialChanged -= a5u;
			}
			a5Z = value;
			if (a5Z != null)
			{
				a5Z.MaterialChanged += a5u;
			}
			bn(a5Z);
		}
	}

	public a SolverUpdateables => new a(a5L.a57);

	public _6 Constraints => new _6(a5L.a57);

	W.h W.b.EventCreator => CollisionInformation.Events;

	public Z._0006 ActivityInformation => a5L;

	bool W._6.IsActive => a5L.IsActive;

	bool m.h.IsActive => a5L.IsActive;

	public bool IgnoreShapeChanges
	{
		[CompilerGenerated]
		get
		{
			return a5_0012;
		}
		[CompilerGenerated]
		set
		{
			a5_0012 = value;
		}
	}

	W._0006 W._6.ForceUpdater
	{
		get
		{
			return a5Y;
		}
		set
		{
			a5Y = value;
		}
	}

	r.a r.h.Space
	{
		get
		{
			return a5m;
		}
		set
		{
			a5m = value;
		}
	}

	public r.a Space => a5m;

	m._6 m.h.PositionUpdater
	{
		[CompilerGenerated]
		get
		{
			return a5g;
		}
		[CompilerGenerated]
		set
		{
			a5g = value;
		}
	}

	public m._7 PositionUpdateMode
	{
		get
		{
			return a5T;
		}
		set
		{
			m._7 obj = a5T;
			a5T = value;
			if (a5T != obj && ((m.h)this).PositionUpdater != null && ((m.h)this).PositionUpdater is m.a)
			{
				(((m.h)this).PositionUpdater as m.a).UpdateableModeChanged(this, obj);
			}
		}
	}

	public float AngularDamping
	{
		get
		{
			return a5_0013;
		}
		set
		{
			a5_0013 = MathHelper.Clamp(value, 0f, 1f);
		}
	}

	public float LinearDamping
	{
		get
		{
			return a5x;
		}
		set
		{
			a5x = value;
		}
	}

	public object Tag
	{
		[CompilerGenerated]
		get
		{
			return a5P;
		}
		[CompilerGenerated]
		set
		{
			a5P = value;
		}
	}

	q._7 q.h.CollisionRules
	{
		get
		{
			return ((Y.a)collisionInformation).a56;
		}
		set
		{
			collisionInformation.CollisionRules = value;
		}
	}

	Y.a Y.h.Entry => collisionInformation;

	public event Action<h> PositionUpdated
	{
		add
		{
			Action<h> action = a5_000F;
			Action<h> action2;
			do
			{
				action2 = action;
				Action<h> value2 = (Action<h>)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange(ref a5_000F, value2, action2);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<h> action = a5_000F;
			Action<h> action2;
			do
			{
				action2 = action;
				Action<h> value2 = (Action<h>)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange(ref a5_000F, value2, action2);
			}
			while ((object)action != action2);
		}
	}

	private void bn(T._6 P_0)
	{
		for (int i = 0; i < collisionInformation.a56.Count; i++)
		{
			collisionInformation.a56[i].UpdateMaterialProperties();
		}
	}

	protected h()
	{
		BufferedStates = new n._6(this);
		a5Z = new T._6();
		a5u = bn;
		a5Z.MaterialChanged += a5u;
		a5z = OnShapeChanged;
		a5L = new Z._0006(this);
	}

	public h(s.b collisionInformation)
		: this()
	{
		Initialize(collisionInformation);
	}

	public h(s.b collisionInformation, float mass)
		: this()
	{
		Initialize(collisionInformation, mass);
	}

	public h(s.b collisionInformation, float mass, N._7 inertiaTensor)
		: this()
	{
		Initialize(collisionInformation, mass, inertiaTensor);
	}

	public h(s.b collisionInformation, float mass, N._7 inertiaTensor, float volume)
		: this()
	{
		Initialize(collisionInformation, mass, inertiaTensor, volume);
	}

	public h(_000E.b shape)
		: this()
	{
		Initialize(shape.GetCollidableInstance());
	}

	public h(_000E.b shape, float mass)
		: this()
	{
		Initialize(shape.GetCollidableInstance(), mass);
	}

	public h(_000E.b shape, float mass, N._7 inertiaTensor)
		: this()
	{
		Initialize(shape.GetCollidableInstance(), mass, inertiaTensor);
	}

	public h(_000E.b shape, float mass, N._7 inertiaTensor, float volume)
		: this()
	{
		Initialize(shape.GetCollidableInstance(), mass, inertiaTensor, volume);
	}

	protected internal void Initialize(s.b collisionInformation)
	{
		CollisionInformation = collisionInformation;
		BecomeKinematic();
		collisionInformation.Entity = this;
	}

	protected internal void Initialize(s.b collisionInformation, float mass)
	{
		CollisionInformation = collisionInformation;
		collisionInformation.Shape.ComputeDistributionInformation(out var shapeInfo);
		N._7.Multiply(ref shapeInfo.VolumeDistribution, mass * y.a.InertiaTensorScale, out shapeInfo.VolumeDistribution);
		a5_0001 = shapeInfo.Volume;
		BecomeDynamic(mass, shapeInfo.VolumeDistribution);
		collisionInformation.Entity = this;
	}

	protected internal void Initialize(s.b collisionInformation, float mass, N._7 inertiaTensor)
	{
		CollisionInformation = collisionInformation;
		a5_0001 = collisionInformation.Shape.ComputeVolume();
		BecomeDynamic(mass, inertiaTensor);
		collisionInformation.Entity = this;
	}

	protected internal void Initialize(s.b collisionInformation, float mass, N._7 inertiaTensor, float volume)
	{
		CollisionInformation = collisionInformation;
		a5_0001 = volume;
		BecomeDynamic(mass, inertiaTensor);
		collisionInformation.Entity = this;
	}

	public void ApplyImpulse(Vector3 location, Vector3 impulse)
	{
		ApplyImpulse(ref location, ref impulse);
	}

	public void ApplyImpulse(ref Vector3 location, ref Vector3 impulse)
	{
		if (a5B)
		{
			ApplyLinearImpulse(ref impulse);
			Vector3 vector = new Vector3
			{
				X = location.X - a5h.X,
				Y = location.Y - a5h.Y,
				Z = location.Z - a5h.Z
			};
			Vector3.Cross(ref vector, ref impulse, out var result);
			ApplyAngularImpulse(ref result);
			a5L.Activate();
		}
	}

	public void ApplyLinearImpulse(ref Vector3 impulse)
	{
		a57.X += impulse.X;
		a57.Y += impulse.Y;
		a57.Z += impulse.Z;
		a5a.X = a57.X * a5r;
		a5a.Y = a57.Y * a5r;
		a5a.Z = a57.Z * a5r;
	}

	public void ApplyAngularImpulse(ref Vector3 impulse)
	{
		a5v.X += impulse.X;
		a5v.Y += impulse.Y;
		a5v.Z += impulse.Z;
		if (_0014._6.ConserveAngularMomentum)
		{
			a5_0006.X = a5v.X * a5_0018.M11 + a5v.Y * a5_0018.M21 + a5v.Z * a5_0018.M31;
			a5_0006.Y = a5v.X * a5_0018.M12 + a5v.Y * a5_0018.M22 + a5v.Z * a5_0018.M32;
			a5_0006.Z = a5v.X * a5_0018.M13 + a5v.Y * a5_0018.M23 + a5v.Z * a5_0018.M33;
		}
		else
		{
			a5_0006.X += impulse.X * a5_0018.M11 + impulse.Y * a5_0018.M21 + impulse.Z * a5_0018.M31;
			a5_0006.Y += impulse.X * a5_0018.M12 + impulse.Y * a5_0018.M22 + impulse.Z * a5_0018.M32;
			a5_0006.Z += impulse.X * a5_0018.M13 + impulse.Y * a5_0018.M23 + impulse.Z * a5_0018.M33;
		}
	}

	protected void OnShapeChanged(_000E.h shape)
	{
		if (!IgnoreShapeChanges)
		{
			collisionInformation.Shape.ComputeDistributionInformation(out var shapeInfo);
			a5_0001 = shapeInfo.Volume;
			if (a5B)
			{
				N._7.Multiply(ref shapeInfo.VolumeDistribution, y.a.InertiaTensorScale * a5y, out shapeInfo.VolumeDistribution);
				LocalInertiaTensor = shapeInfo.VolumeDistribution;
			}
			else
			{
				LocalInertiaTensorInverse = default(N._7);
			}
		}
	}

	public void BecomeKinematic()
	{
		bool flag = a5B;
		a5B = false;
		LocalInertiaTensorInverse = default(N._7);
		a5y = float.MaxValue;
		a5r = 0f;
		if (flag)
		{
			if (a5L.DeactivationManager != null)
			{
				a5L.DeactivationManager.RemoveSimulationIslandFromMember(a5L);
			}
			if (((W._6)this).ForceUpdater != null)
			{
				((W._6)this).ForceUpdater.ForceUpdateableBecomingKinematic(this);
			}
		}
		if (collisionInformation.CollisionRules.Group == q._7.DefaultDynamicCollisionGroup || collisionInformation.CollisionRules.Group == null)
		{
			collisionInformation.CollisionRules.Group = q._7.DefaultKinematicCollisionGroup;
		}
		a5L.Activate();
		LinearVelocity = a5a;
		AngularVelocity = a5_0006;
	}

	public void BecomeDynamic(float mass)
	{
		N._7 matrix = collisionInformation.Shape.ComputeVolumeDistribution();
		N._7.Multiply(ref matrix, mass * y.a.InertiaTensorScale, out matrix);
		BecomeDynamic(mass, matrix);
	}

	public void BecomeDynamic(float mass, N._7 localInertiaTensor)
	{
		if (mass <= 0f || float.IsInfinity(mass) || float.IsNaN(mass))
		{
			throw new InvalidOperationException("Cannot use a mass of " + mass + " for a dynamic entity.  Consider using a kinematic entity instead.");
		}
		bool flag = a5B;
		a5B = true;
		LocalInertiaTensor = localInertiaTensor;
		a5y = mass;
		a5r = 1f / mass;
		if (!flag)
		{
			if (a5L.DeactivationManager != null)
			{
				a5L.DeactivationManager.AddSimulationIslandToMember(a5L);
			}
			if (((W._6)this).ForceUpdater != null)
			{
				((W._6)this).ForceUpdater.ForceUpdateableBecomingDynamic(this);
			}
		}
		if (collisionInformation.CollisionRules.Group == q._7.DefaultKinematicCollisionGroup || collisionInformation.CollisionRules.Group == null)
		{
			collisionInformation.CollisionRules.Group = q._7.DefaultDynamicCollisionGroup;
		}
		a5L.Activate();
		LinearVelocity = a5a;
		AngularVelocity = a5_0006;
	}

	void W._6.UpdateForForces(float dt)
	{
		if (IsAffectedByGravity)
		{
			Vector3.Add(ref a5Y.a5b, ref a5a, out a5a);
		}
		if (a5L.DeactivationManager.a57 && a5L.a5X && (a5L.a5a || a5L.a56 > a5L.DeactivationManager.a5a))
		{
			float num = a5a.LengthSquared() + a5_0006.LengthSquared();
			if (num < a5L.DeactivationManager.a56)
			{
				float damping = 1f - num / (2f * a5L.DeactivationManager.a56);
				ModifyAngularDamping(damping);
				ModifyLinearDamping(damping);
			}
		}
		float num2 = LinearDamping + a5q;
		if (num2 > 0f)
		{
			Vector3.Multiply(ref a5a, (float)Math.Pow(MathHelper.Clamp(1f - num2, 0f, 1f), dt), out a5a);
		}
		float num3 = AngularDamping + a5E;
		if (num3 > 0f && _0014._6.ConserveAngularMomentum)
		{
			Vector3.Multiply(ref a5v, (float)Math.Pow(MathHelper.Clamp(1f - num3, 0f, 1f), dt), out a5v);
		}
		else if (num3 > 0f)
		{
			Vector3.Multiply(ref a5_0006, (float)Math.Pow(MathHelper.Clamp(1f - num3, 0f, 1f), dt), out a5_0006);
		}
		a5q = 0f;
		a5E = 0f;
		Vector3.Multiply(ref a5a, a5y, out a57);
		N._7.MultiplyTransposed(ref a56, ref a5_000E, out var result);
		N._7.Multiply(ref result, ref a56, out a5_0018);
		N._7.MultiplyTransposed(ref a56, ref a5_0002, out result);
		N._7.Multiply(ref result, ref a56, out a5W);
		if (_0014._6.ConserveAngularMomentum)
		{
			N._7.Transform(ref a5v, ref a5_0018, out a5_0006);
		}
		else
		{
			N._7.Transform(ref a5_0006, ref a5W, out a5v);
		}
	}

	void r.h.OnAdditionToSpace(r.a newSpace)
	{
		OnAdditionToSpace(newSpace);
	}

	protected virtual void OnAdditionToSpace(r.a newSpace)
	{
	}

	void r.h.OnRemovalFromSpace(r.a oldSpace)
	{
		OnRemovalFromSpace(oldSpace);
	}

	protected virtual void OnRemovalFromSpace(r.a oldSpace)
	{
	}

	void m.b.UpdateTimeOfImpacts(float dt)
	{
		for (int i = 0; i < collisionInformation.a56.a5h; i++)
		{
			if (_0014._6.UseCCDForNoSolverPairs || collisionInformation.a56.Elements[i].a5h.a56 < q.a.NoSolver)
			{
				collisionInformation.a56.Elements[i].UpdateTimeOfImpact(collisionInformation, dt);
			}
		}
	}

	void m.b.UpdatePositionContinuously(float dt)
	{
		float num = 1f;
		for (int i = 0; i < collisionInformation.a56.Count; i++)
		{
			if (collisionInformation.a56.Elements[i].timeOfImpact < num)
			{
				num = collisionInformation.a56.Elements[i].timeOfImpact;
			}
		}
		Vector3.Multiply(ref a5a, dt * num, out var result);
		Vector3.Add(ref a5h, ref result, out a5h);
		collisionInformation.UpdateWorldTransform(ref a5h, ref a5b);
		if (a5_000F != null)
		{
			a5_000F(this);
		}
	}

	void m.h.PreUpdatePosition(float dt)
	{
		Vector3 result;
		if (_0014._6.UseRk4AngularIntegration && a5B)
		{
			r.X.a_0012(ref a5b, ref a5_000E, ref a5v, dt, out a5b);
		}
		else
		{
			Vector3.Multiply(ref a5_0006, dt * 0.5f, out result);
			Quaternion quaternion = new Quaternion(result.X, result.Y, result.Z, 0f);
			Quaternion.Multiply(ref quaternion, ref a5b, out quaternion);
			Quaternion.Add(ref a5b, ref quaternion, out a5b);
			a5b.Normalize();
		}
		N._7.CreateFromQuaternion(ref a5b, out a56);
		if (PositionUpdateMode == m._7.Discrete)
		{
			Vector3.Multiply(ref a5a, dt, out result);
			Vector3.Add(ref a5h, ref result, out a5h);
			collisionInformation.UpdateWorldTransform(ref a5h, ref a5b);
			if (a5_000F != null)
			{
				a5_000F(this);
			}
		}
		collisionInformation.UpdateWorldTransform(ref a5h, ref a5b);
	}

	public void ModifyLinearDamping(float damping)
	{
		float num = LinearDamping + a5q;
		float num2 = 1f - num;
		a5q += damping * num2;
	}

	public void ModifyAngularDamping(float damping)
	{
		float num = AngularDamping + a5E;
		float num2 = 1f - num;
		a5E += damping * num2;
	}

	public override string ToString()
	{
		if (Tag == null)
		{
			return base.ToString();
		}
		return base.ToString() + ", " + Tag;
	}
}
