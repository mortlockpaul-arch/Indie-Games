using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Helper class that provides quick and easy triangle rendering.
/// </summary>
public class TriangleRenderHelper : BasePrimitiveRenderHelper
{
	/// <summary>
	/// Primitive type used by the render helper.
	/// </summary>
	protected override PrimitiveType PrimitiveType => PrimitiveType.TriangleList;

	/// <summary>
	/// Creates a new TriangleRenderHelper instance.
	/// </summary>
	public TriangleRenderHelper()
	{
	}

	/// <summary>
	/// Submits a single triangle to the render helper.
	///
	/// Please note: all vertices and primitives contained in the render helper
	/// *must* be in the same space (ie: object space, world space, ...), as they
	/// are rendered all at once in the Render method using the same effect
	/// property values (including world, view, and projection transforms).
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <param name="c"></param>
	/// <param name="acolor"></param>
	/// <param name="bcolor"></param>
	/// <param name="ccolor"></param>
	public void Submit(Vector3 a, Vector3 b, Vector3 c, Color acolor, Color bcolor, Color ccolor)
	{
		SubmitVertex(ref a, ref acolor);
		SubmitVertex(ref b, ref bcolor);
		SubmitVertex(ref c, ref ccolor);
	}

	/// <summary>
	/// Submits a single triangle to the render helper.
	///
	/// Please note: all vertices and primitives contained in the render helper
	/// *must* be in the same space (ie: object space, world space, ...), as they
	/// are rendered all at once in the Render method using the same effect
	/// property values (including world, view, and projection transforms).
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <param name="c"></param>
	/// <param name="color"></param>
	public void Submit(Vector3 a, Vector3 b, Vector3 c, Color color)
	{
		SubmitVertex(ref a, ref color);
		SubmitVertex(ref b, ref color);
		SubmitVertex(ref c, ref color);
	}
}
