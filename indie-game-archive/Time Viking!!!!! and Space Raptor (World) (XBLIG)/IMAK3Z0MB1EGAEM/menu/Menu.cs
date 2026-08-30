using IMAK3Z0MB1EGAEM.director;
using IMAK3Z0MB1EGAEM.hud;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Viking_x86;

namespace IMAK3Z0MB1EGAEM.menu;

internal class Menu
{
	public enum PlayerState
	{
		Out,
		In,
		Ready
	}

	private const string JOIN1 = "j0in";

	private const string JOIN2 = "game!1";

	private const string READY = "ready!";

	private const string A_GO = "(a) go";

	private const string A_OK = "(a) ok";

	private const string B_CANCEL = "(B) cancel";

	private const string Y_SCORES = "(Y) scores";

	private const string READY1 = "ready??";

	private const string READYA1 = "press (A)!";

	private const string READY2 = "READY!!!1";

	private const string IMADEAGAME = "I MAED A GAM3 W1TH";

	private const string ZOMBIESINIT = "Z0MBIES 1N IT!!!1";

	private const string TWINSTICKSHOOTER = "(IT'S A TW1N ST1CK SH00T3R)";

	private const string TIME = "TIME";

	private const string VIKING = "VIKING";

	private const string TIME_VIKING = "TIME VIKING!!!!!";

	private const string EXC = "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!";

	private const string AND_SPACE_RAPTOR = "!!!!!!!AND SPACE RAPTOR";

	private const string ENDLESS = "ENDL3SS";

	private const string ZOMBIES = "Z0MB1ES!!!1";

	private const string X_INFO = "(x) infos";

	private static GamePadState[] pgs = new GamePadState[4];

	public static PlayerState[] playerState = new PlayerState[4];

	public static int quitYouSure = -1;

	public static bool needsQuit = false;

	public static int infos = -1;

	private static QuitYouSure quitMenu = new QuitYouSure();

	private static InfosMenu infosMenu = new InfosMenu();

	private static HighScoreMenu highScoreMenu = new HighScoreMenu();

	public static float timeGo = 0f;

	public static int grace;

	public static int scoreMode = -1;

	private static float inFrame = 0f;

	public static void Reset()
	{
		scoreMode = -1;
		quitYouSure = -1;
		for (int i = 0; i < playerState.Length; i++)
		{
			playerState[i] = PlayerState.Out;
		}
		for (int j = 0; j < VikingGame.mainPlayerIdx.Length; j++)
		{
			VikingGame.mainPlayerIdx[j] = -1;
		}
		timeGo = 0f;
		inFrame = 1f;
	}

	public static void Update()
	{
		if (inFrame > 0f)
		{
			inFrame -= Game1.frameTime * 4f;
			return;
		}
		if (scoreMode > -1)
		{
			for (int i = 0; i < 4; i++)
			{
				highScoreMenu.Update(i);
			}
		}
		else if (infos > -1)
		{
			infosMenu.Update(infos);
		}
		else if (quitYouSure > -1)
		{
			quitMenu.Update(quitYouSure);
		}
		else
		{
			for (int j = 0; j < 4; j++)
			{
				GamePadState state = GamePad.GetState((PlayerIndex)j);
				if (grace <= 0)
				{
					if ((state.Buttons.A == ButtonState.Pressed && pgs[j].Buttons.A == ButtonState.Released) || (state.Buttons.Start == ButtonState.Pressed && pgs[j].Buttons.Start == ButtonState.Released))
					{
						if (ZombieGame.mainPlayerIndex < 0)
						{
							ZombieGame.mainPlayerIndex = j;
							Game1.store.GetDevice();
						}
						int num = -1;
						for (int k = 0; k < 2; k++)
						{
							if (VikingGame.mainPlayerIdx[k] == j)
							{
								num = k;
							}
						}
						if (num == -1)
						{
							for (int l = 0; l < 2; l++)
							{
								if (VikingGame.mainPlayerIdx[l] != -1)
								{
									continue;
								}
								VikingGame.mainPlayerIdx[l] = j;
								num = l;
								HUD.playerName[l] = "Player" + (l + 1);
								foreach (SignedInGamer signedInGamer in Gamer.SignedInGamers)
								{
									if (signedInGamer.PlayerIndex == (PlayerIndex)j)
									{
										HUD.playerName[l] = signedInGamer.Gamertag;
									}
								}
								break;
							}
						}
						if (num > -1)
						{
							switch (playerState[num])
							{
							case PlayerState.Out:
								playerState[num] = PlayerState.In;
								break;
							case PlayerState.In:
								playerState[num] = PlayerState.Ready;
								break;
							}
						}
					}
					if (state.Buttons.B == ButtonState.Pressed && pgs[j].Buttons.B == ButtonState.Released)
					{
						int num2 = -1;
						for (int m = 0; m < 2; m++)
						{
							if (VikingGame.mainPlayerIdx[m] == j)
							{
								num2 = m;
							}
						}
						if (num2 > -1)
						{
							switch (playerState[num2])
							{
							case PlayerState.Ready:
								playerState[num2] = PlayerState.In;
								break;
							case PlayerState.In:
								playerState[num2] = PlayerState.Out;
								VikingGame.mainPlayerIdx[num2] = -1;
								break;
							case PlayerState.Out:
								quitYouSure = j;
								break;
							}
						}
						else
						{
							quitYouSure = j;
							quitMenu.grace = 5;
						}
					}
					if (state.Buttons.Y == ButtonState.Pressed && pgs[j].Buttons.Y == ButtonState.Released)
					{
						scoreMode = j;
						if (ZombieGame.mainPlayerIndex < 0)
						{
							ZombieGame.mainPlayerIndex = j;
						}
						highScoreMenu.grace = 5;
						Game1.store.Read();
					}
					if (state.Buttons.X == ButtonState.Pressed && pgs[j].Buttons.X == ButtonState.Released)
					{
						infos = j;
						infosMenu.grace = 5;
					}
				}
				pgs[j] = state;
			}
			if (grace > 0)
			{
				grace--;
				return;
			}
		}
		bool flag = false;
		bool flag2 = false;
		for (int n = 0; n < 4; n++)
		{
			if (playerState[n] == PlayerState.Ready)
			{
				flag2 = true;
			}
		}
		if (flag2)
		{
			bool flag3 = false;
			for (int num3 = 0; num3 < 4; num3++)
			{
				if (playerState[num3] == PlayerState.In)
				{
					flag3 = true;
				}
			}
			if (!flag3)
			{
				flag = true;
			}
		}
		if (flag)
		{
			timeGo += Game1.frameTime;
			if (timeGo > 1f)
			{
				switch (GameState.state)
				{
				case GameState.State.VikingMenu:
					Game1.vgame.Play();
					break;
				case GameState.State.ZombiesMenu:
				case GameState.State.ZombiesPlaying:
				case GameState.State.VikingPlaying:
				case GameState.State.EndlessZombiesMenu:
					break;
				}
			}
		}
		else
		{
			timeGo = 0f;
		}
	}

	public static void DrawVikingMenu()
	{
		DrawVikingMenu(3);
	}

	public static void DrawVikingMenu(int rows)
	{
		Text.DrawString("TIME VIKING!!!!!", new Vector2(VScroll.screenSize.X / 2f, 120f), 16f, new Color(Rand.GetRandomFloat(0.4f, 1f), Rand.GetRandomFloat(0.4f, 1f), Rand.GetRandomFloat(0.4f, 1f), 1f), Text.Justify.Center);
		if (rows > 0)
		{
			Text.DrawString("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", new Vector2(VScroll.screenSize.X / 2f, 205f), 14f, new Color(Rand.GetRandomFloat(0.4f, 1f), Rand.GetRandomFloat(0.4f, 1f), Rand.GetRandomFloat(0.4f, 1f), 1f), Text.Justify.Center);
		}
		if (rows > 1)
		{
			Text.DrawString("!!!!!!!AND SPACE RAPTOR", new Vector2(VScroll.screenSize.X / 2f, 280f), 12f, new Color(Rand.GetRandomFloat(0.4f, 1f), Rand.GetRandomFloat(0.4f, 1f), Rand.GetRandomFloat(0.4f, 1f), 1f), Text.Justify.Center);
		}
	}

	public static void Draw()
	{
		if (scoreMode > -1)
		{
			highScoreMenu.Draw(new Vector2(640f, 150f));
		}
		else if (infos > -1)
		{
			infosMenu.Draw(new Vector2(640f, 150f));
		}
		else if (quitYouSure > -1)
		{
			quitMenu.Draw(new Vector2(640f, 300f));
		}
		else
		{
			switch (GameState.state)
			{
			case GameState.State.VikingMenu:
				DrawVikingMenu();
				break;
			case GameState.State.ZombiesMenu:
				Text.DrawString("I MAED A GAM3 W1TH", new Vector2(VScroll.screenSize.X / 2f, 120f), 11f, new Color(Rand.GetRandomFloat(0.4f, 1f), Rand.GetRandomFloat(0.4f, 1f), Rand.GetRandomFloat(0.4f, 1f), 1f), Text.Justify.Center);
				Text.DrawString("Z0MBIES 1N IT!!!1", new Vector2(VScroll.screenSize.X / 2f, 200f), 11f, new Color(Rand.GetRandomFloat(0.4f, 1f), Rand.GetRandomFloat(0.4f, 1f), Rand.GetRandomFloat(0.4f, 1f), 1f), Text.Justify.Center);
				break;
			case GameState.State.EndlessZombiesMenu:
				Text.DrawString("ENDL3SS", new Vector2(VScroll.screenSize.X / 2f, 100f), 22f, new Color(Rand.GetRandomFloat(0.4f, 1f), Rand.GetRandomFloat(0.4f, 1f), Rand.GetRandomFloat(0.4f, 1f), 1f), Text.Justify.Center);
				Text.DrawString("Z0MB1ES!!!1", new Vector2(VScroll.screenSize.X / 2f, 200f), 16f, new Color(Rand.GetRandomFloat(0.4f, 1f), Rand.GetRandomFloat(0.4f, 1f), Rand.GetRandomFloat(0.4f, 1f), 1f), Text.Justify.Center);
				break;
			}
			bool flag = false;
			float num = 40f;
			for (int i = 0; i < 2; i++)
			{
				float x = (float)(i + 2) / 5f * 1280f;
				if (playerState[i] != PlayerState.Out)
				{
					Text.DrawString(HUD.playerName[i], new Vector2(x, 380f + num), 5f, Color.Gray, Text.Justify.Center);
				}
				switch (playerState[i])
				{
				case PlayerState.Out:
					Text.DrawString("j0in", new Vector2(x, 430f + num), 5f, Color.White, Text.Justify.Center);
					Text.DrawString("game!1", new Vector2(x, 465f + num), 5f, Color.White, Text.Justify.Center);
					break;
				case PlayerState.In:
					flag = true;
					Text.DrawString("ready??", new Vector2(x, 430f + num), 5f, Color.White, Text.Justify.Center);
					Text.DrawString("press (A)!", new Vector2(x, 465f + num), 5f, new Color(Rand.GetRandomFloat(0.4f, 1f), Rand.GetRandomFloat(0.4f, 1f), Rand.GetRandomFloat(0.4f, 1f), 1f), Text.Justify.Center);
					break;
				case PlayerState.Ready:
					flag = true;
					Text.DrawString("READY!!!1", new Vector2(x, 430f + num), 5f, HUD.idToColor(i), Text.Justify.Center);
					break;
				}
			}
			if (flag)
			{
				Text.DrawString("(IT'S A TW1N ST1CK SH00T3R)", new Vector2(VScroll.screenSize.X / 2f, 360f), 6f, new Color(Rand.GetRandomFloat(0.4f, 1f), Rand.GetRandomFloat(0.4f, 1f), Rand.GetRandomFloat(0.4f, 1f), 1f), Text.Justify.Center);
			}
			if (timeGo > 0f)
			{
				SpriteTools.End();
				SpriteTools.BeginAlpha();
				SpriteTools.sprite.Draw(Game1.nullTex, new Rectangle(0, 0, (int)ScrollMan.screenSize.X, (int)ScrollMan.screenSize.Y), new Color(0f, 0f, 0f, timeGo));
			}
		}
		if (scoreMode > -1 || infos > -1)
		{
			DrawOk();
		}
		else if (quitYouSure > -1)
		{
			DrawOkCancel();
		}
		else
		{
			DrawOkCancelScores();
		}
		if (inFrame > 0f)
		{
			SpriteTools.End();
			SpriteTools.BeginAlpha();
			SpriteTools.sprite.Draw(Game1.nullTex, new Rectangle(0, 0, (int)ScrollMan.screenSize.X, (int)ScrollMan.screenSize.Y), new Color(0f, 0f, 0f, inFrame));
		}
	}

	public static void DrawOk()
	{
		Text.DrawString("(a) ok", new Vector2(VScroll.screenSize.X / 2f, VScroll.screenSize.Y - 100f), 6f, Color.Lime, Text.Justify.Center);
	}

	public static void DrawReady()
	{
		Text.DrawString("ready!", new Vector2(VScroll.screenSize.X / 2f, VScroll.screenSize.Y - 100f), 6f, Color.Lime, Text.Justify.Center);
	}

	public static void DrawOkCancel()
	{
		Text.DrawString("(a) go", new Vector2(VScroll.screenSize.X / 2f - 150f, VScroll.screenSize.Y - 100f), 6f, Color.Lime, Text.Justify.Center);
		Text.DrawString("(B) cancel", new Vector2(VScroll.screenSize.X / 2f + 150f, VScroll.screenSize.Y - 100f), 6f, Color.Red, Text.Justify.Center);
	}

	public static void DrawOkCancelScores()
	{
		Text.DrawString("(a) go", new Vector2(VScroll.screenSize.X / 2f + 125f, VScroll.screenSize.Y - 100f), 6f, Color.Lime, Text.Justify.Center);
		Text.DrawString("(B) cancel", new Vector2(VScroll.screenSize.X / 2f + 400f, VScroll.screenSize.Y - 100f), 6f, Color.Red, Text.Justify.Center);
		Text.DrawString("(Y) scores", new Vector2(VScroll.screenSize.X / 2f - 125f, VScroll.screenSize.Y - 100f), 6f, Color.Yellow, Text.Justify.Center);
		Text.DrawString("(x) infos", new Vector2(VScroll.screenSize.X / 2f - 400f, VScroll.screenSize.Y - 100f), 6f, Color.RoyalBlue, Text.Justify.Center);
	}
}
