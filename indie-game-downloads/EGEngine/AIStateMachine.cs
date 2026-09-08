using System;
using DataContent;
using Microsoft.Xna.Framework;

namespace EGEngine;

public class AIStateMachine
{
	public static AIStateMachine[] allStates = new AIStateMachine[22];

	public static HitIndicatorEntry[] HitIndicatorArray = new HitIndicatorEntry[16];

	public static int NumberBotsHuntingPlayer = 0;

	public static int MaxBotsHuntingPlayer = 0;

	public static float IsBotAttackingTimer = 0f;

	public static bool IsBotAttackingPlayer = false;

	public static Random RandGenerator = new Random();

	private static bool AttackPlayerEnabled = false;

	private static bool AttackPlayerCanceled = false;

	protected IntersectSegmentParams segParams;

	protected BaseData baseDataSet;

	protected AIBotStates stateType;

	public static void AddAttackIndicator(ref Vector3 fromPos)
	{
		for (int i = 0; i < 16; i++)
		{
			if (HitIndicatorArray[i].AlphaTimer <= 0f)
			{
				Vector3 vector = fromPos - LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition;
				vector.Y = 0f;
				vector.Normalize();
				Vector3 vector2 = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].CameraDirection * 100f;
				vector2.Y = 0f;
				vector2.Normalize();
				Vector3 vector3 = Vector3.Zero;
				vector3.X = 0f;
				vector3.Y = 1f;
				vector3.Z = 0f;
				Vector3.Cross(ref vector2, ref vector3, out vector3);
				float result = 0f;
				Vector3.Dot(ref vector2, ref vector, out result);
				result = (float)Math.Acos(result);
				if (Vector3.Dot(vector3, vector) < 0f)
				{
					result *= -1f;
				}
				HitIndicatorArray[i].DirectionAngle = result;
				HitIndicatorArray[i].AlphaTimer = 1f;
				break;
			}
		}
	}

	public static void CancelAttackPlayer(bool e)
	{
		AttackPlayerCanceled = e;
		if (e)
		{
			AttackPlayerEnabled = false;
		}
	}

	public static void SetAttackPlayerEnable(bool e)
	{
		if (!AttackPlayerCanceled && (!e || !AIBase.WaveCoolingDown))
		{
			AttackPlayerEnabled = e;
		}
	}

	public static bool GetAttackPlayerEnable()
	{
		return AttackPlayerEnabled;
	}

	public AIStateMachine(AIBotStates e)
	{
		stateType = e;
	}

	public void SetInternalReference(BaseData e)
	{
		baseDataSet = e;
	}

	public void CurrentState(BaseData e, int qIndex)
	{
		baseDataSet = e;
		switch (baseDataSet.InternalState)
		{
		case InternalStates.Enter:
			Enter(baseDataSet);
			break;
		case InternalStates.Update:
			Update(qIndex);
			break;
		case InternalStates.Exit:
			Exit();
			break;
		}
	}

	public void DrawState(int qIndex, BaseData e)
	{
		baseDataSet = e;
		Draw(qIndex);
	}

	public void ExitState(AIStateMachine s)
	{
		Exit();
		s.Enter(baseDataSet);
	}

	protected virtual void Enter(BaseData e)
	{
		e.InternalState = InternalStates.Update;
		e.BotState = this;
	}

	protected virtual void Update(int qIndex)
	{
		baseDataSet.UpdateBase(0.01667f, qIndex);
	}

	protected virtual void Exit()
	{
		baseDataSet.InternalState = InternalStates.InValid;
	}

	protected virtual void Draw(int qIndex)
	{
	}

	public virtual void ClientUpdateBot(BaseData e, ePacketTypes pType, float px, float pz, float dx, float dz, byte animation)
	{
	}
}
