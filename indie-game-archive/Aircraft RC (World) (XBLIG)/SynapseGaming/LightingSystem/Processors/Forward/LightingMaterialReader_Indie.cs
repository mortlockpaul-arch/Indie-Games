using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Effects.Forward;
using V;

namespace SynapseGaming.LightingSystem.Processors.Forward;

/// <summary />
public class LightingMaterialReader_Indie : ContentTypeReader<LightingEffect>
{
	private GraphicsDevice HCB;

	private Effect _0002R()
	{
		return new LightingEffect(HCB);
	}

	/// <summary />
	protected override LightingEffect Read(ContentReader input, LightingEffect instance)
	{
		HCB = (input.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService).GraphicsDevice;
		string text = input.ReadString();
		string text2 = input.ReadString();
		LightingEffect lightingEffect = ResourceManager._0002R(text2, _0002R) as LightingEffect;
		lightingEffect.MaterialName = text;
		lightingEffect.MaterialFile = text2;
		lightingEffect.ProjectFile = input.ReadString();
		lightingEffect.DiffuseMapFile = input.ReadString();
		lightingEffect.DiffuseMapTexture = input.ReadExternalReference<Texture2D>();
		lightingEffect.NormalMapFile = input.ReadString();
		lightingEffect.NormalMapTexture = input.ReadExternalReference<Texture2D>();
		lightingEffect.Skinned = input.ReadBoolean();
		lightingEffect.DoubleSided = input.ReadBoolean();
		TransparencyMode mode = (TransparencyMode)input.ReadInt32();
		float threshold = input.ReadSingle();
		lightingEffect.SetTransparencyModeAndMap(mode, threshold, lightingEffect.DiffuseMapTexture);
		lightingEffect.SpecularPower = input.ReadSingle();
		lightingEffect.SpecularAmount = input.ReadSingle();
		Vector4 vector = input.ReadVector4();
		lightingEffect.DiffuseColor = new Vector3(vector.X, vector.Y, vector.Z);
		lightingEffect.TransparencyAmount = vector.W;
		vector = input.ReadVector4();
		lightingEffect.EmissiveColor = new Vector3(vector.X, vector.Y, vector.Z);
		lightingEffect.Elasticity = input.ReadSingle();
		lightingEffect.Friction = input.ReadSingle();
		lightingEffect.AddressModeU = (TextureAddressMode)input.ReadInt32();
		lightingEffect.AddressModeV = (TextureAddressMode)input.ReadInt32();
		lightingEffect.AddressModeW = (TextureAddressMode)input.ReadInt32();
		V.B.H_0005(input);
		if (input.ReadInt32() != 1234)
		{
			throw new Exception("Error loading asset.");
		}
		return lightingEffect;
	}
}
