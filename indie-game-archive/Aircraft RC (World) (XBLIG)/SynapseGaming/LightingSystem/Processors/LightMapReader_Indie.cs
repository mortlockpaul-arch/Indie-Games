using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Lights;
using V;

namespace SynapseGaming.LightingSystem.Processors;

/// <summary />
public class LightMapReader_Indie : ContentTypeReader<LightMap>
{
	/// <summary />
	protected override LightMap Read(ContentReader input, LightMap instance)
	{
		Texture2D colortexture = input.ReadObject<Texture2D>();
		Texture2D directionaltexture = input.ReadObject<Texture2D>();
		V.B.H_0005(input);
		if (input.ReadInt32() != 1234)
		{
			throw new Exception("Error loading asset.");
		}
		return new LightMap(colortexture, directionaltexture);
	}
}
