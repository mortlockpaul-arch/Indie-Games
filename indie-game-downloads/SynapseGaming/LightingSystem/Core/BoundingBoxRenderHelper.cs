using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Helper class that provides quick and easy BoundingBox rendering.
/// </summary>
public class BoundingBoxRenderHelper : LineRenderHelper
{
	private static Vector3[] _3A_0018 = new Vector3[8];

	/// <summary>
	/// Creates a new BoundingBoxRenderHelper instance.
	/// </summary>
	public BoundingBoxRenderHelper()
	{
	}

	/// <summary>
	/// Submits a single BoundingBox to the render helper.
	///
	/// Please note: all BoundingBoxes contained in the render helper
	/// *must* be in the same space (ie: object space, world space, ...), as they
	/// are rendered all at once in the Render method using the same effect
	/// property values (including world, view, and projection transforms).
	/// </summary>
	/// <param name="bounds"></param>
	/// <param name="color"></param>
	public void Submit(BoundingBox bounds, Color color)
	{
		bounds.GetCorners(_3A_0018);
		for (int i = 0; i < 3; i++)
		{
			Submit(_3A_0018[i], _3A_0018[i + 1], color);
			Submit(_3A_0018[i + 4], _3A_0018[i + 5], color);
			Submit(_3A_0018[i], _3A_0018[i + 4], color);
		}
		Submit(_3A_0018[0], _3A_0018[3], color);
		Submit(_3A_0018[4], _3A_0018[7], color);
		Submit(_3A_0018[3], _3A_0018[7], color);
	}
}
