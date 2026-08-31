using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Helper class that provides quick and easy BoundingBox rendering.
/// </summary>
public class BoundingBoxRenderHelper : LineRenderHelper
{
	private static Vector3[] HCB = new Vector3[8];

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
		bounds.GetCorners(HCB);
		for (int i = 0; i < 3; i++)
		{
			Submit(HCB[i], HCB[i + 1], color);
			Submit(HCB[i + 4], HCB[i + 5], color);
			Submit(HCB[i], HCB[i + 4], color);
		}
		Submit(HCB[0], HCB[3], color);
		Submit(HCB[4], HCB[7], color);
		Submit(HCB[3], HCB[7], color);
	}
}
