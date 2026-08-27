using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class WeaponClass : WeaponData
{
	public int BulletsInMag = 30;

	public int BulletsTotal = 300;

	public int BulletsMagMax = 30;

	public bool NaderToggled;

	public ItemCls InventoryItemRef;

	public Texture2D BaseWeaponSkin;

	public Texture2D BaseWeaponSkinIcon;

	public static Texture2D[] GunSkins = new Texture2D[8];

	private static string[] skinName = new string[8] { "deagle", "desilver", "dewarsaw", "detiger", "degold", "m4blue", "arx160red", "scarltiger" };

	public static Texture2D[] GunSkinIcons = new Texture2D[8];

	private static string[] skinIconName = new string[8] { "degunmetal", "desilver", "dewarsaw", "detiger", "degold", "blue", "red", "tiger" };

	private static bool OneOffInit = true;

	public WeaponClass()
	{
	}

	public WeaponClass(WeaponData data)
		: base(data)
	{
	}

	public void Set()
	{
		model = EndGameEngine.GameAssetMgr.Load<Model>("models\\weapons\\" + Resource);
		transforms = new Matrix[model.Bones.Count];
		model.CopyAbsoluteBoneTransformsTo(transforms);
		int num = 0;
		foreach (ModelMesh mesh in model.Meshes)
		{
			mesh.Tag = FPSWeaponBase.SetWeaponPart(mesh.Name, num++);
			foreach (ModelMeshPart meshPart in mesh.MeshParts)
			{
				meshPart.Tag = new WeaponEffectParams(meshPart.Effect, this);
			}
		}
		if (OneOffInit && !PlayerBase.ApocalypseZ_Hack)
		{
			OneOffInit = false;
			for (int i = 0; i < 8; i++)
			{
				GunSkins[i] = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\weapons\\" + skinName[i]);
				GunSkinIcons[i] = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\weapons\\skinicons\\" + skinIconName[i]);
			}
		}
		if (!PlayerBase.ApocalypseZ_Hack)
		{
			if (Resource == "m4")
			{
				BaseWeaponSkin = model.Meshes[3].MeshParts[0].Effect.Parameters["TexDiffuse"].GetValueTexture2D();
			}
			else
			{
				BaseWeaponSkin = model.Meshes[0].MeshParts[0].Effect.Parameters["TexDiffuse"].GetValueTexture2D();
			}
		}
		if (!PlayerBase.ApocalypseZ_Hack)
		{
			if (Resource == "scarl")
			{
				BaseWeaponSkinIcon = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\weapons\\skinicons\\scarl");
			}
			else
			{
				BaseWeaponSkinIcon = GunSkinIcons[0];
			}
		}
		BulletsTotal = MaxAmmo;
		BulletsInMag = MaxAmmoInClip;
		BulletsMagMax = MaxAmmoInClip;
	}

	public Texture2D GetWeaponSkinIcon(WeaponSkin e)
	{
		if (e == WeaponSkin.GunMetal)
		{
			return BaseWeaponSkinIcon;
		}
		return GunSkinIcons[(int)e];
	}

	public Matrix GetBoneTransform(WeaponPart wepPart)
	{
		for (int i = 0; i < model.Meshes.Count; i++)
		{
			if (((WeaponPartStruct)model.Meshes[i].Tag).PartType == wepPart)
			{
				return transforms[model.Meshes[i].ParentBone.Index];
			}
		}
		return Matrix.Identity;
	}

	public void ResetSpawn()
	{
		BulletsTotal = MaxAmmo;
		BulletsMagMax = MaxAmmoInClip;
		BulletsInMag = MaxAmmoInClip;
		NaderToggled = false;
	}

	public void Reload()
	{
		int num = MaxAmmoInClip - BulletsInMag;
		if (BulletsTotal >= num)
		{
			BulletsInMag += num;
			BulletsTotal -= num;
		}
		else
		{
			BulletsInMag += BulletsTotal;
			BulletsTotal = 0;
		}
	}

	public void SetAttachments(WeaponAttachment e)
	{
		if (e == WeaponAttachment.NadeLauncher)
		{
			if (AttachmentTwo != WeaponAttachment.Nothing)
			{
				AttachmentTwo = WeaponAttachment.Nothing;
			}
			else
			{
				AttachmentTwo = e;
			}
		}
		else if (Attachment == e)
		{
			Attachment = WeaponAttachment.Nothing;
		}
		else
		{
			Attachment = e;
		}
	}

	public void SetSkin(WeaponSkin skin)
	{
		foreach (ModelMesh mesh in model.Meshes)
		{
			foreach (ModelMeshPart meshPart in mesh.MeshParts)
			{
				Texture2D value = GunSkins[(int)skin];
				if (skin == WeaponSkin.GunMetal)
				{
					value = BaseWeaponSkin;
				}
				if (WepType == WeaponType.USA)
				{
					if (mesh.Name == "BODY")
					{
						meshPart.Effect.Parameters["TexDiffuse"].SetValue(value);
					}
				}
				else
				{
					meshPart.Effect.Parameters["TexDiffuse"].SetValue(value);
				}
			}
		}
	}

	public static Matrix GetBoneTransform(Model m, Matrix[] t, WeaponPart wepPart)
	{
		for (int i = 0; i < m.Meshes.Count; i++)
		{
			if (((WeaponPartStruct)m.Meshes[i].Tag).PartType == wepPart)
			{
				return t[m.Meshes[i].ParentBone.Index];
			}
		}
		return Matrix.Identity;
	}
}
