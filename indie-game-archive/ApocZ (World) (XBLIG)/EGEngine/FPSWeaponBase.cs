using System;
using System.Collections.Generic;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using SkinnedModel;

namespace EGEngine;

public class FPSWeaponBase
{
	private static Random rand = new Random(3);

	public int NumberOfWeapons;

	public int NextWeaponType;

	public int NextWeaponIndex;

	public bool InSpecialOneOff;

	public bool DrawKnife;

	public bool SwitchingWeapon;

	public bool ReloadingWeapon;

	public bool InJump;

	public int ScopeMagnificationLevel;

	public bool TriggerHeldDown;

	public float Zoom = (float)Math.PI / 4f;

	public float BulletAccuracy = 1f;

	public float AttackWithKnifeTimer;

	public float AttackWithSwordTimer;

	public float FireTimer;

	public float FireRate = 5f / 66f;

	public float ParticleSpawnTimer;

	public int TracerBullet;

	public float YawSwayX;

	public float YawSwayY;

	public Vector3 vecHeadSway = Vector3.Zero;

	public Vector3 vecHeadSwayTarget = Vector3.Zero;

	public Vector4 curRecoil = Vector4.Zero;

	public Matrix matHeadTransform = Matrix.Identity;

	public Matrix[] matWeaponTransform = new Matrix[2];

	public Matrix[] matMagTransform = new Matrix[2];

	public Matrix[] matM203Transform = new Matrix[2];

	public Matrix[] matBoltTransform = new Matrix[2];

	public Matrix[] matWeaponSway = new Matrix[2];

	public Matrix[] matKnifeTransform = new Matrix[2];

	public static RPG RPGRockets = new RPG();

	public static Javlin JavlinRockets = new Javlin();

	public static List<WeaponClass> weapon = new List<WeaponClass>();

	public Model hands;

	public Texture2D diffuse;

	public Texture2D normal;

	public Animation fpsAmin = new Animation();

	public float WeaponZoom = (float)Math.PI / 4f;

	public Matrix[] WeaponProjection;

	public Vector3[] vecFPSLightColor = new Vector3[2];

	public Vector4[] vecFPSLightPosition = new Vector4[2];

	public Cue reloadSoundCue;

	public Cue fireSoundCue0;

	public Cue shotgunCockCue;

	public Cue knifeSoundCue;

	public Cue bulletImpactCue;

	public FPSScopeSightsBase ScopeSights = new FPSScopeSightsBase();

	public PlayerBase Owner;

	private static float SHOTGUN_RANGE = 1960000f;

	private static float AK74u_RANGE = 1960000f;

	private static Vector3 VecUnitX = Vector3.UnitX;

	private static Vector3 VecUnitY = Vector3.UnitY;

	private static Vector3 VecUnitZ = Vector3.UnitZ;

	public static float M203OffsetTimer;

	public static Vector3 M203Offset;

	public static Vector3 M203OffsetTarget;

	public static Model M203Nader;

	public static Matrix[] M203NaderTransforms;

	private bool cansightweaponoverride;

	private int currentWeaponIndex;

	private WeaponType currentWeaponType;

	private static bool IsInitialized = false;

	private static List<WeaponData> tmpWepData = new List<WeaponData>();

	private static float truhbrtgh = 10f;

	public bool HackReloadBlendoutReached;

	public static bool WeaponBulletTest = false;

	private static Vector4 TextureOffset = Vector4.Zero;

	private static Cue[] tmpSndCue = new Cue[5];

	private static Matrix tmpPlayerScale = Matrix.CreateScale(0.7f);

	private static Matrix tmpWeaponScale = Matrix.CreateScale(0.75f);

	private static Vector3 weaponHeightFix = new Vector3(0f, 16f, 0f);

	private static Vector3 vecAKOffset = new Vector3(0f, 3f, 0f);

	private static Vector3 wepOffset = Vector3.Zero;

	private static Vector3 tmpMuzzleSpawnPos = Vector3.Zero;

	public static int NumBulletFired = 0;

	private static Matrix tmpMatSway = Matrix.Identity;

	private static Vector3 tmpVecSway = Vector3.Zero;

	private static Matrix tmpHeadJump = Matrix.Identity;

	private static Vector3 fireDirection = Vector3.Zero;

	private static Vector3 patricledir = Vector3.Zero;

	private static Vector3 hitPos = Vector3.Zero;

	private static Vector3 hitNorm = Vector3.Zero;

	private static Vector3 tmpOriginPos = Vector3.Zero;

	private static Matrix tmpMatWorld = Matrix.Identity;

	private static Matrix tmpLeftHandNader = Matrix.Identity;

	private static Vector2 tmpSpecialSway = Vector2.Zero;

	private static Vector3 tmpVertice = Vector3.Zero;

	private static Matrix tmpMatrice = Matrix.Identity;

	private static MediaStruct tmpMediaStruct = default(MediaStruct);

	private float HackNoShootTimer;

	private static Vector3 JumpWepOffset = new Vector3(0f, 15.8f, 0f);

	private static bool tmpHitInWorld = false;

	private static MaterialType tmpHitMaterial = MaterialType.Undefined;

	private static Vector3 tmpVelocity = Vector3.Zero;

	private static IntersectSegmentParams tmpSegmentParams = default(IntersectSegmentParams);

	private static ModelMesh drawMesh;

	private static ModelMeshPart drawMeshPart;

	private static Matrix drawTexProj = Matrix.Identity;

	private static Matrix tmpSight = Matrix.Identity;

	private static Matrix drawRearSight = Matrix.CreateRotationX(MathHelper.ToRadians(-90f));

	private static Matrix drawFrontSight = Matrix.CreateRotationX(MathHelper.ToRadians(90f));

	private static Effect drawEffect;

	private static Matrix tmpDrawMat = Matrix.Identity;

	private static Vector3 tmpDrawDir = Vector3.Zero;

	private Vector3 eyePosition = Vector3.Zero;

	private Vector3 tmpUDPlrPos = Vector3.Zero;

	private Vector3 tmpUDPlrDir = Vector3.UnitZ;

	private Matrix thisFrameMFmat = Matrix.Identity;

	public Matrix matMuzzleFlash = Matrix.Identity;

	private static int MuzzleHeatPartIndex = 0;

	private static Vector3 MHPartPos = Vector3.Zero;

	private static Matrix matMuzzleMesh = Matrix.Identity;

	public FPSAnimationState CurrentFPSAnimation
	{
		get
		{
			return fpsAmin.CurrentAnimationState;
		}
		set
		{
		}
	}

	public bool CanSightWeaponOverride
	{
		get
		{
			return cansightweaponoverride;
		}
		set
		{
			cansightweaponoverride = value;
		}
	}

	public int CurWeaponIndex => currentWeaponIndex;

	public WeaponType CurWeaponType
	{
		get
		{
			return currentWeaponType;
		}
		set
		{
			currentWeaponType = value;
			for (int i = 0; i < weapon.Count; i++)
			{
				if (currentWeaponType == weapon[i].WepType)
				{
					currentWeaponIndex = i;
					break;
				}
			}
		}
	}

	public WeaponClass CurrentWeapon => weapon[CurWeaponIndex];

	public float FOV
	{
		get
		{
			if (NumberOfWeapons == 0)
			{
				ScopeMagnificationLevel = 0;
				return (float)Math.PI / 4f;
			}
			if (fpsAmin.CurrentAnimationState.AnimType != AnimationType.Sights && (fpsAmin.CurrentAnimationState.AnimType != AnimationType.WeaponPutaway || fpsAmin.CurrentAnimationState.AnimType != AnimationType.WeaponPullout))
			{
				ScopeMagnificationLevel = 0;
				return (float)Math.PI / 4f;
			}
			if (CurrentWeapon.WepType == WeaponType.Vet16 || CurrentWeapon.WepType == WeaponType.TAC50 || CurrentWeapon.WepType == WeaponType.RSUSniper)
			{
				return (float)Math.PI / 12f;
			}
			if (CurrentWeapon.WepType == WeaponType.Sniper)
			{
				return (float)Math.PI / 16f;
			}
			if (CurrentWeapon.Attachment == WeaponAttachment.HoloGraphicSight)
			{
				ScopeMagnificationLevel = 0;
				return (float)Math.PI / 4f;
			}
			if (CurrentWeapon.Attachment == WeaponAttachment.SniperScope)
			{
				if (fpsAmin.CurrentAnimationState.AnimType == AnimationType.Sights)
				{
					if (ScopeMagnificationLevel == 0)
					{
						ScopeMagnificationLevel = 1;
					}
					if (ScopeMagnificationLevel == 1)
					{
						return (float)Math.PI / 6f;
					}
					if (ScopeMagnificationLevel == 2)
					{
						return (float)Math.PI / 18f;
					}
					if (ScopeMagnificationLevel == 3)
					{
						return -0.1f;
					}
					ScopeMagnificationLevel = 0;
					return (float)Math.PI / 4f;
				}
				ScopeMagnificationLevel = 0;
				return fpsAmin.CurrentAnimationState.FOV;
			}
			ScopeMagnificationLevel = 0;
			return fpsAmin.CurrentAnimationState.FOV;
		}
		set
		{
		}
	}

	public Vector3 HeadPosition
	{
		get
		{
			if (NumberOfWeapons == 0)
			{
				return Vector3.Zero;
			}
			return matHeadTransform.Translation;
		}
		set
		{
		}
	}

	public Vector3 HeadDirection
	{
		get
		{
			if (NumberOfWeapons == 0)
			{
				return Vector3.Zero;
			}
			return matHeadTransform.Down;
		}
		set
		{
		}
	}

	public Vector3 HeadUp
	{
		get
		{
			if (NumberOfWeapons == 0)
			{
				return Vector3.Zero;
			}
			return matHeadTransform.Backward;
		}
		set
		{
		}
	}

	public float RecoilUp
	{
		get
		{
			if (NumberOfWeapons == 0)
			{
				return 0f;
			}
			if (weapon[CurWeaponIndex].fireMode != WeaponFireMode.SemiAuto)
			{
				return curRecoil.Y;
			}
			return curRecoil.Y * 0.1f;
		}
		set
		{
		}
	}

	public float RecoilSide
	{
		get
		{
			if (NumberOfWeapons == 0)
			{
				return 0f;
			}
			if (weapon[CurWeaponIndex].fireMode != WeaponFireMode.SemiAuto)
			{
				return curRecoil.X;
			}
			return curRecoil.X * 0.25f;
		}
		set
		{
		}
	}

	public bool CanRun
	{
		get
		{
			if (!SwitchingWeapon)
			{
				return !ReloadingWeapon;
			}
			return false;
		}
		set
		{
		}
	}

	public bool IsAnimTypeOnStack(AnimationType e)
	{
		return fpsAmin.IsAnimTypeOnStack(e);
	}

	public WeaponClass GetWeaponReference(WeaponType e)
	{
		for (int i = 0; i < weapon.Count; i++)
		{
			if (e == weapon[i].WepType)
			{
				return weapon[i];
			}
		}
		return null;
	}

	public void ResetSpawn()
	{
		foreach (WeaponClass item in weapon)
		{
			item.ResetSpawn();
		}
		InSpecialOneOff = false;
		SwitchingWeapon = false;
		ReloadingWeapon = false;
	}

	public void ScopeMagnifyLevelUp()
	{
		ScopeMagnificationLevel++;
		if (ScopeMagnificationLevel > 3)
		{
			ScopeMagnificationLevel = 3;
		}
	}

	public void ScopeMagnifyLevelDown()
	{
		ScopeMagnificationLevel--;
		if (ScopeMagnificationLevel < 1)
		{
			ScopeMagnificationLevel = 1;
		}
	}

	public bool Jump()
	{
		vecHeadSwayTarget.X = -0.3f;
		fpsAmin.PlayAnimation(CurrentWeapon.JumpAnim, force: true, EndGameEngine.FIXED_TIME_STEP + EndGameEngine.FIXED_TIME_STEP);
		return true;
	}

	public bool Crouch()
	{
		vecHeadSwayTarget.X = 0.2f;
		return true;
	}

	public bool SwitchWeapon(WeaponType e)
	{
		int wepIndex = 0;
		return SwitchWeapon(e, ref wepIndex);
	}

	public bool SwitchWeapon(WeaponType e, ref int wepIndex)
	{
		if (NumberOfWeapons == 0 || fpsAmin.PlayingOneOff())
		{
			return false;
		}
		for (int i = 0; i < weapon.Count; i++)
		{
			if (e == weapon[i].WepType)
			{
				wepIndex = i;
				NextWeaponIndex = i;
				SwitchingWeapon = true;
				InSpecialOneOff = true;
				fpsAmin.AnimationBlendOut += fpsAmin_BlendOutReached;
				fpsAmin.PlayAnimation(weapon[CurWeaponIndex].PutawayAnim, force: true, EndGameEngine.FIXED_TIME_STEP + EndGameEngine.FIXED_TIME_STEP);
				if (Owner != null)
				{
					Owner.Set3rdPersonSwitchWeapon(e);
				}
				return true;
			}
		}
		return false;
	}

	public void SetWeapon(WeaponType e)
	{
		for (int i = 0; i < weapon.Count; i++)
		{
			if (e == weapon[i].WepType)
			{
				NextWeaponIndex = i;
				currentWeaponIndex = NextWeaponIndex;
				CurWeaponType = e;
				fpsAmin.SetBaseAnimation(weapon[CurWeaponIndex].IdleAnim);
				if (Owner != null)
				{
					Owner.Set3rdPersonBaseanim(weapon[CurWeaponIndex].CoOpIdleAnim);
				}
				break;
			}
		}
	}

	public bool ReloadWeapon()
	{
		if (PlayerBase.ApocalypseZ_Hack)
		{
			if (NumberOfWeapons == 0 || fpsAmin.PlayingOneOff())
			{
				return false;
			}
			ItemCls itemRef = null;
			ushort item = (ushort)WeaponsCls.GetAmmoType(CurrentWeapon.WepType);
			if (!AIBase.PlayerInventory.HaveItem(1024, item, ref itemRef))
			{
				return false;
			}
			itemRef.reserved0 = ((itemRef.reserved0 > (byte)CurrentWeapon.BulletsMagMax) ? ((byte)CurrentWeapon.BulletsMagMax) : itemRef.reserved0);
			if (CurrentWeapon.BulletsInMag >= CurrentWeapon.BulletsMagMax)
			{
				CurrentWeapon.BulletsInMag = CurrentWeapon.BulletsMagMax;
				return false;
			}
			if (CurrentWeapon.BulletsInMag + itemRef.reserved0 > CurrentWeapon.BulletsMagMax)
			{
				itemRef.reserved0 -= (byte)(CurrentWeapon.BulletsMagMax - CurrentWeapon.BulletsInMag);
				CurrentWeapon.BulletsTotal = CurrentWeapon.BulletsMagMax;
			}
			else
			{
				CurrentWeapon.BulletsTotal = CurrentWeapon.BulletsInMag + itemRef.reserved0;
				AIBase.PlayerInventory.DestroyItem(1024, itemRef);
			}
		}
		else if (NumberOfWeapons == 0 || fpsAmin.PlayingOneOff() || CurrentWeapon.BulletsTotal < 1 || CurrentWeapon.BulletsInMag == CurrentWeapon.BulletsMagMax || CurrentWeapon.WepType == WeaponType.BloeTorch || CurrentWeapon.WepType == WeaponType.BaitBomb)
		{
			return false;
		}
		ReloadingWeapon = true;
		InSpecialOneOff = true;
		fpsAmin.AnimationBlendIn += fpsAmin_BlendInReached;
		fpsAmin.AnimationEndReached += fpsAmin_EndAnimationReached;
		if (weapon[CurWeaponIndex].WepType != WeaponType.Shotgun)
		{
			fpsAmin.PlayAnimation(weapon[CurWeaponIndex].ReloadAnim, force: true, EndGameEngine.FIXED_TIME_STEP);
		}
		else
		{
			fpsAmin.QueueAnimation(weapon[CurWeaponIndex].ReloadAnim, force: true, EndGameEngine.FIXED_TIME_STEP);
		}
		if (reloadSoundCue != null)
		{
			reloadSoundCue.Stop(AudioStopOptions.Immediate);
			reloadSoundCue.Dispose();
		}
		reloadSoundCue = EndGameEngine.SoundBnk.GetCue(CurrentWeapon.WeaponReloadSound);
		reloadSoundCue.Play();
		if (weapon[CurWeaponIndex].WepType != WeaponType.Shotgun)
		{
			reloadSoundCue.SetVariable("Pitch", 50f + Owner.tmpCommandoSpeed * 50f);
		}
		else
		{
			reloadSoundCue.SetVariable("Pitch", 50f);
		}
		return true;
	}

	public bool PullKnife()
	{
		if (NumberOfWeapons == 0 || fpsAmin.PlayingOneOff())
		{
			return false;
		}
		InSpecialOneOff = true;
		fpsAmin.AnimationBlendIn += fpsAmin_BlendInReached;
		fpsAmin.PlayAnimation(WeaponAnim.IdleKnife, force: true, EndGameEngine.FIXED_TIME_STEP + (int)(Owner.tmpCommandoSpeed * (float)EndGameEngine.FIXED_TIME_STEP));
		ScheduleAttackWithKnife();
		return true;
	}

	private void ScheduleAttackWithKnife()
	{
		AttackWithKnifeTimer = 0.35f;
	}

	private void AttackWithKnife()
	{
	}

	public bool PullSword()
	{
		if (NumberOfWeapons == 0 || fpsAmin.PlayingOneOff())
		{
			return false;
		}
		if (currentWeaponType == WeaponType.RomanSword)
		{
			AttackWithSwordTimer = 0.35f;
			fpsAmin.PlayAnimation(WeaponAnim.RomanSwordAttack01, force: true, EndGameEngine.FIXED_TIME_STEP + (int)(Owner.tmpCommandoSpeed * (float)EndGameEngine.FIXED_TIME_STEP));
		}
		else
		{
			AttackWithSwordTimer = 0.55f;
			SwitchWeapon(WeaponType.RomanSword);
		}
		return true;
	}

	private void ScheduleAttackWithSword()
	{
		AttackWithSwordTimer = 0.35f;
	}

	private void AttackWithSword()
	{
		bool flag = false;
		flag = AIBase.PlayerAttackSword(0, ref Owner.vecPosition, ref Owner.vecDirection);
		if (knifeSoundCue != null)
		{
			knifeSoundCue.Stop(AudioStopOptions.Immediate);
			knifeSoundCue.Dispose();
		}
		if (flag)
		{
			knifeSoundCue = EndGameEngine.SoundBnk.GetCue("stabknife");
			Vector3 spawnPos = Owner.vecPosition;
			spawnPos.Y += 40f;
			spawnPos += Owner.vecDirection * 100f;
			Vector3 velocity = Vector3.Zero;
			particles.SpawnBulletHitMutant(ref spawnPos, ref velocity);
			ControllerBase.SetVibration(Owner.playerIndex, 0.2f, 0.2f, 0.8f, 0.5f);
		}
		else
		{
			knifeSoundCue = EndGameEngine.SoundBnk.GetCue("swishknife");
			ControllerBase.SetVibration(Owner.playerIndex, 0.2f, 0f, 0.2f, 0f);
		}
		knifeSoundCue.Play();
		knifeSoundCue.SetVariable("Pitch", 50f);
	}

	public Matrix GetBoneTransform(WeaponPart wepPart, Model e, ref Matrix[] t)
	{
		for (int i = 0; i < e.Meshes.Count; i++)
		{
			if (((WeaponPartStruct)e.Meshes[i].Tag).PartType == wepPart)
			{
				return t[e.Meshes[i].ParentBone.Index];
			}
		}
		return Matrix.Identity;
	}

	public virtual void LoadContent(int index)
	{
		if (!IsInitialized)
		{
			IsInitialized = true;
			tmpWepData = EndGameEngine.GameAssetMgr.Load<List<WeaponData>>("data\\WeaponDataXml");
			if (EndGameEngine.GameSettings.GameName != "_AvR_" && EndGameEngine.GameSettings.GameName != "ApocalypseZ" && EndGameEngine.GameSettings.GameName != "ToyPlane")
			{
				int num = 0;
				M203OffsetTimer = 0f;
				M203Offset = Vector3.Zero;
				M203OffsetTarget = Vector3.Zero;
				M203Nader = EndGameEngine.GameAssetMgr.Load<Model>("models\\weapons\\m203");
				M203NaderTransforms = new Matrix[M203Nader.Bones.Count];
				M203Nader.CopyAbsoluteBoneTransformsTo(M203NaderTransforms);
				foreach (ModelMesh mesh in M203Nader.Meshes)
				{
					mesh.Tag = SetWeaponPart(mesh.Name, num++);
					foreach (ModelMeshPart meshPart in mesh.MeshParts)
					{
						meshPart.Tag = new WeaponEffectParams(meshPart.Effect, null);
						((WeaponEffectParams)meshPart.Tag).fReflectiveness.SetValue(0.25f);
					}
				}
			}
			foreach (WeaponData tmpWepDatum in tmpWepData)
			{
				weapon.Add(new WeaponClass(tmpWepDatum));
			}
			foreach (WeaponClass item in weapon)
			{
				item.Set();
			}
			if (EndGameEngine.GameSettings.GameName == "_AvR_")
			{
				RPGRockets.Load("models\\props\\TurretRocket");
				JavlinRockets.Load("models\\props\\javlinmissle");
			}
			else if (EndGameEngine.GameSettings.GameName == "TowerDefense")
			{
				RPGRockets.Load("models\\props\\RPGGrenade");
				JavlinRockets.Load("models\\props\\javlinmissle");
			}
		}
		NumberOfWeapons = tmpWepData.Count;
		if (NumberOfWeapons == 0)
		{
			return;
		}
		WeaponProjection = new Matrix[2];
		for (int i = 0; i < 2; i++)
		{
			ref Matrix reference = ref WeaponProjection[i];
			reference = Matrix.Identity;
		}
		hands = PlayerBaseState.fpsHandsBase[0];
		fpsAmin.Initialize(hands, 0);
		fpsAmin.SetBaseAnimation(WeaponAnim.IdlePistol);
		if (PlayerBase.ApocalypseZ_Hack)
		{
			SetWeapon(WeaponType.Sniper);
			foreach (WeaponClass item2 in weapon)
			{
				item2.BulletsInMag = 0;
				item2.BulletsTotal = 0;
			}
		}
		else
		{
			SetWeapon(WeaponType.NineMil);
		}
		ScopeSights.LoadContent();
		fireSoundCue0 = EndGameEngine.SoundBnk.GetCue(weapon[0].WeaponShotSound0);
		FireRate = weapon[0].FireRate;
	}

	public void SetLocalFPSAnimationKeys()
	{
		fpsAmin.SetAnimationKeyEvent(WeaponAnim.ThrowKnife, 12, ThrowKnifeEvent);
	}

	public void KillSound()
	{
		if (!fireSoundCue0.IsDisposed)
		{
			fireSoundCue0.Stop(AudioStopOptions.Immediate);
			fireSoundCue0.Dispose();
		}
		RPGRockets.KillSound();
		JavlinRockets.KillSound();
	}

	private void fpsAmin_BlendInReached(object sender, AnimationEventArgs e)
	{
		if (e.CurrentAnimation == WeaponAnim.IdleKnife)
		{
			DrawKnife = true;
			e.NewHandler = (EventHandler<AnimationEventArgs>)Delegate.Combine(e.NewHandler, null);
			fpsAmin.AnimationBlendOut += fpsAmin_BlendOutReached;
		}
		else if (e.CurrentAnimation == WeaponAnim.NadeReload)
		{
			M203OffsetTimer = 0.25f;
			M203OffsetTarget.Y = -6f;
			M203OffsetTarget.Z = -1f;
			e.NewHandler = (EventHandler<AnimationEventArgs>)Delegate.Combine(e.NewHandler, null);
			fpsAmin.AnimationBlendOut += fpsAmin_BlendOutReached;
		}
		else if (e.CurrentAnimationType == AnimationType.Reload)
		{
			if (currentWeaponType == WeaponType.Grenader)
			{
				CurrentWeapon.Reload();
			}
			HackReloadBlendoutReached = false;
			e.NewHandler = (EventHandler<AnimationEventArgs>)Delegate.Combine(e.NewHandler, null);
			fpsAmin.AnimationBlendOut += fpsAmin_BlendOutReached;
		}
		else if (e.CurrentAnimation == WeaponAnim.ShotgunCock)
		{
			if (e.AnimationPlayerIndex == 2)
			{
				fpsAmin.SetBaseOneAnimation(WeaponAnim.Idle);
			}
		}
		else if (e.CurrentAnimation == WeaponAnim.ShotgunCockSighted && e.AnimationPlayerIndex == 2)
		{
			fpsAmin.SetBaseOneAnimation(WeaponAnim.Sights);
		}
	}

	private void fpsAmin_BlendOutReached(object sender, AnimationEventArgs e)
	{
		if (e.CurrentAnimationType == AnimationType.WeaponPutaway)
		{
			if (PlayerBase.AvR_Hack && AttackWithSwordTimer > 0f)
			{
				SwitchingWeapon = false;
				InSpecialOneOff = false;
				currentWeaponIndex = NextWeaponIndex;
				currentWeaponType = weapon[NextWeaponIndex].WepType;
				Owner.PrimaryWeapon = WeaponType.RomanSword;
				fpsAmin.QueueAnimation(WeaponAnim.RomanSwordAttack01, force: true, 332000);
				e.NewHandler = (EventHandler<AnimationEventArgs>)Delegate.Combine(e.NewHandler, null);
				fpsAmin.SetBaseAnimation(weapon[CurWeaponIndex].IdleAnim);
				return;
			}
			currentWeaponIndex = NextWeaponIndex;
			currentWeaponType = weapon[NextWeaponIndex].WepType;
			if (weapon[NextWeaponIndex].WepCategory == WeaponCategory.Pistol)
			{
				Owner.SetSecondaryWeapon(currentWeaponType);
			}
			else
			{
				Owner.SetPrimaryWeapon(currentWeaponType);
			}
			fpsAmin.QueueAnimation(WeaponAnim.WeaponPullout, force: true, 332000);
			e.NewHandler = (EventHandler<AnimationEventArgs>)Delegate.Combine(e.NewHandler, null);
			fpsAmin.AnimationEndReached += fpsAmin_EndAnimationReached;
			fpsAmin.SetBaseAnimation(weapon[CurWeaponIndex].IdleAnim);
		}
		else if (e.CurrentAnimationType == AnimationType.Idle && e.CurrentAnimation == WeaponAnim.IdleKnife)
		{
			DrawKnife = false;
			InSpecialOneOff = false;
			e.NewHandler = (EventHandler<AnimationEventArgs>)Delegate.Combine(e.NewHandler, null);
		}
		else if (e.CurrentAnimation == WeaponAnim.NadeReload)
		{
			M203OffsetTimer = 0.3f;
			M203OffsetTarget = Vector3.Zero;
			e.NewHandler = (EventHandler<AnimationEventArgs>)Delegate.Combine(e.NewHandler, null);
		}
		else
		{
			if (e.CurrentAnimationType != AnimationType.Reload)
			{
				return;
			}
			if (CurrentWeapon.WepType == WeaponType.Shotgun)
			{
				CurrentWeapon.BulletsTotal--;
				CurrentWeapon.BulletsInMag++;
				if (CurrentWeapon.BulletsTotal == 0 || CurrentWeapon.BulletsInMag == CurrentWeapon.MaxAmmoInClip)
				{
					ReloadingWeapon = false;
					InSpecialOneOff = false;
					e.NewHandler = (EventHandler<AnimationEventArgs>)Delegate.Combine(e.NewHandler, null);
					if (CurrentWeapon.BulletsInMag > 0)
					{
						fpsAmin.QueueAnimation(weapon[CurWeaponIndex].IdleAnim, force: true, EndGameEngine.FIXED_TIME_STEP);
						fpsAmin.QueueAnimation(WeaponAnim.ShotgunCock, force: true, EndGameEngine.FIXED_TIME_STEP);
						if (shotgunCockCue != null)
						{
							shotgunCockCue.Stop(AudioStopOptions.Immediate);
							shotgunCockCue.Dispose();
						}
						shotgunCockCue = EndGameEngine.SoundBnk.GetCue("ShotgunCock");
						shotgunCockCue.Play();
						shotgunCockCue.SetVariable("Pitch", 50f);
					}
				}
				else
				{
					e.NewHandler = (EventHandler<AnimationEventArgs>)Delegate.Combine(e.NewHandler, null);
					fpsAmin.AnimationBlendIn += fpsAmin_BlendInReached;
					fpsAmin.QueueAnimation(WeaponAnim.ShotgunReloadBase, force: true, EndGameEngine.FIXED_TIME_STEP);
					fpsAmin.QueueAnimation(WeaponAnim.ShotgunReload, force: true, EndGameEngine.FIXED_TIME_STEP);
					if (shotgunCockCue != null)
					{
						shotgunCockCue.Stop(AudioStopOptions.Immediate);
						shotgunCockCue.Dispose();
					}
					shotgunCockCue = EndGameEngine.SoundBnk.GetCue("ShotgunLoadShell");
					shotgunCockCue.Play();
					shotgunCockCue.SetVariable("Pitch", 50f);
				}
			}
			else
			{
				HackReloadBlendoutReached = true;
				e.NewHandler = (EventHandler<AnimationEventArgs>)Delegate.Combine(e.NewHandler, null);
				fpsAmin.AnimationEndReached += fpsAmin_EndAnimationReached;
			}
		}
	}

	private void fpsAmin_EndAnimationReached(object sender, AnimationEventArgs e)
	{
		if (e.CurrentAnimationType == AnimationType.WeaponPullout)
		{
			SwitchingWeapon = false;
			InSpecialOneOff = false;
			e.NewHandler = (EventHandler<AnimationEventArgs>)Delegate.Combine(e.NewHandler, null);
			if (EndGameEngine.GameSettings.GameName == "_AvR_" && currentWeaponType != WeaponType.RomanSword)
			{
				Owner.PrimaryWeapon = WeaponType.RomanSword;
			}
		}
		else if (e.CurrentAnimationType == AnimationType.WeaponPutaway)
		{
			currentWeaponIndex = NextWeaponIndex;
			currentWeaponType = weapon[NextWeaponIndex].WepType;
			fpsAmin.QueueAnimation(WeaponAnim.WeaponPullout, force: true, 332000);
			e.NewHandler = (EventHandler<AnimationEventArgs>)Delegate.Combine(e.NewHandler, null);
			fpsAmin.AnimationEndReached += fpsAmin_EndAnimationReached;
			fpsAmin.SetBaseAnimation(weapon[CurWeaponIndex].IdleAnim);
		}
		else if (e.CurrentAnimationType == AnimationType.Reload)
		{
			if (CurrentWeapon.WepType != WeaponType.Shotgun && CurrentWeapon.WepType != WeaponType.Grenader)
			{
				ReloadingWeapon = false;
				InSpecialOneOff = false;
				e.NewHandler = (EventHandler<AnimationEventArgs>)Delegate.Combine(e.NewHandler, null);
				CurrentWeapon.Reload();
			}
			else
			{
				ReloadingWeapon = false;
				InSpecialOneOff = false;
				e.NewHandler = (EventHandler<AnimationEventArgs>)Delegate.Combine(e.NewHandler, null);
			}
		}
		else if (e.CurrentAnimation == WeaponAnim.JumpLand)
		{
			InJump = false;
			InSpecialOneOff = false;
			e.NewHandler = (EventHandler<AnimationEventArgs>)Delegate.Combine(e.NewHandler, null);
		}
	}

	private void ThrowKnifeEvent(object sender, AnimationEventArgs e)
	{
	}

	public virtual void Update(GameTime gameTime, int qIndex, PlayerBase player)
	{
		WeaponBulletTest = false;
		HackNoShootTimer -= 0.03f;
		if (HackNoShootTimer < 0f && (fpsAmin.CurrentAnimationState.Flags & AnimFlag.AF_ONEOFF) == 0)
		{
			HackNoShootTimer = 5f;
			ReloadingWeapon = false;
			InSpecialOneOff = false;
		}
		if (NumberOfWeapons == 0)
		{
			matHeadTransform = player.mDataQueue[qIndex].world;
			return;
		}
		float num = (float)gameTime.ElapsedGameTime.Milliseconds * 0.001f;
		TextureOffset.X += num * 0.05f;
		TextureOffset.Y += num * 0.025f;
		ParticleSpawnTimer += num;
		BulletAccuracy += num;
		BulletAccuracy = ((BulletAccuracy > 1f) ? 1f : BulletAccuracy);
		bool flag = false;
		if (!CurrentWeapon.NaderToggled && !CanSightWeaponOverride)
		{
			flag = player.currentGamePadState.IsButtonDown(Buttons.LeftTrigger);
		}
		if (CurrentWeapon.InventoryItemRef != null)
		{
			CurrentWeapon.InventoryItemRef.reserved0 = (byte)CurrentWeapon.BulletsInMag;
		}
		bool allowfire = true;
		float num2 = 0.003f;
		player.vecCameraUpSway = HeadUp;
		RPGRockets.Update(num, qIndex);
		JavlinRockets.Update(num, qIndex);
		if (CurrentWeapon.WepType == WeaponType.ThrowingKnife)
		{
			allowfire = false;
			if (fpsAmin.CurrentAnimation != WeaponAnim.ThrowKnife)
			{
				if (player.TriggerDown && !TriggerHeldDown && Owner.NumberThrowingKnife > 0)
				{
					Owner.NumberThrowingKnife--;
					TriggerHeldDown = true;
					SwitchingWeapon = true;
					InSpecialOneOff = true;
					for (int i = 0; i < weapon.Count; i++)
					{
						if (weapon[i].WepType == Owner.PrimaryWeapon)
						{
							NextWeaponIndex = i;
							break;
						}
					}
					fpsAmin.AnimationBlendIn += fpsAmin_BlendInReached;
					fpsAmin.AnimationBlendOut += fpsAmin_BlendOutReached;
					fpsAmin.PlayAnimation(WeaponAnim.ThrowKnife, force: false, EndGameEngine.FIXED_TIME_STEP + (int)(Owner.tmpCommandoSpeed * (float)EndGameEngine.FIXED_TIME_STEP));
					if (knifeSoundCue != null)
					{
						knifeSoundCue.Stop(AudioStopOptions.Immediate);
						knifeSoundCue.Dispose();
					}
					knifeSoundCue = EndGameEngine.SoundBnk.GetCue("swishknife");
					knifeSoundCue.Play();
					knifeSoundCue.SetVariable("Pitch", 10f + Owner.tmpCommandoSpeed * 90f);
				}
				else
				{
					TriggerHeldDown = false;
				}
			}
		}
		else
		{
			tmpMuzzleSpawnPos = Vector3.Zero;
			if (CurrentWeapon.fireMode == WeaponFireMode.SemiAuto && player.TriggerDown && TriggerHeldDown)
			{
				allowfire = false;
			}
			if (player.ThirdPersonCamera && PlayerBase.ThirdPersonCameraFire < 0.5f)
			{
				allowfire = false;
				TriggerHeldDown = false;
			}
			FireWeapon(num, qIndex, player, flag, ref allowfire);
			if (player.ThirdPersonCamera && PlayerBase.ThirdPersonCameraFire < 0.5f)
			{
				TriggerHeldDown = false;
			}
		}
		curRecoil.X = MathHelper.Lerp(curRecoil.X, 0f, num * 10f);
		curRecoil.Y = MathHelper.Lerp(curRecoil.Y, 0f, num * 10f);
		curRecoil.Z = MathHelper.Lerp(curRecoil.Z, 0f, num * 10f);
		curRecoil.W = MathHelper.SmoothStep(curRecoil.W, 0f, num * 25f);
		if (player.Stance == PlayerStance.Walk)
		{
			if (flag)
			{
				fpsAmin.PlayAnimation(weapon[CurWeaponIndex].SightedAnim, force: false);
			}
			else
			{
				fpsAmin.PlayAnimation(weapon[CurWeaponIndex].WalkAnim, force: false);
			}
		}
		else if (player.Stance == PlayerStance.Run)
		{
			fpsAmin.PlayAnimation(weapon[CurWeaponIndex].RunAnim, force: false);
		}
		else if (flag)
		{
			fpsAmin.PlayAnimation(weapon[CurWeaponIndex].SightedAnim, force: false);
		}
		else
		{
			fpsAmin.PlayAnimation(weapon[CurWeaponIndex].IdleAnim, force: false);
		}
		float value = weapon[CurWeaponIndex].FOV;
		if (player.Sighted)
		{
			value = ((CurrentWeapon.Attachment != WeaponAttachment.SniperScope) ? weapon[CurWeaponIndex].SightedFOV : ((CurrentWeapon.WepType == WeaponType.NewTech) ? ((float)Math.PI / 12f) : ((float)Math.PI / 10f)));
		}
		WeaponZoom = MathHelper.SmoothStep(WeaponZoom, value, num * 30f);
		ref Matrix reference = ref WeaponProjection[qIndex];
		reference = Matrix.CreatePerspectiveFieldOfView(WeaponZoom, player.AspectRatio, 1f, 100000f);
		float num3 = Math.Abs(player.MoveY);
		if (num3 < Math.Abs(player.MoveX))
		{
			num3 = Math.Abs(player.MoveX);
		}
		float x = player.InputRightStick.X;
		float y = player.InputRightStick.Y;
		float num4 = 1f;
		if (player.Sighted)
		{
			num4 = -0.5f;
			if (CurrentWeapon.Attachment == WeaponAttachment.SniperScope)
			{
				x *= -0.0015f;
				y *= -0.0015f;
			}
			else if (CurrentWeapon.Attachment == WeaponAttachment.HoloGraphicSight)
			{
				x *= -0.015f;
				y *= -0.015f;
			}
			else if (player.ThirdPersonCamera)
			{
				x *= -0.018f;
				y *= -0.018f;
			}
			else
			{
				x *= -0.002f;
				y *= -0.002f;
			}
		}
		else
		{
			x = Math.Min(x + player.InputLeftStick.X, 1f);
			x *= -0.06f;
			y *= -0.04f;
		}
		if (CurrentWeapon.NaderToggled)
		{
			y -= 0.1f;
		}
		YawSwayX = MathHelper.SmoothStep(YawSwayX, x, num * 15f);
		YawSwayY = MathHelper.SmoothStep(YawSwayY, y, num * 15f);
		tmpMatSway = Matrix.CreateRotationZ(YawSwayX + curRecoil.X * 0.015f);
		tmpVecSway = tmpMatSway.Right * (YawSwayX * -20f);
		tmpMatSway *= Matrix.CreateFromAxisAngle(tmpMatSway.Up, YawSwayX * num4);
		tmpMatSway *= Matrix.CreateFromAxisAngle(tmpMatSway.Right, YawSwayY + curRecoil.Y);
		tmpMatSway.Translation = tmpVecSway + (tmpMatSway.Backward * (YawSwayY * 2f) + tmpMatSway.Down * (curRecoil.Z * 0.7f));
		fpsAmin.ApplyUserTransform(2, ref tmpMatSway);
		tmpHeadJump = Matrix.CreateFromYawPitchRoll(vecHeadSway.Y, vecHeadSway.X, vecHeadSway.Z);
		fpsAmin.ApplyUserTransform(1, ref tmpHeadJump);
		vecHeadSway = Vector3.Lerp(vecHeadSway, vecHeadSwayTarget, num * 7.5f);
		vecHeadSwayTarget = Vector3.Lerp(vecHeadSwayTarget, Vector3.Zero, num * 7.5f);
		if (CurrentWeapon.WepType == WeaponType.SUB)
		{
			tmpLeftHandNader = Matrix.CreateRotationY(MathHelper.ToRadians(Owner.LeftArmRotationY));
			tmpLeftHandNader *= Matrix.CreateRotationX(MathHelper.ToRadians(Owner.LeftArmRotationX + 2f));
			tmpLeftHandNader.Translation = tmpLeftHandNader.Left * 0.5f;
		}
		else
		{
			tmpLeftHandNader = Matrix.CreateRotationY(MathHelper.ToRadians(Owner.LeftArmRotationY));
			tmpLeftHandNader *= Matrix.CreateRotationX(MathHelper.ToRadians(Owner.LeftArmRotationX));
		}
		fpsAmin.ApplyUserTransform(19, ref tmpLeftHandNader);
		if (player.cPlayer.mergeAnimPlayer.CurrentClip != null && player.cPlayer.mergeAnimPlayer.CurrentClip.AnimType == AnimationType.Jump)
		{
			tmpMatrice = player.mDataQueue[qIndex].world;
			tmpMatrice.Translation = player.thirdPersonHeadmat.Translation + weaponHeightFix - JumpWepOffset;
			fpsAmin.Update(EndGameEngine.currentEleapsedTime.ElapsedGameTime, ref tmpMatrice, qIndex, num3);
		}
		else
		{
			tmpMatrice = player.mDataQueue[qIndex].world;
			tmpMatrice.Translation += weaponHeightFix;
			fpsAmin.Update(EndGameEngine.currentEleapsedTime.ElapsedGameTime, ref tmpMatrice, qIndex, num3);
		}
		fpsAmin.GetWorldTransformBlend(qIndex, 1, out matHeadTransform);
		if (player.TriggerDown && allowfire && !ReloadingWeapon)
		{
			NumBulletFired++;
			if (Owner.Sighted && CurrentWeapon.WepCategory != WeaponCategory.Equipment)
			{
				num2 += 0.005f;
				curRecoil.Z -= weapon[CurWeaponIndex].Recoil.W * 0.2f;
			}
			player.vecCameraUpSway = Vector3.Transform(HeadUp, Matrix.CreateFromAxisAngle(HeadDirection, ((float)rand.NextDouble() - 0.5f) * num2));
			BulletAccuracy -= weapon[CurWeaponIndex].BulletAccuraceyRecoil.X;
			BulletAccuracy = ((BulletAccuracy < weapon[CurWeaponIndex].BulletAccuraceyRecoil.Y) ? weapon[CurWeaponIndex].BulletAccuraceyRecoil.Y : BulletAccuracy);
		}
		int bone = 5;
		tmpMatWorld = Matrix.Identity;
		if (DrawKnife)
		{
			fpsAmin.GetWorldTransformBlend(qIndex, 21, out tmpMatWorld);
			math.RemoveScaling(ref tmpMatWorld);
			ref Matrix reference2 = ref matKnifeTransform[qIndex];
			reference2 = Matrix.Identity;
			ref Matrix reference3 = ref matKnifeTransform[qIndex];
			reference3 = Matrix.CreateFromAxisAngle(Vector3.UnitX, (float)Math.PI / 2f);
			ref Matrix reference4 = ref matKnifeTransform[qIndex];
			reference4 = matKnifeTransform[qIndex] * tmpPlayerScale;
			ref Matrix reference5 = ref matKnifeTransform[qIndex];
			reference5 = matKnifeTransform[qIndex] * tmpMatWorld;
		}
		fpsAmin.GetWorldTransformBlend(qIndex, bone, out tmpMatWorld);
		math.RemoveScaling(ref tmpMatWorld);
		ref Matrix reference6 = ref matWeaponTransform[qIndex];
		reference6 = Matrix.Identity;
		ref Matrix reference7 = ref matWeaponTransform[qIndex];
		reference7 = Matrix.CreateFromAxisAngle(Vector3.UnitX, (float)Math.PI / 2f);
		ref Matrix reference8 = ref matWeaponTransform[qIndex];
		reference8 = matWeaponTransform[qIndex] * tmpWeaponScale;
		ref Matrix reference9 = ref matWeaponTransform[qIndex];
		reference9 = matWeaponTransform[qIndex] * tmpMatWorld;
		float x2 = weapon[CurWeaponIndex].WepOffsetSighted.X;
		float y2 = weapon[CurWeaponIndex].WepOffsetSighted.Y;
		if (weapon[CurWeaponIndex].WepType == WeaponType.FiftyCal && weapon[CurWeaponIndex].Attachment == WeaponAttachment.SniperScope)
		{
			weapon[CurWeaponIndex].WepOffsetSighted.X = -2.9f;
			weapon[CurWeaponIndex].WepOffsetSighted.Y = 18f;
		}
		if (player.Sighted)
		{
			float num5 = 0f;
			if (CurrentWeapon.WepType == WeaponType.Shotgun)
			{
				if (CurrentWeapon.Attachment != WeaponAttachment.RedDotSight)
				{
					num5 = 1.1f;
				}
			}
			else if (CurrentWeapon.Attachment == WeaponAttachment.RedDotSight)
			{
				num5 = 1.1f;
			}
			wepOffset = tmpMatWorld.Down * (weapon[CurWeaponIndex].WepOffsetSighted.X + num5);
			wepOffset += tmpMatWorld.Backward * weapon[CurWeaponIndex].WepOffsetSighted.Y;
			wepOffset += tmpMatWorld.Left * weapon[CurWeaponIndex].WepOffsetSighted.Z;
			if (CurrentWeapon.WepType == WeaponType.SUB && CurrentWeapon.Attachment != WeaponAttachment.RedDotSight)
			{
				wepOffset += tmpMatWorld.Down * 2.1f;
			}
			matWeaponTransform[qIndex].Translation = matWeaponTransform[qIndex].Translation + wepOffset;
		}
		else
		{
			wepOffset = tmpMatWorld.Down * weapon[CurWeaponIndex].WepOffset.X;
			wepOffset += tmpMatWorld.Backward * weapon[CurWeaponIndex].WepOffset.Y;
			wepOffset += tmpMatWorld.Left * weapon[CurWeaponIndex].WepOffset.Z;
			matWeaponTransform[qIndex].Translation = matWeaponTransform[qIndex].Translation + wepOffset;
		}
		int bone2 = fpsAmin.CurrentAnimationState.BoneIndices[6];
		fpsAmin.GetWorldTransformBlend(qIndex, bone2, out tmpMatWorld);
		math.RemoveScaling(ref tmpMatWorld);
		ref Matrix reference10 = ref matMagTransform[qIndex];
		reference10 = Matrix.Identity;
		ref Matrix reference11 = ref matMagTransform[qIndex];
		reference11 = Matrix.CreateFromAxisAngle(Vector3.UnitX, (float)Math.PI / 2f);
		ref Matrix reference12 = ref matMagTransform[qIndex];
		reference12 = matMagTransform[qIndex] * tmpPlayerScale;
		ref Matrix reference13 = ref matMagTransform[qIndex];
		reference13 = matMagTransform[qIndex] * tmpMatWorld;
		if (player.Sighted)
		{
			wepOffset = tmpMatWorld.Down * weapon[CurWeaponIndex].WepOffsetSighted.X;
			wepOffset += tmpMatWorld.Backward * weapon[CurWeaponIndex].WepOffsetSighted.Y;
			wepOffset += tmpMatWorld.Left * weapon[CurWeaponIndex].WepOffsetSighted.Z;
			matMagTransform[qIndex].Translation = matMagTransform[qIndex].Translation + wepOffset;
		}
		else
		{
			wepOffset = tmpMatWorld.Down * weapon[CurWeaponIndex].WepOffset.X;
			wepOffset += tmpMatWorld.Backward * weapon[CurWeaponIndex].WepOffset.Y;
			wepOffset += tmpMatWorld.Left * weapon[CurWeaponIndex].WepOffset.Z;
			matMagTransform[qIndex].Translation = matMagTransform[qIndex].Translation + wepOffset;
		}
		int bone3 = fpsAmin.CurrentAnimationState.BoneIndices[8];
		fpsAmin.GetWorldTransformBlend(qIndex, bone3, out tmpMatWorld);
		math.RemoveScaling(ref tmpMatWorld);
		ref Matrix reference14 = ref matBoltTransform[qIndex];
		reference14 = Matrix.Identity;
		ref Matrix reference15 = ref matBoltTransform[qIndex];
		reference15 = Matrix.CreateFromAxisAngle(Vector3.UnitX, (float)Math.PI / 2f);
		ref Matrix reference16 = ref matBoltTransform[qIndex];
		reference16 = matBoltTransform[qIndex] * tmpPlayerScale;
		ref Matrix reference17 = ref matBoltTransform[qIndex];
		reference17 = matBoltTransform[qIndex] * tmpMatWorld;
		if (player.Sighted)
		{
			wepOffset = tmpMatWorld.Down * weapon[CurWeaponIndex].WepOffsetSighted.X;
			wepOffset += tmpMatWorld.Backward * (weapon[CurWeaponIndex].WepOffsetSighted.Y + curRecoil.W);
			wepOffset += tmpMatWorld.Left * weapon[CurWeaponIndex].WepOffsetSighted.Z;
			matBoltTransform[qIndex].Translation = matBoltTransform[qIndex].Translation + wepOffset;
		}
		else if (weapon[CurWeaponIndex].WepType != WeaponType.LightMachineGun)
		{
			wepOffset = tmpMatWorld.Down * weapon[CurWeaponIndex].WepOffset.X;
			wepOffset += tmpMatWorld.Backward * (weapon[CurWeaponIndex].WepOffset.Y + curRecoil.W);
			wepOffset += tmpMatWorld.Left * weapon[CurWeaponIndex].WepOffset.Z;
			matBoltTransform[qIndex].Translation = matBoltTransform[qIndex].Translation + wepOffset;
		}
		if (weapon[CurWeaponIndex].WepType == WeaponType.FiftyCal && weapon[CurWeaponIndex].Attachment == WeaponAttachment.SniperScope)
		{
			weapon[CurWeaponIndex].WepOffsetSighted.X = x2;
			weapon[CurWeaponIndex].WepOffsetSighted.Y = y2;
		}
		if (CurrentWeapon.AttachmentTwo == WeaponAttachment.NadeLauncher)
		{
			float num6 = num + Owner.tmpCommandoSpeed * num;
			tmpMatWorld = CurrentWeapon.GetBoneTransform(WeaponPart.M203);
			math.RemoveScaling(ref tmpMatWorld);
			Vector3 translation = tmpMatWorld.Translation;
			tmpMatWorld.Translation = Vector3.Zero;
			tmpMatWorld *= Matrix.CreateFromAxisAngle(Vector3.UnitX, (float)Math.PI / 2f);
			M203OffsetTimer -= num6;
			if (M203OffsetTimer <= 0f)
			{
				M203Offset = Vector3.SmoothStep(M203Offset, M203OffsetTarget, num6 * 25f);
			}
			tmpMatWorld.Translation = translation + M203Offset;
			ref Matrix reference18 = ref matM203Transform[qIndex];
			reference18 = tmpMatWorld * matWeaponTransform[qIndex];
		}
		ScopeSights.Update(qIndex, this);
		if (player.ShotFired)
		{
			tmpVelocity = Vector3.Zero;
			try
			{
				tmpMatrice = matWeaponTransform[qIndex];
				if (Owner.ThirdPersonCamera)
				{
					tmpMatrice = thisFrameMFmat;
				}
				tmpMuzzleSpawnPos = (weapon[CurWeaponIndex].GetBoneTransform(WeaponPart.Muzzle) * tmpMatrice).Translation;
			}
			catch (IndexOutOfRangeException ex)
			{
				MessagePump.AddMessage("MuzzleBonePosition: " + ex.Message);
			}
			if (ParticleSpawnTimer > 0.2f)
			{
				ParticleSpawnTimer = 0f;
				if (EndGameEngine.GameSettings.GameName.Contains("_AvR_"))
				{
					if (CurrentWeapon.WepType == WeaponType.AlienSniper)
					{
						if (!Owner.isSighted[qIndex])
						{
							particles.SpawnMuzzleSmoke(ref tmpMuzzleSpawnPos, ref player.vecDirection, fps: true);
							particles.SpawnMuzzleFlash2(ref tmpMuzzleSpawnPos, fps: true);
						}
					}
					else
					{
						if (CurrentWeapon.WepType == WeaponType.AlienShotty)
						{
							particles.SpawnMFAleinShotty(ref tmpMuzzleSpawnPos, ref player.vecDirection, fps: true);
						}
						else
						{
							particles.SpawnMuzzleSmoke(ref tmpMuzzleSpawnPos, ref player.vecDirection, fps: true);
						}
						particles.SpawnMuzzleFlash2(ref tmpMuzzleSpawnPos, fps: true);
					}
				}
				else if (CurrentWeapon.WepCategory != WeaponCategory.Equipment)
				{
					if (CurrentWeapon.WepType == WeaponType.Shotgun)
					{
						particles.SpawnMuzzleFlashShotty(ref tmpMuzzleSpawnPos, ref player.CameraDirection, !Owner.ThirdPersonCamera);
					}
					else
					{
						particles.SpawnMuzzleSmoke(ref tmpMuzzleSpawnPos, ref player.CameraDirection, fps: true);
						particles.SpawnMuzzleFlash2(ref tmpMuzzleSpawnPos, fps: true);
					}
				}
			}
			TracerBullet++;
			if (tmpHitMaterial != MaterialType.Undefined)
			{
				patricledir = hitPos - tmpMuzzleSpawnPos;
			}
			else
			{
				patricledir = fireDirection * 4000f;
			}
			if (EndGameEngine.GameSettings.GameName.Contains("_AvR_"))
			{
				if (!player.IsSplitScreen)
				{
					if (CurrentWeapon.WepType == WeaponType.AlienSMG || CurrentWeapon.WepType == WeaponType.AlienPistol || CurrentWeapon.WepType == WeaponType.AlienSniper)
					{
						particles.SpawnLaserLight(ref tmpMuzzleSpawnPos, ref patricledir, Color.LightBlue, fps: true);
					}
					else if (CurrentWeapon.WepType == WeaponType.AlienLMG)
					{
						particles.SpawnLaserLight(ref tmpMuzzleSpawnPos, ref patricledir, Color.LightYellow, fps: true);
					}
				}
			}
			else if (CurrentWeapon.WepCategory != WeaponCategory.Equipment && TracerBullet > 3 && CurrentWeapon.WepType != WeaponType.Shotgun)
			{
				TracerBullet = 0;
				particles.SpawnTracerBullet(ref tmpMuzzleSpawnPos, ref patricledir, fps: true);
			}
			bool flag2 = false;
			float testDistance = float.MaxValue;
			if (LevelBaseMenu.gameMode == GameMode.SurvivorLocal)
			{
				testDistance = ((tmpHitMaterial == MaterialType.Undefined) ? (-1f) : tmpSegmentParams.hitDistance);
				flag2 = AIBase.RayCast(qIndex, ref tmpOriginPos, ref fireDirection, CurrentWeapon, ref testDistance);
			}
			if (EGENetWorkNext.networkSession != null && !EGENetWorkNext.networkSession.IsDisposed)
			{
				float scaling = 1f;
				if (CurrentWeapon.WepType == WeaponType.Shotgun)
				{
					scaling = 2f;
				}
				for (int j = 0; j < EGENetWorkNext.networkSession.AllGamers.Count; j++)
				{
					NetworkGamer networkGamer = EGENetWorkNext.networkSession.AllGamers[j];
					PlayerBase playerBase = ((networkGamer != null) ? (networkGamer.Tag as PlayerBase) : null);
					if (playerBase == null || playerBase == player || !playerBase.Spawned)
					{
						continue;
					}
					int num7 = 0;
					DamegePacketType damageType = DamegePacketType.Body;
					if (tmpHitInWorld)
					{
						float num8 = (playerBase.vecPosition - player.vecPosition).LengthSquared();
						if (num8 < LevelOutside.RaycastHitDistance * LevelOutside.RaycastHitDistance)
						{
							WeaponBulletTest = true;
							num7 = playerBase.RayCast(tmpOriginPos, ref fireDirection, ref damageType, scaling);
						}
					}
					else
					{
						float num9 = (playerBase.vecPosition - player.vecPosition).LengthSquared();
						if (num9 < testDistance)
						{
							WeaponBulletTest = true;
							num7 = playerBase.RayCast(tmpOriginPos, ref fireDirection, ref damageType, scaling);
						}
					}
					if (num7 <= 0)
					{
						continue;
					}
					Owner.CurrentBulletsHitCount++;
					Owner.fHitIndicatorTimer = 0.1f;
					Owner.TotalPoints++;
					num7 -= (int)(Owner.PlayerArmor * 10f);
					num7 = ((num7 < 0) ? 5 : num7);
					if (CurrentWeapon.WepType == WeaponType.Shotgun)
					{
						float num10 = (tmpOriginPos - playerBase.vecPosition).LengthSquared();
						num7 = ((num10 < SHOTGUN_RANGE) ? ((int)((float)(num7 + 100) * (1f - num10 / SHOTGUN_RANGE))) : 0);
					}
					if (CurrentWeapon.WepType == WeaponType.Russian)
					{
						float num11 = (tmpOriginPos - playerBase.vecPosition).LengthSquared();
						if (num11 < AK74u_RANGE)
						{
							num7 += (int)(20f * (1f - num11 / AK74u_RANGE));
						}
					}
					else if (CurrentWeapon.WepType == WeaponType.NewTech)
					{
						num7 += 10;
					}
					else if (CurrentWeapon.WepType == WeaponType.FiftyCal || (CurrentWeapon.Attachment == WeaponAttachment.SniperScope && Owner.Sighted))
					{
						num7 += 60;
					}
					else if (CurrentWeapon.WepType == WeaponType.AlienSniper)
					{
						num7 += 100;
					}
					num7 = (playerBase.IsAttached0 ? (num7 / 2) : num7);
					if (EGENetWorkNext.networkSession.IsHost)
					{
						playerBase.BloodLevel -= num7;
						PacketWriter packetWriter = EGENetWorkNext.packetWriter;
						packetWriter.Write((byte)130);
						packetWriter.Write((byte)1);
						packetWriter.Write(networkGamer.Id);
						packetWriter.Write((byte)((num7 > 255) ? 255u : ((uint)num7)));
						packetWriter.Write((byte)1);
						EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.ReliableInOrder);
					}
					else
					{
						PacketWriter packetWriter2 = EGENetWorkNext.packetWriter;
						packetWriter2.Write((byte)130);
						packetWriter2.Write((byte)1);
						packetWriter2.Write(networkGamer.Id);
						packetWriter2.Write((byte)((num7 > 255) ? 255u : ((uint)num7)));
						packetWriter2.Write((byte)1);
						EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter2, SendDataOptions.ReliableInOrder, EGENetWorkNext.networkSession.Host);
					}
				}
			}
			else
			{
				float scaling2 = 1f;
				DamegePacketType damageType2 = DamegePacketType.Body;
				for (int k = 0; k < 4; k++)
				{
					if (LevelBaseMenu.Players[k].IsValid && LevelBaseMenu.Players[k] != player)
					{
						LevelBaseMenu.Players[k].RayCast(tmpOriginPos, ref fireDirection, ref damageType2, scaling2);
						if (LevelBaseMenu.Players[k].Health <= 0f)
						{
							player.NumKillsThisMatch++;
							player.TotalNumberKills++;
						}
					}
				}
			}
			if (!flag2 && tmpHitMaterial != MaterialType.Undefined)
			{
				if (tmpSegmentParams.TargetIndex >= 0)
				{
					Owner.CurrentBulletsHitCount++;
				}
				tmpHitInWorld = true;
				if (tmpHitMaterial == MaterialType.Metal)
				{
					particles.SpawnBulletHitMetal(ref hitPos, ref hitNorm);
				}
				else
				{
					particles.SpawnBulletHitRock(ref hitPos, ref hitNorm);
				}
			}
		}
		if (AttackWithKnifeTimer > 0f)
		{
			AttackWithKnifeTimer -= num;
			if (AttackWithKnifeTimer <= 0f)
			{
				AttackWithKnife();
			}
		}
		if (AttackWithSwordTimer > 0f)
		{
			AttackWithSwordTimer -= num;
			if (AttackWithSwordTimer <= 0f)
			{
				AttackWithSword();
			}
		}
	}

	public void FireWeapon(float eTimeMS, int qIndex, PlayerBase player, bool sighted, ref bool allowfire)
	{
		tmpHitInWorld = false;
		tmpHitMaterial = MaterialType.Undefined;
		player.ShotFired = false;
		FireTimer -= eTimeMS * 2f;
		FireRate = CurrentWeapon.FireRate;
		if (player.TriggerDown && allowfire && !ReloadingWeapon)
		{
			if (CurrentWeapon.BulletsInMag < 1)
			{
				if (ReloadWeapon())
				{
					Owner.tmpMergeAnim = WeaponAnim.CoOpReload;
					Owner.cPlayer.PlayMergedAnimation(WeaponAnim.CoOpReload);
				}
				else
				{
					foreach (InventoryItemCls item2 in AIBase.PlayerInventory.InventoryArray[1].list)
					{
						if (item2.desc != 0)
						{
							ushort item = (ushort)WeaponsCls.GetAmmoType((WeaponType)item2.ItemType);
							if (AIBase.PlayerInventory.HaveItem(1024, item))
							{
								AIBase.PlayerInventory.UseItem(item2, Owner);
							}
						}
					}
				}
				allowfire = false;
				TriggerHeldDown = false;
			}
			else
			{
				if (!(FireTimer < 0f) || (fpsAmin.CurrentAnimFlags & AnimFlag.AF_CAN_FIRE) <= AnimFlag.AF_CLEAR)
				{
					return;
				}
				player.WeaponFired();
				Owner.CurrentBulletsFiredCount++;
				FireTimer = CurrentWeapon.FireRate;
				float num = 0f;
				if (Owner.Sighted && CurrentWeapon.Attachment == WeaponAttachment.HoloGraphicSight)
				{
					num = 0.65f;
				}
				float num2 = ((Owner.tmpWeaponAccuracey + num > 1f) ? 1f : (Owner.tmpWeaponAccuracey + num));
				float num3 = 60f * (1f - num2) + 30f;
				float num4 = 100f * (1f - num2) + 75f;
				float num5 = ((player.Speed > player.SideStep) ? player.Speed : player.SideStep) * 0.04f;
				float num6 = (player.Sighted ? num3 : num4) * (1f - BulletAccuracy + num5);
				fireDirection = player.CameraDirection * (1000f * BulletAccuracy);
				fireDirection.X += ((float)rand.NextDouble() - 0.5f) * num6;
				fireDirection.Y += (float)rand.NextDouble() * 0.5f * num6;
				fireDirection.Z += ((float)rand.NextDouble() - 0.5f) * num6;
				fireDirection.Normalize();
				if (CurrentWeapon.WepType == WeaponType.AlienGrenader)
				{
					tmpMuzzleSpawnPos = (weapon[CurWeaponIndex].GetBoneTransform(WeaponPart.Muzzle) * matWeaponTransform[qIndex]).Translation;
					particles.SpawnNaderMuzzleFlash(ref tmpMuzzleSpawnPos, ref fireDirection);
				}
				else
				{
					hitPos.X = 0f;
					hitPos.Y = 0f;
					hitPos.Z = 0f;
					hitNorm.X = 0f;
					hitNorm.Y = 0f;
					hitNorm.Z = 0f;
					tmpOriginPos = player.vecHeadPosition[qIndex];
					tmpSegmentParams.OnlyWalkable = false;
					tmpSegmentParams.SegmentDirection = fireDirection;
					tmpSegmentParams.SegmentLength = 12000f;
					tmpSegmentParams.SegmentStart = tmpOriginPos;
					tmpSegmentParams.SegmentEnd = tmpOriginPos + fireDirection * tmpSegmentParams.SegmentLength;
					tmpSegmentParams.PreComputeParameters();
					tmpHitMaterial = LevelOutside.RayCast(qIndex, ref tmpSegmentParams, spawnSparks: true);
					hitNorm = tmpSegmentParams.hitNormal;
					hitPos = tmpSegmentParams.hitPosition;
					_ = tmpHitMaterial;
				}
				if (CurrentWeapon.WepCategory != WeaponCategory.Equipment)
				{
					if (sighted)
					{
						if (CurrentWeapon.fireMode == WeaponFireMode.SemiAuto)
						{
							curRecoil.X -= weapon[CurWeaponIndex].Recoil.X;
							curRecoil.Y -= weapon[CurWeaponIndex].Recoil.Y;
						}
						else
						{
							curRecoil.X -= weapon[CurWeaponIndex].Recoil.X * 0.5f;
							curRecoil.Y -= weapon[CurWeaponIndex].Recoil.Y * 0.5f;
						}
						curRecoil.Z -= weapon[CurWeaponIndex].Recoil.Z;
						curRecoil.W = weapon[CurWeaponIndex].Recoil.W;
					}
					else
					{
						curRecoil.X -= weapon[CurWeaponIndex].Recoil.X;
						curRecoil.Y -= weapon[CurWeaponIndex].Recoil.Y;
						curRecoil.Z -= weapon[CurWeaponIndex].Recoil.Z;
						curRecoil.W = weapon[CurWeaponIndex].Recoil.W;
					}
					player.PlayerFlags |= FPS_NET_FLAGS.FireWeapon;
					player.PlayerFlags = (TriggerHeldDown ? (player.PlayerFlags | FPS_NET_FLAGS.FireAuto) : player.PlayerFlags);
					if (CurrentWeapon.fireMode == WeaponFireMode.SemiAuto)
					{
						ControllerBase.SetVibration(player.playerIndex, 0.2f, 0.2f, 1f, 1f);
					}
					else
					{
						ControllerBase.SetVibration(player.playerIndex, 0.07f, 0.07f, 1f, 1f);
					}
				}
				if (CurrentWeapon.WepType == WeaponType.Shotgun && CurrentWeapon.BulletsInMag > 0)
				{
					if (player.Sighted)
					{
						fpsAmin.AnimationBlendIn += fpsAmin_BlendInReached;
						fpsAmin.AnimationBlendOut += fpsAmin_BlendOutReached;
						fpsAmin.AnimationEndReached += fpsAmin_EndAnimationReached;
						fpsAmin.PlayAnimation(WeaponAnim.ShotgunCockSighted, force: true, EndGameEngine.FIXED_TIME_STEP);
					}
					else
					{
						fpsAmin.AnimationBlendIn += fpsAmin_BlendInReached;
						fpsAmin.PlayAnimation(WeaponAnim.ShotgunCock, force: true, EndGameEngine.FIXED_TIME_STEP);
					}
					if (shotgunCockCue != null)
					{
						shotgunCockCue.Stop(AudioStopOptions.Immediate);
						shotgunCockCue.Dispose();
					}
					shotgunCockCue = EndGameEngine.SoundBnk.GetCue("ShotgunCock");
					shotgunCockCue.Play();
					shotgunCockCue.SetVariable("Pitch", 50f);
				}
				if (!fireSoundCue0.IsDisposed)
				{
					fireSoundCue0.Stop(AudioStopOptions.Immediate);
					fireSoundCue0.Dispose();
				}
				fireSoundCue0 = EndGameEngine.SoundBnk.GetCue(CurrentWeapon.WeaponShotSound0);
				fireSoundCue0.Play();
				CurrentWeapon.BulletsInMag--;
				TriggerHeldDown = true;
			}
		}
		else if (CurrentWeapon.fireMode == WeaponFireMode.SemiAuto && player.TriggerDown)
		{
			TriggerHeldDown = true;
		}
		else
		{
			TriggerHeldDown = false;
		}
	}

	public virtual void DrawRockets(int qIndex, PlayerBase player)
	{
		RPGRockets.Draw(player, qIndex);
		JavlinRockets.Draw(player, qIndex);
	}

	public virtual void Draw(int qIndex, PlayerBase player)
	{
		if (NumberOfWeapons == 0)
		{
			return;
		}
		player.ThermalScope = false;
		if (player.isSighted[qIndex] && CurrentWeapon.WepType == WeaponType.AlienSniper)
		{
			player.ThermalScope = true;
			return;
		}
		tmpDrawMat = Matrix.Identity;
		if (PlayerBase.ApocalypseZ_Hack)
		{
			tmpDrawDir = -player.vecDirection;
			tmpDrawMat.Translation = new Vector3(0f - player.vecHeadPosition[qIndex].X, 0f, 0f - player.vecHeadPosition[qIndex].Z);
			thisFrameMFmat = tmpDrawMat;
		}
		else
		{
			tmpDrawDir = -player.vecDirection;
		}
		GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
		graphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Black, 1f, 1);
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullCC;
		graphicsDevice.BlendState = BlendState.Opaque;
		drawTexProj = player.mDataQueue[qIndex].lightView * player.mDataQueue[qIndex].lightProj * LevelBaseMenu.matTextureProj;
		for (int i = 0; i < hands.Meshes.Count; i++)
		{
			drawMesh = hands.Meshes[i];
			for (int j = 0; j < drawMesh.MeshParts.Count; j++)
			{
				drawMeshPart = drawMesh.MeshParts[j];
				((WeaponEffectParams)drawMeshPart.Tag).matSkinnedWorldTransform.SetValue(tmpDrawMat);
				((WeaponEffectParams)drawMeshPart.Tag).matBones.SetValue(fpsAmin.GetSkinTransforms(qIndex));
				((WeaponEffectParams)drawMeshPart.Tag).matViewProj.SetValue(player.mDataQueue[qIndex].view * WeaponProjection[qIndex]);
				drawMeshPart.Effect.Parameters["vecEyePosition"].SetValue(player.mDataQueue[qIndex].cameraEyePos);
				drawMeshPart.Effect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
				drawMeshPart.Effect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
				if (player.ThermalScope)
				{
					drawMeshPart.Effect.CurrentTechnique.Passes[5].Apply();
				}
				else
				{
					drawMeshPart.Effect.CurrentTechnique.Passes[4].Apply();
				}
				drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
			}
		}
		for (int k = 0; k < weapon[CurWeaponIndex].model.Meshes.Count; k++)
		{
			drawMesh = weapon[CurWeaponIndex].model.Meshes[k];
			if ((CurrentWeapon.WepType == WeaponType.Grenader && CurrentWeapon.BulletsInMag == 0 && ((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.Magizine) || ((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.Muzzle || ((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.Lens || ((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.EmissiveLight || ((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.SelfIllumination)
			{
				continue;
			}
			for (int l = 0; l < drawMesh.MeshParts.Count; l++)
			{
				drawMeshPart = drawMesh.MeshParts[l];
				drawEffect = drawMeshPart.Effect;
				drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
				drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
				((WeaponEffectParams)drawMeshPart.Tag).texOffset.SetValue(TextureOffset);
				drawMeshPart.Effect.Parameters["vecEyePosition"].SetValue(player.mDataQueue[qIndex].cameraEyePos);
				float value = ((LevelOutside.DayLightScalar < 0.001f) ? 0.001f : (LevelOutside.DayLightScalar * 0.005f));
				drawMeshPart.Effect.Parameters["MaterialReflectScalar"].SetValue(value);
				((WeaponEffectParams)drawMeshPart.Tag).matViewProj.SetValue(player.mDataQueue[qIndex].view * WeaponProjection[qIndex]);
				if (CurrentWeapon.WepType == WeaponType.European && CurrentWeapon.Attachment != WeaponAttachment.Nothing && ((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.RearSight)
				{
					tmpSight = weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index];
					tmpSight *= drawRearSight;
					tmpSight.Translation = weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index].Translation;
					((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(tmpSight * matWeaponTransform[qIndex]);
				}
				else if (CurrentWeapon.WepType == WeaponType.European && CurrentWeapon.Attachment != WeaponAttachment.Nothing && ((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.FrontSight)
				{
					tmpSight = weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index];
					tmpSight *= drawFrontSight;
					tmpSight.Translation = weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index].Translation;
					((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(tmpSight * matWeaponTransform[qIndex]);
				}
				else if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.Magizine)
				{
					if (PlayerBase.ApocalypseZ_Hack)
					{
						tmpDrawMat = matMagTransform[qIndex];
						tmpDrawMat.Translation -= new Vector3(player.vecHeadPosition[qIndex].X, 0f, player.vecHeadPosition[qIndex].Z);
						((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index] * tmpDrawMat);
					}
					else
					{
						((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index] * matMagTransform[qIndex]);
					}
				}
				else if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.Bolt)
				{
					if (PlayerBase.ApocalypseZ_Hack)
					{
						tmpDrawMat = matBoltTransform[qIndex];
						tmpDrawMat.Translation -= new Vector3(player.vecHeadPosition[qIndex].X, 0f, player.vecHeadPosition[qIndex].Z);
						((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index] * tmpDrawMat);
					}
					else
					{
						((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index] * matBoltTransform[qIndex]);
					}
				}
				else if (PlayerBase.ApocalypseZ_Hack)
				{
					tmpDrawMat = matWeaponTransform[qIndex];
					tmpDrawMat.Translation -= new Vector3(player.vecHeadPosition[qIndex].X, 0f, player.vecHeadPosition[qIndex].Z);
					((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index] * tmpDrawMat);
				}
				else
				{
					((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index] * matWeaponTransform[qIndex]);
				}
				drawEffect.CurrentTechnique.Passes[10].Apply();
				drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
			}
		}
		if (DrawKnife)
		{
			int index = 0;
			for (int m = 0; m < weapon.Count; m++)
			{
				if (weapon[m].WepType == WeaponType.Knife)
				{
					index = m;
					break;
				}
			}
			for (int n = 0; n < weapon[index].model.Meshes.Count; n++)
			{
				drawMesh = weapon[index].model.Meshes[n];
				if (((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.Muzzle)
				{
					for (int num = 0; num < drawMesh.MeshParts.Count; num++)
					{
						drawMeshPart = drawMesh.MeshParts[num];
						drawEffect = drawMeshPart.Effect;
						drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
						drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
						((WeaponEffectParams)drawMeshPart.Tag).TextureShadowMap.SetValue(LevelBaseMenu.shadowRenderTarget);
						Vector3 value2 = Vector3.Transform(-player.mDataQueue[qIndex].view.Translation, Matrix.Transpose(player.mDataQueue[qIndex].view));
						drawMeshPart.Effect.Parameters["vecEyePosition"].SetValue(value2);
						((WeaponEffectParams)drawMeshPart.Tag).matTexProj.SetValue(drawTexProj);
						((WeaponEffectParams)drawMeshPart.Tag).matView.SetValue(player.mDataQueue[qIndex].view);
						((WeaponEffectParams)drawMeshPart.Tag).matProj.SetValue(WeaponProjection[qIndex]);
						((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(weapon[index].transforms[drawMesh.ParentBone.Index] * matKnifeTransform[qIndex]);
						drawEffect.CurrentTechnique.Passes[10].Apply();
						drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
					}
				}
			}
		}
		ScopeSights.vecFPSLightColor = vecFPSLightColor;
		ScopeSights.vecFPSLightPosition = vecFPSLightPosition;
		ScopeSights.Draw(qIndex, this, ref player.mDataQueue[qIndex].view, ref WeaponProjection[qIndex], ref drawTexProj, isMenu: false);
		if (CurrentWeapon.AttachmentTwo != WeaponAttachment.NadeLauncher)
		{
			return;
		}
		for (int num2 = 0; num2 < M203Nader.Meshes.Count; num2++)
		{
			drawMesh = M203Nader.Meshes[num2];
			for (int num3 = 0; num3 < drawMesh.MeshParts.Count; num3++)
			{
				drawMeshPart = drawMesh.MeshParts[num3];
				drawEffect = drawMeshPart.Effect;
				drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
				drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
				((WeaponEffectParams)drawMeshPart.Tag).texOffset.SetValue(TextureOffset);
				((WeaponEffectParams)drawMeshPart.Tag).TextureShadowMap.SetValue(LevelBaseMenu.shadowRenderTarget);
				Vector3 value3 = Vector3.Transform(-player.mDataQueue[qIndex].view.Translation, Matrix.Transpose(player.mDataQueue[qIndex].view));
				drawEffect.Parameters["vecEyePosition"].SetValue(value3);
				((WeaponEffectParams)drawMeshPart.Tag).matTexProj.SetValue(drawTexProj);
				((WeaponEffectParams)drawMeshPart.Tag).matView.SetValue(player.mDataQueue[qIndex].view);
				((WeaponEffectParams)drawMeshPart.Tag).matProj.SetValue(WeaponProjection[qIndex]);
				((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(M203NaderTransforms[drawMesh.ParentBone.Index] * matM203Transform[qIndex]);
				drawEffect.CurrentTechnique.Passes[10].Apply();
				drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
			}
		}
	}

	public void DrawPostLens(int qIndex, PlayerBase player)
	{
		if (NumberOfWeapons == 0)
		{
			return;
		}
		tmpDrawMat = Matrix.Identity;
		tmpDrawDir = -player.vecDirection;
		GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullCC;
		graphicsDevice.DepthStencilState = EndGameEngine.DepthEnabled;
		drawTexProj = player.mDataQueue[qIndex].lightView * player.mDataQueue[qIndex].lightProj * LevelBaseMenu.matTextureProj;
		for (int i = 0; i < weapon[CurWeaponIndex].model.Meshes.Count; i++)
		{
			drawMesh = weapon[CurWeaponIndex].model.Meshes[i];
			if (((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.Lens && ((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.SelfIllumination)
			{
				continue;
			}
			for (int j = 0; j < drawMesh.MeshParts.Count; j++)
			{
				drawMeshPart = drawMesh.MeshParts[j];
				drawEffect = drawMeshPart.Effect;
				drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
				drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
				((WeaponEffectParams)drawMeshPart.Tag).texOffset.SetValue(TextureOffset);
				((WeaponEffectParams)drawMeshPart.Tag).TextureShadowMap.SetValue(LevelBaseMenu.shadowRenderTarget);
				Vector3 value = Vector3.Transform(-player.mDataQueue[qIndex].view.Translation, Matrix.Transpose(player.mDataQueue[qIndex].view));
				drawMeshPart.Effect.Parameters["vecEyePosition"].SetValue(value);
				((WeaponEffectParams)drawMeshPart.Tag).matTexProj.SetValue(drawTexProj);
				((WeaponEffectParams)drawMeshPart.Tag).matView.SetValue(player.mDataQueue[qIndex].view);
				((WeaponEffectParams)drawMeshPart.Tag).matProj.SetValue(WeaponProjection[qIndex]);
				((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index] * matWeaponTransform[qIndex]);
				drawEffect.Parameters["DepthTexture"].SetValue(LevelBaseMenu.DepthRenderTarget);
				if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.SelfIllumination)
				{
					graphicsDevice.BlendState = BlendState.Opaque;
					drawEffect.CurrentTechnique.Passes[14].Apply();
				}
				else
				{
					graphicsDevice.BlendState = BlendState.NonPremultiplied;
					drawEffect.CurrentTechnique.Passes[12].Apply();
				}
				drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
			}
		}
	}

	public void DrawScope(int qIndex, PlayerBase player, Texture2D scene, Texture2D bloom)
	{
		if (NumberOfWeapons != 0 && CurrentWeapon.Attachment == WeaponAttachment.SniperScope)
		{
			ScopeSights.PostDrawScope(qIndex, player, this, ref WeaponProjection[qIndex], ref drawTexProj, scene, bloom);
		}
	}

	public void DrawMuzzleFlash(int qIndex, PlayerBase player, float alphaValue)
	{
		if (NumberOfWeapons == 0)
		{
			return;
		}
		if (PlayerBase.ApocalypseZ_Hack)
		{
			bool flag = Owner.NetGamerRef != null && Owner.NetGamerRef.IsLocal;
			if (Owner.ThirdPersonCamera || !flag)
			{
				tmpDrawMat = thisFrameMFmat;
			}
			else
			{
				tmpDrawMat = matWeaponTransform[qIndex];
			}
			tmpDrawDir = tmpDrawMat.Translation;
			tmpDrawDir.X -= player.vecHeadPosition[qIndex].X;
			tmpDrawDir.Z -= player.vecHeadPosition[qIndex].Z;
			tmpDrawMat.Translation = tmpDrawDir;
		}
		else
		{
			tmpDrawMat = matWeaponTransform[qIndex];
		}
		GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
		graphicsDevice.BlendState = BlendState.Additive;
		graphicsDevice.DepthStencilState = EndGameEngine.DepthNoWrite;
		for (int i = 0; i < weapon[CurWeaponIndex].model.Meshes.Count; i++)
		{
			drawMesh = weapon[CurWeaponIndex].model.Meshes[i];
			if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.Muzzle)
			{
				for (int j = 0; j < drawMesh.MeshParts.Count; j++)
				{
					drawMeshPart = drawMesh.MeshParts[j];
					drawEffect = drawMeshPart.Effect;
					drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
					drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
					((WeaponEffectParams)drawMeshPart.Tag).texOffset.SetValue(TextureOffset);
					Vector3 value = Vector3.Transform(-player.mDataQueue[qIndex].view.Translation, Matrix.Transpose(player.mDataQueue[qIndex].view));
					drawMeshPart.Effect.Parameters["vecEyePosition"].SetValue(value);
					((WeaponEffectParams)drawMeshPart.Tag).matView.SetValue(player.mDataQueue[qIndex].view);
					((WeaponEffectParams)drawMeshPart.Tag).matProj.SetValue(player.mDataQueue[qIndex].projection);
					((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(Matrix.CreateRotationZ((float)EndGameEngine.randGenerator.NextDouble() * 3.14f) * weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index] * tmpDrawMat);
					drawEffect.Parameters["fMuzzleAlpha"].SetValue(alphaValue);
					drawEffect.CurrentTechnique.Passes[13].Apply();
					drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
				}
			}
		}
	}

	public virtual void DrawDepth(int qIndex, PlayerBase player)
	{
		if (NumberOfWeapons == 0)
		{
			return;
		}
		Matrix identity = Matrix.Identity;
		Matrix view = player.mDataQueue[qIndex].view;
		for (int i = 0; i < hands.Meshes.Count; i++)
		{
			drawMesh = hands.Meshes[i];
			for (int j = 0; j < drawMesh.MeshParts.Count; j++)
			{
				drawMeshPart = drawMesh.MeshParts[j];
				((WeaponEffectParams)drawMeshPart.Tag).matSkinnedWorldTransform.SetValue(identity);
				((WeaponEffectParams)drawMeshPart.Tag).matBones.SetValue(fpsAmin.GetSkinTransforms(qIndex));
				((WeaponEffectParams)drawMeshPart.Tag).matView.SetValue(view);
				((WeaponEffectParams)drawMeshPart.Tag).matProj.SetValue(WeaponProjection[qIndex]);
				drawMeshPart.Effect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
				drawMeshPart.Effect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
				drawMeshPart.Effect.CurrentTechnique.Passes[1].Apply();
				drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
			}
		}
		for (int k = 0; k < weapon[CurWeaponIndex].model.Meshes.Count; k++)
		{
			drawMesh = weapon[CurWeaponIndex].model.Meshes[k];
			if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.Muzzle)
			{
				continue;
			}
			for (int l = 0; l < drawMesh.MeshParts.Count; l++)
			{
				drawMeshPart = drawMesh.MeshParts[l];
				drawEffect = drawMeshPart.Effect;
				drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
				drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
				((WeaponEffectParams)drawMeshPart.Tag).matView.SetValue(view);
				((WeaponEffectParams)drawMeshPart.Tag).matProj.SetValue(WeaponProjection[qIndex]);
				if (CurrentWeapon.WepType == WeaponType.European && (CurrentWeapon.Attachment == WeaponAttachment.SniperScope || CurrentWeapon.Attachment == WeaponAttachment.HoloGraphicSight) && ((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.RearSight)
				{
					tmpSight = weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index];
					tmpSight *= drawRearSight;
					tmpSight.Translation = weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index].Translation;
					((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(tmpSight * matWeaponTransform[qIndex]);
				}
				else if (CurrentWeapon.WepType == WeaponType.European && (CurrentWeapon.Attachment == WeaponAttachment.SniperScope || CurrentWeapon.Attachment == WeaponAttachment.HoloGraphicSight) && ((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.FrontSight)
				{
					tmpSight = weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index];
					tmpSight *= drawFrontSight;
					tmpSight.Translation = weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index].Translation;
					((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(tmpSight * matWeaponTransform[qIndex]);
				}
				else if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.Magizine)
				{
					((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index] * matMagTransform[qIndex]);
				}
				else if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.Bolt)
				{
					((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index] * matBoltTransform[qIndex]);
				}
				else
				{
					((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index] * matWeaponTransform[qIndex]);
				}
				drawEffect.CurrentTechnique.Passes[8].Apply();
				drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
			}
		}
		if (DrawKnife)
		{
			int index = 0;
			for (int m = 0; m < weapon.Count; m++)
			{
				if (weapon[m].WepType == WeaponType.Knife)
				{
					index = m;
					break;
				}
			}
			for (int n = 0; n < weapon[index].model.Meshes.Count; n++)
			{
				drawMesh = weapon[index].model.Meshes[n];
				if (((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.Muzzle)
				{
					for (int num = 0; num < drawMesh.MeshParts.Count; num++)
					{
						drawMeshPart = drawMesh.MeshParts[num];
						drawEffect = drawMeshPart.Effect;
						drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
						drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
						((WeaponEffectParams)drawMeshPart.Tag).matView.SetValue(view);
						((WeaponEffectParams)drawMeshPart.Tag).matProj.SetValue(WeaponProjection[qIndex]);
						((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(weapon[index].transforms[drawMesh.ParentBone.Index] * matKnifeTransform[qIndex]);
						drawEffect.CurrentTechnique.Passes[8].Apply();
						drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
					}
				}
			}
		}
		ScopeSights.DrawDepth(qIndex, this, ref view, ref WeaponProjection[qIndex]);
	}

	public void DrawPlayerWeapon(int qIndex, PlayerBase player, PlayerBase viewer, Matrix transform, Vector2 muzzleHeat)
	{
		matWeaponTransform[qIndex] = transform;
		transform = tmpPlayerScale * transform;
		thisFrameMFmat = transform;
		for (int i = 0; i < weapon[CurWeaponIndex].model.Meshes.Count; i++)
		{
			drawMesh = weapon[CurWeaponIndex].model.Meshes[i];
			if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.Muzzle)
			{
				MuzzleHeatPartIndex++;
				if (MuzzleHeatPartIndex > 5)
				{
					MuzzleHeatPartIndex = 0;
				}
				if (MuzzleHeatPartIndex == 0 && muzzleHeat.Y > 0.25f)
				{
					matMuzzleMesh = matMuzzleFlash * weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index] * transform;
					MHPartPos = matMuzzleMesh.Translation;
					particles.SpawnMuzzleHeat(ref MHPartPos, muzzleHeat.Y);
				}
				continue;
			}
			for (int j = 0; j < drawMesh.MeshParts.Count; j++)
			{
				drawMeshPart = drawMesh.MeshParts[j];
				if (((WeaponEffectParams)drawMeshPart.Tag).weaponPart == WeaponPart.Body || ((WeaponEffectParams)drawMeshPart.Tag).weaponPart == WeaponPart.Magizine || ((WeaponEffectParams)drawMeshPart.Tag).weaponPart == WeaponPart.Bolt)
				{
					drawEffect = drawMeshPart.Effect;
					drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
					drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
					((WeaponEffectParams)drawMeshPart.Tag).texOffset.SetValue(TextureOffset);
					((WeaponEffectParams)drawMeshPart.Tag).matViewProj.SetValue(viewer.mDataQueue[qIndex].viewProj);
					if (PlayerBase.ApocalypseZ_Hack)
					{
						tmpDrawMat = transform;
						MHPartPos = tmpDrawMat.Translation;
						MHPartPos.X -= viewer.vecHeadPosition[qIndex].X;
						MHPartPos.Z -= viewer.vecHeadPosition[qIndex].Z;
						tmpDrawMat.Translation = MHPartPos;
						((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index] * tmpDrawMat);
					}
					else
					{
						((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index] * transform);
					}
					drawEffect.Parameters["vecEyePosition"].SetValue(viewer.mDataQueue[qIndex].cameraEyePos);
					drawEffect.Parameters["fMuzzleHeat"].SetValue(muzzleHeat);
					drawEffect.CurrentTechnique.Passes[10].Apply();
					drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
				}
			}
		}
	}

	public void DrawPlayerWeaponMF(int qIndex, PlayerBase player, PlayerBase viewer, Matrix transform, float muzzleHeat)
	{
		transform = tmpPlayerScale * transform;
		_ = Matrix.Identity;
		_ = -viewer.vecDirection;
		drawTexProj = viewer.mDataQueue[qIndex].lightView * viewer.mDataQueue[qIndex].lightProj * LevelBaseMenu.matTextureProj;
		GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
		graphicsDevice.BlendState = BlendState.Additive;
		graphicsDevice.DepthStencilState = EndGameEngine.DepthNoWrite;
		matMuzzleFlash = Matrix.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(rand.Next(0, 4) * 90));
		matMuzzleFlash *= Matrix.CreateScale(1f + (float)rand.NextDouble() * 0.1f);
		for (int i = 0; i < weapon[CurWeaponIndex].model.Meshes.Count; i++)
		{
			drawMesh = weapon[CurWeaponIndex].model.Meshes[i];
			if (((WeaponPartStruct)drawMesh.Tag).PartType == WeaponPart.Muzzle)
			{
				for (int j = 0; j < drawMesh.MeshParts.Count; j++)
				{
					drawMeshPart = drawMesh.MeshParts[j];
					drawEffect = drawMeshPart.Effect;
					drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
					drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
					((WeaponEffectParams)drawMeshPart.Tag).texOffset.SetValue(TextureOffset);
					((WeaponEffectParams)drawMeshPart.Tag).TextureShadowMap.SetValue(LevelBaseMenu.shadowRenderTarget);
					((WeaponEffectParams)drawMeshPart.Tag).matTexProj.SetValue(drawTexProj);
					((WeaponEffectParams)drawMeshPart.Tag).matView.SetValue(viewer.mDataQueue[qIndex].view);
					((WeaponEffectParams)drawMeshPart.Tag).matProj.SetValue(viewer.mDataQueue[qIndex].projection);
					Vector3 value = Vector3.Transform(-viewer.mDataQueue[qIndex].view.Translation, Matrix.Transpose(viewer.mDataQueue[qIndex].view));
					drawEffect.Parameters["vecEyePosition"].SetValue(value);
					((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(matMuzzleFlash * weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index] * transform);
					drawEffect.Parameters["DepthTexture"].SetValue(LevelBaseMenu.DepthRenderTarget);
					drawEffect.Parameters["fMuzzleAlpha"].SetValue(1);
					drawEffect.CurrentTechnique.Passes[13].Apply();
					drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
				}
			}
		}
	}

	public void DrawWeaponMenu(int qIndex, PlayerBase playerRef, Matrix transform, Vector4 lightColor, Vector4 ambientColor, Vector3 lightPos)
	{
		transform = tmpPlayerScale * transform;
		_ = Matrix.Identity;
		_ = -playerRef.vecDirection;
		drawTexProj = playerRef.menuView * playerRef.menuProj * LevelBaseMenu.matTextureProj;
		for (int i = 0; i < 2; i++)
		{
			if (i == 1)
			{
				transform = Matrix.CreateRotationX(-(float)Math.PI / 2f) * Matrix.CreateRotationY((float)Math.PI * -19f / 20f) * Matrix.CreateScale(3.5f);
				transform.Translation = new Vector3(120f, -40f, 0f);
				matWeaponTransform[qIndex] = transform;
			}
			for (int j = 0; j < weapon[CurWeaponIndex].model.Meshes.Count; j++)
			{
				drawMesh = weapon[CurWeaponIndex].model.Meshes[j];
				if (((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.Muzzle && ((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.M203 && ((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.Scope)
				{
					for (int k = 0; k < drawMesh.MeshParts.Count; k++)
					{
						drawMeshPart = drawMesh.MeshParts[k];
						drawEffect = drawMeshPart.Effect;
						drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
						drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
						((WeaponEffectParams)drawMeshPart.Tag).texOffset.SetValue(TextureOffset);
						((WeaponEffectParams)drawMeshPart.Tag).TextureShadowMap.SetValue(LevelBaseMenu.shadowRenderTarget);
						((WeaponEffectParams)drawMeshPart.Tag).EnvMap0.SetValue(LevelBaseMenu.EnvMap);
						((WeaponEffectParams)drawMeshPart.Tag).vecLightColor.SetValue(lightColor);
						((WeaponEffectParams)drawMeshPart.Tag).vecAmbientLightColor.SetValue(ambientColor);
						((WeaponEffectParams)drawMeshPart.Tag).vecLightPosition.SetValue(lightPos);
						((WeaponEffectParams)drawMeshPart.Tag).fSpecularPower.SetValue(64f);
						((WeaponEffectParams)drawMeshPart.Tag).vecFPSLightPos.SetValue(vecFPSLightPosition[qIndex]);
						((WeaponEffectParams)drawMeshPart.Tag).vecFPSLightColor.SetValue(vecFPSLightColor[qIndex]);
						((WeaponEffectParams)drawMeshPart.Tag).matTexProj.SetValue(drawTexProj);
						((WeaponEffectParams)drawMeshPart.Tag).matView.SetValue(playerRef.menuView);
						((WeaponEffectParams)drawMeshPart.Tag).matProj.SetValue(playerRef.menuProj);
						((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index] * transform);
						((WeaponEffectParams)drawMeshPart.Tag).vecMuzzleFlash.SetValue(particles.MuzzleFlash());
						drawEffect.CurrentTechnique.Passes[9].Apply();
						drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
						((WeaponEffectParams)drawMeshPart.Tag).SetConstants();
					}
				}
			}
		}
		if (weapon[CurWeaponIndex].Attachment != WeaponAttachment.Nothing)
		{
			ScopeSights.Update(qIndex, this);
			ScopeSights.vecFPSLightColor = vecFPSLightColor;
			ScopeSights.vecFPSLightPosition = vecFPSLightPosition;
			ScopeSights.Draw(0, this, ref playerRef.menuView, ref playerRef.menuProj, ref drawTexProj, isMenu: true);
		}
		if (CurrentWeapon.AttachmentTwo != WeaponAttachment.NadeLauncher)
		{
			return;
		}
		tmpMatWorld = CurrentWeapon.GetBoneTransform(WeaponPart.M203);
		math.RemoveScaling(ref tmpMatWorld);
		Vector3 translation = tmpMatWorld.Translation;
		tmpMatWorld.Translation = Vector3.Zero;
		tmpMatWorld *= Matrix.CreateFromAxisAngle(Vector3.UnitX, (float)Math.PI / 2f);
		tmpMatWorld.Translation = translation;
		ref Matrix reference = ref matM203Transform[qIndex];
		reference = tmpMatWorld * matWeaponTransform[qIndex];
		for (int l = 0; l < M203Nader.Meshes.Count; l++)
		{
			drawMesh = M203Nader.Meshes[l];
			for (int m = 0; m < drawMesh.MeshParts.Count; m++)
			{
				drawMeshPart = drawMesh.MeshParts[m];
				drawEffect = drawMeshPart.Effect;
				drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
				drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
				((WeaponEffectParams)drawMeshPart.Tag).texOffset.SetValue(TextureOffset);
				((WeaponEffectParams)drawMeshPart.Tag).TextureShadowMap.SetValue(LevelBaseMenu.shadowRenderTarget);
				((WeaponEffectParams)drawMeshPart.Tag).matTexProj.SetValue(drawTexProj);
				((WeaponEffectParams)drawMeshPart.Tag).matView.SetValue(playerRef.menuView);
				((WeaponEffectParams)drawMeshPart.Tag).matProj.SetValue(playerRef.menuProj);
				((WeaponEffectParams)drawMeshPart.Tag).vecLightColor.SetValue(lightColor);
				((WeaponEffectParams)drawMeshPart.Tag).vecAmbientLightColor.SetValue(ambientColor);
				((WeaponEffectParams)drawMeshPart.Tag).vecLightPosition.SetValue(lightPos);
				((WeaponEffectParams)drawMeshPart.Tag).vecFPSLightPos.SetValue(vecFPSLightPosition[qIndex]);
				((WeaponEffectParams)drawMeshPart.Tag).vecFPSLightColor.SetValue(vecFPSLightColor[qIndex]);
				((WeaponEffectParams)drawMeshPart.Tag).vecMuzzleFlash.SetValue(particles.MuzzleFlash());
				((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(M203NaderTransforms[drawMesh.ParentBone.Index] * matM203Transform[qIndex]);
				drawEffect.CurrentTechnique.Passes[9].Apply();
				drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
				((WeaponEffectParams)drawMeshPart.Tag).SetConstants();
			}
		}
	}

	public void DrawWeaponPreviewMenu(int qIndex, PlayerBase playerRef, Matrix transform, Vector4 lightColor, Vector4 ambientColor, Vector3 lightPos)
	{
		transform = tmpPlayerScale * transform;
		_ = Matrix.Identity;
		drawTexProj = playerRef.mDataQueue[qIndex].lightView * playerRef.mDataQueue[qIndex].lightProj * LevelBaseMenu.matTextureProj;
		transform = Matrix.CreateRotationX(-(float)Math.PI / 2f) * Matrix.CreateRotationY(-(float)Math.PI / 2f) * Matrix.CreateScale(1f);
		transform.Translation = new Vector3(140f, 38f, 0f);
		matWeaponTransform[qIndex] = transform;
		for (int i = 0; i < weapon[CurWeaponIndex].model.Meshes.Count; i++)
		{
			drawMesh = weapon[CurWeaponIndex].model.Meshes[i];
			if (((WeaponPartStruct)drawMesh.Tag).PartType != WeaponPart.Muzzle)
			{
				for (int j = 0; j < drawMesh.MeshParts.Count; j++)
				{
					drawMeshPart = drawMesh.MeshParts[j];
					drawEffect = drawMeshPart.Effect;
					drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
					drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
					((WeaponEffectParams)drawMeshPart.Tag).texOffset.SetValue(TextureOffset);
					((WeaponEffectParams)drawMeshPart.Tag).TextureShadowMap.SetValue(LevelBaseMenu.shadowRenderTarget);
					((WeaponEffectParams)drawMeshPart.Tag).EnvMap0.SetValue(LevelBaseMenu.EnvMap);
					((WeaponEffectParams)drawMeshPart.Tag).vecLightColor.SetValue(lightColor);
					((WeaponEffectParams)drawMeshPart.Tag).vecAmbientLightColor.SetValue(ambientColor);
					((WeaponEffectParams)drawMeshPart.Tag).vecLightPosition.SetValue(lightPos);
					((WeaponEffectParams)drawMeshPart.Tag).fSpecularPower.SetValue(64f);
					((WeaponEffectParams)drawMeshPart.Tag).vecFPSLightPos.SetValue(vecFPSLightPosition[qIndex]);
					((WeaponEffectParams)drawMeshPart.Tag).vecFPSLightColor.SetValue(vecFPSLightColor[qIndex]);
					((WeaponEffectParams)drawMeshPart.Tag).matTexProj.SetValue(drawTexProj);
					((WeaponEffectParams)drawMeshPart.Tag).matView.SetValue(playerRef.menuView);
					((WeaponEffectParams)drawMeshPart.Tag).matProj.SetValue(playerRef.menuProj);
					((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(weapon[CurWeaponIndex].transforms[drawMesh.ParentBone.Index] * transform);
					((WeaponEffectParams)drawMeshPart.Tag).vecMuzzleFlash.SetValue(particles.MuzzleFlash());
					drawEffect.CurrentTechnique.Passes[9].Apply();
					drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
					((WeaponEffectParams)drawMeshPart.Tag).SetConstants();
				}
			}
		}
		if (weapon[CurWeaponIndex].Attachment != WeaponAttachment.Nothing)
		{
			ScopeSights.Update(qIndex, this);
			ScopeSights.vecFPSLightColor = vecFPSLightColor;
			ScopeSights.vecFPSLightPosition = vecFPSLightPosition;
			ScopeSights.Draw(0, this, ref playerRef.menuView, ref playerRef.menuProj, ref drawTexProj, isMenu: true);
		}
		if (CurrentWeapon.AttachmentTwo != WeaponAttachment.NadeLauncher)
		{
			return;
		}
		tmpMatWorld = CurrentWeapon.GetBoneTransform(WeaponPart.M203);
		math.RemoveScaling(ref tmpMatWorld);
		Vector3 translation = tmpMatWorld.Translation;
		tmpMatWorld.Translation = Vector3.Zero;
		tmpMatWorld *= Matrix.CreateFromAxisAngle(Vector3.UnitX, (float)Math.PI / 2f);
		tmpMatWorld.Translation = translation;
		ref Matrix reference = ref matM203Transform[qIndex];
		reference = tmpMatWorld * matWeaponTransform[qIndex];
		for (int k = 0; k < M203Nader.Meshes.Count; k++)
		{
			drawMesh = M203Nader.Meshes[k];
			for (int l = 0; l < drawMesh.MeshParts.Count; l++)
			{
				drawMeshPart = drawMesh.MeshParts[l];
				drawEffect = drawMeshPart.Effect;
				drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
				drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
				((WeaponEffectParams)drawMeshPart.Tag).texOffset.SetValue(TextureOffset);
				((WeaponEffectParams)drawMeshPart.Tag).TextureShadowMap.SetValue(LevelBaseMenu.shadowRenderTarget);
				((WeaponEffectParams)drawMeshPart.Tag).matTexProj.SetValue(drawTexProj);
				((WeaponEffectParams)drawMeshPart.Tag).matView.SetValue(playerRef.menuView);
				((WeaponEffectParams)drawMeshPart.Tag).matProj.SetValue(playerRef.menuProj);
				((WeaponEffectParams)drawMeshPart.Tag).vecLightColor.SetValue(lightColor);
				((WeaponEffectParams)drawMeshPart.Tag).vecAmbientLightColor.SetValue(ambientColor);
				((WeaponEffectParams)drawMeshPart.Tag).vecLightPosition.SetValue(lightPos);
				((WeaponEffectParams)drawMeshPart.Tag).vecFPSLightPos.SetValue(vecFPSLightPosition[qIndex]);
				((WeaponEffectParams)drawMeshPart.Tag).vecFPSLightColor.SetValue(vecFPSLightColor[qIndex]);
				((WeaponEffectParams)drawMeshPart.Tag).vecMuzzleFlash.SetValue(particles.MuzzleFlash());
				((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(M203NaderTransforms[drawMesh.ParentBone.Index] * matM203Transform[qIndex]);
				drawEffect.CurrentTechnique.Passes[9].Apply();
				drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
				((WeaponEffectParams)drawMeshPart.Tag).SetConstants();
			}
		}
	}

	public void DrawAttachmentPreviewMenu(int qIndex, PlayerBase playerRef, Vector4 lightColor, Vector4 ambientColor, Vector3 lightPos)
	{
		Matrix identity = Matrix.Identity;
		drawTexProj = playerRef.mDataQueue[qIndex].lightView * playerRef.mDataQueue[qIndex].lightProj * LevelBaseMenu.matTextureProj;
		float scale = 3.2f;
		float num = -0.95f;
		Vector3 translation = new Vector3(165f, -10f, 0f);
		if (CurrentWeapon.Attachment == WeaponAttachment.IronSights)
		{
			scale = 4f;
			num = -1f;
			translation.X = 175f;
			translation.Y = -28f;
		}
		if (CurrentWeapon.Attachment == WeaponAttachment.SniperScope)
		{
			translation.X = 175f;
			translation.Y = -12f;
		}
		identity = Matrix.CreateRotationX(-(float)Math.PI / 2f) * Matrix.CreateRotationY((float)Math.PI * num) * Matrix.CreateScale(scale);
		identity.Translation = translation;
		matWeaponTransform[qIndex] = identity;
		if (weapon[CurWeaponIndex].Attachment != WeaponAttachment.Nothing)
		{
			ScopeSights.Update(qIndex, this);
			ScopeSights.vecFPSLightColor = vecFPSLightColor;
			ScopeSights.vecFPSLightPosition = vecFPSLightPosition;
			ScopeSights.Draw(0, this, ref playerRef.menuView, ref playerRef.menuProj, ref drawTexProj, isMenu: false);
		}
	}

	public void DrawM203PreviewMenu(int qIndex, PlayerBase playerRef, Vector4 lightColor, Vector4 ambientColor, Vector3 lightPos)
	{
		Matrix identity = Matrix.Identity;
		drawTexProj = playerRef.mDataQueue[qIndex].lightView * playerRef.mDataQueue[qIndex].lightProj * LevelBaseMenu.matTextureProj;
		float scale = 2.5f;
		float num = -0.95f;
		Vector3 translation = new Vector3(130f, 55f, 0f);
		identity = Matrix.CreateRotationX(-(float)Math.PI / 2f) * Matrix.CreateRotationY((float)Math.PI * num) * Matrix.CreateScale(scale);
		identity.Translation = translation;
		matM203Transform[qIndex] = identity;
		for (int i = 0; i < M203Nader.Meshes.Count; i++)
		{
			drawMesh = M203Nader.Meshes[i];
			for (int j = 0; j < drawMesh.MeshParts.Count; j++)
			{
				drawMeshPart = drawMesh.MeshParts[j];
				drawEffect = drawMeshPart.Effect;
				drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
				drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
				((WeaponEffectParams)drawMeshPart.Tag).texOffset.SetValue(TextureOffset);
				((WeaponEffectParams)drawMeshPart.Tag).TextureShadowMap.SetValue(LevelBaseMenu.shadowRenderTarget);
				((WeaponEffectParams)drawMeshPart.Tag).matTexProj.SetValue(drawTexProj);
				((WeaponEffectParams)drawMeshPart.Tag).matView.SetValue(playerRef.menuView);
				((WeaponEffectParams)drawMeshPart.Tag).matProj.SetValue(playerRef.menuProj);
				((WeaponEffectParams)drawMeshPart.Tag).vecFPSLightPos.SetValue(vecFPSLightPosition[qIndex]);
				((WeaponEffectParams)drawMeshPart.Tag).vecFPSLightColor.SetValue(vecFPSLightColor[qIndex]);
				((WeaponEffectParams)drawMeshPart.Tag).vecMuzzleFlash.SetValue(particles.MuzzleFlash());
				((WeaponEffectParams)drawMeshPart.Tag).matWorld.SetValue(M203NaderTransforms[drawMesh.ParentBone.Index] * matM203Transform[qIndex]);
				drawEffect.CurrentTechnique.Passes[0].Apply();
				drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
			}
		}
	}

	public void Reset()
	{
		for (int i = 0; i < weapon.Count; i++)
		{
			weapon[i].BulletsInMag = 30;
			weapon[i].BulletsTotal = 300;
		}
		for (int j = 0; j < weapon.Count; j++)
		{
			weapon[j].NaderToggled = false;
		}
	}

	public static WeaponPartStruct SetWeaponPart(string n, int index)
	{
		WeaponPartStruct result = new WeaponPartStruct
		{
			BoneIndex = index,
			PartType = WeaponPart.Body
		};
		if (n.Contains("mscope"))
		{
			result.PartType = WeaponPart.Scope;
		}
		else if (n.Contains("scope"))
		{
			result.PartType = WeaponPart.Scope;
		}
		else if (n.Contains("magnify"))
		{
			result.PartType = WeaponPart.Magnify;
		}
		else if (n.Contains("cross"))
		{
			result.PartType = WeaponPart.CrossHairs;
		}
		else if (n.Contains("lens"))
		{
			result.PartType = WeaponPart.Lens;
		}
		else if (n.Contains("reddot"))
		{
			result.PartType = WeaponPart.RedDot;
		}
		else if (n.Contains("m203"))
		{
			result.PartType = WeaponPart.M203;
		}
		else if (n.Contains("rearsight"))
		{
			result.PartType = WeaponPart.RearSight;
		}
		else if (n.Contains("frontsight"))
		{
			result.PartType = WeaponPart.FrontSight;
		}
		else if (n.Contains("rs_pivot"))
		{
			result.PartType = WeaponPart.RS_Pivot;
		}
		else if (n.Contains("fs_pivot"))
		{
			result.PartType = WeaponPart.FS_Pivot;
		}
		else if (n.Contains("muzzle"))
		{
			result.PartType = WeaponPart.Muzzle;
		}
		else if (n.Contains("mag"))
		{
			result.PartType = WeaponPart.Magizine;
		}
		else if (n.Contains("bolt"))
		{
			result.PartType = WeaponPart.Bolt;
		}
		else if (n.Contains("EmissiveLight"))
		{
			result.PartType = WeaponPart.EmissiveLight;
		}
		else if (n.Contains("SelfIllumination"))
		{
			result.PartType = WeaponPart.SelfIllumination;
		}
		return result;
	}
}
