using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.GameScreens;
using RacingGame.Graphics;
using RacingGame.Sounds;

namespace RacingGame.GameLogic;

public class Player : ChaseCamera
{
	private const int LapCount = 3;

	private const float InAirTimeoutMilliseconds = 3000f;

	private List<float> lapTimes;

	private float inAirTimeMilliseconds;

	public void AddLapTime(float setLapTime)
	{
		lapTimes.Add(setLapTime);
	}

	public Player(Vector3 setCarPosition)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		lapTimes = new List<float>();
		base._002Ector(setCarPosition);
	}

	public override void Reset()
	{
		base.Reset();
		lapTimes.Clear();
	}

	public override void Update()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		if (RacingGameManager.InGame && base.ZoomInTime <= 0f && Input.IsGamePadConnected)
		{
			if (isGameOver)
			{
				cameraPos = base.CarPosition + new Vector3(0f, -5f, 20f) + Vector3.TransformNormal(new Vector3(30f, 0f, 0f), Matrix.CreateRotationZ(BaseGame.TotalTimeMilliseconds / 2593f));
				BaseGame.ViewMatrix = Matrix.CreateLookAt(cameraPos, base.CarPosition, base.CarUpVector);
				int rankFromCurrentTime = Highscores.GetRankFromCurrentTime(levelNum, (int)base.BestTimeMilliseconds);
				currentGameTimeMilliseconds = base.BestTimeMilliseconds;
				if (victory)
				{
					TextureFont.WriteTextCentered(BaseGame.Width / 2, BaseGame.Height / 7, "Victory! You won.", Color.LightGreen, 1.25f);
				}
				else
				{
					TextureFont.WriteTextCentered(BaseGame.Width / 2, BaseGame.Height / 7, "Game Over! You lost.", Color.Red, 1.25f);
				}
				for (int i = 0; i < lapTimes.Count; i++)
				{
					TextureFont.WriteTextCentered(BaseGame.Width / 2, BaseGame.Height / 7 + BaseGame.YToRes(35) * (1 + i), "Lap " + (i + 1) + " Time: " + ((int)lapTimes[i] / 60).ToString("00") + ":" + ((int)lapTimes[i] % 60).ToString("00") + "." + ((int)(lapTimes[i] * 100f) % 100).ToString("00"), Color.White, 1.25f);
				}
				TextureFont.WriteTextCentered(BaseGame.Width / 2, BaseGame.Height / 7 + BaseGame.YToRes(35) * (1 + lapTimes.Count), "Rank: " + (1 + rankFromCurrentTime), Color.White, 1.25f);
				return;
			}
			if (!isCarOnGround)
			{
				inAirTimeMilliseconds += BaseGame.ElapsedTimeThisFrameInMilliseconds;
			}
			else
			{
				inAirTimeMilliseconds = 0f;
			}
			float num = Vector3.Distance(base.CarPosition, groundPlanePos);
			if (num > 20f || inAirTimeMilliseconds > 3000f)
			{
				ClearVariablesForGameOver();
				isGameOver = true;
				victory = false;
				Sound.Play(Sound.Sounds.CarLose);
				Sound.StopGearSound();
			}
			if (base.CurrentLap >= 3)
			{
				ClearVariablesForGameOver();
				lap--;
				isGameOver = true;
				victory = true;
				Sound.Play(Sound.Sounds.Victory);
				Sound.StopGearSound();
			}
		}
		base.Update();
	}
}
