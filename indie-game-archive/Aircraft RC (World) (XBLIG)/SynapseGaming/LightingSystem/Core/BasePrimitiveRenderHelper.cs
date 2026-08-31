using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Base class used in primitive rendering helper classes.
/// </summary>
public abstract class BasePrimitiveRenderHelper
{
	private List<VertexPositionColor> HCB = new List<VertexPositionColor>();

	private static VertexPositionColor[] HC_0002 = new VertexPositionColor[2040];

	/// <summary>
	/// Primitive type. This is implemented by descendant classes.
	/// </summary>
	protected abstract PrimitiveType PrimitiveType { get; }

	/// <summary>
	/// Creates a new BasePrimitiveRenderHelper instance.
	/// </summary>
	public BasePrimitiveRenderHelper()
	{
	}

	/// <summary>
	/// Submits a single vertex to the render helper.
	///
	/// Please note: all vertices and primitives contained in the render helper
	/// *must* be in the same space (ie: object space, world space, ...), as they
	/// are rendered all at once in the Render method using the same effect
	/// property values (including world, view, and projection transforms).
	/// </summary>
	/// <param name="position"></param>
	/// <param name="color"></param>
	protected void SubmitVertex(ref Vector3 position, ref Color color)
	{
		HCB.Add(new VertexPositionColor(position, color));
	}

	/// <summary>
	/// Clears all submitted vertices from the render helper.
	/// </summary>
	public virtual void Clear()
	{
		HCB.Clear();
	}

	/// <summary>
	/// Renders all contained vertices at once using the supplied effect.
	///
	/// Please note: this method expects all effect property values
	/// including transforms to be set correctly.  If using BasicEffect remember
	/// to enable VertexColorEnabled for vertex colors to be visible.
	/// </summary>
	/// <param name="effect"></param>
	public void Render(Effect effect)
	{
		if (HCB.Count < 2)
		{
			return;
		}
		int num = 0;
		switch (PrimitiveType)
		{
		case PrimitiveType.LineList:
			num = 2;
			break;
		case PrimitiveType.TriangleList:
			num = 3;
			break;
		}
		if (num >= 1)
		{
			GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
			for (int i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
			{
				effect.CurrentTechnique.Passes[i].Apply();
				_0002u(graphicsDevice, num);
			}
		}
	}

	private void _0002u(GraphicsDevice P_0, int P_1)
	{
		int num = HC_0002.Length / P_1;
		int num2 = num * P_1;
		int num3 = 0;
		int count = HCB.Count;
		for (int i = 0; i < count; i++)
		{
			ref VertexPositionColor reference = ref HC_0002[num3];
			reference = HCB[i];
			num3++;
			if (num3 >= num2)
			{
				P_0.DrawUserPrimitives(PrimitiveType, HC_0002, 0, num3 / P_1);
				num3 = 0;
			}
		}
		if (num3 > 0)
		{
			P_0.DrawUserPrimitives(PrimitiveType, HC_0002, 0, num3 / P_1);
		}
	}
}
