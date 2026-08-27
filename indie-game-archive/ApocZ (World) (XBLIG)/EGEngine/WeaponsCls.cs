using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class WeaponsCls : PropModelBase
{
	public static string[] WeaponsItemsDesc = new string[10] { "Invalid", "Hatchet", "Battle Rifle Takes 7.62 NATO 20 Round Magazine", "Assualt Rifle Takes 5.56 NATO 30 Round Magazine", "Sniper SA Takes .308, 20 Round Magazine", "Soviet MK47 Takes 7.62, 30 Round Magazine", "LMG Takes 5.56 NATO 100 Round Ammo Belt", "Shotgun Takes 8, 20 Guage Shells", "MI Pistol Takes .50, 7 Round Magazine", "Combat Pistol Takes 9mm, 15 Round Magazine" };

	private Vector3 position = Vector3.Zero;

	private Vector3 direction = Vector3.UnitZ;

	private Vector3 right = Vector3.UnitX;

	public static WeaponType[] availableWeapons = new WeaponType[10]
	{
		WeaponType.NumOfWeapons,
		WeaponType.Hatchet,
		WeaponType.European,
		WeaponType.USA,
		WeaponType.Sniper,
		WeaponType.Russian,
		WeaponType.LightMachineGun,
		WeaponType.Shotgun,
		WeaponType.FiftyCal,
		WeaponType.NineMil
	};

	private static EquipmentItemType[] AmmoType = new EquipmentItemType[10]
	{
		EquipmentItemType.Invalid,
		EquipmentItemType.Invalid,
		EquipmentItemType.Nato762Clip,
		EquipmentItemType.ARClip,
		EquipmentItemType.SniperClip,
		EquipmentItemType.u762Clip,
		EquipmentItemType.M249Ammobox,
		EquipmentItemType.ShotgunShells,
		EquipmentItemType.Pistol50Clip,
		EquipmentItemType.PistolM9Clip
	};

	public static WeaponClass[] itemsModels = new WeaponClass[availableWeapons.Length];

	private static bool Initialized = false;

	private static Vector2 MuzzleHeat = Vector2.Zero;

	private static Vector4 TextureOffset = Vector4.Zero;

	public static WeaponType GetWeaponType(int e)
	{
		return availableWeapons[e];
	}

	public static ushort CreateRandom(int seed)
	{
		return (ushort)EndGameEngine.randGenerator.Next(1, availableWeapons.Length);
	}

	public static ushort CreateRandom(int seed, byte range)
	{
		int num = EndGameEngine.randGenerator.Next(1, availableWeapons.Length);
		switch (range)
		{
		case 4:
			num = ((EndGameEngine.randGenerator.Next(0, 100) < 50) ? 7 : 9);
			break;
		case 5:
			if (num == 7 || num == 9)
			{
				num--;
			}
			break;
		default:
			num = EndGameEngine.randGenerator.Next(1, availableWeapons.Length);
			break;
		}
		return (ushort)num;
	}

	public static EquipmentItemType GetAmmoType(WeaponType e)
	{
		for (int i = 1; i < availableWeapons.Length; i++)
		{
			if (availableWeapons[i] == e)
			{
				return AmmoType[i];
			}
		}
		return AmmoType[0];
	}

	public override void Load(string s)
	{
		if (!Initialized)
		{
			LoadWeaponReferences();
		}
	}

	private void LoadWeaponReferences()
	{
		for (int i = 1; i < availableWeapons.Length; i++)
		{
			for (int j = 0; j < FPSWeaponBase.weapon.Count; j++)
			{
				Initialized = true;
				if (FPSWeaponBase.weapon[j].WepType == availableWeapons[i])
				{
					itemsModels[i] = FPSWeaponBase.weapon[j];
					break;
				}
			}
		}
	}

	public void DrawCameraSpace(PlayerBase viewer, int qIndex, int modelIndex)
	{
		if (itemsModels[modelIndex] == null)
		{
			LoadWeaponReferences();
		}
		PropModelBase.tmpMatWorld = Matrix.CreateRotationZ(MathHelper.ToRadians(90f)) * matWorld[qIndex];
		PropModelBase.eyePosition = PropModelBase.tmpMatWorld.Translation;
		PropModelBase.eyePosition.X -= viewer.vecHeadPosition[qIndex].X;
		PropModelBase.eyePosition.Z -= viewer.vecHeadPosition[qIndex].Z;
		PropModelBase.tmpMatWorld.Translation = PropModelBase.eyePosition;
		PropModelBase.tmpMatWorld.Translation = PropModelBase.tmpMatWorld.Translation;
		for (int i = 0; i < itemsModels[modelIndex].model.Meshes.Count; i++)
		{
			PropModelBase.drawMesh = itemsModels[modelIndex].model.Meshes[i];
			if (((WeaponPartStruct)PropModelBase.drawMesh.Tag).PartType == WeaponPart.Body || ((WeaponPartStruct)PropModelBase.drawMesh.Tag).PartType == WeaponPart.Magizine || ((WeaponPartStruct)PropModelBase.drawMesh.Tag).PartType == WeaponPart.Bolt)
			{
				for (int j = 0; j < PropModelBase.drawMesh.MeshParts.Count; j++)
				{
					PropModelBase.drawMeshPart = PropModelBase.drawMesh.MeshParts[j];
					Effect effect = PropModelBase.drawMeshPart.Effect;
					effect.GraphicsDevice.SetVertexBuffer(PropModelBase.drawMeshPart.VertexBuffer, PropModelBase.drawMeshPart.VertexOffset);
					effect.GraphicsDevice.Indices = PropModelBase.drawMeshPart.IndexBuffer;
					((WeaponEffectParams)PropModelBase.drawMeshPart.Tag).texOffset.SetValue(TextureOffset);
					((WeaponEffectParams)PropModelBase.drawMeshPart.Tag).matViewProj.SetValue(viewer.mDataQueue[qIndex].viewProj);
					((WeaponEffectParams)PropModelBase.drawMeshPart.Tag).matWorld.SetValue(itemsModels[modelIndex].transforms[PropModelBase.drawMesh.ParentBone.Index] * PropModelBase.tmpMatWorld);
					effect.Parameters["vecEyePosition"].SetValue(viewer.mDataQueue[qIndex].cameraEyePos);
					effect.Parameters["fMuzzleHeat"].SetValue(MuzzleHeat);
					effect.CurrentTechnique.Passes[10].Apply();
					effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, PropModelBase.drawMeshPart.NumVertices, PropModelBase.drawMeshPart.StartIndex, PropModelBase.drawMeshPart.PrimitiveCount);
				}
			}
		}
	}
}
