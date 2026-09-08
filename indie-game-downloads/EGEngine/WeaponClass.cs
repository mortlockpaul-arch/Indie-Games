using System;
using System.Collections.ObjectModel;
using System.IO;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class WeaponClass : WeaponData
{
	private static int WeaponLayoutDebugCount;

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
		if (WeaponLayoutDebugCount < 24)
		{
			foreach (ModelMesh mesh in model.Meshes)
			{
				foreach (ModelMeshPart meshPart in mesh.MeshParts)
				{
					if (WeaponLayoutDebugCount++ >= 24)
					{
						break;
					}
					string text = string.Join(", ", Array.ConvertAll(meshPart.VertexBuffer.VertexDeclaration.GetVertexElements(), (VertexElement e) => e.ToString()));
					File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "ApocZ-weapon-materials.log"), "asset=models\\weapons\\" + Resource + " mesh=" + mesh.Name + " stride=" + meshPart.VertexBuffer.VertexDeclaration.VertexStride + " declaration=[" + text + "]" + Environment.NewLine);
					for (int num = 0; num < meshPart.Effect.Parameters.Count; num++)
					{
						EffectParameter effectParameter = meshPart.Effect.Parameters[num];
						if (effectParameter.ParameterType == EffectParameterType.Texture || effectParameter.ParameterType == EffectParameterType.Texture2D)
						{
							Texture2D valueTexture2D = effectParameter.GetValueTexture2D();
							if (valueTexture2D != null)
							{
								File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "ApocZ-weapon-materials.log"), "  texture=" + effectParameter.Name + " size=" + valueTexture2D.Width + "x" + valueTexture2D.Height + Environment.NewLine);
							}
						}
					}
				}
			}
		}
		transforms = new Matrix[((ReadOnlyCollection<ModelBone>)model.Bones).Count];
		model.CopyAbsoluteBoneTransformsTo(transforms);
		int num2 = 0;
		ModelMeshCollection.Enumerator enumerator3 = model.Meshes.GetEnumerator();
		try
		{
			while (enumerator3.MoveNext())
			{
				ModelMesh current3 = enumerator3.Current;
				current3.Tag = FPSWeaponBase.SetWeaponPart(current3.Name, num2++);
				ModelMeshPartCollection.Enumerator enumerator4 = current3.MeshParts.GetEnumerator();
				try
				{
					while (enumerator4.MoveNext())
					{
						ModelMeshPart current4 = enumerator4.Current;
						current4.Tag = new WeaponEffectParams(current4.Effect, this);
					}
				}
				finally
				{
					enumerator4.Dispose();
				}
			}
		}
		finally
		{
			enumerator3.Dispose();
		}
		if (OneOffInit && !PlayerBase.ApocalypseZ_Hack)
		{
			OneOffInit = false;
			for (int num3 = 0; num3 < 8; num3++)
			{
				GunSkins[num3] = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\weapons\\" + skinName[num3]);
				GunSkinIcons[num3] = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\weapons\\skinicons\\" + skinIconName[num3]);
			}
		}
		if (!PlayerBase.ApocalypseZ_Hack)
		{
			if (Resource == "m4")
			{
				BaseWeaponSkin = ((ReadOnlyCollection<ModelMeshPart>)((ReadOnlyCollection<ModelMesh>)model.Meshes)[3].MeshParts)[0].Effect.Parameters["TexDiffuse"].GetValueTexture2D();
			}
			else
			{
				BaseWeaponSkin = ((ReadOnlyCollection<ModelMeshPart>)((ReadOnlyCollection<ModelMesh>)model.Meshes)[0].MeshParts)[0].Effect.Parameters["TexDiffuse"].GetValueTexture2D();
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
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)model.Meshes).Count; i++)
		{
			if (((WeaponPartStruct)((ReadOnlyCollection<ModelMesh>)model.Meshes)[i].Tag).PartType == wepPart)
			{
				return transforms[((ReadOnlyCollection<ModelMesh>)model.Meshes)[i].ParentBone.Index];
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
		ModelMeshCollection.Enumerator enumerator = model.Meshes.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ModelMesh current = enumerator.Current;
				ModelMeshPartCollection.Enumerator enumerator2 = current.MeshParts.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						ModelMeshPart current2 = enumerator2.Current;
						Texture2D texture2D = GunSkins[(int)skin];
						if (skin == WeaponSkin.GunMetal)
						{
							texture2D = BaseWeaponSkin;
						}
						if (WepType == WeaponType.USA)
						{
							if (current.Name == "BODY")
							{
								current2.Effect.Parameters["TexDiffuse"].SetValue(texture2D);
							}
						}
						else
						{
							current2.Effect.Parameters["TexDiffuse"].SetValue(texture2D);
						}
					}
				}
				finally
				{
					enumerator2.Dispose();
				}
			}
		}
		finally
		{
			enumerator.Dispose();
		}
	}

	public static Matrix GetBoneTransform(Model m, Matrix[] t, WeaponPart wepPart)
	{
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)m.Meshes).Count; i++)
		{
			if (((WeaponPartStruct)((ReadOnlyCollection<ModelMesh>)m.Meshes)[i].Tag).PartType == wepPart)
			{
				return t[((ReadOnlyCollection<ModelMesh>)m.Meshes)[i].ParentBone.Index];
			}
		}
		return Matrix.Identity;
	}
}
