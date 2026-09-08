using System;
using Microsoft.Xna.Framework.Content;
using SynapseGaming.LightingSystem.Effects;

namespace SynapseGaming.LightingSystem.Processors;

/// <summary />
public class SunBurnInternalEffect_Indie : ContentTypeReader<EffectData>
{
	/// <summary />
	protected override EffectData Read(ContentReader input, EffectData instance)
	{
		int count = input.ReadInt32();
		EffectData result = new EffectData(input.ReadBytes(count));
		if (input.ReadInt32() != 1234)
		{
			throw new Exception("Error loading asset.");
		}
		return result;
	}
}
