using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceBlast.Screens;

internal class GameResultsScreen : MenuScreen
{
	private Rectangle m_ScreenRect;

	private List<Player> m_PlayerScores;

	private double m_NewGameTimer;

	private double m_EnableInputTimer;

	public GameResultsScreen(ScreenManager manager)
		: base(manager)
	{
	}

	public override void LoadContent()
	{
		RecalcScreenRect();
		base.LoadContent();
	}

	protected override void OnRedButtonPressed()
	{
		if (!(TimeManager.RawTime < m_EnableInputTimer))
		{
			MainGame.NetMan.AbortGameSession();
			m_ScreenManager.ShowScreen(ScreenType.MainMenu);
			base.OnRedButtonPressed();
		}
	}

	protected override void OnGreenButtonPressed()
	{
		if (!(TimeManager.RawTime < m_EnableInputTimer))
		{
			if (!MainGame.NetMan.IsNetworkGame)
			{
				MainGame.Instance.StartNextLevel();
			}
			base.OnGreenButtonPressed();
		}
	}

	public override void Update()
	{
		if (MainGame.NetMan.IsNetworkGame && (int)(m_NewGameTimer - TimeManager.RawTime) <= 0)
		{
			if (MainGame.NetMan.IsHost)
			{
				MainGame.NetMan.StartNextLevel();
			}
			m_ScreenManager.HideScreen();
		}
		base.Update();
	}

	public override void Draw(float alpha)
	{
		Color white = Color.White;
		white.A = (byte)(alpha * 255f);
		Color yellow = Color.Yellow;
		yellow.A = (byte)(alpha * 255f);
		Spritebatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
		Vector2 center = new Vector2
		{
			X = m_ScreenRect.Center.X,
			Y = (float)m_ScreenRect.Top + 20f
		};
		DrawText(FontMenuItem, center, "Game Results", white, TextAlign.textCentered);
		center.Y += 32f;
		Vector2 center2 = new Vector2((float)m_ScreenRect.Left + (float)m_ScreenRect.Width * 0.2f, center.Y);
		Vector2 center3 = new Vector2((float)m_ScreenRect.Left + (float)m_ScreenRect.Width * 0.25f, center.Y);
		Vector2 center4 = new Vector2((float)m_ScreenRect.Left + (float)m_ScreenRect.Width * 0.8f, center.Y);
		ETeam eTeam = ETeam.None;
		foreach (Player playerScore in m_PlayerScores)
		{
			if (playerScore.Team != eTeam)
			{
				center3.Y += 25f;
				center4.Y += 25f;
				center2.Y = center3.Y;
				int teamScore = MainGame.Players.GetTeamScore(playerScore.Team);
				string teamName = PlayerList.GetTeamName(playerScore.Team);
				DrawText(FontMenuItem, center2, teamName + " Team", white, TextAlign.textLeft);
				DrawText(FontMenuItem, center4, teamScore.ToString(), white, TextAlign.textLeft);
				center3.Y += 32f;
				center4.Y += 32f;
				eTeam = playerScore.Team;
			}
			DrawText(FontSmallMenuItem, center3, playerScore.GetGamerTag() + " ", (playerScore is HumanPlayer) ? yellow : white, TextAlign.textLeft);
			center3.Y += 25f;
			DrawText(FontSmallMenuItem, center4, playerScore.Kills.ToString(), (playerScore is HumanPlayer) ? yellow : white, TextAlign.textLeft);
			center4.Y += 25f;
		}
		if (MainGame.NetMan.IsNetworkGame)
		{
			center.Y = center4.Y + 25f;
			string text = "Next level starts automatically in " + (int)(m_NewGameTimer - TimeManager.RawTime) + " seconds";
			DrawText(FontSmallMenuItem, center, text, white, TextAlign.textCentered);
		}
		Spritebatch.End();
		base.Draw(alpha);
	}

	public override void OnShowScreen()
	{
		m_NewGameTimer = TimeManager.RawTime + 10.0;
		m_EnableInputTimer = TimeManager.RawTime + 2.0;
		m_PlayerScores = MainGame.Players.GetPlayers();
		m_PlayerScores.Sort(ComparePlayersByScore);
		if (m_PlayerScores[0] is HumanPlayer)
		{
			Utils.SetRichPresence(GamerPresenceMode.WonTheGame, null);
		}
		else
		{
			Utils.SetRichPresence(GamerPresenceMode.GameOver, null);
		}
		RedButtonText = "Exit";
		GreenButtonText = null;
		BlueButtonText = null;
		YellowButtonText = null;
		if (!MainGame.NetMan.IsNetworkGame)
		{
			GreenButtonText = "Play";
		}
		base.OnShowScreen();
	}

	private static int ComparePlayersByScore(Player player1, Player player2)
	{
		if (player1 == null && player2 == null)
		{
			return 0;
		}
		if (player1 == null)
		{
			return 1;
		}
		if (player2 == null)
		{
			return -1;
		}
		if (player1.Team != player2.Team)
		{
			int teamScore = MainGame.Players.GetTeamScore(player1.Team);
			int teamScore2 = MainGame.Players.GetTeamScore(player2.Team);
			if (teamScore > teamScore2)
			{
				return -1;
			}
			if (teamScore < teamScore2)
			{
				return 1;
			}
		}
		if (player1.Kills > player2.Kills)
		{
			return -1;
		}
		return 1;
	}

	public override Rectangle GetScreenRect()
	{
		RecalcScreenRect();
		return m_ScreenRect;
	}

	public override void OnScreenResize()
	{
		RecalcScreenRect();
		base.OnScreenResize();
	}

	private void RecalcScreenRect()
	{
		Vector2 vector = new Vector2(600f, 500f);
		vector.Y = 150 + MainGame.Players.Count * 40;
		float num = MainGame.Instance.GraphicsDevice.Viewport.Width;
		float num2 = num * 0.5f;
		float num3 = MainGame.Instance.GraphicsDevice.Viewport.Height;
		float num4 = num3 * 0.5f;
		Vector2 vector2 = new Vector2(num2 - vector.X * 0.5f, num4 - vector.Y * 0.5f);
		m_ScreenRect = default(Rectangle);
		m_ScreenRect.X = (int)vector2.X;
		m_ScreenRect.Y = (int)vector2.Y;
		m_ScreenRect.Width = (int)vector.X;
		m_ScreenRect.Height = (int)vector.Y;
	}
}
