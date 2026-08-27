using DataContent;
using EGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GameEngine;

public class ZombieHuntPlayer(AIBotStates e) : AIStateMachine(e)
{
	public static int NumInThisState = 0;

	public static float ClosestDisSqr = float.MaxValue;

	public static Cue MoanSoundCue;

	public static string[] MoanSoundNames = new string[5] { "zed_moan_00", "zed_moan_01", "zed_moan_02", "zed_moan_03", "zed_moan_04" };

	private Vector3 hitNormal = Vector3.Zero;

	private Vector3 tmpDirection = Vector3.Zero;

	protected override void Enter(BaseData e)
	{
		base.Enter(e);
		e.WaitNoSightTimer = 3f;
		e.IdleTimer = (float)BaseData.RandGenerator.NextDouble() * 12f;
		e.WanderTimer = 0f;
		e.NavMeshStart = 0;
		e.NavMeshCount = 0;
		e.State = AIBotStates.ZombieHuntPlayer;
		e.UpdateNetworkTimeStep = 0.96250004f;
		if (e.TargetPlayer != null)
		{
			e.TargetPosition = e.TargetPlayer.vecPosition;
			e.TargetLastPosition = e.TargetPlayer.vecPosition;
		}
		e.PlayAnimation(WeaponAnim.ZombieRun, randStart: true);
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
		baseDataSet.Position = baseDataSet.BotHordeRef.pos;
		baseDataSet.Direction = baseDataSet.BotHordeRef.dir;
		baseDataSet.MoveDirection = baseDataSet.Direction;
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
			e.PlayAnimation((WeaponAnim)animation, randStart: true);
		}
	}
}
