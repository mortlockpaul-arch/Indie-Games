using System;
using _7;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Lights;

namespace SynapseGaming.LightingSystem.Processors;

/// <summary />
public class LightMapReader_Indie : ContentTypeReader<LightMap>
{
	/// <summary />
	protected override LightMap Read(ContentReader input, LightMap instance)
	{
		Texture2D colortexture = input.ReadObject<Texture2D>();
		Texture2D directionaltexture = input.ReadObject<Texture2D>();
		_7._0018._3_0013(input);
		if (input.ReadInt32() != 1234)
		{
			throw new Exception("Error loading asset.");
		}
		return new LightMap(colortexture, directionaltexture);
	}
}
