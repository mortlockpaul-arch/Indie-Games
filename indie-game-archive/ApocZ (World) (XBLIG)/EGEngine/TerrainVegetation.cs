using System;
using System.Collections.Generic;
using DataContent;
using MaxScriptDefines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace EGEngine;

public class TerrainVegetation
{
	public struct VegTileStruct
	{
		public bool render;

		public int vertexIndex;

		public int startVertex;

		public int primitiveDivisor;

		public Vector3 worldOffset;
	}

	public struct VegRenderStruct
	{
		public VegTileStruct[,] PlayerRenderQue;
	}

	public class TreePositionStruct
	{
		public float distanceSqr;

		public byte treeType;

		public byte rotation;

		public Vector3 position;
	}

	public struct TreeStruct
	{
		public bool[,] PlayerRenderLOD;

		public bool[,] PlayerRenderMOD;

		public BoundingBox bBox;

		public int NumBillboardVertices;

		public int NumBillboardPrimitives;

		public VertexBuffer BillboardVertexBuffer;
	}

	public struct GrassTileClass
	{
		public bool Render;

		public int VertexIndex;

		public int PrimitiveDivisor;

		public Vector3 WorldOffset;
	}

	public struct GrassRenderClass
	{
		public GrassTileClass[,] PlayerRenderQue;
	}

	public class VegitationGridClass
	{
		public bool[] Render;

		public BoundingBox AABB;

		public GrassRenderClass[] GrassTiles;

		public Vector2 TreeGridXZ;
	}

	public struct TreeLODTileClass
	{
		public int xGrid;

		public int zGrid;
	}

	public const int HM_MaxHeight = 10000;

	public const int HM_MinHeight = 800;

	public const int HM_NumberOfGrid = 5;

	public const int HM_SIZE_X = 1024;

	public const int HM_SIZE_Z = 1024;

	public const int HM_POS_OFFSET = 524288;

	public const int HM_POS_HALF = 262144;

	public const int HM_AABB_SIZE_X = 2;

	public const int HM_AABB_SIZE_Z = 2;

	public const int HM_EXTENT_X = 524288;

	public const int HM_EXTENT_Z = 524288;

	public const float TREE_Y_OFFSET = 48f;

	public const int TREE_QUICK_LOOKUP = 8;

	public const int TREE_GRID_SIZE = 16384;

	public const int NumVegitationTypes = 1;

	private const int VegGridDim = 5;

	private const int VegTileDim = 4;

	private const int MaxRenderGrid = 712;

	private const int MaxTreeLODRenderTiles = 25;

	private const int MaxGrassRenderTiles = 144;

	private const int GridCellSize = 8192;

	private const int CourseAABBSize = 4;

	private const int CourseGridCellSize = 32768;

	private const int numTreeTypes = 5;

	private const int nRotationMatrices = 24;

	private int NumTreeGridX;

	private int NumTreeGridZ;

	private int numTreePerGrid = 500;

	private bool m_Valid;

	private int SizeX = 512;

	private int SizeZ = 512;

	private int LevelGirth;

	private Texture2D TestDiffuseMap;

	private TerrainVegitationHighResStruct[] VegetationGrid = new TerrainVegitationHighResStruct[1];

	private byte[] LevelVegitationMap;

	private VegitationGridClass[,] VeggyGrid;

	private TreeStruct[,] TreeGrid;

	private int CourseGridSizeX;

	private int CourseGridSizeZ;

	private BoundingBox[,] GridCourseAABB;

	private int[] CurrentNumRenderGrid = new int[2];

	private VegitationGridClass[,] RenderVeggyGridArray = new VegitationGridClass[712, 2];

	private int[] CurrentNumRenderGrassTile = new int[2];

	private GrassRenderClass[,] RenderGrassTileArray = new GrassRenderClass[712, 2];

	private int[] CurrentNumRenderTreeLODTile0 = new int[2];

	private TreeLODTileClass[,] CurrentRenderTreeLODTilePosition = new TreeLODTileClass[25, 2];

	private int[] CurNumTreeShadowLOD = new int[2];

	private TreeLODTileClass[,] CurNumTreeShadowLODPosition = new TreeLODTileClass[25, 2];

	private int[] CurrentNumRenderGrassTile0 = new int[2];

	private GrassTileClass[,] CurrentRenderGrassTilePosition = new GrassTileClass[144, 2];

	private Vector2 LodBillBoardSize = Vector2.Zero;

	private Texture2D AllTreeLodTex;

	private Texture2D SmoothSlopeSingleTex;

	private int TreeTypeDivisor = 1;

	private Model[] TreeModels;

	private Matrix[][] TreeTransforms;

	private Matrix[] tmptreeRots = new Matrix[24];

	private float[] tmptreeScale = new float[24];

	private TreePositionStruct[] TreePositions;

	private int[] NumberTreeThisFrame;

	private TreePositionStruct[,] TreeRenderQue;

	private List<Vector2>[,] TreeQuickLookupTable;

	private int numTreeModel = 128;

	private float windTime;

	private Vector3 eyePosition = Vector3.Zero;

	private Vector3 vecLOD0Pos = Vector3.Zero;

	private Vector3 treeModWorldPos = Vector3.Zero;

	private Vector3 treeModViewPos = Vector3.Zero;

	private Vector3 tmpVecWorldOffset = Vector3.Zero;

	private Vector3 tmpVecTileOffset = Vector3.Zero;

	private Vector3 tmpDisVec = Vector3.Zero;

	private Vector3 TreeGridPosAdjust = new Vector3(500f, 0f, 500f);

	private BoundingBox aabbFrustTest = default(BoundingBox);

	private BoundingBox grassFrustTest = default(BoundingBox);

	private BoundingFrustum tmpFrustum = new BoundingFrustum(Matrix.Identity);

	private float gridTestMultiplyer = 5f;

	private int TESTNUM = 32;

	private bool initializeShader = true;

	private Matrix tmpworld = Matrix.Identity;

	private Vector3 tmptranslation = Vector3.Zero;

	private Vector3 vecCameraRight = Vector3.Zero;

	private Vector4[] tmpMatTreeInstance;

	private Vector3 lightEyeDir = Vector3.Zero;

	private Matrix playerView = Matrix.Identity;

	private Matrix playerProj = Matrix.Identity;

	public static int MaxGTPList = 64;

	public static Vector3[] GetTreePosList = new Vector3[MaxGTPList];

	public void Update(float eTime, ref Vector3 cameraPos, int qIndex, int playerIndex)
	{
		if (!m_Valid)
		{
			return;
		}
		windTime += 0.00139f;
		Vector3 normal = Vector3.Zero;
		_ = Vector3.Zero;
		PlayerBase playerBase = LevelBaseMenu.Players[playerIndex];
		int num = 0;
		Vector3 zero = Vector3.Zero;
		Vector3 zero2 = Vector3.Zero;
		BoundingBox boundingBox = default(BoundingBox);
		float num2 = 1.0737418E+09f;
		CurrentNumRenderTreeLODTile0[qIndex] = 0;
		float num3 = (int)playerBase.vecHeadPosition[qIndex].X + 65536;
		float num4 = (int)playerBase.vecHeadPosition[qIndex].Z + 65536;
		int num5 = (int)Math.Floor(num3 / 16384f) - 2;
		int num6 = (int)Math.Floor(num4 / 16384f) - 2;
		num5 = ((num5 >= 0) ? num5 : 0);
		num6 = ((num6 >= 0) ? num6 : 0);
		int num7 = 8;
		int num8 = 8;
		num7 = ((num5 + 5 < num7) ? (num5 + 5) : num7);
		num8 = ((num6 + 5 < num8) ? (num6 + 5) : num8);
		for (int i = num5; i < num7; i++)
		{
			for (int j = num6; j < num8; j++)
			{
				if (TreeGrid[i, j].NumBillboardPrimitives <= 0)
				{
					continue;
				}
				boundingBox = TreeGrid[i, j].bBox;
				boundingBox.Min -= TreeGridPosAdjust;
				boundingBox.Max += TreeGridPosAdjust;
				boundingBox.Min.X -= playerBase.vecHeadPosition[qIndex].X;
				boundingBox.Min.Z -= playerBase.vecHeadPosition[qIndex].Z;
				boundingBox.Max.X -= playerBase.vecHeadPosition[qIndex].X;
				boundingBox.Max.Z -= playerBase.vecHeadPosition[qIndex].Z;
				ContainmentType result = ContainmentType.Disjoint;
				playerBase.bFrustum[qIndex].Contains(ref boundingBox, out result);
				if (result != ContainmentType.Contains && result != ContainmentType.Intersects)
				{
					continue;
				}
				CurrentRenderTreeLODTilePosition[CurrentNumRenderTreeLODTile0[qIndex], qIndex].xGrid = i;
				CurrentRenderTreeLODTilePosition[CurrentNumRenderTreeLODTile0[qIndex], qIndex].zGrid = j;
				CurrentNumRenderTreeLODTile0[qIndex]++;
				zero = TreeGrid[i, j].bBox.Min + (TreeGrid[i, j].bBox.Max - TreeGrid[i, j].bBox.Min);
				zero -= playerBase.vecHeadPosition[qIndex];
				zero.Y = 0f;
				if (!(zero.LengthSquared() < num2))
				{
					continue;
				}
				for (int k = 0; k < numTreePerGrid; k++)
				{
					if (num >= numTreeModel)
					{
						break;
					}
					zero2 = TreeGrid[i, j].bBox.Min + TreePositions[k].position;
					zero = zero2 - playerBase.vecHeadPosition[qIndex];
					zero.Y = 0f;
					float num9 = zero.LengthSquared();
					if (!(num9 < 36000000f))
					{
						continue;
					}
					byte b = HeightMapPhysics.ReadTreeMap(ref zero2);
					if (b == 0)
					{
						continue;
					}
					int num10 = b / TreeTypeDivisor;
					zero2.Y = HeightMapPhysics.GetHeight(ref zero2, out normal) - 48f;
					boundingBox.Min = zero2;
					boundingBox.Min.X -= playerBase.vecHeadPosition[qIndex].X;
					boundingBox.Min.Z -= playerBase.vecHeadPosition[qIndex].Z;
					boundingBox.Min.Y -= 500f;
					boundingBox.Min.X -= 300f;
					boundingBox.Min.Z -= 300f;
					boundingBox.Max = boundingBox.Min;
					boundingBox.Max.Y += 3500f;
					boundingBox.Max.X += 600f;
					boundingBox.Max.Z += 600f;
					result = ContainmentType.Disjoint;
					playerBase.bFrustum[qIndex].Contains(ref boundingBox, out result);
					if (result == ContainmentType.Contains || result == ContainmentType.Intersects || num9 < 1000000f)
					{
						TreeRenderQue[num, qIndex].distanceSqr = num9;
						TreeRenderQue[num, qIndex].treeType = (byte)num10;
						TreeRenderQue[num, qIndex].rotation = TreePositions[k].rotation;
						zero2.X -= playerBase.vecHeadPosition[qIndex].X;
						zero2.Z -= playerBase.vecHeadPosition[qIndex].Z;
						TreeRenderQue[num, qIndex].position = zero2;
						num++;
						if (num >= numTreeModel - 1)
						{
							num = numTreeModel - 1;
						}
					}
				}
			}
		}
		NumberTreeThisFrame[qIndex] = num;
		for (int l = 0; l < num; l++)
		{
			for (int m = 0; m < num; m++)
			{
				if (TreeRenderQue[m, qIndex].distanceSqr < TreeRenderQue[l, qIndex].distanceSqr)
				{
					TreePositionStruct treePositionStruct = TreeRenderQue[m, qIndex];
					TreeRenderQue[m, qIndex] = TreeRenderQue[l, qIndex];
					TreeRenderQue[l, qIndex] = treePositionStruct;
				}
			}
		}
		CurrentNumRenderGrassTile0[qIndex] = 0;
		float num11 = (int)playerBase.vecHeadPosition[qIndex].X + 65536;
		float num12 = (int)playerBase.vecHeadPosition[qIndex].Z + 65536;
		int num13 = (int)(num11 / (float)VegetationGrid[0].sizeX) - 1;
		int num14 = (int)(num12 / (float)VegetationGrid[0].sizeZ) - 1;
		num13 = ((num13 >= 0) ? num13 : 0);
		num14 = ((num14 >= 0) ? num14 : 0);
		int num15 = 131072 / VegetationGrid[0].sizeX;
		int num16 = 131072 / VegetationGrid[0].sizeZ;
		num15 = ((num13 + 3 < num15) ? (num13 + 3) : num15);
		num16 = ((num14 + 3 < num16) ? (num14 + 3) : num16);
		float num17 = 36000000f;
		for (int n = num13; n < num15; n++)
		{
			for (int num18 = num14; num18 < num16; num18++)
			{
				num11 = n * VegetationGrid[0].sizeX;
				num12 = num18 * VegetationGrid[0].sizeZ;
				num11 -= 65536f;
				num12 -= 65536f;
				int num19 = 2048;
				grassFrustTest.Min.Y = -4000f;
				grassFrustTest.Min.X = num11 - playerBase.vecHeadPosition[qIndex].X;
				grassFrustTest.Min.Z = num12 - playerBase.vecHeadPosition[qIndex].Z;
				grassFrustTest.Max.Y = 30000f;
				grassFrustTest.Max.X = grassFrustTest.Min.X + (float)VegetationGrid[0].sizeX;
				grassFrustTest.Max.Z = grassFrustTest.Min.Z + (float)VegetationGrid[0].sizeZ;
				ContainmentType result2 = ContainmentType.Disjoint;
				playerBase.bFrustum[qIndex].Contains(ref grassFrustTest, out result2);
				if (result2 != ContainmentType.Contains && result2 != ContainmentType.Intersects)
				{
					continue;
				}
				tmpVecWorldOffset = Vector3.Zero;
				tmpVecWorldOffset.X = num11;
				tmpVecWorldOffset.Z = num12;
				int num20 = 0;
				for (int num21 = 0; num21 < 4; num21++)
				{
					for (int num22 = 0; num22 < 4; num22++)
					{
						tmpVecTileOffset = Vector3.Zero;
						tmpVecTileOffset.X = num11 + (float)(num19 * num21);
						tmpVecTileOffset.Z = num12 + (float)(num19 * num22);
						tmpDisVec = tmpVecTileOffset - playerBase.vecHeadPosition[qIndex];
						tmpDisVec.Y = 0f;
						float num23 = tmpDisVec.LengthSquared();
						if (num23 < num17)
						{
							grassFrustTest.Min.Y = -4000f;
							grassFrustTest.Min.X = tmpVecTileOffset.X - playerBase.vecHeadPosition[qIndex].X;
							grassFrustTest.Min.Z = tmpVecTileOffset.Z - playerBase.vecHeadPosition[qIndex].Z;
							grassFrustTest.Max.Y = 30000f;
							grassFrustTest.Max.X = grassFrustTest.Min.X + (float)num19;
							grassFrustTest.Max.Z = grassFrustTest.Min.Z + (float)num19;
							result2 = ContainmentType.Disjoint;
							playerBase.bFrustum[qIndex].Contains(ref grassFrustTest, out result2);
							if (result2 == ContainmentType.Contains || result2 == ContainmentType.Intersects)
							{
								CurrentRenderGrassTilePosition[CurrentNumRenderGrassTile0[qIndex], qIndex].VertexIndex = num20;
								CurrentRenderGrassTilePosition[CurrentNumRenderGrassTile0[qIndex], qIndex].WorldOffset = tmpVecWorldOffset;
								CurrentNumRenderGrassTile0[qIndex]++;
							}
						}
						num20++;
					}
				}
			}
		}
	}

	public void Draw(PlayerBase playerRef, int playerIndex, int qIndex)
	{
		if (!m_Valid)
		{
			return;
		}
		GraphicsDevice graphicsDevice = Terrain.TerrainEffect.GraphicsDevice;
		graphicsDevice.VertexTextures[1] = SmoothSlopeSingleTex;
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
		if (initializeShader)
		{
			Terrain.TerrainEffect.Parameters["AllTreesLODTex"].SetValue(AllTreeLodTex);
			Terrain.TerrainEffect.Parameters["GrassBillboardTexture"].SetValue(TestDiffuseMap);
			initializeShader = false;
		}
		Terrain.TerrainParams.vecEyePosition.SetValue(playerRef.mDataQueue[qIndex].cameraEyePos);
		Terrain.TerrainParams.matViewProj.SetValue(playerRef.mDataQueue[qIndex].viewProj);
		Terrain.TerrainParams.windTime.SetValue(windTime);
		eyePosition = playerRef.mDataQueue[qIndex].cameraEyePos;
		vecCameraRight.X = 0f - playerRef.mDataQueue[qIndex].view.M11;
		vecCameraRight.Y = 0f - playerRef.mDataQueue[qIndex].view.M21;
		vecCameraRight.Z = 0f - playerRef.mDataQueue[qIndex].view.M31;
		Terrain.TerrainParams.vecCameraRight.SetValue(vecCameraRight);
		Terrain.TerrainParams.gridTileOffset.SetValue(playerRef.vecHeadPosition[qIndex]);
		Terrain.TerrainEffect.GraphicsDevice.SetVertexBuffer(VegetationGrid[0].vertexBuffer[0]);
		for (int i = 0; i < CurrentNumRenderGrassTile0[qIndex]; i++)
		{
			int vertexIndex = CurrentRenderGrassTilePosition[i, qIndex].VertexIndex;
			Terrain.TerrainParams.vecWorldOffset.SetValue(CurrentRenderGrassTilePosition[i, qIndex].WorldOffset);
			Terrain.TerrainEffect.CurrentTechnique.Passes[5].Apply();
			Terrain.TerrainEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, VegetationGrid[0].startVertice[vertexIndex], VegetationGrid[0].numPrimitives[vertexIndex]);
		}
		for (int j = 0; j < 5; j++)
		{
			for (int k = 0; k < TreeModels[j].Meshes.Count; k++)
			{
				ModelMesh modelMesh = TreeModels[j].Meshes[k];
				if (!modelMesh.Name.Contains("LOD2"))
				{
					continue;
				}
				for (int l = 0; l < modelMesh.MeshParts.Count; l++)
				{
					ModelMeshPart modelMeshPart = modelMesh.MeshParts[l];
					((PropEffectParams)modelMeshPart.Tag).matViewProj.SetValue(playerRef.mDataQueue[qIndex].viewProj);
					((PropEffectParams)modelMeshPart.Tag).eyePosition.SetValue(playerRef.mDataQueue[qIndex].cameraEyePos);
					modelMeshPart.Effect.GraphicsDevice.SetVertexBuffer(modelMeshPart.VertexBuffer, modelMeshPart.VertexOffset);
					modelMeshPart.Effect.GraphicsDevice.Indices = modelMeshPart.IndexBuffer;
					int num = 1;
					if (modelMesh.Name.Contains("_Leaf_"))
					{
						modelMeshPart.Effect.Parameters["windTime"].SetValue(windTime);
						if (j == 2)
						{
							graphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
						}
						else
						{
							graphicsDevice.RasterizerState = EndGameEngine.RasterCullCC;
						}
					}
					else
					{
						modelMeshPart.Effect.Parameters["windTime"].SetValue(0);
						graphicsDevice.RasterizerState = EndGameEngine.RasterCullCC;
					}
					for (int m = 0; m < NumberTreeThisFrame[qIndex]; m++)
					{
						if (TreeRenderQue[m, qIndex].treeType == (byte)j)
						{
							tmpworld = tmptreeRots[TreeRenderQue[m, qIndex].rotation];
							tmptranslation.X = TreeRenderQue[m, qIndex].position.X;
							tmptranslation.Y = TreeRenderQue[m, qIndex].position.Y;
							tmptranslation.Z = TreeRenderQue[m, qIndex].position.Z;
							tmpworld.Translation = tmptranslation;
							((PropEffectParams)modelMeshPart.Tag).matWorld.SetValue(tmpworld);
							modelMeshPart.Effect.CurrentTechnique.Passes[5].Apply();
							modelMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, modelMeshPart.NumVertices, modelMeshPart.StartIndex, modelMeshPart.PrimitiveCount / num);
						}
					}
				}
			}
		}
		Vector3 zero = Vector3.Zero;
		int num2 = CurrentNumRenderTreeLODTile0[qIndex];
		for (int n = 0; n < num2; n++)
		{
			int xGrid = CurrentRenderTreeLODTilePosition[n, qIndex].xGrid;
			int zGrid = CurrentRenderTreeLODTilePosition[n, qIndex].zGrid;
			Terrain.TerrainParams.vecWorldOffset.SetValue(zero);
			graphicsDevice.SetVertexBuffer(TreeGrid[xGrid, zGrid].BillboardVertexBuffer);
			Terrain.TerrainEffect.CurrentTechnique.Passes[6].Apply();
			graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, TreeGrid[xGrid, zGrid].NumBillboardPrimitives);
		}
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullCC;
	}

	public void DrawShadowMap(ref Matrix LightViewProj, ref Matrix LightView, ref Vector3 lightPos, int qIndex)
	{
		if (m_Valid)
		{
			GraphicsDevice graphicsDevice = Terrain.TerrainEffect.GraphicsDevice;
			graphicsDevice.BlendState = BlendState.Opaque;
			graphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
			lightEyeDir.X = 0f - LightView.M13;
			lightEyeDir.Y = 0f - LightView.M23;
			lightEyeDir.Z = 0f - LightView.M33;
			Vector3 zero = Vector3.Zero;
			zero.X = 0f - LightView.M11;
			zero.Y = 0f - LightView.M21;
			zero.Z = 0f - LightView.M31;
			Vector3 zero2 = Vector3.Zero;
			int num = CurrentNumRenderTreeLODTile0[qIndex];
			Terrain.TerrainEffect.Parameters["matLightViewProj"].SetValue(LightViewProj);
			Terrain.TerrainEffect.Parameters["vecEyePosition"].SetValue(lightPos);
			Terrain.TerrainParams.vecCameraRight.SetValue(zero);
			float y = lightEyeDir.Y * -800f;
			lightEyeDir.Y = 0f;
			lightEyeDir *= 100f;
			lightEyeDir.Normalize();
			lightEyeDir.Y = y;
			Terrain.TerrainParams.vecLightDirection.SetValue(lightEyeDir);
			Terrain.TerrainParams.gridTileOffset.SetValue(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecHeadPosition[qIndex]);
			graphicsDevice.VertexSamplerStates[0] = SamplerState.PointClamp;
			graphicsDevice.VertexSamplerStates[1] = SamplerState.PointWrap;
			graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			for (int i = 0; i < num; i++)
			{
				int xGrid = CurrentRenderTreeLODTilePosition[i, qIndex].xGrid;
				int zGrid = CurrentRenderTreeLODTilePosition[i, qIndex].zGrid;
				Terrain.TerrainParams.vecWorldOffset.SetValue(zero2);
				graphicsDevice.SetVertexBuffer(TreeGrid[xGrid, zGrid].BillboardVertexBuffer);
				Terrain.TerrainEffect.CurrentTechnique.Passes[8].Apply();
				graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, TreeGrid[xGrid, zGrid].NumBillboardPrimitives);
			}
			graphicsDevice.RasterizerState = EndGameEngine.RasterCullCC;
		}
	}

	private void DrawGrassTiles(GrassTileClass dgtRenderQue)
	{
		if (VegetationGrid[0].numPrimitives[dgtRenderQue.VertexIndex] != 0)
		{
			Terrain.TerrainParams.vecWorldOffset.SetValue(dgtRenderQue.WorldOffset);
			Terrain.TerrainEffect.CurrentTechnique.Passes[5].Apply();
			Terrain.TerrainEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, VegetationGrid[0].startVertice[dgtRenderQue.VertexIndex], VegetationGrid[0].numPrimitives[dgtRenderQue.VertexIndex] / 1);
		}
	}

	public void Initialize(string terrainName)
	{
	}

	public void Load(string terrainName)
	{
		SmoothSlopeSingleTex = EndGameEngine.GameAssetMgr.Load<Texture2D>("terrain\\SmoothSlope");
	}

	public void Finalize()
	{
		if (m_Valid)
		{
			return;
		}
		m_Valid = true;
		LevelGirth = 16;
		SizeX = LevelGirth;
		SizeZ = LevelGirth;
		CourseGridSizeX = SizeX / 4;
		CourseGridSizeZ = SizeZ / 4;
		GridCourseAABB = new BoundingBox[CourseGridSizeX, CourseGridSizeZ];
		Vector3 position = Vector3.Zero;
		_ = Vector3.Zero;
		for (int i = 0; i < 25; i++)
		{
			CurrentRenderTreeLODTilePosition[i, 0] = default(TreeLODTileClass);
			CurrentRenderTreeLODTilePosition[i, 1] = default(TreeLODTileClass);
		}
		for (int j = 0; j < 144; j++)
		{
			CurrentRenderGrassTilePosition[j, 0] = default(GrassTileClass);
			CurrentRenderGrassTilePosition[j, 1] = default(GrassTileClass);
		}
		for (int k = 0; k < CourseGridSizeX; k++)
		{
			for (int l = 0; l < CourseGridSizeZ; l++)
			{
				GridCourseAABB[k, l] = default(BoundingBox);
				GridCourseAABB[k, l].Min.Y = float.MaxValue;
				GridCourseAABB[k, l].Min.X = k * 32768 - 65536;
				GridCourseAABB[k, l].Min.Z = l * 32768 - 65536;
				GridCourseAABB[k, l].Max.Y = float.MinValue;
				GridCourseAABB[k, l].Max.X = GridCourseAABB[k, l].Min.X + 32768f;
				GridCourseAABB[k, l].Max.Z = GridCourseAABB[k, l].Min.Z + 32768f;
				for (float num = GridCourseAABB[k, l].Min.X; num < GridCourseAABB[k, l].Max.X; num += 512f)
				{
					for (float num2 = GridCourseAABB[k, l].Min.Z; num2 < GridCourseAABB[k, l].Max.Z; num2 += 512f)
					{
						position.X = num;
						position.Z = num2;
						position.Y = HeightMapPhysics.GetHeight(ref position) - 48f;
						if (position.Y < GridCourseAABB[k, l].Min.Y)
						{
							GridCourseAABB[k, l].Min.Y = position.Y;
						}
						if (position.Y > GridCourseAABB[k, l].Max.Y)
						{
							GridCourseAABB[k, l].Max.Y = position.Y;
						}
					}
				}
				GridCourseAABB[k, l].Min.Y -= 256f;
				GridCourseAABB[k, l].Max.Y += 256f;
			}
		}
		VeggyGrid = new VegitationGridClass[SizeX, SizeZ];
		for (int m = 0; m < SizeX; m++)
		{
			for (int n = 0; n < SizeZ; n++)
			{
				VeggyGrid[m, n] = new VegitationGridClass();
				VeggyGrid[m, n].Render = new bool[2];
				VeggyGrid[m, n].GrassTiles = new GrassRenderClass[16];
				VeggyGrid[m, n].TreeGridXZ = Vector2.Zero;
				VeggyGrid[m, n].AABB = default(BoundingBox);
				VeggyGrid[m, n].AABB.Min.Y = float.MaxValue;
				VeggyGrid[m, n].AABB.Min.X = m * 8192 - 65536;
				VeggyGrid[m, n].AABB.Min.Z = n * 8192 - 65536;
				VeggyGrid[m, n].AABB.Max.Y = float.MinValue;
				VeggyGrid[m, n].AABB.Max.X = VeggyGrid[m, n].AABB.Min.X + 8192f;
				VeggyGrid[m, n].AABB.Max.Z = VeggyGrid[m, n].AABB.Min.Z + 8192f;
				for (float num3 = VeggyGrid[m, n].AABB.Min.X; num3 < VeggyGrid[m, n].AABB.Max.X; num3 += 512f)
				{
					for (float num4 = VeggyGrid[m, n].AABB.Min.Z; num4 < VeggyGrid[m, n].AABB.Max.Z; num4 += 512f)
					{
						position.X = num3;
						position.Z = num4;
						position.Y = HeightMapPhysics.GetHeight(ref position) - 48f;
						if (position.Y < VeggyGrid[m, n].AABB.Min.Y)
						{
							VeggyGrid[m, n].AABB.Min.Y = position.Y;
						}
						if (position.Y > VeggyGrid[m, n].AABB.Max.Y)
						{
							VeggyGrid[m, n].AABB.Max.Y = position.Y;
						}
					}
				}
				VeggyGrid[m, n].AABB.Min.Y -= 256f;
				VeggyGrid[m, n].AABB.Max.Y += 256f;
				int num5 = 0;
				for (int num6 = 0; num6 < 4; num6++)
				{
					for (int num7 = 0; num7 < 4; num7++)
					{
						VeggyGrid[m, n].GrassTiles[num5] = default(GrassRenderClass);
						VeggyGrid[m, n].GrassTiles[num5].PlayerRenderQue = new GrassTileClass[4, 2];
						for (int num8 = 0; num8 < 4; num8++)
						{
							VeggyGrid[m, n].GrassTiles[num5].PlayerRenderQue[num8, 0] = default(GrassTileClass);
							VeggyGrid[m, n].GrassTiles[num5].PlayerRenderQue[num8, 1] = default(GrassTileClass);
							VeggyGrid[m, n].GrassTiles[num5].PlayerRenderQue[num8, 0].Render = false;
							VeggyGrid[m, n].GrassTiles[num5].PlayerRenderQue[num8, 1].Render = false;
						}
						num5++;
					}
				}
			}
		}
		LevelVegitationMap = new byte[SizeX * SizeZ];
		TestDiffuseMap = EndGameEngine.GameAssetMgr.Load<Texture2D>("terrain\\brush00");
		for (int num9 = 0; num9 < 1; num9++)
		{
			VegetationGrid[num9].sizeX = 8192;
			VegetationGrid[num9].sizeZ = 8192;
			VegetationGrid[num9].scale = 32;
			VegetationGrid[num9].numBillboards = 12000;
			GenerateHighMesh(ref VegetationGrid[num9]);
			VegetationGrid[num9].tileRender = new bool[4, 16, 2];
			VegetationGrid[num9].curOffset = new Vector3[4, 16, 2];
			VegetationGrid[num9].tileOffset = new Vector3[1];
		}
		for (int num10 = 0; num10 < SizeX * SizeZ; num10++)
		{
			LevelVegitationMap[num10] = 0;
		}
		TreeTypeDivisor = 55;
		TreeModels = new Model[5];
		TreeTransforms = new Matrix[5][];
		TreeModels[0] = EndGameEngine.GameAssetMgr.Load<Model>("models\\props\\tree09");
		TreeModels[3] = EndGameEngine.GameAssetMgr.Load<Model>("models\\props\\tree10");
		TreeModels[4] = EndGameEngine.GameAssetMgr.Load<Model>("models\\props\\tree11");
		TreeModels[1] = EndGameEngine.GameAssetMgr.Load<Model>("models\\props\\tree12");
		TreeModels[2] = EndGameEngine.GameAssetMgr.Load<Model>("models\\props\\tree13");
		AllTreeLodTex = EndGameEngine.GameAssetMgr.Load<Texture2D>("terrain\\AllTreeLOD");
		for (int num11 = 0; num11 < 5; num11++)
		{
			TreeTransforms[num11] = new Matrix[TreeModels[num11].Bones.Count];
			TreeModels[num11].CopyAbsoluteBoneTransformsTo(TreeTransforms[num11]);
			for (int num12 = 0; num12 < TreeModels[num11].Meshes.Count; num12++)
			{
				ModelMesh modelMesh = TreeModels[num11].Meshes[num12];
				if (modelMesh.Name.Contains("ALPHATEST"))
				{
					((MeshAttributesParams)modelMesh.Tag).Opacity = EnumOpacityTypes.AlphaTest;
				}
				for (int num13 = 0; num13 < modelMesh.MeshParts.Count; num13++)
				{
					PropEffectParams tag = new PropEffectParams(modelMesh.MeshParts[num13].Effect);
					modelMesh.MeshParts[num13].Tag = tag;
				}
			}
		}
		for (int num14 = 0; num14 < 24; num14++)
		{
			tmptreeScale[num14] = 0.8f + (float)EndGameEngine.randGenerator.NextDouble() * 0.5f;
			ref Matrix reference = ref tmptreeRots[num14];
			reference = Matrix.CreateScale(tmptreeScale[num14]) * Matrix.CreateRotationY((float)EndGameEngine.randGenerator.NextDouble() * 6.28f);
		}
		GenerateTreeMesh();
	}

	private Byte4 MyByte4(float x, float y, float z, float w)
	{
		return new Byte4(x * 127f, y * 127f, z + 127f, w);
	}

	public void GenerateHighMesh(ref TerrainVegitationHighResStruct ts)
	{
		float num = 1f;
		_ = Vector2.Zero;
		_ = Vector3.Zero;
		Vector3 zero = Vector3.Zero;
		int num2 = ts.numBillboards * 6 * 2;
		VRT_TerrainVegitation[] array = new VRT_TerrainVegitation[num2];
		Random random = new Random(11);
		Matrix identity = Matrix.Identity;
		ts.startVertice = new int[16];
		ts.numVertices = new int[16];
		ts.numPrimitives = new int[16];
		ts.vertexBuffer = new VertexBuffer[16];
		int num3 = 2048;
		int num4 = ts.numBillboards / 24;
		int num5 = (ts.numBillboards - num4) / 24;
		int num6 = ts.numBillboards - num4 - num5;
		Vector3[] array2 = new Vector3[num4];
		Vector3[] array3 = new Vector3[num5];
		Vector3[] array4 = new Vector3[num6];
		for (int i = 0; i < num4; i++)
		{
			ref Vector3 reference = ref array2[i];
			reference = Vector3.Zero;
			array2[i].X = (float)random.NextDouble() * (float)ts.sizeX * num;
			array2[i].Z = (float)random.NextDouble() * (float)ts.sizeZ * num;
		}
		for (int j = 0; j < num5; j++)
		{
			ref Vector3 reference2 = ref array3[j];
			reference2 = Vector3.Zero;
			array3[j].X = (float)random.NextDouble() * (float)ts.sizeX * num;
			array3[j].Z = (float)random.NextDouble() * (float)ts.sizeZ * num;
		}
		for (int k = 0; k < num6; k++)
		{
			ref Vector3 reference3 = ref array4[k];
			reference3 = Vector3.Zero;
			array4[k].X = (float)random.NextDouble() * (float)ts.sizeX * num;
			array4[k].Z = (float)random.NextDouble() * (float)ts.sizeZ * num;
		}
		int num7 = 0;
		int num8 = 0;
		int num9 = 0;
		for (int l = 0; l < 4; l++)
		{
			for (int m = 0; m < 4; m++)
			{
				int num10 = l * num3;
				int num11 = l * num3 + num3;
				int num12 = m * num3;
				int num13 = m * num3 + num3;
				ts.startVertice[num9] = num7;
				for (int n = 0; n < ts.numBillboards; n++)
				{
					if (n < num4)
					{
						zero = array2[n];
						if (zero.X >= (float)num10 && zero.X < (float)num11 && zero.Z >= (float)num12 && zero.Z < (float)num13)
						{
							float num14 = (float)random.NextDouble();
							float num15 = 80f + num14 * 30f;
							float y = 60f + num14 * 15f;
							array[num7].Texcoord.R = 127;
							array[num7].Texcoord.G = 0;
							array[num7].Texcoord.B = (byte)(0f - num15 + 127f);
							array[num7].Texcoord.A = byte.MaxValue;
							array[num7].Position = zero;
							array[num7].Position.Z += array[num7].Texcoord.B - 127;
							array[num7++].Position.Y = y;
							array[num7].Texcoord.R = 127;
							array[num7].Texcoord.G = 0;
							array[num7].Texcoord.B = (byte)(0f - num15 + 127f);
							array[num7].Texcoord.A = byte.MaxValue;
							array[num7].Position = zero;
							array[num7].Position.Z += array[num7].Texcoord.B - 127;
							array[num7++].Position.Y = y;
							array[num7].Texcoord.R = byte.MaxValue;
							array[num7].Texcoord.G = 0;
							array[num7].Texcoord.B = (byte)(num15 + 127f);
							array[num7].Texcoord.A = byte.MaxValue;
							array[num7].Position = zero;
							array[num7].Position.Z += array[num7].Texcoord.B - 127;
							array[num7++].Position.Y = y;
							array[num7].Texcoord.R = 127;
							array[num7].Texcoord.G = 127;
							array[num7].Texcoord.B = (byte)(0f - num15 + 127f);
							array[num7].Texcoord.A = 0;
							array[num7].Position = zero;
							array[num7].Position.Z += array[num7].Texcoord.B - 127;
							array[num7++].Position.Y = 0f;
							array[num7].Texcoord.R = byte.MaxValue;
							array[num7].Texcoord.G = 127;
							array[num7].Texcoord.B = (byte)(num15 + 127f);
							array[num7].Texcoord.A = 0;
							array[num7].Position = zero;
							array[num7].Position.Z += array[num7].Texcoord.B - 127;
							array[num7++].Position.Y = 0f;
							array[num7].Texcoord.R = byte.MaxValue;
							array[num7].Texcoord.G = 127;
							array[num7].Texcoord.B = (byte)(num15 + 127f);
							array[num7].Texcoord.A = 0;
							array[num7].Position = zero;
							array[num7].Position.Z += array[num7].Texcoord.B - 127;
							array[num7++].Position.Y = 0f;
							num8 = num7 - 6;
							for (int num16 = 0; num16 < 6; num16++)
							{
								array[num7].Texcoord = array[num8].Texcoord;
								array[num7].Position = array[num8].Position;
								array[num7].Position.Z = zero.Z;
								array[num7].Position.X += array[num8].Texcoord.B - 127;
								num7++;
								num8++;
							}
							float radians = (float)(random.NextDouble() * Math.PI);
							identity = Matrix.CreateScale(num) * Matrix.CreateRotationY(radians);
							num7 -= 12;
							for (float num17 = 0f; num17 < 12f; num17++)
							{
								array[num7].Position -= zero;
								array[num7].Position = Vector3.Transform(array[num7].Position, identity);
								array[num7].Position += zero;
								num7++;
							}
						}
					}
					if (n < num5)
					{
						zero = array3[n];
						if (zero.X >= (float)num10 && zero.X < (float)num11 && zero.Z >= (float)num12 && zero.Z < (float)num13)
						{
							float num18 = (float)random.NextDouble();
							float num19 = 60f + num18 * 25f;
							float y2 = 40f + num18 * 20f;
							array[num7].Texcoord.R = 0;
							array[num7].Texcoord.G = 0;
							array[num7].Texcoord.B = (byte)(0f - num19 + 127f);
							array[num7].Texcoord.A = byte.MaxValue;
							array[num7].Position = zero;
							array[num7].Position.Z += array[num7].Texcoord.B - 127;
							array[num7++].Position.Y = y2;
							array[num7].Texcoord.R = 0;
							array[num7].Texcoord.G = 0;
							array[num7].Texcoord.B = (byte)(0f - num19 + 127f);
							array[num7].Texcoord.A = byte.MaxValue;
							array[num7].Position = zero;
							array[num7].Position.Z += array[num7].Texcoord.B - 127;
							array[num7++].Position.Y = y2;
							array[num7].Texcoord.R = 127;
							array[num7].Texcoord.G = 0;
							array[num7].Texcoord.B = (byte)(num19 + 127f);
							array[num7].Texcoord.A = byte.MaxValue;
							array[num7].Position = zero;
							array[num7].Position.Z += array[num7].Texcoord.B - 127;
							array[num7++].Position.Y = y2;
							array[num7].Texcoord.R = 0;
							array[num7].Texcoord.G = 127;
							array[num7].Texcoord.B = (byte)(0f - num19 + 127f);
							array[num7].Texcoord.A = 0;
							array[num7].Position = zero;
							array[num7].Position.Z += array[num7].Texcoord.B - 127;
							array[num7++].Position.Y = 0f;
							array[num7].Texcoord.R = 127;
							array[num7].Texcoord.G = 127;
							array[num7].Texcoord.B = (byte)(num19 + 127f);
							array[num7].Texcoord.A = 0;
							array[num7].Position = zero;
							array[num7].Position.Z += array[num7].Texcoord.B - 127;
							array[num7++].Position.Y = 0f;
							array[num7].Texcoord.R = 127;
							array[num7].Texcoord.G = 127;
							array[num7].Texcoord.B = (byte)(num19 + 127f);
							array[num7].Texcoord.A = 0;
							array[num7].Position = zero;
							array[num7].Position.Z += array[num7].Texcoord.B - 127;
							array[num7++].Position.Y = 0f;
							num8 = num7 - 6;
							for (int num20 = 0; num20 < 6; num20++)
							{
								array[num7].Texcoord = array[num8].Texcoord;
								array[num7].Position = array[num8].Position;
								array[num7].Position.Z = zero.Z;
								array[num7].Position.X += array[num8].Texcoord.B - 127;
								num7++;
								num8++;
							}
							float radians2 = (float)(random.NextDouble() * Math.PI);
							identity = Matrix.CreateScale(num) * Matrix.CreateRotationY(radians2);
							num7 -= 12;
							for (float num21 = 0f; num21 < 12f; num21++)
							{
								array[num7].Position -= zero;
								array[num7].Position = Vector3.Transform(array[num7].Position, identity);
								array[num7].Position += zero;
								num7++;
							}
						}
					}
					if (n >= num6)
					{
						continue;
					}
					zero = array4[n];
					if (zero.X >= (float)num10 && zero.X < (float)num11 && zero.Z >= (float)num12 && zero.Z < (float)num13)
					{
						float num22 = 100f;
						float y3 = 30f;
						array[num7].Texcoord.R = 0;
						array[num7].Texcoord.G = 127;
						array[num7].Texcoord.B = (byte)(0f - num22 + 127f);
						array[num7].Texcoord.A = byte.MaxValue;
						array[num7].Position = zero;
						array[num7].Position.Z += array[num7].Texcoord.B - 127;
						array[num7++].Position.Y = y3;
						array[num7].Texcoord.R = 0;
						array[num7].Texcoord.G = 127;
						array[num7].Texcoord.B = (byte)(0f - num22 + 127f);
						array[num7].Texcoord.A = byte.MaxValue;
						array[num7].Position = zero;
						array[num7].Position.Z += array[num7].Texcoord.B - 127;
						array[num7++].Position.Y = y3;
						array[num7].Texcoord.R = byte.MaxValue;
						array[num7].Texcoord.G = 127;
						array[num7].Texcoord.B = (byte)(num22 + 127f);
						array[num7].Texcoord.A = byte.MaxValue;
						array[num7].Position = zero;
						array[num7].Position.Z += array[num7].Texcoord.B - 127;
						array[num7++].Position.Y = y3;
						array[num7].Texcoord.R = 0;
						array[num7].Texcoord.G = 194;
						array[num7].Texcoord.B = (byte)(0f - num22 + 127f);
						array[num7].Texcoord.A = 0;
						array[num7].Position = zero;
						array[num7].Position.Z += array[num7].Texcoord.B - 127;
						array[num7++].Position.Y = 0f;
						array[num7].Texcoord.R = byte.MaxValue;
						array[num7].Texcoord.G = 194;
						array[num7].Texcoord.B = (byte)(num22 + 127f);
						array[num7].Texcoord.A = 0;
						array[num7].Position = zero;
						array[num7].Position.Z += array[num7].Texcoord.B - 127;
						array[num7++].Position.Y = 0f;
						array[num7].Texcoord.R = byte.MaxValue;
						array[num7].Texcoord.G = 194;
						array[num7].Texcoord.B = (byte)(num22 + 127f);
						array[num7].Texcoord.A = 0;
						array[num7].Position = zero;
						array[num7].Position.Z += array[num7].Texcoord.B - 127;
						array[num7++].Position.Y = 0f;
						num8 = num7 - 6;
						for (int num23 = 0; num23 < 6; num23++)
						{
							array[num7].Texcoord = array[num8].Texcoord;
							array[num7].Position = array[num8].Position;
							array[num7].Position.Z = zero.Z;
							array[num7].Position.X += array[num8].Texcoord.B - 127;
							num7++;
							num8++;
						}
						float radians3 = (float)(random.NextDouble() * Math.PI);
						identity = Matrix.CreateScale(num) * Matrix.CreateRotationY(radians3);
						num7 -= 12;
						for (float num24 = 0f; num24 < 12f; num24++)
						{
							array[num7].Position -= zero;
							array[num7].Position = Vector3.Transform(array[num7].Position, identity);
							array[num7].Position += zero;
							num7++;
						}
					}
				}
				ts.numVertices[num9] = num7 - ts.startVertice[num9];
				ts.numPrimitives[num9] = ts.numVertices[num9] / 6 * 4;
				num9++;
			}
		}
		ts.vertexBuffer[0] = new VertexBuffer(EndGameEngine.GraphicMgr.GraphicsDevice, typeof(VRT_TerrainVegitation), num7, BufferUsage.None);
		ts.vertexBuffer[0].SetData(array, 0, num7);
		ts.totalVertices = num7;
		ts.totalPrimitives = num7 / 6 * 4;
	}

	public void GenerateTreeMesh()
	{
		int num = 2;
		_ = SizeX / num;
		_ = SizeZ / num;
		Vector3 normal = Vector3.Zero;
		Random random = new Random(5);
		for (int i = 0; i < 24; i++)
		{
			tmptreeScale[i] = 0.8f + (float)random.NextDouble() * 0.5f;
			ref Matrix reference = ref tmptreeRots[i];
			reference = Matrix.CreateScale(tmptreeScale[i]) * Matrix.CreateRotationY((float)random.NextDouble() * 6.28f);
		}
		NumTreeGridX = 8;
		NumTreeGridZ = 8;
		List<TreePositionStruct> list = new List<TreePositionStruct>();
		for (int j = 0; j < numTreePerGrid; j++)
		{
			TreePositionStruct treePositionStruct = new TreePositionStruct();
			treePositionStruct.position = Vector3.Zero;
			treePositionStruct.position.X = (float)random.NextDouble() * 16384f;
			treePositionStruct.position.Z = (float)random.NextDouble() * 16384f;
			treePositionStruct.position.X = ((treePositionStruct.position.X > 256f) ? treePositionStruct.position.X : (treePositionStruct.position.X + 256f));
			treePositionStruct.position.Z = ((treePositionStruct.position.Z > 256f) ? treePositionStruct.position.Z : (treePositionStruct.position.Z + 256f));
			treePositionStruct.position.X = ((treePositionStruct.position.X < 16128f) ? treePositionStruct.position.X : (treePositionStruct.position.X - 256f));
			treePositionStruct.position.Z = ((treePositionStruct.position.Z < 16128f) ? treePositionStruct.position.Z : (treePositionStruct.position.Z - 256f));
			treePositionStruct.position.Y = HeightMapPhysics.GetHeight(ref treePositionStruct.position) - 48f;
			bool flag = true;
			for (int k = 0; k < list.Count; k++)
			{
				if ((treePositionStruct.position - list[k].position).LengthSquared() < 160000f)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				list.Add(treePositionStruct);
			}
		}
		numTreePerGrid = list.Count;
		TreePositions = list.ToArray();
		for (int l = 0; l < numTreePerGrid; l++)
		{
			TreePositions[l].rotation = (byte)random.Next(0, 24);
		}
		int num2 = 2048;
		TreeQuickLookupTable = new List<Vector2>[8, 8];
		for (int m = 0; m < 8; m++)
		{
			for (int n = 0; n < 8; n++)
			{
				int num3 = m * num2;
				int num4 = n * num2;
				int num5 = num3 + num2;
				int num6 = num4 + num2;
				TreeQuickLookupTable[m, n] = new List<Vector2>();
				for (int num7 = 0; num7 < numTreePerGrid; num7++)
				{
					if (TreePositions[num7].position.X >= (float)num3 && TreePositions[num7].position.X <= (float)num5 && TreePositions[num7].position.Z >= (float)num4 && TreePositions[num7].position.Z <= (float)num6)
					{
						Vector2 zero = Vector2.Zero;
						zero.X = TreePositions[num7].position.X;
						zero.Y = TreePositions[num7].position.Z;
						TreeQuickLookupTable[m, n].Add(zero);
					}
				}
			}
		}
		int num8 = numTreePerGrid * 6;
		NumberTreeThisFrame = new int[2];
		TreeRenderQue = new TreePositionStruct[numTreeModel, 2];
		for (int num9 = 0; num9 < numTreeModel; num9++)
		{
			TreeRenderQue[num9, 0] = new TreePositionStruct();
			TreeRenderQue[num9, 1] = new TreePositionStruct();
		}
		TreeGrid = new TreeStruct[NumTreeGridX, NumTreeGridZ];
		for (int num10 = 0; num10 < NumTreeGridX; num10++)
		{
			for (int num11 = 0; num11 < NumTreeGridZ; num11++)
			{
				TreeGrid[num10, num11] = default(TreeStruct);
				TreeGrid[num10, num11].PlayerRenderLOD = new bool[4, 2];
				TreeGrid[num10, num11].PlayerRenderMOD = new bool[4, 2];
				TreeGrid[num10, num11].NumBillboardVertices = 0;
				TreeGrid[num10, num11].NumBillboardPrimitives = 0;
			}
		}
		for (int num12 = 0; num12 < NumTreeGridX; num12++)
		{
			for (int num13 = 0; num13 < NumTreeGridZ; num13++)
			{
				_ = Vector2.Zero;
				_ = Vector3.Zero;
				Vector3 zero2 = Vector3.Zero;
				VRT_TreeLOD[] array = new VRT_TreeLOD[num8];
				int num14 = 0;
				float num15 = float.MaxValue;
				float num16 = float.MinValue;
				for (int num17 = 0; num17 < numTreePerGrid; num17++)
				{
					zero2 = TreePositions[num17].position;
					zero2.X += num12 * 16384;
					zero2.Z += num13 * 16384;
					zero2.X -= 65536f;
					zero2.Z -= 65536f;
					zero2.Y = HeightMapPhysics.GetHeight(ref zero2, out normal) - 48f;
					num15 = ((zero2.Y < num15) ? zero2.Y : num15);
					num16 = ((zero2.Y > num16) ? zero2.Y : num16);
					byte r = 0;
					byte r2 = 51;
					Vector2 vector = new Vector2(60f, 1400f);
					byte b = HeightMapPhysics.ReadTreeMap(ref zero2);
					if (b != 0)
					{
						switch (b / TreeTypeDivisor)
						{
						case 1:
							r = 51;
							r2 = 102;
							break;
						case 2:
							r = 102;
							r2 = 153;
							break;
						case 3:
							r = 153;
							r2 = 204;
							break;
						case 4:
							r = 204;
							r2 = byte.MaxValue;
							break;
						}
						Color normal2 = new Color((byte)((normal.X + 1f) * 127f), (byte)((normal.Y + 1f) * 127f), (byte)((normal.Z + 1f) * 127f), 0);
						vector *= tmptreeScale[TreePositions[num17].rotation];
						array[num14].Texcoord.R = r;
						array[num14].Texcoord.G = 0;
						array[num14].Texcoord.B = (byte)(0f - vector.X + 127f);
						array[num14].Texcoord.A = byte.MaxValue;
						array[num14].Normal = normal2;
						array[num14].Position = zero2;
						array[num14++].Position.Y += vector.Y;
						array[num14].Texcoord.R = r;
						array[num14].Texcoord.G = 0;
						array[num14].Texcoord.B = (byte)(0f - vector.X + 127f);
						array[num14].Texcoord.A = byte.MaxValue;
						array[num14].Normal = normal2;
						array[num14].Position = zero2;
						array[num14++].Position.Y += vector.Y;
						array[num14].Texcoord.R = r2;
						array[num14].Texcoord.G = 0;
						array[num14].Texcoord.B = (byte)(vector.X + 127f);
						array[num14].Texcoord.A = byte.MaxValue;
						array[num14].Normal = normal2;
						array[num14].Position = zero2;
						array[num14++].Position.Y += vector.Y;
						array[num14].Texcoord.R = r;
						array[num14].Texcoord.G = byte.MaxValue;
						array[num14].Texcoord.B = (byte)(0f - vector.X + 127f);
						array[num14].Texcoord.A = 0;
						array[num14].Normal = normal2;
						array[num14++].Position = zero2;
						array[num14].Texcoord.R = r2;
						array[num14].Texcoord.G = byte.MaxValue;
						array[num14].Texcoord.B = (byte)(vector.X + 127f);
						array[num14].Texcoord.A = 0;
						array[num14].Normal = normal2;
						array[num14++].Position = zero2;
						array[num14].Texcoord.R = r2;
						array[num14].Texcoord.G = byte.MaxValue;
						array[num14].Texcoord.B = (byte)(vector.X + 127f);
						array[num14].Texcoord.A = 0;
						array[num14].Normal = normal2;
						array[num14++].Position = zero2;
					}
				}
				TreeGrid[num12, num13].bBox.Min.Y = num15;
				TreeGrid[num12, num13].bBox.Min.X = num12 * 16384 - 65536;
				TreeGrid[num12, num13].bBox.Min.Z = num13 * 16384 - 65536;
				TreeGrid[num12, num13].bBox.Max.Y = num16;
				TreeGrid[num12, num13].bBox.Max.X = TreeGrid[num12, num13].bBox.Min.X + 16384f;
				TreeGrid[num12, num13].bBox.Max.Z = TreeGrid[num12, num13].bBox.Min.Z + 16384f;
				if (num14 > 0)
				{
					TreeGrid[num12, num13].NumBillboardVertices = num14;
					TreeGrid[num12, num13].NumBillboardPrimitives = num14 - 2;
					TreeGrid[num12, num13].BillboardVertexBuffer = new VertexBuffer(EndGameEngine.GraphicMgr.GraphicsDevice, typeof(VRT_TreeLOD), num14, BufferUsage.None);
					TreeGrid[num12, num13].BillboardVertexBuffer.SetData(array, 0, num14);
				}
				else
				{
					TreeGrid[num12, num13].NumBillboardVertices = 0;
					TreeGrid[num12, num13].NumBillboardPrimitives = 0;
				}
			}
		}
	}

	public int GetTreePositions(ref Vector3 position, int qIndex, PlayerBase playerRef)
	{
		if (!m_Valid)
		{
			return 0;
		}
		Vector3 normal = Vector3.Zero;
		Vector3 position2 = position;
		float num = position.X + 65536f;
		float num2 = position.Z + 65536f;
		int num3 = (int)Math.Floor(num / 16384f);
		int num4 = (int)Math.Floor(num2 / 16384f);
		num3 = ((num3 >= 0) ? num3 : 0);
		num4 = ((num4 >= 0) ? num4 : 0);
		int num5 = 8;
		int num6 = 8;
		num5 = ((num3 + 1 < num5) ? (num3 + 1) : num5);
		num6 = ((num4 + 1 < num6) ? (num4 + 1) : num6);
		int num7 = 0;
		for (int i = num3; i < num5; i++)
		{
			for (int j = num4; j < num6; j++)
			{
				if (TreeGrid[i, j].NumBillboardPrimitives <= 0)
				{
					continue;
				}
				int num8 = 2048;
				int num9 = (int)Math.Floor((position.X - TreeGrid[i, j].bBox.Min.X) / (float)num8);
				int num10 = (int)Math.Floor((position.Z - TreeGrid[i, j].bBox.Min.Z) / (float)num8);
				num9 = ((num9 > 0) ? num9 : 0);
				num10 = ((num10 > 0) ? num10 : 0);
				int count = TreeQuickLookupTable[num9, num10].Count;
				for (int k = 0; k < count; k++)
				{
					if (num7 >= MaxGTPList)
					{
						break;
					}
					position2.Y = position.Y;
					position2.X = TreeGrid[i, j].bBox.Min.X + TreeQuickLookupTable[num9, num10][k].X;
					position2.Z = TreeGrid[i, j].bBox.Min.Z + TreeQuickLookupTable[num9, num10][k].Y;
					float num11 = (position2 - position).LengthSquared();
					if (num11 < 160000f)
					{
						position2.Y = HeightMapPhysics.GetHeight(ref position2, out normal) - 48f;
						byte b = HeightMapPhysics.ReadTreeMap(ref position2);
						if (b > 0)
						{
							GetTreePosList[num7] = position2;
							num7++;
						}
					}
				}
			}
		}
		return num7;
	}
}
