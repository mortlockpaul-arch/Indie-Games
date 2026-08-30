using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Joints;

public abstract class Joint
{
	internal JointEdge EdgeA;

	internal JointEdge EdgeB;

	protected float InvIA;

	protected float InvIB;

	protected float InvMassA;

	protected float InvMassB;

	internal bool IslandFlag;

	protected Vector2 LocalCenterA;

	protected Vector2 LocalCenterB;

	public JointType JointType { get; set; }

	public Body BodyA { get; set; }

	public Body BodyB { get; set; }

	public abstract Vector2 WorldAnchorA { get; }

	public abstract Vector2 WorldAnchorB { get; }

	public object UserData { get; set; }

	public bool Active
	{
		get
		{
			if (BodyA.Active)
			{
				return BodyB.Active;
			}
			return false;
		}
	}

	public bool CollideConnected { get; set; }

	protected Joint(Body body, Body bodyB)
	{
		BodyA = body;
		EdgeA = new JointEdge();
		BodyB = bodyB;
		EdgeB = new JointEdge();
		CollideConnected = false;
	}

	protected Joint(Body body)
	{
		BodyA = body;
		CollideConnected = false;
		EdgeA = new JointEdge();
	}

	public abstract Vector2 GetReactionForce(float inv_dt);

	public abstract float GetReactionTorque(float inv_dt);

	protected void WakeBodies()
	{
		BodyA.Awake = true;
		if (BodyB != null)
		{
			BodyB.Awake = true;
		}
	}

	public bool IsFixedType()
	{
		if (JointType != JointType.FixedRevolute && JointType != JointType.FixedDistance && JointType != JointType.FixedPrismatic && JointType != JointType.FixedLine && JointType != JointType.FixedMouse)
		{
			return JointType == JointType.FixedAngle;
		}
		return true;
	}

	internal abstract void InitVelocityConstraints(ref TimeStep step);

	internal abstract void SolveVelocityConstraints(ref TimeStep step);

	internal abstract bool SolvePositionConstraints();
}
