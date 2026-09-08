using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DataContent;

[ContentSerializerRuntimeType("DataContent.WeaponData, DataContent")]
public class WeaponData
{
	[ContentSerializerIgnore]
	public Model model;

	public string Name;

	public string Resource;

	public Matrix[] transforms;

	public float FOV;

	public float SightedFOV;

	public float ReflectivePower;

	public float SpecularPower;

	public float FireRate;

	public float MaxRangeRatio;

	public float DamageRatio;

	public WeaponFireMode fireMode;

	public Vector2 BulletAccuraceyRecoil;

	public Vector4 Recoil;

	public Vector3 WepOffset;

	public Vector3 WepOffsetSighted;

	public Vector3 WepCoOpOffset;

	public WeaponAttachment Attachment;

	public WeaponAttachment AttachmentTwo;

	public WeaponType WepType;

	public WeaponCategory WepCategory;

	public WeaponSlot WepSlot;

	public WeaponAnim IdleAnim;

	public WeaponAnim SightedAnim;

	public WeaponAnim WalkAnim;

	public WeaponAnim RunAnim;

	public WeaponAnim PullOutAnim;

	public WeaponAnim PutawayAnim;

	public WeaponAnim ReloadAnim;

	public WeaponAnim JumpAnim;

	public WeaponAnim CoOpCrouchAnim;

	public WeaponAnim CoOpCrouchWalkAnim;

	public WeaponAnim CoOpCrouchWalkBackAnim;

	public WeaponAnim CoOpIdleAnim;

	public WeaponAnim CoOpSightedAnim;

	public WeaponAnim CoOpWalkAnim;

	public WeaponAnim CoOpWalkBackAnim;

	public WeaponAnim CoOpWalkSightedAnim;

	public WeaponAnim CoOpWalkStrafeLeftAnim;

	public WeaponAnim CoOpWalkStrafeRightAnim;

	public WeaponAnim CoOpRunAnim;

	public WeaponAnim CoOpPullOutAnim;

	public WeaponAnim CoOpPutawayAnim;

	public WeaponAnim CoOpReloadAnim;

	public WeaponAnim CoOpJumpAnim;

	public string WeaponReloadSound;

	public string WeaponShotSound0;

	public int MaxAmmo;

	public int MaxAmmoInClip;

	public List<WeaponSkin> AvailableSkins;

	public List<WeaponAttachment> AttachmentList;

	public WeaponData()
	{
	}

	public WeaponData(WeaponData data)
	{
		WepType = data.WepType;
		WepCategory = data.WepCategory;
		WepSlot = data.WepSlot;
		Name = data.Name;
		Resource = data.Resource;
		FOV = data.FOV;
		SightedFOV = data.SightedFOV;
		ReflectivePower = data.ReflectivePower;
		SpecularPower = data.SpecularPower;
		Recoil = data.Recoil;
		BulletAccuraceyRecoil = data.BulletAccuraceyRecoil;
		FireRate = data.FireRate;
		MaxRangeRatio = data.MaxRangeRatio;
		DamageRatio = data.DamageRatio;
		fireMode = data.fireMode;
		IdleAnim = data.IdleAnim;
		SightedAnim = data.SightedAnim;
		WalkAnim = data.WalkAnim;
		RunAnim = data.RunAnim;
		PullOutAnim = data.PullOutAnim;
		PutawayAnim = data.PutawayAnim;
		ReloadAnim = data.ReloadAnim;
		JumpAnim = data.JumpAnim;
		CoOpCrouchAnim = data.CoOpCrouchAnim;
		CoOpCrouchWalkAnim = data.CoOpCrouchWalkAnim;
		CoOpCrouchWalkBackAnim = data.CoOpCrouchWalkBackAnim;
		CoOpIdleAnim = data.CoOpIdleAnim;
		CoOpSightedAnim = data.CoOpSightedAnim;
		CoOpWalkAnim = data.CoOpWalkAnim;
		CoOpWalkBackAnim = data.CoOpWalkBackAnim;
		CoOpWalkSightedAnim = data.CoOpWalkSightedAnim;
		CoOpWalkStrafeLeftAnim = data.CoOpWalkStrafeLeftAnim;
		CoOpWalkStrafeRightAnim = data.CoOpWalkStrafeRightAnim;
		CoOpRunAnim = data.CoOpRunAnim;
		CoOpPullOutAnim = data.CoOpPullOutAnim;
		CoOpPutawayAnim = data.CoOpPutawayAnim;
		CoOpReloadAnim = data.CoOpReloadAnim;
		CoOpJumpAnim = data.CoOpJumpAnim;
		WepOffset = data.WepOffset;
		WepOffsetSighted = data.WepOffsetSighted;
		WepCoOpOffset = data.WepCoOpOffset;
		WeaponReloadSound = data.WeaponReloadSound;
		WeaponShotSound0 = data.WeaponShotSound0;
		MaxAmmo = data.MaxAmmo;
		MaxAmmoInClip = data.MaxAmmoInClip;
		AvailableSkins = data.AvailableSkins;
		AttachmentList = data.AttachmentList;
		Attachment = AttachmentList[0];
		AttachmentTwo = WeaponAttachment.Nothing;
	}
}
