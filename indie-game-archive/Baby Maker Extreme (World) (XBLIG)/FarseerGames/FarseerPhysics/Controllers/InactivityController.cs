using FarseerGames.FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Controllers;

public class InactivityController : Controller
{
	public float ActivationDistance;

	private int _bodiesEnabled;

	private Vector2 _difference;

	public float MaxIdleTime;

	private PhysicsSimulator _physicsSimulator;

	public int BodiesEnabled => _bodiesEnabled;

	public InactivityController(PhysicsSimulator physicsSimulator)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		ActivationDistance = 100f;
		_difference = Vector2.Zero;
		MaxIdleTime = 1000f;
		base._002Ector();
		_physicsSimulator = physicsSimulator;
	}

	public override void Validate()
	{
	}

	public override void Update(float dt, float dtReal)
	{
		if (base.IsDisposed)
		{
			return;
		}
		float num = dt * 1000f;
		_bodiesEnabled = 0;
		foreach (Body body in _physicsSimulator.BodyList)
		{
			if (body.IsStatic || !body.Enabled)
			{
				continue;
			}
			_bodiesEnabled++;
			if (!body.Moves && body.IsAutoIdle)
			{
				body.IdleTime += num;
				if (body.IdleTime >= MaxIdleTime)
				{
					body.Enabled = false;
				}
				continue;
			}
			body.IdleTime = 0f;
			foreach (Body body2 in _physicsSimulator.BodyList)
			{
				if (!body2.Enabled && body2.IsAutoIdle && IsInActivationDistance(body, body2))
				{
					body2.Enabled = true;
					body2.IdleTime = 0f;
				}
			}
		}
	}

	public bool IsInActivationDistance(Body b1, Body b2)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		_difference = b1.position - b2.position;
		float num = ((Vector2)(ref _difference)).Length();
		if (num < 0f)
		{
			_difference *= -1f;
		}
		return num <= ActivationDistance;
	}
}
