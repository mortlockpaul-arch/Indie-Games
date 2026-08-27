using System.Collections.Generic;
using DataContent;
using MaxScriptDefines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PropModel;

namespace EGEngine;

public class InstancePropsManager
{
	public const float MaxDrawDistance = 22000f;

	public static bool MenuIsPuased = false;

	public static List<PropInstanceDrawCls> PropCanInstanceDraw = new List<PropInstanceDrawCls>();

	public static List<PropModelBase> InstancePropsList = new List<PropModelBase>();

	private static string[] PropModelNames = new string[55]
	{
		"building\\WoodBarn00", "building\\DoubleWideMH03", "building\\DoubleWideMH04", "building\\DoubleWideMH06", "building\\DoubleWideMH08", "building\\SingleWideMH03", "building\\SingleWideMH04", "building\\SingleWideMH08", "building\\SingleWideMH12", "building\\RedBrick3Story03",
		"building\\RedBrick3Story00", "building\\EuroGarage00", "building\\CityHall00", "building\\PoliceStation01", "building\\ApartmentHouse48", "building\\Warehouse52", "building\\House2Story00", "building\\House1Story00", "building\\House1Story01", "building\\StorageDepot00",
		"building\\OldBuilding00", "building\\UtilityBuilding00", "building\\GasStation00", "building\\GeneralStore00", "building\\Building66", "building\\Building75", "building\\Building19", "building\\AptHouse107", "building\\AptHouse90", "building\\AptHouse58",
		"building\\BlueWoodBarn00", "building\\Church00", "building\\BlueBuilding00", "building\\WarehouseMill00", "building\\Factory4", "building\\HangarMil00", "Objects\\TownSign00", "Objects\\TownSign01", "Objects\\TownSign02", "Objects\\TownSign03",
		"Objects\\TownSign04", "Objects\\TownSign05", "Objects\\TownSign06", "Objects\\TownSign07", "Objects\\TownSign08", "Objects\\TownSign09", "Objects\\Airplanei76Crash", "vehicles\\OldYellowSchoolBus", "vehicles\\OldRedFlatbed", "vehicles\\OldRedWaterTruck",
		"vehicles\\OldBlueTrailer", "fences\\MilitaryGate00", "fences\\MilitaryWall00", "instanceProps\\BrickWallSmall00", "instanceProps\\WoodFenceSmall00"
	};

	protected bool Loaded;

	private static Vector3 tmpRadVec = Vector3.UnitX;

	private static Vector3 tmpUpdateVec = Vector3.Zero;

	private static Matrix tmpUpdateMat = Matrix.Identity;

	private static Matrix matNoTran = Matrix.Identity;

	private static ModelMesh tmpUpdateMesh = null;

	private static BoundingSphere tmpUpdateSphere = default(BoundingSphere);

	private static Matrix SpereColMat = Matrix.Identity;

	private static Matrix SpereColInvTransform = Matrix.Identity;

	private static Matrix SpereColInvTransformNoTrans = Matrix.Identity;

	private static BoundingSphere SpereColInvSphere = default(BoundingSphere);

	private static Vector3 SpereColRadius = Vector3.Zero;

	private static Vector3 VecUnitY = Vector3.UnitY;

	public virtual void Load()
	{
		if (Loaded)
		{
			return;
		}
		Loaded = true;
		for (int i = 0; i < PropModelNames.Length; i++)
		{
			PropModelBase propModelBase = new PropModelBase();
			propModelBase.Load("models\\" + PropModelNames[i]);
			for (int j = 0; j < propModelBase.propModel.Meshes.Count; j++)
			{
				ModelMesh modelMesh = propModelBase.propModel.Meshes[j];
				if (modelMesh.Name.Contains("AllStreets00"))
				{
					((MeshAttributesParams)modelMesh.Tag).ObjectType = EnumObjectTypes.RenderCastNoShadow;
				}
				else
				{
					((MeshAttributesParams)modelMesh.Tag).ObjectType = EnumObjectTypes.Render;
				}
				if (modelMesh.Name.Contains("MilitaryGate00") || modelMesh.Name.Contains("MilitaryWall00"))
				{
					((MeshAttributesParams)modelMesh.Tag).Opacity = EnumOpacityTypes.AlphaTest;
				}
			}
			InstancePropsList.Add(propModelBase);
			PropInstanceDrawCls propInstanceDrawCls = new PropInstanceDrawCls();
			if (PropModelNames[i].Contains("instanceProps"))
			{
				string n = string.Concat("models\\", PropModelNames[i] + "LOD");
				propInstanceDrawCls.MakeValid(128, n);
			}
			PropCanInstanceDraw.Add(propInstanceDrawCls);
		}
	}

	public virtual int GetIndexForModel(string n)
	{
		for (int i = 0; i < PropModelNames.Length; i++)
		{
			if (PropModelNames[i].Contains(n))
			{
				return i;
			}
		}
		return -1;
	}

	public static void ResetModelsInstances(int qIndex)
	{
		for (int i = 0; i < InstancePropsList.Count; i++)
		{
			if (PropCanInstanceDraw[i].IsValid)
			{
				PropCanInstanceDraw[i].Reset(qIndex);
			}
		}
	}

	public static void UpdateModelsInstances(int qIndex)
	{
		for (int i = 0; i < InstancePropsList.Count; i++)
		{
			if (PropCanInstanceDraw[i].IsValid)
			{
				PropCanInstanceDraw[i].Update(qIndex);
			}
		}
	}

	public static void DrawModelsInstances(PlayerBase viewer, int qIndex)
	{
		for (int i = 0; i < InstancePropsList.Count; i++)
		{
			if (PropCanInstanceDraw[i].IsValid)
			{
				PropCanInstanceDraw[i].Draw(viewer, qIndex);
			}
		}
	}

	public static void DrawModelShadowInstances(PlayerBase viewer, ref Matrix LightViewProj, ref Vector3 lightPos, int qIndex)
	{
		for (int i = 0; i < InstancePropsList.Count; i++)
		{
			if (PropCanInstanceDraw[i].IsValid)
			{
				PropCanInstanceDraw[i].DrawShadowMap(viewer, ref LightViewProj, ref lightPos, qIndex);
			}
		}
	}

	public virtual bool Update(float eTime, int qIndex, PlayerBase playerRef, MeshInstanceData mData, ref Matrix tmpMat)
	{
		bool result = false;
		int referenceId = mData.ReferenceId;
		float num = mData.DistanceSqr[qIndex];
		float num2 = 484000000f;
		if (num < num2)
		{
			tmpUpdateVec = tmpMat.Translation;
			tmpUpdateVec.X -= playerRef.vecHeadPosition[qIndex].X;
			tmpUpdateVec.Z -= playerRef.vecHeadPosition[qIndex].Z;
			tmpMat.Translation = tmpUpdateVec;
			if (PropCanInstanceDraw[referenceId].IsValid && num < 25000000f)
			{
				tmpUpdateMesh = PropCanInstanceDraw[referenceId].propModel.Meshes[0];
				tmpUpdateSphere = tmpUpdateMesh.BoundingSphere;
				tmpUpdateSphere.Center += tmpMat.Translation;
				ContainmentType result2 = ContainmentType.Disjoint;
				playerRef.bFrustum[qIndex].Contains(ref tmpUpdateSphere, out result2);
				if (result2 == ContainmentType.Contains || result2 == ContainmentType.Intersects)
				{
					PropCanInstanceDraw[referenceId].AddShadow(qIndex, ref tmpMat);
				}
			}
			if (PropCanInstanceDraw[referenceId].IsValid && num > 1000000f)
			{
				tmpUpdateMesh = PropCanInstanceDraw[referenceId].propModel.Meshes[0];
				tmpUpdateSphere = tmpUpdateMesh.BoundingSphere;
				tmpUpdateSphere.Center += tmpMat.Translation;
				ContainmentType result3 = ContainmentType.Disjoint;
				playerRef.bFrustum[qIndex].Contains(ref tmpUpdateSphere, out result3);
				if (result3 == ContainmentType.Contains || result3 == ContainmentType.Intersects)
				{
					PropCanInstanceDraw[referenceId].Add(qIndex, ref tmpMat);
				}
			}
			else
			{
				for (int i = 0; i < InstancePropsList[referenceId].propModel.Meshes.Count; i++)
				{
					tmpUpdateMesh = InstancePropsList[referenceId].propModel.Meshes[i];
					tmpUpdateSphere.Center = Vector3.Transform(tmpUpdateMesh.BoundingSphere.Center, tmpMat);
					tmpUpdateSphere.Radius = Vector3.Transform(tmpRadVec * tmpUpdateMesh.BoundingSphere.Radius, tmpMat).Length();
					ContainmentType result4 = ContainmentType.Disjoint;
					playerRef.bFrustum[qIndex].Contains(ref tmpUpdateSphere, out result4);
					if (result4 == ContainmentType.Contains || result4 == ContainmentType.Intersects)
					{
						result = true;
						break;
					}
				}
			}
		}
		return result;
	}

	public virtual void Draw(PlayerBase viewer, int qIndex)
	{
	}

	public virtual void DrawCameraSpace(PlayerBase viewer, int qIndex, int instIndex, ref Matrix tmpMat, float lod)
	{
		ref Matrix reference = ref InstancePropsList[instIndex].matWorld[qIndex];
		reference = tmpMat;
		InstancePropsList[instIndex].DrawCameraSpace(viewer, qIndex, lod);
	}

	public virtual void DrawShadowMap(PlayerBase viewer, ref Matrix LightViewProj, ref Vector3 lightPos, ref Matrix tmpMat, int instIndex, int qIndex, bool lod)
	{
		if (!PropCanInstanceDraw[instIndex].IsValid)
		{
			ref Matrix reference = ref InstancePropsList[instIndex].matWorld[qIndex];
			reference = tmpMat;
			InstancePropsList[instIndex].DrawShadowMap(viewer, ref LightViewProj, ref lightPos, qIndex, InstancePropsList[instIndex].LODIndex != -1);
		}
	}

	public virtual void SphereCollision(int qIndex, ref BoundingSphere sphere, int instIndex, ref Matrix tmpMatWorld, ref bool onWalkable, ref bool inCollision)
	{
		CollisionData collisionData = ((MeshUserData)InstancePropsList[instIndex].propModel.Tag).collisionData;
		SpereColMat = collisionData.transform * tmpMatWorld;
		Matrix.Invert(ref SpereColMat, out SpereColInvTransform);
		SpereColInvTransformNoTrans = SpereColInvTransform;
		SpereColInvTransformNoTrans.Translation = Vector3.Zero;
		SpereColInvSphere = sphere;
		Vector3.Transform(ref SpereColInvSphere.Center, ref SpereColInvTransform, out SpereColInvSphere.Center);
		SpereColRadius.Y = 0f;
		SpereColRadius.Z = 0f;
		SpereColRadius.X = SpereColInvSphere.Radius;
		Vector3.Transform(ref SpereColRadius, ref SpereColInvTransformNoTrans, out SpereColRadius);
		SpereColInvSphere.Radius = SpereColRadius.Length();
		Vector3.Transform(ref VecUnitY, ref SpereColInvTransformNoTrans, out PropModelBase.ObjectSpaceUpVector);
		PropModelBase.ObjectSpaceUpVector.Normalize();
		if (InstancePropsList[instIndex].SphereCollision(ref SpereColInvSphere, qIndex, ref onWalkable))
		{
			inCollision = true;
			Vector3.Transform(ref SpereColInvSphere.Center, ref SpereColMat, out sphere.Center);
		}
	}

	public virtual bool RayCast(int qIndex, ref Ray ray, int instIndex, ref Matrix tmpMatWorld, ref Vector3 hitPosition, ref Vector3 hitNormal, ref float hitDistance)
	{
		bool result = false;
		if (instIndex >= 0 || instIndex < InstancePropsList.Count)
		{
			CollisionData collisionData = ((MeshUserData)InstancePropsList[instIndex].propModel.Tag).collisionData;
			Matrix matrix = collisionData.transform * tmpMatWorld;
			Matrix matrix2 = Matrix.Invert(matrix);
			Matrix matrix3 = matrix2;
			matrix3.Translation = Vector3.Zero;
			Ray ray2 = ray;
			ray2.Position = Vector3.Transform(ray2.Position, matrix2);
			ray2.Direction = Vector3.Transform(ray2.Direction, matrix3);
			if (InstancePropsList[instIndex].RayCast(ref ray2, qIndex, ref hitPosition, ref hitNormal, ref hitDistance))
			{
				result = true;
				Matrix matrix4 = matrix;
				matrix4.Translation = Vector3.Zero;
				hitPosition = Vector3.Transform(hitPosition, matrix);
				hitNormal = Vector3.Transform(hitNormal, matrix);
				hitNormal.Normalize();
			}
		}
		return result;
	}
}
