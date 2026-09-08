using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Effects;

namespace SynapseGaming.LightingSystem.Processors;

/// <summary />
public class SunBurnBasicEffectReader_Indie : ContentTypeReader<SunBurnBasicEffect>
{
	/// <summary />
	protected override SunBurnBasicEffect Read(ContentReader input, SunBurnBasicEffect instance)
	{
		IGraphicsDeviceService graphicsDeviceService = (IGraphicsDeviceService)input.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService));
		SunBurnBasicEffect sunBurnBasicEffect = new SunBurnBasicEffect(graphicsDeviceService.GraphicsDevice);
		Texture2D texture2D = input.ReadExternalReference<Texture2D>();
		if (texture2D != null)
		{
			sunBurnBasicEffect.Texture = texture2D;
			sunBurnBasicEffect.TextureEnabled = true;
		}
		sunBurnBasicEffect.DiffuseColor = input.ReadVector3();
		sunBurnBasicEffect.EmissiveColor = input.ReadVector3();
		sunBurnBasicEffect.SpecularColor = input.ReadVector3();
		sunBurnBasicEffect.SpecularPower = input.ReadSingle();
		sunBurnBasicEffect.Alpha = input.ReadSingle();
		sunBurnBasicEffect.VertexColorEnabled = input.ReadBoolean();
		if (input.ReadInt32() != 1234)
		{
			throw new Exception("Error loading asset.");
		}
		return sunBurnBasicEffect;
	}
}
