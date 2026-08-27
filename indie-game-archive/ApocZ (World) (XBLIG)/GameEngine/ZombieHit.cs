using DataContent;
using EGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GameEngine;

public class ZombieHit(AIBotStates e) : AIStateMachine(e)
{
	public static int DamageToPlayer = 4;

	public static float ClosestDisSqr = float.MaxValue;

	public static Cue MoanSoundCue;

	public static string[] MoanSoundNames = new string[5] { "zed_moan_00", "zed_moan_01", "zed_moan_02", "zed_moan_03", "zed_moan_04" };

	private bool testGetPAth;

	private Vector3 hitNormal = Vector3.Zero;

	private Vector3 tmpDirection = Vector3.Zero;

	private BoundingSphere collisionSphere = default(BoundingSphere);

	protected override void Enter(BaseData e)
	{
		base.Enter(e);
		e.WaitNoSightTimer = 3f;
		e.IdleTimer = (float)BaseData.RandGenerator.NextDouble() * 12f;
		e.WanderTimer = 0f;
		e.NavMeshStart = 0;
		e.NavMeshCount = 0;
		e.State = AIBotStates.ZombieHit;
		e.UpdateNetworkTimeStep = 0.7f;
		if (e.TargetPlayer != null)
		{
			e.TargetPosition = e.TargetPlayer.vecPosition;
			e.TargetLastPosition = e.TargetPlayer.vecPosition;
		}
		if (EndGameEngine.randGenerator.Next(0, 100) > 50)
		{
			e.PlayAnimation(WeaponAnim.ZombieHit00, randStart: false);
		}
		else
		{
			e.PlayAnimation(WeaponAnim.ZombieHit01, randStart: false);
		}
	}

	protected override void Update(int qIndex)
	{
		float fFIXED_TIME_STEP = EndGameEngine.fFIXED_TIME_STEP;
		baseDataSet.IdleTimer -= fFIXED_TIME_STEP;
		baseDataSet.WanderTimer -= fFIXED_TIME_STEP;
		baseDataSet.UpdateMoveDistanceTimer += fFIXED_TIME_STEP;
		baseDataSet.Speed = 0f;
		baseDataSet.UpdateTimer += fFIXED_TIME_STEP;
		baseDataSet.UpdateTimerTripped = false;
		float num = 1.5f;
		if (baseDataSet.UpdateTimer >= num)
		{
			baseDataSet.UpdateTimer -= 1.5f;
			baseDataSet.UpdateTimerTripped = true;
		}
		if (baseDataSet.TargetPlayer != null)
		{
			baseDataSet.BotState.ExitState(AIStateMachine.allStates[17]);
		}
		else
		{
			baseDataSet.BotState.ExitState(AIStateMachine.allStates[14]);
		}
	}

	public override void ClientUpdateBot(BaseData e, ePacketTypes pType, float px, float pz, float dx, float dz, byte animation)
	{
		if (e.CurrentPathingData == null)
		{
			return;
		}
		_ = e.MovePosition.X;
		_ = e.MovePosition.Z;
		if (pType == ePacketTypes.ZombieUpdatePosition)
		{
			dx = dx * (1f / 127f) - 1f;
			dz = dz * (1f / 127f) - 1f;
			e.MoveDirection.X = dx;
			e.MoveDirection.Z = dz;
			e.MovePosition.X = px;
			e.MovePosition.Z = pz;
			float num = (e.Position - e.MovePosition).LengthSquared();
			if (num > 10000f)
			{
				e.Position.X = e.MovePosition.X;
				e.Position.Z = e.MovePosition.Z;
			}
		}
		else
		{
			e.NavMeshRoute[0].X = px;
			e.NavMeshRoute[0].Z = pz;
		}
		if ((WeaponAnim)animation != e.CurrentAnimation)
		{
			e.PlayAnimation((WeaponAnim)animation, randStart: false);
		}
	}
}
