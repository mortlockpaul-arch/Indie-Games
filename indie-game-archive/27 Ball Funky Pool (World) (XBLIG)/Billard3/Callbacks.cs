using Maximinus;
using Microsoft.Xna.Framework;

namespace Billard3;

public class Callbacks
{
	private MaximinusGame game;

	public double DeferredEnableMenusTime = -1.0;

	public bool GameIsActive => game.IsActive;

	public Callbacks(MaximinusGame game)
	{
		this.game = game;
	}

	public void Exit()
	{
		game.Exit();
	}

	public void Activate(GameTime gameTime)
	{
		_ = Statics.balls[0];
		switch (GameState.Current)
		{
		case GameState.Type.AIMING:
			GameState.Change(GameState.Type.CHOOSING_POWER, gameTime);
			break;
		case GameState.Type.CHOOSING_POWER:
			GameState.ChangeWithTransition(GameState.Type.WATCHING_MOVE, gameTime);
			break;
		case GameState.Type.REPOSITION_WBALL:
			if (GameState.RepoWballAvailable)
			{
				GameState.ChangeWithTransition(GameState.Type.AIMING, gameTime);
			}
			break;
		case GameState.Type.GAME_OVER:
			Statics.menus.WantedScreenId = 0;
			Statics.menus.Enable();
			GameState.Change(GameState.Type.MENUS, gameTime);
			break;
		case GameState.Type.WATCHING_MOVE:
			break;
		}
	}

	public void InputDirection(Vector2 direction, bool highSensitivity)
	{
		Ball ball = Statics.balls[0];
		switch (GameState.Current)
		{
		case GameState.Type.AIMING:
		{
			float x = direction.X;
			if (x != 0f)
			{
				Aiming.Change(x * 0.03f * ((!highSensitivity) ? 0.05f : 1f));
				Statics.cam.Update_Aiming_Normal(Aiming.AimVector);
				Cue.Update_Aiming();
				foreach (FunkyBandes.CollisionInfoFourBande item in FunkyBandes.listCollisionInfo)
				{
					FunkyBandes.CollisionInfoOneBande[] data = item.Data;
					foreach (FunkyBandes.CollisionInfoOneBande collisionInfoOneBande in data)
					{
						collisionInfoOneBande.Hit = false;
					}
				}
				ball.updateChangementDeDirection(ball.Pos.Value2D, Aiming.AimVector2D);
				LigneVisee.Compute();
			}
			x = Utils.PowerCurve(direction.Y, 2f);
			if (x != 0f)
			{
				Statics.cam.ChangeHeight(x * 0.05f);
			}
			break;
		}
		case GameState.Type.REPOSITION_WBALL:
			if (!GameState.DisableInputRepoWball && direction != Vector2.Zero)
			{
				direction.Y *= -1f;
				Vector2 vector = Vector2.Zero;
				if (Statics.cam.type == CameraBillard.Type.NORMAL)
				{
					vector = new Vector2(direction.Y, 0f - direction.X);
				}
				else if (Statics.cam.type == CameraBillard.Type.ALT)
				{
					vector = ((!Statics.cam.AltCamUpVectorSens) ? direction : (direction * -1f));
				}
				Vector2 vector2 = ball.Pos.Value2D + vector * 0.4f;
				if (GameState.IsRepoWballValid(vector2))
				{
					ball.Pos.Set(vector2);
					ball.UpdateDisplayMatrix();
					GameState.UpdateRepoWball(vector2);
				}
			}
			break;
		}
	}

	public void FirstFrame(GameTime gameTime)
	{
		Menus(gameTime, 13);
	}

	public void MainMenu(GameTime gameTime)
	{
		Menus(gameTime, 0);
	}

	public void Menus(GameTime gameTime, int wantedScreenID)
	{
		Statics.menus.WantedScreenId = wantedScreenID;
		if (wantedScreenID == 13)
		{
			DeferredEnableMenusTime = gameTime.TotalGameTime.TotalSeconds + 2.5;
		}
		else
		{
			Statics.menus.Enable();
		}
		GameState.Change(GameState.Type.MENUS, gameTime);
		Statics.cam.InitiateTransitionMenu(gameTime);
	}

	public void PauseON(PlayerIndex p)
	{
		if (!Statics.menus.Paused)
		{
			Statics.menus.Paused = true;
			Statics.menus.PauseMenu.Enable();
			Statics.menus.PauseController = p;
		}
	}

	public void PauseOFF()
	{
		if (Statics.menus.Paused)
		{
			Statics.menus.Paused = false;
			Statics.menus.PauseMenu.Disable();
			Statics.menus.PauseController = (PlayerIndex)(-2);
		}
	}

	public void PauseMenuHandleInput(Utils.Input.ActionMenu a)
	{
		if (a != Utils.Input.ActionMenu.MENU_LEFT && a != Utils.Input.ActionMenu.MENU_RIGHT)
		{
			Audio.PlaySFX(Audio.SFXID.Menu);
			Statics.menus.PauseMenu.HandleInput(a);
		}
	}

	public void GameOver(GameTime gameTime, int winningTeamIndex)
	{
		Statics.menus.GameOver(gameTime, winningTeamIndex);
		GameState.Change(GameState.Type.GAME_OVER, gameTime);
		Statics.cam.InitiateTransitionMenu(gameTime);
	}

	public void ResetBalls()
	{
		foreach (Ball ball in Statics.balls)
		{
			ball.Reset(isAlive: true);
		}
	}

	public void DebugSwitchAimingHelp(PlayerIndex currentPlayer)
	{
		LigneVisee.Levels[(int)currentPlayer] = (LigneVisee.Level)((int)(LigneVisee.Levels[(int)currentPlayer] + 1) % 3);
	}
}
