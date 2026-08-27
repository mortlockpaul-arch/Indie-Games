using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DataContent;
using MaxScriptDefines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PropModel;

namespace EGEngine;

public class WorldAreaCls : PropModelBase
{
	private const string PathingDataName = "TrailerPark00NavMeshXbox360";

	private const int NumRandomOffsets = 8;

	public dtStatNavMesh.dtStatNavMeshHeader PathingData;

	public string Name = "UnKnown";

	public new bool[] InFrustum = new bool[2];

	private int NumOfItemPoints;

	private int NumOfSpawnPoints;

	private MeshInstanceData[] InstanceMeshes;

	private InstancePropsManager InstanceModelMgr = new InstancePropsManager();

	private List<BoundingBox> WalkableBBox = new List<BoundingBox>();

	public int NumZombieSpawns;

	public List<Vector3> ZombieSpawnPos = new List<Vector3>();

	private List<SpawnPositionData> ItemSpawnPositions;

	public Vector3 Center = Vector3.Zero;

	public Vector3 Min = Vector3.Zero;

	public Vector3 Max = Vector3.Zero;

	private CollisionGrid CollisionGridStruct = default(CollisionGrid);

	private int[] ItemSpawnOffsetIndice;

	private static Vector3[] ItemPlacmentRandomOffsets = new Vector3[8]
	{
		new Vector3(32f, 0f, 0f),
		new Vector3(-32f, 0f, 0f),
		new Vector3(32f, 0f, 32f),
		new Vector3(-32f, 0f, 32f),
		new Vector3(32f, 0f, -32f),
		new Vector3(-32f, 0f, -32f),
		new Vector3(0f, 0f, 32f),
		new Vector3(0f, 0f, -32f)
	};

	private static int GenericItemPlaceRandIndice = 0;

	private int spawnIndex;

	private int itemSpawnIndex0;

	private static Vector3 tmpVecTo = Vector3.Zero;

	private static Matrix tmpUpdateMat = Matrix.Identity;

	private static BoundingBox tmpBBox = default(BoundingBox);

	private static Vector3 firePos = Vector3.Zero;

	private static Color fireColor = Color.Yellow;

	private static Matrix tmpDrawMat = Matrix.Identity;

	private static Vector4 SearchGridExtents = Vector4.Zero;

	private static Matrix SphereColMat = Matrix.Identity;

	private static BoundingBox bbox = default(BoundingBox);

	private static Matrix raycastTmpMat = Matrix.Identity;

	public int GetNumOfItemPoints
	{
		get
		{
			return NumOfItemPoints;
		}
		set
		{
		}
	}

	public static Vector3 GetItemPlaceRandOffset()
	{
		GenericItemPlaceRandIndice++;
		if (GenericItemPlaceRandIndice >= 8)
		{
			GenericItemPlaceRandIndice = 0;
		}
		return ItemPlacmentRandomOffsets[GenericItemPlaceRandIndice];
	}

	public void Reset()
	{
	}

	public void Finalize()
	{
		try
		{
			Vector3 normal = Vector3.UnitY;
			Vector3 position = matWorld[0].Translation;
			position.Y = HeightMapPhysics.GetHeight(ref position, out normal);
			matWorld[0].Translation = position;
			matWorld[1].Translation = position;
			Vector3[] array = new Vector3[8]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			};
			FinalizeAdjustToTerrain();
			MeshUserData meshUserData = propModel.Tag as MeshUserData;
			InstanceMeshes = meshUserData.instanceData;
			for (int i = 0; i < InstanceMeshes.Length; i++)
			{
				int length = InstanceMeshes[i].Name.IndexOf("_");
				string n = InstanceMeshes[i].Name.Substring(0, length);
				InstanceMeshes[i].ReferenceId = InstanceModelMgr.GetIndexForModel(n);
				Vector3 position2 = InstanceMeshes[i].matWorld.Translation;
				position2 += matWorld[0].Translation;
				position2.Y = HeightMapPhysics.GetHeight(ref position2);
				InstanceMeshes[i].matWorld.Translation = position2 - matWorld[0].Translation;
				if (InstancePropsManager.InstancePropsList[InstanceMeshes[i].ReferenceId].propModel.Tag != null)
				{
					CollisionData collisionData = ((MeshUserData)InstancePropsManager.InstancePropsList[InstanceMeshes[i].ReferenceId].propModel.Tag).collisionData;
					Matrix matrix = InstanceMeshes[i].matWorld;
					matrix.Translation += matWorld[0].Translation;
					matrix = collisionData.transform * matrix;
					array[0].X = collisionData.bBox.Min.X;
					array[0].Y = collisionData.bBox.Min.Y;
					array[0].Z = collisionData.bBox.Min.Z;
					array[1].X = collisionData.bBox.Max.X;
					array[1].Y = collisionData.bBox.Min.Y;
					array[1].Z = collisionData.bBox.Max.Z;
					array[2].X = collisionData.bBox.Min.X;
					array[2].Y = collisionData.bBox.Min.Y;
					array[2].Z = collisionData.bBox.Max.Z;
					array[3].X = collisionData.bBox.Max.X;
					array[3].Y = collisionData.bBox.Min.Y;
					array[3].Z = collisionData.bBox.Min.Z;
					array[4].X = collisionData.bBox.Min.X;
					array[4].Y = collisionData.bBox.Max.Y;
					array[4].Z = collisionData.bBox.Min.Z;
					array[5].X = collisionData.bBox.Max.X;
					array[5].Y = collisionData.bBox.Max.Y;
					array[5].Z = collisionData.bBox.Max.Z;
					array[6].X = collisionData.bBox.Min.X;
					array[6].Y = collisionData.bBox.Max.Y;
					array[6].Z = collisionData.bBox.Max.Z;
					array[7].X = collisionData.bBox.Max.X;
					array[7].Y = collisionData.bBox.Max.Y;
					array[7].Z = collisionData.bBox.Min.Z;
					for (int j = 0; j < 8; j++)
					{
						ref Vector3 reference = ref array[j];
						reference = Vector3.Transform(array[j], matrix);
						Min.X = ((Min.X > array[j].X) ? array[j].X : Min.X);
						Min.Y = ((Min.Y > array[j].Y) ? array[j].Y : Min.Y);
						Min.Z = ((Min.Z > array[j].Z) ? array[j].Z : Min.Z);
						Max.X = ((Max.X < array[j].X) ? array[j].X : Max.X);
						Max.Y = ((Max.Y < array[j].Y) ? array[j].Y : Max.Y);
						Max.Z = ((Max.Z < array[j].Z) ? array[j].Z : Max.Z);
					}
				}
			}
			Min.X -= 500f;
			Min.Z -= 500f;
			Max.X += 500f;
			Max.Z += 500f;
			Center = Min + (Max - Min);
			BoundingBox b = default(BoundingBox);
			CollisionGridStruct.Create(Min, Max);
			for (int k = 0; k < InstanceMeshes.Length; k++)
			{
				if (InstancePropsManager.InstancePropsList[InstanceMeshes[k].ReferenceId].propModel.Tag == null)
				{
					continue;
				}
				CollisionData collisionData2 = ((MeshUserData)InstancePropsManager.InstancePropsList[InstanceMeshes[k].ReferenceId].propModel.Tag).collisionData;
				Matrix matrix2 = InstanceMeshes[k].matWorld;
				matrix2.Translation += matWorld[0].Translation;
				matrix2 = collisionData2.transform * matrix2;
				array[0].X = collisionData2.bBox.Min.X;
				array[0].Y = collisionData2.bBox.Min.Y;
				array[0].Z = collisionData2.bBox.Min.Z;
				array[1].X = collisionData2.bBox.Max.X;
				array[1].Y = collisionData2.bBox.Min.Y;
				array[1].Z = collisionData2.bBox.Max.Z;
				array[2].X = collisionData2.bBox.Min.X;
				array[2].Y = collisionData2.bBox.Min.Y;
				array[2].Z = collisionData2.bBox.Max.Z;
				array[3].X = collisionData2.bBox.Max.X;
				array[3].Y = collisionData2.bBox.Min.Y;
				array[3].Z = collisionData2.bBox.Min.Z;
				array[4].X = collisionData2.bBox.Min.X;
				array[4].Y = collisionData2.bBox.Max.Y;
				array[4].Z = collisionData2.bBox.Min.Z;
				array[5].X = collisionData2.bBox.Max.X;
				array[5].Y = collisionData2.bBox.Max.Y;
				array[5].Z = collisionData2.bBox.Max.Z;
				array[6].X = collisionData2.bBox.Min.X;
				array[6].Y = collisionData2.bBox.Max.Y;
				array[6].Z = collisionData2.bBox.Max.Z;
				array[7].X = collisionData2.bBox.Max.X;
				array[7].Y = collisionData2.bBox.Max.Y;
				array[7].Z = collisionData2.bBox.Min.Z;
				b.Min.X = float.MaxValue;
				b.Min.Z = float.MaxValue;
				b.Max.X = float.MinValue;
				b.Max.Z = float.MinValue;
				for (int l = 0; l < 8; l++)
				{
					ref Vector3 reference2 = ref array[l];
					reference2 = Vector3.Transform(array[l], matrix2);
					if (b.Min.X > array[l].X)
					{
						b.Min.X = array[l].X;
					}
					if (b.Min.Z > array[l].Z)
					{
						b.Min.Z = array[l].Z;
					}
					if (b.Max.X < array[l].X)
					{
						b.Max.X = array[l].X;
					}
					if (b.Max.Z < array[l].Z)
					{
						b.Max.Z = array[l].Z;
					}
				}
				CollisionGridStruct.Add(InstanceMeshes[k], b);
			}
		}
		catch (Exception threadExceptionArgument)
		{
			EndGameEngine.ThreadExceptionArgument = threadExceptionArgument;
		}
	}

	public void Load(string n, Matrix matworldNew, float contourAdjustRad, float spawnAreaMargin)
	{
		Name = n;
		InFrustum[0] = false;
		InFrustum[1] = false;
		base.Load("level\\" + n);
		matWorld[0] = matworldNew;
		matWorld[1] = matworldNew;
		string text = n + "NavMeshXbox360";
		PathingData = LevelBaseMenu.NavigationMesh.LoadNavigationMesh(EndGameEngine.GameAssetMgr.RootDirectory + "\\level\\" + text, matWorld[0].Translation);
		LevelBaseMenu.NavigationMesh.PickExtents.Y = 20000f;
		Min = PathingData.bmin + PathingData.worldOffset;
		Max = PathingData.bmax + PathingData.worldOffset;
		TransformBoundingSphere(matWorld[0]);
		AdjustTerrainWithContourData(contourAdjustRad);
		for (int i = 0; i < NumZombieSpawns; i++)
		{
			ushort num = 0;
			Vector3 position = Vector3.Zero;
			Vector3 min = Min;
			Vector3 max = Max;
			min.X += spawnAreaMargin;
			min.Z += spawnAreaMargin;
			max.X -= spawnAreaMargin;
			max.Z -= spawnAreaMargin;
			do
			{
				position.X = min.X + (float)EndGameEngine.randGenerator.NextDouble() * (max.X - min.X);
				position.Z = min.Z + (float)EndGameEngine.randGenerator.NextDouble() * (max.Z - min.Z);
				position.Y = 0f;
				position -= PathingData.worldOffset;
			}
			while (LevelBaseMenu.NavigationMesh.GetValidPathPosition(ref position, ref LevelBaseMenu.NavigationMesh.PickExtents) == 0);
			position.X += PathingData.worldOffset.X;
			position.Z += PathingData.worldOffset.Z;
			position.Y = HeightMapPhysics.GetHeight(ref position);
			ZombieSpawnPos.Add(position);
		}
	}

	public void AdjustTerrainWithContourData(float contourAdjustRad)
	{
		for (int i = 0; i < propModel.Meshes.Count; i++)
		{
			ModelMesh modelMesh = propModel.Meshes[i];
			if (modelMesh.Name.Contains("TerrainFlatten_Contour_"))
			{
				((MeshAttributesParams)modelMesh.Tag).ObjectType = EnumObjectTypes.NoRender;
				HeightMapPhysics.ForceHeightDataToVertice(modelMesh.MeshParts[0], propTransforms[propModel.Meshes[i].ParentBone.Index], matWorld[0], Matrix.Identity, noTrees: false, noGrass: false, modelMesh.Name.Contains("_Iterate_"), contourAdjustRad);
			}
		}
		for (int j = 0; j < propModel.Meshes.Count; j++)
		{
			ModelMesh modelMesh2 = propModel.Meshes[j];
			if (modelMesh2.Name.Contains("_RelaxHMtoVert_"))
			{
				Matrix matrix = matWorld[0];
				Vector3 translation = matrix.Translation;
				translation.Y = 0f;
				matrix.Translation = translation;
				bool noTrees = modelMesh2.Name.Contains("_NoTrees_");
				bool noGrass = modelMesh2.Name.Contains("_NoGrass_");
				((MeshAttributesParams)modelMesh2.Tag).ObjectType = EnumObjectTypes.NoRender;
				for (int k = 0; k < modelMesh2.MeshParts.Count; k++)
				{
					HeightMapPhysics.RelaxHeightMapAtVertice(modelMesh2.MeshParts[k], propTransforms[propModel.Meshes[j].ParentBone.Index], matWorld[0], noTrees, noGrass, Matrix.Identity);
				}
			}
		}
	}

	public unsafe void FinalizeAdjustToTerrain()
	{
		for (int i = 0; i < propModel.Meshes.Count; i++)
		{
			ModelMesh modelMesh = propModel.Meshes[i];
			if (modelMesh.Name.Contains("_ATEST_"))
			{
				((MeshAttributesParams)modelMesh.Tag).ObjectType = EnumObjectTypes.RenderCastNoShadow;
				((MeshAttributesParams)modelMesh.Tag).Opacity = EnumOpacityTypes.AlphaTest;
			}
			if (modelMesh.Name.Contains("_SetHeightToHM_"))
			{
				Matrix matrix = matWorld[0];
				Vector3 translation = matrix.Translation;
				translation.Y = 0f;
				matrix.Translation = translation;
				((MeshAttributesParams)modelMesh.Tag).ObjectType = EnumObjectTypes.NumberOf;
				for (int j = 0; j < modelMesh.MeshParts.Count; j++)
				{
					BoundingBox boundingBox = new BoundingBox
					{
						Min = Min,
						Max = Max
					};
					HeightMapPhysics.SetVerticeToHeightData(modelMesh.MeshParts[j], propTransforms[propModel.Meshes[i].ParentBone.Index], matWorld[0], Matrix.Identity, transformToWorldSpace: false, resetNormals: true, noTrees: true, noGrass: true, ref boundingBox);
					Min.X = ((Min.X > boundingBox.Min.X) ? boundingBox.Min.X : Min.X);
					Min.Y = ((Min.Y > boundingBox.Min.Y) ? boundingBox.Min.Y : Min.Y);
					Min.Z = ((Min.Z > boundingBox.Min.Z) ? boundingBox.Min.Z : Min.Z);
					Max.X = ((Max.X < boundingBox.Max.X) ? boundingBox.Max.X : Max.X);
					Max.Y = ((Max.Y < boundingBox.Max.Y) ? boundingBox.Max.Y : Max.Y);
					Max.Z = ((Max.Z < boundingBox.Max.Z) ? boundingBox.Max.Z : Max.Z);
				}
			}
		}
		for (int k = 0; k < propModel.Meshes.Count; k++)
		{
			ModelMesh modelMesh2 = propModel.Meshes[k];
			if (modelMesh2.Name.Contains("_WalkableClsn_"))
			{
				((MeshAttributesParams)modelMesh2.Tag).ObjectType = EnumObjectTypes.NoRender;
				int vertexStride = modelMesh2.MeshParts[0].VertexBuffer.VertexDeclaration.VertexStride;
				int vertexCount = modelMesh2.MeshParts[0].VertexBuffer.VertexCount;
				int vertexOffset = modelMesh2.MeshParts[0].VertexOffset;
				int numVertices = modelMesh2.MeshParts[0].NumVertices;
				Matrix matrix2 = propTransforms[propModel.Meshes[k].ParentBone.Index];
				Matrix matrix3 = matrix2;
				byte[] array = new byte[vertexCount * vertexStride];
				modelMesh2.MeshParts[0].VertexBuffer.GetData(array);
				PropModelVertStruct* ptr = (PropModelVertStruct*)(void*)GCHandle.Alloc(array, GCHandleType.Pinned).AddrOfPinnedObject();
				_ = Vector3.Zero;
				_ = Vector3.Zero;
				Vector3 zero = Vector3.Zero;
				BoundingBox item = new BoundingBox
				{
					Min = 
					{
						X = float.MaxValue,
						Y = float.MaxValue,
						Z = float.MaxValue
					},
					Max = 
					{
						X = float.MinValue,
						Y = float.MinValue,
						Z = float.MinValue
					}
				};
				for (int l = vertexOffset; l < vertexOffset + numVertices; l++)
				{
					zero = Vector3.Transform(ptr[l].pos, matrix3);
					item.Min.X = ((zero.X < item.Min.X) ? zero.X : item.Min.X);
					item.Min.Y = ((zero.Y < item.Min.Y) ? zero.Y : item.Min.Y);
					item.Min.Z = ((zero.Z < item.Min.Z) ? zero.Z : item.Min.Z);
					item.Max.X = ((zero.X > item.Max.X) ? zero.X : item.Max.X);
					item.Max.Y = ((zero.Y > item.Max.Y) ? zero.Y : item.Max.Y);
					item.Max.Z = ((zero.Z > item.Max.Z) ? zero.Z : item.Max.Z);
				}
				Vector3 translation2 = matWorld[0].Translation;
				translation2.Y = 0f;
				item.Min += translation2;
				item.Max += translation2;
				translation2 = item.Min + (item.Max - item.Min);
				translation2.Y = HeightMapPhysics.GetHeight(ref translation2) + 6f;
				item.Min.Y += translation2.Y;
				item.Max.Y += translation2.Y;
				item.Max.Y += 6f;
				WalkableBBox.Add(item);
			}
		}
		for (int m = 0; m < propModel.Meshes.Count; m++)
		{
			ModelMesh modelMesh3 = propModel.Meshes[m];
			if (modelMesh3.Name.Contains("_Vehicle_"))
			{
				((MeshAttributesParams)modelMesh3.Tag).Culling = EnumCullingTypes.CullNone;
			}
		}
		for (int n = 0; n < propModel.Meshes.Count; n++)
		{
			ModelMesh modelMesh4 = propModel.Meshes[n];
			if (modelMesh4.Name.Contains("SpawnPosition"))
			{
				NumOfSpawnPoints++;
				((MeshAttributesParams)modelMesh4.Tag).ObjectType = EnumObjectTypes.NoRender;
			}
		}
		for (int num = 0; num < propModel.Meshes.Count; num++)
		{
			ModelMesh modelMesh5 = propModel.Meshes[num];
			if (modelMesh5.Name.Contains("Item_Spawn"))
			{
				NumOfItemPoints++;
				((MeshAttributesParams)modelMesh5.Tag).ObjectType = EnumObjectTypes.NoRender;
			}
		}
		for (int num2 = 0; num2 < propModel.Meshes.Count; num2++)
		{
			ModelMesh modelMesh6 = propModel.Meshes[num2];
			if (modelMesh6.Name.Contains("NoRender"))
			{
				((MeshAttributesParams)modelMesh6.Tag).ObjectType = EnumObjectTypes.NoRender;
			}
		}
		ItemSpawnOffsetIndice = new int[NumOfItemPoints];
		for (int num3 = 0; num3 < NumOfItemPoints; num3++)
		{
			ItemSpawnOffsetIndice[num3] = 0;
		}
		foreach (BoundingBox item2 in WalkableBBox)
		{
			Min.X = ((Min.X > item2.Min.X) ? item2.Min.X : Min.X);
			Min.Y = ((Min.Y > item2.Min.Y) ? item2.Min.Y : Min.Y);
			Min.Z = ((Min.Z > item2.Min.Z) ? item2.Min.Z : Min.Z);
			Max.X = ((Max.X < item2.Max.X) ? item2.Max.X : Max.X);
			Max.Y = ((Max.Y < item2.Max.Y) ? item2.Max.Y : Max.Y);
			Max.Z = ((Max.Z < item2.Max.Z) ? item2.Max.Z : Max.Z);
		}
		ItemSpawnPositions = new List<SpawnPositionData>(NumOfItemPoints);
		for (int num4 = 0; num4 < propModel.Meshes.Count; num4++)
		{
			ModelMesh modelMesh7 = propModel.Meshes[num4];
			if (modelMesh7.Name.Contains("Item_Spawn"))
			{
				SpawnPositionData spawnPositionData = new SpawnPositionData();
				spawnPositionData.spawmPosition = (propTransforms[modelMesh7.ParentBone.Index] * matWorld[0]).Translation;
				if (modelMesh7.Name.Contains("Consumable"))
				{
					spawnPositionData.spawnType = 256;
				}
				else if (modelMesh7.Name.Contains("Equipment"))
				{
					spawnPositionData.spawnType = 1024;
				}
				else if (modelMesh7.Name.Contains("Weapon"))
				{
					spawnPositionData.spawnType = 512;
				}
				else if (modelMesh7.Name.Contains("Vehicle"))
				{
					spawnPositionData.spawnType = 2048;
				}
				if (modelMesh7.Name.Contains("Medical"))
				{
					spawnPositionData.itemRange = 1;
				}
				else if (modelMesh7.Name.Contains("Nutrition"))
				{
					spawnPositionData.itemRange = 2;
				}
				else if (modelMesh7.Name.Contains("Mechanical"))
				{
					spawnPositionData.itemRange = 3;
				}
				else if (modelMesh7.Name.Contains("Civilian"))
				{
					spawnPositionData.itemRange = 4;
				}
				else if (modelMesh7.Name.Contains("Military"))
				{
					spawnPositionData.itemRange = 5;
				}
				else
				{
					spawnPositionData.itemRange = 0;
				}
				ItemSpawnPositions.Add(spawnPositionData);
			}
		}
	}

	public void GetNextSpawnPosition(ref Vector3 pos)
	{
		int num = 0;
		for (int i = 0; i < propModel.Meshes.Count; i++)
		{
			ModelMesh modelMesh = propModel.Meshes[i];
			if (!modelMesh.Name.Contains("SpawnPosition"))
			{
				continue;
			}
			if (num == spawnIndex)
			{
				pos = (propTransforms[modelMesh.ParentBone.Index] * matWorld[0]).Translation;
				spawnIndex++;
				if (spawnIndex >= NumOfSpawnPoints)
				{
					spawnIndex = 0;
				}
				break;
			}
			num++;
		}
	}

	public void GetNextItemSpawn(ref SpawnPositionData e)
	{
		GetItemSpawnAtIndex(ref e, itemSpawnIndex0);
		itemSpawnIndex0++;
		if (itemSpawnIndex0 >= NumOfItemPoints)
		{
			itemSpawnIndex0 = 0;
		}
	}

	public void GetItemSpawnAtIndex(ref SpawnPositionData e, int idx)
	{
		e.spawmPosition = ItemSpawnPositions[idx].spawmPosition;
		Ray ray = new Ray(e.spawmPosition + Vector3.UnitY * 20000f, -Vector3.UnitY);
		Vector3 hitPosition = Vector3.Zero;
		Vector3 hitNormal = Vector3.Zero;
		float hitDistance = float.MaxValue;
		if (RayCast(0, ref ray, ref hitPosition, ref hitNormal, ref hitDistance))
		{
			e.spawmPosition = hitPosition;
		}
		else
		{
			e.spawmPosition.Y = HeightMapPhysics.GetHeight(ref e.spawmPosition);
		}
		e.spawmPosition.Y += 12f;
		e.spawmPosition += ItemPlacmentRandomOffsets[ItemSpawnOffsetIndice[idx]];
		ItemSpawnOffsetIndice[idx]++;
		if (ItemSpawnOffsetIndice[idx] >= 8)
		{
			ItemSpawnOffsetIndice[idx] = 0;
		}
		e.spawnType = ItemSpawnPositions[idx].spawnType;
		e.itemRange = ItemSpawnPositions[idx].itemRange;
	}

	public void Update(float eTime, int qIndex, PlayerBase playerRef)
	{
		if (Name == "PlaneCrash00")
		{
			firePos.X = -29080f - playerRef.vecPosition.X;
			firePos.Z = -50120f - playerRef.vecPosition.Z;
			firePos.Y = 0f;
			if (firePos.LengthSquared() < 100000000f)
			{
				float num = (float)EndGameEngine.randGenerator.NextDouble() * 300f;
				firePos.X = -30280f;
				firePos.Z = -51120f;
				firePos.Y = HeightMapPhysics.GetHeight(ref firePos) + 200f;
				LevelBaseMenu.PointLights.AddDynamicPointLight(ref firePos, ref fireColor, 700f + num, 0.09f, qIndex);
				num = (float)EndGameEngine.randGenerator.NextDouble() * 200f;
				firePos.X = -28080f;
				firePos.Z = -49450f;
				firePos.Y = HeightMapPhysics.GetHeight(ref firePos) + 200f;
				LevelBaseMenu.PointLights.AddDynamicPointLight(ref firePos, ref fireColor, 700f + num, 0.09f, qIndex);
			}
		}
		InFrustum[qIndex] = false;
		tmpVecTo = playerRef.vecHeadPosition[qIndex];
		tmpVecTo.X = ((tmpVecTo.X < Min.X) ? (Min.X - tmpVecTo.X) : ((tmpVecTo.X > Max.X) ? (Max.X - tmpVecTo.X) : 0f));
		tmpVecTo.Z = ((tmpVecTo.Z < Min.Z) ? (Min.Z - tmpVecTo.Z) : ((tmpVecTo.Z > Max.Z) ? (Max.Z - tmpVecTo.Z) : 0f));
		tmpVecTo.Y = 0f;
		if (!(tmpVecTo.LengthSquared() < 484000000f))
		{
			return;
		}
		tmpBBox.Min = Min;
		tmpBBox.Min.X -= playerRef.vecHeadPosition[qIndex].X;
		tmpBBox.Min.Z -= playerRef.vecHeadPosition[qIndex].Z;
		tmpBBox.Min.Y = -4000f;
		tmpBBox.Max = Max;
		tmpBBox.Max.X -= playerRef.vecHeadPosition[qIndex].X;
		tmpBBox.Max.Z -= playerRef.vecHeadPosition[qIndex].Z;
		tmpBBox.Max.Y = 30000f;
		ContainmentType result = ContainmentType.Disjoint;
		playerRef.bFrustum[qIndex].Contains(ref tmpBBox, out result);
		if (result == ContainmentType.Contains || result == ContainmentType.Intersects)
		{
			InFrustum[qIndex] = true;
			for (int i = 0; i < InstanceMeshes.Length; i++)
			{
				tmpUpdateMat = InstanceMeshes[i].matWorld * matWorld[0];
				tmpVecTo = tmpUpdateMat.Translation;
				tmpVecTo.Y = 0f;
				tmpVecTo.X -= playerRef.vecHeadPosition[qIndex].X;
				tmpVecTo.Z -= playerRef.vecHeadPosition[qIndex].Z;
				InstanceMeshes[i].DistanceSqr[qIndex] = tmpVecTo.LengthSquared();
				InstanceMeshes[i].InFrustum[qIndex] = InstanceModelMgr.Update(eTime, qIndex, playerRef, InstanceMeshes[i], ref tmpUpdateMat);
			}
		}
	}

	public override void Draw(PlayerBase viewer, int qIndex)
	{
		ShaderPass = 0;
		base.Draw(viewer, qIndex);
	}

	public override void DrawCameraSpace(PlayerBase viewer, int qIndex, float lod)
	{
		for (int i = 0; i < InstanceMeshes.Length; i++)
		{
			if (InstanceMeshes[i].InFrustum[qIndex])
			{
				tmpDrawMat = InstanceMeshes[i].matWorld * matWorld[0];
				InstanceModelMgr.DrawCameraSpace(viewer, qIndex, InstanceMeshes[i].ReferenceId, ref tmpDrawMat, InstanceMeshes[i].DistanceSqr[qIndex]);
			}
		}
		base.DrawCameraSpace(viewer, qIndex, 1f);
	}

	public override void DrawShadowMap(PlayerBase viewer, ref Matrix LightViewProj, ref Vector3 lightPos, int qIndex, bool lod)
	{
		for (int i = 0; i < InstanceMeshes.Length; i++)
		{
			if (InstanceMeshes[i].InFrustum[qIndex])
			{
				Matrix tmpMat = InstanceMeshes[i].matWorld * matWorld[0];
				InstanceModelMgr.DrawShadowMap(viewer, ref LightViewProj, ref lightPos, ref tmpMat, InstanceMeshes[i].ReferenceId, qIndex, InstanceMeshes[i].DistanceSqr[qIndex] > 4000000f);
			}
		}
		base.DrawShadowMap(viewer, ref LightViewProj, ref lightPos, qIndex, lod: false);
	}

	public void SphereCollision(int qIndex, ref BoundingSphere sphere, ref bool onWalkable, ref bool inCollision, bool isPlayer, bool testWalkable)
	{
		float num = 3f;
		if (isPlayer)
		{
			num = 3f;
		}
		if (testWalkable)
		{
			int count = WalkableBBox.Count;
			float num2 = sphere.Center.Y - sphere.Radius;
			for (int i = 0; i < count; i++)
			{
				if (sphere.Center.X >= WalkableBBox[i].Min.X && sphere.Center.X <= WalkableBBox[i].Max.X && sphere.Center.Z >= WalkableBBox[i].Min.Z && sphere.Center.Z <= WalkableBBox[i].Max.Z && num2 < WalkableBBox[i].Max.Y + num)
				{
					inCollision = true;
					onWalkable = true;
					PropModelBase.RayCastCollision = true;
					if (isPlayer)
					{
						sphere.Center.Y = MathHelper.Lerp(sphere.Center.Y, WalkableBBox[i].Max.Y + sphere.Radius + num, 0.9f);
					}
					else
					{
						sphere.Center.Y = WalkableBBox[i].Max.Y + sphere.Radius + num;
					}
				}
			}
		}
		if (!CollisionGridStruct.GetSearchExtents(ref sphere, ref SearchGridExtents))
		{
			return;
		}
		int num3 = (int)SearchGridExtents.X;
		int num4 = (int)SearchGridExtents.Y;
		int num5 = (int)SearchGridExtents.Z;
		int num6 = (int)SearchGridExtents.W;
		for (int j = num3; j <= num5; j++)
		{
			for (int k = num4; k <= num6; k++)
			{
				int count2 = CollisionGridStruct.collisionGrid[j, k].Count;
				for (int l = 0; l < count2; l++)
				{
					SphereColMat = CollisionGridStruct.collisionGrid[j, k][l].matWorld;
					SphereColMat.Translation += matWorld[0].Translation;
					InstanceModelMgr.SphereCollision(qIndex, ref sphere, CollisionGridStruct.collisionGrid[j, k][l].ReferenceId, ref SphereColMat, ref onWalkable, ref inCollision);
				}
			}
		}
	}

	public float? WalkableHeight(ref Vector3 pos, int qIndex)
	{
		int count = WalkableBBox.Count;
		for (int i = 0; i < count; i++)
		{
			if (pos.X >= WalkableBBox[i].Min.X && pos.X <= WalkableBBox[i].Max.X && pos.Z >= WalkableBBox[i].Min.Z && pos.Z <= WalkableBBox[i].Max.Z)
			{
				return WalkableBBox[i].Max.Y;
			}
		}
		return null;
	}

	public bool RayCast(int qIndex, ref Ray ray, ref Vector3 hitPosition, ref Vector3 hitNormal, ref float hitDistance)
	{
		bool result = false;
		bbox.Min = Min;
		bbox.Max = Max;
		if (bbox.Intersects(ray).HasValue)
		{
			float hitDistance2 = float.MaxValue;
			hitDistance = float.MaxValue;
			for (int i = 0; i < CollisionGridStruct.nXGrid; i++)
			{
				for (int j = 0; j < CollisionGridStruct.nZGrid; j++)
				{
					bbox.Min = CollisionGridStruct.Min;
					bbox.Min.Y = -1000f;
					bbox.Min.X += i * 1024;
					bbox.Min.Z += j * 1024;
					bbox.Max = bbox.Min;
					bbox.Max.Y = 30000f;
					bbox.Max.X += 1024f;
					bbox.Max.Z += 1024f;
					if (!bbox.Intersects(ray).HasValue)
					{
						continue;
					}
					int count = CollisionGridStruct.collisionGrid[i, j].Count;
					for (int k = 0; k < count; k++)
					{
						raycastTmpMat = CollisionGridStruct.collisionGrid[i, j][k].matWorld;
						raycastTmpMat.Translation += matWorld[0].Translation;
						if (InstanceModelMgr.RayCast(qIndex, ref ray, CollisionGridStruct.collisionGrid[i, j][k].ReferenceId, ref raycastTmpMat, ref hitPosition, ref hitNormal, ref hitDistance2) && hitDistance2 < hitDistance)
						{
							hitDistance = hitDistance2;
							result = true;
						}
					}
				}
			}
		}
		return result;
	}
}
