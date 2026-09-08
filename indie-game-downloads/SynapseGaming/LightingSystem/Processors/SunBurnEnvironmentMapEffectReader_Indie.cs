using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Effects;

namespace SynapseGaming.LightingSystem.Processors;

/// <summary />
public class SunBurnEnvironmentMapEffectReader_Indie : ContentTypeReader<SunBurnEnvironmentMapEffect>
{
	/// <summary />
	protected override SunBurnEnvironmentMapEffect Read(ContentReader input, SunBurnEnvironmentMapEffect instance)
	{
		IGraphicsDeviceService graphicsDeviceService = (IGraphicsDeviceService)input.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService));
		SunBurnEnvironmentMapEffect sunBurnEnvironmentMapEffect = new SunBurnEnvironmentMapEffect(graphicsDeviceService.GraphicsDevice);
		sunBurnEnvironmentMapEffect.Texture = input.ReadExternalReference<Texture2D>();
		sunBurnEnvironmentMapEffect.EnvironmentMap = input.ReadExternalReference<TextureCube>();
		sunBurnEnvironmentMapEffect.EnvironmentMapAmount = input.ReadSingle();
		sunBurnEnvironmentMapEffect.EnvironmentMapSpecular = input.ReadVector3();
		sunBurnEnvironmentMapEffect.FresnelFactor = input.ReadSingle();
		sunBurnEnvironmentMapEffect.DiffuseColor = input.ReadVector3();
		sunBurnEnvironmentMapEffect.EmissiveColor = input.ReadVector3();
		sunBurnEnvironmentMapEffect.Alpha = input.ReadSingle();
		if (input.ReadInt32() != 1234)
		{
			throw new Exception("Error loading asset.");
		}
		return sunBurnEnvironmentMapEffect;
	}
}
