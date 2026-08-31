using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Effects;

namespace SynapseGaming.LightingSystem.Processors;

/// <summary />
public class SunBurnDualTextureEffectReader_Indie : ContentTypeReader<SunBurnDualTextureEffect>
{
	/// <summary />
	protected override SunBurnDualTextureEffect Read(ContentReader input, SunBurnDualTextureEffect instance)
	{
		IGraphicsDeviceService graphicsDeviceService = (IGraphicsDeviceService)input.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService));
		SunBurnDualTextureEffect sunBurnDualTextureEffect = new SunBurnDualTextureEffect(graphicsDeviceService.GraphicsDevice);
		sunBurnDualTextureEffect.Texture = input.ReadExternalReference<Texture2D>();
		sunBurnDualTextureEffect.Texture2 = input.ReadExternalReference<Texture2D>();
		sunBurnDualTextureEffect.DiffuseColor = input.ReadVector3();
		sunBurnDualTextureEffect.Alpha = input.ReadSingle();
		sunBurnDualTextureEffect.VertexColorEnabled = input.ReadBoolean();
		if (input.ReadInt32() != 1234)
		{
			throw new Exception("Error loading asset.");
		}
		return sunBurnDualTextureEffect;
	}
}
