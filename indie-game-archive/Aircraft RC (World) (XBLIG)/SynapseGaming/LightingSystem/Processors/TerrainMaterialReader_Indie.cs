using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Effects;
using SynapseGaming.LightingSystem.Effects.Forward;
using SynapseGaming.LightingSystem.Rendering;
using V;

namespace SynapseGaming.LightingSystem.Processors;

/// <summary />
public class TerrainMaterialReader_Indie : ContentTypeReader<MeshData>
{
	/// <summary />
	protected override MeshData Read(ContentReader input, MeshData instance)
	{
		IGraphicsDeviceService graphicsDeviceService = (IGraphicsDeviceService)input.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService));
		MeshData meshData = new MeshData();
		input.ReadBoolean();
		BaseTerrainEffect baseTerrainEffect = (BaseTerrainEffect)(meshData.Effect = new TerrainEffect(graphicsDeviceService.GraphicsDevice));
		meshData.InfiniteBounds = false;
		meshData.MeshToObject = input.ReadMatrix();
		meshData.PrimitiveCount = input.ReadInt32();
		meshData.VertexCount = input.ReadInt32();
		meshData.VertexStride = input.ReadInt32();
		baseTerrainEffect.MeshSegments = input.ReadInt32();
		meshData.VertexBuffer = input.ReadObject<VertexBuffer>();
		meshData.IndexBuffer = input.ReadObject<IndexBuffer>();
		baseTerrainEffect.MaterialName = input.ReadString();
		baseTerrainEffect.MaterialFile = input.ReadString();
		baseTerrainEffect.ProjectFile = input.ReadString();
		baseTerrainEffect.DoubleSided = input.ReadBoolean();
		baseTerrainEffect.Elasticity = input.ReadSingle();
		baseTerrainEffect.Friction = input.ReadSingle();
		baseTerrainEffect.NormalMapStrength = input.ReadSingle();
		baseTerrainEffect.DiffuseScale = input.ReadSingle();
		baseTerrainEffect.HeightScale = input.ReadSingle();
		baseTerrainEffect.Tiling = input.ReadSingle();
		baseTerrainEffect.TileRepeatCount = input.ReadInt32();
		baseTerrainEffect.SpecularPower = input.ReadSingle();
		baseTerrainEffect.SpecularAmount = input.ReadSingle();
		meshData.ObjectSpaceBoundingBox = new BoundingBox(Vector3.Zero, new Vector3(0.5f));
		meshData.ObjectSpaceBoundingSphere = BoundingSphere.CreateFromBoundingBox(meshData.ObjectSpaceBoundingBox);
		Vector4 vector = input.ReadVector4();
		baseTerrainEffect.SpecularColor = new Vector3(vector.X, vector.Y, vector.Z);
		baseTerrainEffect.DiffuseMapLayer1File = input.ReadString();
		baseTerrainEffect.DiffuseMapLayer1Texture = input.ReadExternalReference<Texture2D>();
		baseTerrainEffect.DiffuseMapLayer2File = input.ReadString();
		baseTerrainEffect.DiffuseMapLayer2Texture = input.ReadExternalReference<Texture2D>();
		baseTerrainEffect.DiffuseMapLayer3File = input.ReadString();
		baseTerrainEffect.DiffuseMapLayer3Texture = input.ReadExternalReference<Texture2D>();
		baseTerrainEffect.DiffuseMapLayer4File = input.ReadString();
		baseTerrainEffect.DiffuseMapLayer4Texture = input.ReadExternalReference<Texture2D>();
		baseTerrainEffect.NormalMapLayer1File = input.ReadString();
		baseTerrainEffect.NormalMapLayer1Texture = input.ReadExternalReference<Texture2D>();
		baseTerrainEffect.NormalMapLayer2File = input.ReadString();
		baseTerrainEffect.NormalMapLayer2Texture = input.ReadExternalReference<Texture2D>();
		baseTerrainEffect.NormalMapLayer3File = input.ReadString();
		baseTerrainEffect.NormalMapLayer3Texture = input.ReadExternalReference<Texture2D>();
		baseTerrainEffect.NormalMapLayer4File = input.ReadString();
		baseTerrainEffect.NormalMapLayer4Texture = input.ReadExternalReference<Texture2D>();
		baseTerrainEffect.BlendMapFile = input.ReadString();
		baseTerrainEffect.BlendMapTexture = input.ReadExternalReference<Texture2D>();
		baseTerrainEffect.HeightMapFile = input.ReadString();
		baseTerrainEffect.HeightMapTexture = input.ReadExternalReference<Texture2D>();
		V.B.H_0005(input);
		if (input.ReadInt32() != 1234)
		{
			throw new Exception("Error loading asset.");
		}
		return meshData;
	}
}
