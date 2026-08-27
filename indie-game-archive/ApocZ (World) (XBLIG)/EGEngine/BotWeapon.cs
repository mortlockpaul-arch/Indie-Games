using System;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace EGEngine;

public class BotWeapon
{
	public static int DifficultyLevel = 0;

	public static float PinnedDownAccuracey = 1f;

	public bool FireWeapon;

	public Vector3 WeaponPosition = Vector3.Zero;

	public Matrix[] WeaponMatrix;

	public WeaponType CurrentWeaponType = WeaponType.FiftyCal;

	public int BulletsInMag;

	private float DamageTimer;

	private BaseData Owner;

	private Cue fireSND0;

	private FPSWeaponBase fpsWeapon = new FPSWeaponBase();

	public bool shotFired;

	private static int laserlightflag = 0;

	private static Color tmpLaserColor = Color.Violet;

	private static Vector3 DumbDir = Vector3.Zero;

	private static Vector3 tmpFireDirection = Vector3.Zero;

	private static IntersectSegmentParams segParams = default(IntersectSegmentParams);

	public WeaponClass CurrentWeapon
	{
		get
		{
			return fpsWeapon.CurrentWeapon;
		}
		set
		{
		}
	}

	public BotWeapon(BaseData e)
	{
		Owner = e;
		fpsWeapon.LoadContent(0);
		fpsWeapon.Owner = null;
		fpsWeapon.SetWeapon(WeaponType.AlienPistol);
		fireSND0 = EndGameEngine.SoundBnk.GetCue(fpsWeapon.CurrentWeapon.WeaponShotSound0);
		WeaponMatrix = new Matrix[2];
		ref Matrix reference = ref WeaponMatrix[0];
		reference = Matrix.Identity;
		ref Matrix reference2 = ref WeaponMatrix[1];
		reference2 = Matrix.Identity;
	}

	public void UpdateWeapon(float eTimeMS, int qIndex)
	{
		shotFired = false;
		WeaponPosition = WeaponMatrix[qIndex].Translation;
		if (fpsWeapon.CurWeaponType != CurrentWeaponType)
		{
			fpsWeapon.SetWeapon(CurrentWeaponType);
		}
		fpsWeapon.FireRate = fpsWeapon.CurrentWeapon.FireRate;
		fpsWeapon.FireTimer -= eTimeMS;
		DamageTimer -= eTimeMS;
		if (fpsWeapon.FireTimer < 0f)
		{
			Owner.foutValueDirection = 1f;
		}
		if (FireWeapon && BulletsInMag > 0 && fpsWeapon.FireTimer < 0f)
		{
			shotFired = true;
			BulletsInMag--;
			if (Math.Abs(fpsWeapon.FireTimer) >= fpsWeapon.FireRate)
			{
				fpsWeapon.FireTimer = fpsWeapon.FireRate;
			}
			else
			{
				fpsWeapon.FireTimer = fpsWeapon.FireRate - Math.Abs(fpsWeapon.FireTimer);
			}
		}
		if (shotFired)
		{
			Owner.foutValueDirection = -1f;
			float num = 100f * Owner.DistanceScalar * PinnedDownAccuracey;
			tmpFireDirection = Owner.TargetDirection;
			tmpFireDirection.X += ((float)BaseData.RandGenerator.NextDouble() - 0.5f) * num;
			tmpFireDirection.Y += ((float)BaseData.RandGenerator.NextDouble() - 0.5f) * num;
			tmpFireDirection.Z += ((float)BaseData.RandGenerator.NextDouble() - 0.5f) * num;
			if (CurrentWeaponType == WeaponType.Shotgun)
			{
				particles.SpawnMuzzleFlashShotty(ref WeaponPosition, ref tmpFireDirection, fps: false);
			}
			else
			{
				particles.SpawnMuzzleFlash2(ref WeaponPosition, fps: false);
			}
			particles.SpawnMuzzleSmoke(ref WeaponPosition, ref tmpFireDirection, fps: false);
			fpsWeapon.TriggerHeldDown = true;
			if (!fireSND0.IsDisposed)
			{
				fireSND0.Stop(AudioStopOptions.Immediate);
				fireSND0.Dispose();
			}
			fireSND0 = EndGameEngine.SoundBnk.GetCue(fpsWeapon.CurrentWeapon.WeaponShotSound0);
			float num2 = (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition - WeaponPosition).LengthSquared();
			num2 /= 500000f;
			fireSND0.Play();
			fireSND0.SetVariable("Distance", num2 * 10000f);
			segParams.OnlyWalkable = true;
			segParams.SegmentStart = WeaponPosition;
			segParams.SegmentDirection = tmpFireDirection;
			float num3 = segParams.SegmentDirection.Length();
			segParams.SegmentLength = num3 + 500f;
			segParams.SegmentDirection.Normalize();
			segParams.SegmentEnd = segParams.SegmentStart + segParams.SegmentDirection * segParams.SegmentLength;
			segParams.PreComputeParameters();
			tmpFireDirection = segParams.hitPosition - WeaponPosition;
			bool flag = true;
			if (CurrentWeaponType == WeaponType.AlienLMG || CurrentWeaponType == WeaponType.AlienPistol)
			{
				tmpLaserColor = Color.Yellow;
				tmpLaserColor.R = 200;
				tmpLaserColor.G = 200;
			}
			else if (CurrentWeaponType == WeaponType.AlienSMG || CurrentWeaponType == WeaponType.AlienSniper)
			{
				tmpLaserColor = Color.Blue;
				tmpLaserColor.R = 60;
				tmpLaserColor.G = 60;
			}
			else if (CurrentWeaponType == WeaponType.AlienShotty || CurrentWeaponType == WeaponType.AlienGrenader)
			{
				tmpLaserColor = Color.Orange;
			}
			else
			{
				flag = false;
				tmpLaserColor = Color.Yellow;
			}
			laserlightflag++;
			if (laserlightflag == 2)
			{
				laserlightflag = 0;
				if (flag)
				{
					particles.SpawnLaserTrialBoard(ref WeaponPosition, ref segParams.hitPosition, tmpLaserColor);
				}
				else
				{
					particles.SpawnTracerBullet(ref WeaponPosition, ref tmpFireDirection, fps: true);
				}
			}
			else if (flag)
			{
				particles.SpawnLaserTrialBoard(ref WeaponPosition, ref segParams.hitPosition, tmpLaserColor);
			}
			MaterialType materialType = LevelOutside.RayCast(0, ref segParams, spawnSparks: false);
			if (materialType == MaterialType.Undefined || segParams.hitDistance > num3)
			{
				if (Owner.TargetPlayer != null && Owner.TargetPlayer.IsValid)
				{
					int num4 = 0;
					tmpFireDirection.Normalize();
					DamegePacketType damageType = DamegePacketType.Body;
					num4 = Owner.TargetPlayer.RayCast(WeaponPosition, ref tmpFireDirection, ref damageType, 1f);
					if (num4 > 0)
					{
						if (damageType == DamegePacketType.HeadShot)
						{
							num4 = 30;
						}
						if (DamageTimer < 0f)
						{
							DamageTimer = 2f;
							for (int i = 0; i < 16; i++)
							{
								if (AIStateMachine.HitIndicatorArray[i].AlphaTimer <= 0f)
								{
									Vector3 vector = tmpFireDirection * -100f;
									vector.Y = 0f;
									vector.Normalize();
									Vector3 vector2 = Owner.TargetPlayer.vecFlatDirection;
									vector2.Y = 0f;
									Vector3 vector3 = Vector3.Zero;
									vector3.X = 0f;
									vector3.Y = 1f;
									vector3.Z = 0f;
									Vector3.Cross(ref vector2, ref vector3, out vector3);
									float result = 0f;
									Vector3.Dot(ref vector2, ref vector, out result);
									result = (float)Math.Acos(result);
									if (Vector3.Dot(vector3, vector) < 0f)
									{
										result *= -1f;
									}
									AIStateMachine.HitIndicatorArray[i].DirectionAngle = result;
									AIStateMachine.HitIndicatorArray[i].AlphaTimer = 1f;
									break;
								}
							}
							float num5 = 0.6f;
							if (DifficultyLevel == 1)
							{
								num5 = 0.8f;
							}
							if (DifficultyLevel == 2)
							{
								num5 = 1f;
							}
							Owner.TargetPlayer.Health -= (int)((float)num4 * num5);
							if (Owner.TargetPlayer.Health <= 0f)
							{
								Owner.TargetPlayer.ProcessDeath(DamegePacketType.None, ref DumbDir);
							}
						}
					}
				}
			}
			else if (materialType == MaterialType.Metal)
			{
				particles.SpawnBulletHitMetal(ref segParams.hitPosition, ref segParams.hitNormal);
			}
			else
			{
				particles.SpawnBulletHitRock(ref segParams.hitPosition, ref segParams.hitNormal);
			}
		}
		if (fpsWeapon.CurrentWeapon.fireMode == WeaponFireMode.Auto && !FireWeapon && !fireSND0.IsDisposed)
		{
			fpsWeapon.TriggerHeldDown = false;
		}
	}

	public bool IsShooting()
	{
		if (!fireSND0.IsPlaying || fireSND0.IsDisposed)
		{
			return false;
		}
		return true;
	}

	public void Reset()
	{
		BulletsInMag = fpsWeapon.CurrentWeapon.BulletsMagMax;
		if (!fireSND0.IsDisposed)
		{
			fireSND0.Stop(AudioStopOptions.Immediate);
			fireSND0.Dispose();
		}
	}

	public void Draw(int qIndex, PlayerBase playerRef, ref Matrix transform, Vector2 muzzleHeat)
	{
		fpsWeapon.DrawPlayerWeapon(qIndex, playerRef, playerRef, WeaponMatrix[qIndex], muzzleHeat);
	}

	public void DrawMuzzleFlash(int qIndex, PlayerBase playerRef, float muzzleHeat)
	{
		fpsWeapon.DrawPlayerWeaponMF(qIndex, playerRef, playerRef, WeaponMatrix[qIndex], muzzleHeat);
	}

	public void Reload()
	{
		BulletsInMag = fpsWeapon.CurrentWeapon.BulletsMagMax;
	}
}
