using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Effects;
using SynapseGaming.LightingSystem.Effects.Forward;
using V;

namespace SynapseGaming.LightingSystem.Processors.Forward;

/// <summary />
public class XSIMaterialReader_Indie : ContentTypeReader<XSIEffect>
{
	/// <summary />
	protected override XSIEffect Read(ContentReader input, XSIEffect instance)
	{
		IGraphicsDeviceService graphicsDeviceService = (IGraphicsDeviceService)input.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService));
		bool skinned = input.ReadBoolean();
		EffectData effectData = input.ReadObject<EffectData>();
		XSIEffect xSIEffect = new XSIEffect(graphicsDeviceService.GraphicsDevice, effectData.ByteCode);
		xSIEffect.Skinned = skinned;
		V.B.H_0005(input);
		return xSIEffect;
	}
}
