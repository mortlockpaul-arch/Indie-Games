using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xbox360Game1.Sprites;

public interface IExplosion
{
	Texture2D TextureOfExplosion { get; set; }

	List<Rectangle> SourceOfExplosion { get; set; }

	Vector2 PositionOfExplosion { get; set; }

	bool ShowExplosion { get; set; }

	int NextFrameIndexOfExplosion { get; set; }

	DateTime ExplosionFrameTime { get; set; }
}
