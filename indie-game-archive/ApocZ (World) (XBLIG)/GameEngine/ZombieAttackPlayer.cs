using DataContent;
using EGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Net;

namespace GameEngine;

public class ZombieAttackPlayer(AIBotStates e) : AIStateMachine(e)
{
	public static int NumInThisState = 0;

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
		e.State = AIBotStates.ZombieAttackPlayer;
		e.UpdateNetworkTimeStep = 0.7f;
		if (e.TargetPlayer != null)
		{
			e.TargetPosition = e.TargetPlayer.vecPosition;
			e.TargetLastPosition = e.TargetPlayer.vecPosition;
		}
		if (EndGameEngine.randGenerator.Next(0, 100) > 50)
		{
			e.PlayAnimation(WeaponAnim.ZombieAttack1, randStart: false);
		}
		else
		{
			e.PlayAnimation(WeaponAnim.ZombieAttack0, randStart: false);
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
		if (baseDataSet.TargetPlayer == null)
		{
			baseDataSet.BotState.ExitState(AIStateMachine.allStates[14]);
			return;
		}
		if (EGENetWorkNext.networkSession.IsHost)
		{
			if (baseDataSet.UpdateNetworkTimerTripped)
			{
				byte quant = 0;
				uint offset = 0u;
				HeightMapPhysics.GetQuantizedPosition(ref baseDataSet.Position, ref offset, ref quant);
				byte value = (byte)((baseDataSet.Direction.X + 1f) * 127f);
				byte value2 = (byte)((baseDataSet.Direction.Z + 1f) * 127f);
				PacketWriter packetWriter = EGENetWorkNext.packetWriter;
				packetWriter.Write((byte)120);
				packetWriter.Write(((ZombieBot)baseDataSet)._uid);
				packetWriter.Write(offset);
				packetWriter.Write(quant);
				packetWriter.Write(value);
				packetWriter.Write(value2);
				packetWriter.Write((byte)baseDataSet.CurrentAnimation);
			}
			float num2 = (baseDataSet.TargetPlayer.vecPosition - baseDataSet.Position).LengthSquared();
			if (num2 > 160000f && ZombieHuntPlayer.NumInThisState < 6)
			{
				baseDataSet.BotState.ExitState(AIStateMachine.allStates[15]);
			}
			baseDataSet.InflictDamageTimer += fFIXED_TIME_STEP;
			if (baseDataSet.InflictDamageTimer > 2f)
			{
				baseDataSet.InflictDamageTimer -= 2f;
				byte b = ((EndGameEngine.randGenerator.Next(0, 100) < 20) ? ((byte)1) : ((byte)0));
				if (baseDataSet.TargetPlayer.NetGamerRef.IsLocal)
				{
					baseDataSet.TargetPlayer.Health -= DamageToPlayer;
					baseDataSet.TargetPlayer.BloodLoss = ((baseDataSet.TargetPlayer.BloodLoss > (float)(int)b) ? baseDataSet.TargetPlayer.BloodLoss : ((float)(int)b));
				}
				else
				{
					PacketWriter packetWriter2 = EGENetWorkNext.packetWriter;
					packetWriter2.Write((byte)103);
					packetWriter2.Write(baseDataSet.TargetPlayer.NetGamerRef.Id);
					packetWriter2.Write((byte)DamageToPlayer);
					packetWriter2.Write(b);
				}
			}
		}
		else
		{
			_ = baseDataSet.AnimPlayer.CurrentAnimation;
			_ = 100;
			if (baseDataSet.CurrentAnimation == WeaponAnim.ZombieIdle)
			{
				baseDataSet.Speed = 0f;
				float amount = 0.1f;
				baseDataSet.Direction = Vector3.Lerp(baseDataSet.Direction * 100f, baseDataSet.MoveDirection * 100f, amount);
				baseDataSet.Direction.Normalize();
				baseDataSet.Position.X = MathHelper.Lerp(baseDataSet.Position.X, baseDataSet.MovePosition.X, 0.1f);
				baseDataSet.Position.Z = MathHelper.Lerp(baseDataSet.Position.Z, baseDataSet.MovePosition.Z, 0.1f);
			}
		}
		if (!EGENetWorkNext.networkSession.IsHost)
		{
			return;
		}
		for (int i = 0; i < baseDataSet.nNeighbors; i++)
		{
			if (baseDataSet.NeighborDisSqrList[i] < 4096f)
			{
				float num3 = 1f - baseDataSet.NeighborDisSqrList[i] / 4096f;
				baseDataSet.Position -= baseDataSet.NeighborVectorList[i] * num3 * 0.5f;
			}
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
