using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Effects;

namespace SynapseGaming.LightingSystem.Processors;

/// <summary />
public class SunBurnAlphaTestEffectReader_Indie : ContentTypeReader<SunBurnAlphaTestEffect>
{
	/// <summary />
	protected override SunBurnAlphaTestEffect Read(ContentReader input, SunBurnAlphaTestEffect instance)
	{
		IGraphicsDeviceService graphicsDeviceService = (IGraphicsDeviceService)input.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService));
		SunBurnAlphaTestEffect sunBurnAlphaTestEffect = new SunBurnAlphaTestEffect(graphicsDeviceService.GraphicsDevice);
		sunBurnAlphaTestEffect.Texture = input.ReadExternalReference<Texture2D>();
		sunBurnAlphaTestEffect.AlphaFunction = (CompareFunction)input.ReadInt32();
		sunBurnAlphaTestEffect.ReferenceAlpha = input.ReadInt32();
		sunBurnAlphaTestEffect.DiffuseColor = input.ReadVector3();
		sunBurnAlphaTestEffect.Alpha = input.ReadSingle();
		sunBurnAlphaTestEffect.VertexColorEnabled = input.ReadBoolean();
		if (input.ReadInt32() != 1234)
		{
			throw new Exception("Error loading asset.");
		}
		return sunBurnAlphaTestEffect;
	}
}
