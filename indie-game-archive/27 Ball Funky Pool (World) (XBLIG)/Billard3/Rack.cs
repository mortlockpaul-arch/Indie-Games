using System;
using Maximinus;
using Microsoft.Xna.Framework;

namespace Billard3;

public static class Rack
{
	private const float OFFSET_BETWEEN_BALLS = 1.005f;

	private static Vector2 pos_OFFSET_XM_ZP = new Vector2((float)Math.Sqrt(3.0) * 0.833333f * -1f, 0.833333f) * 1.005f;

	private static Vector2 pos_OFFSET_XM_ZM = new Vector2(pos_OFFSET_XM_ZP.X, 0f - pos_OFFSET_XM_ZP.Y);

	private static float FirstBreak_Offset_Y = 0.020833327f;

	private static float FirstBreak_Offset_X = 0.020833327f;

	private static Vector2 pos_PLEINE_1 = new Vector2(-15f, 0f);

	private static Vector2 pos_PLEINE_2 = pos_PLEINE_1 + pos_OFFSET_XM_ZP;

	private static Vector2 pos_PLEINE_3 = pos_PLEINE_1 + 3f * pos_OFFSET_XM_ZP;

	private static Vector2 pos_PLEINE_4 = pos_PLEINE_3 + pos_OFFSET_XM_ZM;

	private static Vector2 pos_PLEINE_6 = pos_PLEINE_1 + 2f * pos_OFFSET_XM_ZM;

	private static Vector2 pos_PLEINE_5 = pos_PLEINE_6 + pos_OFFSET_XM_ZP;

	private static Vector2 pos_PLEINE_7 = pos_PLEINE_6 + 2f * pos_OFFSET_XM_ZM;

	private static Vector2 pos_RAYEE_1 = pos_PLEINE_1 + pos_OFFSET_XM_ZM;

	private static Vector2 pos_RAYEE_2 = pos_PLEINE_1 + 2f * pos_OFFSET_XM_ZP;

	private static Vector2 pos_RAYEE_3 = pos_PLEINE_1 + 4f * pos_OFFSET_XM_ZP;

	private static Vector2 pos_RAYEE_4 = pos_RAYEE_2 + pos_OFFSET_XM_ZM;

	private static Vector2 pos_RAYEE_5 = pos_RAYEE_2 + 2f * pos_OFFSET_XM_ZM;

	private static Vector2 pos_RAYEE_7 = pos_RAYEE_1 + 2f * pos_OFFSET_XM_ZM;

	private static Vector2 pos_RAYEE_6 = pos_RAYEE_7 + pos_OFFSET_XM_ZP;

	private static Vector2 pos_8BALL = pos_RAYEE_1 + pos_OFFSET_XM_ZP;

	private static Vector2 pos_0 = new Vector2(15f, 0f);

	private static Vector2[] InitialPositions8BallAnd9Ball = new Vector2[16]
	{
		pos_0,
		pos_PLEINE_1,
		pos_PLEINE_2 + Vector2.UnitY * FirstBreak_Offset_Y + Vector2.UnitX * FirstBreak_Offset_X,
		(MaximinusGame.Id == MaximinusGame.ID.Billard9Ball) ? pos_PLEINE_6 : pos_PLEINE_3,
		(MaximinusGame.Id == MaximinusGame.ID.Billard9Ball) ? pos_RAYEE_4 : pos_PLEINE_4,
		pos_PLEINE_5,
		(MaximinusGame.Id == MaximinusGame.ID.Billard9Ball) ? pos_RAYEE_2 : pos_PLEINE_6,
		((MaximinusGame.Id == MaximinusGame.ID.Billard9Ball) ? pos_RAYEE_5 : pos_PLEINE_7) - Vector2.UnitX * FirstBreak_Offset_X,
		(MaximinusGame.Id == MaximinusGame.ID.Billard9Ball) ? (pos_RAYEE_1 - Vector2.UnitY * FirstBreak_Offset_Y + Vector2.UnitX * FirstBreak_Offset_X) : pos_8BALL,
		(MaximinusGame.Id == MaximinusGame.ID.Billard9Ball) ? pos_8BALL : (pos_RAYEE_1 - Vector2.UnitY * FirstBreak_Offset_Y + Vector2.UnitX * FirstBreak_Offset_X),
		pos_RAYEE_2,
		pos_RAYEE_3 - Vector2.UnitX * FirstBreak_Offset_X,
		pos_RAYEE_4,
		pos_RAYEE_5,
		pos_RAYEE_6,
		pos_RAYEE_7
	};

	private static Vector2[] InitialPositions9Ball = new Vector2[10]
	{
		pos_0,
		pos_PLEINE_1,
		pos_PLEINE_2 + Vector2.UnitY * FirstBreak_Offset_Y + Vector2.UnitX * FirstBreak_Offset_X,
		pos_PLEINE_6,
		pos_RAYEE_4,
		pos_PLEINE_5,
		pos_RAYEE_2,
		pos_RAYEE_5 - Vector2.UnitX * FirstBreak_Offset_X,
		pos_RAYEE_1 - Vector2.UnitY * FirstBreak_Offset_Y + Vector2.UnitX * FirstBreak_Offset_X,
		pos_8BALL
	};

	private static Vector2[] InitialPositionsFunky;

	private static Vector2[] FunkyZones = new Vector2[3]
	{
		Vector2.UnitY * 30f / 2f,
		Vector2.UnitY * 30f / -2f,
		Vector2.UnitY * 30f / 2f + Vector2.UnitX * 30f
	};

	public static Vector2[] InitialPositions
	{
		get
		{
			if (MaximinusGame.Id != MaximinusGame.ID.FunkyPool)
			{
				return InitialPositions8BallAnd9Ball;
			}
			return InitialPositionsFunky;
		}
	}

	public static void InitializeRack()
	{
		InitialPositionsFunky = new Vector2[28];
		ref Vector2 reference = ref InitialPositionsFunky[0];
		reference = Vector2.UnitX * 8f + Vector2.UnitY * -6f;
		int num = 1;
		for (int i = 0; i < 3; i++)
		{
			for (int j = 1; j <= 9; j++)
			{
				ref Vector2 reference2 = ref InitialPositionsFunky[num++];
				reference2 = InitialPositions9Ball[j] + FunkyZones[i];
			}
		}
		RepositionWBall.DefaultPos = InitialPositions[0];
		RepositionWBall.DefaultPosV3 = new Vector3(RepositionWBall.DefaultPos.X, 0.833333f, RepositionWBall.DefaultPos.Y);
	}
}
