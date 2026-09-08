using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Processors;

/// <summary />
public class BoundingVolumeReader_Indie : ContentTypeReader<BoundingVolume>
{
	/// <summary />
	protected override BoundingVolume Read(ContentReader input, BoundingVolume instance)
	{
		BoundingVolume boundingVolume = new BoundingVolume();
		boundingVolume.BoundingBox = input.ReadObject<BoundingBox>();
		boundingVolume.BoundingSphere = input.ReadObject<BoundingSphere>();
		return boundingVolume;
	}
}
