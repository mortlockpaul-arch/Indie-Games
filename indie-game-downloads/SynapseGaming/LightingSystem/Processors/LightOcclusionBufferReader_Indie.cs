using System;
using _7;
using Microsoft.Xna.Framework.Content;
using SynapseGaming.LightingSystem.Lights;

namespace SynapseGaming.LightingSystem.Processors;

/// <summary />
public class LightOcclusionBufferReader_Indie : ContentTypeReader<LightOcclusionBuffer>
{
	/// <summary />
	protected override LightOcclusionBuffer Read(ContentReader input, LightOcclusionBuffer instance)
	{
		LightOcclusionBuffer lightOcclusionBuffer = new LightOcclusionBuffer();
		input.ReadInt32();
		lightOcclusionBuffer.Ly(input);
		_7._0018._3_0013(input);
		if (input.ReadInt32() != 1234)
		{
			throw new Exception("Error loading asset.");
		}
		return lightOcclusionBuffer;
	}
}
