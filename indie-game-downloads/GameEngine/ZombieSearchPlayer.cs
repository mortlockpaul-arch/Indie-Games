using System.IO;
using DataContent;
using EGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Net;

namespace GameEngine;

public class ZombieSearchPlayer : AIStateMachine
{
	public static float ClosestDisSqr = float.MaxValue;

	public static Cue MoanSoundCue;

	public static string[] MoanSoundNames = new string[5] { "zed_moan_00", "zed_moan_01", "zed_moan_02", "zed_moan_03", "zed_moan_04" };

	private Vector3 hitNormal;

	private Vector3 tmpDirection;

	public ZombieSearchPlayer(AIBotStates e)
		: base(e)
	{
		hitNormal = Vector3.Zero;
		tmpDirection = Vector3.Zero;
	}

	protected override void Enter(BaseData e)
	{
		base.Enter(e);
		e.WaitNoSightTimer = 16f + (float)BaseData.RandGenerator.NextDouble() * 16f;
		e.IdleTimer = 6f;
		e.WanderTimer = -1f;
		e.NavMeshStart = 0;
		e.NavMeshCount = 0;
		e.State = AIBotStates.ZombieSearchPlayer;
		if (e.TargetPlayer != null)
		{
			e.TargetPosition = e.TargetPlayer.vecPosition;
			e.TargetLastPosition = e.TargetPlayer.vecPosition;
		}
		e.PlayAnimation(WeaponAnim.ZombieIdle, randStart: true);
		byte quant = 0;
		uint offset = 0u;
		HeightMapPhysics.GetQuantizedPosition(ref e.Position, ref offset, ref quant);
		byte value = (byte)((e.Direction.X + 1f) * 127f);
		byte value2 = (byte)((e.Direction.Z + 1f) * 127f);
		PacketWriter packetWriter = EGENetWorkNext.packetWriter;
		((BinaryWriter)packetWriter).Write((byte)120);
		((BinaryWriter)packetWriter).Write(e._uid);
		((BinaryWriter)packetWriter).Write(offset);
		((BinaryWriter)packetWriter).Write(quant);
		((BinaryWriter)packetWriter).Write(value);
		((BinaryWriter)packetWriter).Write(value2);
		((BinaryWriter)packetWriter).Write((byte)e.CurrentAnimation);
	}

	protected override void Update(int qIndex)
	{
		float fFIXED_TIME_STEP = EndGameEngine.fFIXED_TIME_STEP;
		baseDataSet.IdleTimer -= fFIXED_TIME_STEP;
		baseDataSet.WanderTimer -= fFIXED_TIME_STEP;
		baseDataSet.WaitNoSightTimer -= fFIXED_TIME_STEP;
		baseDataSet.UpdateMoveDistanceTimer += fFIXED_TIME_STEP;
		baseDataSet.Speed = 0f;
		baseDataSet.UpdateTimer += fFIXED_TIME_STEP;
		baseDataSet.UpdateTimerTripped = false;
		float num = 1.5f;
		if (baseDataSet.UpdateTimer >= num)
		{
			baseDataSet.UpdateTimer -= num;
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
		_ = e.MovePosition;
		_ = e.MovePosition;
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
