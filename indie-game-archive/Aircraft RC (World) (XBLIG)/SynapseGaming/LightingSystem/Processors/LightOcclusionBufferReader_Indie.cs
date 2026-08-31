using System;
using Microsoft.Xna.Framework.Content;
using SynapseGaming.LightingSystem.Lights;
using V;

namespace SynapseGaming.LightingSystem.Processors;

/// <summary />
public class LightOcclusionBufferReader_Indie : ContentTypeReader<LightOcclusionBuffer>
{
	/// <summary />
	protected override LightOcclusionBuffer Read(ContentReader input, LightOcclusionBuffer instance)
	{
		LightOcclusionBuffer lightOcclusionBuffer = new LightOcclusionBuffer();
		input.ReadInt32();
		lightOcclusionBuffer._0002A(input);
		V.B.H_0005(input);
		if (input.ReadInt32() != 1234)
		{
			throw new Exception("Error loading asset.");
		}
		return lightOcclusionBuffer;
	}
}
