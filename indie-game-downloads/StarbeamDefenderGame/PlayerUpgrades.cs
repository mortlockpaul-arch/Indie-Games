namespace StarbeamDefenderGame;

public class PlayerUpgrades
{
	private int missilespeed;

	private int missilerof;

	private int blasterspeed;

	private int blasterrof;

	private int power;

	public float MissileSpeedMod => 1f + 0.2f * (float)(missilespeed - 1);

	public float MissileROFMod => 1f + 0.2f * (float)(missilerof - 1);

	public float BlasterSpeedMod => 1f + 0.2f * (float)(blasterspeed - 1);

	public float BlasterROFMod => 1f + 0.2f * (float)(blasterrof - 1);

	public float PowerUpgradeMod => 1f + 0.2f * (float)(power - 1);

	public int MissileSpeedUnlockCount => missilespeed;

	public int MissileROFUnlockCount => missilerof;

	public int BlasterSpeedUnlockCount => blasterspeed;

	public int BlasterROFUnlockCount => blasterrof;

	public int PowerUpgradeUnlockCount => power;

	public PlayerUpgrades()
	{
		missilerof = 1;
		missilespeed = 1;
		blasterrof = 1;
		blasterspeed = 1;
		power = 1;
	}

	public bool UpgradeMissileSpeed()
	{
		if (missilespeed > 9)
		{
			return false;
		}
		missilespeed++;
		return true;
	}

	public bool UpgradeMissileROF()
	{
		if (missilerof > 9)
		{
			return false;
		}
		missilerof++;
		return true;
	}

	public bool UpgradeBlasterSpeed()
	{
		if (blasterspeed > 9)
		{
			return false;
		}
		blasterspeed++;
		return true;
	}

	public bool UpgradeBlasterROF()
	{
		if (blasterrof > 9)
		{
			return false;
		}
		blasterrof++;
		return true;
	}

	public bool UpgradePowerGrid()
	{
		if (power > 9)
		{
			return false;
		}
		power++;
		return true;
	}
}
