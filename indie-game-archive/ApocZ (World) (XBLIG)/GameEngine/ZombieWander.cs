using DataContent;
using EGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Net;

namespace GameEngine;

public class ZombieWander(AIBotStates e) : AIStateMachine(e)
{
	public static float ClosestDisSqr = float.MaxValue;

	public static Cue MoanSoundCue;

	public static string[] MoanSoundNames = new string[5] { "zed_moan_00", "zed_moan_01", "zed_moan_02", "zed_moan_03", "zed_moan_04" };

	private Vector3 hitNormal = Vector3.Zero;

	private Vector3 tmpDirection = Vector3.Zero;

	protected override void Enter(BaseData e)
	{
		base.Enter(e);
		e.UpdateTimer = (float)BaseData.RandGenerator.NextDouble() * 2.5f;
		e.WaitNoSightTimer = 3f;
		e.IdleTimer = (float)BaseData.RandGenerator.NextDouble() * 12f;
		e.WanderTimer = 0f;
		e.State = AIBotStates.ZombieWander;
		e.UpdateNetworkTimeStep = 3.5f;
		e.PlayAnimation(WeaponAnim.ZombieWalk, randStart: true);
	}

	protected override void Update(int qIndex)
	{
		if (baseDataSet.CurrentAnimation != (WeaponAnim)baseDataSet.BotHordeRef.bAnimation)
		{
			baseDataSet.UpdateAnimation();
		}
		if (baseDataSet.BotHordeRef.bState == 18 && baseDataSet.BotHordeRef.CurrentFrameLoopCount > 0)
		{
			if (ZombieLODEntry.HuntPlayerEnabled)
			{
				baseDataSet.BotHordeRef.EnterState(AIBotStates.ZombieAttackPlayer);
			}
			else
			{
				baseDataSet.BotHordeRef.EnterState(AIBotStates.ZombieWander);
			}
		}
		baseDataSet.Position = baseDataSet.BotHordeRef.pos;
		baseDataSet.Direction = baseDataSet.BotHordeRef.dir;
		baseDataSet.MoveDirection = baseDataSet.Direction;
	}

	private void GoIntoIdle()
	{
		baseDataSet.NavMeshStart = baseDataSet.NavMeshCount;
		baseDataSet.IdleTimer = 6f + (float)BaseData.RandGenerator.NextDouble() * 12f;
		baseDataSet.WanderTimer = baseDataSet.IdleTimer + 12f + (float)BaseData.RandGenerator.NextDouble() * 24f;
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
		packetWriter.Write((byte)97);
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
