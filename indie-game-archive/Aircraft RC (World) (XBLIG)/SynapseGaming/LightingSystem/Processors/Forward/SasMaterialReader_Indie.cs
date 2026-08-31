using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Effects;
using SynapseGaming.LightingSystem.Effects.Forward;
using V;

namespace SynapseGaming.LightingSystem.Processors.Forward;

/// <summary />
public class SasMaterialReader_Indie : ContentTypeReader<SasEffect>
{
	private GraphicsDevice HCB;

	private EffectData HC_0002;

	private Effect _0002R()
	{
		return SasEffect.H_0001(HCB, HC_0002.ByteCode, true);
	}

	/// <summary />
	protected override SasEffect Read(ContentReader input, SasEffect instance)
	{
		HCB = (input.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService).GraphicsDevice;
		HC_0002 = input.ReadObject<EffectData>();
		string text = input.ReadString();
		string text2 = input.ReadString();
		SasEffect sasEffect = ResourceManager._0002R(text2, _0002R) as SasEffect;
		sasEffect.MaterialName = text;
		sasEffect.MaterialFile = text2;
		sasEffect.ProjectFile = input.ReadString();
		sasEffect.Skinned = input.ReadBoolean();
		sasEffect.Elasticity = input.ReadSingle();
		sasEffect.Friction = input.ReadSingle();
		sasEffect.EffectFile = input.ReadString();
		Dictionary<string, object> dictionary = input.ReadObject<Dictionary<string, object>>();
		Dictionary<string, Texture> dictionary2 = input.ReadObject<Dictionary<string, Texture>>();
		sasEffect._0012L(dictionary2);
		sasEffect._0012h(dictionary);
		sasEffect._0012_0014();
		V.B.H_0005(input);
		if (input.ReadInt32() != 1234)
		{
			throw new Exception("Error loading asset.");
		}
		return sasEffect;
	}
}
