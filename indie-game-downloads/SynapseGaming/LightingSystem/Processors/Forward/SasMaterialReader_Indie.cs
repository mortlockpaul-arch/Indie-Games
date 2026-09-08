using System;
using System.Collections.Generic;
using _7;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Effects;
using SynapseGaming.LightingSystem.Effects.Forward;

namespace SynapseGaming.LightingSystem.Processors.Forward;

/// <summary />
public class SasMaterialReader_Indie : ContentTypeReader<SasEffect>
{
	private GraphicsDevice _3A_0018;

	private EffectData _3AL;

	private Effect L_0016()
	{
		return SasEffect._36(_3A_0018, _3AL.ByteCode, true);
	}

	/// <summary />
	protected override SasEffect Read(ContentReader input, SasEffect instance)
	{
		_3A_0018 = (input.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService).GraphicsDevice;
		_3AL = input.ReadObject<EffectData>();
		string text = input.ReadString();
		string text2 = input.ReadString();
		SasEffect sasEffect = ResourceManager.L_0016(text2, L_0016) as SasEffect;
		sasEffect.MaterialName = text;
		sasEffect.MaterialFile = text2;
		sasEffect.ProjectFile = input.ReadString();
		sasEffect.Skinned = input.ReadBoolean();
		sasEffect.Elasticity = input.ReadSingle();
		sasEffect.Friction = input.ReadSingle();
		sasEffect.EffectFile = input.ReadString();
		Dictionary<string, object> dictionary = input.ReadObject<Dictionary<string, object>>();
		Dictionary<string, Texture> dictionary2 = input.ReadObject<Dictionary<string, Texture>>();
		sasEffect._0019_0014(dictionary2);
		sasEffect._0019v(dictionary);
		sasEffect._0019z();
		_7._0018._3_0013(input);
		if (input.ReadInt32() != 1234)
		{
			throw new Exception("Error loading asset.");
		}
		return sasEffect;
	}
}
