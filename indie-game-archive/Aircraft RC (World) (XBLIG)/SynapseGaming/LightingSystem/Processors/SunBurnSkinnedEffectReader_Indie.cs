using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Effects;

namespace SynapseGaming.LightingSystem.Processors;

/// <summary />
public class SunBurnSkinnedEffectReader_Indie : ContentTypeReader<SunBurnSkinnedEffect>
{
	/// <summary />
	protected override SunBurnSkinnedEffect Read(ContentReader input, SunBurnSkinnedEffect instance)
	{
		IGraphicsDeviceService graphicsDeviceService = (IGraphicsDeviceService)input.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService));
		SunBurnSkinnedEffect sunBurnSkinnedEffect = new SunBurnSkinnedEffect(graphicsDeviceService.GraphicsDevice);
		sunBurnSkinnedEffect.Texture = input.ReadExternalReference<Texture2D>();
		sunBurnSkinnedEffect.WeightsPerVertex = input.ReadInt32();
		sunBurnSkinnedEffect.DiffuseColor = input.ReadVector3();
		sunBurnSkinnedEffect.EmissiveColor = input.ReadVector3();
		sunBurnSkinnedEffect.SpecularColor = input.ReadVector3();
		sunBurnSkinnedEffect.SpecularPower = input.ReadSingle();
		sunBurnSkinnedEffect.Alpha = input.ReadSingle();
		if (input.ReadInt32() != 1234)
		{
			throw new Exception("Error loading asset.");
		}
		return sunBurnSkinnedEffect;
	}
}
