using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Y
{
	internal interface _0006
	{
		b BroadPhase { get; }

		bool RayCast(Ray ray, IList<a> outputIntersections);

		bool RayCast(Ray ray, float maximumLength, IList<a> outputIntersections);

		void GetEntries(BoundingBox boundingShape, IList<a> overlaps);

		void GetEntries(BoundingSphere boundingShape, IList<a> overlaps);

		void GetEntries(BoundingFrustum boundingShape, IList<a> overlaps);
	}
}
namespace y
{
	internal enum _0006
	{
		DoubleSided,
		Clockwise,
		Counterclockwise
	}
}
