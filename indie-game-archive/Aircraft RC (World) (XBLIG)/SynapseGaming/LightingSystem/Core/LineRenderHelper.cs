using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Helper class that provides quick and easy line rendering.
/// </summary>
public class LineRenderHelper : BasePrimitiveRenderHelper
{
	/// <summary>
	/// Primitive type used by the render helper.
	/// </summary>
	protected override PrimitiveType PrimitiveType => PrimitiveType.LineList;

	/// <summary>
	/// Creates a new LineRenderHelper instance.
	/// </summary>
	public LineRenderHelper()
	{
	}

	/// <summary>
	/// Submits a single line to the render helper.
	///
	/// Please note: all vertices and primitives contained in the render helper
	/// *must* be in the same space (ie: object space, world space, ...), as they
	/// are rendered all at once in the Render method using the same effect
	/// property values (including world, view, and projection transforms).
	/// </summary>
	/// <param name="start"></param>
	/// <param name="end"></param>
	/// <param name="startcolor"></param>
	/// <param name="endcolor"></param>
	public void Submit(Vector3 start, Vector3 end, Color startcolor, Color endcolor)
	{
		SubmitVertex(ref start, ref startcolor);
		SubmitVertex(ref end, ref endcolor);
	}

	/// <summary>
	/// Submits a single line to the render helper.
	///
	/// Please note: all vertices and primitives contained in the render helper
	/// *must* be in the same space (ie: object space, world space, ...), as they
	/// are rendered all at once in the Render method using the same effect
	/// property values (including world, view, and projection transforms).
	/// </summary>
	/// <param name="start"></param>
	/// <param name="end"></param>
	/// <param name="color"></param>
	public void Submit(Vector3 start, Vector3 end, Color color)
	{
		SubmitVertex(ref start, ref color);
		SubmitVertex(ref end, ref color);
	}
}
