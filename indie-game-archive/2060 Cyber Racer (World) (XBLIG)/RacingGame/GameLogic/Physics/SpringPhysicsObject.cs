namespace RacingGame.GameLogic.Physics;

public class SpringPhysicsObject
{
	public const float DefaultMass = 0.5f;

	public const float DefaultFriction = 0.9f;

	public const float DefaultSpringConstant = 1f;

	private float mass = 0.5f;

	private float friction = 0.9f;

	private float springConstant = 1f;

	public float pos;

	public float velocity;

	public float force;

	public SpringPhysicsObject()
	{
	}

	public SpringPhysicsObject(float setMass, float setFriction, float setSpringConstant, float setInitialPos)
	{
		mass = setMass;
		friction = setFriction;
		springConstant = setSpringConstant;
		pos = setInitialPos;
		force = 0f;
		velocity = 0f;
	}

	public void Simulate(float timeChange)
	{
		force += (0f - pos) * springConstant;
		velocity = force / mass;
		pos += timeChange * velocity;
		force *= 1f - timeChange * friction;
	}

	public void ChangePos(float change)
	{
		pos += change;
	}
}
