using Microsoft.Xna.Framework;

namespace SpaceBlast;

internal static class GameConstants
{
	public const short BuildNumber = 1;

	public const float PlayfieldSizeX = 64000f;

	public const float PlayfieldSizeY = 50000f;

	public const float ShipSpeedAdjustment = 60f;

	public const int NumAsteroids = 0;

	public const float AsteroidMinSpeed = 100f;

	public const float AsteroidMaxSpeed = 300f;

	public const float AsteroidSpeedAdjustment = 0f;

	public const float AsteroidBoundingSphereScale = 0.95f;

	public const float ShipBoundingSphereScale = 0.5f;

	public const int NumBullets = 30;

	public const float BulletSpeedAdjustment = 60f;

	public const int StartingAmmo = 500;

	public const int AmmoPowerupAmmo = 400;

	public const int GunDamage = 15;

	public const int BlasterDamage = 30;

	public const float RearFireRateAdjust = 1.2f;

	public const int MegaDamagePUMultiplier = 3;

	public const float MegaDamagePUDuration = 45f;

	public const float RefuelPUFuel = 150f;

	public const int RepairPUAmount = 50;

	public const float AccelerationPUAmount = 1f;

	public const float TopSpeedPUAmount = 50f;

	public const float ShieldBoostPUAmount = 50f;

	public const float ShieldRegenPUAmount = 1f;

	public const float FireRatePUReductionFactor = 0.8f;

	public const float InvinciblityPUDuration = 30f;

	public const float CloakPUDuration = 30f;

	public const float ThrustFuelCost = 1f;

	public const int WorldSegmentWidth = 10000;

	public const int WorldSegmentHeight = 10000;

	public const float constBulletSpeed = 400f;

	public const float constBlasterSpeed = 600f;

	public static float CameraHeight;

	public static Vector3 Gravity;

	public static double PlayerLightChangeDuration;

	public static float constBulletLifetime;

	public static int KillsToWin;

	public static int TeamKillsToWin;

	public static int KillsToNearbyOverThreshold;

	static GameConstants()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		CameraHeight = 90000f;
		Gravity = new Vector3(0f, 0f, 0f);
		PlayerLightChangeDuration = 3.0;
		constBulletLifetime = 10f;
		KillsToWin = 10;
		TeamKillsToWin = 30;
		KillsToNearbyOverThreshold = 7;
	}
}
