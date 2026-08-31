using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Xbox360Game1.Sprites;

public interface IAnimation
{
	List<Rectangle> SourceRectangles { get; set; }

	int NextFrameIndex { get; set; }

	DateTime FrameTime { get; set; }
}
