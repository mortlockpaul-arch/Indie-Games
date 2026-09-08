using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace StarbeamDefenderGame;

public class Turret
{
	private PlayerIndex player;

	private Vector2 position;

	private Color color;

	private Vector2 cursorposition;

	private int missiledelay;

	private int cannondelay;

	private int score;

	private float currentpower;

	private float maxpower;

	private PlayerUpgrades upgrades;

	private UpgradePoints upgradepoints;

	public PlayerUpgrades Upgrades => upgrades;

	public UpgradePoints UpgradeScore => upgradepoints;

	public string Power => (int)(100f / maxpower * currentpower) + "%";

	public int Score
	{
		get
		{
			return score;
		}
		set
		{
			if (value > score && upgradepoints.IncrementScore(value - score))
			{
				if (upgrades.BlasterROFUnlockCount < 10)
				{
					while (upgrades.BlasterROFUnlockCount < 10)
					{
						upgrades.UpgradeBlasterROF();
					}
				}
				if (upgrades.BlasterSpeedUnlockCount < 10)
				{
					while (upgrades.BlasterSpeedUnlockCount < 10)
					{
						upgrades.UpgradeBlasterSpeed();
					}
				}
				if (upgrades.MissileSpeedUnlockCount < 10)
				{
					while (upgrades.MissileSpeedUnlockCount < 10)
					{
						upgrades.UpgradeMissileSpeed();
					}
				}
				if (upgrades.MissileROFUnlockCount < 10)
				{
					while (upgrades.MissileROFUnlockCount < 10)
					{
						upgrades.UpgradeMissileROF();
					}
				}
				if (upgrades.PowerUpgradeUnlockCount < 10)
				{
					while (upgrades.PowerUpgradeUnlockCount < 10)
					{
						upgrades.UpgradePowerGrid();
					}
				}
				UpgradeScore.AvailableUpgrades = 0;
			}
			score = value;
		}
	}

	public Color PlayerColor => color;

	public PlayerIndex ControllerIndex => player;

	public Vector2 CursorPosition
	{
		get
		{
			return cursorposition;
		}
		set
		{
			cursorposition = value;
		}
	}

	public Vector2 Position => position;

	public Vector2 CannonPosition => position + new Vector2(0f, 10f);

	public Vector2 LauncherPosition => position + new Vector2(0f, 10f);

	public Turret(PlayerIndex controller, Vector2 position, Color playercolor)
	{
		this.position = position;
		player = controller;
		cursorposition = new Vector2(640f, 360f);
		color = playercolor;
		maxpower = 100f;
		currentpower = 100f;
		upgradepoints = new UpgradePoints();
		upgrades = new PlayerUpgrades();
	}

	public int Update(int timems, MissileManager missiles, ProjectileManager projectiles)
	{
		currentpower += 5f * upgrades.PowerUpgradeMod / 1000f * (float)timems;
		if (currentpower > maxpower)
		{
			currentpower = maxpower;
		}
		return CheckInput(timems, missiles, projectiles);
	}

	private int CheckInput(int timems, MissileManager missiles, ProjectileManager projectiles)
	{
		GamePadState state = GamePad.GetState(player);
		KeyboardState state2 = Keyboard.GetState();
		if (state.IsButtonDown(Buttons.Start) | state.IsButtonDown(Buttons.Back))
		{
			return 1;
		}
		if (!state.IsConnected)
		{
			return 1;
		}
		if (FileAndGamerServices.gamer.IsGuideVisible)
		{
			return 1;
		}
		if (ControllerIndex == PlayerIndex.One && state2.IsKeyDown(Keys.Escape))
		{
			return 1;
		}
		cursorposition.X += state.ThumbSticks.Left.X * 10f;
		cursorposition.Y -= state.ThumbSticks.Left.Y * 10f;
		cursorposition.X += state.ThumbSticks.Right.X * 10f;
		cursorposition.Y -= state.ThumbSticks.Right.Y * 10f;
		if (player == PlayerIndex.One)
		{
			if (state2.IsKeyDown(Keys.Left))
			{
				cursorposition.X -= 10f;
			}
			if (state2.IsKeyDown(Keys.Right))
			{
				cursorposition.X += 10f;
			}
			if (state2.IsKeyDown(Keys.Up))
			{
				cursorposition.Y -= 10f;
			}
			if (state2.IsKeyDown(Keys.Down))
			{
				cursorposition.Y += 10f;
			}
		}
		if (state.ThumbSticks.Left.X == 0f && state.ThumbSticks.Right.X == 0f && ((state.ThumbSticks.Left.Y == 0f) & (state.ThumbSticks.Right.Y == 0f)))
		{
			if (state.IsButtonDown(Buttons.DPadLeft))
			{
				cursorposition.X -= 10f;
			}
			if (state.IsButtonDown(Buttons.DPadRight))
			{
				cursorposition.X += 10f;
			}
			if (state.IsButtonDown(Buttons.DPadUp))
			{
				cursorposition.Y -= 10f;
			}
			if (state.IsButtonDown(Buttons.DPadDown))
			{
				cursorposition.Y += 10f;
			}
		}
		if (cursorposition.X < 32f)
		{
			cursorposition.X = 32f;
		}
		else if (cursorposition.X > 1248f)
		{
			cursorposition.X = 1248f;
		}
		if (cursorposition.Y < 32f)
		{
			cursorposition.Y = 32f;
		}
		else if (cursorposition.Y > 600f)
		{
			cursorposition.Y = 600f;
		}
		if (cannondelay > 0)
		{
			cannondelay -= timems;
			if (cannondelay < 1)
			{
				cannondelay = 0;
			}
		}
		else if (currentpower > 4f)
		{
			if (state.IsButtonDown(Buttons.B) | state.IsButtonDown(Buttons.RightTrigger))
			{
				projectiles.FirePlayerProjectile(CannonPosition, cursorposition, 5f * upgrades.BlasterSpeedMod, this);
				cannondelay = (int)(500f / upgrades.BlasterROFMod);
				currentpower -= 5f;
			}
			if (player == PlayerIndex.One && state2.IsKeyDown(Keys.B))
			{
				projectiles.FirePlayerProjectile(CannonPosition, cursorposition, 5f * Upgrades.BlasterSpeedMod, this);
				cannondelay = (int)(500f / Upgrades.BlasterROFMod);
				currentpower -= 5f;
			}
		}
		if (missiledelay > 0)
		{
			missiledelay -= timems;
			if (missiledelay < 1)
			{
				missiledelay = 0;
			}
		}
		else if (currentpower > 9f)
		{
			if (state.IsButtonDown(Buttons.A) | state.IsButtonDown(Buttons.LeftTrigger))
			{
				missiles.FirePlayerMissile(LauncherPosition, cursorposition, 50f, 10f * Upgrades.MissileSpeedMod, color, player);
				missiledelay = (int)(500f / upgrades.MissileROFMod);
				currentpower -= 10f;
			}
			if (player == PlayerIndex.One && state2.IsKeyDown(Keys.Space))
			{
				missiles.FirePlayerMissile(LauncherPosition, cursorposition, 50f, 20f * Upgrades.MissileSpeedMod, color, player);
				missiledelay = (int)(500f / upgrades.MissileROFMod);
				currentpower -= 10f;
			}
		}
		return 0;
	}
}
