using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PropModel;

namespace EGEngine;

public class Terrain : TerrainBase
{
	public class TerrainEffectParams
	{
		public EffectParameter matInvView;

		public EffectParameter matWorld;

		public EffectParameter matView;

		public EffectParameter matProj;

		public EffectParameter matViewProj;

		public EffectParameter matWorldViewProj;

		public EffectParameter matLightViewProj;

		public EffectParameter matTexProj;

		public EffectParameter matLightViewProj2;

		public EffectParameter matTexProj2;

		public EffectParameter matRotation;

		public EffectParameter vecLightDirection;

		public EffectParameter vecLightPosition;

		public EffectParameter vecLightColor;

		public EffectParameter vecAmbientLightColor;

		public EffectParameter vecFogColor;

		public EffectParameter vecWorldOffset;

		public EffectParameter vecEyePosition;

		public EffectParameter vecViewDirection;

		public EffectParameter vecTerrianParams;

		public EffectParameter uvDisplacement;

		public EffectParameter vecFrustumCorners;

		public EffectParameter vecDefLightPosition;

		public EffectParameter vecDefLightColor;

		public EffectParameter worldNormals;

		public EffectParameter worldTangents;

		public EffectParameter diffuseBlend;

		public EffectParameter diffuseBlendPalette;

		public EffectParameter DiffuseTex;

		public EffectParameter GrassDiffuse;

		public EffectParameter DirtDiffuse;

		public EffectParameter RockDiffuse;

		public EffectParameter PavementDiffuse;

		public EffectParameter GrassNormal;

		public EffectParameter DirtNormal;

		public EffectParameter RockNormal;

		public EffectParameter PavementNormal;

		public EffectParameter GravelDiffuse;

		public EffectParameter GravelNormal;

		public EffectParameter stickersTexture;

		public EffectParameter stickersNormTexture;

		public EffectParameter editTexture;

		public EffectParameter HeightmapOverlay;

		public EffectParameter HeightmapOverlayPalette;

		public EffectParameter windTime;

		public EffectParameter vecCameraRight;

		public EffectParameter gridTileOffset;

		public EffectParameter texCoords;

		public EffectTechnique T_Terrain;

		public void InitEffectParams(Effect mEffect)
		{
			matInvView = mEffect.Parameters["matInvView"];
			matWorld = mEffect.Parameters["matWorld"];
			matView = mEffect.Parameters["matView"];
			matProj = mEffect.Parameters["matProj"];
			matViewProj = mEffect.Parameters["matViewProj"];
			matWorldViewProj = mEffect.Parameters["matWorldViewProj"];
			matLightViewProj = mEffect.Parameters["matLightViewProj"];
			matTexProj = mEffect.Parameters["matTexProj"];
			matLightViewProj2 = mEffect.Parameters["matLightViewProj2"];
			matTexProj2 = mEffect.Parameters["matTexProj2"];
			matRotation = mEffect.Parameters["matRotation"];
			vecLightDirection = mEffect.Parameters["vecLightDirection"];
			vecLightPosition = mEffect.Parameters["vecLightPosition"];
			vecLightColor = mEffect.Parameters["vecLightColor"];
			vecAmbientLightColor = mEffect.Parameters["vecAmbientLightColor"];
			vecFogColor = mEffect.Parameters["vecFogColor"];
			vecWorldOffset = mEffect.Parameters["vecWorldOffset"];
			vecEyePosition = mEffect.Parameters["vecEyePosition"];
			vecViewDirection = mEffect.Parameters["vecViewDirection"];
			vecTerrianParams = mEffect.Parameters["vecTerrianParams"];
			uvDisplacement = mEffect.Parameters["uvDisplacement"];
			vecFrustumCorners = mEffect.Parameters["vecFrustumCorners"];
			vecDefLightPosition = mEffect.Parameters["vecDefLightPosition"];
			vecDefLightColor = mEffect.Parameters["vecDefLightColor"];
			worldNormals = mEffect.Parameters["worldNormals"];
			worldTangents = mEffect.Parameters["worldTangents"];
			diffuseBlend = mEffect.Parameters["diffuseBlend"];
			diffuseBlendPalette = mEffect.Parameters["diffuseBlendPalette"];
			DiffuseTex = mEffect.Parameters["DiffuseTex"];
			GrassDiffuse = mEffect.Parameters["GrassDiffuse"];
			DirtDiffuse = mEffect.Parameters["DirtDiffuse"];
			RockDiffuse = mEffect.Parameters["RockDiffuse"];
			PavementDiffuse = mEffect.Parameters["PavementDiffuse"];
			GrassNormal = mEffect.Parameters["GrassNormal"];
			DirtNormal = mEffect.Parameters["DirtNormal"];
			RockNormal = mEffect.Parameters["RockNormal"];
			PavementNormal = mEffect.Parameters["PavementNormal"];
			GravelDiffuse = mEffect.Parameters["GravelDiffuse"];
			GravelNormal = mEffect.Parameters["GravelNormal"];
			stickersTexture = mEffect.Parameters["stickersTexture"];
			stickersNormTexture = mEffect.Parameters["stickersNormTexture"];
			editTexture = mEffect.Parameters["editTexture"];
			HeightmapOverlay = mEffect.Parameters["HeightmapOverlay"];
			HeightmapOverlayPalette = mEffect.Parameters["HeightmapOverlayPalette"];
			windTime = mEffect.Parameters["windTime"];
			vecCameraRight = mEffect.Parameters["vecCameraRight"];
			gridTileOffset = mEffect.Parameters["gridTileOffset"];
			texCoords = mEffect.Parameters["texCoords"];
			T_Terrain = mEffect.Techniques["T_Terrain"];
		}
	}

	public const int MAX_LEVELS = 2;

	private bool OuputDiffuseTextureMap;

	private bool OuputSpecialTextureMap;

	private bool m_Valid;

	private int m_Size;

	private int m_SizeX;

	private int m_SizeY;

	private int m_NumPrimitives;

	private float m_Scale;

	private float m_MaxHeight;

	private float m_SeaDepth;

	private string m_HeightMapName;

	private Texture2D GrassMap;

	private Texture2D DirtMap;

	private Texture2D RockMap;

	private Texture2D PavementMap;

	private Texture2D GravelMap;

	private Texture2D GrassNorm;

	private Texture2D DirtNorm;

	private Texture2D RockNorm;

	private Texture2D PavementNorm;

	private Texture2D GravelNorm;

	private Texture2D HeightmapOverlay;

	private Texture2D HeightmapOverlayPalette;

	public static Effect TerrainEffect;

	public static TerrainEffectParams TerrainParams = new TerrainEffectParams();

	private bool m_TerrainLoading;

	private string m_CurrentLevel = "";

	private TerainLevelPacket[] m_CurrentLevels = new TerainLevelPacket[2]
	{
		new TerainLevelPacket("Yucca Wasteland", "lvl02", 256, 256, 512, 20000f, 800f),
		new TerainLevelPacket("Editor", "lvl02", 256, 256, 512, 20000f, 800f)
	};

	private TerrainStruct[] m_TerrainGrid = new TerrainStruct[5];

	private Vector3 tmpOffset = Vector3.Zero;

	private int HM_TEST_NUM = 3;

	private bool initializeShader = true;

	private Vector3 eyePosition = Vector3.Zero;

	public bool TerrainLoaded
	{
		get
		{
			return m_Valid;
		}
		set
		{
			m_Valid = value;
		}
	}

	public bool IsLoading
	{
		get
		{
			if (!m_Valid)
			{
				return m_TerrainLoading;
			}
			return true;
		}
		set
		{
			m_TerrainLoading = value;
		}
	}

	public string CurrentLevel => m_CurrentLevel;

	public void GetTerrianLevels(out TerainLevelPacket[] e)
	{
		e = m_CurrentLevels;
	}

	public void Update(float eTime, ref Vector3 cameraPos, int qIndex)
	{
		BoundingBox box = default(BoundingBox);
		if (!m_Valid)
		{
			return;
		}
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < m_TerrainGrid[i].numTiles; j++)
			{
				m_TerrainGrid[i].tileRender[(int)EndGameEngine.controllingPlayer.Value, j, qIndex] = false;
				tmpOffset = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecHeadPosition[qIndex];
				int num = (int)(tmpOffset.X / 1024f);
				int num2 = (int)(tmpOffset.Z / 1024f);
				tmpOffset.X = num * 1024;
				tmpOffset.Z = num2 * 1024;
				tmpOffset = tmpOffset - LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecHeadPosition[qIndex] + m_TerrainGrid[i].tileOffset[j];
				tmpOffset.Y = 0f;
				box.Min = m_TerrainGrid[i].aabb.Min + tmpOffset;
				box.Max = m_TerrainGrid[i].aabb.Max + tmpOffset;
				bool result = false;
				LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].bFrustum[qIndex].Intersects(ref box, out result);
				if (result)
				{
					m_TerrainGrid[i].tileRender[(int)EndGameEngine.controllingPlayer.Value, j, qIndex] = true;
					ref Vector3 reference = ref m_TerrainGrid[i].curOffset[(int)EndGameEngine.controllingPlayer.Value, j, qIndex];
					reference = tmpOffset;
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
		GraphicsDevice graphicsDevice = TerrainEffect.GraphicsDevice;
		graphicsDevice.VertexSamplerStates[0] = SamplerState.PointClamp;
		graphicsDevice.VertexSamplerStates[1] = SamplerState.PointWrap;
		if (initializeShader)
		{
			TerrainEffect.Parameters["FarZPlane"].SetValue(PlayerBase.FarZPlane * 0.9f);
			TerrainParams.diffuseBlend.SetValue(HeightMapPhysics.texAlphaMap);
			TerrainParams.diffuseBlendPalette.SetValue(HeightMapPhysics.texAlphaMapPalette);
			initializeShader = false;
		}
		TerrainParams.matViewProj.SetValue(playerRef.mDataQueue[qIndex].viewProj);
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullCC;
		graphicsDevice.VertexTextures[0] = HeightMapPhysics.texHeightMap;
		TerrainParams.vecEyePosition.SetValue(playerRef.mDataQueue[qIndex].cameraEyePos);
		for (int i = 0; i < HM_TEST_NUM; i++)
		{
			graphicsDevice.SetVertexBuffer(m_TerrainGrid[i].vertexBuffer);
			for (int j = 0; j < m_TerrainGrid[i].numTiles; j++)
			{
				if (m_TerrainGrid[i].tileRender[playerIndex, j, qIndex])
				{
					Vector3 value = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecHeadPosition[qIndex];
					int num = (int)(value.X / 1024f);
					int num2 = (int)(value.Z / 1024f);
					value.X = num * 1024;
					value.Z = num2 * 1024;
					value += m_TerrainGrid[i].tileOffset[j];
					value.Y = 0f;
					TerrainParams.gridTileOffset.SetValue(value);
					TerrainParams.vecWorldOffset.SetValue(m_TerrainGrid[i].curOffset[playerIndex, j, qIndex]);
					TerrainEffect.CurrentTechnique.Passes[0].Apply();
					graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, m_TerrainGrid[i].numPrimitives);
				}
			}
		}
	}

	public void Initialize(string terrainName)
	{
		TerrainEffect = EndGameEngine.ContentMgr.Load<Effect>("shaders\\ShaderTerrain");
		TerrainParams.InitEffectParams(TerrainEffect);
		_ = TerrainEffect.GraphicsDevice;
		m_Valid = false;
		m_SizeX = 1024;
		m_SizeY = 1024;
		m_Size = m_SizeX * m_SizeY;
		m_NumPrimitives = (m_SizeX * 2 + 2) * (m_SizeY - 1);
		m_Scale = 128f;
		m_MaxHeight = 1500f;
		m_SeaDepth = 64f;
		m_HeightMapName = "hbase";
		GenerateTerrainMesh(m_SizeX, m_SizeY, m_Scale);
	}

	public void Load(string terrainName)
	{
		m_CurrentLevel = "Yucca Wasteland";
		if (!m_TerrainLoading)
		{
			m_TerrainLoading = true;
			LoadTerrainThread();
		}
	}

	private void LoadTerrainThread()
	{
		m_Valid = false;
		for (int i = 0; i < 2; i++)
		{
			if (m_CurrentLevels[i].kname == m_CurrentLevel)
			{
				m_SizeX = m_CurrentLevels[i].sizex;
				m_SizeY = m_CurrentLevels[i].sizey;
				m_Size = m_SizeX * m_SizeY;
				m_NumPrimitives = (m_SizeX * 2 + 2) * (m_SizeY - 1);
				m_Scale = m_CurrentLevels[i].scale;
				m_MaxHeight = m_CurrentLevels[i].maxHeight;
				m_SeaDepth = m_CurrentLevels[i].seaDepth;
				m_HeightMapName = m_CurrentLevels[i].heightMap;
				if (!OuputDiffuseTextureMap)
				{
					_ = OuputSpecialTextureMap;
				}
				GrassMap = EndGameEngine.GameAssetMgr.Load<Texture2D>("terrain\\Grass");
				DirtMap = EndGameEngine.GameAssetMgr.Load<Texture2D>("terrain\\Dirt");
				PavementMap = EndGameEngine.GameAssetMgr.Load<Texture2D>("terrain\\Pavement00");
				GravelMap = EndGameEngine.GameAssetMgr.Load<Texture2D>("terrain\\Gravel");
				GrassNorm = EndGameEngine.GameAssetMgr.Load<Texture2D>("terrain\\Grass_norm");
				DirtNorm = EndGameEngine.GameAssetMgr.Load<Texture2D>("terrain\\Dirt_norm");
				PavementNorm = EndGameEngine.GameAssetMgr.Load<Texture2D>("terrain\\Pavement00_norm");
				GravelNorm = EndGameEngine.GameAssetMgr.Load<Texture2D>("terrain\\Gravel_norm");
				HeightmapOverlay = EndGameEngine.GameAssetMgr.Load<Texture2D>("terrain\\heightmapOverlay");
				Vector4 value = new Vector4(m_CurrentLevels[i].maxHeight, m_CurrentLevels[i].seaDepth, 0f, 0f);
				TerrainParams.vecTerrianParams.SetValue(value);
				m_Valid = true;
				break;
			}
		}
		TerrainParams.diffuseBlend.SetValue(HeightMapPhysics.texAlphaMap);
		TerrainParams.diffuseBlendPalette.SetValue(HeightMapPhysics.texAlphaMapPalette);
		TerrainParams.GrassDiffuse.SetValue(GrassMap);
		TerrainParams.GrassNormal.SetValue(GrassNorm);
		TerrainParams.DirtDiffuse.SetValue(DirtMap);
		TerrainParams.DirtNormal.SetValue(DirtNorm);
		TerrainParams.RockDiffuse.SetValue(RockMap);
		TerrainParams.RockNormal.SetValue(RockNorm);
		TerrainParams.PavementDiffuse.SetValue(PavementMap);
		TerrainParams.PavementNormal.SetValue(PavementNorm);
		TerrainParams.GravelDiffuse.SetValue(GravelMap);
		TerrainParams.GravelNormal.SetValue(GravelNorm);
		TerrainParams.HeightmapOverlay.SetValue(HeightmapOverlay);
		TerrainEffect.CurrentTechnique = TerrainParams.T_Terrain;
		m_TerrainLoading = false;
	}

	public void Finalize()
	{
		try
		{
			TerrainParams.diffuseBlend.SetValue(HeightMapPhysics.texAlphaMap);
			TerrainParams.diffuseBlendPalette.SetValue(HeightMapPhysics.texAlphaMapPalette);
		}
		catch (Exception threadExceptionArgument)
		{
			EndGameEngine.ThreadExceptionArgument = threadExceptionArgument;
		}
	}

	public void Release()
	{
		m_Valid = false;
	}

	public void GenerateTerrainMesh(int sizeX, int sizeY, float scale)
	{
		int[] array = new int[5] { 16, 16, 16, 32, 64 };
		int[] array2 = new int[5] { 256, 512, 1024, 1024, 1024 };
		for (int i = 0; i < 5; i++)
		{
			m_TerrainGrid[i].sizeX = array[i];
			m_TerrainGrid[i].sizeZ = array[i];
			m_TerrainGrid[i].scale = array2[i];
			GenerateMesh(ref m_TerrainGrid[i], i);
		}
		m_TerrainGrid[0].numTiles = 16;
		m_TerrainGrid[0].tileRender = new bool[4, 16, 2];
		m_TerrainGrid[0].curOffset = new Vector3[4, 16, 2];
		int num = m_TerrainGrid[0].sizeX * m_TerrainGrid[0].scale;
		int num2 = num / 2;
		m_TerrainGrid[0].tileOffset = new Vector3[16]
		{
			new Vector3(-num2, 0f, num2),
			new Vector3(num2, 0f, num2),
			new Vector3(-num2, 0f, -num2),
			new Vector3(num2, 0f, -num2),
			new Vector3(-(num + num2), 0f, num + num2),
			new Vector3(-num2, 0f, num + num2),
			new Vector3(num2, 0f, num + num2),
			new Vector3(num + num2, 0f, num + num2),
			new Vector3(-(num + num2), 0f, num2),
			new Vector3(num + num2, 0f, num2),
			new Vector3(-(num + num2), 0f, -num2),
			new Vector3(num + num2, 0f, -num2),
			new Vector3(-(num + num2), 0f, -(num + num2)),
			new Vector3(-num2, 0f, -(num + num2)),
			new Vector3(num2, 0f, -(num + num2)),
			new Vector3(num + num2, 0f, -(num + num2))
		};
		m_TerrainGrid[1].numTiles = 12;
		m_TerrainGrid[1].tileRender = new bool[4, 12, 2];
		m_TerrainGrid[1].curOffset = new Vector3[4, 12, 2];
		num = (m_TerrainGrid[1].sizeX - 2) * m_TerrainGrid[1].scale;
		num2 = num / 2;
		m_TerrainGrid[1].tileOffset = new Vector3[12]
		{
			new Vector3(-(num + num2), 0f, num + num2),
			new Vector3(-num2, 0f, num + num2),
			new Vector3(num + num2, 0f, num + num2),
			new Vector3(num2, 0f, num + num2),
			new Vector3(-(num + num2), 0f, num2),
			new Vector3(num + num2, 0f, num2),
			new Vector3(-(num + num2), 0f, -num2),
			new Vector3(num + num2, 0f, -num2),
			new Vector3(-(num + num2), 0f, -(num + num2)),
			new Vector3(-num2, 0f, -(num + num2)),
			new Vector3(num + num2, 0f, -(num + num2)),
			new Vector3(num2, 0f, -(num + num2))
		};
		m_TerrainGrid[2].numTiles = 12;
		m_TerrainGrid[2].tileRender = new bool[4, 12, 2];
		m_TerrainGrid[2].curOffset = new Vector3[4, 12, 2];
		num = (m_TerrainGrid[2].sizeX - 2) * m_TerrainGrid[2].scale;
		num2 = num / 2;
		m_TerrainGrid[2].tileOffset = new Vector3[12]
		{
			new Vector3(-(num + num2), 0f, num + num2),
			new Vector3(-num2, 0f, num + num2),
			new Vector3(num + num2, 0f, num + num2),
			new Vector3(num2, 0f, num + num2),
			new Vector3(-(num + num2), 0f, num2),
			new Vector3(num + num2, 0f, num2),
			new Vector3(-(num + num2), 0f, -num2),
			new Vector3(num + num2, 0f, -num2),
			new Vector3(-(num + num2), 0f, -(num + num2)),
			new Vector3(-num2, 0f, -(num + num2)),
			new Vector3(num + num2, 0f, -(num + num2)),
			new Vector3(num2, 0f, -(num + num2))
		};
		m_TerrainGrid[3].numTiles = 12;
		m_TerrainGrid[3].tileRender = new bool[4, 12, 2];
		m_TerrainGrid[3].curOffset = new Vector3[4, 12, 2];
		num = (m_TerrainGrid[3].sizeX - 2) * m_TerrainGrid[3].scale;
		num2 = num / 2;
		m_TerrainGrid[3].tileOffset = new Vector3[12]
		{
			new Vector3(-(num + num2), 0f, num + num2),
			new Vector3(-num2, 0f, num + num2),
			new Vector3(num + num2, 0f, num + num2),
			new Vector3(num2, 0f, num + num2),
			new Vector3(-(num + num2), 0f, num2),
			new Vector3(num + num2, 0f, num2),
			new Vector3(-(num + num2), 0f, -num2),
			new Vector3(num + num2, 0f, -num2),
			new Vector3(-(num + num2), 0f, -(num + num2)),
			new Vector3(-num2, 0f, -(num + num2)),
			new Vector3(num + num2, 0f, -(num + num2)),
			new Vector3(num2, 0f, -(num + num2))
		};
		m_TerrainGrid[4].numTiles = 12;
		m_TerrainGrid[4].tileRender = new bool[4, 12, 2];
		m_TerrainGrid[4].curOffset = new Vector3[4, 12, 2];
		num = (m_TerrainGrid[4].sizeX - 2) * m_TerrainGrid[4].scale;
		num2 = num / 2;
		m_TerrainGrid[4].tileOffset = new Vector3[12]
		{
			new Vector3(-(num + num2), 0f, num + num2),
			new Vector3(-num2, 0f, num + num2),
			new Vector3(num + num2, 0f, num + num2),
			new Vector3(num2, 0f, num + num2),
			new Vector3(-(num + num2), 0f, num2),
			new Vector3(num + num2, 0f, num2),
			new Vector3(-(num + num2), 0f, -num2),
			new Vector3(num + num2, 0f, -num2),
			new Vector3(-(num + num2), 0f, -(num + num2)),
			new Vector3(-num2, 0f, -(num + num2)),
			new Vector3(num + num2, 0f, -(num + num2)),
			new Vector3(num2, 0f, -(num + num2))
		};
		for (int j = 0; j < 5; j++)
		{
			Vector3 zero = Vector3.Zero;
			Vector3 zero2 = Vector3.Zero;
			float num3 = m_TerrainGrid[j].sizeX * m_TerrainGrid[j].scale / 2;
			zero.X = 0f - num3;
			zero2.X = num3;
			zero.Z = 0f - num3;
			zero2.Z = num3;
			zero.Y = 0f;
			zero2.Y = 20000f;
			m_TerrainGrid[j].aabb = new BoundingBox(zero, zero2);
		}
	}

	public void GenerateMesh(ref TerrainStruct ts, int gridLevel)
	{
		bool flag = false;
		if (gridLevel > 0 && gridLevel < 5)
		{
			ts.sizeX += 2;
			ts.sizeZ += 2;
			flag = true;
		}
		ts.numVertices = ts.sizeX * ((ts.sizeX + 1) * 2 + 2);
		ts.numPrimitives = ((ts.sizeX + 1) * 2 + 2) * ts.sizeX - 2;
		ts.vertexBuffer = new VertexBuffer(EndGameEngine.GraphicMgr.GraphicsDevice, typeof(VERT_TERRIAN), ts.numVertices, BufferUsage.None);
		Vector3 position = Vector3.Zero;
		Vector3 zero = Vector3.Zero;
		VERT_TERRIAN[] array = new VERT_TERRIAN[ts.numVertices];
		int num = -(ts.sizeX / 2);
		int num2 = ts.sizeX / 2;
		int num3 = -(ts.sizeZ / 2);
		int num4 = ts.sizeZ / 2;
		int num5 = 0;
		for (int i = num; i < num2; i++)
		{
			for (int j = num3; j < num4 + 1; j++)
			{
				if (flag && (j == num3 || j == num4))
				{
					position.Y = -128f;
					zero.Y = -128f;
				}
				else if (flag && i == num)
				{
					position.Y = -128f;
					zero.Y = 0f;
				}
				else if (flag && i == num2 - 1)
				{
					position.Y = 0f;
					zero.Y = -128f;
				}
				else
				{
					position.Y = 0f;
					zero.Y = 0f;
				}
				position.X = i * ts.scale;
				position.Z = j * ts.scale;
				zero.X = (i + 1) * ts.scale;
				zero.Z = j * ts.scale;
				array[num5].Position = position;
				num5++;
				array[num5].Position = zero;
				num5++;
			}
			position = array[num5 - 1].Position;
			array[num5].Position = position;
			num5++;
			if (flag)
			{
				zero.Y = -128f;
			}
			else
			{
				zero.Y = 0f;
			}
			zero.X = (i + 1) * ts.scale;
			zero.Z = num3 * ts.scale;
			array[num5].Position = zero;
			num5++;
		}
		ts.vertexBuffer.SetData(array);
	}

	public void GenerateNormalTangentMap(int sizeX, int sizeY, float scale)
	{
		Color[] array = new Color[sizeX * sizeY];
		Color[] array2 = new Color[sizeX * sizeY];
		uint[] array3 = new uint[sizeX * sizeY];
		Vector3[] array4 = new Vector3[sizeX * sizeY];
		Vector3[] array5 = new Vector3[sizeX * sizeY];
		Vector3[] array6 = new Vector3[sizeX * sizeY];
		for (int i = 0; i < sizeX * sizeY; i++)
		{
			ref Vector3 reference = ref array4[i];
			reference = Vector3.Zero;
			ref Vector3 reference2 = ref array5[i];
			reference2 = Vector3.Zero;
			ref Vector3 reference3 = ref array6[i];
			reference3 = Vector3.Zero;
		}
		for (int j = 0; j < sizeX; j++)
		{
			for (int k = 0; k < sizeY; k++)
			{
				int num = ((j + 1 >= sizeX) ? (sizeX - 1) : (j + 1));
				int num2 = ((k + 1 >= sizeY) ? (sizeY - 1) : (k + 1));
				int num3 = j + k * sizeX;
				int num4 = num + k * sizeX;
				int num5 = j + num2 * sizeX;
				float y = HeightMapPhysics.Heightmap[j, k];
				float y2 = HeightMapPhysics.Heightmap[num, k];
				float y3 = HeightMapPhysics.Heightmap[j, num2];
				Vector3 vector = new Vector3(0f, y, 0f);
				Vector3 vector2 = new Vector3(scale, y2, 0f);
				Vector3 vector3 = new Vector3(0f, y3, scale);
				Vector2 vector4 = new Vector2(0f, 0f);
				Vector2 vector5 = new Vector2(1f, 0f);
				Vector2 vector6 = new Vector2(0f, 1f);
				float num6 = vector2.X - vector.X;
				float num7 = vector3.X - vector.X;
				float num8 = vector2.Y - vector.Y;
				float num9 = vector3.Y - vector.Y;
				float num10 = vector2.Z - vector.Z;
				float num11 = vector3.Z - vector.Z;
				float num12 = vector5.X - vector4.X;
				float num13 = vector6.X - vector4.X;
				float num14 = vector5.Y - vector4.Y;
				float num15 = vector6.Y - vector4.Y;
				float num16 = 1f / (num12 * num15 - num13 * num14);
				Vector3 vector7 = new Vector3((num15 * num6 - num14 * num7) * num16, (num15 * num8 - num14 * num9) * num16, (num15 * num10 - num14 * num11) * num16);
				Vector3 vector8 = new Vector3((num12 * num7 - num13 * num6) * num16, (num12 * num9 - num13 * num8) * num16, (num12 * num11 - num13 * num10) * num16);
				array5[num3] += vector7;
				array5[num4] += vector7;
				array5[num5] += vector7;
				array6[num3] += vector8;
				array6[num4] += vector8;
				array6[num5] += vector8;
				Vector3 vector9 = new Vector3(num6, num8, num10);
				Vector3 vector10 = new Vector3(num7, num9, num11);
				Vector3 vector11 = Vector3.Cross(vector10, vector9);
				vector11.Normalize();
				array4[num3] += vector11;
				array4[num4] += vector11;
				array4[num5] += vector11;
				num3 = j + num2 * sizeX;
				num4 = num + k * sizeX;
				num5 = num + num2 * sizeX;
				y = HeightMapPhysics.Heightmap[j, num2];
				y2 = HeightMapPhysics.Heightmap[num, k];
				y3 = HeightMapPhysics.Heightmap[num, num2];
				vector = new Vector3(0f, y3, scale);
				vector2 = new Vector3(scale, y2, 0f);
				vector3 = new Vector3(scale, y3, scale);
				vector4 = new Vector2(0f, 1f);
				vector5 = new Vector2(1f, 0f);
				vector6 = new Vector2(1f, 1f);
				num6 = vector2.X - vector.X;
				num7 = vector3.X - vector.X;
				num8 = vector2.Y - vector.Y;
				num9 = vector3.Y - vector.Y;
				num10 = vector2.Z - vector.Z;
				num11 = vector3.Z - vector.Z;
				num12 = vector5.X - vector4.X;
				num13 = vector6.X - vector4.X;
				num14 = vector5.Y - vector4.Y;
				num15 = vector6.Y - vector4.Y;
				num16 = 1f / (num12 * num15 - num13 * num14);
				vector7 = new Vector3((num15 * num6 - num14 * num7) * num16, (num15 * num8 - num14 * num9) * num16, (num15 * num10 - num14 * num11) * num16);
				vector8 = new Vector3((num12 * num7 - num13 * num6) * num16, (num12 * num9 - num13 * num8) * num16, (num12 * num11 - num13 * num10) * num16);
				array5[num3] += vector7;
				array5[num4] += vector7;
				array5[num5] += vector7;
				array6[num3] += vector8;
				array6[num4] += vector8;
				array6[num5] += vector8;
				vector9 = new Vector3(num6, num8, num10);
				vector10 = new Vector3(num7, num9, num11);
				vector11 = Vector3.Cross(vector10, vector9);
				vector11.Normalize();
				array4[num3] += vector11;
				array4[num4] += vector11;
				array4[num5] += vector11;
			}
		}
		for (int l = 0; l < sizeX * sizeY; l++)
		{
			Vector3 vector12 = array4[l];
			Vector3 vector13 = array5[l];
			vector12.Normalize();
			array[l].A = byte.MaxValue;
			array[l].R = (byte)((vector12.X + 1f) * 127.5f);
			array[l].G = (byte)((vector12.Y + 1f) * 127.5f);
			array[l].B = (byte)((vector12.Z + 1f) * 127.5f);
			Vector3 vector14 = vector13 - vector12 * Vector3.Dot(vector12, vector13);
			vector14.Normalize();
			float num17 = ((Vector3.Dot(Vector3.Cross(vector12, vector14), array6[l]) < 0f) ? (-1f) : 1f);
			array2[l].A = (byte)((num17 + 1f) * 127.5f);
			array2[l].R = (byte)((vector14.X + 1f) * 127.5f);
			array2[l].G = (byte)((vector14.Y + 1f) * 127.5f);
			array2[l].B = (byte)((vector14.Z + 1f) * 127.5f);
			array3[l] = 0u;
			float num18 = Vector3.Dot(vector12, Vector3.UnitY);
			if (num18 > 0.875f)
			{
				array3[l] |= 255u;
			}
			else if (Math.Abs(vector12.X) < Math.Abs(vector12.Z))
			{
				array3[l] |= 4278190080u;
			}
			else
			{
				array3[l] |= 16711680u;
			}
		}
		int num19 = 1024 / sizeX;
		int num20 = 1024 / sizeY;
		for (int m = 0; m < sizeX; m++)
		{
			for (int n = 0; n < sizeY; n++)
			{
				for (int num21 = 0; num21 < num19; num21++)
				{
					for (int num22 = 0; num22 < num20; num22++)
					{
					}
				}
			}
		}
	}

	public void GenerateNormalTangentMap_00(int sizeX, int sizeY, float scale)
	{
		Color[] array = new Color[sizeX * sizeY];
		Color[] array2 = new Color[sizeX * sizeY];
		uint[] array3 = new uint[sizeX * sizeY];
		Vector3[] array4 = new Vector3[sizeX * sizeY];
		Vector3[] array5 = new Vector3[sizeX * sizeY];
		Vector3[] array6 = new Vector3[sizeX * sizeY];
		for (int i = 0; i < sizeX * sizeY; i++)
		{
			ref Vector3 reference = ref array4[i];
			reference = Vector3.Zero;
			ref Vector3 reference2 = ref array5[i];
			reference2 = Vector3.Zero;
			ref Vector3 reference3 = ref array6[i];
			reference3 = Vector3.Zero;
		}
		for (int j = 0; j < sizeX; j++)
		{
			for (int k = 0; k < sizeY; k++)
			{
				int num = ((j + 1 >= sizeX) ? (sizeX - 1) : (j + 1));
				int num2 = ((k + 1 >= sizeY) ? (sizeY - 1) : (k + 1));
				int num3 = j + k * sizeX;
				int num4 = num + k * sizeX;
				int num5 = j + num2 * sizeX;
				float y = HeightMapPhysics.Heightmap[j, k];
				float y2 = HeightMapPhysics.Heightmap[num, k];
				float y3 = HeightMapPhysics.Heightmap[j, num2];
				Vector3 vector = new Vector3(0f, y, 0f);
				Vector3 vector2 = new Vector3(scale, y2, 0f);
				Vector3 vector3 = new Vector3(0f, y3, scale);
				Vector2 vector4 = new Vector2(0f, 0f);
				Vector2 vector5 = new Vector2(1f, 0f);
				Vector2 vector6 = new Vector2(0f, 1f);
				float num6 = vector2.X - vector.X;
				float num7 = vector3.X - vector.X;
				float num8 = vector2.Y - vector.Y;
				float num9 = vector3.Y - vector.Y;
				float num10 = vector2.Z - vector.Z;
				float num11 = vector3.Z - vector.Z;
				float num12 = vector5.X - vector4.X;
				float num13 = vector6.X - vector4.X;
				float num14 = vector5.Y - vector4.Y;
				float num15 = vector6.Y - vector4.Y;
				float num16 = 1f / (num12 * num15 - num13 * num14);
				Vector3 vector7 = new Vector3((num15 * num6 - num14 * num7) * num16, (num15 * num8 - num14 * num9) * num16, (num15 * num10 - num14 * num11) * num16);
				Vector3 vector8 = new Vector3((num12 * num7 - num13 * num6) * num16, (num12 * num9 - num13 * num8) * num16, (num12 * num11 - num13 * num10) * num16);
				array5[num3] += vector7;
				array5[num4] += vector7;
				array5[num5] += vector7;
				array6[num3] += vector8;
				array6[num4] += vector8;
				array6[num5] += vector8;
				Vector3 vector9 = new Vector3(num6, num8, num10);
				Vector3 vector10 = new Vector3(num7, num9, num11);
				Vector3 vector11 = Vector3.Cross(vector10, vector9);
				vector11.Normalize();
				array4[num3] += vector11;
				array4[num4] += vector11;
				array4[num5] += vector11;
				num3 = j + num2 * sizeX;
				num4 = num + k * sizeX;
				num5 = num + num2 * sizeX;
				y = HeightMapPhysics.Heightmap[j, num2];
				y2 = HeightMapPhysics.Heightmap[num, k];
				y3 = HeightMapPhysics.Heightmap[num, num2];
				vector = new Vector3(0f, y3, scale);
				vector2 = new Vector3(scale, y2, 0f);
				vector3 = new Vector3(scale, y3, scale);
				vector4 = new Vector2(0f, 1f);
				vector5 = new Vector2(1f, 0f);
				vector6 = new Vector2(1f, 1f);
				num6 = vector2.X - vector.X;
				num7 = vector3.X - vector.X;
				num8 = vector2.Y - vector.Y;
				num9 = vector3.Y - vector.Y;
				num10 = vector2.Z - vector.Z;
				num11 = vector3.Z - vector.Z;
				num12 = vector5.X - vector4.X;
				num13 = vector6.X - vector4.X;
				num14 = vector5.Y - vector4.Y;
				num15 = vector6.Y - vector4.Y;
				num16 = 1f / (num12 * num15 - num13 * num14);
				vector7 = new Vector3((num15 * num6 - num14 * num7) * num16, (num15 * num8 - num14 * num9) * num16, (num15 * num10 - num14 * num11) * num16);
				vector8 = new Vector3((num12 * num7 - num13 * num6) * num16, (num12 * num9 - num13 * num8) * num16, (num12 * num11 - num13 * num10) * num16);
				array5[num3] += vector7;
				array5[num4] += vector7;
				array5[num5] += vector7;
				array6[num3] += vector8;
				array6[num4] += vector8;
				array6[num5] += vector8;
				vector9 = new Vector3(num6, num8, num10);
				vector10 = new Vector3(num7, num9, num11);
				vector11 = Vector3.Cross(vector10, vector9);
				vector11.Normalize();
				array4[num3] += vector11;
				array4[num4] += vector11;
				array4[num5] += vector11;
			}
		}
		for (int l = 0; l < sizeX * sizeY; l++)
		{
			Vector3 vector12 = array4[l];
			Vector3 vector13 = array5[l];
			vector12.Normalize();
			array[l].A = byte.MaxValue;
			array[l].R = (byte)((vector12.X + 1f) * 127.5f);
			array[l].G = (byte)((vector12.Y + 1f) * 127.5f);
			array[l].B = (byte)((vector12.Z + 1f) * 127.5f);
			Vector3 vector14 = vector13 - vector12 * Vector3.Dot(vector12, vector13);
			vector14.Normalize();
			float num17 = ((Vector3.Dot(Vector3.Cross(vector12, vector14), array6[l]) < 0f) ? (-1f) : 1f);
			array2[l].A = (byte)((num17 + 1f) * 127.5f);
			array2[l].R = (byte)((vector14.X + 1f) * 127.5f);
			array2[l].G = (byte)((vector14.Y + 1f) * 127.5f);
			array2[l].B = (byte)((vector14.Z + 1f) * 127.5f);
			array3[l] = 0u;
			float num18 = Vector3.Dot(vector12, Vector3.UnitY);
			if (num18 > 0.96f)
			{
				array3[l] = 255u;
			}
			else if (num18 > 0.875f)
			{
				array3[l] |= 65280u;
			}
			else if (Math.Abs(vector12.X) < Math.Abs(vector12.Z))
			{
				array3[l] |= 4278190080u;
			}
			else
			{
				array3[l] |= 16711680u;
			}
		}
		HeightMapPhysics.AlphaMap = array3;
	}

	public void RenderHeight(ModelMesh mesh)
	{
		CustomContent customContent = mesh.Tag as CustomContent;
		Vector3[] positionsFromMesh = MeshTools.GetPositionsFromMesh(mesh, VertexType.BakedLight);
		Vector3 vector = Vector3.Transform(new OOBB(positionsFromMesh, customContent.transform).extents, customContent.transform) * 1.01f;
		Vector3 translation = customContent.transform.Translation;
		GraphicsDevice graphicsDevice = TerrainEffect.GraphicsDevice;
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.DepthStencilState = EndGameEngine.DepthEnabled;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
		TerrainEffect.CurrentTechnique = TerrainParams.T_Terrain;
		TerrainParams.matWorld.SetValue(Matrix.Identity);
		TerrainParams.matView.SetValue(Matrix.CreateLookAt(translation + new Vector3(0f, 100000f, 0f), translation, Vector3.Right));
		TerrainParams.matProj.SetValue(Matrix.CreateOrthographicOffCenter(0f - vector.Z, vector.Z, vector.X, 0f - vector.X, 0f, 500000f));
		Matrix transform = customContent.transform;
		TerrainParams.matWorld.SetValue(transform);
		foreach (ModelMeshPart meshPart in mesh.MeshParts)
		{
			graphicsDevice.SetVertexBuffer(meshPart.VertexBuffer, meshPart.VertexOffset);
			graphicsDevice.Indices = meshPart.IndexBuffer;
			TerrainEffect.CurrentTechnique.Passes[0].Apply();
			graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
		}
	}
}
