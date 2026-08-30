using System;
using Maximinus;
using Microsoft.Xna.Framework;

namespace Billard3;

public class GameState
{
	public enum Type
	{
		MENUS,
		AIMING,
		CHOOSING_POWER,
		WATCHING_MOVE,
		REPOSITION_WBALL,
		GAME_OVER,
		LOBBY,
		CHEAT_PROMPT,
		TRANSITION
	}

	private class WatchingMoveCam
	{
		private const float CoeffClose = 1.2f;

		private const float CoeffFar = 1.1f;

		private static float position_CamMoving_dimY = 16f;

		private static Vector3 position_CamMoving_045_135 = new Vector3(0f, position_CamMoving_dimY + 12f, -60f) * 1.1f;

		private static Vector3 position_CamMoving_225_315 = new Vector3(0f, position_CamMoving_dimY + 12f, 60f) * 1.1f;

		private static Vector3 position_CamMoving_135_225 = new Vector3(55f, position_CamMoving_dimY + 10f, 0f) * 1.2f;

		private static Vector3 position_CamMoving_315_045 = new Vector3(-55f, position_CamMoving_dimY + 10f, 0f) * 1.2f;

		public static Vector3 Choose
		{
			get
			{
				Vector2 aimVector2D = Aiming.AimVector2D;
				if (Math.Abs(aimVector2D.X) > Math.Abs(aimVector2D.Y))
				{
					if (aimVector2D.X > 0f)
					{
						return position_CamMoving_315_045;
					}
					return position_CamMoving_135_225;
				}
				if (aimVector2D.Y > 0f)
				{
					return position_CamMoving_045_135;
				}
				return position_CamMoving_225_315;
			}
		}
	}

	private static Type current;

	private static Type statePendingAfterTransition;

	private static bool repoWballAvailable;

	public static Type Current => current;

	public static bool CpuAiming
	{
		get
		{
			if (Current == Type.AIMING)
			{
				return GameModeRules.CurrentPlayer == GameModeRules.IndexCPU;
			}
			return false;
		}
	}

	public static bool AllowMultiplayerInput
	{
		get
		{
			switch (current)
			{
			case Type.AIMING:
			case Type.CHOOSING_POWER:
			case Type.WATCHING_MOVE:
			case Type.REPOSITION_WBALL:
			case Type.TRANSITION:
				return true;
			default:
				return false;
			}
		}
	}

	public static bool InMenu
	{
		get
		{
			if (current != Type.MENUS && current != Type.LOBBY)
			{
				return current == Type.CHEAT_PROMPT;
			}
			return true;
		}
	}

	public static bool CameraMenu
	{
		get
		{
			switch (current)
			{
			case Type.MENUS:
			case Type.GAME_OVER:
			case Type.LOBBY:
			case Type.CHEAT_PROMPT:
				return true;
			default:
				return false;
			}
		}
	}

	private static bool DisableGameplayInput
	{
		get
		{
			if (!Statics.cam.Transitioning)
			{
				return Statics.cam.TransitioningMenu;
			}
			return true;
		}
	}

	public static bool DisableActivate
	{
		get
		{
			if (!DisableGameplayInput && !Statics.menus.DisableGameplayInput && !Statics.cam.TransitioningALT)
			{
				return Statics.menus.popupControls.state != Menus.Screen.State.Hidden;
			}
			return true;
		}
	}

	public static bool DisableInputRepoWball => Statics.cam.TransitioningALT;

	public static bool SpecialMenuInput
	{
		get
		{
			if (!Statics.menus.Paused && current != Type.LOBBY)
			{
				return current == Type.CHEAT_PROMPT;
			}
			return true;
		}
	}

	public static bool AnyBallDying
	{
		get
		{
			foreach (Ball ball in Statics.balls)
			{
				if (ball.state == Ball.State.DYING)
				{
					return true;
				}
			}
			return false;
		}
	}

	public static bool RepoWballAvailable => repoWballAvailable;

	public static void Initialize(GameTime gameTime)
	{
		ChangeWithTransition(Type.AIMING, gameTime);
	}

	public static void Change(Type newType, GameTime gameTime)
	{
		Type type = current;
		current = newType;
		Ball ball = Statics.balls[0];
		switch (current)
		{
		case Type.CHEAT_PROMPT:
			Statics.cheatPrompt.Enable((type == Type.LOBBY) ? GameModeRules.Type.MultiPlayer : GameModeRules.Type.SinglePlayer, gameTime);
			break;
		case Type.AIMING:
			if (GameModeRules.CurrentPlayer == GameModeRules.IndexCPU)
			{
				Bot.StartMyTurn(gameTime);
			}
			if (!ball.Alive)
			{
				throw new Exception("bad, should have been detected in Updates.Update()");
			}
			Cue.Update_Aiming();
			ball.updateChangementDeDirection(ball.Pos.Value2D, new VectorBillard(Aiming.AimVector).Value2D);
			LigneVisee.Compute();
			break;
		case Type.CHOOSING_POWER:
			ChoosePower.Start(gameTime.TotalGameTime.TotalSeconds);
			break;
		case Type.WATCHING_MOVE:
		{
			Force force = new Force();
			force.Name = "shoot force";
			force.Kind = ForceKind.Thrust;
			force.Position = ball.Pos.Value;
			force.Vector = Aiming.AimVector * 1.85f * ((MaximinusGame.Id == MaximinusGame.ID.FunkyPool) ? 1.5f : 1f) * ((GameModeRules.CurrentPlayer != GameModeRules.IndexCPU) ? ChoosePower.Ratio : Bot.PowerRatio);
			ball.applyMiscForce(force);
			Audio.PlaySFX(Audio.SFXID.BallCue);
			break;
		}
		case Type.REPOSITION_WBALL:
			ball.Reset(isAlive: true, RepositionWBall.DefaultPos);
			UpdateRepoWball(ball.Pos.Value2D);
			if (GameModeRules.CurrentPlayer == GameModeRules.IndexCPU)
			{
				Bot.ComputeRepositionWball(gameTime);
			}
			break;
		case Type.TRANSITION:
			throw new Exception("should have called ChangeWithTransition()");
		case Type.LOBBY:
			Statics.lobby.Enable(gameTime);
			break;
		case Type.GAME_OVER:
			break;
		}
	}

	public static void ChangeWithTransition(Type newType, GameTime gameTime)
	{
		statePendingAfterTransition = newType;
		current = Type.TRANSITION;
		Ball ball = Statics.balls[0];
		Vector3 outPos = Vector3.Zero;
		Vector3 outLookAt = Vector3.Zero;
		switch (newType)
		{
		case Type.WATCHING_MOVE:
			outPos = WatchingMoveCam.Choose;
			outLookAt = Vector3.UnitY * 0.833333f;
			break;
		case Type.AIMING:
			Aiming.Initialize();
			Statics.cam.ComputePosLookAt(ball.Pos.Value, Aiming.AimVector, out outPos, out outLookAt);
			break;
		case Type.REPOSITION_WBALL:
			Statics.cam.ComputePosLookAt(RepositionWBall.DefaultPosV3, RepositionWBall.LookAtDirection, out outPos, out outLookAt);
			break;
		default:
			throw new Exception("not supported");
		}
		Statics.cam.InitiateTransitionNormal(gameTime, outPos, outLookAt);
	}

	public static void EndTransition(GameTime gameTime)
	{
		Change(statePendingAfterTransition, gameTime);
	}

	public static bool IsTransitioningTo(Type newType)
	{
		float ratioTrans;
		return IsTransitioningTo(newType, out ratioTrans);
	}

	public static bool IsTransitioningTo(Type newType, out float ratioTrans)
	{
		ratioTrans = Statics.cam.TransitionRatio;
		if (current == Type.TRANSITION)
		{
			return statePendingAfterTransition == newType;
		}
		return false;
	}

	public static bool WasPosAvailableNextFrame(int ballNum, Vector2 pos, out int collidedIndex, out float distance)
	{
		collidedIndex = -1;
		distance = -1f;
		foreach (Ball ball in Statics.balls)
		{
			if (ball.Alive && ball.Number != ballNum && Updates.CollisionList.AlreadyRepositioned(ball.Number) && AreThesePositionsTooClose(pos, ball.Pos.Value2D, out distance))
			{
				collidedIndex = ball.Number;
				return false;
			}
		}
		return true;
	}

	public static void UpdateRepoWball(Vector2 pos)
	{
		repoWballAvailable = IsPositionAvailable(0, pos);
	}

	public static bool IsRepoWballValid(Vector2 pos)
	{
		if (MaximinusGame.Id != MaximinusGame.ID.FunkyPool)
		{
			if (pos.X + 0.833333f > 29.791666f || ((MaximinusGame.Id == MaximinusGame.ID.Billard9Ball && GameModeRules.BreakDone) ? (pos.X - 0.833333f < -29.791666f) : (pos.X < 15f)))
			{
				return false;
			}
		}
		else if (pos.X + 0.833333f > 29.791666f || pos.X - 0.833333f < -29.791666f)
		{
			return false;
		}
		if (pos.Y + 0.833333f > 29.791666f || pos.Y - 0.833333f < -29.791666f)
		{
			return false;
		}
		return true;
	}

	public static bool IsPositionAvailable(int ballNum, Vector2 pos)
	{
		int ballTooCloseId;
		float distance;
		return IsPositionAvailable(ballNum, pos, out ballTooCloseId, out distance);
	}

	public static bool IsPositionAvailable(int ballNum, Vector2 pos, out int ballTooCloseId, out float distance)
	{
		ballTooCloseId = -1;
		distance = -1f;
		bool flag = true;
		foreach (Ball ball in Statics.balls)
		{
			if (flag && ball.Alive && ball.Number != ballNum)
			{
				flag = !AreThesePositionsTooClose(pos, ball.Pos.Value2D, out distance);
				if (!flag)
				{
					ballTooCloseId = ball.Number;
				}
			}
		}
		return flag;
	}

	public static bool AnyBallOverlap_HavingRepositionedUpTo(int repositionedUpToIndex, out int i0, out int i1, out float d)
	{
		foreach (Ball ball3 in Statics.balls)
		{
			foreach (Ball ball4 in Statics.balls)
			{
				if (ball3.Alive && ball4.Alive && ball3.Number <= repositionedUpToIndex && ball4.Number <= repositionedUpToIndex && ball3.Number < ball4.Number && AreThesePositionsTooClose(ball3.Pos.Value2D, ball4.Pos.Value2D, out var distance))
				{
					i0 = ball3.Number;
					i1 = ball4.Number;
					d = distance;
					return true;
				}
			}
		}
		i0 = -1;
		i1 = -1;
		d = -1f;
		return false;
	}

	public static bool CheckIntegrity_Next(out int i0, out int i1, out float d)
	{
		foreach (Ball ball3 in Statics.balls)
		{
			foreach (Ball ball4 in Statics.balls)
			{
				if (ball3.Alive && ball4.Alive && (Updates.CollisionList.AlreadyRepositioned(ball3.Number) || ball3.AppliedFullVelo) && (Updates.CollisionList.AlreadyRepositioned(ball4.Number) || ball4.AppliedFullVelo) && ball3.Number < ball4.Number && AreThesePositionsTooClose(ball3.Pos.Value2D, ball4.Pos.Value2D, out d))
				{
					i0 = ball3.Number;
					i1 = ball4.Number;
					return true;
				}
			}
		}
		i0 = -1;
		i1 = -1;
		d = -1f;
		return false;
	}

	public static bool AreThesePositionsTooClose(Vector2 p0, Vector2 p1)
	{
		float distance;
		return AreThesePositionsTooClose(p0, p1, out distance);
	}

	public static bool IsPositionAvailable_NextFrame(Ball b0, out int collidedIndex)
	{
		collidedIndex = -1;
		bool flag = true;
		Vector2 p = b0.Pos.Value2D + b0.Velo.Value2D;
		foreach (Ball ball in Statics.balls)
		{
			if (ball.Alive && Updates.CollisionList.AlreadyRepositioned(ball.Number))
			{
				flag &= !AreThesePositionsTooClose(p, ball.Pos.Value2D);
				if (!flag)
				{
					collidedIndex = ball.Number;
				}
			}
		}
		return flag;
	}

	public static bool AreThesePositionsTooClose(Vector2 p0, Vector2 p1, out float distance)
	{
		distance = Vector2.Distance(p0, p1);
		return distance < 1.665666f;
	}

	public static bool IsPositionValid(Vector2 pos)
	{
		foreach (Trou listTrou in CollisionBande.listTrous)
		{
			float num = Vector2.Distance(pos, listTrou.pos);
			if (num < listTrou.rayon - 0.001f)
			{
				return false;
			}
		}
		foreach (BandeObject listBande in CollisionBande.listBandes)
		{
			foreach (Vector2 point in listBande.points)
			{
				float num2 = Vector2.Distance(point, pos);
				if (num2 < 0.832333f)
				{
					return false;
				}
			}
		}
		if (Math.Abs(pos.X) < 29.167667f || Math.Abs(pos.Y) > 28.27f)
		{
			if (!(Math.Abs(pos.Y) < 29.167667f))
			{
				if (!(Math.Abs(pos.X) < 1.24f))
				{
					return Math.Abs(pos.X) > 28.27f;
				}
				return true;
			}
			return true;
		}
		return false;
	}
}
