using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarbeamDefenderGame;

public class MainGame
{
	private Texture2D backgroundtexture;

	private Texture2D buildingtexture;

	private Texture2D buildingshieldtexture;

	private int floorheight;

	private Vector2 buildingsize;

	private Vector2 buildingtexturesize;

	private int buildingmaxframes;

	private int buildinganimspeed;

	private Texture2D floortexture;

	private Texture2D backlayertexture;

	private Texture2D turrettexture;

	private Texture2D cursortexture;

	private Texture2D trailtexture;

	private Texture2D missiletexture;

	private Texture2D blastertexture;

	private Texture2D explosiontexture;

	private Texture2D enemyprojectiletexture;

	private Texture2D graphicexplosiontexture;

	private Texture2D boulderstexture;

	private Texture2D saucertexture;

	private Texture2D purchasetexture;

	private Texture2D purchaseselected;

	private Texture2D purchasebordertexture;

	private Texture2D smoketexture;

	private Texture2D pausedtexture;

	private int purchaseplayerone;

	private int purchaseplayertwo;

	private int playeroneinputdelay;

	private int playertwoinputdelay;

	private Building[] buildings;

	private Turret[] players;

	private SpriteFont gamefont;

	private SpriteFont notificationfont;

	private MissileManager missilemanager;

	private ExplosionManager explosionmanager;

	private ProjectileManager projectilemanager;

	private AIManager aimanager;

	private int roundtimeout;

	private int inputdelay;

	private int gamestate;

	private AudioManager audiomanager;

	public Turret[] Players => players;

	public int[] Scores
	{
		get
		{
			int[] array = new int[players.Count()];
			for (int i = 0; i < players.Count(); i++)
			{
				array[i] = players[i].Score;
			}
			return array;
		}
	}

	public void SetupLevel(int floorheight, Vector2 buildingsize, Vector2 buildingtexturesize, int buildingmaxframes, int buildinganimspeed)
	{
		this.floorheight = floorheight;
		this.buildingsize = buildingsize;
		this.buildingtexturesize = buildingtexturesize;
		this.buildingmaxframes = buildingmaxframes;
		this.buildinganimspeed = buildinganimspeed;
	}

	public void LoadAssets(string levelfolder, ContentManager content, AudioManager audiomanager)
	{
		gamefont = content.Load<SpriteFont>("Fonts/GameFont");
		notificationfont = content.Load<SpriteFont>("Fonts/LargeFont");
		backgroundtexture = content.Load<Texture2D>("Levels/" + levelfolder + "/Sky");
		buildingtexture = content.Load<Texture2D>("Levels/" + levelfolder + "/Building");
		buildingshieldtexture = content.Load<Texture2D>("Levels/" + levelfolder + "/BuildingShield");
		floortexture = content.Load<Texture2D>("Levels/" + levelfolder + "/GroundTile");
		backlayertexture = content.Load<Texture2D>("Levels/" + levelfolder + "/Mountains");
		turrettexture = content.Load<Texture2D>("Units/Turret");
		cursortexture = content.Load<Texture2D>("Units/Cursor");
		trailtexture = content.Load<Texture2D>("Units/Trail");
		blastertexture = content.Load<Texture2D>("Units/Projectile");
		explosiontexture = content.Load<Texture2D>("Units/Explosion");
		enemyprojectiletexture = content.Load<Texture2D>("Units/EnemyProjectile");
		graphicexplosiontexture = content.Load<Texture2D>("Units/GraphicExplosion");
		saucertexture = content.Load<Texture2D>("Units/Saucer");
		boulderstexture = content.Load<Texture2D>("Units/Boulders");
		missiletexture = content.Load<Texture2D>("Units/Missile");
		smoketexture = content.Load<Texture2D>("Units/Trail");
		purchasetexture = content.Load<Texture2D>("PurchaseMenus/Background");
		purchaseselected = content.Load<Texture2D>("PurchaseMenus/Button");
		purchasebordertexture = content.Load<Texture2D>("PurchaseMenus/Border");
		pausedtexture = content.Load<Texture2D>("Menus/Paused");
		this.audiomanager = audiomanager;
		missilemanager = new MissileManager(this.audiomanager);
		explosionmanager = new ExplosionManager(this.audiomanager);
		aimanager = new AIManager();
		projectilemanager = new ProjectileManager(this.audiomanager);
	}

	private void ShowRoundWindow(SpriteBatch spritebatch)
	{
		if (roundtimeout <= 0)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < buildings.Count(); i++)
		{
			if (buildings[i].Shields > 0f)
			{
				num++;
			}
		}
		Vector2 vector = gamefont.MeasureString("Round " + aimanager.RoundNumber);
		spritebatch.DrawString(gamefont, "Round " + aimanager.RoundNumber, new Vector2(640f - vector.X / 2f, 250f), players[0].PlayerColor);
		if (aimanager.RoundNumber != 1)
		{
			vector = gamefont.MeasureString("Defender Bonus " + num + " x 250 points");
			spritebatch.DrawString(gamefont, "Defender Bonus " + num + " x 250 points", new Vector2(640f - vector.X / 2f, 300f), players[0].PlayerColor);
			return;
		}
		vector = gamefont.MeasureString("Press A or Right Trigger for Anti Missile Rockets");
		spritebatch.DrawString(gamefont, "Press A or Right Trigger for Anti Missile Rockets", new Vector2(640f - vector.X / 2f, 300f), players[0].PlayerColor);
		vector = gamefont.MeasureString("Press B or Left Trigger for Anti Ship Proton Blaster");
		spritebatch.DrawString(gamefont, "Press B or Right Trigger for Anti Ship Proton Blaster", new Vector2(640f - vector.X / 2f, 350f), players[0].PlayerColor);
	}

	public void CheckRoundComplete(int timems)
	{
		if (roundtimeout > 0)
		{
			roundtimeout -= timems;
			if (roundtimeout < 1)
			{
				aimanager.NextRound();
				roundtimeout = 0;
			}
			return;
		}
		int num = 0;
		for (int i = 0; i < buildings.Count(); i++)
		{
			if (buildings[i].Shields > 0f)
			{
				num++;
			}
		}
		if (num == 0)
		{
			roundtimeout = 5000;
			gamestate = 2;
		}
		else
		{
			if (!aimanager.RoundComplete() || missilemanager.ActiveEnemyMissiles() != 0)
			{
				return;
			}
			for (int j = 0; j < buildings.Count(); j++)
			{
				if (buildings[j].Shields > 0f)
				{
					for (int k = 0; k < players.Count(); k++)
					{
						players[k].Score += 250;
					}
				}
			}
			gamestate = 4;
			purchaseplayerone = 0;
			purchaseplayertwo = 0;
			playeroneinputdelay = 500;
			playertwoinputdelay = 500;
			roundtimeout = 3000;
		}
	}

	private void ResetComponents(int difficulty)
	{
		aimanager.Difficulty = difficulty;
		missilemanager.Reset();
		aimanager.Reset();
		projectilemanager.Reset();
		explosionmanager.Reset();
		buildings = new Building[4];
		int num = 320;
		int num2 = num / 2 - 100;
		int num3 = floorheight;
		buildings[0] = new Building(new Vector2(num2, num3), buildingsize, buildingmaxframes, buildinganimspeed);
		buildings[1] = new Building(new Vector2(num + num2, num3), buildingsize, buildingmaxframes, buildinganimspeed);
		buildings[2] = new Building(new Vector2(num * 2 + num2, num3), buildingsize, buildingmaxframes, buildinganimspeed);
		buildings[3] = new Building(new Vector2(num * 3 + num2, num3), buildingsize, buildingmaxframes, buildinganimspeed);
		roundtimeout = 5000;
		gamestate = 0;
		inputdelay = 500;
	}

	public void Reset(PlayerIndex playerone, Color playeronecolor, PlayerIndex playertwo, Color playertwocolor, int difficulty)
	{
		ResetComponents(difficulty);
		players = new Turret[2];
		players[0] = new Turret(playerone, new Vector2(buildings[0].Position.X + buildings[0].Size.X + 60f, floorheight + 35), playeronecolor);
		players[1] = new Turret(playertwo, new Vector2(buildings[2].Position.X + buildings[2].Size.X + 60f, floorheight + 35), playertwocolor);
		aimanager.PlayersCount = players.Count();
	}

	public void Reset(PlayerIndex playerone, Color playercolor, int difficulty)
	{
		ResetComponents(difficulty);
		players = new Turret[1];
		players[0] = new Turret(playerone, new Vector2(640f, floorheight + 35), playercolor);
		aimanager.PlayersCount = players.Count();
	}

	public void Draw(SpriteBatch spritebatch)
	{
		Rectangle rectangle = new Rectangle(0, 0, backgroundtexture.Width, backgroundtexture.Height);
		Rectangle rectangle2 = new Rectangle(0, 0, 1280, 625);
		spritebatch.Draw(backgroundtexture, rectangle, rectangle, Color.White);
		rectangle = new Rectangle(0, 0, 1280, 600);
		spritebatch.Draw(destinationRectangle: new Rectangle(0, 120, 1280, 600), texture: backlayertexture, sourceRectangle: rectangle, color: Color.White);
		rectangle = new Rectangle(0, 0, floortexture.Width, floortexture.Height);
		for (int i = 0; i < 1280; i += floortexture.Width)
		{
			spritebatch.Draw(destinationRectangle: new Rectangle(i, 720 - floortexture.Height, floortexture.Width, floortexture.Height), texture: floortexture, sourceRectangle: rectangle, color: Color.White);
		}
		DrawBuildings(spritebatch);
		DrawTurrets(spritebatch);
		DrawExplosions(spritebatch);
		DrawMissiles(spritebatch);
		DrawProjectiles(spritebatch);
		DrawSaucers(spritebatch);
		DrawAsteroids(spritebatch);
		DrawHud(spritebatch);
		switch (gamestate)
		{
		case 0:
			ShowRoundWindow(spritebatch);
			break;
		case 1:
		{
			Vector2 vector = notificationfont.MeasureString("Paused");
			spritebatch.DrawString(notificationfont, "Paused", new Vector2(640f - vector.X / 2f, 350f), Color.Yellow);
			break;
		}
		case 2:
			DrawGameOver(spritebatch);
			break;
		case 4:
			ShowPurchaseMenu(spritebatch);
			break;
		case 5:
			ShowPauseMenu(spritebatch);
			break;
		case 3:
			break;
		}
	}

	private void ShowPauseMenu(SpriteBatch spritebatch)
	{
		Rectangle rectangle = new Rectangle(0, 0, 1280, 720);
		spritebatch.Draw(pausedtexture, rectangle, rectangle, Color.White);
		Vector2 vector = gamefont.MeasureString("Game Paused");
		spritebatch.DrawString(gamefont, "Game Paused", new Vector2(640f - vector.X / 2f, 200f), Color.Yellow);
		vector = gamefont.MeasureString("Press A To Continue");
		spritebatch.DrawString(gamefont, "Press A To Continue", new Vector2(640f - vector.X / 2f, 300f), Color.Yellow);
		vector = gamefont.MeasureString("Press B To Abandon The Game");
		spritebatch.DrawString(gamefont, "Press B To Abandon The Game", new Vector2(640f - vector.X / 2f, 350f), Color.Yellow);
	}

	private void DrawTurrets(SpriteBatch spritebatch)
	{
		Rectangle value = new Rectangle(0, 0, 128, 128);
		for (int i = 0; i < players.Count(); i++)
		{
			spritebatch.Draw(destinationRectangle: new Rectangle((int)players[i].Position.X - 32, (int)players[i].Position.Y, 64, 64), texture: turrettexture, sourceRectangle: value, color: Color.White);
		}
		value = new Rectangle(0, 0, 64, 64);
		for (int j = 0; j < players.Count(); j++)
		{
			spritebatch.Draw(destinationRectangle: new Rectangle((int)players[j].CursorPosition.X - 22, (int)players[j].CursorPosition.Y - 22, 44, 44), texture: cursortexture, sourceRectangle: value, color: players[j].PlayerColor);
		}
	}

	private void DrawMissiles(SpriteBatch spritebatch)
	{
		DrawEnemyMissiles(spritebatch);
		DrawPlayerMissiles(spritebatch);
	}

	private void DrawEnemyMissiles(SpriteBatch spritebatch)
	{
		for (int i = 0; i < missilemanager.SmokeClouds.Count(); i++)
		{
			if (missilemanager.SmokeClouds[i].Active)
			{
				Rectangle value = new Rectangle(32 * missilemanager.SmokeClouds[i].Frame, 0, 32, (int)missilemanager.SmokeClouds[i].Size * 2);
				spritebatch.Draw(destinationRectangle: new Rectangle((int)missilemanager.SmokeClouds[i].Position.X, (int)missilemanager.SmokeClouds[i].Position.Y, 16, (int)missilemanager.SmokeClouds[i].Size), texture: smoketexture, sourceRectangle: value, color: Color.White * missilemanager.SmokeClouds[i].Alpha, rotation: MathHelper.ToRadians(missilemanager.SmokeClouds[i].Angle), origin: new Vector2(16f, 32f), effects: SpriteEffects.None, layerDepth: 0f);
			}
		}
		for (int i = 0; i < missilemanager.EnemyMissiles.Count(); i++)
		{
			if (missilemanager.EnemyMissiles[i].Active)
			{
				Rectangle value = new Rectangle(32 * missilemanager.EnemyMissiles[i].Frame, 0, 32, 1026);
				spritebatch.Draw(destinationRectangle: new Rectangle((int)missilemanager.EnemyMissiles[i].Position.X, (int)missilemanager.EnemyMissiles[i].Position.Y, 16, 513), texture: missiletexture, sourceRectangle: value, color: Color.White, rotation: MathHelper.ToRadians(missilemanager.EnemyMissiles[i].AngleOfTravel), origin: new Vector2(16f, 32f), effects: SpriteEffects.None, layerDepth: 0f);
			}
		}
	}

	private void DrawPlayerMissiles(SpriteBatch spritebatch)
	{
		for (int i = 0; i < missilemanager.PlayerMissiles.Count(); i++)
		{
			if (missilemanager.PlayerMissiles[i].Active)
			{
				Rectangle value;
				Rectangle destinationRectangle;
				if (missilemanager.PlayerMissiles[i].LaunchLength > 512)
				{
					value = new Rectangle(32 * missilemanager.PlayerMissiles[i].Frame, 0, 32, 1024);
					destinationRectangle = new Rectangle((int)missilemanager.PlayerMissiles[i].Position.X, (int)missilemanager.PlayerMissiles[i].Position.Y, 16, 512);
				}
				else
				{
					value = new Rectangle(0, 0, 32, missilemanager.PlayerMissiles[i].LaunchLength * 2);
					destinationRectangle = new Rectangle((int)missilemanager.PlayerMissiles[i].Position.X, (int)missilemanager.PlayerMissiles[i].Position.Y, 16, missilemanager.PlayerMissiles[i].LaunchLength);
				}
				spritebatch.Draw(missiletexture, destinationRectangle, value, Color.White, MathHelper.ToRadians(missilemanager.PlayerMissiles[i].AngleOfTravel), new Vector2(16f, 32f), SpriteEffects.None, 0f);
			}
		}
	}

	private void DrawProjectiles(SpriteBatch spritebatch)
	{
		new Rectangle(0, 0, 8, 8);
		for (int i = 0; i < projectilemanager.EnemyProjectiles.Count(); i++)
		{
			if (projectilemanager.EnemyProjectiles[i].Active)
			{
				spritebatch.Draw(destinationRectangle: new Rectangle((int)projectilemanager.EnemyProjectiles[i].Position.X, (int)projectilemanager.EnemyProjectiles[i].Position.Y, 8, 8), texture: enemyprojectiletexture, color: Color.White);
			}
		}
		for (int i = 0; i < projectilemanager.PlayerProjectiles.Count(); i++)
		{
			if (projectilemanager.PlayerProjectiles[i].Active)
			{
				spritebatch.Draw(destinationRectangle: new Rectangle((int)projectilemanager.PlayerProjectiles[i].Position.X, (int)projectilemanager.PlayerProjectiles[i].Position.Y, 8, 8), texture: blastertexture, color: projectilemanager.PlayerProjectiles[i].Color);
			}
		}
	}

	private void DrawLine(SpriteBatch spritebatch, float width, Color color, Vector2 point1, Vector2 point2)
	{
		float rotation = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
		float x = Vector2.Distance(point1, point2);
		spritebatch.Draw(trailtexture, point1, null, color, rotation, Vector2.Zero, new Vector2(x, width), SpriteEffects.None, 0f);
	}

	private void DrawBuildings(SpriteBatch spritebatch)
	{
		for (int i = 0; i < buildings.Count(); i++)
		{
			if (buildings[i].Shields > 0f)
			{
				Rectangle value = new Rectangle(0, (int)(buildingtexturesize.Y * (float)buildings[i].AnimationFrame), (int)buildingtexturesize.X, (int)buildingtexturesize.Y);
				Rectangle destinationRectangle = new Rectangle((int)buildings[i].Position.X, (int)buildings[i].Position.Y, (int)buildings[i].Size.X, (int)buildings[i].Size.Y);
				spritebatch.Draw(buildingtexture, destinationRectangle, value, Color.White);
				spritebatch.Draw(destinationRectangle: new Rectangle((int)((double)destinationRectangle.Left - (double)destinationRectangle.Width * 0.15), (int)((float)destinationRectangle.Top - (float)destinationRectangle.Height * 0.3f), (int)((float)destinationRectangle.Width * 1.3f), (int)((float)destinationRectangle.Height * 1.3f)), sourceRectangle: new Rectangle(0, 0, 256, 128), texture: buildingshieldtexture, color: Color.White * buildings[i].ShieldTransparency);
				spritebatch.DrawString(gamefont, buildings[i].Shields.ToString("000") + "%", new Vector2(buildings[i].Position.X + buildings[i].Size.X / 2f, buildings[i].Position.Y + buildings[i].Size.Y), Color.Red);
			}
			else
			{
				Rectangle value = new Rectangle((int)buildingtexturesize.X, 0, (int)buildingtexturesize.X, (int)buildingtexturesize.Y);
				spritebatch.Draw(destinationRectangle: new Rectangle((int)buildings[i].Position.X, (int)buildings[i].Position.Y, (int)buildings[i].Size.X, (int)buildings[i].Size.Y), texture: buildingtexture, sourceRectangle: value, color: Color.White);
			}
		}
	}

	private void DrawExplosions(SpriteBatch spritebatch)
	{
		Rectangle value = new Rectangle(0, 0, 150, 150);
		for (int i = 0; i < explosionmanager.Explosions.Count(); i++)
		{
			if (explosionmanager.Explosions[i].Active)
			{
				spritebatch.Draw(destinationRectangle: new Rectangle((int)(explosionmanager.Explosions[i].Position.X - (float)(int)((float)(explosionmanager.Explosions[i].Size * 2) * 1.17f / 2f)), (int)(explosionmanager.Explosions[i].Position.Y - (float)(int)((float)(explosionmanager.Explosions[i].Size * 2) * 1.17f / 2f)), (int)((float)(explosionmanager.Explosions[i].Size * 2) * 1.17f), (int)((float)(explosionmanager.Explosions[i].Size * 2) * 1.17f)), texture: explosiontexture, sourceRectangle: value, color: Color.White);
			}
		}
		for (int j = 0; j < explosionmanager.GraphicExplosions.Count(); j++)
		{
			if (explosionmanager.GraphicExplosions[j].Active)
			{
				spritebatch.Draw(destinationRectangle: new Rectangle((int)explosionmanager.GraphicExplosions[j].Position.X, (int)explosionmanager.GraphicExplosions[j].Position.Y, (int)explosionmanager.GraphicExplosions[j].Size.X, (int)explosionmanager.GraphicExplosions[j].Size.Y), sourceRectangle: new Rectangle(256 * explosionmanager.GraphicExplosions[j].CurrentFrame, 0, 256, 256), texture: graphicexplosiontexture, color: Color.White);
			}
		}
	}

	public bool Update(int timems)
	{
		switch (gamestate)
		{
		case 0:
		{
			CheckRoundComplete(timems);
			if (inputdelay > 0)
			{
				inputdelay -= timems;
			}
			else
			{
				for (int i = 0; i < players.Count(); i++)
				{
					switch (players[i].Update(timems, missilemanager, projectilemanager))
					{
					case 1:
						gamestate = 5;
						inputdelay = 1000;
						break;
					}
				}
			}
			for (int j = 0; j < buildings.Count(); j++)
			{
				buildings[j].Update(timems, players[0]);
			}
			missilemanager.Update(timems, explosionmanager, players);
			explosionmanager.Update(timems);
			aimanager.Update(timems, missilemanager, buildings, projectilemanager, audiomanager, explosionmanager);
			projectilemanager.Update(timems, explosionmanager);
			break;
		}
		case 2:
			roundtimeout -= timems;
			if (roundtimeout < 0)
			{
				gamestate = 3;
				SaveGame();
			}
			break;
		case 3:
			return false;
		case 4:
			if (inputdelay > 0)
			{
				inputdelay -= timems;
			}
			else
			{
				CheckUpgradesInput(timems);
			}
			break;
		case 5:
			if (inputdelay > 0)
			{
				inputdelay -= timems;
			}
			else
			{
				CheckPauseInput(timems);
			}
			break;
		}
		return true;
	}

	private void CheckPauseInput(int timems)
	{
		GamePadState state = GamePad.GetState(players[0].ControllerIndex);
		KeyboardState state2 = Keyboard.GetState();
		if (state.IsButtonDown(Buttons.A) | state2.IsKeyDown(Keys.Enter))
		{
			gamestate = 0;
			inputdelay = 250;
		}
		else if (state.IsButtonDown(Buttons.B) | state2.IsKeyDown(Keys.Escape))
		{
			gamestate = 3;
			SaveGame();
			inputdelay = 250;
		}
		else if (players.Count() > 1)
		{
			state = GamePad.GetState(players[1].ControllerIndex);
			if (state.IsButtonDown(Buttons.A))
			{
				gamestate = 0;
				inputdelay = 250;
			}
			else if (state.IsButtonDown(Buttons.B))
			{
				gamestate = 3;
				SaveGame();
				inputdelay = 250;
			}
		}
	}

	private void DrawGameOver(SpriteBatch spritebatch)
	{
		Vector2 vector = notificationfont.MeasureString("Game Over");
		spritebatch.DrawString(gamefont, "Game Over", new Vector2(640f - vector.X / 2f, 250f), Color.Red);
	}

	private void DrawSaucers(SpriteBatch spritebatch)
	{
		Rectangle value = new Rectangle(0, 0, 64, 64);
		for (int i = 0; i < aimanager.missilesaucers.Count(); i++)
		{
			if (aimanager.missilesaucers[i].Active)
			{
				spritebatch.Draw(destinationRectangle: new Rectangle((int)aimanager.missilesaucers[i].Position.X, (int)aimanager.missilesaucers[i].Position.Y, 50, 50), texture: saucertexture, sourceRectangle: value, color: Color.White);
			}
		}
	}

	private void DrawAsteroids(SpriteBatch spritebatch)
	{
		for (int i = 0; i < aimanager.Asteroids.Count(); i++)
		{
			if (aimanager.Asteroids[i].Active)
			{
				Rectangle value = new Rectangle(aimanager.Asteroids[i].SpriteIndex * 128, 0, 128, 128);
				Vector2 size = aimanager.Asteroids[i].Size;
				spritebatch.Draw(destinationRectangle: new Rectangle((int)(aimanager.Asteroids[i].Position.X + size.X / 2f), (int)(aimanager.Asteroids[i].Position.Y + size.Y / 2f), (int)size.X, (int)size.Y), texture: boulderstexture, sourceRectangle: value, color: Color.White, rotation: MathHelper.ToRadians(aimanager.Asteroids[i].Angle), origin: new Vector2(64f, 64f), effects: SpriteEffects.None, layerDepth: 0f);
			}
		}
	}

	public void DrawHud(SpriteBatch spritebatch)
	{
		if (gamestate == 5)
		{
			return;
		}
		Rectangle destinationRectangle = new Rectangle(10, 10, 280, 150);
		Rectangle value = new Rectangle(0, 0, 5, 5);
		spritebatch.Draw(pausedtexture, destinationRectangle, value, Color.White);
		spritebatch.DrawString(gamefont, FileAndGamerServices.gamer.PlayerNames[(int)players[0].ControllerIndex], new Vector2(20f, 20f), players[0].PlayerColor);
		spritebatch.DrawString(gamefont, "Score: " + players[0].Score, new Vector2(20f, 50f), players[0].PlayerColor);
		spritebatch.DrawString(gamefont, "Power: " + players[0].Power, new Vector2(20f, 80f), players[0].PlayerColor);
		if (!players[0].UpgradeScore.MaxedOut)
		{
			spritebatch.DrawString(gamefont, "Upgrade Points: " + players[0].UpgradeScore.AvailableUpgrades, new Vector2(20f, 110f), players[0].PlayerColor);
		}
		else
		{
			spritebatch.DrawString(gamefont, "Upgrade Points: Max", new Vector2(20f, 110f), players[0].PlayerColor);
		}
		if (players.Count() > 1)
		{
			spritebatch.Draw(destinationRectangle: new Rectangle(990, 10, 280, 150), texture: pausedtexture, sourceRectangle: value, color: Color.White);
			spritebatch.DrawString(gamefont, FileAndGamerServices.gamer.PlayerNames[(int)players[1].ControllerIndex], new Vector2(1010f, 20f), players[1].PlayerColor);
			spritebatch.DrawString(gamefont, "Score: " + players[1].Score, new Vector2(1010f, 50f), players[1].PlayerColor);
			spritebatch.DrawString(gamefont, "Power: " + players[1].Power, new Vector2(1010f, 80f), players[1].PlayerColor);
			if (!players[1].UpgradeScore.MaxedOut)
			{
				spritebatch.DrawString(gamefont, "Upgrade Points: " + players[1].UpgradeScore.AvailableUpgrades, new Vector2(1010f, 110f), players[1].PlayerColor);
			}
			else
			{
				spritebatch.DrawString(gamefont, "Upgrade Points: Max", new Vector2(1010f, 110f), players[1].PlayerColor);
			}
		}
	}

	public void ShowPurchaseMenu(SpriteBatch spritebatch)
	{
		if (players.Count() == 1)
		{
			DrawPlayerUpgrades(spritebatch, new Vector2(320f, 0f), 0, purchaseplayerone);
			return;
		}
		DrawPlayerUpgrades(spritebatch, new Vector2(0f, 0f), 0, purchaseplayerone);
		DrawPlayerUpgrades(spritebatch, new Vector2(640f, 0f), 1, purchaseplayertwo);
	}

	private void DrawPlayerUpgrades(SpriteBatch spritebatch, Vector2 dest, int playerindex, int selectedindex)
	{
		Rectangle value = new Rectangle(0, 0, 640, 720);
		spritebatch.Draw(destinationRectangle: new Rectangle((int)dest.X, (int)dest.Y, 640, 720), texture: purchasetexture, sourceRectangle: value, color: Color.White);
		DrawUpgradeItem(spritebatch, selectedindex == 0, new Vector2(dest.X + 60f, dest.Y + 170f), players[playerindex].Upgrades.MissileSpeedUnlockCount, "Missile Speed");
		DrawUpgradeItem(spritebatch, selectedindex == 1, new Vector2(dest.X + 60f, dest.Y + 255f), players[playerindex].Upgrades.MissileROFUnlockCount, "Missile Rate Of Fire");
		DrawUpgradeItem(spritebatch, selectedindex == 2, new Vector2(dest.X + 60f, dest.Y + 340f), players[playerindex].Upgrades.BlasterSpeedUnlockCount, "Blaster Projectile Speed");
		DrawUpgradeItem(spritebatch, selectedindex == 3, new Vector2(dest.X + 60f, dest.Y + 425f), players[playerindex].Upgrades.BlasterROFUnlockCount, "Blaster Rate Of Fire");
		DrawUpgradeItem(spritebatch, selectedindex == 4, new Vector2(dest.X + 60f, dest.Y + 510f), players[playerindex].Upgrades.PowerUpgradeUnlockCount, "Power Upgrade");
		if (players[playerindex].UpgradeScore.AvailableUpgrades > 0)
		{
			value = new Rectangle(0, 0, 350, 31);
			spritebatch.Draw(destinationRectangle: new Rectangle((int)dest.X + 50, (int)dest.Y + 160 + selectedindex * 85, 550, 50), texture: purchasebordertexture, sourceRectangle: value, color: Color.Green);
		}
		spritebatch.DrawString(gamefont, players[playerindex].UpgradeScore.AvailableUpgrades.ToString(), new Vector2(dest.X + 530f, dest.Y + 640f), Color.Green);
	}

	private void DrawUpgradeItem(SpriteBatch spritebatch, bool selected, Vector2 pos, int itemcount, string itemname)
	{
		spritebatch.DrawString(gamefont, itemname, new Vector2(pos.X, pos.Y - 40f), Color.Yellow);
		Rectangle value = new Rectangle(0, 0, 64, 64);
		for (int i = 0; i < 10; i++)
		{
			if (i < itemcount)
			{
				spritebatch.Draw(destinationRectangle: new Rectangle((int)pos.X + i * 55, (int)pos.Y, 32, 32), texture: purchaseselected, sourceRectangle: value, color: Color.Green);
				continue;
			}
			spritebatch.Draw(destinationRectangle: new Rectangle((int)pos.X + i * 55, (int)pos.Y, 32, 32), texture: purchaseselected, sourceRectangle: value, color: Color.Red);
		}
	}

	private void CheckUpgradesInput(int timems)
	{
		KeyboardState state = Keyboard.GetState();
		if (players[0].UpgradeScore.AvailableUpgrades == 0)
		{
			if (players.Count() > 1)
			{
				if (players[1].UpgradeScore.AvailableUpgrades == 0)
				{
					gamestate = 0;
					playeroneinputdelay = 500;
					playeroneinputdelay = 500;
				}
			}
			else
			{
				gamestate = 0;
				playeroneinputdelay = 500;
				playeroneinputdelay = 500;
			}
		}
		GamePadState state2;
		if (players[0].UpgradeScore.AvailableUpgrades > 0)
		{
			playeroneinputdelay -= timems;
			if (playeroneinputdelay < 0)
			{
				playeroneinputdelay = 0;
				state2 = GamePad.GetState(players[0].ControllerIndex);
				if (state2.ThumbSticks.Left.Y > 0.2f || state2.ThumbSticks.Right.Y > 0.2f || state2.IsButtonDown(Buttons.DPadUp) || state.IsKeyDown(Keys.Up))
				{
					purchaseplayerone--;
					if (purchaseplayerone < 0)
					{
						purchaseplayerone = 0;
					}
					playeroneinputdelay = 200;
				}
				else if (state2.ThumbSticks.Left.Y < -0.2f || state2.ThumbSticks.Right.Y < -0.2f || state2.IsButtonDown(Buttons.DPadDown) || state.IsKeyDown(Keys.Down))
				{
					purchaseplayerone++;
					if (purchaseplayerone > 4)
					{
						purchaseplayerone = 4;
					}
					playeroneinputdelay = 200;
				}
				if (state2.IsButtonDown(Buttons.A) || state.IsKeyDown(Keys.Enter))
				{
					playeroneinputdelay = 200;
					switch (purchaseplayerone)
					{
					case 0:
						if (players[0].Upgrades.UpgradeMissileSpeed())
						{
							players[0].UpgradeScore.AvailableUpgrades--;
						}
						break;
					case 1:
						if (players[0].Upgrades.UpgradeMissileROF())
						{
							players[0].UpgradeScore.AvailableUpgrades--;
						}
						break;
					case 2:
						if (players[0].Upgrades.UpgradeBlasterSpeed())
						{
							players[0].UpgradeScore.AvailableUpgrades--;
						}
						break;
					case 3:
						if (players[0].Upgrades.UpgradeBlasterROF())
						{
							players[0].UpgradeScore.AvailableUpgrades--;
						}
						break;
					case 4:
						if (players[0].Upgrades.UpgradePowerGrid())
						{
							players[0].UpgradeScore.AvailableUpgrades--;
						}
						break;
					}
				}
			}
		}
		if (players.Count() <= 1)
		{
			return;
		}
		playertwoinputdelay -= timems;
		if (playertwoinputdelay >= 0)
		{
			return;
		}
		playertwoinputdelay = 0;
		if (players[1].UpgradeScore.AvailableUpgrades <= 0)
		{
			return;
		}
		state2 = GamePad.GetState(players[1].ControllerIndex);
		if (state2.ThumbSticks.Left.Y > 0.2f || state2.ThumbSticks.Right.Y > 0.2f || state2.IsButtonDown(Buttons.DPadUp))
		{
			purchaseplayertwo--;
			if (purchaseplayertwo < 0)
			{
				purchaseplayertwo = 0;
			}
			playertwoinputdelay = 200;
		}
		else if (state2.ThumbSticks.Left.Y < -0.2f || state2.ThumbSticks.Right.Y < -0.2f || state2.IsButtonDown(Buttons.DPadDown))
		{
			purchaseplayertwo++;
			if (purchaseplayertwo > 4)
			{
				purchaseplayertwo = 4;
			}
			playertwoinputdelay = 200;
		}
		if (!state2.IsButtonDown(Buttons.A))
		{
			return;
		}
		playertwoinputdelay = 200;
		switch (purchaseplayertwo)
		{
		case 0:
			if (players[1].Upgrades.UpgradeMissileSpeed())
			{
				players[1].UpgradeScore.AvailableUpgrades--;
			}
			break;
		case 1:
			if (players[1].Upgrades.UpgradeMissileROF())
			{
				players[1].UpgradeScore.AvailableUpgrades--;
			}
			break;
		case 2:
			if (players[1].Upgrades.UpgradeBlasterSpeed())
			{
				players[1].UpgradeScore.AvailableUpgrades--;
			}
			break;
		case 3:
			if (players[1].Upgrades.UpgradeBlasterROF())
			{
				players[1].UpgradeScore.AvailableUpgrades--;
			}
			break;
		case 4:
			if (players[1].Upgrades.UpgradePowerGrid())
			{
				players[1].UpgradeScore.AvailableUpgrades--;
			}
			break;
		}
	}

	private void SaveGame()
	{
		switch (aimanager.Difficulty)
		{
		case 1:
		{
			for (int j = 0; j < players.Length; j++)
			{
				players[j].Score = (int)((float)players[j].Score * 1.5f);
			}
			break;
		}
		case 2:
		{
			for (int i = 0; i < players.Length; i++)
			{
				players[i].Score = (int)((float)players[i].Score * 2f);
			}
			break;
		}
		}
		if (players.Length > 1)
		{
			int score = players[0].Score + players[1].Score;
			FileAndGamerServices.savegames.MultiplayerScores.AddNewItem(FileAndGamerServices.gamer.PlayerNames[(int)players[0].ControllerIndex], FileAndGamerServices.gamer.PlayerNames[(int)players[1].ControllerIndex], score, aimanager.RoundNumber);
		}
		else
		{
			FileAndGamerServices.savegames.SinglePlayerScores.AddNewItem(FileAndGamerServices.gamer.PlayerNames[(int)players[0].ControllerIndex], players[0].Score, aimanager.RoundNumber);
		}
		FileAndGamerServices.savegames.SaveFiles();
	}
}
